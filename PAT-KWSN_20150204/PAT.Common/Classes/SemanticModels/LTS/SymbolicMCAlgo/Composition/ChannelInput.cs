using System.Collections.Generic;
using PAT.Common.Classes.Expressions.ExpressionClass;

namespace PAT.Common.Classes.SemanticModels.LTS.BDD
{
    public partial class AutomataBDD
    {
        /// <summary>
        /// Return the AutomataBDD of the Channel Input
        /// Currently Channel Input is translated as assginment, not guard. We don't support using channel input to expect a certain value
        /// </summary>
        /// <param name="channelName">Channel's name</param>
        /// <param name="channelEventIndex"></param>
        /// <param name="guard">Guard expression of the channel input</param>
        /// <param name="exps">List of input expression to the channel</param>
        /// <param name="assignmetExp"></param>
        /// <param name="P1">AutomataBDD of process P1 after the channel input</param>
        /// <param name="model"></param>
        /// <returns></returns>
        public static AutomataBDD ChannelInputPrefixing(string channelName, int channelEventIndex, Expression guard, List<Expression> exps, Expression assignmetExp, AutomataBDD P1, Model model)
        {
            AutomataBDD result = new AutomataBDD();

            ChannelInputSetVariable(exps, P1, model, result);
            EventPrefixSetInit(result);
            ChannelInputEncodeTransition(channelName, channelEventIndex, guard, exps, assignmetExp, P1, model, result);

            //
            return result;
        }

        private static void ChannelInputSetVariable(List<Expression> exps, AutomataBDD P1, Model model, AutomataBDD result)
        {
            //Add new variable used in channel input
            foreach (var expression in exps)
            {
                if (!(expression is IntConstant) && !model.ContainsVar(expression.ExpressionID))
                {
                    model.AddLocalVar(expression.ExpressionID, Model.MIN_ELEMENT_BUFFER, Model.MAX_ELEMENT_BUFFER);
                    result.variableIndex.Add(model.GetNumberOfVars() - 1);
                }
            }

            EventPrefixSetVariable(P1, model, result);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="channelName">The channel Name</param>
        /// <param name="channelEventIndex"></param>
        /// <param name="guard">If no guard, give BoolConstant(true)</param>
        /// <param name="exps">List of expressions of channel out</param>
        /// <param name="assignmentExp">If no guard, give BoolConstant(true)</param>
        /// <param name="P1">Process after channel in</param>
        /// <param name="model"></param>
        /// <param name="result"></param>
        private static void ChannelInputEncodeTransition(string channelName, int channelEventIndex, Expression guard, List<Expression> exps, Expression assignmentExp, AutomataBDD P1, Model model, AutomataBDD result)
        {
            List<Expression> guardUpdateChannel = GetGuardUpdateOfChannelInput(channelName, guard, exps, assignmentExp, model);

            //set update Model.Event_Name
            guardUpdateChannel[1] = new Sequence(guardUpdateChannel[1], new Assignment(Model.EVENT_NAME, new IntConstant(channelEventIndex)));

            EventPrefixEncodeTransition(guardUpdateChannel[0], guardUpdateChannel[1], P1, model, result);
        }

        /// <summary>
        /// Return guard, update of channel input
        /// guard: not empty and same size (buffer and received) and guard and expected value in buffer
        /// update: received = buffer and length of buffer - 1
        /// Does not include the update of Event
        /// </summary>
        /// <param name="channelName"></param>
        /// <param name="guard"></param>
        /// <param name="exps"></param>
        /// <param name="assignmentExp">null if does not exist</param>
        /// <param name="model"></param>
        /// <returns></returns>
        private static List<Expression> GetGuardUpdateOfChannelInput(string channelName, Expression guard, List<Expression> exps, Expression assignmentExp, Model model)
        {
            string topChannelVariable = Model.GetTopVarChannel(channelName);
            string countChannelVariable = Model.GetCountVarChannel(channelName);
            string sizeElementArray = Model.GetArrayOfSizeElementChannel(channelName);

            Expression guardOfChannel, updateOfChannel = null;

            //Not empty channel buffer: count_a > 0
            Expression notEmptyChannel = Expression.GT(new Variable(countChannelVariable), new IntConstant(0));

            //The poped element size must have the same size of exps: size_a[top_a - count_a %L] == exps.count
            //(top_a - count_a) % L
            Expression popedElementPosition = new PrimitiveApplication(PrimitiveApplication.MOD,
                                                Expression.MINUS(new Variable(topChannelVariable), new Variable(countChannelVariable)),
                                                new IntConstant(model.mapChannelToSize[channelName]));

            Expression sameSize = Expression.EQ(
                                    new PrimitiveApplication(PrimitiveApplication.ARRAY, new Variable(sizeElementArray), popedElementPosition),
                                    new IntConstant(exps.Count));

            guardOfChannel = Expression.AND(notEmptyChannel, sameSize);
            guardOfChannel = Expression.AND(guardOfChannel, guard);

            //Assign data from buffer to exps
            //exps[i] = a[top_a - count_a % L][i]
            for (int i = 0; i < exps.Count; i++)
            {
                //Expect value in the channel
                if (exps[i] is IntConstant)
                {
                    //a[top_a - count_a % L][i] == exps[i]
                    guardOfChannel = Expression.AND(guardOfChannel, Expression.EQ(
                                        new PrimitiveApplication(PrimitiveApplication.ARRAY, new Variable(channelName), Expression.PLUS(
                                            Expression.TIMES(popedElementPosition, new IntConstant(Model.MAX_MESSAGE_LENGTH)),
                                            new IntConstant(i))),
                                        exps[i]));
                }
                else
                {
                    //receive value from buffer
                    updateOfChannel = Expression.CombineProgramBlock(updateOfChannel, new Assignment(exps[i].expressionID,
                                                                                                new PrimitiveApplication(PrimitiveApplication.ARRAY, new Variable(channelName),
                                                                                                                         Expression.PLUS(
                                                                                                                                                  new PrimitiveApplication(
                                                                                                                                                      PrimitiveApplication.TIMES,
                                                                                                                                                      popedElementPosition,
                                                                                                                                                      new IntConstant(
                                                                                                                                                          Model.MAX_MESSAGE_LENGTH)),
                                                                                                                                                  new IntConstant(i)))));
                }

            }

            //Update size: count_a = count_a - 1
            updateOfChannel = Expression.CombineProgramBlock(updateOfChannel,
                                                        new Assignment(countChannelVariable,
                                                                       Expression.MINUS(new Variable(countChannelVariable), new IntConstant(1))));
            updateOfChannel = Expression.CombineProgramBlock(updateOfChannel, assignmentExp);

            return new List<Expression>() { guardOfChannel, updateOfChannel };
        }
    }
}
