using System.Collections.Generic;
using PAT.Common.Classes.Expressions.ExpressionClass;
using PAT.Common.Classes.SemanticModels.LTS.BDD;

namespace PAT.Common.Classes.SemanticModels.TTS
{
    public partial class TimeBehaviors
    {
        /// <summary>
        /// Return the AutomataBDD of the Channel Output
        /// </summary>
        /// <param name="channelName">Channel's name</param>
        /// <param name="channelEventIndex"></param>
        /// <param name="exps">List of output expressions of the channel</param>
        /// <param name="guardOfTick"></param>
        /// <param name="P1"><AutomataBDD of process P1 after the channel input/param>
        /// <param name="model"></param>
        /// <returns></returns>
        public static AutomataBDD SyncChannelOutputPrefixing(int channelEventIndex, List<Expression> exps, Expression guardOfTick, AutomataBDD P1, Model model)
        {
            AutomataBDD result = AutomataBDD.SyncChannelOutputPrefixing(channelEventIndex, exps, P1, model);
            EventPrefixEncodeTick(guardOfTick, P1, model, result);

            //
            return result;
        }
    }
}
