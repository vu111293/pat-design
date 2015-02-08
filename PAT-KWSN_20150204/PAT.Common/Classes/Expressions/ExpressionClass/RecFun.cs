using System;
using System.Collections.Generic;

namespace PAT.ModelChecking.Expressions.ExpressionClass
{
    public sealed class RecFun : Expression
    {
        public String FunVar;
        public List<string>  Formals;
        public Expression body;
        public RecFun(String f, List<string> xs, Expression b)
        {
            FunVar = f;
            Formals = xs;
            body = b;
            BuildVars();
            ExpressionType = ExpressionType.RecFun;

        }
        public override String ToString()
        {
            String s = "";
            foreach (string formal in Formals)
            {
                s = s + " " + formal;
            }			
            return "recfun " + FunVar + " " + s + " -> " + body;
        }

        public override void BuildVars()
        {
            this.body.BuildVars();
            this.Vars = new List<string>(16);
            this.Vars.AddRange(this.body.Vars);
        }

        public override Expression ClearConstant(List<string> avoidsVars, Dictionary<string, Expression> constMapping, bool needClone)
        {
            if (!needClone)
            {
                body = body.ClearConstant(avoidsVars, constMapping, needClone);
                return this;                
            }
            else
            {
                return new RecFun(FunVar, Formals, body.ClearConstant(avoidsVars, constMapping, needClone));
            }
        }
    }
}