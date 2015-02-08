using System.Collections.Generic;
using PAT.Common.Classes.CUDDLib;
using PAT.Common.Classes.Expressions.ExpressionClass;
using PAT.Common.Classes.SemanticModels.LTS.BDD;

namespace PAT.Common.Classes.SemanticModels.TTS
{
    public partial class TimeBehaviors
    {
        /// <summary>
        /// Return AutomataBDD of the guard process
        /// </summary>
        /// <param name="guard">The guard expression of the process P1</param>
        /// <param name="P1">AutomataBDD of the process P1</param>
        /// <param name="model"></param>
        /// <returns></returns>
        public static AutomataBDD Guard(Expression guard, AutomataBDD P1, Model model)
        {
            AutomataBDD result = AutomataBDD.Guard(guard, P1, model);
            GuardEncodeTick(guard, P1, model, result);
            //
            return result;
        }

        /// <summary>
        /// Note that this is different from EventPrefixEncodeTick because it need to make sure local variable unchanged
        /// </summary>
        /// <param name="P1"></param>
        /// <param name="model"></param>
        /// <param name="result"></param>
        private static void GuardEncodeTick(Expression guard1, AutomataBDD P1, Model model, AutomataBDD result)
        {
            //allow time evolution when the guard is not resolved
            //temp = 0 and P1.init and not (b and P1.init and event = tau and P1.transition) and event = tick and temp = 0
            Expression exp;
            List<CUDDNode> expDD;
            exp = Expression.AND(guard1,
                        Expression.AND(P1.initExpression,
                            AutomataBDD.GetTauTransExpression()));
            expDD = exp.TranslateBoolExpToBDD(model).GuardDDs;

            CUDD.Ref(P1.transitionBDD);
            expDD = CUDD.Function.And(expDD, P1.transitionBDD);

            //find state where b is true and tau is enable
            expDD = CUDD.Abstract.ThereExists(expDD, model.AllColVars);

            CUDDNode tauIsNotEnabled = CUDD.Function.Not(expDD);

            exp = Expression.AND(
                                             Expression.AND(
                                                                      new PrimitiveApplication(
                                                                          PrimitiveApplication.EQUAL,
                                                                          new Variable(result.newLocalVarName),
                                                                          new IntConstant(0)),
                                                                      P1.initExpression),
                                             GetTickTransExpression());
            expDD = exp.TranslateBoolExpToBDD(model).GuardDDs;

            expDD = CUDD.Function.And(expDD, tauIsNotEnabled);


            expDD = model.AddVarUnchangedConstraint(expDD, model.GlobalVarIndex);
            expDD = model.AddVarUnchangedConstraint(expDD, result.variableIndex);

            result.Ticks.AddRange(expDD);

            //temp & P1.tick & temp'
            exp = Expression.AND(
                                new Variable(result.newLocalVarName), new Assignment(result.newLocalVarName, new IntConstant(1)));
            expDD = exp.TranslateBoolExpToBDD(model).GuardDDs;

            expDD = CUDD.Function.And(P1.Ticks, expDD);
            result.Ticks.AddRange(expDD);
        }
    }
}
