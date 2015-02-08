using System.Collections.Generic;
using PAT.Common.Classes.CUDDLib;

namespace PAT.Common.Classes.SemanticModels.LTS.BDD
{
    public partial class AutomataBDD
    {
        /// <summary>
        /// Return AutomataBDD of the hiding process
        /// [ REFS: 'none', DEREFS: 'hidingEvent, tauEvent, P1' ]
        /// </summary>
        /// <param name="hidingEvent">CUDDNode of hiding events</param>
        /// <param name="tauEvent">CUDDNode of tau event</param>
        /// <param name="P1">AutomataBDD of the process P1</param>
        /// <param name="model"></param>
        /// <returns></returns>
        public static AutomataBDD Hiding(CUDDNode hidingEvent, CUDDNode tauEvent, AutomataBDD P1, Model model)
        {
            CUDD.Ref(P1.transitionBDD);
            CUDD.Ref(hidingEvent);
            List<CUDDNode> hidingTransition = CUDD.Function.And(P1.transitionBDD, hidingEvent);

            hidingTransition = CUDD.Abstract.ThereExists(hidingTransition, model.GetEventColVars());
            CUDD.Ref(tauEvent);
            hidingTransition = CUDD.Function.And(hidingTransition, tauEvent);

            //
            CUDD.Ref(P1.transitionBDD);
            CUDD.Ref(hidingEvent);
            List<CUDDNode> notHidingTransition = CUDD.Function.And(P1.transitionBDD, CUDD.Function.Not(hidingEvent));

            //
            CUDD.Deref(hidingEvent, tauEvent);
            CUDD.Deref(P1.transitionBDD);
            P1.transitionBDD.Clear();

            P1.transitionBDD.AddRange(hidingTransition);
            P1.transitionBDD.AddRange(notHidingTransition);

            return P1;
        }
    }
}
