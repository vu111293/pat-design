using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
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
        /// <summary>
        /// Improved Nested DFS 
        /// A Note on On-The-Fly Verification Algorithms - TACAS 2005
        /// Stefan Schwoon and Javier Esparza
        /// </summary>
        public void ImprovedNestedDFS()
        {
            // data for on-the-fly Nested DFS
            Dictionary<string, List<string>> outgoingTransitionTable = new Dictionary<string, List<string>>(Ultility.Ultility.MC_INITIAL_SIZE);
            Stack<LocalPair> blueStack = new Stack<LocalPair>(5000);
            Dictionary<string, Color> colorData = new Dictionary<string, Color>(5000);
            Dictionary<string, List<LocalPair>> expendedNodes = new Dictionary<string, List<LocalPair>>(1024);

            // create initial states
            List<LocalPair> initialStates = LocalPair.GetInitialPairsLocal(BA, InitialStep);
            if (initialStates.Count == 0 || !BA.HasAcceptState)
            {
                VerificationOutput.VerificationResult = VerificationResultType.VALID;
                return;
            }

            // test
            //expendedNodesNumber = 0;

            for (int i = 0; i < initialStates.Count; i++)
            {
                LocalPair tmp = initialStates[i];
                blueStack.Push(tmp);
                string ID = tmp.GetCompressedState();
                colorData.Add(ID, new Color());
                outgoingTransitionTable.Add(ID, new List<string>(8));
            }

            while (blueStack.Count > 0)
            {
                // cancel if too long action or couterexample reported
                if (CancelRequested)
                {
                    VerificationOutput.NoOfStates = colorData.Count;
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

                    // increase number of states expended
                    //expendedNodesNumber++;

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
                                blueReportAcceptinCycle(succ, blueStack, colorData, outgoingTransitionTable);
                                return;
                            }
                            else
                            {
                                successors.RemoveAt(i);
                            }
                        }
                    }
                }

                // find unvisited successor
                bool blueDone = true;
                for (int i = successors.Count - 1; i >= 0; i--)
                {
                    LocalPair succ = successors[i];
                    string w = succ.GetCompressedState();
                    if (colorData[w].isWhite())
                    {
                        blueStack.Push(succ);
                        successors.RemoveAt(i);
                        blueDone = false;
                        break;
                    }
                    else
                    {
                        successors.RemoveAt(i);
                    }
                }

                // if there is no unvisited successor
                if (blueDone)
                {
                    // if v is accepting
                    if (pair.state.EndsWith(Constants.ACCEPT_STATE))
                    {
                        if (LTSState.IsDeadLock)
                        {
                            blueReportAcceptinCycle(pair, blueStack, colorData, outgoingTransitionTable);
                            return;
                        }
                        else // start red DFS
                        {
                            bool stop = redDFSImprovedNestedDFS(pair, blueStack, colorData, outgoingTransitionTable);
                            if (stop)
                            {
                                return;
                            }
                        }

                        // SET PINK FOR ACCEPTING STATE
                        vColor.setPink();
                    }
                    else // v is not accepting
                    {
                        vColor.setBlue();
                    }

                    // pop blueStack
                    blueStack.Pop();
                }
            }

            // report no couterexample
            VerificationOutput.VerificationResult = VerificationResultType.VALID;
            VerificationOutput.NoOfStates = colorData.Count;
            return;
        }

        // RED DFS
        private bool redDFSImprovedNestedDFS(LocalPair acceptingState, Stack<LocalPair> blueStack, Dictionary<string, Color> colorData, Dictionary<string, List<string>> outgoingTransitionTable)
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
                if (CancelRequested)
                {
                    VerificationOutput.NoOfStates = colorData.Count;
                    return false;
                }

                // get the top of redStack
                LocalPair pair = redStack.Peek();
                ConfigurationBase LTSState = pair.configuration;
                string BAState = pair.state;
                string v = pair.GetCompressedState();

                // if v is not in expendedNodes of red DFS
                if (!expendedNodes.ContainsKey(v))
                {
                    // create next states from v & add to expendedNodes
                    IEnumerable<ConfigurationBase> nextLTSStates = LTSState.MakeOneMove();
                    pair.SetEnabled(nextLTSStates, FairnessType);
                    List<LocalPair> nextStates = LocalPair.NextLocal(BA, nextLTSStates, BAState);
                    expendedNodes.Add(v, nextStates);
                }

                // get successors of v
                List<LocalPair> successors = expendedNodes[v];

                // check all successors if there is at least cyan state
                for (int i = successors.Count - 1; i >= 0; i--)
                {
                    LocalPair succ = successors[i];
                    string w = succ.GetCompressedState();
                    Color wColor = colorData[w];
                    
                    // if they are cyan
                    if (wColor.isCyan())
                    {
                        // report accepting cycle
                        redReportAcceptinCycle(succ, blueStack, redStack, colorData, outgoingTransitionTable);
                        return true;
                    }
                    else if (!wColor.isBlue())
                    {
                        successors.RemoveAt(i);
                    }
                }

                // find invisited state in red DFS
                bool redDone = true;
                for (int i = successors.Count - 1; i >= 0; i--)
                {
                    LocalPair succ = successors[i];
                    string w = succ.GetCompressedState();
                    Color wColor = colorData[w];
                    
                    // if w is blue
                    if (wColor.isBlue())
                    {
                        // set w pink
                        wColor.setPink();

                        // add w to redStack
                        redStack.Push(succ);
                        successors.RemoveAt(i);
                        redDone = false;
                        break;
                    }
                    else
                    {
                        successors.RemoveAt(i);
                    }
                }
                
                // if there is no blue successors
                if (redDone)
                {
                    redStack.Pop();
                }
            }
            return false;
        }

        // Report accepting cycle in blue DFS
        private void blueReportAcceptinCycle(LocalPair succ, Stack<LocalPair> blueStack, Dictionary<string, Color> colorData, Dictionary<string, List<string>> outgoingTransitionTable)
        {
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

            VerificationOutput.VerificationResult = VerificationResultType.INVALID;
            VerificationOutput.NoOfStates = colorData.Count;
            LocalTaskStack = blueStack;
            LocalGetCounterExample(acceptingCycle, outgoingTransitionTable);
        }

        // Report accepting cycle in red DFS
        private void redReportAcceptinCycle(LocalPair succ, Stack<LocalPair> blueStack, Stack<LocalPair> redStack, Dictionary<string, Color> colorData, Dictionary<string, List<string>> outgoingTransitionTable)
        {
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

            VerificationOutput.VerificationResult = VerificationResultType.INVALID;
            VerificationOutput.NoOfStates = colorData.Count;
            LocalTaskStack = blueStack;
            LocalGetCounterExample(acceptingCycle, outgoingTransitionTable);
        }
    }
}
