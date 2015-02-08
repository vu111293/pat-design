using System;
using System.Collections.Generic;
using PAT.Common.Classes.Expressions.ExpressionClass;

namespace PAT.Common.Classes.SemanticModels.Prob.SystemStructure
{
    public class ConstantList
    {
        public List<string> names = new List<string>();
        public List<Expression> constants = new List<Expression>();
        public List<VarType> types = new List<VarType>();

        public void AddConstant(string n, Expression c, VarType t)
        {
            names.Add(n);
            constants.Add(c);
            types.Add(t);
        }

        public override string ToString()
        {
            String s = "";

            for (int i = 0; i < names.Count; i++)
            {
                s += "const ";
                s += types[i] + " ";
                s += names[i];
                
                if (constants[i] != null)
                {
                    s += " = " + constants[i];
                }
                s += ";\n";
            }

            return s;
        }
    }
}
