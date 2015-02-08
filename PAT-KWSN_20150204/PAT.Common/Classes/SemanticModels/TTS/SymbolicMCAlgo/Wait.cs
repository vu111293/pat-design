using System.Collections.Generic;
using PAT.Common.Classes.CUDDLib;
using PAT.Common.Classes.SemanticModels.LTS.BDD;
using PAT.Common.Classes.Expressions.ExpressionClass;

namespace PAT.Common.Classes.SemanticModels.TTS
{
    public partial class TimeBehaviors
    {
        /// <summary>
        /// Delays the system execution for a period of t time units then terminates.
        /// </summary>
        /// <param name="t"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public static AutomataBDD Wait(int t, Model model)
        {
            AutomataBDD result = new AutomataBDD();
            
            List<string> varNames = WaitSetVariable(t, model, result);
            WaitSetInit(varNames[0], result);
            WaitEncodeTransitionChannel(varNames[0], t, model, result);
            WaitEncodeTick(varNames[0], t, model, result);

            //
            return result;
        }

        /// <summary>
        /// P.var : 0 <= state <= t + 1
        /// </summary>
        /// <param name="t"></param>
        /// <param name="model"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        private static List<string> WaitSetVariable(int t, Model model, AutomataBDD result)
        {
            string state = Model.GetNewTempVarName();
            model.AddLocalVar(state, 0, t + 1);
            result.variableIndex.Add(model.GetNumberOfVars() - 1);

            return new List<string>() { state };
        }

        /// <summary>
        /// P.init: state = 0
        /// </summary>
        private static void WaitSetInit(string state, AutomataBDD result)
        {
            result.initExpression = Expression.EQ(new Variable(state), new IntConstant(0));
        }

        private static void WaitEncodeTransitionChannel(string state, int t, Model model, AutomataBDD result)
        {
            //1. state = t and event' = terminate and state' = t + 1
            Expression guard;
            List<CUDDNode> guardDD;
            guard = Expression.AND(Expression.EQ(new Variable(state), new IntConstant(t)),
                        Expression.AND(AutomataBDD.GetTerminateTransExpression(),
                            new Assignment(state, new IntConstant(t + 1))));
            guardDD = guard.TranslateBoolExpToBDD(model).GuardDDs;
            guardDD = model.AddVarUnchangedConstraint(guardDD, model.GlobalVarIndex);
            result.transitionBDD.AddRange(guardDD);
        }

        private static void WaitEncodeTick(string state, int t, Model model, AutomataBDD result)
        {
            //1. state < t and event' = tick and state' = state + 1
            Expression guard;
            List<CUDDNode> guardDD;

            guard = Expression.AND(Expression.LT(new Variable(state), new IntConstant(t)),
                        Expression.AND(GetTickTransExpression(),
                            new Assignment(state, Expression.PLUS(new Variable(state), new IntConstant(1)))));
            guardDD = guard.TranslateBoolExpToBDD(model).GuardDDs;
            guardDD = model.AddVarUnchangedConstraint(guardDD, model.GlobalVarIndex);
            result.Ticks.AddRange(guardDD);

            //1. state = t and event' = tick and state' = t
            //1. state = t + 1 and event' = tick and state' = t + 1
            guard = Expression.AND(Expression.OR(
                            Expression.EQ(new Variable(state), new IntConstant(t)),
                            Expression.EQ(new Variable(state), new IntConstant(t + 1))),
                                    Expression.AND(GetTickTransExpression(),
                                        new Assignment(state, new Variable(state))));
            guardDD = guard.TranslateBoolExpToBDD(model).GuardDDs;
            guardDD = model.AddVarUnchangedConstraint(guardDD, model.GlobalVarIndex);
            result.Ticks.AddRange(guardDD);
        }

    }
}
