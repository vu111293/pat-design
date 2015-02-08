using System.Collections.Generic;
using PAT.Common.Classes.CUDDLib;
using PAT.Common.Classes.SemanticModels.LTS.BDD;

namespace PAT.Common.Classes.SemanticModels.TTS
{
    public partial class TimeBehaviors
    {
        /// <summary>
        /// Return AutomataBDD of Interleave process
        /// </summary>
        /// <param name="processes">List of AutomataBDD of interleaving processes</param>
        /// <param name="model"></param>
        /// <returns></returns>
        public static AutomataBDD Interleave(List<AutomataBDD> processes, Model model)
        {
            AutomataBDD result = AutomataBDD.Interleave(processes, model);
            ParallelEncodeTick(processes, model, result);

            //Combine simulation relation
            result.SimulationRel = CUDD.Constant(1);
            foreach (var automataBdd in processes)
            {
                result.SimulationRel = CUDD.Function.And(result.SimulationRel, automataBdd.SimulationRel);
            }

            result.SimulationRel = model.AddVarUnchangedConstraint(result.SimulationRel, model.GlobalVarIndex);

            //
            return result;
        }
    }
}
