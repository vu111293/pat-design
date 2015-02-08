using System;
using System.Collections.Generic;
using System.Collections;
//using System.Reactive.Concurrency;
using System.Linq;
using System.Text;
using System.Threading;
using System.Diagnostics;
using PAT.Common.Classes.BA;
using PAT.Common.Classes.DataStructure;
using PAT.Common.Classes.LTS;
using PAT.Common.Classes.ModuleInterface;
using PAT.Common.Classes.Ultility;

namespace PAT.Common.Classes.SemanticModels.LTS.Assertion
{
    public partial class AssertionLTL
    {
        private int MULTICORE_IMPROVED_TARJAN_GELDENHUYSVALMARI_NUM_THREADS = Environment.ProcessorCount;
        private StringDictionary<bool> foundSCCTarjanGeldenhuysValmari;

        /// <summary>
        /// Initiate multiple threads running improved Tarjanh-GeldenhuysValmari algorithm with shared data
        /// </summary>
        /// <returns></returns>
        public void MultiCoreImprovedTarjanGeldenhuysValmari()
        {
            //init common data of processes
            MultiCoreOutgoingTransitionTable = null;
            MultiCoreResultedLoop = null; ;
            MultiCoreLocalTaskStack = null;
            MultiCoreLock = new Object();
            StopMutliCoreThreads = false;
            foundSCCTarjanGeldenhuysValmari = new StringDictionary<bool>(5000);

            //init result & start threads
            VerificationOutput.VerificationResult = VerificationResultType.VALID;
            Thread[] workerThreads = new Thread[MULTICORE_IMPROVED_TARJAN_GELDENHUYSVALMARI_NUM_THREADS];


            //start running processes
            for (int i = 0; i < MULTICORE_IMPROVED_TARJAN_GELDENHUYSVALMARI_NUM_THREADS; i++)
            {
                workerThreads[i] = new Thread(new ThreadStart(localImprovedTarjanGeldenhuysValmari));
                workerThreads[i].Start();
            }

            //wait for threads stop
            for (int i = 0; i < MULTICORE_IMPROVED_TARJAN_GELDENHUYSVALMARI_NUM_THREADS; i++)
            {
                workerThreads[i].Join();
            }

            //if any process report couterexample then report counterexample
            if (MultiCoreResultedLoop != null)
            {
                VerificationOutput.VerificationResult = VerificationResultType.INVALID;
                LocalTaskStack = MultiCoreLocalTaskStack;
                LocalGetCounterExample(MultiCoreResultedLoop, MultiCoreOutgoingTransitionTable);
            }
        }

