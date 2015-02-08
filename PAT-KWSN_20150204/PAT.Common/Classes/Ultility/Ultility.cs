using System;
using System.Collections.Generic;
using System.Diagnostics;
using PAT.Common.Classes.DataStructure;
using PAT.Common.Classes.Expressions.ExpressionClass;
using PAT.Common.Classes.ModuleInterface;

namespace PAT.Common.Classes.Ultility
{
    public class Ultility
    {
        public static int MC_INITIAL_SIZE = 1048576; //2^20

        public static int SIMULATION_BOUND = 300;

        public static int DEFAULT_THREAD_NIMBER = 64;

        public static int ABSTRACT_CUT_NUMBER = 2;

        public static int ABSTRACT_CUT_NUMBER_BOUND = 50;

        public static int PARALLEL_MODEL_CHECKIMG_BOUND = 16;


        public static bool PERFORM_DETERMINIZATION = true;

        public static bool ENABLE_PARSING_OUTPUT = true;


        public const string PROMELA = "@@@PROMELA###";
        public const string UML = "@@@UML###";

        public static List<List<T>> CalculateCartesianProduct<T>(List<List<T>> inputList)
        {
            List<List<T>> cartesianProductList = new List<List<T>>();
            List<T> seedList = new List<T>();
            cartesianProductList.Add(seedList);

            for (int i = 0; i < inputList.Count; i++)
            {
                foreach (T valuation in inputList[i])
                {
                    if (cartesianProductList[0].Count == i)
                    {
                        foreach (List<T> list in cartesianProductList)
                        {
                            list.Add(valuation);
                        }
                    }
                    else
                    {                       
                        List<List<T>> toAdd = new List<List<T>>(cartesianProductList.Count);

                        foreach (List<T> list in cartesianProductList)
                        {
                            List<T> newProcList = new List<T>();

                            for (int j = 0; j < list.Count - 1; j++)
                            {
                                newProcList.Add(list[j]);
                            }

                            newProcList.Add(valuation);
                            toAdd.Add(newProcList);
                        }

                        cartesianProductList.AddRange(toAdd);
                    }
                }
            }
            return cartesianProductList;
        }

        public static string GetVariableString(StringDictionaryWithKey<ExpressionValue> Variables)
        {
            string id = "";
            if (Variables != null)
            {

                foreach (StringDictionaryEntryWithKey<ExpressionValue> pair in Variables._entries)
                {
                    if (pair != null)
                    {
                        id += pair.Value.ExpressionID+ ",";
                    }
                }
            }
            return id;
        }


        #region Set operations

        /// <summary>
        /// return the union of the two lists: add the new elements into list1.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list1"></param>
        /// <param name="list2"></param>
        /// <returns></returns>
        public static List<T> Union<T>(List<T> list1, List<T> list2)
        {
            for (int i = 0; i < list2.Count; i++)
            {
                T item = list2[i];
                if (!list1.Contains(item))
                {
                    list1.Add(item);
                }
            }
            return list1;
        }


        /// <summary>
        /// return the union of the two lists: add the new elements into list1.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list1"></param>
        /// <param name="list2"></param>
        /// <returns></returns>
        public static List<T> Substract<T>(List<T> list1, List<T> list2)
        {
            List<T> returnList = new List<T>();

            for (int i = 0; i < list1.Count; i++)
            {
                T item = list1[i];
                if(list2.Contains(item))
                {
                    returnList.Add(item);
                }
            }
            return returnList;
        }



        public static List<T> Intersect<T>(List<T> list1, List<T> list2)
        {
            List<T> returnList = new List<T>(list1.Count);
            for (int i = 0; i < list2.Count; i++)
            {
                T item = list2[i];
                if (list1.Contains(item))
                {
                    returnList.Add(item);
                }
            }
            return returnList;
        }



        /// <summary>
        /// add list2 into list1 with no duplication
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list1"></param>
        /// <param name="list2"></param>
        public static void AddList<T>(List<T> list1, List<T> list2)
        {
            foreach (T item in list2)
            {
                if (!list1.Contains(item))
                {
                    list1.Add(item);
                }
            }
        }






        //without modifying the original lists
        public static List<T> AddList2<T>(List<T> list1, List<T> list2)
        {
            List<T> result = new List<T>();

            result.AddRange(list1);

            for (int i = 0; i < list2.Count; i++)
            {
                T item = list2[i];
                if (!result.Contains(item))
                {
                    result.Add(item);
                }
            }

            return result;
        }

        public static string PPStringList<T>(List<T> list)
        {
            if (list == null)
            {
                return "";
            }

            string s = "";

            for (int i = 0; i < list.Count; i++)
            {
                T item = list[i];
                s += "," + item;
            }
            return s.TrimStart(',');
        }

        public static string PPStringList<T>(List<T> list, string separator)
        {
            if (list == null)
            {
                return "";
            }

            string s = "";

            for (int i = 0; i < list.Count; i++)
            {
                T item = list[i];
                s += separator + item.ToString();
            }
            return s.Substring(separator.Length);
        }



        public static string PPStringListDot<T>(List<T> list)
        {
            if (list == null)
            {
                return "";
            }

            string s = "";
            foreach (T item in list)
            {
                s += "." + item;
            }
            return s;
        }

        public static string PPStringList<T>(T[] list)
        {
            if (list == null)
            {
                return "";
            }

            string s = "";
            for (int i = 0; i < list.Length; i++)
            {
                T item = list[i];
                if (item != null)
                {
                    s += "," + item;
                }
            }
            return s.TrimStart(',');
        }

