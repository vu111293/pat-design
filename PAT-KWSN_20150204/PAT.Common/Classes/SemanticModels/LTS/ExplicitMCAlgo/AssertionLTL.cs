using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using PAT.Common.Classes.BA;
using PAT.Common.Classes.DataStructure;
using PAT.Common.Classes.ModuleInterface;
using PAT.Common.Classes.Ultility;

namespace PAT.Common.Classes.SemanticModels.LTS.Assertion
{
    public abstract partial class AssertionLTL : AssertionBase
    {
        #region "Fields"

        public BuchiAutomata BA;
        public BuchiAutomata negationLTLBuchi;

        public virtual void SeteBAs(BuchiAutomata ba, BuchiAutomata positiveBA)
        {
            negationLTLBuchi = ba;

            //this is a safety 
            if (!LivenessChecking.isLiveness(positiveBA))
            {
                //AssertType = AssertionType.LTLSafety;
                IsSafety = true;
                BA = positiveBA;
            }
            else
            {
                BA = ba;
            }
        }

        public bool hasFairness;
        protected bool IsNegateLiveness;
        public bool IsSafety;
        public FairnessType FairnessType;
        
        //assertion related variables
        public string LTLString;

        #endregion

        #region "Basic Methods"

        protected AssertionLTL(string ltl)
        {
            LTLString = ltl;
            //AssertType = AssertionType.LTL;
        }

        /// <summary>
        /// Assertion Initialization to create the initial step based on the concrete types.
        /// This method shall be invoked after the parsing immediately to instanciate the initial step
        /// </summary>
        /// <param name="spec">The concrete specification of the module</param>
        public override void Initialize(SpecificationBase spec)
        {
            //initialize model checking options
            ModelCheckingOptions = new ModelCheckingOptions();
            List<string> LTLEngine = new List<string>();

            if(IsSafety)
            {
                LTLEngine.Add(Constants.ENGINE_DEPTH_FIRST_SEARCH);
                LTLEngine.Add(Constants.ENGINE_BREADTH_FIRST_SEARCH);
                ModelCheckingOptions.AddAddimissibleBehavior(Constants.COMPLETE_BEHAVIOR, LTLEngine);
            }
            else
            {
                LTLEngine.Add(Constants.ENGINE_SCC_BASED_SEARCH);
#if DEBUG
                LTLEngine.Add(Constants.ENGINE_SCC_BASED_SEARCH_IMPROVED);
                LTLEngine.Add(Constants.ENGINE_SCC_BASED_SEARCH_MULTICORE);
#endif

                ModelCheckingOptions.AddAddimissibleBehavior(Constants.COMPLETE_BEHAVIOR, LTLEngine);

                LTLEngine = new List<string>();
                LTLEngine.Add(Constants.ENGINE_SCC_BASED_SEARCH);
#if DEBUG
                LTLEngine.Add(Constants.ENGINE_SCC_BASED_SEARCH_MULTICORE);
#endif
                ModelCheckingOptions.AddAddimissibleBehavior(Constants.BEHAVIOR_EVENT_LEVEL_WEAK_FAIRNESS, LTLEngine);

                LTLEngine = new List<string>();
                LTLEngine.Add(Constants.ENGINE_SCC_BASED_SEARCH);
#if DEBUG
                LTLEngine.Add(Constants.ENGINE_SCC_BASED_SEARCH_MULTICORE);
#endif
                ModelCheckingOptions.AddAddimissibleBehavior(Constants.BEHAVIOR_EVENT_LEVEL_STRONG_FAIRNESS, LTLEngine);

                if (IsProcessLevelFairnessApplicable())
                {
                    LTLEngine = new List<string>();
                    LTLEngine.Add(Constants.ENGINE_SCC_BASED_SEARCH);
#if DEBUG
                    LTLEngine.Add(Constants.ENGINE_SCC_BASED_SEARCH_MULTICORE);
#endif
                    ModelCheckingOptions.AddAddimissibleBehavior(Constants.BEHAVIOR_PROCESS_LEVEL_WEAK_FAIRNESS, LTLEngine);

                    LTLEngine = new List<string>();
                    LTLEngine.Add(Constants.ENGINE_SCC_BASED_SEARCH);
#if DEBUG
                    LTLEngine.Add(Constants.ENGINE_SCC_BASED_SEARCH_MULTICORE);
#endif
                    ModelCheckingOptions.AddAddimissibleBehavior(Constants.BEHAVIOR_PROCESS_LEVEL_STRONG_FAIRNESS, LTLEngine);                    
                }

                LTLEngine = new List<string>();
                LTLEngine.Add(Constants.ENGINE_SCC_BASED_SEARCH);
#if DEBUG
                LTLEngine.Add(Constants.ENGINE_SCC_BASED_SEARCH_MULTICORE);
#endif
                ModelCheckingOptions.AddAddimissibleBehavior(Constants.BEHAVIOR_GLOBAL_FAIRNESS, LTLEngine);    
            }            
        }

