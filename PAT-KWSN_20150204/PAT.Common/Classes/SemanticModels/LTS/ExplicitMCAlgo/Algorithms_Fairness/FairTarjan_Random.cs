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
        /// <summary>
        /// Tarjan algorithm with farness checking
        /// </summary>
        public void FairTarjan()
        {
            VerificationOutput.CounterExampleTrace = null;

            //on-the-fly data
            Stack<LocalPair> callStack = new Stack<LocalPair>(5000);
            Stack<LocalPair> currentStack = new Stack<LocalPair>(5000);
            Stack<LocalPair> goalStack = new Stack<LocalPair>(1024);
            Dictionary<string, int[]> dfsNumber = new Dictionary<string, int[]>(5000);
            Dictionary<string, List<LocalPair>> expendedNodes = new Dictionary<string, List<LocalPair>>(5000);
            int number = 0;
            Random rand = new Random();

            //initial states
            List<LocalPair> initialStates = LocalPair.GetInitialPairsLocal(BA, InitialStep);

            //check valid result
            if (initialStates.Count == 0 || !BA.HasAcceptState)
            {
                VerificationOutput.VerificationResult = VerificationResultType.VALID;
                return;
            }

            //push initial states to call stack
            foreach (LocalPair tmp in initialStates)
            {
                callStack.Push(tmp);
            }

            //start loop
            while (callStack.Count > 0)
            {
                //cancel if take long time
                if (CancelRequested)
                {
                    VerificationOutput.NoOfStates = dfsNumber.Count;
                    return;
                }

                //get top of call stack
                LocalPair pair = callStack.Peek();
                ConfigurationBase LTSState = pair.configuration;
                string BAState = pair.state;
                string v = pair.GetCompressedState();

                //get successors
                List<LocalPair> successors = null;
                if (expendedNodes.ContainsKey(v))
                {
                    successors = expendedNodes[v];
                }
                else
                {
                    IEnumerable<ConfigurationBase> nextLTSStates = LTSState.MakeOneMove();
                    pair.SetEnabled(nextLTSStates, FairnessType);
                    successors = LocalPair.NextLocal(BA, nextLTSStates, BAState);
                    expendedNodes.Add(v, successors);
                }

                //if v is not number yet
                if (!dfsNumber.ContainsKey(v))
                {
                    //number v
                    int[] vData = new int[] { number, number };
                    dfsNumber.Add(v, vData);
                    number = number + 1;

                    //push to currentStack
                    currentStack.Push(pair);

                    //update lowlink for already numbered successors
                    for (int i = successors.Count - 1; i >= 0; i--)
                    {
                        LocalPair succ = successors[i];
                        string w = succ.GetCompressedState();

                        //if w is already numbered
                        if (dfsNumber.ContainsKey(w))
                        {
                            int[] wData = dfsNumber[w];

                            //if w is in current stack
                            if (wData[0] >= 0)
                            {
                                vData[1] = Math.Min(vData[1], wData[0]);
                            }
                        }
                    }
                }

                //------------------------------------------------------------------------
                //check if there is an unnumbered successor
                List<int> unvisitedIndexs = new List<int>(successors.Count);//not visited
                for (int i = successors.Count - 1; i >= 0; i--)
                {
                    LocalPair succ = successors[i];
                    string w = succ.GetCompressedState();

                    //if w is not already numbered
                    if (!dfsNumber.ContainsKey(w))
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
                    callStack.Push(succ);
                }
                else
                {
                    int[] vData = dfsNumber[v];
                    //if v is root
                    if (vData[0] == vData[1])
                    {
                        // check selfLoop
                        bool selfLoop = false;
                        foreach (LocalPair succ in successors)
                        {
                            string w = succ.GetCompressedState();
                            if (v.Equals(w))
                            {
                                selfLoop = true;
                                break;
                            }
                        }

                        //remove states from current stack and goal stack
                        Dictionary<string, LocalPair> newSCC = new Dictionary<string, LocalPair>(1024);
                        List<ConfigurationBase> cycle = new List<ConfigurationBase>(1024);
                        bool isBuchiFair = false;
                        LocalPair tmp = null;
                        string tmpID = null;
                        do
                        {
                            //current stack
                            tmp = currentStack.Pop();
                            if (!isBuchiFair && tmp.state.EndsWith(Constants.ACCEPT_STATE))
                            {
                                isBuchiFair = true;
                            }
                            tmpID = tmp.GetCompressedState();
                            newSCC.Add(tmpID, tmp);
                            cycle.Insert(0,tmp.configuration);

                            //mark visited
                            dfsNumber[tmpID][0] = SCC_FOUND;
                        } while (tmp != pair);

                        //check fairness
                        if (isBuchiFair && (selfLoop || newSCC.Count > 1 || LTSState.IsDeadLock))
                        {
                            //get outgoing transition table
                            Dictionary<string, List<string>> outgoingTransitionTable = new Dictionary<string, List<string>>(newSCC.Count);
                            foreach (KeyValuePair<string, LocalPair> kv in newSCC)
                            {
                                string s = kv.Key;
                                List<LocalPair> nextStates = expendedNodes[s];
                                List<string> outgoing = new List<string>(nextStates.Count);
                                foreach (LocalPair next in nextStates)
                                {
                                    string n = next.GetCompressedState();
                                    outgoing.Add(n);
                                }
                                outgoingTransitionTable.Add(s, outgoing);
                            }

                            Dictionary<string, LocalPair> fairSCC = IsFair(newSCC, outgoingTransitionTable);

                            if (fairSCC != null)
                            {
                                //REPORT COUNTEREXAMPLE
                                GetFairCounterExample(callStack, cycle, dfsNumber, LTSState.IsDeadLock);
                                return;
                            }
                        }

                        //pop call stack
                        callStack.Pop();
                    }
                    else
                    {
                        //pop call stack and update lowlink of top
                        LocalPair pop = callStack.Pop();
                        LocalPair top = callStack.Peek();
                        string popID = pop.GetCompressedState();
                        string topID = top.GetCompressedState();
                        int[] popData = dfsNumber[popID];
                        int[] topData = dfsNumber[topID];
                        topData[1] = Math.Min(topData[1], popData[1]);
                    }
                }
            }

            //no counter example
            VerificationOutput.VerificationResult = VerificationResultType.VALID;
            VerificationOutput.NoOfStates = dfsNumber.Count;
            return;
        }

        /// <summary>
        /// Get fair counter example
        /// </summary>
        /// <param name="callStack"></param>
        /// <param name="cycle"></param>
        /// <param name="dfsNumber"></param>
        /// <param name="isDeadLock"></param>
        public void GetFairCounterExample(Stack<LocalPair> callStack, List<ConfigurationBase> cycle, Dictionary<string, int[]> dfsNumber, bool isDeadLock)
        {
            callStack.Pop();
            int traceLen = callStack.Count;
            List<ConfigurationBase> trace = new List<ConfigurationBase>(traceLen);
            while (callStack.Count > 0)
            {
                LocalPair tmp = callStack.Pop();
                trace.Insert(0, tmp.configuration);

                if (tmp.configuration.Event == Constants.INITIAL_EVENT)
                {
                    break;
                }
            }
            if (!isDeadLock)
            {
                VerificationOutput.LoopIndex = trace.Count;
            }
            
            VerificationOutput.VerificationResult = VerificationResultType.INVALID;
            VerificationOutput.NoOfStates = dfsNumber.Count;
            trace.AddRange(cycle);
            VerificationOutput.CounterExampleTrace = trace;
        }
    }
}