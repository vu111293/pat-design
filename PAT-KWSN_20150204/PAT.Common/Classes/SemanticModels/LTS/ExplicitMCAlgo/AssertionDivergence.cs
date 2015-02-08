using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using PAT.Common.Classes.DataStructure;
using PAT.Common.Classes.ModuleInterface;
using PAT.Common.Classes.Ultility;
using System;

namespace PAT.Common.Classes.SemanticModels.LTS.Assertion
{
    public abstract class AssertionDivergence : AssertionBase
    {
        private int LoopIndex = -1;

        public override string ToString()
        {
            return StartingProcess + " divergencefree";
        }

        public override void Initialize(SpecificationBase spec)
        {
            //initialize model checking options, the default option is for deadlock/reachablity algorithms
            ModelCheckingOptions = new ModelCheckingOptions();
            List<string> DeadlockEngine = new List<string>();
            DeadlockEngine.Add(Constants.ENGINE_SCC_BASED_SEARCH);
            DeadlockEngine.Add(Constants.ENGINE_DEPTH_FIRST_SEARCH);
            DeadlockEngine.Add(Constants.ENGINE_BREADTH_FIRST_SEARCH);
            ModelCheckingOptions.AddAddimissibleBehavior(Constants.COMPLETE_BEHAVIOR, DeadlockEngine);
        }

        /// <summary>
        /// Run the verification and get the result.
        /// </summary>
        /// <returns></returns>
        public override void RunVerification()
        {
            LoopIndex = -1;
            if (SelectedEngineName == Constants.ENGINE_DEPTH_FIRST_SEARCH)
            {
                DFSVerification();
            }
            else if (SelectedEngineName == Constants.ENGINE_BREADTH_FIRST_SEARCH)
            {
                BFSVerification();
            }
            else if (SelectedEngineName == Constants.ENGINE_SCC_BASED_SEARCH)
            {
                SCCVerification();
            }
        }

        public void DFSVerification()
        {
            StringHashTable Visited = new StringHashTable(Common.Classes.Ultility.Ultility.MC_INITIAL_SIZE);
            Stack<ConfigurationBase> pendingImpl = new Stack<ConfigurationBase>(1000);
            VisitedNonDivStates = new HashSet<string>();

            //The following are for identifying a counterexample trace. 
            Stack<int> depthStack = new Stack<int>(1000);
            depthStack.Push(0);
            List<int> depthList = new List<int>(1000);
            //The above are for identifying a counterexample trace. 

            //implementation initial state
            ConfigurationBase currentImpl = InitialStep;
            pendingImpl.Push(currentImpl);

            Visited.Add(currentImpl.GetID());

            while (pendingImpl.Count > 0)
            {
                if (CancelRequested)
                {
                    this.VerificationOutput.NoOfStates = Visited.Count;
                    return;
                }

                currentImpl = pendingImpl.Pop();

                //The following are for identifying a counterexample trace. 
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

                this.VerificationOutput.CounterExampleTrace.Add(currentImpl);
                depthList.Add(depth);

                //if (currentImpl.IsDivergent())
                if (!VisitedNonDivStates.Contains(currentImpl.GetID()) && IsDivergent(currentImpl))
                {
                    VerificationOutput.VerificationResult = VerificationResultType.INVALID;
                    VerificationOutput.NoOfStates = Visited.Count;
                    //VerificationOutput.CounterExampleTrace.RemoveAt(VerificationOutput.CounterExampleTrace.Count - 1);
                    LoopIndex = VerificationOutput.CounterExampleTrace.Count;
                    return;
                }

                ConfigurationBase[] list = currentImpl.MakeOneMove().ToArray();

                this.VerificationOutput.Transitions += list.Length;

                for (int i = list.Length - 1; i >= 0; i--)
                {
                    ConfigurationBase step = list[i];
                    string stepID = step.GetID();
                    if (!Visited.ContainsKey(stepID))
                    {
                        Visited.Add(stepID);
                        pendingImpl.Push(step);
                        depthStack.Push(depth + 1);
                    }
                }
            }

            VerificationOutput.CounterExampleTrace = null;
            VerificationOutput.VerificationResult = VerificationResultType.VALID;
            VerificationOutput.NoOfStates = Visited.Count;
        }

