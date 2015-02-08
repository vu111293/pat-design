using System;
using System.Collections.Generic;
using System.Text;
using PAT.ModelChecking.Expressions.ExpressionClass;

namespace PAT.ModelChecking.Expressions.DenotationalClass
{
    public sealed class ArrayValue : Value
    {
        public bool Empty
        {
            get
            {
                return (Matrix.Count == 0);
            }

        }
        public List<Value[]> Matrix;

        public ArrayValue(List<Value[]> matrix)
        {
            Matrix = matrix;
        }

      
        public Value Access(int index1, int index2)
        {
            if (index1 < 0 || index2 < 0)
            {
                 throw new NegativeArraySizeException();
            }
            else if (index1 >= Matrix.Count || index2 >= Matrix[index1].Length)
            {
                throw new IndexOutOfBoundsException();
            }

            return Matrix[index1][index2];
        }

        //public virtual bool HasProperty(String p)
        //{
        //    return Associations.ContainsKey(p);
        //}

        public Value GetClone()
        {

            List<Value[]> newMatrix = new List<Value[]>();
            foreach(Value[] array in Matrix)
            {
                Value[] newArray = new Value[array.Length];
                for (int i = 0; i < array.Length; i++)
                {
                    newArray[i] = array[i];    
                }
                newMatrix.Add(newArray);
            }

            ArrayValue newav = new ArrayValue(newMatrix);
            return newav;
        }

        public override String ToString()
        {
            StringBuilder sb = new StringBuilder();

            foreach (Value[] array in Matrix)
            {
                for (int i = 0; i < array.Length - 1; i++)
                {
                    Value var = array[i];
                    sb.Append(var + ", ");
                }
                sb.Append(array[array.Length - 1].ToString());
            }
            return sb.ToString();
        }
    }
}