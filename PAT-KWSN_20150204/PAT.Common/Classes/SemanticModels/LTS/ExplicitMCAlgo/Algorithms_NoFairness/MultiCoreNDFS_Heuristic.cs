using System.Collections.Generic;
using System.Diagnostics;
using PAT.Common.Classes.BA;
using PAT.Common.Classes.DataStructure;
using PAT.Common.Classes.LTS;
using PAT.Common.Classes.ModuleInterface;
using PAT.Common.Classes.Ultility;
using System;
using System.Collections.Concurrent;
using System.Threading;

namespace PAT.Common.Classes.SemanticModels.LTS.Assertion
{
    public partial class AssertionLTL
    {
        /// <summary>
        /// Multi-Core Nested Depth-First Search - ATVA 2011
        /// Alfons Laarman, Rom Langerak, Jaco van de Pol, Michael Weber, Anton Wijs
        /// Extension Version
        /// </summary>
        public void MultiCoreNDFSRed()
        {
            //inititialize global variables
            finalTrace = null;
            finalLoopIndex = -1;
            isGlobalStop = false;
            globalCounterExampleLocker = new object();

            int threadNumber = CORES;
            globalRedStates = new ConcurrentDictionary<string, bool>(threadNumber, 5000);
            globalAcceptingCounter = new ConcurrentDictionary<string, int>(threadNumber, 256);

            //initialize visited times
            allVisitedStates = new ConcurrentDictionary<string, int>(threadNumber, 5000);

            //initialize threads
            Thread[] workerThreads = new Thread[threadNumber];
            for (int i = 0; i < threadNumber; i++)
            {
                int tmp = i;
                workerThreads[i] = new Thread(LocalBlue1);
                workerThreads[i].Start(tmp);
            }

            //wait for all threads complete
            for (int i = 0; i < threadNumber; i++)
            {
                workerThreads[i].Join();
            }

            //get final result
            if (finalTrace == null)
            {
                VerificationOutput.VerificationResult = VerificationResultType.VALID;
            }
            else
            {
                VerificationOutput.VerificationResult = VerificationResultType.INVALID;
                VerificationOutput.CounterExampleTrace = finalTrace;
                VerificationOutput.LoopIndex = finalLoopIndex;
            }

            //write visited times to file
            writeOverlapToFile(allVisitedStates);
        }

