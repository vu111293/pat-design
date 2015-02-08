using System;
using System.Collections.Generic;
using PAT.Common.Classes.Expressions.ExpressionClass;

namespace PAT.Common.Classes.SemanticModels.Prob.ExpressionClass
{
    public enum FunctionName { Min, Max, Floor, Ceiling, Pow, Mod, Log }

    public class FuncNary: Expression
    {
        public FunctionName Operator;
        public List<Expression> expressions = new List<Expression>();

        public override string ToString()
        {
            string result = string.Empty;
            result += Operator + "(";
            for (int i = 0; i < expressions.Count; i++)
            {
                result += expressions[i];
                if (i < expressions.Count - 1)
                {
                    result += ",";
                }
            }

            return result;
        }

        public FuncNary(string opt, List<Expression> exps)
        {
            switch (opt)
            {
                case "min":
                    Operator = FunctionName.Min;
                    break;
                case "max":
                    Operator = FunctionName.Max;
                    break;
                case "floor":
                    Operator = FunctionName.Floor;
                    break;
                case "ceiling":
                    Operator = FunctionName.Ceiling;
                    break;
                case "pow":
                    Operator = FunctionName.Pow;
                    break;
                case "mod":
                    Operator = FunctionName.Mod;
                    break;
                case "log":
                    Operator = FunctionName.Log;
                    break;
                default:
                    throw new Exception("Funtion name not exists!");
                    
            }
            expressions = exps;
        }

        public FuncNary(FunctionName opt, List<Expression> exps)
        {
            Operator = opt;
            expressions = exps;
        }

        public override Expression ClearConstant(Dictionary<string, Expression> constMapping)
        {
            List<Expression> newExpressions = new List<Expression>();

            foreach (var expression in expressions)
            {
                newExpressions.Add(expression.ClearConstant(constMapping));
            }

            return new FuncNary(Operator, newExpressions);
        }
    }
}
