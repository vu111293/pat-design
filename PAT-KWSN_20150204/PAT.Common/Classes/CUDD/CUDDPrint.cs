using System;
using System.Collections.Generic;

namespace PAT.Common.Classes.CUDDLib
{
    public partial class CUDD
    {
        /// <summary>
        /// Print a boolean expression to the output mean
        /// </summary>
        public class Print
        {
            // print vector/matrix accuracy
            public const int ZERO_ONE = 1;
            public const int LOW = 2;
            public const int NORMAL = 3;
            public const int HIGH = 4;
            public const int LIST = 5;

            /// <summary>
            /// Print the cache information
            /// [ REFS: 'none', DEREFS: 'none' ]
            /// </summary>
            public static void PrintCacheInfo()
            {
                PlatformInvoke.DD_PrintCacheInfo(manager);
            }

            /// <summary>
            /// Print about number of nodes, terminals, minterms
            /// [ REFS: 'none', DEREFS: 'none' ]
            /// </summary>
            public static void PrintInfo(CUDDNode dd, int numVars)
            {
                PlatformInvoke.DD_PrintInfo(manager, dd.Ptr, numVars);
            }

            /// <summary>
            /// Print about number of nodes, terminals, minterms
            /// [ REFS: 'none', DEREFS: 'none' ]
            /// </summary>
            public static void PrintInfoBrief(CUDDNode dd, int numVars)
            {
                PlatformInvoke.DD_PrintInfoBrief(manager, dd.Ptr, numVars);
            }

            /// <summary>
            /// Return string of number of nodes, terminals, minterms
            /// [ REFS: 'none', DEREFS: 'none' ]
            /// </summary>
            public static string GetInfoString(CUDDNode dd, int numVars)
            {
                return GetNumNodes(dd) + " nodes (" + GetNumTerminals(dd) + " terminal), " + GetNumMinterms(dd, numVars) + " minterms";
            }

            /// <summary>
            /// Return string of number of nodes, terminals, minterms
            /// [ REFS: 'none', DEREFS: 'none' ]
            /// </summary>
            public static string GetInfoBriefString(CUDDNode dd, int numVars)
            {
                return "[" + GetNumNodes(dd) + "," + GetNumTerminals(dd) + "," + GetNumMinterms(dd, numVars) + "]";
            }

            /// <summary>
            /// Print the variables on which a DD depends.
            /// [ REFS: 'none', DEREFS: 'none' ]
            /// </summary>
            public static void PrintSupport(CUDDNode dd)
            {
                PlatformInvoke.DD_PrintSupport(manager, dd.Ptr);
            }

            /// <summary>
            /// Print vector dd, suppos that all variables supporting dd belong to vars
            /// [ REFS: 'none', DEREFS: 'none' ]
            /// </summary>
            public static void PrintVector(CUDDNode dd, CUDDVars vars, int accuracy)
            {
                PlatformInvoke.DD_PrintVector(manager, dd.Ptr, vars.GetArrayPointer(), vars.GetNumVars(), accuracy);
            }

            /// <summary>
            /// Print vector dd, suppos that all variables supporting dd belong to vars
            /// [ REFS: 'none', DEREFS: 'none' ]
            /// </summary>
            public static void PrintVector(CUDDNode dd, CUDDVars vars)
            {
                PlatformInvoke.DD_PrintVector(manager, dd.Ptr, vars.GetArrayPointer(), vars.GetNumVars(), LOW);
            }

            /// <summary>
            /// [ REFS: 'none', DEREFS: 'none' ]
            /// </summary>
            public static void PrintMatrix(CUDDNode dd, CUDDVars rVars, int numRowVars, CUDDVars cVars, int numColVars, int accuracy)
            {
                PlatformInvoke.DD_PrintMatrix(manager, dd.Ptr, rVars.GetArrayPointer(), numRowVars, cVars.GetArrayPointer(), numColVars, accuracy);
            }

            /// <summary>
            /// [ REFS: 'none', DEREFS: 'none' ]
            /// </summary>
            public static void PrintVectorFiltered(CUDDNode dd, CUDDNode filter, CUDDVars vars, int numVars, int accuracy)
            {
                PlatformInvoke.DD_PrintVectorFiltered(manager, dd.Ptr, filter.Ptr, vars.GetArrayPointer(), numVars);
            }

            /// <summary>
            /// [ REFS: 'none', DEREFS: 'none' ]
            /// </summary>
            public static void PrintMinterm(CUDDNode dd)
            {
                Console.WriteLine("--------------------------Start");
                PlatformInvoke.Cudd_PrintMinterm(manager, dd.Ptr);
                Console.WriteLine("--------------------------End");
            }

            /// <summary>
            /// [ REFS: 'none', DEREFS: 'none' ]
            /// </summary>
            public static void PrintMinterm(List<CUDDNode> dds)
            {
                Console.WriteLine("--------------------------Start");
                foreach (CUDDNode dd in dds)
                {
                    PrintMinterm(dd);
                }
                Console.WriteLine("--------------------------End");
            }


            /// <summary>
            /// Print terminals of DD
            /// [ REFS: 'none', DEREFS: 'none' ]
            /// </summary>
            public static void PrintTerminals(CUDDNode dd)
            {
                PlatformInvoke.DD_PrintTerminals(manager, dd.Ptr);
            }

            /// <summary>
            /// [ REFS: 'none', DEREFS: 'none' ]
            /// </summary>
            public static void PrintTerminalsAndNumbers(CUDDNode dd, int numVars)
            {
                PlatformInvoke.DD_PrintTerminalsAndNumbers(manager, dd.Ptr, numVars);
            }

            /// <summary>
            /// Print all information to file
            /// </summary>
            public static void PrintInfo(string fileName)
            {
                PlatformInvoke.Cudd_bddPrintInfo(manager, fileName);
            }

            public static int PrintBDDTree(CUDDNode node)
            {
                return PlatformInvoke.Cudd_PrintBDDTree(manager, node.Ptr, "BDDTree.dot");
            }
        }
    }
}
