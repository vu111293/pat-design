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
        /// Run the verification using the Nested DFS algorithm and get the result.
        /// Based on:
        /// A Note on On-The-Fly Verification Algorithms
        /// http://dl.acm.org/citation.cfm?id=2140670
        /// Stefan Schwoon and Javier Esparza
        /// Proceeding TACAS'05 Proceedings of the 11th international conference on 
        /// Tools and Algorithms for the Construction and Analysis of Systems
        /// Springer-Verlag Berlin, Heidelberg, 2005
        /// </summary>
        /// <returns></returns>        
        public void NestedDFSModelChecking()
        {
            Dictionary<string, List<string>> OutgoingTransitionTable = new Dictionary<string, List<string>>(Ultility.Ultility.MC_INITIAL_SIZE);
            VerificationOutput.CounterExampleTrace = null;

            List<LocalPair> initials = LocalPair.GetInitialPairsLocal(BA, InitialStep);

            if (initials.Count == 0)
            {
                VerificationOutput.VerificationResult = VerificationResultType.VALID;
                return;
            }

            Stack<LocalPair> BlueStack = new Stack<LocalPair>(5000);
            StringDictionary<StateColor> colorData = new StringDictionary<StateColor>(5000);

            for (int z = 0; z < initials.Count; z++)
            {
                LocalPair initState = initials[z];
                BlueStack.Push(initState);
                string ID = initState.GetCompressedState();
                colorData.Add(ID, new StateColor());
                OutgoingTransitionTable.Add(ID, new List<string>(8));
            }

            //store the expended event step of a node to avoid multiple invocation of the make one move.
            Dictionary<string, List<LocalPair>> ExpendedNode = new Dictionary<string, List<LocalPair>>(1024);

            do
            {
                if (CancelRequested)
                {
                    VerificationOutput.NoOfStates = colorData.Count; // VisitedWithID.Count;
                    return;
                }

                LocalPair pair = BlueStack.Peek();
                ConfigurationBase evt = pair.configuration;
                string BAState = pair.state;
                
                string v = pair.GetCompressedState();

                List<string> outgoing = OutgoingTransitionTable[v];

                StateColor nodeColor = colorData.GetContainsKey(v);

                if (nodeColor.IsWhite())
                {
                    nodeColor.SetCyan();
                }

                bool blueDone = true;

                if (ExpendedNode.ContainsKey(v))
                {
                    List<LocalPair> list = ExpendedNode[v];

                    if (list.Count > 0)
                    {
                        //transverse all neighbour nodes
                        for (int k = list.Count - 1; k >= 0; k--)
                        {
                            LocalPair step = list[k];
                            string tmp = step.GetCompressedState();
                            StateColor neighbourColor = colorData.GetContainsKey(tmp);

                            //if the neighbour node is white 
                            if (neighbourColor.IsWhite())
                            {
                                //only add the first unvisited node
                                //for the second or more unvisited steps, ignore at the monent
                                if (blueDone)
                                {
                                    BlueStack.Push(step);
                                    blueDone = false;
                                    list.RemoveAt(k);
                                }
                            }

                            // if the neighbour node is cyan,
                            // and either this node or the neibour node is the accept state
                            // then report cycle
                            else if (neighbourColor.IsCyan())
                            {
                                if (step.state.EndsWith(Constants.ACCEPT_STATE) || pair.state.EndsWith(Constants.ACCEPT_STATE))
                                {
                                    // Report cycle
                                    ReportBlueCycle(step, pair, BlueStack, colorData, OutgoingTransitionTable);
                                    return;
                                }
                                else
                                {
                                    list.RemoveAt(k);
                                }
                            }
                            // if the neighbour node is either blue or red, 
                            // can remove from the list
                            else
                            {
                                list.RemoveAt(k);
                            }

                        }
                    }
                }
                else
                {
                    //ConfigurationBase[] list = evt.MakeOneMove().ToArray();
                    IEnumerable<ConfigurationBase> list = evt.MakeOneMove();
                    pair.SetEnabled(list, FairnessType);
                    List<LocalPair> product = LocalPair.NextLocal(BA, list, BAState);

                    //count the transitions visited
                    VerificationOutput.Transitions += product.Count;

                    for (int k = product.Count - 1; k >= 0; k--)
                    {
                        LocalPair step = product[k];
                        string tmp = step.GetCompressedState();
                        StateColor neighbourColor = colorData.GetContainsKey(tmp);

                        if (neighbourColor != null)
                        {
                            //update the incoming and outgoing edges
                            outgoing.Add(tmp);

                            //if this node is still not visited
                            if (neighbourColor.IsWhite())
                            {
                                //only add the first unvisited node
                                //for the second or more unvisited steps, ignore at the monent
                                if (blueDone)
                                {
                                    BlueStack.Push(step);
                                    blueDone = false;
                                    product.RemoveAt(k);
                                }
                            }
                            // if the neighbour node is cyan,
                            // and either this node or the neibour node is the accept state
                            // then report cycle
                            else if (neighbourColor.IsCyan())
                            {
                                if (step.state.EndsWith(Constants.ACCEPT_STATE) || pair.state.EndsWith(Constants.ACCEPT_STATE))
                                {
                                    // Report cycle
                                    ReportBlueCycle(step, pair, BlueStack, colorData, OutgoingTransitionTable);
                                    return;
                                }
                                else
                                {
                                    product.RemoveAt(k);
                                }
                            }
                            // if the neighbour node is either blue or red, 
                            // can remove from the list
                            else
                            {
                                product.RemoveAt(k);
                            }
                        }
                        
                        // If the node is not initiated
                        else
                        {
                            colorData.Add(tmp, new StateColor());
                            OutgoingTransitionTable.Add(tmp, new List<string>(8));
                            outgoing.Add(tmp);

                            if (blueDone)
                            {
                                BlueStack.Push(step);
                                blueDone = false;
                                product.RemoveAt(k);
                            }
                        }
                    }

                    //create the remaining steps as the expending list for v
                    ExpendedNode.Add(v, product);
                }

                if (blueDone)
                {
                    BlueStack.Pop();

                    // If the current node is an accept state,
                    // do the red DFS
                    if (pair.state.EndsWith(Constants.ACCEPT_STATE))
                    {
                        if (evt.IsDeadLock)
                        {
                            VerificationOutput.VerificationResult = VerificationResultType.INVALID;
                            VerificationOutput.NoOfStates = colorData.Count;
                            Dictionary<string, LocalPair> LoopPairs = new Dictionary<string, LocalPair>();
                            LoopPairs.Add(v, pair);
                            LocalTaskStack = BlueStack;
                            LocalGetCounterExample(LoopPairs, OutgoingTransitionTable);
                            return;
                        }

                        bool stop = DepthFirstSearchRed(pair, BlueStack, OutgoingTransitionTable, colorData);

                        if (stop)
                            return;

                        nodeColor.SetRed();
                    }
                    else
                    {
                        nodeColor.SetBlue();
                    }
                }

            } while (BlueStack.Count > 0);

            VerificationOutput.VerificationResult = VerificationResultType.VALID;
            VerificationOutput.NoOfStates = colorData.Count;
            return;
        }

        // Report cycle detected at blue DFS
        public void ReportBlueCycle(LocalPair step, LocalPair pair, Stack<LocalPair> BlueStack, StringDictionary<StateColor> colorData, Dictionary<string, List<string>> OutgoingTransitionTable)
        {
            Dictionary<string, LocalPair> LoopPairs = new Dictionary<string, LocalPair>(1024);
            string tmp = step.GetCompressedState();
            string v = pair.GetCompressedState();


            LocalPair node = BlueStack.Pop();
            string nodeID = node.GetCompressedState();

            while (!nodeID.Equals(tmp))
            {
                LoopPairs.Add(nodeID, node);
                node = BlueStack.Pop();
                nodeID = node.GetCompressedState();
            }

            LoopPairs.Add(nodeID, node);

            VerificationOutput.VerificationResult = VerificationResultType.INVALID;
            VerificationOutput.NoOfStates = colorData.Count;
            LocalTaskStack = BlueStack;
            LocalGetCounterExample(LoopPairs, OutgoingTransitionTable);
        }

        public bool DepthFirstSearchRed(LocalPair s, Stack<LocalPair> BlueStack, Dictionary<string, List<string>> OutgoingTransitionTable, StringDictionary<StateColor> colorData)
        {

            //store the expended event step of a node to avoid multiple invocation of the make one move.
            Dictionary<string, List<LocalPair>> ExpendedNode = new Dictionary<string, List<LocalPair>>(256);
            
            Stack<LocalPair> RedStack = new Stack<LocalPair>(5000);
            RedStack.Push(s);

            do
            {
                if (CancelRequested)
                {
                    VerificationOutput.NoOfStates = colorData.Count; // VisitedWithID.Count;
                    return false;
                }

                LocalPair pair = RedStack.Peek();
                string v = pair.GetCompressedState();
                ConfigurationBase evt = pair.configuration;
                string BAState = pair.state;

                List<string> outgoing = OutgoingTransitionTable[v];

                bool redDone = true;

                if (!ExpendedNode.ContainsKey(v))
                {
                    //ConfigurationBase[] list = evt.MakeOneMove().ToArray();
                    IEnumerable<ConfigurationBase> list = evt.MakeOneMove();
                    pair.SetEnabled(list, FairnessType);
                    List<LocalPair> product = LocalPair.NextLocal(BA, list, BAState);
                    ExpendedNode.Add(v, product);
                }

                List<LocalPair> neighbourList = ExpendedNode[v];
                if (neighbourList.Count > 0)
                {
                    //transverse all neighbour nodes
                    for (int k = neighbourList.Count - 1; k >= 0; k--)
                    {
                        LocalPair step = neighbourList[k];
                        string tmp = step.GetCompressedState();
                        StateColor neighbourColor = colorData.GetContainsKey(tmp);

                        // if the neighbour node is blue
                        if (neighbourColor.IsBlue())
                        {
                            //only add the first unvisited node
                            //for the second or more unvisited steps, ignore at the monent
                            if (redDone)
                            {
                                neighbourColor.SetRed();
                                RedStack.Push(step);
                                redDone = true;
                                neighbourList.RemoveAt(k);
                            }
                        }
                        // if the neighbour is cyan
                        // report cycle
                        else if (neighbourColor.IsCyan())
                        {
                            ReportRedCycle(s, step, pair, BlueStack, RedStack, colorData, OutgoingTransitionTable);
                            return true;
                        }
                        else
                        {
                            neighbourList.RemoveAt(k);
                        }
                    }
                }

                if (redDone)
                {
                    RedStack.Pop();
                }

            } while (RedStack.Count > 0);

            return false;
        }


        // Report cycle detected at red DFS
        public void ReportRedCycle(LocalPair s, LocalPair step, LocalPair pair, Stack<LocalPair> BlueStack, Stack<LocalPair> RedStack, StringDictionary<StateColor> colorData, Dictionary<string, List<string>> OutgoingTransitionTable)
        {
            Dictionary<string, LocalPair> LoopPairs = new Dictionary<string, LocalPair>(1024);
            string tmp = step.GetCompressedState();
            string v = pair.GetCompressedState();
            string sID = s.GetCompressedState();

            LocalPair node;
            string nodeID;

            // If s is start of the loop
            if (sID.Equals(tmp))
            {
                do
                {
                    node = RedStack.Pop();
                    nodeID = node.GetCompressedState();
                    LoopPairs.Add(nodeID, node);
                } while (RedStack.Count > 0);
            }
            // If the start of the loop is the parent of s
            else
            { 
                do
                {
                    node = RedStack.Pop();
                    nodeID = node.GetCompressedState();
                    LoopPairs.Add(nodeID, node);
                } while (RedStack.Count > 0);

                node = BlueStack.Pop();
                nodeID = node.GetCompressedState();

                while (!nodeID.Equals(tmp))
                {
                    LoopPairs.Add(nodeID, node);
                    node = BlueStack.Pop();
                    nodeID = node.GetCompressedState();
                }

                LoopPairs.Add(nodeID, node);
            }

            LocalTaskStack = BlueStack;
            VerificationOutput.VerificationResult = VerificationResultType.INVALID;
            VerificationOutput.NoOfStates = colorData.Count;
            LocalGetCounterExample(LoopPairs, OutgoingTransitionTable);
        }
    }

    public sealed class StateColor
    {
        // Two bits color encoding
        // (0,0) is white
        // (0,1) is cyan
        // (1,0)is blue
        // (1,1) is red
        private bool bool1, bool2;

        public StateColor()
        {
            bool1 = false;
            bool2 = false;
        }

        public void SetBlue()
        {
            bool1 = true;
            bool2 = false;
        }

        public void SetRed()
        {
            bool1 = true;
            bool2 = true;
        }

        public void SetCyan()
        {
            bool1 = false;
            bool2 = true;
        }

        public bool IsWhite()
        {
            return (bool1 == false && bool2 == false);
        }

        public bool IsBlue()
        {
            return (bool1 == true && bool2 == false);
        }

        public bool IsCyan()
        {
            return (bool1 == false && bool2 == true);
        }

        public bool IsRed()
        {
            return (bool1 == true && bool2 == true);
        }
    }
}