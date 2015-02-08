using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using PAT.Common.Classes.ModuleInterface;
using PAT.Common.Classes.Ultility;

namespace PAT.Common.Classes.DataStructure
{
    public class DBMNode
    {
        public int Value;
        public bool Strict;

        public DBMNode(int value, bool strict)
        {
            Value = value;
            Strict = strict;
        }

        public DBMNode()
        {
            Value = 0;
            Strict = false;
        }

        public DBMNode(DBMNode node)
        {
            Value = node.Value;
            Strict = node.Strict;
        }

        //public void SetAs(int value, bool strict)
        //{
        //    Value = value;
        //    Strict = strict;
        //}

        //public void SetAs(DBMNode node)
        //{
        //    Value = node.Value;
        //    Strict = node.Strict;
        //}

        public DBMNode Clone()
        {
            return new DBMNode(Value, Strict);
        }

        public static bool operator <(DBMNode node1, DBMNode node2)
        {
            return (node1.Value < node2.Value)
                || (node1.Value == node2.Value && node1.Strict == true && node2.Strict == false);
        }

        public static bool operator >(DBMNode node1, DBMNode node2)
        {
            return node2 < node1;
        }

        public static DBMNode operator +(DBMNode node1, DBMNode node2)
        {
            return new DBMNode(node1.Value + node2.Value, node1.Strict || node2.Strict);
        }

        public static bool operator ==(DBMNode node1, DBMNode node2)
        {
            return node1.Value == node2.Value && node1.Strict == node2.Strict;
        }

        public static bool operator !=(DBMNode node1, DBMNode node2)
        {
            return !(node1 == node2);
        }
    }


    /// <summary>
    /// This class manipulates difference bound matrix for explicit timing requirements.
    /// </summary>
    public class FullDiffDBM
    {
        public static readonly DBMNode Zero = new DBMNode(0, false);
        public static readonly DBMNode Infinite = new DBMNode(int.MaxValue, true);
        public static readonly DBMNode Invalid = new DBMNode(-1, false);

        public bool IsEmpty
        {
            get
            {
                return Dimension == 1;
            }
        }

        public bool IsValid
        {
            get
            {
                return !(Matrix[0] < Zero);
            }
        }

        public bool IsInvalid
        {
            get
            {
                return Matrix[0] < Zero;
            }  
        }

        public DBMNode[] Matrix;
        public int Dimension;

        public int Pos(int x, int y)
        {
            return x * Dimension + y;
        }

        public FullDiffDBM(int dimension)
        {
            Dimension = dimension;
            Matrix = new DBMNode[Dimension * Dimension];
            for (int i = 0; i != Dimension * Dimension; ++i)
            {
                Matrix[i] = new DBMNode();
            }
        }

        public FullDiffDBM(int dim, DBMNode[] matrix)
        {
            Dimension = dim;
            Matrix = matrix;
        }

        public void Reset()
        {
            for (int i = 0; i != Dimension * Dimension; ++i)
            {
                Matrix[i] = new DBMNode(Zero);
            }
        }

        public void Reset(short x)
        {
            for (int i = 0; i != Dimension; ++i)
            {
                Matrix[Pos(x, i)] = new DBMNode(Matrix[Pos(0, i)]);
                Matrix[Pos(i, x)] = new DBMNode(Matrix[Pos(i, 0)]);
            }
        }

        //public void ResetAs(short x, int constant)
        //{
        //    for (int i = 0; i != Dimension; ++i)
        //    {
        //        Matrix[Pos(x, i)] = Matrix[Pos(0, i)] + new DBMNode(constant, false);
        //        Matrix[Pos(i, x)] = Matrix[Pos(i, 0)] + new DBMNode(-1 * constant, false);
        //    }
        //}

        public void Free(short x)
        {
            for (int i = 0; i != Dimension; ++i)
            {
                if (i == x)
                {
                    continue;
                }

                Matrix[Pos(x, i)] = new DBMNode(Infinite);
                Matrix[Pos(i, x)] = new DBMNode(Matrix[Pos(i, 0)]);
            }
        }

        public void TotalFree(short x)
        {
            for (int i = 0; i != Dimension; ++i)
            {
                if (i == x)
                {
                    continue;
                }

                Matrix[Pos(x, i)] = new DBMNode(Infinite);
                Matrix[Pos(i, x)] = new DBMNode(Infinite);
            }
        }

        public void Extrapolation(int[,] bound)
        {
            for (int i = 0; i != Dimension; ++i)
            {
                for (int j = 0; j != Dimension; ++j)
                {
                    if (Matrix[Pos(i, j)].Value > -1 * bound[Pos(j, i), 0])
                    {
                        Matrix[Pos(i, j)] = new DBMNode(Infinite);
                    }
                    else if (Matrix[Pos(i, j)].Value < -1 * bound[Pos(j, i), 1])
                    {
                        Matrix[Pos(i, j)] = new DBMNode(-1 * bound[Pos(j, i), 1], true);
                    }
                }
            }
        }

