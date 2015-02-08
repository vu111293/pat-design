using System;

namespace PAT.Common.Classes.CUDDLib
{
    public partial class CUDD
    {
        /// <summary>
        /// Support functions to debug
        /// </summary>
        public class Debug
        {
            /// <summary>
            /// Used when CUDD reports that during garbage collection the number of nodes actually deleted from the unique table is
            /// different from the count of dead nodes kept by the manager.
            /// </summary>
            public static int CheckKeys()
            {
                return PlatformInvoke.Cudd_CheckKeys(manager);
            }

            /// <summary>
            /// This function should be called immediately before shutting down the manager.
            /// Cudd_CheckZeroRef checks that the only nodes left with non-zero reference counts are the predefined constants,
            /// the BDD projection functions, and nodes whose reference counts are saturated. 
            /// </summary
            public static int CheckZeroRef()
            {
                return PlatformInvoke.Cudd_CheckZeroRef(manager);
            }

            /// <summary>
            /// Used when CUDD reports that during garbage collection the number of nodes actually deleted from the unique table is
            /// different from the count of dead nodes kept by the manager.
            /// Returns 0 if no inconsistencies are found; DD_OUT_OF_MEM if there is not enough memory; 1 otherwise.
            /// </summary>
            public static int DebugCheck()
            {
                return PlatformInvoke.Cudd_DebugCheck(manager);
            }

            /// <summary>
            /// Call all debug functions to test reference numver
            /// </summary>
            public static void Debugs()
            {
                CheckKeys();
                if (CUDD.Debug.DebugCheck() > 0)
                {
                    System.Diagnostics.Debug.WriteLine("Warning: CUDD reports an error on closing.");
                }

                int nonZero = CUDD.Debug.CheckZeroRef();
                if (nonZero > 0)
                {
                    System.Diagnostics.Debug.WriteLine("Warning: CUDD reports {0} non-zero references.", nonZero);
                }
            }
        }
    }
}
