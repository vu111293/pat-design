using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace PAT.Common.Classes.Expressions.ExpressionClass
{
    public sealed class NewObjectCreation : Expression
    {
        public string ClassName;
        public Expression[] Arguments;

        public NewObjectCreation(string method, Expression[] args)
        {
            ExpressionType = ExpressionType.NewObjectCreation;
            ClassName = method;
            Arguments = args;

            HasVar = true;

            StringBuilder sb = new StringBuilder("N@" + ClassName + "(");
            for (int i = 0; i < Arguments.Length; i++)
            {
                if (i == Arguments.Length - 1)
                {
                    sb.Append(Arguments[i].ExpressionID);
                }
                else
                {
                    sb.Append(Arguments[i].ExpressionID  + ",");
                }
            }
            sb.Append(")");
            expressionID = sb.ToString();

        }

        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("new " + ClassName + "(");
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
        //    StringBuilder sb = new StringBuilder("N@"+ ClassName + "(");
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
            return vars;
        }

        public override Expression ClearConstant(Dictionary<string, Expression> constMapping)
        {
            Expression[] newArgs = new Expression[Arguments.Length];

            for (int i = 0; i < Arguments.Length; i++)
            {
                newArgs[i] = Arguments[i].ClearConstant(constMapping);
            }

            return new NewObjectCreation(ClassName, newArgs);
        }

        public override bool HasExternalLibraryCall()
        {
            return true;
        }
#if DEBUG
        public override void GenerateMSIL(ILGenerator ilGenerator, TypeBuilder typeBuilder)
        {
            Type classType = Type.GetType(ClassName);

            Type[] typesOfArguments = new Type[Arguments.Length];
            for(int i = 0; i < typesOfArguments.Length; i++)
            {
                typesOfArguments[i] = typeof (int);
            }

            ConstructorInfo constructorInfo = classType.GetConstructor(typesOfArguments);

            ilGenerator.Emit(OpCodes.Newobj, constructorInfo);
        }
#endif
    }
}