        public void Close()
        {
            for (int k = 0; k < Dimension; k++)
            {
                for (int i = 0; i < Dimension; i++)
                {
                    for (int j = 0; j < Dimension; j++)
                    {
                        if (i != k && k != j && j != i)
                        {
                            if (Matrix[Pos(i, k)] != Infinite && Matrix[Pos(k, j)] != Infinite)
                            {
                                DBMNode newNode = Matrix[Pos(i, k)] + Matrix[Pos(k, j)];
                                if (newNode < Matrix[Pos(i, j)])
                                {
                                    Matrix[Pos(i, j)] = newNode;
                                }
                            }
                        }
                    }

                    if (Matrix[Pos(i, i)] < Zero)
                    {
                        Matrix[0] = new DBMNode(Invalid);
                        return;
                    }
                }
            }
        }

        public void AddConstraint(short x, short y, TimerOperationType op, int constant)
        {
            Debug.Assert(x != y);
            Debug.Assert(x >= 0);
            Debug.Assert(y >= 0);

            switch (op)
            {
                case TimerOperationType.Equals:
                    AddConstraint(x, y, false, constant);
                    AddConstraint(y, x, false, -1 * constant);
                    break;
                case TimerOperationType.GreaterThanOrEqualTo:
                    AddConstraint(y, x, false, -1 * constant);
                    break;
                case TimerOperationType.LessThanOrEqualTo:
                    AddConstraint(x, y, false, constant);
                    break;
                case TimerOperationType.GreaterThan:
                    AddConstraint(y, x, true, -1 * constant);
                    break;
                case TimerOperationType.LessThan:
                    AddConstraint(x, y, true, constant);
                    break;
            }
        }

        // constraint of the form: (x - y < c) or (x - y <= c)
        protected void AddConstraint(short x, short y, bool strict, int constant)
        {
            DBMNode node = new DBMNode(constant, strict);
            DBMNode tmp;

            if (Matrix[Pos(y, x)] != Infinite && node != Infinite && Matrix[Pos(y, x)] + node < Zero)
            {
                Matrix[0] = new DBMNode(Invalid);
                return;
            }
            else if (node < Matrix[Pos(x, y)])
            {
                Matrix[Pos(x, y)] = node;
                for (int i = 0; i != Dimension; ++i)
                {
                    for (int j = 0; j != Dimension; ++j)
                    {
                        if (Matrix[Pos(i, x)] != Infinite && Matrix[Pos(x, j)] != Infinite)
                        {
                            tmp = Matrix[Pos(i, x)] + Matrix[Pos(x, j)];
                            if (tmp < Matrix[Pos(i, j)])
                            {
                                Matrix[Pos(i, j)] = tmp;
                            }
                        }
                        if (Matrix[Pos(i, y)] != Infinite && Matrix[Pos(y, j)] != Infinite)
                        {
                            tmp = Matrix[Pos(i, y)] + Matrix[Pos(y, j)];
                            if (tmp < Matrix[Pos(i, j)])
                            {
                                Matrix[Pos(i, j)] = tmp;
                            }
                        }
                    }
                }
            }
        }

        public void Delay()
        {
            for (int i = 1; i < Dimension; i++)
            {
                Matrix[Pos(i, 0)] = new DBMNode(Infinite);
            }
        }

        public String ToString(Dictionary<string, short> clockMapping)
        {
            if (IsEmpty)
            {
                return "";
            }

            StringBuilder sb = new StringBuilder();

            foreach (KeyValuePair<string, short> pair1 in clockMapping)
            {
                string x = pair1.Key;
                int i = pair1.Value;

                sb.Append(x);
                sb.AppendLine((Matrix[Pos(i, 0)].Strict ? " < " : " <= ")
                            + (Matrix[Pos(i, 0)].Value == int.MaxValue ?
                                Constants.INFINITE : Matrix[Pos(i, 0)].Value.ToString()));

                sb.Append(x);
                sb.AppendLine((Matrix[Pos(i, 0)].Strict ? " > " : " >= ")
                            + (Matrix[Pos(i, 0)].Value == int.MaxValue ?
                                "-" + Constants.INFINITE : (-1 * Matrix[Pos(i, 0)].Value).ToString()));

                foreach (KeyValuePair<string, short> pair2 in clockMapping)
                {
                    string y = pair2.Key;
                    int j = pair2.Value;

                    if (i == j)
                    {
                        continue;
                    }

                    sb.Append(x + " - " + y);
                    sb.AppendLine((Matrix[Pos(i, j)].Strict ? " < " : " <= ")
                                + (Matrix[Pos(i, j)].Value == int.MaxValue ?
                                    Constants.INFINITE : Matrix[Pos(i, j)].Value.ToString()));
                }
            }

            return sb.ToString();
        }

        public override String ToString()
        {
            return GetID();
        }

        public FullDiffDBM Clone()
        {
            return new FullDiffDBM(Dimension, (DBMNode[])Matrix.Clone());
        }

        public string GetID()
        {
            if (IsEmpty)
            {
                return "";
            }

            StringBuilder sb = new StringBuilder();

            sb.Append(Constants.SEPARATOR);
            for (int i = 0; i != Dimension; i++)
            {
                for (int j = 0; j != Dimension; j++)
                {
                    if (i != j)
                    {
                        sb.Append((Matrix[Pos(i, j)].Strict ? "" : "=") 
                            + (Matrix[Pos(i, j)].Value == int.MaxValue ? 
                                Constants.INFINITE : Matrix[Pos(i, j)].Value.ToString()) + Constants.SEPARATOR);
                    }
                }
            }

            return sb.ToString();
        }
    }
}
