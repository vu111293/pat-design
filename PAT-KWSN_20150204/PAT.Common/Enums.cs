namespace PAT.Common
{

    /// <summary>
    /// Verification result enumeration
    /// </summary>
    public enum VerificationResultType : byte
    {
        VALID,
        INVALID,
        UNKNOWN,
        WITHPROBABILITY
    }

    /// <summary>
    /// Assertion type enumeration
    /// </summary>
    public enum AssertionType : byte
    {
        Deadlock,
        Reachability,
        Deterministic,
        Divergence,
        LTL,
        LTLSafety,
        TraceEquivalence,
        FailureEquivalence,
        FailureDivergenceEquivalence,
        TraceRefinement,
        FailureRefinement,
        FailureDivergenceRefinement,
        
    }
}
