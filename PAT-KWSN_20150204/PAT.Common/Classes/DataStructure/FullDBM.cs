using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using PAT.Common.Classes.ModuleInterface;
using PAT.Common.Classes.Ultility;

namespace PAT.Common.Classes.DataStructure
{
    /// <summary>
    /// This class manipulates difference bound matrix for explicit timing requirements.
    /// </summary>
    public class FullDBM
    {
        public int[] ClockMaxValues;
        public int[] ClockLowerValues;
        public int[] ClockUpperValues;

        public bool IsEmpty
        {
            get
            {
                return dimention == 1;
            }
        }

        //matrix to store the 
        public int[] Matrix;
        public bool[] MatrixStrictness;

        //if FullDBM is empty, it is in connoicalform
        public bool IsCanonicalForm = true;
        public int dimention;

        public FullDBM(int dim)
        {
            dimention = dim;
            Matrix = new int[dimention * dimention];
            MatrixStrictness = new bool[Matrix.Length];
            ClockMaxValues = new int[dimention];
            ClockLowerValues = new int[dimention];
            ClockUpperValues = new int[dimention];

            for (int i = 1; i < dim; i++)
            {
                ClockMaxValues[i] = int.MinValue;
                ClockLowerValues[i] = int.MinValue;
                ClockUpperValues[i] = int.MinValue;
            }
        }

        public FullDBM(int dim, int[] matrix, bool[] matrixStrictness, bool isCanonical, int[] clockMaxValues, int[] clockLowerValues, int[] clockUpperValues)
        {
            dimention = dim;
            Matrix = matrix;
            MatrixStrictness = matrixStrictness;
            IsCanonicalForm = isCanonical;

            ClockMaxValues = clockMaxValues;
            ClockLowerValues = clockLowerValues;
            ClockUpperValues = clockUpperValues;
        }

        public int GetTimerUpper(short timerID)
        {
            Debug.Assert(IsCanonicalForm);
            return Matrix[timerID * dimention];
        }

        public void ResetTimer(short timerid)
        {
            Debug.Assert(IsCanonicalForm);

            for (int i = 0; i < dimention; i++)
            {
                if (i == timerid)
                {
                    continue;
                }
                Matrix[i * dimention + timerid] = Matrix[i * dimention + 0];
                Matrix[timerid * dimention + i] = Matrix[0 * dimention + i];
                MatrixStrictness[i * dimention + timerid] = MatrixStrictness[i * dimention + 0];
                MatrixStrictness[timerid * dimention + i] = MatrixStrictness[0 * dimention + i];
            }
        }

        // For special purpose only
        // This is different from free them individually 
        // Do not use this function when you are not sure
        public void FreeTimer(short timerid)
        {
            Debug.Assert(IsCanonicalForm);
            for (int i = 0; i != dimention; ++i)
            {
                if (i == timerid)
                {
                    continue;
                }
                Matrix[Pos(timerid, i)] = int.MaxValue;
                MatrixStrictness[Pos(timerid, i)] = true;
                //Matrix[Pos(i, timerid)] = Matrix[Pos(i, 0)];
                //MatrixStrictness[Pos(i, timerid)] = MatrixStrictness[Pos(i, 0)];
                Matrix[Pos(i, timerid)] = int.MaxValue;
                MatrixStrictness[Pos(i, timerid)] = true;
            }
        }

        // For special purpose only
        // This is different from free them individually 
        // Do not use this function when you are not sure
        //public void FreeTimer(short[] timerids)
        //{
        //    Debug.Assert(IsCanonicalForm);
        //    foreach (short timerid in timerids)
        //    {
        //        for (int i = 0; i != dimention; ++i)
        //        {
        //            if (i == timerid)
        //            {
        //                continue;
        //            }
        //            Matrix[Pos(timerid, i)] = int.MaxValue;
        //            MatrixStrictness[Pos(timerid, i)] = true;
        //            Matrix[Pos(i, timerid)] = int.MaxValue;
        //            MatrixStrictness[Pos(i, timerid)] = true;
        //        }
        //    }
        //    foreach (short x in timerids)
        //    {
        //        foreach (short y in timerids)
        //        {
        //            if (x == y)
        //            {
        //                continue;
        //            }

        //            Matrix[Pos(x, y)] = 0;
        //            MatrixStrictness[Pos(x, y)] = false;
        //        }
        //    }
        //}

