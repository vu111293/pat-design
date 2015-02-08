using System;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Collections.Concurrent;
using PAT.Common.Classes.BA;
using PAT.Common.Classes.DataStructure;
using PAT.Common.Classes.LTS;
using PAT.Common.Classes.ModuleInterface;
using PAT.Common.Classes.Ultility;

namespace PAT.Common.Classes.SemanticModels.LTS.Assertion
{
    public partial class AssertionLTL
    {
        /// <summary>
        /// Multi-Core Nested Depth-First Search - ATVA 2011
        /// Alfons Laarman, Rom Langerak, Jaco van de Pol, Michael Weber, Anton Wijs
        /// Extension Version
        /// </summary>
        public void MultiCoreNestedDFSSharedRed_DotNet4()
        {
            // set number of cores
            int cores = setCores(4);

            // initalize common data
            finalOutgoingTransitionTable = null;
            finalAcceptingCycle = null;
            finalLocalTaskStack = null;
            sharedRedStates = new ConcurrentDictionary<string, bool>(cores, 5000);
            sharedAcceptingCountRedDFS = new ConcurrentDictionary<string, int>(cores, 256);
            isStop = false;
            reportLocker = new Object();

            // data for overlap calculation
            //visitedTimes = new ConcurrentDictionary<string, int>(cores, 5000);

            // initialize result & start threads
            VerificationOutput.VerificationResult = VerificationResultType.VALID;
            Thread[] workerThreads = new Thread[cores];
            for (int i = 0; i < cores; i++)
            {
                int tmp = i;
                workerThreads[i] = new Thread(localBlueDFSNestedDFSSharedRed);
                workerThreads[i].Start(tmp);
            }

            // wait for threads stop
            for (int i = 0; i < cores; i++)
            {
                workerThreads[i].Join();
            }

            // if any process report couterexample then report counterexample
            if (finalAcceptingCycle != null)
            {
                VerificationOutput.VerificationResult = VerificationResultType.INVALID;
                LocalTaskStack = finalLocalTaskStack;
                LocalGetCounterExample(finalAcceptingCycle, finalOutgoingTransitionTable);
            }

            // write overlap
            //writeOverlap(visitedTimes);
        }
        
