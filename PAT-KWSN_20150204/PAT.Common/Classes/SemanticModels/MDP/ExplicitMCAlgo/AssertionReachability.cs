using System.Collections.Generic;
using PAT.Common.Classes.Expressions;
using PAT.Common.Classes.Expressions.ExpressionClass;
using System.Text;
using System.Linq;
using PAT.Common.Classes.LTS;
using PAT.Common.Classes.ModuleInterface;
using PAT.Common.Classes.Ultility;
using QueryConstraintType=PAT.Common.Classes.Ultility.QueryConstraintType;

namespace PAT.Common.Classes.SemanticModels.MDP.Assertion
{
    public abstract class AssertionReachability : LTS.Assertion.AssertionReachability
    {
        protected double Min = -1;
        protected double Max = -1;
        
        protected QueryConstraintType ConstraintType;


        protected AssertionReachability(string reachableState) : base(reachableState)
        {
        }

        public override void Initialize(SpecificationBase spec)
        {
            if (ConstraintType == QueryConstraintType.NONE)
            {
                base.Initialize(spec);
            }
            else
            {
                //initialize model checking options
                ModelCheckingOptions = new ModelCheckingOptions();
                List<string> LTLEngine = new List<string>();
                LTLEngine.Add(Constants.ENGINE_MDP_SEARCH);
                ModelCheckingOptions.AddAddimissibleBehavior(Constants.COMPLETE_BEHAVIOR, LTLEngine);
            }
        }


        public override string ToString()
        {
            if (ConstraintType == QueryConstraintType.NONE)
            {
                return base.ToString();
            }
            return StartingProcess.ToString() + " reaches " + ReachableStateLabel + " with " + ConstraintType.ToString().ToLower();
        }

        /// <summary>
        /// Run the verification and get the result.
        /// </summary>
        /// <returns></returns>
        public override void RunVerification()
        {
            if (ConstraintType == QueryConstraintType.NONE)
            {
                base.RunVerification();
                return;
            }

            MDP mdp = BuildMDP(); //GetTransitionRelation();
            //MDP mdp = BuildDTMCSSZ(); //GetTransitionRelation();
            if (!CancelRequested)
            {
                switch (ConstraintType)
                {
                    case QueryConstraintType.PROB:
                        Min = mdp.MinProbability(VerificationOutput);
                        mdp.ResetNonTargetState();
                        Max = mdp.MaxProbability(VerificationOutput);                        
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
                    this.VerificationOutput.VerificationResult = VerificationResultType.VALID;
                }
                else if (Max == 0)
                {
                    this.VerificationOutput.VerificationResult = VerificationResultType.INVALID;
                }
                else
                {
                    this.VerificationOutput.VerificationResult = VerificationResultType.WITHPROBABILITY;
                }
            }
        }

