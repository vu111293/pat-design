using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PAT.Common.Classes.DataStructure;
using PAT.Common.Classes.ModuleInterface;
using PAT.Common.Classes.Ultility;

namespace PAT.Common.Classes.SemanticModels.LTS.Assertion
{
    public abstract class AssertionRefinement : AssertionBase
    {
        public ConfigurationBase InitSpecStep;

        protected AssertionRefinement()
        {
        }

        public override string ToString()
        {
            return StartingProcess + " refines " + SpecProcess;
        }

        public abstract string SpecProcess
        {
            get;
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
            DeadlockEngine.Add(Constants.ENGINE_ANTICHAIN_DEPTH_FIRST_SEARCH);
            DeadlockEngine.Add(Constants.ENGINE_ANTICHAIN_BREADTH_FIRST_SEARCH);
            DeadlockEngine.Add(Constants.ENGINE_REFINEMENT_DEPTH_FIRST_SEARCH);
            DeadlockEngine.Add(Constants.ENGINE_REFINEMENT_BREADTH_FIRST_SEARCH);
            ModelCheckingOptions.AddAddimissibleBehavior(Constants.COMPLETE_BEHAVIOR, DeadlockEngine);
        }

        /// <summary>
        /// Run the verification and get the result.
        /// </summary>
        /// <returns></returns>
        public override void RunVerification()
        {
            if (SelectedEngineName == Constants.ENGINE_ANTICHAIN_DEPTH_FIRST_SEARCH)
            {
                Automata spec = BuildAutomata(InitSpecStep);
                TraceInclusionCheckDFSAntiChain(spec);
            }
            else
            {
                if (SelectedEngineName == Constants.ENGINE_ANTICHAIN_BREADTH_FIRST_SEARCH)
                {
                    Automata spec = BuildAutomata(InitSpecStep);
                    TraceInclusionCheckBFSAntiChain(spec);
                }
                else
                {
                    DeterministicAutomata auto = BuildDeterministicAutomata(InitSpecStep);

                    if (SelectedEngineName == Constants.ENGINE_REFINEMENT_DEPTH_FIRST_SEARCH)
                    {
                        TraceInclusionCheckDFS(auto);
                    }
                    else
                    {
                        TraceInclusionCheckBFS(auto);
                    }
                }
            }
        }

        /// <summary>
        /// Build the state graph reachable from the given configuration
        /// </summary>
        /// <param name="initial"></param>
        /// <returns></returns>
        public static DeterministicAutomata BuildDeterministicAutomata(ConfigurationBase initStep)
        {
            return BuildAutomata(initStep).Determinize();
        }


        //used to store the subset relation during deterministic states; used to calculate probabilistic refinement 
        public static DeterministicAutomata_Subset BuildDeterministicAutomata_Subset(ConfigurationBase initStep)
        {
            return BuildAutomata(initStep).DeterminizeSubset();
        }

        public static Automata BuildAutomata(ConfigurationBase initStep)
        {
            Dictionary<string, FAState> visited = new Dictionary<string, FAState>();
            Stack<ConfigurationBase> working = new Stack<ConfigurationBase>(1024);
            working.Push(initStep);
            Automata auto = new Automata();
            FAState init = auto.AddState();
            auto.SetInitialState(init);
            visited.Add(initStep.GetID(), init);

            do
            {
                ConfigurationBase current = working.Pop();
                FAState currentState = visited[current.GetID()];

                IEnumerable<ConfigurationBase> list = current.MakeOneMove();

                //for (int i = 0; i < list.Length; i++)
                foreach (ConfigurationBase step in list)
                {
                    //ConfigurationBase step = list[i];
                    FAState target;
                    string nextID = step.GetID();
                    if (visited.ContainsKey(nextID))
                    {
                        target = visited[nextID];
                    }
                    else
                    {
                        target = auto.AddState();
                        working.Push(step);
                        visited.Add(nextID, target);
                    }

                    auto.AddTransition(currentState, step.Event, target);
                }
            }
            while (working.Count > 0);

            return auto;
        }

