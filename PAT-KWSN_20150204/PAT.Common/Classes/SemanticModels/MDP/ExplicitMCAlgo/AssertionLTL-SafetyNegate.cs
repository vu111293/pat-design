using System.Collections.Generic;
using System.Linq;
using PAT.Common.Classes.Assertion;
using PAT.Common.Classes.BA;
using PAT.Common.Classes.ModuleInterface;
using PAT.Common.Classes.Ultility;

namespace PAT.Common.Classes.SemanticModels.MDP.Assertion
{
    public partial class AssertionLTL 
    {
        /// <summary>
        /// Run the verification and get the result.
        /// </summary>
        /// <returns></returns>
        public override void RunVerificationNegate()
        {
            if (ConstraintType == QueryConstraintType.NONE)
            {
                System.Diagnostics.Debug.Assert(false);
                base.RunVerificationNegate();
                return;
            }

            BuildMDPSafety();
            if (!CancelRequested)
            {
                switch (ConstraintType)
                {
                    case QueryConstraintType.PROB:
                        Max = mdp.MaxProbability(VerificationOutput);
                        mdp.ResetNonTargetState();
                        Min = mdp.MinProbability(VerificationOutput);
                        break;
                    case QueryConstraintType.PMAX:
                        Max = mdp.MaxProbability(VerificationOutput);
                        break;
                    case QueryConstraintType.PMIN:
                        Min = mdp.MinProbability(VerificationOutput);
                        break;
                }

                if (Min == 1)
                {
                    VerificationOutput.VerificationResult = VerificationResultType.VALID;
                }
                else if (Max == 0)
                {
                    VerificationOutput.VerificationResult = VerificationResultType.INVALID;
                }
                else
                {
                    VerificationOutput.VerificationResult = VerificationResultType.WITHPROBABILITY;
                }

            }
        }

        private void BuildMDPSafety()
        {
            Stack<KeyValuePair<EventBAPairSafetyPCSP, MDPState>> working = new Stack<KeyValuePair<EventBAPairSafetyPCSP, MDPState>>(1024);
            EventBAPairSafetyPCSP initialstep = EventBAPairSafetyPCSP.GetInitialPairs(BA, InitialStep as MDPConfiguration);
            string initID = initialstep.GetID();
            MDPState init = new MDPState(initID);

            working.Push(new KeyValuePair<EventBAPairSafetyPCSP, MDPState>(initialstep, init));
            mdp = new MDP(init, Ultility.Ultility.DEFAULT_PRECISION, Ultility.Ultility.MAX_DIFFERENCE);

            do
            {
                if (CancelRequested)
                {
                    VerificationOutput.NoOfStates = mdp.States.Count;
                }

                KeyValuePair<EventBAPairSafetyPCSP, MDPState> current = working.Pop();
                if (current.Key.BAStates.Count == 0)
                {
                    mdp.AddTargetStates(current.Value);
                }
                else
                {
                    MDPConfiguration[] steps = current.Key.Config.MakeOneMoveLocal().ToArray();
                    this.VerificationOutput.Transitions += steps.Length;

                    List<EventBAPairSafetyPCSP> products = EventBAPairSafetyPCSP.Next(BA, steps, current.Key.BAStates);

                    int currentDistriIndex = -1;
                    Distribution newDis = new Distribution(Constants.TAU);

                    foreach (EventBAPairSafetyPCSP eventBaPairSafetyPcsp in products)
                    {
                        string stepID = eventBaPairSafetyPcsp.GetID();
                        MDPState nextState;

                        if (!mdp.States.TryGetValue(stepID, out nextState))
                        {
                            nextState = new MDPState(stepID);
                            mdp.AddState(nextState);
                            working.Push(new KeyValuePair<EventBAPairSafetyPCSP, MDPState>(eventBaPairSafetyPcsp, nextState));
                        }

                        if (eventBaPairSafetyPcsp.Config.DisIndex == -1)
                        {
                            if (currentDistriIndex >= 0)
                            {
                                current.Value.AddDistribution(newDis);
                                newDis = new Distribution(Constants.TAU);
                            }

                            Distribution newTrivialDis = new Distribution(eventBaPairSafetyPcsp.Config.Event);
                            newTrivialDis.AddProbStatePair(1, nextState);
                            current.Value.AddDistribution(newTrivialDis);
                        }
                        else if (currentDistriIndex != -1 && eventBaPairSafetyPcsp.Config.DisIndex != currentDistriIndex)
                        {
                            current.Value.AddDistribution(newDis);
                            newDis = new Distribution(Constants.TAU);
                            newDis.AddProbStatePair(eventBaPairSafetyPcsp.Config.Probability, nextState);
                        }
                        else
                        {
                            newDis.AddProbStatePair(eventBaPairSafetyPcsp.Config.Probability, nextState);
                        }

                        currentDistriIndex = eventBaPairSafetyPcsp.Config.DisIndex;
                    }

                    if (currentDistriIndex >= 0)
                    {
                        current.Value.AddDistribution(newDis);
                    }
                }
            } while (working.Count > 0);

            VerificationOutput.NoOfStates = mdp.States.Count;
            //mdp.BackUpTargetStates();
        }

