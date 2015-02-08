using System.Collections.Generic;
using PAT.Common.Classes.SemanticModels.LTS.BDD;

namespace PAT.Common.Classes.SemanticModels.TTS
{
    public partial class TimeBehaviors
    {
        /// <summary>
        /// Return AutomataBDD of the process which is composed of some choices
        /// </summary>
        /// <param name="choices">List of AutomataBDD of choices</param>
        /// <param name="model"></param>
        /// <returns></returns>
        public static AutomataBDD InternalChoice(List<AutomataBDD> choices, Model model)
        {
            AutomataBDD result = AutomataBDD.InternalChoice(choices, model);

            ResolvedChoiceEncodeTick(choices, model, result);

            //
            return result;
        }
    }
}
