using System;
using System.Linq;
using System.Threading;
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
        private int SWARM_NDFS_NUM_THREADS = Environment.ProcessorCount;


        /// <summary>
        /// Run the verification using the swarm Nested DFS algorithm
        /// </summary>
        /// <returns></returns>        
        public void SwarmNestedDFSModelChecking()
        {
            Thread[] workerThreads = new Thread[SWARM_NDFS_NUM_THREADS];
            MultiCoreLock = new Object();
            StopMutliCoreThreads = false;
            MultiCoreResultedLoop = null;

            VerificationOutput.VerificationResult = VerificationResultType.VALID;

            for (int i = 0; i < SWARM_NDFS_NUM_THREADS; i++)
            {
                workerThreads[i] = new Thread(new ThreadStart(SwarmNestedDFSModelCheckingWorker));
                workerThreads[i].Start();
            }

            for (int i = 0; i < SWARM_NDFS_NUM_THREADS; i++)
            {
                workerThreads[i].Join();
            }

            if (MultiCoreResultedLoop != null)
            {
                VerificationOutput.VerificationResult = VerificationResultType.INVALID;
                LocalTaskStack = MultiCoreLocalTaskStack;
                LocalGetCounterExample(MultiCoreResultedLoop, MultiCoreOutgoingTransitionTable);
            }

        }

        /// <summary>
        /// The function of each worker in swarm NDFS
        /// </summary>
        /// <returns></returns>        
        public void SwarmNestedDFSModelCheckingWorker()
        {
            Dictionary<string, List<string>> OutgoingTransitionTable = new Dictionary<string, List<string>>(Ultility.Ultility.MC_INITIAL_SIZE);
            List<LocalPair> initials = LocalPair.GetInitialPairsLocal(BA, InitialStep);

            if (initials.Count == 0)
            {
                VerificationOutput.VerificationResult = VerificationResultType.VALID;
                return;
            }

            Stack<LocalPair> BlueStack = new Stack<LocalPair>(5000);
            Dictionary<string, StateColor> colorData = new Dictionary<string, StateColor>(5000);

            int threadId;
            // Create random variable with different seed for each thread
            Random rand; // helps different threads access different region of the graph 

            lock (MultiCoreLock)
            {
                threadId = MultiCoreSeed;
                rand = new Random(MultiCoreSeed);
                MultiCoreSeed++;
            }


            // Create the list of initial states
            // May apply randomness here to increase performance
            int[] permutation = generatePermutation(initials.Count, rand);

            for (int z = 0; z < initials.Count; z++)
            {
                LocalPair initState = initials[permutation[z]];
                BlueStack.Push(initState);
                string ID = initState.GetCompressedState();
                colorData.Add(ID, new StateColor());
                OutgoingTransitionTable.Add(ID, new List<string>(8));
            }

            //store the expended event step of a node to avoid multiple invocation of the make one move.
            Dictionary<string, List<LocalPair>> ExpendedNode = new Dictionary<string, List<LocalPair>>(1024);
            
            do
            {
                if (CancelRequested || StopMutliCoreThreads)
                {
                    return;
                }

                LocalPair pair = BlueStack.Peek();
                ConfigurationBase evt = pair.configuration;
                string BAState = pair.state;

                string v = pair.GetCompressedState();

                List<string> outgoing = OutgoingTransitionTable[v];

                StateColor nodeColor = colorData[v];
             
                if (nodeColor.IsWhite())
                {
                    nodeColor.SetCyan();
                }

                bool blueDone = true;

                // Initialize the node if first time visited
                if (!ExpendedNode.ContainsKey(v))
                {
                    //ConfigurationBase[] configList = evt.MakeOneMove().ToArray();
                    IEnumerable<ConfigurationBase> configList = evt.MakeOneMove();
                    pair.SetEnabled(configList, FairnessType);
                    List<LocalPair> product = LocalPair.NextLocal(BA, configList, BAState);
                    ExpendedNode.Add(v, product);

                    for (int k = product.Count - 1; k >= 0; k--)
                    {
                        LocalPair step = product[k];
                        string tmp = step.GetCompressedState();
                        //update the incoming and outgoing edges
                        outgoing.Add(tmp);
                        if (!colorData.ContainsKey(tmp))
                        {
                            colorData.Add(tmp, new StateColor());
                            OutgoingTransitionTable.Add(tmp, new List<string>(8));
                        }
                    }
                
                }

                List<LocalPair> list = ExpendedNode[v];

                //transverse all neighbour nodes
                for (int k = list.Count - 1; k >= 0; k--)
                {
                    int randIndex = rand.Next(list.Count);
                    //int randIndex = list.Count - 1;
                    LocalPair step = list[randIndex];
                    string tmp = step.GetCompressedState();
                    StateColor neighbourColor = colorData[tmp];

                    //if the neighbour node is white 
                    if (neighbourColor.IsWhite())
                    {
                        if (blueDone)
                        {
                            BlueStack.Push(step);
                            blueDone = false;
                            list.RemoveAt(randIndex);
                            break;
                        }
                    }
                    // if the neighbour node is cyan,
                    // and either this node or the neibour node is the accept state
                    // then report cycle
                    else if (neighbourColor.IsCyan())
                    {
                        if (step.state.EndsWith(Constants.ACCEPT_STATE) || pair.state.EndsWith(Constants.ACCEPT_STATE))
                        {
                            SwarmNestedDFSReportBlueCycle(step, pair, BlueStack, colorData, OutgoingTransitionTable);
                            return;
                        }
                        else
                        {
                            list.RemoveAt(randIndex);
                        }
                    }
                    // if the neighbour node is either blue or red, 
                    // can remove from the list
                    else
                    {
                        list.RemoveAt(randIndex);
                    }
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
                            lock (MultiCoreLock)
                            {
                                StopMutliCoreThreads = true;
                                Dictionary<string, LocalPair> LoopPairs = new Dictionary<string, LocalPair>();
                                LoopPairs.Add(v, pair);
                                MultiCoreLocalTaskStack = BlueStack;
                                MultiCoreResultedLoop = LoopPairs;
                                MultiCoreOutgoingTransitionTable = OutgoingTransitionTable;
                                return;
                            }
                        }


                        bool stop = SwarmNestedDFSDepthFirstSearchRed(pair, BlueStack, OutgoingTransitionTable, colorData);

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

            StopMutliCoreThreads = true;
            VerificationOutput.VerificationResult = VerificationResultType.VALID;
            VerificationOutput.NoOfStates = colorData.Count;
            return;
        }


        // Report cycle detected at blue DFS
        public void SwarmNestedDFSReportBlueCycle(LocalPair step, LocalPair pair, Stack<LocalPair> BlueStack, Dictionary<string, StateColor> colorData, Dictionary<string, List<string>> OutgoingTransitionTable)
        {
            lock (MultiCoreLock)
            {
                StopMutliCoreThreads = true;
                Dictionary<string, LocalPair> LoopPairs = new Dictionary<string, LocalPair>(1024);
                string tmp = step.GetCompressedState();
                string m = pair.GetCompressedState();
                LocalPair node = BlueStack.Pop();
                string nodeID = node.GetCompressedState();

                while (!nodeID.Equals(tmp))
                {
                    LoopPairs.Add(nodeID, node);
                    node = BlueStack.Pop();
                    nodeID = node.GetCompressedState();
                }

                LoopPairs.Add(nodeID, node);

                MultiCoreLocalTaskStack = BlueStack;
                MultiCoreResultedLoop = LoopPairs;
                MultiCoreOutgoingTransitionTable = OutgoingTransitionTable;
            }
        }

        // Perform the redDFS
        public bool SwarmNestedDFSDepthFirstSearchRed(LocalPair s, Stack<LocalPair> BlueStack, Dictionary<string, List<string>> OutgoingTransitionTable, Dictionary<string, StateColor> colorData)
        {

            //store the expended event step of a node to avoid multiple invocation of the make one move.
            Dictionary<string, List<LocalPair>> ExpendedNode = new Dictionary<string, List<LocalPair>>(256);

            Stack<LocalPair> RedStack = new Stack<LocalPair>(5000);
            RedStack.Push(s);

            do
            {
                if (CancelRequested || StopMutliCoreThreads)
                {
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
                
                //transverse all neighbour nodes
                for (int k = neighbourList.Count - 1; k >= 0; k--)
                {
                    LocalPair step = neighbourList[k];
                    string tmp = step.GetCompressedState();
                    StateColor neighbourColor = colorData[tmp];

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
                        SwarmNestedDFSReportRedCycle(s, step, pair, BlueStack, RedStack, colorData, OutgoingTransitionTable);
                        return true;
                    }
                    else
                    {
                        neighbourList.RemoveAt(k);
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
        public void SwarmNestedDFSReportRedCycle(LocalPair s, LocalPair step, LocalPair pair, Stack<LocalPair> BlueStack, Stack<LocalPair> RedStack, Dictionary<string, StateColor> colorData, Dictionary<string, List<string>> OutgoingTransitionTable)
        {
            lock (MultiCoreLock)
            {
                StopMutliCoreThreads = true;
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

                MultiCoreLocalTaskStack = BlueStack;
                MultiCoreResultedLoop = LoopPairs;
                MultiCoreOutgoingTransitionTable = OutgoingTransitionTable;                
            }
        }
    }
}