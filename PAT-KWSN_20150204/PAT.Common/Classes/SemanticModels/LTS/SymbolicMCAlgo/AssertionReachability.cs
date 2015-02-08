using System.Collections.Generic;
using PAT.Common.Classes.CUDDLib;
using PAT.Common.Classes.Expressions;
using PAT.Common.Classes.Expressions.ExpressionClass;
using PAT.Common.Classes.ModuleInterface;
using PAT.Common.Classes.SemanticModels.LTS.BDD;

namespace PAT.Common.Classes.SemanticModels.LTS.Assertion
{
    public abstract partial class AssertionReachability 
    {
        public List<CUDDNode> traces = new List<CUDDNode>();

        /// <summary>
        /// Check whethere the goal can be reachable from the initial state of automataBDD
        /// [ REFS: traces, DEREFS: ]
        /// </summary>
        /// <param name="automataBDD"></param>
        /// <param name="goal"></param>
        /// <param name="model"></param>
        public void MC(AutomataBDD automataBDD, Expression goal, Model model)
        {
            //Clear the old data
            this.traces.Clear();

            ExpressionBDDEncoding goalBddEncoding = goal.TranslateBoolExpToBDD(model);

            ExpressionBDDEncoding initEncoding = automataBDD.initExpression.TranslateBoolExpToBDD(model);
            if (initEncoding.GuardDDs.Count == 0)
            {
                VerificationOutput.VerificationResult = VerificationResultType.INVALID;
            }
            else
            {
                CUDDNode initDD = CUDD.Function.Or(initEncoding.GuardDDs);
                CUDDNode goalDD = CUDD.Function.Or(goalBddEncoding.GuardDDs);

                CUDD.Ref(automataBDD.transitionBDD);
                List<CUDDNode> noEventTrans = CUDD.Abstract.ThereExists(automataBDD.transitionBDD, model.GetAllEventVars());

                bool reachable = model.Path(initDD, goalDD, noEventTrans, traces, SelectedEngineName, VerificationOutput.GenerateCounterExample);
                CUDD.Deref(noEventTrans);

                //
                CUDD.Deref(initDD, goalDD);

                VerificationOutput.VerificationResult = (reachable) ? VerificationResultType.VALID : VerificationResultType.INVALID;
            }
        }

        /// <summary>
        /// Check whethere the goal can be reachable from the initial state of automataBDD
        /// [ REFS: traces, DEREFS: ]
        /// </summary>
        /// <param name="automataBDD"></param>
        /// <param name="goal"></param>
        /// <param name="model"></param>
        public void MCForTA(AutomataBDD automataBDD, Expression goal, Model model)
        {
            //Clear the old data
            this.traces.Clear();

            ExpressionBDDEncoding goalBddEncoding = goal.TranslateBoolExpToBDD(model);

            ExpressionBDDEncoding initEncoding = automataBDD.initExpression.TranslateBoolExpToBDD(model);
            if (initEncoding.GuardDDs.Count == 0)
            {
                VerificationOutput.VerificationResult = VerificationResultType.INVALID;
            }
            else
            {
                CUDDNode initDD = CUDD.Function.Or(initEncoding.GuardDDs);
                CUDDNode goalDD = CUDD.Function.Or(goalBddEncoding.GuardDDs);

                CUDD.Ref(automataBDD.transitionBDD);
                List<CUDDNode> discreteTrans = CUDD.Abstract.ThereExists(automataBDD.transitionBDD, model.GetAllEventVars());

                CUDD.Ref(automataBDD.Ticks);
                List<CUDDNode> tickTrans = CUDD.Abstract.ThereExists(automataBDD.Ticks, model.GetAllEventVars());

                bool reachable = model.PathForTA(initDD, goalDD, discreteTrans, tickTrans, automataBDD.SimulationRel,
                                                 SelectedEngineName);
                
                CUDD.Deref(discreteTrans, tickTrans);
                CUDD.Deref(initDD, goalDD);

                VerificationOutput.VerificationResult = (reachable) ? VerificationResultType.VALID : VerificationResultType.INVALID;
            }
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
                    CUDD.Ref(this.traces[i], this.traces[i - 1]);
                    CUDDNode transitionTemp = CUDD.Function.And(this.traces[i - 1], encoder.model.SwapRowColVars(this.traces[i]));

                    CUDD.Ref(allTransitions);
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