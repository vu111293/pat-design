using System.Collections.Generic;
using PAT.Common.Classes.CUDDLib;
using PAT.Common.Classes.Expressions.ExpressionClass;
using PAT.Common.Classes.SemanticModels.LTS.BDD;

namespace PAT.Common.Classes.SemanticModels.TTS
{
    public partial class TimeBehaviors
    {
        /// <summary>
        /// Return AutomataBDD of the Skip process
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static AutomataBDD Skip(Model model)
        {
            AutomataBDD result = AutomataBDD.Skip(model);
            SkipEncodeTick(model, result);
            //
            return result;
        }
        
        
        private static void SkipEncodeTick(Model model, AutomataBDD result)
        {
            //2. state = 0 & event' = tick & state = 0
            //3. state = 1 & event' = tick & state = 1
            Expression guard = Expression.AND(GetTickTransExpression(),
                            new Assignment(result.newLocalVarName, new Variable(result.newLocalVarName)));
            List<CUDDNode> guardDD = guard.TranslateBoolExpToBDD(model).GuardDDs;
            guardDD = model.AddVarUnchangedConstraint(guardDD, model.GlobalVarIndex);
            result.Ticks.AddRange(guardDD);
        }
    }
}
