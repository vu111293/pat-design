using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace PAT.Common.Classes.Expressions.ExpressionClass
{
    public sealed class ClassMethodCallInstance : Expression
    {
        public Expression Variable;
        public string MethodName;
        public Expression[] Arguments;

        public ClassMethodCallInstance(Expression var, String method, Expression[] args)
        {
            Variable = var;
            ExpressionType = ExpressionType.ClassMethodCallInstance;
            MethodName = method;
            Arguments = args;

            HasVar = true;

            StringBuilder sb = new StringBuilder(Variable + "." + MethodName + "(");
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
                    sb.Append(Arguments[i] + ", ");
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
            List<string> vars = Variable.GetVars();
            foreach (Expression exp in Arguments)
            {
                Ultility.Ultility.AddList(vars, exp.GetVars());
            }

            return vars;
        }

        public override Expression ClearConstant(Dictionary<string, Expression> constMapping)
        {
            Expression[] newArgs = new Expression[Arguments.Length];

            for (int i = 0; i < Arguments.Length; i++)
            {
                newArgs[i] = Arguments[i].ClearConstant(constMapping);
            }

            return new ClassMethodCallInstance(Variable.ClearConstant(constMapping), MethodName, newArgs);
        }

        public override bool HasExternalLibraryCall()
        {
            return true;
        }
#if DEBUG
        public override void GenerateMSIL(ILGenerator ilGenerator, TypeBuilder typeBuilder)
        {
            FieldInfo fb = typeBuilder.GetField(Variable.ExpressionID);
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