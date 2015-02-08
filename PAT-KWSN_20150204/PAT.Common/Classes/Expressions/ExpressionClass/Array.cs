using System;
using System.Collections.Generic;
using System.Text;

namespace PAT.ModelChecking.Expressions.ExpressionClass
{
    public sealed class Array : Expression
    {
        public List<IntConstant[]> Matrix;

        public Array(List<IntConstant[]> matrix)
        {
            Matrix = matrix;
            BuildVars();
        }

        public Array(List<int> dimentions)
        {            
            for (int i = 0; i < dimentions.Count; i++)
            {
                int size = dimentions[i];
                IntConstant[] Associations = new IntConstant[size];
                for (int j = 0; j < size; j++)
                {
                    Associations[j] = new IntConstant(0);
                }
                Matrix.Add(Associations);
            }
        }

        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("[");
            
            foreach (IntConstant[] array in Matrix)
            {
                sb.Append("[");
                for (int i = 0; i < array.Length - 1; i++)
                {
                    IntConstant var = array[i];
                    sb.Append(var + ", ");
                }
                sb.Append(array[array.Length - 1].ToString());
                sb.Append("]");
            }

            sb.Append("]");
            return sb.ToString();
        }

        public override void BuildVars()
        {
            this.Vars = new List<string>(16);
            foreach (IntConstant[] array in Matrix)
            {
                foreach (IntConstant val in array)
                {
                    val.BuildVars();
                    Ultility.Ultility.Union(this.Vars, val.Vars);
                }                
            }
        }

        public override Expression ClearConstant(List<string> avoidsVars, Dictionary<string, Expression> constMapping, bool needClone)
        {
            if (!needClone)
            {                
                
                for (int j = 0; j < Matrix.Count; j++)
                {
                    IntConstant[] array = Matrix[j];
                    for (int i = 0; i < array.Length; i++)
                    {
                        array[i] = array[i].ClearConstant(avoidsVars, constMapping, needClone) as IntConstant;                     
                    }
                    Matrix[j] = array;
                }
                return this;                
            }
            else
            {
                List<IntConstant[]> newmaxtrix = new List<IntConstant[]>(this.Matrix.Count);
                for (int j = 0; j < Matrix.Count; j++)
                {
                    IntConstant[] array = Matrix[j];
                    IntConstant[] newArray = new IntConstant[array.Length];
                    for (int i = 0; i < array.Length; i++)
                    {
                        newArray[i] = array[i].ClearConstant(avoidsVars, constMapping, needClone) as IntConstant;
                    }
                    newmaxtrix.Add(newArray);
                }

                return new Array(newmaxtrix);
            }
        }
    }
}