        /// <summary>
        /// Local blue DFS in each thread
        /// </summary>
        /// <param name="o"></param>
        public void LocalBlue1(object o)
        {
            //order of this thread
            int order = (int)o;
            Random rand = new Random(order);

            //on-the-fly data
            Stack<LocalPair> localBlueStack = new Stack<LocalPair>(5000);
            Dictionary<string, Color> localDFSColor = new Dictionary<string, Color>(5000);
            Dictionary<string, List<LocalPair>> localExpendedNodes = new Dictionary<string, List<LocalPair>>(5000);

            //initial states
            List<LocalPair> initialStates = LocalPair.GetInitialPairsLocal(BA, InitialStep);

            //check valid result
            if (initialStates.Count == 0 || !BA.HasAcceptState)
            {
                return;
            }

            //push local initial states to local blue stack
            int[] localPerm = Permutation(initialStates.Count, rand);
            for (int i = 0; i < initialStates.Count; i++)
            {
                LocalPair tmp = initialStates[localPerm[i]];
                localBlueStack.Push(tmp);
            }

            //start loop
            while (localBlueStack.Count > 0)
            {
                //cancel if take long time
                if (CancelRequested || isGlobalStop)
                {
                    return;
                }

                //get top of blue stack
                LocalPair pair = localBlueStack.Peek();
                ConfigurationBase LTSState = pair.configuration;
                string BAState = pair.state;
                string v = pair.GetCompressedState();

                //get successors
                List<LocalPair> successors = null;
                if (localExpendedNodes.ContainsKey(v))
                {
                    successors = localExpendedNodes[v];
                }
                else
                {
                    IEnumerable<ConfigurationBase> nextLTSStates = LTSState.MakeOneMove();
                    pair.SetEnabled(nextLTSStates, FairnessType);
                    successors = LocalPair.NextLocal(BA, nextLTSStates, BAState);
                    localExpendedNodes.Add(v, successors);
                }

                //if v is white
                if (!localDFSColor.ContainsKey(v))
                {
                    //set v cyan
                    Color vColor = new Color();
                    vColor.setCyan();
                    localDFSColor.Add(v, vColor);
                    
                    //early cycle detection
                    for (int i = successors.Count - 1; i >= 0; i--)
                    {
                        LocalPair succ = successors[i];
                        string w = succ.GetCompressedState();

                        //if w is cyan and (v or w is accepting)
                        if (localDFSColor.ContainsKey(w))
                        {
                            Color wColor = localDFSColor[w];
                            if (wColor.isCyan() && (succ.state.EndsWith(Constants.ACCEPT_STATE) || pair.state.EndsWith(Constants.ACCEPT_STATE)))
                            {
                                //REPORT COUNTEREXAMPLE
                                GetLocalLoopCounterExample(w, localBlueStack);
                                return;
                            }
                        }
                    }
                }

                //------------------------------------------------------------------------
                //filter successors and check if all successors are red
                bool isAllRed = true;
                List<int> unvisitedIndexs = new List<int>(successors.Count);//white and not global red
                for (int i = successors.Count - 1; i >= 0; i--)
                {
                    LocalPair succ = successors[i];
                    string w = succ.GetCompressedState();

                    //if w is white and not global red
                    if (!localDFSColor.ContainsKey(w) && !globalRedStates.ContainsKey(w))
                    {
                        unvisitedIndexs.Add(i);
                    }

                    //set all red variable to false if w not global red
                    if (isAllRed)
                    {
                        if (!globalRedStates.ContainsKey(w))
                        {
                            isAllRed = false;
                        }
                    }
                }
                //------------------------------------------------------------------------

                //get random unvisited successors
                if (unvisitedIndexs.Count > 0)
                {
                    bool isFresh = false;
                    List<int> unFreshIndexs = new List<int>();
                    while (unvisitedIndexs.Count > 0)
                    {
                        int r = rand.Next(unvisitedIndexs.Count);
                        LocalPair succ = successors[unvisitedIndexs[r]];
                        string w = succ.GetCompressedState();

                        //if w is fresh successor
                        if (!allVisitedStates.ContainsKey(w))
                        {
                            localBlueStack.Push(succ);

                            allVisitedStates.GetOrAdd(w, 0);
                            allVisitedStates[w]++;

                            isFresh = true;
                            break;
                        }
                        else
                        {
                            unFreshIndexs.Add(unvisitedIndexs[r]);
                            unvisitedIndexs.RemoveAt(r);
                        }
                    }
                    if (!isFresh)
                    {
                        int r = rand.Next(unFreshIndexs.Count);
                        LocalPair succ = successors[unFreshIndexs[r]];
                        string w = succ.GetCompressedState();
                        localBlueStack.Push(succ);

                        allVisitedStates.GetOrAdd(w, 0);
                        allVisitedStates[w]++;
                    }
                }
                else
                {
                    Color vColor = localDFSColor[v];

                    //deadlock at accepting state
                    if (pair.state.EndsWith(Constants.ACCEPT_STATE) && LTSState.IsDeadLock)
                    {
                        //REPORT COUNTEREXAMPLE
                        GetLocalDeadlockCounterExample(localBlueStack);
                        return;
                    }

                    //if all successors are red
                    if (isAllRed)
                    {
                        globalRedStates.GetOrAdd(v, true);
                    }
                    else if (pair.state.EndsWith(Constants.ACCEPT_STATE))
                    {
                        //increase counter of v by 1
                        globalAcceptingCounter.GetOrAdd(v, 0);
                        globalAcceptingCounter[v]++;

                        //start local red DFS
                        bool isStop = LocalRed1(pair, localBlueStack, localDFSColor, rand, localExpendedNodes);

                        if (isStop) { return; }
                    }

                    //set v blue
                    localDFSColor[v].setBlue();

                    //pop v out of blueStack
                    localBlueStack.Pop();
                }
            }
        }

        /// <summary>
        /// Local red DFS in each thread
        /// </summary>
        /// <param name="acceptingState"></param>
        /// <param name="localBlueStack"></param>
        /// <param name="localDFSColor"></param>
        /// <param name="rand"></param>
        /// <returns></returns>
        public bool LocalRed1(LocalPair acceptingState, Stack<LocalPair> localBlueStack, Dictionary<string, Color> localDFSColor, Random rand, Dictionary<string, List<LocalPair>> localExpendedNodes)
        {
            Stack<LocalPair> localRedStack = new Stack<LocalPair>(5000);

            //push accepting state to red stack
            localRedStack.Push(acceptingState);

            //start loop
            while (localRedStack.Count > 0)
            {
                //cancel if take long time
                if (CancelRequested || isGlobalStop)
                {
                    return false;
                }

                //get top of red stack
                LocalPair pair = localRedStack.Peek();
                string v = pair.GetCompressedState();
                Color vColor = localDFSColor[v];

                //get successors
                List<LocalPair> successors = localExpendedNodes[v];

                if (!vColor.isPink())
                {
                    //set v pink
                    vColor.setPink();

                    //check if there is cyan successor
                    for (int i = successors.Count - 1; i >= 0; i--)
                    {
                        LocalPair succ = successors[i];
                        string w = succ.GetCompressedState();

                        if (localDFSColor.ContainsKey(w) && localDFSColor[w].isCyan())
                        {
                            //REPORT COUNTER EXAMPLE
                            GetLocalLoopCounterExample(w, localBlueStack, localRedStack);
                            return true;
                        }
                    }
                }

                //------------------------------------------------------------------------
                //filter successors
                List<int> unvisitedIndexs = new List<int>(successors.Count);//not pink and not global red
                for (int i = successors.Count - 1; i >= 0; i--)
                {
                    LocalPair succ = successors[i];
                    string w = succ.GetCompressedState();

                    //if w is white and not global red
                    if (!localDFSColor[w].isPink() && !globalRedStates.ContainsKey(w))
                    {
                        unvisitedIndexs.Add(i);
                    }
                }
                //------------------------------------------------------------------------

                //get random unvisited successors
                if (unvisitedIndexs.Count > 0)
                {
                    int r = rand.Next(unvisitedIndexs.Count);
                    LocalPair succ = successors[unvisitedIndexs[r]];
                    localRedStack.Push(succ);
                }
                else
                {
                    //v is accepting
                    if (pair.state.EndsWith(Constants.ACCEPT_STATE))
                    {
                        //decrease count at v by 1
                        globalAcceptingCounter[v]--;

                        //wait for count at v equal 0
                        while (globalAcceptingCounter[v] != 0) ;
                    }

                    //set v global red
                    globalRedStates.GetOrAdd(v, true);

                    // pop red stack
                    localRedStack.Pop();
                }
            }

            //cannot report counter example
            return false;
        }

