using System.Collections.Generic;
using PAT.Common.Classes.CUDDLib;
using PAT.Common.Classes.Expressions;
using PAT.Common.Classes.Expressions.ExpressionClass;
using PAT.Common.Classes.ModuleInterface;
using PAT.Common.Classes.SemanticModels.LTS.BDD;
using PAT.Common.Classes.SemanticModels.TTS;

namespace PAT.Common.Classes.SemanticModels.LTS.Assertion
{
    public abstract partial class AssertionDeadLock 
    {
        List<CUDDNode> traces = new List<CUDDNode>();

        /// <summary>
        /// [ REFS: traces, DEREFS: ]
        /// </summary>
        /// <param name="automataBDD"></param>
        /// <param name="model"></param>
        public void MC(AutomataBDD automataBDD, Model model)
        {
            //Clear the old data
            this.traces.Clear();

            List<CUDDNode> allTransitions = new List<CUDDNode>(automataBDD.transitionBDD);

            CUDDNode deadlockGoadDD = GetDeadlockDD(allTransitions, model);
            ExpressionBDDEncoding initEncoding = automataBDD.initExpression.TranslateBoolExpToBDD(model);
            if (initEncoding.GuardDDs.Count == 0)
            {
                this.VerificationOutput.VerificationResult = VerificationResultType.VALID;
            }
            else
            {
                CUDD.Ref(automataBDD.transitionBDD);
                List<CUDDNode> notTerminateNoEventTrans = CUDD.Function.And(automataBDD.transitionBDD,
                                                                       CUDD.Function.Not(AutomataBDD.GetTerminationTransEncoding(model)));
                notTerminateNoEventTrans = CUDD.Abstract.ThereExists(notTerminateNoEventTrans, model.GetAllEventVars());

                bool reachable = model.Path(CUDD.Function.Or(initEncoding.GuardDDs), deadlockGoadDD, notTerminateNoEventTrans, traces, SelectedEngineName,
                                            VerificationOutput.GenerateCounterExample);

                CUDD.Deref(notTerminateNoEventTrans);

                this.VerificationOutput.VerificationResult = (reachable) ? VerificationResultType.INVALID : VerificationResultType.VALID;
            }
        }

        /// <summary>
        /// [ REFS: traces, DEREFS: ]
        /// </summary>
        /// <param name="automataBDD"></param>
        /// <param name="model"></param>
        public void MCForTA(AutomataBDD automataBDD, Model model)
        {
            //Clear the old data
            this.traces.Clear();

            List<CUDDNode> allTransitions = new List<CUDDNode>(automataBDD.transitionBDD);

            CUDDNode deadlockGoadDD = GetDeadlockDD(allTransitions, model);
            ExpressionBDDEncoding initEncoding = automataBDD.initExpression.TranslateBoolExpToBDD(model);
            if (initEncoding.GuardDDs.Count == 0)
            {
                this.VerificationOutput.VerificationResult = VerificationResultType.VALID;
            }
            else
            {
                CUDD.Ref(automataBDD.transitionBDD);
                List<CUDDNode> discreteTrans = CUDD.Function.And(automataBDD.transitionBDD,
                                                                       CUDD.Function.Not(AutomataBDD.GetTerminationTransEncoding(model)));
                discreteTrans = CUDD.Abstract.ThereExists(discreteTrans, model.GetAllEventVars());

                CUDD.Ref(automataBDD.Ticks);
                List<CUDDNode> tickTrans = CUDD.Function.And(automataBDD.Ticks,
                                                                       CUDD.Function.Not(AutomataBDD.GetTerminationTransEncoding(model)));
                tickTrans = CUDD.Abstract.ThereExists(tickTrans, model.GetAllEventVars());

                bool reachable = model.PathForTA(CUDD.Function.Or(initEncoding.GuardDDs), deadlockGoadDD, discreteTrans,
                                                 tickTrans, automataBDD.SimulationRel, SelectedEngineName);

                CUDD.Deref(discreteTrans, tickTrans);

                this.VerificationOutput.VerificationResult = (reachable) ? VerificationResultType.INVALID : VerificationResultType.VALID;
            }
        }

