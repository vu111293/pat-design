using System;
using System.Collections.Generic;
using PAT.Common.Classes.Expressions;
using PAT.Common.Classes.Expressions.ExpressionClass;
using System.Text;
using PAT.Common.Classes.LTS;
using PAT.Common.Classes.ModuleInterface;
using PAT.Common.Classes.Ultility;
using System.Diagnostics;
using System.Timers;
using QueryConstraintType = PAT.Common.Classes.Ultility.QueryConstraintType;
using System.Threading.Tasks;

namespace PAT.Common.Classes.SemanticModels.DTMC.Assertion
{
    public abstract class AssertionReachability : LTS.Assertion.AssertionReachability
    {
        protected double Min = -1;
        protected double Max = -1;
        protected int Index = 0;
        protected QueryConstraintType ConstraintType;
        protected bool Sparse = false;
        protected int SCCBound = 100;
        protected const int BOUNDFORUSINGMATLAB = 200000;
        protected double BuildTime = 0;
        protected double VerifyTime = 0;
        protected AssertionReachability(string reachableState)
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

            
            DTMC dtmc = BuildDTMC_SCC_Cut();
            //DTMC dtmc = BuildDTMC();


            Stopwatch VerifyWatch = new Stopwatch();
            VerifyWatch.Start();
            if (!CancelRequested)
            {
                switch (ConstraintType)
                {
                    case QueryConstraintType.PROB:
                        Min = dtmc.MinProbability(VerificationOutput);
                        dtmc.ResetNonTargetState();
                        Max = dtmc.MaxProbability(VerificationOutput);
                        break;
                    case QueryConstraintType.PMAX:
                        Max = dtmc.MaxProbability(VerificationOutput);
                        break;
                    case QueryConstraintType.PMIN:
                        Min = dtmc.MinProbability(VerificationOutput);
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
                VerifyTime = VerifyWatch.Elapsed.TotalSeconds;
            }
        }


        //protected DTMC BuildDTMC_NoTrivial(bool Optimization = false)//note here we assume the model is DTMC
        //{
        //    Stopwatch timer = new Stopwatch();
        //    timer.Start();

        //    int counter1 = 0;
        //    int counter2 = 0;
        //    Stack<KeyValuePair<DTMCConfiguration, DTMCState>> working = new Stack<KeyValuePair<DTMCConfiguration, DTMCState>>(1024);

        //    string initID = InitialStep.GetID();
        //    DTMCState init = new DTMCState(initID);
        //    working.Push(new KeyValuePair<DTMCConfiguration, DTMCState>(InitialStep as DTMCConfiguration, init));
        //    DTMC dtmc = new DTMC(init, Ultility.Ultility.DEFAULT_PRECISION, Ultility.Ultility.MAX_DIFFERENCE);

        //    //Dictionary<string, DTMCState> visited = new Dictionary<string, DTMCState>();
        //    //visited.Add(initID, init);
        //    Dictionary<string, DTMCState> representative = new Dictionary<string, DTMCState>();
        //    DTMCState AbsDTMCState = init;

        //    bool newAbs = true;

        //    do
        //    {
        //        if (CancelRequested)
        //        {
        //            VerificationOutput.NoOfStates = dtmc.States.Count;
        //            return dtmc;
        //        }

        //        KeyValuePair<DTMCConfiguration, DTMCState> current = working.Pop();
        //        IEnumerable<DTMCConfiguration> List = current.Key.MakeOneMoveLocal();
        //        List<DTMCConfiguration> list = new List<DTMCConfiguration>(List);

        //        if (list.Count == 0) continue;

        //        VerificationOutput.Transitions += list.Count;

        //        //if (dtmc.States.ContainsValue(current.Value))
        //        if (newAbs)
        //        {
        //            AbsDTMCState = current.Value;
        //        }

        //        if (list.Count > 1)
        //        {
        //            //Distribution newDis = new Distribution(Constants.TAU);
        //            newAbs = true;
        //            for (int i = 0; i < list.Count; i++)
        //            {

        //                DTMCConfiguration step = list[i];
        //                string stepID = step.GetID();
        //                DTMCState nextState;

        //                if (representative.TryGetValue(stepID, out nextState))
        //                {
        //                    AbsDTMCState.AddTransition(new KeyValuePair<double, DTMCState>(step.Probability, nextState));
        //                    continue;
        //                }
        //                if (!dtmc.States.TryGetValue(stepID, out nextState))
        //                {
        //                    nextState = new DTMCState(stepID);
        //                    dtmc.AddState(nextState);
        //                    //visited.Add(stepID, nextState);

        //                    ExpressionValue v = EvaluatorDenotational.Evaluate(ReachableStateCondition, step.GlobalEnv);

        //                    if ((v as BoolConstant).Value)
        //                    {
        //                        dtmc.AddTargetStates(nextState);
        //                    }
        //                    else
        //                    {
        //                        working.Push(new KeyValuePair<DTMCConfiguration, DTMCState>(step, nextState));
        //                    }
        //                }

        //                AbsDTMCState.AddTransition(new KeyValuePair<double, DTMCState>(step.Probability, nextState));

        //            }

        //        }
        //        else
        //        {

        //            DTMCConfiguration step = list[0];
        //            string stepID = step.GetID();
        //            DTMCState nextState;
        //            //if (dtmc.States.TryGetValue(stepID, out nextState))
        //            //{
        //            //    if (nextState.CurrentProb == 1)
        //            //    {
        //            //        dtmc.AddTargetStates(AbsDTMCState);
        //            //    }
        //            //    else
        //            //    {
        //            //        foreach (var tran in nextState.Transitions)
        //            //        {
        //            //            AbsDTMCState.AddTransition(tran);
        //            //        }
        //            //    }
        //            //    newAbs = true;
        //            //    continue;
        //            //}
        //            if (!representative.TryGetValue(stepID, out nextState))
        //            {

        //                nextState = new DTMCState(stepID);
        //                representative.Add(stepID, AbsDTMCState);

        //                ExpressionValue v = EvaluatorDenotational.Evaluate(ReachableStateCondition, step.GlobalEnv);

        //                if ((v as BoolConstant).Value)
        //                {
        //                    dtmc.AddTargetStates(AbsDTMCState);
        //                    newAbs = true;
        //                }
        //                else
        //                {
        //                    working.Push(new KeyValuePair<DTMCConfiguration, DTMCState>(step, nextState));
        //                    newAbs = false;
        //                }

        //            }
        //            else if (dtmc.States.TryGetValue(stepID, out nextState) || representative.TryGetValue(stepID, out nextState))
        //            {
        //                //note here nextState is actually the abstraction state of the real nextState
        //                newAbs = true;
        //                if (nextState.CurrentProb == 1)
        //                {
        //                    //counter1++;
        //                    dtmc.AddTargetStates(AbsDTMCState);
        //                }
        //                else
        //                {
        //                    foreach (var tran in nextState.Transitions)
        //                    {
        //                        AbsDTMCState.AddTransition(tran);
        //                    }
        //                }

        //            }
        //        }
        //    } while (working.Count > 0);

        //    if (Optimization)
        //    {
        //        VerificationOutput.NoOfStates = dtmc.States.Count;
        //        timer.Stop();

        //        //VerificationOutput.InitModelBuildingTime = timer.Elapsed.TotalSeconds;
        //        return OptimizedDTMC(dtmc);
        //    }
        //    else
        //    {
        //        VerificationOutput.NoOfStates = dtmc.States.Count;
        //        timer.Stop();

        //        //VerificationOutput.InitModelBuildingTime = timer.Elapsed.TotalSeconds;
        //        //DTMC.BackUpTargetStates();
        //        //foreach (var pair in dtmc.States)
        //        //{
        //        //    Debug.WriteLine(pair.Key + "  " + pair.Value.ToString());
        //        //}

        //        return dtmc;
        //    }

        //}

        //private DTMC OptimizedDTMC(DTMC dtmc)//note this function groups two steps together
        //{
        //    DTMCState init = dtmc.InitState;
        //    DTMC dtmc_opt = new DTMC(init, Ultility.Ultility.DEFAULT_PRECISION, Ultility.Ultility.MAX_DIFFERENCE);
        //    VerificationOutput.Transitions = 0;
        //    //dtmc_opt.AddState(init);
        //    Stack<DTMCState> newworking = new Stack<DTMCState>(1024);
        //    newworking.Push(init);
        //    do
        //    {
        //        DTMCState current = newworking.Pop();
        //        if (current.CurrentProb == 1)
        //        {
        //            dtmc_opt.AddTargetStates(current);
        //            continue;
        //        }
        //        VerificationOutput.Transitions += current.Transitions.Count;
        //        List<KeyValuePair<double, DTMCState>> currentTrans = new List<KeyValuePair<double, DTMCState>>();
        //        foreach (var tran in current.Transitions)
        //        {
        //            currentTrans.Add(tran);
        //        }
        //        current.Transitions.Clear();

        //        foreach (var tran in currentTrans)
        //        {
        //            DTMCState state = tran.Value;
        //            if (!dtmc_opt.States.ContainsValue(state))
        //            {
        //                if (state.Transitions.Count > 0)
        //                {
        //                    foreach (var transi in state.Transitions)
        //                    {
        //                        DTMCState State = transi.Value;
        //                        current.AddTransition(new KeyValuePair<double, DTMCState>(tran.Key * transi.Key, State));
        //                        if (!dtmc_opt.States.ContainsValue(State))
        //                        {
        //                            dtmc_opt.AddState(State);
        //                            if (State.CurrentProb == 1)
        //                            {
        //                                dtmc_opt.AddTargetStates(State);
        //                            }
        //                            else
        //                            {
        //                                newworking.Push(State);
        //                            }
        //                        }

        //                    }
        //                    state.RemovePre();
        //                }
        //                else
        //                {
        //                    dtmc_opt.AddState(state);
        //                    current.AddTransition(new KeyValuePair<double, DTMCState>(tran.Key, state));
        //                    if (state.CurrentProb == 1)
        //                    {
        //                        dtmc_opt.AddTargetStates(state);
        //                    }
        //                }

        //            }
        //            else
        //            {
        //                current.AddTransition(new KeyValuePair<double, DTMCState>(tran.Key, state));
        //            }
        //        }

        //    } while (newworking.Count > 0);

        //    VerificationOutput.NoOfStates = dtmc_opt.States.Count;
        //    foreach (var pair in dtmc_opt.States)
        //    {
        //        Debug.WriteLine(pair.Key + "  " + pair.Value.ToString());
        //    }
        //    return dtmc_opt;
        //}

        //protected DTMC BuildDTMC_RoundGame_OTF()//note: must no loop in one round. On the fly approach
        //{
        //    Stopwatch timer = new Stopwatch();
        //    timer.Start();
        //    int counter1 = 0;
        //    int counter2 = 0;
        //    List<KeyValuePair<DTMCConfiguration, DTMCState>> dtmcStates = new List<KeyValuePair<DTMCConfiguration, DTMCState>>();
        //    string initID = InitialStep.GetID();
        //    DTMCState init = new DTMCState(initID);

        //    HashSet<DTMCState> Targets = new HashSet<DTMCState>();

        //    dtmcStates.Add(new KeyValuePair<DTMCConfiguration, DTMCState>(InitialStep as DTMCConfiguration, init));
        //    DTMC dtmc = new DTMC(init, Ultility.Ultility.DEFAULT_PRECISION, Ultility.Ultility.MAX_DIFFERENCE);


        //    for (int I = 0; I < dtmcStates.Count; I++)
        //    {
        //        if (CancelRequested)
        //        {
        //            VerificationOutput.NoOfStates = dtmc.States.Count;
        //            return dtmc;
        //        }
        //        Stack<KeyValuePair<DTMCConfiguration, DTMCState>> working = new Stack<KeyValuePair<DTMCConfiguration, DTMCState>>(1024);
        //        HashSet<DTMCState> oneRoundSuccessor = new HashSet<DTMCState>();

        //        dtmc.ResetNonTargetState();
        //        if (Targets.Contains(dtmcStates[I].Value))// == 1)
        //        {
        //            continue;
        //        }

        //        working.Push(dtmcStates[I]);
        //        DTMCState AbsDTMCState = dtmcStates[I].Value;
        //        AbsDTMCState.CurrentProb = 1;

        //        Dictionary<string, DTMCState> visited = dtmc.States;
        //        //foreach (var pair in dtmc.States)
        //        //{
        //        //    visited.Add(pair.Key, pair.Value);
        //        //}

        //        do
        //        {
        //            KeyValuePair<DTMCConfiguration, DTMCState> current = working.Pop();

        //            DTMCState c_state = current.Value;

        //            IEnumerable<DTMCConfiguration> List = current.Key.MakeOneMoveLocal();
        //            List<DTMCConfiguration> list = new List<DTMCConfiguration>(List);
        //            if (list.Count == 0) continue;
        //            if (list.Count == 1)
        //            {
        //                if (current.Key.Event != "Round")
        //                {
        //                    counter1++;
        //                    DTMCConfiguration step = list[0];
        //                    string stepID = step.GetID();
        //                    DTMCState nextState = new DTMCState(stepID);
        //                    nextState.CurrentProb = c_state.CurrentProb;
        //                    working.Push(new KeyValuePair<DTMCConfiguration, DTMCState>(step, nextState));
        //                }
        //                else
        //                {

        //                    counter2++;
        //                    DTMCConfiguration step = list[0];
        //                    string stepID = step.GetID();
        //                    DTMCState nextState;
        //                    if (!visited.TryGetValue(stepID, out nextState))
        //                    {
        //                        nextState = new DTMCState(stepID);

        //                        ExpressionValue v = EvaluatorDenotational.Evaluate(ReachableStateCondition, step.GlobalEnv);

        //                        if ((v as BoolConstant).Value)
        //                        {
        //                            //Targets.Add(nextState);
        //                            Targets.Add(nextState);
        //                            //nextState.CurrentProb = c_state.CurrentProb * step.Probability;
        //                        }

        //                        nextState.CurrentProb = c_state.CurrentProb * step.Probability;
        //                        visited.Add(stepID, nextState);
        //                        dtmcStates.Add(new KeyValuePair<DTMCConfiguration, DTMCState>(step, nextState));

        //                    }
        //                    else
        //                    {
        //                        if (nextState.CurrentProb == 1)
        //                        {
        //                            nextState.CurrentProb = 0;
        //                        }
        //                        nextState.CurrentProb += c_state.CurrentProb * step.Probability;
        //                    }

        //                    //if (nextState.CurrentProb != 1)
        //                    //{
        //                    //    ExpressionValue v = EvaluatorDenotational.Evaluate(ReachableStateCondition, step.GlobalEnv);

        //                    //    if ((v as BoolConstant).Value)
        //                    //    {
        //                    //        Targets.Add(nextState);
        //                    //        //dtmc.AddTargetStates(nextState);
        //                    //    }
        //                    //}

        //                    oneRoundSuccessor.Add(nextState);

        //                    //if (!dtmc.States.ContainsKey(stepID))
        //                    //{
        //                    //    dtmcStates.Add(new KeyValuePair<DTMCConfiguration, DTMCState>(step, nextState));
        //                    //    dtmc.States.Add(stepID, nextState);
        //                    //}                        
        //                }

        //            }

        //            else
        //            {

        //                for (int i = 0; i < list.Count; i++)
        //                {
        //                    counter1++;
        //                    DTMCConfiguration step = list[i];
        //                    string stepID = step.GetID();
        //                    DTMCState nextState = new DTMCState(stepID);

        //                    //if (!visited.TryGetValue(stepID, out nextState))
        //                    //{
        //                    //    nextState = new DTMCState(stepID);                                
        //                    //nextState.CurrentProb = c_state.CurrentProb * step.Probability;
        //                    //    visited.Add(stepID, nextState);
        //                    //}
        //                    //else
        //                    //{
        //                    nextState.CurrentProb = c_state.CurrentProb * step.Probability;
        //                    //}
        //                    //bool newElement = true;
        //                    //foreach (var pair in working)
        //                    //{
        //                    //    if (pair.Key.GetID() == stepID)
        //                    //    {
        //                    //        newElement = false;
        //                    //        break;
        //                    //    }
        //                    //}
        //                    //if (newElement)
        //                    //{
        //                    working.Push(new KeyValuePair<DTMCConfiguration, DTMCState>(step, nextState));
        //                    //}

        //                }
        //            }
        //            //else
        //            //{

        //            //}

        //        } while (working.Count > 0);

        //        VerificationOutput.Transitions += oneRoundSuccessor.Count;

        //        foreach (DTMCState state in oneRoundSuccessor)
        //        {
        //            AbsDTMCState.AddTransition(new KeyValuePair<double, DTMCState>(state.CurrentProb, state));
        //        }

        //    }
        //    foreach (DTMCState target in Targets)
        //    {
        //        dtmc.AddTargetStates(target);
        //    }
        //    dtmc.ResetNonTargetState();
        //    VerificationOutput.NoOfStates = dtmc.States.Count;
        //    timer.Stop();

