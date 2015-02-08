using System;
using System.Text;
using PAT.ModelChecking.Expressions.ExpressionClass;

namespace PAT.ModelChecking.Expressions.DenotationalClass
{
    public sealed class RecordValue : Value
    {
        //public bool Empty
        //{
        //    get
        //    {
        //        return (Values.Length == 0);
        //    }
        //}

        //public virtual bool HasProperty(String p)
        //{
        //    return Associations.ContainsKey(p);
        //}

        public Value[] Values;

        public RecordValue(Value[] ass)
        {
            Values = ass;
        }

      
        public Value Access(int index)
        {
            if (index < 0)
            {
                 throw new NegativeArraySizeException();
            }
            else if (index >= Values.Length)
            {
                throw new IndexOutOfBoundsException();
            }
            
            return Values[index];
        }

        public Value GetClone()
        {
            Value[] newAssociations = new Value[Values.Length];
            for (int i = 0; i < Values.Length; i++)
            {
                newAssociations[i] = Values[i].GetClone();
            }

            RecordValue newrv = new RecordValue(newAssociations);
            return newrv;
        }

        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("[");
            for (int i = 0; i < Values.Length - 1; i++)
            {
                Value exp = Values[i];
                sb.Append(exp.ToString() + ",");
            }
            sb.Append(Values[Values.Length - 1].ToString() + "]");
            return sb.ToString();
        }
    }
}