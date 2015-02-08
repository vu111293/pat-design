using System.Collections.Generic;
using PAT.Common.Classes.CUDDLib;
using PAT.Common.Classes.Expressions.ExpressionClass;

namespace PAT.Common.Classes.SemanticModels.LTS.BDD
{
    public partial class AutomataBDD
    {
        /// <summary>
        /// Return AutomataBDD of Parallel process.
        /// Note: synchronized is created by AND of transition of 2 participant process a time. Therefore this only happens when the transition does not change any global variable
        /// because when encoding transition, each transition will make variable unchanged if it is not updated in that transition. This synchronization is similar to 
        /// Explicit model checking when does not allow synchronized transition having program block.
        /// </summary>
        /// <param name="processes">List of AutomataBDD of parallel processes</param>
        /// <param name="alphabets">alphabet of each process if provided. Give True if not provided</param>
        /// <param name="model"></param>
        /// <returns></returns>
        public static AutomataBDD Parallel(List<AutomataBDD> processes, List<CUDDNode> alphabets, Model model)
        {
            AutomataBDD result = new AutomataBDD();

            ParallelSetVariable(processes, result);
            ParallelSetInit(processes, result);
            ParallelEncodeTransition(processes, alphabets, model, result);
            InterleaveEncodeChannel(processes, model, result);

            //
            return result;
        }


        /// <summary>
        /// ∪ Pi.var
        /// </summary>
        private static void ParallelSetVariable(List<AutomataBDD> processes, AutomataBDD result)
        {
            foreach (AutomataBDD process in processes)
            {
                result.variableIndex.AddRange(process.variableIndex);
            }
        }

        /// <summary>
        /// ∧ Pi.init
        /// </summary>
        private static void ParallelSetInit(List<AutomataBDD> processes, AutomataBDD result)
        {
            result.initExpression = processes[0].initExpression;

            //This loop and the implementation of AND of a list make sure that processes having the same event can run simultaneously
            for (int i = 1; i < processes.Count; i++)
            {
                result.initExpression = Expression.AND(result.initExpression, processes[i].initExpression);
            }
        }


        /// <summary>
        /// syncTransition = P1.transition ∧ P2.transition
        /// syncEvent: formula of synchronized events
        /// P.transition = syncTransition ∨ (Pi.transition ∧ !syncEvent ∧ unchanged.i)
        /// Applied this formula for each pair of process: P1 || P2 || P3 = (P1 || P2) || P3
        /// [ REFS: '', DEREFS: 'processes[i].transitionBDD, alphabet']
        /// </summary>
        /// <param name="processes"></param>
        /// <param name="alphabets">alphabet of each process if provided. Give True if not provided</param>
        /// <param name="model"></param>
        /// <param name="result"></param>
        private static void ParallelEncodeTransition(List<AutomataBDD> processes, List<CUDDNode> alphabets, Model model, AutomataBDD result)
        {
            List<CUDDNode> transition = processes[0].transitionBDD;
            CUDDNode lastAlphabet = alphabets[0];

            //Do encoding at each step, 2 processes once
            for (int i = 1; i < processes.Count; i++)
            {
                //find sync transitions
                CUDD.Ref(transition);
                CUDD.Ref(processes[i].transitionBDD);
                List<CUDDNode> syncTransition = CUDD.Function.And(transition, processes[i].transitionBDD);

                //sync alphabet = (P0.alphabet and P1.alphabet) or termination event
                //We rename the event with assignment whose name may be also in the alphabet
                //After the rename, they do not belong to the alphabet
                CUDD.Ref(lastAlphabet, alphabets[i]);
                CUDDNode syncEvent = CUDD.Function.Or(CUDD.Function.And(lastAlphabet, alphabets[i]),
                                                      GetTerminationTransEncoding(model));

                //sync transition must have no program block which mean no global variable updated
                foreach (var globalVarIndex in model.GlobalVarIndex)
                {
                    CUDD.Ref(model.varIdentities[globalVarIndex]);
                    syncEvent = CUDD.Function.And(syncEvent, model.varIdentities[globalVarIndex]);
                }

                CUDD.Ref(syncEvent);
                syncTransition = CUDD.Function.And(syncTransition, syncEvent);

                //update current alphabet
                lastAlphabet = CUDD.Function.Or(lastAlphabet, alphabets[i]);



                CUDD.Ref(syncTransition);
                List<CUDDNode> tempTransition = new List<CUDDNode>(syncTransition);

                //find not sync event, we need to add global variable unchanged to syncEvent because for example process a -> a {x = 1} -> P;
                //a may be in the alphabet, and the first a can be synced, but the secondtion is not
                CUDDNode notSyncEvents = CUDD.Function.Not(syncEvent);

                
                CUDD.Ref(notSyncEvents);
                tempTransition.AddRange(CUDD.Function.And(transition, notSyncEvents));

                tempTransition.AddRange(CUDD.Function.And(processes[i].transitionBDD, notSyncEvents));

                transition = tempTransition;
            }

            //
            CUDD.Deref(lastAlphabet);

            transition = model.AddVarUnchangedConstraint(transition, result.variableIndex);

            //
            result.transitionBDD = transition;
        }
    }
}