        //    //VerificationOutput.InitModelBuildingTime = timer.Elapsed.TotalSeconds;
        //    return dtmc;
        //}

        //protected DTMC BuildDTMC_SCC_MutliCore2()
        //{
        //    string initID = InitialStep.GetID();
        //    DTMCState init = new DTMCState(initID);

        //    DTMC dtmc = new DTMC(init, Ultility.Ultility.DEFAULT_PRECISION, Ultility.Ultility.MAX_DIFFERENCE);

        //    HashSet<string> dtmcSCCstates = new HashSet<string>();

        //    Stack<KeyValuePair<DTMCConfiguration, DTMCState>> working = new Stack<KeyValuePair<DTMCConfiguration, DTMCState>>(1024);
        //    Stack<DTMCState> stepStack = new Stack<DTMCState>();
        //    working.Push(new KeyValuePair<DTMCConfiguration, DTMCState>(InitialStep as DTMCConfiguration, init));
        //    List<HashSet<DTMCState>> SCCs = new List<HashSet<DTMCState>>(64);


        //    Dictionary<string, int> preorder = new Dictionary<string, int>(Common.Classes.Ultility.Ultility.MC_INITIAL_SIZE);
        //    Dictionary<string, int> lowlink = new Dictionary<string, int>(Common.Classes.Ultility.Ultility.MC_INITIAL_SIZE);

        //    int preorderCounter = 0;
        //    Dictionary<string, List<DTMCConfiguration>> ExpendedNode = new Dictionary<string, List<DTMCConfiguration>>();


        //    //Dictionary<HashSet<DTMCState>, HashSet<DTMCState>> SCC2Outputs = new Dictionary<HashSet<DTMCState>, HashSet<DTMCState>>();
        //    //HashSet<DTMCState> INPUTS = new HashSet<DTMCState>();
        //    //HashSet<DTMCState> Middles = new HashSet<DTMCState>();
        //    //HashSet<DTMCState> OUTPUTS = new HashSet<DTMCState>();

        //    //added for multicores
        //    var mapSCCsWithOutput = new Dictionary<HashSet<DTMCState>, HashSet<DTMCState>>();

        //    bool reachTarget = true;
        //    int reachCounter = 0;

        //    do
        //    {
        //        if (CancelRequested)
        //        {
        //            this.VerificationOutput.NoOfStates = this.VerificationOutput.NoOfStates + dtmc.States.Count;
        //            return dtmc;
        //        }

        //        KeyValuePair<DTMCConfiguration, DTMCState> pair = working.Peek();
        //        DTMCConfiguration evt = pair.Key;
        //        string currentID = pair.Key.GetID();
        //        DTMCState currentState = pair.Value;

        //        //List<Distribution> outgoing = pair.Value.;

        //        if (!preorder.ContainsKey(currentID))
        //        {
        //            preorder.Add(currentID, preorderCounter);
        //            preorderCounter++;
        //        }

        //        List<KeyValuePair<double, DTMCState>> Transitions = currentState.Transitions;

        //        bool done = true;

        //        if (ExpendedNode.ContainsKey(currentID))
        //        {
        //            if (reachTarget)
        //            {
        //                currentState.ReachTarget = reachTarget;
        //            }
        //            else
        //            {
        //                reachTarget = currentState.ReachTarget;
        //            }

        //            List<DTMCConfiguration> list = ExpendedNode[currentID];
        //            if (list.Count > 0)
        //            {
        //                for (int k = list.Count - 1; k >= 0; k--)
        //                {
        //                    DTMCConfiguration step = list[k];

        //                    string stepID = step.GetID();
        //                    if (!preorder.ContainsKey(stepID))
        //                    {
        //                        if (done)
        //                        {
        //                            working.Push(new KeyValuePair<DTMCConfiguration, DTMCState>(step, dtmc.States[stepID]));
        //                            done = false;
        //                            list.RemoveAt(k);
        //                        }
        //                    }
        //                    else
        //                    {
        //                        //reachTarget = dtmc.States[stepID].ReachTarget;
        //                        //currentState.ReachTarget = reachTarget;
        //                        if (dtmc.States[stepID].ReachTarget)
        //                        {
        //                            reachTarget = true;
        //                            currentState.ReachTarget = reachTarget;
        //                        }
        //                        list.RemoveAt(k);
        //                        //DTMCState nextState = dtmc.States[stepID];
        //                        //if (Middles.Contains(nextState))
        //                        //{
        //                        //Middles.Remove(nextState);
        //                        //INPUTS.Add(nextState);
        //                        //HashSet<DTMCState> input = new HashSet<DTMCState>();
        //                        //input.Add(nextState);
        //                        //foreach(HashSet<DTMCState> scc in SCCs)
        //                        //{
        //                        //    if(scc.Contains(nextState))
        //                        //    {
        //                        //        HashSet<DTMCState> middle = new HashSet<DTMCState>();
        //                        //        foreach (var state in scc)
        //                        //        {
        //                        //            if(state.ID != stepID)
        //                        //            {
        //                        //                middle.Add(state);
        //                        //            }
        //                        //        }
        //                        //        SCCReduction(input, middle, SCC2Outputs[scc]);
        //                        //        break;
        //                        //    }
        //                        //}
        //                        //}
        //                    }
        //                }
        //            }
        //        }
        //        else
        //        {
        //            //int currentDistriIndex = -1;
        //            //Distribution newDis = new Distribution(Constants.TAU);
        //            reachTarget = false;
        //            IEnumerable<DTMCConfiguration> steps = evt.MakeOneMoveLocal();

        //            //NOTE: here we play a trick for deadlock case: if a deadlock exist in the MDP, we will make a
        //            //self loop transition to remove the deadlock. Deadlock is meaningless in MDP.
        //            if (evt.IsDeadLock)
        //            {
        //                //List<DTMCConfiguration> stepsList = new List<DTMCConfiguration>(steps);
        //                working.Pop();
        //                lowlink.Add(currentID, preorderCounter);
        //                continue;
        //                //stepsList.Add(CreateSelfLoopStep(evt));
        //                //steps = stepsList.ToArray();
        //                //HasDeadLock = true;
        //            }

        //            //List<PCSPEventDRAPair> product = Next(steps, DRAState);
        //            List<DTMCConfiguration> Steps = new List<DTMCConfiguration>(steps);
        //            this.VerificationOutput.Transitions += Steps.Count;

        //            for (int k = Steps.Count - 1; k >= 0; k--)
        //            {
        //                DTMCConfiguration step = Steps[k];
        //                string tmp = step.GetID();

        //                bool target = false;
        //                ExpressionValue v = EvaluatorDenotational.Evaluate(ReachableStateCondition, step.GlobalEnv);

        //                if ((v as BoolConstant).Value)
        //                {
        //                    target = true;
        //                    if (!preorder.ContainsKey(tmp))
        //                    {
        //                        dtmcSCCstates.Add(tmp);
        //                        preorder.Add(tmp, preorderCounter);
        //                        lowlink.Add(tmp, preorderCounter);
        //                        preorderCounter++;
        //                    }
        //                }

        //                //int nextIndex = VisitedWithID.Count;
        //                DTMCState nextState;

        //                if (dtmc.States.TryGetValue(tmp, out nextState))
        //                {
        //                    if (!target && !preorder.ContainsKey(tmp))
        //                    {
        //                        if (done)
        //                        {
        //                            working.Push(new KeyValuePair<DTMCConfiguration, DTMCState>(step, nextState));
        //                            done = false;
        //                            Steps.RemoveAt(k);
        //                        }
        //                        else
        //                        {
        //                            Steps[k] = step;
        //                        }
        //                    }
        //                    else
        //                    {
        //                        Steps.RemoveAt(k);

        //                        if (dtmc.States[tmp].ReachTarget)
        //                        {
        //                            reachTarget = true;
        //                            currentState.ReachTarget = reachTarget;
        //                        }
        //                    }

        //                }
        //                else
        //                {
        //                    nextState = new DTMCState(tmp);
        //                    dtmc.States.Add(tmp, nextState);

        //                    if (done)
        //                    {
        //                        if (target)
        //                        {
        //                            dtmc.AddTargetStates(nextState);
        //                            reachTarget = true;
        //                        }
        //                        else
        //                        {
        //                            working.Push(new KeyValuePair<DTMCConfiguration, DTMCState>(step, nextState));
        //                        }

        //                        done = false;
        //                        Steps.RemoveAt(k);
        //                    }
        //                    else
        //                    {
        //                        if (target)
        //                        {
        //                            Steps.RemoveAt(k);
        //                            dtmc.AddTargetStates(nextState);
        //                            reachTarget = true;
        //                        }
        //                        else
        //                        {
        //                            Steps[k] = step;
        //                        }
        //                    }

        //                }

        //                //DTMCConfiguration pstep = step;
        //                currentState.Transitions.Add(new KeyValuePair<double, DTMCState>(step.Probability, nextState));

        //                //currentState.AddTransition(new KeyValuePair<double, DTMCState>(step.Probability, nextState));

        //            }
        //            ExpendedNode.Add(currentID, Steps);
        //        }

        //        if (done)
        //        {
        //            int lowlinkV = preorder[currentID];
        //            int preorderV = preorder[currentID];

        //            bool selfLoop = false;
        //            foreach (KeyValuePair<double, DTMCState> state in Transitions)
        //            {
        //                string w = state.Value.ID;

        //                if (w == currentID)
        //                {
        //                    selfLoop = true;
        //                }

        //                if (!dtmcSCCstates.Contains(w))
        //                {
        //                    if (preorder[w] > preorderV)
        //                    {
        //                        lowlinkV = Math.Min(lowlinkV, lowlink[w]);
        //                    }
        //                    else
        //                    {
        //                        lowlinkV = Math.Min(lowlinkV, preorder[w]);
        //                    }
        //                }
        //            }


        //            lowlink[currentID] = lowlinkV;
        //            working.Pop();

        //            if (lowlinkV == preorderV)
        //            {
        //                if (currentState.ReachTarget)
        //                {
        //                    HashSet<DTMCState> SCC = new HashSet<DTMCState>();
        //                    //bool reduceScc = false;
        //                    SCC.Add(currentState);
        //                    dtmcSCCstates.Add(currentID);
        //                    while (stepStack.Count > 0 && preorder[stepStack.Peek().ID] > preorderV)
        //                    {
        //                        DTMCState s = stepStack.Pop();
        //                        s.ReachTarget = true;
        //                        string sID = s.ID;
        //                        SCC.Add(s);
        //                        dtmcSCCstates.Add(sID);
        //                    }

        //                    if (SCC.Count > 1 || selfLoop)
        //                    {
        //                        HashSet<DTMCState> outputs = new HashSet<DTMCState>();

        //                        //input.Add(currentState);
        //                        foreach (DTMCState state in SCC)
        //                        {
        //                            foreach (KeyValuePair<double, DTMCState> KVpair in state.Transitions)
        //                            {
        //                                if (!SCC.Contains(KVpair.Value))
        //                                {
        //                                    outputs.Add(KVpair.Value);
        //                                }
        //                            }
        //                        }
        //                        List<DTMCState> scc = new List<DTMCState>(SCC);
        //                        SCCs.Add(SCC);
        //                        mapSCCsWithOutput.Add(SCC, outputs);
        //                        //List<DTMCState> output = new List<DTMCState>(outputs);
        //                        //SCCReductionMatlab(scc, output);
        //                    }
        //                }
        //                else
        //                {
        //                    dtmcSCCstates.Add(currentID);
        //                    while (stepStack.Count > 0 && preorder[stepStack.Peek().ID] > preorderV)
        //                    {
        //                        string sID = stepStack.Pop().ID;
        //                        dtmcSCCstates.Add(sID);
        //                    }
        //                }

        //            }
        //            else
        //            {
        //                stepStack.Push(pair.Value);
        //            }
        //        }

        //    } while (working.Count > 0);

        //    //HashSet<DTMCState> nonSafe = RemoveUselessSCC(dtmc, SCCs);
        //    VerificationOutput.NoOfStates = VerificationOutput.NoOfStates + dtmc.States.Count;


        //    //Mutlicores
        //    Parallel.ForEach(SCCs, scc => SCCReductionGaussian(new List<DTMCState>(scc), new List<DTMCState>(mapSCCsWithOutput[scc])));
        //    //Parallel.ForEach(SCCs, scc => SCCReductionMatlab(new List<DTMCState>(scc), new List<DTMCState>(mapSCCsWithOutput[scc])));
        //    int counter = 0;
        //    BuildPRE(dtmc, ref counter); //TODO




        //    return dtmc;

        //}

        //protected DTMC BuildDTMC_SCC_Mutlicores()
        //{
        //    string initID = InitialStep.GetID();
        //    DTMCState init = new DTMCState(initID);

        //    DTMC dtmc = new DTMC(init, Ultility.Ultility.DEFAULT_PRECISION, Ultility.Ultility.MAX_DIFFERENCE);

        //    HashSet<string> dtmcSCCstates = new HashSet<string>();

        //    Stack<KeyValuePair<DTMCConfiguration, DTMCState>> working = new Stack<KeyValuePair<DTMCConfiguration, DTMCState>>(1024);
        //    Stack<DTMCState> stepStack = new Stack<DTMCState>();
        //    working.Push(new KeyValuePair<DTMCConfiguration, DTMCState>(InitialStep as DTMCConfiguration, init));
        //    List<List<DTMCState>> SCCs = new List<List<DTMCState>>(64);


        //    Dictionary<string, int> preorder = new Dictionary<string, int>(Common.Classes.Ultility.Ultility.MC_INITIAL_SIZE);
        //    Dictionary<string, int> lowlink = new Dictionary<string, int>(Common.Classes.Ultility.Ultility.MC_INITIAL_SIZE);

        //    int preorderCounter = 0;
        //    Dictionary<string, List<DTMCConfiguration>> ExpendedNode = new Dictionary<string, List<DTMCConfiguration>>();


        //    //Dictionary<HashSet<DTMCState>, HashSet<DTMCState>> SCC2Outputs = new Dictionary<HashSet<DTMCState>, HashSet<DTMCState>>();
        //    //HashSet<DTMCState> INPUTS = new HashSet<DTMCState>();
        //    HashSet<DTMCState> Middles = new HashSet<DTMCState>();
        //    //HashSet<DTMCState> OUTPUTS = new HashSet<DTMCState>();

        //    //added for multicores
        //    var mapSCCsWithOutput = new Dictionary<List<DTMCState>, List<DTMCState>>();

        //    do
        //    {
        //        if (CancelRequested)
        //        {
        //            this.VerificationOutput.NoOfStates = this.VerificationOutput.NoOfStates + dtmc.States.Count;
        //            return dtmc;
        //        }

        //        KeyValuePair<DTMCConfiguration, DTMCState> pair = working.Peek();
        //        DTMCConfiguration evt = pair.Key;
        //        string currentID = pair.Key.GetID();
        //        DTMCState currentState = pair.Value;

        //        //List<Distribution> outgoing = pair.Value.;

        //        if (!preorder.ContainsKey(currentID))
        //        {
        //            preorder.Add(currentID, preorderCounter);
        //            preorderCounter++;
        //        }

        //        List<KeyValuePair<double, DTMCState>> Transitions = currentState.Transitions;

        //        bool done = true;

        //        if (ExpendedNode.ContainsKey(currentID))
        //        {
        //            List<DTMCConfiguration> list = ExpendedNode[currentID];
        //            if (list.Count > 0)
        //            {
        //                for (int k = list.Count - 1; k >= 0; k--)
        //                {
        //                    DTMCConfiguration step = list[k];

        //                    string stepID = step.GetID();
        //                    if (!preorder.ContainsKey(stepID))
        //                    {
        //                        if (done)
        //                        {
        //                            working.Push(new KeyValuePair<DTMCConfiguration, DTMCState>(step, dtmc.States[stepID]));
        //                            done = false;
        //                            list.RemoveAt(k);
        //                        }
        //                    }
        //                    else
        //                    {
        //                        list.RemoveAt(k);
        //                        DTMCState nextState = dtmc.States[stepID];
        //                        if (Middles.Contains(nextState))
        //                        {
        //                            Middles.Remove(nextState);

        //                        }
        //                    }
        //                }
        //            }
        //        }
        //        else
        //        {
        //            //int currentDistriIndex = -1;
        //            //Distribution newDis = new Distribution(Constants.TAU);

        //            IEnumerable<DTMCConfiguration> steps = evt.MakeOneMoveLocal();

        //            //NOTE: here we play a trick for deadlock case: if a deadlock exist in the MDP, we will make a
        //            //self loop transition to remove the deadlock. Deadlock is meaningless in MDP.
        //            if (evt.IsDeadLock)
        //            {
        //                //List<DTMCConfiguration> stepsList = new List<DTMCConfiguration>(steps);
        //                working.Pop();
        //                lowlink.Add(currentID, preorderCounter);
        //                continue; ;
        //            }

        //            //List<PCSPEventDRAPair> product = Next(steps, DRAState);
        //            List<DTMCConfiguration> Steps = new List<DTMCConfiguration>(steps);
        //            this.VerificationOutput.Transitions += Steps.Count;

