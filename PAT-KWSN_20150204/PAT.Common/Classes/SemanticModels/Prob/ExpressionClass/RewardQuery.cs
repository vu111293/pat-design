using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PAT.Common.Classes.Expressions.ExpressionClass;

namespace PAT.Common.Classes.SemanticModels.Prob.ExpressionClass
{
    class RewardQuery : Expression
    {
        public int rewardStructIndex = -1;
        public string rewardName = null;
        public string relOp = null;
        public Expression reward;
        public Expression expression;

        public RewardQuery(Expression e, string opt, Expression reward, int index, string name)
        {
            expression = e;
            relOp = opt;
            this.reward = reward;
            rewardStructIndex = index;
            rewardName = name;
        }

        public override string ToString()
        {
            string s = "";

            s += "R";
            if (rewardStructIndex >= 0)
            {
                s += "{" + rewardStructIndex + "}";
            }
            else
            {
                s += "{\"" + rewardName + "\"}";
            }
            s += relOp;
            s += (reward == null) ? "?" : reward.ToString();
            s += " [ " + expression + " ]";

            return s;
        }


    }
}
