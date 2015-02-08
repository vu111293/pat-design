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
        /// <summary>
        /// Improve Tarjan-Geldenhuy algorithm
        /// Iterative version
        /// </summary>
        public void ImprovedTarjanGeldenhuysValmari()
        {
            VerificationOutput.CounterExampleTrace = null;
            // data for on-the-fly and Tarjan algorithm
            Dictionary<string, List<string>> outgoingTransitionTable = new Dictionary<string, List<string>>(Ultility.Ultility.MC_INITIAL_SIZE);
            Stack<LocalPair> callStack = new Stack<LocalPair>(5000);
            Stack<LocalPair> currentStack = new Stack<LocalPair>(1024);
            Stack<LocalPair> goalStack = new Stack<LocalPair>(256);
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

            // test
            //expendedNodesNumber = 0;

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
                // cancel if too long action
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

                // get data of the top
                List<string> outgoing = outgoingTransitionTable[v];
                int[] vData = dfsData[v];

                // if not expended then expend to next states from v
                if (!expendedNodes.ContainsKey(v))
                {
                    // create next states of v & add to expendedNodes
                    IEnumerable<ConfigurationBase> nextLTSStates = LTSState.MakeOneMove(); //.ToArray()
                    pair.SetEnabled(nextLTSStates, FairnessType);
                    List<LocalPair> nextStates = LocalPair.NextLocal(BA, nextLTSStates, BAState);
                    expendedNodes.Add(v, nextStates);

                    // increase number of states expended
                    //expendedNodesNumber++;

                    // increase transitions visited
                    VerificationOutput.Transitions += nextStates.Count;

                    // update outgoing of v and set initial data for successors
                    // no need to use inverse for statement
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

                    // check whether v is accepting
                    if (pair.state.EndsWith(Constants.ACCEPT_STATE))
                    {
                        goalStack.Push(pair);
                    }

                    // update lowlink according to successors in currentStack
                    // remove already visited successors
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

                            // check for report accepting cycle
                            if (goalStack.Count > 0 && vData[1] <= dfsData[goalStack.Peek().GetCompressedState()][0])
                            {
                                // REPORT COUNTEREXAMPLE
                                reportAcceptingCycle(succ, callStack, dfsData, outgoingTransitionTable);
                                return;
                            }
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
                    // check for loop at deadlock & accepting state
                    if (LTSState.IsDeadLock && pair.state.EndsWith(Constants.ACCEPT_STATE))
                    {
                        // report accepting cycle
                        reportAcceptingCycle(pair, callStack, dfsData, outgoingTransitionTable);
                        return;
                    }

                    if (vData[0] == vData[1])
                    {
                        LocalPair tmp = null;
                        string tmpID = null;

                        // pop currentStack and update SCC_FOUND until v
                        do
                        {
                            // remove from currentStack
                            tmp = currentStack.Pop();
                            tmpID = tmp.GetCompressedState();
                            int[] tmpData = dfsData[tmpID];
                            tmpData[0] = SCC_FOUND;

                            // remove from goalStack
                            if (goalStack.Count > 0 && tmp == goalStack.Peek())
                            {
                                goalStack.Pop();
                            }
                        } while (!tmpID.Equals(v));

                        // pop callStack
                        callStack.Pop();
                    }
                    else
                    {
                        // pop callStack & update the parent
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
            // end while loop
            VerificationOutput.VerificationResult = VerificationResultType.VALID;
            VerificationOutput.NoOfStates = dfsData.Count;
            return;
        }

        private void reportAcceptingCycle(LocalPair succ, Stack<LocalPair> callStack, Dictionary<string, int[]> dfsData, Dictionary<string, List<string>> outgoingTransitionTable)
        {
            Dictionary<string, LocalPair> acceptingCycle = new Dictionary<string, LocalPair>(1024);
            // string from = pair.GetCompressedState();
            string to = succ.GetCompressedState();

            // get states in the cycle
            LocalPair tmp = callStack.Pop();
            string tmpID = tmp.GetCompressedState();
            while (!tmpID.Equals(to))
            {
                acceptingCycle.Add(tmpID, tmp);
                tmp = callStack.Pop();
                tmpID = tmp.GetCompressedState();
            }
            acceptingCycle.Add(tmpID, tmp);

            // get the path to accepting cycle
            VerificationOutput.VerificationResult = VerificationResultType.INVALID;
            VerificationOutput.NoOfStates = dfsData.Count;
            LocalTaskStack = callStack;
            LocalGetCounterExample(acceptingCycle, outgoingTransitionTable);
        }
    }
}