        //            for (int k = Steps.Count - 1; k >= 0; k--)
        //            {
        //                DTMCConfiguration step = Steps[k];
        //                string tmp = step.GetID();

        //                bool target = false;
        //                ExpressionValue v = EvaluatorDenotational.Evaluate(ReachableStateCondition, step.GlobalEnv);

        //                if ((v as BoolConstant).Value)
        //                {
        //                    target = true;
        //                    preorder.Add(tmp, preorderCounter);
        //                    lowlink.Add(tmp, preorderCounter);
        //                    preorderCounter++;
        //                }

        //                //int nextIndex = VisitedWithID.Count;
        //                DTMCState nextState;

        //                if (dtmc.States.TryGetValue(tmp, out nextState))
        //                {
        //                    if (!target && !preorder.ContainsKey(tmp))
        //                    {
        //                        if (done)
        //                        {
        //                            working.Push(new KeyValuePair<DTMCConfiguration, DTMCState>(step, nextState));
        //                            done = false;
        //                            Steps.RemoveAt(k);
        //                        }
        //                        else
        //                        {
        //                            Steps[k] = step;
        //                        }
        //                    }
        //                    else
        //                    {
        //                        Steps.RemoveAt(k);
        //                        if (Middles.Contains(nextState))
        //                        {
        //                            Middles.Remove(nextState);

        //                        }
        //                    }

        //                }
        //                else
        //                {
        //                    nextState = new DTMCState(tmp);
        //                    dtmc.States.Add(tmp, nextState);

        //                    if (done)
        //                    {
        //                        if (target)
        //                        {
        //                            dtmc.AddTargetStates(nextState);
        //                        }
        //                        else
        //                        {
        //                            working.Push(new KeyValuePair<DTMCConfiguration, DTMCState>(step, nextState));
        //                        }

        //                        done = false;
        //                        Steps.RemoveAt(k);
        //                    }
        //                    else
        //                    {
        //                        if (target)
        //                        {
        //                            Steps.RemoveAt(k);
        //                        }
        //                        else
        //                        {
        //                            Steps[k] = step;
        //                        }
        //                    }

        //                }

        //                //DTMCConfiguration pstep = step;
        //                currentState.AddTransition(new KeyValuePair<double, DTMCState>(step.Probability, nextState));

        //            }
        //            ExpendedNode.Add(currentID, Steps);
        //        }

        //        if (done)
        //        {
        //            int lowlinkV = preorder[currentID];
        //            int preorderV = preorder[currentID];

        //            bool selfLoop = false;
        //            foreach (KeyValuePair<double, DTMCState> state in Transitions)
        //            {
        //                string w = state.Value.ID;

        //                if (w == currentID)
        //                {
        //                    selfLoop = true;
        //                }

        //                if (!dtmcSCCstates.Contains(w))
        //                {
        //                    if (preorder[w] > preorderV)
        //                    {
        //                        lowlinkV = Math.Min(lowlinkV, lowlink[w]);
        //                    }
        //                    else
        //                    {
        //                        lowlinkV = Math.Min(lowlinkV, preorder[w]);
        //                    }
        //                }
        //            }


        //            lowlink[currentID] = lowlinkV;
        //            working.Pop();

        //            if (lowlinkV == preorderV)
        //            {
        //                HashSet<DTMCState> SCC = new HashSet<DTMCState>();
        //                SCC.Add(currentState);
        //                dtmcSCCstates.Add(currentID);
        //                while (stepStack.Count > 0 && preorder[stepStack.Peek().ID] > preorderV)
        //                {
        //                    DTMCState s = stepStack.Pop();
        //                    string sID = s.ID;
        //                    SCC.Add(s);
        //                    dtmcSCCstates.Add(sID);
        //                }

        //                if (SCC.Count > 1 || selfLoop) //evt.IsDeadLock || 
        //                {
        //                    //HashSet<DTMCState> input = new HashSet<DTMCState>();
        //                    //HashSet<DTMCState> middle = new HashSet<DTMCState>();
        //                    List<DTMCState> outputs = new List<DTMCState>();

        //                    //input.Add(currentState);
        //                    foreach (DTMCState state in SCC)
        //                    {
        //                        foreach (KeyValuePair<double, DTMCState> KVpair in state.Transitions)
        //                        {
        //                            if (!SCC.Contains(KVpair.Value))
        //                            {
        //                                outputs.Add(KVpair.Value);
        //                            }
        //                        }

        //                        if (state.ID != currentID)
        //                        {
        //                            Middles.Add(state);
        //                        }
        //                    }
        //                    List<DTMCState> scc = new List<DTMCState>(SCC);
        //                    SCCs.Add(scc);
        //                    mapSCCsWithOutput.Add(scc, outputs);
        //                    //Middles.UnionWith(middle);
        //                    //SCC2Outputs.Add(SCC, outputs);
        //                    // SCCReduction(SCC, outputs);
        //                }
        //            }
        //            else
        //            {
        //                stepStack.Push(pair.Value);
        //            }
        //        }

        //    } while (working.Count > 0);

        //    //Mutlicores
        //    Parallel.ForEach(SCCs, scc => SCCReductionGaussian(scc, mapSCCsWithOutput[scc]));


        //    VerificationOutput.NoOfStates = VerificationOutput.NoOfStates + dtmc.States.Count;

        //    foreach (DTMCState state in Middles)
        //    {
        //        dtmc.RemoveState(state);
        //    }

        //    return dtmc;

        //    //mdp.BackUpTargetStates();
        //}
        
        //find scc when build DTMC
        //protected DTMC BuildDTMC_SCC()
        //{
            
        //    string initID = InitialStep.GetID();
        //    DTMCState init = new DTMCState(initID);

        //    DTMC dtmc = new DTMC(init, Ultility.Ultility.DEFAULT_PRECISION, Ultility.Ultility.MAX_DIFFERENCE);

        //    HashSet<string> dtmcSCCstates = new HashSet<string>();

        //    Stack<KeyValuePair<DTMCConfiguration, DTMCState>> working = new Stack<KeyValuePair<DTMCConfiguration, DTMCState>>(1024);
        //    Stack<DTMCState> stepStack = new Stack<DTMCState>();
        //    working.Push(new KeyValuePair<DTMCConfiguration, DTMCState>(InitialStep as DTMCConfiguration, init));
        //    //List<HashSet<DTMCState>> SCCs = new List<HashSet<DTMCState>>(64);


        //    Dictionary<string, int> preorder = new Dictionary<string, int>(Common.Classes.Ultility.Ultility.MC_INITIAL_SIZE);
        //    Dictionary<string, int> lowlink = new Dictionary<string, int>(Common.Classes.Ultility.Ultility.MC_INITIAL_SIZE);

        //    int preorderCounter = 0;
        //    Dictionary<string, List<DTMCConfiguration>> ExpendedNode = new Dictionary<string, List<DTMCConfiguration>>();



        //    bool reachTarget = true;
        //    int reachCounter = 0;

        //    do
        //    {
        //        if (CancelRequested)
        //        {
        //            this.VerificationOutput.NoOfStates = this.VerificationOutput.NoOfStates + dtmc.States.Count;
        //            return dtmc;
        //        }

        //        KeyValuePair<DTMCConfiguration, DTMCState> pair = working.Peek();
        //        DTMCConfiguration evt = pair.Key;
        //        string currentID = pair.Key.GetID();
        //        DTMCState currentState = pair.Value;

        //        //List<Distribution> outgoing = pair.Value.;

        //        if (!preorder.ContainsKey(currentID))
        //        {
        //            preorder.Add(currentID, preorderCounter);
        //            preorderCounter++;
        //        }
        //        List<KeyValuePair<double, DTMCState>> Transitions = currentState.Transitions;

        //        bool done = true;

        //        if (ExpendedNode.ContainsKey(currentID))
        //        {
        //            if (reachTarget)
        //            {
        //                currentState.ReachTarget = reachTarget;
        //            }
        //            else
        //            {
        //                reachTarget = currentState.ReachTarget;
        //            }

        //            List<DTMCConfiguration> list = ExpendedNode[currentID];
        //            if (list.Count > 0)
        //            {
        //                for (int k = list.Count - 1; k >= 0; k--)
        //                {
        //                    DTMCConfiguration step = list[k];

        //                    string stepID = step.GetID();
        //                    if (!preorder.ContainsKey(stepID))
        //                    {
        //                        if (done)
        //                        {
        //                            working.Push(new KeyValuePair<DTMCConfiguration, DTMCState>(step, dtmc.States[stepID]));
        //                            done = false;
        //                            list.RemoveAt(k);
        //                        }
        //                    }
        //                    else
        //                    {
        //                        //reachTarget = dtmc.States[stepID].ReachTarget;
        //                        //currentState.ReachTarget = reachTarget;
        //                        if (dtmc.States[stepID].ReachTarget)
        //                        {
        //                            reachTarget = true;
        //                            currentState.ReachTarget = reachTarget;
        //                        }
        //                        list.RemoveAt(k);
        //                        //DTMCState nextState = dtmc.States[stepID];
        //                        //if (Middles.Contains(nextState))
        //                        //{
        //                        //Middles.Remove(nextState);
        //                        //INPUTS.Add(nextState);
        //                        //HashSet<DTMCState> input = new HashSet<DTMCState>();
        //                        //input.Add(nextState);
        //                        //foreach(HashSet<DTMCState> scc in SCCs)
        //                        //{
        //                        //    if(scc.Contains(nextState))
        //                        //    {
        //                        //        HashSet<DTMCState> middle = new HashSet<DTMCState>();
        //                        //        foreach (var state in scc)
        //                        //        {
        //                        //            if(state.ID != stepID)
        //                        //            {
        //                        //                middle.Add(state);
        //                        //            }
        //                        //        }
        //                        //        SCCReduction(input, middle, SCC2Outputs[scc]);
        //                        //        break;
        //                        //    }
        //                        //}
        //                        //}
        //                    }
        //                }
        //            }
        //        }
        //        else
        //        {
        //            //int currentDistriIndex = -1;
        //            //Distribution newDis = new Distribution(Constants.TAU);
        //            reachTarget = false;
        //            IEnumerable<DTMCConfiguration> steps = evt.MakeOneMoveLocal();

        //            //NOTE: here we play a trick for deadlock case: if a deadlock exist in the MDP, we will make a
        //            //self loop transition to remove the deadlock. Deadlock is meaningless in MDP.
        //            if (evt.IsDeadLock)
        //            {
        //                //List<DTMCConfiguration> stepsList = new List<DTMCConfiguration>(steps);
        //                working.Pop();
        //                if (!lowlink.ContainsKey(currentID))
        //                {
        //                    lowlink.Add(currentID, preorderCounter);
        //                }

        //                continue;
        //                //stepsList.Add(CreateSelfLoopStep(evt));
        //                //steps = stepsList.ToArray();
        //                //HasDeadLock = true;
        //            }

        //            //List<PCSPEventDRAPair> product = Next(steps, DRAState);
        //            List<DTMCConfiguration> Steps = new List<DTMCConfiguration>(steps);
        //            this.VerificationOutput.Transitions += Steps.Count;

        //            for (int k = Steps.Count - 1; k >= 0; k--)
        //            {
        //                DTMCConfiguration step = Steps[k];
        //                string tmp = step.GetID();

        //                bool target = false;
        //                ExpressionValue v = EvaluatorDenotational.Evaluate(ReachableStateCondition, step.GlobalEnv);

        //                if ((v as BoolConstant).Value)
        //                {
        //                    target = true;
        //                    if (!preorder.ContainsKey(tmp))
        //                    {
        //                        dtmcSCCstates.Add(tmp);
        //                        preorder.Add(tmp, preorderCounter);
        //                        lowlink.Add(tmp, preorderCounter);
        //                        preorderCounter++;
        //                    }
        //                }

        //                //int nextIndex = VisitedWithID.Count;
        //                DTMCState nextState;

        //                if (dtmc.States.TryGetValue(tmp, out nextState))
        //                {
        //                    if (!target && !preorder.ContainsKey(tmp))
        //                    {
        //                        if (done)
        //                        {
        //                            working.Push(new KeyValuePair<DTMCConfiguration, DTMCState>(step, nextState));
        //                            done = false;
        //                            Steps.RemoveAt(k);
        //                        }
        //                        else
        //                        {
        //                            Steps[k] = step;
        //                        }
        //                    }
        //                    else
        //                    {
        //                        Steps.RemoveAt(k);

        //                        if (dtmc.States[tmp].ReachTarget)
        //                        {
        //                            reachTarget = true;
        //                            currentState.ReachTarget = reachTarget;
        //                        }
        //                    }

        //                }
        //                else
        //                {
        //                    nextState = new DTMCState(tmp);
        //                    dtmc.States.Add(tmp, nextState);

        //                    if (done)
        //                    {
        //                        if (target)
        //                        {
        //                            dtmc.AddTargetStates(nextState);
        //                            reachTarget = true;
        //                        }
        //                        else
        //                        {
        //                            working.Push(new KeyValuePair<DTMCConfiguration, DTMCState>(step, nextState));
        //                        }

        //                        done = false;
        //                        Steps.RemoveAt(k);
        //                    }
        //                    else
        //                    {
        //                        if (target)
        //                        {
        //                            Steps.RemoveAt(k);
        //                            dtmc.AddTargetStates(nextState);
        //                            reachTarget = true;
        //                        }
        //                        else
        //                        {
        //                            Steps[k] = step;
        //                        }
        //                    }

        //                }

        //                //DTMCConfiguration pstep = step;
        //                currentState.Transitions.Add(new KeyValuePair<double, DTMCState>(step.Probability, nextState));

        //                //currentState.AddTransition(new KeyValuePair<double, DTMCState>(step.Probability, nextState));

        //            }
        //            ExpendedNode.Add(currentID, Steps);
        //        }

        //        if (done)
        //        {
        //            int lowlinkV = preorder[currentID];
        //            int preorderV = preorder[currentID];

        //            bool selfLoop = false;
        //            foreach (KeyValuePair<double, DTMCState> state in Transitions)
        //            {
        //                string w = state.Value.ID;

        //                if (w == currentID)
        //                {
        //                    selfLoop = true;
        //                }

        //                if (!dtmcSCCstates.Contains(w))
        //                {
        //                    if (preorder[w] > preorderV)
        //                    {
        //                        lowlinkV = Math.Min(lowlinkV, lowlink[w]);
        //                    }
        //                    else
        //                    {
        //                        lowlinkV = Math.Min(lowlinkV, preorder[w]);
        //                    }
        //                }
        //            }


        //            lowlink[currentID] = lowlinkV;
        //            working.Pop();

        //            if (lowlinkV == preorderV)
        //            {
        //                if (currentState.ReachTarget)
        //                {
        //                    HashSet<DTMCState> SCC = new HashSet<DTMCState>();
        //                    //bool reduceScc = false;
        //                    SCC.Add(currentState);
        //                    dtmcSCCstates.Add(currentID);
        //                    while (stepStack.Count > 0 && preorder[stepStack.Peek().ID] > preorderV)
        //                    {
        //                        DTMCState s = stepStack.Pop();
        //                        s.ReachTarget = true;
        //                        string sID = s.ID;
        //                        SCC.Add(s);
        //                        dtmcSCCstates.Add(sID);
        //                    }

        //                    if (SCC.Count > 1 || selfLoop)
        //                    {
        //                        HashSet<DTMCState> outputs = new HashSet<DTMCState>();

        //                        //input.Add(currentState);
        //                        foreach (DTMCState state in SCC)
        //                        {
        //                            foreach (KeyValuePair<double, DTMCState> KVpair in state.Transitions)
        //                            {
        //                                if (!SCC.Contains(KVpair.Value))
        //                                {
        //                                    outputs.Add(KVpair.Value);
        //                                }
        //                            }
        //                        }
        //                        //SCCs.Add(SCC);
        //                        List<DTMCState> scc = new List<DTMCState>(SCC);
        //                        List<DTMCState> output = new List<DTMCState>(outputs);
        //                        //SCCReductionMatlab(scc, output);
        //                       // SCCReduction(scc, output);
        //                        //SCCReductionGaussian(scc, output);
        //                        SCCReductionGaussianSparseMatrix(scc,output);
        //                    }
        //                }
        //                else
        //                {
        //                    dtmcSCCstates.Add(currentID);
        //                    while (stepStack.Count > 0 && preorder[stepStack.Peek().ID] > preorderV)
        //                    {
        //                        string sID = stepStack.Pop().ID;
        //                        dtmcSCCstates.Add(sID);
        //                    }
        //                }

        //            }
        //            else
        //            {
        //                stepStack.Push(pair.Value);
        //            }
        //        }

        //    } while (working.Count > 0);

        //    //HashSet<DTMCState> nonSafe = RemoveUselessSCC(dtmc, SCCs);
        //    VerificationOutput.NoOfStates = VerificationOutput.NoOfStates + dtmc.States.Count;
        //    int counter = 0;
        //    BuildPRE(dtmc, ref counter);

        //    return dtmc;

        //}

