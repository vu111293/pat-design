using System;
using System.Collections.Generic;

namespace PAT.ModelChecking.Expressions.ExpressionClass
{
    public class Association
    {
        public string Property;
        public Expression Expression;
        public Association(string p, Expression e)
        {
            Property = p;
            Expression = e;
        }
		
        public override String ToString()
        {
            return Property + " : " + Expression;
        }

        public Association CloneWithRanamedVarialbe(Dictionary<string, string> variableMapping)
        {
            Association assignment = new Association(Property, this.Expression.CloneWithRanamedVarialbe(variableMapping));            
            return assignment;
        }


    }
}