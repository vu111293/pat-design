using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;

namespace PAT.Common.Classes.Expressions.ExpressionClass
{
    public sealed class Record : Expression
    {
        public Expression[] Associations;

        public Record(Expression[] ass)
        {
            Associations = ass;

            StringBuilder sb = new StringBuilder("[");
            foreach (Expression exp in Associations)
            {
                HasVar = HasVar || exp.HasVar;
                sb.Append(exp.ExpressionID + ",");

            }
            ExpressionType = ExpressionType.Record;

            expressionID = sb.ToString();
        }

        public Record(int size)
        {
            Associations = new Expression[size];
            for (int i = 0; i < size; i++)
            {
                Associations[i] = new IntConstant(0);
            }
            ExpressionType = ExpressionType.Record;
        }

        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("[");
            for (int i = 0; i < Associations.Length; i++)
            {
                if (i != Associations.Length - 1)
                {
                    sb.Append(Associations[i].ToString() + ", ");
                }
                else
                {
                    sb.Append(Associations[i].ToString());
                }
            }
            sb.Append("]");
            return sb.ToString();
        }

        //public override String GetID()
        //{
        //    StringBuilder sb = new StringBuilder("[");
        //    for (int i = 0; i < Associations.Length; i++)
        //    {
        //        if(i != Associations.Length - 1)
        //        {
        //            sb.Append(Associations[i].GetID() + ",");    
        //        }
        //        else
        //        {
        //            sb.Append(Associations[i].GetID());    
        //        }                
        //    }            
        //    sb.Append("]");
        //    return sb.ToString();
        //}

        public override List<string> GetVars()
        {
            List<string> vars = new List<string>(16);
            foreach (Expression exp in Associations)
            {
                Ultility.Ultility.AddList(vars, exp.GetVars());
            }
            return vars;
        }

        public override Expression ClearConstant(Dictionary<string, Expression> constMapping)
        {
            Expression[] newAssociations = new Expression[Associations.Length];

            for (int i = 0; i < Associations.Length; i++)
            {
                newAssociations[i] = Associations[i].ClearConstant(constMapping);
            }

            return new Record(newAssociations);

        }

        public override bool HasExternalLibraryCall()
        {
            for (int i = 0; i < Associations.Length; i++)
            {
                if(Associations[i].HasExternalLibraryCall())
                {
                    return true;
                }
            }

            return false;
        }

#if DEBUG

        public override void GenerateMSIL(ILGenerator ilGenerator, TypeBuilder typeBuilder)
        {
            LocalBuilder newVar = ilGenerator.DeclareLocal(typeof(int));

            int size = Associations.Length;
            new IntConstant(size).GenerateMSIL(ilGenerator, typeBuilder);
            ilGenerator.Emit(OpCodes.Newarr, typeof(int));

            ilGenerator.Emit(OpCodes.Stloc, newVar);

            for (int i = 0; i < Associations.Length; i++)
            {
                ilGenerator.Emit(OpCodes.Ldloc, newVar);
                new IntConstant(i).GenerateMSIL(ilGenerator, typeBuilder);//index
                Associations[i].GenerateMSIL(ilGenerator, typeBuilder);//value
                ilGenerator.Emit(OpCodes.Stelem_I4);
            }

            ilGenerator.Emit(OpCodes.Ldloc, newVar);
        }
#endif
    }
}