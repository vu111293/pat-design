using System.Collections.Generic;
using PAT.Common.Classes.CUDDLib;
using PAT.Common.Classes.Expressions.ExpressionClass;
using PAT.Common.Classes.SemanticModels.LTS.BDD;

namespace PAT.Common.Classes.SemanticModels.TTS
{
    public partial class TimeBehaviors
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
            AutomataBDD result = AutomataBDD.Interrupt(P1, P2, model);
            InterruptEncodeTick(P1, P2, model, result);

            //
            return result;
        }

        /// <summary>
        /// 1. Interrupt is not resolved, time evolution is synchronized
        /// 2. P1 terminate, or P2 interrupt, not need synchronization
        /// </summary>
        /// <param name="P1"></param>
        /// <param name="P2"></param>
        /// <param name="model"></param>
        /// <param name="result"></param>
        private static void InterruptEncodeTick(AutomataBDD P1, AutomataBDD P2, Model model, AutomataBDD result)
        {
            //1. (isInterrupted = 0 ∧ P1.Tick ∧ P2.Tick ∧ isInterrupted' = 0) 
            Expression guard = Expression.EQ(new Variable(result.newLocalVarName), new IntConstant(0));
            Expression update = new Assignment(result.newLocalVarName, new IntConstant(0));

            List<CUDDNode> transition = model.EncodeTransition(guard, update, new List<int>());
            CUDD.Ref(P2.Ticks);
            transition = CUDD.Function.And(CUDD.Function.And(transition, P1.Ticks), P2.Ticks);

            result.Ticks.AddRange(CUDD.Function.And(transition, P1.Ticks));

            //2. (isInterrupted = 1 and P1.tick ∧ isInterrupted' = 1)
            guard = Expression.EQ(new Variable(result.newLocalVarName), new IntConstant(1));
            update = new Assignment(result.newLocalVarName, new IntConstant(1));

            transition = model.EncodeTransition(guard, update, new List<int>());

            result.Ticks.AddRange(CUDD.Function.And(transition, P1.Ticks));

            //3. (isInterrupted = 2 and P2.tick ∧ isInterrupted' = 2)
            guard = Expression.EQ(new Variable(result.newLocalVarName), new IntConstant(2));
            update = new Assignment(result.newLocalVarName, new IntConstant(2));

            transition = model.EncodeTransition(guard, update, new List<int>());
            
            result.Ticks.AddRange(CUDD.Function.And(transition, P2.Ticks));
        }
    }
}
