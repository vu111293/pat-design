using System.Collections.Generic;
using PAT.Common.Classes.CUDDLib;

namespace PAT.Common.Classes.SemanticModels.Prob.SystemStructure
{
    public interface IModel
    {
        
    }

    public class ProbModel: IModel
    {
        public VariableList varList;

        /// <summary>
        /// And of all row-condition on all vars in range
        /// </summary>
        public CUDDNode allRowVarRanges;

        /// <summary>
        /// Identity of all variable
        /// </summary>
        public List<CUDDNode> varIdentities;

        /// <summary>
        /// Encoding of all variable
        /// </summary>
        public List<CUDDNode> varEncodings = new List<CUDDNode>();

        /// <summary>
        /// Prob transition matrix
        /// </summary>
        public CUDDNode trans;

        /// <summary>
        /// Init state
        /// </summary>
        public CUDDNode start;

        /// <summary>
        /// State reward of all reward structures
        /// </summary>
        public List<CUDDNode> stateRewards = new List<CUDDNode>();

        /// <summary>
        /// transition rewards of all reward structures
        /// </summary>
        public List<CUDDNode> transRewards = new List<CUDDNode>();

        /// <summary>
        /// All row vars
        /// </summary>
        public CUDDVars allRowVars;

        /// <summary>
        /// All col vars
        /// </summary>
        public CUDDVars allColVars;


        public ProbModel(CUDDNode trans, CUDDNode start, List<CUDDNode> stateRewards, List<CUDDNode> transRewards, CUDDVars allRowVars, CUDDVars allColVars,
                            VariableList varList, CUDDNode allRowVarRanges, List<CUDDNode> varIdentities, List<CUDDNode> varEncodings)
        {
            this.trans = trans;
            this.start = start;
            this.stateRewards = stateRewards;
            this.transRewards = transRewards;

            this.allRowVars = allRowVars;
            this.allColVars = allColVars;
            this.varList = varList;
            this.allRowVarRanges = allRowVarRanges;
            this.varIdentities = varIdentities;
            this.varEncodings = varEncodings;
        }
    }
}
