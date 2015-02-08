using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PAT.Common.Classes.Expressions.ExpressionClass;

namespace PAT.Common.Classes.SemanticModels.Prob.ExpressionClass
{
    public class Proposition: Expression
    {
        public string name;

        public override string ToString()
        {
            return "\"" + name + "\"";
        }
    }
}
