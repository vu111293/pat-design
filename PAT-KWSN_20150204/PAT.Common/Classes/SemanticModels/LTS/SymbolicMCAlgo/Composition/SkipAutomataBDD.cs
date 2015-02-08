using PAT.Common.Classes.Expressions.ExpressionClass;

namespace PAT.Common.Classes.SemanticModels.LTS.BDD
{
    public partial class AutomataBDD
    {
        /// <summary>
        /// Return AutomataBDD of the Skip process
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static AutomataBDD Skip(Model model)
        {
            AutomataBDD result = new AutomataBDD();
            
            SkipSetVariable(model, result);
            SkipSetInit(result);
            SkipEncodeTransition(model, result);

            //
            return result;
        }
        
        /// <summary>
        /// P.var = {temp}
        /// </summary>
        private static void SkipSetVariable(Model model, AutomataBDD result)
        {
            result.newLocalVarName = Model.GetNewTempVarName();
            model.AddLocalVar(result.newLocalVarName, 0, 1);
            result.variableIndex.Add(model.GetNumberOfVars() - 1);
        }

        /// <summary>
        /// P.init: !temp'
        /// </summary>
        private static void SkipSetInit(AutomataBDD result)
        {
            result.initExpression = Expression.EQ(new Variable(result.newLocalVarName), new IntConstant(0));
        }

        /// <summary>
        /// (!temp ∧ event ' = terminate ∧ temp ')
        /// </summary>
        private static void SkipEncodeTransition(Model model, AutomataBDD result)
        {
            Expression guard = Expression.AND(
                                                        Expression.AND(
                                                                                 new PrimitiveApplication(
                                                                                     PrimitiveApplication.EQUAL,
                                                                                     new Variable(result.newLocalVarName),
                                                                                     new IntConstant(0)),
                                                                                 GetTerminateTransExpression()),
                                                        new Assignment(result.newLocalVarName, new IntConstant(1)));

            result.transitionBDD.AddRange(model.AddVarUnchangedConstraint(guard.TranslateBoolExpToBDD(model).GuardDDs, model.GlobalVarIndex));
        }
    }
}