        private void GetCanonicalForm()
        {
            if (IsCanonicalForm)
            {
                return;
            }

            close();
            Extrapolation();
            IsCanonicalForm = true;
        }

        /* extrapolation and Close as mentioned in paper#2 Gerd and Larsen*/
        private void Extrapolation()
        {
            if (Matrix[0] >= 0) //!SpecificationBase.IsSimulation && 
            {
                for (int i = 0; i < dimention; i++)
                {
                    for (int j = 0; j < dimention; j++)
                    {
                        ////=======================diaganol L/U extrapolation======================
                        if (Matrix[i * dimention + j] > ClockLowerValues[i]) //i > 0 && ClockLowerValues[i] > 0 && 
                        {
                            Matrix[i * dimention + j] = int.MaxValue;
                        }

                        if (-1 * Matrix[i] > ClockLowerValues[i]) //i > 0 && ClockLowerValues[i] > 0 && 
                        {
                            Matrix[i * dimention + j] = int.MaxValue;
                        }

                        if (i != 0 && -1 * Matrix[j] > ClockUpperValues[j]) //j > 0 && ClockUpperValues[j] > 0 && 
                        {
                            Matrix[i * dimention + j] = int.MaxValue;
                        }

                        if (i == 0 && -1 * Matrix[i * dimention + j] > ClockUpperValues[j]) //j > 0 && ClockUpperValues[j] > 0 &&  
                        {
                            Matrix[i * dimention + j] = -1 * ClockUpperValues[j];
                            MatrixStrictness[i * dimention + j] = true;
                        }
                        ////=======================diaganol L/U extrapolation======================

                        //////=======================simple diaganol L/U extrapolation======================
                        //////=======================simple diaganol L/U extrapolation======================
                    }
                }
            }
        }

        private void GetCanonicalForm(int[,] bound)
        {
            if (IsCanonicalForm)
            {
                return;
            }

            close();
            if (bound != null)
            {
                Extrapolation(bound);
            }
            IsCanonicalForm = true;
        }

        private void Extrapolation(int[,] bound)
        {
            for (int j = 0; j != dimention; ++j)
            {
                if (Matrix[Pos(0, j)] > -1 * bound[Pos(j, 0), 0])
                {
                    Matrix[Pos(0, j)] = 0;
                    MatrixStrictness[Pos(0, j)] = true;
                }
                else if (Matrix[Pos(0, j)] < -1 * bound[Pos(j, 0), 1])
                {
                    Matrix[Pos(0, j)] = -1 * bound[Pos(j, 0), 1];
                    MatrixStrictness[Pos(0, j)] = true;
                }
            }

            for (int i = 1; i != dimention; ++i)
            {
                for (int j = 0; j != dimention; ++j)
                {
                    if (Matrix[Pos(i, j)] > -1 * bound[Pos(j, i), 0])
                    {
                        Matrix[Pos(i, j)] = int.MaxValue;
                        MatrixStrictness[Pos(i, j)] = true;
                    }
                    else if (Matrix[Pos(i, j)] < -1 * bound[Pos(j, i), 1])
                    {
                        Matrix[Pos(i, j)] = -1 * bound[Pos(j, i), 1];
                        MatrixStrictness[Pos(i, j)] = true;
                    }
                }
            }
        }

        private void close()
        {
            for (int k = 0; k < dimention; k++)
            {
                for (int i = 0; i < dimention; i++)
                {
                    if (i != k)
                    {
                        for (int j = 0; j < dimention; j++)
                        {
                            //check for the overflow problem
                            if (Matrix[i * dimention + k] != int.MaxValue && Matrix[k * dimention + j] != int.MaxValue)
                            {
                                if (Matrix[i * dimention + j] > Matrix[i * dimention + k] + Matrix[k * dimention + j])
                                {
                                    Matrix[i * dimention + j] = Matrix[i * dimention + k] + Matrix[k * dimention + j];
                                    MatrixStrictness[i * dimention + j] = MatrixStrictness[i * dimention + k] || MatrixStrictness[k * dimention + j];
                                }
                                else if (Matrix[i * dimention + j] == Matrix[i * dimention + k] + Matrix[k * dimention + j])
                                {
                                    MatrixStrictness[i * dimention + j] = MatrixStrictness[i * dimention + j] || (MatrixStrictness[i * dimention + k] || MatrixStrictness[k * dimention + j]);
                                }
                            }
                        }
                    }
                    if (Matrix[i * dimention + i] < 0 || MatrixStrictness[0 * dimention + 0])
                    {
                        IsCanonicalForm = true;
                        Matrix[0 * dimention + 0] = -1;
                        return;
                    }
                }
            }
        }

