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
        /// This is a randome choice resolved by any (visible/invisible) event
        /// all choices do not need to take time transition
        /// the choice takes tick transition and idle there
        /// </summary>
        /// <param name="choices">List of AutomataBDD of choices</param>
        /// <param name="model"></param>
        /// <returns></returns>
        public static AutomataBDD Choice(List<AutomataBDD> choices, Model model)
        {
            AutomataBDD result = AutomataBDD.Choice(choices, model);
            ChoiceEncodeTick(choices, model, result);
            //
            return result;
        }

        /// <summary>
        /// [ REFS: '', DEREFS: 'choices.Ticks']
        /// </summary>
        /// <param name="choices"></param>
        /// <param name="model"></param>
        /// <param name="result"></param>
        private static void ChoiceEncodeTick(List<AutomataBDD> choices, Model model, AutomataBDD result)
        {
            //1.idle there
            List<CUDDNode> guardDD = new List<CUDDNode>() {GetTickTransEncoding(model)};
            guardDD = CUDD.Function.And(guardDD, result.initExpression.TranslateBoolExpToBDD(model).GuardDDs);

            guardDD = model.AddVarUnchangedConstraint(guardDD, result.variableIndex);
            guardDD = model.AddVarUnchangedConstraint(guardDD, model.GlobalVarIndex);
            result.Ticks.AddRange(guardDD);

            ResolvedChoiceEncodeTick(choices, model, result);
            
        }

        /// <summary>
        /// choice = i and Tick.i and choice' = i
        /// </summary>
        /// <param name="choices"></param>
        /// <param name="model"></param>
        /// <param name="result"></param>
        private static void ResolvedChoiceEncodeTick(List<AutomataBDD> choices, Model model, AutomataBDD result)
        {
            //2. choice = i and Tick.i and choice' = i
            for (int i = 0; i < choices.Count; i++)
            {
                Expression guard = Expression.AND(Expression.EQ(new Variable(result.newLocalVarName), new IntConstant(i)),
                        new Assignment(result.newLocalVarName, new IntConstant(i)));
                List<CUDDNode> guardDD = guard.TranslateBoolExpToBDD(model).GuardDDs;
                guardDD = CUDD.Function.And(guardDD, choices[i].Ticks);
                result.Ticks.AddRange(guardDD);
            }
        }
    }
}
