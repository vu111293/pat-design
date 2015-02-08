using System;
using System.Linq;
using System.Collections.Generic;
using PAT.Common.Classes.BA;
using PAT.Common.Classes.DataStructure;
using PAT.Common.Classes.LTS;
using PAT.Common.Classes.ModuleInterface;
using PAT.Common.Classes.SemanticModels.LTS.Assertion;
using PAT.Common.Classes.Ultility;

namespace PAT.Common.Classes.Assertion.Parallel
{
    public class TarjanThread
    {
        public Dictionary<string, List<string>> OutgoingTransitionTable;
        public VerificationResultType VerificationResult;
        public Stack<LocalPair> TaskStack;
        private BuchiAutomata BA;
        public int SearchedDepth;
        public long Transitions;
        private PATThreadPool ThreadPool;
        public bool JobFinished;
        public bool CancelRequested;
        private ConfigurationBase initialStep;
        private StringDictionary<int[]> DFSData;
        public Dictionary<string, LocalPair> FairSCC;
        public int SCCCount;
        public int BigSCCCount;
        public long SCCTotalSize;
        public FairnessType FairnessType;

        // Constant
        protected const int SCC_MINIMUM_SIZE_BOUND_FOR_THREAD_FORKING = 100;
        protected const int VISITED_NOPREORDER = -1;
        protected const int SCC_FOUND = -2;

        public TarjanThread(ConfigurationBase initialStep, BuchiAutomata ba, PATThreadPool threadPool, FairnessType FairnessType)
        {
            this.initialStep = initialStep;
            this.VerificationResult = VerificationResultType.UNKNOWN;
            this.BA = ba;
            this.ThreadPool = threadPool;
            this.FairnessType = FairnessType;
            this.FairSCC = null;
            this.JobFinished = false;
        }

        public long GetNoOfStates()
        {
            return DFSData.Count;
        }