        public void TraceInclusionCheckDFS(DeterministicAutomata spec)
        {
            StringHashTable Visited = new StringHashTable(Ultility.Ultility.MC_INITIAL_SIZE);
            List<ConfigurationBase> toReturn = new List<ConfigurationBase>();

            Stack<ConfigurationBase> pendingImpl = new Stack<ConfigurationBase>(1024);
            Stack<DeterministicFAState> pendingSpec = new Stack<DeterministicFAState>(1024);

            //The following are for identifying a counterexample trace. 
            Stack<int> depthStack = new Stack<int>(1024);
            depthStack.Push(0);
            List<int> depthList = new List<int>(1024);
            //The above are for identifying a counterexample trace. 

            //implementation initial state
            pendingImpl.Push(InitialStep);

            //specification initial state
            pendingSpec.Push(spec.InitialState);

            Visited.Add(InitialStep.GetID() + Constants.SEPARATOR + spec.InitialState.GetID());

            while (pendingImpl.Count > 0)
            {
                if (CancelRequested)
                {
                    VerificationOutput.NoOfStates = Visited.Count; // VisitedWithID.Count;
                    return;
                }

                ConfigurationBase currentImpl = pendingImpl.Pop();
                DeterministicFAState currentSpec = pendingSpec.Pop();

                //The following are for identifying a counterexample trace. 
                int depth = depthStack.Pop();

                if (depth > 0)
                {
                    while (depthList[depthList.Count - 1] >= depth)
                    {
                        int lastIndex = depthList.Count - 1;
                        depthList.RemoveAt(lastIndex);
                        toReturn.RemoveAt(lastIndex);
                    }
                }

                toReturn.Add(currentImpl);
                depthList.Add(depth);

                //If the specification has no corresponding state, then it implies that the trace is allowed by the 
                //implementation but not the specification -- which means trace-refinement is failed.

                IEnumerable<ConfigurationBase> nextImpl = currentImpl.MakeOneMove();
                VerificationOutput.Transitions += nextImpl.Count();

                //for (int k = 0; k < nextImpl.Length; k++)
                foreach (ConfigurationBase next in nextImpl)
                {
                    //ConfigurationBase next = nextImpl[k];
                    DeterministicFAState nextSpec = currentSpec;
                    if (next.Event != Constants.TAU)
                    {
                        nextSpec = currentSpec.Next(next.Event);

                        if (nextSpec == null)
                        {
                            toReturn.Add(next);
                            VerificationOutput.NoOfStates = Visited.Count;
                            VerificationOutput.CounterExampleTrace = toReturn;
                            VerificationOutput.VerificationResult = VerificationResultType.INVALID;
                            return;
                        }
                    }

                    string ID = next.GetID() + Constants.SEPARATOR + nextSpec.GetID();
                    if (!Visited.ContainsKey(ID))
                    {
                        pendingImpl.Push(next);
                        pendingSpec.Push(nextSpec);
                        depthStack.Push(depth + 1);
                        Visited.Add(ID);
                    }
                }
            }

            VerificationOutput.NoOfStates = Visited.Count;
            VerificationOutput.VerificationResult = VerificationResultType.VALID;
        }

        public void TraceInclusionCheckBFS(DeterministicAutomata spec)
        {
            StringHashTable Visited = new StringHashTable(Ultility.Ultility.MC_INITIAL_SIZE);

            Queue<ConfigurationBase> pendingImpl = new Queue<ConfigurationBase>(1024);
            Queue<DeterministicFAState> pendingSpec = new Queue<DeterministicFAState>(1024);
            Queue<List<ConfigurationBase>> paths = new Queue<List<ConfigurationBase>>(1024);

            //The following are for identifying a counterexample trace. 
            Stack<int> depthStack = new Stack<int>(1024);
            depthStack.Push(0);
            //The above are for identifying a counterexample trace. 

            //implementation initial state
            pendingImpl.Enqueue(InitialStep);
            pendingSpec.Enqueue(spec.InitialState);

            string statestring = spec.InitialState.GetID() + Constants.SEPARATOR + InitialStep.GetID();
            Visited.Add(statestring);

            List<ConfigurationBase> path = new List<ConfigurationBase>();
            path.Add(InitialStep);
            paths.Enqueue(path);

            while (pendingImpl.Count > 0)
            {
                if (CancelRequested)
                {
                    VerificationOutput.NoOfStates = Visited.Count;
                    return;
                }

                ConfigurationBase currentImpl = pendingImpl.Dequeue();
                DeterministicFAState currentSpec = pendingSpec.Dequeue();
                List<ConfigurationBase> currentPath = paths.Dequeue();

                IEnumerable<ConfigurationBase> nextImpl = currentImpl.MakeOneMove();
                VerificationOutput.Transitions += nextImpl.Count();//.Length;

                //for (int k = 0; k < nextImpl.Length; k++)
                foreach (ConfigurationBase next in nextImpl)
                {
                    //ConfigurationBase next = nextImpl[k];
                    DeterministicFAState nextSpec = currentSpec;
                    if (next.Event != Constants.TAU)
                    {
                        nextSpec = currentSpec.Next(next.Event);

                        if (nextSpec == null)
                        {
                            currentPath.Add(next);
                            VerificationOutput.NoOfStates = Visited.Count;
                            VerificationOutput.CounterExampleTrace = currentPath;
                            VerificationOutput.VerificationResult = VerificationResultType.INVALID;
                            return;
                        }
                    }

                    statestring = nextSpec.GetID() + Constants.SEPARATOR + next.GetID();
                    if (!Visited.ContainsKey(statestring))
                    {
                        Visited.Add(statestring);
                        pendingImpl.Enqueue(next);
                        pendingSpec.Enqueue(nextSpec);
                        List<ConfigurationBase> newPath = new List<ConfigurationBase>(currentPath);
                        newPath.Add(next);
                        paths.Enqueue(newPath);
                    }
                }
            }

            VerificationOutput.VerificationResult = VerificationResultType.VALID;
            VerificationOutput.NoOfStates = Visited.Count;
        }

