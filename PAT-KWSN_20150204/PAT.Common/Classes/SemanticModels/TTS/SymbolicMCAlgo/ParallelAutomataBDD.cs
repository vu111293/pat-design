using System.Collections.Generic;
using PAT.Common.Classes.CUDDLib;
using PAT.Common.Classes.SemanticModels.LTS.BDD;

namespace PAT.Common.Classes.SemanticModels.TTS
{
    public partial class TimeBehaviors
    {
        /// <summary>
        /// Return AutomataBDD of Parallel process.
        /// Note: synchronized is created by AND of transition of participant process. Therefore this only happens when the transition does not change any global variable
        /// because when encoding transition, each transition will make variable unchanged if it is not updated in that transition. This synchronization is similar to 
        /// Explicit model checking when does not allow synchronized transition having program block.
        /// </summary>
        /// <param name="processes">List of AutomataBDD of parallel processes</param>
        /// <param name="model"></param>
        /// <returns></returns>
        public static AutomataBDD Parallel(List<AutomataBDD> processes, List<CUDDNode> alphabets, Model model)
        {
            AutomataBDD result = AutomataBDD.Parallel(processes, alphabets, model);
            ParallelEncodeTick(processes, model, result);

            //
            return result;
        }

        private static void ParallelEncodeTick(List<AutomataBDD> processes, Model model, AutomataBDD result)
        {
            //Convert tick-transitions as single BDD
            foreach (AutomataBDD process in processes)
            {
                process.Ticks = new List<CUDDNode>() { CUDD.Function.Or(process.Ticks) };
            }


            List<CUDDNode> tickTrans = processes[0].Ticks;
            for (int i = 1; i < processes.Count; i++)
            {
                tickTrans = CUDD.Function.And(tickTrans, processes[i].Ticks);
            }

            result.Ticks.AddRange(tickTrans);
        }
    }
}
