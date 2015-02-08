using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PAT.Common.Classes.Expressions.ExpressionClass;

namespace PAT.Common.Classes.SemanticModels.Prob.SystemStructure
{
    public class FormulaList
    {
        public List<String> names = new List<string>();
        public List<Expression> formulas = new List<Expression>();

        public void AddFormula(string name, Expression exp)
        {
            names.Add(name);
            formulas.Add(exp);
        }

        public override string ToString()
        {
            String s = "";

            for (int i = 0; i < names.Count; i++)
            {
                s += "formula " + names[i];
                s += " = " + formulas[i] + ";\n";
            }

            return s;
        }
    }
}
