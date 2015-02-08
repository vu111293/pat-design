using System;
using System.Collections.Generic;
using PAT.ModelChecking.Expressions.ExpressionClass;

namespace PAT.ModelChecking.Expressions.DenotationalClass
{
    public sealed class FunValue : Value
    {
        public Environment Environment;
        public List<string>  Formals;
        public Expression Body;

        public FunValue(Environment e, List<string> xs, Expression b)
        {
            Environment = e;
            Formals = xs;
            Body = b;
        }

        public Value GetClone()
        {
            return new FunValue(this.Environment, Formals, Body);
        }

        public override String ToString()
        {
            String s = "";
            foreach (string formal in Formals)
            {
                s = s + " " + formal;
            }
            return "fun {...} " + s + " -> " + Body.ToString();
        }
    }
}