        private void close_LU()
        {
            //// for k E low ^ up :
            //      for i E low:
            //          for j E up:
            //              c(i,j) = min(c(i,k) + c(k,j),c(i,j)
            // new faster subsitute of floyd warshall algorithm
            // O(|low||up||low^up|)

            for (int i = 0; i < dimention; i++)
            {
                for (int j = 0; j < dimention; j++)
                {
                    if (ClockLowerValues[i] == -int.MaxValue || ClockUpperValues[j] == -int.MaxValue)
                    {
                        Matrix[i * dimention + j] = int.MaxValue;
                    }
                }
            }

            for (int k = 0; k < dimention && ClockLowerValues[k] > -int.MaxValue && ClockUpperValues[k] > -int.MaxValue; k++)
            {
                for (int i = 0; i < dimention && i != k && ClockLowerValues[i] > -int.MaxValue; i++)
                {
                    for (int j = 0; j < dimention && ClockUpperValues[j] > -int.MaxValue; j++)
                    {
                        if (Matrix[i * dimention + k] != int.MaxValue && Matrix[k * dimention + j] != int.MaxValue)
                        {
                            if (Matrix[i * dimention + j] > Matrix[i * dimention + k] + Matrix[k * dimention + j])
                            {
                                Matrix[i * dimention + j] = Matrix[i * dimention + k] + Matrix[k * dimention + j];
                                MatrixStrictness[i * dimention + j] = MatrixStrictness[i * dimention + k] || MatrixStrictness[k * dimention + j];
                            }
                            else if (Matrix[i * dimention + j] == Matrix[i * dimention + k] + Matrix[k * dimention + j])
                            {
                                MatrixStrictness[i * dimention + j] = MatrixStrictness[i * dimention + j] || (MatrixStrictness[i * dimention + k] || MatrixStrictness[k * dimention + j]);
                            }
                        }

                    }
                }
            }
            for (int i = 0; i < dimention; i++)
            {
                if (Matrix[i * dimention + i] < 0 || MatrixStrictness[0 * dimention + 0])
                {
                    IsCanonicalForm = true;
                    Matrix[0 * dimention + 0] = -1;
                    return;
                }
            }
        }

