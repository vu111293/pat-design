using PAT.Common.Classes.Expressions;
using PAT.Common.Classes.Expressions.ExpressionClass;
namespace PAT.PN.LTS
{
    /// <summary>
    /// This is the generated class. Need to be checked
    /// </summary>
    public class PNConfigurationWithChannelData : PNConfiguration
    {
        public string ChannelName;
        public Expression[] Expressions;

        public PNConfigurationWithChannelData(PetriNet p, string e, string hiddenEvent, Valuation globalEnv, bool isDataOperation, string name, Expression[] expressions)
            : base(p, e, hiddenEvent, globalEnv, isDataOperation, null)
        {
            ChannelName = name;
            Expressions = expressions;
        }
    }
}