        public void BFSVerification()
        {
            StringHashTable Visited = new StringHashTable(Ultility.Ultility.MC_INITIAL_SIZE);
            VisitedNonDivStates = new HashSet<string>();
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
                    this.VerificationOutput.NoOfStates = Visited.Count;
                    return;
                }

                ConfigurationBase current = working.Dequeue();
                List<ConfigurationBase> currentPath = paths.Dequeue();
                ConfigurationBase[] list = current.MakeOneMove().ToArray();

                Debug.Assert(currentPath[currentPath.Count - 1].GetID() == current.GetID());

                VerificationOutput.Transitions += list.Length;

                //if (current.IsDivergent())
                if (!VisitedNonDivStates.Contains(current.GetID()) && IsDivergent(current))
                {
                    VerificationOutput.VerificationResult = VerificationResultType.INVALID;
                    VerificationOutput.NoOfStates = Visited.Count;
                    VerificationOutput.CounterExampleTrace = currentPath;
                    //VerificationOutput.CounterExampleTrace.RemoveAt(VerificationOutput.CounterExampleTrace.Count - 1);
                    LoopIndex = VerificationOutput.CounterExampleTrace.Count;
                    return;
                }

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

            VerificationOutput.CounterExampleTrace = null;
            VerificationOutput.VerificationResult = VerificationResultType.VALID;
            VerificationOutput.NoOfStates = Visited.Count;
            return;
        }


        public void SCCVerification()
        {
            StringHashTable Visited = new StringHashTable(Common.Classes.Ultility.Ultility.MC_INITIAL_SIZE);
            VisitedNonDivStates = new HashSet<string>();
            Stack<ConfigurationBase> pendingImpl = new Stack<ConfigurationBase>(1000);

            //The following are for identifying a counterexample trace. 
            Stack<int> depthStack = new Stack<int>(1000);
            depthStack.Push(0);
            List<int> depthList = new List<int>(1000);
            //The above are for identifying a counterexample trace. 

            //implementation initial state
            ConfigurationBase currentImpl = InitialStep;
            pendingImpl.Push(currentImpl);

            string statestring = currentImpl.GetID();
            //DataStore.DataManager.Visit(statestring);
            Visited.Add(statestring);
            // List<string> procPath = new List<string>(1000);

            while (pendingImpl.Count > 0)
            {
                if (CancelRequested)
                {
                    this.VerificationOutput.NoOfStates = Visited.Count;
                    return;
                }

                currentImpl = pendingImpl.Pop();
                statestring = currentImpl.GetID();

                //The following are for identifying a counterexample trace. 
                int depth = depthStack.Pop();
                if (depth > 0)
                {
                    while (depth > 0 && depthList[depthList.Count - 1] >= depth)
                    {
                        int lastIndex = depthList.Count - 1;
                        depthList.RemoveAt(lastIndex);
                        VerificationOutput.CounterExampleTrace.RemoveAt(lastIndex);                       
                    }
                }

                VerificationOutput.CounterExampleTrace.Add(currentImpl);
                depthList.Add(depth);

                if (!VisitedNonDivStates.Contains(statestring) && IsDivergent(currentImpl))
                {
                    VerificationOutput.VerificationResult = VerificationResultType.INVALID;
                    VerificationOutput.NoOfStates = Visited.Count;
                    //VerificationOutput.CounterExampleTrace.RemoveAt(VerificationOutput.CounterExampleTrace.Count - 1);
                    LoopIndex = VerificationOutput.CounterExampleTrace.Count;
                    return;
                }

                ConfigurationBase[] list = currentImpl.MakeOneMove().ToArray();
                this.VerificationOutput.Transitions += list.Length;

                for (int i = list.Length - 1; i >= 0; i--)
                {
                    ConfigurationBase step = list[i];
                    string stepID = step.GetID();
                    if (!Visited.ContainsKey(stepID))
                    {
                        Visited.Add(stepID);
                        pendingImpl.Push(step);
                        depthStack.Push(depth + 1);
                    }
                }
            }

            VerificationOutput.CounterExampleTrace = null;
            VerificationOutput.VerificationResult = VerificationResultType.VALID;
            VerificationOutput.NoOfStates = Visited.Count;

        }

