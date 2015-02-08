using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PAT.Common.Classes.Expressions;
using PAT.Common.Classes.Expressions.ExpressionClass;
using System.Text;
using System.Linq;
using PAT.Common.Classes.LTS;
using PAT.Common.Classes.ModuleInterface;
using PAT.Common.Classes.SemanticModels.MDP2;
using PAT.Common.Classes.Ultility;
using QueryConstraintType = PAT.Common.Classes.Ultility.QueryConstraintType;

namespace PAT.Common.Classes.SemanticModels.MDP
{
    public abstract class AssertionReachabilityCut : LTS.Assertion.AssertionReachability
    {
        protected double Min = -1;
        protected double Max = -1;

        protected QueryConstraintType ConstraintType;

        protected bool BFS_CUT = false;
        protected AssertionReachabilityCut(string reachableState)
            : base(reachableState)
        {
        }

        public override void Initialize(SpecificationBase spec)
        {
            if (ConstraintType == QueryConstraintType.NONE)
            {
                base.Initialize(spec);
            }
            else
            {
                //initialize model checking options
                ModelCheckingOptions = new ModelCheckingOptions();
                List<string> LTLEngine = new List<string>();
                LTLEngine.Add(Constants.ENGINE_MDP_SEARCH);
                ModelCheckingOptions.AddAddimissibleBehavior(Constants.COMPLETE_BEHAVIOR, LTLEngine);
            }
        }


        public override string ToString()
        {
            if (ConstraintType == QueryConstraintType.NONE)
            {
                return base.ToString();
            }
            return StartingProcess.ToString() + " reaches " + ReachableStateLabel + " with " + ConstraintType.ToString().ToLower();
        }

        /// <summary>
        /// Run the verification and get the result.
        /// </summary>
        /// <returns></returns>
        public override void RunVerification()
        {
            if (ConstraintType == QueryConstraintType.NONE)
            {
                base.RunVerification();
                return;
            }

            //MDP mdp = BuildMDP(); //GetTransitionRelation();
            //int[] test = new int[3];
            //foreach(var i in test)
            //{
            //    Console.Write(i);
            //}
            MDP mdp = BuildMDP_SCC_Cut();
            //MDP mdp = BuildMDP_SCC_Cut_Mutlicores();
            //MDP mdp = BuildDTMCSSZ(); //GetTransitionRelation();
            if (!CancelRequested)
            {
                switch (ConstraintType)
                {
                    case QueryConstraintType.PROB:
                        Min = mdp.MinProbability(VerificationOutput);
                        mdp.ResetNonTargetState();
                        Max = mdp.MaxProbability(VerificationOutput);
                        break;
                    case QueryConstraintType.PMAX:
                        Max = mdp.MaxProbability(VerificationOutput);
                        break;
                    case QueryConstraintType.PMIN:
                        Min = mdp.MinProbability(VerificationOutput);
                        break;
                }

                if (Min == 1)
                {
                    this.VerificationOutput.VerificationResult = VerificationResultType.VALID;
                }
                else if (Max == 0)
                {
                    this.VerificationOutput.VerificationResult = VerificationResultType.INVALID;
                }
                else
                {
                    this.VerificationOutput.VerificationResult = VerificationResultType.WITHPROBABILITY;
                }
            }
        }

