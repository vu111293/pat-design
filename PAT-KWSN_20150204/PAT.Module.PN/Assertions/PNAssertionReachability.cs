using System.Collections.Generic;
using PAT.Common.Classes.Assertion;
using PAT.Common.Classes.Expressions;
using PAT.Common.Classes.LTS;
using PAT.Common.Classes.ModuleInterface;
using PAT.Common.Classes.Ultility;
using PAT.Common.Classes.SemanticModels.LTS.Assertion;
using PAT.PN.LTS;

namespace PAT.PN.Assertions{
    public partial class PNAssertionReachability : AssertionReachability
    {
        private DefinitionRef Process;

        public PNAssertionReachability(DefinitionRef processDef, string reachableState) : base(reachableState)
        {
            Process = processDef;
        }

        public override string StartingProcess
        {
            get
            {
                return Process.ToString();
            }
        }

        public override void Initialize(SpecificationBase spec)
        {
            //initialize the ModelCheckingOptions
            base.Initialize(spec);
            
            Specification Spec = spec as Specification;
            ReachableStateCondition = Spec.DeclarationDatabase[ReachableStateLabel];

            List<string> varList = Process.GetGlobalVariables();
            varList.AddRange(ReachableStateCondition.GetVars());
                                    
            // Valuation GlobalEnv = Spec.SpecValuation.GetVariableChannelClone(varList, Process.GetChannels());
            // editted by Tinh
            Valuation GlobalEnv = Spec.SpecValuation.GetClone();
            
            //Initialize InitialStep
            // editted by Tinh
            InitialStep = new PNConfiguration(Process, Constants.INITIAL_EVENT, null, GlobalEnv, false, spec);
            MustAbstract = Process.MustBeAbstracted();
        }    
    }
}