        /// <summary>
        /// Update the FullDBM with a new constraint
        /// </summary>
        /// <param name="timer">which timer the constraint is on</param>
        /// <param name="op">0 for equal; 1 for >=; -1 for <=</param>
        /// <param name="constant"></param>
        public void AddConstraint(short timer, TimerOperationType op, int constant)
        {
            Debug.Assert(timer > 0);

            if (Matrix[timer * dimention] != int.MaxValue && Matrix[timer] != int.MaxValue && Matrix[timer * dimention] + Matrix[timer] < 0)
            {
                IsCanonicalForm = true;
                Matrix[0] = -1;
                return;
            }

            switch (op)
            {
                case TimerOperationType.Equals:
                    if (Matrix[timer * dimention] > constant)
                    {
                        Matrix[timer * dimention] = constant;
                        MatrixStrictness[timer * dimention] = false;
                    }
                    if (Matrix[timer] > -1 * constant)
                    {
                        Matrix[timer] = -1 * constant;
                        MatrixStrictness[timer] = false;
                    }
                    break;
                case TimerOperationType.GreaterThanOrEqualTo:
                    if (Matrix[timer] > -1 * constant)
                    {
                        Matrix[timer] = -1 * constant;
                        MatrixStrictness[timer] = false;
                    }
                    break;
                case TimerOperationType.LessThanOrEqualTo:
                    if (Matrix[timer * dimention] > constant)
                    {
                        Matrix[timer * dimention] = constant;
                        MatrixStrictness[timer * dimention] = false;
                    }
                    break;
                case TimerOperationType.GreaterThan:
                    if (Matrix[timer] >= -1 * constant)
                    {
                        Matrix[timer] = -1 * constant;
                        MatrixStrictness[timer] = true;
                    }
                    break;
                case TimerOperationType.LessThan:
                    if (Matrix[timer * dimention] >= constant)
                    {
                        Matrix[timer * dimention] = constant;
                        MatrixStrictness[timer * dimention] = true;
                    }
                    break;
            }

            for (int i = 0; i < dimention; i++)
            {
                for (int j = 0; j < dimention; j++)
                {
                    if (Matrix[i * dimention + timer] != int.MaxValue && Matrix[timer * dimention + j] != int.MaxValue)
                    {
                        if (Matrix[i * dimention + j] > Matrix[i * dimention + timer] + Matrix[timer * dimention + j])
                        {
                            Matrix[i * dimention + j] = Matrix[i * dimention + timer] + Matrix[timer * dimention + j];
                            MatrixStrictness[i * dimention + j] = MatrixStrictness[i * dimention + timer] || MatrixStrictness[timer * dimention + j];
                        }
                        else if (Matrix[i * dimention + j] == Matrix[i * dimention + timer] + Matrix[timer * dimention + j])
                        {
                            MatrixStrictness[i * dimention + j] = MatrixStrictness[i * dimention + j] || (MatrixStrictness[i * dimention + timer] || MatrixStrictness[timer * dimention + j]);
                        }
                    }
                    if (Matrix[i * dimention] != int.MaxValue && Matrix[j] != int.MaxValue)
                    {
                        if (Matrix[i * dimention + j] > Matrix[i * dimention] + Matrix[j])
                        {
                            Matrix[i * dimention + j] = Matrix[i * dimention] + Matrix[j];
                            MatrixStrictness[i * dimention + j] = MatrixStrictness[i * dimention] || MatrixStrictness[j];
                        }
                        else if (Matrix[i * dimention + j] == Matrix[i * dimention] + Matrix[j])
                        {
                            MatrixStrictness[i * dimention + j] = MatrixStrictness[i * dimention + j] || (MatrixStrictness[i * dimention] || MatrixStrictness[j]);
                        }
                    }
                }

                if (Matrix[i * dimention + i] < 0 || MatrixStrictness[0])
                {
                    IsCanonicalForm = true;
                    Matrix[0] = -1;
                    return;
                }
            }

            Extrapolation();
            IsCanonicalForm = true;
        }

        public void AddConstraint(short x, short y, TimerOperationType op, int constant, int[,] bound)
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

            if (bound != null)
            {
                Extrapolation(bound);
            }
            IsCanonicalForm = true;
        }

        public int Pos(int x, int y)
        {
            return x * dimention + y;
        }

        // constraint of the form: (x - y < c) or (x - y <= c)
        protected void AddConstraint(short x, short y, bool strict, int constant)
        {
            int v;
            bool s;

            if ((Matrix[Pos(y, x)] < -1 * constant) || (Matrix[Pos(y, x)] + constant == 0 && (MatrixStrictness[Pos(y, x)] || strict) == true))
            {
                IsCanonicalForm = true;
                Matrix[0] = -1;
                return;
            }
            else if (constant < Matrix[Pos(x, y)] || (constant == Matrix[Pos(x, y)] && strict == true && MatrixStrictness[Pos(x, y)] == false))
            {
                Matrix[Pos(x, y)] = constant;
                MatrixStrictness[Pos(x, y)] = strict;
                for (int i = 0; i != dimention; ++i)
                {
                    for (int j = 0; j != dimention; ++j)
                    {
                        if (Matrix[Pos(i, x)] != int.MaxValue && Matrix[Pos(x, j)] != int.MaxValue)
                        {
                            v = Matrix[Pos(i, x)] + Matrix[Pos(x, j)];
                            s = MatrixStrictness[Pos(i, x)] || MatrixStrictness[Pos(x, j)];
                            if (v < Matrix[Pos(i, j)] || (v == Matrix[Pos(i, j)] && MatrixStrictness[Pos(i, j)] == false && s == true))
                            {
                                Matrix[Pos(i, j)] = v;
                                MatrixStrictness[Pos(i, j)] = s;
                            }
                        }
                        if (Matrix[Pos(i, y)] != int.MaxValue && Matrix[Pos(y, j)] != int.MaxValue)
                        {
                            v = Matrix[Pos(i, y)] + Matrix[Pos(y, j)];
                            s = MatrixStrictness[Pos(i, y)] || MatrixStrictness[Pos(y, j)];
                            if (v < Matrix[Pos(i, j)] || (v == Matrix[Pos(i, j)] && MatrixStrictness[Pos(i, j)] == false && s == true))
                            {
                                Matrix[Pos(i, j)] = v;
                                MatrixStrictness[Pos(i, j)] = s;
                            }
                        }
                    }
                }
            }
        }

