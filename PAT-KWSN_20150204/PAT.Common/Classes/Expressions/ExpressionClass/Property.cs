using System;
using System.Collections.Generic;

namespace PAT.ModelChecking.Expressions.ExpressionClass
{
    public class Property : Expression
    {
        public String PropertyName;

        public Property(String nam)
        {
            PropertyName = nam;
        }
        public override String ToString()
        {
            return PropertyName;
        }

        public override Expression CloneWithRanamedVarialbe(Dictionary<string, string> variableMapping)
        {
            return this;
        }
    }
}