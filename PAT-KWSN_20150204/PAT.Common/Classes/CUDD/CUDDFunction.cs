using System.Collections.Generic;

namespace PAT.Common.Classes.CUDDLib
{
    public partial class CUDD
    {
        /// <summary>
        /// Suport functions in BDD and ADD including logic and arithmetic functions
        /// </summary>
        public class Function
        {
            /// <summary>
            /// Return the complement of node
            /// [ REFS: 'result', DEREFS: 'dd' ]
            /// </summary>
            public static CUDDNode Not(CUDDNode dd)
            {
                return new CUDDNode(PlatformInvoke.DD_Not(manager, dd.Ptr));
            }


            /// <summary>
            /// Return the complement of dds
            /// [ REFS: 'result', DEREFS: 'dds' ]
            /// </summary>
            public static CUDDNode Not(List<CUDDNode> dds)
            {
                CUDDNode result = CUDD.Constant(1);

                foreach (CUDDNode dd in dds)
                {
                    result = CUDD.Function.And(result, CUDD.Function.Not(dd));
                }

                return result;
            }


            /// <summary>
            /// Or 0-1 ADDs, difficult to know the result if not 0-1 ADDs, refer the C++ code
            /// [ REFS: 'result', DEREFS: dd1, dd2 ]
            /// </summary>
            public static CUDDNode Or(CUDDNode dd1, CUDDNode dd2)
            {
                return new CUDDNode(PlatformInvoke.DD_Or(manager, dd1.Ptr, dd2.Ptr));
            }


            /// <summary>
            /// Or all CUDDNode in dds
            /// [ REFS: 'result', DEREFS: dds ]
            /// </summary>
            public static CUDDNode Or(List<CUDDNode> dds)
            {
                CUDDNode result = CUDD.Constant(0);

                foreach (CUDDNode dd in dds)
                {
                    result = CUDD.Function.Or(result, dd);
                }

                return result;
            }

            /// <summary>
            /// And 0-1 ADDs, difficult to know the result if not 0-1 ADDs, refer the C++ code
            /// [ REFS: 'result', DEREFS: dd1, dd2 ]
            /// </summary>
            public static CUDDNode And(CUDDNode dd1, CUDDNode dd2)
            {
                return new CUDDNode(PlatformInvoke.DD_And(manager, dd1.Ptr, dd2.Ptr));
            }

            /// <summary>
            /// list1, list 2 are 2 list of nodes. These nodes are Or-explicit
            /// Return (list1 and list2)
            /// [ REFS: 'result', DEREFS: dd1, dd2 ]
            /// </summary>
            public static List<CUDDNode> And(List<CUDDNode> list1, List<CUDDNode> list2)
            {
                List<CUDDNode> result = new List<CUDDNode>();
                foreach (CUDDNode dd1 in list1)
                {
                    foreach (CUDDNode dd2 in list2)
                    {
                        CUDD.Ref(dd1);
                        CUDD.Ref(dd2);
                        CUDDNode dd = CUDD.Function.And(dd1, dd2);
                        if (!dd.Equals(CUDD.ZERO))
                        {
                            result.Add(dd);
                        }
                        else
                        {
                            CUDD.Deref(dd);
                        }
                    }
                }
                CUDD.Deref(list1);
                CUDD.Deref(list2);

                //
                return result;
            }

            /// <summary>
            /// (dd && list[0]) || (dd && list[1]) ...
            /// [ REFS: 'result', DEREFS: dd, list ]
            /// </summary>
            /// <param name="dd"></param>
            /// <param name="list"></param>
            /// <returns></returns>
            public static CUDDNode And(CUDDNode dd, List<CUDDNode> list)
            {
                CUDDNode result = CUDD.Constant(0);

                foreach (CUDDNode dd1 in list)
                {
                    CUDD.Ref(dd);
                    CUDDNode temp = CUDD.Function.And(dd, dd1);
                    result = CUDD.Function.Or(result, temp);
                }
                CUDD.Deref(dd);
                return result;
            }