        protected MDP BuildMDP()
        {
            Stack<KeyValuePair<MDPConfiguration, MDPState>> working = new Stack<KeyValuePair<MDPConfiguration, MDPState>>(1024);

            string initID = InitialStep.GetID();
            MDPState init = new MDPState(initID);
            working.Push(new KeyValuePair<MDPConfiguration, MDPState>(InitialStep as MDPConfiguration, init));
            MDP mdp = new MDP(init, Ultility.Ultility.DEFAULT_PRECISION, Ultility.Ultility.MAX_DIFFERENCE);

            //if initial state is target
            ExpressionValue initV = EvaluatorDenotational.Evaluate(ReachableStateCondition, InitialStep.GlobalEnv);
            if ((initV as BoolConstant).Value)
            {
                mdp.AddTargetStates(init);
                VerificationOutput.NoOfStates = 1;
                return mdp;
            }
            //if initial state is target

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

                int currentDistriIndex = -1;
                Distribution newDis = new Distribution(Constants.TAU);

                //for (int i = 0; i < list.Count; i++)
                foreach (var step in list)
                {
                    //MDPConfiguration step = list[i];
                    string stepID = step.GetID();
                    MDPState nextState;

                    if (!mdp.States.TryGetValue(stepID, out nextState))
                    {
                        nextState = new MDPState(stepID);
                        mdp.AddState(nextState);

                        ExpressionValue v = EvaluatorDenotational.Evaluate(ReachableStateCondition, step.GlobalEnv);

                        if ((v as BoolConstant).Value)
                        {
                            mdp.AddTargetStates(nextState);
                        }
                        else
                        {
                            working.Push(new KeyValuePair<MDPConfiguration, MDPState>(step, nextState));
                        }
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
            } while (working.Count > 0);

            VerificationOutput.NoOfStates = mdp.States.Count;
            //mdp.BackUpTargetStates();
            return mdp;
        }

        //protected MDP BuildDTMCSSZ()
        //{
        //    Stack<KeyValuePair<MDPConfiguration, MDPState>> working = new Stack<KeyValuePair<MDPConfiguration, MDPState>>(1024);

        //    string initID = InitialStep.GetID();
        //    MDPState init = new MDPState(initID);
        //    //working.Push(new KeyValuePair<MDPConfiguration, MDPState>(InitialStep as MDPConfiguration, init));
        //    KeyValuePair<MDPConfiguration, MDPState> Init = new KeyValuePair<MDPConfiguration, MDPState>(InitialStep as MDPConfiguration, init);
        //    MDP mdp = new MDP(init, Ultility.Ultility.DEFAULT_PRECISION, Ultility.Ultility.MAX_DIFFERENCE);

        //    List<KeyValuePair<MDPState, double>> currentTrace = new List<KeyValuePair<MDPState, double>>();

        //    DTMCDFS(mdp, currentTrace, Init);

        //    return mdp;
        //    //do
        //    //{
        //    //    if (CancelRequested)
        //    //    {
        //    //        VerificationOutput.NoOfStates = mdp.States.Count;
        //    //        return mdp;
        //    //    }

        //    //    KeyValuePair<MDPConfiguration, MDPState> current = working.Pop();
        //    //    MDPConfiguration[] list = current.Key.MakeOneMoveLocal();
        //    //    VerificationOutput.Transitions += list.Length;

        //    //    int currentDistriIndex = -1;
        //    //    Distribution newDis = new Distribution(Constants.TAU);

        //    //    //for (int i = 0; i < list.Length; i++)
        //    //    //{
        //    //        MDPConfiguration step = list[0];
        //    //        string stepID = step.GetID();
        //    //        MDPState nextState;

        //    //        if (!mdp.States.TryGetValue(stepID, out nextState))
        //    //        {
        //    //            nextState = new MDPState(stepID);
        //    //            mdp.AddState(nextState);
                        

        //    //            ExpressionValue v = EvaluatorDenotational.Evaluate(ReachableStateCondition, step.GlobalEnv);

        //    //            if ((v as BoolConstant).Value)
        //    //            {
        //    //                mdp.AddTargetStates(nextState);
        //    //            }
        //    //            else
        //    //            {
        //    //                working.Push(new KeyValuePair<MDPConfiguration, MDPState>(step, nextState));
        //    //            }
        //    //        }

        //    //        if (step.DisIndex == -1)
        //    //        {
        //    //            if (currentDistriIndex >= 0)
        //    //            {
        //    //                current.Value.AddDistribution(newDis);
        //    //                newDis = new Distribution(Constants.TAU);
        //    //            }

        //    //            Distribution newTrivialDis = new Distribution(step.Event);
        //    //            newTrivialDis.AddProbStatePair(1, nextState);
        //    //            current.Value.AddDistribution(newTrivialDis);
        //    //        }
        //    //        else if (currentDistriIndex != -1 && step.DisIndex != currentDistriIndex)
        //    //        {
        //    //            current.Value.AddDistribution(newDis);
        //    //            newDis = new Distribution(Constants.TAU);
        //    //            newDis.AddProbStatePair(step.Probability, nextState);
        //    //        }
        //    //        else
        //    //        {
        //    //            newDis.AddProbStatePair(step.Probability, nextState);
        //    //        }

        //    //        currentDistriIndex = step.DisIndex;
        //    //    //}

        //    //    if (currentDistriIndex >= 0)
        //    //    {
        //    //        current.Value.AddDistribution(newDis);
        //    //    }
        //    //} while (working.Count > 0);

        //    //VerificationOutput.NoOfStates = mdp.States.Count;
        //    //return mdp;
        //}

        //private void DTMCDFS(MDP mdp, List<KeyValuePair<MDPState, double>> currentTrace, KeyValuePair<MDPConfiguration, MDPState> current)
        //{
            
        //    MDPConfiguration[] list = current.Key.MakeOneMoveLocal();
            
        //    for(int i = 0; i < list.Length; i++)
        //    {
        //        MDPConfiguration step = list[i];
        //        string stepID = step.GetID();
        //        MDPState nextState;

        //        currentTrace.Add(new KeyValuePair<MDPState, double>(current.Value, step.Probability));

        //        if (!mdp.States.TryGetValue(stepID, out nextState))
        //        {
        //            nextState = new MDPState(stepID);
        //            mdp.AddState(nextState);


        //            ExpressionValue v = EvaluatorDenotational.Evaluate(ReachableStateCondition, step.GlobalEnv);

        //            if ((v as BoolConstant).Value)
        //            {
        //                mdp.AddTargetStates(nextState);
        //            }
        //            else
        //            {
        //                DTMCDFS(mdp, currentTrace, new KeyValuePair<MDPConfiguration, MDPState>(step, nextState));
        //            }
        //        }
        //        else
        //        {
        //            //double loopProb = 1;

        //            for (int j = currentTrace.Count - 1; j >=0; j--)
        //            {
        //                list.
        //                    //loopProb *= currentTrace[j].Value;
        //                    if (currentTrace[j].Key == nextState) break;
        //            }
        //        }
                
        //    }

        //}

        //protected virtual MDP GetTransitionRelation()///max means we want to calculate the maximal reachability.
        //{
        //    //List<int> Targets = new List<int>();

        //    Dictionary<string, int> Visited =
        //        new Dictionary<string, int>(Common.Classes.Ultility.Ultility.MC_INITIAL_SIZE);
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

        //                ExpressionValue v = EvaluatorDenotational.Evaluate(ReachableStateCondition, step.GlobalEnv);
        //                if ((v as BoolConstant).Value)
        //                {
        //                    mdp.TargetStates.Add(nextIndex);
        //                }
        //                else
        //                {
        //                    working.Push(step);
        //                }
        //            }
        //        }
        //    } while (working.Count > 0);

        //    //mdp.SetTarget(Targets, isMax);
        //    this.VerificationOutput.NoOfStates = Visited.Count;
        //    return mdp;
        //}

        public override string GetResultString()
        {
            if (ConstraintType == QueryConstraintType.NONE)
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
                //sb.AppendLine("\t***The Assertion (" + ToString() + ") is Valid With Probability " + Ultility.Ultility.GetProbIntervalString(Min, Max, Precision) + ".");
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
                sb.AppendLine("The Assertion (" + ToString() + ") is NOT valid.");
                sb.AppendLine();
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

        //public override string StartingProcess
        //{
        //    get { return Process.ToString(); }
        //}
    }
}