        public static string PPStringListDot<T>(T[] list)
        {
            if (list == null)
            {
                return "";
            }

            string s = "";
            foreach (T item in list)
            {
                s += "." + item;
            }
            return s;
        }

        public static string PPIDListDot(Expression[] list)
        {
            if (list == null)
            {
                return "";
            }

            string s = "";
            foreach (Expression item in list)
            {
                s += "." + item.ExpressionID; //.GetID();
            }
            return s;
        }


        public static string ListIntsToString(List<int> listOfInts)
        {
            string result = "";
            foreach (int a in listOfInts)
            {
                result += a.ToString() + ",";
            }
            return result.TrimEnd(',');
        }

        #endregion

        public static int CutNumber = 2;

        public static int ProcessCounterIncrement(int cutNumber, int counter, int increment)
        {
            if (counter == -1)
            {
                return -1;
            }

            if (cutNumber == -1 || cutNumber >= counter + increment)
            {
                return counter + increment;
            }

            return -1;
        }

        public static List<Dictionary<string, int>> ProcessCounterDecrement(int cutNumber, Dictionary<string, int> counters, string index, int stepSize)
        {
            List<Dictionary<string,int>> toReturn = new List<Dictionary<string, int>>();
            //not sure this clone is necessary or not!
            Dictionary<string, int> clone = new Dictionary<string, int>(counters);

            if (counters[index] == -1)
            {
                toReturn.Add(clone);

                //added by Sun Jun on Sep 16 2009
                if (cutNumber != -1)
                {
                    Dictionary<string, int> newclone = new Dictionary<string, int>(counters);
                    newclone[index] = cutNumber + 1 - stepSize;
                    toReturn.Add(newclone);                    
                }
            }
            else
            {
                int tmp = counters[index] - stepSize;
                Debug.Assert(tmp >= 0);
                clone[index] = tmp;
                toReturn.Add(clone);
            }

            return toReturn;
        }

        //public static void LogException(Exception ex)
        //{
        //    throw new NotImplementedException();
        //}

        public static object ShareDataLock;
        public static bool GrabSharedDataLock()
        {
            if (ShareDataLock == null)
            {
                ShareDataLock = true;
                return true;
            }
            return false;
        }


        public static void LockSharedData(SpecificationBase specification)
        {
            if (ShareDataLock != null && ShareDataLock.ToString() == "True")
            {
                lock (ShareDataLock)
                {
                    ShareDataLock = specification;
                    specification.LockSpecificationData();
                    
                }
            }
        }

        public static bool UnLockSharedData(SpecificationBase specification)
        {
            //try to lock the data and operations.
            if (ShareDataLock != null && ShareDataLock == specification)
            {
                lock (ShareDataLock)
                {
                    specification.UnLockSpecificationData();
                    ShareDataLock = null;
                }
                return true;
            }
            return false;
        }


        public static int DEFAULT_PRECISION = 5; //the precision for calculating probability.        
        public static double MAX_DIFFERENCE = 0.000001; //the precision for calculating probability.        


        public static string GetProbIntervalString(double min)
        {
            double precision = 1 * (double)Math.Pow(10, -1 * (DEFAULT_PRECISION + 1));
            int digits = precision.ToString().Length;
            return Math.Round(min, digits).ToString();
        }


        public static string GetProbIntervalString(double min, double max)
        {
            double precision = 1 * (double)Math.Pow(10, -1 * (DEFAULT_PRECISION + 1));
            int digits = precision.ToString().Length;
            return "[" + Math.Round(min, digits) + ", " + Math.Round(max, digits) + "]";
        }

        public static float RoundProbWithPrecision(double prob)
        {
            float precision = 1 * (float)Math.Pow(10, -1 * (DEFAULT_PRECISION + 1));
            int digits = precision.ToString().Length;
            return (float)Math.Round(prob, digits);
        }


        public static float RoundProbWithPrecision(double prob, int precisionDigits)
        {
            float precision = 1 * (float)Math.Pow(10, -1 * (precisionDigits + 1));
            int digits = precision.ToString().Length;
            return (float)Math.Round(prob, digits);
        }

        /// <summary>
        /// Shortest path
        /// </summary>
        /// <param name="n">size of the array</param>
        /// <param name="weights">weight_{ij} = (1) 0 if i = j (2) w_{ij} if i != j and (i, j) is connected (3) infinity if i != j and (i, j) is not connected</param>
        /// <returns>Distance [d_{ij}] where d_{ij} is the distance from vertext i to j</returns>
        public static int[,] FloydAlgorithm(int n, int[,] weights)
        {
            int[,] distance = new int[n,n];

            //Copy
            for (int i = 0; i < n; i++)
            {
                for(int j = 0; j < n; j++)
                {
                    distance[i, j] = weights[i, j];
                }
            }

            for (int k = 0; k < n; k++)
            {
                for (int i = 0; i < n; i++)
                {
                    for (int j = 0; j < n; j++)
                    {
                        if (SumWithOverFlowChecking(distance[i, k], distance[k, j]) < distance[i, j])
                        {
                            distance[i, j] = SumWithOverFlowChecking(distance[i, k], distance[k, j]);
                        }
                    }
                }
            }

            return distance;
        }

        /// <summary>
        /// Return sum of a, b
        /// a, b can be int.Max, but now Max
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        private static int SumWithOverFlowChecking(int a, int b)
        {
            return (a == int.MaxValue || b == int.MaxValue) ? int.MaxValue : (a + b);
        }


    }
}