        //protected DTMC BuildDTMC_SCC_Cut()
        //{
        //    string initID = InitialStep.GetID();
        //    DTMCState init = new DTMCState(initID);

        //    DTMC dtmc = new DTMC(init, Ultility.Ultility.DEFAULT_PRECISION, Ultility.Ultility.MAX_DIFFERENCE);

        //    HashSet<string> dtmcSCCstates = new HashSet<string>();

        //    Stack<KeyValuePair<DTMCConfiguration, DTMCState>> working = new Stack<KeyValuePair<DTMCConfiguration, DTMCState>>(1024);
        //    Stack<DTMCState> stepStack = new Stack<DTMCState>();
        //    working.Push(new KeyValuePair<DTMCConfiguration, DTMCState>(InitialStep as DTMCConfiguration, init));
        //    List<HashSet<DTMCState>> SCCs = new List<HashSet<DTMCState>>(64);//note here SCCs is just used to store sccs which is cut;

        //    Dictionary<string, int> preorder = new Dictionary<string, int>(Common.Classes.Ultility.Ultility.MC_INITIAL_SIZE);
        //    Dictionary<string, int> lowlink = new Dictionary<string, int>(Common.Classes.Ultility.Ultility.MC_INITIAL_SIZE);

        //    int preorderCounter = 0;
        //    Dictionary<string, List<DTMCConfiguration>> ExpendedNode = new Dictionary<string, List<DTMCConfiguration>>();
        //    Dictionary<int, List<DTMCState>> scc2out = new Dictionary<int, List<DTMCState>>();
        //    Dictionary<int, HashSet<DTMCState>> scc2input = new Dictionary<int, HashSet<DTMCState>>();

        //    bool reachTarget = true;

        //    do
        //    {
        //        if (CancelRequested)
        //        {
        //            this.VerificationOutput.NoOfStates = this.VerificationOutput.NoOfStates + dtmc.States.Count;
        //            return dtmc;
        //        }

        //        KeyValuePair<DTMCConfiguration, DTMCState> pair = working.Peek();
        //        DTMCConfiguration evt = pair.Key;
        //        string currentID = evt.GetID();
        //        DTMCState currentState = pair.Value;

        //        //List<Distribution> outgoing = pair.Value.;

        //        if (!preorder.ContainsKey(currentID))
        //        {
        //            preorder.Add(currentID, preorderCounter);
        //            preorderCounter++;
        //        }

        //        List<KeyValuePair<double, DTMCState>> Transitions = currentState.Transitions;

        //        bool done = true;

        //        if (ExpendedNode.ContainsKey(currentID))
        //        {
        //            if (reachTarget)
        //            {
        //                currentState.ReachTarget = reachTarget;
        //            }
        //            else
        //            {
        //                reachTarget = currentState.ReachTarget;
        //            }

        //            List<DTMCConfiguration> list = ExpendedNode[currentID];
        //            if (list.Count > 0)
        //            {
        //                for (int k = list.Count - 1; k >= 0; k--)
        //                {
        //                    DTMCConfiguration step = list[k];

        //                    string stepID = step.GetID();
        //                    if (!preorder.ContainsKey(stepID))
        //                    {
        //                        if (done)
        //                        {
        //                            working.Push(new KeyValuePair<DTMCConfiguration, DTMCState>(step, dtmc.States[stepID]));
        //                            done = false;
        //                            list.RemoveAt(k);
        //                        }
        //                    }
        //                    else
        //                    {
        //                        //reachTarget = dtmc.States[stepID].ReachTarget;
        //                        //currentState.ReachTarget = reachTarget;
        //                        DTMCState s = dtmc.States[stepID];
        //                        if (s.ReachTarget)
        //                        {
        //                            reachTarget = true;
        //                            currentState.ReachTarget = reachTarget;

        //                            if (s.SCCIndex >= 0)
        //                            {
        //                                scc2input[s.SCCIndex].Add(s);
        //                            }

        //                        }
        //                        list.RemoveAt(k);
        //                    }
        //                }
        //            }
        //        }
        //        else
        //        {
        //            //int currentDistriIndex = -1;
        //            //Distribution newDis = new Distribution(Constants.TAU);
        //            reachTarget = false;
        //            IEnumerable<DTMCConfiguration> steps = evt.MakeOneMoveLocal();
        //            if (evt.IsDeadLock)
        //            {
        //                //List<DTMCConfiguration> stepsList = new List<DTMCConfiguration>(steps);
        //                working.Pop();
        //                dtmcSCCstates.Add(currentID);
        //                if (!lowlink.ContainsKey(currentID))
        //                {
        //                    lowlink.Add(currentID, preorderCounter);
        //                }
        //                continue;
        //            }

        //            //List<PCSPEventDRAPair> product = Next(steps, DRAState);
        //            List<DTMCConfiguration> Steps = new List<DTMCConfiguration>(steps);
        //            this.VerificationOutput.Transitions += Steps.Count;

        //            for (int k = Steps.Count - 1; k >= 0; k--)
        //            {
        //                DTMCConfiguration step = Steps[k];
        //                string tmp = step.GetID();

        //                bool target = false;
        //                ExpressionValue v = EvaluatorDenotational.Evaluate(ReachableStateCondition, step.GlobalEnv);

        //                if ((v as BoolConstant).Value)
        //                {
        //                    target = true;
        //                    if (!preorder.ContainsKey(tmp))
        //                    {
        //                        dtmcSCCstates.Add(tmp);
        //                        preorder.Add(tmp, preorderCounter);
        //                        lowlink.Add(tmp, preorderCounter);
        //                        preorderCounter++;
        //                    }
        //                }

        //                //int nextIndex = VisitedWithID.Count;
        //                DTMCState nextState;

        //                if (dtmc.States.TryGetValue(tmp, out nextState))
        //                {
        //                    if (!target && !preorder.ContainsKey(tmp))
        //                    {
        //                        if (done)
        //                        {
        //                            working.Push(new KeyValuePair<DTMCConfiguration, DTMCState>(step, nextState));
        //                            done = false;
        //                            Steps.RemoveAt(k);
        //                        }
        //                        else
        //                        {
        //                            Steps[k] = step;
        //                        }
        //                    }
        //                    else
        //                    {
        //                        Steps.RemoveAt(k);
        //                        DTMCState s = dtmc.States[tmp];
        //                        if (s.ReachTarget)
        //                        {
        //                            reachTarget = true;
        //                            currentState.ReachTarget = reachTarget;
        //                            if (s.SCCIndex >= 0)
        //                            {
        //                                scc2input[s.SCCIndex].Add(s);
        //                            }
        //                        }
        //                    }

        //                }
        //                else
        //                {
        //                    nextState = new DTMCState(tmp);
        //                    dtmc.States.Add(tmp, nextState);

        //                    if (done)
        //                    {
        //                        if (target)
        //                        {
        //                            dtmc.AddTargetStates(nextState);
        //                            reachTarget = true;
        //                        }
        //                        else
        //                        {
        //                            working.Push(new KeyValuePair<DTMCConfiguration, DTMCState>(step, nextState));
        //                        }

        //                        done = false;
        //                        Steps.RemoveAt(k);
        //                    }
        //                    else
        //                    {
        //                        if (target)
        //                        {
        //                            Steps.RemoveAt(k);
        //                            dtmc.AddTargetStates(nextState);
        //                            reachTarget = true;
        //                        }
        //                        else
        //                        {
        //                            Steps[k] = step;
        //                        }
        //                    }

        //                }

        //                //DTMCConfiguration pstep = step;
        //                currentState.Transitions.Add(new KeyValuePair<double, DTMCState>(step.Probability, nextState));

        //                //currentState.AddTransition(new KeyValuePair<double, DTMCState>(step.Probability, nextState));

        //            }
        //            ExpendedNode.Add(currentID, Steps);
        //        }

        //        if (done)
        //        {
        //            int lowlinkV = preorder[currentID];
        //            int preorderV = preorder[currentID];

        //            bool selfLoop = false;
        //            foreach (KeyValuePair<double, DTMCState> state in Transitions)
        //            {
        //                string w = state.Value.ID;

        //                if (w == currentID)
        //                {
        //                    selfLoop = true;
        //                }

        //                if (!dtmcSCCstates.Contains(w))
        //                {
        //                    if (preorder[w] > preorderV)
        //                    {
        //                        lowlinkV = Math.Min(lowlinkV, lowlink[w]);
        //                    }
        //                    else
        //                    {
        //                        lowlinkV = Math.Min(lowlinkV, preorder[w]);
        //                    }
        //                }
        //            }


        //            lowlink[currentID] = lowlinkV;
        //            working.Pop();

        //            if (lowlinkV == preorderV)
        //            {
        //                if (currentState.ReachTarget)
        //                {
        //                    HashSet<DTMCState> SCC = new HashSet<DTMCState>();
        //                    List<HashSet<DTMCState>> Groupings = new List<HashSet<DTMCState>>();
        //                    HashSet<DTMCState> Grouping = new HashSet<DTMCState>();
        //                    //bool reduceScc = false;
        //                    Grouping.Add(currentState);
        //                    SCC.Add(currentState);
        //                    int count = 1;
        //                    dtmcSCCstates.Add(currentID);
        //                    while (stepStack.Count > 0 && preorder[stepStack.Peek().ID] > preorderV && count < SCCBound)
        //                    {
        //                        DTMCState s = stepStack.Pop();
        //                        s.ReachTarget = true;
        //                        string sID = s.ID;
        //                        Grouping.Add(s);
        //                        SCC.Add(s);
        //                        dtmcSCCstates.Add(sID);
        //                        count++;
        //                    }
        //                    Groupings.Add(Grouping);

        //                    while (stepStack.Count > 0 && preorder[stepStack.Peek().ID] > preorderV)
        //                    {
        //                        count = 0;
        //                        Grouping = new HashSet<DTMCState>();
        //                        while (stepStack.Count > 0 && preorder[stepStack.Peek().ID] > preorderV && count < SCCBound)
        //                        {
        //                            DTMCState s = stepStack.Pop();
        //                            s.ReachTarget = true;
        //                            string sID = s.ID;
        //                            Grouping.Add(s);
        //                            SCC.Add(s);
        //                            dtmcSCCstates.Add(sID);
        //                            count++;
        //                        }
        //                        Groupings.Add(Grouping);
        //                    }

        //                    if (SCC.Count > 1 || selfLoop)
        //                    {
        //                        if (Groupings.Count > 1)
        //                        {
        //                            SCCs.Add(SCC);
        //                            int Count = SCCs.Count;
        //                            foreach (var state in SCC)
        //                            {
        //                                state.SCCIndex = Count - 1;
        //                            }
        //                            HashSet<DTMCState> inputs = new HashSet<DTMCState>();
        //                            inputs.Add(currentState);
        //                            scc2input.Add(currentState.SCCIndex, inputs);
        //                            HashSet<DTMCState> outofScc = new HashSet<DTMCState>();
        //                            foreach (var Group in Groupings)
        //                            {
        //                                if (Group.Count > 1 || selfLoop)
        //                                {
        //                                    HashSet<DTMCState> outputs = new HashSet<DTMCState>();

        //                                    //input.Add(currentState);
        //                                    foreach (DTMCState state in Group)
        //                                    {
        //                                        foreach (KeyValuePair<double, DTMCState> KVpair in state.Transitions)
        //                                        {
        //                                            DTMCState nextState = KVpair.Value;
        //                                            if (!Group.Contains(nextState))
        //                                            {
        //                                                outputs.Add(nextState);
        //                                                if (!SCC.Contains(nextState))
        //                                                {
        //                                                    outofScc.Add(nextState);
        //                                                }
        //                                            }
        //                                        }
        //                                    }
        //                                    List<DTMCState> group = new List<DTMCState>(Group);
        //                                    List<DTMCState> output = new List<DTMCState>(outputs);
        //                                    //SCCReductionMatlab(scc, output);
        //                                    SCCReduction(group, output);
        //                                }
        //                            }
        //                            List<DTMCState> outofscc = new List<DTMCState>(outofScc);
        //                            scc2out.Add(SCCs.IndexOf(SCC), outofscc);
        //                        }
        //                        else
        //                        {
        //                            HashSet<DTMCState> outputs = new HashSet<DTMCState>();

        //                            //input.Add(currentState);
        //                            foreach (DTMCState state in SCC)
        //                            {
        //                                foreach (KeyValuePair<double, DTMCState> KVpair in state.Transitions)
        //                                {
        //                                    if (!SCC.Contains(KVpair.Value))
        //                                    {
        //                                        outputs.Add(KVpair.Value);
        //                                    }
        //                                }
        //                            }
        //                            //SCCs.Add(SCC);
        //                            List<DTMCState> scc = new List<DTMCState>(SCC);
        //                            List<DTMCState> output = new List<DTMCState>(outputs);
        //                            //SCCReductionMatlab(scc, output);
        //                            SCCReduction(scc, output);
        //                        }
        //                    }


        //                }
        //                else
        //                {
        //                    dtmcSCCstates.Add(currentID);
        //                    while (stepStack.Count > 0 && preorder[stepStack.Peek().ID] > preorderV)
        //                    {
        //                        string sID = stepStack.Pop().ID;
        //                        dtmcSCCstates.Add(sID);
        //                    }
        //                }

        //            }
        //            else
        //            {
        //                stepStack.Push(pair.Value);
        //            }
        //        }

        //    } while (working.Count > 0);

        //    //bool reduced = true;
        //    while (SCCs.Count > 0)// && reduced)
        //    {
        //        //reduced = false;
        //        for (int i = SCCs.Count - 1; i >= 0; i--)
        //        {
        //            int index = 0;
        //            foreach(var state in SCCs[i])
        //            {
        //                index = state.SCCIndex;
        //                break;
        //            }
        //            HashSet<DTMCState> Working = scc2input[index];
        //            HashSet<DTMCState> visited = new HashSet<DTMCState>();
        //            visited.UnionWith(Working);
        //            do
        //            {
        //                HashSet<DTMCState> newWorking = new HashSet<DTMCState>();
        //                foreach (var state in Working)
        //                {
        //                    foreach (var tran in state.Transitions)
        //                    {
        //                        DTMCState tranV = tran.Value;
        //                        if (!visited.Contains(tranV) && SCCs[i].Contains(tranV))
        //                        {
        //                            newWorking.Add(tranV);
        //                            visited.Add(tranV);
        //                        }
        //                    }
        //                }
        //                Working = newWorking;

        //            } while (Working.Count > 0);



        //            if (visited.Count > SCCBound && visited.Count < SCCs[i].Count)
        //            {
        //                //reduced = true;
        //                SCCs[i] = visited;
        //                //note cut again
        //                List<DTMCState> SCCi = new List<DTMCState>(SCCs[i]);
        //                int Counter = 0;
        //                while (Counter < SCCi.Count - SCCBound)
        //                {
        //                    List<DTMCState> group = SCCi.GetRange(Counter, SCCBound);
        //                    HashSet<DTMCState> outputs = new HashSet<DTMCState>();

        //                    //input.Add(currentState);
        //                    foreach (DTMCState state in group)
        //                    {
        //                        foreach (KeyValuePair<double, DTMCState> KVpair in state.Transitions)
        //                        {
        //                            DTMCState nextState = KVpair.Value;
        //                            if (!group.Contains(nextState))
        //                            {
        //                                outputs.Add(nextState);
        //                            }
        //                        }
        //                    }

        //                    List<DTMCState> output = new List<DTMCState>(outputs);
        //                    //SCCReductionMatlab(scc, output);
        //                    SCCReduction(group, output);
        //                    Counter += SCCBound;
        //                }
        //                List<DTMCState> Group = SCCi.GetRange(Counter, SCCi.Count - Counter);
        //                HashSet<DTMCState> Outputs = new HashSet<DTMCState>();

        //                //input.Add(currentState);
        //                foreach (DTMCState state in Group)
        //                {
        //                    foreach (KeyValuePair<double, DTMCState> KVpair in state.Transitions)
        //                    {
        //                        DTMCState nextState = KVpair.Value;
        //                        if (!Group.Contains(nextState))
        //                        {
        //                            Outputs.Add(nextState);
        //                        }
        //                    }
        //                }

        //                List<DTMCState> Output = new List<DTMCState>(Outputs);
        //                //SCCReductionMatlab(scc, output);
        //                SCCReduction(Group, Output);

        //            }
        //            else
        //            {
        //                List<DTMCState> scc = new List<DTMCState>(visited);
        //                SCCReduction(scc, scc2out[index]);
        //                SCCs.RemoveAt(i);
        //            }
        //        }
        //    }

        //    //HashSet<DTMCState> nonSafe = RemoveUselessSCC(dtmc, SCCs);
        //    VerificationOutput.NoOfStates = VerificationOutput.NoOfStates + dtmc.States.Count;
        //    int counter = 0;
        //    BuildPRE(dtmc, ref counter);

        //    return dtmc;

        //}

        //protected DTMC BuildDTMC_SCC_Cut_MultiCores()
        //{
        //    string initID = InitialStep.GetID();
        //    DTMCState init = new DTMCState(initID);

