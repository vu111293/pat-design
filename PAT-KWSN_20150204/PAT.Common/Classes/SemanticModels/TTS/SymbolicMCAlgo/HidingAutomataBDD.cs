using System.Collections.Generic;
using PAT.Common.Classes.CUDDLib;
using PAT.Common.Classes.SemanticModels.LTS.BDD;

namespace PAT.Common.Classes.SemanticModels.TTS
{
    public partial class TimeBehaviors
    {
        /// <summary>
        /// Return TimedAutomataBDD of the hiding process
        /// [ REFS: 'none', DEREFS: 'hidingEvent, tauEvent, P1' ]
        /// </summary>
        /// <param name="hidingEvent">CUDDNode of hiding events</param>
        /// <param name="tauEvent">CUDDNode of tau event</param>
        /// <param name="P1">TimedAutomataBDD of the process P1</param>
        /// <param name="model"></param>
        /// <returns></returns>
        public static TimedAutomataBDD Hiding(CUDDNode hidingEvent, CUDDNode tauEvent, TimedAutomataBDD P1, Model model)
        {
            return (TimedAutomataBDD) AutomataBDD.Hiding(hidingEvent, tauEvent, P1, model);
        }
    }
}
