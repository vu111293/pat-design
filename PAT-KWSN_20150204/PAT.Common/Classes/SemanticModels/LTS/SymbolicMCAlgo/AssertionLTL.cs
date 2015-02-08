using System.Collections.Generic;
using PAT.Common.Classes.ModuleInterface;
using PAT.Common.Classes.SemanticModels.LTS.BDD;
using PAT.Common.Classes.CUDDLib;
using PAT.Common.Classes.Expressions;
using PAT.Common.Classes.Expressions.ExpressionClass;
using PAT.Common.Classes.SemanticModels.TTS;

namespace PAT.Common.Classes.SemanticModels.LTS.Assertion
{
    public abstract partial class AssertionLTL: AssertionBase
    {
        public List<CUDDNode> transitionsNoEvents = new List<CUDDNode>();

        public List<CUDDNode> prefix = new List<CUDDNode>();
        public List<CUDDNode> period = new List<CUDDNode>();

        
        /// <summary>
        /// Return strongly connected components
        /// Does not include Event variable
        /// this.transitionsNoEvents is the transtion from reachble states
        /// From Feasible algorithm in the article "LTL Symbolic MC"
        /// [ REFS: 'result', DEREFS:]
        /// </summary>
        /// <param name="model"></param>
        /// <param name="initState"></param>
        /// <param name="finalState"></param>
        /// <returns></returns>
        private CUDDNode SCCHull(Model model, CUDDNode initState, CUDDNode finalState)
        {
            CUDDNode old = CUDD.Constant(0);

            CUDD.Ref(initState);
            CUDDNode New = model.SuccessorsStart(initState, this.transitionsNoEvents);

            //Limit transition to transition of reachable states
            CUDD.Ref(New);
            transitionsNoEvents = CUDD.Function.And(transitionsNoEvents, New);
            
            //Pruning step: remove states whose succeessors are not belong to that set
            while (!New.Equals(old))
            {
                CUDD.Deref(old);
                CUDD.Ref(New);
                old = New;

                //final state as justice requirement
                //new = (new and J) x R*
                CUDD.Ref(finalState);
                New = model.SuccessorsStart(CUDD.Function.And(New, finalState), this.transitionsNoEvents);

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
        /// [ REFS: 'prefix, period', DEREFS:]
        /// </summary>
        /// <param name="automataBDD"></param>
        /// <param name="model"></param>
        public void MC(AutomataBDD automataBDD, Model model)
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

            CUDDNode allSCCs = SCCHull(model, initState, finalStateWithNoEvent);

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
                CUDD.Deref(transitionNotInSCC[0], transitionNotInSCC[1]);

                //Dummy value
                period.Add((prefix.Count == 0) ? initState : prefix[prefix.Count - 1]);

                //cycle must pass final state
                CUDD.Ref(period);
                CUDD.Ref(finalStateWithNoEvent);
                CUDDNode temp1 = CUDD.Function.And(CUDD.Function.Or(period), finalStateWithNoEvent);
                if (temp1.Equals(CUDD.ZERO))
                {
                    CUDD.Ref(scc, finalStateWithNoEvent);
                    CUDDNode acceptanceStateInCyle = CUDD.Function.And(scc, finalStateWithNoEvent);
                    model.Path(period[period.Count - 1], acceptanceStateInCyle, R, period, true);

                    CUDD.Deref(acceptanceStateInCyle);
                }
                CUDD.Deref(temp1);

                //
                bool isEmptyPathAllowed = period.Count != 1;
                model.Path(period[period.Count - 1], period[0], R, period, isEmptyPathAllowed);


                //Remove dummy
                CUDD.Deref(period[0]); period.RemoveAt(0);

                //
                CUDD.Deref(initState, finalStateWithNoEvent, allSCCs, s, scc, notInSCC);
                CUDD.Deref(transitionsNoEvents[0], transitionsNoEvents[1]);
                CUDD.Deref(R[0], R[1]);

            }
            else
            {
                this.VerificationOutput.VerificationResult = VerificationResultType.VALID;
                CUDD.Deref(initState, finalStateWithNoEvent, allSCCs);
                CUDD.Deref(transitionsNoEvents[0], transitionsNoEvents[1]);
            }

        }

        /// <summary>
        /// Generate the counter example for LTL model checking, including 2 parts: prefix, and period.
        /// [ REFS: '', DEREFS: automataBDD.transitionBDD, this.prefix, this.period ]
        /// </summary>
        /// <param name="automataBDD"></param>
        /// <param name="encoder"></param>
        /// <returns></returns>
        public void GetMCResult(AutomataBDD automataBDD, BDDEncoder encoder)
        {
            VerificationOutput.CounterExampleTrace.Add(InitialStep);

            if (VerificationOutput.VerificationResult == VerificationResultType.INVALID && VerificationOutput.GenerateCounterExample)
            {
                VerificationOutput.LoopIndex = this.prefix.Count + 1;

                List<CUDDNode> traces = new List<CUDDNode>();
                ExpressionBDDEncoding initEncoding = automataBDD.initExpression.TranslateBoolExpToBDD(encoder.model);
                traces.Add(CUDD.Function.Or(initEncoding.GuardDDs));
                traces.AddRange(this.prefix);
                traces.AddRange(this.period);

                Valuation currentValuation = this.InitialStep.GlobalEnv;
                Valuation lastValuation;
                for (int i = 1; i < traces.Count; i++)
                {
                    //Get event information
                    
                    CUDD.Ref(traces[i], traces[i - 1]);
                    CUDDNode transitionTemp = CUDD.Function.And(traces[i - 1], encoder.model.SwapRowColVars(traces[i]));

                    CUDD.Ref(automataBDD.transitionBDD);
                    CUDDNode transWithEventInfo = CUDD.Function.And(transitionTemp, automataBDD.transitionBDD);

                    transWithEventInfo = CUDD.Abstract.ThereExists(transWithEventInfo, encoder.model.AllRowVarsExceptSingleCopy);
                    transWithEventInfo = CUDD.RestrictToFirst(transWithEventInfo, encoder.model.AllColVars);

                    lastValuation = currentValuation;
                    currentValuation = encoder.GetValuationFromBDD(transWithEventInfo, this.InitialStep.GlobalEnv);

                    string eventName = encoder.GetEventChannelName(lastValuation, currentValuation, transWithEventInfo);

                    VerificationOutput.CounterExampleTrace.Add(new ConfigurationBDD(eventName, currentValuation));

                    //
                    CUDD.Deref(transWithEventInfo);
                }

                //
                CUDD.Deref(traces);
            }

            CUDD.Deref(automataBDD.transitionBDD);

            VerificationOutput.ActualMemoryUsage = CUDD.ReadMemoryInUse();
            VerificationOutput.numberOfBoolVars = encoder.model.NumberOfBoolVars;

            encoder.model.Close();
        }
    }
}