        //    DTMC dtmc = new DTMC(init, Ultility.Ultility.DEFAULT_PRECISION, Ultility.Ultility.MAX_DIFFERENCE);

        //    HashSet<string> dtmcSCCstates = new HashSet<string>();

        //    Stack<KeyValuePair<DTMCConfiguration, DTMCState>> working = new Stack<KeyValuePair<DTMCConfiguration, DTMCState>>(1024);
        //    Stack<DTMCState> stepStack = new Stack<DTMCState>();
        //    working.Push(new KeyValuePair<DTMCConfiguration, DTMCState>(InitialStep as DTMCConfiguration, init));
        //    List<HashSet<DTMCState>> SCCs = new List<HashSet<DTMCState>>(64);//note here SCCs is just used to store sccs which is cut;
        //    List<HashSet<DTMCState>> CUTs = new List<HashSet<DTMCState>>();


        //    Dictionary<string, int> preorder = new Dictionary<string, int>(Common.Classes.Ultility.Ultility.MC_INITIAL_SIZE);
        //    Dictionary<string, int> lowlink = new Dictionary<string, int>(Common.Classes.Ultility.Ultility.MC_INITIAL_SIZE);

        //    int preorderCounter = 0;
        //    Dictionary<string, List<DTMCConfiguration>> ExpendedNode = new Dictionary<string, List<DTMCConfiguration>>();
        //    Dictionary<int, List<DTMCState>> scc2out = new Dictionary<int, List<DTMCState>>();
        //    Dictionary<int, HashSet<DTMCState>> scc2input = new Dictionary<int, HashSet<DTMCState>>();

        //    bool reachTarget = true;
        //    int reachCounter = 0;

        //    //GL: For multicores usage: collect small SCCs
        //    //List<List<DTMCState>> groupsSmallSCCs = new List<List<DTMCState>>(); 
        //    var MapSmallSCCsWithOutput = new Dictionary<List<DTMCState>, List<DTMCState>>();

        //    do
        //    {
        //        if (CancelRequested)
        //        {
        //            this.VerificationOutput.NoOfStates = this.VerificationOutput.NoOfStates + dtmc.States.Count;
        //            return dtmc;
        //        }

        //        KeyValuePair<DTMCConfiguration, DTMCState> pair = working.Peek();
        //        DTMCConfiguration evt = pair.Key;
        //        string currentID = evt.GetID();
        //        DTMCState currentState = pair.Value;

        //        //List<Distribution> outgoing = pair.Value.;

        //        if (!preorder.ContainsKey(currentID))
        //        {
        //            preorder.Add(currentID, preorderCounter);
        //            preorderCounter++;
        //        }

        //        List<KeyValuePair<double, DTMCState>> Transitions = currentState.Transitions;

        //        bool done = true;

        //        if (ExpendedNode.ContainsKey(currentID))
        //        {
        //            if (reachTarget)
        //            {
        //                currentState.ReachTarget = reachTarget;
        //            }
        //            else
        //            {
        //                reachTarget = currentState.ReachTarget;
        //            }

        //            List<DTMCConfiguration> list = ExpendedNode[currentID];
        //            if (list.Count > 0)
        //            {
        //                for (int k = list.Count - 1; k >= 0; k--)
        //                {
        //                    DTMCConfiguration step = list[k];

        //                    string stepID = step.GetID();
        //                    if (!preorder.ContainsKey(stepID))
        //                    {
        //                        if (done)
        //                        {
        //                            working.Push(new KeyValuePair<DTMCConfiguration, DTMCState>(step, dtmc.States[stepID]));
        //                            done = false;
        //                            list.RemoveAt(k);
        //                        }
        //                    }
        //                    else
        //                    {
        //                        //reachTarget = dtmc.States[stepID].ReachTarget;
        //                        //currentState.ReachTarget = reachTarget;
        //                        DTMCState s = dtmc.States[stepID];
        //                        if (s.ReachTarget)
        //                        {
        //                            reachTarget = true;
        //                            currentState.ReachTarget = reachTarget;

        //                            if (s.SCCIndex >= 0)
        //                            {
        //                                scc2input[s.SCCIndex].Add(s);
        //                            }

        //                        }
        //                        list.RemoveAt(k);
        //                    }
        //                }
        //            }
        //        }
        //        else
        //        {
        //            //int currentDistriIndex = -1;
        //            //Distribution newDis = new Distribution(Constants.TAU);
        //            reachTarget = false;
        //            IEnumerable<DTMCConfiguration> steps = evt.MakeOneMoveLocal();

        //            //NOTE: here we play a trick for deadlock case: if a deadlock exist in the MDP, we will make a
        //            //self loop transition to remove the deadlock. Deadlock is meaningless in MDP.
        //            if (evt.IsDeadLock)
        //            {
        //                //List<DTMCConfiguration> stepsList = new List<DTMCConfiguration>(steps);
        //                working.Pop();
        //                dtmcSCCstates.Add(currentID);
        //                if (!lowlink.ContainsKey(currentID))
        //                {
        //                    lowlink.Add(currentID, preorderCounter);
        //                }
        //                continue;
        //            }

        //            //List<PCSPEventDRAPair> product = Next(steps, DRAState);
        //            List<DTMCConfiguration> Steps = new List<DTMCConfiguration>(steps);
        //            this.VerificationOutput.Transitions += Steps.Count;

        //            for (int k = Steps.Count - 1; k >= 0; k--)
        //            {
        //                DTMCConfiguration step = Steps[k];
        //                string tmp = step.GetID();

        //                bool target = false;
        //                ExpressionValue v = EvaluatorDenotational.Evaluate(ReachableStateCondition, step.GlobalEnv);

        //                if ((v as BoolConstant).Value)
        //                {
        //                    target = true;
        //                    if (!preorder.ContainsKey(tmp))
        //                    {
        //                        dtmcSCCstates.Add(tmp);
        //                        preorder.Add(tmp, preorderCounter);
        //                        lowlink.Add(tmp, preorderCounter);
        //                        preorderCounter++;
        //                    }
        //                }

        //                //int nextIndex = VisitedWithID.Count;
        //                DTMCState nextState;

        //                if (dtmc.States.TryGetValue(tmp, out nextState))
        //                {
        //                    if (!target && !preorder.ContainsKey(tmp))
        //                    {
        //                        if (done)
        //                        {
        //                            working.Push(new KeyValuePair<DTMCConfiguration, DTMCState>(step, nextState));
        //                            done = false;
        //                            Steps.RemoveAt(k);
        //                        }
        //                        else
        //                        {
        //                            Steps[k] = step;
        //                        }
        //                    }
        //                    else
        //                    {
        //                        Steps.RemoveAt(k);
        //                        DTMCState s = dtmc.States[tmp];
        //                        if (s.ReachTarget)
        //                        {
        //                            reachTarget = true;
        //                            currentState.ReachTarget = reachTarget;
        //                            if (s.SCCIndex >= 0)
        //                            {
        //                                scc2input[s.SCCIndex].Add(s);
        //                            }
        //                        }
        //                    }

        //                }
        //                else
        //                {
        //                    nextState = new DTMCState(tmp);
        //                    dtmc.States.Add(tmp, nextState);

        //                    if (done)
        //                    {
        //                        if (target)
        //                        {
        //                            dtmc.AddTargetStates(nextState);
        //                            reachTarget = true;
        //                        }
        //                        else
        //                        {
        //                            working.Push(new KeyValuePair<DTMCConfiguration, DTMCState>(step, nextState));
        //                        }

        //                        done = false;
        //                        Steps.RemoveAt(k);
        //                    }
        //                    else
        //                    {
        //                        if (target)
        //                        {
        //                            Steps.RemoveAt(k);
        //                            dtmc.AddTargetStates(nextState);
        //                            reachTarget = true;
        //                        }
        //                        else
        //                        {
        //                            Steps[k] = step;
        //                        }
        //                    }

        //                }

        //                //DTMCConfiguration pstep = step;
        //                currentState.Transitions.Add(new KeyValuePair<double, DTMCState>(step.Probability, nextState));

        //                //currentState.AddTransition(new KeyValuePair<double, DTMCState>(step.Probability, nextState));

        //            }
        //            ExpendedNode.Add(currentID, Steps);
        //        }

        //        if (done)
        //        {
        //            int lowlinkV = preorder[currentID];
        //            int preorderV = preorder[currentID];

        //            bool selfLoop = false;
        //            foreach (KeyValuePair<double, DTMCState> state in Transitions)
        //            {
        //                string w = state.Value.ID;

        //                if (w == currentID)
        //                {
        //                    selfLoop = true;
        //                }

        //                if (!dtmcSCCstates.Contains(w))
        //                {
        //                    if (preorder[w] > preorderV)
        //                    {
        //                        lowlinkV = Math.Min(lowlinkV, lowlink[w]);
        //                    }
        //                    else
        //                    {
        //                        lowlinkV = Math.Min(lowlinkV, preorder[w]);
        //                    }
        //                }
        //            }


        //            lowlink[currentID] = lowlinkV;
        //            working.Pop();

        //            if (lowlinkV == preorderV)
        //            {
        //                if (currentState.ReachTarget)
        //                {
        //                    HashSet<DTMCState> SCC = new HashSet<DTMCState>();
        //                    List<HashSet<DTMCState>> Groupings = new List<HashSet<DTMCState>>();
        //                    HashSet<DTMCState> Grouping = new HashSet<DTMCState>();
        //                    //bool reduceScc = false;
        //                    Grouping.Add(currentState);
        //                    SCC.Add(currentState);
        //                    int count = 1;
        //                    dtmcSCCstates.Add(currentID);
        //                    while (stepStack.Count > 0 && preorder[stepStack.Peek().ID] > preorderV && count < SCCBound)
        //                    {
        //                        DTMCState s = stepStack.Pop();
        //                        s.ReachTarget = true;
        //                        string sID = s.ID;
        //                        Grouping.Add(s);
        //                        SCC.Add(s);
        //                        dtmcSCCstates.Add(sID);
        //                        count++;
        //                    }
        //                    Groupings.Add(Grouping);

        //                    while (stepStack.Count > 0 && preorder[stepStack.Peek().ID] > preorderV)
        //                    {
        //                        count = 0;
        //                        Grouping = new HashSet<DTMCState>();
        //                        while (stepStack.Count > 0 && preorder[stepStack.Peek().ID] > preorderV && count < SCCBound)
        //                        {
        //                            DTMCState s = stepStack.Pop();
        //                            s.ReachTarget = true;
        //                            string sID = s.ID;
        //                            Grouping.Add(s);
        //                            SCC.Add(s);
        //                            dtmcSCCstates.Add(sID);
        //                            count++;
        //                        }
        //                        Groupings.Add(Grouping);
        //                    }

        //                    if (SCC.Count > 1 || selfLoop)
        //                    {
        //                        if (Groupings.Count > 1)
        //                        {
        //                            SCCs.Add(SCC);
        //                            int Count = SCCs.Count;
        //                            foreach (var state in SCC)
        //                            {
        //                                state.SCCIndex = Count - 1;
        //                            }
        //                            HashSet<DTMCState> inputs = new HashSet<DTMCState>();
        //                            inputs.Add(currentState);
        //                            scc2input.Add(currentState.SCCIndex, inputs);
        //                            HashSet<DTMCState> outofScc = new HashSet<DTMCState>();

        //                            //GL: added for multicores process
        //                            //List<List<DTMCState>> groups = new List<List<DTMCState>>();
        //                            var localMapSCCsWithOutput = new Dictionary<List<DTMCState>, List<DTMCState>>();
        //                            foreach (var Group in Groupings)
        //                            {
        //                                if (Group.Count > 1 || selfLoop)
        //                                {
        //                                    HashSet<DTMCState> outputs = new HashSet<DTMCState>();

        //                                    //input.Add(currentState);
        //                                    foreach (DTMCState state in Group)
        //                                    {
        //                                        foreach (KeyValuePair<double, DTMCState> KVpair in state.Transitions)
        //                                        {
        //                                            DTMCState nextState = KVpair.Value;
        //                                            if (!Group.Contains(nextState))
        //                                            {
        //                                                outputs.Add(nextState);
        //                                                if (!SCC.Contains(nextState))
        //                                                {
        //                                                    outofScc.Add(nextState);
        //                                                }
        //                                            }
        //                                        }
        //                                    }
        //                                    //CUTs.Add(Group);
        //                                    //List<DTMCState> group = new List<DTMCState>(Group);
        //                                    //List<DTMCState> output = new List<DTMCState>(outputs);
        //                                    //SCCReductionMatlab(scc, output);
        //                                    //SCCReduction(group, output);
        //                                    //groups.Add(group);
        //                                    localMapSCCsWithOutput.Add(new List<DTMCState>(Group), new List<DTMCState>(outputs));
        //                                }
        //                            }

        //                            //Parallel.ForEach(groups, subscc => SCCReduction(new List<DTMCState>(subscc), new List<DTMCState>(localMapSCCsWithOutput[subscc])));
        //                            Parallel.ForEach(localMapSCCsWithOutput, subscc2out => SCCReduction(subscc2out.Key, subscc2out.Value));

        //                            List<DTMCState> outofscc = new List<DTMCState>(outofScc);
        //                            scc2out.Add(SCCs.IndexOf(SCC), outofscc);
        //                        }
        //                        else
        //                        {
        //                            HashSet<DTMCState> outputs = new HashSet<DTMCState>();

        //                            //input.Add(currentState);
        //                            foreach (DTMCState state in SCC)
        //                            {
        //                                foreach (KeyValuePair<double, DTMCState> KVpair in state.Transitions)
        //                                {
        //                                    if (!SCC.Contains(KVpair.Value))
        //                                    {
        //                                        outputs.Add(KVpair.Value);
        //                                    }
        //                                }
        //                            }
        //                            //SCCs.Add(SCC);
        //                            List<DTMCState> scc = new List<DTMCState>(SCC);
        //                            //SCCReductionMatlab(scc, output);
        //                            //SCCReduction(scc, output);
        //                            //List<DTMCState> output = new List<DTMCState>(outputs);
        //                            //SCCReductionMatlab(scc, output);
        //                            //SCCReduction(group, output);
        //                            //groupsSmallSCCs.Add(scc);
        //                            MapSmallSCCsWithOutput.Add(scc, new List<DTMCState>(outputs));
        //                        }
        //                    }


        //                }
        //                else
        //                {
        //                    dtmcSCCstates.Add(currentID);
        //                    while (stepStack.Count > 0 && preorder[stepStack.Peek().ID] > preorderV)
        //                    {
        //                        string sID = stepStack.Pop().ID;
        //                        dtmcSCCstates.Add(sID);
        //                    }
        //                }

        //            }
        //            else
        //            {
        //                stepStack.Push(pair.Value);
        //            }
        //        }

        //    } while (working.Count > 0);

        //    //Parallel.ForEach(groupsSmallSCCs, scc => SCCReduction(new List<DTMCState>(scc), new List<DTMCState>(MapSmallSCCsWithOutput[scc])));
        //    Parallel.ForEach(MapSmallSCCsWithOutput, sccTOout => SCCReduction(sccTOout.Key, sccTOout.Value));
        //    //bool reduced = true;
        //    while (SCCs.Count > 0)// && reduced)
        //    {
        //        //reduced = false;
        //        for (int i = SCCs.Count - 1; i >= 0; i--)
        //        {
        //            int index = 0;
        //            foreach (var state in SCCs[i])
        //            {
        //                index = state.SCCIndex;
        //                break;
        //            }
        //            HashSet<DTMCState> Working = scc2input[index];
        //            HashSet<DTMCState> visited = new HashSet<DTMCState>();
        //            do
        //            {
        //                visited.UnionWith(Working);
        //                HashSet<DTMCState> newWorking = new HashSet<DTMCState>();
        //                foreach (var state in Working)
        //                {
        //                    foreach (var tran in state.Transitions)
        //                    {
        //                        DTMCState tranV = tran.Value;
        //                        if (!visited.Contains(tranV) && SCCs[i].Contains(tranV))
        //                        {
        //                            newWorking.Add(tranV);
        //                        }
        //                    }
        //                }
        //                Working = newWorking;

        //            } while (Working.Count > 0);



        //            if (visited.Count > SCCBound && visited.Count < SCCs[i].Count)//note: bug for second round cut
        //            {
        //                //reduced = true;
        //                SCCs[i] = visited;
        //                //note cut again
        //                List<DTMCState> SCCi = new List<DTMCState>(SCCs[i]);
        //                int Counter = 0;

        //                //GL: added for multicores process                                    
        //                //List<List<DTMCState>> groups = new List<List<DTMCState>>();
        //                var localMapSCCsWithOutput = new Dictionary<List<DTMCState>, List<DTMCState>>();

        //                while (Counter < SCCi.Count - SCCBound)
        //                {
        //                    List<DTMCState> group = SCCi.GetRange(Counter, SCCBound);
        //                    HashSet<DTMCState> outputs = new HashSet<DTMCState>();