        /// <summary>
        /// Get global counter example in red DFS and stop
        /// </summary>
        /// <param name="s"></param>
        /// <param name="localBlueStack"></param>
        /// <param name="localRedStack"></param>
        public void GetLocalLoopCounterExample(string s, Stack<LocalPair> localBlueStack, Stack<LocalPair> localRedStack)
        {
            int traceLen = localRedStack.Count + localBlueStack.Count - 1;
            List<ConfigurationBase> trace = new List<ConfigurationBase>(traceLen);
            int count = 0;
            int reverseIndex = -1;

            //get trace from red stack
            while (localRedStack.Count > 1)
            {
                LocalPair tmpRed = localRedStack.Pop();
                trace.Insert(0, tmpRed.configuration);
                count++;
            }

            //get trace from blue stack
            while (localBlueStack.Count > 0)
            {
                LocalPair tmpBlue = localBlueStack.Pop();
                trace.Insert(0, tmpBlue.configuration);

                string tmpBlueID = tmpBlue.GetCompressedState();
                if (s.Equals(tmpBlueID))
                {
                    reverseIndex = count;
                }

                count++;

                if (tmpBlue.configuration.Event == Constants.INITIAL_EVENT)
                {
                    break;
                }
            }

            //get global counter example and stop
            lock (globalCounterExampleLocker)
            {
                if (isGlobalStop)
                {
                    return;
                }
                finalTrace = trace;
                finalLoopIndex = count - 1 - reverseIndex;
                isGlobalStop = true;
            }
        }

        /// <summary>
        /// Get local counter example in case of deadlock in blue DFS
        /// </summary>
        /// <param name="localBlueStack"></param>
        public void GetLocalDeadlockCounterExample(Stack<LocalPair> localBlueStack)
        {
            int traceLen = localBlueStack.Count;
            List<ConfigurationBase> trace = new List<ConfigurationBase>(traceLen);
            int count = 0;
            while (localBlueStack.Count > 0)
            {
                LocalPair tmp = localBlueStack.Pop();
                trace.Insert(0, tmp.configuration);
                string tmpID = tmp.GetCompressedState();

                count++;

                if (tmp.configuration.Event == Constants.INITIAL_EVENT)
                {
                    break;
                }
            }

            //get global counter example and stop
            lock (globalCounterExampleLocker)
            {
                if (isGlobalStop)
                {
                    return;
                }
                finalTrace = trace;
                isGlobalStop = true;
            }
        }

        /// <summary>
        /// Get global counter example in blue DFS and stop
        /// </summary>
        /// <param name="s"></param>
        /// <param name="localBlueStack"></param>
        public void GetLocalLoopCounterExample(string s, Stack<LocalPair> localBlueStack)
        {
            int traceLen = localBlueStack.Count;
            List<ConfigurationBase> trace = new List<ConfigurationBase>(traceLen);
            int count = 0;
            int reverseIndex = -1;
            while (localBlueStack.Count > 0)
            {
                LocalPair tmp = localBlueStack.Pop();
                trace.Insert(0, tmp.configuration);

                string tmpID = tmp.GetCompressedState();
                if (s.Equals(tmpID))
                {
                    reverseIndex = count;
                }

                count++;

                if (tmp.configuration.Event == Constants.INITIAL_EVENT)
                {
                    break;
                }
            }

            //get global counter example and stop
            lock (globalCounterExampleLocker)
            {
                if (isGlobalStop)
                {
                    return;
                }
                finalTrace = trace;
                finalLoopIndex = count - 1 - reverseIndex;
                isGlobalStop = true;
            }
        }
    }
}