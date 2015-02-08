using System.Collections.Generic;
using PAT.Common.Classes.CUDDLib;
using PAT.Common.Classes.Expressions.ExpressionClass;

namespace PAT.Common.Classes.SemanticModels.LTS.BDD
{
    public partial class AutomataBDD
    {
        /// <summary>
        /// Return AutomataBDD of the process which is composed of some choices
        /// Choice is not resolved until not tau transition happens
        /// </summary>
        /// <param name="choices">List of AutomataBDD of choices</param>
        /// <param name="model"></param>
        /// <returns></returns>
        public static AutomataBDD ExternalChoice(List<AutomataBDD> choices, Model model)
        {
            AutomataBDD result = new AutomataBDD();

            ExternalChoiceSetVariable(choices, model, result);
            ExternalChoiceSetInit(choices, result);
            ExternalChoiceEncodeTransition(choices, model, result);

            //
            return result;
        }


        /// <summary>
        /// P.var : [∪ {i = 1..n}Pi.var] ∪ {temp}
        /// </summary>
        private static void ExternalChoiceSetVariable(List<AutomataBDD> choices, Model model, AutomataBDD result)
        {
            foreach (AutomataBDD choice in choices)
            {
                result.variableIndex.AddRange(choice.variableIndex);
            }
            result.newLocalVarName = Model.GetNewTempVarName();
            model.AddLocalVar(result.newLocalVarName, -1, choices.Count - 1);
            result.variableIndex.Add(model.GetNumberOfVars() - 1);
        }

        /// <summary>
        /// P.init : ∧{i = 1..n}Pi.init and temp = -1
        /// </summary>
        private static void ExternalChoiceSetInit(List<AutomataBDD> choices, AutomataBDD result)
        {
            result.initExpression = choices[0].initExpression;
            for (int i = 1; i < choices.Count; i++)
            {
                result.initExpression = Expression.AND(result.initExpression, choices[i].initExpression);
            }

            result.initExpression = Expression.AND(result.initExpression,
                                        Expression.EQ(new Variable(result.newLocalVarName), new IntConstant(-1)));
        }

        /// <summary>
        /// Tau transition ∧ ((temp = -1 and temp' = -1 and other unchanged) or (temp = i and temp' = i)
        /// (temp = i or temp = -1) and Not Tau Transition ∧ temp' = i;
        /// If Tau transitions happens first then temp still not initialized.
        /// After Not Tau Transition happen, then temp is initialize. Later although Tau transition can happen, this selection is not changed.
        /// [ REFS: 'none', DEREFS: 'choices, tauEvent' ]
        /// </summary>
        /// <param name="choices"></param>
        /// <param name="model"></param>
        /// <param name="result"></param>
        private static void ExternalChoiceEncodeTransition(List<AutomataBDD> choices, Model model, AutomataBDD result)
        {
            for (int i = 0; i < choices.Count; i++)
            {
                //2. (temp = i or temp = -1) and Not Tau Transition ∧ temp' = i
                Expression guard = Expression.OR(
                                                            Expression.EQ(
                                                                                     new Variable(result.newLocalVarName),
                                                                                     new IntConstant(-1)),
                                                            Expression.EQ(
                                                                                     new Variable(result.newLocalVarName),
                                                                                     new IntConstant(i)));

                guard = Expression.AND(guard,
                                                 new Assignment(result.newLocalVarName, new IntConstant(i)));

                List<CUDDNode> transition = guard.TranslateBoolExpToBDD(model).GuardDDs;

                
                CUDD.Ref(transition);
                List<CUDDNode> notTauTransition = CUDD.Function.And(transition, CUDD.Function.Not(GetTauTransEncoding(model)));

                //
                CUDD.Ref(choices[i].transitionBDD);
                result.transitionBDD.AddRange(CUDD.Function.And(choices[i].transitionBDD, notTauTransition));

                //Channel communication vacuously is not tau transition
                CUDD.Ref(transition);
                result.channelInTransitionBDD.AddRange(CUDD.Function.And(choices[i].channelInTransitionBDD, transition));

                result.channelOutTransitionBDD.AddRange(CUDD.Function.And(choices[i].channelOutTransitionBDD, transition));


                //1. Tau transition ∧ ((temp = -1 and temp' = -1 and other unchanged) or (temp = i and temp' = i)
                guard = Expression.OR(
                                                 Expression.AND(
                                                                          new PrimitiveApplication(
                                                                              PrimitiveApplication.EQUAL,
                                                                              new Variable(result.newLocalVarName),
                                                                              new IntConstant(-1)),
                                                                          new Assignment(result.newLocalVarName, new IntConstant(-1))),
                                                 Expression.AND(
                                                                          new PrimitiveApplication(
                                                                              PrimitiveApplication.EQUAL,
                                                                              new Variable(result.newLocalVarName),
                                                                              new IntConstant(i)),
                                                                          new Assignment(result.newLocalVarName, new IntConstant(-1))));


                transition = guard.TranslateBoolExpToBDD(model).GuardDDs;

                List<CUDDNode> tauTransition = CUDD.Function.And(choices[i].transitionBDD, GetTauTransEncoding(model));
                transition = CUDD.Function.And(transition, tauTransition);


                //already includes temp
                List<int> unchangedVariableIndex = new List<int>(result.variableIndex);
                foreach (int index in choices[i].variableIndex)
                {
                    unchangedVariableIndex.Remove(index);
                }
                model.AddVarUnchangedConstraint(transition, unchangedVariableIndex);

                result.transitionBDD.AddRange(transition);
            }

        }

    }
}
