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
        /// Improved Multi-Core Nested Depth-First Search - ATVA 2012
        /// Sami Evangelista, Alfons Laarman, Laure Petrucci, and Jaco van de Pol
        /// </summary>
        public void MultiCoreCombinationNestedDFS_DotNet4()
        {
            // set number of cores
            int cores = setCores(4);

            // initalize common data
            finalOutgoingTransitionTable = null;
            finalAcceptingCycle = null;
            finalLocalTaskStack = null;
            sharedBlueRedStates = new ConcurrentDictionary<string, bool>(cores, 5000);
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
                workerThreads[i] = new Thread(localBlueDFSCombinationNestedDFS);
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
        public void localBlueDFSCombinationNestedDFS(object o)
        {
            // get order
            int order = (int)o;

            // local data of each thread for on-the-fly Nested DFS algorithm
            Dictionary<string, List<string>> outgoingTransitionTable = new Dictionary<string, List<string>>(Ultility.Ultility.MC_INITIAL_SIZE);
            Stack<LocalPair> blueStack = new Stack<LocalPair>(5000);
            Dictionary<string, bool> cyanData = new Dictionary<string, bool>(5000);
            Dictionary<string, List<LocalPair>> expendedNodes = new Dictionary<string, List<LocalPair>>(1024);

            // create initial states
            List<LocalPair> initialStates = LocalPair.GetInitialPairsLocal(BA, InitialStep);
            if (initialStates.Count == 0 || !BA.HasAcceptState)
            {
                return;
            }

            // create random variable for each process
            Random rand = new Random(order);

            // push initial states to blueStack in random order & init data
            int[] permutation = generatePermutation(initialStates.Count, rand);
            for (int i = 0; i < initialStates.Count; i++)
            {
                LocalPair tmp = initialStates[permutation[i]];
                blueStack.Push(tmp);
                string ID = tmp.GetCompressedState();
                cyanData.Add(ID, false);
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

                // get local data of the top
                List<string> outgoing = outgoingTransitionTable[v];
                bool isVCyan = cyanData[v];

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

                    // update outgoing of v and set cyan color false
                    // no need to use inverse for statement and use nextStates
                    foreach (LocalPair next in nextStates)
                    {
                        string w = next.GetCompressedState();
                        outgoing.Add(w);
                        if (!cyanData.ContainsKey(w))
                        {
                            cyanData.Add(w, false);
                            outgoingTransitionTable.Add(w, new List<string>(8));
                        }
                    }
                }

                // get successor of v
                List<LocalPair> successors = expendedNodes[v];

                // if v is not cyan
                if (!isVCyan)
                {
                    cyanData[v] = true;

                    for (int i = successors.Count - 1; i >= 0; i--)
                    {
                        LocalPair succ = successors[i];
                        string w = succ.GetCompressedState();
                        bool isWCyan = cyanData[w];

                        if (isWCyan)
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

                // traverse all sucessors in a random order to find unvisited state
                bool blueDone = true;
                for (int i = successors.Count - 1; i >= 0; i--)
                {
                    int randIndex = rand.Next(successors.Count);
                    LocalPair succ = successors[randIndex];
                    string w = succ.GetCompressedState();
                    bool isWCyan = cyanData[w];

                    // if w is not cyan & not global blue
                    // if w is in sharedBlueRedStates then w is global Blue
                    // if sharedBlueRedStates[w] is true then w is also global Red
                    if (!isWCyan && !sharedBlueRedStates.ContainsKey(w))
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
                    // mark v as global blue & false to indicate not globally red yet
                    sharedBlueRedStates.GetOrAdd(v, false);

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
                        // not in shareBlueRedStates or in but not yet red
                        if (!sharedBlueRedStates.ContainsKey(w) || !sharedBlueRedStates[w])
                        {
                            isAllRed = false;
                            break;
                        }
                    }

                    // if all sucessors are red then v also red
                    if (isAllRed)
                    {
                        sharedBlueRedStates[v] = true;
                    }
                    // if v is accepting state
                    else if (pair.state.EndsWith(Constants.ACCEPT_STATE))
                    {
                        Dictionary<LocalPair, bool> rSet = new Dictionary<LocalPair, bool>(1024);

                        // initialize red DFS at v
                        bool stop = localRedDFSCombinationNestedDFS(rSet, pair, blueStack, cyanData, outgoingTransitionTable, rand);
                        if (stop) { return; }

                        // wait for all accepting states EXCEPT v in rSet become red
                        foreach (KeyValuePair<LocalPair, bool> kv in rSet)
                        {
                            string s = kv.Key.GetCompressedState();
                            if (kv.Key.state.EndsWith(Constants.ACCEPT_STATE) && !s.Equals(v))
                            {
                                while (!sharedBlueRedStates[s]) { };
                            }
                        }

                        // set all states in rSet to red
                        // states already blue and in shareBlueRedStates blue
                        foreach (KeyValuePair<LocalPair, bool> kv in rSet)
                        {
                            string kvID = kv.Key.GetCompressedState();
                            sharedBlueRedStates[kvID] = true;
                        }
                    }
                        
                    //pop v out of blueStack
                    blueStack.Pop();

                    // uncyan v
                    cyanData[v] = false;
                }
            }

            VerificationOutput.VerificationResult = VerificationResultType.VALID;
            return;
        }

        // Red DFS
        public bool localRedDFSCombinationNestedDFS(Dictionary<LocalPair, bool> rSet, LocalPair acceptingState, Stack<LocalPair> blueStack, Dictionary<string, bool> cyanData, Dictionary<string, List<string>> outgoingTransitionTable, Random rand)
        {
            // local data for red DFS
            Stack<LocalPair> redStack = new Stack<LocalPair>(5000);
            Dictionary<string, bool> inRSet = new Dictionary<string, bool>(1024);
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
                bool isVInRSet = inRSet.ContainsKey(v);
                
                // if v is not expended in red DFS
                if (!expendedNodes.ContainsKey(v))
                {
                    // get successors & add to expendedNodes
                    IEnumerable<ConfigurationBase> nextLTSStates = LTSState.MakeOneMove();
                    pair.SetEnabled(nextLTSStates, FairnessType);
                    List<LocalPair> nextStates = LocalPair.NextLocal(BA, nextLTSStates, BAState);
                    expendedNodes.Add(v, nextStates);
                }

                // get successor of v
                List<LocalPair> successors = expendedNodes[v];

                // if v is not pink then add to rSet
                if (!isVInRSet)
                {
                    // add v to R
                    rSet.Add(pair, true);
                    inRSet.Add(v, true);

                    // check for accepting cycle
                    for (int i = successors.Count - 1; i >= 0; i--)
                    {
                        LocalPair succ = successors[i];
                        string w = succ.GetCompressedState();
                        //bool isWCyan = cyanData[w];

                        // if w is cyan then report accepting cycle
                        // push to redStack states not in R set & not red, so may push states not in the thread's cyanData
                        if (cyanData.ContainsKey(w) && cyanData[w])
                        {
                            // report accepting cycle
                            localRedReportAcceptinCycle(succ, blueStack, redStack, outgoingTransitionTable);
                            return true;
                        }
                        else if (inRSet.ContainsKey(w))
                        {
                            successors.RemoveAt(i);
                        }
                    }
                }

                // traverse all successors in a random order to find successor not in R & not global red
                bool redDone = true;
                for (int i = successors.Count - 1; i >= 0; i--)
                {
                    int randIndex = rand.Next(successors.Count);
                    LocalPair succ = successors[randIndex];
                    string w = succ.GetCompressedState();

                    // if w is not in rSet & not global red then add w to redStack
                    if (!inRSet.ContainsKey(w) && !sharedBlueRedStates[w])
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
                    //pop red stack
                    redStack.Pop();
                }
            }

            // return not found counterexample
            return false;
        }
    }
}