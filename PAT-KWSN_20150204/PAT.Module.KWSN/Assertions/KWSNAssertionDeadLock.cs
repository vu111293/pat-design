using PAT.Common.Classes.Assertion;
using PAT.Common.Classes.LTS;
using PAT.Common.Classes.ModuleInterface;
using PAT.Common.Classes.SemanticModels.LTS.Assertion;
using PAT.KWSN.LTS;

namespace PAT.KWSN.Assertions
{
    public class KWSNAssertionDeadLock : AssertionDeadLock
    {
        private DefinitionRef Process;
        public KWSNAssertionDeadLock(DefinitionRef processDef)
            : base()
        {
            Process = processDef;
        }

        public KWSNAssertionDeadLock(DefinitionRef processDef, bool isNontermination)
            : base(isNontermination)
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
            get { return Process.ToString(); }
        }

        //todo: override ToString method if your assertion uses different syntax as PAT
        //public override string ToString()
        //{
        //		return "";
        //}
    }
}