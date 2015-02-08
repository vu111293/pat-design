using System.Collections.Generic;
using System.Linq;
using System.Text;
using PAT.Common.Classes.BA;
using PAT.Common.Classes.DataStructure;
using PAT.Common.Classes.LTS;
using PAT.Common.Classes.ModuleInterface;
using PAT.Common.Classes.Ultility;

namespace PAT.Common.Classes.SemanticModels.LTS.Assertion
{
    public partial class AssertionLTL 
    {
        /// <summary>
        /// Run the verification and get the result.
        /// </summary>
        /// <returns></returns>
        public virtual void RunVerificationSafety()
        {
            if (SelectedEngineName == Constants.ENGINE_DEPTH_FIRST_SEARCH)
            {
                DFSVerification();
            }
            else
            {
                BFSVerification();
            }           
        }

        public void DFSVerification()
        {
            //The following are for identifying a counterexample trace. 
            Stack<int> depthStack = new Stack<int>(1024);
            depthStack.Push(0);
            List<int> depthList = new List<int>(1024);
            //The above are for identifying a counterexample trace. 

            Stack<EventBAPairSafety> TaskStack = new Stack<EventBAPairSafety>();

            EventBAPairSafety initialstep = EventBAPairSafety.GetInitialPairs(BA, InitialStep);
            TaskStack.Push(initialstep);

            //Dictionary<string, bool> Visited = new Dictionary<string, bool>();
            StringHashTable Visited = new StringHashTable(Ultility.Ultility.MC_INITIAL_SIZE);


            while (TaskStack.Count != 0)
            {
                if (CancelRequested)
                {
                    this.VerificationOutput.NoOfStates = Visited.Count;
                    return;
                }

                EventBAPairSafety now = TaskStack.Pop();
                string ID = now.GetCompressedState();


                //The following are for identifying a counterexample trace. 
                int depth = depthStack.Pop();

                if (depth > 0)
                {
                    while (depthList[depthList.Count - 1] >= depth)
                    {
                        int lastIndex = depthList.Count - 1;
                        depthList.RemoveAt(lastIndex);
                        this.VerificationOutput.CounterExampleTrace.RemoveAt(lastIndex);
                    }
                }

                this.VerificationOutput.CounterExampleTrace.Add(now.configuration);
                depthList.Add(depth);
                //The above are for identifying a counterexample trace. 

                if (now.States.Count == 0)
                {
                    this.VerificationOutput.NoOfStates = Visited.Count;
                    this.VerificationOutput.VerificationResult = VerificationResultType.INVALID;
                    return;
                }

                if (!Visited.ContainsKey(ID))
                {
                    Visited.Add(ID);

                    ConfigurationBase[] steps = now.configuration.MakeOneMove().ToArray();
                    this.VerificationOutput.Transitions += steps.Length;
                    EventBAPairSafety[] products = now.Next(BA,steps);
                    foreach (EventBAPairSafety step in products)
                    {
                        TaskStack.Push(step);
                        depthStack.Push(depth + 1);
                    }
                }
            }

            this.VerificationOutput.CounterExampleTrace = null;
            this.VerificationOutput.NoOfStates = Visited.Count;
            this.VerificationOutput.VerificationResult = VerificationResultType.VALID;
        }

        public void BFSVerification()
        {
            //Dictionary<string, bool> Visited = new Dictionary<string, bool>();
            StringHashTable Visited = new StringHashTable(Ultility.Ultility.MC_INITIAL_SIZE);
            Queue<EventBAPairSafety> working = new Queue<EventBAPairSafety>();
            Queue<List<ConfigurationBase>> paths = new Queue<List<ConfigurationBase>>(1024);

            EventBAPairSafety initialstep = EventBAPairSafety.GetInitialPairs(BA,InitialStep);
            working.Enqueue(initialstep);

            List<ConfigurationBase> path = new List<ConfigurationBase>();
            path.Add(InitialStep);
            paths.Enqueue(path);
            Visited.Add(initialstep.GetCompressedState());
            
            do
            {
                if (CancelRequested)
                {
                    VerificationOutput.NoOfStates = Visited.Count;
                    return;
                }

                EventBAPairSafety current = working.Dequeue();
                List<ConfigurationBase> currentPath = paths.Dequeue();
            
                if (current.States.Count == 0)
                {
                    VerificationOutput.VerificationResult = VerificationResultType.INVALID;
                    VerificationOutput.NoOfStates = Visited.Count;
                    VerificationOutput.CounterExampleTrace = currentPath;
                    return;
                }

                ConfigurationBase[] steps = current.configuration.MakeOneMove().ToArray();
                VerificationOutput.Transitions += steps.Length;
                EventBAPairSafety[] products = current.Next(BA, steps);
                foreach (EventBAPairSafety step in products)
                {
                    string stepID = step.GetCompressedState();
                    if (!Visited.ContainsKey(stepID))
                    {
                        Visited.Add(stepID);
                        working.Enqueue(step);

                        List<ConfigurationBase> newPath = new List<ConfigurationBase>(currentPath);
                        newPath.Add(step.configuration);
                        paths.Enqueue(newPath);
                    }
                }
               
            } while (working.Count > 0);

            VerificationOutput.NoOfStates = Visited.Count;
            VerificationOutput.VerificationResult = VerificationResultType.VALID;
        }

        public virtual string GetResultStringSafety()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine(Constants.VERFICATION_RESULT_STRING);

            if (this.VerificationOutput.VerificationResult == VerificationResultType.VALID)
            {
                sb.AppendLine("The Assertion (" + ToString() + ") is VALID.");
            }
            else
            {
                if (this.VerificationOutput.VerificationResult == VerificationResultType.UNKNOWN)
                {
                    sb.AppendLine("The Assertion (" + ToString() + ") is NEITHER PROVED NOR DISPROVED.");
                }
                else
                {
                    sb.AppendLine("The Assertion (" + ToString() + ") is NOT valid.");
                    sb.AppendLine("A counterexample is presented as follows.");
                    //GetCounterxampleString(sb);
                    VerificationOutput.GetCounterxampleString(sb);
                }
            }

            sb.AppendLine();
            sb.AppendLine("********Verification Setting********");
            sb.AppendLine("Admissible Behavior: " + SelectedBahaviorName);
            if (SelectedEngineName == Constants.ENGINE_DEPTH_FIRST_SEARCH)
            {
                sb.AppendLine("Method: Refinement Based Safety Analysis using DFS - The LTL formula is a safety property!"); 
            }
            else
            {
                sb.AppendLine("Method: Refinement Based Safety Analysis using BFS - The LTL formula is a safety property!");                    
            }            
            sb.AppendLine("System Abstraction: " + MustAbstract);
            sb.AppendLine();

            return sb.ToString();
        }
    }
}