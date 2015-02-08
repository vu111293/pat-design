using System.Collections.Generic;
using System.Linq;
using PAT.Common.Classes.DataStructure;
using System.Text;
using PAT.Common.Classes.ModuleInterface;
using PAT.Common.Classes.Ultility;

namespace PAT.Common.Classes.SemanticModels.LTS.Assertion
{
    public abstract class AssertionRefinementFD : AssertionRefinementF
    {
        public override string ToString()
        {
            return StartingProcess + " refines <FD> " + SpecProcess;
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
            engines.Add(Constants.ENGINE_FD_REFINEMENT_ANTICHAIN_DEPTH_FIRST_SEARCH);
            engines.Add(Constants.ENGINE_FD_REFINEMENT_ANTICHAIN_BREADTH_FIRST_SEARCH);
            engines.Add(Constants.ENGINE_FD_REFINEMENT_DEPTH_FIRST_SEARCH);
            engines.Add(Constants.ENGINE_FD_REFINEMENT_BREADTH_FIRST_SEARCH);
            ModelCheckingOptions.AddAddimissibleBehavior(Constants.COMPLETE_BEHAVIOR, engines);
        }
        
        /// <summary>
        /// Run the verification and get the result.
        /// </summary>
        /// <returns></returns>
        public override void RunVerification()
        {
            if (SelectedEngineName == Constants.ENGINE_FD_REFINEMENT_ANTICHAIN_DEPTH_FIRST_SEARCH)
            {
                Automata spec = BuildAutomataWithRefusalsAndDiv(InitSpecStep);
                FailuresDivergenceInclusionCheckDFSAntichain(spec);
            }
            else
            {
                if (SelectedEngineName == Constants.ENGINE_FD_REFINEMENT_ANTICHAIN_BREADTH_FIRST_SEARCH)
                {
                    Automata spec = BuildAutomataWithRefusalsAndDiv(InitSpecStep);
                    FailuresDivergenceInclusionCheckBFSAntichain(spec);
                }
                else
                {
                    DeterministicAutomata auto = BuildDeterministicAutomataWithRefusalsAndDiv(InitSpecStep);
                    if (SelectedEngineName == Constants.ENGINE_FD_REFINEMENT_DEPTH_FIRST_SEARCH)
                    {
                        FailuresDivergenceInclusionCheckDFS(auto);
                    }
                    else
                    {
                        FailuresDivergenceInclusionCheckBFS(auto);
                    }
                }
            }
        }

        public static DeterministicAutomata BuildDeterministicAutomataWithRefusalsAndDiv(ConfigurationBase initStep)
        {
            return BuildAutomataWithRefusalsAndDiv(initStep).DeterminizeWithRefusalsAndDiv();
        }

        private static Automata BuildAutomataWithRefusalsAndDiv(ConfigurationBase InitSpecStep)
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

