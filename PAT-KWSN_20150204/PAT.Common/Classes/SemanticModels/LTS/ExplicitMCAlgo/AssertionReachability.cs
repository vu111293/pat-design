using System.Collections.Generic;
using System.Linq;
using PAT.Common.Classes.DataStructure;
using PAT.Common.Classes.Expressions.ExpressionClass;
using PAT.Common.Classes.LTS;
using System.Text;
using PAT.Common.Classes.ModuleInterface;
using PAT.Common.Classes.Ultility;

namespace PAT.Common.Classes.SemanticModels.LTS.Assertion
{
    public abstract partial class AssertionReachability : AssertionBase
    {
        //=========================================================
        //model checking related varaibles
        public Expression ReachableStateCondition;
        public string ReachableStateLabel;
        
        protected AssertionReachability(string reachableState)
        {
            ReachableStateLabel = reachableState;
            //AssertType = AssertionType.Reachability;
        }

        public override string ToString()
        {
            return StartingProcess + " reaches " + ReachableStateLabel;
        }

        /// <summary>
        /// Run the verification and get the result.
        /// </summary>
        /// <returns></returns>
        public override void RunVerification()
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

        virtual public void DFSVerification()
        {
            StringHashTable Visited = new StringHashTable(Ultility.Ultility.MC_INITIAL_SIZE);

            Expression conditionExpression = ReachableStateCondition;

            Stack<ConfigurationBase> working = new Stack<ConfigurationBase>(1024);

            Visited.Add(InitialStep.GetID());

            working.Push(InitialStep);
            Stack<int> depthStack = new Stack<int>(1024);
            depthStack.Push(0);

            List<int> depthList = new List<int>(1024);

            do
            {
                if (CancelRequested)
                {
                    VerificationOutput.NoOfStates = Visited.Count;
                    return;
                }

                ConfigurationBase current = working.Pop();
                int depth = depthStack.Pop();

                if (depth > 0)
                {
                    while (depthList[depthList.Count - 1] >= depth)
                    {
                        int lastIndex = depthList.Count - 1;
                        depthList.RemoveAt(lastIndex);
                        VerificationOutput.CounterExampleTrace.RemoveAt(lastIndex);
                    }
                }

                VerificationOutput.CounterExampleTrace.Add(current);

                if (current.ImplyCondition(conditionExpression))
                {
                    VerificationOutput.VerificationResult = VerificationResultType.VALID;
                    VerificationOutput.NoOfStates = Visited.Count;
                    return;
                }

                depthList.Add(depth);
                IEnumerable<ConfigurationBase> list = current.MakeOneMove();
                VerificationOutput.Transitions += list.Count();

                //for (int i = list.Length - 1; i >= 0; i--)
                foreach (ConfigurationBase step in list)
                {
                    //ConfigurationBase step = list[i];

                    string stepID = step.GetID();
                    if (!Visited.ContainsKey(stepID))
                    {
                        Visited.Add(stepID);
                        working.Push(step);
                        depthStack.Push(depth + 1);
                    }
                }
            } while (working.Count > 0);

            VerificationOutput.CounterExampleTrace = null;
            VerificationOutput.VerificationResult = VerificationResultType.INVALID;
            VerificationOutput.NoOfStates = Visited.Count;
        }

        virtual public void BFSVerification()
        {
            StringHashTable Visited = new StringHashTable(Ultility.Ultility.MC_INITIAL_SIZE);

            Expression conditionExpression = this.ReachableStateCondition;

            Queue<ConfigurationBase> working = new Queue<ConfigurationBase>(1024);
            Queue<List<ConfigurationBase>> paths = new Queue<List<ConfigurationBase>>(1024);

            Visited.Add(InitialStep.GetID());

            working.Enqueue(InitialStep);

            List<ConfigurationBase> path = new List<ConfigurationBase>();
            path.Add(InitialStep);
            paths.Enqueue(path);

            do
            {
                if (CancelRequested)
                {
                    VerificationOutput.NoOfStates = Visited.Count;
                    return;
                }

                ConfigurationBase current = working.Dequeue();
                List<ConfigurationBase> currentPath = paths.Dequeue();

                if (current.ImplyCondition(conditionExpression))
                {
                    VerificationOutput.VerificationResult = VerificationResultType.VALID;
                    VerificationOutput.NoOfStates = Visited.Count;
                    VerificationOutput.CounterExampleTrace = currentPath;
                    return;
                }

                //ConfigurationBase[] list = current.MakeOneMove().ToArray();
                //VerificationOutput.Transitions += list.Length;
                IEnumerable<ConfigurationBase> list = current.MakeOneMove();
                VerificationOutput.Transitions += list.Count();
                
                //for (int i = list.Length - 1; i >= 0; i--)
                foreach (ConfigurationBase step in list)
                {
                    //ConfigurationBase step = list[i];
                    string stepID = step.GetID();
                    if (!Visited.ContainsKey(stepID))
                    {
                        Visited.Add(stepID);
                        working.Enqueue(step);

                        List<ConfigurationBase> newPath = new List<ConfigurationBase>(currentPath);
                        newPath.Add(step);
                        paths.Enqueue(newPath);
                    }
                }
            } while (working.Count > 0);


            VerificationOutput.VerificationResult = VerificationResultType.INVALID;

            VerificationOutput.NoOfStates = Visited.Count;
        }


        public override string GetResultString()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine(Constants.VERFICATION_RESULT_STRING);
            if (VerificationOutput.VerificationResult == VerificationResultType.VALID)
            {
                sb.AppendLine("The Assertion (" + ToString() + ") is VALID.");
                sb.AppendLine("The following trace leads to a state where the condition is satisfied.");
                VerificationOutput.GetCounterxampleString(sb);
            }
            else
            {
                if (VerificationOutput.VerificationResult == VerificationResultType.UNKNOWN)
                {
                    sb.AppendLine("The Assertion (" + ToString() + ") is NEITHER PROVED NOR DISPROVED.");
                }
                else
                {
                    sb.AppendLine("The Assertion (" + ToString() + ") is NOT valid.");
                }
            }

            sb.AppendLine();

            sb.AppendLine("********Verification Setting********");
            sb.AppendLine("Admissible Behavior: " + SelectedBahaviorName);
            sb.AppendLine("Search Engine: " + SelectedEngineName);
            sb.AppendLine("System Abstraction: " + MustAbstract);
            sb.AppendLine();

            return sb.ToString();
        }
    }
}