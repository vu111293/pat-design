using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using Antlr.Runtime;
using PAT.Common.Classes.BA;
using PAT.Common.Classes.LTL2DRA;
using PAT.Common.Classes.ModuleInterface;
using PAT.Common.Classes.Ultility;


namespace PAT.Common.Classes.SemanticModels.MDP.Assertion
{
    public abstract partial class AssertionLTL : LTS.Assertion.AssertionLTL
    {
        protected QueryConstraintType ConstraintType;
        protected DRA DRA;
        public DRA PositiveDRA;
        public DRA NegativeDRA;
        public BuchiAutomata PositiveBA;
        protected double Min = -1;
        protected double Max = -1;
        protected MDP mdp;
        protected Dictionary<string, int> MDPState2DRAStateMapping;
        private bool HasDeadLock;
        //private DefinitionRef Process;
        public const string DUMMY_INIT = "dummy";

        private bool AlmostFair = false;
  

        protected AssertionLTL(string ltl) : base(ltl)
        {
        }


        public override void Initialize(SpecificationBase spec)
        {
            ltl2ba.Node LTLHeadNode;
            
            switch (ConstraintType)
            {
                case QueryConstraintType.PROB:
                    //create the DRA from the BA
                    LTLHeadNode = LTL2BA.ParseLTL("!(" + this.LTLString + ")", "", new CommonToken(0, ""));
                    NegativeDRA = BA2DRAConverter.ConvertBA2DRA(BA, LTLHeadNode);
                    NegativeDRA.DeclarationDatabase = BA.DeclarationDatabase;

                    //todo: builf the positve DRA too
                    //create the DRA from the BA
                    LTLHeadNode = LTL2BA.ParseLTL(this.LTLString, "", new CommonToken(0, ""));
                    PositiveDRA = BA2DRAConverter.ConvertBA2DRA(PositiveBA, LTLHeadNode);
                    PositiveDRA.DeclarationDatabase = BA.DeclarationDatabase;
                    break;
                case QueryConstraintType.PMAX:
                    //create the DRA from the BA
                    LTLHeadNode = LTL2BA.ParseLTL(this.LTLString, "", new CommonToken(0, ""));
                    PositiveDRA = BA2DRAConverter.ConvertBA2DRA(PositiveBA, LTLHeadNode);
                    PositiveDRA.DeclarationDatabase = BA.DeclarationDatabase;
                    break;
                case QueryConstraintType.PMIN:
                    //create the DRA from the BA
                    if(AlmostFair)//note add by ssz
                    {
                        LTLHeadNode = LTL2BA.ParseLTL(this.LTLString, "", new CommonToken(0, ""));
                        PositiveDRA = BA2DRAConverter.ConvertBA2DRA(PositiveBA, LTLHeadNode);
                        PositiveDRA.DeclarationDatabase = BA.DeclarationDatabase;
                    }
                    else
                    {
                        LTLHeadNode = LTL2BA.ParseLTL("!(" + this.LTLString + ")", "", new CommonToken(0, ""));
                        NegativeDRA = BA2DRAConverter.ConvertBA2DRA(BA, LTLHeadNode);
                        NegativeDRA.DeclarationDatabase = BA.DeclarationDatabase;
                    }
                    break;
                    
            }

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

#if DEBUG
                LTLEngine.Add(Constants.ENGINE_MDP_MEC_SEARCH);
                LTLEngine.Add(Constants.ENGINE_MDP_SIM_SEARCH);

#endif

                ModelCheckingOptions.AddAddimissibleBehavior(Constants.COMPLETE_BEHAVIOR, LTLEngine);               
            }
        }


