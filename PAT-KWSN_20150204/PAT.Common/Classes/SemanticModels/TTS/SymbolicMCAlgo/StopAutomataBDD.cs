using System.Collections.Generic;
using PAT.Common.Classes.CUDDLib;
using PAT.Common.Classes.Expressions.ExpressionClass;
using PAT.Common.Classes.SemanticModels.LTS.BDD;

namespace PAT.Common.Classes.SemanticModels.TTS
{
    public partial class TimeBehaviors
    {
        /// <summary>
        /// Return AutomataBDD of Stop process
        /// </summary>
        /// <returns></returns>
        public static AutomataBDD Stop(Model model)
        {
            AutomataBDD result = new AutomataBDD();

            Expression guard = GetTickTransExpression();
            List<CUDDNode> guardDD = guard.TranslateBoolExpToBDD(model).GuardDDs;
            guardDD = model.AddVarUnchangedConstraint(guardDD, model.GlobalVarIndex);
            result.Ticks.AddRange(guardDD);

            return result;
        }
    }
}