        public void TarjanModelChecking()
        {
            OutgoingTransitionTable = new Dictionary<string, List<string>>(Ultility.Ultility.MC_INITIAL_SIZE);
            TaskStack = new Stack<LocalPair>(Ultility.Ultility.MC_INITIAL_SIZE);
            DFSData = new StringDictionary<int[]>(Ultility.Ultility.MC_INITIAL_SIZE);

            List<LocalPair> initials = LocalPair.GetInitialPairsLocal(BA, initialStep);
      
            if (initials.Count == 0 || !BA.HasAcceptState)
            {
                VerificationResult = VerificationResultType.VALID;
                return;
            }

            for (int z = 0; z < initials.Count; z++)
            {
                LocalPair initState = initials[z];
                TaskStack.Push(initState);
                string ID = initState.GetCompressedState();
                DFSData.Add(ID, new int[] { VISITED_NOPREORDER, 0 });
                OutgoingTransitionTable.Add(ID, new List<string>(8));
            }

            Dictionary<string, LocalPair> SCCPairs = new Dictionary<string, LocalPair>(1024);
            Stack<LocalPair> stepStack = new Stack<LocalPair>(1024);

            //# Preorder counter 
            int i = 0;

            //store the expended event step of a node to avoid multiple invocation of the make one move.
            Dictionary<string, List<LocalPair>> ExpendedNode = new Dictionary<string, List<LocalPair>>(1024);

            //PrintMessage("Start to find the Strongly Connected Component of graph.");

            do
            {
                if (SearchedDepth < TaskStack.Count)
                {
                    SearchedDepth = TaskStack.Count;
                }

                if (JobFinished)
                {
                    return;
                }

                LocalPair pair = TaskStack.Peek();
                ConfigurationBase evt = pair.configuration;
                string BAState = pair.state;

                string v = pair.GetCompressedState();

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
                    List<LocalPair> list = ExpendedNode[v];
                    if (list.Count > 0)
                    {
                        //transverse all steps
                        for (int k = list.Count - 1; k >= 0; k--)
                        {
                            LocalPair step = list[k];

                            string tmp = step.GetCompressedState();

                            //if the step is a unvisited step
                            if (DFSData.GetContainsKey(tmp)[0] == VISITED_NOPREORDER)
                            {
                                //only add the first unvisited step
                                //for the second or more unvisited steps, ignore at the monent
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
                    //ConfigurationBase[] list = evt.MakeOneMove().ToArray();
                    IEnumerable<ConfigurationBase> list = evt.MakeOneMove();
                    pair.SetEnabled(list, FairnessType);
                    List<LocalPair> product = LocalPair.NextLocal(BA, list, BAState);

                    //count the transitions visited
                    Transitions += product.Count;

                    for (int k = product.Count - 1; k >= 0; k--)
                    {
                        LocalPair step = product[k];
                        string tmp = step.GetCompressedState();

                        //if (VisitedWithID.ContainsKey(tmp))
                        int[] data = DFSData.GetContainsKey(tmp);
                        if (data != null)
                        {
                            //update the incoming and outgoing edges
                            //int t = VisitedWithID.GetContainsKey(tmp); //DataStore.DataManager.GetID(tmp);
                            outgoing.Add(tmp);

                            //if this node is still not visited
                            if (data[0] == VISITED_NOPREORDER)
                            {
                                //step.ID = t;
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
                            //int stateID = VisitedWithID.Count;
                            //VisitedWithID.Add(tmp,false);
                            DFSData.Add(tmp, new int[] { VISITED_NOPREORDER, 0 });

                            OutgoingTransitionTable.Add(tmp, new List<string>(8));
                            //step.ID = stateID;
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

                    // Calculate the low link of an explored state
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

                    // Check whether the current state is the root of the SCC
                    if (lowlinkV == preorderV)
                    {
                        SCCPairs.Add(v, pair);
                        nodeData[0] = SCC_FOUND;

                        bool BuchiFair = pair.state.EndsWith(Constants.ACCEPT_STATE); //for buchi-fair

                        // Get the elements of the SCC from the step stack 
                        while (stepStack.Count > 0 && DFSData.GetContainsKey(stepStack.Peek().GetCompressedState())[0] > preorderV)
                        {
                            LocalPair s = stepStack.Pop();
                            string k = s.GetCompressedState();

                            SCCPairs.Add(k, s);
                            DFSData.GetContainsKey(k)[0] = SCC_FOUND;

                            if (!BuchiFair && s.state.EndsWith(Constants.ACCEPT_STATE))
                            {
                                BuchiFair = true;
                            }
                        }

                        //outgoing.Count == 0 --> deadlock, we need to check //outgoing.Count == 0
                        //StronglyConnectedComponets.Count > 1 || selfLoop -> non-trivial case, we need to check
                        int SCCSize = SCCPairs.Count;

                        if (BuchiFair && (evt.IsDeadLock || SCCSize > 1 || selfLoop))
                        {
                            //SCCCount++;
                            SCCTotalSize += SCCSize;

                            //System.Diagnostics.Debug.WriteLine(size);

                            // If forking condition is met, create new thread to process the SCC
                            if (SCCSize >= SCC_MINIMUM_SIZE_BOUND_FOR_THREAD_FORKING && ThreadPool.ThreadNumber < Ultility.Ultility.PARALLEL_MODEL_CHECKIMG_BOUND)
                            {
                                //System.Diagnostics.Debug.WriteLine("fork");
                                //BigSCCCount++;
                                Dictionary<string, LocalPair> SCC = new Dictionary<string, LocalPair>(SCCPairs);
                                StartFairThread(SCC);
                                //System.Diagnostics.Debug.WriteLine("(" + ThreadPool.ThreadNumber + ")");
                            }

                            // If the size of scc is small or the thread pool is full,
                            // process the SCC locally is more efficient
                            else
                            {
                                //System.Diagnostics.Debug.WriteLine("self");
                                Dictionary<string, LocalPair> value = IsFair(SCCPairs, OutgoingTransitionTable);
                                if(value != null)
                                {
                                    JobFinished = true;
                                    VerificationResult = VerificationResultType.INVALID;
                                    FairSCC = value;
                                    ThreadPool.StopAllThreads();
                                    return;
                                }
                            }
                            
                        }

                        foreach (string componet in SCCPairs.Keys)
                        {
                            ExpendedNode.Remove(componet);
                            //BuchiFairTable.Remove(componet.ToString());

                            //OutgoingTransitionTable.Remove(componet);
                        }

                        SCCPairs = new Dictionary<string, LocalPair>(1024);
                    }
                    else
                    {
                        stepStack.Push(pair);
                    }
                }
            } while (TaskStack.Count > 0);

            JobFinished = true;

            return;

        }

        /// <summary>
        /// Once the SCC is found, we check whether the fairness condition is satisfied or not.
        /// </summary>
        /// <param name="StronglyConnectedComponents"></param>
        /// <returns></returns>
        private void StartFairThread(Dictionary<string, LocalPair> StronglyConnectedComponents)
        {       

            /********************************************************************
             * IMPORTANT: Be careful with the Stack copy constructor
             * It will make a new stack which is reversed of the original stack
             * So do not use Stack<LocalPair> newTaskStack = new Stack<LocalPair>(TaskStack);
             ********************************************************************/
            Stack<LocalPair> newTaskStack = new Stack<LocalPair>(TaskStack.Count);
            LocalPair[] pairArray = TaskStack.ToArray();
            for (int i = pairArray.Length - 1; i >= 0; i--)
            {
                newTaskStack.Push(pairArray[i]);
            }

            // This will reverse the order of the state in the stack
            //foreach (LocalPair pair in TaskStack)
            //{
            //    newTaskStack.Push(pair);
            //}

            // Copy the outgoing table
            Dictionary<string, List<string>> transTable = new Dictionary<string, List<string>>(1024);
            foreach (KeyValuePair<string, LocalPair> keyvaluePair in StronglyConnectedComponents)
            {
                string v = keyvaluePair.Key;
                transTable.Add(v, OutgoingTransitionTable[v]);
            }

            FairThread ft = new FairThread(StronglyConnectedComponents, newTaskStack, FairnessType, transTable);
            ThreadPool.AddThread(ft);
            //System.Threading.ThreadPool.QueueUserWorkItem(ft.Start);
        }


        /// <summary>
        /// Once the SCC is found, we check whether the fairness condition is satisfied or not.
        /// </summary>
        /// <param name="StronglyConnectedComponents"></param>
        /// <returns></returns>
        protected Dictionary<string, LocalPair> IsFair(Dictionary<string, LocalPair> StronglyConnectedComponents, Dictionary<string, List<string>> OutgoingTransitionTable)
        {
            switch (FairnessType)
            {
                case FairnessType.NO_FAIRNESS:
                    return StronglyConnectedComponents;
                case FairnessType.GLOBAL_FAIRNESS:
                    return IsGlobalFair(StronglyConnectedComponents, OutgoingTransitionTable);
                case FairnessType.EVENT_LEVEL_STRONG_FAIRNESS:
                    return IsEventStrongFair(StronglyConnectedComponents, OutgoingTransitionTable);
                case FairnessType.EVENT_LEVEL_WEAK_FAIRNESS:
                    return IsEventWeakFair(StronglyConnectedComponents);
                case FairnessType.PROCESS_LEVEL_STRONG_FAIRNESS:
                    return IsProcessStrongFair(StronglyConnectedComponents, OutgoingTransitionTable);
                case FairnessType.PROCESS_LEVEL_WEAK_FAIRNESS:
                    return IsProcesstWeakFair(StronglyConnectedComponents);
            }

            return null;
        }

        private Dictionary<string, LocalPair> IsGlobalFair(Dictionary<string, LocalPair> StronglyConnectedComponents, Dictionary<string, List<string>> OutgoingTransitionTable)
        {
            //for (int i = 0; i < StronglyConnectedComponents.Count; i++)
            foreach (KeyValuePair<string, LocalPair> value in StronglyConnectedComponents)
            {
                string componet = value.Key;
                LocalPair pair = value.Value;

                List<string> nextStates = OutgoingTransitionTable[componet];

                foreach (string nextID in pair.Enabled)
                {
                    bool found = false;

                    foreach (string state in nextStates)
                    {
                        if (StronglyConnectedComponents.ContainsKey(state))
                        {
                            if (StronglyConnectedComponents[state].configuration.GetIDWithEvent() == nextID)
                            {
                                found = true;
                                break;
                            }
                        }
                    }

                    if (!found)
                    {
                        return null;
                    }
                }
            }

            return StronglyConnectedComponents;
            //return true;
        }

        private Dictionary<string, LocalPair> IsEventStrongFair(Dictionary<string, LocalPair> StronglyConnectedComponents, Dictionary<string, List<string>> OutgoingTransitionTable)
        {
            List<string> EngagedEvents = new List<string>();
            EngagedEvents.Add(Constants.TAU);

            foreach (KeyValuePair<string, LocalPair> value in StronglyConnectedComponents)
            {
                ConfigurationBase componetStep = value.Value.configuration;
                if (!EngagedEvents.Contains(componetStep.Event) && (StronglyConnectedComponents.Count > 1 || !componetStep.IsDeadLock)) //&& componetStep.Event != Constants.TERMINATION
                {
                    EngagedEvents.Add(componetStep.Event);
                }
            }

            List<string> badStates = new List<string>();

            foreach (KeyValuePair<string, LocalPair> value in StronglyConnectedComponents)
            {
                string componet = value.Key;
                LocalPair pair = value.Value;

                foreach (string evt in pair.Enabled)
                {
                    if (!EngagedEvents.Contains(evt))
                    {
                        badStates.Add(componet);
                    }
                }
            }

            if (badStates.Count > 0)
            {
                foreach (string var in badStates)
                {
                    StronglyConnectedComponents.Remove(var);
                }

                if (StronglyConnectedComponents.Count > 0)
                {
                    Dictionary<string, LocalPair> value = TarjanModelChecking2(StronglyConnectedComponents, OutgoingTransitionTable);
                    if (value != null)
                    {
                        return value;
                    }
                }

                return null;
            }
            else
            {
                //This SCC is fair.
                return StronglyConnectedComponents;
                //return true;
            }
        }

        private Dictionary<string, LocalPair> IsEventWeakFair(Dictionary<string, LocalPair> StronglyConnectedComponents)
        {
            List<string> AlwaysEnabled = null; //for weak-fair annotation.

            HashSet<string> EngagedEvents = new HashSet<string>();
            EngagedEvents.Add(Constants.TAU);

            foreach (KeyValuePair<string, LocalPair> value in StronglyConnectedComponents)
            {
                //string componet = value.Key;
                LocalPair componetStep = value.Value;

                if (!EngagedEvents.Contains(componetStep.configuration.Event) && (StronglyConnectedComponents.Count > 1 || !componetStep.configuration.IsDeadLock)) //componetStep.Event != Constants.TERMINATION &&
                {
                    EngagedEvents.Add(componetStep.configuration.Event);
                }

                if (AlwaysEnabled == null)
                {
                    AlwaysEnabled = new List<string>();
                    AlwaysEnabled.AddRange(componetStep.Enabled);
                }
                else if (AlwaysEnabled.Count > 0)
                {
                    AlwaysEnabled = Ultility.Ultility.Intersect(AlwaysEnabled, componetStep.Enabled);
                }
            }

            //discard the SCC if there exists an event which is always enabled but never engaged. 
            for (int j = 0; j < AlwaysEnabled.Count; j++)
            {
                string s = AlwaysEnabled[j];
                if (!EngagedEvents.Contains(s))
                {
                    return null;
                }
            }

            return StronglyConnectedComponents;
            //FairSCC = StronglyConnectedComponents;
            //return true;
        }

        private Dictionary<string, LocalPair> IsProcessStrongFair(Dictionary<string, LocalPair> StronglyConnectedComponents, Dictionary<string, List<string>> OutgoingTransitionTable)
        {
            HashSet<string> EngagedProcesses = new HashSet<string>();

            //the following collects the processes which make progress during the SCC.
            foreach (KeyValuePair<string, LocalPair> value in StronglyConnectedComponents)
            {
                //string componet = value.Key;
                ConfigurationBase componetStep = value.Value.configuration;

                if (componetStep.ParticipatingProcesses != null)
                {
                    string[] indexes = componetStep.ParticipatingProcesses;
                    if (indexes != null)
                    {
                        for (int j = 0; j < indexes.Length; j++)
                        {
                            if (!EngagedProcesses.Contains(indexes[j]))
                            {
                                EngagedProcesses.Add(indexes[j]);
                            }
                        }
                    }
                }
            }
            //the above collects the processes which make progress during the SCC.

            //the following collects the processes which has been enabled at least once but failed to make progress during the SCC.
            HashSet<string> badStates = new HashSet<string>();

            foreach (KeyValuePair<string, LocalPair> value in StronglyConnectedComponents)
            {
                string componet = value.Key;
                LocalPair componetStep = value.Value;

                for (int j = 0; j < componetStep.Enabled.Count; j++)
                {
                    if (!EngagedProcesses.Contains(componetStep.Enabled[j]) && !badStates.Contains(componet))
                    {
                        badStates.Add(componet);
                    }
                }
            }
            //the above collects the processes which has been enabled at least once but failed to make progress during the SCC.

            //if there are bad states, the SCC is not fair.
            if (badStates.Count > 0)
            {
                foreach (string var in badStates)
                {
                    StronglyConnectedComponents.Remove(var);
                }

                if (StronglyConnectedComponents.Count > 0)
                {
                    Dictionary<string, LocalPair> value = TarjanModelChecking2(StronglyConnectedComponents, OutgoingTransitionTable);
                    if (value != null)
                    {
                        return value;
                    }
                }

                return null;
            }
            //otherwise, it is fair.
            else
            {
                //This SCC is fair.
                return StronglyConnectedComponents;
                //return true;
            }
        }

        private Dictionary<string, LocalPair> IsProcesstWeakFair(Dictionary<string, LocalPair> StronglyConnectedComponents)
        {
            List<string> AlwaysEnabledProcesses = new List<string>();
            HashSet<string> EngagedProcesses = new HashSet<string>();

            foreach (KeyValuePair<string, LocalPair> value in StronglyConnectedComponents)
            {
                string componet = value.Key;
                LocalPair componetStep = value.Value;

                if (AlwaysEnabledProcesses.Count > 0)
                {
                    AlwaysEnabledProcesses = Ultility.Ultility.Intersect(AlwaysEnabledProcesses, componetStep.Enabled);
                }
                else
                {
                    AlwaysEnabledProcesses.AddRange(componetStep.Enabled);
                }
                //The above gets the processes which are always enabled during the SCC.

                //The below gets the processes which made some progress during the SCC.
                if (componetStep.configuration.ParticipatingProcesses != null)
                {
                    string[] indexes = componetStep.configuration.ParticipatingProcesses;
                    foreach (string s in indexes)
                    {
                        if (!EngagedProcesses.Contains(s))
                        {
                            EngagedProcesses.Add(s);
                        }
                    }
                }
                //The above gets the processes which made some progress during the SCC.
            }

            //discard the SCC if there exists a process which is always enabled but never engaged. 
            foreach (string s in AlwaysEnabledProcesses)
            {
                if (!EngagedProcesses.Contains(s))
                {
                    return null;
                }
            }

            return StronglyConnectedComponents;
            //return true;
        }

        protected Dictionary<string, LocalPair> TarjanModelChecking2(Dictionary<string, LocalPair> SCC, Dictionary<string, List<string>> OutgoingTransitionTable)
        {
            Dictionary<string, LocalPair> StronglyConnectedComponets = new Dictionary<string, LocalPair>(64);

            Dictionary<string, int> preorder = new Dictionary<string, int>(64);
            Dictionary<string, int> lowlink = new Dictionary<string, int>(64);
            Stack<string> scc_stack = new Stack<string>(64);
            StringHashTable scc_found = new StringHashTable(64);

            int i = 0; //# Preorder counter 

            Stack<string> idStack = new Stack<string>(1024);

            Dictionary<string, LocalPair>.KeyCollection.Enumerator emun = SCC.Keys.GetEnumerator();
            emun.MoveNext();

            idStack.Push(emun.Current);

            do
            {
                if (JobFinished)
                {
                    return null;
                }

                string v = idStack.Peek();
                List<string> outgoing = OutgoingTransitionTable[v];

                if (!preorder.ContainsKey(v))
                {
                    preorder.Add(v, i);
                    i++;
                }
                bool done = true;
                for (int j = 0; j < outgoing.Count; j++)
                {
                    string w = outgoing[j];
                    if (SCC.ContainsKey(w) && !preorder.ContainsKey(w))
                    {
                        idStack.Push(w);
                        done = false;
                        break;
                    }
                }

                if (done)
                {
                    int lowlinkV = preorder[v];
                    int preorderV = lowlinkV;

                    bool selfLoop = false;

                    for (int j = 0; j < outgoing.Count; j++)
                    {
                        string w = outgoing[j];

                        if (SCC.ContainsKey(w))
                        {
                            if (w == v)
                            {
                                selfLoop = true;
                            }
                            if (!scc_found.ContainsKey(w))
                            {
                                if (preorder[w] > preorderV)
                                {
                                    lowlinkV = Math.Min(lowlinkV, lowlink[w]);
                                }
                                else
                                {
                                    lowlinkV = Math.Min(lowlinkV, preorder[w]);
                                }
                            }
                        }
                    }
                    lowlink[v] = lowlinkV;
                    idStack.Pop();

                    if (lowlinkV == preorderV)
                    {
                        scc_found.Add(v);
                        StronglyConnectedComponets.Add(v, SCC[v]);

                        //checking for buchi fair
                        bool BuchiFair = SCC[v].state.EndsWith(Constants.ACCEPT_STATE);


                        while (scc_stack.Count > 0 && preorder[scc_stack.Peek()] > preorderV)
                        {
                            string k = scc_stack.Pop();//.Dequeue();
                            StronglyConnectedComponets.Add(k, SCC[k]);
                            scc_found.Add(k);

                            if (!BuchiFair && SCC[k].state.EndsWith(Constants.ACCEPT_STATE))
                            {
                                BuchiFair = true;
                            }
                        }

                        if (BuchiFair && (outgoing.Count == 0 || StronglyConnectedComponets.Count > 1 || selfLoop))
                        {
                            Dictionary<string, LocalPair> value = IsFair(StronglyConnectedComponets, OutgoingTransitionTable);
                            if (value != null)
                            {
                                return value;
                            }
                        }

                        StronglyConnectedComponets.Clear();
                    }
                    else
                    {
                        scc_stack.Push(v);
                    }
                }

                //because the SCC can be brekon by removing bad states, 
                //if there is such case, the SCC are forests. so we have to check all components
                if (idStack.Count == 0 && scc_found.Count != SCC.Count)
                {
                    foreach (string key in SCC.Keys)
                    {
                        if (!scc_found.ContainsKey(key))
                        {
                            idStack.Push(key);
                            break;
                        }
                    }
                }

            } while (idStack.Count > 0);

            return null;
        }
    }
}