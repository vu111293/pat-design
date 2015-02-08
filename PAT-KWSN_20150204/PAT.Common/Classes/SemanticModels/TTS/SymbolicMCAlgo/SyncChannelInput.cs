using System.Collections.Generic;
using PAT.Common.Classes.Expressions.ExpressionClass;
using PAT.Common.Classes.SemanticModels.LTS.BDD;

namespace PAT.Common.Classes.SemanticModels.TTS
{
    public partial class TimeBehaviors
    {
        /// <summary>
        /// Return the AutomataBDD of the Channel Input
        /// </summary>
        /// <param name="channelName">Channel's name</param>
        /// <param name="channelEventIndex"></param>
        /// <param name="guard">Guard expression of the channel input</param>
        /// <param name="exps">List of input expression to the channel</param>
        /// <param name="guardOfTick"></param>
        /// <param name="P1">AutomataBDD of process P1 after the channel input</param>
        /// <param name="model"></param>
        /// <returns></returns>
        public static AutomataBDD SyncChannelInputPrefixing(int channelEventIndex, Expression guard, List<Expression> exps, Expression guardOfTick, AutomataBDD P1, Model model)
        {
            AutomataBDD result = AutomataBDD.SyncChannelInputPrefixing(channelEventIndex, guard, exps, P1, model);
            EventPrefixEncodeTick(guardOfTick, P1, model, result);

            //
            return result;
        }
    }
}
