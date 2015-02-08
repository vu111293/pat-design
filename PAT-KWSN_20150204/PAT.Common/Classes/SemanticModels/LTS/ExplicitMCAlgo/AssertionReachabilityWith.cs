using System;
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
    public abstract class AssertionReachabilityWith : AssertionReachability
    {
        public Expression ConstraintCondition;
        public QueryConstraintType Contraint;
        protected int? ExtremValue;
        protected int ReachableCount;
        public bool IsMonoIncreasing = true;

        protected AssertionReachabilityWith(string reachableState, QueryConstraintType cont, Expression constraintCondition) : base(reachableState)
        {
            Contraint = cont;
            ConstraintCondition = constraintCondition;
        }

        public override string ToString()
        {
            return StartingProcess + " reaches " + ReachableStateLabel + " with " + Contraint.ToString().ToLower() + "(" + ConstraintCondition + ")";
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
            DeadlockEngine.Add(Constants.ENGINE_DEPTH_FIRST_SEARCH);
            DeadlockEngine.Add(Constants.ENGINE_BREADTH_FIRST_SEARCH);
            DeadlockEngine.Add(Constants.ENGINE_BREADTH_FIRST_SEARCH_MONO);
            ModelCheckingOptions.AddAddimissibleBehavior(Constants.COMPLETE_BEHAVIOR, DeadlockEngine);
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
            else if (SelectedEngineName == Constants.ENGINE_BREADTH_FIRST_SEARCH)
            {
                BFSVerification();
            }
            else
            {
                MonoBFSVerification();
            }
        }

        private void BFSVerification()
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

            ExtremValue = null;
            ReachableCount = 0;

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
                    ReachableCount++;
                    Expression v = current.EvaluateExpression(ConstraintCondition);
                    if (v is IntConstant)
                    {
                        int value = (v as IntConstant).Value;
                        if (ExtremValue == null)
                        {
                            ExtremValue = value;
                            VerificationOutput.CounterExampleTrace = new List<ConfigurationBase>(currentPath);
                        }
                        else
                        {
                            switch (Contraint)
                            {
                                case QueryConstraintType.MAX:
                                    if (value > ExtremValue.Value)
                                    {
                                        ExtremValue = value;
                                        VerificationOutput.CounterExampleTrace = new List<ConfigurationBase>(currentPath);
                                    }
                                    break;
                                case QueryConstraintType.MIN:
                                    if (value < ExtremValue.Value)
                                    {
                                        ExtremValue = value;
                                        VerificationOutput.CounterExampleTrace = new List<ConfigurationBase>(currentPath);
                                    }
                                    break;
                            }
                        }
                    }
                }

                ConfigurationBase[] list = current.MakeOneMove().ToArray();
                VerificationOutput.Transitions += list.Length;

                for (int i = list.Length - 1; i >= 0; i--)
                {
                    ConfigurationBase step = list[i];
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

            if (ExtremValue == null)
            {
                VerificationOutput.VerificationResult = VerificationResultType.INVALID;
                VerificationOutput.CounterExampleTrace = null;
            }
            else
            {
                VerificationOutput.VerificationResult = VerificationResultType.VALID;
            }

            VerificationOutput.NoOfStates = Visited.Count;
        }

        public void DFSVerification()
        {
            StringHashTable Visited = new StringHashTable(Ultility.Ultility.MC_INITIAL_SIZE);
            Expression conditionExpression = this.ReachableStateCondition;

            Stack<ConfigurationBase> working = new Stack<ConfigurationBase>(1024);
            Visited.Add(InitialStep.GetID());

            working.Push(InitialStep);
            Stack<int> depthStack = new Stack<int>(1024);
            depthStack.Push(0);
            List<int> depthList = new List<int>(1024);

            ExtremValue = null;
            ReachableCount = 0;

            List<ConfigurationBase> CounterExampleTraceLocal = new List<ConfigurationBase>();
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
                        CounterExampleTraceLocal.RemoveAt(lastIndex);
                    }
                }

                CounterExampleTraceLocal.Add(current);

                if (current.ImplyCondition(conditionExpression))
                {
                    ReachableCount++;
                    Expression v = current.EvaluateExpression(ConstraintCondition);
                    if(v is IntConstant)
                    {
                        int value = (v as IntConstant).Value;
                        if(ExtremValue == null)
                        {
                            ExtremValue = value;
                            VerificationOutput.CounterExampleTrace = new List<ConfigurationBase>(CounterExampleTraceLocal);
                        }
                        else
                        {
                            switch (Contraint)
                            {
                                case QueryConstraintType.MAX:
                                    if(value > ExtremValue.Value)
                                    {
                                        ExtremValue = value;
                                        VerificationOutput.CounterExampleTrace = new List<ConfigurationBase>(CounterExampleTraceLocal);
                                    }
                                    break;
                                case QueryConstraintType.MIN:
                                    if(value < ExtremValue.Value)
                                    {
                                        ExtremValue = value;
                                        VerificationOutput.CounterExampleTrace = new List<ConfigurationBase>(CounterExampleTraceLocal);
                                    }
                                    break;
                            }
                        }
                    }
                }

                depthList.Add(depth);
                ConfigurationBase[] list = current.MakeOneMove().ToArray();
                VerificationOutput.Transitions += list.Length;

                for (int i = list.Length - 1; i >= 0; i--)
                {
                    ConfigurationBase step = list[i];

                    string stepID = step.GetID();
                    if (!Visited.ContainsKey(stepID))
                    {
                        Visited.Add(stepID);
                        working.Push(step);
                        depthStack.Push(depth + 1);
                    }
                }

            } while (working.Count > 0);

            if (ExtremValue == null)
            {
                VerificationOutput.VerificationResult = VerificationResultType.INVALID;
                VerificationOutput.CounterExampleTrace = null;
            }
            else
            {
                VerificationOutput.VerificationResult = VerificationResultType.VALID;    
            }

            VerificationOutput.NoOfStates = Visited.Count;          
            return;
        }


        public void MonoDFSVerification()
        {
            StringHashTable Visited = new StringHashTable(Ultility.Ultility.MC_INITIAL_SIZE);
            Expression conditionExpression = this.ReachableStateCondition;

            Stack<ConfigurationBase> working = new Stack<ConfigurationBase>(1024);
            Visited.Add(InitialStep.GetID());

            working.Push(InitialStep);
            Stack<int> depthStack = new Stack<int>(1024);
            depthStack.Push(0);
            List<int> depthList = new List<int>(1024);

            ExtremValue = null;
            ReachableCount = 0;

            List<ConfigurationBase> CounterExampleTraceLocal = new List<ConfigurationBase>();
            do
            {
                if (CancelRequested)
                {
                    VerificationOutput.NoOfStates = Visited.Count;
                    return;
                }

                ConfigurationBase current = working.Pop();
                int depth = depthStack.Pop();

                if(depth > 0)
                {
                    while (depthList[depthList.Count - 1] >= depth)
                    {
                        int lastIndex = depthList.Count - 1;
                        depthList.RemoveAt(lastIndex);
                        CounterExampleTraceLocal.RemoveAt(lastIndex);
                    }
                }
                CounterExampleTraceLocal.Add(current);

                Expression v = current.EvaluateExpression(ConstraintCondition);
                if (v is IntConstant)
                {
                    int value = (v as IntConstant).Value;
                    if (current.ImplyCondition(conditionExpression))
                    {
                        ReachableCount++;                       

                        if (ExtremValue == null)
                        {
                            ExtremValue = value;
                            VerificationOutput.CounterExampleTrace =
                                new List<ConfigurationBase>(CounterExampleTraceLocal);
                        }
                        else
                        {
                            switch (Contraint)
                            {
                                case QueryConstraintType.MAX:
                                    if (value > ExtremValue.Value)
                                    {
                                        ExtremValue = value;
                                        VerificationOutput.CounterExampleTrace =
                                            new List<ConfigurationBase>(CounterExampleTraceLocal);
                                    }
                                    break;
                                case QueryConstraintType.MIN:
                                    if (value < ExtremValue.Value)
                                    {
                                        ExtremValue = value;
                                        VerificationOutput.CounterExampleTrace =
                                            new List<ConfigurationBase>(CounterExampleTraceLocal);
                                    }
                                    break;
                            }
                        }
                    }
                    else
                    {
                        bool Skip = false;
                        if (ExtremValue != null)
                        {
                            switch (Contraint)
                            {
                                case QueryConstraintType.MAX:
                                    if (value < ExtremValue.Value)
                                    {
                                        Skip = true;
                                    }
                                    break;
                                case QueryConstraintType.MIN:
                                    if (value > ExtremValue.Value)
                                    {
                                        Skip = true;
                                    }
                                    break;
                            }
                        }

                        if (!Skip)
                        {
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
                        }
                    }
                }
            } while (working.Count > 0);

            if (ExtremValue == null)
            {
                VerificationOutput.VerificationResult = VerificationResultType.INVALID;
                VerificationOutput.CounterExampleTrace = null;
            }
            else
            {
                VerificationOutput.VerificationResult = VerificationResultType.VALID;
            }

            VerificationOutput.NoOfStates = Visited.Count;
            return;
        }

        private void MonoBFSVerification()
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

            ExtremValue = null;
            ReachableCount = 0;

            do
            {
                if (CancelRequested)
                {
                    VerificationOutput.NoOfStates = Visited.Count;
                    return;
                }

                ConfigurationBase current = working.Dequeue();
                List<ConfigurationBase> currentPath = paths.Dequeue();

                Expression v = current.EvaluateExpression(ConstraintCondition);
                if (v is IntConstant)
                {
                    int value = (v as IntConstant).Value;

                    if (current.ImplyCondition(conditionExpression))
                    {
                        ReachableCount++;

                        if (ExtremValue == null)
                        {
                            ExtremValue = value;
                            VerificationOutput.CounterExampleTrace = new List<ConfigurationBase>(currentPath);
                        }
                        else
                        {
                            switch (Contraint)
                            {
                                case QueryConstraintType.MAX:
                                    if (value > ExtremValue.Value)
                                    {
                                        ExtremValue = value;
                                        VerificationOutput.CounterExampleTrace = new List<ConfigurationBase>(currentPath);
                                    }
                                    break;
                                case QueryConstraintType.MIN:
                                    if (value < ExtremValue.Value)
                                    {
                                        ExtremValue = value;
                                        VerificationOutput.CounterExampleTrace = new List<ConfigurationBase>(currentPath);
                                    }
                                    break;
                            }
                        }
                    }
                    else
                    {
                        bool Skip = false;
                        if (ExtremValue != null)
                        {
                            switch (Contraint)
                            {
                                case QueryConstraintType.MAX:
                                    if (value < ExtremValue.Value)
                                    {
                                        Skip = true;
                                    }
                                    break;
                                case QueryConstraintType.MIN:
                                    if (value > ExtremValue.Value)
                                    {
                                        Skip = true;
                                    }
                                    break;
                            }
                        }

                        if (!Skip)
                        {
                            ConfigurationBase[] list = current.MakeOneMove().ToArray();
                            VerificationOutput.Transitions += list.Length;


                            for (int i = list.Length - 1; i >= 0; i--)
                            {
                                ConfigurationBase step = list[i];
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
                        }
                    }
                }

            } while (working.Count > 0);

            if (ExtremValue == null)
            {
                VerificationOutput.VerificationResult = VerificationResultType.INVALID;
                VerificationOutput.CounterExampleTrace = null;
            }
            else
            {
                VerificationOutput.VerificationResult = VerificationResultType.VALID;
            }

            VerificationOutput.NoOfStates = Visited.Count;
        }

        public override string GetResultString()
        {
            StringBuilder sb = new StringBuilder();

            //original reachability testing
            sb.AppendLine(Constants.VERFICATION_RESULT_STRING);
            if (VerificationOutput.VerificationResult == VerificationResultType.VALID)
            {
                sb.AppendLine("The Assertion (" + ToString() + ") is VALID and " + Contraint.ToString().ToLower() + "(" + ConstraintCondition + ") = " + this.ExtremValue + " among " + ReachableCount + " reachable states.");
                sb.AppendLine("The following trace leads to a state where the condition is satisfied with the above value.");

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
                    sb.AppendLine("The Assertion is NOT valid.");
                }
            }

            if (SelectedEngineName == Constants.ENGINE_BREADTH_FIRST_SEARCH_MONO)
            {
                sb.AppendLine("Warning: the correctness of the verification result requires the value of " + ConstraintCondition + " changes monotonically during the system execution. ");
            }

            sb.AppendLine();

            sb.AppendLine("********Verification Setting********");
            sb.AppendLine("Admissible Behavior: " + SelectedBahaviorName);
            sb.AppendLine("Search Engine: " + SelectedEngineName);
            sb.AppendLine("System Abstraction: " + MustAbstract); 
            sb.AppendLine();

            return sb.ToString();
        }


        /// <summary>
        /// GetResultStringForUnfinishedSearching
        /// </summary>
        /// <returns></returns>
        public override string GetResultStringForUnfinishedSearching(Exception ex)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine(Constants.VERFICATION_RESULT_STRING);

            if (ex != null)
            {
                sb.AppendLine(Resources.Exception_happened_during_the_verification);
                string trace = "";
                if (ex.Data.Contains("trace"))
                {
                    trace = Environment.NewLine + "Trace leads to exception:" + Environment.NewLine + ex.Data["trace"].ToString();
                }
                sb.AppendLine(ex.Message + trace);

            }
            else
            {
                sb.AppendLine("Verification cancelled");
            }

            
            if (ExtremValue != null)
            {
                sb.AppendLine("During the incomplete search:");
                sb.AppendLine("The Assertion (" + ToString() + ") is VALID and " + Contraint.ToString().ToLower() + "(" + ConstraintCondition + ") = " + this.ExtremValue + " among " + ReachableCount + " reachable states.");
                sb.AppendLine("The following trace leads to a state where the condition is satisfied with the above value.");
                VerificationOutput.GetCounterxampleString(sb);

                if (SelectedEngineName == Constants.ENGINE_BREADTH_FIRST_SEARCH_MONO)
                {
                    sb.AppendLine("Warning: the correctness of the verification result requires the value of " + ConstraintCondition + " changes monotonically during the system execution. ");
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