        // Blue DFS
        public void localBlueDFSNestedDFSSharedRed(object o)
        {
            // get order
            int order = (int)o;

            // local data for on-the-fly Nested DFS
            Dictionary<string, List<string>> outgoingTransitionTable = new Dictionary<string, List<string>>(Ultility.Ultility.MC_INITIAL_SIZE);
            Stack<LocalPair> blueStack = new Stack<LocalPair>(5000);
            Dictionary<string, Color> colorData = new Dictionary<string, Color>(5000);
            Dictionary<string, List<LocalPair>> expendedNodes = new Dictionary<string, List<LocalPair>>(1024);

            // create initial states
            List<LocalPair> initialStates = LocalPair.GetInitialPairsLocal(BA, InitialStep);
            if (initialStates.Count == 0 || !BA.HasAcceptState)
            {
                return;
            }

            // create random variable for each thread
            Random rand = new Random(order);

            // push initial states to blueStack in random order & initialize data
            int[] permutation = generatePermutation(initialStates.Count, rand);
            for (int i = 0; i < initialStates.Count; i++)
            {
                LocalPair tmp = initialStates[permutation[i]];
                blueStack.Push(tmp);
                string ID = tmp.GetCompressedState();
                colorData.Add(ID, new Color());
                outgoingTransitionTable.Add(ID, new List<string>(8));
            }

            // start loop
            while (blueStack.Count > 0)
            {
                // cancel if too long action or couterexample reported
                if (CancelRequested || isStop)
                {
                    return;
                }

                // get the top of blueStack
                LocalPair pair = blueStack.Peek();
                ConfigurationBase LTSState = pair.configuration;
                string BAState = pair.state;
                string v = pair.GetCompressedState();

                // local data of the top
                List<string> outgoing = outgoingTransitionTable[v];
                Color vColor = colorData[v];

                // if v is not expended then expend successors of v
                if (!expendedNodes.ContainsKey(v))
                {
                    // create next states from v & add to expendedNodes
                    IEnumerable<ConfigurationBase> nextLTSStates = LTSState.MakeOneMove();
                    pair.SetEnabled(nextLTSStates, FairnessType);
                    List<LocalPair> nextStates = LocalPair.NextLocal(BA, nextLTSStates, BAState);
                    expendedNodes.Add(v, nextStates);

                    // add to visitedTimes in this thread
                    //visitedTimes.GetOrAdd(v, 0);
                    //visitedTimes[v]++;

                    // update outgoing of v and set initial data for successors
                    // no need to use inverse for statement and use nextStates
                    foreach (LocalPair next in nextStates)
                    {
                        string w = next.GetCompressedState();
                        outgoing.Add(w);
                        if (!colorData.ContainsKey(w))
                        {
                            colorData.Add(w, new Color());
                            outgoingTransitionTable.Add(w, new List<string>(8));
                        }
                    }
                }

                // get successor of v
                List<LocalPair> successors = expendedNodes[v];

                // visit v & early accepting cycle detection
                if (vColor.isWhite())
                {
                    vColor.setCyan();

                    // early cycle detection
                    // use reverse order to remove visited states
                    for (int i = successors.Count - 1; i >= 0; i--)
                    {
                        LocalPair succ = successors[i];
                        string w = succ.GetCompressedState();
                        Color wColor = colorData[w];

                        // if successor is not white then remove from expendedNodes
                        if (wColor.isCyan())
                        {
                            if (succ.state.EndsWith(Constants.ACCEPT_STATE) || pair.state.EndsWith(Constants.ACCEPT_STATE))
                            {
                                localBlueReportAcceptinCycle(succ, blueStack, outgoingTransitionTable);
                                return;
                            }
                            else
                            {
                                successors.RemoveAt(i);
                            }
                        }
                    }
                }

                // traverse all sucessors in a random order
                bool blueDone = true;
                for (int i = successors.Count - 1; i >= 0; i--)
                {
                    int randIndex = rand.Next(successors.Count);
                    LocalPair succ = successors[randIndex];
                    string w = succ.GetCompressedState();

                    // only visite white & not red globally successors
                    if (colorData[w].isWhite() && !sharedRedStates.ContainsKey(w))
                    {
                        blueStack.Push(succ);
                        successors.RemoveAt(randIndex);
                        blueDone = false;
                        break;
                    }
                    else // remove already visited states in expendedNodes
                    {
                        successors.RemoveAt(randIndex);
                    }
                }

                // if all successors are visited by blue DFS
                if (blueDone)
                {
                    // check loop at an accepting & deadlock state
                    if (pair.state.EndsWith(Constants.ACCEPT_STATE) && LTSState.IsDeadLock)
                    {
                        lock (reportLocker)
                        {
                            isStop = true;
                            Dictionary<string, LocalPair> acceptingCycle = new Dictionary<string, LocalPair>();
                            acceptingCycle.Add(v, pair);
                            blueStack.Pop();
                            finalLocalTaskStack = blueStack;
                            finalAcceptingCycle = acceptingCycle;
                            finalOutgoingTransitionTable = outgoingTransitionTable;
                            return;
                        }
                    }

                    // check whether all successors of v are already red
                    // also right for deadlock state
                    bool isAllRed = true;
                    foreach (string w in outgoing)
                    {
                        if (!sharedRedStates.ContainsKey(w))
                        {
                            isAllRed = false;
                            break;
                        }
                    }

                    // if all sucessors are red then v also red
                    if (isAllRed)
                    {
                        sharedRedStates.GetOrAdd(v, true);
                    }
                        // if v is accepting states
                    else if (pair.state.EndsWith(Constants.ACCEPT_STATE))
                    {
                        // increase count red of v by 1
                        sharedAcceptingCountRedDFS.GetOrAdd(v, 0);
                        sharedAcceptingCountRedDFS[v]++;

                        // initialize red DFS at v
                        bool stop = localRedDFSNestedDFSSharedRed(pair, blueStack, colorData, outgoingTransitionTable, rand);

                        if (stop) { return; }
                    }

                    // set v to blue
                    colorData[v].setBlue();

                    //pop v out of blueStack
                    blueStack.Pop();
                }
            }

            VerificationOutput.VerificationResult = VerificationResultType.VALID;
            return;
        }

