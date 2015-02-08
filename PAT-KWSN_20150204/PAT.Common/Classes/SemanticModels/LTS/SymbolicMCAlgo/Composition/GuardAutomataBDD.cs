using System.Collections.Generic;
using PAT.Common.Classes.CUDDLib;
using PAT.Common.Classes.Expressions.ExpressionClass;

namespace PAT.Common.Classes.SemanticModels.LTS.BDD
{
    public partial class AutomataBDD
    {
        /// <summary>
        /// Return AutomataBDD of the guard process
        /// </summary>
        /// <param name="guard">The guard expression of the process P1</param>
        /// <param name="P1">AutomataBDD of the process P1</param>
        /// <param name="model"></param>
        /// <returns></returns>
        public static AutomataBDD Guard(Expression guard, AutomataBDD P1, Model model)
        {
            AutomataBDD result = new AutomataBDD();

            GuardSetVariable(P1, model, result);
            GuardSetInit(P1, result);
            GuardEncodeTransition(guard, P1, model, result);

            //
            return result;
        }

        /// <summary>
        /// P.var : P1.var
        /// </summary>
        private static void GuardSetVariable(AutomataBDD P1, Model model, AutomataBDD result)
        {
            result.variableIndex.AddRange(P1.variableIndex);

            result.newLocalVarName = Model.GetNewTempVarName();
            model.AddLocalVar(result.newLocalVarName, 0, 1);
            result.variableIndex.Add(model.GetNumberOfVars() - 1);
        }

        /// <summary>
        /// P1.init & !temp'
        /// </summary>
        private static void GuardSetInit(AutomataBDD P1, AutomataBDD result)
        {
            result.initExpression = Expression.AND(P1.initExpression,
                                            Expression.EQ(new Variable(result.newLocalVarName), new IntConstant(0)));
        }

        /// <summary>
        /// [ REFS: '', DEREFS: 'P1']
        /// </summary>
        /// <param name="b"></param>
        /// <param name="P1"></param>
        /// <param name="model"></param>
        /// <param name="result"></param>
        private static void GuardEncodeTransition(Expression b, AutomataBDD P1, Model model, AutomataBDD result)
        {
            //(P1.Trans/In/Out and [(b and !temp and temp') or (temp and temp')]
            Expression guard = Expression.OR(Expression.AND(b,
                        Expression.AND(Expression.NOT(new Variable(result.newLocalVarName)),
                            new Assignment(result.newLocalVarName, new IntConstant(1)))), Expression.AND(
                                new Variable(result.newLocalVarName), new Assignment(result.newLocalVarName, new IntConstant(1))));
            List<CUDDNode> guardDD = guard.TranslateBoolExpToBDD(model).GuardDDs;

            CUDD.Ref(guardDD);
            result.transitionBDD.AddRange(CUDD.Function.And(guardDD, P1.transitionBDD));

            CUDD.Ref(guardDD);
            result.channelInTransitionBDD.AddRange(CUDD.Function.And(guardDD, P1.channelInTransitionBDD));

            result.channelOutTransitionBDD.AddRange(CUDD.Function.And(guardDD, P1.channelOutTransitionBDD));
        }
    }
}