        /// <summary>
        /// The local function of each process running Improved MultiCore Tarjan algorithm
        /// </summary>
        /// <returns></returns>        
        public void localImprovedTarjanGeldenhuysValmari()
        {
            //local data for on-the-fly and Tarjan algorithm
            Dictionary<string, List<string>> outgoingTransitionTable = new Dictionary<string, List<string>>(Ultility.Ultility.MC_INITIAL_SIZE);
            StringDictionary<int[]> dfsData = new StringDictionary<int[]>(5000);
            Dictionary<string, List<LocalPair>> expendedNodes = new Dictionary<string, List<LocalPair>>(1024);
            Stack<LocalPair> callStack = new Stack<LocalPair>(5000);
            Stack<LocalPair> currentStack = new Stack<LocalPair>(1024);
            int counter = 0;
            string goal = null;
            int[] goalData = new int[2] { -2, 0 };
            //--------------------------

            //create initial states
            List<LocalPair> initialStates = LocalPair.GetInitialPairsLocal(BA, InitialStep);
            if (initialStates.Count == 0 || !BA.HasAcceptState)
            {
                VerificationOutput.VerificationResult = VerificationResultType.VALID;
                return;
            }

            //create random variable for each process
            Random rand = null;
            lock (MultiCoreLock)
            {
                rand = new Random(MultiCoreSeed);
                MultiCoreSeed++;
            }

            //put all initial states to callStack in different order & init data
            int[] initPermutation = generatePermutation(initialStates.Count, rand);
            for (int i = 0; i < initPermutation.Length; i++)
            {
                //get data from initialStates
                LocalPair tmp = initialStates[initPermutation[i]];
                callStack.Push(tmp);
                string tmpID = tmp.GetCompressedState();
                dfsData.Add(tmpID, new int[] { VISITED_NOPREORDER, 0 });
                outgoingTransitionTable.Add(tmpID, new List<string>(8));
            }

            //start loop
            while (callStack.Count > 0)
            {
                //cancel if too long action
                if (CancelRequested || StopMutliCoreThreads)
                {
                    return;
                }

                //get the top of callStack
                LocalPair pair = callStack.Peek();
                ConfigurationBase LTSState = pair.configuration;
                string BAState = pair.state;
                string v = pair.GetCompressedState();

                //get local data of the top
                List<string> outgoing = outgoingTransitionTable[v];
                int[] vData = dfsData.GetContainsKey(v);

                //if not expended then expend to next states from v
                if (!expendedNodes.ContainsKey(v))
                {
                    //create next states of v
                    //ConfigurationBase[] nextLTSStates = LTSState.MakeOneMove().ToArray();
                    IEnumerable<ConfigurationBase> nextLTSStates = LTSState.MakeOneMove(); //.ToArray()
                    pair.SetEnabled(nextLTSStates, FairnessType);
                    List<LocalPair> nextStates = LocalPair.NextLocal(BA, nextLTSStates, BAState);
                    expendedNodes.Add(v, nextStates);

                    //update outgoing of v and set initial data for successors
                    //no need to use inverse for statement and use nextStates
                    foreach (LocalPair next in nextStates)
                    {
                        string w = next.GetCompressedState();
                        outgoing.Add(w);
                        if (!dfsData.ContainsKey(w))
                        {
                            dfsData.Add(w, new int[] { VISITED_NOPREORDER, 0 });
                            outgoingTransitionTable.Add(w, new List<string>(8));
                        }
                    }
                }

                //get successors of v
                List<LocalPair> successors = expendedNodes[v];

                //process if v is not numbered yet
                if (vData[0] == VISITED_NOPREORDER)
                {
                    vData[0] = counter;
                    vData[1] = counter;
                    counter = counter + 1;

                    //push to currentStack
                    currentStack.Push(pair);

                    //check whether v is accepting
                    if (pair.state.EndsWith(Constants.ACCEPT_STATE))
                    {
                        goal = v;
                        goalData = vData;
                    }

                    //update lowlink according to successors in currentStack
                    //remove already visited successors
                    //no need random because consider all successors
                    for (int i = successors.Count - 1; i >= 0; i--)
                    {
                        LocalPair succ = successors[i];
                        string w = succ.GetCompressedState();
                        int[] wData = dfsData.GetContainsKey(w);
                        if (wData[0] >= 0 && !foundSCCTarjanGeldenhuysValmari.ContainsKey(w))
                        {
                            //update & remove from expendedNodes(v)
                            vData[1] = Math.Min(vData[1], wData[0]);
                            successors.RemoveAt(i);

                            //check for report accepting cycle
                            if (vData[1] <= goalData[0])
                            {
                                //REPORT COUNTEREXAMPLE
                                localReportAcceptingCycle(succ, callStack, dfsData, outgoingTransitionTable);
                                return;
                            }
                        }
                    }
                }

                //check if there is any successor not numbered & not visited by other threads
                //choose random
                bool completed = true;
                LocalPair firstUnnumbered = null;
                for (int i = successors.Count - 1; i >= 0; i--)
                {
                    int randIndex = rand.Next(successors.Count);
                    LocalPair succ = successors[randIndex];
                    string w = succ.GetCompressedState();
                    int[] wData = dfsData.GetContainsKey(w);

                    //only check states not in foundSCCTarjanGeldenhuysValmari
                    if (wData[0] == VISITED_NOPREORDER && !foundSCCTarjanGeldenhuysValmari.ContainsKey(w))
                    {
                        completed = false;
                        firstUnnumbered = succ;
                        successors.RemoveAt(randIndex);
                        break;
                    }
                    else
                    {
                        successors.RemoveAt(randIndex);
                    }
                }

                // if there at least one unnumbered successor
                if (!completed)
                {
                    callStack.Push(firstUnnumbered);
                }
                else //all successors are numbered
                {
                    //check for loop at an accepting & deadlock state
                    if (LTSState.IsDeadLock && pair.state.EndsWith(Constants.ACCEPT_STATE))
                    {
                        //report AcceptingCycle
                        localReportAcceptingCycle(pair, callStack, dfsData, outgoingTransitionTable);
                        return;
                    }

                    if (vData[0] == vData[1])
                    {
                        //find the root -> mark as local and global for all processes
                        LocalPair tmp = null;
                        string tmpID = null;

                        //pop currentStack and update SCC_FOUND and add to global memory until v
                        do
                        {
                            //local
                            tmp = currentStack.Pop();
                            tmpID = tmp.GetCompressedState();
                            int[] tmpData = dfsData.GetContainsKey(tmpID);
                            tmpData[0] = SCC_FOUND;

                            //global
                            lock (MultiCoreLock)
                            {
                                if (!foundSCCTarjanGeldenhuysValmari.ContainsKey(tmpID))
                                {
                                    foundSCCTarjanGeldenhuysValmari.Add(tmpID, true);
                                }
                            }
                        } while (!tmpID.Equals(v));

                        //pop callStack
                        callStack.Pop();
                    }
                    else
                    {
                        //pop callStack & update the parent
                        LocalPair pop = callStack.Pop();
                        LocalPair top = callStack.Peek();
                        string popID = pop.GetCompressedState();
                        string topID = top.GetCompressedState();
                        int[] popData = dfsData.GetContainsKey(popID);
                        int[] topData = dfsData.GetContainsKey(topID);
                        topData[1] = Math.Min(topData[1], popData[1]);
                    }
                }
            }
        }

        private void localReportAcceptingCycle(LocalPair succ, Stack<LocalPair> callStack, StringDictionary<int[]> dfsData, Dictionary<string, List<string>> outgoingTransitionTable)
        {
            //set flag to stop other processes
            lock (MultiCoreLock)
            {
                StopMutliCoreThreads = true;

                Dictionary<string, LocalPair> localAcceptingCycle = new Dictionary<string, LocalPair>(1024);
                string to = succ.GetCompressedState();

                //get states in the cycle
                LocalPair tmp = callStack.Pop();
                string tmpID = tmp.GetCompressedState();
                while (!tmpID.Equals(to))
                {
                    localAcceptingCycle.Add(tmpID, tmp);
                    tmp = callStack.Pop();
                    tmpID = tmp.GetCompressedState();
                }
                localAcceptingCycle.Add(tmpID, tmp);

                //return the result for global multi-core
                MultiCoreLocalTaskStack = callStack;
                MultiCoreResultedLoop = localAcceptingCycle;
                MultiCoreOutgoingTransitionTable = outgoingTransitionTable;
            }
        }
    }
}