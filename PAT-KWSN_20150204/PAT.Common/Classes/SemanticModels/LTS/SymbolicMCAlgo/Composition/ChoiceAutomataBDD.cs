using System.Collections.Generic;
using PAT.Common.Classes.CUDDLib;
using PAT.Common.Classes.Expressions.ExpressionClass;

namespace PAT.Common.Classes.SemanticModels.LTS.BDD
{
    public partial class AutomataBDD
    {
        /// <summary>
        /// Return AutomataBDD of the process which is composed of some choices
        /// This is a randome choice resolved by any (visible/invisible tau) event
        /// </summary>
        /// <param name="choices">List of AutomataBDD of choices</param>
        /// <param name="model"></param>
        /// <returns></returns>
        public static AutomataBDD Choice(List<AutomataBDD> choices, Model model)
        {
            AutomataBDD result = new AutomataBDD();

            ChoiceSetVariable(choices, model, result);
            ChoiceSetInit(choices, result);
            ChoiceEncodeTransition(choices, model, result);

            //
            return result;
        }


        /// <summary>
        /// P.var : [∪ {i = 1..n}Pi.var] ∪ {temp}
        /// </summary>
        private static void ChoiceSetVariable(List<AutomataBDD> choices, Model model, AutomataBDD result)
        {
            foreach (AutomataBDD choice in choices)
            {
                result.variableIndex.AddRange(choice.variableIndex);
            }
            result.newLocalVarName = Model.GetNewTempVarName();
            model.AddLocalVar(result.newLocalVarName, 0, choices.Count - 1);
            result.variableIndex.Add(model.GetNumberOfVars()- 1);
        }

        /// <summary>
        /// P.init : ∧{i = 1..n}Pi.init ∧ temp in Range[0, n-1]
        /// </summary>
        private static void ChoiceSetInit(List<AutomataBDD> choices, AutomataBDD result)
        {
            result.initExpression = choices[0].initExpression;
            for (int i = 1; i < choices.Count; i++)
            {
                result.initExpression = Expression.AND(result.initExpression, choices[i].initExpression);
            }

            result.initExpression = Expression.AND(result.initExpression,
                                                             new PrimitiveApplication(
                                                                 PrimitiveApplication.GREATER_EQUAL,
                                                                 new Variable(result.newLocalVarName),
                                                                 new IntConstant(0)));

            result.initExpression = Expression.AND(result.initExpression,
                                                 new PrimitiveApplication(
                                                     PrimitiveApplication.LESS_EQUAL,
                                                     new Variable(result.newLocalVarName),
                                                     new IntConstant(choices.Count - 1)));

        }

        /// <summary>
        /// (temp = i ∧ Pi.transition ∧ temp' = i)
        /// </summary>
        /// <param name="choices"></param>
        /// <param name="model"></param>
        /// <param name="result"></param>
        private static void ChoiceEncodeTransition(List<AutomataBDD> choices, Model model, AutomataBDD result)
        {
            for (int i = 0; i < choices.Count; i++)
            {
                Expression guard = Expression.AND(
                                                            Expression.EQ(
                                                                                     new Variable(result.newLocalVarName),
                                                                                     new IntConstant(i)),
                                                            new Assignment(result.newLocalVarName, new IntConstant(i)));

                List<CUDDNode> transition = guard.TranslateBoolExpToBDD(model).GuardDDs;

                //
                CUDD.Ref(transition);
                result.transitionBDD.AddRange(CUDD.Function.And(transition, choices[i].transitionBDD));

                CUDD.Ref(transition);
                result.channelInTransitionBDD.AddRange(CUDD.Function.And(transition, choices[i].channelInTransitionBDD));

                result.channelOutTransitionBDD.AddRange(CUDD.Function.And(transition, choices[i].channelOutTransitionBDD));                
            }
        }
    }
}
