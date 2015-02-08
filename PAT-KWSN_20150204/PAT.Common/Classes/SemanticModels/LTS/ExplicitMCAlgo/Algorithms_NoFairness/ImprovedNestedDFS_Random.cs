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
        /// Improved Nested DFS
        /// A Note on On-The-Fly Verification Algorithms - TACAS 2005
        /// Blue DFS
        /// </summary>
        public void BlueDFS()
        {
            VerificationOutput.CounterExampleTrace = null;

            //on-the-fly data
            Stack<LocalPair> blueStack = new Stack<LocalPair>(5000);
            Dictionary<string, Color> dfsColor = new Dictionary<string, Color>(5000);
            Dictionary<string, List<LocalPair>> expendedNodes = new Dictionary<string, List<LocalPair>>(5000);
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
                blueStack.Push(tmp);
            }

            //start loop
            while (blueStack.Count > 0)
            {
                //cancel if take long time
                if (CancelRequested)
                {
                    VerificationOutput.NoOfStates = dfsColor.Count;
                    return;
                }

                //get top of call stack
                LocalPair pair = blueStack.Peek();
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

                //if v is white
                if (!dfsColor.ContainsKey(v))
                {
                    //set v cyan
                    Color vColor = new Color();
                    vColor.setCyan();
                    dfsColor.Add(v, vColor);

                    //early cycle detection
                    for (int i = successors.Count - 1; i >= 0; i--)
                    {
                        LocalPair succ = successors[i];
                        string w = succ.GetCompressedState();

                        //if w is cyan and (v or w is accepting)
                        if (dfsColor.ContainsKey(w))
                        {
                            Color wColor = dfsColor[w];
                            if (wColor.isCyan() && (succ.state.EndsWith(Constants.ACCEPT_STATE) || pair.state.EndsWith(Constants.ACCEPT_STATE)))
                            {
                                //REPORT COUNTEREXAMPLE
                                GetLoopCounterExample(w, blueStack, dfsColor);
                                return;
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

                    //if w is white
                    if (!dfsColor.ContainsKey(w))
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
                    blueStack.Push(succ);
                }
                else
                {
                    Color vColor = dfsColor[v];

                    //if v is accepting
                    if (pair.state.EndsWith(Constants.ACCEPT_STATE))
                    {
                        //if v is deadlock
                        if (LTSState.IsDeadLock)
                        {
                            //REPORT COUNTEREXAMPLE
                            GetDeadlockCounterExample(blueStack, dfsColor);
                            return;
                        }
                        else
                        {
                            bool stop = RedDFS(pair, blueStack, dfsColor, expendedNodes, rand);
                            if (stop)
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

                    //pop blue stack
                    blueStack.Pop();
                }
            }

            //no counter example
            VerificationOutput.VerificationResult = VerificationResultType.VALID;
            VerificationOutput.NoOfStates = dfsColor.Count;
            return;
        }

        /// <summary>
        /// Red DFS
        /// </summary>
        /// <param name="acceptingState"></param>
        /// <param name="blueStack"></param>
        /// <param name="dfsColor"></param>
        /// <returns></returns>
        public bool RedDFS(LocalPair acceptingState, Stack<LocalPair> blueStack, Dictionary<string, Color> dfsColor, Dictionary<string, List<LocalPair>> expendedNodes, Random rand)
        {
            Stack<LocalPair> redStack = new Stack<LocalPair>(5000);
            
            //push accepting state to red stack
            redStack.Push(acceptingState);

            //start loop
            while (redStack.Count > 0)
            {
                //cancel if take long time
                if (CancelRequested)
                {
                    VerificationOutput.NoOfStates = dfsColor.Count;
                    return false;
                }

                //get top of red stack
                LocalPair pair = redStack.Peek();
                string v = pair.GetCompressedState();

                //get successors
                List<LocalPair> successors = expendedNodes[v];

                //check if there is cyan successor
                for (int i = successors.Count - 1; i >= 0; i--)
                {
                    LocalPair succ = successors[i];
                    string w = succ.GetCompressedState();

                    Color wColor = dfsColor[w];
                    if (wColor.isCyan())
                    {
                        //REPORT COUNTEREXAMPLE
                        GetLoopCounterExample(w, blueStack, redStack, dfsColor);
                        return true;
                    }
                }

                //find a blue successor
                List<int> unvisitedIndexs = new List<int>(successors.Count);//not visited
                for (int i = successors.Count - 1; i >= 0; i--)
                {
                    LocalPair succ = successors[i];
                    string w = succ.GetCompressedState();
                    Color wColor = dfsColor[w];
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
                    Color wColor = dfsColor[w];

                    wColor.setPink();
                    redStack.Push(succ);
                }
                else
                {
                    redStack.Pop();
                }
            }
            return false;
        }

        /// <summary>
        /// Get counter example in case of loop in red DFS
        /// </summary>
        /// <param name="s"></param>
        /// <param name="blueStack"></param>
        /// <param name="redStack"></param>
        /// <param name="dfsColor"></param>
        public void GetLoopCounterExample(string s, Stack<LocalPair> blueStack, Stack<LocalPair> redStack, Dictionary<string, Color> dfsColor)
        {
            VerificationOutput.VerificationResult = VerificationResultType.INVALID;
            VerificationOutput.NoOfStates = dfsColor.Count;
            int traceLen = redStack.Count + blueStack.Count - 1;
            List<ConfigurationBase> trace = new List<ConfigurationBase>(traceLen);
            int count = 0;
            int reverseIndex = -1;

            //get trace from red stack
            while (redStack.Count > 1)
            {
                LocalPair tmpRed = redStack.Pop();
                trace.Insert(0, tmpRed.configuration);
                count++;
            }

            //get trace from blue stack
            while (blueStack.Count > 0)
            {
                LocalPair tmpBlue = blueStack.Pop();
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

            VerificationOutput.CounterExampleTrace = trace;
            VerificationOutput.LoopIndex = count - 1 - reverseIndex;
        }

        /// <summary>
        /// Get counter example in case of deadlock in blue DFS
        /// </summary>
        /// <param name="blueStack"></param>
        /// <param name="dfsColor"></param>
        public void GetDeadlockCounterExample(Stack<LocalPair> blueStack, Dictionary<string, Color> dfsColor)
        {
            VerificationOutput.VerificationResult = VerificationResultType.INVALID;
            VerificationOutput.NoOfStates = dfsColor.Count;
            int traceLen = blueStack.Count;
            List<ConfigurationBase> trace = new List<ConfigurationBase>(traceLen);
            int count = 0;
            while (blueStack.Count > 0)
            {
                LocalPair tmp = blueStack.Pop();
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

        /// <summary>
        /// Get counter example in case of loop in blue DFS
        /// </summary>
        /// <param name="s"></param>
        /// <param name="blueStack"></param>
        /// <param name="dfsColor"></param>
        public void GetLoopCounterExample(string s, Stack<LocalPair> blueStack, Dictionary<string, Color> dfsColor)
        {
            VerificationOutput.VerificationResult = VerificationResultType.INVALID;
            VerificationOutput.NoOfStates = dfsColor.Count;
            int traceLen = blueStack.Count;
            List<ConfigurationBase> trace = new List<ConfigurationBase>(traceLen);
            int count = 0;
            int reverseIndex = -1;
            while (blueStack.Count > 0)
            {
                LocalPair tmp = blueStack.Pop();
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
    }
}