            /// <summary>
            /// (dd && list[0]) || (dd && list[1]) ...
            /// [ REFS: 'result', DEREFS: dd, list ]
            /// </summary>
            /// <param name="dd"></param>
            /// <param name="list"></param>
            /// <returns></returns>
            public static List<CUDDNode> And(List<CUDDNode> list, CUDDNode dd)
            {
                List<CUDDNode> result = new List<CUDDNode>();
                foreach (CUDDNode dd1 in list)
                {
                    CUDD.Ref(dd);
                    CUDDNode temp = CUDD.Function.And(dd, dd1);
                    if (temp.Equals(CUDD.ZERO))
                    {
                        CUDD.Deref(temp);
                    }
                    else
                    {
                        result.Add(temp);
                    }
                }
                CUDD.Deref(dd);
                return result;
            }

            /// <summary>
            /// return 1 if node1 is 1 and node2 is 0, others return 0.
            ///  belong dd1 but not belong dd2
            /// [ REFS: 'result', DEREFS: dd1, dd2 ]
            /// </summary>
            public static CUDDNode Different(CUDDNode dd1, CUDDNode dd2)
            {
                return new CUDDNode(PlatformInvoke.DD_Different(manager, dd1.Ptr, dd2.Ptr));
            }

            /// <summary>
            /// Xor 0-1 ADDs, difficult to know the result if not 0-1 ADDs, refer the C++ code
            /// [ REFS: 'result', DEREFS: dd1, dd2 ]
            /// </summary>
            public static CUDDNode Xor(CUDDNode dd1, CUDDNode dd2)
            {
                return new CUDDNode(PlatformInvoke.DD_Xor(manager, dd1.Ptr, dd2.Ptr));
            }

            /// <summary>
            /// Imply 0-1 ADDs, difficult to know the result if not 0-1 ADDs, refer the C++ code
            /// [ REFS: 'result', DEREFS: dd1, dd2 ]
            /// </summary>
            public static CUDDNode Implies(CUDDNode dd1, CUDDNode dd2)
            {
                return new CUDDNode(PlatformInvoke.DD_Implies(manager, dd1.Ptr, dd2.Ptr));
            }

            /// <summary>
            /// Return the sum ADD of dd1, dd2
            /// [ REFS: 'result', DEREFS: dd1, dd2 ]
            /// </summary>
            public static CUDDNode Plus(CUDDNode dd1, CUDDNode dd2)
            {
                return new CUDDNode(PlatformInvoke.DD_Plus(manager, dd1.Ptr, dd2.Ptr));
            }

            /// <summary>
            /// Return the remain ADD of dd1, dd2
            /// [ REFS: 'result', DEREFS: dd1, dd2 ]
            /// </summary>
            public static CUDDNode Minus(CUDDNode dd1, CUDDNode dd2)
            {
                return new CUDDNode(PlatformInvoke.DD_Minus(manager, dd1.Ptr, dd2.Ptr));
            }

            /// <summary>
            /// Return the product ADD of dd1, dd2
            /// [ REFS: 'result', DEREFS: dd1, dd2 ]
            /// </summary>
            public static CUDDNode Times(CUDDNode dd1, CUDDNode dd2)
            {
                return new CUDDNode(PlatformInvoke.DD_Times(manager, dd1.Ptr, dd2.Ptr));
            }

            /// <summary>
            /// Return the ratio ADD of dd1, dd2
            /// [ REFS: 'result', DEREFS: dd1, dd2 ]
            /// </summary>
            public static CUDDNode Divide(CUDDNode dd1, CUDDNode dd2)
            {
                return new CUDDNode(PlatformInvoke.DD_Divide(manager, dd1.Ptr, dd2.Ptr));
            }

            /// <summary>
            /// Return the ratio ADD of dd1, dd2
            /// [ REFS: 'result', DEREFS: dd1, dd2 ]
            /// </summary>
            public static CUDDNode BitwiseAnd(CUDDNode dd1, CUDDNode dd2)
            {
                return new CUDDNode(PlatformInvoke.DD_BitwiseAnd(manager, dd1.Ptr, dd2.Ptr));
            }

