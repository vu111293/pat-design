using System.Collections.Generic;
using PAT.Common.Classes.Expressions.ExpressionClass;

namespace PAT.Common.Classes.SemanticModels.Prob.SystemStructure
{
    public class Update
    {
        public List<Assignment> Assignments = new List<Assignment>();
        public Expression Probability;

        public Update(List<Assignment> updates, Expression p)
        {
            this.Assignments = updates;
            Probability = p;
        }

        public void AddAssignment(Assignment assgn)
        {
            Assignments.Add(assgn);
        }

        public override string ToString()
        {
            string s = Probability.ToString() + " : ";
            for(int i = 0; i < Assignments.Count; i++)
            {
                s += "(" + Assignments[i].ToString() + ")";
                if(i < Assignments.Count - 1)
                {
                    s += " & ";
                }
            }

            return s;

        }
    }
}