        public void TraceInclusionCheckDFSAntiChain(Automata spec)
        {
            //The following are for identifying a counterexample trace.             
            Stack<int> depthStack = new Stack<int>(1024);
            depthStack.Push(0);
            List<int> depthList = new List<int>(1024);
            //The above are for identifying a counterexample trace. 
            List<ConfigurationBase> toReturn = new List<ConfigurationBase>();

            Stack<ConfigurationBase> workingImpl = new Stack<ConfigurationBase>(1024);
            Stack<NormalizedFAState> workingSpec = new Stack<NormalizedFAState>(1024);

            NormalizedFAState initialSpec = (new NormalizedFAState(spec.InitialState)).TauReachable();

            workingImpl.Push(InitialStep);
            workingSpec.Push(initialSpec);

            AntiChain antichain = new AntiChain();
            antichain.Add(InitialStep.GetID(), initialSpec.States);

            while (workingImpl.Count > 0)
            {
                if (CancelRequested)
                {
                    VerificationOutput.NoOfStates = antichain.GetNoOfStates();
                    return;
                }

                ConfigurationBase currentImpl = workingImpl.Pop();
                NormalizedFAState currentSpec = workingSpec.Pop();

                //The following are for identifying a counterexample trace. 
                int depth = depthStack.Pop();
                if(depth > 0)
                {
                    while (depthList[depthList.Count - 1] >= depth)
                    {
                        int lastIndex = depthList.Count - 1;
                        depthList.RemoveAt(lastIndex);
                        toReturn.RemoveAt(lastIndex);
                    }
                }
                toReturn.Add(currentImpl);
                depthList.Add(depth);
                //The above are for identifying a counterexample trace.

                if (currentSpec.States.Count == 0)
                {                    
                    VerificationOutput.NoOfStates += antichain.GetNoOfStates();
                    VerificationOutput.CounterExampleTrace = toReturn;
                    VerificationOutput.VerificationResult = VerificationResultType.INVALID;
                    return;
                }

                IEnumerable<ConfigurationBase> implPosts = currentImpl.MakeOneMove();
                VerificationOutput.Transitions += implPosts.Count();

                foreach (ConfigurationBase evt in implPosts)
                {
                    NormalizedFAState specState = currentSpec;
                    if (evt.Event != Constants.TAU)
                    {
                        specState = specState.NextWithTauReachable(evt.Event);
                    }

                    if (!antichain.Add(evt.GetID(), specState.States))
                    {
                        workingImpl.Push(evt);
                        workingSpec.Push(specState);
                        depthStack.Push(depth + 1);
                    }
                }
            }

            VerificationOutput.VerificationResult = VerificationResultType.VALID;
            VerificationOutput.NoOfStates = antichain.GetNoOfStates();
        }

