using System.Collections.Generic;
using PAT.Common.Classes.CUDDLib;
using PAT.Common.Classes.Expressions.ExpressionClass;
using PAT.Common.Classes.SemanticModels.LTS.BDD;

namespace PAT.Common.Classes.SemanticModels.TTS
{
    public partial class TimeBehaviors
    {
        /// <summary>
        /// Return AutomataBDD of Sequence process
        /// </summary>
        /// <param name="P1">AutomataBDD of the first process</param>
        /// <param name="P2">AutomataBDD of the second process</param>
        /// <param name="model"></param>
        /// <returns></returns>
        public static AutomataBDD Sequence(AutomataBDD P1, AutomataBDD P2, Model model)
        {
            AutomataBDD result = new AutomataBDD();

            List<string> varNames = AutomataBDD.SequenceSetVariable(P1, P2, model, result);
            AutomataBDD.SequenceSetInit(varNames[0], P1, P2, result);

            CUDD.Ref(P1.transitionBDD);
            AutomataBDD.SequenceEncodeTransition(varNames[0], P1, P2, model, result);
            SequenceEncodeTick(varNames[0], P1, P2, model, result);
            //
            return result;
        }

        /// <summary>
        /// [ REFS: 'result', DEREFS: 'P1.Trans, P1.Ticks, P2.Ticks']
        /// </summary>
        /// <param name="isP1Terminate"></param>
        /// <param name="P1"></param>
        /// <param name="P2"></param>
        /// <param name="model"></param>
        /// <param name="result"></param>
        private static void SequenceEncodeTick(string isP1Terminate, AutomataBDD P1, AutomataBDD P2, Model model, AutomataBDD result)
        {
            // Remove tick transition delays the tau transition to initial state of P2
            CUDDNode terminateTransInP1 = CUDD.Function.And(AutomataBDD.GetTerminationTransEncoding(model), P1.transitionBDD);

            CUDDNode terminatingStates = CUDD.Abstract.ThereExists(terminateTransInP1, model.AllColVars);
            CUDDNode notTerminatingStates = CUDD.Function.Not(terminatingStates);

            //
            P1.Ticks = CUDD.Function.And(P1.Ticks, notTerminatingStates);


            //2. (temp and P2.tick ∧ temp')
            Expression guard = Expression.AND(
                                                        Expression.EQ(
                                                                                 new Variable(isP1Terminate),
                                                                                 new IntConstant(1)),
                                                        new Assignment(isP1Terminate, new IntConstant(1)));

            List<CUDDNode> transition = guard.TranslateBoolExpToBDD(model).GuardDDs;
            result.Ticks.AddRange(CUDD.Function.And(transition, P2.Ticks));
        }
    }
}
