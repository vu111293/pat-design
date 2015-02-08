using System.Collections.Generic;
using PAT.Common.Classes.CUDDLib;
using PAT.Common.Classes.Expressions.ExpressionClass;

namespace PAT.Common.Classes.SemanticModels.LTS.BDD
{
    public partial class AutomataBDD
    {
        /// <summary>
        /// Return AutomataBDD of the process which is composed of some choices
        /// Having tau transition to resolve the choice
        /// </summary>
        /// <param name="choices">List of AutomataBDD of choices</param>
        /// <param name="model"></param>
        /// <returns></returns>
        public static AutomataBDD InternalChoice(List<AutomataBDD> choices, Model model)
        {
            AutomataBDD result = new AutomataBDD();

            InternalChoiceSetVariable(choices, model, result);
            InternalChoiceSetInit(choices, result);
            InternalChoiceEncodeTransition(choices, model, result);

            //
            return result;
        }


        /// <summary>
        /// P.var : [∪ {i = 1..n}Pi.var] ∪ {temp}
        /// </summary>
        private static void InternalChoiceSetVariable(List<AutomataBDD> choices, Model model, AutomataBDD result)
        {
            foreach (AutomataBDD choice in choices)
            {
                result.variableIndex.AddRange(choice.variableIndex);
            }
            result.newLocalVarName = Model.GetNewTempVarName();
            model.AddLocalVar(result.newLocalVarName, -1, choices.Count - 1);
            result.variableIndex.Add(model.GetNumberOfVars()- 1);
        }

        /// <summary>
        /// P.init : ∧{i = 1..n}Pi.init and temp = -1 
        /// </summary>
        private static void InternalChoiceSetInit(List<AutomataBDD> choices, AutomataBDD result)
        {
            result.initExpression = Expression.EQ(new Variable(result.newLocalVarName), new IntConstant(-1));
            for (int i = 0; i < choices.Count; i++)
            {
                result.initExpression = Expression.AND(result.initExpression, choices[i].initExpression);
            }
        }

        /// <summary>
        /// (temp = -1 and tau and temp' in [0, n-1])
        /// (temp = i ∧ Pi.transition ∧ temp' = i)
        /// </summary>
        /// <param name="choices"></param>
        /// <param name="model"></param>
        /// <param name="result"></param>
        private static void InternalChoiceEncodeTransition(List<AutomataBDD> choices, Model model, AutomataBDD result)
        {
            //Tau transition resolves the choice
            Expression guard = Expression.AND(
                                                        Expression.AND(
                                                                                 new PrimitiveApplication(
                                                                                     PrimitiveApplication.AND,
                                                                                     new PrimitiveApplication(
                                                                                         PrimitiveApplication.EQUAL,
                                                                                         new Variable(result.newLocalVarName),
                                                                                         new IntConstant(-1)),
                                                                                     GetTauTransExpression()),
                                                                                 new PrimitiveApplication(
                                                                                     PrimitiveApplication.GREATER_EQUAL,
                                                                                     new VariablePrime(
                                                                                         result.newLocalVarName),
                                                                                     new IntConstant(0))),
                                                        Expression.LE(
                                                                                 new VariablePrime(result.newLocalVarName),
                                                                                 new IntConstant(choices.Count - 1)));
            List<CUDDNode> transition = guard.TranslateBoolExpToBDD(model).GuardDDs;

            model.AddVarUnchangedConstraint(transition, model.GlobalVarIndex);
            result.transitionBDD.AddRange(transition);

            //Copy transitions from other processes after the choice is resolved
            for (int i = 0; i < choices.Count; i++)
            {
                //(temp = i ∧ Pi.transition ∧ temp' = i)
                guard = Expression.AND(Expression.EQ(new Variable(result.newLocalVarName), new IntConstant(i)),
                                        new Assignment(result.newLocalVarName, new IntConstant(i)));

                transition = guard.TranslateBoolExpToBDD(model).GuardDDs;

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
