using System.Collections.Generic;
using PAT.Common.Classes.CUDDLib;

namespace PAT.Common.Classes.SemanticModels.Prob.SystemStructure
{
    public class NonDetModel: ProbModel
    {
        /// <summary>
        /// Vars for nondeterminisitic choices
        /// </summary>
       public CUDDVars allNondetVars;
       
        public NonDetModel(CUDDNode trans, CUDDNode start, List<CUDDNode> stateRewards, List<CUDDNode> transReward, CUDDVars allRowVars, CUDDVars allColVars, CUDDVars allNondetVars,
                    VariableList varList, CUDDNode allRowVarRanges, List<CUDDNode> varIdentities, List<CUDDNode> variableEncodings)
            : base(trans, start, stateRewards, transReward, allRowVars, allColVars, varList, allRowVarRanges, varIdentities, variableEncodings)
        {
            this.allNondetVars = allNondetVars;
        }

    }
}
