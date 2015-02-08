using System.Collections.Generic;
using PAT.Common.Classes.CUDDLib;
using PAT.Common.Classes.Expressions.ExpressionClass;
using PAT.Common.Classes.SemanticModels.LTS.BDD;

namespace PAT.Common.Classes.SemanticModels.TTS
{
    public partial class TimeBehaviors
    {
        /// <summary>
        /// Return AutomataBDD of the not tau event prefix process with guard [b] e -> P1
        /// </summary>
        /// <param name="guard"></param>
        /// <param name="updateOfEvent">Update command happening with the event</param>
        /// <param name="guardOfTick">Guard of the tick before engaging the event</param>
        /// <param name="P1">AutomataBDD of the process P1 engaging after the event</param>
        /// <param name="model"></param>
        /// <returns></returns>
        public static AutomataBDD EventPrefix(Expression guard, Expression updateOfEvent, Expression guardOfTick, AutomataBDD P1, Model model)
        {
            AutomataBDD result = AutomataBDD.EventPrefix(guard, updateOfEvent, P1, model);
            EventPrefixEncodeTick(guardOfTick, P1, model, result);

            //
            return result;
        }

        
        /// <summary>
        /// Tick transition if done is false, don't need to add unchanged condition of other local variables.
        /// They will be updated after the event becomes true
        /// use the tick transition of P1 if done is true
        /// </summary>
        /// <param name="P1"></param>
        /// <param name="model"></param>
        /// <param name="result"></param>
        private static void EventPrefixEncodeTick(Expression guardOfTick, AutomataBDD P1, Model model, AutomataBDD result)
        {
            Expression guard;
            List<CUDDNode> guardDD;

            //1. !done and guardOfTick and event = tick and !done'
            if (!(guardOfTick is BoolConstant && !(guardOfTick as BoolConstant).Value))
            {
                guard = Expression.AND(
                                                            Expression.EQ(
                                                                                     new Variable(result.newLocalVarName),
                                                                                     new IntConstant(0)),
                                                            Expression.AND(
                                                                                     guardOfTick,
                                                                                     new PrimitiveApplication(
                                                                                         PrimitiveApplication.AND,
                                                                                         AutomataBDD.
                                                                                             GetTerminateTransExpression
                                                                                             (),
                                                                                         new Assignment(
                                                                                             result.newLocalVarName,
                                                                                             new IntConstant(0)))));

                guardDD = guard.TranslateBoolExpToBDD(model).GuardDDs;
                guardDD = model.AddVarUnchangedConstraint(guardDD, model.GlobalVarIndex);
                result.Ticks.AddRange(guardDD);
            }

            //2. done and p1.Tick and done'
            guard = Expression.AND(Expression.EQ(new Variable(result.newLocalVarName), new IntConstant(1)),
                        new Assignment(result.newLocalVarName, new IntConstant(0)));
            guardDD = guard.TranslateBoolExpToBDD(model).GuardDDs;
            guardDD = CUDD.Function.And(guardDD, P1.Ticks);
            result.Ticks.AddRange(guardDD);
        }
    }
}
