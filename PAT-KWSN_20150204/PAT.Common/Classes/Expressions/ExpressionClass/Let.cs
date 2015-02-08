using System;
using System.Collections.Generic;

namespace PAT.ModelChecking.Expressions.ExpressionClass
{
    public sealed class Let : Expression
    {
        public List<LetDefinition> Definitions;
        public Expression Body;

        public Let(List<LetDefinition> ds, Expression b)
        {
            Definitions = ds;
            Body = b;
            BuildVars();
            ExpressionType = ExpressionType.Let;

        }

        public override String ToString()
        {
            String s = "";

            foreach (LetDefinition definition in Definitions)
            {
                s = s + " " + definition;
            }
			
            return "(let " + s + " in " + Body + ")";
        }

        public override void BuildVars()
        {
            this.Vars = new List<string>(16);
            foreach (LetDefinition definition in Definitions)
            {
                definition.RightHandExpression.BuildVars();
                Ultility.Ultility.Union(this.Vars, definition.RightHandExpression.Vars);

            }

        }

        public override Expression ClearConstant(List<string> avoidsVars, Dictionary<string, Expression> constMapping, bool needClone)
        {
            if (!needClone)
            {
                for (int i = 0; i < Definitions.Count; i++)
                {
                    Definitions[i] = Definitions[i].ClearConstant(avoidsVars, constMapping, needClone) as LetDefinition;
                }
                
                Body = Body.ClearConstant(avoidsVars, constMapping, needClone);

                return this;                
            }
            else
            {
                 List<LetDefinition> newDefinitions = new List<LetDefinition>(Definitions.Count);
                for (int i = 0; i < Definitions.Count; i++)
                {
                    newDefinitions.Add((LetDefinition)Definitions[i].ClearConstant(avoidsVars, constMapping, needClone));
                }

                return new Let(newDefinitions, Body.ClearConstant(avoidsVars, constMapping, needClone));
            }
        }
    }
}