using System.Collections.Generic;
using PAT.Common.Classes.Expressions.ExpressionClass;

namespace PAT.Common.Classes.SemanticModels.MDP
{
    public class RewardConfig
    {
        public double? IsRewardOnAllEvent = null;
        public Dictionary<string, KeyValuePair<Expression, double>> EventToRewardMapping;
        public List<KeyValuePair<Expression, double>> StateRewards;

        public RewardConfig(double? isAll, Dictionary<string, KeyValuePair<Expression, double>> events, List<KeyValuePair<Expression, double>> states)
        {
            IsRewardOnAllEvent = isAll;
            EventToRewardMapping = events;
            StateRewards = states;
        }

    }
}
