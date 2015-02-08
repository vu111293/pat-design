using System.Collections.Generic;
using PAT.Common;
using PAT.Common.Classes.Assertion;
using PAT.Common.Classes.Expressions;
using PAT.Common.Classes.Expressions.ExpressionClass;
using PAT.Common.Classes.LTS;
using PAT.Common.Classes.ModuleInterface;
using PAT.Common.Classes.Ultility;
using PAT.Common.Classes.SemanticModels.LTS.Assertion;
using PAT.PN.LTS;

namespace PAT.PN.Assertions{
    public class PNAssertionReachabilityWith : AssertionReachabilityWith
    {
        private DefinitionRef Process;

        public PNAssertionReachabilityWith(DefinitionRef processDef, string reachableState, QueryConstraintType cont, Expression constraintCondition)
            : base(reachableState, cont, constraintCondition)
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
            varList.AddRange(ConstraintCondition.GetVars());                        

            Valuation GlobalEnv = Spec.SpecValuation.GetVariableChannelClone(varList, Process.GetChannels());
            
            //Initialize InitialStep
            InitialStep = new PNConfiguration(Process, Constants.INITIAL_EVENT, null, GlobalEnv, false);

            MustAbstract = Process.MustBeAbstracted();

            if (MustAbstract)
            {
                throw new ParsingException(
                    "Process " + StartingProcess +
                    " has infinite states and therefore can not be used to assert reachability with MIN/MAX constraints!",
                    AssertToken);

            }
        }
    }
}