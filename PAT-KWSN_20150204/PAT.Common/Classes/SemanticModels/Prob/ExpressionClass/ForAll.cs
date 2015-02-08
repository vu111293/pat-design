using PAT.Common.Classes.Expressions.ExpressionClass;

namespace PAT.Common.Classes.SemanticModels.Prob.ExpressionClass
{

    public class ForAll : Expression
    {
        public Expression exp;

        public ForAll(Expression e)
        {
            exp = e;
        }

        public override string ToString()
        {
            return "A [ " + exp + " ]";
        }
    }
}