        private static HashSet<string> VisitedNonDivStates;
        protected const int VISITED_NOPREORDER = -1;
        protected const int SCC_FOUND = -2;
        /// <summary>
        /// To get all states which can be reached by tau-transitions only; meanwhile, identify which of them are divergent states.
        /// </summary>
        /// <param name="States"></param>
        /// <returns></returns>
        private static bool IsDivergent(ConfigurationBase States)
        {
            Dictionary<string, int[]> DFSData = new Dictionary<string, int[]>();
            Dictionary<string, List<string>> transitions = new Dictionary<string, List<string>>();
            List<string> StronglyConnectedComponets = new List<string>();

            Stack<ConfigurationBase> TaskStack = new Stack<ConfigurationBase>();


            TaskStack.Push(States);
            DFSData.Add(States.GetID(), new int[] { VISITED_NOPREORDER, 0 });

            transitions.Add(States.GetID(), new List<string>());

            Stack<ConfigurationBase> stepStack = new Stack<ConfigurationBase>();
        
            //# Preorder counter 
            int ii = 0;

            //store the expended event step of a node to avoid multiple invocation of the make one move.
            Dictionary<string, List<ConfigurationBase>> ExpendedNode = new Dictionary<string, List<ConfigurationBase>>();

            do
            {
                ConfigurationBase pair = TaskStack.Peek();
                string v = pair.GetID();
               

                List<string> outgoing = transitions[v];

                int[] nodeData = DFSData[v];

                if (nodeData[0] == VISITED_NOPREORDER)
                {
                    nodeData[0] = ii;
                    ii++;
                }

                bool done = true;

                if (ExpendedNode.ContainsKey(v))
                {
                    List<ConfigurationBase> list = ExpendedNode[v];
                    if (list.Count > 0)
                    {
                        //transverse all steps
                        for (int k = list.Count - 1; k >= 0; k--)
                        {
                            ConfigurationBase step = list[k];

                            string tmp = step.GetID();

                            //if the step is a unvisited step
                            //if (!preorder.ContainsKey(t))
                            if (DFSData[tmp][0] == VISITED_NOPREORDER)
                            {
                                //only add the first unvisited step
                                //for the second or more unvisited steps, ignore at the monent
                                if (done)
                                {
                                    TaskStack.Push(step);

                                    //procPath.Add(step.GetID());

                                    done = false;
                                    list.RemoveAt(k);
                                }
                            }
                            else
                            {
                                list.RemoveAt(k);
                            }
                        }
                    }
                }
                else
                {
                    List<ConfigurationBase> product = new List<ConfigurationBase>(pair.MakeOneMove(Constants.TAU));
                        //.AmpleTau(procPath, false);

                    for (int k = product.Count - 1; k >= 0; k--)
                    {
                        ConfigurationBase step = product[k];

                        string tmp = step.GetID();

                        //if (DFSData.ContainsKey(tmp))
                        int[] data;
                        if (DFSData.TryGetValue(tmp, out data))
                        {
                            //int t = visited[stateString];
                            outgoing.Add(tmp);

                            //if this node is still not visited
                            //if (!preorder.ContainsKey(tmp))
                            if (data[0] == VISITED_NOPREORDER)
                            {
                                //only put the first one to the work list stack.
                                //if there are more than one node to be visited, 
                                //simply ignore them and keep its event step in the list.
                                if (done)
                                {
                                    TaskStack.Push(step);
                                    done = false;
                                    product.RemoveAt(k);
                                }
                                else
                                {
                                    product[k] = step;
                                }
                            }
                                //this node is truly visited. can be removed
                            else
                            {
                                product.RemoveAt(k);
                            }
                        }
                        else
                        {
                            DFSData.Add(tmp, new int[] { VISITED_NOPREORDER, 0 });

                            transitions.Add(tmp, new List<string>(8));
                            outgoing.Add(tmp);
                            //only put the first one into the stack.
                            if (done)
                            {
                                TaskStack.Push(step);


                                done = false;
                                product.RemoveAt(k);
                            }
                            else
                            {
                                product[k] = step;
                            }
                        }
                    }

                    //create the remaining steps as the expending list for v
                    ExpendedNode.Add(v, product);
                }

                if (done)
                {
                    int lowlinkV = nodeData[0];
                    int preorderV = lowlinkV;

                    bool selfLoop = false;
                    for (int j = 0; j < outgoing.Count; j++)
                    {
                        string w = outgoing[j];
                        if (w == v)
                        {
                            selfLoop = true;
                        }

                        int[] wdata = DFSData[w];
                        if (wdata[0] != SCC_FOUND)
                        {
                            if (wdata[0] > preorderV)
                            {
                                lowlinkV = Math.Min(lowlinkV, wdata[1]);
                            }
                            else
                            {
                                lowlinkV = Math.Min(lowlinkV, wdata[0]);
                            }
                        }
                    }
                    nodeData[1] = lowlinkV;

                    TaskStack.Pop();

                    if (lowlinkV == preorderV)
                    {
                        StronglyConnectedComponets.Add(v);
                        nodeData[0] = SCC_FOUND;

                        while (stepStack.Count > 0 && DFSData[stepStack.Peek().GetID()][0] > preorderV)
                        {
                            ConfigurationBase s = stepStack.Pop();
                            string tmp = s.GetID();
                            //int k = visited[tmp];
                            StronglyConnectedComponets.Add(tmp);
                            //scc_found.Add(tmp);
                        }

                        //outgoing.Count == 0 --> deadlock, we need to check //outgoing.Count == 0
                        //StronglyConnectedComponets.Count > 1 || selfLoop -> non-trivial case, we need to check
                        if (StronglyConnectedComponets.Count > 1 || selfLoop)
                        {
                            return true;
                        }

                        foreach (string componet in StronglyConnectedComponets)
                        {
                            ExpendedNode.Remove(componet);
                        }

                        StronglyConnectedComponets.Clear();
                    }
                    else
                    {
                        stepStack.Push(pair);
                    }
                }
            } while (TaskStack.Count > 0);


            foreach (string key in DFSData.Keys)
            {
                VisitedNonDivStates.Add(key);
            }

            return false;

        }

