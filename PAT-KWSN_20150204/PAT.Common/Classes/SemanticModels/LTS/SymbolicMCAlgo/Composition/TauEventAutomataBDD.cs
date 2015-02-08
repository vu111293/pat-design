using PAT.Common.Classes.Expressions.ExpressionClass;

namespace PAT.Common.Classes.SemanticModels.LTS.BDD
{
    public partial class AutomataBDD
    {
        /// <summary>
        /// Return AutomataBDD of the tau event prefix process with guard [b] tau -> P1.
        /// We separate this since in RTS, tau transition has higher priority and must not be delayed
        /// </summary>
        /// <param name="guard">Guard of the event</param>
        /// <param name="tauEventExp">Update command happening with the event</param>
        /// <param name="P1">AutomataBDD of the process P1 engaging after the event</param>
        /// <param name="model"></param>
        /// <returns></returns>
        public static AutomataBDD TauEventPrefix(Expression guard, Expression tauEventExp, AutomataBDD P1, Model model)
        {
            return EventPrefix(guard, tauEventExp, P1, model);
        }
    }
}
