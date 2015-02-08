using System.Collections.Generic;
using System.Linq;
using System.Text;
using PAT.Common.Classes.DataStructure;
using PAT.Common.Classes.ModuleInterface;
using PAT.Common.Classes.Ultility;

namespace PAT.Common.Classes.SemanticModels.LTS.Assertion
{
    public abstract class AssertionRefinementF : AssertionRefinement
    {
        public RefinementCheckingResultType FailureType; //used to record the event name for counterexample display.

        public override string ToString()
        {
            return StartingProcess + " refines <F> " + SpecProcess;
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
            List<string> engines = new List<string>();
            engines.Add(Constants.ENGINE_F_REFINEMENT_ANTICHAIN_DEPTH_FIRST_SEARCH);
            engines.Add(Constants.ENGINE_F_REFINEMENT_ANTICHAIN_BREADTH_FIRST_SEARCH); 
            engines.Add(Constants.ENGINE_F_REFINEMENT_DEPTH_FIRST_SEARCH);
            engines.Add(Constants.ENGINE_F_REFINEMENT_BREADTH_FIRST_SEARCH);
            ModelCheckingOptions.AddAddimissibleBehavior(Constants.COMPLETE_BEHAVIOR, engines);
        }

        private static Automata BuildAutomataWithRefusals(ConfigurationBase InitSpecStep)
        {
            Dictionary<string, FAState> visited = new Dictionary<string, FAState>();
            Stack<ConfigurationBase> working = new Stack<ConfigurationBase>(1024);
            working.Push(InitSpecStep);
            Automata auto = new Automata();
            FAState init = auto.AddState();
            auto.SetInitialState(init);
            visited.Add(InitSpecStep.GetID(), init);

            do
            {
                ConfigurationBase current = working.Pop();
                FAState currentState = visited[current.GetID()];

                IEnumerable<ConfigurationBase> list = current.MakeOneMove();
                List<string> negatedRefusal = new List<string>();
                bool hasTau = false;

                //for (int i = 0; i < list.Length; i++)
                foreach (ConfigurationBase step in list)
                {
                    //ConfigurationBase step = list[i];

                    if (step.Event == Constants.TAU)
                    {
                        hasTau = true;
                    }
                    else
                    {
                        negatedRefusal.Add(step.Event);
                    }

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

                if (hasTau)
                {
                    currentState.NegatedRefusal = null;
                }
                else
                {
                    currentState.NegatedRefusal = negatedRefusal;
                }
            }
            while (working.Count > 0);

            return auto;
        }

        /// <summary>
        /// Run the verification and get the result.
        /// </summary>
        /// <returns></returns>
        public override void RunVerification()
        {
            if (SelectedEngineName == Constants.ENGINE_F_REFINEMENT_ANTICHAIN_DEPTH_FIRST_SEARCH)
            {
                Automata spec = BuildAutomataWithRefusals(InitSpecStep);
                FailuresInclusionCheckDFSAntiChain(spec);
            }
            else
            {
                if (SelectedEngineName == Constants.ENGINE_F_REFINEMENT_ANTICHAIN_BREADTH_FIRST_SEARCH)
                {
                    Automata spec = BuildAutomataWithRefusals(InitSpecStep);
                    FailuresInclusionCheckBFSAntiChain(spec);
                }
                else
                {
                    DeterministicAutomata auto = BuildDeterministicAutomataWithRefusals(InitSpecStep);

                    if (SelectedEngineName == Constants.ENGINE_F_REFINEMENT_DEPTH_FIRST_SEARCH)
                    {
                        FailuresInclusionCheckDFS(auto);
                    }
                    else
                    {
                        FailuresInclusionCheckBFS(auto);
                    }
                }
            }
        }

        private void FailuresInclusionCheckDFSAntiChain(Automata spec)
        {
            Stack<ConfigurationBase> pendingImpl = new Stack<ConfigurationBase>(1024);
            Stack<NormalizedFAState> pendingSpec = new Stack<NormalizedFAState>(1024);
            List<ConfigurationBase> toReturn = new List<ConfigurationBase>();

            //The following are for identifying a counterexample trace. 
            Stack<int> depthStack = new Stack<int>(1000);
            depthStack.Push(0);
            List<int> depthList = new List<int>(1000);
            //The above are for identifying a counterexample trace. 

            NormalizedFAState initialSpec = (new NormalizedFAState(spec.InitialState)).TauReachable();

            //implementation initial state
            pendingImpl.Push(InitialStep);

            //specification initial state
            pendingSpec.Push(initialSpec);

            AntiChain antichain = new AntiChain();
            antichain.Add(InitialStep.GetID(), initialSpec.States);

            while (pendingImpl.Count > 0)
            {
                if (CancelRequested)
                {
                    VerificationOutput.NoOfStates = antichain.GetNoOfStates();
                    return;
                }

                ConfigurationBase currentImpl = pendingImpl.Pop();
                NormalizedFAState currentSpec = pendingSpec.Pop();

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
                //The above are for identifying a counterexample trace. 

                //if the implememtation state is stable, then check for failures inclusion 
                IEnumerable<ConfigurationBase> nextImpl = currentImpl.MakeOneMove();
                VerificationOutput.Transitions += nextImpl.Count();
                List<string> negatedRefusal = new List<string>();
                bool hasTau = false;

                //for (int k = 0; k < nextImpl.Length; k++)
                foreach (ConfigurationBase next in nextImpl)
                {
                    //ConfigurationBase next = nextImpl[k];

                    NormalizedFAState nextSpec = currentSpec;

                    if (next.Event != Constants.TAU)
                    {
                        negatedRefusal.Add(next.Event);
                        nextSpec = currentSpec.NextWithTauReachable(next.Event);
                        //If the specification has no corresponding state, then it implies that the trace is allowed by the 
                        //implementation but not the specification -- which means trace-refinement is failed.
                        if (nextSpec.States.Count == 0)
                        {
                            toReturn.Add(next);
                            VerificationOutput.NoOfStates = antichain.GetNoOfStates();
                            VerificationOutput.CounterExampleTrace = toReturn;
                            VerificationOutput.VerificationResult = VerificationResultType.INVALID;
                            FailureType = RefinementCheckingResultType.TraceRefinementFailure;
                            return;
                        }
                    }
                    else
                    {
                        hasTau = true;
                    }

                    if (!antichain.Add(next.GetID(), nextSpec.States))
                    {
                        pendingImpl.Push(next);
                        pendingSpec.Push(nextSpec);
                        depthStack.Push(depth + 1);
                    }
                }

                if (!hasTau)
                {
                    if (!RefusalContainment(negatedRefusal, currentSpec.GetFailuresNegate()))
                    {
                        VerificationOutput.NoOfStates = antichain.GetNoOfStates();
                        VerificationOutput.CounterExampleTrace = toReturn;
                        VerificationOutput.VerificationResult = VerificationResultType.INVALID;
                        FailureType = RefinementCheckingResultType.FailuresRefinementFailure;
                        return;
                    }
                }
            }

            VerificationOutput.NoOfStates = antichain.GetNoOfStates();
            VerificationOutput.VerificationResult = VerificationResultType.VALID;
            FailureType = RefinementCheckingResultType.Valid;
        }

        private void FailuresInclusionCheckBFSAntiChain(Automata spec)
        {
            Queue<ConfigurationBase> pendingImpl = new Queue<ConfigurationBase>(1000);
            Queue<NormalizedFAState> pendingSpec = new Queue<NormalizedFAState>(1000);
            Queue<List<ConfigurationBase>> paths = new Queue<List<ConfigurationBase>>(1024);

            NormalizedFAState initialSpec = (new NormalizedFAState(spec.InitialState)).TauReachable();
            //implementation initial state
            pendingImpl.Enqueue(InitialStep);

            //specification initial state
            pendingSpec.Enqueue(initialSpec);

            List<ConfigurationBase> path = new List<ConfigurationBase>();
            path.Add(InitialStep);
            paths.Enqueue(path);

            AntiChain antichain = new AntiChain();
            antichain.Add(InitialStep.GetID(), initialSpec.States);

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
                List<string> negatedRefusal = new List<string>();
                bool hasTau = false;

                //for (int k = 0; k < nextImpl.Length; k++)
                foreach (ConfigurationBase next in nextImpl)
                {
                    //ConfigurationBase next = nextImpl[k];

                    NormalizedFAState nextSpec = currentSpec;
                    if (next.Event != Constants.TAU)
                    {
                        negatedRefusal.Add(next.Event);
                        nextSpec = currentSpec.NextWithTauReachable(next.Event);

                        if (nextSpec.States.Count == 0)
                        {
                            currentPath.Add(next);
                            VerificationOutput.NoOfStates = antichain.GetNoOfStates();
                            VerificationOutput.CounterExampleTrace = currentPath;
                            VerificationOutput.VerificationResult = VerificationResultType.INVALID;
                            FailureType = RefinementCheckingResultType.TraceRefinementFailure;
                            return;
                        }
                    }
                    else
                    {
                        hasTau = true;
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

                //if the implememtation state is stable, then check for failures inclusion 
                if (!hasTau)
                {
                    //enabledS is empty if and only if the spec state is divergent.
                    if (!RefusalContainment(negatedRefusal, currentSpec.GetFailuresNegate()))
                    {
                        //return toReturn;
                        VerificationOutput.NoOfStates = antichain.GetNoOfStates();
                        VerificationOutput.CounterExampleTrace = currentPath;
                        VerificationOutput.VerificationResult = VerificationResultType.INVALID;
                        FailureType = RefinementCheckingResultType.FailuresRefinementFailure;
                        return;
                    }
                }
            }

            VerificationOutput.NoOfStates = antichain.GetNoOfStates();
            VerificationOutput.VerificationResult = VerificationResultType.VALID;
            FailureType = RefinementCheckingResultType.Valid;
        }

        public static DeterministicAutomata BuildDeterministicAutomataWithRefusals(ConfigurationBase initStep)
        {
            return BuildAutomataWithRefusals(initStep).DeterminizeWithRefusals();
        }

        public void FailuresInclusionCheckDFS(DeterministicAutomata spec)
        {
            StringHashTable Visited = new StringHashTable(Ultility.Ultility.MC_INITIAL_SIZE);
            Stack<ConfigurationBase> pendingImpl = new Stack<ConfigurationBase>(1000);
            Stack<DeterministicFAState> pendingSpec = new Stack<DeterministicFAState>(1000);
            List<ConfigurationBase> toReturn = new List<ConfigurationBase>();

            //The following are for identifying a counterexample trace. 
            Stack<int> depthStack = new Stack<int>(1000);
            depthStack.Push(0);
            List<int> depthList = new List<int>(1000);
            //The above are for identifying a counterexample trace. 

            //implementation initial state
            pendingImpl.Push(InitialStep);

            //specification initial state
            pendingSpec.Push(spec.InitialState);

            string statestring = InitialStep.GetID() + Constants.SEPARATOR + spec.InitialState.GetID();
            Visited.Add(statestring);

            while (pendingImpl.Count > 0)
            {
                if (CancelRequested)
                {
                    VerificationOutput.NoOfStates = Visited.Count;
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
                //The above are for identifying a counterexample trace. 

                //if the implememtation state is stable, then check for failures inclusion 
                IEnumerable<ConfigurationBase> nextImpl = currentImpl.MakeOneMove();
                VerificationOutput.Transitions += nextImpl.Count();
                List<string> negatedRefusal = new List<string>();
                bool hasTau = false;

                //for (int k = 0; k < nextImpl.Length; k++)
                foreach (ConfigurationBase next in nextImpl)
                {
                    //ConfigurationBase next = nextImpl[k];

                    DeterministicFAState nextSpec = currentSpec;

                    if (next.Event != Constants.TAU)
                    {
                        negatedRefusal.Add(next.Event);
                        nextSpec = currentSpec.Next(next.Event);
                        //If the specification has no corresponding state, then it implies that the trace is allowed by the 
                        //implementation but not the specification -- which means trace-refinement is failed.
                        if (nextSpec == null)
                        {
                            toReturn.Add(next);
                            VerificationOutput.NoOfStates = Visited.Count;
                            VerificationOutput.CounterExampleTrace = toReturn;
                            VerificationOutput.VerificationResult = VerificationResultType.INVALID;
                            FailureType = RefinementCheckingResultType.TraceRefinementFailure;
                            return;
                        }
                    }
                    else
                    {
                        hasTau = true;
                    }

                    statestring = next.GetID() + Constants.SEPARATOR + nextSpec.GetID();

                    if (!Visited.ContainsKey(statestring))
                    {
                        Visited.Add(statestring);
                        pendingImpl.Push(next);
                        pendingSpec.Push(nextSpec);
                        depthStack.Push(depth + 1);
                    }
                }

                if (!hasTau)
                {
                    //enabledS is empty if and only if the spec state is divergent.
                    if (!RefusalContainment(negatedRefusal, currentSpec.NegatedRefusals))
                    {
                        VerificationOutput.NoOfStates = Visited.Count;
                        VerificationOutput.CounterExampleTrace = toReturn;
                        VerificationOutput.VerificationResult = VerificationResultType.INVALID;
                        FailureType = RefinementCheckingResultType.FailuresRefinementFailure;
                        return;
                    }
                }
            }

            VerificationOutput.NoOfStates = Visited.Count;
            VerificationOutput.VerificationResult = VerificationResultType.VALID;
            FailureType = RefinementCheckingResultType.Valid;
        }

        public void FailuresInclusionCheckBFS(DeterministicAutomata spec)
        {
            StringHashTable Visited = new StringHashTable(Ultility.Ultility.MC_INITIAL_SIZE);
            Queue<ConfigurationBase> pendingImpl = new Queue<ConfigurationBase>(1000);
            Queue<DeterministicFAState> pendingSpec = new Queue<DeterministicFAState>(1000);
            Queue<List<ConfigurationBase>> paths = new Queue<List<ConfigurationBase>>(1024);

            //implementation initial state
            pendingImpl.Enqueue(InitialStep);

            //specification initial state
            pendingSpec.Enqueue(spec.InitialState);

            string statestring = InitialStep.GetID() + Constants.SEPARATOR + spec.InitialState.GetID();
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
                VerificationOutput.Transitions += nextImpl.Count();
                List<string> negatedRefusal = new List<string>();
                bool hasTau = false;

                //for (int k = 0; k < nextImpl.Length; k++)
                foreach (ConfigurationBase next in nextImpl)
                {
                    //ConfigurationBase next = nextImpl[k];

                    DeterministicFAState nextSpec = currentSpec;
                    if (next.Event != Constants.TAU)
                    {
                        negatedRefusal.Add(next.Event);
                        //nextSpec = currentSpec.Post[next.Event];
                        nextSpec = currentSpec.Next(next.Event);

                        if (nextSpec == null)
                        {
                            currentPath.Add(next);
                            VerificationOutput.NoOfStates = Visited.Count;
                            VerificationOutput.CounterExampleTrace = currentPath;
                            VerificationOutput.VerificationResult = VerificationResultType.INVALID;
                            FailureType = RefinementCheckingResultType.TraceRefinementFailure;
                            return;
                        }

                    }
                    else
                    {
                        hasTau = true;
                    }

                    statestring = next.GetID() + Constants.SEPARATOR + nextSpec.GetID();

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

                //if the implememtation state is stable, then check for failures inclusion 
                if (!hasTau)
                {
                    //enabledS is empty if and only if the spec state is divergent.
                    if (!RefusalContainment(negatedRefusal, currentSpec.NegatedRefusals))
                    {
                        //return toReturn;
                        VerificationOutput.NoOfStates = Visited.Count;
                        VerificationOutput.CounterExampleTrace = currentPath;
                        VerificationOutput.VerificationResult = VerificationResultType.INVALID;
                        FailureType = RefinementCheckingResultType.FailuresRefinementFailure;
                        return;
                    }
                }
            }

            VerificationOutput.NoOfStates = Visited.Count;
            VerificationOutput.VerificationResult = VerificationResultType.VALID;
            FailureType = RefinementCheckingResultType.Valid;
        }

        //there must exists one element in enabledS such that it is a subset of enabledEvents 
        protected bool RefusalContainment(List<string> enabledEvents, List<List<string>> enabledS)
        {
            foreach (List<string> s in enabledS)
            {
                bool isSubset = true;

                foreach (string s1 in s)
                {
                    if (!enabledEvents.Contains(s1))
                    {
                        isSubset = false;
                        break;
                    }
                }

                if (isSubset)
                {
                    return true;
                }
            }

            return false;
        }

        public override string GetResultString()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine(Constants.VERFICATION_RESULT_STRING);

            if (VerificationOutput.VerificationResult == VerificationResultType.INVALID)
            {
                sb.AppendLine("The Assertion (" + ToString() + ") is NOT valid.");
                if (FailureType != RefinementCheckingResultType.FailuresRefinementFailure)
                {
                    sb.AppendLine("The following trace is allowed in " + StartingProcess + ", but not in " + SpecProcess + ".");
                }
                else
                {
                    sb.AppendLine("After the following trace: failures refinement checking failed.");                    
                }
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
}