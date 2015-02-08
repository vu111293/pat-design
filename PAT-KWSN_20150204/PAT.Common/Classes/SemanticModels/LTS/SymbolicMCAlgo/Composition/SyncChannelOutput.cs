using System.Collections.Generic;
using PAT.Common.Classes.CUDDLib;
using PAT.Common.Classes.Expressions.ExpressionClass;

namespace PAT.Common.Classes.SemanticModels.LTS.BDD
{
    public partial class AutomataBDD
    {
        /// <summary>
        /// Return the AutomataBDD of the Sync Channel Input. Encode this as event c!a.b.c and put it to ChannelIOutTransition
        /// </summary>
        /// <param name="channelEventIndex"></param>
        /// <param name="exps">List of output expressions of the channel</param>
        /// <param name="P1">AutomataBDD of process P1 after the channel input</param>
        /// <param name="model"></param>
        /// <returns></returns>
        public static AutomataBDD SyncChannelOutputPrefixing(int channelEventIndex, List<Expression> exps, AutomataBDD P1,
                                                            Model model)
        {
            AutomataBDD result = new AutomataBDD();
            result.newLocalVarName = Model.GetNewTempVarName();

            EventPrefixSetVariable(P1, model, result);
            EventPrefixSetInit(result);
            SyncChannelOutputEncodeTransition(channelEventIndex, exps, P1, model, result);

            //
            return result;
        }
      

        /// <summary>
        /// 
        /// </summary>
        /// <param name="channelEventIndex"></param>
        /// <param name="exps">List of expressions of channel in</param>
        /// <param name="P1">Process after channel in</param>
        /// <param name="model">Process after channel in</param>
        /// <param name="result"></param>
        private static void SyncChannelOutputEncodeTransition(int channelEventIndex, List<Expression> exps, AutomataBDD P1, Model model, AutomataBDD result)
        {
            Expression guard = Expression.EQ(new Variable(result.newLocalVarName), new IntConstant(0));

            Expression update = new Assignment(Model.EVENT_NAME, new IntConstant(channelEventIndex));

            
            for (int i = 0; i < exps.Count; i++)
            {
                //Update eventParameterVariables[i] = exps[i]
                //Don't need to update exps because later after synchronization, not updated variable keeps the same value
                update = new Sequence(update, new Assignment(model.eventParameterVariables[i], exps[i]));
            }

            update = new Sequence(update, new Assignment(result.newLocalVarName, new IntConstant(1)));

            List<CUDDNode> transition = model.EncodeTransition(guard, update, new List<int>());
            transition = CUDD.Function.And(transition, P1.GetInitInColumn(model));
            transition = model.AddVarUnchangedConstraint(transition, model.GlobalVarIndex);
            result.channelOutTransitionBDD.AddRange(transition);
            //
            CopyTransitionAfterEventChannel(P1, model, result);
        }
    }
}
