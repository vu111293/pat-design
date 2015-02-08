using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using Antlr.Runtime;
using PAT.Common.Classes.Assertion;
using PAT.Common.Classes.BA;
using PAT.Common.Classes.LTL2DRA;
using PAT.Common.Classes.LTS;
using PAT.Common.Classes.ModuleInterface;
using PAT.Common.Classes.Ultility;


namespace PAT.Common.Classes.SemanticModels.MDP.Assertion
{
    public abstract partial class AssertionLTL 
    {
        //protected QueryConstraintType ConstraintType;
        //protected DRA DRA;
        //public DRA PositiveDRA;
        //public DRA NegativeDRA;
        //public BuchiAutomata PositiveBA;
        //protected double Min = -1;
        //protected double Max = -1;
        //protected MDP mdp;
        //protected Dictionary<string, int> MDPState2DRAStateMapping;
        //private bool HasDeadLock;
        ////private DefinitionRef Process;
        //public const string DUMMY_INIT = "dummy";

        //private bool AlmostFair = false;
  




        protected void BuildMD_ImprovedTarjan()
        {
            //int counter = 0;
            MDPState dummyInit = new MDPState(DUMMY_INIT);
            mdp = new MDP(dummyInit, Ultility.Ultility.DEFAULT_PRECISION, Ultility.Ultility.MAX_DIFFERENCE);


            List<PCSPEventDRAPairMEC> initials = GetInitialPairsEMC(InitialStep as MDPConfiguration);
            Stack<KeyValuePair<PCSPEventDRAPairMEC, MDPState>> working = new Stack<KeyValuePair<PCSPEventDRAPairMEC, MDPState>>(1024);
            //Stack<PCSPEventDRAPairMEC> stepStack = new Stack<PCSPEventDRAPairMEC>(1024);
            List<PCSPEventDRAPairMEC> stepStack = new List<PCSPEventDRAPairMEC>(1024);
            
            MDPState2DRAStateMapping = new Dictionary<string, int>(Common.Classes.Ultility.Ultility.MC_INITIAL_SIZE);
            //HashSet<string> MDPStateMapping = new HashSet<string>();
            //build the MDP while identifying the SCCs
            List<List<string>> SCCs = new List<List<string>>(64);

            for (int z = 1; z <= initials.Count; z++)
            {
                PCSPEventDRAPairMEC initState = initials[z - 1];
                string stringID = initState.ID;
                MDPState newinit = new MDPState(stringID);
                mdp.AddState(newinit);
                dummyInit.AddDistribution(new Distribution(stringID, newinit));
                stepStack.Add(initState);
                //newinit.AddDistribution(new Distribution(Constants.TAU, newinit));
                working.Push(new KeyValuePair<PCSPEventDRAPairMEC, MDPState>(initState, newinit));
            }

            Dictionary<string, int> preorder = new Dictionary<string, int>(Common.Classes.Ultility.Ultility.MC_INITIAL_SIZE);
            Dictionary<string, int> lowlink = new Dictionary<string, int>(Common.Classes.Ultility.Ultility.MC_INITIAL_SIZE);
            Dictionary<string, int> distrRecord = new Dictionary<string, int>();

            int preorderCounter = 0;
            Dictionary<string, List<PCSPEventDRAPairMEC>> ExpendedNode = new Dictionary<string, List<PCSPEventDRAPairMEC>>();

            do
            {
                if (CancelRequested)
                {
                    this.VerificationOutput.NoOfStates = this.VerificationOutput.NoOfStates + mdp.States.Count;
                    return;
                }

                KeyValuePair<PCSPEventDRAPairMEC, MDPState> pair = working.Peek();
                MDPConfiguration evt = pair.Key.configuration;
                int DRAState = pair.Key.state;
                string currentID = pair.Key.ID;
                List<Distribution> outgoing = pair.Value.Distributions;

                if (!preorder.ContainsKey(currentID))
                {
                    preorder.Add(currentID, preorderCounter);
                    distrRecord.Add(currentID, 0);
                    //stepStack.Add(new PCSPEventDRAPairMEC(currentID + "separate" + 0));
                    preorderCounter++;
                }

                bool done = true;

                if (ExpendedNode.ContainsKey(currentID))
                {
                    List<PCSPEventDRAPairMEC> list = ExpendedNode[currentID];
                    //int counter = 0;
                    if (list.Count > 0)
                    {
                        int k = list.Count - 1;
                        //for (int k = list.Count - 1; k >= 0; k--)

                        PCSPEventDRAPairMEC step = list[k];
                        if (step == null)
                        {
                            stepStack.Add(new PCSPEventDRAPairMEC(currentID + "separate" + (distrRecord[currentID]++)));
                            list.RemoveAt(k);
                            continue;
                        }
                        else
                        {
                            string stepID = step.ID;
                            if (!preorder.ContainsKey(stepID))
                            {
                                //if (done)
                                //{
                                    working.Push(new KeyValuePair<PCSPEventDRAPairMEC, MDPState>(step, mdp.States[stepID]));
                                    stepStack.Add(step);
                                    done = false;
                                    list.RemoveAt(k);
                                //}
                            }
                            else
                            {
                                //stepStack.Add(step.ID);
                                stepStack.Add(step);
                                list.RemoveAt(k);
                                done = false;
                            }
                        }


                    }
                    //else
                    //{
                    //    stepStack.Add(new PCSPEventDRAPairMEC(currentID + "end"));
                    //}
                }
                else
                {
                    int currentDistriIndex = -1;
                    Distribution newDis = new Distribution(Constants.TAU);

                    IEnumerable<MDPConfiguration> steps = evt.MakeOneMoveLocal();
                    //int counter = 0;
                    //NOTE: here we play a trick for deadlock case: if a deadlock exist in the MDP, we will make a
                    //self loop transition to remove the deadlock. Deadlock is meaningless in MDP.
                    if (evt.IsDeadLock)
                    {
                        List<MDPConfiguration> stepsList = new List<MDPConfiguration>(steps);

                        stepsList.Add(CreateSelfLoopStep(evt));
                        steps = stepsList;
                        HasDeadLock = true;
                    }

                    List<PCSPEventDRAPairMEC> product = NextMEC(steps.ToArray(), DRAState);
                    this.VerificationOutput.Transitions += product.Count;

                    for (int k = product.Count - 1; k >= 0; k--)
                    {
                        PCSPEventDRAPairMEC step = product[k];
                        string tmp = step.ID;
                        //int nextIndex = VisitedWithID.Count;
                        MDPState nextState;

                        if (mdp.States.TryGetValue(tmp, out nextState))
                        {

                            if (!preorder.ContainsKey(tmp))
                            {
                                if (done)
                                {
                                    working.Push(new KeyValuePair<PCSPEventDRAPairMEC, MDPState>(step, nextState));
                                    //stepStack.Add(step.ID);
                                    stepStack.Add(step);
                                    product.RemoveAt(k);
                                    done = false;

                                }
                                else
                                {
                                    product[k] = step;
                                }
                            }
                            else
                            {
                                if (done)
                                {
                                    product.RemoveAt(k);
                                    done = false;
                                }
                            }
                        }
                        else
                        {
                            nextState = new MDPState(tmp);
                            mdp.States.Add(tmp, nextState);

                            if (done)
                            {
                                working.Push(new KeyValuePair<PCSPEventDRAPairMEC, MDPState>(step, nextState));
                                //stepStack.Add(step.ID);
                                stepStack.Add(step);
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
                                product.Insert(k+1, null);//separator
                                newDis = new Distribution(Constants.TAU);
                            }
                                                        
                            Distribution newTrivialDis = new Distribution(pstep.Event, nextState);
                            pair.Value.AddDistribution(newTrivialDis);
                            if (k != 0)
                            {
                                product.Insert(k, null);//separator
                            }
                            
                        }
                        else if (currentDistriIndex != -1 && pstep.DisIndex != currentDistriIndex)
                        {
                            pair.Value.AddDistribution(newDis);
                            product.Insert(k+1, null);//separator
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

                    //List<bool> temp = new List<bool>(outgoing.Count);
                    int length = outgoing.Count;

                    for (int i = 0; i < length; i++)//Distribution list in outgoing)
                    {
                        //temp[i] = true;
                        Distribution list = outgoing[i];//note the order of distribution is reversed
                        int templowlinkV = int.MaxValue;
                        //bool tempUpdate = false;
                        foreach (KeyValuePair<double, MDPState> state in list.States)
                        {
                            string w = state.Value.ID;

                            if (w == currentID && list.States.Count == 1)
                            {
                                selfLoop = true;
                            }

                            if (!MDPState2DRAStateMapping.ContainsKey(w))
                            {
                                //tempUpdate = true;
                                if (preorder[w] > preorderV)
                                {
                                    templowlinkV = Math.Min(templowlinkV, lowlink[w]);
                                    //lowlinkV = Math.Min(lowlinkV, lowlink[w]);
                                }
                                else
                                {
                                    templowlinkV = Math.Min(templowlinkV, preorder[w]);
                                    //lowlinkV = Math.Min(lowlinkV, preorder[w]);
                                }
                            }
                            else
                            {
                                templowlinkV = int.MaxValue;
                                break;
                            }
                        }

                        //if (tempUpdate)
                        //{
                            
                        //}

                        if (templowlinkV == int.MaxValue)
                        {
                            //int index = stepStack.IndexOf(currentID + "separate" + i);
                            //for (int index = stepStack.IndexOf(currentID + "separate" + (i)); index < stepStack.Count; )
                            //{
                            //    if (stepStack[index] != currentID + "separate" + (i+1).ToString() && stepStack[index] != currentID + "end")
                            //    {
                            //        stepStack.RemoveAt(index);
                            //    }
                            //    else
                            //    {
                            //        break;
                            //    }
                            //}
                            bool remove = false;

                            List<int> toRemove = new List<int>();

                            for (int index = stepStack.Count - 1; index >= 0; index--)
                            {
                                //counter++;
                                if (stepStack[index].ID == currentID + "separate" + i)
                                {
                                    remove = true;
                                }
                                if (remove)
                                {
                                    if (stepStack[index].ID != currentID + "separate" + (i - 1) && stepStack[index].ID != currentID)
                                    {
                                        stepStack.RemoveAt(index);
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }
                                else
                                {
                                    if (stepStack[index].ID == currentID)
                                    {
                                        foreach(var ind in toRemove)
                                        {
                                            stepStack.RemoveAt(ind);
                                        }
                                        break;
                                    }
                                    toRemove.Add(index);
                                }
                            }
                        }
                        else
                        {
                            lowlinkV = Math.Min(templowlinkV, lowlinkV);
                        }


                    }

                    lowlink[currentID] = lowlinkV;
                    working.Pop();

                    if (lowlinkV == preorderV)
                    {
                        List<string> SCC = new List<string>(1024);
                        SCC.Add(currentID);
                        MDPState2DRAStateMapping.Add(currentID, DRAState);
                        //while (stepStack.Count > 0 && preorder[stepStack[stepStack.Count - 1]] > preorderV)
                        while(stepStack[stepStack.Count - 1].ID != currentID)
                        {
                            PCSPEventDRAPairMEC s = stepStack[stepStack.Count - 1];
                            string sID = s.ID;                            
                            stepStack.RemoveAt(stepStack.Count - 1);
                            if (!sID.Contains("separate") && !SCC.Contains(sID))
                            {
                                SCC.Add(sID);
                                MDPState2DRAStateMapping.Add(sID, s.state);
                                //MDPState2DRAStateMapping.Add(sID, );
                            }
                            
                        }

                        stepStack.RemoveAt(stepStack.Count - 1);

                        if (SCC.Count > 1 || selfLoop) //evt.IsDeadLock || 
                        {
                            SCCs.Add(SCC);
                        }
                    }
                    //else
                    //{
                    //    stepStack.Push(pair.Key);
                    //}
                }
            } while (working.Count > 0);


            Debug.WriteLine(mdp.States.Count);
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
                //if(sentence.Count == 0)
                //{

                //}
                for (int index = 0; index < count; index++)
                {
                    List<string> newSCC = new List<string>();
                    List<string> targets = new List<string>();

                    if (AlmostFair)
                    {
                        //bool bottom = true;
                        ////int SCCcount = scc.Count;
                        ////note that as long as one SCC(might not be a real MEC) has a U state, the whole SCC cannot be targets.
                        ////RemoveNonECStates(scc, targets);
                        //foreach (string i in scc)
                        //{
                        //    int draState = MDPState2DRAStateMapping[i];
                        //    if (bottom)
                        //    {
                        //        if (DRA.acceptance().isStateInAcceptance_U(index, draState))
                        //        {
                        //            bottom = false;
                        //        }
                        //    }

                        //    newSCC.Add(i);

                        //    if (DRA.acceptance().isStateInAcceptance_L(index, draState))
                        //    {
                        //        targets.Add(i);
                        //    }

                        //}
                        //if (bottom)
                        //{
                        //    if (!BottomECStates(newSCC, targets))
                        //    {
                        //        if (newSCC.Count > 0)
                        //        {
                        //            GroupMEC(newSCC);
                        //        }
                        //    }
                        //    else
                        //    {
                        //        Common.Classes.Ultility.Ultility.AddList(EndComponents, scc);
                        //    }
                        //}
                        //else
                        //{
                        //    RemoveNonECStates(newSCC);
                        //    if (newSCC.Count > 0)
                        //    {
                        //        GroupMEC(newSCC);
                        //    }
                        //}
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
                        if (targets.Count > 0)
                        {
                            RemoveNonECStates(newSCC, targets);
                        }
                        if (targets.Count > 0)
                        {
                            //List<string> endComponent = TarjanModelChecking(index, newSCC);
                            Debug.WriteLine(helper++);
                            //Common.Classes.Ultility.Ultility.AddList(EndComponents, endComponent);
                            Common.Classes.Ultility.Ultility.AddList(EndComponents, scc);//note here newSCC changed to scc
                        }
                        else
                        {
                            if (scc.Count > 1)
                            {
                                GroupMEC(scc);
                            }
                        }

                    }

                }
            }

            foreach (string s in EndComponents)
            {
                mdp.AddTargetStates(mdp.States[s]);
            }
            Debug.WriteLine(mdp.States.Count);
            VerificationOutput.NoOfStates = VerificationOutput.NoOfStates + mdp.States.Count;
            
        }



        //this method is used to group MEC together.
        private void GroupMEC(List<string> scc)
        {
            //used [0] to be a representative state
            MDPState rep = mdp.States[scc[0]];
            HashSet<string> scc_hash = new HashSet<string>(scc);
            HashSet<MDPState> visitted = new HashSet<MDPState>();

            for (int i = 0; i < scc.Count; i++)
            {
                MDPState toGroup = mdp.States[scc[i]];

                for (int j = toGroup.Distributions.Count - 1; j >= 0; j--)
                {
                    Distribution distr = toGroup.Distributions[j];
                    bool keepDistr = false;
                    bool remove = false;
                    foreach (KeyValuePair<double, MDPState> pair in distr.States)
                    {
                        if (!scc.Contains(pair.Value.ID))
                        {
                            keepDistr = true;
                        }
                        else
                        {
                            remove = true;
                        }

                    }

                    toGroup.RemoveDistributionAt(j);

                    if (keepDistr)
                    {
                        if (remove)
                        {
                            Distribution newDistr = DistrbutionReCalculate(scc_hash, toGroup, distr);
                            rep.AddDistribution(newDistr);
                        }
                        else
                        {
                            rep.AddDistribution(distr);
                        }
                    }

                }

                if (i != 0)
                {
                    List<MDPState> PRE = toGroup.Pre;
                    for (int j = PRE.Count - 1; j >= 0; j--)
                    {
                        MDPState PRE_J = PRE[j];
                        if (!visitted.Contains(PRE_J) && !scc_hash.Contains(PRE_J.ID))
                        {
                            visitted.Add(PRE_J);
                            foreach (var distr in PRE_J.Distributions)
                            {
                                for (int k = distr.States.Count - 1; k >= 0; k-- )
                                {
                                    KeyValuePair<double, MDPState> pair = distr.States[k];
                                    if (scc_hash.Contains(pair.Value.ID))
                                    {
                                        distr.AddProbStatePair(pair.Key, rep);
                                        rep.Pre.Add(PRE_J);
                                        distr.States.Remove(pair);
                                    }
                                }
          
                            }
                        }
                        PRE.Remove(PRE_J);
                    }
                    mdp.States.Remove(toGroup.ID);
                }
            }
        }

        public Distribution DistrbutionReCalculate(HashSet<string> scc, MDPState node, Distribution distr)
        {

            double nonSelfLoopProb = 0.0;
            for (int i = 0; i < distr.States.Count; i++)
            {
                KeyValuePair<double, MDPState> pair = distr.States[i];
                if (scc.Contains(pair.Value.ID))
                {
                    //selfLoopProb += pair.Key;
                    //hasSelfLoop = true;
                    //the self loop is removed in this distribution
                    distr.States.Remove(pair);
                    //i-- is used to keep the loop correct after removing one element
                    i--;
                }
                else
                {
                    nonSelfLoopProb += pair.Key;
                }

            }
            foreach (KeyValuePair<double, MDPState> pair in distr.States)
            {
                //KeyValuePair<double, MDPState> newPair = new KeyValuePair<double, MDPState>(pair.Key / nonSelfLoopProb, pair.Value);
                distr.AddProbStatePair(pair.Key / nonSelfLoopProb, pair.Value);
                distr.States.Remove(pair);
            }
            return distr;
        }

        /// <summary>
        /// Given one environment, get the initial states of the product of the system and the automata. Notice that the automata 
        /// is allowed to make one move first. This is necessary to check the very first state of the system. 
        /// </summary>
        /// <param name="initialStep"></param>
        /// <returns></returns>
        private List<PCSPEventDRAPairMEC> GetInitialPairsEMC(MDPConfiguration initialStep)
        {
            List<PCSPEventDRAPairMEC> toReturn = new List<PCSPEventDRAPairMEC>();
            List<int> existed = new List<int>();

            int sIndex = DRA.getStartState().Index;
            List<int> next = DRA.MakeOneMove(sIndex, initialStep.GlobalEnv, initialStep.Event);

            foreach (int var in next)
            {
                if (!existed.Contains(var))
                {
                    existed.Add(var);
                    toReturn.Add(new PCSPEventDRAPairMEC(initialStep, var));
                }
            }

            return toReturn;
        }

        private List<PCSPEventDRAPairMEC> NextMEC(MDPConfiguration[] steps, int BAState)
        {
            List<PCSPEventDRAPairMEC> product = new List<PCSPEventDRAPairMEC>(steps.Length * BA.States.Length);

            for (int i = 0; i < steps.Length; i++)
            {
                MDPConfiguration step = steps[i];
                List<int> states = DRA.MakeOneMove(BAState, step.GlobalEnv, step.Event);

                for (int j = 0; j < states.Count; j++)
                {
                    product.Add(new PCSPEventDRAPairMEC(step, states[j]));
                }
            }

            return product;
        }
  
    }

    public sealed class PCSPEventDRAPairMEC
    {
        public MDPConfiguration configuration;
        public int state;
        public string ID;
        //public int ID;

        public PCSPEventDRAPairMEC(MDPConfiguration e, int s)
        {
            configuration = e;
            state = s;
            ID = this.GetCompressedState();
            //ID = -1;
        }

        public PCSPEventDRAPairMEC(string s)
        {
            configuration = null;
            state = -1;
            ID = s;
        }

        public string GetCompressedState()
        {
            return configuration.GetIDWithEvent() + Constants.SEPARATOR + state;
        }
    }
}