using System.Collections.Generic;
using PAT.Common;
using PAT.Common.Classes.Assertion;
using PAT.Common.Classes.Expressions;
using PAT.Common.Classes.LTS;
using PAT.Common.Classes.Ultility;
using PAT.Common.Classes.ModuleInterface;
using PAT.Common.Classes.SemanticModels.LTS.Assertion;
using PAT.PN.LTS;

namespace PAT.PN.Assertions{
	public class Assertion
    {
        public static void Initialize(AssertionBase Assertion, PetriNet Process, SpecificationBase spec)
        {
            Specification Spec = spec as Specification;
            
            //get the relevant global variables; remove irrelevant variables so as to save memory;
            Valuation GlobalEnv = Spec.SpecValuation.GetClone();

            //Initialize InitialStep
            Assertion.InitialStep = new PNConfiguration(Process, Constants.INITIAL_EVENT, null, GlobalEnv, false, spec);
        }

        public static void Initialize(AssertionRefinement Assertion, DefinitionRef ImplementationProcess, DefinitionRef SpecificationProcess, SpecificationBase spec)
        {
            Specification Spec = spec as Specification;

            //get the relevant global variables; remove irrelevant variables so as to save memory;
            List<string> varList = ImplementationProcess.GetGlobalVariables();
            varList.AddRange(SpecificationProcess.GetGlobalVariables());

            Valuation GlobalEnv = Spec.SpecValuation.GetVariableChannelClone(varList, ImplementationProcess.GetChannels());

            //Initialize InitialStep
            Assertion.InitialStep = new PNConfiguration(ImplementationProcess, Constants.INITIAL_EVENT, null, GlobalEnv, false, spec);
            Assertion.InitSpecStep = new PNConfiguration(SpecificationProcess, Constants.INITIAL_EVENT, null, GlobalEnv, false, spec);

            Assertion.MustAbstract = ImplementationProcess.MustBeAbstracted();
            if (SpecificationProcess.MustBeAbstracted())
            {
                throw new ParsingException(
                    "Process " + Assertion.SpecProcess + " has infinite states and therefore can not be used as a property model for refinement checking!", Assertion.AssertToken);
            }
        }
    }
}