        // Red DFS
        public bool localRedDFSNestedDFSSharedRed(LocalPair acceptingState, Stack<LocalPair> blueStack, Dictionary<string, Color> colorData, Dictionary<string, List<string>> outgoingTransitionTable, Random rand)
        {
            // local data for red DFS
            Stack<LocalPair> redStack = new Stack<LocalPair>(5000);
            Dictionary<string, List<LocalPair>> expendedNodes = new Dictionary<string, List<LocalPair>>(256);

            // push accepting state to redStack
            redStack.Push(acceptingState);

            // start loop
            while (redStack.Count > 0)
            {
                // too long action or counterexample reported
                if (CancelRequested || isStop)
                {
                    return false;
                }

                // get the top of redStack
                LocalPair pair = redStack.Peek();
                ConfigurationBase LTSState = pair.configuration;
                string BAState = pair.state;
                string v = pair.GetCompressedState();

                // local data of v in red DFS
                Color vColor = colorData[v];

                // expend v in red DFS
                if (!expendedNodes.ContainsKey(v))
                {
                    // get successors & add to expendedNodes
                    IEnumerable<ConfigurationBase> nextLTSStates = LTSState.MakeOneMove();
                    pair.SetEnabled(nextLTSStates, FairnessType);
                    List<LocalPair> nextStates = LocalPair.NextLocal(BA, nextLTSStates, BAState);
                    expendedNodes.Add(v, nextStates);
                }

                // successors of v
                List<LocalPair> successors = expendedNodes[v];

                if (!vColor.isPink())
                {
                    vColor.setPink();

                    // check accepting cycle
                    for (int i = successors.Count - 1; i >= 0; i--)
                    {
                        LocalPair succ = successors[i];
                        string w = succ.GetCompressedState();
                        Color wColor = colorData[w];

                        if (wColor.isCyan())
                        {
                            // report accepting cycle
                            localRedReportAcceptinCycle(succ, blueStack, redStack, outgoingTransitionTable);
                            return true;
                        }
                            // remove states visited in red DFS
                        else if (wColor.isPink())
                        {
                            successors.RemoveAt(i);
                        }
                    }
                }

                // traverse all successors in a random order
                bool redDone = true;
                for (int i = successors.Count - 1; i >= 0; i--)
                {
                    int randIndex = rand.Next(successors.Count);
                    LocalPair succ = successors[randIndex];
                    string w = succ.GetCompressedState();
                    Color wColor = colorData[w];

                    // if successor is cyan then report counterexample
                    if (!wColor.isPink() && !sharedRedStates.ContainsKey(w))
                    {
                        redStack.Push(succ);
                        successors.RemoveAt(randIndex);
                        redDone = false;
                        break;
                    }
                    else
                    {
                        successors.RemoveAt(randIndex);
                    }
                }

                // backtrack
                if (redDone)
                {
                    // if accepting state then decrease count
                    if (pair.state.EndsWith(Constants.ACCEPT_STATE))
                    {
                        // decrease count
                        sharedAcceptingCountRedDFS[v]--;

                        // wait for count equal 0
                        while (sharedAcceptingCountRedDFS[v] != 0) ;
                    }

                    // mark v globally red
                    sharedRedStates.GetOrAdd(v, true);

                    // pop red stack
                    redStack.Pop();
                }
            }

            // return not found counterexample
            return false;
        }

        // report accepting cycle detected in blue DFS
        public void localBlueReportAcceptinCycle(LocalPair succ, Stack<LocalPair> blueStack, Dictionary<string, List<string>> outgoingTransitionTable)
        {
            lock (reportLocker)
            {
                if (isStop)
                {
                    return;
                }

                Dictionary<string, LocalPair> acceptingCycle = new Dictionary<string, LocalPair>(1024);
                string to = succ.GetCompressedState();

                // get accepting cycle
                LocalPair tmp = blueStack.Pop();
                string tmpID = tmp.GetCompressedState();

                while (!tmpID.Equals(to))
                {
                    acceptingCycle.Add(tmpID, tmp);
                    tmp = blueStack.Pop();
                    tmpID = tmp.GetCompressedState();
                }
                acceptingCycle.Add(tmpID, tmp);

                // get final result
                finalLocalTaskStack = blueStack;
                finalAcceptingCycle = acceptingCycle;
                finalOutgoingTransitionTable = outgoingTransitionTable;

                isStop = true;
            }
        }

        // Report cycle detected at red DFS
        public void localRedReportAcceptinCycle(LocalPair succ, Stack<LocalPair> blueStack, Stack<LocalPair> redStack, Dictionary<string, List<string>> outgoingTransitionTable)
        {
            lock (reportLocker)
            {
                if (isStop)
                {
                    return;
                }

                Dictionary<string, LocalPair> acceptingCycle = new Dictionary<string, LocalPair>(1024);
                string to = succ.GetCompressedState();

                // get from redStack
                while (redStack.Count > 0)
                {
                    LocalPair tmpRed = redStack.Pop();
                    string tmpRedID = tmpRed.GetCompressedState();
                    acceptingCycle.Add(tmpRedID, tmpRed);
                }

                // pop the top of blueStack
                LocalPair topBlueStack = blueStack.Pop();
                string topBlueStackID = topBlueStack.GetCompressedState();

                if (!topBlueStackID.Equals(to))
                {
                    LocalPair tmpBlue = blueStack.Pop();
                    string tmpBlueID = tmpBlue.GetCompressedState();
                    while (!tmpBlueID.Equals(to))
                    {
                        acceptingCycle.Add(tmpBlueID, tmpBlue);
                        tmpBlue = blueStack.Pop();
                        tmpBlueID = tmpBlue.GetCompressedState();
                    }
                    acceptingCycle.Add(tmpBlueID, tmpBlue);
                }

                // get final result
                finalLocalTaskStack = blueStack;
                finalAcceptingCycle = acceptingCycle;
                finalOutgoingTransitionTable = outgoingTransitionTable;

                isStop = true;
            }
        }
    }
}