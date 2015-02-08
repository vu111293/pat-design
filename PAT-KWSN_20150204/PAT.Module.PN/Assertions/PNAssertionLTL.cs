using System.Collections.Generic;
using PAT.Common.Classes.Assertion;
using PAT.Common.Classes.Expressions;
using PAT.Common.Classes.Expressions.ExpressionClass;
using PAT.Common.Classes.LTS;
using PAT.Common.Classes.ModuleInterface;
using PAT.Common.Classes.Ultility;
using PAT.Common.Classes.SemanticModels.LTS.Assertion;
using PAT.PN.LTS;

namespace PAT.PN.Assertions{
    public class PNAssertionLTL : AssertionLTL
    {
        private DefinitionRef Process;

        public PNAssertionLTL(DefinitionRef processDef, string ltl) : base(ltl)
        {
            Process = processDef;
        }

        public override void Initialize(SpecificationBase spec)
        {
            Specification Spec = spec as Specification;

            List<string> varList = Process.GetGlobalVariables();

            BA.Initialize(Spec.DeclarationDatabase);

            foreach (KeyValuePair<string, Expression> pair in BA.DeclarationDatabase)
            {
                varList.AddRange(pair.Value.GetVars());
            }

            // editted by Tinh
            Valuation GlobalEnv = Spec.SpecValuation.GetClone();

            InitialStep = new PNConfiguration(Process, Constants.INITIAL_EVENT, null, GlobalEnv, false, spec);

            MustAbstract = Process.MustBeAbstracted();

            base.Initialize(spec);
        }

        public override string StartingProcess
        {
            get
            {
                return Process.ToString();
            }
        }
    }
}