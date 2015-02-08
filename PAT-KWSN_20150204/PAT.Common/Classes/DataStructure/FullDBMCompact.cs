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
    public sealed class FullDBMCompact
    {
        public static int dbm_INFINITY = int.MaxValue >> 1; /**< infinity                           */
        public static int dbm_OVERFLOW = int.MaxValue >> 2;  /**< to detect overflow on computations */

        public const int dbm_LE_ZERO = 1;                       /**< Less Equal Zero                    */
        public static int dbm_LS_INFINITY = (dbm_INFINITY << 1); /**< Less Strict than infinity          */
        public static int dbm_LE_OVERFLOW = dbm_LS_INFINITY >> 1; /**< to detect overflow on computations */


        public const int dbm_STRICT = 0; /**< strict less than constraints:  < x */
        public const int dbm_WEAK = 1;    /**< less or equal constraints   : <= x */


        public static int[] ClockMaxValues;
        public static int[] ClockLowerValues;
        public static int[] ClockUpperValues;

        public bool IsEmpty
        {
            get
            {
                return dimention == 1;
            }
        }

        //matrix to store the 
        public int[] Matrix;
        //public bool[] MatrixStrictness;

        //if FullDBM is empty, it is in connoicalform
        public bool IsCanonicalForm = true;
        public int dimention;

        public FullDBMCompact(int dim)
        {
            dimention = dim;
            Matrix = new int[dimention * dimention];
        }

        public FullDBMCompact(int dim, int[] matrix, bool isCanonical)
        {
            dimention = dim;
            Matrix = matrix;
            //MatrixStrictness = matrixStrictness;
            IsCanonicalForm = isCanonical;
        }


        public void ResetTimer(byte timerid)
        {

            Debug.Assert(IsCanonicalForm);

            Matrix[timerid * dimention + 0] = 0;
            Matrix[0 * dimention + timerid] = 0;
            //MatrixStrictness[timerid * dimention + 0] = false;
            //MatrixStrictness[0 * dimention + timerid] = false;

            for (int i = 1; i < dimention; i++)
            {
                Matrix[i * dimention + timerid] = Matrix[i * dimention + 0];
                Matrix[timerid * dimention + i] = Matrix[0 * dimention + i];

                //MatrixStrictness[i * dimention + timerid] = MatrixStrictness[i * dimention + 0];
                //MatrixStrictness[timerid * dimention + i] = MatrixStrictness[0 * dimention + i];
            }
        }


        private void GetCanonicalForm()
        {
            //int dimention = Matrix.Count;
            for (int k = 0; k < dimention; k++)
            {
                for (int i = 0; i < dimention; i++)
                {
                    if (i != k)
                    {
                        for (int j = 0; j < dimention; j++)
                        {
                            //check for the overflow problem
                            if (Matrix[i * dimention + k] != dbm_LS_INFINITY && Matrix[k * dimention + j] != dbm_LS_INFINITY)
                            {
                                //Attension, Matrix[i* dimention + k] + Matrix[k* dimention + j] is bigger than int.MaxValue, there is a possbility of overflow.
                                //Matrix[i* dimention + j] = Math.Min(Matrix[i* dimention + j], Matrix[i* dimention + k] + Matrix[k* dimention + j]);

                                int dbm_ikkj = dbm_addFiniteFinite(Matrix[i * dimention + k], Matrix[k * dimention + j]);

                                if (Matrix[i * dimention + j] > dbm_ikkj)
                                {
                                    Matrix[i * dimention + j] = dbm_ikkj;
                                }

                                //if (Matrix[i * dimention + j] > Matrix[i * dimention + k] + Matrix[k * dimention + j])
                                //{
                                //    Matrix[i * dimention + j] = Matrix[i * dimention + k] + Matrix[k * dimention + j];
                                //    MatrixStrictness[i * dimention + j] = MatrixStrictness[i * dimention + k] || MatrixStrictness[k * dimention + j];
                                //}
                                //else if (Matrix[i * dimention + j] == Matrix[i * dimention + k] + Matrix[k * dimention + j])
                                //{
                                //    MatrixStrictness[i * dimention + j] = MatrixStrictness[i * dimention + j] || (MatrixStrictness[i * dimention + k] || MatrixStrictness[k * dimention + j]);
                                //}
                            }
                        }

                        if (Matrix[i * dimention + i] < dbm_LE_ZERO)
                        {
                            IsCanonicalForm = true;
                            Matrix[0 * dimention + 0] = -1;
                            return;
                        }
                    }
                }
            }

            if (!SpecificationBase.IsSimulation && Matrix[0 * dimention + 0] >= 0)
            {
                for (int i = 0; i < dimention; i++)
                {
                    int boundi = DBMConstraint.dbm_raw2bound(Matrix[0 * dimention + i]);
                    for (int j = 0; j < dimention; j++)
                    {
                        int boundij = DBMConstraint.dbm_raw2bound(Matrix[i * dimention + j]);
                        int boundj = DBMConstraint.dbm_raw2bound(Matrix[0 * dimention + j]);
                        //=======================classic extrapolation======================
                        if (i > 0 && ClockMaxValues[i] > 0 && boundij > ClockMaxValues[i])
                        {
                            //Debug.Assert(Matrix[i* dimention + j] <= ClockMaxValues[i - 1]);

                            Matrix[i * dimention + j] = dbm_LS_INFINITY; // int.MaxValue;                            
                        }

                        if (j > 0 && ClockMaxValues[j] > 0 && -1 * boundij > ClockMaxValues[j])
                        {
                            //Debug.Assert(-1*Matrix[i* dimention + j] <= ClockMaxValues[j - 1]);

                            Matrix[i * dimention + j] = DBMConstraint.dbm_bound2raw(-1 * ClockMaxValues[j], dbm_STRICT);
                            //MatrixStrictness[i * dimention + j] = true;
                        }
                        //=======================classic extrapolation======================


                        ////=======================diaganol extrapolation======================
                        if (i > 0 && ClockMaxValues[i] > 0 && -1 * boundi > ClockMaxValues[i])
                        {
                            //Debug.Assert(-1 * Matrix[0* dimention + i] <= ClockMaxValues[i - 1]);
                            Matrix[i * dimention + j] = dbm_LS_INFINITY;
                        }

                        if (j > 0 && ClockMaxValues[j] > 0 && i != 0 && -1 * boundj > ClockMaxValues[j])
                        {
                            //Debug.Assert(-1 * Matrix[0* dimention + j] <= ClockMaxValues[j - 1]);
                            Matrix[i * dimention + j] = dbm_LS_INFINITY;
                        }

                        if (j > 0 && ClockMaxValues[j] > 0 && i == 0 && -1 * boundi > ClockMaxValues[j])
                        {
                            //Debug.Assert(-1 * Matrix[i* dimention + j] <= ClockMaxValues[j - 1]);
                            Matrix[i * dimention + j] = DBMConstraint.dbm_bound2raw(-1 * ClockMaxValues[i], dbm_STRICT);
                            //MatrixStrictness[i * dimention + j] = true;
                        }
                        ////=======================diaganol extrapolation======================


                        ////=======================L/U extrapolation======================
                        if (i > 0 && ClockLowerValues[i] > 0 && boundij > ClockLowerValues[i])
                        {
                            //Debug.Assert(Matrix[i* dimention + j] <= ClockLowerValues[i - 1]);
                            Matrix[i * dimention + j] = dbm_LS_INFINITY;
                        }


                        if (j > 0 && ClockUpperValues[j] > 0 && -1 * Matrix[i * dimention + j] > ClockUpperValues[j])
                        {
                            //Debug.Assert(-1*Matrix[i* dimention + j] <= ClockUpperValues[j - 1]);
                            Matrix[i * dimention + j] = DBMConstraint.dbm_bound2raw(-1 * ClockUpperValues[j], dbm_STRICT);
                            //MatrixStrictness[i * dimention + j] = true;
                        }
                        ////=======================L/U extrapolation======================


                        ////=======================diaganol L/U extrapolation======================
                        if (i > 0 && ClockLowerValues[i] > 0 && -1 * Matrix[0 * dimention + i] > ClockLowerValues[i])
                        {
                            //Debug.Assert(-1 * Matrix[0* dimention + i] <= ClockLowerValues[i - 1]);
                            Matrix[i * dimention + j] = dbm_LS_INFINITY;
                        }


                        if (j > 0 && ClockUpperValues[j] > 0 && i != 0 && -1 * boundj > ClockUpperValues[j])
                        {
                            //Debug.Assert(-1 * Matrix[0* dimention + j] <= ClockUpperValues[j - 1]);
                            Matrix[i * dimention + j] = dbm_LS_INFINITY;
                        }

                        if (j > 0 && ClockUpperValues[j] > 0 && i == 0 && -1 * boundi > ClockUpperValues[j])
                        {
                            //Debug.Assert(-1 * Matrix[i* dimention + j] <= ClockUpperValues[j - 1]);
                            Matrix[i * dimention + j] = DBMConstraint.dbm_bound2raw(-1 * ClockUpperValues[j], dbm_STRICT);
                            //MatrixStrictness[i * dimention + j] = true;
                        }

                        ////=======================diaganol L/U extrapolation======================
                    }
                }
            }

            IsCanonicalForm = true;
        }



        /// <summary>
        /// Update the FullDBM with a new constraint
        /// </summary>
        /// <param name="timerID">which timer the constraint is on</param>
        /// <param name="op">0 for equal; 1 for >=; -1 for <=</param>
        /// <param name="constant"></param>
        public void AddConstraint(byte timer, TimerOperationType op, int constant)
        {

            Debug.Assert(timer > 0);
            int boundt0 = DBMConstraint.dbm_raw2bound(Matrix[timer * dimention + 0]);
            int bound0t = DBMConstraint.dbm_raw2bound(Matrix[0 * dimention + timer]);

            switch (op)
            {
                case TimerOperationType.Equals:
                    if (boundt0 > constant)
                    {
                        Matrix[timer * dimention + 0] = DBMConstraint.dbm_bound2raw(constant, dbm_WEAK);
                    }

                    if (bound0t > -1 * constant)
                    {
                        Matrix[0 * dimention + timer] = DBMConstraint.dbm_bound2raw(-1 * constant, dbm_WEAK);
                    }

                    break;
                case TimerOperationType.GreaterThanOrEqualTo:
                    if (bound0t > -1 * constant)
                    {
                        Matrix[0 * dimention + timer] = DBMConstraint.dbm_bound2raw(-1 * constant, dbm_WEAK);
                    }
                    break;
                case TimerOperationType.LessThanOrEqualTo:
                    if (boundt0 > constant)
                    {
                        Matrix[timer * dimention + 0] = DBMConstraint.dbm_bound2raw(constant, dbm_WEAK);
                    }
                    break;
                case TimerOperationType.GreaterThan:
                    if (bound0t > -1 * constant)
                    {
                        Matrix[0 * dimention + timer] = DBMConstraint.dbm_bound2raw(-1 * constant, dbm_STRICT);
                        //MatrixStrictness[0 * dimention + timer] = true;
                    }
                    //ClockLowerValues[timer - 1] = Math.Max(constant, ClockLowerValues[timer - 1]);
                    break;
                case TimerOperationType.LessThan:
                    if (boundt0 > constant)
                    {
                        Matrix[timer * dimention + 0] = DBMConstraint.dbm_bound2raw(constant, dbm_STRICT);
                    }
                    break;
            }

            IsCanonicalForm = false;
        }


        public void Delay()
        {
            Debug.Assert(IsCanonicalForm); /* chengbin */

            for (int i = 0; i < dimention; i++)
            {

                if (i != 0)
                {
                    Matrix[i * dimention + 0] = dbm_LS_INFINITY; // int.MaxValue;
                    //MatrixStrictness[i * dimention + 0] = false;
                }
                else
                {
                    Matrix[i * dimention + 0] = 0;
                    //MatrixStrictness[i * dimention + 0] = false;
                }
            }
        }

        public FullDBMCompact KeepTimers(List<byte> activeTimer)
        {
            Debug.Assert(this.IsCanonicalForm); /* chengbin */

            int[] newMatrix = new int[Matrix.Length];
            //bool[] newMatrixStrictness = new bool[Matrix.Length];


            foreach (int i in activeTimer)
            {
                foreach (int j in activeTimer)
                {
                    newMatrix[i * dimention + j] = Matrix[i * dimention + j];
                    //newMatrixStrictness[i * dimention + j] = MatrixStrictness[i * dimention + j];
                }
            }

            return new FullDBMCompact(dimention, newMatrix, true);
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

            return Matrix[0 * dimention + 0] < dbm_LE_ZERO; // < 0 || MatrixStrictness[0* dimention + 0];
        }



        public void Conjunction(FullDBM myDBM)
        {
            if (myDBM.IsEmpty)
            {
                return;
            }

            if (IsEmpty)
            {
                Matrix = myDBM.Matrix;
                //MatrixStrictness = myDBM.MatrixStrictness;
                IsCanonicalForm = myDBM.IsCanonicalForm;
            }
            else if (!myDBM.IsEmpty)
            {
                //assert the dimention of the two dbm are same
                //int dimention = Matrix.Count;
                for (int i = 0; i < dimention; i++)
                {
                    //int myith = i + 1;
                    //int ith = TimerArray.IndexOf(myDBM.TimerArray[i]) + 1;

                    for (int j = 0; j < dimention; j++)
                    {
                        //int myjth = j + 1;
                        //int jth = TimerArray.IndexOf(myDBM.TimerArray[j]) + 1;

                        if (Matrix[i * dimention + j] > myDBM.Matrix[i * dimention + j])
                        {
                            Matrix[i * dimention + j] = myDBM.Matrix[i * dimention + j];
                            //MatrixStrictness[i* dimention + j] = myDBM.MatrixStrictness[i* dimention + j];
                        }
                        //else if (Matrix[i* dimention + j] == myDBM.Matrix[i* dimention + j])
                        //{
                        //    MatrixStrictness[i* dimention + j] = MatrixStrictness[i* dimention + j] || myDBM.MatrixStrictness[i* dimention + j];
                        //}
                    }
                }

                IsCanonicalForm = false;
            }
        }

        public String ToString(Dictionary<string, byte> clockMapping)
        {
            if (IsEmpty)
            {
                return "";
            }

            StringBuilder sb = new StringBuilder();

            foreach (KeyValuePair<string, byte> pair in clockMapping)
            {
                sb.AppendLine(pair.Key + (DBMConstraint.dbm_rawIsStrict(Matrix[0 * dimention + pair.Value]) ? ":(" : ":[") +
                              (Matrix[0 * dimention + pair.Value] == int.MaxValue ? "-" + Constants.INFINITE : (DBMConstraint.dbm_raw2bound(Matrix[0 * dimention + pair.Value] * -1)).ToString()) +
                              "," + (Matrix[pair.Value * dimention + 0] == int.MaxValue ? Constants.INFINITE : DBMConstraint.dbm_raw2bound(Matrix[pair.Value * dimention + 0]).ToString()) +
                              (DBMConstraint.dbm_rawIsStrict(Matrix[pair.Value * dimention + 0]) ? ");" : "];"));
            }


            return sb.ToString();
        }

        public override String ToString()
        {
            if (IsEmpty)
            {
                return "";
            }

            StringBuilder sb = new StringBuilder();

            for (int i = 1; i < dimention; i++)
            {
                sb.AppendLine("clock" + i + (DBMConstraint.dbm_rawIsStrict(Matrix[0 * dimention + i]) ? ":(" : ":[") + (DBMConstraint.dbm_raw2bound(Matrix[0 * dimention + i]) == int.MaxValue ? "-" + Constants.INFINITE : (Matrix[0 * dimention + i] * -1).ToString()) + "," + (Matrix[i * dimention + 0] == int.MaxValue ? Constants.INFINITE : Matrix[i * dimention + 0].ToString()) + (DBMConstraint.dbm_rawIsStrict(Matrix[i * dimention + 0]) ? ");" : "];"));
            }

            return sb.ToString();
        }

        public FullDBMCompact Clone()
        {
            if (IsEmpty)
            {
                return this;
            }

            return new FullDBMCompact(dimention, (int[])Matrix.Clone(), IsCanonicalForm);
        }

        public string GetID()
        {
            if (IsEmpty)
            {
                return "";
            }

            string toReturn = "";

            for (int i = 1; i < dimention; i++)
            {
                toReturn += (DBMConstraint.dbm_rawIsStrict(Matrix[0 * dimention + i]) ? "(" : "[") + DBMConstraint.dbm_raw2bound(Matrix[0 * dimention + i] * -1) + "," + DBMConstraint.dbm_raw2bound(Matrix[i * dimention + 0]) + (DBMConstraint.dbm_rawIsStrict(Matrix[i * dimention + 0]) ? ")" : "]");
            }

            return toReturn;
        }


        static int dbm_addFiniteFinite(int x, int y)
        {
            Debug.Assert(x < dbm_LS_INFINITY);
            Debug.Assert(y < dbm_LS_INFINITY);
            Debug.Assert(dbm_isValidRaw(x));
            Debug.Assert(dbm_isValidRaw(y));
            return (x + y) - ((x | y) & 1);
        }

        static bool dbm_isValidRaw(int x)
        {
            return (x == dbm_LS_INFINITY || (x < dbm_LE_OVERFLOW && -x < dbm_LE_OVERFLOW));
        }
    }
}
