using System;

namespace PAT.ModelChecking.Expressions.DenotationalClass
{
    public sealed class BoolValue : Value
    {
        public bool Value;
        public BoolValue(bool v)
        {
            Value = v;
        }
        public override String ToString()
        {
            //return Value ? "T" : "F";;
            return Value ? "1" : "0"; ;
        }

        public Value GetClone()
        {
            //return new Value(Value);
            return this;
        }
    }
}