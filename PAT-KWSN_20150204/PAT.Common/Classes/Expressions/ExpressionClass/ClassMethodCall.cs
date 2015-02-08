using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace PAT.Common.Classes.Expressions.ExpressionClass
{
    public sealed class ClassMethodCall : Expression
    {
        public string Variable;
        public string MethodName;
        public Expression[] Arguments;

        public ClassMethodCall(string var, String method, Expression[] args)
        {
            Variable = var;
            ExpressionType = ExpressionType.ClassMethodCall;
            MethodName = method;
            Arguments = args;

            HasVar = true;

           StringBuilder sb = new StringBuilder(Variable+ "." + MethodName + "(");
            for (int i = 0; i < Arguments.Length; i++)
            {
                if (i == Arguments.Length - 1)
                {
                    sb.Append(Arguments[i].ExpressionID);
                }
                else
                {
                    sb.Append(Arguments[i].ExpressionID + ",");
                }
            }
            sb.Append(")");
            expressionID = sb.ToString();

        }

        public override String ToString()
        {
            StringBuilder sb = new StringBuilder(Variable + "." + MethodName + "(");
            for (int i = 0; i < Arguments.Length; i++)
            {
                if (i == Arguments.Length - 1)
                {
                    sb.Append(Arguments[i].ToString());
                }
                else
                {
                    sb.Append(Arguments[i].ToString() + ", ");
                }
            }
            sb.Append(")");
            return sb.ToString();
        }

        //public override String GetID()
        //{
        //    StringBuilder sb = new StringBuilder(Variable+ "." + MethodName + "(");
        //    for (int i = 0; i < Arguments.Length; i++)
        //    {
        //        if (i == Arguments.Length - 1)
        //        {
        //            sb.Append(Arguments[i].GetID());
        //        }
        //        else
        //        {
        //            sb.Append(Arguments[i].GetID() + ",");
        //        }
        //    }
        //    sb.Append(")");
        //    return sb.ToString();
        //}

        public override List<string> GetVars()
        {
            List<string> vars = new List<string>(16);
            foreach (Expression exp in Arguments)
            {
                Ultility.Ultility.AddList(vars, exp.GetVars());
            }
            vars.Add(Variable);
            return vars;
        }

        public override Expression ClearConstant(Dictionary<string, Expression> constMapping)
        {
            Expression[] newArgs = new Expression[Arguments.Length];

            for (int i = 0; i < Arguments.Length; i++)
            {
                newArgs[i] = Arguments[i].ClearConstant(constMapping);
            }
            
            if(constMapping.ContainsKey(Variable))
            {
                return new ClassMethodCallInstance(constMapping[Variable] as ExpressionValue, MethodName, newArgs);
            }
            else
            {
                return new ClassMethodCall(Variable, MethodName, newArgs);
            }
        }

        public override bool HasExternalLibraryCall() 
        {
            return true;
        }
#if DEBUG
        public override void GenerateMSIL(ILGenerator ilGenerator, TypeBuilder typeBuilder)
        {
            FieldInfo fb = typeBuilder.GetField(Variable);
            ilGenerator.Emit(OpCodes.Ldsfld, fb);

            foreach (var argument in Arguments)
            {
                argument.GenerateMSIL(ilGenerator, typeBuilder);
            }

            Type type = fb.GetType();

            ilGenerator.Emit(OpCodes.Callvirt, type.GetMethod(MethodName));
        }
#endif
    }
}