        public void TraceInclusionCheckBFSAntiChain(Automata spec)
        {
            //StringHashTable Visited = new StringHashTable(Ultility.Ultility.MC_INITIAL_SIZE);
            AntiChain antichain = new AntiChain();
            Queue<ConfigurationBase> pendingImpl = new Queue<ConfigurationBase>(1024);
            Queue<NormalizedFAState> pendingSpec = new Queue<NormalizedFAState>(1024);
            Queue<List<ConfigurationBase>> paths = new Queue<List<ConfigurationBase>>(1024);

            //The following are for identifying a counterexample trace. 
            Stack<int> depthStack = new Stack<int>(1024);
            depthStack.Push(0);
            //The above are for identifying a counterexample trace. 

            //implementation initial state
            pendingImpl.Enqueue(InitialStep);
            NormalizedFAState initialSpec = (new NormalizedFAState(spec.InitialState)).TauReachable();
            pendingSpec.Enqueue(initialSpec);

            //Visited.Add(statestring);
            antichain.Add(InitialStep.GetID(), initialSpec.States);
 
            List<ConfigurationBase> path = new List<ConfigurationBase>();
            path.Add(InitialStep);
            paths.Enqueue(path);

            while (pendingImpl.Count > 0)
            {
                if (CancelRequested)
                {
                    VerificationOutput.NoOfStates = antichain.GetNoOfStates();
                    return;
                }

                ConfigurationBase currentImpl = pendingImpl.Dequeue();
                NormalizedFAState currentSpec = pendingSpec.Dequeue();
                List<ConfigurationBase> currentPath = paths.Dequeue();

                IEnumerable<ConfigurationBase> nextImpl = currentImpl.MakeOneMove();
                VerificationOutput.Transitions += nextImpl.Count();

                //for (int k = 0; k < nextImpl.Length; k++)
                foreach (ConfigurationBase next in nextImpl)
                {
                    //ConfigurationBase next = nextImpl[k];

                    NormalizedFAState nextSpec = currentSpec;
                    if (next.Event != Constants.TAU)
                    {
                        nextSpec = currentSpec.NextWithTauReachable(next.Event);

                        if (nextSpec.States.Count == 0)
                        {
                            currentPath.Add(next);
                            VerificationOutput.NoOfStates = antichain.GetNoOfStates();
                            VerificationOutput.CounterExampleTrace = currentPath;
                            VerificationOutput.VerificationResult = VerificationResultType.INVALID;
                            return;
                        }
                    }

                    if (!antichain.Add(next.GetID(), nextSpec.States))
                    {
                        pendingImpl.Enqueue(next);
                        pendingSpec.Enqueue(nextSpec);
                        List<ConfigurationBase> newPath = new List<ConfigurationBase>(currentPath);
                        newPath.Add(next);
                        paths.Enqueue(newPath);
                    }
                }
            }

            VerificationOutput.VerificationResult = VerificationResultType.VALID;
            VerificationOutput.NoOfStates = antichain.GetNoOfStates();
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
                    this.VerificationOutput.CounterExampleTrace = ConcreteCounterExampleTrace;
                    return false;
                }
                else
                {
                    ConfigurationBase abstractStep = this.VerificationOutput.CounterExampleTrace[depth + 1];

                    IEnumerable<ConfigurationBase> steps = current.MakeOneMove(abstractStep.Event);

                    foreach (ConfigurationBase step in steps)
                    //for (int j = 0; j < steps.Length; j++)
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

            if (this.VerificationOutput.VerificationResult == VerificationResultType.INVALID)
            {
                sb.AppendLine("The Assertion (" + ToString() + ") is NOT valid.");
                sb.AppendLine("The following trace is allowed in " + StartingProcess + ", but not in " + SpecProcess + ".");
                VerificationOutput.GetCounterxampleString(sb);
            }
            else
            {
                sb.AppendLine("The Assertion (" + ToString() + ") is VALID.");
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

    public sealed class AntiChain 
    {
        Dictionary<string, List<HashSet<FAState>>> Pairs = new Dictionary<string, List<HashSet<FAState>>>(Ultility.Ultility.MC_INITIAL_SIZE);

        /// <summary>
        /// return true if and only if the pair can be skipped. If not true, add the new set.
        /// </summary>
        /// <param name="implStateID"> </param>
        /// <param name="specState"> </param>
        /// <returns></returns>
        public bool Add(string implStateID, HashSet<FAState> specState)
        {
            List<HashSet<FAState>> items;
            if (Pairs.TryGetValue(implStateID, out items))
            {
                foreach (HashSet<FAState> item in items)
                {
                    if (item.IsSubsetOf(specState))
                    {
                        return true;
                    }
                }

                //the following adds the set
                List<HashSet<FAState>> newList = new List<HashSet<FAState>>();
                newList.Add(specState);

                foreach (HashSet<FAState> item in items)
                {
                    if (!specState.IsSubsetOf(item))
                    {
                        newList.Add(item);
                    }
                }

                Pairs[implStateID] = newList;
                return false;
                //the above adds the set
            }
            else {
                List<HashSet<FAState>> newList = new List<HashSet<FAState>>();
                newList.Add(specState);
                Pairs.Add(implStateID, newList);
                return false;
            }
        }

        public int GetNoOfStates()
        {
            int toReturn = 0;

            foreach (List<HashSet<FAState>> set in Pairs.Values)
            {
                toReturn += set.Count;
            }

            return toReturn;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            foreach (KeyValuePair<string, List<HashSet<FAState>>> item in Pairs)
            {
                sb.Append("(" + item.Key + ":");

                foreach (HashSet<FAState> state in item.Value)
                {
                    sb.Append("(" + state.ToString() + ")");
                }
                sb.AppendLine(")");
            }

            return sb.ToString();
        }
    }
}