        public override string ToString()
        {
            if (ConstraintType == QueryConstraintType.NONE)
            {
                return base.ToString();
            }
            else
            {
                return StartingProcess.ToString() + " |= " + LTLString + " with " + ConstraintType.ToString().ToLower();     
            }
            
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

            if (IsSafety)
            {
                RunVerificationSafety();
                return;
            }

            if (!IsNegateLiveness)
            {
                RunVerificationNegate();
                return;
            }

            if (SelectedEngineName == Constants.ENGINE_MDP_SEARCH)
            {

                switch (ConstraintType)
                {
                    case QueryConstraintType.PROB:
                        DRA = NegativeDRA;
                        BuildMDP(); //note this function is just used to calculate the maximal probability.
                        Min = 1 - mdp.MaxProbability(VerificationOutput);

                        DRA = PositiveDRA;
                        BuildMDP();
                        Max = mdp.MaxProbability(VerificationOutput);

                        break;
                    case QueryConstraintType.PMAX:
                        DRA = PositiveDRA;
                        BuildMDP();
                        Max = mdp.MaxProbability(VerificationOutput);
                        break;
                    case QueryConstraintType.PMIN:
                        if (AlmostFair) //note add by ssz
                        {
                            DRA = PositiveDRA;
                            BuildMDP();
                            Min = mdp.MinProbability(VerificationOutput);
                        }
                        else
                        {
                            DRA = NegativeDRA;
                            BuildMDP();
                            Min = 1 - mdp.MaxProbability(VerificationOutput);
                        }
                        break;
                }
            }
            else if (SelectedEngineName == Constants.ENGINE_MDP_MEC_SEARCH)
            {

                switch (ConstraintType)
                {
                    case QueryConstraintType.PROB:
                        DRA = NegativeDRA;
                        BuildMD_ImprovedTarjan(); //note this function is just used to calculate the maximal probability.
                        Min = 1 - mdp.MaxProbability(VerificationOutput);

                        DRA = PositiveDRA;
                        BuildMD_ImprovedTarjan();
                        Max = mdp.MaxProbability(VerificationOutput);

                        break;
                    case QueryConstraintType.PMAX:
                        DRA = PositiveDRA;
                        //BuildMDP();
                        BuildMD_ImprovedTarjan();
                        Max = mdp.MaxProbability(VerificationOutput);
                        break;
                    case QueryConstraintType.PMIN:
                        if (AlmostFair) //note add by ssz
                        {
                            DRA = PositiveDRA;
                            BuildMD_ImprovedTarjan();
                            Min = mdp.MinProbability(VerificationOutput);
                        }
                        else
                        {
                            DRA = NegativeDRA;
                            BuildMD_ImprovedTarjan();
                            Min = 1 - mdp.MaxProbability(VerificationOutput);
                        }
                        break;
                }
            }
            else if (SelectedEngineName == Constants.ENGINE_MDP_SIM_SEARCH)
            {
                switch (ConstraintType)
                {
                    case QueryConstraintType.PROB:
                        DRA = NegativeDRA;
                        BuildMDP(); //note this function is just used to calculate the maximal probability.
                        mdp = mdp.ComputeGCPP(VerificationOutput);
                        Min = 1 - mdp.MaxProbability(VerificationOutput);

                        DRA = PositiveDRA;
                        BuildMDP();
                        mdp = mdp.ComputeGCPP(VerificationOutput);
                        Max = mdp.MaxProbability(VerificationOutput);

                        break;
                    case QueryConstraintType.PMAX:
                        DRA = PositiveDRA;
                        BuildMDP();
                        mdp = mdp.ComputeGCPP(VerificationOutput);
                        Max = mdp.MaxProbability(VerificationOutput);
                        break;
                    case QueryConstraintType.PMIN:
                        if (AlmostFair) //note add by ssz
                        {
                            DRA = PositiveDRA;
                            BuildMDP();
                            mdp = mdp.ComputeGCPP(VerificationOutput);
                            Min = mdp.MinProbability(VerificationOutput);
                        }
                        else
                        {
                            DRA = NegativeDRA;
                            BuildMDP();
                            mdp = mdp.ComputeGCPP(VerificationOutput);
                            Min = 1 - mdp.MaxProbability(VerificationOutput);
                        }
                        break;
                }
            }




            if (Min == 1)
            {
                VerificationOutput.VerificationResult = VerificationResultType.VALID;
            }
            else if (Max == 0)
            {
                VerificationOutput.VerificationResult = VerificationResultType.INVALID;
            }
            else
            {
                VerificationOutput.VerificationResult = VerificationResultType.WITHPROBABILITY;
            }
        }

        private void RemoveNonECStates(List<string> SCC, List<string> targets)//note changed by ssz
        {
            List<string> toRemove;

            do
            {
                
                toRemove = new List<string>();

                for (int j = 0; j < SCC.Count; j++)
                {
                    string state = SCC[j];

                    List<Distribution> outgoing = mdp.States[state].Distributions;//.ForwardTransition[state];
                    if (outgoing.Count > 0)
                    {
                        bool tokeep = true;
                        for (int i = 0; i < outgoing.Count; i++)
                        {
                            tokeep = true;

                            Distribution next = outgoing[i];

                            foreach (KeyValuePair<double, MDPState> distribution in next.States)
                            {
                                if (!SCC.Contains(distribution.Value.ID))
                                {
                                    tokeep = false;
                                    break;
                                }
                            }

                            if (tokeep)
                            {
                                break;
                            }
                        }

                        if (!tokeep)
                        {
                            toRemove.Add(state);
                        }
                    }
                }


                foreach (string i in toRemove)
                {
                    SCC.Remove(i);
                    //note add by ssz 1
                    if(targets.Contains(i))
                    {
                        targets.Remove(i);
                    }
                    //note add by ssz 1
                }

                //note add by ssz 2
                if (targets.Count == 0)
                {
                    break;
                }
                //note add by ssz 2
            } while (toRemove.Count > 0 && SCC.Count > 0);
            
        }

        private void RemoveNonECStates(List<string> SCC)
        {
            List<string> toRemove;

            do
            {

                toRemove = new List<string>();

                for (int j = 0; j < SCC.Count; j++)
                {
                    string state = SCC[j];

                    List<Distribution> outgoing = mdp.States[state].Distributions;//.ForwardTransition[state];
                    if (outgoing.Count > 0)
                    {
                        bool tokeep = true;
                        for (int i = 0; i < outgoing.Count; i++)
                        {
                            tokeep = true;

                            Distribution next = outgoing[i];

                            foreach (KeyValuePair<double, MDPState> distribution in next.States)
                            {
                                if (!SCC.Contains(distribution.Value.ID))
                                {
                                    tokeep = false;
                                    break;
                                }
                            }

                            if (tokeep)
                            {
                                break;
                            }
                        }

                        if (!tokeep)
                        {
                            toRemove.Add(state);
                        }
                    }
                }


                foreach (string i in toRemove)
                {
                    SCC.Remove(i);
                }

            } while (toRemove.Count > 0 && SCC.Count > 0);

        }
        //if this scc is a bottem Target Scc, then return true; else false
        private bool BottomECStates(List<string> SCC, List<string> targets)//note changed by ssz
        {
            bool bottom = true;

            List<string> toRemove;

            do
            {

                toRemove = new List<string>();

                for (int j = 0; j < SCC.Count; j++)
                {
                    string state = SCC[j];

                    List<Distribution> outgoing = mdp.States[state].Distributions;//.ForwardTransition[state];
                    if (outgoing.Count > 0)
                    {
                        bool tokeep = true;
                        for (int i = 0; i < outgoing.Count; i++)
                        {
                            tokeep = true;
                            Distribution next = outgoing[i];

                            foreach (KeyValuePair<double, MDPState> distribution in next.States)
                            {
                                if (!SCC.Contains(distribution.Value.ID))
                                {
                                    tokeep = false;
                                    bottom = false;
                                    break;
                                }
                            }

                            if (tokeep)
                            {
                                break;
                            }
                        }

                        if (!tokeep)
                        {
                            toRemove.Add(state);
                        }
                    }
                }


                foreach (string i in toRemove)
                {
                    SCC.Remove(i);
                    //note add by ssz 1
                    if (targets.Contains(i))
                    {
                        targets.Remove(i);
                    }
                    //note add by ssz 1
                }

                //note add by ssz 2
                if (targets.Count == 0)
                {
                    bottom = false;
                }
                //note add by ssz 2
            } while (toRemove.Count > 0 && SCC.Count > 0);

            return bottom;

        }
       
