using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PAT.Common.Classes.DataStructure
{
    public class ClockValuation
    {
        /// <summary>
        /// Return valution where all clocks are set to 0
        /// </summary>
        /// <param name="dim"></param>
        /// <returns></returns>
        public static int[] ZeroValuation()
        {
            return new int[NewDBM.dim];
        }

        /// <summary>
        /// Return empty valuation where valuation[0] = -1;
        /// </summary>
        /// <param name="dim"></param>
        /// <returns></returns>
        public static int[] EmptyValuation()
        {
            int[] result = new int[NewDBM.dim];
            result[0] = -1;

            //
            return result;
        }

        /// <summary>
        /// Suppose that in the set of added constraints does not contain conflict
        /// </summary>
        /// <param name="valuation"></param>
        /// <param name="timer"></param>
        /// <param name="op"></param>
        /// <param name="constant"></param>
        public static void UpdateToSmallestSatisfyingValuation(int[] valuation, short timer, TimerOperationType op, int constant)
        {
            switch (op)
            {
                case TimerOperationType.Equals:
                    if (valuation[timer] > constant)
                    {
                        SetValuationEmpty(valuation);
                    }
                    else
                    {
                        int increasing = constant - valuation[timer];

                        if(increasing > 0)
                        {
                            for(int i = 1; i < valuation.Length; i++)
                            {
                                valuation[i] += increasing;
                            }
                        }
                    }
                    break;
                case TimerOperationType.GreaterThanOrEqualTo:
                    if (valuation[timer] < constant)
                    {
                        int increasing = constant - valuation[timer];

                        if (increasing > 0)
                        {
                            for (int i = 1; i < valuation.Length; i++)
                            {
                                valuation[i] += increasing;
                            }
                        } 
                    }
                    break;
                case TimerOperationType.LessThanOrEqualTo:
                    if (valuation[timer] > constant)
                    {
                        SetValuationEmpty(valuation);
                    }
                    break;
                case TimerOperationType.GreaterThan:
                    if (valuation[timer] < constant + 1)
                    {
                        int increasing = (constant + 1)- valuation[timer];

                        if (increasing > 0)
                        {
                            for (int i = 1; i < valuation.Length; i++)
                            {
                                valuation[i] += increasing;
                            }
                        }
                    }
                    break;
                case TimerOperationType.LessThan:
                    if (valuation[timer] >= constant - 1)
                    {
                        SetValuationEmpty(valuation);
                    }
                    break;
            }
        }

        private static void AddConstraint(int[] valuation, short timer, TimerOperationType op, int constant)
        {
            switch (op)
            {
                case TimerOperationType.Equals:
                    if( valuation[timer] != constant)
                    {
                        SetValuationEmpty(valuation);
                    }
                    break;
                case TimerOperationType.GreaterThanOrEqualTo:
                    if(valuation[timer] < constant)
                    {
                        SetValuationEmpty(valuation);
                    }
                    break;
                case TimerOperationType.LessThanOrEqualTo:
                    if(valuation[timer] > constant)
                    {
                        SetValuationEmpty(valuation);
                    }
                    break;
                case TimerOperationType.GreaterThan:
                    if(valuation[timer] <= constant)
                    {
                        SetValuationEmpty(valuation);
                    }
                    break;
                case TimerOperationType.LessThan:
                    if(valuation[timer] >= constant)
                    {
                        SetValuationEmpty(valuation);
                    }
                    break;
            }
        }

        public static void SetValuationEmpty(int[] valuation)
        {
            valuation[0] = -1;
        }

        public static bool IsEmpty(int[] valuation)
        {
            return (valuation[0] == -1);
        }

        public static void Reset(int[] valuation, short x)
        {
            valuation[x] = 0;
        }

        /// <summary>
        /// Return true if valution2 simulates valuation1
        /// </summary>
        /// <param name="valuation1"></param>
        /// <param name="valuation2"></param>
        /// <param name="currentState"></param>
        /// <returns></returns>
        public static bool IsLUSimulated(int[] valuation1, int[] valuation2, string[] currentState)
        {
            for(int i = 1; i < valuation1.Length; i++)
            {
                if(!IsLUSimulated(valuation1[i], valuation2[i], NewDBM.GetLower(i, true, currentState), NewDBM.GetUpper(i, true, currentState)))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Return true if valution2 simulates valuation1
        /// </summary>
        /// <param name="valuation1"></param>
        /// <param name="valuation2"></param>
        /// <param name="currentState">All the clocks in the same process. currentState is the state of this process</param>
        /// <returns></returns>
        public static bool IsLUSimulated(int[] valuation1, int[] valuation2, string currentState)
        {
            for (int i = 1; i < valuation1.Length; i++)
            {
                if (!IsLUSimulated(valuation1[i], valuation2[i], NewDBM.GetLower(i, true, currentState), NewDBM.GetUpper(i, true, currentState)))
                {
                    return false;
                }
            }

            return true;
        }

        private static bool IsLUSimulated(int clockValue1, int clockValue2, int clockLowerbound, int clockUpperBound)
        {
            return (clockValue1 == clockValue2 || (clockLowerbound < clockValue2 && clockValue2 < clockValue1) || (clockUpperBound < clockValue1 && clockValue1 < clockValue2));
        }


        /// <summary>
        /// If this is true, the current valution can simulate all the valuation in the future
        /// </summary>
        /// <param name="valuation"></param>
        /// <param name="currentState"></param>
        /// <returns></returns>
        public static bool AllClockGreaterThanLower(int[] valuation, string[] currentState)
        {
            for (int i = 1; i < valuation.Length; i++)
            {
                if (valuation[i] <= NewDBM.GetLower(i, true, currentState))
                {
                    return false;
                }
            }

            return true;
        }

    }
}
