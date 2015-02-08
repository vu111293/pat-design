using System.Collections.Generic;

namespace PAT.Common.Classes.CUDDLib
{
    public partial class CUDD
    {
        /// <summary>
        /// Support all functions related with the Abstract a boolean expression
        /// </summary>
        public class Abstract
        {
            /// <summary>
            /// Or Abstract variables in vars, dd must be 0-1 ADD.
            /// result will not contain boolean variables in vars
            /// [ REFS: 'result', DEREFS: dd ]
            /// </summary>
            public static CUDDNode ThereExists(CUDDNode dd, CUDDVars vars)
            {
                return new CUDDNode(PlatformInvoke.DD_ThereExists(manager, dd.Ptr, vars.GetArrayPointer(), vars.GetNumVars()));
            }

            /// <summary>
            /// Or Abstract variables in vars, dd must be 0-1 ADD.
            /// result will not contain boolean variables in vars
            /// [ REFS: 'result', DEREFS: dd ]
            /// </summary>
            public static List<CUDDNode> ThereExists(List<CUDDNode> dds, CUDDVars vars)
            {
                List<CUDDNode> result = new List<CUDDNode>();
                foreach (CUDDNode dd in dds)
                {
                    result.Add(ThereExists(dd, vars));
                }
                return result;
            }

            /// <summary>
            /// Universal Abstract (ie. product, *) variables in vars
            /// [ REFS: 'result', DEREFS: dd ]
            /// </summary>
            public static CUDDNode ForAll(CUDDNode dd, CUDDVars vars)
            {
                return new CUDDNode(PlatformInvoke.DD_ForAll(manager, dd.Ptr, vars.GetArrayPointer(), vars.GetNumVars()));
            }

            /// <summary>
            /// Sum (ie. +) Abstract variables in vars
            /// [ REFS: 'result', DEREFS: dd ]
            /// </summary>
            public static CUDDNode SumAbstract(CUDDNode dd, CUDDVars vars)
            {
                return new CUDDNode(PlatformInvoke.DD_SumAbstract(manager, dd.Ptr, vars.GetArrayPointer(), vars.GetNumVars()));
            }

            /// <summary>
            /// Universal Abstract (ie. product, *) variables in vars, the same with ForAll
            /// [ REFS: 'result', DEREFS: dd ]
            /// </summary>
            public static CUDDNode ProductAbstract(CUDDNode dd, CUDDVars vars)
            {
                return new CUDDNode(PlatformInvoke.DD_ProductAbstract(manager, dd.Ptr, vars.GetArrayPointer(), vars.GetNumVars()));
            }


            /// <summary>
            /// Return (f and g) abstract vars
            /// [ REFS: 'result', DEREFS: f ]
            /// </summary>
            public static CUDDNode MinAbstract(CUDDNode f, CUDDVars vars)
            {
                return new CUDDNode(PlatformInvoke.DD_MinAbstract(manager, f.Ptr, vars.GetArrayPointer(), vars.GetNumVars()));
            }

            /// <summary>
            /// Return (f and g) abstract vars
            /// [ REFS: 'result', DEREFS: f ]
            /// </summary>
            public static CUDDNode MaxAbstract(CUDDNode f, CUDDVars vars)
            {
                return new CUDDNode(PlatformInvoke.DD_MaxAbstract(manager, f.Ptr, vars.GetArrayPointer(), vars.GetNumVars()));
            }
        }
    }
}
