using PAT.Common.Classes.Assertion;
using PAT.Common.Classes.ModuleInterface;
using PAT.Common.Classes.Ultility;

namespace PAT.Common.Classes.SemanticModels.MDP.Assertion
{
    public abstract partial class AssertionLTL 
    {
        /// <summary>
        /// Run the verification and get the result.
        /// </summary>
        /// <returns></returns>
        public override void RunVerificationSafety()
        {
            if (ConstraintType == QueryConstraintType.NONE)
            {
                System.Diagnostics.Debug.Assert(false);
                base.RunVerificationSafety();
                return;
            }

            BuildMDPSafety(); 
            if (!CancelRequested)
            {
                switch (ConstraintType)
                {
                    case QueryConstraintType.PROB:
                        Max = 1 - mdp.MinProbability(VerificationOutput);
                        mdp.ResetNonTargetState();
                        Min = 1 - mdp.MaxProbability(VerificationOutput);
                        break;
                    case QueryConstraintType.PMAX:
                        Max = 1 - mdp.MinProbability(VerificationOutput);
                        break;
                    case QueryConstraintType.PMIN:
                        Min = 1 - mdp.MaxProbability(VerificationOutput);
                        break;
                }

                if (Min == 1)
                {
                    VerificationOutput.VerificationResult = VerificationResultType.VALID;
                }
                else if (Max == 0)
                {
                    VerificationOutput.VerificationResult = VerificationResultType.INVALID;
                }
                else
                {
                    VerificationOutput.VerificationResult = VerificationResultType.WITHPROBABILITY;
                }
            }
        }
    }

}