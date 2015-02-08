#define CONTRACTS_FULL 
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using PAT.Common.Classes.Ultility;

namespace PAT.Common.Classes.DataStructure
{
    public enum TimerOperationType : byte
    {
        Equals,               
        LessThanOrEqualTo,
        GreaterThanOrEqualTo,
        LessThan,
        GreaterThan
    }

    /// <summary>
    /// This class manipulates difference bound matrix for explicit timing requirements.
    /// </summary>
    public sealed class DBM
    {
        /// <summary>
        /// matrix to store the DBM
        /// </summary>
        public List<List<int>> Matrix;

        //if DBM is empty, it is in connoicalform
        private bool IsCanonicalForm = true;

        public DBM()
        {
            Matrix = new List<List<int>>(Ultility.Constants.DBM_INIT_SIZE);
            List<int> constantRow = new List<int>(Ultility.Constants.DBM_INIT_SIZE);
            constantRow.Add(0);
            Matrix.Add(constantRow);
        }

        public DBM(List<List<int>> matrix, bool isCanonical)//, byte lastestTimer, bool lastestTimerIsNew)
        {
            Matrix = matrix;
            IsCanonicalForm = isCanonical;
        }

        public void AddTimer(int timerID)
        {
            if (!IsCanonicalForm)
            {
                GetCanonicalForm();
            }

            Matrix[0].Add(0);
            for (int i = 1; i < Matrix.Count; i++)
            {
                Matrix[i].Add(Matrix[i][0]);
            }

            List<int> newTimerRow = new List<int>(Matrix[0].Count);
            
            //the last row are all 0
            //newTimerRow.AddRange(new int[TimerArray.Count + 1]);
            for (int i = 0; i < Matrix[0].Count; i++)
            {
                newTimerRow.Add(Matrix[0][i]);
            }

            //add the last row
            Matrix.Add(newTimerRow);
        }

        public int GetTimerUpper(int timerID)
        {
            Debug.Assert(IsCanonicalForm);
            return Matrix[timerID][0];
        }

        public int GetTimerLower(int timerID)
        {
            Debug.Assert(IsCanonicalForm);
            return Matrix[0][timerID] * -1;
        }

        public void GetCanonicalForm()
        {
            int dimention = Matrix.Count;
            for (int k = 0; k < dimention; k++)
            {
                for (int i = 0; i < dimention; i++)
                {
                    if (i != k)
                    {
                        for (int j = 0; j < dimention; j++)
                        {
                            //check for the overflow problem
                            if (Matrix[i][k] != int.MaxValue && Matrix[k][j] != int.MaxValue)
                            {
                                //Attension, Matrix[i][k] + Matrix[k][j] is bigger than int.MaxValue, there is a possbility of overflow.
                                Matrix[i][j] = Math.Min(Matrix[i][j], Matrix[i][k] + Matrix[k][j]);
                            }
                        }

                        if (Matrix[i][i] < 0)
                        {
                            IsCanonicalForm = true;
                            Matrix[0][0] = -1;
                            return;
                        }
                    }
                }
            }

            IsCanonicalForm = true;
        }

        public void Down ()
        {
            int dimension = Matrix.Count;
            for (int i = 1; i < dimension; i++)
            {
                Matrix[0][i] = 0;

                for (int j = 1; j < dimension; j++)
                {
                    if (Matrix[j][i] < Matrix[0][i])
                    {
                        Matrix[0][i] = Matrix[j][i];
                    }
                }
            }
        }

        /// <summary>
        /// Add the clock constraint into the DBM, after adding the DBM is still in carnomical form
        /// </summary>
        /// <param name="timerID"></param>
        /// <param name="op"></param>
        /// <param name="constant"></param>
        public void AddConstraint(int timerID, TimerOperationType op, int constant)
        {
            Debug.Assert(timerID != 0);

            switch (op)
            {
                case TimerOperationType.Equals:
                    AddConstraintXY(timerID, 0, constant);
                    AddConstraintXY(0, timerID, -1*constant);
                    break;
                case TimerOperationType.GreaterThanOrEqualTo:
                    AddConstraintXY(0, timerID, -1 * constant);
                    break;
                case TimerOperationType.LessThanOrEqualTo:
                    AddConstraintXY(timerID, 0, constant);
                    break;
            }
        }

