using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PAT.Common.Classes.Expressions.ExpressionClass;

namespace PAT.Common.Classes.SemanticModels.Prob.ExpressionClass
{
    public enum FilterOperator
    {
        MIN, MAX, ARGMIN, ARGMAX, COUNT, SUM, AVG, FIRST, RANGE, FORALL, EXISTS, PRINT, STATE
    };

    public class NewStyleFilter : Expression
    {
        public FilterOperator opType;
        public String opName;

        /// <summary>
        /// Expression that filter is applied to
        /// </summary>
        public Expression operand;


        /// <summary>
        /// Expression defining states that filter is over
        /// (optional; can be null, denoting "true")
        /// </summary>
        public Expression filter;


        /// <summary>
        /// Filter can be invisible, meaning it is not actually displayed
        /// by toString(). This is used to add filters to P/R/S operators that
        /// were created with old-style filter syntax. 
        /// </summary>
        public bool invisible = false;

        public NewStyleFilter(String opName, Expression operand, Expression filter)
        {
            setOperator(opName);
            this.operand = operand;
            this.filter = filter;
        }

        public void setOperator(String opName)
        {
            this.opName = opName;
            if (opName.Equals("min"))
                opType = FilterOperator.MIN;
            else if (opName.Equals("max"))
                opType = FilterOperator.MAX;
            else if (opName.Equals("argmin"))
                opType = FilterOperator.ARGMIN;
            else if (opName.Equals("argmax"))
                opType = FilterOperator.ARGMAX;
            else if (opName.Equals("count"))
                opType = FilterOperator.COUNT;
            else if (opName.Equals("sum") || opName.Equals("+"))
                opType = FilterOperator.SUM;
            else if (opName.Equals("avg"))
                opType = FilterOperator.AVG;
            else if (opName.Equals("first"))
                opType = FilterOperator.FIRST;
            else if (opName.Equals("range"))
                opType = FilterOperator.RANGE;
            else if (opName.Equals("forall") || opName.Equals("&"))
                opType = FilterOperator.FORALL;
            else if (opName.Equals("exists") || opName.Equals("|"))
                opType = FilterOperator.EXISTS;
            else if (opName.Equals("print"))
                opType = FilterOperator.PRINT;
            else if (opName.Equals("state"))
                opType = FilterOperator.STATE;
        }

        public override string ToString()
        {
            String s = string.Empty;

            s += "filter(" + opName + ", " + operand;
            if (filter != null)
            {
                s += ", " + filter;
            }
            s += ")";
            return s;
        }

    }
}
