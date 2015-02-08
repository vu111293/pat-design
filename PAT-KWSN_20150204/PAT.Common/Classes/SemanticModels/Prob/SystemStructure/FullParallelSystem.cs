using System.Collections.Generic;

namespace PAT.Common.Classes.SemanticModels.Prob.SystemStructure
{
    /// <summary>
    /// Running in parallel, synchronize the common actions
    /// </summary>
    public class FullParallelSystem : SystemDef
    {
        private List<SystemDef> systems;

        public FullParallelSystem(List<SystemDef> s)
        {
            systems = s;
        }

        public int NumberSystem
        {
            get { return systems.Count; }
        }

        public SystemDef GetSystem(int index)
        {
            return systems[index];
        }
    }
}