            /// <summary>
            /// Return the ratio ADD of dd1, dd2
            /// [ REFS: 'result', DEREFS: dd1, dd2 ]
            /// </summary>
            public static CUDDNode BitwiseOr(CUDDNode dd1, CUDDNode dd2)
            {
                return new CUDDNode(PlatformInvoke.DD_BitwiseOr(manager, dd1.Ptr, dd2.Ptr));
            }

            /// <summary>
            /// At each terminal, return min(dd1, dd2)
            /// [ REFS: 'result', DEREFS: dd1, dd2 ]
            /// </summary>
            public static CUDDNode Minimum(CUDDNode dd1, CUDDNode dd2)
            {
                return new CUDDNode(PlatformInvoke.DD_Minimum(manager, dd1.Ptr, dd2.Ptr));
            }

            /// <summary>
            /// At each terminal, return max(dd1, dd2)
            /// [ REFS: 'result', DEREFS: dd1, dd2 ]
            /// </summary>
            public static CUDDNode Maximum(CUDDNode dd1, CUDDNode dd2)
            {
                return new CUDDNode(PlatformInvoke.DD_Maximum(manager, dd1.Ptr, dd2.Ptr));
            }

            /// <summary>
            /// At each terminal, return dd1 if dd1 == dd2; 0 if dd1 != dd2
            /// [ REFS: 'result', DEREFS: dd1, dd2 ]
            /// </summary>
            public static CUDDNode Agreement(CUDDNode dd1, CUDDNode dd2)
            {
                return new CUDDNode(PlatformInvoke.DD_Agreement(manager, dd1.Ptr, dd2.Ptr));
            }

            /// <summary>
            /// At each terminal, return plusinfinity if dd1 == dd2; min(dd1, dd2) if dd1 != dd2
            /// [ REFS: 'result', DEREFS: dd1, dd2 ]
            /// </summary>
            public static CUDDNode Diff(CUDDNode dd1, CUDDNode dd2)
            {
                return new CUDDNode(PlatformInvoke.DD_Diff(manager, dd1.Ptr, dd2.Ptr));
            }

            /// <summary>
            /// At each terminal, return 1 if dd1 == dd2; 0 if dd1 != dd2
            /// [ REFS: 'result', DEREFS: dd1, dd2 ]
            /// </summary>
            public static CUDDNode Equal(CUDDNode dd1, CUDDNode dd2)
            {
                return new CUDDNode(PlatformInvoke.DD_ApplyEqual(manager, dd1.Ptr, dd2.Ptr));
            }

            /// <summary>
            /// At each terminal, return 1 if dd1 != dd2; 0 if dd1 == dd2
            /// [ REFS: 'result', DEREFS: dd1, dd2 ]
            /// </summary>
            public static CUDDNode NotEqual(CUDDNode dd1, CUDDNode dd2)
            {
                return new CUDDNode(PlatformInvoke.DD_ApplyNotEqual(manager, dd1.Ptr, dd2.Ptr));
            }

            /// <summary>
            /// At each terminal, return 1 if dd1 > dd2; 0 otherwise
            /// [ REFS: 'result', DEREFS: dd1, dd2 ]
            /// </summary>
            public static CUDDNode Greater(CUDDNode dd1, CUDDNode dd2)
            {
                return new CUDDNode(PlatformInvoke.DD_ApplyGreater(manager, dd1.Ptr, dd2.Ptr));
            }

            /// <summary>
            /// At each terminal, return 1 if dd1 >= dd2; 0 otherwise
            /// [ REFS: 'result', DEREFS: dd1, dd2 ]
            /// </summary>
            public static CUDDNode GreaterEqual(CUDDNode dd1, CUDDNode dd2)
            {
                return new CUDDNode(PlatformInvoke.DD_ApplyGreaterEqual(manager, dd1.Ptr, dd2.Ptr));
            }

