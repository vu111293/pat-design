using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using PAT.Common.Classes.DataStructure;
using PAT.Common.Classes.LTS;
using System.Text;
using System;
using PAT.Common.Classes.ModuleInterface;
using PAT.Common.Classes.Ultility;

namespace PAT.Common.Classes.SemanticModels.LTS.Assertion
{
    public abstract partial class AssertionCSPDeadLockAI : AssertionBase
    {
        protected bool isNotTerminationTesting;
        public List<ConfigurationBase> deadlockStates = new List<ConfigurationBase>();
        public List<List<ConfigurationBase>> TLoops = new List<List<ConfigurationBase>>();
        public int reachDLCounter;
        public int reachTLCounter;
        protected AssertionCSPDeadLockAI()
        {
        }

        protected AssertionCSPDeadLockAI(bool isNontermination)
        {
            isNotTerminationTesting = isNontermination;
        }

        public override string ToString()
        {
            if (isNotTerminationTesting)
            {
                return StartingProcess + " nonterminating";
            }

            return StartingProcess + " deadlockfree";
        }

        /// <summary>
        /// Run the verification and get the result.
        /// </summary>
        /// <returns></returns>
        public override void RunVerification()
        {
            //if (SelectedEngineName == Constants.ENGINE_DEPTH_FIRST_SEARCH)
            //{
            deadlockStates.Clear();
            TLoops.Clear();
            reachTLCounter = 0;
            reachDLCounter = 0;
            DFSVerification();
            //}
            //else
            //{
            //    BFSVerification();
            //}
        }

