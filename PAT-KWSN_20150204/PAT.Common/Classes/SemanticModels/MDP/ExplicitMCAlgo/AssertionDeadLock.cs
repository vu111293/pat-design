using System.Collections.Generic;
using System.Linq;
using System.Text;
using PAT.Common.Classes.LTS;
using PAT.Common.Classes.ModuleInterface;
using PAT.Common.Classes.Ultility;

namespace PAT.Common.Classes.SemanticModels.MDP.Assertion
{
    public abstract class AssertionDeadLock : LTS.Assertion.AssertionDeadLock
    {
        //=========================================================
        //model checking related varaibles
        //=========================================================
        private double Min = -1;
        private double Max = -1;
        protected QueryConstraintType QueryConstraintType;

        protected AssertionDeadLock(bool isNontermination) : base(isNontermination)
        {
        }

        /// <summary>
        /// Assertion Initialization to create the initial step based on the concrete types.
        /// This method shall be invoked after the parsing immediately to instanciate the initial step
        /// </summary>
        /// <param name="spec">The concrete specification of the module</param>
        public override void Initialize(SpecificationBase spec)
        {
            //initialize model checking options
            if (QueryConstraintType == QueryConstraintType.NONE)
            {
                base.Initialize(spec);
            }
            else
            {
                ModelCheckingOptions = new ModelCheckingOptions();
                List<string> DeadlockEngine = new List<string>();
                DeadlockEngine.Add(Constants.ENGINE_MDP_SEARCH);
                ModelCheckingOptions.AddAddimissibleBehavior(Constants.COMPLETE_BEHAVIOR, DeadlockEngine);
            }
        }
   
        public override string ToString()
        {
            if (QueryConstraintType == QueryConstraintType.NONE)
            {
                return base.ToString();
            }
            else
            {
                if (isNotTerminationTesting)
                {
                    return StartingProcess + " nonterminating with " + QueryConstraintType.ToString().ToLower();
                }
                else
                {
                    return StartingProcess + " deadlockfree with " + QueryConstraintType.ToString().ToLower();
                }
            }
        }

