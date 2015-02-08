using System.Collections.Generic;
using PAT.Common.Classes.ModuleInterface;
using PAT.Common.Classes.Ultility;
using PAT.Common.Classes.SemanticModels.LTS.BDD;
using PAT.Common.Classes.CUDD;
using PAT.Common.Classes.Expressions.ExpressionClass;

namespace PAT.Common.Classes.SemanticModels.LTS.Assertion
{
    public abstract partial class AssertionLTL : AssertionBase
    {
        private CUDDNode eventMustHappen = CUDD.CUDD.Constant(0);
        private CUDDNode scc = CUDD.CUDD.Constant(0);

        /// <summary>
        /// Check whether scc is strong fairness.
        /// [ REFS: , DEREFS: ]
        /// </summary>
        /// <param name="scc"></param>
        /// <param name="intersection"></param>
        /// <param name="modelBDD"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        private bool IsStrongFairness(CUDDNode scc, AutomataBDD intersection, AutomataBDD modelBDD, Model model)
        {
            CUDD.CUDD.Ref(modelBDD.transitionBDD);
            CUDD.CUDD.Ref(scc);
            List<CUDDNode> transitionFromSCC = CUDD.CUDD.Function.And(modelBDD.transitionBDD, scc);

            CUDD.CUDD.Ref(intersection.transitionBDD);
            CUDD.CUDD.Ref(scc, scc);
            List<CUDDNode> transitionInSCC = CUDD.CUDD.Function.And(CUDD.CUDD.Function.And(intersection.transitionBDD, scc), model.SwapRowColVars(scc));

            CUDDNode eventHappened = model.GetEventRowInTransition(transitionInSCC);

            CUDDNode eventEnabled = model.GetEventRowInTransition(transitionFromSCC);

            bool isStrongFairness = CUDD.CUDD.IsSubSet(eventHappened, eventEnabled);
            if (isStrongFairness)
            {
                CUDD.CUDD.Ref(eventEnabled);
                CUDD.CUDD.Deref(this.eventMustHappen);
                this.eventMustHappen = eventEnabled;

                CUDD.CUDD.Ref(scc);
                CUDD.CUDD.Deref(this.scc);
                this.scc = scc;
            }

            //
            CUDD.CUDD.Deref(eventHappened, eventEnabled);

            return isStrongFairness;
        }


        /// <summary>
        /// Check whether scc is strong fairness.
        /// [ REFS: , DEREFS: ]
        /// </summary>
        /// <param name="scc"></param>
        /// <param name="intersection"></param>
        /// <param name="modelBDD"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        private bool IsWeakFairness(CUDDNode scc, AutomataBDD intersection, AutomataBDD modelBDD, Model model)
        {
            CUDD.CUDD.Ref(intersection.transitionBDD);
            CUDD.CUDD.Ref(scc, scc);
            List<CUDDNode> transitionInSCC = CUDD.CUDD.Function.And(CUDD.CUDD.Function.And(intersection.transitionBDD, scc), model.SwapRowColVars(scc));

            CUDDNode eventHappened = model.GetEventRowInTransition(transitionInSCC);

            //-----------------------------------
            CUDD.CUDD.Ref(modelBDD.transitionBDD);
            CUDD.CUDD.Ref(scc);
            List<CUDDNode> transitionFromSCC = CUDD.CUDD.Function.And(modelBDD.transitionBDD, scc);

            CUDD.CUDD.Ref(transitionFromSCC);
            CUDDNode eventEnabled = model.GetEventRowInTransition(transitionFromSCC);

            //Get all the combination of state in scc and event
            CUDD.CUDD.Ref(scc, eventEnabled);
            CUDDNode complementTransitionFromSCC = CUDD.CUDD.Function.And(scc, model.SwapRowColVars(eventEnabled));

            //
            CUDDVars varsTemp = new CUDDVars();
            varsTemp.AddVars(model.AllColVars);
            varsTemp.RemoveVars(model.GetEventColVars());
            //Get the source state (in scc) and event
            transitionFromSCC = CUDD.CUDD.Abstract.ThereExists(transitionFromSCC, varsTemp);

            complementTransitionFromSCC = CUDD.CUDD.Function.Different(complementTransitionFromSCC, CUDD.CUDD.Function.Or(transitionFromSCC));

            CUDDNode eventNotAllEnabled = model.GetEventRowInTransition(new List<CUDDNode> { complementTransitionFromSCC });

            CUDDNode eventAllEnabled = CUDD.CUDD.Function.Different(eventEnabled, eventNotAllEnabled);

            bool isWeakFairness = CUDD.CUDD.IsSubSet(eventHappened, eventAllEnabled);
            if (isWeakFairness)
            {
                //Must assign this.eventMushHappen to eventHappened to make sure that all enabled events will happen.
                //In some case though the eventAllEnable is empty but if this.evenMustHappen = eventAllEnable, the scc is not restricted
                //and a small part of this scc is explored, so in this small part, the eventAllEnable may be not empty.
                CUDD.CUDD.Ref(eventHappened);
                CUDD.CUDD.Deref(this.eventMustHappen);
                this.eventMustHappen = eventHappened;

                CUDD.CUDD.Ref(scc);
                CUDD.CUDD.Deref(this.scc);
                this.scc = scc;
            }

            //
            CUDD.CUDD.Deref(eventHappened, eventAllEnabled);

            return isWeakFairness;
        }