        private void FindMECDistribution(HashSet<MDPState> SCC)
        {
            HashSet<MDPState> SCC_Copy = new HashSet<MDPState>();
            foreach (MDPState state in SCC)
            {
                SCC_Copy.Add(state);
            }
            List<HashSet<MDPState>> MECs = new List<HashSet<MDPState>>();
            MECs.Add(SCC_Copy);
            const int NotInEMC = -2;
            for (int sccI = 0; sccI < MECs.Count; )
            {
                HashSet<MDPState> workingSCC = MECs[sccI];
                List<MDPState> toRemove = new List<MDPState>();
                do
                {
                    toRemove = new List<MDPState>();

                    //for (int j = 0; j < SCC.Count; j++)
                    foreach (MDPState state in workingSCC)
                    {
                        if (state.SCCIndex != NotInEMC)
                        {
                            List<Distribution> outgoing = state.Distributions; //.ForwardTransition[state];
                            if (outgoing.Count > 0)
                            {
                                bool stateKeep = false;
                                for (int i = outgoing.Count - 1; i >= 0; i--)
                                {
                                    bool distKeep = true;

                                    Distribution next = outgoing[i];
                                    if (next.inMatrix)
                                    {
                                        //distKeep = false;
                                        continue;
                                    }
                                    if (next.States.Count == 1 && next.States[0].Value == state)
                                    {
                                        //distKeep = false;
                                        outgoing.RemoveAt(i);
                                        continue;
                                    }
                                    foreach (KeyValuePair<double, MDPState> distribution in next.States)
                                    {
                                        if (distribution.Value.SCCIndex == NotInEMC ||
                                            !workingSCC.Contains(distribution.Value))
                                        {
                                            distKeep = false;
                                            next.inMatrix = true;
                                            break;
                                        }
                                    }

                                    stateKeep = stateKeep || distKeep;
                                    //if (tokeep)
                                    //{
                                    //    break;
                                    //}
                                }

                                if (!stateKeep)
                                {
                                    toRemove.Add(state);
                                    state.SCCIndex = NotInEMC;
                                }
                            }

                        }
                    }

                    foreach (MDPState state in toRemove)
                    {
                        workingSCC.Remove(state);
                    }

                } while (workingSCC.Count > 0 && toRemove.Count > 0);

                if (workingSCC.Count > 0)
                {
                    bool stillSCC = true;

                    const int VISITED_NOPREORDER = -1;
                    const int SCC_FOUND = -2;
                    //DFS data, which mapping each state to an int[] of size 3, first is the pre-order, second is the lowlink, last one is DRAState
                    HashSet<MDPState> visited = new HashSet<MDPState>();
                    List<HashSet<MDPState>> SCCs = new List<HashSet<MDPState>>();
                    Dictionary<string, int[]> DFSData = new Dictionary<string, int[]>(Ultility.Ultility.MC_INITIAL_SIZE);
                    while (visited.Count < workingSCC.Count)
                    {

                        Stack<MDPState> working = new Stack<MDPState>();
                        //for (int i = 0; i < workingSCC.Count; i++)
                        //{
                        //    if (!visited.Contains(workingSCC[i]))
                        //    {
                        //        visited.Add(workingSCC[i]);
                        //        working.Push(workingSCC[i]);
                        //        DFSData.Add(workingSCC[i].ID, new int[] { VISITED_NOPREORDER, 0 });
                        //        break;
                        //    }
                        //}
                        foreach(MDPState state in workingSCC)
                        {
                            if (!visited.Contains(state))
                            {
                                visited.Add(state);
                                working.Push(state);
                                DFSData.Add(state.ID, new int[] { VISITED_NOPREORDER, 0 });
                                break;
                            }
                        }
                        int preordercounter = 0;
                        Dictionary<string, List<MDPState>> ExpendedNode = new Dictionary<string, List<MDPState>>();
                        Stack<MDPState> stepStack = new Stack<MDPState>(1024);
                        do
                        {

                            MDPState currentState = working.Peek();
                            string currentID = currentState.ID;
                            List<Distribution> outgoing = currentState.Distributions;

                            int[] nodeData = DFSData[currentID];

                            if (nodeData[0] == VISITED_NOPREORDER)
                            {
                                nodeData[0] = preordercounter;
                                preordercounter++;
                            }

                            bool done = true;

                            if (ExpendedNode.ContainsKey(currentID))
                            {

                                List<MDPState> list = ExpendedNode[currentID];
                                if (list.Count > 0)
                                {
                                    for (int k = list.Count - 1; k >= 0; k--)
                                    {
                                        MDPState step = list[k];
                                        string stepID = step.ID;
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
                                            list.RemoveAt(k);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                List<MDPState> stepsList = new List<MDPState>();
                                foreach (Distribution distr in currentState.Distributions)
                                {
                                    if (distr.inMatrix) continue;
                                    foreach (KeyValuePair<double, MDPState> kvPair in distr.States)
                                    {
                                        stepsList.Add(kvPair.Value);
                                    }
                                }

                                for (int k = stepsList.Count - 1; k >= 0; k--)
                                {
                                    MDPState nextState = stepsList[k];
                                    //string tmp = step.ID;
                                    int[] data;
                                    if (visited.Contains(nextState))
                                    {
                                        DFSData.TryGetValue(nextState.ID, out data);
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
                                            stepsList.RemoveAt(k);
                                        }
                                    }
                                    else
                                    {

                                        DFSData.Add(nextState.ID, new int[] { VISITED_NOPREORDER, 0 });
                                        visited.Add(nextState);
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
                                foreach (Distribution list in outgoing)
                                {
                                    if (list.inMatrix) continue;
                                    foreach (KeyValuePair<double, MDPState> state in list.States)
                                    {
                                        string w = state.Value.ID;

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
                                }

                                nodeData[1] = lowlinkV;
                                working.Pop();

                                if (lowlinkV == preorderV)
                                {

                                    HashSet<MDPState> scc = new HashSet<MDPState>();
                                    scc.Add(currentState);

                                    nodeData[0] = SCC_FOUND;
                                    while (stepStack.Count > 0 && DFSData[stepStack.Peek().ID][0] > preorderV)
                                    {
                                        MDPState s = stepStack.Pop();
                                        string sID = s.ID;
                                        scc.Add(s);
                                        DFSData[sID][0] = SCC_FOUND;
                                    }

                                    if (scc.Count > 1 || selfLoop)
                                    {
                                        SCCs.Add(scc);
                                    }
                                    else
                                    {
                                        stillSCC = false;
                                    }


                                }
                                else
                                {
                                    stepStack.Push(currentState);
                                }
                            }

                        } while (working.Count > 0);
                    }


                    if (stillSCC && SCCs.Count == 1)
                    {
                        //foreach (var state in MECs[sccI])
                        //{
                        //    state.SCCIndex = sccI;
                        //}
                        sccI++;
                    }
                    else
                    {
                        MECs.RemoveAt(sccI);
                        MECs.AddRange(SCCs);
                    }
                }
                else
                {
                    MECs.RemoveAt(sccI);
                }

            }


            for (int i = 0; i < MECs.Count; i++)
            {
                List<MDPState> MECI = new List<MDPState>(MECs[i]);
                //for (int j = 0; j < MECI.Count; j++)
                //{
                MDPState rep = MECI[0];
                for (int k = 1; k < MECI.Count; k++)
                {
                    for (int m = MECI[k].Distributions.Count - 1; m >= 0; m--)
                    {
                        Distribution Distr = MECI[k].Distributions[m];
                        if (Distr.inMatrix)
                        {
                            rep.Distributions.Add(Distr);
                        }
                        MECI[k].Distributions.RemoveAt(m);
                    }
                    MECI[k].Distributions.Add(new Distribution(Constants.TAU, rep));
                }
                for (int indexofDistr = rep.Distributions.Count - 1; indexofDistr >= 0; indexofDistr--)
                {
                    if (!rep.Distributions[indexofDistr].inMatrix)
                    {
                        rep.Distributions.RemoveAt(indexofDistr);
                    }
                }
                //}

            }
            //foreach (MDPState state in SCC)
            //{
            //    state.SCCIndex = index;
            //}
        }


        protected MDP BuildMDP_SCC_Cut()
        {
            Stack<KeyValuePair<MDPConfiguration, MDPState>> working = new Stack<KeyValuePair<MDPConfiguration, MDPState>>(1024);

            string initID = InitialStep.GetID();
            MDPState init = new MDPState(initID);
            working.Push(new KeyValuePair<MDPConfiguration, MDPState>(InitialStep as MDPConfiguration, init));
            MDP mdp = new MDP(init, Ultility.Ultility.DEFAULT_PRECISION, Ultility.Ultility.MAX_DIFFERENCE);

            Stack<MDPState> stepStack = new Stack<MDPState>(1024);
            //MDPState2DRAStateMapping = new Dictionary<string, int>(Common.Classes.Ultility.Ultility.MC_INITIAL_SIZE);
            const int VISITED_NOPREORDER = -1;
            const int SCC_FOUND = -2;

            //DFS data, which mapping each state to an int[] of size 3, first is the pre-order, second is the lowlink, last one is DRAState
            Dictionary<string, int[]> DFSData = new Dictionary<string, int[]>(Ultility.Ultility.MC_INITIAL_SIZE);

            DFSData.Add(initID, new int[] { VISITED_NOPREORDER, 0 });
            //build the MDP while identifying the SCCs
            List<HashSet<MDPState>> SCCs = new List<HashSet<MDPState>>();
            //Dictionary<int, List<MDPState>> scc2out = new Dictionary<int, List<MDPState>>();
            List<HashSet<MDPState>> scc2out = new List<HashSet<MDPState>>();
            List<HashSet<MDPState>> scc2input = new List<HashSet<MDPState>>();
            //Dictionary<int, HashSet<MDPState>> scc2input = new Dictionary<int, HashSet<MDPState>>();
            //Dictionary<string, int> preorder = new Dictionary<string, int>(Common.Classes.Ultility.Ultility.MC_INITIAL_SIZE);
            //Dictionary<string, int> lowlink = new Dictionary<string, int>(Common.Classes.Ultility.Ultility.MC_INITIAL_SIZE);
            int preordercounter = 0;
            Dictionary<string, List<MDPConfiguration>> ExpendedNode = new Dictionary<string, List<MDPConfiguration>>();

            bool reachTarget = true;

            do
            {
                if (CancelRequested)
                {
                    this.VerificationOutput.NoOfStates = this.VerificationOutput.NoOfStates + mdp.States.Count;
                    return mdp;
                }

                KeyValuePair<MDPConfiguration, MDPState> pair = working.Peek();
                MDPConfiguration evt = pair.Key;
                //int DRAState = pair.Key.state;
                string currentID = pair.Key.GetID();
                MDPState currentState = pair.Value;
                List<Distribution> outgoing = currentState.Distributions;

                //if (!preorder.ContainsKey(currentID))
                //{
                //    preorder.Add(currentID, preorderCounter);
                //    preorderCounter++;
                //}

                int[] nodeData = DFSData[currentID];

                if (nodeData[0] == VISITED_NOPREORDER)
                {
                    nodeData[0] = preordercounter;
                    preordercounter++;
                }

                bool done = true;

                if (ExpendedNode.ContainsKey(currentID))
                {
                    if (reachTarget)
                    {
                        currentState.ReachTarget = reachTarget;
                    }
                    else
                    {
                        reachTarget = currentState.ReachTarget;
                    }
                    List<MDPConfiguration> list = ExpendedNode[currentID];
                    if (list.Count > 0)
                    {
                        for (int k = list.Count - 1; k >= 0; k--)
                        {
                            MDPConfiguration step = list[k];

                            string stepID = step.GetID();
                            //if (!preorder.ContainsKey(stepID))
                            if (DFSData[stepID][0] == VISITED_NOPREORDER)
                            {
                                if (done)
                                {
                                    working.Push(new KeyValuePair<MDPConfiguration, MDPState>(step, mdp.States[stepID]));
                                    done = false;
                                    list.RemoveAt(k);
                                }
                            }
                            else
                            {
                                MDPState s = mdp.States[stepID];
                                if (s.ReachTarget)
                                {
                                    reachTarget = true;
                                    currentState.ReachTarget = reachTarget;

                                    if (s.SCCIndex >= 0)
                                    {
                                        scc2input[s.SCCIndex].Add(s);
                                    }

                                }
                                list.RemoveAt(k);
                            }
                        }
                    }
                }
                else
                {
                    reachTarget = false;
                    int currentDistriIndex = -1;
                    Distribution newDis = new Distribution(Constants.TAU);

                    //MDPConfiguration[] steps = evt.MakeOneMoveLocal();
                    IEnumerable<MDPConfiguration> steps = evt.MakeOneMoveLocal();
                    //NOTE: here we play a trick for deadlock case: if a deadlock exist in the MDP, we will make a
                    //self loop transition to remove the deadlock. Deadlock is meaningless in MDP.
                    if (evt.IsDeadLock)
                    {
                        //List<MDPConfiguration> stepsList = new List<MDPConfiguration>(steps);
                        working.Pop();
                        nodeData[0] = SCC_FOUND;
                        continue;
                        //stepsList.Add(CreateSelfLoopStep(evt));
                        //steps = stepsList.ToArray();
                        //HasDeadLock = true;
                    }
                    List<MDPConfiguration> stepsList = new List<MDPConfiguration>(steps);
                    //List<PCSPEventDRAPair> product = Next(steps, DRAState);
                    this.VerificationOutput.Transitions += stepsList.Count;

                    for (int k = stepsList.Count - 1; k >= 0; k--)
                    {
                        MDPConfiguration step = stepsList[k];
                        string tmp = step.GetID();
                        bool target = false;
                        ExpressionValue v = EvaluatorDenotational.Evaluate(ReachableStateCondition, step.GlobalEnv);

                        MDPState nextState;

                        if ((v as BoolConstant).Value)
                        {
                            target = true;
                            if (!DFSData.Keys.Contains(tmp))
                            {
                                DFSData.Add(tmp, new int[] { SCC_FOUND, SCC_FOUND });
                                //    nextState = new MDPState(tmp);
                                //    mdp.States.Add(tmp, nextState);
                                //preordercounter++;
                            }
                            //else if(DFSData[tmp][0] == VISITED_NOPREORDER)
                            //{
                            //    DFSData[tmp][0] = preordercounter;
                            //    preordercounter++;
                            //}
                        }
                        //int nextIndex = VisitedWithID.Count;

                        int[] data;
                        if (mdp.States.TryGetValue(tmp, out nextState))
                        {
                            nextState.TrivialPre = false;
                            DFSData.TryGetValue(tmp, out data);
                            if (!target && data[0] == VISITED_NOPREORDER)
                            {
                                //if (mdp.States.TryGetValue(tmp, out nextState))
                                //{

                                //    if (!preorder.ContainsKey(tmp))
                                //    {
                                if (done)
                                {
                                    working.Push(new KeyValuePair<MDPConfiguration, MDPState>(step, nextState));
                                    done = false;
                                    stepsList.RemoveAt(k);
                                }
                                else
                                {
                                    stepsList[k] = step;
                                }
                            }
                            else
                            {
                                stepsList.RemoveAt(k);
                                MDPState s = mdp.States[tmp];
                                if (s.ReachTarget)
                                {
                                    reachTarget = true;
                                    currentState.ReachTarget = reachTarget;
                                    if (s.SCCIndex >= 0 && !scc2input[s.SCCIndex].Contains(s))
                                    {
                                        scc2input[s.SCCIndex].Add(s);
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (!target)
                            {
                                DFSData.Add(tmp, new int[] { VISITED_NOPREORDER, 0 });
                            }

                            nextState = new MDPState(tmp);
                            mdp.States.Add(tmp, nextState);
                            if (done)
                            {
                                if (target)
                                {
                                    mdp.AddTargetStates(nextState);
                                    reachTarget = true;
                                }
                                else
                                {
                                    working.Push(new KeyValuePair<MDPConfiguration, MDPState>(step, nextState));
                                }

                                done = false;
                                stepsList.RemoveAt(k);
                            }
                            else
                            {
                                if (target)
                                {
                                    stepsList.RemoveAt(k);
                                    mdp.AddTargetStates(nextState);
                                    reachTarget = true;
                                }
                                else
                                {
                                    stepsList[k] = step;
                                }
                            }
                        }

                        MDPConfiguration pstep = step;

                        if (pstep.DisIndex == -1)
                        {
                            if (currentDistriIndex >= 0)
                            {
                                
                                    //note here no pre is stored
                                    currentState.Distributions.Add(newDis);

                                
                                newDis = new Distribution(Constants.TAU);
                            }
                            if (nextState.ID != currentID)//note here self loop distribution is removed; so just used for PMAX
                            {
                                Distribution newTrivialDis = new Distribution(pstep.Event, nextState);

                                currentState.Distributions.Add(newTrivialDis);
                            }

                        }
                        else if (currentDistriIndex != -1 && pstep.DisIndex != currentDistriIndex)
                        {
                           //note here no pre is stored
                            currentState.Distributions.Add(newDis);
                           
                            newDis = new Distribution(Constants.TAU);
                            newDis.AddProbStatePair(pstep.Probability, nextState);
                            nextState.TrivialPre = false;
                        }
                        else
                        {
                            newDis.AddProbStatePair(pstep.Probability, nextState);
                            nextState.TrivialPre = false;
                        }

                        currentDistriIndex = pstep.DisIndex;
                    }

                    if (currentDistriIndex >= 0)
                    {
                        currentState.Distributions.Add(newDis);
                    }

                    ExpendedNode.Add(currentID, stepsList);
                }

                if (done)
                {
                    int lowlinkV = nodeData[0];
                    int preorderV = lowlinkV;

                    bool selfLoop = false;
                    foreach (Distribution list in outgoing)
                    {
                        foreach (KeyValuePair<double, MDPState> state in list.States)
                        {
                            string w = state.Value.ID;

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
                    }

                    nodeData[1] = lowlinkV;
                    working.Pop();

                    if (lowlinkV == preorderV)
                    {
                        if (currentState.ReachTarget)
                        {
                            HashSet<MDPState> SCC = new HashSet<MDPState>();
                            //List<HashSet<MDPState>> Groupings = new List<HashSet<MDPState>>();
                            //HashSet<MDPState> Grouping = new HashSet<MDPState>();
                            //bool reduceScc = false;
                            //Grouping.Add(currentState);
                            SCC.Add(currentState);
                            //int count = 1;
                            //dtmcSCCstates.Add(currentID);
                            nodeData[0] = SCC_FOUND;
                            while (stepStack.Count > 0 && DFSData[stepStack.Peek().ID][0] > preorderV)
                            {
                                MDPState s = stepStack.Pop();
                                s.ReachTarget = true;
                                string sID = s.ID;
                                //Grouping.Add(s);
                                SCC.Add(s);
                                //dtmcSCCstates.Add(sID);
                                DFSData[sID][0] = SCC_FOUND;
                                //count++;
                            }

                            if (SCC.Count > 1 || selfLoop)
                            {
                                SCCs.Add(SCC);
                                HashSet<MDPState> inputs = new HashSet<MDPState>();
                                inputs.Add(currentState);
                                int Count = SCCs.Count;
                                foreach (MDPState state in SCC)
                                {
                                    state.SCCIndex = Count - 1;
                                }
                                scc2input.Add(inputs);

                                HashSet<MDPState> outputs = new HashSet<MDPState>();

                                //input.Add(currentState);
                                foreach (MDPState state in SCC)
                                {
                                    foreach (Distribution distr in state.Distributions)
                                    {
                                        foreach (KeyValuePair<double, MDPState> KVpair in distr.States)
                                        {
                                            if (!SCC.Contains(KVpair.Value))
                                            {
                                                outputs.Add(KVpair.Value);
                                            }
                                        }
                                    }

                                }
                                //List<MDPState> outofscc = new List<MDPState>(outputs);
                                scc2out.Add(outputs);
                            }
                            //}
                        }
                        else
                        {
                            //dtmcSCCstates.Add(currentID);
                            nodeData[0] = SCC_FOUND;
                            while (stepStack.Count > 0 && DFSData[stepStack.Peek().ID][0] > preorderV)
                            {
                                string sID = stepStack.Pop().ID;
                                DFSData[sID][0] = SCC_FOUND;
                            }
                        }

                    }
                    else
                    {
                        stepStack.Push(pair.Value);
                    }
                }
            } while (working.Count > 0);

            RemoveOneTran(SCCs, scc2input);
            for (int i = 0; i < SCCs.Count; i++)
            {
                FindMECDistribution(SCCs[i]);
            }
            if(BFS_CUT)
            {
                BFS_Cut(SCCs, scc2out, scc2input);
            }
            else
            {
                //DFS_CutAutoAdjustTreeLike(SCCs, scc2out, scc2input);
                 DFS_CutAutoAdjust(SCCs, scc2out, scc2input);
                 //DFS_Cut(SCCs, scc2out, scc2input);
            }
            

            VerificationOutput.NoOfStates = VerificationOutput.NoOfStates + mdp.States.Count;
            int counter = 0;
            BuildPRE(mdp, ref counter);
            return mdp;
        }
        protected void RemoveOneTran(List<HashSet<MDPState>> SCCs, List<HashSet<MDPState>> scc2input)
        {
            for (int i = SCCs.Count - 1; i >= 0; i--)
            {
                HashSet<MDPState> Working = scc2input[i];
                HashSet<string> visited = new HashSet<string>();
                foreach (MDPState state in Working)
                {
                    visited.Add(state.ID);
                }
                //HashSet<MDPState> Working = new HashSet<MDPState>();
                do
                {
                    HashSet<MDPState> newWorking = new HashSet<MDPState>();
                    foreach (MDPState state in Working)
                    {
                        for (int distr = 0; distr < state.Distributions.Count; )
                        {
                            Distribution Dis = state.Distributions[distr];
                            bool updated = false;
                            foreach (KeyValuePair<double, MDPState> next in Dis.States)
                            {
                                MDPState nextstate = next.Value;
                                if (nextstate.TrivialPre && SCCs[i].Contains(nextstate))
                                {
                                    state.Distributions.RemoveAt(distr);
                                    state.Distributions.AddRange(nextstate.Distributions);
                                    SCCs[i].Remove(nextstate);
                                    updated = true;
                                    break;
                                }
                                else
                                {
                                    if (!visited.Contains(nextstate.ID) && SCCs[i].Contains(nextstate))
                                    {
                                        visited.Add(nextstate.ID);
                                        newWorking.Add(nextstate);
                                    }

                                }

                            }
                            if (!updated) distr++;
                        }
                    }
                    Working = newWorking;
                } while (Working.Count > 0);
            }
        }

        protected void DFS_CutAutoAdjust(List<HashSet<MDPState>> SCCs, List<HashSet<MDPState>> scc2out, List<HashSet<MDPState>> scc2input)
        {
            int countforGraph = 0;
            const int SCCBound = 2;
            //List<List<MDPState>> SCCsList = new List<List<MDPState>>(SCCs.Count);
            bool firstCut = true;
           // int numberOfRound = 0; //try to rotate cutting sequence
            for (int i = SCCs.Count - 1; i >= 0; )
            {
                List<MDPState> SCC = new List<MDPState>();

                //ajust the order
                //if (numberOfRound > 0)
                //{
                //    List<MDPState> SCC1 = new List<MDPState>(SCCs[i]);

                //    SCC.AddRange(SCC1.GetRange(countforGraph, SCC1.Count - countforGraph));
                //    SCC.AddRange(SCC1.GetRange(0, countforGraph));
                //   // SCC.Reverse();
                //}
                //else
                //{
                //    SCC = new List<MDPState>(SCCs[i]);//[i];
                //}
                //end ofr adjusting the order
               
                if (SCC.Count <= SCCBound)
                {
                    SCCReductionGaussianSparseMatrix(SCC, new List<MDPState>(scc2out[i]), firstCut);
                    //SCCs.RemoveAt(i);
                    i--;
                    firstCut = true;
                }
                else
                {
                    //List<MDPState> SCCi = new List<MDPState>(SCCs[i]);
                    int Counter = SCC.Count;
                    while (Counter > 0)
                    {
                        List<MDPState> group;
                        if (SCCBound > Counter)
                        {
                            group = SCC.GetRange(SCC.Count - Counter, Counter);
                            HashSet<MDPState> outputs = new HashSet<MDPState>();
                            //input.Add(currentState);
                            foreach (MDPState state in group)
                            {
                                foreach (Distribution Distr in state.Distributions)
                                {
                                    foreach (KeyValuePair<double, MDPState> KVpair in Distr.States)
                                    {
                                        MDPState nextState = KVpair.Value;
                                        if (!group.Contains(nextState))
                                        {
                                            outputs.Add(nextState);
                                        }
                                    }
                                }
                            }

                            List<MDPState> output = new List<MDPState>(outputs);
#if DEBUG
                            LinearEquationsSolver.ToDot(group, Counter, countforGraph++);
#endif
                            SCCReductionGaussianSparseMatrix(group, output, firstCut);
#if DEBUG
                            LinearEquationsSolver.ToDot(group, Counter, countforGraph++);
#endif
                            //SCCReduction(group, output);
                            Counter -= SCCBound;
                        }
                        else
                        {
                            //const int BREATHCONTROL = 5;
                            //const int REDUCTIONBOUND = 0;
                            //  int SCCBound1 = 0;
                            group = SCC.GetRange(SCC.Count - Counter, SCCBound);
                            //group = new List<MDPState>(SCCBound);
                            //for(int inneri=0;inneri<SCCBound;inneri++)
                            //{

                            //    MDPState state = SCC[SCC.Count - Counter + inneri];
                            //    //state.TimesReduction++;  //actually, I only one parameter to record how many times SCC has been cut, and calculated.
                            //    //if (state.TimesReduction<REDUCTIONBOUND&state.Distributions.Count > BREATHCONTROL)
                            //    //{
                            //    //    continue;
                            //    //}
                            //    group.Add(state);
                            // //   SCCBound1++;
                            //}
                            HashSet<MDPState> outputs = new HashSet<MDPState>();
                            //input.Add(currentState);
                            foreach (MDPState state in group)
                            {
                                foreach (Distribution Distr in state.Distributions)
                                {
                                    foreach (KeyValuePair<double, MDPState> KVpair in Distr.States)
                                    {
                                        MDPState nextState = KVpair.Value;
                                        if (!group.Contains(nextState))
                                        {
                                            outputs.Add(nextState);
                                        }
                                    }
                                }
                            }
                            int outputNumber = outputs.Count;
                            Counter -= SCCBound;
                            int outputNumberMove = outputNumber;
                            //int stepsize = 0;
                            int moveCounter = 0;
                            List<MDPState> group1 = new List<MDPState>(group);
                            HashSet<MDPState> outputs1 = new HashSet<MDPState>(outputs);
                            const int HORIZON = 5; //put as system parameter
                            //while (Counter>0&outputNumber >= outputNumberMove)
                            //{
                            while (moveCounter < HORIZON && -Counter + moveCounter < 0)
                            {

                                MDPState mdpState = SCC[SCC.Count - Counter + moveCounter];
                                //mdpState.TimesReduction++; 
                                moveCounter++;
                                group1.Add(mdpState);
                                if (outputs1.Contains(mdpState))
                                {
                                    outputs1.Remove(mdpState);
                                }
                                foreach (Distribution Distr in mdpState.Distributions)
                                {
                                    foreach (KeyValuePair<double, MDPState> KVpair in Distr.States)
                                    {
                                        MDPState nextState = KVpair.Value;
                                        if (!group1.Contains(nextState))
                                        {
                                            outputs1.Add(nextState);
                                        }
                                    }
                                }
                                outputNumberMove = outputs1.Count;
                                if (outputNumber >= outputNumberMove)
                                {
                                    outputNumber = outputNumberMove;
                                    Counter -= moveCounter;

                                    outputs = new HashSet<MDPState>(outputs1);
                                    group = new List<MDPState>(group1);
                                    moveCounter = 0;
                                }
                                //}

                            }


                            List<MDPState> output = new List<MDPState>(outputs);
#if DEBUG
                            LinearEquationsSolver.ToDot(group, Counter, countforGraph++);
#endif
                            SCCReductionGaussianSparseMatrix(group, output, firstCut);
#if DEBUG
                            LinearEquationsSolver.ToDot(group, Counter, countforGraph++);
#endif
                            //SCCReduction(group, output);
                            //  Counter -= SCCBound;


                        }



                    }

                    
                    HashSet<MDPState> visited = new HashSet<MDPState>();
                   // RandomLabel(scc2input[i], visited, SCCs[i]);
                    //the fourth parameter: true means use mixed bfs; false means don't use mixed bfs
                    DFSLabel(scc2input[i], visited, scc2out[i], false);

                    if (visited.Count > SCCBound && visited.Count < SCCs[i].Count&&numberOfRound>10)
                    {
                        SCCs[i] = visited;
                        firstCut = false;
                        numberOfRound++;
                    }
                    else
                    {
                        List<MDPState> scc = new List<MDPState>(visited);
                        SCCReductionGaussianSparseMatrix(scc, new List<MDPState>(scc2out[i]), firstCut);
                        //SCCReductionMatlab(scc, scc2out[i]);
                        i--;
                        numberOfRound = 0;
                        firstCut = true;
#if DEBUG
                        LinearEquationsSolver.ToDot(scc, Counter, countforGraph++);
#endif
                    }
                }
            }

        }

        private int numberOfRound ;
        protected void DFS_CutAutoAdjustTreeLike(List<HashSet<MDPState>> SCCs, List<HashSet<MDPState>> scc2out, List<HashSet<MDPState>> scc2input)
        {
            int countforGraph = 0;
            const int SCCBound = 2;
            const int HORIZON = 5; //put as system parameter
            //List<List<MDPState>> SCCsList = new List<List<MDPState>>(SCCs.Count);
            bool firstCut = true;
            for (int i = SCCs.Count - 1; i >= 0; )
            {
                List<MDPState> SCC = new List<MDPState>(SCCs[i]);//[i];
                if (SCC.Count <= SCCBound)
                {
                    SCCReductionGaussianSparseMatrix(SCC, new List<MDPState>(scc2out[i]), firstCut);
                    //SCCs.RemoveAt(i);
                    i--;
                    firstCut = true;
                }
                else
                {
                    //List<MDPState> SCCi = new List<MDPState>(SCCs[i]);
                    int Counter = SCC.Count;
                    
                    while (Counter > 0)
                    {
                        bool isLoop = false; //if isLoop is false, that means, this is tree structure.
                        bool isOverlap = false;
                        List<MDPState> group;
                        if (SCCBound > Counter)
                        {
                            group = SCC.GetRange(SCC.Count - Counter, Counter);
                            HashSet<MDPState> outputs = new HashSet<MDPState>();
                            //input.Add(currentState);
                            foreach (MDPState state in group)
                            {
                                foreach (Distribution Distr in state.Distributions)
                                {
                                    foreach (KeyValuePair<double, MDPState> KVpair in Distr.States)
                                    {
                                        MDPState nextState = KVpair.Value;
                                        if (!group.Contains(nextState))
                                        {
                                            outputs.Add(nextState);
                                        }else if(!isLoop)
                                        {
                                            isLoop = true;
                                        }
                                    }
                                }
                            }

                            List<MDPState> output = new List<MDPState>(outputs);
#if DEBUG
                            LinearEquationsSolver.ToDot(group, Counter, countforGraph++);
#endif
                            
                            SCCReductionGaussianSparseMatrix(group, output, firstCut);
#if DEBUG
                            LinearEquationsSolver.ToDot(group, Counter, countforGraph++);
#endif
                            //SCCReduction(group, output);
                            Counter -= SCCBound;
                        }
                        else
                        {
                            //const int BREATHCONTROL = 5;
                            //const int REDUCTIONBOUND = 0;
                            //  int SCCBound1 = 0;
                          
                            group = new List<MDPState>(SCCBound);
                            // group = SCC.GetRange(SCC.Count - Counter, SCCBound);
                            HashSet<MDPState> outputs = new HashSet<MDPState>();
                            for (int inneri = 0; inneri < SCCBound; inneri++)
                            {

                                MDPState state = SCC[SCC.Count - Counter + inneri];
                                group.Add(state);
                                if(outputs.Contains(state))
                                {
                                    outputs.Remove(state);
                                    isOverlap = true;
                                }
                                foreach (Distribution Distr in state.Distributions)
                                {
                                    foreach (KeyValuePair<double, MDPState> KVpair in Distr.States)
                                    {
                                        MDPState nextState = KVpair.Value;
                                        if (!group.Contains(nextState))
                                        {
                                            outputs.Add(nextState);
                                        }
                                        else if (!isLoop)
                                        {
                                            isLoop = true;
                                        }
                                    }
                                }

                                //   SCCBound1++;
                            }
                           
                         
                            //HashSet<MDPState> outputs = new HashSet<MDPState>();
                            ////input.Add(currentState);
                            //foreach (MDPState state in group)
                            //{
                            //    foreach (Distribution Distr in state.Distributions)
                            //    {
                            //        foreach (KeyValuePair<double, MDPState> KVpair in Distr.States)
                            //        {
                            //            MDPState nextState = KVpair.Value;
                            //            if (!group.Contains(nextState))
                            //            {
                            //                outputs.Add(nextState);
                            //            }else if(!isLoop)
                            //            {
                            //                isLoop = true;
                            //            }
                            //        }
                            //    }
                            //}
                            int outputNumber = outputs.Count;
                            Counter -= SCCBound;
                            int outputNumberMove = outputNumber;
                            //int stepsize = 0;
                            int moveCounter = 0;
                            List<MDPState> group1 = new List<MDPState>(group);
                            HashSet<MDPState> outputs1 = new HashSet<MDPState>(outputs);
                           
                            //while (Counter>0&outputNumber >= outputNumberMove)
                            //{
                            while (moveCounter < HORIZON && -Counter + moveCounter < 0)
                            {
                                bool isLoop2=false;
                                bool isOverlap2 = false;
                                MDPState mdpState = SCC[SCC.Count - Counter + moveCounter];
                                //mdpState.TimesReduction++; 
                                moveCounter++;
                                group1.Add(mdpState);
                                if (outputs1.Contains(mdpState))
                                {
                                    isOverlap2 = true;
                                    outputs1.Remove(mdpState);
                                }
                                foreach (Distribution Distr in mdpState.Distributions)
                                {
                                    foreach (KeyValuePair<double, MDPState> KVpair in Distr.States)
                                    {
                                        MDPState nextState = KVpair.Value;
                                        if (!group1.Contains(nextState))
                                        {
                                            outputs1.Add(nextState);
                                        }else if(!(isLoop||isLoop2))
                                        {
                                            isLoop2 = true;
                                        }
                                    }
                                }
                                outputNumberMove = outputs1.Count;
                                if (outputNumber >= outputNumberMove)
                                {
                                    outputNumber = outputNumberMove;
                                    Counter -= moveCounter;

                                    outputs = new HashSet<MDPState>(outputs1);
                                    group = new List<MDPState>(group1);
                                    moveCounter = 0;
                                    isLoop = isLoop || isLoop2;
                                    isOverlap = isOverlap || isOverlap2;
                                }
                                //}

                            }


                            List<MDPState> output = new List<MDPState>(outputs);
#if DEBUG
                            LinearEquationsSolver.ToDot(group, Counter, countforGraph++);                            
#endif
                            if (isOverlap)
                            {
                                if (!isLoop)
                                {
                                    SCCReductionGaussianSparseMatrixTreeStructure(group, output, firstCut);
                                }
                                else
                                {

                                    SCCReductionGaussianSparseMatrix(group, output, firstCut);
                                }
                            }

#if DEBUG
                            LinearEquationsSolver.ToDot(group, Counter, countforGraph++);
#endif
                            //SCCReduction(group, output);
                            //  Counter -= SCCBound;


                        }
                    }

                    
                    HashSet<MDPState> visited = new HashSet<MDPState>();
                    //RandomLabel(scc2input[i], visited, SCCs[i]);
                    DFSLabel(scc2input[i], visited, scc2out[i], true);

                    if (visited.Count > SCCBound && visited.Count < SCCs[i].Count)
                    {
                        SCCs[i] = visited;
                        firstCut = false;
                    }
                    else
                    {
                        List<MDPState> scc = new List<MDPState>(visited);
                        SCCReductionGaussianSparseMatrix(scc, new List<MDPState>(scc2out[i]), firstCut);
                        i--;
                        firstCut = true;
#if DEBUG
                            LinearEquationsSolver.ToDot(scc, Counter, countforGraph++);                            
#endif
                    }
                }
            }

        }


        protected void DFS_Cut(List<HashSet<MDPState>> SCCs, List<HashSet<MDPState>> scc2out, List<HashSet<MDPState>> scc2input)
        {
            const int SCCBound = 2;
            //List<List<MDPState>> SCCsList = new List<List<MDPState>>(SCCs.Count);
            bool firstCut = true;
            for (int i = SCCs.Count - 1; i >= 0; )
            {
                List<MDPState> SCC = new List<MDPState>(SCCs[i]);//[i];
                if (SCC.Count <= SCCBound)
                {
                    SCCReductionGaussianSparseMatrix(SCC, new List<MDPState>(scc2out[i]), firstCut);
                    //SCCs.RemoveAt(i);
                    i--;
                    firstCut = true;
                }
                else
                {
                    //List<MDPState> SCCi = new List<MDPState>(SCCs[i]);
                    int Counter = SCC.Count;
                    while (Counter > 0)
                    {
                        List<MDPState> group;
                        if (SCCBound > Counter)
                        {
                            group = SCC.GetRange(SCC.Count - Counter, Counter);
                        }
                        else
                        {
                            group = SCC.GetRange(SCC.Count - Counter, SCCBound);
                        }

                        HashSet<MDPState> outputs = new HashSet<MDPState>();

                        //input.Add(currentState);
                        foreach (MDPState state in group)
                        {
                            foreach (Distribution Distr in state.Distributions)
                            {
                                foreach (KeyValuePair<double, MDPState> KVpair in Distr.States)
                                {
                                    MDPState nextState = KVpair.Value;
                                    if (!group.Contains(nextState))
                                    {
                                        outputs.Add(nextState);
                                    }
                                }
                            }
                        }

                        List<MDPState> output = new List<MDPState>(outputs);
                        SCCReductionGaussianSparseMatrix(group, output, firstCut);
                        //SCCReduction(group, output);
                        Counter -= SCCBound;
                    }

                    HashSet<MDPState> visited = new HashSet<MDPState>();
                    //RandomLabel(scc2input[i], visited, SCCs[i]);
                    DFSLabel(scc2input[i], visited, scc2out[i], true);
                    

                    if (visited.Count > SCCBound && visited.Count < SCCs[i].Count)
                    {
                        SCCs[i] = visited;
                        firstCut = false;
                    }
                    else
                    {
                        List<MDPState> scc = new List<MDPState>(visited);
                        SCCReductionGaussianSparseMatrix(scc, new List<MDPState>(scc2out[i]), firstCut);
                        //SCCReductionMatlab(scc, scc2out[i]);
                        i--;
                        firstCut = true;
                    }
                }
            }

        }

        protected void BFS_Cut(List<HashSet<MDPState>> SCCs, List<HashSet<MDPState>> scc2out, List<HashSet<MDPState>> scc2input)
        {
            const int SCCBound = 3;
            //List<List<MDPState>> SCCsList = new List<List<MDPState>>(SCCs.Count);
            //bool firstCut = true;
            for (int i = SCCs.Count - 1; i >= 0; i--)
            {
                //List<MDPState> SCC = new List<MDPState>(SCCs[i]); //[i];
                if (SCCs[i].Count <= SCCBound)
                {
                    SCCReductionGaussianSparseMatrix(new List<MDPState>(SCCs[i]), new List<MDPState>(scc2out[i]), true);
                }
                else
                {
                    HashSet<MDPState> working = new HashSet<MDPState>();
                    HashSet<MDPState> visited = new HashSet<MDPState>();
                    working.UnionWith(scc2input[i]);
                    visited.UnionWith(working);
                    do
                    {
                        HashSet<MDPState> newWorking = new HashSet<MDPState>();
                        foreach (MDPState state in working)
                        {
                            foreach (var distr in state.Distributions)
                            {
                                foreach (var tran in distr.States)
                                {

                                    MDPState tranV = tran.Value;
                                    if(!tranV.Pre.Contains(state))
                                    {
                                        tranV.Pre.Add(state);
                                    }

                                    if (!visited.Contains(tranV) && SCCs[i].Contains(tranV))
                                    {
                                        visited.Add(tranV);
                                        newWorking.Add(tranV);
                                    }

                                }

                            }
                        }
                        working = newWorking;
                    } while (working.Count > 0);

                    HashSet<MDPState> inputs = scc2input[i];
                    foreach(var input in inputs)
                    {
                        //MDPState input = inputs[j];
                        do
                        {
                            HashSet<MDPState> toRemove = new HashSet<MDPState>();
                            foreach (Distribution distr in input.Distributions)
                            {
                                foreach (var kvpair in distr.States)
                                {
                                    if (!inputs.Contains(kvpair.Value) && !scc2out[i].Contains(kvpair.Value))
                                    {
                                        toRemove.Add(kvpair.Value);
                                    }
                                }
                            }
                            if (toRemove.Count == 0) break;
                            HashSet<MDPState> group = new HashSet<MDPState>();
                            foreach (MDPState state in toRemove)
                            {
                                group.UnionWith(state.Pre);
                            }
                            group.UnionWith(toRemove);

                            HashSet<MDPState> outputs = new HashSet<MDPState>();
                            foreach (MDPState state in group)
                            {
                                foreach (Distribution Distr in state.Distributions)
                                {
                                    foreach (KeyValuePair<double, MDPState> KVpair in Distr.States)
                                    {
                                        MDPState nextState = KVpair.Value;
                                        if (!group.Contains(nextState))
                                        {
                                            outputs.Add(nextState);
                                        }
                                    }
                                }
                            }

                            List<MDPState> output = new List<MDPState>(outputs);
                            SCCReductionGaussianSparseMatrix(new List<MDPState>(group), output, true, true, toRemove);
                            foreach(var state in toRemove)
                            {
                                SCCs[i].Remove(state);
                            }
                            //if (visited.Count > SCCBound && visited.Count < SCCs[i].Count)
                            //{
                            //    SCCs[i] = visited;
                            //    firstCut = false;
                            //}
                            //else
                            //{
                            //    List<MDPState> scc = new List<MDPState>(visited);
                            //    SCCReductionGaussianSparseMatrix(scc, scc2out[i], firstCut);
                            //    //SCCReductionMatlab(scc, scc2out[i]);
                            //    i--;
                            //    firstCut = true;
                            //}
                        } while (true);
                        
                    }
                }
            }
        }

        protected void RandomLabel(HashSet<MDPState> inputs, HashSet<MDPState> visited, HashSet<MDPState> SCC)
        {
            HashSet<MDPState> Working = inputs;
            visited.UnionWith(Working);
            do
            {
                HashSet<MDPState> newWorking = new HashSet<MDPState>();
                foreach (MDPState state in Working)
                {
                    foreach (Distribution distr in state.Distributions)
                    {
                        foreach (KeyValuePair<double, MDPState> tran in distr.States)
                        {
                            MDPState tranV = tran.Value;
                            if (!visited.Contains(tranV) && SCC.Contains(tranV))
                            {
                                visited.Add(tranV);
                                newWorking.Add(tranV);
                            }
                        }
                    }
                }
                Working = newWorking;

            } while (Working.Count > 0);
        }

        protected void DFSLabel(HashSet<MDPState> inputs, HashSet<MDPState> visited, HashSet<MDPState> outputs, bool mixBFS)
        {
            
            HashSet<MDPState> newVisited = new HashSet<MDPState>();
            
            foreach (var Instate in inputs)
            {
                if (visited.Contains(Instate)) continue;
                Stack<MDPState> working = new Stack<MDPState>();
                Dictionary<string, List<MDPState>> ExpendedNode = new Dictionary<string, List<MDPState>>();
                working.Push(Instate);
                //visited.Add(Instate);
                //newVisited.Add(Instate);
                do
                {
                    MDPState currentState = working.Peek();
                    string currentID = currentState.ID;
                    bool done = true;
                    if (ExpendedNode.ContainsKey(currentID))
                    {

                        List<MDPState> list = ExpendedNode[currentID];
                        if (list.Count > 0)
                        {
                            for (int k = list.Count - 1; k >= 0; k--)
                            {
                                MDPState step = list[k];
                                if (mixBFS)
                                {
                                    if (!newVisited.Contains(step))
                                    {
                                        if (done)
                                        {
                                            newVisited.Add(step);
                                            working.Push(step);
                                            done = false;
                                            list.RemoveAt(k);
                                        }
                                        visited.Add(step);
                                    }
                                    else
                                    {
                                        list.RemoveAt(k);
                                    }
                                }
                                else
                                {
                                    if (!visited.Contains(step))
                                    {
                                        if (done)
                                        {
                                            visited.Add(step);
                                            working.Push(step);
                                            done = false;
                                            list.RemoveAt(k);
                                        }
                                    }
                                    else
                                    {
                                        list.RemoveAt(k);
                                    }
                                }
                                
                            }
                        }
                        else
                        {
                            working.Pop();
                        }
                    }
                    else
                    {
                        List<MDPState> stepsList = new List<MDPState>();
                        foreach (Distribution distr in currentState.Distributions)
                        {
                            foreach (KeyValuePair<double, MDPState> kvPair in distr.States)
                            {
                                if (outputs.Contains(kvPair.Value)) continue;
                                stepsList.Add(kvPair.Value);
                            }
                        }

                        for (int k = stepsList.Count - 1; k >= 0; k--)
                        {
                            MDPState nextState = stepsList[k];
                            if (mixBFS)
                            {
                                if (newVisited.Contains(nextState))
                                {
                                    stepsList.RemoveAt(k);
                                }
                                else
                                {
                                    if (done)
                                    {
                                        newVisited.Add(nextState);
                                        working.Push(nextState);
                                        done = false;
                                        stepsList.RemoveAt(k);
                                    }
                                    visited.Add(nextState);
                                }
                            }
                            else
                            {
                                if (visited.Contains(nextState))
                                {
                                    stepsList.RemoveAt(k);
                                }
                                else
                                {
                                    if (done)
                                    {
                                        visited.Add(nextState);
                                        working.Push(nextState);
                                        done = false;
                                        stepsList.RemoveAt(k);
                                    }
                                }
                            }
                            
                        }

                        ExpendedNode.Add(currentID, stepsList);
                    }
                } while (working.Count > 0);
            }
        }
       
        protected List<HashSet<MDPState>> TarjanSCC(MDPState initialState, HashSet<MDPState> OUTPUTS, HashSet<MDPState> visited, List<HashSet<MDPState>> scc2out, List<HashSet<MDPState>> scc2input)
        {
            const int VISITED_NOPREORDER = -1;
            const int SCC_FOUND = -2;
            int preordercounter = 0;
            Dictionary<string, List<MDPState>> ExpendedNode = new Dictionary<string, List<MDPState>>();
            Stack<MDPState> working = new Stack<MDPState>();
            working.Push(initialState);
            initialState.SCCIndex = -1;
            Stack<MDPState> stepStack = new Stack<MDPState>(1024);
            Dictionary<string, int[]> DFSData = new Dictionary<string, int[]>(Ultility.Ultility.MC_INITIAL_SIZE);
            DFSData.Add(initialState.ID, new int[] { VISITED_NOPREORDER, 0 });
            HashSet<MDPState> newVisited = new HashSet<MDPState>();
            newVisited.Add(initialState);
            List<HashSet<MDPState>> SCCs = new List<HashSet<MDPState>>();

            do
            {

                MDPState currentState = working.Peek();
                string currentID = currentState.ID;
                List<Distribution> outgoing = currentState.Distributions;

                int[] nodeData = DFSData[currentID];

                if (nodeData[0] == VISITED_NOPREORDER)
                {
                    nodeData[0] = preordercounter;
                    preordercounter++;
                }

                bool done = true;

                if (ExpendedNode.ContainsKey(currentID))
                {

                    List<MDPState> list = ExpendedNode[currentID];
                    if (list.Count > 0)
                    {
                        for (int k = list.Count - 1; k >= 0; k--)
                        {
                            MDPState step = list[k];
                            if (OUTPUTS.Contains(step)) continue;
                            string stepID = step.ID;
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
                                if (step.SCCIndex >= 0)
                                {
                                    scc2input[step.SCCIndex].Add(step);
                                }


                                list.RemoveAt(k);
                            }
                        }
                    }
                }
                else
                {
                    List<MDPState> stepsList = new List<MDPState>();
                    foreach (Distribution distr in currentState.Distributions)
                    {
                        foreach (KeyValuePair<double, MDPState> kvPair in distr.States)
                        {
                            stepsList.Add(kvPair.Value);
                        }
                    }

                    for (int k = stepsList.Count - 1; k >= 0; k--)
                    {
                        MDPState nextState = stepsList[k];
                        //string tmp = step.ID;
                        if (OUTPUTS.Contains(nextState)) continue;
                        int[] data;
                        if (newVisited.Contains(nextState))
                        {
                            DFSData.TryGetValue(nextState.ID, out data);
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
                                if (nextState.SCCIndex >= 0)
                                {
                                    scc2input[nextState.SCCIndex].Add(nextState);
                                }
                                stepsList.RemoveAt(k);
                            }
                        }
                        else
                        {
                            DFSData.Add(nextState.ID, new int[] { VISITED_NOPREORDER, 0 });
                            newVisited.Add(nextState);
                            nextState.SCCIndex = -1;

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
                    foreach (Distribution list in outgoing)
                    {
                        foreach (KeyValuePair<double, MDPState> state in list.States)
                        {
                            string w = state.Value.ID;
                            if (OUTPUTS.Contains(state.Value)) continue;
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
                    }

                    nodeData[1] = lowlinkV;
                    working.Pop();

                    if (lowlinkV == preorderV)
                    {

                        HashSet<MDPState> scc = new HashSet<MDPState>();
                        scc.Add(currentState);

                        nodeData[0] = SCC_FOUND;
                        while (stepStack.Count > 0 && DFSData[stepStack.Peek().ID][0] > preorderV)
                        {
                            MDPState s = stepStack.Pop();
                            string sID = s.ID;
                            scc.Add(s);
                            DFSData[sID][0] = SCC_FOUND;
                        }

                        if (scc.Count > 1 || selfLoop)
                        {
                            SCCs.Add(scc);
                            int Count = SCCs.Count;
                            foreach (MDPState state in scc)
                            {
                                state.SCCIndex = Count - 1;
                            }
                            HashSet<MDPState> inputs = new HashSet<MDPState>();
                            inputs.Add(currentState);
                            scc2input.Add(inputs);
                            HashSet<MDPState> outputs = new HashSet<MDPState>();

                            //input.Add(currentState);
                            foreach (MDPState state in scc)
                            {
                                foreach (Distribution distr in state.Distributions)
                                {
                                    foreach (KeyValuePair<double, MDPState> KVpair in distr.States)
                                    {
                                        if (!scc.Contains(KVpair.Value))
                                        {
                                            outputs.Add(KVpair.Value);
                                        }
                                    }
                                }

                            }
                            //List<MDPState> outofscc = new List<MDPState>(outputs);
                            scc2out.Add(outputs);
                        }
                        else
                        {
                            nodeData[0] = SCC_FOUND;
                            while (stepStack.Count > 0 && DFSData[stepStack.Peek().ID][0] > preorderV)
                            {
                                string sID = stepStack.Pop().ID;
                                DFSData[sID][0] = SCC_FOUND;
                            }
                        }


                    }
                    else
                    {
                        stepStack.Push(currentState);
                    }
                }

            } while (working.Count > 0);
            visited.UnionWith(newVisited);
            return SCCs;
        }

        protected MDP BuildMDP_SCC_Cut_Mutlicores()
        {
            Stack<KeyValuePair<MDPConfiguration, MDPState>> working = new Stack<KeyValuePair<MDPConfiguration, MDPState>>(1024);

            string initID = InitialStep.GetID();
            MDPState init = new MDPState(initID);
            working.Push(new KeyValuePair<MDPConfiguration, MDPState>(InitialStep as MDPConfiguration, init));
            MDP mdp = new MDP(init, Ultility.Ultility.DEFAULT_PRECISION, Ultility.Ultility.MAX_DIFFERENCE);

            Stack<MDPState> stepStack = new Stack<MDPState>(1024);
            //MDPState2DRAStateMapping = new Dictionary<string, int>(Common.Classes.Ultility.Ultility.MC_INITIAL_SIZE);
            const int VISITED_NOPREORDER = -1;
            const int SCC_FOUND = -2;
            const int SCCBound = 20;
            //DFS data, which mapping each state to an int[] of size 3, first is the pre-order, second is the lowlink, last one is DRAState
            Dictionary<string, int[]> DFSData = new Dictionary<string, int[]>(Ultility.Ultility.MC_INITIAL_SIZE);

            DFSData.Add(initID, new int[] { VISITED_NOPREORDER, 0 });
            //build the MDP while identifying the SCCs
            List<List<MDPState>> SCCs = new List<List<MDPState>>(64);
            Dictionary<int, List<MDPState>> scc2out = new Dictionary<int, List<MDPState>>();
            Dictionary<int, HashSet<MDPState>> scc2input = new Dictionary<int, HashSet<MDPState>>();
            //Dictionary<string, int> preorder = new Dictionary<string, int>(Common.Classes.Ultility.Ultility.MC_INITIAL_SIZE);
            //Dictionary<string, int> lowlink = new Dictionary<string, int>(Common.Classes.Ultility.Ultility.MC_INITIAL_SIZE);

            int preordercounter = 0;
            Dictionary<string, List<MDPConfiguration>> ExpendedNode = new Dictionary<string, List<MDPConfiguration>>();

            bool reachTarget = true;

            do
            {
                if (CancelRequested)
                {
                    this.VerificationOutput.NoOfStates = this.VerificationOutput.NoOfStates + mdp.States.Count;
                    return mdp;
                }

                KeyValuePair<MDPConfiguration, MDPState> pair = working.Peek();
                MDPConfiguration evt = pair.Key;
                //int DRAState = pair.Key.state;
                string currentID = pair.Key.GetID();
                MDPState currentState = pair.Value;
                List<Distribution> outgoing = currentState.Distributions;

                //if (!preorder.ContainsKey(currentID))
                //{
                //    preorder.Add(currentID, preorderCounter);
                //    preorderCounter++;
                //}

                int[] nodeData = DFSData[currentID];

                if (nodeData[0] == VISITED_NOPREORDER)
                {
                    nodeData[0] = preordercounter;
                    preordercounter++;
                }

                bool done = true;

                if (ExpendedNode.ContainsKey(currentID))
                {
                    if (reachTarget)
                    {
                        currentState.ReachTarget = reachTarget;
                    }
                    else
                    {
                        reachTarget = currentState.ReachTarget;
                    }
                    List<MDPConfiguration> list = ExpendedNode[currentID];
                    if (list.Count > 0)
                    {
                        for (int k = list.Count - 1; k >= 0; k--)
                        {
                            MDPConfiguration step = list[k];

                            string stepID = step.GetID();
                            //if (!preorder.ContainsKey(stepID))
                            if (DFSData[stepID][0] == VISITED_NOPREORDER)
                            {
                                if (done)
                                {
                                    working.Push(new KeyValuePair<MDPConfiguration, MDPState>(step, mdp.States[stepID]));
                                    done = false;
                                    list.RemoveAt(k);
                                }
                            }
                            else
                            {
                                MDPState s = mdp.States[stepID];
                                if (s.ReachTarget)
                                {
                                    reachTarget = true;
                                    currentState.ReachTarget = reachTarget;

                                    if (s.SCCIndex >= 0)
                                    {
                                        scc2input[s.SCCIndex].Add(s);
                                    }

                                }
                                list.RemoveAt(k);
                            }
                        }
                    }
                }
                else
                {
                    reachTarget = false;
                    int currentDistriIndex = -1;
                    Distribution newDis = new Distribution(Constants.TAU);

                    //MDPConfiguration[] steps = evt.MakeOneMoveLocal();
                    IEnumerable<MDPConfiguration> steps = evt.MakeOneMoveLocal();
                    //NOTE: here we play a trick for deadlock case: if a deadlock exist in the MDP, we will make a
                    //self loop transition to remove the deadlock. Deadlock is meaningless in MDP.
                    if (evt.IsDeadLock)
                    {
                        //List<MDPConfiguration> stepsList = new List<MDPConfiguration>(steps);
                        working.Pop();
                        nodeData[0] = SCC_FOUND;
                        continue;
                        //stepsList.Add(CreateSelfLoopStep(evt));
                        //steps = stepsList.ToArray();
                        //HasDeadLock = true;
                    }
                    List<MDPConfiguration> stepsList = new List<MDPConfiguration>(steps);
                    //List<PCSPEventDRAPair> product = Next(steps, DRAState);
                    this.VerificationOutput.Transitions += stepsList.Count;

                    for (int k = stepsList.Count - 1; k >= 0; k--)
                    {
                        MDPConfiguration step = stepsList[k];
                        string tmp = step.GetID();
                        bool target = false;
                        ExpressionValue v = EvaluatorDenotational.Evaluate(ReachableStateCondition, step.GlobalEnv);

                        MDPState nextState;

                        if ((v as BoolConstant).Value)
                        {
                            target = true;
                            if (!DFSData.Keys.Contains(tmp))
                            {
                                DFSData.Add(tmp, new int[] { SCC_FOUND, SCC_FOUND });
                                //    nextState = new MDPState(tmp);
                                //    mdp.States.Add(tmp, nextState);
                                //preordercounter++;
                            }
                            //else if(DFSData[tmp][0] == VISITED_NOPREORDER)
                            //{
                            //    DFSData[tmp][0] = preordercounter;
                            //    preordercounter++;
                            //}
                        }
                        //int nextIndex = VisitedWithID.Count;

                        int[] data;
                        if (mdp.States.TryGetValue(tmp, out nextState))
                        {
                            DFSData.TryGetValue(tmp, out data);
                            if (!target && data[0] == VISITED_NOPREORDER)
                            {
                                //if (mdp.States.TryGetValue(tmp, out nextState))
                                //{

                                //    if (!preorder.ContainsKey(tmp))
                                //    {
                                if (done)
                                {
                                    working.Push(new KeyValuePair<MDPConfiguration, MDPState>(step, nextState));
                                    done = false;
                                    stepsList.RemoveAt(k);
                                }
                                else
                                {
                                    stepsList[k] = step;
                                }
                            }
                            else
                            {
                                stepsList.RemoveAt(k);
                                MDPState s = mdp.States[tmp];
                                if (s.ReachTarget)
                                {
                                    reachTarget = true;
                                    currentState.ReachTarget = reachTarget;
                                    if (s.SCCIndex >= 0)
                                    {
                                        scc2input[s.SCCIndex].Add(s);
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (!target)
                            {
                                DFSData.Add(tmp, new int[] { VISITED_NOPREORDER, 0 });
                            }

                            nextState = new MDPState(tmp);
                            mdp.States.Add(tmp, nextState);
                            if (done)
                            {
                                if (target)
                                {
                                    mdp.AddTargetStates(nextState);
                                    reachTarget = true;
                                }
                                else
                                {
                                    working.Push(new KeyValuePair<MDPConfiguration, MDPState>(step, nextState));
                                }

                                done = false;
                                stepsList.RemoveAt(k);
                            }
                            else
                            {
                                if (target)
                                {
                                    stepsList.RemoveAt(k);
                                    mdp.AddTargetStates(nextState);
                                    reachTarget = true;
                                }
                                else
                                {
                                    stepsList[k] = step;
                                }
                            }
                        }

                        MDPConfiguration pstep = step;

                        if (pstep.DisIndex == -1)
                        {
                            if (currentDistriIndex >= 0)
                            {
                                //currentState.AddDistribution(newDis); 
                                //note here no pre is stored
                                currentState.Distributions.Add(newDis);
                                newDis = new Distribution(Constants.TAU);
                            }

                            Distribution newTrivialDis = new Distribution(pstep.Event, nextState);
                            //currentState.AddDistribution(newTrivialDis);
                            currentState.Distributions.Add(newTrivialDis);
                        }
                        else if (currentDistriIndex != -1 && pstep.DisIndex != currentDistriIndex)
                        {
                            //currentState.AddDistribution(newDis);
                            currentState.Distributions.Add(newDis);
                            newDis = new Distribution(Constants.TAU);
                            newDis.AddProbStatePair(pstep.Probability, nextState);
                        }
                        else
                        {
                            newDis.AddProbStatePair(pstep.Probability, nextState);
                        }

                        currentDistriIndex = pstep.DisIndex;
                    }

                    if (currentDistriIndex >= 0)
                    {
                        //currentState.AddDistribution(newDis);
                        currentState.Distributions.Add(newDis);
                    }

                    ExpendedNode.Add(currentID, stepsList);
                }

                if (done)
                {
                    int lowlinkV = nodeData[0];
                    int preorderV = lowlinkV;

                    bool selfLoop = false;
                    foreach (Distribution list in outgoing)
                    {
                        foreach (KeyValuePair<double, MDPState> state in list.States)
                        {
                            string w = state.Value.ID;

                            if (w == currentID)
                            {
                                selfLoop = true;
                            }

                            //if (!MDPState2DRAStateMapping.ContainsKey(w))
                            //{
                            //    if (preorder[w] > preorderV)
                            //    {
                            //        lowlinkV = Math.Min(lowlinkV, lowlink[w]);
                            //    }
                            //    else
                            //    {
                            //        lowlinkV = Math.Min(lowlinkV, preorder[w]);
                            //    }
                            //}

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
                    }

                    nodeData[1] = lowlinkV;
                    working.Pop();

                    if (lowlinkV == preorderV)
                    {
                        if (currentState.ReachTarget)
                        {
                            List<MDPState> SCC = new List<MDPState>();
                            //List<HashSet<MDPState>> Groupings = new List<HashSet<MDPState>>();
                            HashSet<MDPState> Grouping = new HashSet<MDPState>();
                            //bool reduceScc = false;
                            Grouping.Add(currentState);
                            SCC.Add(currentState);
                            //int count = 1;
                            //dtmcSCCstates.Add(currentID);
                            nodeData[0] = SCC_FOUND;
                            while (stepStack.Count > 0 && DFSData[stepStack.Peek().ID][0] > preorderV)
                            {
                                MDPState s = stepStack.Pop();
                                s.ReachTarget = true;
                                string sID = s.ID;
                                //Grouping.Add(s);
                                SCC.Add(s);
                                //dtmcSCCstates.Add(sID);
                                DFSData[sID][0] = SCC_FOUND;
                                //count++;
                            }

                            if (SCC.Count > 1 || selfLoop)
                            {
                                SCCs.Add(SCC);
                                HashSet<MDPState> inputs = new HashSet<MDPState>();
                                inputs.Add(currentState);
                                int Count = SCCs.Count;
                                foreach (var state in SCC)
                                {
                                    state.SCCIndex = Count - 1;
                                }
                                scc2input.Add(currentState.SCCIndex, inputs);

                                HashSet<MDPState> outputs = new HashSet<MDPState>();

                                //input.Add(currentState);
                                foreach (MDPState state in SCC)
                                {
                                    foreach (var distr in state.Distributions)
                                    {
                                        foreach (var KVpair in distr.States)
                                        {
                                            if (!SCC.Contains(KVpair.Value))
                                            {
                                                outputs.Add(KVpair.Value);
                                            }
                                        }
                                    }

                                }
                                List<MDPState> outofscc = new List<MDPState>(outputs);
                                scc2out.Add(SCCs.IndexOf(SCC), outofscc);
                            }
                            //}
                        }
                        else
                        {
                            //dtmcSCCstates.Add(currentID);
                            nodeData[0] = SCC_FOUND;
                            while (stepStack.Count > 0 && DFSData[stepStack.Peek().ID][0] > preorderV)
                            {
                                string sID = stepStack.Pop().ID;
                                DFSData[sID][0] = SCC_FOUND;
                            }
                        }

                    }
                    else
                    {
                        stepStack.Push(pair.Value);
                    }
                }
            } while (working.Count > 0);

            //List<string> EndComponents = new List<string>(SCCs.Count);

            //foreach (List<MDPState> scc in SCCs)
            var localMapSCCsWithOutputSmallSCC = new Dictionary<List<MDPState>, List<MDPState>>();
            while (SCCs.Count > 0)
            {

                for (int i = SCCs.Count - 1; i >= 0; i--)
                {
                    List<MDPState> SCC = SCCs[i];
                    int index;
                    //foreach(var state in SCC)
                    //{
                    //    index = state.SCCIndex;
                    //    break;
                    //}
                    //FindMECDistribution(SCC);
                    if (SCC.Count <= SCCBound)
                    {
                        // SCCReductionGaussianSparseMatrix(new List<MDPState>(SCC), scc2out[index]);
                        localMapSCCsWithOutputSmallSCC.Add(new List<MDPState>(SCC), scc2out[i]);
                        SCCs.RemoveAt(i);
                    }
                    else
                    {
                        List<MDPState> SCCi = new List<MDPState>(SCCs[i]);
                        int Counter = 0;
                        var localMapSCCsWithOutputSubCutSCC = new Dictionary<List<MDPState>, List<MDPState>>();
                        while (Counter < SCCi.Count - SCCBound)
                        {
                            List<MDPState> group = SCCi.GetRange(Counter, SCCBound);
                            HashSet<MDPState> outputs = new HashSet<MDPState>();

                            //input.Add(currentState);
                            foreach (MDPState state in group)
                            {
                                foreach (var Distr in state.Distributions)
                                {
                                    foreach (KeyValuePair<double, MDPState> KVpair in Distr.States)
                                    {
                                        MDPState nextState = KVpair.Value;
                                        if (!group.Contains(nextState))
                                        {
                                            outputs.Add(nextState);
                                        }
                                    }
                                }
                            }

                            // List<MDPState> output = new List<MDPState>(outputs);
                            //SCCReductionGaussianSparseMatrix(group, output);
                            localMapSCCsWithOutputSubCutSCC.Add(group, new List<MDPState>(outputs));
                            //SCCReduction(group, output);
                            Counter += SCCBound;
                        }
                        Parallel.ForEach(localMapSCCsWithOutputSubCutSCC,
                                         subscc2Out =>
                                         SCCReductionGaussianSparseMatrix(subscc2Out.Key, subscc2Out.Value, true));

                        List<MDPState> Group = SCCi.GetRange(Counter, SCCi.Count - Counter);
                        HashSet<MDPState> Outputs = new HashSet<MDPState>();

                        //input.Add(currentState);
                        foreach (MDPState state in Group)
                        {
                            foreach (var Distr in state.Distributions)
                            {
                                foreach (var KVpair in Distr.States)
                                {
                                    MDPState nextState = KVpair.Value;
                                    if (!Group.Contains(nextState))
                                    {
                                        Outputs.Add(nextState);
                                    }
                                }
                            }
                        }

                        List<MDPState> Output = new List<MDPState>(Outputs);

                        //GL: TODO: this one is not in parallel
                        SCCReductionGaussianSparseMatrix(Group, Output, true);

                        //search for the useful subset of SCC that connecting to input states after reduction. 
                        HashSet<MDPState> Working = scc2input[i];
                        HashSet<MDPState> visited = new HashSet<MDPState>();
                        do
                        {
                            HashSet<MDPState> newWorking = new HashSet<MDPState>();
                            foreach (var state in Working)
                            {
                                visited.Add(state);
                                foreach (var distr in state.Distributions)
                                {
                                    foreach (var tran in distr.States)
                                    {
                                        MDPState tranV = tran.Value;
                                        if (!visited.Contains(tranV) && SCCs[i].Contains(tranV))
                                        {
                                            newWorking.Add(tranV);
                                        }
                                    }
                                }
                            }
                            Working = newWorking;

                        } while (Working.Count > 0);



                        if (visited.Count > SCCBound && visited.Count < SCCs[i].Count)
                        {
                            SCCs[i] = new List<MDPState>(visited);
                        }
                        else
                        {
                            //List<MDPState> scc = new List<MDPState>(SCCs[i]);
                            // SCCReductionGaussianSparseMatrix(scc, scc2out[index]);
                            //SCCReductionMatlab(scc, scc2out[i]);
                            localMapSCCsWithOutputSmallSCC.Add(new List<MDPState>(visited), scc2out[i]);
                            SCCs.RemoveAt(i);
                        }
                    }

                }
            }
            Parallel.ForEach(localMapSCCsWithOutputSmallSCC,
                             subscc2Out =>
                             SCCReductionGaussianSparseMatrix(subscc2Out.Key, subscc2Out.Value, true));


            VerificationOutput.NoOfStates = VerificationOutput.NoOfStates + mdp.States.Count;
            int counter = 0;
            BuildPRE(mdp, ref counter);

            return mdp;
        }

        private void BuildPRE(MDP mdp, ref int counter)
        {
            HashSet<MDPState> working = new HashSet<MDPState>();
            HashSet<MDPState> visited = new HashSet<MDPState>();
            working.Add(mdp.InitState);
            visited.Add(mdp.InitState);
            do
            {
                HashSet<MDPState> newWorking = new HashSet<MDPState>();
                foreach (MDPState state in working)
                {
                    counter++;
                    foreach (var distr in state.Distributions)
                    {
                        foreach (var tran in distr.States)
                        {

                            MDPState tranV = tran.Value;
                            if (tranV.ReachTarget)
                            {
                                if (!tranV.Pre.Contains(state))
                                {
                                    tranV.Pre.Add(state);
                                }
                                if (!visited.Contains(tranV))
                                {
                                    visited.Add(tranV);
                                    newWorking.Add(tranV);
                                }
                            }
                        }

                    }
                }
                working = newWorking;
            } while (working.Count > 0);

            return;
        }

        protected MDP BuildMDP()
        {
            Stack<KeyValuePair<MDPConfiguration, MDPState>> working = new Stack<KeyValuePair<MDPConfiguration, MDPState>>(1024);

            string initID = InitialStep.GetID();
            MDPState init = new MDPState(initID);
            working.Push(new KeyValuePair<MDPConfiguration, MDPState>(InitialStep as MDPConfiguration, init));
            MDP mdp = new MDP(init, Ultility.Ultility.DEFAULT_PRECISION, Ultility.Ultility.MAX_DIFFERENCE);

            do
            {
                if (CancelRequested)
                {
                    VerificationOutput.NoOfStates = mdp.States.Count;
                    return mdp;
                }

                KeyValuePair<MDPConfiguration, MDPState> current = working.Pop();
                IEnumerable<MDPConfiguration> list = current.Key.MakeOneMoveLocal();
                VerificationOutput.Transitions += list.Count();

                int currentDistriIndex = -1;
                Distribution newDis = new Distribution(Constants.TAU);

                //for (int i = 0; i < list.Count; i++)
                foreach (var step in list)
                {
                    //MDPConfiguration step = list[i];
                    string stepID = step.GetID();
                    MDPState nextState;

                    if (!mdp.States.TryGetValue(stepID, out nextState))
                    {
                        nextState = new MDPState(stepID);
                        mdp.AddState(nextState);

                        ExpressionValue v = EvaluatorDenotational.Evaluate(ReachableStateCondition, step.GlobalEnv);

                        if ((v as BoolConstant).Value)
                        {
                            mdp.AddTargetStates(nextState);
                        }
                        else
                        {
                            working.Push(new KeyValuePair<MDPConfiguration, MDPState>(step, nextState));
                        }
                    }

                    if (step.DisIndex == -1)
                    {
                        if (currentDistriIndex >= 0)
                        {
                            current.Value.AddDistribution(newDis);
                            newDis = new Distribution(Constants.TAU);
                        }

                        Distribution newTrivialDis = new Distribution(step.Event);
                        newTrivialDis.AddProbStatePair(1, nextState);
                        current.Value.AddDistribution(newTrivialDis);
                    }
                    else if (currentDistriIndex != -1 && step.DisIndex != currentDistriIndex)
                    {
                        current.Value.AddDistribution(newDis);
                        newDis = new Distribution(Constants.TAU);
                        newDis.AddProbStatePair(step.Probability, nextState);
                    }
                    else
                    {
                        newDis.AddProbStatePair(step.Probability, nextState);
                    }

                    currentDistriIndex = step.DisIndex;
                }

                if (currentDistriIndex >= 0)
                {
                    current.Value.AddDistribution(newDis);
                }
            } while (working.Count > 0);

            VerificationOutput.NoOfStates = mdp.States.Count;
            //mdp.BackUpTargetStates();
            return mdp;
        }

        private void SCCReductionGaussianSparseMatrixTreeStructure(List<MDPState> SCC, List<MDPState> outputs, bool firstCut, bool bfs = false, HashSet<MDPState> toRemove = null)
        {
            var smatrix = LinearEquationsSolver.Read2MatrixToSparseMatrix(SCC, outputs);

            LinearEquationsSolver.GaussianJordenEliminationForTreeStructure(smatrix, firstCut);
            int r = smatrix.Ngroup;
            //there, the distributions in matrix are updated and changed to another set that connecting SCC states with output states. 
            for (int i = 0; i < r; i++)  //TODO: change groups from List to Dictionary, may improve efficiency
            {
                MDPState state = SCC[i];
                if (bfs)
                {
                    state.RemoveDistributions();
                }
                else
                {
                    state.Distributions.Clear();
                }
                if (bfs && toRemove.Contains(state)) continue;
                foreach (var row in smatrix.Groups[i].RowsInSameGroup)
                {
                    var dic = new Distribution("composed"); //can add dic name with relation to the memoryless schedulers choices
                    for (int j = 0; j < row.col.Count; j++)
                    {
                        //dic.States.Add(new KeyValuePair<double, MDPState>(-row.val[j], outputs[j-r]));
                        dic.States.Add(new KeyValuePair<double, MDPState>(-row.val[j], outputs[row.col[j] - r]));
                    }
                    if (bfs)
                    {
                        state.AddDistribution(dic);
                    }
                    else
                    {
                        state.Distributions.Add(dic);
                    }
                }

            }

        }

        private void SCCReductionGaussianSparseMatrix(List<MDPState> SCC, List<MDPState> outputs, bool firstCut, bool bfs = false, HashSet<MDPState> toRemove = null)
        {
            var smatrix = LinearEquationsSolver.Read2MatrixToSparseMatrix(SCC, outputs);

            LinearEquationsSolver.GaussianJordenElimination(smatrix, firstCut);
            int r = smatrix.Ngroup;
            //there, the distributions in matrix are updated and changed to another set that connecting SCC states with output states. 
            for (int i = 0; i < r; i++)  //TODO: change groups from List to Dictionary, may improve efficiency
            {
                MDPState state = SCC[i];
                if (bfs)
                {
                    state.RemoveDistributions();
                }
                else
                {
                    state.Distributions.Clear();
                }
                if(bfs && toRemove.Contains(state)) continue;
                foreach (var row in smatrix.Groups[i].RowsInSameGroup)
                {
                    var dic = new Distribution("composed"); //can add dic name with relation to the memoryless schedulers choices
                    for (int j = 0; j < row.col.Count; j++)
                    {
                        //dic.States.Add(new KeyValuePair<double, MDPState>(-row.val[j], outputs[j-r]));
                        dic.States.Add(new KeyValuePair<double, MDPState>(-row.val[j], outputs[row.col[j] - r]));
                    }
                    if(bfs)
                    {
                        state.AddDistribution(dic);  
                    }
                    else
                    {
                        state.Distributions.Add(dic);
                    }
                }

            }

        }

        //protected MDP BuildDTMCSSZ()
        //{
        //    Stack<KeyValuePair<MDPConfiguration, MDPState>> working = new Stack<KeyValuePair<MDPConfiguration, MDPState>>(1024);

        //    string initID = InitialStep.GetID();
        //    MDPState init = new MDPState(initID);
        //    //working.Push(new KeyValuePair<MDPConfiguration, MDPState>(InitialStep as MDPConfiguration, init));
        //    KeyValuePair<MDPConfiguration, MDPState> Init = new KeyValuePair<MDPConfiguration, MDPState>(InitialStep as MDPConfiguration, init);
        //    MDP mdp = new MDP(init, Ultility.Ultility.DEFAULT_PRECISION, Ultility.Ultility.MAX_DIFFERENCE);

        //    List<KeyValuePair<MDPState, double>> currentTrace = new List<KeyValuePair<MDPState, double>>();

        //    DTMCDFS(mdp, currentTrace, Init);

        //    return mdp;
        //    //do
        //    //{
        //    //    if (CancelRequested)
        //    //    {
        //    //        VerificationOutput.NoOfStates = mdp.States.Count;
        //    //        return mdp;
        //    //    }

        //    //    KeyValuePair<MDPConfiguration, MDPState> current = working.Pop();
        //    //    MDPConfiguration[] list = current.Key.MakeOneMoveLocal();
        //    //    VerificationOutput.Transitions += list.Length;

        //    //    int currentDistriIndex = -1;
        //    //    Distribution newDis = new Distribution(Constants.TAU);

        //    //    //for (int i = 0; i < list.Length; i++)
        //    //    //{
        //    //        MDPConfiguration step = list[0];
        //    //        string stepID = step.GetID();
        //    //        MDPState nextState;

        //    //        if (!mdp.States.TryGetValue(stepID, out nextState))
        //    //        {
        //    //            nextState = new MDPState(stepID);
        //    //            mdp.AddState(nextState);


        //    //            ExpressionValue v = EvaluatorDenotational.Evaluate(ReachableStateCondition, step.GlobalEnv);

        //    //            if ((v as BoolConstant).Value)
        //    //            {
        //    //                mdp.AddTargetStates(nextState);
        //    //            }
        //    //            else
        //    //            {
        //    //                working.Push(new KeyValuePair<MDPConfiguration, MDPState>(step, nextState));
        //    //            }
        //    //        }

        //    //        if (step.DisIndex == -1)
        //    //        {
        //    //            if (currentDistriIndex >= 0)
        //    //            {
        //    //                current.Value.AddDistribution(newDis);
        //    //                newDis = new Distribution(Constants.TAU);
        //    //            }

        //    //            Distribution newTrivialDis = new Distribution(step.Event);
        //    //            newTrivialDis.AddProbStatePair(1, nextState);
        //    //            current.Value.AddDistribution(newTrivialDis);
        //    //        }
        //    //        else if (currentDistriIndex != -1 && step.DisIndex != currentDistriIndex)
        //    //        {
        //    //            current.Value.AddDistribution(newDis);
        //    //            newDis = new Distribution(Constants.TAU);
        //    //            newDis.AddProbStatePair(step.Probability, nextState);
        //    //        }
        //    //        else
        //    //        {
        //    //            newDis.AddProbStatePair(step.Probability, nextState);
        //    //        }

        //    //        currentDistriIndex = step.DisIndex;
        //    //    //}

        //    //    if (currentDistriIndex >= 0)
        //    //    {
        //    //        current.Value.AddDistribution(newDis);
        //    //    }
        //    //} while (working.Count > 0);

        //    //VerificationOutput.NoOfStates = mdp.States.Count;
        //    //return mdp;
        //}

        //private void DTMCDFS(MDP mdp, List<KeyValuePair<MDPState, double>> currentTrace, KeyValuePair<MDPConfiguration, MDPState> current)
        //{

        //    MDPConfiguration[] list = current.Key.MakeOneMoveLocal();

        //    for(int i = 0; i < list.Length; i++)
        //    {
        //        MDPConfiguration step = list[i];
        //        string stepID = step.GetID();
        //        MDPState nextState;

        //        currentTrace.Add(new KeyValuePair<MDPState, double>(current.Value, step.Probability));

        //        if (!mdp.States.TryGetValue(stepID, out nextState))
        //        {
        //            nextState = new MDPState(stepID);
        //            mdp.AddState(nextState);


        //            ExpressionValue v = EvaluatorDenotational.Evaluate(ReachableStateCondition, step.GlobalEnv);

        //            if ((v as BoolConstant).Value)
        //            {
        //                mdp.AddTargetStates(nextState);
        //            }
        //            else
        //            {
        //                DTMCDFS(mdp, currentTrace, new KeyValuePair<MDPConfiguration, MDPState>(step, nextState));
        //            }
        //        }
        //        else
        //        {
        //            //double loopProb = 1;

        //            for (int j = currentTrace.Count - 1; j >=0; j--)
        //            {
        //                list.
        //                    //loopProb *= currentTrace[j].Value;
        //                    if (currentTrace[j].Key == nextState) break;
        //            }
        //        }

        //    }

        //}

        //protected virtual MDP GetTransitionRelation()///max means we want to calculate the maximal reachability.
        //{
        //    //List<int> Targets = new List<int>();

        //    Dictionary<string, int> Visited =
        //        new Dictionary<string, int>(Common.Classes.Ultility.Ultility.MC_INITIAL_SIZE);
        //    Stack<MDPConfiguration> working = new Stack<MDPConfiguration>(1024);

        //    Visited.Add(InitialStep.GetID(), Visited.Count);
        //    working.Push(InitialStep as MDPConfiguration);

        //    MDP mdp = new MDP(0, Ultility.Ultility.DEFAULT_PRECISION, Ultility.Ultility.MAX_DIFFERENCE);

        //    do
        //    {
        //        if (CancelRequested)
        //        {
        //            this.VerificationOutput.NoOfStates = Visited.Count;
        //            return mdp;
        //        }

        //        MDPConfiguration current = working.Pop();
        //        int currentIndex = Visited[current.GetID()];
        //        List<MDPConfiguration> list = current.MakeOneMoveLocal();
        //        this.VerificationOutput.Transitions += list.Count;

        //        int currentDistriIndex = -1;

        //        for (int i = 0; i < list.Count; i++)
        //        {
        //            MDPConfiguration step = list[i];
        //            string stepID = step.GetID();
        //            int nextIndex = Visited.Count;

        //            if (Visited.ContainsKey(stepID))
        //            {
        //                nextIndex = Visited[stepID];
        //            }

        //            //add a seperator into the transition relation table to seperate different distributions
        //            if (mdp.ContainsState(currentIndex) && (step.DisIndex == -1 || step.DisIndex != currentDistriIndex))
        //            {
        //                mdp.AddSeperator(currentIndex);
        //            }

        //            currentDistriIndex = step.DisIndex;

        //            mdp.AddTransition(currentIndex, nextIndex, step.Probability);

        //            if (!Visited.ContainsKey(stepID))
        //            {
        //                Visited.Add(stepID, nextIndex);                      

        //                ExpressionValue v = EvaluatorDenotational.Evaluate(ReachableStateCondition, step.GlobalEnv);
        //                if ((v as BoolConstant).Value)
        //                {
        //                    mdp.TargetStates.Add(nextIndex);
        //                }
        //                else
        //                {
        //                    working.Push(step);
        //                }
        //            }
        //        }
        //    } while (working.Count > 0);

        //    //mdp.SetTarget(Targets, isMax);
        //    this.VerificationOutput.NoOfStates = Visited.Count;
        //    return mdp;
        //}

        public override string GetResultString()
        {
            if (ConstraintType == QueryConstraintType.NONE)
            {
                return base.GetResultString();
            }

            StringBuilder sb = new StringBuilder();

            sb.AppendLine(Constants.VERFICATION_RESULT_STRING);
            if (this.VerificationOutput.VerificationResult == VerificationResultType.VALID)
            {
                sb.AppendLine("The Assertion (" + ToString() + ") is VALID.");
            }
            else if (this.VerificationOutput.VerificationResult == VerificationResultType.UNKNOWN)
            {
                sb.AppendLine("The Assertion (" + ToString() + ") is NEITHER PROVED NOR DISPROVED.");
            }
            else if (this.VerificationOutput.VerificationResult == VerificationResultType.WITHPROBABILITY)
            {
                //sb.AppendLine("\t***The Assertion (" + ToString() + ") is Valid With Probability " + Ultility.Ultility.GetProbIntervalString(Min, Max, Precision) + ".");
                if (this.Max != -1 && Min != -1)
                {
                    sb.AppendLine("The Assertion (" + ToString() + ") is Valid with Probability " + Ultility.Ultility.GetProbIntervalString(Min, Max) + ";");
                }
                else if (this.Max != -1)
                {
                    sb.AppendLine("The Assertion (" + ToString() + ") is Valid with Max Probability " + Ultility.Ultility.GetProbIntervalString(Max) + ";");
                }
                else if (this.Min != -1)
                {
                    sb.AppendLine("The Assertion (" + ToString() + ") is Valid with Min Probability " + Ultility.Ultility.GetProbIntervalString(Min) + ";");
                }
            }
            else
            {
                sb.AppendLine("The Assertion (" + ToString() + ") is NOT valid.");
                sb.AppendLine();
            }

            sb.AppendLine();

            sb.AppendLine("********Verification Setting********");
            sb.AppendLine("Admissible Behavior: " + SelectedBahaviorName);
            sb.AppendLine("Search Engine: " + SelectedEngineName);
            sb.AppendLine("System Abstraction: " + MustAbstract);
            sb.AppendLine("Maximum difference threshold : " + Ultility.Ultility.MAX_DIFFERENCE);


            sb.AppendLine();

            return sb.ToString();
        }

        //public override string StartingProcess
        //{
        //    get { return Process.ToString(); }
        //}
    }
}