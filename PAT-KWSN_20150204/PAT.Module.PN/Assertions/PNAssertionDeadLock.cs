using PAT.Common.Classes.Assertion;
using PAT.Common.Classes.LTS;
using PAT.Common.Classes.ModuleInterface;
using PAT.Common.Classes.SemanticModels.LTS.Assertion;
using PAT.PN.LTS;

namespace PAT.PN.Assertions{
    public class PNAssertionDeadLock : AssertionDeadLock
    {
        private PetriNet Process;              

        public PNAssertionDeadLock(PetriNet processDef) : base()
        {
            Process = processDef;
        }

        public PNAssertionDeadLock(PetriNet processDef, bool isNontermination) : base(isNontermination)
        {
            Process = processDef;
        }

        public override void Initialize(SpecificationBase spec)
        {
            //initialize the ModelCheckingOptions
            base.Initialize(spec);
            
            Assertion.Initialize(this, Process, spec);
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