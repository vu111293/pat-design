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
    public class NewDBM
    {
        /// <summary>
        /// Use our own int.MinValue to make sure that -int.MinValue = int.MaxValue
        /// </summary>
        public const int MINVALUE = int.MinValue + 1;

        public const bool LessThan = true;
        public const bool LessThanEqual = false;

        public static int[] ClockMaxValues;
        public static int[] ClockLowerValues;
        public static int[] ClockUpperValues;

        /// <summary>
        /// LocalMax[clockID][stateID]. Dont call this property directly
        /// Use below function
        /// </summary>
        public static Dictionary<string, int>[] LocalMax;

        /// <summary>
        /// LocalLocal[clockID][stateID]. Dont call this property directly
        /// Use below function
        /// </summary>
        public static Dictionary<string, int>[] LocalLower;

        /// <summary>
        /// LocalUpper[clockID][stateID]. Dont call this property directly
        /// Use below function
        /// </summary>
        public static Dictionary<string, int>[] LocalUpper;

        public static Dictionary<int, int> clockToProcessIndex; 

        public static int dim;


        /// <summary>
        /// Store the matrix of the DBM
        /// </summary>
        public int[] matrix;

        public bool[] isLessThans;

        public NewDBM()
        {
            matrix = new int[dim*dim];
            isLessThans = new bool[dim * dim];

            for(int i = 0; i < dim; i++)
            {
                for(int j = 0; j < dim; j++)
                {
                    int index = i*dim + j;

                    if(i == 0 || i == j)
                    {
                        SetValue(index, 0, LessThanEqual);
                    }
                    else
                    {
                        SetValue(index, int.MaxValue, LessThan);
                    }
                }
            }
        }

        public NewDBM(int[] matrix, bool[]isLessThans)
        {
            this.matrix = matrix;
            this.isLessThans = isLessThans;
        }

        /// <summary>
        /// Return the DBM where all clocks are zero
        /// </summary>
        /// <returns></returns>
        public static NewDBM ZeroDBM()
        {
            //as default, matrix[i] = 0, isLessThans[i] = false = LessThanEqual
            int[] matrix = new int[dim * dim];
            bool[] isLessThans = new bool[dim*dim];

            return new NewDBM(matrix, isLessThans);
        }

        public static NewDBM DBMFromValuation(int[] valuation)
        {
            int[] matrix = new int[dim * dim];
            bool[] isLessThans = new bool[dim * dim];

            for (int i = 0; i < dim; i++ )
            {
                for(int j = 0; j < dim; j++)
                {
                    if(i != j)
                    {
                        matrix[i*dim + j] = valuation[i] - valuation[j];
                    }
                }
            }

            return new NewDBM(matrix, isLessThans);
        }

        private void SetValue(int index, int value, bool isLessThan)
        {
            this.matrix[index] = value;
            this.isLessThans[index] = isLessThan;
        }

        public static int GetMax(int clockID, bool isLocalBased, string[] currentLocation)
        {
            if(!isLocalBased)
            {
                return ClockMaxValues[clockID];
            }
            else
            {
                if (clockID == 0) return 0;

                int processIndexHavingClock = clockToProcessIndex[clockID];

                return LocalMax[clockID][currentLocation[processIndexHavingClock]];
            }
        }

        public static int GetMax(int clockID, bool isLocalBased, string stateID)
        {
            if (!isLocalBased)
            {
                return ClockMaxValues[clockID];
            }
            else
            {
                if (clockID == 0) return 0;

                return LocalMax[clockID][stateID];
            }
        }

        public static int GetLower(int clockID, bool isLocalBased, string[] currentLocation)
        {
            if (!isLocalBased)
            {
                return ClockLowerValues[clockID];
            }
            else
            {
                if (clockID == 0) return 0;

                int processIndexHavingClock = clockToProcessIndex[clockID];

                return LocalLower[clockID][currentLocation[processIndexHavingClock]];
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="clockID"></param>
        /// <param name="isLocalBased"></param>
        /// <param name="stateID">StateID</param>
        /// <returns></returns>
        public static int GetLower(int clockID, bool isLocalBased, string stateID)
        {
            if (!isLocalBased)
            {
                return ClockLowerValues[clockID];
            }
            else
            {
                if (clockID == 0) return 0;

                return LocalLower[clockID][stateID];
            }
        }

        public static int GetUpper(int clockID, bool isLocalBased, string[] currentLocation)
        {
            if (!isLocalBased)
            {
                return ClockUpperValues[clockID];
            }
            else
            {
                if (clockID == 0) return 0;

                int processIndexHavingClock = clockToProcessIndex[clockID];

                return LocalUpper[clockID][currentLocation[processIndexHavingClock]];
            }
        }

        public static int GetUpper(int clockID, bool isLocalBased, string stateID)
        {
            if (!isLocalBased)
            {
                return ClockUpperValues[clockID];
            }
            else
            {
                if (clockID == 0) return 0;


                return LocalUpper[clockID][stateID];
            }
        }

        

        /// <summary>
        /// TODO TKN: Do we need this function
        /// </summary>
        /// <returns></returns>
        public bool HasOneClock()
        {
            return (dim == 1);
        }

        public int GetTimerUpper(short timerID)
        {
            return matrix[timerID*dim];
        }

        public void Reset(short x)
        {
            for (int i = 0; i < dim; i++)
            {
                Plus(x*dim + i, 0, LessThanEqual, matrix[i], isLessThans[i]);
                Plus(i*dim + x, matrix[i*dim], isLessThans[i*dim], 0, LessThanEqual);
            }
        }

        public void GlobalExtraPlusM()
        {
            ExtraPlusM(false, null);
        }

        public void LocalExtraPlusM(string[] currentStates)
        {
            ExtraPlusM(true, currentStates);
        }

        /// <summary>
        /// Extra+M
        /// </summary>
        /// <param name="currentStates"></param>
        private void ExtraPlusM(bool isLocalbased, string[] currentStates)
        {
            if (matrix[0] >= 0)
            {
                //calculate local max for each clock in advance
                int[] maxClocks = new int[dim];
                for (int i = 0; i < dim; i++)
                {
                    maxClocks[i] = GetMax(i, isLocalbased, currentStates);
                }

                for (int i = 0; i < dim; i++)
                {
                    for (int j = 0; j < dim; j++)
                    {
                        //Only update matrix[i,j] where i!=j
                        if (j != i)
                        {
                            int index = i*dim + j;

                            //Remove constraint x_i - x_j < c_ij where c_ij > M(x_i)
                            if (matrix[index] > maxClocks[i])
                            {
                                SetElementIgnored(index);
                            }

                            //Remoev any constraint x_i - x_j < c_ij if -c_0i > M(x_i)
                            if (-matrix[i] > maxClocks[i])
                            {
                                SetElementIgnored(index);
                            }

                            //if -c_0j > M(x_j), remove any constraint x_i - x_j < c_ij and update c_0j to Max(0, -ClockUpperValues[j])
                            if (-matrix[j] > maxClocks[j])
                            {
                                if (i != 0)
                                {
                                    SetElementIgnored(index);
                                }
                                else
                                {
                                    if (maxClocks[j] < 0)
                                    {
                                        SetValue(index, 0, LessThanEqual);
                                    }
                                    else
                                    {
                                        SetValue(index, -GetMax(j, isLocalbased, currentStates), LessThan);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public void GlobalExtraPlusLU()
        {
            ExtraPlusLU(false, null);
        }

        public void LocalExtraPlusLU(string[] currentStates)
        {
            ExtraPlusLU(true, currentStates);
        }

        /// <summary>
        /// Extra+LU
        /// </summary>
        private void ExtraPlusLU(bool isLocalbased, string[] currentStates)
        {
            if (matrix[0] >= 0)
            {
                //calculate local max for each clock in advance
                int[] lowerClocks = new int[dim];
                int[] upperClocks = new int[dim];

                for (int i = 0; i < dim; i++)
                {
                    lowerClocks[i] = GetLower(i, isLocalbased, currentStates);
                    upperClocks[i] = GetUpper(i, isLocalbased, currentStates);
                }

                for (int i = 0; i < dim; i++)
                {
                    for (int j = 0; j < dim; j++)
                    {
                        //Only update matrix[i,j] where i!=j
                        if (i != j)
                        {
                            int index = i * dim + j;

                            //Remove constraint x_i - x_j < c_ij where c_ij > L(x_i)
                            if (matrix[index] > lowerClocks[i])
                            {
                                SetElementIgnored(index);
                            }

                            //Remove any constraint x_i - x_j < c_ij if -c_0i > L(x_i)
                            if (-matrix[i] > lowerClocks[i])
                            {
                                SetElementIgnored(index);
                            }

                            //if -c_0j > U(x_j), remove any constraint x_i - x_j < c_ij and update c_0j to Max(0, -ClockUpperValues[j])
                            if (-matrix[j] > upperClocks[j])
                            {
                                if (i != 0)
                                {
                                    SetElementIgnored(index);
                                }
                                else
                                {
                                    if (upperClocks[j] < 0)
                                    {
                                        SetValue(index, 0, LessThanEqual);
                                    }
                                    else
                                    {
                                        SetValue(index, -upperClocks[j], LessThan);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private bool HasNegativeCycle(int clock)
        {
            return (matrix[clock * dim + clock] < 0 || isLessThans[0]);
        }

        /// <summary>
        /// Add constraint of only 1 clock
        /// </summary>
        /// <param name="timer"></param>
        /// <param name="op"></param>
        /// <param name="constant"></param>
        public void AddConstraint(short timer, TimerOperationType op, int constant)
        {
            switch (op)
            {
                case TimerOperationType.Equals:
                    AddConstraint(timer, 0, constant, LessThanEqual);
                    AddConstraint(0, timer, -constant, LessThanEqual);
                    break;
                case TimerOperationType.GreaterThanOrEqualTo:
                    AddConstraint(0, timer, -constant, LessThanEqual);
                    break;
                case TimerOperationType.LessThanOrEqualTo:
                    AddConstraint(timer, 0, constant, LessThanEqual);
                    break;
                case TimerOperationType.GreaterThan:
                    AddConstraint(0, timer, -constant, LessThan);
                    break;
                case TimerOperationType.LessThan:
                    AddConstraint(timer, 0, constant, LessThan);
                    break;
            }
        }

        /// <summary>
        /// constraint of the form: (x - y less c) or (x - y leq c)
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="bound"></param>
        public void AddConstraint(short x, short y, int value, bool isLessThan)
        {
            if (IsSumLessThan(matrix[y * dim + x], isLessThans[y * dim + x], value, isLessThan, 0, LessThan))
            {
                SetDBMEmpty();
            }
            else if(IsLessThan(value, isLessThan, matrix[x*dim+y], isLessThans[x*dim+y]))
            {
                SetValue(x*dim + y, value, isLessThan);

                for(int i = 0; i < dim;i++)
                {
                    for(int j = 0; j < dim; j++)
                    {
                        PlusMin(i * dim + j, i * dim + x, x * dim + j);

                        PlusMin(i * dim + j, i * dim + y, y * dim + j);

                        if(i == j && HasNegativeCycle(i))
                        {
                            SetDBMEmpty();
                            return;
                        }
                    }
                }
            }
        }

        public void Up()
        {
            for (int i = 1; i < dim; i++)
            {
                SetElementIgnored(i*dim);
            }
        }

        /// <summary>
        /// Check Empty Zone
        /// </summary>
        /// <returns></returns>
        public bool IsEmpty()
        {
            return HasNegativeCycle(0);
        }


        /// <summary>
        /// Check dbm1 is a subset of dbm2;
        /// </summary>
        /// <param name="dbm1"></param>
        /// <param name="dbm2"></param>
        /// <returns></returns>
       public static bool IsSubSet(NewDBM dbm1, NewDBM dbm2)
        {
            int dim = NewDBM.dim;

           for(int i = 0; i < dim; i++)
           {
               for(int j = 0; j < dim; j++)
               {
                   int index = i*dim + j;
                   if (i != j && !IsLessEqual(dbm1.matrix[index], dbm1.isLessThans[index], dbm2.matrix[index], dbm2.isLessThans[index]))
                   {
                       return false;
                   }
               }
           }
           return true;
       }

        public String ToString(Dictionary<string, short> clockMapping)
        {
            StringBuilder sb = new StringBuilder();

            foreach (KeyValuePair<string, short> pair in clockMapping)
            {
                string clockName = pair.Key;
                int clockID = pair.Value;

                if (!IsElementIgnored(clockID * dim))
                {
                    sb.AppendLine(clockName + matrix[clockID*dim]);
                }

                if (!IsElementIgnored(clockID))
                {
                    sb.AppendLine(clockName + NegationElementToString(clockID));
                }

                foreach (KeyValuePair<string, short> pair2 in clockMapping)
                {
                    string clockName2 = pair2.Key;
                    int clockID2 = pair2.Value;

                    if (clockID != clockID2)
                    {
                        int index = clockID*dim + clockID2;
                        if (!IsElementIgnored(clockID*dim + clockID2))
                        {
                            if (matrix[index] > 0)
                            {
                                sb.AppendLine(clockName + "-" + clockName2 + ElementToString(index));
                            }
                            else if (matrix[index] == 0)
                            {
                                sb.AppendLine(clockName + GetOpt(index) + clockName2);
                            }
                            else
                            {
                                sb.AppendLine(clockName2 + "-" + clockName + NegationElementToString(index));
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

        public NewDBM Clone()
        {

            int[] matrixClone = new int[dim * dim];
            bool[] isLessThansClone = new bool[dim*dim];

            for (int i = 0; i < dim * dim; i++ )
            {
                matrixClone[i] = matrix[i];
                isLessThansClone[i] = isLessThans[i];
            }

            return new NewDBM(matrixClone, isLessThansClone);
        }

        public string GetID()
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < dim; i++)
            {
                for (int j = 0; j < dim; j++)
                {
                    if (i != j)
                    {
                        int index = i*dim + j;
                        sb.Append((isLessThans[index] ? "" : "=") + (IsElementIgnored(index) ? "INF" : matrix[index].ToString()) + Constants.SEPARATOR);
                    }
                }
            }

            return sb.ToString();
        }

        public static bool IsGloballySimSubset(NewDBM z1, NewDBM z2)
        {
            return IsSimSubset(z1, z2, false, null);
        }

        public static bool IsLocallySimSubset(NewDBM z1, NewDBM z2, string[] currentStates)
        {
            return IsSimSubset(z1, z2, true, currentStates);
        }

        /// <summary>
        /// return z1 subset a_LU(z2)
        /// iff there exist x, y such that
        /// z1_0x ≥ (≤, -U_x) && z2_yx ≺ z1_yx && z2_yx + (≺, -L_y) ≺ z1_0x
        /// No need to check the simple case where dbm is empty since these dbm are created from TAConfiguratino.MakeOneMove
        /// </summary>
        /// <param name="z1"></param>
        /// <param name="z2"></param>
        /// <param name="currentState"></param>
        /// <returns></returns>
        private static bool IsSimSubset(NewDBM z1, NewDBM z2, bool isLocalbased, string[] currentState)
        {
            //
            int dim = NewDBM.dim;

            for (int x = 1; x < dim; x++)
            {
                int minusUpperX = -GetUpper(x, isLocalbased, currentState);

                if(IsLessEqual(minusUpperX, LessThanEqual, z1.matrix[x], z1.isLessThans[x]))
                {
                    for (int y = 1; y < dim; y++)
                    {
                        if (x != y)
                        {
                            int index = y*dim + x;
                            //TODO TKN: 2 variables must different and not zero-clock
                            if (IsLessThan(z2.matrix[index], z2.isLessThans[index], z1.matrix[index], z1.isLessThans[index]) &&
                                IsSumLessThan(z2.matrix[index], z2.isLessThans[index], -GetLower(y, isLocalbased, currentState), LessThan, z1.matrix[x], z1.isLessThans[x]))
                            {
                                return false;
                            }
                        }
                    }
                }
            }

            return true;
        }

        public void RelaxConstraints(Dictionary<short, HashSet<short>> TDep)
        {
            for (short row = 1; row < dim; row++)
            {
                for (short col = 1; col < dim; col++)
                {
                    if (row != col)
                    {
                        if (!(TDep.ContainsKey(row) && TDep[row].Contains(col)))
                        {
                            SetElementIgnored(row * dim + col);
                        }
                    }
                }
            }
        }


        /// <summary>
        /// Return the minimum solution
        /// </summary>
        /// <returns></returns>
        public int[] GetMinValuation()
        {
            int[] result = new int[dim];

            for (short i = 1; i < dim; i++)
            {
                result[i] = (isLessThans[i]) ? -matrix[i] + 1 : -matrix[i];
                AddConstraint(i, TimerOperationType.Equals, result[i]);
            }

            return result;
        }

        #region Bound

        /// <summary>
        /// Check bound1 lessthan bound2
        /// </summary>
        /// <param name="index1"></param>
        /// <param name="index2"></param>
        /// <returns></returns>
        private static bool IsLessThan(int value1, bool isLessThan1, int value2, bool isLessThan2)
        {
            return ((value1 < value2) || (value1 == value2 && isLessThan1 && !isLessThan2));

        }

        /// <summary>
        /// Check bound1 == bound 2
        /// </summary>
        /// <param name="index1"></param>
        /// <param name="index2"></param>
        /// <returns></returns>
        private static bool IsLessEqual(int value1, bool isLessThan1, int value2, bool isLessThan2)
        {
            return ((value1 < value2) || (value1 == value2 && !(!isLessThan1 && isLessThan2)));
        }

        /// <summary>
        /// (bound1 + bound 2) lessthan bound
        /// </summary>
        /// <param name="value1"></param>
        /// <param name="isLessThan1"></param>
        /// <param name="value2"></param>
        /// <param name="isLessThan2"></param>
        /// <param name="value"></param>
        /// <param name="isLessThan"></param>
        /// <returns></returns>
        private static bool IsSumLessThan(int value1, bool isLessThan1, int value2, bool isLessThan2, int value, bool isLessThan)
        {
            if (value1 == int.MaxValue || value2 == int.MaxValue)
            {
                return false;
            }
            else if (!isLessThan1 && !isLessThan2)
            {
                return IsLessThan(value1 + value2, LessThanEqual, value, isLessThan);
            }
            else
            {
                return IsLessThan(value1 + value2, LessThan, value, isLessThan);
            }
        }

        /// <summary>
        /// Update the index.th element as the sum of bound 1 and bound 2
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value1"></param>
        /// <param name="isLessThan1"></param>
        /// <param name="value2"></param>
        /// <param name="isLessThan2"></param>
        private void Plus(int index, int value1, bool isLessThan1, int value2, bool isLessThan2)
        {
            if (value1 == int.MaxValue || value2 == int.MaxValue)
            {
                SetElementIgnored(index);
            }
            else if (!isLessThan1 && !isLessThan2)
            {
                SetValue(index, value1 + value2, LessThanEqual);
            }
            else
            {
                SetValue(index, value1 + value2, LessThan);
            }
        }

        /// <summary>
        /// update index = min(index1+index2)
        /// </summary>
        /// <param name="index"></param>
        /// <param name="index1"></param>
        /// <param name="index2"></param>
        private void PlusMin(int index, int index1, int index2)
        {
            if (matrix[index1] != int.MaxValue && matrix[index2] != int.MaxValue)
            {
                if (matrix[index1] + matrix[index2] < matrix[index])
                {
                    if (!isLessThans[index1] && !isLessThans[index2])
                    {
                        SetValue(index, matrix[index1] + matrix[index2], LessThanEqual);
                    }
                    else
                    {
                        SetValue(index, matrix[index1] + matrix[index2], LessThan);
                    }
                }
                else if (matrix[index1] + matrix[index2] == matrix[index] && (isLessThans[index1] || isLessThans[index2]) && !isLessThans[index])
                {
                    SetValue(index, matrix[index], LessThan);
                }
            }
        }

        private void SetElementIgnored(int index)
        {
            matrix[index] = int.MaxValue;
            isLessThans[index] = LessThan;
        }

        private bool IsElementIgnored(int index)
        {
            return (matrix[index] == int.MaxValue);
        }

        private void SetDBMEmpty()
        {
            matrix[0] = -1;
            isLessThans[0] = LessThan;
        }

        /// <summary>
        /// Return lessthan/lessequal value
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public string ElementToString(int index)
        {
            return (isLessThans[index]) ? "< " + matrix[index] : "≤ " + matrix[index];
        }

        /// <summary>
        /// return greaterthan/greaterequal -value
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public string NegationElementToString(int index)
        {
            return (isLessThans[index]) ? "> " + (-matrix[index]) : "≥ " + (-matrix[index]);
        }

        public string GetOpt(int index)
        {
            return (isLessThans[index]) ? "<" : "≤";
        }
        #endregion Bound
    }
}
