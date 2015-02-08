using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PAT.Common.Classes.Expressions.ExpressionClass;

namespace PAT.Common.Classes.SemanticModels.Prob.SystemStructure
{
    public class LabelList
    {
        public List<String> names = new List<string>();
        public List<Expression> labels = new List<Expression>();

        public void AddLabel(String name, Expression label)
        {
            names.Add(name);
            labels.Add(label);
        }

        public override string ToString()
        {
            String s = "";
            
            for (int i = 0; i < names.Count; i++)
            {
                s += "label \"" + names[i];
                s += "\" = " + labels[i] + ";\n";
            }

            return s;
        }
    }
}
