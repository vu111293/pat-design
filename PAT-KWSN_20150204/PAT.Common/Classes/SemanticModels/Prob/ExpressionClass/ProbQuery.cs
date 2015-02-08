using PAT.Common.Classes.Expressions.ExpressionClass;
using PAT.Common.Classes.SemanticModels.Prob.SystemStructure;

namespace PAT.Common.Classes.SemanticModels.Prob.ExpressionClass
{

    public class ProbQuery : Expression
    {
        string relOp = null;
        Expression prob = null;
        Expression expression = null;

        public ProbQuery(Expression e, string r, Expression p)
        {
            expression = e;
            relOp = r;
            prob = p;
        }

        public override string ToString()
        {
            string s = "";

            s += "P" + relOp;
            s += (prob == null) ? "?" : prob.ToString();
            s += " [ " + expression + " ]";

            return s;
        }
    }
}
