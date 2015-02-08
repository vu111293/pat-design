using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace PAT.Common.Classes.Expressions.ExpressionClass
{
    public sealed class ClassProperty : Expression
    {
        public Expression Variable;
        public string PropertyName;

        public ClassProperty(Expression var, String property)
        {
            Variable = var;
            ExpressionType = ExpressionType.ClassProperty;
            PropertyName = property;

            HasVar = true;

            expressionID = Variable + "." + PropertyName;
        }

        public override String ToString()
        {
            return Variable + "." + PropertyName;
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
            return Variable.GetVars();
        }

        public override Expression ClearConstant(Dictionary<string, Expression> constMapping)
        {
            return new ClassProperty(Variable.ClearConstant(constMapping), PropertyName);
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

            Type type = fb.GetType();
            FieldInfo property = typeBuilder.GetField(PropertyName);
            ilGenerator.Emit(OpCodes.Ldsfld, property);
        }
#endif
    }
}