        public void DFSVerification()
        {
            StringHashTable Visited = new StringHashTable(Ultility.Ultility.MC_INITIAL_SIZE);
            Stack<ConfigurationBase> working = new Stack<ConfigurationBase>(1024);
            Dictionary<string, List<string>> Transitions = new Dictionary<string, List<string>>();
            Dictionary<string, ConfigurationBase> IDs = new Dictionary<string, ConfigurationBase>();

            Visited.Add(InitialStep.GetID());
            working.Push(InitialStep);

            const int VISITED_NOPREORDER = -1;
            const int SCC_FOUND = -2;
            //const int DL_FOUND = -3;
            //DFS data, which mapping each state to an int[] of size 3, first is the pre-order, second is the lowlink, last one is DRAState
            bool reachDL = true;
            bool reachTL = true;

            Dictionary<string, int[]> DFSData = new Dictionary<string, int[]>(Ultility.Ultility.MC_INITIAL_SIZE);
            DFSData.Add(InitialStep.GetID(), new int[] { VISITED_NOPREORDER, 0 });
            HashSet<string> ReachDLStates = new HashSet<string>();
            HashSet<string> ReachTLStates = new HashSet<string>();
            int preordercounter = 0;
            Dictionary<string, List<ConfigurationBase>> ExpendedNode = new Dictionary<string, List<ConfigurationBase>>();
            Stack<ConfigurationBase> stepStack = new Stack<ConfigurationBase>(1024);
            do
            {

                ConfigurationBase currentState = working.Peek();
                string currentID = currentState.GetID();
               
                int[] nodeData = DFSData[currentID];

                if (nodeData[0] == VISITED_NOPREORDER)
                {
                    nodeData[0] = preordercounter;
                    preordercounter++;
                }

                bool done = true;

                if (ExpendedNode.ContainsKey(currentID))
                {
                    if (reachDL)
                    {
                        ReachDLStates.Add(currentID);
                    }
                    else
                    {
                        reachDL = ReachDLStates.Contains(currentID);
                    }

                    if (reachTL)
                    {
                        ReachTLStates.Add(currentID);
                    }
                    else
                    {
                        reachTL = ReachTLStates.Contains(currentID);
                    }

                    List<ConfigurationBase> list = ExpendedNode[currentID];
                    if (list.Count > 0)
                    {
                        for (int k = list.Count - 1; k >= 0; k--)
                        {
                            ConfigurationBase step = list[k];
                            string stepID = step.GetID();
                            //if (!preorder.ContainsKey(stepID))
                            if (DFSData[stepID][0] == VISITED_NOPREORDER)
                            {
                                if (done)
                                {
                                    working.Push(step);
                                    done = false;
                                    list.RemoveAt(k);
                                }
                            }
                            else
                            {
                                if (ReachDLStates.Contains(stepID))
                                {
                                    reachDL = true;
                                    ReachDLStates.Add(stepID);
                                }
                                if (ReachTLStates.Contains(stepID))
                                {
                                    reachTL = true;
                                    ReachTLStates.Add(stepID);
                                }
                                list.RemoveAt(k);
                            }
                        }
                    }
                }
                else
                {
                    reachTL = false;
                    reachDL = false;
                    //if(currentState.IsDeadLock)
                    //{
                    //    DeadlockStates.Add(currentID);
                    //    DFSData[currentID][0] = SCC_FOUND;
                    //    continue;
                    //}
                    List<ConfigurationBase> stepsList = new List<ConfigurationBase>(currentState.MakeOneMove());//List<ConfigurationBase>();
                    if (stepsList.Count == 0)
                    {
                        deadlockStates.Add(currentState);
                        //DFSData[currentID][0] = SCC_FOUND;
                        DFSData[currentID][0] = SCC_FOUND;
                        ReachDLStates.Add(currentID);
                        reachDL = true;
                        working.Pop();
                        continue;
                    }
                    this.VerificationOutput.Transitions += stepsList.Count();
                    //List<ConfigurationBase> backupSteps = new List<ConfigurationBase>(currentState.MakeOneMove());
                    List<string> ids = new List<string>();
                    foreach(var conf in stepsList)
                    {
                        ids.Add(conf.GetID());
                    }
                    Transitions.Add(currentID, ids);
                    //foreach (var distr in currentState.Distributions)
                    //{
                    //    if (distr.inMatrix) continue;
                    //    foreach (var kvPair in distr.States)
                    //    {
                    //        stepsList.Add(kvPair.Value);
                    //    }
                    //}));

                    for (int k = stepsList.Count - 1; k >= 0; k--)
                    {
                        ConfigurationBase nextState = stepsList[k];
                        //nextState.DisplayName
                        //string tmp = step.ID;
                        int[] data;
                        string NextID = nextState.GetID();

                        if (Visited.ContainsKey(NextID))
                        {
                            DFSData.TryGetValue(NextID, out data);
                            if (data[0] == VISITED_NOPREORDER)
                            {
                                if (done)
                                {
                                    working.Push(nextState);
                                    done = false;
                                    stepsList.RemoveAt(k);
                                }
                                else
                                {
                                    stepsList[k] = nextState;
                                }
                            }
                            else
                            {
                                if(ReachDLStates.Contains(NextID))
                                {
                                    reachDL = true;
                                    ReachDLStates.Add(currentID);
                                }
                                if (ReachTLStates.Contains(NextID))
                                {
                                    reachTL = true;
                                    ReachTLStates.Add(currentID);
                                }
                                stepsList.RemoveAt(k);
                            }
                        }
                        else
                        {

                            DFSData.Add(NextID, new int[] { VISITED_NOPREORDER, 0 });
                            Visited.Add(NextID);
                            IDs.Add(NextID, nextState);
                            if (done)
                            {
                                working.Push(nextState);
                                done = false;
                                stepsList.RemoveAt(k);
                            }
                            else
                            {
                                stepsList[k] = nextState;

                            }
                        }
                    }

                    ExpendedNode.Add(currentID, stepsList);
                }

                if (done)
                {
                    int lowlinkV = nodeData[0];
                    int preorderV = lowlinkV;

                    bool selfLoop = false;
                    foreach (var tran in Transitions[currentID])
                    {
                        //if (list.inMatrix) continue;
                        //foreach (KeyValuePair<double, MDPState> state in list.States)
                        //{
                        string w = tran;

                        if (w == currentID)
                        {
                            selfLoop = true;
                        }
                        int[] wdata = DFSData[w];
                        if (wdata[0] != SCC_FOUND)
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
                    working.Pop();

                    if (lowlinkV == preorderV)
                    {
                        List<string> scc = new List<string>();
                        scc.Add(currentID);

                        nodeData[0] = SCC_FOUND;
                        while (stepStack.Count > 0 && DFSData[stepStack.Peek().GetID()][0] > preorderV)
                        {
                            ConfigurationBase s = stepStack.Pop();
                            string sID = s.GetID();
                            scc.Add(sID);
                            DFSData[sID][0] = SCC_FOUND;
                        }

                        if (scc.Count > 1 || selfLoop)
                        {
                            bool terminal = true;
                            foreach (var state in scc)
                            {
                                foreach (var nextState in Transitions[state])
                                {
                                    if (!scc.Contains(nextState))
                                    {
                                        terminal = false;
                                        break;
                                    }
                                }
                                if (!terminal)
                                {
                                    break;
                                }
                            }
                            if (terminal)
                            {
                                List<ConfigurationBase> SCC = new List<ConfigurationBase>();
                                foreach(string state in scc)
                                {
                                    SCC.Add(IDs[state]);
                                    ReachTLStates.Add(state);
                                }
                                reachTL = true;
                                reachDL = false;
                                TLoops.Add(SCC);
                            }

                        }

                    }
                    else
                    {
                        stepStack.Push(currentState);
                    }
                }

            } while (working.Count > 0);

            if (deadlockStates.Count == 0)
            {
                VerificationOutput.VerificationResult = VerificationResultType.VALID;
            }
            else
            {
                VerificationOutput.VerificationResult = VerificationResultType.INVALID;
                //int i = 0;
                //for (; i < DeadlockStates.Count - 1; i++)
                //{
                //    deadlockstates += DeadlockStates[i] + ", ";
                //}
                //deadlockstates += DeadlockStates[i];
                //for (int j = 0; j < TerminalSCC.Count; j++)
                //{
                //    TLoops += "Loop" + j + "is: <";
                //    int k = 0;
                //    for (; k < TerminalSCC[j].Count - 1; k++)
                //    {
                //        TLoops += TerminalSCC[j][k] + ", ";
                //    }
                //    TLoops += TerminalSCC[j][k] + ">\n\t";
                //}
            }
            reachDLCounter = ReachDLStates.Count;
            reachTLCounter = ReachTLStates.Count;
            VerificationOutput.NoOfStates = Visited.Count;

        }

        public void BFSVerification()
        {
            StringHashTable Visited = new StringHashTable(Ultility.Ultility.MC_INITIAL_SIZE);

            Queue<ConfigurationBase> working = new Queue<ConfigurationBase>(1024);
            Queue<List<ConfigurationBase>> paths = new Queue<List<ConfigurationBase>>(1024);

            Visited.Add(InitialStep.GetID());

            working.Enqueue(InitialStep);
            List<ConfigurationBase> path = new List<ConfigurationBase>();
            path.Add(InitialStep);
            paths.Enqueue(path);

            do
            {
                if (CancelRequested)
                {
                    VerificationOutput.NoOfStates = Visited.Count;
                    return;
                }

                ConfigurationBase current = working.Dequeue();
                List<ConfigurationBase> currentPath = paths.Dequeue();
                IEnumerable<ConfigurationBase> list = current.MakeOneMove();
                this.VerificationOutput.Transitions += list.Count();

                Debug.Assert(currentPath[currentPath.Count - 1].GetID() == current.GetID());

                //If the current process is deadlocked, return true if the current BA state is accepting. Otherwise, return false;
                if (current.IsDeadLock)
                {
                    //if (this.isNotTerminationTesting || current.Event != Constants.TERMINATION)
                    //{
                    this.VerificationOutput.VerificationResult = VerificationResultType.INVALID;
                    this.VerificationOutput.NoOfStates = Visited.Count;
                    this.VerificationOutput.CounterExampleTrace = currentPath;
                    return;
                    //}
                }

                //for (int i = list.Length - 1; i >= 0; i--)
                foreach (ConfigurationBase step in list)
                {
                    //ConfigurationBase step = list[i];
                    string stepID = step.GetID();

                    if (step.Event == Constants.TERMINATION)
                    {
                        if (isNotTerminationTesting)
                        {
                            this.VerificationOutput.CounterExampleTrace.Add(step);
                            this.VerificationOutput.VerificationResult = VerificationResultType.INVALID;
                            this.VerificationOutput.NoOfStates = Visited.Count;
                            return;
                        }
                    }
                    else
                    {
                        if (!Visited.ContainsKey(stepID))
                        {
                            Visited.Add(stepID);
                            working.Enqueue(step);

                            List<ConfigurationBase> newPath = new List<ConfigurationBase>(currentPath);
                            newPath.Add(step);
                            paths.Enqueue(newPath);
                        }
                    }
                }
            } while (working.Count > 0);

            this.VerificationOutput.CounterExampleTrace = null;
            if (MustAbstract)
            {
                VerificationOutput.VerificationResult = VerificationResultType.UNKNOWN;
            }
            else
            {
                VerificationOutput.VerificationResult = VerificationResultType.VALID;
            }

            VerificationOutput.NoOfStates = Visited.Count;
        }

        public override string GetResultString()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine(Constants.VERFICATION_RESULT_STRING);
            if (VerificationOutput.VerificationResult == VerificationResultType.VALID)
            {
                sb.AppendLine("The Assertion (" + ToString() + ") is VALID.");
            }
            else if (VerificationOutput.VerificationResult == VerificationResultType.UNKNOWN)
            {
                sb.AppendLine("The Assertion (" + ToString() + ") is NEITHER PROVED NOR DISPROVED.");
            }
            else
            {
                sb.AppendLine("The Assertion (" + ToString() + ") is NOT valid.");
                if (isNotTerminationTesting)
                {
                    sb.AppendLine("The following trace leads to a terminating situation.");
                }
                else
                {
                    sb.AppendLine("The following trace leads to a deadlock situation.");
                }

                VerificationOutput.GetCounterxampleString(sb);
            }

            sb.AppendLine();

            sb.AppendLine("********Verification Setting********");
            sb.AppendLine("Admissible Behavior: " + SelectedBahaviorName);
            sb.AppendLine("Search Engine: " + SelectedEngineName);
            sb.AppendLine("System Abstraction: " + MustAbstract);
            sb.AppendLine();

            return sb.ToString();
        }
    }
}