        /// <summary>
        /// Check whether a SCC is fairness
        /// [ REFS: , DEREFS: ]
        /// </summary>
        /// <param name="scc"></param>
        /// <param name="intersection"></param>
        /// <param name="modelBDD"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        private bool IsFairScc(CUDDNode scc, AutomataBDD intersection, AutomataBDD modelBDD, Model model)
        {
            return ((this.FairnessType == FairnessType.EVENT_LEVEL_STRONG_FAIRNESS && IsStrongFairness(scc, intersection, modelBDD, model)) ||
                   (this.FairnessType == FairnessType.EVENT_LEVEL_WEAK_FAIRNESS && IsWeakFairness(scc, intersection, modelBDD, model)));

        }

        /// <summary>
        /// Generate fair counter example
        /// [ REFS: '', DEREFS:'']
        /// </summary>
        /// <param name="finalState"></param>
        /// <param name="automataBDD"></param>
        /// <param name="model"></param>
        private void BuildFairCounterExample(CUDDNode finalState, AutomataBDD automataBDD, Model model)
        {
            CUDD.CUDD.Ref(this.transitionsNoEvents);
            CUDD.CUDD.Ref(this.scc, this.scc);
            List<CUDDNode> transInSCCNoEvent = CUDD.CUDD.Function.And(CUDD.CUDD.Function.And(this.transitionsNoEvents, this.scc), model.SwapRowColVars(this.scc));

            CUDD.CUDD.Ref(automataBDD.transitionBDD);
            CUDD.CUDD.Ref(this.scc, this.scc);
            List<CUDDNode> transInSCCWithEvent = CUDD.CUDD.Function.And(CUDD.CUDD.Function.And(automataBDD.transitionBDD, this.scc), model.SwapRowColVars(this.scc));

            //
            List<CUDDNode> path = new List<CUDDNode>();

            CUDD.CUDD.Ref(period[0], finalState);
            CUDDNode temp1 = CUDD.CUDD.Function.And(period[0], finalState);
            if (temp1.Equals(CUDD.CUDD.ZERO))
            {
                CUDD.CUDD.Ref(this.scc, finalState);
                CUDDNode acceptanceStateInCyle = CUDD.CUDD.Function.And(this.scc, finalState);
                model.Path(period[0], acceptanceStateInCyle, transInSCCWithEvent, path, true, true);
                period.AddRange(path);

                CUDD.CUDD.Deref(acceptanceStateInCyle);
            }

            //
            CUDD.CUDD.Deref(temp1);

            //
            BuildFairPeriod(transInSCCWithEvent, model);

            bool isEmptyPathAllowed = (period.Count == 1 ? false : true);
            model.Path(period[period.Count - 1], period[0], transInSCCNoEvent, path, isEmptyPathAllowed, false);
            period.AddRange(path);

            CUDD.CUDD.Deref(transInSCCNoEvent);
            CUDD.CUDD.Deref(transInSCCWithEvent);
            //Remove dummy value
            CUDD.CUDD.Deref(period[0]); period.RemoveAt(0);
        }

        /// <summary>
        /// Generate fair cycle
        /// [ REFS: '', DEREFS:'']
        /// </summary>
        /// <param name="trans"></param>
        /// <param name="model"></param>
        private void BuildFairPeriod(List<CUDDNode> transInSCCWithEvent, Model model)
        {
            do
            {
                CUDD.CUDD.Ref(period);
                CUDDNode eventHappened = GetEventInStates(model);

                CUDD.CUDD.Ref(this.eventMustHappen);
                CUDDNode eventNotHappend = CUDD.CUDD.Function.Different(this.eventMustHappen, eventHappened);

                if (eventNotHappend.Equals(CUDD.CUDD.ZERO))
                {
                    CUDD.CUDD.Deref(eventNotHappend);
                    return;
                }
                else
                {
                    List<CUDDNode> path = new List<CUDDNode>();
                    model.Path(period[period.Count - 1], eventNotHappend, transInSCCWithEvent, path, false, true);
                    period.AddRange(path);
                }

            } while (true);
        }

        /// <summary>
        /// Only find event from period[1]
        /// </summary>
        /// <param name="period"></param>
        /// <returns></returns>
        private CUDDNode GetEventInStates(Model model)
        {
            CUDDNode result = CUDD.CUDD.Constant(0);

            if (period.Count > 1)
            {
                CUDDVars temp = new CUDDVars();
                temp.AddVars(model.AllRowVars);
                temp.RemoveVars(model.GetEventRowVars());

                for (int i = 1; i < period.Count; i++)
                {
                    CUDD.CUDD.Ref(period[i]);
                    result = CUDD.CUDD.Function.Or(result, CUDD.CUDD.Abstract.ThereExists(period[i], temp));
                }
            }

            return result;
        }