        /// <summary>
        /// Find the deadlock state. This state can be reached by a terminate event.
        /// Later termination transition is removed from the transition to finding path
        /// [ REFS: , DEREFS: ]
        /// </summary>
        /// <param name="transitions"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public static CUDDNode GetDeadlockDD(List<CUDDNode> transitions, Model model)
        {
            CUDDNode result = CUDD.Constant(1);
            
            //result contains state not having not tick outgoing transition
            foreach (CUDDNode transition in transitions)
            {
                CUDD.Ref(transition);
                CUDDNode notTickTrans = CUDD.Function.And(transition, CUDD.Function.Not(TimeBehaviors.GetTickTransEncoding(model)));

                //having not tick outgoing transition
                CUDDNode notDeadlockState = CUDD.Abstract.ThereExists(notTickTrans, model.AllColVars);

                CUDDNode deadlockState = CUDD.Function.Not(notDeadlockState);
                result = CUDD.Function.And(result, deadlockState);
            }

            CUDD.Ref(transitions);
            List<CUDDNode> tickTrans = CUDD.Function.And(transitions, TimeBehaviors.GetTickTransEncoding(model));

            CUDD.Ref(tickTrans);
            CUDDNode stateHasTickTrans = CUDD.Function.Or(CUDD.Abstract.ThereExists(tickTrans, model.AllColVars));

            List<int> eventIndex = model.GetEventIndex();
            for (int i = 0; i < model.GetNumberOfVars(); i++)
            {
                if(!eventIndex.Contains(i))
                {
                    CUDD.Ref(model.varIdentities[i]);
                    tickTrans = CUDD.Function.And(tickTrans, model.varIdentities[i]);
                }
            }
            CUDDNode stateHasLoopTick = CUDD.Function.Or(CUDD.Abstract.ThereExists(tickTrans, model.AllColVars));

            //Deadlock state: not having not tick transition and if has tick, then it must loop tick
            CUDD.Ref(result);
            result = CUDD.Function.Or(CUDD.Function.And(result, CUDD.Function.Not(stateHasTickTrans)), CUDD.Function.And(result, stateHasLoopTick));

            return result;
        }


        /// <summary>
        /// Generate counter example for reachability and deadlock
        ///  [ REFS: '', DEREFS: automataBDD.transitionBDD, automataBDD.priorityTransitionsBDD, this.traces ]
        /// </summary>
        /// <param name="automataBDD"></param>
        /// <param name="encoder"></param>
        /// <returns></returns>
        public void GetMCResult(AutomataBDD automataBDD, BDDEncoder encoder)
        {
            VerificationOutput.CounterExampleTrace.Add(InitialStep);

            if (this.traces.Count > 0)
            {
                ExpressionBDDEncoding initEncoding = automataBDD.initExpression.TranslateBoolExpToBDD(encoder.model);
                this.traces.Insert(0, CUDD.Function.Or(initEncoding.GuardDDs));

                Valuation currentValuation = this.InitialStep.GlobalEnv;
                Valuation lastValuation;

                List<CUDDNode> allTransitions = new List<CUDDNode>();
                allTransitions.AddRange(automataBDD.transitionBDD);
                allTransitions.AddRange(automataBDD.Ticks);

                for (int i = 1; i < this.traces.Count; i++)
                {
                    //Get event information
                    CUDD.Ref(allTransitions);
                    CUDD.Ref(this.traces[i], this.traces[i - 1]);
                    CUDDNode transitionTemp = CUDD.Function.And(this.traces[i - 1], encoder.model.SwapRowColVars(this.traces[i]));
                    CUDDNode transWithEventInfo = CUDD.Function.And(transitionTemp, allTransitions);

                    transWithEventInfo = CUDD.Abstract.ThereExists(transWithEventInfo, encoder.model.AllRowVarsExceptSingleCopy);
                    transWithEventInfo = CUDD.RestrictToFirst(transWithEventInfo, encoder.model.AllColVars);

                    lastValuation = currentValuation;
                    currentValuation = encoder.GetValuationFromBDD(transWithEventInfo, this.InitialStep.GlobalEnv);

                    string eventName = encoder.GetEventChannelName(lastValuation, currentValuation, transWithEventInfo);

                    VerificationOutput.CounterExampleTrace.Add(new ConfigurationBDD(eventName, currentValuation));

                    //
                    CUDD.Deref(transWithEventInfo);
                }
            }

            //
            CUDD.Deref(automataBDD.transitionBDD, automataBDD.Ticks, traces);

            //
            VerificationOutput.ActualMemoryUsage = CUDD.ReadMemoryInUse();
            VerificationOutput.numberOfBoolVars = encoder.model.NumberOfBoolVars;

            encoder.model.Close();
        }
    }
}