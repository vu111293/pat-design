using System;

namespace PAT.ModelChecking.Expressions.DenotationalClass
{
    public sealed class IntValue : Value
    {
        public int Value;

        public IntValue(int v)
        {
            Value = v;
        }
        public override String ToString()
        {
            return Value.ToString();
        }

        public Value GetClone()
        {
            return this;
        }
    }
}