using System.Collections.Generic;
using PAT.Common.Classes.CUDDLib;
using PAT.Common.Classes.Expressions.ExpressionClass;

namespace PAT.Common.Classes.SemanticModels.LTS.BDD
{
    public partial class AutomataBDD
    {
        /// <summary>
        /// Return the AutomataBDD of the Sync Channel Input. Encode this as event c?a.b.c and put it to ChannelInputTransition
        /// Currently Channel Input is translated as assginment, not guard. We don't support using channel input to expect a certain value
        /// </summary>
        /// <param name="channelEventIndex"></param>
        /// <param name="guard">Guard expression of the channel input</param>
        /// <param name="exps">List of input expression to the channel</param>
        /// <param name="P1">AutomataBDD of process P1 after the channel input</param>
        /// <param name="model"></param>
        /// <returns></returns>
        public static AutomataBDD SyncChannelInputPrefixing(int channelEventIndex, Expression guard, List<Expression> exps, AutomataBDD P1, Model model)
        {
            AutomataBDD result = new AutomataBDD();

            ChannelInputSetVariable(exps, P1, model, result);
            EventPrefixSetInit(result);
            SyncChannelInputEncodeTransition(channelEventIndex, guard, exps, P1, model, result);

            //
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="channelEventIndex"></param>
        /// <param name="guard">If no guard, give BoolConstant(true)</param>
        /// <param name="exps">List of expressions of channel in</param>
        /// <param name="P1">Process after channel in</param>
        /// <param name="model"></param>
        /// <param name="result"></param>
        private static void SyncChannelInputEncodeTransition(int channelEventIndex, Expression guard, List<Expression> exps, AutomataBDD P1, Model model, AutomataBDD result)
        {
            //temp = result.processName
            //!temp and guard and event' = channelEventIndex and temp' and Update_Channel and P1.init'

            guard = Expression.AND(guard, Expression.EQ(new Variable(result.newLocalVarName),
                                                                                    new IntConstant(0)));
            guard = Expression.AND(guard, new Assignment(Model.EVENT_NAME, new IntConstant(channelEventIndex)));
            guard = Expression.AND(guard, new Assignment(result.newLocalVarName, new IntConstant(1)));

            for (int i = 0; i < exps.Count; i++)
            {
                if (exps[i] is IntConstant)
                {
                    //eventParameterVariables[i]' = exps[i]
                    guard = Expression.AND(guard, new Assignment(model.eventParameterVariables[i], exps[i]));
                }
                else
                {
                    //eventParameterVariables[i]' = exps[i]'
                    guard = Expression.AND(guard,
                                 Expression.EQ(new VariablePrime(model.eventParameterVariables[i]),
                                                          new VariablePrime(exps[i].expressionID)));

                }
            }

            //
            List<CUDDNode> transition = guard.TranslateBoolExpToBDD(model).GuardDDs;

            transition = CUDD.Function.And(transition, P1.GetInitInColumn(model));
            transition = model.AddVarUnchangedConstraint(transition, model.GlobalVarIndex);
            result.channelInTransitionBDD.AddRange(transition);

            //
            CopyTransitionAfterEventChannel(P1, model, result);
        }
    }
}
