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
        private int MULTICORE_TARJAN_WITH_SHARED_MEMORY_NUM_THREADS = Environment.ProcessorCount;
        private StringDictionary<bool> SharedSCCFoundStatesMultiCoreTarjan;

        /// <summary>
        /// Run the verification using the multicore with shared memory Tarjn algorithm
        /// </summary>
        /// <returns></returns>
        public void MultiCoreWithSharedMemoryTarjanModelChecking()
        {
            Thread[] workerThreads = new Thread[MULTICORE_TARJAN_WITH_SHARED_MEMORY_NUM_THREADS];
            MultiCoreLock = new Object();
            StopMutliCoreThreads = false;
            MultiCoreResultedLoop = null;
            SharedSCCFoundStatesMultiCoreTarjan = new StringDictionary<bool>(5000);

            VerificationOutput.VerificationResult = VerificationResultType.VALID;

            for (int i = 0; i < MULTICORE_TARJAN_WITH_SHARED_MEMORY_NUM_THREADS; i++)
            {
                workerThreads[i] = new Thread(new ThreadStart(MultiCoreWithSharedMemoryTarjanModelCheckingWorker));
                workerThreads[i].Start();
            }

            //MultiCoreWithSharedMemoryTarjanModelCheckingWorker();

            for (int i = 0; i < MULTICORE_TARJAN_WITH_SHARED_MEMORY_NUM_THREADS; i++)
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
        /// The function of each worker in MultiCore Tarjan Model Checking
        /// </summary>
        /// <returns></returns>        
        public void MultiCoreWithSharedMemoryTarjanModelCheckingWorker()
        {
            Dictionary<string, List<string>> OutgoingTransitionTable = new Dictionary<string, List<string>>(Ultility.Ultility.MC_INITIAL_SIZE);
            Stack<LocalPair> TaskStack = new Stack<LocalPair>(5000);

            List<LocalPair> initials = LocalPair.GetInitialPairsLocal(BA, InitialStep);
            Dictionary<string, int[]> DFSData = new Dictionary<string, int[]>(5000);

            if (initials.Count == 0)
            {
                VerificationOutput.VerificationResult = VerificationResultType.VALID;
                return;
            }

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
                TaskStack.Push(initState);
                string ID = initState.GetCompressedState();
                DFSData.Add(ID, new int[] { VISITED_NOPREORDER, 0 });
                OutgoingTransitionTable.Add(ID, new List<string>(8));
            }

            Dictionary<string, LocalPair> SCCPairs = new Dictionary<string, LocalPair>(1024);
            Stack<LocalPair> stepStack = new Stack<LocalPair>(1024);

            // Preorder counter 
            int i = 0;
            int numSCCs = 0;

            //store the expended event step of a node to avoid multiple invocation of the make one move.
            Dictionary<string, List<LocalPair>> ExpendedNode = new Dictionary<string, List<LocalPair>>(1024);

            do
            {
                if (CancelRequested || StopMutliCoreThreads)
                {
                    return;
                }

                LocalPair pair = TaskStack.Peek();
                ConfigurationBase evt = pair.configuration;
                string BAState = pair.state;

                string v = pair.GetCompressedState();

                List<string> outgoing = OutgoingTransitionTable[v];
                                int[] nodeData = DFSData[v];

                if (nodeData[0] == VISITED_NOPREORDER)
                {
                    nodeData[0] = i;
                    i++;
                }

                bool done = true;

                // Initialize the states if first time visited
                if (!ExpendedNode.ContainsKey(v))
                {
                    List<LocalPair> product;
                    //lock (MultiCoreLock)
                    //{
                    ConfigurationBase[] configList = evt.MakeOneMove();
                    pair.SetEnabled(configList, FairnessType);
                    product = LocalPair.NextLocal(BA, configList, BAState);

                    int[] randomPermutation = generatePermutation(product.Count, rand);
                    List<LocalPair> randomProduct = new List<LocalPair>();
                    for (int k = 0; k < product.Count; k++)
                    {
                        randomProduct.Add(product[randomPermutation[k]]);
                    }

                    ExpendedNode.Add(v, randomProduct);
                    //}

                    for (int k = product.Count - 1; k >= 0; k--)
                    {
                        LocalPair step = product[k];
                        string tmp = step.GetCompressedState();
                        //update the incoming and outgoing edges
                        outgoing.Add(tmp);
                        if (!DFSData.ContainsKey(tmp))
                        {
                            DFSData.Add(tmp, new int[] { VISITED_NOPREORDER, 0 });
                            OutgoingTransitionTable.Add(tmp, new List<string>(8));
                        }
                    }
                }

                List<LocalPair> list = ExpendedNode[v];

                //transverse all neighbour nodes
                for (int k = list.Count - 1; k >= 0; k--)
                {
                    //int randIndex = rand.Next(list.Count);
                    int randIndex = k;
                    LocalPair step = list[randIndex];
                    string tmp = step.GetCompressedState();
                    int[] neighbourDFSData = DFSData[tmp];

                    //if the step is a unvisited step
                    if (neighbourDFSData[0] == VISITED_NOPREORDER && !SharedSCCFoundStatesMultiCoreTarjan.ContainsKey(tmp))
                    {
                        if (done)
                        {
                            TaskStack.Push(step);
                            done = false;
                            list.RemoveAt(randIndex);
                            break;
                        }
                    }
                    else
                    {
                        list.RemoveAt(randIndex);
                    }
                }

                if (done)
                {
                    TaskStack.Pop();

                    int lowlinkV = nodeData[0];
                    int preorderV = lowlinkV;

                    bool selfLoop = false;
                    for (int j = 0; j < outgoing.Count; j++)
                    {
                        string w = outgoing[j];
                        if (w == v)
                        {
                            selfLoop = true;
                        }

                        int[] wdata = DFSData[w];
                        if (wdata[0] != SCC_FOUND && !SharedSCCFoundStatesMultiCoreTarjan.ContainsKey(w))
                        {
                            if (wdata[0] > preorderV)
                            {
                                lowlinkV = Math.Min(lowlinkV, wdata[1]);
                            }
                            else
                            {
                                lowlinkV = Math.Min(lowlinkV, wdata[0]);
                            }
                        }
                    }

                    nodeData[1] = lowlinkV;

                    if (lowlinkV == preorderV)
                    {
                        bool stopCheckSCC = false;
                        
                        SCCPairs.Add(v, pair);
                        nodeData[0] = SCC_FOUND;
                        //if (!SharedSCCFoundStatesMultiCoreTarjan.ContainsKey(v))
                        //{
                            lock (MultiCoreLock)
                            {
                                if (!SharedSCCFoundStatesMultiCoreTarjan.ContainsKey(v))
                                {
                                    numSCCs++;
                                    SharedSCCFoundStatesMultiCoreTarjan.Add(v, true);
                                }
                                else
                                {
                                    stopCheckSCC = true;
                                }
                            }
                        //}
                        //else
                        //{
                        //    stopCheckSCC = true;
                        //}

                        //checking for buchi-fair
                        bool BuchiFair = pair.state.EndsWith(Constants.ACCEPT_STATE);

                        while (stepStack.Count > 0 && DFSData[stepStack.Peek().GetCompressedState()][0] > preorderV)
                        {
                            LocalPair stepStackNode = stepStack.Pop();
                            string stepStackNodeID = stepStackNode.GetCompressedState();
                            DFSData[stepStackNodeID][0] = SCC_FOUND;


                            if (!stopCheckSCC)
                            {
                                SCCPairs.Add(stepStackNodeID, stepStackNode);

                                //if (!SharedSCCFoundStatesMultiCoreTarjan.ContainsKey(stepStackNodeID))
                                //{
                                    lock (MultiCoreLock)
                                    {
                                        if (!SharedSCCFoundStatesMultiCoreTarjan.ContainsKey(stepStackNodeID))
                                        {
                                            SharedSCCFoundStatesMultiCoreTarjan.Add(stepStackNodeID, true);
                                        }
                                    }
                                //}

                                if (!BuchiFair && stepStackNode.state.EndsWith(Constants.ACCEPT_STATE))
                                {
                                    BuchiFair = true;
                                }
                            }
                            else
                            {
                                ExpendedNode.Remove(stepStackNodeID);
                                OutgoingTransitionTable.Remove(stepStackNodeID);
                            }
                        }

                        int SCCSize = SCCPairs.Count;

                        if (!stopCheckSCC && BuchiFair && (evt.IsDeadLock || SCCSize > 1 || selfLoop))
                        {
                            Dictionary<string, LocalPair> value = IsFair(SCCPairs, OutgoingTransitionTable);
                            if (value != null)
                            {
                                lock (MultiCoreLock)
                                {
                                    StopMutliCoreThreads = true;
                                    MultiCoreResultedLoop = value;
                                    MultiCoreOutgoingTransitionTable = OutgoingTransitionTable;
                                    MultiCoreLocalTaskStack = TaskStack;
                                }
                                return;
                            }
                        }

                        // Avoid access to found SCCs
                        foreach (string componet in SCCPairs.Keys)
                        {
                            ExpendedNode.Remove(componet);
                            OutgoingTransitionTable.Remove(componet);
                        }

                        //StronglyConnectedComponets.Clear();
                        SCCPairs.Clear();
                    }
                    else
                    {
                        stepStack.Push(pair);
                    }
                }

            } while (TaskStack.Count > 0);

            //Console.WriteLine("Thread {0} has traversed {1} SCCs", threadId, numSCCs);
        }
    }
}
