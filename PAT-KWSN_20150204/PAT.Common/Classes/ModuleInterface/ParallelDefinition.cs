using System;
using System.Collections.Generic;
using Antlr.Runtime;
using PAT.Common.Classes.Expressions;
using PAT.Common.Classes.Expressions.ExpressionClass;

namespace PAT.Common.Classes.LTS
{
    public struct ParallelDefinition
    {
        public string Parameter;
        public List<Expression> Domain;
        public Expression LowerBound;
        public Expression UpperBound;
        public List<int> DomainValues;
        //public bool HasVaraible;
        public IToken Token;

        public ParallelDefinition(string para, IToken token)
        {
            Parameter = para;
            Domain = new List<Expression>();
            DomainValues = new List<int>();
            //HasVaraible = false;
            LowerBound = null;
            UpperBound = null;
            Token = token;
        }

        public bool HasExternalLibraryAccess ()
        {
            if (Domain != null)
            {
                for (int i = 0; i < Domain.Count; i++)
                {
                    if (Domain[i].HasExternalLibraryCall())
                    {
                        return true;
                    }
                }
            }

            if (LowerBound != null && LowerBound.HasExternalLibraryCall())
            {
                return true;
            }

            if (LowerBound != null && UpperBound.HasExternalLibraryCall())
            {
                return true;
            }

            return false;
        }

        public override string ToString()
        {
            if (LowerBound == null)
            {
                string s = Parameter + ":{";

                foreach (int i in DomainValues)
                {
                    s += i + ",";
                }
                return s.TrimEnd(',') + "}";
            }
            else
            {
                return Parameter + ":{" + LowerBound + ".." + UpperBound + "}";
            }
        }

        public ParallelDefinition ClearConstant(Dictionary<string, Expression> constMapping)
        {
            ParallelDefinition pd = new ParallelDefinition(Parameter, Token);

            try
            {
                if (LowerBound == null)
                {
                    foreach (Expression expression in Domain)
                    {
                        Expression newExp = expression.ClearConstant(constMapping);
                        if (!newExp.HasVar)
                        {
                            ExpressionValue val = EvaluatorDenotational.Evaluate(newExp, null);

                            if (val is IntConstant)
                            {
                                pd.Domain.Add(val);
                                pd.DomainValues.Add((val as IntConstant).Value);
                            }
                            else
                            {
                                throw new ParsingException("An integer value is expected, but not " + val, Token);
                            }
                        }
                        else
                        {
                            pd.Domain.Add(newExp);
                        }
                    }
                }
                else
                {
                    pd.LowerBound = LowerBound.ClearConstant(constMapping);
                    pd.UpperBound = UpperBound.ClearConstant(constMapping);

                    if (!pd.LowerBound.HasVar && !pd.UpperBound.HasVar)
                    {
                        ExpressionValue lower = EvaluatorDenotational.Evaluate(pd.LowerBound, null);
                        ExpressionValue upper = EvaluatorDenotational.Evaluate(pd.UpperBound, null);

                        if (lower is IntConstant && upper is IntConstant)
                        {
                            int low = (lower as IntConstant).Value;
                            int up = (upper as IntConstant).Value;
                            if (low > up)
                            {
                                throw new ParsingException("The variable range's starting value " + low + " should be smaller than ending value" + up + ".", Token);
                            }
                            else
                            {
                                pd = new ParallelDefinition(Parameter, Token);
                                for (int i = low; i <= up; i++)
                                {
                                    pd.Domain.Add(new IntConstant(i));
                                    pd.DomainValues.Add(i);
                                }
                            }
                        }
                        else
                        {
                            throw new ParsingException("Integers value are expected, but not " + lower + " and " + upper, Token);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new ParsingException(ex.Message, Token);
            }
            return pd;
        }

        public List<string> GetGlobalVariables()
        {
            List<string> Variables = new List<string>();
            if (LowerBound == null)
            {
                foreach (Expression expression in Domain)
                {
                    Ultility.Ultility.AddList(Variables, expression.GetVars());
                }
            }
            else
            {
                Ultility.Ultility.AddList(Variables, LowerBound.GetVars());
                Ultility.Ultility.AddList(Variables, UpperBound.GetVars());
            }

            return Variables;
        }
    }
}
