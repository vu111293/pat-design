using System.Collections.Generic;
using PAT.Common.Classes.ModuleInterface;
using PAT.Common.Classes.SemanticModels.LTS.BDD;
using PAT.Common.Classes.CUDDLib;
using PAT.Common.Classes.Expressions.ExpressionClass;
using PAT.Common.Classes.SemanticModels.TTS;

namespace PAT.Common.Classes.SemanticModels.LTS.Assertion
{
    public abstract partial class AssertionLTL: AssertionBase
    {
        public const string EVENT_DUMMY = "#event_dummy";

        /// <summary>
        /// Return strongly connected components
        /// Does not include Event variable
        /// this.transitionsNoEvents is the transtion from reachble states
        /// From Feasible algorithm in the article "LTL Symbolic MC"
        /// [ REFS: 'result', DEREFS:]
        /// </summary>
        /// <param name="model"></param>
        /// <param name="initState"></param>
        /// <param name="cycleMustPass">The cycle must pass all states in the cycleMustPass</param>
        /// <returns></returns>
        private CUDDNode SCCHull(Model model, CUDDNode initState, List<CUDDNode> cycleMustPass)
        {
            CUDDNode old = CUDD.Constant(0);

            CUDD.Ref(initState);
            CUDDNode New = model.SuccessorsStart(initState, this.transitionsNoEvents);

            //Limit transition to transition of reachable states
            CUDD.Ref(New, New);
            transitionsNoEvents[0] = CUDD.Function.And(transitionsNoEvents[0], New);
            transitionsNoEvents[1] = CUDD.Function.And(transitionsNoEvents[1], New);

            //Pruning step: remove states whose succeessors are not belong to that set
            while (!New.Equals(old))
            {
                CUDD.Deref(old);
                CUDD.Ref(New);
                old = New;

                foreach (var justice in cycleMustPass)
                {
                    //final state as justice requirement
                    //new = (new and J) x R*
                    CUDD.Ref(justice);
                    New = model.SuccessorsStart(CUDD.Function.And(New, justice), this.transitionsNoEvents);
                }

                //while new is not comprised of the set of all successors of new states
                while (true)
                {
                    CUDD.Ref(New, New);
                    CUDDNode temp = CUDD.Function.And(New, model.Successors(New, this.transitionsNoEvents));

                    if (temp.Equals(New))
                    {
                        CUDD.Deref(temp);
                        break;
                    }
                    else
                    {
                        CUDD.Deref(New);
                        New = temp;
                    }
                }

            }

            return New;
        }

