using System;
using System.Collections.Generic;

namespace PAT.ModelChecking.Expressions.ExpressionClass
{
    public class Application : Expression
    {
        public Expression Operator;
        public List<Expression> Operands;

        public Application(Expression rator, List<Expression> rands)
        {
            ExpressionType = ExpressionType.Application;
            Operator = rator; 
            Operands = rands;
            BuildVars();
        }

        public override String ToString()
        {
            string s = "";
            foreach (Expression exp in Operands)
            {
                s += " " + exp;
            }
            return "(" + Operator + " " + s + ")";
        }

        public override void BuildVars()
        {
            this.Vars = new List<string>(16);
            foreach (Expression exp in Operands)
            {
                exp.BuildVars();
                Ultility.Ultility.Union(this.Vars, exp.Vars);

            }
        }

        public override Expression ClearConstant(List<string> avoidsVars, Dictionary<string, Expression> constMapping, bool needClone)
        {
            if (!needClone)
            {
                Operator = Operator.ClearConstant(avoidsVars, constMapping, needClone);
                for (int i = 0; i < Operands.Count; i++)
                {
                    Operands[i] = Operands[i].ClearConstant(avoidsVars, constMapping, needClone);
                }

                return this;                
            }
            else
            {
                List<Expression> newOperands = new List<Expression>(Operands.Count); 
                for (int i = 0; i < Operands.Count; i++)
                {
                    newOperands.Add(Operands[i].ClearConstant(avoidsVars, constMapping, needClone));
                }

                return new Application(Operator.ClearConstant(avoidsVars, constMapping, needClone), newOperands);
            }

        }
    }
}