        public void Delay()
        {
            Debug.Assert(IsCanonicalForm); /* chengbin */

            for (int i = 1; i < dimention; i++)
            {
                Matrix[i * dimention] = int.MaxValue; // index + 0
                MatrixStrictness[i * dimention] = true; // index +  0
            }
        }

        public bool IsConstraintNotSatisfied()
        {
            if (IsEmpty)
            {
                return false;
            }

            if (!IsCanonicalForm)
            {
                GetCanonicalForm();
            }

            return Matrix[0 * dimention + 0] < 0 || MatrixStrictness[0 * dimention + 0];
        }

        public bool IsConstraintNotSatisfied(int[,] bound)
        {
            if (IsEmpty)
            {
                return false;
            }

            if (!IsCanonicalForm)
            {
                GetCanonicalForm(bound);
            }

            if (bound != null)
            {
                Extrapolation(bound);
            }

            return Matrix[0 * dimention + 0] < 0 || MatrixStrictness[0 * dimention + 0];
        }

        /// <summary>
        /// this method should be avoided as it seemed incorrect working with extrapolation
        /// </summary>
        /// <param name="myDBM"></param>
        public void Conjunction(FullDBM myDBM)
        {
            if (myDBM.IsEmpty)
            {
                return;
            }

            if (IsEmpty)
            {
                dimention = myDBM.dimention;
                Matrix = myDBM.Matrix;
                MatrixStrictness = myDBM.MatrixStrictness;
                IsCanonicalForm = myDBM.IsCanonicalForm;

                ClockMaxValues = myDBM.ClockMaxValues;
                ClockLowerValues = myDBM.ClockLowerValues;
                ClockUpperValues = myDBM.ClockUpperValues;
            }
            else if (!myDBM.IsEmpty)
            {
                System.Diagnostics.Debug.Assert(dimention == myDBM.dimention);

                //assert the dimention of the two dbm are same
                for (int i = 0; i < dimention; i++)
                {
                    for (int j = 0; j < dimention; j++)
                    {
                        if (Matrix[i * dimention + j] > myDBM.Matrix[i * dimention + j])
                        {
                            Matrix[i * dimention + j] = myDBM.Matrix[i * dimention + j];
                            MatrixStrictness[i * dimention + j] = myDBM.MatrixStrictness[i * dimention + j];
                        }
                        else if (Matrix[i * dimention + j] == myDBM.Matrix[i * dimention + j])
                        {
                            MatrixStrictness[i * dimention + j] = MatrixStrictness[i * dimention + j] || myDBM.MatrixStrictness[i * dimention + j];
                        }
                    }
                }

                IsCanonicalForm = false;
            }
        }

