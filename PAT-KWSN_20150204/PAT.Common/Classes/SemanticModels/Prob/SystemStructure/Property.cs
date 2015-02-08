using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PAT.Common.Classes.Expressions.ExpressionClass;

namespace PAT.Common.Classes.SemanticModels.Prob.SystemStructure
{
    public class Property
    {
        
        /// <summary>
        /// PRISM expression representing property
        /// </summary>
        public Expression expr;

        /// <summary>
        /// Optional name for property (null if absent)
        /// </summary>
        public string name;

        /// <summary>
        /// Optional comment for property (null if absent)
        /// </summary>
        public String comment;

        public Property(string name, Expression exp)
        {
            this.name = name;
            this.expr = exp;
        }

        public override string ToString()
        {
            String s = "";
            if (name != null)
                s += "\"" + name + "\": ";
            s += expr;
            return s;
        }
    }
}
