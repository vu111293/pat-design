using System.Collections.Generic;
using PAT.Common.Classes.Expressions.ExpressionClass;

namespace PAT.Common.Classes.SemanticModels.LTS.BDD
{
    public partial class AutomataBDD
    {
        /// <summary>
        /// Return the AutomataBDD of the Channel Output
        /// </summary>
        /// <param name="channelName">Channel's name</param>
        /// <param name="channelEventIndex"></param>
        /// <param name="exps">List of output expressions of the channel</param>
        /// <param name="P1"><AutomataBDD of process P1 after the channel input/param>
        /// <param name="model"></param>
        /// <returns></returns>
        public static AutomataBDD ChannelOutputPrefixing(string channelName, int channelEventIndex, List<Expression> exps, Expression assignmentExp, AutomataBDD P1, Model model)
        {
            AutomataBDD result = new AutomataBDD();

            EventPrefixSetVariable(P1, model, result);
            EventPrefixSetInit(result);
            ChannelOutputEncodeTransition(channelName, channelEventIndex, exps, assignmentExp, P1, model, result);

            //
            return result;
        }
      
        /// <summary>
        /// 
        /// </summary>
        /// <param name="channelName">The channel Name</param>
        /// <param name="channelEventIndex"></param>
        /// <param name="exps">List of expressions of channel in</param>
        /// <param name="P1">Process after channel in</param>
        /// <param name="result"></param>
        private static void ChannelOutputEncodeTransition(string channelName, int channelEventIndex, List<Expression> exps, Expression assignmentExp, AutomataBDD P1, Model model, AutomataBDD result)
        {
            List<Expression> guardUpdateChannel = GetGuardUpdateOfChannelOutput(channelName, exps, assignmentExp, model);

            //set update Model.Event_Name
            guardUpdateChannel[1] = new Sequence(guardUpdateChannel[1], new Assignment(Model.EVENT_NAME, new IntConstant(channelEventIndex)));

            EventPrefixEncodeTransition(guardUpdateChannel[0], guardUpdateChannel[1], P1, model, result);
        }

        /// <summary>
        /// Return guard, update of channel output
        /// guard: buffer not full
        /// update: buffer = sender and update size of message and update length of buffer and update top of buffer
        /// Does not include update Model.Event_Name
        /// </summary>
        /// <param name="channelName"></param>
        /// <param name="exps"></param>
        /// <param name="assignmentExp">null if not exist</param>
        /// <param name="model"></param>
        /// <returns></returns>
        private static List<Expression> GetGuardUpdateOfChannelOutput(string channelName, List<Expression> exps, Expression assignmentExp, Model model)
        {
            string topChannelVariable = Model.GetTopVarChannel(channelName);
            string countChannelVariable = Model.GetCountVarChannel(channelName);
            string sizeElementArray = Model.GetArrayOfSizeElementChannel(channelName);

            //count_a  < L
            Expression guardOfChannel = Expression.LT(new Variable(countChannelVariable), new IntConstant(model.mapChannelToSize[channelName]));

            Expression updateOfChannel = new BoolConstant(true);

            //Update buffer channel
            //a[top_a] [ i] = exps[i]
            for (int i = 0; i < exps.Count; i++)
            {
                updateOfChannel = new Sequence(updateOfChannel, new PropertyAssignment(new Variable(channelName),
                            Expression.PLUS(
                                Expression.TIMES(new Variable(topChannelVariable), new IntConstant(Model.MAX_MESSAGE_LENGTH)),
                                new IntConstant(i)),
                            exps[i]));
            }

            //Set size of the new element
            //size_a[top_a] = exps.count
            updateOfChannel = new Sequence(updateOfChannel, new PropertyAssignment(new Variable(sizeElementArray), new Variable(topChannelVariable), new IntConstant(exps.Count)));

            //Update size: count_a = count_a + 1
            updateOfChannel = new Sequence(updateOfChannel, new Assignment(countChannelVariable, Expression.PLUS(
                                                                            new Variable(countChannelVariable), new IntConstant(1))));
            //Update top position: top_a = (top_a + 1) %L
            updateOfChannel = new Sequence(updateOfChannel, new Assignment(topChannelVariable, Expression.MOD(
                            Expression.PLUS(new Variable(topChannelVariable), new IntConstant(1)), new IntConstant(model.mapChannelToSize[channelName]))));

            if(assignmentExp != null)
            {
                updateOfChannel = new Sequence(updateOfChannel, assignmentExp);
            }

            return new List<Expression>() { guardOfChannel, updateOfChannel };
        }
    }
}
