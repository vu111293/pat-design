using System;
using PAT.Common.Classes.Expressions.ExpressionClass;

namespace PAT.Common.Classes.SemanticModels.Prob.SystemStructure
{
    public class RewardItem
    {
        /// <summary>
        /// null for state reward
        /// action name for transition reward
        /// </summary>
        public String synch;

        /// <summary>
        /// Guard of reward
        /// </summary>
        public Expression guard;

        /// <summary>
        /// Reward value
        /// </summary>
        public Expression reward;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="action">give null for state reward</param>
        /// <param name="guard">Give Boolean(true) if not provided</param>
        /// <param name="rewardValue"></param>
        public RewardItem(String action, Expression guard, Expression rewardValue)
        {
            synch = action;
            this.guard = guard;
            reward = rewardValue;
        }

        public bool IsTransitionReward()
        {
            return (synch != null);
        }

        public override string ToString()
        {
            String s = string.Empty;

            if (synch != null) s += "[" + synch + "] ";
            s += guard + " : " + reward + ";";

            return s;
        }
    }
}
