using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PAT.Common.Classes.Expressions.ExpressionClass;

namespace PAT.Common.Classes.SemanticModels.Prob.SystemStructure
{
    public class OldStyleFilter
    {
        public Expression expr = null;
        // Either "min" or "max", or neither or both.
        // In the latter two cases, this means "state" or "range"
        public bool minReq = false;
        public bool maxReq = false;

        public OldStyleFilter(Expression expr)
        {
            this.expr = expr;
        }

        public String getFilterOpString()
        {
            if (minReq)
            {
                return maxReq ? "range" : "min";
            }
            else
            {
                return maxReq ? "max" : "state";
            }
        }
    }
}
