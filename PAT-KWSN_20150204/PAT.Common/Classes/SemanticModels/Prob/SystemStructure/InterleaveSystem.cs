using System.Collections.Generic;

namespace PAT.Common.Classes.SemanticModels.Prob.SystemStructure
{
    /// <summary>
    /// Running interleaving, there is not synchronization
    /// </summary>
    public class InterleaveSystem : SystemDef
    {
        private List<SystemDef> systems; 

        public InterleaveSystem(List<SystemDef> s)
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
