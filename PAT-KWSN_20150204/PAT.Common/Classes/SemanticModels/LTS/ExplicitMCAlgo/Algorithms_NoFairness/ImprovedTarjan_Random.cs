using System.Collections.Generic;
using System.Diagnostics;
using PAT.Common.Classes.BA;
using PAT.Common.Classes.DataStructure;
using PAT.Common.Classes.LTS;
using PAT.Common.Classes.ModuleInterface;
using PAT.Common.Classes.Ultility;
using System;

namespace PAT.Common.Classes.SemanticModels.LTS.Assertion
{
    public partial class AssertionLTL
    {
        /// <summary>
        /// Improved Tarjan Algorithm
        /// A Note on On-The-Fly Verification Algorithms - TACAS 2005
        /// </summary>
        public void ImprovedTarjan()
        {
            VerificationOutput.CounterExampleTrace = null;

            //on-the-fly data
            Stack<LocalPair> callStack = new Stack<LocalPair>(5000);
            Stack<LocalPair> currentStack = new Stack<LocalPair>(5000);
            Stack<LocalPair> goalStack = new Stack<LocalPair>(1024);
            Dictionary<string, int[]> dfsNumber = new Dictionary<string, int[]>(5000);
            Dictionary<string, List<LocalPair>> expendedNodes = new Dictionary<string, List<LocalPair>>(5000);
            int counter = 0;
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
                    int[] vData = new int[] { counter, counter };
                    dfsNumber.Add(v, vData);
                    counter = counter + 1;

                    //push to currentStack
                    currentStack.Push(pair);

                    //check whether v is accepting
                    if (pair.state.EndsWith(Constants.ACCEPT_STATE))
                    {
                        goalStack.Push(pair);
                    }

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

                                //check for report accepting cycle
                                if (goalStack.Count > 0 && vData[1] <= dfsNumber[goalStack.Peek().GetCompressedState()][0])
                                {
                                    //REPORT COUNTEREXAMPLE
                                    GetLoopCounterExample(w, callStack, dfsNumber);
                                    return;
                                }
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
                    //deadlock at accepting state
                    if (LTSState.IsDeadLock && pair.state.EndsWith(Constants.ACCEPT_STATE))
                    {
                        //REPORT COUNTEREXAMPLE
                        GetDeadlockCounterExample(callStack, dfsNumber);
                        return;
                    }

                    int[] vData = dfsNumber[v];
                    //if v is root
                    if (vData[0] == vData[1])
                    {
                        //remove states from current stack and goal stack
                        LocalPair tmp = null;
                        string tmpID = null;
                        do
                        {
                            //current stack
                            tmp = currentStack.Pop();
                            tmpID = tmp.GetCompressedState();
                            dfsNumber[tmpID][0] = SCC_FOUND;

                            //goal stack
                            if (goalStack.Count > 0 && tmp == goalStack.Peek())
                            {
                                goalStack.Pop();
                            }
                        } while (tmp != pair);

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
        /// Get counter example in case of loop
        /// </summary>
        /// <param name="s"></param>
        /// <param name="callStack"></param>
        /// <param name="dfsData"></param>
        public void GetLoopCounterExample(string s, Stack<LocalPair> callStack, Dictionary<string, int[]> dfsData)
        {
            VerificationOutput.VerificationResult = VerificationResultType.INVALID;
            VerificationOutput.NoOfStates = dfsData.Count;
            int traceLen = callStack.Count;
            List<ConfigurationBase> trace = new List<ConfigurationBase>(traceLen);
            int count = 0;
            int reverseIndex = -1;
            while (callStack.Count > 0)
            {
                LocalPair tmp = callStack.Pop();
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
            
            VerificationOutput.CounterExampleTrace = trace;
            VerificationOutput.LoopIndex = count - 1 - reverseIndex;
        }

        /// <summary>
        /// Get counter example in case of deadlock
        /// </summary>
        /// <param name="callStack"></param>
        /// <param name="dfsData"></param>
        public void GetDeadlockCounterExample(Stack<LocalPair> callStack, Dictionary<string, int[]> dfsData)
        {
            VerificationOutput.VerificationResult = VerificationResultType.INVALID;
            VerificationOutput.NoOfStates = dfsData.Count;
            int traceLen = callStack.Count;
            List<ConfigurationBase> trace = new List<ConfigurationBase>(traceLen);
            int count = 0;
            while (callStack.Count > 0)
            {
                LocalPair tmp = callStack.Pop();
                trace.Insert(0, tmp.configuration);
                string tmpID = tmp.GetCompressedState();
                
                count++;

                if (tmp.configuration.Event == Constants.INITIAL_EVENT)
                {
                    break;
                }
            }
            VerificationOutput.CounterExampleTrace = trace;
        }
    }
}