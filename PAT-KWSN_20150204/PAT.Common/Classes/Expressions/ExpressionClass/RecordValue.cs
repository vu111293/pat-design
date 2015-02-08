using System;
using System.Reflection.Emit;
using System.Text;

namespace PAT.Common.Classes.Expressions.ExpressionClass
{
    public sealed class RecordValue : ExpressionValue
    {
        public ExpressionValue[] Associations;

        public RecordValue(ExpressionValue[] ass)
        {
            Associations = ass;
            //expressionID = GetID();
            GetID();
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
        
        public override ExpressionValue GetClone()
        {
            ExpressionValue[] newAssociations = new ExpressionValue[Associations.Length];
            for (int i = 0; i < Associations.Length; i++)
            {
                newAssociations[i] = Associations[i].GetClone();
            }
            return new RecordValue(newAssociations);
        }

        public void GetID()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < Associations.Length; i++)
            {
                if (i != Associations.Length - 1)
                {
                    sb.Append(Associations[i].ExpressionID + ",");
                }
                else
                {
                    sb.Append(Associations[i].ExpressionID);
                }
            }
            expressionID = sb.ToString();
        }

#if DEBUG
        public override void GenerateMSIL(ILGenerator ilGenerator, TypeBuilder typeBuilder)
        {
            LocalBuilder newVar = ilGenerator.DeclareLocal(typeof(int));
            
            int size = Associations.Length;
            new IntConstant(size).GenerateMSIL(ilGenerator, typeBuilder);
            ilGenerator.Emit(OpCodes.Newarr, typeof (int));

            ilGenerator.Emit(OpCodes.Stloc, newVar);

            for(int i = 0; i < Associations.Length; i++)
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