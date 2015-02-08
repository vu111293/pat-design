using PAT.Common.Classes.Expressions.ExpressionClass;

namespace PAT.Common.Classes.SemanticModels.Prob.SystemStructure
{
    public abstract class DeclarationType
    {

    }

    public class DeclarationBool : DeclarationType
    {

    }

    public class DeclarationInt : DeclarationType
    {
        public Expression low;
        public Expression high;

        public DeclarationInt(Expression low, Expression high)
        {
            this.low = low;
            this.high = high;

        }
    }

}
