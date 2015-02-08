using System;
using System.Collections.Generic;
using Environment = PAT.ModelChecking.Expressions.DenotationalClass.Environment;

namespace PAT.ModelChecking.Expressions.ExpressionClass
{
    public sealed class Fun : Expression
    {
        public List<string>  Formals;
        public Expression Body;
        public Fun(List<string> xs, Expression b)
        {
            Formals = xs;
            Body = b;
            BuildVars();
            ExpressionType = ExpressionType.Fun;

        }

        public override String ToString()
        {
            String s = "";
            foreach (string formal in Formals)
            {
                s = s + " " + formal;
            }
            return "fun " + s + " -> " + Body;
        }

        public override void BuildVars()
        {
            this.Body.BuildVars();
            this.Vars = new List<string>(16);
            this.Vars.AddRange(this.Body.Vars);
        }

        public override Expression ClearConstant(List<string> avoidsVars, Dictionary<string, Expression> constMapping, bool needClone)
        {
            if (!needClone)
            {
                Body = Body.ClearConstant(avoidsVars, constMapping, needClone);
                return this;                
            }
            else
            {
                return new Fun(Formals, Body.ClearConstant(avoidsVars, constMapping, needClone));
            }
        }
    }
}