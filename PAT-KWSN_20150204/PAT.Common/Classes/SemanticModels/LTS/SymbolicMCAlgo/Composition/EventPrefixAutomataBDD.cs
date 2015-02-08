using System.Collections.Generic;
using PAT.Common.Classes.CUDDLib;
using PAT.Common.Classes.Expressions.ExpressionClass;

namespace PAT.Common.Classes.SemanticModels.LTS.BDD
{
    public partial class AutomataBDD
    {
        /// <summary>
        /// Return AutomataBDD of the not tau event prefix process with guard [b] e -> P1
        /// </summary>
        /// <param name="guard">Guard of this event to happen</param>
        /// <param name="updateOfEvent">Update command happening with the event</param>
        /// <param name="P1">AutomataBDD of the process P1 engaging after the event</param>
        /// <param name="model"></param>
        /// <returns></returns>
        public static AutomataBDD EventPrefix(Expression guard, Expression updateOfEvent, AutomataBDD P1, Model model)
        {
            AutomataBDD result = new AutomataBDD();

            EventPrefixSetVariable(P1, model, result);
            EventPrefixSetInit(result);
            EventPrefixEncodeTransition(guard, updateOfEvent, P1, model, result);

            //
            return result;
        }

        /// <summary>
        /// P.var : P1.var ∪ {temp}
        /// </summary>
        private static void EventPrefixSetVariable(AutomataBDD P1, Model model, AutomataBDD result)
        {
            result.variableIndex.AddRange(P1.variableIndex);

            result.newLocalVarName = Model.GetNewTempVarName();
            model.AddLocalVar(result.newLocalVarName, 0, 1);
            result.variableIndex.Add(model.GetNumberOfVars() - 1);
        }

        /// <summary>
        /// P.init: !temp '
        /// </summary>
        private static void EventPrefixSetInit(AutomataBDD result)
        {
            result.initExpression = Expression.EQ(new Variable(result.newLocalVarName), new IntConstant(0));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="guard">If null, give BoolConstant(true)</param>
        /// <param name="updateOfEvent">If null, give BoolConstant(true)</param>
        /// <param name="P1"></param>
        /// <param name="model"></param>
        /// <param name="result"></param>
        private static void EventPrefixEncodeTransition(Expression guard, Expression updateOfEvent, AutomataBDD P1, Model model, AutomataBDD result)
        {
            EventPrefixTransitition(guard, updateOfEvent, P1, model, result);

            CopyTransitionAfterEventChannel(P1, model, result);
        }

        /// <summary>
        /// (!temp ∧ guard∧ ∧ update ∧ temp ' ∧ P1.init)
        /// </summary>
        private static void EventPrefixTransitition(Expression guardOfTrans, Expression updateOfTrans, AutomataBDD P1, Model model, AutomataBDD result)
        {
            Expression guard = Expression.AND(Expression.EQ(new Variable(result.newLocalVarName), new IntConstant(0)),
                                    guardOfTrans);


            Expression update = new Sequence(updateOfTrans, new Assignment(result.newLocalVarName, new IntConstant(1)));

            List<CUDDNode> transition = model.EncodeTransition(guard, update, new List<int>());
            transition = CUDD.Function.And(transition, P1.GetInitInColumn(model));
            transition = model.AddVarUnchangedConstraint(transition, model.GlobalVarIndex);

            result.transitionBDD.AddRange(transition);
        }

        /// <summary>
        /// (temp ∧ P1.Trans/In/Out ∧ temp') 
        /// [ REFS: '', DEREFS: 'P1']
        /// </summary>
        private static void CopyTransitionAfterEventChannel(AutomataBDD P1, Model model, AutomataBDD result)
        {
            Expression guard = Expression.AND(Expression.EQ(new Variable(result.newLocalVarName), new IntConstant(1)),
                        new Assignment(result.newLocalVarName, new IntConstant(1)));
            List<CUDDNode> guardDD = guard.TranslateBoolExpToBDD(model).GuardDDs;

            //
            CUDD.Ref(guardDD);
            result.transitionBDD.AddRange(CUDD.Function.And(guardDD, P1.transitionBDD));

            CUDD.Ref(guardDD);
            result.channelInTransitionBDD.AddRange(CUDD.Function.And(guardDD, P1.channelInTransitionBDD));

            //CUDD.Ref(guardDD);
            result.channelOutTransitionBDD.AddRange(CUDD.Function.And(guardDD, P1.channelOutTransitionBDD));
        }
    }
}
