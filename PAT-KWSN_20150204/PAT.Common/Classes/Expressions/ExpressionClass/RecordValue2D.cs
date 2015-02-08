using System;
using System.Text;

namespace PAT.ModelChecking.Expressions.ExpressionClass
{
    public sealed class RecordValue2D : Expression
    {
        public Expression[,] Associations;

        public RecordValue2D(Expression[,] ass)
        {
            Associations = ass;
        }

        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("[");
            for (int i = 0; i < Associations.Length - 1; i++)
            {
                sb.Append(Associations[i].ToString() + ",");
            }
            sb.Append(Associations[Associations.Length - 1].ToString() + "]");
            return sb.ToString();
        }


        public override Expression GetClone()
        {
            Expression[] newAssociations = new Expression[Associations.Length];
            for (int i = 0; i < Associations.Length; i++)
            {
                newAssociations[i] = Associations[i].GetClone();
            }

            RecordValue newrv = new RecordValue(newAssociations);
            return newrv;
        }


        //public Expression Access(int index)
        //{
        //    if (index < 0)
        //    {
        //        throw new NegativeArraySizeException("Access negative index " + index + " for record " + ToString());
        //    }
        //    else if (index >= Associations.Length)
        //    {
        //        throw new IndexOutOfBoundsException("Access out of range " + index + " for record " + ToString());
        //    }

        //    return Associations[index];
        //}
    }
}