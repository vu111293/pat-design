using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using PAT.Common.Classes.BA;
using PAT.Common.Classes.DataStructure;
using PAT.Common.Classes.LTS;
using PAT.Common.Classes.ModuleInterface;
using PAT.Common.Classes.Ultility;

namespace PAT.Common.Classes.SemanticModels.LTS.Assertion
{
    public partial class AssertionLTL
    {
        public Stack<LocalPair> LocalTaskStack;

        /// <summary>
        /// Run the verification and get the result.
        /// </summary>
        /// <returns></returns>
        public void ModelCheckingLivenessWithFairness()
        {
            Dictionary<string, List<string>> OutgoingTransitionTable = new Dictionary<string, List<string>>(Ultility.Ultility.MC_INITIAL_SIZE);
            LocalTaskStack = new Stack<LocalPair>(5000);
            VerificationOutput.CounterExampleTrace = null;

            List<LocalPair> initials = LocalPair.GetInitialPairsLocal(BA, InitialStep);
            StringDictionary<int[]> DFSData = new StringDictionary<int[]>();
            
            if (initials.Count == 0)
            {
                VerificationOutput.VerificationResult = VerificationResultType.VALID;
                return;
            }

            for (int z = 0; z < initials.Count; z++)
            {
                LocalPair initState = initials[z];             
                LocalTaskStack.Push(initState);                
                string ID = initState.GetCompressedState(FairnessType);
                DFSData.Add(ID, new int[] { VISITED_NOPREORDER, 0 });
                OutgoingTransitionTable.Add(ID, new List<string>(8));
            }

            Dictionary<string, LocalPair> SCCPairs = new Dictionary<string, LocalPair>(1024);
            Stack<LocalPair> stepStack = new Stack<LocalPair>(1024);

            int i = 0;

            //store the expended event step of a node to avoid multiple invocation of the make one move.
            Dictionary<string, List<LocalPair>> ExpendedNode = new Dictionary<string, List<LocalPair>>(1024);

            do
            {
                if (CancelRequested)
                {
                    VerificationOutput.NoOfStates = DFSData.Count; // VisitedWithID.Count;
                    return;
                }

                LocalPair pair = LocalTaskStack.Peek();
                ConfigurationBase evt = pair.configuration;
                string BAState = pair.state;

                string v = pair.GetCompressedState(FairnessType);

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

                            //if the step is a unvisited step
                            string tmp = step.GetCompressedState(FairnessType);
                            if (DFSData.GetContainsKey(tmp)[0] == VISITED_NOPREORDER)
                            //if (!preorder.ContainsKey(step.GetCompressedState()))
                            {
                                //only add the first unvisited step
                                //for the second or more unvisited steps, ignore at the monent
                                if (done)
                                {
                                    LocalTaskStack.Push(step);
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
                    VerificationOutput.Transitions += product.Count;

                    for (int k = product.Count - 1; k >= 0; k--)
                    {
                        LocalPair step = product[k];
                        string tmp = step.GetCompressedState(FairnessType);

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
                                    LocalTaskStack.Push(step);
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
                                LocalTaskStack.Push(step);
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
                    int lowlinkV = nodeData[0]; // preorder[v];
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

                    LocalTaskStack.Pop();

                    if (lowlinkV == preorderV)
                    {
                        SCCPairs.Add(v, pair);
                        nodeData[0] = SCC_FOUND;

                        //checking for buchi-fair
                        bool BuchiFair = pair.state.EndsWith(Constants.ACCEPT_STATE);

                        //while (stepStack.Count > 0 && preorder[stepStack.Peek().GetCompressedState()] > preorderV)
                        while (stepStack.Count > 0 && DFSData.GetContainsKey(stepStack.Peek().GetCompressedState(FairnessType))[0] > preorderV)
                        {
                            LocalPair s = stepStack.Pop();
                            string k = s.GetCompressedState(FairnessType);

                            DFSData.GetContainsKey(k)[0] = SCC_FOUND;
                            
                            SCCPairs.Add(k, s);

                            if (!BuchiFair && s.state.EndsWith(Constants.ACCEPT_STATE))
                            {
                                BuchiFair = true;
                            }
                        }

                        int SCCSize = SCCPairs.Count;

                        if (BuchiFair && (evt.IsDeadLock || SCCSize > 1 || selfLoop))
                        {
                            PrintMessage("A SCC of size " + SCCSize + " is found");

                            Dictionary<string, LocalPair> value = IsFair(SCCPairs, OutgoingTransitionTable);
                            if (value != null)
                            {
                                PrintMessage("The SCC found is FAIR.");

                                VerificationOutput.VerificationResult = VerificationResultType.INVALID;
                                VerificationOutput.NoOfStates = DFSData.Count; // VisitedWithID.Count;
                                LocalGetCounterExample(value, OutgoingTransitionTable);
                                return;
                            }

                            PrintMessage("The SCC found is NOT fair.");
                        }

                        foreach (string componet in SCCPairs.Keys)
                        {
                            ExpendedNode.Remove(componet);
                            OutgoingTransitionTable.Remove(componet);
                        }

                        //StronglyConnectedComponets.Clear();
                        SCCPairs.Clear();
                    }
                    else
                    {
                        stepStack.Push(pair);
                    }
                }
            } while (LocalTaskStack.Count > 0);

            VerificationOutput.VerificationResult = VerificationResultType.VALID;
            VerificationOutput.NoOfStates = DFSData.Count; // VisitedWithID.Count;
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

                List<string > nextStates = OutgoingTransitionTable[componet];

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

                if(AlwaysEnabled == null)
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
                //string componet = value.Key;
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

        protected virtual Dictionary<string, LocalPair> TarjanModelChecking2(Dictionary<string, LocalPair> SCC, Dictionary<string, List<string>> OutgoingTransitionTable)
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

        protected void LocalGetCounterExample(Dictionary<string, LocalPair> FairSCC, Dictionary<string, List<string>> OutgoingTransitionTable)
        {
            try
            {

            if(!VerificationOutput.GenerateCounterExample)
            {
                return;
            }

            if(LocalTaskStack.Count == 0)
            {
                VerificationOutput.CounterExampleTrace = GetConcreteTrace(InitialStep, new List<string>());
                return;
            }

            while (LocalTaskStack.Count > 0 && LocalTaskStack.Peek().configuration.Event != Constants.INITIAL_EVENT)
            {
                LocalTaskStack.Pop();
            }

            string startID = LocalTaskStack.Peek().GetCompressedState(FairnessType);

            List<string> scc = new List<string>(FairSCC.Keys);
            List<string> HashedCounterExample = Path(startID, scc, OutgoingTransitionTable);

            string startingState = scc[0];

            //deadlock case
            if (FairSCC.Count > 1 || OutgoingTransitionTable[startingState].Contains(startingState))
            {
                VerificationOutput.LoopIndex = HashedCounterExample.Count;
                Dictionary<string, bool> localVisited = new Dictionary<string, bool>(FairSCC.Count);

                Stack<string> idStack = new Stack<string>(64);
                idStack.Push(startingState);

                do
                {
                    string v = idStack.Pop();

                    if (!localVisited.ContainsKey(v))
                    {
                        localVisited.Add(v, false);
                        HashedCounterExample.Add(v);

                        List<string> outgoing = OutgoingTransitionTable[v];

                        bool HasNewNode = false;

                        for (int j = 0; j < outgoing.Count; j++)
                        {
                            string w = outgoing[j];

                            if (FairSCC.ContainsKey(w))
                            {
                                if (!localVisited.ContainsKey(w)) // && !idStack.Contains(w)
                                {
                                    HasNewNode = true;
                                    idStack.Push(w);
                                }
                            }
                        }

                        if (!HasNewNode)
                        {
                            //pop up the visited nodes
                            while (idStack.Count > 0 && localVisited.ContainsKey(idStack.Peek()))
                            {
                                idStack.Pop();
                            }

                            if (idStack.Count > 0)
                            {
                                HashedCounterExample.AddRange(FindShortestPath(v, idStack.Peek(), localVisited, idStack, FairSCC, OutgoingTransitionTable));
                            }
                            else
                            {
                                if (v != startingState)
                                {
                                    HashedCounterExample.AddRange(FindShortestPath(v, startingState, localVisited, null, FairSCC, OutgoingTransitionTable));
                                }
                            }
                        }
                    }
                } while (idStack.Count > 0);

                //make sure all fcc is visisted.
                Debug.Assert(localVisited.Count == FairSCC.Count);
                HashedCounterExample.Add(startingState);
            }

            VerificationOutput.CounterExampleTrace = MyGetConcreteTraceWithFairness(HashedCounterExample);

            }
            catch (CancelRunningException)
            {

              
            }
        }

        private List<ConfigurationBase> MyGetConcreteTraceWithFairness(List<string> trace)
        {
            List<ConfigurationBase> toReturn = new List<ConfigurationBase>(64);
            ConfigurationBase currentConfig = InitialStep;
            toReturn.Add(currentConfig);

            for (int i = 1; i < trace.Count; i++)
            {
                string id = GetConfigID(trace[i]);
                IEnumerable<ConfigurationBase> next = currentConfig.MakeOneMove();

                foreach (ConfigurationBase configurationBase in next)
                {
                    string participateProcesses = "";

                    if (FairnessType == FairnessType.PROCESS_LEVEL_STRONG_FAIRNESS || FairnessType == FairnessType.PROCESS_LEVEL_WEAK_FAIRNESS)
                    {
                        if (configurationBase.ParticipatingProcesses != null)
                        {
                            foreach (string item in configurationBase.ParticipatingProcesses)
                            {
                                participateProcesses += Constants.SEPARATOR + item;
                            }
                        }
                    }

                    if (configurationBase.GetIDWithEvent() + participateProcesses == id)
                    {
                        toReturn.Add(configurationBase);
                        currentConfig = configurationBase;
                        break;
                    }
                }
            }

            return toReturn;
        }

        /// <summary>
        /// a BFS to find the shortest path from start node to the end node
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="localVisited"></param>
        private List<string> FindShortestPath(string start, string end, Dictionary<string, bool> localVisited, Stack<string> outStack, Dictionary<string, LocalPair> FairSCC, Dictionary<string, List<string>> OutgoingTransitionTable)
        {
            int Count = localVisited.Count;

            Hashtable checkedState = new Hashtable(Count);
            Queue<string> idStack = new Queue<string>();
            Queue<List<string>> pathStack = new Queue<List<string>>();

            idStack.Enqueue(start);
            pathStack.Enqueue(new List<string>());
            checkedState.Add(start, null);
            List<string> newPath = null;

            string current;
            List<string> outgoing;
            do
            {
                if (CancelRequested)
                {
                    throw new CancelRunningException();
                }

                current = idStack.Dequeue();
                List<string> path = pathStack.Dequeue();

                newPath = new List<string>(path);
                newPath.Add(current);

                outgoing = OutgoingTransitionTable[current];
                for (int j = 0; j < outgoing.Count; j++)
                {
                    string w = outgoing[j];

                    if (w == end)
                    {
                        newPath.RemoveAt(0);
                        return newPath;
                    }

                    if (localVisited.ContainsKey(w))
                    {
                        if (FairSCC.ContainsKey(w) && !checkedState.ContainsKey(w))
                        {
                            checkedState.Add(w, null);
                            idStack.Enqueue(w);
                            pathStack.Enqueue(newPath);
                        }
                    }
                }
            } while (idStack.Count > 0);

            outgoing = OutgoingTransitionTable[current];
            for (int j = 0; j < outgoing.Count; j++)
            {
                string w = outgoing[j];
                if (FairSCC.ContainsKey(w) && !localVisited.ContainsKey(w))
                {
                    outStack.Push(w);
                }
            }

            return newPath;
        }

        private bool CheckConcreteExampleFairness(List<ConfigurationBase> counterExampleVM, List<List<string>> counterExampleEnabled)
        {
            List<string> Engaged = new List<string>();
            Engaged.Add(Constants.TAU);

            switch (FairnessType)
            {
                case FairnessType.GLOBAL_FAIRNESS:
                    Dictionary<string, bool> engagedSteps = new Dictionary<string, bool>(counterExampleVM.Count);

                    for (int i = VerificationOutput.LoopIndex; i < counterExampleVM.Count; i++)
                    {
                        ConfigurationBase componetStep = counterExampleVM[i];

                        string componetStepID;
                        if (i == counterExampleVM.Count - 1)
                        {
                            componetStepID = componetStep.GetID() + Constants.SEPARATOR + counterExampleVM[VerificationOutput.LoopIndex].GetIDWithEvent();
                        }
                        else
                        {
                            componetStepID = componetStep.GetID() + Constants.SEPARATOR + counterExampleVM[i + 1].GetIDWithEvent();
                        }

                        if (!engagedSteps.ContainsKey(componetStepID))
                        {
                            engagedSteps.Add(componetStepID, false);
                        }
                    }

                    for (int i = VerificationOutput.LoopIndex; i < counterExampleVM.Count; i++)
                    {
                        ConfigurationBase componetStep = counterExampleVM[i];
                        string componetStepID = componetStep.GetID();

                        List<string> enabled = counterExampleEnabled[i];
                        foreach (string vm in enabled)
                        {
                            string nextState = componetStepID + Constants.SEPARATOR + vm;

                            if (!engagedSteps.ContainsKey(nextState))
                            {
                                return false;
                            }
                        }
                    }

                    return true;

                case FairnessType.EVENT_LEVEL_WEAK_FAIRNESS:
                    List<string> AlwaysEnabled = new List<string>(); //for weak-fair annotation.

                    //collect all events which are enabled at every state in the SCC; meanwhile check whether there is an accepting state.
                    for (int i = VerificationOutput.LoopIndex; i < counterExampleVM.Count; i++)
                    {
                        ConfigurationBase componetStep = counterExampleVM[i];
                        if (!Engaged.Contains(componetStep.Event) && (counterExampleVM.Count > VerificationOutput.LoopIndex + 1 || !componetStep.IsDeadLock)) //componetStep.Event != Constants.TERMINATION &&
                        {
                            Engaged.Add(componetStep.Event);
                        }

                        if (i == VerificationOutput.LoopIndex)
                        {
                            AlwaysEnabled.AddRange(counterExampleEnabled[i]);
                        }
                        else if (AlwaysEnabled.Count > 0)
                        {
                            //List<string> enabled = componetStep.GetEnabled();
                            AlwaysEnabled = Ultility.Ultility.Intersect(AlwaysEnabled, counterExampleEnabled[i]);
                        }
                    }

                    for (int j = 0; j < AlwaysEnabled.Count; j++)
                    {
                        string s = AlwaysEnabled[j];
                        if (!Engaged.Contains(s))
                        {
                            return false;
                        }
                    }

                    return true;

                case FairnessType.EVENT_LEVEL_STRONG_FAIRNESS:

                    for (int i = VerificationOutput.LoopIndex; i < counterExampleVM.Count; i++)
                    {
                        ConfigurationBase componetStep = counterExampleVM[i];
                        if (!Engaged.Contains(componetStep.Event) && (counterExampleVM.Count > VerificationOutput.LoopIndex + 1 || !componetStep.IsDeadLock)) // && componetStep.Event != Constants.TERMINATION 
                        {
                            Engaged.Add(componetStep.Event);
                        }
                    }


                    for (int i = VerificationOutput.LoopIndex; i < counterExampleVM.Count; i++)
                    {
                        List<string> enabled = counterExampleEnabled[i];

                        foreach (string evt in enabled)
                        {
                            if (!Engaged.Contains(evt))
                            {
                                return false;
                            }
                        }
                    }

                    return true;

                case FairnessType.PROCESS_LEVEL_STRONG_FAIRNESS:
                    //the following collects the processes which make progress during the SCC.
                    for (int i = VerificationOutput.LoopIndex; i < counterExampleVM.Count; i++)
                    {

                        ConfigurationBase componetStep = counterExampleVM[i];

                        if (componetStep.ParticipatingProcesses != null)
                        {
                            string[] indexes = componetStep.ParticipatingProcesses;
                            if (indexes != null)
                            {
                                for (int j = 0; j < indexes.Length; j++)
                                {
                                    if (!Engaged.Contains(indexes[j]))
                                    {
                                        Engaged.Add(indexes[j]);
                                    }
                                }
                            }
                        }
                    }
                    //the above collects the processes which make progress during the SCC.

                    //the following collects the processes which has been enabled at least once but failed to make progress during the SCC.                    
                    for (int i = VerificationOutput.LoopIndex; i < counterExampleVM.Count; i++)
                    {
                        List<string> enabled = counterExampleEnabled[i]; //componetStep.GetEnabledProcesses();

                        if (enabled != null)
                        {
                            for (int j = 0; j < enabled.Count; j++)
                            {
                                if (!Engaged.Contains(enabled[j]))
                                {
                                    return false;
                                }
                            }
                        }
                    }

                    return true;

                case FairnessType.PROCESS_LEVEL_WEAK_FAIRNESS:

                    List<string> AlwaysEnabledProcesses = new List<string>();

                    for (int i = VerificationOutput.LoopIndex; i < counterExampleVM.Count; i++)
                    {
                        ConfigurationBase componetStep = counterExampleVM[i];

                        //The below gets the processes which are always enabled during the SCC.
                        //List<string> enabled = componetStep.GetEnabledProcesses();
                        if (AlwaysEnabledProcesses.Count > 0)
                        {
                            AlwaysEnabledProcesses = Ultility.Ultility.Intersect(AlwaysEnabledProcesses, counterExampleEnabled[i]);
                        }
                        else
                        {
                            AlwaysEnabledProcesses.AddRange(counterExampleEnabled[i]);
                        }
                        //The above gets the processes which are always enabled during the SCC.

                        //The below gets the processes which made some progress during the SCC.
                        if (componetStep.ParticipatingProcesses != null)
                        {
                            string[] indexes = componetStep.ParticipatingProcesses;
                            foreach (string s in indexes)
                            {
                                if (!Engaged.Contains(s))
                                {
                                    Engaged.Add(s);
                                }
                            }
                        }
                        //The above gets the processes which made some progress during the SCC.
                    }

                    //discard the SCC if there exists a process which is always enabled but never engaged. 
                    foreach (string s in AlwaysEnabledProcesses)
                    {
                        if (!Engaged.Contains(s))
                        {
                            return false;
                        }
                    }

                    return true;
            }

            return false;
        }
    }

    public sealed class LocalPair
    {
        public ConfigurationBase configuration;
        public string state;
        public List<string> Enabled;
        //public List<string> Engaged;

        public LocalPair(ConfigurationBase e, string s)
        {
            configuration = e;
            state = s;
            Enabled = new List<string>();
        }

        public string GetCompressedState()
        {
            return configuration.GetIDWithEvent() + Constants.SEPARATOR + state;
        }

        /// <summary>
        /// The following method returns an ID of the configruation with the participating process if process-level fairness is invovled.
        /// </summary>
        /// <param name="fairness"></param>
        /// <returns></returns>
        public string GetCompressedState(FairnessType fairness)
        {
            if (fairness == FairnessType.PROCESS_LEVEL_STRONG_FAIRNESS || fairness == FairnessType.PROCESS_LEVEL_WEAK_FAIRNESS)
            {
                if (configuration.ParticipatingProcesses == null)
                {
                    return configuration.GetIDWithEvent() + Constants.SEPARATOR + state;

                }

                string participateProcesses = "";

                foreach (string item in configuration.ParticipatingProcesses)
                {
                    participateProcesses += item + Constants.SEPARATOR;
                }

                return configuration.GetIDWithEvent() + Constants.SEPARATOR + participateProcesses + state;
            }

            return configuration.GetIDWithEvent() + Constants.SEPARATOR + state;
        }

        /// <summary>
        /// Given one environment, get the initial states of the product of the system and the automata. Notice that the automata 
        /// is allowed to make one move first. This is necessary to check the very first state of the system. 
        /// </summary>
        /// <param name="initialStep"></param>
        /// <returns></returns>
        public static List<LocalPair> GetInitialPairsLocal(BuchiAutomata BA, ConfigurationBase initialStep)
        {
            List<LocalPair> toReturn = new List<LocalPair>();
            HashSet<string> existed = new HashSet<string>();

            foreach (string s in BA.InitialStates)
            {
                List<string> next = BA.MakeOneMove(s, initialStep);

                foreach (string var in next)
                {
                    if (existed.Add(var))
                    {
                        toReturn.Add(new LocalPair(initialStep, var));
                    }
                }
            }

            return toReturn;
        }

        public static List<LocalPair> NextLocal(BuchiAutomata BA, IEnumerable<ConfigurationBase> steps, string BAState)
        {
            List<LocalPair> product = new List<LocalPair>(steps.Count() * BA.States.Length);

            //for (int i = 0; i < steps.Length; i++)
            foreach (var step in steps)
            {
                //ConfigurationBase step = steps[i];
                List<string> states = BA.MakeOneMove(BAState, step);

                for (int j = 0; j < states.Count; j++)
                {
                    product.Add(new LocalPair(step, states[j]));
                }
            }

            return product;
        }

        //public void SetEnabledEngaged(IEnumerable<ConfigurationBase> steps, FairnessType fairness)
        public void SetEnabled(IEnumerable<ConfigurationBase> steps, FairnessType fairness)
        {
            switch (fairness)
            {
                case FairnessType.EVENT_LEVEL_STRONG_FAIRNESS:
                case FairnessType.EVENT_LEVEL_WEAK_FAIRNESS:
                    //for (int i = 0; i < steps.Length; i++)
                    foreach (var step in steps)
                    {
                        Enabled.Add(step.Event);
                    }
                    //Engaged.Add(configuration.Event);
                    break;
                case FairnessType.PROCESS_LEVEL_STRONG_FAIRNESS:
                case FairnessType.PROCESS_LEVEL_WEAK_FAIRNESS:
                    foreach (var step in steps)
                    {
                        Debug.Assert(step.ParticipatingProcesses != null);

                        foreach (string proc in step.ParticipatingProcesses)
                        {
                            if (!Enabled.Contains(proc))
                            {
                                Enabled.Add(proc);
                            }
                        }
                    }

                    //Debug.Assert(configuration.ParticipatingProcesses != null);

                    //    foreach (string proc in configuration.ParticipatingProcesses)
                    //    {
                    //        if (!Engaged.Contains(proc))
                    //        {
                    //            Engaged.Add(proc);
                    //        }
                    //    }
                    break;
                case FairnessType.GLOBAL_FAIRNESS:
                    foreach (var step in steps)
                    {
                        Enabled.Add(step.GetIDWithEvent());
                    }
                    //Engaged.Add(configuration.GetIDWithEvent());
                    break;
            }            
        }
    }
}
