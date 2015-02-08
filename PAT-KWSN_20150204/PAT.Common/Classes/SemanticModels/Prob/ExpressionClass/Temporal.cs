using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PAT.Common.Classes.Expressions.ExpressionClass;
using PAT.Common.Classes.SemanticModels.Prob.SystemStructure;

namespace PAT.Common.Classes.SemanticModels.Prob.ExpressionClass
{
    public enum TemporalOpt
    {
        Next, Until, Future, Global, WeakUntil, Release, Cummulative, Instantaneous, Reachability, SteadyState
    }

    public class Temporal: Expression
    {
        public TemporalOpt op;
        public Expression operand1;
        public Expression operand2;

        public Expression lBound;
        public Expression uBound;
        
        /// <summary>
        /// true: >, false: >= 
        /// </summary>
        public bool lBoundStrict = false;

        /// <summary>
        /// true: <, false: <=
        /// </summary>
        public bool uBoundStrict = false;

        public void SetTimeBound(TimeBound timebound)
        {
            this.lBound = timebound.lBound;
            this.lBoundStrict = timebound.lBoundStrict;
            this.uBound = timebound.uBound;
            this.uBoundStrict = timebound.uBoundStrict;
        }

        public override string ToString()
        {
            String s = string.Empty;
            if (operand1 != null)
            {
                s += operand1 + " ";
            }
            s += op;

            if (lBound == null)
            {
                if (uBound != null)
                {
                    if (op != TemporalOpt.Instantaneous)
                        s += "<" + (uBoundStrict ? "" : "=") + uBound;
                    else
                        s += "=" + uBound;
                }
            }
            else
            {
                if (uBound == null)
                {
                    s += ">" + (lBoundStrict ? "" : "=") + lBound;
                }
                else
                {
                    s += "[" + lBound + "," + uBound + "]";
                }
            }
            if (operand2 != null)
                s += " " + operand2;
            return s;
        }
    }
}
