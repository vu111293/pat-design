using System.Collections.Generic;
using PAT.Common.Classes.CUDDLib;
using PAT.Common.Classes.Expressions.ExpressionClass;

namespace PAT.Common.Classes.SemanticModels.LTS.BDD
{
    public partial class AutomataBDD
    {
        /// <summary>
        /// Return AutomataBDD of interrupt process
        /// </summary>
        /// <param name="P1">AutomataBDD of interrupted process</param>
        /// <param name="P2">AutomataBDD of interrupting process</param>
        /// <param name="model"></param>
        /// <returns></returns>
        public static AutomataBDD Interrupt(AutomataBDD P1, AutomataBDD P2, Model model)
        {
            AutomataBDD result = new AutomataBDD();

            InterruptSetVariable(P1, P2, model, result);
            InterruptSetInit(P1, P2, result);
            InterruptEncodeTransition(P1, P2, model, result);

            //
            return result;
        }
        /// <summary>
        /// P.var : P1.var ∪ : P2.var ∪{isInterrupted}
        /// isInterrupted = 0: not interrupted.
        /// isInterrupted = 1: P1 terminates
        /// isInterrupted = 2: P2 interrupts
        /// </summary>
        private static void InterruptSetVariable(AutomataBDD P1, AutomataBDD P2, Model model, AutomataBDD result)
        {
            result.variableIndex.AddRange(P1.variableIndex);
            result.variableIndex.AddRange(P2.variableIndex);

            result.newLocalVarName = Model.GetNewTempVarName();
            model.AddLocalVar(result.newLocalVarName, 0, 2);
            result.variableIndex.Add(model.GetNumberOfVars() - 1);
        }

        /// <summary>
        /// P1.init & P2.init & isInterrupted = 0
        /// </summary>
        private static void InterruptSetInit(AutomataBDD P1, AutomataBDD P2, AutomataBDD result)
        {
            result.initExpression = Expression.AND(Expression.AND(P1.initExpression, P2.initExpression),
                                        Expression.EQ(new Variable(result.newLocalVarName), new IntConstant(0)));
        }

        /// <summary>
        /// tau transition in process 2 does not resolve
        /// [ REFS: '', DEREFS: 'P1, P2'] 
        /// </summary>
        /// <param name="P1"></param>
        /// <param name="P2"></param>
        /// <param name="model"></param>
        /// <param name="result"></param>
        private static void InterruptEncodeTransition(AutomataBDD P1, AutomataBDD P2, Model model, AutomataBDD result)
        {
            //1. (isInterrupted < 2 ∧ P1.Trans/In/Out ∧ [(event' = termination  and isInterrupted' = 1)  or (event' != termination and  isInterrupted' = isInterrupted)] P2.var = P2.var')
            Expression guard = Expression.AND(
                                    Expression.LT(new Variable(result.newLocalVarName), new IntConstant(2)),
                                    Expression.OR(
                                                    Expression.AND(
                                                        GetTerminateTransExpression(),
                                                        new Assignment(result.newLocalVarName, new IntConstant(1))),
                                                    Expression.AND(
                                                        GetNotTerminateTransExpression(),
                                                        new Assignment(result.newLocalVarName, new Variable(result.newLocalVarName))))
                                    );

            List<CUDDNode> transition = guard.TranslateBoolExpToBDD(model).GuardDDs;
            transition = model.AddVarUnchangedConstraint(transition, P2.variableIndex);
            
            //
            CUDD.Ref(transition);
            result.transitionBDD.AddRange(CUDD.Function.And(transition, P1.transitionBDD));

            CUDD.Ref(transition);
            result.channelInTransitionBDD.AddRange(CUDD.Function.And(transition, P1.channelInTransitionBDD));

            result.channelOutTransitionBDD.AddRange(CUDD.Function.And(transition, P1.channelOutTransitionBDD));

            //2. (isInterrupted != 1 ∧ P2.Trans/In/Out ∧ [(event' = tau  and isInterrupted' = isInterrupted)  or (event' != tau and  isInterrupted' = 2)])
            guard = Expression.AND(
                        Expression.NE(new Variable(result.newLocalVarName), new IntConstant(1)),
                        Expression.OR(
                                        Expression.AND(
                                            GetTauTransExpression(),
                                            new Assignment(result.newLocalVarName, new Variable(result.newLocalVarName))),
                                        Expression.AND(
                                            GetNotTauTransExpression(),
                                            new Assignment(result.newLocalVarName, new IntConstant(2))))
                        );
            transition = guard.TranslateBoolExpToBDD(model).GuardDDs;

            //
            CUDD.Ref(transition);
            result.transitionBDD.AddRange(CUDD.Function.And(transition, P2.transitionBDD));

            CUDD.Ref(transition);
            result.channelInTransitionBDD.AddRange(CUDD.Function.And(transition, P2.channelInTransitionBDD));

            //CUDD.Ref(transition);
            result.channelOutTransitionBDD.AddRange(CUDD.Function.And(transition, P2.channelOutTransitionBDD));
        }
    }
}