        //                    //input.Add(currentState);
        //                    foreach (DTMCState state in group)
        //                    {
        //                        foreach (KeyValuePair<double, DTMCState> KVpair in state.Transitions)
        //                        {
        //                            DTMCState nextState = KVpair.Value;
        //                            if (!group.Contains(nextState))
        //                            {
        //                                outputs.Add(nextState);
        //                            }
        //                        }
        //                    }


        //                    //SCCReductionMatlab(scc, output);
        //                    //SCCReduction(group, output);
        //                    //groups.Add(group);
        //                    localMapSCCsWithOutput.Add(group, new List<DTMCState>(outputs));
        //                    Counter += SCCBound;
        //                }
        //                List<DTMCState> Group = SCCi.GetRange(Counter, SCCi.Count - Counter);
        //                HashSet<DTMCState> Outputs = new HashSet<DTMCState>();

        //                //input.Add(currentState);
        //                foreach (DTMCState state in Group)
        //                {
        //                    foreach (KeyValuePair<double, DTMCState> KVpair in state.Transitions)
        //                    {
        //                        DTMCState nextState = KVpair.Value;
        //                        if (!Group.Contains(nextState))
        //                        {
        //                            Outputs.Add(nextState);
        //                        }
        //                    }
        //                }

        //                //groups.Add(Group);
        //                localMapSCCsWithOutput.Add(Group, new List<DTMCState>(Outputs));

        //                //GL: Multicores calculation
        //                //Parallel.ForEach(groups, subscc => SCCReduction(new List<DTMCState>(subscc), new List<DTMCState>(localMapSCCsWithOutput[subscc])));
        //                Parallel.ForEach(localMapSCCsWithOutput, subscc2out => SCCReduction(subscc2out.Key, subscc2out.Value));

        //            }
        //            else
        //            {
        //                List<DTMCState> scc = new List<DTMCState>(visited);
        //                SCCReduction(scc, scc2out[index]);
        //                SCCs.RemoveAt(i);
        //            }
        //        }
        //    }

        //    //HashSet<DTMCState> nonSafe = RemoveUselessSCC(dtmc, SCCs);
        //    VerificationOutput.NoOfStates = VerificationOutput.NoOfStates + dtmc.States.Count;
        //    int counter = 0;
        //    BuildPRE(dtmc, ref counter);
        //    //for (int i = 0; i < SCCs.Count; i++)
        //    //{
        //    //    List<DTMCState> scc = new List<DTMCState>();
        //    //    foreach(var state in SCCs[i])
        //    //    {
        //    //        if(state.Pre.Count > 0)
        //    //        {
        //    //            scc.Add(state);
        //    //        }
        //    //    }
        //    //    SCCReduction_Pre(scc, scc2out[i]);
        //    //}

        //    return dtmc;

        //}

