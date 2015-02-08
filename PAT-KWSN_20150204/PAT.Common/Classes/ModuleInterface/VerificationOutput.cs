using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using PAT.Common.Classes.Ultility;

namespace PAT.Common.Classes.ModuleInterface
{
    public class VerificationOutput
    {
        /// <summary>
        /// General Verification Output Data
        /// </summary>
        
        public VerificationResultType VerificationResult;
        public List<ConfigurationBase> CounterExampleTrace;

        public string ResultString;

        private Stopwatch Timer;
        private long StartMemroySize;
        public double VerificationTime;
        public float EstimateMemoryUsage;
        public int ActualMemoryUsage;
        public bool GenerateCounterExample = true;
        public bool NonDeterminismInDTMC = false;

        public VerificationOutputType VerificationOutputType;

        //if >= 0, got a loop
        public int LoopIndex = -1; //if >= 0, got a loop
        

        //statistics
        /// <summary>
        /// The number of states explored during the verification
        /// </summary>
        public long NoOfStates;
        /// <summary>
        /// The number of transitions explored during the verification
        /// </summary>
        public long Transitions;


        public int SCCCount;
        public long SCCTotalSize;

        //reduced MDP transition
        public long ReducedMDPTransitions = 0;
        public long ReducedMDPStates = 0;
        public long MDPIterationNumber = 0;

        public double Rewards;

        //bdd related data
        public int numberOfBoolVars = 0;
        public int numberOfBDDOperation = 0;


        public VerificationOutput(string engine)
        {
            VerificationResult = VerificationResultType.UNKNOWN;
            CounterExampleTrace = new List<ConfigurationBase>(64);

            Transitions = 0;
            NoOfStates = 0;
            LoopIndex = -1;

            ReducedMDPTransitions = 0;
            ReducedMDPStates = 0;

            Timer = new Stopwatch();

            switch (engine)
            {
                case Constants.ENGINE_BDD_SEARCH:
                case Constants.ENGINE_BACKWARD_SEARCH_BDD:                
                case Constants.ENGINE_FORWARD_BACKWARD_SEARCH_BDD:
                case Constants.ENGINE_FORWARD_SEARCH_BDD:
                    VerificationOutputType = VerificationOutputType.LTS_BDD;        
                    break;

                case Constants.ENGINE_MDP_SEARCH:
                case Constants.ENGINE_SMT_MDP:
                case Constants.ENGINE_SMT_DTMC:
                    VerificationOutputType = VerificationOutputType.MDP_EXPLICIT;                    
                    break;
             
                case Constants.ENGINE_SCC_BASED_SEARCH_MULTICORE:
                    VerificationOutputType = VerificationOutputType.LTS_EXPLICIT_MULTI_CORE;
                    break;

                default:
                    VerificationOutputType = VerificationOutputType.LTS_EXPLICIT;
                    break;
            }
        }

        public void GetCounterxampleString(StringBuilder sb)
        {
            if (GenerateCounterExample)
            {
                sb.Append("<");

                bool hasVisibleEvent = false;
                for (int i = 0; i < CounterExampleTrace.Count; i++)
                {
                    ConfigurationBase step = CounterExampleTrace[i];
                    if (step.Event == Constants.INITIAL_EVENT)
                    {
                        sb.Append(step.Event);
                    }
                    else
                    {
                        sb.Append(" -> ");

                        if (LoopIndex >= 0 && i == LoopIndex)
                        {
                            sb.Append("(");
                        }

                        sb.Append(step.GetDisplayEvent());

                        if (step.Event != Constants.TAU && i >= LoopIndex && LoopIndex >= 0)
                        {
                            hasVisibleEvent = true;
                        }
                    }
                }

                if (LoopIndex >= 0 && !hasVisibleEvent)
                {
                    sb.Append(" -> (" + Constants.TAU + " -> ");
                }

                if (LoopIndex >= 0)
                {
                    sb.Append(")*");
                }

                sb.AppendLine(">");
            }
            else
            {
                sb.AppendLine("Counterexample generation is ignored.");
            }
        }

