using System.Collections.Generic;
using PAT.Common.Classes.CUDDLib;
using PAT.Common.Classes.Expressions.ExpressionClass;
using PAT.Common.Classes.SemanticModels.LTS.BDD;

namespace PAT.Common.Classes.SemanticModels.TTS
{
    public partial class TimeBehaviors
    {
        /// <summary>
        /// Return AutomataBDD of the process which is composed of some choices
        /// tick transition could not resolve the choice
        /// </summary>
        /// <param name="choices">List of AutomataBDD of choices</param>
        /// <param name="tauEvent">Event' = Tau Event Index</param>
        /// <param name="model"></param>
        /// <returns></returns>
        public static AutomataBDD IndexChoice(List<AutomataBDD> choices, Model model)
        {
            AutomataBDD result = AutomataBDD.Choice(choices, model);

            IndexChoiceEncodeTick(choices, model, result);
            //
            return result;
        }

        /// <summary>
        /// [ REFS: '', DEREFS: '']
        /// </summary>
        /// <param name="choices"></param>
        /// <param name="model"></param>
        /// <param name="result"></param>
        private static void IndexChoiceEncodeTick(List<AutomataBDD> choices, Model model, AutomataBDD result)
        {
            //if all components of the choice may evolve for a particular length of time, then so may the choice
            //1. choice = -1 and all Tick.i and choice' = -1
            Expression guard = Expression.AND(Expression.EQ(new Variable(result.newLocalVarName), new IntConstant(-1)),
                        new Assignment(result.newLocalVarName, new IntConstant(-1)));
            List<CUDDNode> guardDD = guard.TranslateBoolExpToBDD(model).GuardDDs;
            for (int i = 0; i < choices.Count; i++)
            {
                CUDD.Ref(choices[i].Ticks);
                guardDD = CUDD.Function.And(guardDD, choices[i].Ticks);
            }
            result.Ticks.AddRange(guardDD);

            //2. choice = i and Tick.i and choice' = i
            ResolvedChoiceEncodeTick(choices, model, result);
        }
    }
}
