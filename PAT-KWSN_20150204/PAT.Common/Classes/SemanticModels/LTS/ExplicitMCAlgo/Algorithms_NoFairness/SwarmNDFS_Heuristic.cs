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
        /// Swarm Nested DFS
        /// </summary>
        public void SwarmNDFS()
        {
            //inititialize global variables
            finalTrace = null;
            finalLoopIndex = -1;
            isGlobalStop = false;
            globalCounterExampleLocker = new object();

            int threadNumber = CORES;

            //initialize visited times
            allVisitedStates = new ConcurrentDictionary<string, int>(threadNumber, 5000);

            //initialize threads
            Thread[] workerThreads = new Thread[threadNumber];
            for (int i = 0; i < threadNumber; i++)
            {
                int tmp = i;
                workerThreads[i] = new Thread(LocalBlueSwarm);
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
        public void LocalBlueSwarm(object o)
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
                //filter successors
                List<int> unvisitedIndexs = new List<int>(successors.Count);
                for (int i = successors.Count - 1; i >= 0; i--)
                {
                    LocalPair succ = successors[i];
                    string w = succ.GetCompressedState();

                    //if w is white and not global red
                    if (!localDFSColor.ContainsKey(w))
                    {
                        unvisitedIndexs.Add(i);
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

                    //if v is accepting
                    if (pair.state.EndsWith(Constants.ACCEPT_STATE))
                    {
                        //if v is deadlock
                        if (LTSState.IsDeadLock)
                        {
                            //REPORT COUNTEREXAMPLE
                            GetLocalDeadlockCounterExample(localBlueStack);
                            return;
                        }
                        else
                        {
                            bool isStop = LocalRedSwarm(pair, localBlueStack, localDFSColor, rand, localExpendedNodes);
                            if (isStop)
                            {
                                return;
                            }
                        }

                        //set v pink
                        vColor.setPink();
                    }
                    else
                    {
                        vColor.setBlue();
                    }

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
        public bool LocalRedSwarm(LocalPair acceptingState, Stack<LocalPair> localBlueStack, Dictionary<string, Color> localDFSColor, Random rand, Dictionary<string, List<LocalPair>> localExpendedNodes)
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

                //get successors
                List<LocalPair> successors = localExpendedNodes[v];

                //check if there is cyan successor
                for (int i = successors.Count - 1; i >= 0; i--)
                {
                    LocalPair succ = successors[i];
                    string w = succ.GetCompressedState();

                    Color wColor = localDFSColor[w];
                    if (wColor.isCyan())
                    {
                        //REPORT COUNTEREXAMPLE
                        GetLocalLoopCounterExample(w, localBlueStack, localRedStack);
                        return true;
                    }
                }

                //find a blue successor
                List<int> unvisitedIndexs = new List<int>(successors.Count);//not visited
                for (int i = successors.Count - 1; i >= 0; i--)
                {
                    LocalPair succ = successors[i];
                    string w = succ.GetCompressedState();
                    Color wColor = localDFSColor[w];
                    if (wColor.isBlue())
                    {
                        unvisitedIndexs.Add(i);
                    }
                }

                //choose randome successor
                if (unvisitedIndexs.Count > 0)
                {
                    int r = rand.Next(unvisitedIndexs.Count);
                    LocalPair succ = successors[unvisitedIndexs[r]];
                    string w = succ.GetCompressedState();
                    Color wColor = localDFSColor[w];

                    wColor.setPink();
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