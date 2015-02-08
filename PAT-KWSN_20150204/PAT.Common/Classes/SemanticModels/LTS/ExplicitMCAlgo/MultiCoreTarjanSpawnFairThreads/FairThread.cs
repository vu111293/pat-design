using System;
using System.Collections.Generic;
using PAT.Common.Classes.DataStructure;
using PAT.Common.Classes.LTS;
using PAT.Common.Classes.ModuleInterface;
using PAT.Common.Classes.SemanticModels.LTS.Assertion;
using PAT.Common.Classes.Ultility;


namespace PAT.Common.Classes.Assertion.Parallel
{
    public delegate void ReturnEvent(FairThread thread);

    public sealed class FairThread
    {
        public event ReturnEvent ReturnAction;
        
        public bool result;

        Dictionary<string, LocalPair> SCC;
        //private BuchiAutomata BA;
        Dictionary<string, List<string>> OutgoingTransitionTable;
        public FairnessType FairnessType;

        public Dictionary<string, LocalPair> FairSCC;
        public bool CancelRequested;
        public Stack<LocalPair> TaskStack;

        public FairThread(Dictionary<string, LocalPair> scc, Stack<LocalPair> tStack, FairnessType fairType, Dictionary<string, List<string>> OutgoingTransitionTable)
        {
            this.SCC = scc;
            this.TaskStack = tStack;
            this.FairnessType = fairType;
            //this.BA = BA;
            this.OutgoingTransitionTable = OutgoingTransitionTable;
        }

        /*
        protected void GenerateOutgoingTransitionTable()
        {
            // Initialize the outgoing table
            OutgoingTransitionTable = new Dictionary<string, List<string>>(1024);
            string key = null;
            foreach (KeyValuePair<string, LocalPair> keyValuePair in SCC)
            {
                key = keyValuePair.Key;
                OutgoingTransitionTable.Add(key, new List<string>(8));
            }

            // Using DFS to find create the transitions
            Stack< LocalPair > stack = new Stack<LocalPair>(SCC.Count);

            if (key == null)
                return;

            stack.Push(SCC[key]);

            HashSet<string> ExploredStates = new HashSet<string>();

            do
            {
                LocalPair pair = stack.Pop();
                ConfigurationBase evt = pair.configuration;
                string BAState = pair.state;
                string v = pair.GetCompressedState();

                List<string> outgoing = OutgoingTransitionTable[v];

                ConfigurationBase[] list = evt.MakeOneMove();
                List<LocalPair> product = LocalPair.NextLocal(BA, list, BAState);

                foreach (LocalPair step in product)
                {
                    string w = step.GetCompressedState();
                    if (SCC.ContainsKey(w))
                    {
                        outgoing.Add(w);
                        if (!ExploredStates.Contains(w))
                        {
                            stack.Push(step);
                        }
                    }
                }

                ExploredStates.Add(v);

            } while (stack.Count > 0);
        }
        */

        public void InternalStart(object o)
        {
            //GenerateOutgoingTransitionTable();
            FairSCC = IsFair(SCC, OutgoingTransitionTable);
            if (FairSCC != null)
            {
                result = true;
            }
            else
            {
                result = false;
            }

            lock (this)
            {
                ReturnAction(this);
            }
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