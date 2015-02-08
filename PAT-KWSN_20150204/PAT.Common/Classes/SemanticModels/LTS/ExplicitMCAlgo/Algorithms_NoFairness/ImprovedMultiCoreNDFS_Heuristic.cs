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
        /// Improved Multi-Core Nested DFS - ATVA 2012
        /// </summary>
        public void MultiCoreNDFSCombination()
        {
            //inititialize global variables
            finalTrace = null;
            finalLoopIndex = -1;
            isGlobalStop = false;
            globalCounterExampleLocker = new object();

            int threadNumber = CORES;
            globalBlueRedStates = new ConcurrentDictionary<string, bool>(threadNumber, 5000);

            //initialize visited times
            allVisitedStates = new ConcurrentDictionary<string, int>(threadNumber, 5000);

            //initialize threads
            Thread[] workerThreads = new Thread[threadNumber];
            for (int i = 0; i < threadNumber; i++)
            {
                int tmp = i;
                workerThreads[i] = new Thread(LocalBlue2);
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
        public void LocalBlue2(object o)
        {
            //order of this thread
            int order = (int)o;
            Random rand = new Random(order);

            //on-the-fly data
            Stack<LocalPair> localBlueStack = new Stack<LocalPair>(5000);
            Dictionary<string, bool> localCyanData = new Dictionary<string, bool>(5000);
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

                //if v is not cyan
                if (!localCyanData.ContainsKey(v))
                {
                    //set v cyan
                    localCyanData.Add(v, true);

                    //early cycle detection
                    for (int i = successors.Count - 1; i >= 0; i--)
                    {
                        LocalPair succ = successors[i];
                        string w = succ.GetCompressedState();

                        //if w is cyan and (v or w is accepting)
                        if (localCyanData.ContainsKey(w))
                        {
                            bool isWcyan = localCyanData[w];
                            if (isWcyan && (succ.state.EndsWith(Constants.ACCEPT_STATE) || pair.state.EndsWith(Constants.ACCEPT_STATE)))
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
                List<int> unvisitedIndexs = new List<int>(successors.Count);//not visited and not global red
                for (int i = successors.Count - 1; i >= 0; i--)
                {
                    LocalPair succ = successors[i];
                    string w = succ.GetCompressedState();

                    //if w is not cyan and not global blue
                    if (!localCyanData.ContainsKey(w) && !globalBlueRedStates.ContainsKey(w))
                    {
                        unvisitedIndexs.Add(i);
                    }

                    //set all red variable to false if w not global red
                    if (isAllRed)
                    {
                        if (!globalBlueRedStates.ContainsKey(w) || !globalBlueRedStates[w])
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
                    //mark v global blue
                    globalBlueRedStates.GetOrAdd(v, false);

                    //deadlock at accepting state
                    if (pair.state.EndsWith(Constants.ACCEPT_STATE) && LTSState.IsDeadLock)
                    {
                        //REPORT COUNTEREXAMPLE
                        GetLocalDeadlockCounterExample(localBlueStack);
                        return;
                    }

                    //if all successors are global red
                    if (isAllRed)
                    {
                        globalBlueRedStates[v] = true;
                    }
                    else if (pair.state.EndsWith(Constants.ACCEPT_STATE))
                    {
                        Dictionary<LocalPair, bool> rSet = new Dictionary<LocalPair, bool>(1024);

                        //start local red DFS
                        bool isStop = LocalRed2(rSet, pair, localBlueStack, localCyanData, rand, localExpendedNodes);

                        if (isStop) { return; }

                        //wait for all accepting states EXCEPT v in rSet become red
                        foreach (KeyValuePair<LocalPair, bool> kv in rSet)
                        {
                            string s = kv.Key.GetCompressedState();
                            if (kv.Key.state.EndsWith(Constants.ACCEPT_STATE) && !s.Equals(v))
                            {
                                while (!globalBlueRedStates[s]) ;
                            }
                        }

                        //set all states in rSet to red
                        foreach (KeyValuePair<LocalPair, bool> kv in rSet)
                        {
                            string kvID = kv.Key.GetCompressedState();
                            globalBlueRedStates[kvID] = true;
                        }
                    }

                    //pop blue stack
                    localBlueStack.Pop();

                    //uncyan v
                    localCyanData[v] = false;
                }
            }
        }

        /// <summary>
        /// Local red DFS in each thread
        /// </summary>
        /// <param name="rSet"></param>
        /// <param name="acceptingState"></param>
        /// <param name="localBlueStack"></param>
        /// <param name="localCyanData"></param>
        /// <param name="rand"></param>
        /// <returns></returns>
        public bool LocalRed2(Dictionary<LocalPair, bool> rSet, LocalPair acceptingState, Stack<LocalPair> localBlueStack, Dictionary<string, bool> localCyanData, Random rand, Dictionary<string, List<LocalPair>> localExpendedNodes)
        {
            Stack<LocalPair> localRedStack = new Stack<LocalPair>(5000);
            Dictionary<string, bool> inRSet = new Dictionary<string, bool>(1024);

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
                ConfigurationBase LTSState = pair.configuration;
                string BAState = pair.state;
                string v = pair.GetCompressedState();

                //get successors
                //v may not in expendedNodes
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

                //if v not in R set
                if (!inRSet.ContainsKey(v))
                {
                    //add v to R
                    rSet.Add(pair, true);
                    inRSet.Add(v, true);

                    //check if there is cyan successor
                    for (int i = successors.Count - 1; i >= 0; i--)
                    {
                        LocalPair succ = successors[i];
                        string w = succ.GetCompressedState();

                        if (localCyanData.ContainsKey(w) && localCyanData[w])
                        {
                            //REPORT COUNTER EXAMPLE
                            GetLocalLoopCounterExample(w, localBlueStack, localRedStack);
                            return true;
                        }
                    }
                }

                //------------------------------------------------------------------------
                //filter successors
                List<int> unvisitedIndexs = new List<int>(successors.Count);//not in and not global red
                for (int i = successors.Count - 1; i >= 0; i--)
                {
                    LocalPair succ = successors[i];
                    string w = succ.GetCompressedState();

                    //if w is white and not global red
                    if (!inRSet.ContainsKey(w) && !globalBlueRedStates[w])
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
                    localRedStack.Pop();
                }
            }

            //cannot report counter example
            return false;
        }
    }
}