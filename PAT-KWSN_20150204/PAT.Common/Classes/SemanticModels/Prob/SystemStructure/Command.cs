using System.Collections.Generic;
using PAT.Common.Classes.Expressions.ExpressionClass;

namespace PAT.Common.Classes.SemanticModels.Prob.SystemStructure
{
    public class Command
    {
        public string Synch;
        public Expression Guard;
        public List<Update> Updates;

        public Command(Expression guard, List<Update> updates)
        {
            Synch = string.Empty;
            Guard = guard;
            Updates = updates;
        }

        public Command(string synch, Expression guard, List<Update> updates)
        {
            Synch = synch;
            Guard = guard;
            Updates = updates;
        }

        public override string ToString()
        {
            string s = "[" + Synch;
            s += "] " + Guard + " -> ";

            for (int i = 0; i < Updates.Count; i++ )
            {
                s += Updates[i].ToString();
                if(i < Updates.Count - 1)
                {
                    s += " + ";
                }
            }
            return s;
        }
    }
}
