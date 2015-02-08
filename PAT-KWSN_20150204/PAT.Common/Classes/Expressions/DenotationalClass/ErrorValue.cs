using System;

namespace PAT.ModelChecking.Expressions.DenotationalClass
{
    public sealed class ErrorValue : Value
    {
        public override String ToString()
        {
            return "error";
        }
        public Value GetClone()
        {
            return this;
        }
    }
}