using System;
using System.Collections.Generic;
using System.Text;
using PAT.Common;

namespace PAT.ModelChecking.Expressions.ExpressionClass
{
    public sealed class Record2D : Expression
    {
        public Expression[,] Associations;
        private int Size1;
        private int Size2;

        public Record2D(Expression[,] ass)
        {
            Associations = ass;
            //BuildVars();
            ExpressionType = ExpressionType.Record;
        }

        public Record2D(int size1, int size2)
        {            
            Associations = new Expression[size1,size2];
            for (int i = 0; i < size1; i++)
            {
                 for (int j = 0; j < size2; j++)
                 {
                     Associations[i,j] = new IntConstant(0);
                 }
            }
            ExpressionType = ExpressionType.Record;
            Size1 = size1;
            Size2 = size2;
        }

        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("[");

            for (int i = 0; i < Size1-1; i++)
            {
                sb.Append("[");
                for (int j = 0; j < Size2-1; j++)
                {
                    sb.Append(Associations[i,j] + ", ");
                }
                sb.Append(Associations[i,Size2 -1]);
                sb.Append("],");
            }

            sb.Append("[");
            for (int j = 0; j < Size2 - 1; j++)
            {
                sb.Append(Associations[Size1 -1,j] + ", ");
            }
            sb.Append(Associations[Size1 -1,Size2 - 1]);
            sb.Append("]]");
            return sb.ToString();
        }

        public override void BuildVars()
        {
            this.Vars = new List<string>(16);
            for (int i = 0; i < Size1; i++)
            {
                for (int j = 0; j < Size2; j++)
                {
                    Associations[i,j].BuildVars();
                    Ultility.Union(this.Vars, Associations[i,j].Vars);
                }
            }
        }

        public override Expression ClearConstant(List<string> avoidsVars, Dictionary<string, Expression> constMapping, bool needClone)
        {
            if (!needClone)
            {               
                for (int i = 0; i < Size1; i++)
                {
                    for (int j = 0; j < Size2; j++)
                    {
                        Associations[i,j] = Associations[i,j].ClearConstant(avoidsVars, constMapping, needClone);
                    }
                }
                return this;
            }
            else
            {
                Expression[,] newAssociations = new Expression[Size1,Size2]; 

                for (int i = 0; i < Size1; i++)
                {
                    for (int j = 0; j < Size2; j++)
                    {
                        newAssociations[i,j] = Associations[i,j].ClearConstant(avoidsVars, constMapping, needClone);
                    }
                }

                Record2D newRrd = new Record2D(newAssociations);
                newRrd.Vars = new List<string>();
                for (int i = 0; i < Size1; i++)
                {
                    for (int j = 0; j < Size2; j++)
                    {
                        Ultility.Union(newRrd.Vars, newRrd.Associations[i,j].Vars);
                    }
                }

                return newRrd;
            }
        }
    }
}