        public virtual bool IsProcessLevelFairnessApplicable()
        {
            return false;
        }

        /// <summary>
        /// The GUI initialization method is invoked before the verification so that the verification options are passed
        /// into the assertion.
        /// </summary>
        /// <param name="target">The model checking form or null in the console mode</param>
        /// <param name="behavior"></param>
        /// <param name="engine"></param>
        public override void UIInitialize(ISynchronizeInvoke target, int behavior, int engine)
        {
            base.UIInitialize(target, behavior, engine);

            hasFairness = true;
            switch (SelectedBahaviorName)
            {
                case Constants.COMPLETE_BEHAVIOR:
                    FairnessType = FairnessType.NO_FAIRNESS;
                    hasFairness = false;
                    break;
                case Constants.BEHAVIOR_EVENT_LEVEL_WEAK_FAIRNESS:
                    FairnessType = FairnessType.EVENT_LEVEL_WEAK_FAIRNESS;
                    break;
                case Constants.BEHAVIOR_EVENT_LEVEL_STRONG_FAIRNESS:
                    FairnessType = FairnessType.EVENT_LEVEL_STRONG_FAIRNESS;
                    break;
                case Constants.BEHAVIOR_PROCESS_LEVEL_WEAK_FAIRNESS:
                    FairnessType = FairnessType.PROCESS_LEVEL_WEAK_FAIRNESS;
                    break;
                case Constants.BEHAVIOR_PROCESS_LEVEL_STRONG_FAIRNESS:
                    FairnessType = FairnessType.PROCESS_LEVEL_STRONG_FAIRNESS;
                    break;
                case Constants.BEHAVIOR_GLOBAL_FAIRNESS:
                    FairnessType = FairnessType.GLOBAL_FAIRNESS;
                    break;
            }

            CalculateParticipatingProcess = FairnessType == FairnessType.PROCESS_LEVEL_WEAK_FAIRNESS || FairnessType == FairnessType.PROCESS_LEVEL_STRONG_FAIRNESS || MustAbstract;

            if (!IsSafety)
            {
                IsNegateLiveness = hasFairness || LivenessChecking.isLiveness(BA);
                if (!IsNegateLiveness)
                {
                    //direct decide the verification result if possible.
                    if (!BA.HasAcceptState)
                    {
                        VerificationOutput.VerificationResult = VerificationResultType.VALID;
                    }
                }
            }
            else
            {
                //direct decide the verification result if possible.
                if (!BA.HasAcceptState)
                {
                    VerificationOutput.VerificationResult = VerificationResultType.INVALID;
                }
            }
        }

        public override string ToString()
        {
            return StartingProcess + " |= " + LTLString;
        }

        #endregion