        /// <summary>
        /// This method builds a MDP, while identifying SCCs.
        /// </summary>
        protected void BuildMDP_OldTarjan()
        {
            MDPState dummyInit = new MDPState(DUMMY_INIT);
            mdp = new MDP(dummyInit, Ultility.Ultility.DEFAULT_PRECISION, Ultility.Ultility.MAX_DIFFERENCE);

            List<PCSPEventDRAPair> initials = GetInitialPairs(InitialStep as MDPConfiguration);
            Stack<KeyValuePair<PCSPEventDRAPair, MDPState>> working = new Stack<KeyValuePair<PCSPEventDRAPair, MDPState>>(1024);
            Stack<PCSPEventDRAPair> stepStack = new Stack<PCSPEventDRAPair>(1024);
            MDPState2DRAStateMapping = new Dictionary<string, int>(Common.Classes.Ultility.Ultility.MC_INITIAL_SIZE);
            
            //build the MDP while identifying the SCCs
            List<List<string>> SCCs = new List<List<string>>(64);

            for (int z = 1; z <= initials.Count; z++)
            {
                PCSPEventDRAPair initState = initials[z - 1];
                string stringID = initState.GetCompressedState();
                MDPState newinit = new MDPState(stringID);
                mdp.AddState(newinit);
                dummyInit.AddDistribution(new Distribution(stringID, newinit));

                //newinit.AddDistribution(new Distribution(Constants.TAU, newinit));
                working.Push(new KeyValuePair<PCSPEventDRAPair, MDPState>(initState, newinit));
            }

            Dictionary<string, int> preorder = new Dictionary<string, int>(Common.Classes.Ultility.Ultility.MC_INITIAL_SIZE);
            Dictionary<string, int> lowlink = new Dictionary<string, int>(Common.Classes.Ultility.Ultility.MC_INITIAL_SIZE);

            int preorderCounter = 0;
            Dictionary<string, List<PCSPEventDRAPair>> ExpendedNode = new Dictionary<string, List<PCSPEventDRAPair>>();

            do
            {
                if (CancelRequested)
                {
                    this.VerificationOutput.NoOfStates = this.VerificationOutput.NoOfStates + mdp.States.Count;
                    return;
                }

                KeyValuePair<PCSPEventDRAPair, MDPState> pair = working.Peek();
                MDPConfiguration evt = pair.Key.configuration;
                int DRAState = pair.Key.state;
                string currentID = pair.Key.GetCompressedState();

                List<Distribution> outgoing = pair.Value.Distributions;

                if (!preorder.ContainsKey(currentID))
                {
                    preorder.Add(currentID, preorderCounter);
                    preorderCounter++;
                }

                bool done = true;

                if (ExpendedNode.ContainsKey(currentID))
                {
                    List<PCSPEventDRAPair> list = ExpendedNode[currentID];
                    if (list.Count > 0)
                    {
                        for (int k = list.Count - 1; k >= 0; k--)
                        {
                            PCSPEventDRAPair step = list[k];

                            string stepID = step.GetCompressedState();
                            if (!preorder.ContainsKey(stepID))
                            {
                                if (done)
                                {
                                    working.Push(new KeyValuePair<PCSPEventDRAPair, MDPState>(step, mdp.States[stepID]));
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
                    int currentDistriIndex = -1;
                    Distribution newDis = new Distribution(Constants.TAU);

                    MDPConfiguration[] steps = evt.MakeOneMoveLocal().ToArray();

                    //NOTE: here we play a trick for deadlock case: if a deadlock exist in the MDP, we will make a
                    //self loop transition to remove the deadlock. Deadlock is meaningless in MDP.
                    if(evt.IsDeadLock)
                    {
                        List<MDPConfiguration> stepsList = new List<MDPConfiguration>(steps);

                        stepsList.Add(CreateSelfLoopStep(evt));
                        steps = stepsList.ToArray();
                        HasDeadLock = true;
                    }

                    List<PCSPEventDRAPair> product = Next(steps, DRAState);
                    this.VerificationOutput.Transitions += product.Count;

                    for (int k = product.Count - 1; k >= 0; k--)
                    {
                        PCSPEventDRAPair step = product[k];
                        string tmp = step.GetCompressedState();
                        //int nextIndex = VisitedWithID.Count;
                        MDPState nextState;

                        if (mdp.States.TryGetValue(tmp, out nextState))
                        {

                            if (!preorder.ContainsKey(tmp))
                            {
                                if (done)
                                {
                                    working.Push(new KeyValuePair<PCSPEventDRAPair, MDPState>(step, nextState));
                                    done = false;
                                    product.RemoveAt(k);
                                }
                                else
                                {
                                    product[k] = step;
                                }
                            }
                            else
                            {
                                product.RemoveAt(k);
                            }
                        }
                        else
                        {
                            nextState = new MDPState(tmp);
                            mdp.States.Add(tmp, nextState);

                            if (done)
                            {
                                working.Push(new KeyValuePair<PCSPEventDRAPair, MDPState>(step, nextState));
                                done = false;
                                product.RemoveAt(k);
                            }
                            else
                            {
                                product[k] = step;
                            }
                        }

                        MDPConfiguration pstep = step.configuration;

                        if (pstep.DisIndex == -1)
                        {
                            if (currentDistriIndex >= 0)
                            {
                                pair.Value.AddDistribution(newDis);
                                newDis = new Distribution(Constants.TAU);
                            }

                            
                            //note here changed by ssz
                            //int draState = MDPState2DRAStateMapping[pair.Value.ID];
                            if(nextState == pair.Value)
                            {
                                for (int index = 0; index < DRA.acceptance().size(); index++)
                                {
                                    if (DRA.acceptance().isStateInAcceptance_L(index, DRAState))
                                    {
                                        mdp.AddTargetStates(pair.Value);
                                    }
                                }
                            }
                            else
                            {
                                Distribution newTrivialDis = new Distribution(pstep.Event, nextState);
                                pair.Value.AddDistribution(newTrivialDis);
                            }
                                
                            
                            //note here changed by ssz
                        }
                        else if (currentDistriIndex != -1 && pstep.DisIndex != currentDistriIndex)
                        {
                            pair.Value.AddDistribution(newDis);
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
                        pair.Value.AddDistribution(newDis);
                    }

                    ExpendedNode.Add(currentID, product);
                }

                if (done)
                {
                    int lowlinkV = preorder[currentID];
                    int preorderV = preorder[currentID];

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

                            if (!MDPState2DRAStateMapping.ContainsKey(w))
                            {
                                if (preorder[w] > preorderV)
                                {
                                    lowlinkV = Math.Min(lowlinkV, lowlink[w]);
                                }
                                else
                                {
                                    lowlinkV = Math.Min(lowlinkV, preorder[w]);
                                }
                            }
                        }
                    }

                    lowlink[currentID] = lowlinkV;
                    working.Pop();

                    if (lowlinkV == preorderV)
                    {
                        List<string> SCC = new List<string>(1024);
                        SCC.Add(currentID);
                        MDPState2DRAStateMapping.Add(currentID, DRAState);
                        while (stepStack.Count > 0 && preorder[stepStack.Peek().GetCompressedState()] > preorderV)
                        {
                            PCSPEventDRAPair s = stepStack.Pop();
                            string sID = s.GetCompressedState(); 
                            SCC.Add(sID);
                            MDPState2DRAStateMapping.Add(sID, s.state);
                        }

                        if (SCC.Count > 1 || selfLoop) //evt.IsDeadLock || 
                        {                           
                            SCCs.Add(SCC);
                        }
                    }
                    else
                    {
                        stepStack.Push(pair.Key);
                    }
                }
            } while (working.Count > 0);

            List<string> EndComponents = new List<string>(SCCs.Count);
            int count = DRA.acceptance().size();
            int helper = 0;

            foreach (List<string> scc in SCCs)
            {
                //for debug
                //List<MDPState> debug = new List<MDPState>();
                //List<int> drastates = new List<int>();
                //foreach(string state in scc)
                //{
                //    debug.Add(mdp.States[state]);
                //    drastates.Add(MDPState2DRAStateMapping[state]);
                //}
                //for debug
                for (int index = 0; index < count; index++)
                {
                    List<string> newSCC = new List<string>();
                    List<string> targets = new List<string>();

                    if (AlmostFair)
                    {
                        bool bottom = true;
                        //int SCCcount = scc.Count;
                        //note that as long as one SCC(might not be a real MEC) has a U state, the whole SCC cannot be targets.
                        //RemoveNonECStates(scc, targets);
                        foreach (string i in scc)
                        {
                            int draState = MDPState2DRAStateMapping[i];
                            if (bottom)
                            {
                                if (DRA.acceptance().isStateInAcceptance_U(index, draState))
                                {
                                    bottom = false;
                                }
                            }

                            newSCC.Add(i);
                            
                            if (DRA.acceptance().isStateInAcceptance_L(index, draState))
                            {
                                targets.Add(i);
                            }
                            
                        }
                        if(bottom)
                        {
                            if (!BottomECStates(newSCC, targets))
                            {
                                if (newSCC.Count > 0)
                                {
                                    GroupMEC(newSCC);
                                }
                            }
                            else
                            {
                                Common.Classes.Ultility.Ultility.AddList(EndComponents, scc);
                            }
                        }
                        else 
                        {
                            RemoveNonECStates(newSCC);
                            if(newSCC.Count>0)
                            {
                                GroupMEC(newSCC);
                            }
                        }
                    }
                    else
                    {
                        foreach (string i in scc)
                        {
                            int draState = MDPState2DRAStateMapping[i];
                            if (!DRA.acceptance().isStateInAcceptance_U(index, draState))
                            {
                                newSCC.Add(i);
                                //note add by ssz
                                if (DRA.acceptance().isStateInAcceptance_L(index, draState))
                                {
                                    targets.Add(i);
                                }
                                //note add by ssz
                            }

                        }
                        //if (AlmostFair)
                        //{
                        //    RemoveNonECStates(newSCC, targets);
                        //    if (newSCC.Count > 0)
                        //    {
                        //        GroupMEC(newSCC);
                        //    }
                        //    if (newSCC.Count > 0)
                        //    {

                        //    }
                        //}
                        //else
                        //{
                        if (targets.Count > 0)
                        {
                            RemoveNonECStates(newSCC, targets);
                        }
                        else
                        {
                            continue;
                        }

                        if (targets.Count > 0)
                        {
                            //List<string> endComponent = TarjanModelChecking(index, newSCC);
                            Debug.WriteLine(helper++);
                            //Common.Classes.Ultility.Ultility.AddList(EndComponents, endComponent);
                            Common.Classes.Ultility.Ultility.AddList(EndComponents, newSCC);
                        }
                        //}

                    }

                }
            }

            foreach (string s in EndComponents)
            {
                mdp.AddTargetStates(mdp.States[s]);
            }
            
            VerificationOutput.NoOfStates = VerificationOutput.NoOfStates + mdp.States.Count;
            //mdp.BackUpTargetStates();
        }

        /// <summary>
        /// This method builds a MDP, while identifying SCCs.
        /// </summary>
        protected void BuildMDP()
        {
            MDPState dummyInit = new MDPState(DUMMY_INIT);
            mdp = new MDP(dummyInit, Ultility.Ultility.DEFAULT_PRECISION, Ultility.Ultility.MAX_DIFFERENCE);

            List<PCSPEventDRAPair> initials = GetInitialPairs(InitialStep as MDPConfiguration);
            Stack<KeyValuePair<PCSPEventDRAPair, MDPState>> working = new Stack<KeyValuePair<PCSPEventDRAPair, MDPState>>(1024);
            Stack<PCSPEventDRAPair> stepStack = new Stack<PCSPEventDRAPair>(1024);
            //MDPState2DRAStateMapping = new Dictionary<string, int>(Common.Classes.Ultility.Ultility.MC_INITIAL_SIZE);

            //DFS data, which mapping each state to an int[] of size 3, first is the pre-order, second is the lowlink, last one is DRAState
            Dictionary<string, int[]> DFSData = new Dictionary<string, int[]>(Ultility.Ultility.MC_INITIAL_SIZE);

            //build the MDP while identifying the SCCs
            List<List<string>> SCCs = new List<List<string>>(64);

            for (int z = 1; z <= initials.Count; z++)
            {
                PCSPEventDRAPair initState = initials[z - 1];
                string stringID = initState.GetCompressedState();
                MDPState newinit = new MDPState(stringID);
                mdp.AddState(newinit);
                dummyInit.AddDistribution(new Distribution(stringID, newinit));

                DFSData.Add(stringID, new int[] { VISITED_NOPREORDER, 0, 0 });

                //newinit.AddDistribution(new Distribution(Constants.TAU, newinit));
                working.Push(new KeyValuePair<PCSPEventDRAPair, MDPState>(initState, newinit));
            }

   
            //Dictionary<string, int> preorder = new Dictionary<string, int>(Common.Classes.Ultility.Ultility.MC_INITIAL_SIZE);
            //Dictionary<string, int> lowlink = new Dictionary<string, int>(Common.Classes.Ultility.Ultility.MC_INITIAL_SIZE);

            int preordercounter = 0;
            Dictionary<string, List<PCSPEventDRAPair>> ExpendedNode = new Dictionary<string, List<PCSPEventDRAPair>>();

            do
            {
                if (CancelRequested)
                {
                    this.VerificationOutput.NoOfStates = this.VerificationOutput.NoOfStates + mdp.States.Count;
                    return;
                }

                KeyValuePair<PCSPEventDRAPair, MDPState> pair = working.Peek();
                MDPConfiguration evt = pair.Key.configuration;
                int DRAState = pair.Key.state;
                string v = pair.Key.GetCompressedState();

                List<Distribution> outgoing = pair.Value.Distributions;

                //if (!preorder.ContainsKey(currentID))
                //{
                //    preorder.Add(currentID, preorderCounter);
                //    preorderCounter++;
                //}

                int[] nodeData = DFSData[v];

                if (nodeData[0] == VISITED_NOPREORDER)
                {
                    nodeData[0] = preordercounter;
                    preordercounter++;
                }

                bool done = true;

                if (ExpendedNode.ContainsKey(v))
                {
                    List<PCSPEventDRAPair> list = ExpendedNode[v];
                    if (list.Count > 0)
                    {
                        for (int k = list.Count - 1; k >= 0; k--)
                        {
                            PCSPEventDRAPair step = list[k];

                            string stepID = step.GetCompressedState();
                            //if (!preorder.ContainsKey(stepID))
                            if (DFSData[stepID][0] == VISITED_NOPREORDER)
                            {
                                if (done)
                                {
                                    working.Push(new KeyValuePair<PCSPEventDRAPair, MDPState>(step, mdp.States[stepID]));
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
                    int currentDistriIndex = -1;
                    Distribution newDis = new Distribution(Constants.TAU);

                    MDPConfiguration[] steps = evt.MakeOneMoveLocal().ToArray();

                    //NOTE: here we play a trick for deadlock case: if a deadlock exist in the MDP, we will make a
                    //self loop transition to remove the deadlock. Deadlock is meaningless in MDP.
                    if (evt.IsDeadLock)
                    {
                        List<MDPConfiguration> stepsList = new List<MDPConfiguration>(steps);

                        stepsList.Add(CreateSelfLoopStep(evt));
                        steps = stepsList.ToArray();
                        HasDeadLock = true;
                    }

                    List<PCSPEventDRAPair> product = Next(steps, DRAState);
                    this.VerificationOutput.Transitions += product.Count;

                    for (int k = product.Count - 1; k >= 0; k--)
                    {
                        PCSPEventDRAPair step = product[k];
                        string tmp = step.GetCompressedState();
                        //int nextIndex = VisitedWithID.Count;
                        MDPState nextState;

                        int[] data;
                        if (DFSData.TryGetValue(tmp, out data))
                        {
                            mdp.States.TryGetValue(tmp, out nextState);
                            if (data[0] == VISITED_NOPREORDER)
                            {
                        //if (mdp.States.TryGetValue(tmp, out nextState))
                        //{

                        //    if (!preorder.ContainsKey(tmp))
                        //    {
                                if (done)
                                {
                                    working.Push(new KeyValuePair<PCSPEventDRAPair, MDPState>(step, nextState));
                                    done = false;
                                    product.RemoveAt(k);
                                }
                                else
                                {
                                    product[k] = step;
                                }
                            }
                            else
                            {
                                product.RemoveAt(k);
                            }
                        }
                        else
                        {
                            DFSData.Add(tmp, new int[] { VISITED_NOPREORDER, 0, 0 });
                            
                            nextState = new MDPState(tmp);
                            mdp.States.Add(tmp, nextState);

                            if (done)
                            {
                                working.Push(new KeyValuePair<PCSPEventDRAPair, MDPState>(step, nextState));
                                done = false;
                                product.RemoveAt(k);
                            }
                            else
                            {
                                product[k] = step;
                            }
                        }

                        MDPConfiguration pstep = step.configuration;

                        if (pstep.DisIndex == -1)
                        {
                            if (currentDistriIndex >= 0)
                            {
                                pair.Value.AddDistribution(newDis);
                                newDis = new Distribution(Constants.TAU);
                            }


                            //note here changed by ssz
                            //int draState = MDPState2DRAStateMapping[pair.Value.ID];
                            if (nextState == pair.Value)
                            {
                                for (int index = 0; index < DRA.acceptance().size(); index++)
                                {
                                    if (DRA.acceptance().isStateInAcceptance_L(index, DRAState))
                                    {
                                        mdp.AddTargetStates(pair.Value);
                                    }
                                }
                            }
                            else
                            {
                                Distribution newTrivialDis = new Distribution(pstep.Event, nextState);
                                pair.Value.AddDistribution(newTrivialDis);
                            }


                            //note here changed by ssz
                        }
                        else if (currentDistriIndex != -1 && pstep.DisIndex != currentDistriIndex)
                        {
                            pair.Value.AddDistribution(newDis);
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
                        pair.Value.AddDistribution(newDis);
                    }

                    ExpendedNode.Add(v, product);
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

                            if (w == v)
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
                        List<string> SCC = new List<string>(1024);
                        SCC.Add(v);

                        nodeData[0] = SCC_FOUND;

                        //MDPState2DRAStateMapping.Add(v, DRAState);
                        nodeData[2] = DRAState;
                        //while (stepStack.Count > 0 && preorder[stepStack.Peek().GetCompressedState()] > preorderV)
                        while (stepStack.Count > 0 && DFSData[stepStack.Peek().GetCompressedState()][0] > preorderV)
                        {
                            PCSPEventDRAPair s = stepStack.Pop();
                            string sID = s.GetCompressedState();
                            SCC.Add(sID);
                            //MDPState2DRAStateMapping.Add(sID, s.state);

                            int[] wdata = DFSData[sID];
                            wdata[0] = SCC_FOUND;
                            wdata[2] = s.state;
                        }

                        if (SCC.Count > 1 || selfLoop) //evt.IsDeadLock || 
                        {
                            SCCs.Add(SCC);
                        }
                    }
                    else
                    {
                        stepStack.Push(pair.Key);
                    }
                }
            } while (working.Count > 0);

            List<string> EndComponents = new List<string>(SCCs.Count);
            int count = DRA.acceptance().size();
            int helper = 0;

            foreach (List<string> scc in SCCs)
            {
                //for debug
                //List<MDPState> debug = new List<MDPState>();
                //List<int> drastates = new List<int>();
                //foreach(string state in scc)
                //{
                //    debug.Add(mdp.States[state]);
                //    drastates.Add(MDPState2DRAStateMapping[state]);
                //}
                //for debug
                for (int index = 0; index < count; index++)
                {
                    List<string> newSCC = new List<string>();
                    List<string> targets = new List<string>();

                    if (AlmostFair)
                    {
                        bool bottom = true;
                        //int SCCcount = scc.Count;
                        //note that as long as one SCC(might not be a real MEC) has a U state, the whole SCC cannot be targets.
                        //RemoveNonECStates(scc, targets);
                        foreach (string i in scc)
                        {
                            int draState = DFSData[i][2]; // MDPState2DRAStateMapping[i];
                            if (bottom)
                            {
                                if (DRA.acceptance().isStateInAcceptance_U(index, draState))
                                {
                                    bottom = false;
                                }
                            }

                            newSCC.Add(i);

                            if (DRA.acceptance().isStateInAcceptance_L(index, draState))
                            {
                                targets.Add(i);
                            }

                        }
                        if (bottom)
                        {
                            if (!BottomECStates(newSCC, targets))
                            {
                                if (newSCC.Count > 0)
                                {
                                    GroupMEC(newSCC);
                                }
                            }
                            else
                            {
                                Common.Classes.Ultility.Ultility.AddList(EndComponents, scc);
                            }
                        }
                        else
                        {
                            RemoveNonECStates(newSCC);
                            if (newSCC.Count > 0)
                            {
                                GroupMEC(newSCC);
                            }
                        }
                    }
                    else
                    {
                        foreach (string i in scc)
                        {
                            int draState = DFSData[i][2]; //MDPState2DRAStateMapping[i];
                            if (!DRA.acceptance().isStateInAcceptance_U(index, draState))
                            {
                                newSCC.Add(i);
                                //note add by ssz
                                if (DRA.acceptance().isStateInAcceptance_L(index, draState))
                                {
                                    targets.Add(i);
                                }
                                //note add by ssz
                            }

                        }
                        //if (AlmostFair)
                        //{
                        //    RemoveNonECStates(newSCC, targets);
                        //    if (newSCC.Count > 0)
                        //    {
                        //        GroupMEC(newSCC);
                        //    }
                        //    if (newSCC.Count > 0)
                        //    {

                        //    }
                        //}
                        //else
                        //{
                        if (targets.Count > 0)
                        {
                            RemoveNonECStates(newSCC, targets);
                        }
                        else
                        {
                            continue;
                        }

                        if (targets.Count > 0)
                        {
                            //List<string> endComponent = TarjanModelChecking(index, newSCC);
                            Debug.WriteLine(helper++);
                            //Common.Classes.Ultility.Ultility.AddList(EndComponents, endComponent);
                            Common.Classes.Ultility.Ultility.AddList(EndComponents, newSCC);
                        }
                        //}

                    }

                }
            }

            foreach (string s in EndComponents)
            {
                mdp.AddTargetStates(mdp.States[s]);
            }

            VerificationOutput.NoOfStates = VerificationOutput.NoOfStates + mdp.States.Count;
            //mdp.BackUpTargetStates();
        }

        protected abstract MDPConfiguration CreateSelfLoopStep(MDPConfiguration evt);
        //{
            //new MDPConfiguration(evt.Process, Constants.TAU, null, evt.GlobalEnv, false, 1, -1)
        //}

        //this method is used to group MEC together.
        //private void GroupMEC(List<string> newSCC)
        //{
        //    //note add MEC grouping here by ssz
        //    //if (endComponent.Count == 0)
        //    //{
        //    for (int i = 0; i < newSCC.Count; i++)
        //    {
        //        for (int j = mdp.States[newSCC[i]].Distributions.Count - 1; j >= 0; j--)
        //        {
        //            Distribution distr = mdp.States[newSCC[i]].Distributions[j];

        //            bool keepDistr = false;
        //            bool remove = false;
        //            foreach (KeyValuePair<double, MDPState> pair in distr.States)
        //            {
        //                if (!newSCC.Contains(pair.Value.ID))
        //                {
        //                    keepDistr = true;
        //                }
        //                else
        //                {
        //                    remove = true;
        //                    //distr.States.Remove(pair);
        //                }

        //            }
        //            //use [0] as a representative state
        //            if (i == 0)
        //            {
        //                if (keepDistr)
        //                {
        //                    if (remove)
        //                    {
        //                        mdp.States[newSCC[i]].Distributions.RemoveAt(j);

        //                        Distribution newDistr = mdp.DistrbutionReCalculate(newSCC,
        //                                                                           mdp.States[newSCC[i]],
        //                                                                           distr);
        //                        mdp.States[newSCC[i]].AddDistribution(newDistr);
        //                    }
        //                }
        //                else
        //                {
        //                    mdp.States[newSCC[i]].Distributions.RemoveAt(j);
        //                }
        //            }
        //            else
        //            {
        //                if (keepDistr)
        //                {
        //                    if (remove)
        //                    {

        //                        Distribution newDistr = mdp.DistrbutionReCalculate(newSCC,
        //                                                                           mdp.States[newSCC[i]],
        //                                                                           distr);
        //                        mdp.States[newSCC[0]].AddDistribution(newDistr);
        //                    }
        //                    else
        //                    {
        //                        mdp.States[newSCC[0]].AddDistribution(distr);
        //                    }
        //                }


        //            }

        //        }

        //        if (i != 0)
        //        {

        //            mdp.States[newSCC[i]].Distributions.Clear();
        //            Distribution NewDistr = new Distribution(Constants.TAU);
        //            NewDistr.AddProbStatePair(1, mdp.States[newSCC[0]]);
        //            mdp.States[newSCC[i]].AddDistribution(NewDistr);
        //        }


        //    }

        //    //}
        //    //note add MEC grouping here
        //}
        //private List<string> TarjanModelChecking(int index, List<string> SCC)
        //{
        //    //for identifying the SCCs
        //    List<string> StronglyConnectedComponets = new List<string>(64);
        //    Dictionary<string, int> preorder = new Dictionary<string, int>(64);
        //    Dictionary<string, int> lowlink = new Dictionary<string, int>(64);
        //    Stack<string> scc_stack = new Stack<string>(64);

        //    HashSet<string> scc_found = new HashSet<string>();

        //    List<string> SCCSets = new List<string>(1024);
        //    int i = 0; //# Preorder counter 

        //    Stack<string> idStack = new Stack<string>(64);
        //    idStack.Push(SCC[0]);

        //    do
        //    {
        //        string v = idStack.Peek();

        //        List<Distribution> outgoing = mdp.States[v].Distributions;//.ForwardTransition[v];

        //        if (!preorder.ContainsKey(v))
        //        {
        //            preorder.Add(v, i);
        //            i++;
        //        }
        //        bool done = true;
        //        for (int j = 0; j < outgoing.Count; j++)
        //        {
        //            foreach (KeyValuePair<double, MDPState> list in outgoing[j].States)
        //            {
        //                string w = list.Value.ID;

        //                if (SCC.Contains(w) && !preorder.ContainsKey(w))
        //                {
        //                    idStack.Push(w);
        //                    done = false;
        //                    break;
        //                }
        //            }
        //        }

        //        if (done)
        //        {
        //            if (v == DUMMY_INIT)
        //            {
        //                idStack.Pop();
        //            }
        //            else
        //            {
        //                int lowlinkV = preorder[v];
        //                int preorderV = preorder[v];

        //                //lowlink[v] = preorder[v];
        //                bool selfLoop = false;

        //                for (int j = 0; j < outgoing.Count; j++)
        //                {
        //                    foreach (KeyValuePair<double, MDPState> list in outgoing[j].States)
        //                    {
        //                        string w = list.Value.ID;

        //                        if (SCC.Contains(w))
        //                        {
        //                            if (w == v)
        //                            {
        //                                selfLoop = true;
        //                            }

        //                            if (!scc_found.Contains(w.ToString()))
        //                            {
        //                                if (preorder[w] > preorderV)
        //                                {
        //                                    lowlinkV = Math.Min(lowlinkV, lowlink[w]);
        //                                }
        //                                else
        //                                {
        //                                    lowlinkV = Math.Min(lowlinkV, preorder[w]);
        //                                }
        //                            }
        //                        }
        //                    }
        //                }
        //                lowlink[v] = lowlinkV;
        //                idStack.Pop();

        //                if (lowlinkV == preorderV)
        //                {
        //                    scc_found.Add(v.ToString());
        //                    StronglyConnectedComponets.Add(v);

        //                    bool LFair = false;
        //                    if (DRA.acceptance().isStateInAcceptance_L(index, MDPState2DRAStateMapping[v]))
        //                    {
        //                        LFair = true;
        //                    }

        //                    while (scc_stack.Count > 0 && preorder[scc_stack.Peek()] > preorderV)
        //                    {
        //                        string k = scc_stack.Pop();//.Dequeue();
        //                        StronglyConnectedComponets.Add(k);
        //                        scc_found.Add(k);

        //                        if (!LFair && DRA.acceptance().isStateInAcceptance_L(index, MDPState2DRAStateMapping[k]))
        //                        {
        //                            LFair = true;
        //                        }
        //                    }

        //                    if (LFair && (outgoing.Count == 0 || StronglyConnectedComponets.Count > 1 || selfLoop))
        //                    {
        //                        SCCSets.AddRange(StronglyConnectedComponets);
        //                    }

        //                    StronglyConnectedComponets.Clear();
        //                }
        //                else
        //                {
        //                    scc_stack.Push(v);
        //                }
        //            }
        //        }

        //        //because the SCC can be brekon by removing bad states, 
        //        //if there is such case, the SCC are forests. so we have to check all components
        //        if (idStack.Count == 0 && scc_found.Count != SCC.Count)
        //        {
        //            string next = MDP.GetNextUnvisitedState(scc_found, SCC);

        //            if (next != null)
        //            {
        //                idStack.Push(next);
        //            }
        //        }

        //    } while (idStack.Count > 0);

        //    return SCCSets;
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
                if(this.Max != -1 && Min != -1)
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
            }
            sb.AppendLine();

            if (HasDeadLock)
            {
                sb.AppendLine("WARNING: The system has deadlock states. Self-loop transitions have been added to remove the deadlock!");
                sb.AppendLine();
            }

            sb.AppendLine("********Verification Setting********");
            sb.AppendLine("Admissible Behavior: " + SelectedBahaviorName);
            if(!IsNegateLiveness || IsSafety)
            {
                sb.AppendLine("Search Engine: Reachability Analysis; Graph-based Probabity Computation.");                
            }
            else //if (AssertionType == AssertionType.LTLSafety)
            {
                sb.AppendLine("Search Engine: End-Components Identification; Graph-based Probabity Computation.");
            }
            sb.AppendLine("System Abstraction: " + MustAbstract);

            sb.AppendLine("Maximum difference threshold : " + Ultility.Ultility.MAX_DIFFERENCE);
            
            sb.AppendLine();

            return sb.ToString();
        }

        //public override string StartingProcess
        //{
        //    get
        //    {
        //        return Process.ToString();
        //    }
        //}

        /// <summary>
        /// Given one environment, get the initial states of the product of the system and the automata. Notice that the automata 
        /// is allowed to make one move first. This is necessary to check the very first state of the system. 
        /// </summary>
        /// <param name="initialStep"></param>
        /// <returns></returns>
        private List<PCSPEventDRAPair> GetInitialPairs(MDPConfiguration initialStep)
        {
            List<PCSPEventDRAPair> toReturn = new List<PCSPEventDRAPair>();
            List<int> existed = new List<int>();

            int sIndex = DRA.getStartState().Index;
            List<int> next = DRA.MakeOneMove(sIndex, initialStep.GlobalEnv, initialStep.Event);

            foreach (int var in next)
            {
                if (!existed.Contains(var))
                {
                    existed.Add(var);
                    toReturn.Add(new PCSPEventDRAPair(initialStep, var));
                }
            }

            return toReturn;
        }

        private List<PCSPEventDRAPair> Next(MDPConfiguration[] steps, int BAState)
        {
            List<PCSPEventDRAPair> product = new List<PCSPEventDRAPair>(steps.Length * BA.States.Length);

            for (int i = 0; i < steps.Length; i++)
            {
                MDPConfiguration step = steps[i];
                List<int> states = DRA.MakeOneMove(BAState, step.GlobalEnv, step.Event);

                for (int j = 0; j < states.Count; j++)
                {
                    product.Add(new PCSPEventDRAPair(step, states[j]));
                }
            }

            return product;
        }

        ////todo: is this method correct for refinement assersions?
        //protected bool CheckIsProcessLevelFairnessApplicable()
        //{
        //    PCSPProcess nextProcess = Process.GetTopLevelConcurrency(new List<string>());
        //    if (MustAbstract)
        //    {
        //        if (nextProcess is IndexInterleaveAbstract)
        //        {
        //            IndexInterleaveAbstract interleave = nextProcess as IndexInterleaveAbstract;
        //            foreach (PCSPProcess p in interleave.Processes)
        //            {
        //                if (p.MustBeAbstracted())
        //                {
        //                    return false;
        //                }
        //            }

        //            return true;
        //        }
        //    }
        //    else
        //    {
        //        if (nextProcess is IndexInterleave || nextProcess is IndexParallel || nextProcess is IndexInterleaveAbstract)
        //        {
        //            return true;
        //        }
        //    }

        //    return false;
        //}
    }

    public sealed class PCSPEventDRAPair
    {
        public MDPConfiguration configuration;
        public int state;
        //public int ID;

        public PCSPEventDRAPair(MDPConfiguration e, int s)
        {
            configuration = e;
            state = s;
            //ID = -1;
        }

        public string GetCompressedState()
        {
            return configuration.GetIDWithEvent() + Constants.SEPARATOR + state;
        }
    }
}