        public bool Contains(FullDBM dbm)
        {
            Debug.Assert(Matrix[0] > 0 || (Matrix[0] == 0 && MatrixStrictness[0] == false));
            Debug.Assert(dbm.Matrix[0] > 0 || (dbm.Matrix[0] == 0 && dbm.MatrixStrictness[0] == false));
            Debug.Assert(IsCanonicalForm);
            Debug.Assert(dbm.IsCanonicalForm);
            Debug.Assert(dimention == dbm.dimention);

            if (dbm.IsEmpty)
            {
                return true;
            }

            for (int i = 0; i != dimention; ++i)
            {
                for (int j = 0; j != dimention; ++j)
                {
                    if (Matrix[Pos(i, j)] < dbm.Matrix[Pos(i, j)])
                    {
                        return false;
                    }
                    else if (Matrix[Pos(i, j)] == dbm.Matrix[Pos(i, j)])
                    {
                        if (MatrixStrictness[Pos(i, j)] == true && dbm.MatrixStrictness[Pos(i, j)] == false)
                        {
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        public void RelaxConstraints(Dictionary<short, HashSet<short>> TDep)
        {
            for (short row = 1; row < dimention; row++)
            {
                for(short col = 1; col < dimention; col++)
                {
                    if (row != col)
                    {
                        if (!(TDep.ContainsKey(row) && TDep[row].Contains(col)))
                        {
                            Matrix[row * dimention + col] = int.MaxValue;
                            MatrixStrictness[row * dimention + col] = false;
                        }
                    }
                }
            }
        }

        public String ToString(Dictionary<string, short> clockMapping)
        {
            if (IsEmpty)
            {
                return "";
            }

            StringBuilder sb = new StringBuilder();

            foreach (KeyValuePair<string, short> pair in clockMapping)
            {
                string clockName = pair.Key;
                int clockID = pair.Value;

                if (MatrixStrictness[clockID * dimention])
                {
                    if (Matrix[clockID * dimention] != int.MaxValue)
                    {
                        sb.AppendLine(clockName + "<" + Matrix[clockID * dimention].ToString());
                    }
                }
                else
                {
                    if (Matrix[clockID * dimention] != int.MaxValue)
                    {
                        sb.AppendLine(clockName + "<=" + Matrix[clockID * dimention].ToString());
                    }
                }

                if (MatrixStrictness[clockID])
                {
                    if (-1 * Matrix[clockID] != int.MaxValue)
                    {
                        sb.AppendLine(clockName + ">" + (-1 * Matrix[clockID]).ToString());
                    }
                }
                else
                {
                    if (-1 * Matrix[clockID] != int.MaxValue && Matrix[clockID] != 0)
                    {
                        sb.AppendLine(clockName + ">=" + (-1 * Matrix[clockID]).ToString());
                    }
                }

                foreach (KeyValuePair<string, short> pair2 in clockMapping)
                {
                    string clockName2 = pair2.Key;
                    int clockID2 = pair2.Value;

                    if (clockID != clockID2)
                    {
                        if (Matrix[clockID * dimention + clockID2] >= 0)
                        {
                            if (MatrixStrictness[clockID * dimention + clockID2])
                            {
                                if (Matrix[clockID * dimention + clockID2] != int.MaxValue)
                                {
                                    if (Matrix[clockID * dimention + clockID2] != 0)
                                    {
                                        sb.AppendLine(clockName + "-" + clockName2 + "<" + Matrix[clockID * dimention + clockID2].ToString());
                                    }
                                    else
                                    {
                                        sb.AppendLine(clockName + "<" + clockName2);
                                    }
                                }
                            }
                            else
                            {
                                if (Matrix[clockID * dimention + clockID2] != int.MaxValue)
                                {
                                    if (Matrix[clockID * dimention + clockID2] != 0)
                                    {
                                        sb.AppendLine(clockName + "-" + clockName2 + "<=" + Matrix[clockID * dimention + clockID2].ToString());
                                    }
                                    else
                                    {
                                        sb.AppendLine(clockName + "<=" + clockName2);
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (MatrixStrictness[clockID * dimention + clockID2])
                            {
                                if (-1 * Matrix[clockID * dimention + clockID2] != int.MaxValue)
                                {
                                    if (Matrix[clockID * dimention + clockID2] != 0)
                                    {
                                        sb.AppendLine(clockName2 + "-" + clockName + ">" + (-1 * Matrix[clockID * dimention + clockID2]).ToString());
                                    }
                                    else
                                    {
                                        sb.AppendLine(clockName2 + ">" + clockName);
                                    }
                                }
                            }
                            else
                            {
                                if (-1 * Matrix[clockID * dimention + clockID2] != int.MaxValue)
                                {
                                    if (Matrix[clockID * dimention + clockID2] != 0)
                                    {
                                        sb.AppendLine(clockName2 + "-" + clockName + ">=" + (-1 * Matrix[clockID * dimention + clockID2]).ToString());
                                    }
                                    else
                                    {
                                        sb.AppendLine(clockName2 + ">=" + clockName);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return sb.ToString();
        }

        public override String ToString()
        {
            return GetID();
        }

        public FullDBM Clone()
        {
            if (IsEmpty)
            {
                return this;
            }

            return new FullDBM(dimention, (int[])Matrix.Clone(), (bool[])MatrixStrictness.Clone(), IsCanonicalForm, ClockMaxValues, ClockLowerValues, ClockUpperValues);
        }

        public string GetID()
        {
            if (IsEmpty)
            {
                return "";
            }

            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < dimention; i++)
            {
                for (int j = 0; j < dimention; j++)
                {
                    if (i != j)
                    {
                        sb.Append((MatrixStrictness[i * dimention + j] ? "" : "=") + (Matrix[i * dimention + j] == int.MaxValue ? "INF" : Matrix[i * dimention + j].ToString()) + Constants.SEPARATOR);
                    }
                }
            }

            return sb.ToString();
        }
    }
}