        public void StartVerification()
        {
            GC.Collect();
            Timer.Reset();
            StartMemroySize = GC.GetTotalMemory(true);
            Timer.Start();
        }

        public virtual string GetVerificationStatistics()
        {
            Timer.Stop();
            long stopMemorySize = System.GC.GetTotalMemory(false);

            VerificationTime = Timer.Elapsed.TotalSeconds;
            EstimateMemoryUsage = stopMemorySize - StartMemroySize;

            StringBuilder sb = new StringBuilder();
            //sb.AppendLine("********Verification Statistics********");
            
            switch (VerificationOutputType)
            {
                case VerificationOutputType.LTS_EXPLICIT:
                case VerificationOutputType.TTS_EXPLICIT:
                case VerificationOutputType.MDP_EXPLICIT:
                    sb.AppendLine("Visited States:" + (NoOfStates >= 0 ? NoOfStates.ToString() : "Unknown"));
                    sb.AppendLine("Total Transitions:" + (Transitions >= 0 ? Transitions.ToString() : "Unknown"));

                    if (ReducedMDPStates > 0)
                    {
                        sb.AppendLine("Visited States (Reduced MDP):" + ReducedMDPStates);
                        sb.AppendLine("Total Transitions (Reduced MDP):" + ReducedMDPTransitions);
                    }

                    if (MDPIterationNumber > 0)
                    {
                        sb.AppendLine("MDP Iterations:" + MDPIterationNumber);
                    }

                    break;
                case VerificationOutputType.TTS_BDD:
                case VerificationOutputType.MDP_BDD:
                case VerificationOutputType.LTS_BDD:

                    sb.AppendLine("Number of Boolean Variables Used: " + numberOfBoolVars);
                    sb.AppendLine("Number of BDD Operations Performed: " + numberOfBDDOperation);

                    break;
                case VerificationOutputType.LTS_EXPLICIT_MULTI_CORE:

                    sb.AppendLine("Visited States:" + (NoOfStates >= 0 ? NoOfStates.ToString() : "Unknown"));
                    sb.AppendLine("Total Transitions:" + (Transitions >= 0 ? Transitions.ToString() : "Unknown"));



                    sb.AppendLine("Number of SCC found: " + SCCCount);
                    sb.AppendLine("Total SCC states: " + SCCTotalSize);
                    if (SCCCount != 0)
                    {
                        sb.AppendLine("Average SCC Size: " + (SCCTotalSize/SCCCount));
                    }
                    else
                    {
                        sb.AppendLine("Average SCC Size: 0");
                    }

                    sb.AppendLine("SCC Ratio: " + Math.Round(((double) SCCTotalSize/(double) NoOfStates), 2).ToString());


                    break;

            }

            sb.AppendLine("Time Used:" + VerificationTime + "s");

            if (ActualMemoryUsage != 0)
            {
                sb.AppendLine("Memory Used:" + (ActualMemoryUsage) / 1000.0 + "KB\r\n");
            }
            else
            {
                sb.AppendLine("Estimated Memory Used:" + (EstimateMemoryUsage) / 1000.0 + "KB\r\n");
            }
           
            return sb.ToString();
        }
    }

    public enum VerificationResultType : byte
    {
        VALID,
        INVALID,
        UNKNOWN,
        WITHPROBABILITY,
        WITHREWARDS
    }

    public enum VerificationOutputType : byte
    {
        LTS_EXPLICIT,
        LTS_EXPLICIT_MULTI_CORE,
        LTS_BDD,
        TTS_EXPLICIT,
        TTS_BDD,
        MDP_EXPLICIT,
        MDP_BDD,
    }

    public enum RefinementCheckingResultType : byte
    {
        Valid,
        TraceRefinementFailure,
        FailuresRefinementFailure,
        DivCheckingFailure
    }
}