        /// <summary>
        /// Return a computation of a buchi automata in form "prefix (period)*"
        /// [ REFS: 'prefix, period', DEREFS:justices]
        /// </summary>
        /// <param name="automataBDD"></param>
        /// <param name="model"></param>
        /// <param name="justices">The justice must be event-based justices</param>
        public void MC(AutomataBDD automataBDD, Model model, List<CUDDNode> justices)
        {
            //Clear the old data
            this.transitionsNoEvents.Clear();
            this.prefix.Clear();
            this.period.Clear();

            ExpressionBDDEncoding initEncoding = automataBDD.initExpression.TranslateBoolExpToBDD(model);

            if (initEncoding.GuardDDs.Count == 0)
            {
                return;
            }

            ExpressionBDDEncoding finalStateEncoding = automataBDD.acceptanceExpression.TranslateBoolExpToBDD(model);
            if (finalStateEncoding.GuardDDs.Count == 0)
            {
                return;
            }

            CUDDNode initState = CUDD.Function.Or(initEncoding.GuardDDs);
            CUDDNode finalState = CUDD.Function.Or(finalStateEncoding.GuardDDs);
            CUDDNode finalStateWithNoEvent = CUDD.Abstract.ThereExists(finalState, model.GetAllEventVars());

            CUDD.Ref(automataBDD.transitionBDD);
            this.transitionsNoEvents = CUDD.Abstract.ThereExists(automataBDD.transitionBDD, model.GetAllEventVars());

            List<CUDDNode> cycleMustPass = new List<CUDDNode>(justices);
            cycleMustPass.Add(finalStateWithNoEvent);

            CUDDNode allSCCs = SCCHull(model, initState, cycleMustPass);

            if (!allSCCs.Equals(CUDD.ZERO) && VerificationOutput.GenerateCounterExample)
            {
                this.VerificationOutput.VerificationResult = VerificationResultType.INVALID;

                //Transitions out from allSCCs
                CUDD.Ref(transitionsNoEvents);
                CUDD.Ref(allSCCs);
                List<CUDDNode> R = CUDD.Function.And(transitionsNoEvents, allSCCs);

                //pick one state from the set final
                CUDD.Ref(allSCCs);
                CUDDNode s = CUDD.RestrictToFirst(allSCCs, model.AllRowVars);

                //while the states from which we can reach s are not all states that can be reached from s
                CUDDNode scc;
                while (true)
                {
                    CUDD.Ref(s);
                    CUDDNode backwardOfS = model.PredecessorsStart(s, R);

                    CUDD.Ref(s);
                    CUDDNode forwardOfS = model.SuccessorsStart(s, R);

                    //
                    CUDD.Ref(backwardOfS, forwardOfS);
                    CUDDNode temp = CUDD.Function.Different(backwardOfS, forwardOfS);
                    if (temp.Equals(CUDD.ZERO))
                    {
                        scc = backwardOfS;
                        CUDD.Deref(forwardOfS, temp);
                        break;
                    }
                    else
                    {
                        CUDD.Deref(backwardOfS, forwardOfS, s);
                        s = CUDD.RestrictToFirst(temp, model.AllRowVars);
                    }
                }

                //R now contains only transitions within the SCC scc
                CUDD.Ref(scc, scc, scc, scc);
                R[0] = CUDD.Function.And(CUDD.Function.And(R[0], scc), model.SwapRowColVars(scc));
                R[1] = CUDD.Function.And(CUDD.Function.And(R[1], scc), model.SwapRowColVars(scc));

                CUDD.Ref(scc);
                CUDDNode notInSCC = CUDD.Function.Not(scc);

                List<CUDDNode> transitionNotInSCC = new List<CUDDNode>();

                CUDD.Ref(transitionsNoEvents, transitionsNoEvents);
                CUDD.Ref(notInSCC, notInSCC);
                transitionNotInSCC.AddRange(CUDD.Function.And(transitionsNoEvents, notInSCC));
                transitionNotInSCC.AddRange(CUDD.Function.And(transitionsNoEvents, model.SwapRowColVars(notInSCC)));


                //prefix is now a shortest path from an initial state to a state in final
                model.Path(initState, scc, transitionNotInSCC, prefix, true);

                //Dummy value
                period.Add((prefix.Count == 0) ? initState : prefix[prefix.Count - 1]);

                foreach (var justice in justices)
                {
                    //cycle must pass all justice conditions, we will not care the first state of period because the justices are event-based. we need events in period, not in prefix
                    CUDD.Ref(period);
                    CUDD.Ref(justice);
                    CUDDNode temp1 = CUDD.Function.And(CUDD.Function.Or(period.GetRange(1, period.Count - 1)), justice);
                    if (temp1.Equals(CUDD.ZERO))
                    {
                        CUDD.Ref(scc, justice);
                        CUDDNode acceptanceStateInCyle = CUDD.Function.And(scc, justice);
                        
                        //if period.Count == 1, then the event in the first element is not counted
                        model.Path(period[period.Count - 1], acceptanceStateInCyle, R, period, (period.Count > 1));

                        CUDD.Deref(acceptanceStateInCyle);
                    }
                    CUDD.Deref(temp1);
                }

                CUDD.Ref(period);
                CUDD.Ref(finalStateWithNoEvent);
                CUDDNode temp2 = CUDD.Function.And(CUDD.Function.Or(period), finalStateWithNoEvent);
                if (temp2.Equals(CUDD.ZERO))
                {
                    CUDD.Ref(scc, finalStateWithNoEvent);
                    CUDDNode acceptanceStateInCyle = CUDD.Function.And(scc, finalStateWithNoEvent);
                    model.Path(period[period.Count - 1], acceptanceStateInCyle, R, period, true);

                    CUDD.Deref(acceptanceStateInCyle);
                }
                CUDD.Deref(temp2);

                //
                bool isEmptyPathAllowed = (period.Count == 1 ? false : true);
                model.Path(period[period.Count - 1], period[0], R, period, isEmptyPathAllowed);


                //Remove dummy
                CUDD.Deref(period[0]); period.RemoveAt(0);

                //
                CUDD.Deref(initState, allSCCs, s, scc, notInSCC);
                CUDD.Deref(transitionsNoEvents[0], transitionsNoEvents[1]);
                CUDD.Deref(R[0], R[1]);
                CUDD.Deref(justices);//contains finalStateWithNoEvent

            }
            else
            {
                this.VerificationOutput.VerificationResult = VerificationResultType.VALID;
                CUDD.Deref(initState, finalStateWithNoEvent, allSCCs);
                CUDD.Deref(transitionsNoEvents[0], transitionsNoEvents[1]);
                CUDD.Deref(justices);
            }

        }

        /// <summary>
        /// Create a new variable which know whether event is tick or not
        /// Later all event information is removed from the transition
        /// Return the justice based on that new varaible
        /// justice: event = tick, event != tick
        /// </summary>
        /// <param name="modelBDD"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public List<CUDDNode> GetJustices(AutomataBDD modelBDD, Model model)
        {
            model.AddLocalVar(EVENT_DUMMY, 0, 1);

            //Because later event variables are removed, add new variable to check whether it is tick or not
            Expression guard = Expression.OR(
                                                        Expression.AND(
                                                                                 TimeBehaviors.GetTickTransExpression(),
                                                                                 new Assignment(EVENT_DUMMY,
                                                                                                new IntConstant(1))),
                                                        Expression.AND(
                                                                                 TimeBehaviors.GetNotTickTransExpression(),
                                                                                 new Assignment(EVENT_DUMMY,
                                                                                                new IntConstant(0))));
            modelBDD.transitionBDD = CUDD.Function.And(modelBDD.transitionBDD, guard.TranslateBoolExpToBDD(model).GuardDDs);

            //the cycle must contain tick transition (zeno) and other transitions (progress)
            List<CUDDNode> justices = new List<CUDDNode>();

            guard = Expression.EQ(new Variable(EVENT_DUMMY),
                                             new IntConstant(0));

            justices.Add(CUDD.Function.Or(guard.TranslateBoolExpToBDD(model).GuardDDs));

            guard = Expression.EQ(new Variable(EVENT_DUMMY),
                                             new IntConstant(1));

            justices.Add(CUDD.Function.Or(guard.TranslateBoolExpToBDD(model).GuardDDs));

            return justices;
        }
    }
}
