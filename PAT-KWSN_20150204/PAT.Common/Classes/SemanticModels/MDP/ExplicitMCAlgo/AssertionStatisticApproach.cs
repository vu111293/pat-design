using System.Collections.Generic;
using System.Linq;
using PAT.Common.Classes.Assertion;
using PAT.Common.Classes.Expressions;
using PAT.Common.Classes.Expressions.ExpressionClass;
using System.Text;
using PAT.Common.Classes.LTS;
using PAT.Common.Classes.ModuleInterface;
using PAT.Common.Classes.Ultility;
using QueryConstraintType = PAT.Common.Classes.Ultility.QueryConstraintType;


namespace PAT.Common.Classes.SemanticModels.MDP.Assertion
{
    public abstract class AssertionStatisticApproach: LTS.Assertion.AssertionReachability
    {
        public float[] StatisticalParameters = { -1, -1, -1, -1 };


        protected QueryConstraintType ConstraintType;
        private string str;

        protected AssertionStatisticApproach(string reachableState) : base(reachableState)
        {
        }

        /// <summary>
        /// Assertion Initialization to create the initial step based on the concrete types.
        /// This method shall be invoked after the parsing immediately to instanciate the initial step
        /// </summary>
        /// <param name="spec">The concrete specification of the module</param>
        public override void Initialize(SpecificationBase spec)
        {
            //initialize model checking options, the default option is for deadlock/reachablity algorithms
            ModelCheckingOptions = new ModelCheckingOptions();
            List<string> DeadlockEngine = new List<string>();
            DeadlockEngine.Add(Constants.ENGINE_SMT_DTMC);
            DeadlockEngine.Add(Constants.ENGINE_SMT_MDP);
            ModelCheckingOptions.AddAddimissibleBehavior(Constants.COMPLETE_BEHAVIOR, DeadlockEngine);
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
            MDPStat mdp = BuildMDP(); //GetTransitionRelation();
            if (StatisticVerificationMinReach(mdp, VerificationOutput))
            {
                this.VerificationOutput.VerificationResult = VerificationResultType.VALID;
            }
            else
            {
                this.VerificationOutput.VerificationResult = VerificationResultType.INVALID;
            }
        }

        private MDPStat BuildMDP()
        {
            Stack<KeyValuePair<MDPConfiguration, MDPStateStat>> working = new Stack<KeyValuePair<MDPConfiguration, MDPStateStat>>(1024);

            string initID = InitialStep.GetID();
            MDPStateStat init = new MDPStateStat(initID);
            working.Push(new KeyValuePair<MDPConfiguration, MDPStateStat>(InitialStep as MDPConfiguration, init));
            MDPStat mdp = new MDPStat(init, Ultility.Ultility.DEFAULT_PRECISION, Ultility.Ultility.MAX_DIFFERENCE);

            do
            {
                if (CancelRequested)
                {
                    VerificationOutput.NoOfStates = mdp.States.Count;
                    return mdp;
                }

                KeyValuePair<MDPConfiguration, MDPStateStat> current = working.Pop();
                IEnumerable<MDPConfiguration> list = current.Key.MakeOneMoveLocal();
                VerificationOutput.Transitions += list.Count();

                int currentDistriIndex = -1;
                DistributionStat newDis = new DistributionStat(Constants.TAU);

                //for (int i = 0; i < list.Length; i++)
                foreach (MDPConfiguration step in list)
                {
                    //MDPConfiguration step = list[i];
                    string stepID = step.GetID();
                    MDPStateStat nextState;

                    if (!mdp.States.TryGetValue(stepID, out nextState))
                    {
                        nextState = new MDPStateStat(stepID);
                        mdp.AddState(nextState);

                        ExpressionValue v = EvaluatorDenotational.Evaluate(ReachableStateCondition, step.GlobalEnv);

                        if ((v as BoolConstant).Value)
                        {
                            mdp.AddTargetStates(nextState);
                        }
                        else
                        {
                            working.Push(new KeyValuePair<MDPConfiguration, MDPStateStat>(step, nextState));
                        }
                    }

                    if (step.DisIndex == -1)
                    {
                        if (currentDistriIndex >= 0)
                        {
                            current.Value.AddDistribution(newDis);
                            newDis = new DistributionStat(Constants.TAU);
                        }

                        var newTrivialDis = new DistributionStat(step.Event);
                        newTrivialDis.AddProbStatePair(1, nextState);
                        current.Value.AddDistribution(newTrivialDis);
                    }
                    else if (currentDistriIndex != -1 && step.DisIndex != currentDistriIndex)
                    {
                        current.Value.AddDistribution(newDis);
                        newDis = new DistributionStat(Constants.TAU);
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
            return mdp;
        }

        private bool StatisticVerificationMinReach(MDPStat mdp, VerificationOutput verificationOutput)
        {
            mdp.setNonZeroStates();
            var sml = new Simulation(mdp);
            var pl = new Policy(mdp);
            var tst = new RunTest();
            tst.initRunTest(StatisticalParameters[1], StatisticalParameters[2], StatisticalParameters[3], StatisticalParameters[0]);
           // string str;
            return tst.verifyMDPSPRT(sml, pl, out str);
        }

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
            else 
            {
                sb.AppendLine("The Assertion (" + ToString() + ") is Not Valid.");
            }

            sb.AppendLine();

            sb.AppendLine("********Verification Setting********");
            sb.AppendLine("Admissible Behavior: " + SelectedBahaviorName);
            sb.AppendLine("Search Engine: " + SelectedEngineName);
            sb.AppendLine("System Abstraction: " + MustAbstract);
            sb.AppendLine();

            sb.AppendLine("********Statistical Information********");
            sb.AppendLine(str);
            sb.AppendLine();

            return sb.ToString();
        }
    }
}
