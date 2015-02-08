using System;
using System.Collections.Generic;

namespace PAT.Common.Classes.SemanticModels.Prob.SystemStructure
{
    /// <summary>
    /// Running in parallel and synchronized the interface
    /// </summary>
    public class ParallelSystem: SystemDef
    {
        public SystemDef system1;
        public SystemDef system2;

        /// <summary>
        /// Actions in syncActions must be synchronized by system1 and system2
        /// </summary>
        public List<string> syncActions;

        public  ParallelSystem(SystemDef s1, SystemDef s2, List<string> actions)
        {
            system1 = s1;
            system2 = s2;
            syncActions = actions;
        }

        /// <summary>
        /// Check whether an action is contained in the Parallel System
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public bool ContainsAction(String action)
        {
            return syncActions.Contains(action);
        }
    }
}