            /// <summary>
            /// At each terminal, return 1 if dd1 < dd2; 0 if otherwise
            /// [ REFS: 'result', DEREFS: dd1, dd2 ]
            /// </summary>
            public static CUDDNode Less(CUDDNode dd1, CUDDNode dd2)
            {
                return new CUDDNode(PlatformInvoke.DD_ApplyLess(manager, dd1.Ptr, dd2.Ptr));
            }

            /// <summary>
            /// At each terminal, return 1 if dd1 <= dd2; 0 otherwise
            /// [ REFS: 'result', DEREFS: dd1, dd2 ]
            /// </summary>
            public static CUDDNode LessEqual(CUDDNode dd1, CUDDNode dd2)
            {
                return new CUDDNode(PlatformInvoke.DD_ApplyLessEqual(manager, dd1.Ptr, dd2.Ptr));
            }

            /// <summary>
            /// At each terminal, sets dd1 to the value of dd2 wherever dd2 != 0
            /// [ REFS: 'result', DEREFS: dd1, dd2 ]
            /// </summary>
            public static CUDDNode SetNZ(CUDDNode dd1, CUDDNode dd2)
            {
                return new CUDDNode(PlatformInvoke.DD_SetNZ(manager, dd1.Ptr, dd2.Ptr));
            }

            /// <summary>
            /// At each terminal, return dd1 if dd1 >= dd2; 0 if dd1 < dd2
            /// [ REFS: 'result', DEREFS: dd1, dd2 ]
            /// </summary>
            public static CUDDNode Threshold(CUDDNode dd1, CUDDNode dd2)
            {
                return new CUDDNode(PlatformInvoke.DD_Thresholds(manager, dd1.Ptr, dd2.Ptr));
            }

            /// <summary>
            /// Natural logarithm of an ADD
            /// [ REFS: 'result', DEREFS: dd]
            /// </summary>
            public static CUDDNode Log(CUDDNode dd)
            {
                return new CUDDNode(PlatformInvoke.DD_Log(manager, dd.Ptr));
            }

            /// <summary>
            /// Restrict a node according to the cube of variables.
            /// [ REFS: 'result', DEREFS: dd, cube ]
            /// </summary>
            public static CUDDNode Restrict(CUDDNode dd, CUDDNode cube)
            {
                return new CUDDNode(PlatformInvoke.DD_Restrict(manager, dd.Ptr, cube.Ptr));
            }

            /// <summary>
            /// Create node with root node dd1, its then node is dd2 and its else node is dd3.
            /// This procedure assumes that dd1 is a 0-1 ADD.
            /// [ REFS: 'result', DEREFS: dd1, dd2, dd3 ].
            /// </summary>
            public static CUDDNode ITE(CUDDNode dd1, CUDDNode dd2, CUDDNode dd3)
            {
                return new CUDDNode(PlatformInvoke.DD_ITE(manager, dd1.Ptr, dd2.Ptr, dd3.Ptr));
            }

            /// <summary>
            /// At each terminal, return dd1 % dd2
            /// [ REFS: 'result', DEREFS: dd1, dd2 ]
            /// </summary>
            public static CUDDNode Modulo(CUDDNode dd1, CUDDNode dd2)
            {
                return new CUDDNode(PlatformInvoke.DD_Modulo(manager, dd1.Ptr, dd2.Ptr));
            }

            /// <summary>
            /// At each terminal, return dd1 % dd2, non-negative when dd2 > 0
            /// [ REFS: 'result', DEREFS: dd1, dd2 ]
            /// </summary>
            public static CUDDNode ModuloNonNegative(CUDDNode dd1, CUDDNode dd2)
            {
                return new CUDDNode(PlatformInvoke.DD_ModuloNonNegative(manager, dd1.Ptr, dd2.Ptr));
            }

            /// <summary>
            /// Rounds off the discriminants of an ADD. The discriminants are rounded off to N digits after the decimal.
            /// [ REFS: 'result', DEREFS: dd ]
            /// </summary>
            public static CUDDNode RoundOff(CUDDNode dd, int numberOfDigits)
            {
                return new CUDDNode(PlatformInvoke.DD_RoundOff(manager, dd.Ptr, numberOfDigits));
            }
        }
    }
}
