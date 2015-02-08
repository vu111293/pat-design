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
        protected int MULTICORE_NDFS_NUM_THREADS = Environment.ProcessorCount;
        private StringDictionary<bool> SharedRedStatesMultiCoreNDFS;
        private Dictionary<string, int> StateCountMultiCoreNDFS;


        /// <summary>
        /// Run the verification using the Multi Core Nested DFS algorithm and get the result.
        /// Based on:
        /// Multi-Core Nested Depth First Search
        /// http://dl.acm.org/citation.cfm?id=2050942
        /// Alfons Laarman, Rom Langerak, Jaco Van De Pol, Michael Weber and Anton Wijs
        /// Proceeding ATVA'11 Proceedings of the 9th international conference 
        /// on Automated technology for verification and analysis 
        /// Springer-Verlag Berlin, Heidelberg, 2011
        /// </summary>
        /// <returns></returns>
        public void MultiCoreNestedDFSModelChecking()
        {
            Thread[] workerThreads = new Thread[MULTICORE_NDFS_NUM_THREADS];
            MultiCoreLock = new Object();
            StopMutliCoreThreads = false;
            MultiCoreResultedLoop = null;
            SharedRedStatesMultiCoreNDFS = new StringDictionary<bool>(5000);
            StateCountMultiCoreNDFS = new Dictionary<string,int>(5000);

            VerificationOutput.VerificationResult = VerificationResultType.VALID;

            for (int i = 0; i < MULTICORE_NDFS_NUM_THREADS; i++)
            {
                workerThreads[i] = new Thread(new ThreadStart(MultiCoreNestedDFSModelCheckingWorker));
                workerThreads[i].Start();
            }

            for (int i = 0; i < MULTICORE_NDFS_NUM_THREADS; i++)
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
        /// The function of each worker in Multi-Core NDFS
        /// In the implementation of the state color, I do not use the term "pink"
        /// as in the paper for consistency. Instead, I still keep the color as "red" (locally)
        /// and at the same time, the threads share global red states.
        /// </summary>
        /// <returns></returns>        
        public void MultiCoreNestedDFSModelCheckingWorker()
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
                rand = new Random(threadId);
                MultiCoreSeed++;
            }

            // Create the list of initial states
            // May apply randomness here to increase performance
            int[] permutation = generatePermutation(initials.Count, rand);

            for (int z = 0; z < initials.Count; z++)
            {
                LocalPair initState = initials[permutation[z]];
                //LocalPair initState = initials[z];
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
                    List<LocalPair> product;
                    lock (MultiCoreLock)
                    {
                        //ConfigurationBase[] configList = evt.MakeOneMove().ToArray();
                        IEnumerable<ConfigurationBase> configList = evt.MakeOneMove(); //.ToArray()
                        pair.SetEnabled(configList, FairnessType);
                        product = LocalPair.NextLocal(BA, configList, BAState);
                        ExpendedNode.Add(v, product);
                    }
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

                    // if t.color[i]=white && !t.red
                    if (neighbourColor.IsWhite() && !SharedRedStatesMultiCoreNDFS.ContainsKey(tmp))
                    {
                        if (blueDone)
                        {
                            BlueStack.Push(step);
                            blueDone = false;
                            list.RemoveAt(randIndex);
                            break;
                        }
                    }
                    // if t.color[i] = cyan && (s \in A || t \in A)
                    else if (neighbourColor.IsCyan() && (step.state.EndsWith(Constants.ACCEPT_STATE) || pair.state.EndsWith(Constants.ACCEPT_STATE)))
                    {
                        MultiCoreNestedDFSReportBlueCycle(step, pair, BlueStack, colorData, OutgoingTransitionTable);
                        return;
                    }
                    else
                    {
                        list.RemoveAt(randIndex);
                    }
                }

                if (blueDone)
                {
                    BlueStack.Pop();

                    if (pair.state.EndsWith(Constants.ACCEPT_STATE) && evt.IsDeadLock)
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


                    // Check for allred
                    bool allred = true;
                    for (int k = 0; k < outgoing.Count; k++)
                    {
                        string tmp = outgoing[k];
                        if (!SharedRedStatesMultiCoreNDFS.ContainsKey(tmp))
                        {
                            allred = false;
                            break;
                        }
                    }

                    // If all neighbour are red, mark the current state as red as well
                    if (allred)
                    {
                        lock (MultiCoreLock)
                        {
                            if (!SharedRedStatesMultiCoreNDFS.ContainsKey(v))
                            {
                                SharedRedStatesMultiCoreNDFS.Add(v, true);
                            }
                        }

                        nodeColor.SetBlue();
                    }
                    // Else if the current state is an accept state, do the red DFS
                    else if (pair.state.EndsWith(Constants.ACCEPT_STATE))
                    {

                        lock (MultiCoreLock)
                        {
                            if (!StateCountMultiCoreNDFS.ContainsKey(v))
                            {
                                StateCountMultiCoreNDFS.Add(v, 0);
                            }

                            StateCountMultiCoreNDFS[v]++;
                        }

                        bool stop = MultiCoreNestedDFSDepthFirstSearchRed(pair, BlueStack, OutgoingTransitionTable, colorData);

                        if (stop)
                            return;
                    }
                    else
                    {
                        nodeColor.SetBlue();
                    }
                }

            } while (BlueStack.Count > 0);

            VerificationOutput.VerificationResult = VerificationResultType.VALID;
            return;
        }

        // Report cycle detected at blue DFS
        public void MultiCoreNestedDFSReportBlueCycle(LocalPair step, LocalPair pair, Stack<LocalPair> BlueStack, Dictionary<string, StateColor> colorData, Dictionary<string, List<string>> OutgoingTransitionTable)
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
        public bool MultiCoreNestedDFSDepthFirstSearchRed(LocalPair s, Stack<LocalPair> BlueStack, Dictionary<string, List<string>> OutgoingTransitionTable, Dictionary<string, StateColor> colorData)
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

                //List<string> outgoing = OutgoingTransitionTable[v];

                StateColor nodeColor = colorData[v];
                nodeColor.SetRed();

                bool redDone = true;

                if (!ExpendedNode.ContainsKey(v))
                {
                    lock (MultiCoreLock)
                    {
                       // ConfigurationBase[] list = evt.MakeOneMove().ToArray();
                        IEnumerable<ConfigurationBase> list = evt.MakeOneMove();
                        pair.SetEnabled(list, FairnessType);
                        List<LocalPair> product = LocalPair.NextLocal(BA, list, BAState);
                        ExpendedNode.Add(v, product);
                    }
                }

                List<LocalPair> neighbourList = ExpendedNode[v];

                //transverse all neighbour nodes
                for (int k = neighbourList.Count - 1; k >= 0; k--)
                {
                    LocalPair step = neighbourList[k];
                    string tmp = step.GetCompressedState();
                    StateColor neighbourColor = colorData[tmp];

                    // if the neighbour node is blue and it is not globally red
                    if (neighbourColor.IsBlue() && !SharedRedStatesMultiCoreNDFS.ContainsKey(tmp))
                    {
                        //only add the first unvisited node
                        //for the second or more unvisited steps, ignore at the monent
                        if (redDone)
                        {
                            RedStack.Push(step);
                            redDone = true;
                            neighbourList.RemoveAt(k);
                        }
                    }
                    // if the neighbour is cyan
                    // report cycle
                    else if (neighbourColor.IsCyan())
                    {
                        MultiCoreNestedDFSReportRedCycle(s, step, pair, BlueStack, RedStack, colorData, OutgoingTransitionTable);
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

                    if (pair.state.EndsWith(Constants.ACCEPT_STATE))
                    {
                        lock (MultiCoreLock)
                        {
                            if (StateCountMultiCoreNDFS[v] > 0)
                                StateCountMultiCoreNDFS[v]--;
                        }

                        while (StateCountMultiCoreNDFS[v] != 0) ;
                    }

                    lock (MultiCoreLock)
                    {
                        if (!SharedRedStatesMultiCoreNDFS.ContainsKey(v))
                        {
                            SharedRedStatesMultiCoreNDFS.Add(v, true);
                        }
                    }
                }

            } while (RedStack.Count > 0);

            return false;
        }

        // Report cycle detected at red DFS
        public void MultiCoreNestedDFSReportRedCycle(LocalPair s, LocalPair step, LocalPair pair, Stack<LocalPair> BlueStack, Stack<LocalPair> RedStack, Dictionary<string, StateColor> colorData, Dictionary<string, List<string>> OutgoingTransitionTable)
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