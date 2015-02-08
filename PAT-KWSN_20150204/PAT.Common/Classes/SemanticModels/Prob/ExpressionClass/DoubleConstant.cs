using PAT.Common.Classes.Expressions.ExpressionClass;

namespace PAT.Common.Classes.SemanticModels.Prob.ExpressionClass
{
    public class DoubleConstant : Expression
    {
        public double Value;

        public DoubleConstant(double v)
        {
            Value = v;
        }

        public override string ToString()
        {
            return Value.ToString();
        }
    }
}