                if (current.IsDivergent())
                {
                    currentState.IsDiv = true;
                }
                else
                {
                    IEnumerable<ConfigurationBase> list = current.MakeOneMove();
                    List<string> negateRefusal = new List<string>();
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
                            negateRefusal.Add(step.Event);
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
                        currentState.NegatedRefusal = negateRefusal;
                    }
                }
            }
            while (working.Count > 0);

            return auto;
        }

        private void FailuresDivergenceInclusionCheckDFSAntichain(Automata spec)
        {
            List<ConfigurationBase> toReturn = new List<ConfigurationBase>();

            Stack<ConfigurationBase> pendingImpl = new Stack<ConfigurationBase>(1000);
            Stack<NormalizedFAState> pendingSpec = new Stack<NormalizedFAState>(1000);

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

                if (currentSpec.IsDiv())
                {
                    VerificationOutput.NoOfStates = antichain.GetNoOfStates();
                    VerificationOutput.VerificationResult = VerificationResultType.VALID;
                    FailureType = RefinementCheckingResultType.Valid;
                    return;
                }

                bool implIsDiv = currentImpl.IsDivergent();

                if (implIsDiv)
                {
                    VerificationOutput.NoOfStates = antichain.GetNoOfStates();
                    VerificationOutput.CounterExampleTrace = toReturn;
                    VerificationOutput.VerificationResult = VerificationResultType.INVALID;
                    FailureType = RefinementCheckingResultType.DivCheckingFailure;
                    return;
                }

                IEnumerable<ConfigurationBase> nextImpl = currentImpl.MakeOneMove();
                VerificationOutput.Transitions += nextImpl.Count();
                List<string> implRefusal = new List<string>();
                bool hasTau = false;

                //for (int i = 0; i < nextImpl.Length; i++)
                foreach (ConfigurationBase next in nextImpl)
                {
                    NormalizedFAState nextSpec = currentSpec;

                    if (next.Event != Constants.TAU)
                    {
                        implRefusal.Add(next.Event);
                        //nextSpec = currentSpec.Post[nextImpl[i].Event];
                        nextSpec = currentSpec.NextWithTauReachable(next.Event);


                        //The following checks if a violation is found. 
                        //First, check for trace refinement, which is necessary for all other refinement as well.
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

                //if the implememtation state is stable, then check for failures inclusion 
                if (!hasTau)
                {
                    //enabledS is empty if and only if the spec state is divergent.
                    if (!RefusalContainment(implRefusal, currentSpec.GetFailuresNegate()))
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

        private void FailuresDivergenceInclusionCheckBFSAntichain(Automata spec)
        {
            Stack<ConfigurationBase> pendingImpl = new Stack<ConfigurationBase>(1000);
            Stack<NormalizedFAState> pendingSpec = new Stack<NormalizedFAState>(1000);
            Queue<List<ConfigurationBase>> paths = new Queue<List<ConfigurationBase>>(1024);

            NormalizedFAState initialSpec = (new NormalizedFAState(spec.InitialState)).TauReachable();
            //implementation initial state
            pendingImpl.Push(InitialStep);

            //specification initial state
            pendingSpec.Push(initialSpec);

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

                ConfigurationBase currentImpl = pendingImpl.Pop();
                NormalizedFAState currentSpec = pendingSpec.Pop();
                List<ConfigurationBase> currentPath = paths.Dequeue();

                if (currentSpec.IsDiv())
                {
                    VerificationOutput.NoOfStates = antichain.GetNoOfStates();
                    VerificationOutput.VerificationResult = VerificationResultType.VALID;
                    FailureType = RefinementCheckingResultType.Valid;
                    return;
                }

                bool implIsDiv = currentImpl.IsDivergent();

                if (implIsDiv)
                {
                    VerificationOutput.NoOfStates = antichain.GetNoOfStates();
                    VerificationOutput.VerificationResult = VerificationResultType.INVALID;
                    VerificationOutput.CounterExampleTrace = currentPath;
                    FailureType = RefinementCheckingResultType.DivCheckingFailure;
                    return;
                }

                IEnumerable<ConfigurationBase> nextImpl = currentImpl.MakeOneMove();
                VerificationOutput.Transitions += nextImpl.Count();

                List<string> negatedRefusal = new List<string>();
                bool hasTau = false;

                //for (int i = 0; i < nextImpl.Length; i++)
                foreach (ConfigurationBase next in nextImpl)
                {
                    NormalizedFAState nextSpec = currentSpec;

                    if (next.Event != Constants.TAU)
                    {
                        negatedRefusal.Add(next.Event);
                        //nextSpec = currentSpec.Post[nextImpl[i].Event];
                        nextSpec = currentSpec.NextWithTauReachable(next.Event);


                        //The following checks if a violation is found. 
                        //First, check for trace refinement, which is necessary for all other refinement as well.
                        if (nextSpec.States.Count == 0)
                        {
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
                        pendingImpl.Push(next);
                        pendingSpec.Push(nextSpec);
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

        public void FailuresDivergenceInclusionCheckBFS(DeterministicAutomata spec)
        {
            StringHashTable Visited = new StringHashTable(Ultility.Ultility.MC_INITIAL_SIZE);
            Stack<ConfigurationBase> pendingImpl = new Stack<ConfigurationBase>(1000);
            Stack<DeterministicFAState> pendingSpec = new Stack<DeterministicFAState>(1000);
            Queue<List<ConfigurationBase>> paths = new Queue<List<ConfigurationBase>>(1024);

            //implementation initial state
            pendingImpl.Push(InitialStep);

            //specification initial state
            pendingSpec.Push(spec.InitialState);

            string statestring = InitialStep.GetID() + Constants.TAU + spec.InitialState.GetID();
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

                ConfigurationBase currentImpl = pendingImpl.Pop();
                DeterministicFAState currentSpec = pendingSpec.Pop();
                List<ConfigurationBase> currentPath = paths.Dequeue();

                if (currentSpec.IsDivergent)
                {
                    VerificationOutput.NoOfStates = Visited.Count;
                    VerificationOutput.VerificationResult = VerificationResultType.VALID;
                    FailureType = RefinementCheckingResultType.Valid;
                    return;
                }

                bool implIsDiv = currentImpl.IsDivergent();

                if (implIsDiv)
                {
                    VerificationOutput.NoOfStates = Visited.Count;
                    VerificationOutput.VerificationResult = VerificationResultType.INVALID;
                    VerificationOutput.CounterExampleTrace = currentPath;
                    FailureType = RefinementCheckingResultType.DivCheckingFailure;
                    return;
                }

                IEnumerable<ConfigurationBase> nextImpl = currentImpl.MakeOneMove();
                VerificationOutput.Transitions += nextImpl.Count();

                List<string> negatedRefusal = new List<string>();
                bool hasTau = false;

                //for (int i = 0; i < nextImpl.Length; i++)
                foreach (ConfigurationBase next in nextImpl)
                {
                    DeterministicFAState nextSpec = currentSpec;

                    if (next.Event != Constants.TAU)
                    {
                        negatedRefusal.Add(next.Event);
                        //nextSpec = currentSpec.Post[nextImpl[i].Event];
                        nextSpec = currentSpec.Next(next.Event);


                        //The following checks if a violation is found. 
                        //First, check for trace refinement, which is necessary for all other refinement as well.
                        if (nextSpec == null)
                        {
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
                        pendingImpl.Push(next);
                        pendingSpec.Push(nextSpec);
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

        public void FailuresDivergenceInclusionCheckDFS(DeterministicAutomata spec)
        {
            StringHashTable Visited = new StringHashTable(Ultility.Ultility.MC_INITIAL_SIZE);
            List<ConfigurationBase> toReturn = new List<ConfigurationBase>();

            Stack<ConfigurationBase> pendingImpl = new Stack<ConfigurationBase>(1000);
            Stack<DeterministicFAState> pendingSpec = new Stack<DeterministicFAState>(1000);

            //The following are for identifying a counterexample trace. 
            Stack<int> depthStack = new Stack<int>(1000);
            depthStack.Push(0);
            List<int> depthList = new List<int>(1000);
            //The above are for identifying a counterexample trace. 

            //implementation initial state
            pendingImpl.Push(InitialStep);

            //specification initial state
            pendingSpec.Push(spec.InitialState);

            string statestring = InitialStep.GetID() + Constants.TAU + spec.InitialState.GetID();
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

                if (currentSpec.IsDivergent)
                {
                    VerificationOutput.NoOfStates = Visited.Count;
                    VerificationOutput.VerificationResult = VerificationResultType.VALID;
                    FailureType = RefinementCheckingResultType.Valid;
                    return;
                }

                bool implIsDiv = currentImpl.IsDivergent();

                if (implIsDiv)
                {
                    VerificationOutput.NoOfStates = Visited.Count;
                    VerificationOutput.CounterExampleTrace = toReturn;
                    VerificationOutput.VerificationResult = VerificationResultType.INVALID;
                    FailureType = RefinementCheckingResultType.DivCheckingFailure;
                    return;
                }

                IEnumerable<ConfigurationBase> nextImpl = currentImpl.MakeOneMove();
                VerificationOutput.Transitions += nextImpl.Count();
                List<string> implRefusal = new List<string>();
                bool hasTau = false;

                //for (int i = 0; i < nextImpl.Length; i++)
                foreach (ConfigurationBase next in nextImpl)
                {
                    DeterministicFAState nextSpec = currentSpec;

                    if (next.Event != Constants.TAU)
                    {
                        implRefusal.Add(next.Event);
                        nextSpec = currentSpec.Next(next.Event);

                        //The following checks if a violation is found. 
                        //First, check for trace refinement, which is necessary for all other refinement as well.
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

                //if the implememtation state is stable, then check for failures inclusion 
                if (!hasTau)
                {
                    //enabledS is empty if and only if the spec state is divergent.
                    if (!RefusalContainment(implRefusal, currentSpec.NegatedRefusals))
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

        public override string GetResultString()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine(Constants.VERFICATION_RESULT_STRING);

            if (VerificationOutput.VerificationResult == VerificationResultType.INVALID)
            {
                sb.AppendLine("The Assertion (" + ToString() + ") is NOT valid.");

                if (FailureType == RefinementCheckingResultType.DivCheckingFailure)
                {
                    sb.AppendLine("The following trace leads divergence in " + StartingProcess + ", but not in " + SpecProcess + ".");
                }
                else if (FailureType == RefinementCheckingResultType.TraceRefinementFailure)
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