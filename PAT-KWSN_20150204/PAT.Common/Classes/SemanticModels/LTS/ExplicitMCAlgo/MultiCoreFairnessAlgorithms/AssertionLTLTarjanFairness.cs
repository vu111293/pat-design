using System;
using System.Collections.Generic;
using System.Collections;
//using System.Reactive.Concurrency;
using System.Linq;
using System.Text;
using System.Threading;
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
        // local function of each thread      
        public void TarjanFairnessChecking()
        {
            // local data for on-the-fly and Tarjan algorithm
            Dictionary<string, List<string>> outgoingTransitionTable = new Dictionary<string, List<string>>(Ultility.Ultility.MC_INITIAL_SIZE);
            Stack<LocalPair> callStack = new Stack<LocalPair>(5000);
            Stack<LocalPair> currentStack = new Stack<LocalPair>(1024);
            Dictionary<string, int[]> dfsData = new Dictionary<string, int[]>(5000);
            Dictionary<string, List<LocalPair>> expendedNodes = new Dictionary<string, List<LocalPair>>(1024);
            int counter = 0;
            //--------------------------

            // create initial states
            List<LocalPair> initialStates = LocalPair.GetInitialPairsLocal(BA, InitialStep);
            if (initialStates.Count == 0 || !BA.HasAcceptState)
            {
                VerificationOutput.VerificationResult = VerificationResultType.VALID;
                return;
            }

            // put all initial states to callStack & init data
            foreach (LocalPair tmp in initialStates)
            {
                callStack.Push(tmp);
                string tmpID = tmp.GetCompressedState();
                dfsData.Add(tmpID, new int[] { VISITED_NOPREORDER, 0 });
                outgoingTransitionTable.Add(tmpID, new List<string>(8));
            }

            // start loop
            while (callStack.Count > 0)
            {
                // cancel if too long action or any counterexample is reported
                if (CancelRequested)
                {
                    VerificationOutput.NoOfStates = dfsData.Count;
                    return;
                }

                // get the top of callStack
                LocalPair pair = callStack.Peek();
                ConfigurationBase LTSState = pair.configuration;
                string BAState = pair.state;
                string v = pair.GetCompressedState();

                // get local data of the top
                List<string> outgoing = outgoingTransitionTable[v];
                int[] vData = dfsData[v];

                // if not expended then expend to next states from v
                if (!expendedNodes.ContainsKey(v))
                {
                    // create next states of v
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
                        if (!dfsData.ContainsKey(w))
                        {
                            dfsData.Add(w, new int[] { VISITED_NOPREORDER, 0 });
                            outgoingTransitionTable.Add(w, new List<string>(8));
                        }
                    }
                }

                // get successors of v
                List<LocalPair> successors = expendedNodes[v];

                // process if v is not numbered yet
                if (vData[0] == VISITED_NOPREORDER)
                {
                    vData[0] = counter;
                    vData[1] = counter;
                    counter = counter + 1;

                    // push to currentStack
                    currentStack.Push(pair);

                    // update lowlink according to successors in currentStack
                    // remove already visited successors
                    // no need to use random
                    for (int i = successors.Count - 1; i >= 0; i--)
                    {
                        LocalPair succ = successors[i];
                        string w = succ.GetCompressedState();
                        int[] wData = dfsData[w];

                        if (wData[0] >= 0)
                        {
                            // update & remove from expendedNodes(v)
                            vData[1] = Math.Min(vData[1], wData[0]);
                            successors.RemoveAt(i);
                        }
                    }
                }

                // check if there is any successor not numbered
                bool completed = true;
                for (int i = successors.Count - 1; i >= 0; i--)
                {
                    LocalPair succ = successors[i];
                    string w = succ.GetCompressedState();
                    int[] wData = dfsData[w];
                    if (wData[0] == VISITED_NOPREORDER)
                    {
                        callStack.Push(succ);
                        successors.RemoveAt(i);
                        completed = false;
                        break;
                    }
                    else
                    {
                        successors.RemoveAt(i);
                    }
                }

                // if all successors are visited
                if (completed)
                {
                    // check root
                    if (vData[0] == vData[1])
                    {
                        //pop callStack
                        callStack.Pop();

                        Dictionary<string, LocalPair> newSCC = new Dictionary<string, LocalPair>(1024);

                        // check selfLoop
                        bool selfLoop = false;
                        foreach (string w in outgoing)
                        {
                            if (v.Equals(w))
                            {
                                selfLoop = true;
                                break;
                            }
                        }

                        // get all states in a SCC & check Buchi fair
                        bool isBuchiFair = false;
                        LocalPair tmp = null;
                        string tmpID = null;
                        // get from current until v
                        do
                        {
                            tmp = currentStack.Pop();
                            isBuchiFair = isBuchiFair || tmp.state.EndsWith(Constants.ACCEPT_STATE);
                            tmpID = tmp.GetCompressedState();
                            int[] tmpData = dfsData[tmpID];
                            tmpData[0] = SCC_FOUND;
                            newSCC.Add(tmpID, tmp);
                        } while (!tmpID.Equals(v));

                        if (isBuchiFair && (selfLoop || newSCC.Count > 1 || LTSState.IsDeadLock))
                        {
                            Dictionary<string, LocalPair> fairSCC = IsFair(newSCC, outgoingTransitionTable);
                            if (fairSCC != null)
                            {
                                // get the path to accepting cycle
                                VerificationOutput.VerificationResult = VerificationResultType.INVALID;
                                VerificationOutput.NoOfStates = dfsData.Count;
                                LocalTaskStack = callStack;
                                LocalGetCounterExample(fairSCC, outgoingTransitionTable);
                                return;
                            }
                        }
                    }
                    else
                    {
                        //pop callStack & update the parent
                        LocalPair pop = callStack.Pop();
                        LocalPair top = callStack.Peek();
                        string popID = pop.GetCompressedState();
                        string topID = top.GetCompressedState();
                        int[] popData = dfsData[popID];
                        int[] topData = dfsData[topID];
                        topData[1] = Math.Min(topData[1], popData[1]);
                    }
                }
            }
            VerificationOutput.VerificationResult = VerificationResultType.VALID;
            VerificationOutput.NoOfStates = dfsData.Count;
        }
    }
}