        public override void RunVerification()
        {
            if (QueryConstraintType == QueryConstraintType.NONE)
            {
                base.RunVerification();
            }
            else
            {
                MDP mdp = BuildMDP();
                
                if (!CancelRequested)
                {
                    switch (QueryConstraintType)
                    {
                        case QueryConstraintType.PROB:
                            Max = 1 - mdp.MinProbability(VerificationOutput);
                            mdp.ResetNonTargetState();
                            Min = 1 - mdp.MaxProbability(VerificationOutput);
                            break;
                        case QueryConstraintType.PMAX:
                            Max = 1 - mdp.MinProbability(VerificationOutput);
                            break;
                        case QueryConstraintType.PMIN:
                            Min = 1 - mdp.MaxProbability(VerificationOutput);
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
        }

        private MDP BuildMDP()
        {
            Stack<KeyValuePair<MDPConfiguration, MDPState>> working = new Stack<KeyValuePair<MDPConfiguration, MDPState>>(1024);

            string initID = InitialStep.GetID();
            MDPState init = new MDPState(initID);
            working.Push(new KeyValuePair<MDPConfiguration, MDPState>(InitialStep as MDPConfiguration, init));
            MDP mdp = new MDP(init, Ultility.Ultility.DEFAULT_PRECISION, Ultility.Ultility.MAX_DIFFERENCE);

            do
            {
                if (CancelRequested)
                {
                    VerificationOutput.NoOfStates = mdp.States.Count;
                    return mdp;
                }

                KeyValuePair<MDPConfiguration, MDPState> current = working.Pop();
                IEnumerable<MDPConfiguration> list = current.Key.MakeOneMoveLocal();
                VerificationOutput.Transitions += list.Count();

                //check if it is one of the target state                        
                //if (list.Length == 0)
                if(current.Key.IsDeadLock)
                {
                    if (isNotTerminationTesting || current.Key.Event != Constants.TERMINATION)
                    {
                        mdp.AddTargetStates(current.Value);
                    }
                }
                else
                {
                    int currentDistriIndex = -1;
                    Distribution newDis = new Distribution(Constants.TAU);

                    //for (int i = 0; i < list.Length; i++)
                    foreach (var step in list)
                    {
                        //MDPConfiguration step = list[i];
                        string stepID = step.GetID();
                        MDPState nextState;

                        if (!mdp.States.TryGetValue(stepID, out nextState))
                        {
                            nextState = new MDPState(stepID);
                            mdp.AddState(nextState);
                            working.Push(new KeyValuePair<MDPConfiguration, MDPState>(step, nextState));
                        }

                        if (step.DisIndex == -1)
                        {
                            if (currentDistriIndex >= 0)
                            {
                                current.Value.AddDistribution(newDis);
                                newDis = new Distribution(Constants.TAU);
                            }

                            Distribution newTrivialDis = new Distribution(step.Event);
                            newTrivialDis.AddProbStatePair(1, nextState);
                            current.Value.AddDistribution(newTrivialDis);
                        }
                        else if (currentDistriIndex != -1 && step.DisIndex != currentDistriIndex)
                        {
                            current.Value.AddDistribution(newDis);
                            newDis = new Distribution(Constants.TAU);
                            newDis.AddProbStatePair(step.Probability, nextState);
                        }
                        else
                        {
                            newDis.AddProbStatePair(step.Probability, nextState);
                        }

                        currentDistriIndex = step.DisIndex;
                    }

                    if (currentDistriIndex >= 0)
                    {
                        current.Value.AddDistribution(newDis);
                    }
                }
            } while (working.Count > 0);

            VerificationOutput.NoOfStates = mdp.States.Count;
            //mdp.BackUpTargetStates();
            return mdp;
        }

        //private MDP GetTransitionRelation()
        //{
        //    Dictionary<string, int> Visited = new Dictionary<string, int>(Common.Classes.Ultility.Ultility.MC_INITIAL_SIZE);
        //    Stack<MDPConfiguration> working = new Stack<MDPConfiguration>(1024);

        //    Visited.Add(InitialStep.GetID(), Visited.Count);
        //    working.Push(InitialStep as MDPConfiguration);

        //    MDP mdp = new MDP(0, Ultility.Ultility.DEFAULT_PRECISION, Ultility.Ultility.MAX_DIFFERENCE);

        //    do
        //    {
        //        if (CancelRequested)
        //        {
        //            this.VerificationOutput.NoOfStates = Visited.Count;
        //            return mdp;
        //        }

        //        MDPConfiguration current = working.Pop();
        //        int currentIndex = Visited[current.GetID()];
        //        List<MDPConfiguration> list = current.MakeOneMoveLocal();
        //        this.VerificationOutput.Transitions += list.Count;
        //        int currentDistriIndex = -1;

        //        //check if it is one of the target state                        
        //        if (list.Count == 0)
        //        {
        //            if (this.isNotTerminationTesting || current.Event != Constants.TERMINATION)
        //            {
        //                mdp.TargetStates.Add(currentIndex);

        //            }
        //        }

        //        for (int i = 0; i < list.Count; i++)
        //        {
        //            MDPConfiguration step = list[i];
        //            string stepID = step.GetID();
        //            int nextIndex = Visited.Count;

        //            if (Visited.ContainsKey(stepID))
        //            {
        //                nextIndex = Visited[stepID];
        //            }

        //            //add a seperator into the transition relation table to seperate different distributions
        //            if (mdp.ContainsState(currentIndex) && (step.DisIndex == -1 || step.DisIndex != currentDistriIndex))
        //            {
        //                mdp.AddSeperator(currentIndex);
        //            }

        //            currentDistriIndex = step.DisIndex;

        //            mdp.AddTransition(currentIndex, nextIndex, step.Probability);

        //            if (!Visited.ContainsKey(stepID))
        //            {
        //                Visited.Add(stepID, nextIndex);
        //                working.Push(step);
        //            }
        //        }
        //    } while (working.Count > 0);

        //    this.VerificationOutput.NoOfStates = Visited.Count;
        //    return mdp;
        //}

        public override string GetResultString()
        {
            if (QueryConstraintType == QueryConstraintType.NONE)
            {
                return base.GetResultString();
            }

            StringBuilder sb = new StringBuilder();

            sb.AppendLine(Constants.VERFICATION_RESULT_STRING);
            if (this.VerificationOutput.VerificationResult == VerificationResultType.VALID)
            {
                sb.AppendLine("The Assertion (" + ToString() + ") is VALID.");
            }
            else if (this.VerificationOutput.VerificationResult == VerificationResultType.UNKNOWN)
            {
                sb.AppendLine("The Assertion (" + ToString() + ") is NEITHER PROVED NOR DISPROVED.");
            }
            else if (this.VerificationOutput.VerificationResult == VerificationResultType.WITHPROBABILITY)
            {
                //sb.AppendLine("\t***The Assertion (" + ToString() + ") is Valid With Probability " + Ultility.Ultility.GetProbIntervalString(Min, Max, Precision) + ".***");
                if (this.Max != -1 && Min != -1)
                {
                    sb.AppendLine("The Assertion (" + ToString() + ") is Valid with Probability " + Ultility.Ultility.GetProbIntervalString(Min, Max) + ";");
                }
                else if (this.Max != -1)
                {
                    sb.AppendLine("The Assertion (" + ToString() + ") is Valid with Max Probability " + Ultility.Ultility.GetProbIntervalString(Max) + ";");
                }
                else if (this.Min != -1)
                {
                    sb.AppendLine("The Assertion (" + ToString() + ") is Valid with Min Probability " + Ultility.Ultility.GetProbIntervalString(Min) + ";");
                }
            }
            else
            {
                sb.AppendLine("The Assertion (" + ToString() + ") is NOT Valid.");
            }

            sb.AppendLine();
            sb.AppendLine("********Verification Setting********");
            sb.AppendLine("Admissible Behavior: " + SelectedBahaviorName);
            sb.AppendLine("Search Engine: " + SelectedEngineName);
            sb.AppendLine("System Abstraction: " + MustAbstract);       
            sb.AppendLine("Maximum difference threshold : " + Ultility.Ultility.MAX_DIFFERENCE);


            sb.AppendLine();

            return sb.ToString();
        }

    }
}