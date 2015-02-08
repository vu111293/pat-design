using System.Collections.Generic;

namespace PAT.Common.Classes.CUDDLib
{
    public partial class CUDD
    {
        /// <summary>
        /// Support funtions related with the boolean variables
        /// </summary>
        public class Variable
        {
            /// <summary>
            /// Permute variables (i.e. x_i -> y_i) of a node
            /// [ REFS: 'result', DEREFS: dd ]
            /// </summary>
            public static CUDDNode PermuteVariables(CUDDNode dd, CUDDVars oldVars, CUDDVars newVars)
            {
                return new CUDDNode(PlatformInvoke.DD_PermuteVariables(manager, dd.Ptr, oldVars.GetArrayPointer(), newVars.GetArrayPointer(), oldVars.GetNumVars()));
            }

            /// <summary>
            /// Swap variables (i.e. x_i -> y_i and y_i -> x_i) of a node.
            /// [ REFS: 'result', DEREFS: dd ]
            /// </summary>
            public static CUDDNode SwapVariables(CUDDNode dd, CUDDVars oldVars, CUDDVars newVars)
            {
                return new CUDDNode(PlatformInvoke.DD_SwapVariables(manager, dd.Ptr, oldVars.GetArrayPointer(), newVars.GetArrayPointer(), oldVars.GetNumVars()));
            }

            /// <summary>
            /// Swap variables (i.e. x_i -> y_i and y_i -> x_i) of a node.
            /// [ REFS: 'result', DEREFS: dds ]
            /// </summary>
            /// <param name="dds"></param>
            /// <param name="oldVars"></param>
            /// <param name="newVars"></param>
            /// <returns></returns>
            public static List<CUDDNode> SwapVariables(List<CUDDNode> dds, CUDDVars oldVars, CUDDVars newVars)
            {
                List<CUDDNode> result = new List<CUDDNode>();
                foreach (CUDDNode dd in dds)
                {
                    result.Add(CUDD.Variable.SwapVariables(dd, oldVars, newVars));
                }

                //
                return result;
            }

            /// <summary>
            /// Generates BDD for the function x > y
            /// where x, y are num_vars-bit numbers encoded by variables x_vars, y_vars
            /// [ REFS: 'result', DEREFS: 'none' ]
            /// </summary>
            public static CUDDNode VariablesGreaterThan(CUDDVars xVars, CUDDVars yVars)
            {
                return new CUDDNode(PlatformInvoke.DD_VariablesGreaterThan(manager, xVars.GetArrayPointer(), yVars.GetArrayPointer(), xVars.GetNumVars()));
            }

            /// <summary>
            /// Generates BDD for the function x >= y
            /// where x, y are num_vars-bit numbers encoded by variables x_vars, y_vars
            /// [ REFS: 'result', DEREFS: 'none' ]
            /// </summary>
            public static CUDDNode VariablesGreaterThanEquals(CUDDVars xVars, CUDDVars yVars)
            {
                return new CUDDNode(PlatformInvoke.DD_VariablesGreaterThanEquals(manager, xVars.GetArrayPointer(), yVars.GetArrayPointer(), xVars.GetNumVars()));
            }

            /// <summary>
            /// Generates BDD for the function x < y
            /// where x, y are num_vars-bit numbers encoded by variables x_vars, y_vars
            /// [ REFS: 'result', DEREFS: 'none' ]
            /// </summary>
            public static CUDDNode VariablesLessThan(CUDDVars xVars, CUDDVars yVars)
            {
                return new CUDDNode(PlatformInvoke.DD_VariablesLessThan(manager, xVars.GetArrayPointer(), yVars.GetArrayPointer(), xVars.GetNumVars()));
            }

            /// <summary>
            /// Generates BDD for the function x <= y
            /// where x, y are num_vars-bit numbers encoded by variables x_vars, y_vars
            /// [ REFS: 'result', DEREFS: 'none' ]
            /// </summary>
            public static CUDDNode VariablesLessThanEquals(CUDDVars xVars, CUDDVars yVars)
            {
                return new CUDDNode(PlatformInvoke.DD_VariablesLessThanEquals(manager, xVars.GetArrayPointer(), yVars.GetArrayPointer(), xVars.GetNumVars()));
            }

            /// <summary>
            /// Generates BDD for the function x = y
            /// where x, y are num_vars-bit numbers encoded by variables x_vars, y_vars
            /// [ REFS: 'result', DEREFS: 'none' ]
            /// </summary>
            public static CUDDNode VariablesEquals(CUDDVars xVars, CUDDVars yVars)
            {
                return new CUDDNode(PlatformInvoke.DD_VariablesEquals(manager, xVars.GetArrayPointer(), yVars.GetArrayPointer(), xVars.GetNumVars()));
            }
        }
    }
}
