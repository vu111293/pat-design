using PAT.Common.Classes.CUDDLib;
using PAT.Common.Classes.Expressions.ExpressionClass;
using PAT.Common.Classes.SemanticModels.LTS.BDD;

namespace PAT.Common.Classes.SemanticModels.TTS
{
    public partial class TimeBehaviors
    {
        public static CUDDNode GetTickTransEncoding(Model model)
        {
            return CUDD.Function.Or(GetTickTransExpression().TranslateBoolExpToBDD(model).GuardDDs);
        }

        public static Expression GetTickTransExpression()
        {
            return new Assignment(Model.EVENT_NAME, new IntConstant(Model.TOCK_EVENT_INDEX));
        }

        public static Expression GetNotTickTransExpression()
        {
            return Expression.NE(new VariablePrime(Model.EVENT_NAME),
                                     new IntConstant(Model.TOCK_EVENT_INDEX));
        }
    }
}