        //private MDP GetTransitionRelation()///max means we want to calculate the maximal reachability.
        //{
        //    return null;
        //    //Stack<EventBAPairSafetyPCSP> working = new Stack<EventBAPairSafetyPCSP>();

        //    //EventBAPairSafetyPCSP initialstep = EventBAPairSafetyPCSP.GetInitialPairs(BA, InitialStep as MDPConfiguration);
        //    //working.Push(initialstep);

        //    ////List<List<string>> visited = new List<List<string>>(); Dictionary has constant accessing time.
        //    //Dictionary<string, int> Visited = new Dictionary<string, int>(Common.Classes.Ultility.Ultility.MC_INITIAL_SIZE);

        //    //MDP mdp = new MDP(0, Ultility.Ultility.DEFAULT_PRECISION, Ultility.Ultility.MAX_DIFFERENCE);
        //    //Visited.Add(initialstep.GetCompressedState(), 0);

        //    //do//note: this loop takes a long time
        //    //{
        //    //    if (CancelRequested)
        //    //    {
        //    //        this.VerificationOutput.NoOfStates = Visited.Count;
        //    //        return mdp;
        //    //    }

        //    //    EventBAPairSafetyPCSP current = working.Pop();
        //    //    int currentIndex = Visited[current.GetCompressedState()];

        //    //    if (current.state.Count == 0)
        //    //    {
        //    //        mdp.TargetStates.Add(currentIndex);
        //    //    }
        //    //    else
        //    //    {
        //    //        List<MDPConfiguration> steps = current.configuration.MakeOneMoveLocal();
        //    //        this.VerificationOutput.Transitions += steps.Count;
        //    //        List<EventBAPairSafetyPCSP> products = EventBAPairSafetyPCSP.Next(BA, steps, current.state);

        //    //        int currentDistriIndex = -1;

        //    //        foreach (EventBAPairSafetyPCSP step in products)
        //    //        {
        //    //            string stepID = step.GetCompressedState();
        //    //            int nextIndex = Visited.Count;
        //    //            MDPConfiguration stepConfig = step.configuration;

        //    //            if (Visited.ContainsKey(stepID))
        //    //            {
        //    //                nextIndex = Visited[stepID];
        //    //            }

        //    //            //add a seperator into the transition relation table to seperate different distributions
        //    //            if (mdp.ContainsState(currentIndex) && (stepConfig.DisIndex == -1 || stepConfig.DisIndex != currentDistriIndex))
        //    //            {
        //    //                mdp.AddSeperator(currentIndex);
        //    //            }

        //    //            currentDistriIndex = stepConfig.DisIndex;

        //    //            mdp.AddTransition(currentIndex, nextIndex, stepConfig.Probability);

        //    //            if (!Visited.ContainsKey(stepID))
        //    //            {
        //    //                Visited.Add(stepID, nextIndex);
        //    //                working.Push(step);
        //    //            }
        //    //        }
        //    //    }

        //    //} while (working.Count > 0);

        //    //this.VerificationOutput.NoOfStates = Visited.Count;
        //    //return mdp;
        //}
    }

    public sealed class EventBAPairSafetyPCSP
    {
        public MDPConfiguration Config;
        public List<string> BAStates;

        public EventBAPairSafetyPCSP(MDPConfiguration e, List<string> s)
        {
            Config = e;
            BAStates = s;
        }

        public string GetID()
        {
            return Config.GetIDWithEvent() + Constants.SEPARATOR + Common.Classes.Ultility.Ultility.PPStringList(BAStates);
        }

        public static EventBAPairSafetyPCSP GetInitialPairs(BuchiAutomata BA, MDPConfiguration initialStep)
        {
            List<string> intialBAStates = new List<string>();
            //HashSet<string> existed = new HashSet<string>();

            foreach (string s in BA.InitialStates)
            {
                List<string> next = BA.MakeOneMove(s, initialStep);

                foreach (string var in next)
                {
                    //if (!existed.Contains(var))
                    //{
                    //    existed.Add(var);
                    //    intialBAStates.Add(var);
                    //}
                    if (!intialBAStates.Contains(var))
                    {
                        intialBAStates.Add(var);
                    }
                }
            }

            return new EventBAPairSafetyPCSP(initialStep, intialBAStates);
        }

        public static List<EventBAPairSafetyPCSP> Next(BuchiAutomata BA, MDPConfiguration[] steps, List<string> BAStates)
        {
            List<EventBAPairSafetyPCSP> product = new List<EventBAPairSafetyPCSP>(steps.Length * BA.States.Length);

            for (int i = 0; i < steps.Length; i++)
            {
                List<string> targetStates = new List<string>();

                foreach (string state in BAStates)
                {
                    List<string> states = BA.MakeOneMove(state, steps[i]);
                    Common.Classes.Ultility.Ultility.Union(targetStates, states);
                }

                product.Add(new EventBAPairSafetyPCSP(steps[i], targetStates));
            }

            return product;
        }
    }
}