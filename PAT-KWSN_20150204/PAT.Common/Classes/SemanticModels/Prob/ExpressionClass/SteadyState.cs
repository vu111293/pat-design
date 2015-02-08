using PAT.Common.Classes.Expressions.ExpressionClass;

namespace PAT.Common.Classes.SemanticModels.Prob.ExpressionClass
{

    public class SteadyState : Expression
    {
        string relOp = null;
        Expression prob = null;
        Expression expression = null;

        public SteadyState(Expression e, string opt, Expression p)
        {
            expression = e;
            relOp = opt;
            prob = p;
        }

        public override string ToString()
        {
            string s = "";

            s += "S" + relOp;
            s += (prob == null) ? "?" : prob.ToString();
            s += " [ " + expression + " ]";

            return s;
        }
    }
}
