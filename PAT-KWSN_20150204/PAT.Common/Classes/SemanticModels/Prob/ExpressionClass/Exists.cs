using PAT.Common.Classes.Expressions.ExpressionClass;

namespace PAT.Common.Classes.SemanticModels.Prob.ExpressionClass
{
    public class Exists : Expression
    {
        public Expression exp;

        public Exists(Expression e)
        {
            exp = e;
        }

        public override string ToString()
        {
            return "E [ " + exp + " ]";
        }
    }
}
