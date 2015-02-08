using System;
using System.Collections.Generic;
using PAT.Common.Classes.LTS;

namespace PAT.Common.Classes.Expressions.ExpressionClass
{
    public sealed class IndexedExpression //: Expression
    {
        public String Operator;
        public List<ParallelDefinition> Definitions;
        public Expression Expression;

        public IndexedExpression(string opr, List<ParallelDefinition> definitions, Expression expression)
        {
            //ExpressionType = ExpressionType.IndexedExpression;
            Operator = opr;
            Definitions = definitions;
            Expression = expression;

            string domains = "";
            if(definitions != null)
            {
                foreach (ParallelDefinition list in Definitions)
                {
                    domains += list.ToString() + ";";
                }

            }
            //expressionID = Operator + domains + "@" + Expression.ExpressionID;
        }

  
        public override string ToString()
        {
            string returnString = "";
            if(Definitions != null)
            {
                foreach (ParallelDefinition list in Definitions)
                {
                    returnString += list.ToString() + ";";
                }

            }
            
            return Operator + " " + returnString.TrimEnd(';') + "@ {" + Expression + "}";
        }

        public IndexedExpression ClearConstant(Dictionary<string, Expression> constMapping)
        {
            Expression newExpression = Expression.ClearConstant(constMapping);

            List<ParallelDefinition> newDefinitions = new List<ParallelDefinition>(Definitions.Count);

            foreach (ParallelDefinition definition in Definitions)
            {
                newDefinitions.Add(definition.ClearConstant(constMapping));
            }

            return new IndexedExpression(Operator, newDefinitions, newExpression);
        }

        public Expression GetIndexedExpression()
        {
            List<Expression> expressions = new List<Expression>(16);

            foreach (ParallelDefinition pd in Definitions)
            {
                if (pd.GetGlobalVariables().Count > 0)
                {
                    throw new Exception("Global variable can not be used in the index expression!");
                }
                pd.DomainValues.Sort();
            }

            List<List<Expression>> list = new List<List<Expression>>();
            foreach (int v in Definitions[0].DomainValues)
            {
                List<Expression> l = new List<Expression>(Definitions.Count);
                l.Add(new IntConstant(v));
                list.Add(l);
            }

            for (int i = 1; i < Definitions.Count; i++)
            {
                List<List<Expression>> newList = new List<List<Expression>>();
                List<int> domain = Definitions[i].DomainValues;

                for (int j = 0; j < list.Count; j++)
                {
                    foreach (int i1 in domain)
                    {
                        List<Expression> cList = new List<Expression>(list[j]);
                        cList.Add(new IntConstant(i1));
                        newList.Add(cList);
                    }
                }
                list = newList;
            }

            foreach (List<Expression> constants in list)
            {
                //Dictionary<string, Expression> constMappingNew = new Dictionary<string, Expression>(constMapping);
                Dictionary<string, Expression> constMappingNew = new Dictionary<string, Expression>();
                for (int i = 0; i < constants.Count; i++)
                {
                    Expression constant = constants[i];
                    //constant.BuildVars();
                    constMappingNew.Add(Definitions[i].Parameter, constant);

                }

                Expression newExpression = Expression.ClearConstant(constMappingNew);
                expressions.Add(newExpression);
            }

            if (expressions.Count > 1)
            {
                PrimitiveApplication toReturn = new PrimitiveApplication(Operator, expressions[expressions.Count - 2],
                                                                         expressions[expressions.Count - 1]);
                for (int i = expressions.Count - 3; i >= 0; i--)
                {
                    toReturn = new PrimitiveApplication(Operator, expressions[i], toReturn);
                }

                return toReturn;
            }
            
            throw new Exception("The number of expressions must be greater than 1!");
        }  
    }
}