        protected DTMC BuildDTMC()//note here we assume the model is DTMC
        {
            Stopwatch BuildWatch = new Stopwatch();
            BuildWatch.Start();
            Stack<KeyValuePair<DTMCConfiguration, DTMCState>> working = new Stack<KeyValuePair<DTMCConfiguration, DTMCState>>(1024);

            string initID = InitialStep.GetID();
            DTMCState init = new DTMCState(initID);
            working.Push(new KeyValuePair<DTMCConfiguration, DTMCState>(InitialStep as DTMCConfiguration, init));
            DTMC dtmc = new DTMC(init, Ultility.Ultility.DEFAULT_PRECISION, Ultility.Ultility.MAX_DIFFERENCE);

            do
            {
                if (CancelRequested)
                {
                    VerificationOutput.NoOfStates = dtmc.States.Count;
                    return dtmc;
                }

                KeyValuePair<DTMCConfiguration, DTMCState> current = working.Pop();
                IEnumerable<DTMCConfiguration> List = current.Key.MakeOneMoveLocal();
                List<DTMCConfiguration> list = new List<DTMCConfiguration>(List);

                if (list.Count == 0) continue;

                VerificationOutput.Transitions += list.Count;

                double sum = 0;
                for (int k = list.Count - 1; k >= 0; k--)
                {
                    DTMCConfiguration step = list[k];
                    sum += step.Probability;
                }
                if (sum > 1.5)
                {
                    VerificationOutput.NonDeterminismInDTMC = true;
                }
                for (int i = 0; i < list.Count; i++)
                {

                    DTMCConfiguration step = list[i];
                    string stepID = step.GetID();
                    DTMCState nextState;

                    if (!dtmc.States.TryGetValue(stepID, out nextState))
                    {
                        nextState = new DTMCState(stepID);
                        dtmc.AddState(nextState);
                        //visited.Add(stepID, nextState);

                        ExpressionValue v = EvaluatorDenotational.Evaluate(ReachableStateCondition, step.GlobalEnv);

                        if ((v as BoolConstant).Value)
                        {
                            dtmc.AddTargetStates(nextState);
                        }
                        else
                        {
                            working.Push(new KeyValuePair<DTMCConfiguration, DTMCState>(step, nextState));
                        }
                    }

                    current.Value.AddTransition(new KeyValuePair<double, DTMCState>(step.Probability / sum, nextState));

                }


            } while (working.Count > 0);
            VerificationOutput.NoOfStates = dtmc.States.Count;
            BuildTime = BuildWatch.Elapsed.TotalSeconds;
            return dtmc;

        }
        protected DTMC BuildDTMC_SCC_Cut()
        {
            Stopwatch BuildWatch = new Stopwatch();
            BuildWatch.Start();
            string initID = InitialStep.GetID();
            DTMCState init = new DTMCState(initID);

            DTMC dtmc = new DTMC(init, Ultility.Ultility.DEFAULT_PRECISION, Ultility.Ultility.MAX_DIFFERENCE);
            //if initial state is target
            ExpressionValue initV = EvaluatorDenotational.Evaluate(ReachableStateCondition, InitialStep.GlobalEnv);
            if ((initV as BoolConstant).Value)
            {
                dtmc.AddTargetStates(init);
                VerificationOutput.NoOfStates = 1;
                return dtmc;
            }
            //if initial state is target
            HashSet<string> dtmcSCCstates = new HashSet<string>();

            Stack<KeyValuePair<DTMCConfiguration, DTMCState>> working = new Stack<KeyValuePair<DTMCConfiguration, DTMCState>>(1024);
            Stack<DTMCState> stepStack = new Stack<DTMCState>();
            working.Push(new KeyValuePair<DTMCConfiguration, DTMCState>(InitialStep as DTMCConfiguration, init));
            List<HashSet<DTMCState>> SCCs = new List<HashSet<DTMCState>>(64);//note here SCCs is just used to store sccs which is cut;

            Dictionary<string, int> preorder = new Dictionary<string, int>(Common.Classes.Ultility.Ultility.MC_INITIAL_SIZE);
            Dictionary<string, int> lowlink = new Dictionary<string, int>(Common.Classes.Ultility.Ultility.MC_INITIAL_SIZE);

            int preorderCounter = 0;
            Dictionary<string, List<DTMCConfiguration>> ExpendedNode = new Dictionary<string, List<DTMCConfiguration>>();
            List<HashSet<DTMCState>> scc2out = new List<HashSet<DTMCState>>();
            List<HashSet<DTMCState>> scc2input = new List<HashSet<DTMCState>>();

            bool reachTarget = true;

            do
            {
                if (CancelRequested)
                {
                    this.VerificationOutput.NoOfStates = this.VerificationOutput.NoOfStates + dtmc.States.Count;
                    return dtmc;
                }

                KeyValuePair<DTMCConfiguration, DTMCState> pair = working.Peek();
                DTMCConfiguration evt = pair.Key;
                string currentID = evt.GetID();
                DTMCState currentState = pair.Value;
                //List<Distribution> outgoing = pair.Value.;

                if (!preorder.ContainsKey(currentID))
                {
                    preorder.Add(currentID, preorderCounter);
                    preorderCounter++;
                }

                List<KeyValuePair<double, DTMCState>> Transitions = currentState.Transitions;

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

                    List<DTMCConfiguration> list = ExpendedNode[currentID];
                    if (list.Count > 0)
                    {
                        for (int k = list.Count - 1; k >= 0; k--)
                        {
                            DTMCConfiguration step = list[k];

                            string stepID = step.GetID();
                            if (!preorder.ContainsKey(stepID))
                            {
                                if (done)
                                {
                                    working.Push(new KeyValuePair<DTMCConfiguration, DTMCState>(step, dtmc.States[stepID]));
                                    done = false;
                                    list.RemoveAt(k);
                                }
                            }
                            else
                            {
                                //reachTarget = dtmc.States[stepID].ReachTarget;
                                //currentState.ReachTarget = reachTarget;
                                DTMCState s = dtmc.States[stepID];
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
                                //DTMCState nextState = dtmc.States[stepID];
                                //if (Middles.Contains(nextState))
                                //{
                                //Middles.Remove(nextState);
                                //INPUTS.Add(nextState);
                                //HashSet<DTMCState> input = new HashSet<DTMCState>();
                                //input.Add(nextState);
                                //foreach(HashSet<DTMCState> scc in SCCs)
                                //{
                                //    if(scc.Contains(nextState))
                                //    {
                                //        HashSet<DTMCState> middle = new HashSet<DTMCState>();
                                //        foreach (var state in scc)
                                //        {
                                //            if(state.ID != stepID)
                                //            {
                                //                middle.Add(state);
                                //            }
                                //        }
                                //        SCCReduction(input, middle, SCC2Outputs[scc]);
                                //        break;
                                //    }
                                //}
                                //}
                            }
                        }
                    }
                }
                else
                {
                    //int currentDistriIndex = -1;
                    //Distribution newDis = new Distribution(Constants.TAU);
                    reachTarget = false;
                    IEnumerable<DTMCConfiguration> steps = evt.MakeOneMoveLocal();

                    //NOTE: here we play a trick for deadlock case: if a deadlock exist in the MDP, we will make a
                    //self loop transition to remove the deadlock. Deadlock is meaningless in MDP.
                    if (evt.IsDeadLock)
                    {
                        //List<DTMCConfiguration> stepsList = new List<DTMCConfiguration>(steps);
                        working.Pop();
                        dtmcSCCstates.Add(currentID);
                        if (!lowlink.ContainsKey(currentID))
                        {
                            lowlink.Add(currentID, preorderCounter);
                        }
                        continue;
                        //stepsList.Add(CreateSelfLoopStep(evt));
                        //steps = stepsList.ToArray();
                        //HasDeadLock = true;
                    }

                    //List<PCSPEventDRAPair> product = Next(steps, DRAState);
                    List<DTMCConfiguration> Steps = new List<DTMCConfiguration>(steps);
                    double sum = 0;
                    this.VerificationOutput.Transitions += Steps.Count;

                    for (int k = Steps.Count - 1; k >= 0; k--)
                    {
                        DTMCConfiguration step = Steps[k];
                        sum += step.Probability;
                    }
                    if(sum > 1.5)
                    {
                        VerificationOutput.NonDeterminismInDTMC = true;
                    }

                    for (int k = Steps.Count - 1; k >= 0; k--)
                    {
                        
                        DTMCConfiguration step = Steps[k];
                        
                        string tmp = step.GetID();

                        bool target = false;
                        ExpressionValue v = EvaluatorDenotational.Evaluate(ReachableStateCondition, step.GlobalEnv);

                        if ((v as BoolConstant).Value)
                        {
                            target = true;
                            if (!preorder.ContainsKey(tmp))
                            {
                                dtmcSCCstates.Add(tmp);
                                preorder.Add(tmp, preorderCounter);
                                lowlink.Add(tmp, preorderCounter);
                                preorderCounter++;
                            }
                        }

                        //int nextIndex = VisitedWithID.Count;
                        DTMCState nextState;

                        if (dtmc.States.TryGetValue(tmp, out nextState))
                        {
                            if (!target && !preorder.ContainsKey(tmp))
                            {
                                if (done)
                                {
                                    working.Push(new KeyValuePair<DTMCConfiguration, DTMCState>(step, nextState));
                                    done = false;
                                    Steps.RemoveAt(k);
                                }
                                else
                                {
                                    Steps[k] = step;
                                }
                            }
                            else
                            {
                                Steps.RemoveAt(k);
                                DTMCState s = dtmc.States[tmp];
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
                            nextState = new DTMCState(tmp);
                            dtmc.States.Add(tmp, nextState);

                            if (done)
                            {
                                if (target)
                                {
                                    dtmc.AddTargetStates(nextState);
                                    reachTarget = true;
                                }
                                else
                                {
                                    working.Push(new KeyValuePair<DTMCConfiguration, DTMCState>(step, nextState));
                                }

                                done = false;
                                Steps.RemoveAt(k);
                            }
                            else
                            {
                                if (target)
                                {
                                    Steps.RemoveAt(k);
                                    dtmc.AddTargetStates(nextState);
                                    reachTarget = true;
                                }
                                else
                                {
                                    Steps[k] = step;
                                }
                            }

                        }

                        //DTMCConfiguration pstep = step;
                        currentState.Transitions.Add(new KeyValuePair<double, DTMCState>(step.Probability/sum, nextState));

                        //currentState.AddTransition(new KeyValuePair<double, DTMCState>(step.Probability, nextState));

                    }
                    ExpendedNode.Add(currentID, Steps);
                }

                if (done)
                {
                    if(currentID == "0$46")
                    {
                        Console.Write("");
                    }
                    int lowlinkV = preorder[currentID];
                    int preorderV = preorder[currentID];

                    bool selfLoop = false;
                    foreach (KeyValuePair<double, DTMCState> state in Transitions)
                    {
                        string w = state.Value.ID;

                        if (w == currentID)
                        {
                            selfLoop = true;
                        }

                        if (!dtmcSCCstates.Contains(w))
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


                    lowlink[currentID] = lowlinkV;
                    working.Pop();

                    if (lowlinkV == preorderV)
                    {
                        if (currentState.ReachTarget)
                        {
                            HashSet<DTMCState> SCC = new HashSet<DTMCState>();
                            //List<HashSet<DTMCState>> Groupings = new List<HashSet<DTMCState>>();
                            //HashSet<DTMCState> Grouping = new HashSet<DTMCState>();
                            //bool reduceScc = false;
                            //Grouping.Add(currentState);
                            SCC.Add(currentState);
                            //int count = 1;
                            dtmcSCCstates.Add(currentID);
                            while (stepStack.Count > 0 && preorder[stepStack.Peek().ID] > preorderV)
                            {
                                DTMCState s = stepStack.Pop();
                                s.ReachTarget = true;
                                string sID = s.ID;
                                //Grouping.Add(s);
                                SCC.Add(s);
                                dtmcSCCstates.Add(sID);
                                //count++;
                            }

                            if (SCC.Count > 1 || selfLoop)
                            {
                                //if (Groupings.Count > 1)
                                //{
                                SCCs.Add(SCC);
                                int Count = SCCs.Count;
                                foreach (var state in SCC)
                                {
                                    state.SCCIndex = Count - 1;
                                }
                                HashSet<DTMCState> inputs = new HashSet<DTMCState>();
                                inputs.Add(currentState);
                                scc2input.Add(inputs);
                                HashSet<DTMCState> outputs = new HashSet<DTMCState>();

                                //input.Add(currentState);
                                foreach (DTMCState state in SCC)
                                {
                                    foreach (var tran in state.Transitions)
                                    {
                                        if (!SCC.Contains(tran.Value))
                                        {
                                            outputs.Add(tran.Value);
                                        }
                                    }
                                }
                                scc2out.Add(outputs);
                            }


                        }
                        else
                        {
                            dtmcSCCstates.Add(currentID);
                            while (stepStack.Count > 0 && preorder[stepStack.Peek().ID] > preorderV)
                            {
                                string sID = stepStack.Pop().ID;
                                dtmcSCCstates.Add(sID);
                            }
                        }

                    }
                    else
                    {
                        stepStack.Push(pair.Value);
                    }
                }

            } while (working.Count > 0);

            //RemoveTrivial(SCCs, scc2input);
            //DFS_CutAutoAdjustTreeLike(SCCs, scc2out, scc2input);
           DFS_CutAutoAdjust(SCCs, scc2out, scc2input);
            //DFS_Cut(SCCs, scc2out, scc2input);
            //DFS_CutAutoAdjust_MultiCores(SCCs, scc2out, scc2input);

            VerificationOutput.NoOfStates = VerificationOutput.NoOfStates + dtmc.States.Count;
            int counter = 0;
            BuildPRE(dtmc, ref counter);
            BuildTime = BuildWatch.Elapsed.TotalSeconds;
            return dtmc;
            
        }

        protected void DFS_Cut(List<HashSet<DTMCState>> SCCs, List<HashSet<DTMCState>> scc2out, List<HashSet<DTMCState>> scc2input)
        {
            //const int SCCBound = 100;
            int counter = 0;
            //List<List<MDPState>> SCCsList = new List<List<MDPState>>(SCCs.Count);
            for (int i = SCCs.Count - 1; i >= 0; )
            {
                //if ((double)scc2input[i].Count/SCCs[i].Count > 0.5)//if there are too many inputs, then solve it directly
                //{
                //    i--;
                //    continue;
                //}
                List<DTMCState> SCC = new List<DTMCState>(SCCs[i]);//[i];
                if (SCC.Count <= SCCBound)
                {
                    if(Sparse)
                    {
                        SCCReductionGaussianSparseMatrix(SCC, new List<DTMCState>(scc2out[i]));
                    }
                    else
                    {
                        SCCReductionGaussian(SCC, new List<DTMCState>(scc2out[i]));
                    }
                    //SCCs.RemoveAt(i);
                    i--;
                }
                else
                {
                    int Counter = SCC.Count;
                    while (Counter > 0)
                    {
                        List<DTMCState> group;
                        if (SCCBound > Counter)
                        {
                            group = SCC.GetRange(SCC.Count - Counter, Counter);
                        }
                        else
                        {
                            group = SCC.GetRange(SCC.Count - Counter, SCCBound);
                        }

                        HashSet<DTMCState> outputs = new HashSet<DTMCState>();

                        //input.Add(currentState);
                        foreach (DTMCState state in group)
                        {
                            foreach (var tran in state.Transitions)
                            {

                                DTMCState nextState = tran.Value;
                                if (!group.Contains(nextState))
                                {
                                    outputs.Add(nextState);
                                }

                            }
                        }

                        List<DTMCState> output = new List<DTMCState>(outputs);
                        if (Sparse)
                        {
                            SCCReductionGaussianSparseMatrix(group, output);
                        }
                        else
                        {
                            SCCReductionGaussian(group, output);
                        }
                        counter++;
                        //SCCReduction(group, output);
                        Counter -= SCCBound;
                    }

                    HashSet<DTMCState> visited = new HashSet<DTMCState>();
                    //RandomLabel(scc2input[i], visited, SCCs[i]);
                    DFSLabel(scc2input[i], visited, scc2out[i], false);

                    if (visited.Count > scc2input[i].Count && visited.Count < SCCs[i].Count)
                    {
                        SCCs[i] = visited;
                    }
                    else if (visited.Count > scc2input[i].Count && visited.Count == SCCs[i].Count)
                    {
                        SCCs[i] = visited;
                        SCCBound = SCCBound * SCCBound;
                    }
                    else
                    {
                        List<DTMCState> scc = new List<DTMCState>(visited);
                        if(Sparse)
                        {
                            SCCReductionGaussianSparseMatrix(scc, new List<DTMCState>(scc2out[i]));
                        }
                        else
                        {
                            SCCReductionGaussian(scc, new List<DTMCState>(scc2out[i]));
                        }
                        
                        //SCCReductionMatlab(scc, scc2out[i]);
                        i--;
                        SCCBound = 100;
                        //#if DEBUG
                        // LinearEquationsSolver.ToDot(scc, Counter, countforGraph++);
                        //#endif
                    }
                }
            }

        }

        protected void DFS_CutAutoAdjust(List<HashSet<DTMCState>> SCCs, List<HashSet<DTMCState>> scc2out, List<HashSet<DTMCState>> scc2input)
        {
            int countforGraph = 0;
            //const int SCCBound = 50;
            for (int i = SCCs.Count - 1; i >= 0; )
            {
                //if ((double)scc2input[i].Count / SCCs[i].Count > 0.3)
                //{
                //    i--;
                //    continue;
                //}
                List<DTMCState> SCC = new List<DTMCState>(SCCs[i]);//[i];
                if (SCC.Count <= SCCBound)
                {
                    if (Sparse)
                    {
                        SCCReductionGaussianSparseMatrix(SCC, new List<DTMCState>(scc2out[i]));
                    }
                    else
                    {
                        SCCReductionGaussian(SCC, new List<DTMCState>(scc2out[i]));
                    }
                    //SCCs.RemoveAt(i);
                    i--;
                }
                else
                {
                    //List<MDPState> SCCi = new List<MDPState>(SCCs[i]);
                    int Counter = SCC.Count;
                    while (Counter > 0)
                    {
                        List<DTMCState> group;
                        if (SCCBound > Counter)
                        {
                            group = SCC.GetRange(SCC.Count - Counter, Counter);
                            HashSet<DTMCState> outputs = new HashSet<DTMCState>();
                            //input.Add(currentState);
                            foreach (DTMCState state in group)
                            {
                                foreach (var tran in state.Transitions)
                                {

                                    DTMCState nextState = tran.Value;
                                    if (!group.Contains(nextState))
                                    {
                                        outputs.Add(nextState);
                                    }
                                }
                            }


                            List<DTMCState> output = new List<DTMCState>(outputs);
                            //#if DEBUG
                            //                            LinearEquationsSolver.ToDot(group, Counter, countforGraph++);
                            //#endif
                            if (Sparse)
                            {
                                SCCReductionGaussianSparseMatrix(group, output);
                            }
                            else
                            {
                                SCCReductionGaussian(group, output);
                            }
                            //#if DEBUG
                            //                            LinearEquationsSolver.ToDot(group, Counter, countforGraph++);
                            //#endif
                            //SCCReduction(group, output);
                            Counter -= SCCBound;
                        }
                        else
                        {
                            group = SCC.GetRange(SCC.Count - Counter, SCCBound);

                            HashSet<DTMCState> outputs = new HashSet<DTMCState>();
                            //input.Add(currentState);
                            foreach (DTMCState state in group)
                            {
                                foreach (var tran in state.Transitions)
                                {

                                    DTMCState nextState = tran.Value;
                                    if (!group.Contains(nextState))
                                    {
                                        outputs.Add(nextState);
                                    }

                                }
                            }
                            int outputNumber = outputs.Count;
                            Counter -= SCCBound;
                            int outputNumberMove = outputNumber;
                            //int stepsize = 0;
                            int moveCounter = 0;
                            List<DTMCState> group1 = new List<DTMCState>(group);
                            HashSet<DTMCState> outputs1 = new HashSet<DTMCState>(outputs);
                            const int HORIZON = 2; //put as system parameter
                            //while (Counter>0&outputNumber >= outputNumberMove)
                            //{
                            while (moveCounter < HORIZON && -Counter + moveCounter < 0)
                            {

                                DTMCState state = SCC[SCC.Count - Counter + moveCounter];
                                moveCounter++;
                                group1.Add(state);
                                if (outputs1.Contains(state))
                                {
                                    outputs1.Remove(state);
                                }
                                foreach (var tran in state.Transitions)
                                {

                                    DTMCState nextState = tran.Value;
                                    if (!group1.Contains(nextState))
                                    {
                                        outputs1.Add(nextState);
                                    }

                                }
                                outputNumberMove = outputs1.Count;
                                if (outputNumber >= outputNumberMove)
                                {

                                    Counter -= moveCounter;
                                    outputNumber = outputNumberMove;
                                    outputs = new HashSet<DTMCState>(outputs1);
                                    group = new List<DTMCState>(group1);

                                    moveCounter = 0;
                                }

                                //}

                            }


                            List<DTMCState> output = new List<DTMCState>(outputs);
                            //#if DEBUG
                            //                            LinearEquationsSolver.ToDot(group, Counter, countforGraph++);
                            //#endif
                            //SCCReductionGaussianSparseMatrix(group, output);
                            if (Sparse)
                            {
                                SCCReductionGaussianSparseMatrix(group, output);
                            }
                            else
                            {
                                SCCReductionGaussian(group, output);
                            }
                            //#if DEBUG
                            //                            LinearEquationsSolver.ToDot(group, Counter, countforGraph++);
                            //#endif
                            //SCCReduction(group, output);
                            //  Counter -= SCCBound;


                        }



                    }


                    HashSet<DTMCState> visited = new HashSet<DTMCState>();
                    //RandomLabel(scc2input[i], visited, SCCs[i]);
                    //the fourth parameter: true means use mixed bfs; false means don't use mixed bfs
                    DFSLabel(scc2input[i], visited, scc2out[i], false);

                    if (visited.Count > scc2input[i].Count && ((double)SCCs[i].Count - visited.Count) / SCCs[i].Count > 0.1)
                    {
                        SCCs[i] = visited;
                    }
                    //else if (visited.Count > scc2input[i].Count && visited.Count == SCCs[i].Count)
                    //{
                    //    SCCs[i] = visited;
                    //    SCCBound = SCCBound*SCCBound;
                    //}
                    else
                    {
                        List<DTMCState> scc = new List<DTMCState>(visited);
                        if(visited.Count < 200)
                        {
                            if (Sparse)
                            {
                                SCCReductionGaussianSparseMatrix(scc, new List<DTMCState>(scc2out[i]));
                            }
                            else
                            {
                                SCCReductionGaussian(scc, new List<DTMCState>(scc2out[i]));
                            }
                        }
                        

                        //SCCReductionMatlab(scc, scc2out[i]);
                        i--;
                        //SCCBound = 100;
                        //#if DEBUG
                        // LinearEquationsSolver.ToDot(scc, Counter, countforGraph++);
                        //#endif
                    }
                }
            }

        }

        protected void DFS_CutAutoAdjust_MultiCores(List<HashSet<DTMCState>> SCCs, List<HashSet<DTMCState>> scc2out, List<HashSet<DTMCState>> scc2input)
        {
            const int NUMBEROFMAXCORES = 14;
            int countforGraph = 0;
            //const int SCCBound = 2;
           
            var localMapSCCsWithOutputLarge = new Dictionary<List<DTMCState>, int>();

            var localMapSCCsWithOutputSmallSCC = new Dictionary<List<DTMCState>, List<DTMCState>>();
            for (int i = SCCs.Count - 1; i >= 0; )
            {
                //if (scc2input[i].Count > 50)
                //{
                //    i--;
                //    continue;
                //}
                var localMapSCCsWithOutputSmall = new Dictionary<List<DTMCState>, List<DTMCState>>();
                List<DTMCState> SCC = new List<DTMCState>(SCCs[i]);//[i];
                if (SCC.Count <= SCCBound)
                {
                    //SCCReductionGaussianSparseMatrix(SCC, new List<DTMCState>(scc2out[i]));
                    //SCCs.RemoveAt(i);
                    localMapSCCsWithOutputSmallSCC.Add(SCC, new List<DTMCState>(scc2out[i]));
                    i--;
                }
                else
                {
                    //List<MDPState> SCCi = new List<MDPState>(SCCs[i]);
                    int Counter = SCC.Count;
                    while (Counter > 0)
                    {
                        List<DTMCState> group;
                        if (SCCBound > Counter)
                        {
                            group = SCC.GetRange(SCC.Count - Counter, Counter);
                            HashSet<DTMCState> outputs = new HashSet<DTMCState>();
                            //input.Add(currentState);
                            foreach (DTMCState state in group)
                            {
                                foreach (var tran in state.Transitions)
                                {

                                    DTMCState nextState = tran.Value;
                                    if (!group.Contains(nextState))
                                    {
                                        outputs.Add(nextState);
                                    }
                                }
                            }


                            List<DTMCState> output = new List<DTMCState>(outputs);
                            //#if DEBUG
                            //                            LinearEquationsSolver.ToDot(group, Counter, countforGraph++);
                            //#endif
                            localMapSCCsWithOutputSmall.Add(group, output);
                            //#if DEBUG
                            //                            LinearEquationsSolver.ToDot(group, Counter, countforGraph++);
                            //#endif
                            //SCCReduction(group, output);
                            Counter -= SCCBound;
                        }
                        else
                        {
                            group = SCC.GetRange(SCC.Count - Counter, SCCBound);

                            HashSet<DTMCState> outputs = new HashSet<DTMCState>();
                            //input.Add(currentState);
                            foreach (DTMCState state in group)
                            {
                                foreach (var tran in state.Transitions)
                                {

                                    DTMCState nextState = tran.Value;
                                    if (!group.Contains(nextState))
                                    {
                                        outputs.Add(nextState);
                                    }

                                }
                            }
                            int outputNumber = outputs.Count;
                            Counter -= SCCBound;
                            int outputNumberMove = outputNumber;
                            //int stepsize = 0;
                            int moveCounter = 0;
                            List<DTMCState> group1 = new List<DTMCState>(group);
                            HashSet<DTMCState> outputs1 = new HashSet<DTMCState>(outputs);
                            const int HORIZON = 1; //put as system parameter
                            //while (Counter>0&outputNumber >= outputNumberMove)
                            //{
                            while (moveCounter < HORIZON && -Counter + moveCounter < 0)
                            {

                                DTMCState state = SCC[SCC.Count - Counter + moveCounter];
                                moveCounter++;
                                group1.Add(state);
                                if (outputs1.Contains(state))
                                {
                                    outputs1.Remove(state);
                                }
                                foreach (var tran in state.Transitions)
                                {

                                    DTMCState nextState = tran.Value;
                                    if (!group1.Contains(nextState))
                                    {
                                        outputs1.Add(nextState);
                                    }

                                }
                                outputNumberMove = outputs1.Count;
                                if (outputNumber >= outputNumberMove)
                                {
                                    outputNumber = outputNumberMove;
                                    Counter -= moveCounter;

                                    outputs = new HashSet<DTMCState>(outputs1);
                                    group = new List<DTMCState>(group1);
                                    moveCounter = 0;
                                }
                                //}

                            }


                            List<DTMCState> output = new List<DTMCState>(outputs);
                            //#if DEBUG
                            //                            LinearEquationsSolver.ToDot(group, Counter, countforGraph++);
                            //#endif
                            //SCCReductionGaussianSparseMatrix(group, output);
                            localMapSCCsWithOutputSmall.Add(group, output);
                            //#if DEBUG
                            //                            LinearEquationsSolver.ToDot(group, Counter, countforGraph++);
                            //#endif
                            //SCCReduction(group, output);
                            //  Counter -= SCCBound;


                        }
                    }

                    if(Sparse)
                    {
                        Parallel.ForEach(localMapSCCsWithOutputSmall, new ParallelOptions { MaxDegreeOfParallelism = NUMBEROFMAXCORES },
                                         subscc2Out =>
                                         SCCReductionGaussianSparseMatrix(subscc2Out.Key, subscc2Out.Value));
                    }
                    else
                    {
                        Parallel.ForEach(localMapSCCsWithOutputSmall, new ParallelOptions { MaxDegreeOfParallelism = NUMBEROFMAXCORES },
                                         subscc2Out =>
                                         SCCReductionGaussian(subscc2Out.Key, subscc2Out.Value));
                    }
                    


                    HashSet<DTMCState> visited = new HashSet<DTMCState>();
                    RandomLabel(scc2input[i], visited, SCCs[i]);
                    //the fourth parameter: true means use mixed bfs; false means don't use mixed bfs
                    //DFSLabel(scc2input[i], visited, scc2out[i], true);

                    if (visited.Count > SCCBound && visited.Count < SCCs[i].Count)
                    {
                        SCCs[i] = visited;
                    }
                    else
                    {
                        List<DTMCState> scc = new List<DTMCState>(visited);
                        //SCCReductionGaussianSparseMatrix(scc, new List<DTMCState>(scc2out[i]));
                        localMapSCCsWithOutputLarge.Add(scc, i);
                        //SCCReductionMatlab(scc, scc2out[i]);
                        i--;
                        //#if DEBUG
                        //                        LinearEquationsSolver.ToDot(scc, Counter, countforGraph++);
                        //#endif
                    }
                }
            }

            if(Sparse)
            {
                Parallel.ForEach(localMapSCCsWithOutputSmallSCC, new ParallelOptions { MaxDegreeOfParallelism = NUMBEROFMAXCORES },
                                        subscc2Out =>
                                        SCCReductionGaussianSparseMatrix(subscc2Out.Key, subscc2Out.Value));


                Parallel.ForEach(localMapSCCsWithOutputLarge, new ParallelOptions { MaxDegreeOfParallelism = NUMBEROFMAXCORES },
                                            subscc2Out =>
                                            SCCReductionGaussianSparseMatrix(subscc2Out.Key, new List<DTMCState>(scc2input[subscc2Out.Value])));
            }
            else
            {
                Parallel.ForEach(localMapSCCsWithOutputSmallSCC, new ParallelOptions { MaxDegreeOfParallelism = NUMBEROFMAXCORES },
                                        subscc2Out =>
                                        SCCReductionGaussian(subscc2Out.Key, subscc2Out.Value));


                Parallel.ForEach(localMapSCCsWithOutputLarge, new ParallelOptions { MaxDegreeOfParallelism = NUMBEROFMAXCORES },
                                            subscc2Out =>
                                            SCCReductionGaussian(subscc2Out.Key, new List<DTMCState>(scc2input[subscc2Out.Value])));
            }
            

        }



        protected void RandomLabel(HashSet<DTMCState> inputs, HashSet<DTMCState> visited, HashSet<DTMCState> SCC)
        {
            HashSet<DTMCState> Working = inputs;
            visited.UnionWith(Working);
            do
            {
                HashSet<DTMCState> newWorking = new HashSet<DTMCState>();
                foreach (DTMCState state in Working)
                {
                    foreach (var tran in state.Transitions)
                    {
                        DTMCState tranV = tran.Value;
                        if (!visited.Contains(tranV) && SCC.Contains(tranV))
                        {
                            visited.Add(tranV);
                            newWorking.Add(tranV);
                        }
                    }
                }

                Working = newWorking;

            } while (Working.Count > 0);
        }

        protected void DFSLabel(HashSet<DTMCState> inputs, HashSet<DTMCState> visited, HashSet<DTMCState> outputs, bool mixBFS)
        {

            HashSet<DTMCState> newVisited = new HashSet<DTMCState>();

            foreach (var Instate in inputs)
            {
                if (visited.Contains(Instate)) continue;
                Stack<DTMCState> working = new Stack<DTMCState>();
                Dictionary<string, List<DTMCState>> ExpendedNode = new Dictionary<string, List<DTMCState>>();
                working.Push(Instate);
                visited.Add(Instate);
                newVisited.Add(Instate);
                do
                {
                    DTMCState currentState = working.Peek();
                    string currentID = currentState.ID;
                    bool done = true;
                    if (ExpendedNode.ContainsKey(currentID))
                    {

                        List<DTMCState> list = ExpendedNode[currentID];
                        if (list.Count > 0)
                        {
                            for (int k = list.Count - 1; k >= 0; k--)
                            {
                                DTMCState step = list[k];
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
                        List<DTMCState> stepsList = new List<DTMCState>();
                        foreach (var tran in currentState.Transitions)
                        {
                            if (outputs.Contains(tran.Value)) continue;
                            stepsList.Add(tran.Value);
                        }

                        for (int k = stepsList.Count - 1; k >= 0; k--)
                        {
                            DTMCState nextState = stepsList[k];
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

        private void BuildPRE(DTMC dtmc, ref int counter)
        {
            HashSet<DTMCState> working = new HashSet<DTMCState>();
            HashSet<DTMCState> visited = new HashSet<DTMCState>();
            working.Add(dtmc.InitState);
            visited.Add(dtmc.InitState);
            do
            {
                HashSet<DTMCState> newWorking = new HashSet<DTMCState>();
                foreach (DTMCState state in working)
                {
                    counter++;
                    foreach (var tran in state.Transitions)
                    {

                        DTMCState tranV = tran.Value;
                        if (tranV.ReachTarget)
                        {
                            tranV.Pre.Add(state);
                            if (!visited.Contains(tranV))
                            {
                                newWorking.Add(tranV);
                                visited.Add(tranV);
                            }
                        }
                    }
                }
                working = newWorking;
            } while (working.Count > 0);

            return;
        }

        //private HashSet<DTMCState> RemoveUselessSCC(DTMC dtmc, List<HashSet<DTMCState>> SCCs)
        //{
        //    HashSet<DTMCState> working = new HashSet<DTMCState>(dtmc.TargetStates);
        //    HashSet<DTMCState> nonSafe = new HashSet<DTMCState>();

        //    do
        //    {
        //        HashSet<DTMCState> newWorking = new HashSet<DTMCState>();
        //        foreach (var state in working)
        //        {
        //            nonSafe.Add(state);
        //            foreach (var PreS in state.Pre)
        //            {
        //                if (!nonSafe.Contains(PreS))
        //                {
        //                    newWorking.Add(PreS);
        //                }
        //            }
        //        }
        //        working = newWorking;
        //    } while (working.Count > 0);

        //    for (int i = SCCs.Count - 1; i >= 0; i--)
        //    {
        //        var scc = SCCs[i];
        //        if (!scc.IsSubsetOf(nonSafe))
        //        {
        //            SCCs.Remove(scc);
        //        }
        //    }
        //    return nonSafe;
        //}

        //private void SCCReductionMatlab(List<DTMCState> SCC, List<DTMCState> outputs)
        //{
        //    //var num2DTMCState = new Dictionary<int, DTMCState>();
        //    //       double[,] a = LinearEquationsSolver.Read2Matrix(SCC, outputs);
        //    //       LinearEquationsSolver.GuassianJordanElimination(a);

        //    //use Matlab
        //    double[] b = LinearEquationsSolver.Read2MatrixUsedForMatlab(SCC, outputs);
        //    double[,] a = LinearEquationsSolver.UseMatlabSolveLinearEquation(SCC.Count, SCC.Count + outputs.Count, b);

        //    //update DTMC
        //    int r = a.GetLength(0);
        //    int c = a.GetLength(1);
        //    for (int i = 0; i < r; i++)
        //    {
        //        //var dis = new Distribution("abs");
        //        DTMCState state = SCC[i];
        //        state.Transitions.Clear();
        //        //state.Distributions = new List<Distribution>(); //Resign transitions
        //        //state.AddDistribution(dis););
        //        for (int j = SCC.Count; j < c; j++)
        //        {
        //            if (Math.Abs(a[i, j] - 0) > LinearEquationsSolver.EPSILON)
        //            {
        //                state.Transitions.Add(new KeyValuePair<double, DTMCState>(-a[i, j], outputs[j - r]));
        //                //state.AddTransition(new KeyValuePair<double, DTMCState>(-a[i, j], outputs[j - r]));
        //            }
        //        }
        //    }

        //}

        private void SCCReductionGaussian(List<DTMCState> SCC, List<DTMCState> outputs)
        {
            //var num2DTMCState = new Dictionary<int, DTMCState>();
            double[,] a = LinearEquationsSolver.Read2Matrix(SCC, outputs);
            LinearEquationsSolver.GaussianJordanElimination(a);

            //update DTMC
            int r = a.GetLength(0);
            int c = a.GetLength(1);
            for (int i = 0; i < r; i++)
            {
                //var dis = new Distribution("abs");
                DTMCState state = SCC[i];
                //state.RemoveTransitions();
                state.Transitions.Clear();
                //state.Distributions = new List<Distribution>(); //Resign transitions
                //state.AddDistribution(dis););)
                for (int j = SCC.Count; j < c; j++)
                {
                    if (Math.Abs(a[i, j] - 0) > LinearEquationsSolver.EPSILON)
                    {
                        state.Transitions.Add(new KeyValuePair<double, DTMCState>(-a[i, j], outputs[j - r]));
                    }
                }
            }

        }

        private void SCCReductionGaussianSparseMatrix(List<DTMCState> SCC, List<DTMCState> outputs)
        {
            if(outputs.Count==0)
            {
                foreach (DTMCState state in SCC)
                {
                    state.Transitions.Clear();
                    state.Transitions.Add(new KeyValuePair<double, DTMCState>(1,outputs[0]));
                }
                return;
            }
            var smatrix = LinearEquationsSolver.Read2MatrixToSparseMatrix(SCC, outputs);
            //Debug.WriteLine(smatrix);
            LinearEquationsSolver.GaussianJordenElimination(smatrix);
            int r = smatrix.Nrows;
            int c = smatrix.Ncols;
            for (int i = 0; i < r; i++)
            {
                //var dis = new Distribution("abs");
                DTMCState state = SCC[i];
                state.Transitions.Clear();
                Row workingRow = smatrix.Rows[i];
                int j = r;
               // int colValue = workingRow.col.BinarySearch(j);
                //while (colValue<0 && j<c)
                //{
                //    colValue=workingRow.col.BinarySearch(j);
                //    j++;
                //}
                 
                 //if (colValue >= 0)
                 //{
                     //for (int innerj = colValue; innerj < workingRow.col.Count; innerj++)
                for (int innerj = 0; innerj < workingRow.col.Count;innerj++ )
                {
                    double m = workingRow.val[innerj];
                    if (Math.Abs(m) > LinearEquationsSolver.EPSILON)
                    {
                        state.Transitions.Add(new KeyValuePair<double, DTMCState>(-m, outputs[workingRow.col[innerj] - r]));
                    }

                }
                 //}
            }
        }


        //private void SCCReduction(List<DTMCState> SCC, List<DTMCState> outputs)
        //{
        //    if (SCC.Count < BOUNDFORUSINGMATLAB)
        //    {
        //        SCCReductionGaussian(SCC, outputs);
        //        SCCReductionGaussianSparseMatrix(SCC, outputs);
        //    }
        //    else
        //    {
        //        SCCReductionMatlab(SCC, outputs);
        //    }
        //}

        //private void SCCReduction_Pre(List<DTMCState> SCC, List<DTMCState> outputs)
        //{
        //    //var num2DTMCState = new Dictionary<int, DTMCState>();
        //    double[,] a = LinearEquationsSolver.Read2Matrix(SCC, outputs);
        //    LinearEquationsSolver.GaussianJordanElimination(a);

        //    //update DTMC
        //    int r = a.GetLength(0);
        //    int c = a.GetLength(1);
        //    for (int i = 0; i < r; i++)
        //    {
        //        //var dis = new Distribution("abs");
        //        DTMCState state = SCC[i];
        //        state.RemoveTransitions();
        //        //state.Transitions.Clear();
        //        //state.Distributions = new List<Distribution>(); //Resign transitions
        //        //state.AddDistribution(dis););)
        //        for (int j = SCC.Count; j < c; j++)
        //        {
        //            if (Math.Abs(a[i, j] - 0) > LinearEquationsSolver.EPSILON)
        //            {
        //                //state.Transitions.Add(new KeyValuePair<double, DTMCState>(-a[i, j], outputs[j - r]));
        //                state.AddTransition(new KeyValuePair<double, DTMCState>(-a[i, j], outputs[j - r]));
        //            }
        //        }
        //    }

        //    return;
        //}

        

        //protected DTMC BuildDTMC_BFS()//note here we assume the model is DTMC
        //{
        //    Queue<KeyValuePair<DTMCConfiguration, DTMCState>> working = new Queue<KeyValuePair<MDPConfiguration, MDPState>>(1024);

        //    string initID = InitialStep.GetID();
        //    MDPState init = new MDPState(initID);
        //    working.Enqueue(new KeyValuePair<MDPConfiguration, MDPState>(InitialStep as MDPConfiguration, init));
        //    MDP mdp = new MDP(init, Ultility.Ultility.DEFAULT_PRECISION, Ultility.Ultility.MAX_DIFFERENCE);

        //    //Dictionary<string, MDPState> visited = new Dictionary<string, MDPState>();
        //    //visited.Add(initID, init);
        //    Dictionary<string, MDPState> representative = new Dictionary<string, MDPState>();
        //    MDPState AbsMDPState = init;
        //    Dictionary<string, List<MDPState>> sameDistr = new Dictionary<string, List<MDPState>>();

        //    do
        //    {
        //        if (CancelRequested)
        //        {
        //            VerificationOutput.NoOfStates = mdp.States.Count;
        //            return mdp;
        //        }

        //        KeyValuePair<MDPConfiguration, MDPState> current = working.Dequeue();
        //        MDPConfiguration[] list = current.Key.MakeOneMoveLocal();

        //        if (list.Length == 0) continue;

        //        VerificationOutput.Transitions += list.Length;

        //        if (mdp.States.ContainsValue(current.Value))
        //        {
        //            AbsMDPState = current.Value;
        //        }
        //        else if (representative.ContainsKey(current.Value.ID))//, out AbsMDPState))
        //        {
        //            AbsMDPState = representative[current.Value.ID];
        //        }

        //        if (list.Length > 1)
        //        {
        //            Distribution newDis = new Distribution(Constants.TAU);
        //            for (int i = 0; i < list.Length; i++)
        //            {

        //                MDPConfiguration step = list[i];
        //                string stepID = step.GetID();
        //                MDPState nextState;

        //                if (representative.TryGetValue(stepID, out nextState))
        //                {
        //                    newDis.AddProbStatePair(step.Probability, nextState);
        //                    continue;
        //                }
        //                if (!mdp.States.TryGetValue(stepID, out nextState))
        //                {
        //                    nextState = new MDPState(stepID);
        //                    mdp.AddState(nextState);
        //                    //visited.Add(stepID, nextState);

        //                    ExpressionValue v = EvaluatorDenotational.Evaluate(ReachableStateCondition, step.GlobalEnv);

        //                    if ((v as BoolConstant).Value)
        //                    {
        //                        mdp.AddTargetStates(nextState);
        //                    }
        //                    else
        //                    {
        //                        working.Enqueue(new KeyValuePair<MDPConfiguration, MDPState>(step, nextState));
        //                    }
        //                }

        //                newDis.AddProbStatePair(step.Probability, nextState);

        //            }


        //            AbsMDPState.AddDistribution(newDis);

        //        }
        //        else
        //        {
        //            string ID = AbsMDPState.ID;

        //            MDPConfiguration step = list[0];
        //            string stepID = step.GetID();
        //            MDPState nextState;
        //            if (mdp.States.TryGetValue(stepID, out nextState))
        //            {

        //                string id = nextState.ID;
        //                SameDistr(nextState, AbsMDPState, sameDistr, ID);

        //            }
        //            else if (!representative.TryGetValue(stepID, out nextState))
        //            {

        //                nextState = new MDPState(stepID);
        //                representative.Add(stepID, AbsMDPState);

        //                ExpressionValue v = EvaluatorDenotational.Evaluate(ReachableStateCondition, step.GlobalEnv);

        //                if ((v as BoolConstant).Value)
        //                {
        //                    mdp.AddTargetStates(AbsMDPState);
        //                }
        //                else
        //                {
        //                    working.Enqueue(new KeyValuePair<MDPConfiguration, MDPState>(step, nextState));
        //                }

        //            }
        //            else
        //            {
        //                if (nextState.CurrentProb == 1)
        //                {
        //                    mdp.AddTargetStates(AbsMDPState);
        //                }
        //                else
        //                {
        //                    //mdp.States[AbsMDPState.ID] = nextState;
        //                    //foreach (Distribution distr in nextState.Distributions)
        //                    //{
        //                    //    AbsMDPState.AddDistribution(distr);
        //                    //}

        //                    SameDistr(nextState, AbsMDPState, sameDistr, ID);
        //                }

        //            }
        //        }
        //    } while (working.Count > 0);

        //    foreach (var pair in sameDistr)
        //    {
        //        List<MDPState> list = pair.Value;

        //        MDPState State = mdp.States[pair.Key];
        //        foreach (var state in list)
        //        {
        //            foreach (Distribution distr in State.Distributions)
        //            {
        //                state.AddDistribution(distr);
        //            }
        //        }
        //    }

        //    VerificationOutput.NoOfStates = mdp.States.Count;
        //    //mdp.BackUpTargetStates();
        //    foreach (var pair in mdp.States)
        //    {
        //        Debug.WriteLine(pair.Key + "  " + pair.Value.ToString());
        //    }
        //    return mdp;
        //}

        //private void SameDistr(DTMCState nextState, DTMCState AbsDTMCState, Dictionary<string, List<DTMCState>> sameDistr, string ID)
        //{
        //    string id = nextState.ID;
        //    if (ID != id)
        //    {
        //        if (sameDistr.ContainsKey(ID))
        //        {
        //            if (sameDistr.ContainsKey(id))
        //            {
        //                sameDistr[id].Add(AbsDTMCState);
        //                foreach (var state in sameDistr[ID])
        //                {
        //                    sameDistr[id].Add(state);
        //                }
        //            }
        //            else
        //            {
        //                List<DTMCState> List = new List<DTMCState>();
        //                List.Add(AbsDTMCState);
        //                foreach (var state in sameDistr[ID])
        //                {
        //                    List.Add(state);
        //                }
        //                sameDistr.Add(id, List);

        //            }
        //            sameDistr.Remove(ID);

        //        }
        //        else
        //        {
        //            if (sameDistr.ContainsKey(id))
        //            {
        //                sameDistr[id].Add(AbsDTMCState);
        //            }
        //            else
        //            {
        //                List<DTMCState> List = new List<DTMCState>();
        //                List.Add(AbsDTMCState);
        //                sameDistr.Add(id, List);
        //            }

        //        }

        //    }
        //}

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
        protected DTMCConfiguration CreateSelfLoopStep(DTMCConfiguration evt)
        {
            return null;
        }
        public override string GetResultString()
        {
            if (ConstraintType == QueryConstraintType.NONE)
            {
                return base.GetResultString();
            }

            StringBuilder sb = new StringBuilder();

            sb.AppendLine(Constants.VERFICATION_RESULT_STRING);
            if(VerificationOutput.NonDeterminismInDTMC)
            {
                sb.AppendLine("Warning: local nondeterminism is detected and normalized in this DTMC model!");
                sb.AppendLine();
            }
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
            //sb.AppendLine("Admissible Behavior: " + SelectedBahaviorName);
            //sb.AppendLine("Search Engine: " + SelectedEngineName);
            //sb.AppendLine("System Abstraction: " + MustAbstract);
            //sb.AppendLine("Maximum difference threshold : " + Ultility.Ultility.MAX_DIFFERENCE);
            //sb.AppendLine("************************************");
            sb.AppendLine("Building DTMC costs: " + BuildTime + "s");
            sb.AppendLine("Verify DTMC costs: " + VerifyTime + "s");
            sb.AppendLine("************************************");

            sb.AppendLine();

            return sb.ToString();
        }

        //public override string StartingProcess
        //{
        //    get { return Process.ToString(); }
        //}
    }
}