        /// <summary>
        /// This method checks whether the found counterexample is spurious or not using Liushanshan's condition.
        /// This checking only works for abstraction for parameterized systems. 
        /// </summary>
        /// <returns></returns>
        protected override bool IsCounterExampleSpurious()
        {
            Stack<ConfigurationBase> working = new Stack<ConfigurationBase>(1024);
            List<ConfigurationBase> ConcreteCounterExampleTrace = new List<ConfigurationBase>(64);
            working.Push(InitialStep);
            Stack<int> depthStack = new Stack<int>(1024);
            depthStack.Push(0);
            List<int> depthList = new List<int>(1024);
            StringHashTable visited = new StringHashTable(1024);
            visited.Add("0-" + InitialStep.GetID());

            do
            {
                ConfigurationBase current = working.Pop();
                int depth = depthStack.Pop();

                if (depth > 0)
                {
                    while (depthList[depthList.Count - 1] >= depth)
                    {
                        int lastIndex = depthList.Count - 1;
                        depthList.RemoveAt(lastIndex);
                        ConcreteCounterExampleTrace.RemoveAt(lastIndex);
                    }
                }

                ConcreteCounterExampleTrace.Add(current);
                depthList.Add(depth);

                if (ConcreteCounterExampleTrace.Count == this.VerificationOutput.CounterExampleTrace.Count)
                {
                    IEnumerable<ConfigurationBase> steps = current.MakeOneMove(Constants.TAU);

                    foreach (ConfigurationBase step in steps)
                    {
                        if (step.GetID() == ConcreteCounterExampleTrace[LoopIndex].GetID())
                        {
                            this.VerificationOutput.CounterExampleTrace = ConcreteCounterExampleTrace;
                            return false;                            
                        }
                    }
                }
                else
                {
                    ConfigurationBase abstractStep = this.VerificationOutput.CounterExampleTrace[depth + 1];

                    IEnumerable<ConfigurationBase> steps = current.MakeOneMove(abstractStep.Event);
                    //for (int j = 0; j < steps.Length; j++)
                    foreach (ConfigurationBase step in steps)
                    {
                        string tmp = (depth + 1) + "-" + step.GetID();
                        if (!visited.ContainsKey(tmp))
                        {
                            working.Push(step);
                            depthStack.Push(depth + 1);
                            visited.Add(tmp);
                        }
                    }
                }

            } while (working.Count > 0);

            return true;
        }

        public override string GetResultString()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine(Constants.VERFICATION_RESULT_STRING);
            if (this.VerificationOutput.VerificationResult == VerificationResultType.VALID)
            {
                sb.AppendLine("The Assertion (" + ToString() + ") is VALID.");
            }         
            else
            {
                sb.AppendLine("The Assertion (" + ToString() + ") is NOT valid.");
                sb.AppendLine("A state reached via the following trace is divergent.");
                VerificationOutput.GetCounterxampleString(sb);
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