        /// <summary>
        /// Run the verification and get the result.
        /// </summary>
        /// <returns></returns>
        public override void RunVerification()
        {
            switch (SelectedBahaviorName)
            {
                case Constants.COMPLETE_BEHAVIOR:
                    switch (SelectedEngineName)
                    {
                        case Constants.ENGINE_DEPTH_FIRST_SEARCH:
                            DFSVerification();
                            break;
                        case Constants.ENGINE_BREADTH_FIRST_SEARCH:
                            BFSVerification();
                            break;
                        case Constants.ENGINE_SCC_BASED_SEARCH:

                            if (!IsNegateLiveness)
                            {
                                RunVerificationNegate();                                
                            }
                            else
                            {
                                //ImprovedTarjanGeldenhuysValmari();
                                TarjanModelChecking();                                
                            }
                            break;
                        case Constants.ENGINE_SCC_BASED_SEARCH_IMPROVED:
                            if (!IsNegateLiveness)
                            {
                                RunVerificationNegate();
                            }
                            else
                            {
                                ImprovedTarjanGeldenhuysValmari();                                
                            }
                            break;
#if DEBUG
                        case Constants.ENGINE_NESTED_DFS_SEARCH:    
                            NestedDFSModelChecking();
                            break;

                        case Constants.ENGINE_SWARM_NESTED_DFS_SEARCH:
                            SwarmNestedDFSModelChecking();
                            break;
                        case Constants.ENGINE_MULTICORE_NESTED_DFS_SEARCH:
                            MultiCoreNestedDFSModelChecking();
                            break;
                        case Constants.ENGINE_SWARM_SCC_BASED_SEARCH:
                            SwarmTarjanModelCheckingWithFairness();
                            break;
                        //case Constants.ENGINE_MULTICORE_SCC_BASED_SEARCH_SHARED_STATES:
                        //    MultiCoreWithSharedMemoryTarjanModelChecking();
                       //     break;
                        case Constants.ENGINE_MULTICORE_SCC_BASED_SEARCH_SPAWN_FAIR_THREAD:
                            ModelCheckingLivenessWithFairnessMulticore();
                            break;
#endif
                    }
                    break;
                default:
#if DEBUG
                    if (SelectedEngineName == Constants.ENGINE_SWARM_SCC_BASED_SEARCH)
                    {
                        SwarmTarjanModelCheckingWithFairness();
                    }
                    //else if (SelectedEngineName == Constants.ENGINE_MULTICORE_SCC_BASED_SEARCH_SHARED_STATES)
                    //{
                    //    MultiCoreWithSharedMemoryTarjanModelChecking();
                    //}
                    //else if (SelectedEngineName == Constants.ENGINE_MULTICORE_SCC_BASED_SEARCH_SPAWN_FAIR_THREAD)
                    //{
                    //    ModelCheckingLivenessWithFairnessMulticore();
                    //}
#endif
                    {
                        ModelCheckingLivenessWithFairness();    
                    }                    
                    break;
            }
        }

        #region "Tarjan Algo"
        protected const int VISITED_NOPREORDER = -1;
        protected const int SCC_FOUND = -2;