        /// <summary>
        /// Run the model checking algorithm
        /// [ REFS: prefix, period, DEREFS: modelBDD.transitionBDD, this.scc, this.eventMustHappen]
        /// </summary>
        /// <param name="intersection"></param>
        /// <param name="modelBDD"></param>
        /// <param name="model"></param>
        public void FairMC(AutomataBDD intersection, AutomataBDD modelBDD, Model model)
        {
            ExpressionBDDEncoding initEncoding = intersection.initExpression.TranslateBoolExpToBDD(model);

            if (initEncoding.GuardDDs.Count == 0)
            {
                return;
            }

            ExpressionBDDEncoding finalStateEncoding = intersection.acceptanceExpression.TranslateBoolExpToBDD(model);
            if (finalStateEncoding.GuardDDs.Count == 0)
            {
                return;
            }

            CUDDNode initDD = CUDD.CUDD.Function.Or(initEncoding.GuardDDs);
            CUDDNode finalDD = CUDD.CUDD.Function.Or(finalStateEncoding.GuardDDs);

            CUDDNode finalStateWithNoEvent = CUDD.CUDD.Abstract.ThereExists(finalDD, model.GetAllEventVars());

            CUDD.CUDD.Ref(intersection.transitionBDD);
            this.transitionsNoEvents = CUDD.CUDD.Abstract.ThereExists(intersection.transitionBDD, model.GetAllEventVars());

            CUDDNode allSCCs = SCCHull(model, initDD, finalStateWithNoEvent);

            if (!allSCCs.Equals(CUDD.CUDD.ZERO))
            {
                //Pruning step: remove states whose predecessors are not belong to that set
                while (true)
                {
                    CUDD.CUDD.Ref(allSCCs, allSCCs);
                    CUDDNode temp = CUDD.CUDD.Function.And(allSCCs, model.Predecessors(allSCCs, this.transitionsNoEvents));

                    bool isSubSet = temp.Equals(allSCCs);
                    if (!isSubSet)
                    {
                        CUDD.CUDD.Deref(allSCCs);
                        allSCCs = temp;
                    }
                    else
                    {
                        CUDD.CUDD.Deref(temp);
                        break;
                    }

                }


                while (!allSCCs.Equals(CUDD.CUDD.ZERO))
                {
                    CUDD.CUDD.Ref(allSCCs);
                    CUDDNode notInSCC = CUDD.CUDD.Function.Not(allSCCs);

                    CUDD.CUDD.Ref(this.transitionsNoEvents);
                    CUDD.CUDD.Ref(this.transitionsNoEvents);
                    CUDD.CUDD.Ref(notInSCC);//Kill notInSCC
                    List<CUDDNode> transitionNotInSCC = new List<CUDDNode>();
                    transitionNotInSCC.AddRange(CUDD.CUDD.Function.And(this.transitionsNoEvents, notInSCC));
                    transitionNotInSCC.AddRange(CUDD.CUDD.Function.And(this.transitionsNoEvents, model.SwapRowColVars(notInSCC)));

                    List<CUDDNode> path = new List<CUDDNode>();
                    model.Path(initDD, allSCCs, transitionNotInSCC, path, true, false);
                    prefix.AddRange(path);

                    //
                    CUDD.CUDD.Deref(transitionNotInSCC);

                    CUDD.CUDD.Ref(allSCCs, allSCCs);
                    CUDD.CUDD.Ref(this.transitionsNoEvents);
                    //Transitions inside all SCCs
                    List<CUDDNode> R = CUDD.CUDD.Function.And(CUDD.CUDD.Function.And(this.transitionsNoEvents, allSCCs), model.SwapRowColVars(allSCCs));

                    CUDDNode startStateOfCycle = (prefix.Count == 0) ? initDD : prefix[prefix.Count - 1];
                    CUDD.CUDD.Ref(startStateOfCycle);

                    //Find the scc if exists
                    CUDDNode scc = model.SCC(startStateOfCycle, R);

                    //
                    CUDD.CUDD.Deref(R);

                    if (!scc.Equals(CUDD.CUDD.ZERO))
                    {
                        //a new scc is found
                        if (IsFairScc(scc, intersection, modelBDD, model))
                        {
                            //
                            CUDD.CUDD.Deref(modelBDD.transitionBDD);

                            this.period.Add(startStateOfCycle);
                            BuildFairCounterExample(finalStateWithNoEvent, intersection, model);
                            break;
                        }

                        //
                        allSCCs = CUDD.CUDD.Function.Different(allSCCs, scc);
                    }
                    else
                    {
                        //
                        allSCCs = CUDD.CUDD.Function.Different(allSCCs, startStateOfCycle);
                    }

                    CUDD.CUDD.Deref(prefix);
                    prefix.Clear();

                }
            }

            //
            CUDD.CUDD.Deref(initDD, finalStateWithNoEvent, allSCCs);
            CUDD.CUDD.Deref(this.transitionsNoEvents);
            CUDD.CUDD.Deref(this.eventMustHappen, this.scc);
        }
    }
}