        private void AddConstraintXY(int x, int y, int constant) //t_timer - t_0 <= constant
        {
            if (Matrix[y][x] + constant < 0)
            {
                Matrix[0][0] = -1;
            }
            else if (Matrix[x][y] > constant)
            {
                Matrix[x][y] = constant;

                for (int i = 0; i < Matrix.Count; i++)
                {
                    for (int j = 0; j < Matrix.Count; j++)
                    {
                        if (Matrix[i][x] != int.MaxValue && Matrix[x][j] != int.MaxValue && Matrix[i][x] + Matrix[x][j] < Matrix[i][j])
                        {
                            Matrix[i][j] = Matrix[i][x] + Matrix[x][j];
                        }

                        if (Matrix[i][y] != int.MaxValue && Matrix[y][j] != int.MaxValue && Matrix[i][y] + Matrix[y][j] < Matrix[i][j])
                        {
                            Matrix[i][j] = Matrix[i][y] + Matrix[y][j];
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Check whether the DBM satisfies a given primitive constraint 
        /// </summary>
        /// <param name="timerID"></param>
        /// <param name="op"></param>
        /// <param name="constant"></param>
        public bool Implies(int timerID, TimerOperationType op, int constant)
        {
            if (!IsCanonicalForm)
            {
                GetCanonicalForm();
            }

            switch (op)
            {
                case TimerOperationType.Equals:
                    return Matrix[timerID][0] == constant && Matrix[0][timerID] == -1 * constant;
                case TimerOperationType.GreaterThanOrEqualTo:
                    return Matrix[0][timerID] <= -1 * constant;
                default: //TimerOperationType.LessThanOrEqualTo:
                    return Matrix[timerID][0] <= constant;
            }
        }

        public DBM AddUrgency()
        {
            DBM toReturn = this.Clone();
            toReturn.AddConstraint(Matrix.Count-1, TimerOperationType.LessThanOrEqualTo, 0);
            return toReturn;
        }

        public bool IsUrgent()
        {
            return GetTimerUpper(Matrix.Count - 1) == 0;
        }

        public static DBM ReturnUrgentDBM(DBM dbm1, DBM dbm2)
        {
            if (dbm1.IsUrgent())
            {
                return dbm1;
            }

            return dbm2;
        }

        /// <summary>
        /// Delay the DBM and change directly.
        /// </summary>
        public void DelayWithoutClone()
        {
            Debug.Assert(IsCanonicalForm);

            for (int i = 1; i < Matrix.Count; i++)
            {
                Matrix[i][0] = int.MaxValue;
            }
        }

        /// <summary>
        /// Return a cloned DBM which contains only active timers
        /// </summary>
        /// <param name="activeTimer"></param>
        /// <returns></returns>
        public DBM CleanAndRename(Dictionary<int,int> mapping, int clockCounter)
        {
            Debug.Assert(this.IsCanonicalForm); 
            DBM toReturn = new DBM();
            List<int> newClocks = new List<int>();

            for (int i = 1; i <= clockCounter; i++)
            {             
                toReturn.Matrix[0].Add(0);
                List<int> newRow = new List<int>();
                for (int j = 0; j <= clockCounter; j++)
                {
                    newRow.Add(0);
                }
                toReturn.Matrix.Add(newRow);

                if (!mapping.ContainsValue(i))
                {
                    newClocks.Add(i);
                }
            }

            foreach (KeyValuePair<int, int> keyValuePair in mapping)
            {
                toReturn.Matrix[0][keyValuePair.Value] = Matrix[0][keyValuePair.Key];
                toReturn.Matrix[keyValuePair.Value][0] = Matrix[keyValuePair.Key][0];

                foreach (int newClock in newClocks)
                {
                    toReturn.Matrix[newClock][keyValuePair.Value] = Matrix[0][keyValuePair.Key];
                    toReturn.Matrix[keyValuePair.Value][newClock] = Matrix[keyValuePair.Key][0];
                }

                foreach (KeyValuePair<int, int> valuePair in mapping)
                {
                    toReturn.Matrix[valuePair.Value][keyValuePair.Value] = Matrix[valuePair.Key][keyValuePair.Key];
                    toReturn.Matrix[keyValuePair.Value][valuePair.Value] = Matrix[keyValuePair.Key][valuePair.Key];                    
                }
            }

            return toReturn;
        }

        public bool IsConstraintSatisfied()
        {
            if (!IsCanonicalForm)
            {
                GetCanonicalForm();
            }

            return Matrix[0][0] >= 0;
        }

        public override String ToString()
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 1; i < Matrix.Count; i++)
            {
                if (Matrix[0][i] >= 0)
                {
                    sb.AppendLine("c" + i + " <= " + (Matrix[i][0]==int.MaxValue?"inf":Matrix[i][0].ToString()));                    
                }
                else
                {
                    sb.AppendLine(-1 * Matrix[0][i] + " <= c" + i + " <= " + (Matrix[i][0] == int.MaxValue ? "inf" : Matrix[i][0].ToString()));                                        
                }

                for (int k = 1; k < Matrix.Count; k++)
                {
                    if (i != k)
                    {
                        if (Matrix[i][k] >=0)
                        {
                            sb.AppendLine("c" + i + " - c" + k + " <= " + (Matrix[i][k] == int.MaxValue ? "inf" : Matrix[i][k].ToString()));
                        }
                        else
                        {
                            sb.AppendLine("c" + k + " - c" + i + " >= " + -1*Matrix[i][k]);
                        }
                    }
                }
            }

            return sb.ToString();
        }

        public DBM Clone()
        {
            List<List<int>> newMatrix = new List<List<int>>();
            for (int i = 0; i < Matrix.Count; i++)
            {
                List<int> newRow = new List<int>();
                for (int j = 0; j < Matrix.Count; j++)
                {
                    newRow.Add(Matrix[i][j]);
                }
                newMatrix.Add(newRow);
            }

            return new DBM(newMatrix, IsCanonicalForm);
        }

        public string GetID()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 1; i < Matrix.Count; i++)
            {
                if (Matrix[i][0] > 0)
                {
                    sb.Append(i);
                    for (int k = 0; k < Matrix.Count; k++)
                    {
                        if (i != k)
                        {
                            sb.Append(Constants.SEPARATOR + Matrix[i][k] + Constants.SEPARATOR + Matrix[k][i]);
                        }
                    }
                }
            }

            return sb.ToString();
        }
    }
}