        /// <summary>
        /// On the fly feasible checking, 
        /// https://networkx.lanl.gov/Reference/networkx.component-pysrc.html#strongly_connected_components
        //  Returns list of strongly connected components in G.  Uses Tarjan's algorithm with Nuutila's modifications. 
        //  Nonrecursive version of algorithm. 
        //    
        //  References: 
        //    
        //  R. Tarjan (1972). Depth-first search and linear graph algorithms. SIAM Journal of Computing 1(2):146-160. 
        //    
        //  E. Nuutila and E. Soisalon-Soinen (1994). On finding the strongly connected components in a directed graph. 
        //  Information Processing Letters 49(1): 9-14. 
        //  http://coblitz.codeen.org:3125/citeseer.ist.psu.edu/cache/papers/cs/549/http:zSzzSzwww.cs.hut.fizSz~enuzSzpszSzipl-scc.pdf/nuutila94finding.pdf
        //       neighbors=G.neighbors 
        //       preorder={} 
        //       lowlink={}     
        //       scc_found={} 
        //       scc_queue = [] 
        //       scc_list=[] 
        //       i=0     # Preorder counter 
        //       for source in G: 
        //           if source not in scc_found: 
        //               queue=[source] 
        //               while queue: 
        //                   v=queue[-1] 
        //                   if v not in preorder: 
        //                       i=i+1 
        //                       preorder[v]=i 
        //                   done=1 
        //                   for w in neighbors(v): 
        //                       if w not in preorder: 
        //                           queue.append(w) 
        //                           done=0 
        //                           break 
        //                   if done==1: 
        //                       lowlink[v]=preorder[v] 
        //                       for w in neighbors(v): 
        //                           if w not in scc_found: 
        //                               if preorder[w]>preorder[v]: 
        //                                   lowlink[v]=min([lowlink[v],lowlink[w]]) 
        //                               else: 
        //                                   lowlink[v]=min([lowlink[v],preorder[w]]) 
        //                       queue.pop() 
        //                       if lowlink[v]==preorder[v]: 
        //                           scc_found[v]=True 
        //                           scc=[v] 
        //                           while scc_queue and preorder[scc_queue[-1]]>preorder[v]: 
        //                               k=scc_queue.pop() 
        //                               scc_found[k]=True 
        //                               scc.append(k) 
        //                           scc_list.append(scc) 
        //                       else: 
        //                           scc_queue.append(v) 
        //       scc_list.sort(lambda x, y: cmp(len(y),len(x))) 
        //       return scc_list  
        /// </summary>        
        /// <returns>true if feasible, otherwise return false</returns>
        public void TarjanModelChecking()
        {
            VerificationOutput.CounterExampleTrace = null;

            //out-going table.           
            Dictionary<string, List<string>> OutgoingTransitionTable = new Dictionary<string, List<string>>(Ultility.Ultility.MC_INITIAL_SIZE);

            //DFS Stack
            Stack<KeyValuePair<ConfigurationBase, string>> TaskStack = new Stack<KeyValuePair<ConfigurationBase, string>>(5000);
            
            //DFS data, which mapping each state to an int[] of size 2, first is the pre-order, second is the lowlink
            StringDictionary<int[]> DFSData = new StringDictionary<int[]>(Ultility.Ultility.MC_INITIAL_SIZE);

            List<KeyValuePair<ConfigurationBase, string>> initials = new List<KeyValuePair<ConfigurationBase, string>>();
            HashSet<string> existed = new HashSet<string>();

            foreach (string s in BA.InitialStates)
            {
                List<string> next = BA.MakeOneMove(s, InitialStep);

                foreach (string var in next)
                {
                    //if (!existed.Contains(var))
                    //{
                    //    existed.Add(var);
                    //    initials.Add(new KeyValuePair<ConfigurationBase, string>(InitialStep, var));
                    //}
                    if (existed.Add(var))
                    {
                        initials.Add(new KeyValuePair<ConfigurationBase, string>(InitialStep, var));
                    }
                }
            }

            if (initials.Count == 0)
            {
                VerificationOutput.VerificationResult = VerificationResultType.VALID;
                return;
            }

            for (int z = 0; z < initials.Count; z++)
            {
                KeyValuePair<ConfigurationBase, string> initState = initials[z];
                TaskStack.Push(initState);
                string ID = initState.Key.GetIDWithEvent() + Constants.SEPARATOR + initState.Value;
                DFSData.Add(ID, new int[] { VISITED_NOPREORDER, 0 });
                OutgoingTransitionTable.Add(ID, new List<string>(8));
            }

            List<string> StronglyConnectedComponets = new List<string>(1024);
            Stack<KeyValuePair<ConfigurationBase, string>> stepStack = new Stack<KeyValuePair<ConfigurationBase, string>>(1024);

            //# Preorder counter 
            int i = 0;

            //store the expended event step of a node to avoid multiple invocation of the make one move.
            Dictionary<string, List<KeyValuePair<ConfigurationBase, string>>> ExpendedNode = new Dictionary<string, List<KeyValuePair<ConfigurationBase, string>>>(1024);

            do
            {
                if (CancelRequested)
                {
                    VerificationOutput.NoOfStates = DFSData.Count; // VisitedWithID.Count;
                    return;
                }

                KeyValuePair<ConfigurationBase, string> pair = TaskStack.Peek();
                ConfigurationBase config = pair.Key;                               
                string v = pair.Key.GetIDWithEvent() + Constants.SEPARATOR + pair.Value;
                List<string> outgoing = OutgoingTransitionTable[v];

                int[] nodeData = DFSData.GetContainsKey(v);

                if (nodeData[0] == VISITED_NOPREORDER)
                {
                    nodeData[0] = i;
                    i++;
                }

                bool done = true;

                if (ExpendedNode.ContainsKey(v))
                {
                    List<KeyValuePair<ConfigurationBase, string>> list = ExpendedNode[v];
                    if (list.Count > 0)
                    {
                        //transverse all steps
                        for (int k = list.Count - 1; k >= 0; k--)
                        {
                            KeyValuePair<ConfigurationBase, string> step = list[k];

                            //if the step is a unvisited step
                            string tmp = step.Key.GetIDWithEvent() + Constants.SEPARATOR + step.Value;
                            if(DFSData.GetContainsKey(tmp)[0] == VISITED_NOPREORDER)
                            {
                                if (done)
                                {
                                    TaskStack.Push(step);
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
                    IEnumerable<ConfigurationBase> list = config.MakeOneMove();
                    List<KeyValuePair<ConfigurationBase, string>> product = new List<KeyValuePair<ConfigurationBase, string>>();

                    foreach (ConfigurationBase step in list)
                    {
                        List<string> states = BA.MakeOneMove(pair.Value, step);

                        for (int j = 0; j < states.Count; j++)
                        {
                            product.Add(new KeyValuePair<ConfigurationBase, string>(step, states[j]));
                        }
                    }

                    //count the transitions visited
                    VerificationOutput.Transitions += product.Count;

                    for (int k = product.Count - 1; k >= 0; k--)
                    {
                        KeyValuePair<ConfigurationBase, string> step = product[k];
                        string tmp = step.Key.GetIDWithEvent() + Constants.SEPARATOR + step.Value;
                        int[] data = DFSData.GetContainsKey(tmp);
                        if (data != null)
                        {
                            outgoing.Add(tmp);
                            if(data[0] == VISITED_NOPREORDER)
                            {
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
                            else
                            {
                                product.RemoveAt(k);
                            }
                        }
                        else
                        {
                            DFSData.Add(tmp, new int[]{VISITED_NOPREORDER, 0});
                            OutgoingTransitionTable.Add(tmp, new List<string>(8));
                            outgoing.Add(tmp);

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

                        int[] wdata = DFSData.GetContainsKey(w);
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

                        //checking for buchi-fair
                        bool BuchiFair = pair.Value.EndsWith(Constants.ACCEPT_STATE);

                        if (stepStack.Count > 0)
                        {
                            KeyValuePair<ConfigurationBase, string> valuePair = stepStack.Peek();
                            string k = valuePair.Key.GetIDWithEvent() + Constants.SEPARATOR + valuePair.Value;

                            while (stepStack.Count > 0 && DFSData.GetContainsKey(k)[0] > preorderV)
                            {
                                stepStack.Pop();
                                StronglyConnectedComponets.Add(k);
                                DFSData.GetContainsKey(k)[0] = SCC_FOUND;

                                if (!BuchiFair && valuePair.Value.EndsWith(Constants.ACCEPT_STATE))
                                {
                                    BuchiFair = true;
                                }

                                if (stepStack.Count > 0)
                                {
                                    valuePair = stepStack.Peek();
                                    k = valuePair.Key.GetIDWithEvent() + Constants.SEPARATOR + valuePair.Value;
                                }
                            }
                        }

                        if (BuchiFair && (config.IsDeadLock || StronglyConnectedComponets.Count > 1 || selfLoop))
                        {
                            VerificationOutput.VerificationResult = VerificationResultType.INVALID;
                            VerificationOutput.NoOfStates = DFSData.Count;

                            while (TaskStack.Count > 0 && TaskStack.Peek().Key.Event != Constants.INITIAL_EVENT)
                            {
                                TaskStack.Pop();
                            }

                            string startID = null;
                            if (TaskStack.Count > 0)
                            {
                                startID = TaskStack.Peek().Key.GetIDWithEvent() + Constants.SEPARATOR +
                                          TaskStack.Peek().Value;
                            }

                            VerificationOutput.CounterExampleTrace = GetConcreteTrace(InitialStep, GetCounterExample(StronglyConnectedComponets, startID, OutgoingTransitionTable));                                                            
                            return;
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

            VerificationOutput.VerificationResult = VerificationResultType.VALID;
            VerificationOutput.NoOfStates = DFSData.Count;
            return;
        }

        #endregion

        protected string GetConfigID (string ID)
        {
            int index = ID.LastIndexOf(Constants.SEPARATOR);
            return ID.Substring(0, index);
        }

        protected List<ConfigurationBase> GetConcreteTrace(ConfigurationBase init, List<string> trace)
        {
            System.Diagnostics.Debug.Assert(trace.Count == 0 || init.GetIDWithEvent() == GetConfigID(trace[0]));

            List<ConfigurationBase> toReturn = new List<ConfigurationBase>(64);
            ConfigurationBase currentConfig = init;
            toReturn.Add(currentConfig);

            for (int i = 1; i < trace.Count; i++)
            {
                string id = GetConfigID(trace[i]);
                IEnumerable<ConfigurationBase> next = currentConfig.MakeOneMove();

                foreach (ConfigurationBase configurationBase in next)
                {
                    if (configurationBase.GetIDWithEvent() == id)
                    {
                        toReturn.Add(configurationBase);
                        currentConfig = configurationBase;
                        break;
                    }
                }
            }

            return toReturn;
        }

        protected List<string> GetCounterExample(List<string> FairSCC, string startID, Dictionary<string, List<string>> OutgoingTransitionTable)
        {
            try
            {
                System.Diagnostics.Debug.Assert(VerificationOutput.CounterExampleTrace == null);
                List<string> HashedCounterExample;
                if (startID != null)
                {
                    HashedCounterExample = Path(startID, FairSCC, OutgoingTransitionTable);
                }
                else
                {
                    HashedCounterExample = new List<string>();
                }

                string startingState = FairSCC[0];

                if (FairSCC.Count > 1 || OutgoingTransitionTable[startingState].Contains(startingState))
                {
                    VerificationOutput.LoopIndex = HashedCounterExample.Count;
                    string acceptState = FindShortestPathToAFairState(startingState, null, HashedCounterExample, FairSCC,
                                                                      OutgoingTransitionTable);
                    FindShortestPathToAFairState(acceptState, acceptState, HashedCounterExample, FairSCC,
                                                 OutgoingTransitionTable);
                }

                return HashedCounterExample;
            }
            catch (CancelRunningException)
            {
                return new List<string>();
            }
        }

        protected List<string> Path(string startID, List<string> FairSCC, Dictionary<string, List<string>> OutgoingTransitionTable)
        {
            Hashtable checkedState = new Hashtable(1000);
            Queue<string> idStack = new Queue<string>();
            Queue<List<string>> pathStack = new Queue<List<string>>();

            idStack.Enqueue(startID);
            pathStack.Enqueue(new List<string>());
            checkedState.Add(startID, null);

            do
            {
                if (CancelRequested)
                {
                    throw new CancelRunningException();
                }

                string current = idStack.Dequeue();
                List<string> path = pathStack.Dequeue();

                List<string> newPath = new List<string>(path);
                newPath.Add(current);

                if (FairSCC.Contains(current))
                {
                    FairSCC.Remove(current);
                    FairSCC.Insert(0, current);
                    return newPath;
                }

                if (OutgoingTransitionTable.ContainsKey(current))
                {
                    List<string> outgoing = OutgoingTransitionTable[current];
                    for (int j = 0; j < outgoing.Count; j++)
                    {
                        string w = outgoing[j];

                        if (!checkedState.ContainsKey(w))
                        {
                            checkedState.Add(w, null);
                            idStack.Enqueue(w);
                            pathStack.Enqueue(newPath);
                        }
                    }
                }
            } while (idStack.Count > 0);

            System.Diagnostics.Debug.Assert(false);
            return new List<string>(0);
        }

        /// <summary>
        /// a BFS to find the shortest path from start node to any accept state
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        private string FindShortestPathToAFairState(string start, string end, List<string> trace, List<string> FairSCC, Dictionary<string, List<string>> OutgoingTransitionTable)
        {
            int Count = 8;

            Hashtable checkedState = new Hashtable(Count);
            Queue<string> idStack = new Queue<string>();
            Queue<List<string>> pathStack = new Queue<List<string>>();

            idStack.Enqueue(start);
            pathStack.Enqueue(new List<string>());

            do
            {

                if (CancelRequested)
                {
                   throw new CancelRunningException();
                }

                string current = idStack.Dequeue();
                List<string> path = pathStack.Dequeue();

                List<string> newPath = new List<string>(path);
                newPath.Add(current);

                if ((end == null && current.EndsWith(Constants.ACCEPT_STATE)) || (path.Count > 0 && end == current))
                {
                    newPath.RemoveAt(0);
                    trace.AddRange(newPath);
                    return current;
                }

                List<string> outgoing = OutgoingTransitionTable[current];
                for (int j = 0; j < outgoing.Count; j++)
                {
                    string w = outgoing[j];

                    if (FairSCC.Contains(w) && !checkedState.ContainsKey(w))
                    {
                        checkedState.Add(w, null);
                        idStack.Enqueue(w);
                        pathStack.Enqueue(newPath);
                    }
                }
            } while (idStack.Count > 0);

            return null;
        }

        protected override bool IsCounterExampleSpurious()
        {
            //here we perform a DFS using a stack working.
            Stack<ConfigurationBase> working = new Stack<ConfigurationBase>(1024);
            working.Push(InitialStep);

            //a stack to store corresponding depth of the elements in the working stack
            Stack<int> depthStack = new Stack<int>(1024);
            depthStack.Push(0);

            //a concrete counter example trace to store the current expended concrete counter example
            List<ConfigurationBase> ConcreteCounterExampleTrace = new List<ConfigurationBase>(64);
            List<List<string>> CounterExampleTraceEnabled = new List<List<string>>(64);

            List<int> depthList = new List<int>(1024);

            //a hashtable to stored the visited states
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
                        CounterExampleTraceEnabled.RemoveAt(lastIndex);
                    }
                }

                ConcreteCounterExampleTrace.Add(current);
                depthList.Add(depth);

                //get the next steps of concrete state
                IEnumerable<ConfigurationBase> next = current.MakeOneMove();

                List<string> Enabled = new List<string>();
                switch (FairnessType)
                {
                    case FairnessType.EVENT_LEVEL_STRONG_FAIRNESS:
                    case FairnessType.EVENT_LEVEL_WEAK_FAIRNESS:
                        //for (int i = 0; i < next.Length; i++)
                        foreach (ConfigurationBase state in next)
                        {
                            Enabled.Add(state.Event);
                        }
                        break;
                    case FairnessType.PROCESS_LEVEL_STRONG_FAIRNESS:
                    case FairnessType.PROCESS_LEVEL_WEAK_FAIRNESS:
                        foreach (ConfigurationBase state in next)
                        {
                            foreach (string proc in state.ParticipatingProcesses)
                            {
                                if (!Enabled.Contains(proc))
                                {
                                    Enabled.Add(proc);
                                }
                            }
                        }
                        break;
                    case FairnessType.GLOBAL_FAIRNESS:
                        foreach (ConfigurationBase state in next)
                        {
                            Enabled.Add(state.GetIDWithEvent());
                        }
                        break;
                }
                CounterExampleTraceEnabled.Add(Enabled);

                //if the concreate counter example is completed.
                if (ConcreteCounterExampleTrace.Count == VerificationOutput.CounterExampleTrace.Count)
                {
                    foreach (var vm in next)
                    {
                        //check if the loop is established, 
                        if (vm.GetIDWithEvent() == ConcreteCounterExampleTrace[VerificationOutput.LoopIndex].GetIDWithEvent())
                        {
                            if (!hasFairness)
                            {
                                VerificationOutput.CounterExampleTrace = ConcreteCounterExampleTrace;
                                return false;
                            }

                            //if there is fairness requirement, check whether the concrete counter example is fairness
                            if (CheckConcreteExampleFairness(ConcreteCounterExampleTrace, CounterExampleTraceEnabled))
                            {
                                VerificationOutput.CounterExampleTrace = ConcreteCounterExampleTrace;
                                return false;
                            }
                        }
                    }
                }
                else
                {
                    ConfigurationBase abstractStep = VerificationOutput.CounterExampleTrace[depth + 1];

                    //for (int j = 0; j < next.Length; j++)
                    foreach (ConfigurationBase state in next)
                    {
                        if (abstractStep.Event == state.Event && abstractStep.EqualsV(state))
                        {
                            string tmp = (depth + 1) + "-" + state.GetID();
                            if (!visited.ContainsKey(tmp))
                            {
                                working.Push(state);
                                depthStack.Push(depth + 1);
                                visited.Add(tmp);
                            }
                        }
                    }
                }

            } while (working.Count > 0);

            return true;
        }

        public override string GetResultString()
        {
            if (IsSafety)
            {
                return GetResultStringSafety();
            }

            StringBuilder sb = new StringBuilder();

            sb.AppendLine(Constants.VERFICATION_RESULT_STRING);

            if (VerificationOutput.VerificationResult == VerificationResultType.VALID)
            {
                sb.AppendLine("The Assertion (" + ToString() + ") is VALID.");
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
                    if (VerificationOutput.GenerateCounterExample)
                    {
                        sb.AppendLine("A counterexample is presented as follows.");
                        sb.Append("<");
                        bool hasVisibleEvent = false;
                        for (int i = 0; i < VerificationOutput.CounterExampleTrace.Count; i++)
                        {
                            ConfigurationBase step = VerificationOutput.CounterExampleTrace[i];
                            if (step.Event == Constants.INITIAL_EVENT)
                            {
                                sb.Append(step.Event);
                            }
                            else
                            {
                                sb.Append(" -> ");

                                if (VerificationOutput.LoopIndex >= 0 && i == VerificationOutput.LoopIndex)
                                {
                                    sb.Append("(");
                                }

                                sb.Append(step.GetDisplayEvent());

                                if (step.Event != Constants.TAU && i >= VerificationOutput.LoopIndex)
                                {
                                    hasVisibleEvent = true;
                                }
                            }
                        }

                        if (!sb.ToString().Contains("(") && !hasVisibleEvent)
                        {
                            sb.Append(" -> (" + Constants.TAU + " -> ");
                        }

                        if (VerificationOutput.LoopIndex >= 0)
                        {
                            sb.Append(")*");
                        }
                        sb.AppendLine(">");
                    }
                }
            }

            sb.AppendLine();

            sb.AppendLine("********Verification Setting********");
            sb.AppendLine("Admissible Behavior: " + SelectedBahaviorName);
            if (IsNegateLiveness)
            {
                sb.AppendLine("Search Engine: " + SelectedEngineName);
            }
            else
            {
                sb.AppendLine("Search Engine: Loop Existence Checking - The negation of the LTL formula is a safety property!");
                sb.AppendLine("System Abstraction: " + MustAbstract);
            }
            sb.AppendLine();

            return sb.ToString();
        }
    }
}