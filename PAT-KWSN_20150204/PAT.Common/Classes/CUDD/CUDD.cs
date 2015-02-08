using System;
using System.Collections.Generic;

namespace PAT.Common.Classes.CUDDLib
{
    /// <summary>
    /// CUDD library in C# code
    /// </summary>
    public partial class CUDD
    {
        /// <summary>
        /// 0 for absolute |x(k) - x(k-1)| &lt; epsilon, 1 for criteria |((x(k) - x(k-1)) / x(k)| &lt; epsilon
        /// </summary>
        public const int TERMINATION_CRITERIA = 1;

        public const double TERMINATION_EPSILON = 0.000001;


        /// <summary>
        /// Return zero constant node, remember to call Reference
        /// </summary>
        public static CUDDNode ZERO;

        /// <summary>
        /// Return 1 constant node, remember to call Reference
        /// </summary>
        public static CUDDNode ONE;

        /// <summary>
        /// Return plus infinity constant node, remember to call Reference
        /// </summary>
        public static CUDDNode PLUS_INFINITY;

        /// <summary>
        /// Return minus infinity constant node, remember to call Reference
        /// </summary>
        public static CUDDNode MINUS_INFINITY;

        private static IntPtr manager;

        /// <summary>
        /// Manager the CUDD library
        /// </summary>
        public static IntPtr Manager
        {
            get { return CUDD.manager; }
        }

        /// <summary>
        /// Check whether a node is a constant
        /// </summary>
        public static bool IsConstant(CUDDNode dd)
        {
            return (PlatformInvoke.DDN_IsConstant(dd.Ptr) == 1);
        }

        /// <summary>
        /// Return the index of the given node
        /// </summary>
        public static int GetIndex(CUDDNode dd)
        {
            return PlatformInvoke.DDN_GetIndex(dd.Ptr);
        }

        /// <summary>
        /// Return the reference count
        /// </summary>
        public static int GetReference(CUDDNode dd)
        {
            return PlatformInvoke.DDN_GetReference(dd.Ptr);
        }

        /// <summary>
        /// Return the value of the given constant node
        /// </summary>
        public static double GetValue(CUDDNode dd)
        {
            return PlatformInvoke.DDN_GetValue(dd.Ptr);
        }

        /// <summary>
        /// Return the Then node of the given node
        /// </summary>
        public static CUDDNode GetThen(CUDDNode dd)
        {
            return new CUDDNode(PlatformInvoke.DDN_GetThen(dd.Ptr));
        }

        /// <summary>
        /// Return the Else node of the given node
        /// </summary>
        public static CUDDNode GetElse(CUDDNode dd)
        {
            return new CUDDNode(PlatformInvoke.DDN_GetElse(dd.Ptr));
        }

        /// <summary>
        /// Allocate an array of n DdNodes and return the pointer
        /// </summary>
        public static IntPtr AllocateNodes(int n)
        {
            return PlatformInvoke.DDV_AllocateNodes(n);
        }

        /// <summary>
        /// Assign the ith DdNode of array
        /// </summary>
        public static void SetElement(IntPtr vars, IntPtr var, int i)
        {
            PlatformInvoke.DDV_SetElement(vars, var, i);
        }

        /// <summary>
        /// Free the array of DdNodes
        /// </summary>
        public static void FreeArray(IntPtr array)
        {
            PlatformInvoke.DDV_FreeArray(array);
        }

        /// <summary>
        /// Create CUDD with maxMemory in KB
        /// [ REFS: 'none', DEREFS: 'none' ]
        /// </summary>
        public static void InitialiseCUDD(int maxMemory, double epsilon)
        {
            manager = PlatformInvoke.DD_InitialiseCUDD(maxMemory, epsilon);
            ZERO = Constant(0);
            ONE = Constant(1);
            PLUS_INFINITY = PlusInfinity();
            MINUS_INFINITY = MinusInfinity();
            //PlatformInvoke.Cudd_SetStdoutToFile(manager, Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\out.txt");
        }

        /// <summary>
        /// Set the maximum memory of CUDD in KB
        /// [ REFS: 'none', DEREFS: 'none' ]
        /// </summary>
        /// <param name="maxMemory"></param>
        public static void SetCUDDMaxMem(int maxMemory)
        {
            PlatformInvoke.DD_SetCUDDMaxMem(manager, maxMemory);
        }
        /// <summary>
        /// Sets the epsilon parameter of the manager
        /// [ REFS: 'none', DEREFS: 'none' ]
        /// </summary>
        public static void SetCUDDEpsilon(double epsilon)
        {
            PlatformInvoke.DD_SetCUDDEpsilon(manager, epsilon);
        }

        /// <summary>
        /// Close down the CUDD package
        /// Deletes resources associated with a DD manager
        /// [ REFS: 'none', DEREFS: 'none' ]
        /// </summary>
        public static void CloseDownCUDD()
        {
            Deref(ZERO, ONE, PLUS_INFINITY, MINUS_INFINITY);

            //Use in debug mode, to check whether any wrong in calculating reference number
            CUDD.Debug.Debugs();

            //Use this to print the summary information
            //CUDD.Print.PrintInfo(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\info.txt");

            PlatformInvoke.Cudd_Quit(manager);
        }

        /// <summary>
        /// Increases the reference count of a node, if it is not saturated
        /// [ REFS: dd, DEREFS: 'none' ]
        /// </summary>
        public static void Ref(CUDDNode dd)
        {
            PlatformInvoke.Cudd_Ref(dd.Ptr);
        }

        /// <summary>
        /// Ref all dd in list
        /// </summary>
        public static void Ref(List<CUDDNode> dds)
        {
            foreach (CUDDNode dd in dds)
            {
                CUDD.Ref(dd);
            }
        }

        /// <summary>
        /// Ref all dd in dds
        /// </summary>
        /// <param name="dds"></param>
        public static void Ref(params CUDDNode[] dds)
        {
            foreach (CUDDNode dd in dds)
            {
                CUDD.Ref(dd);
            }
        }

        /// <summary>
        /// Ref all dd in dds
        /// </summary>
        /// <param name="dds"></param>
        public static void Ref(params List<CUDDNode>[] dds)
        {
            foreach (List<CUDDNode> dd in dds)
            {
                CUDD.Ref(dd);
            }
        }

        /// <summary>
        /// Decreases the reference count of node
        /// [ REFS: 'none', DEREFS: dd ]
        /// </summary>
        public static void Deref(CUDDNode dd)
        {
            PlatformInvoke.Cudd_RecursiveDeref(manager, dd.Ptr);
        }

        /// <summary>
        /// Deref all dd in dds
        /// </summary>
        /// <param name="dds"></param>
        public static void Deref(List<CUDDNode> dds)
        {
            foreach (CUDDNode dd in dds)
            {
                CUDD.Deref(dd);
            }
        }

        /// <summary>
        /// Deref all vars
        /// </summary>
        /// <param name="dds"></param>
        public static void Deref(List<CUDDVars> vars)
        {
            foreach (CUDDVars v in vars)
            {
                v.Deref();
            }
        }

        /// <summary>
        /// Deref all dd in dds
        /// </summary>
        /// <param name="dds"></param>
        public static void Deref(params List<CUDDVars>[] dds)
        {
            foreach (List<CUDDVars> dd in dds)
            {
                CUDD.Deref(dd);
            }
        }

        /// <summary>
        /// Deref all dd in dds
        /// </summary>
        /// <param name="dds"></param>
        public static void Deref(params CUDDNode[] dds)
        {
            foreach (CUDDNode dd in dds)
            {
                CUDD.Deref(dd);
            }
        }

        /// <summary>
        /// Deref all dd in dds
        /// </summary>
        /// <param name="dds"></param>
        public static void Deref(params List<CUDDNode>[] dds)
        {
            foreach (List<CUDDNode> dd in dds)
            {
                CUDD.Deref(dd);
            }
        }

        /// <summary>
        /// Return a constant
        /// [ REFS: 'result', DEREFS: 'none' ]
        /// </summary>
        public static CUDDNode Constant(double value)
        {
            return new CUDDNode(PlatformInvoke.DD_Constant(manager, value));
        }

        /// <summary>
        /// Return the plus infinity constant
        /// [ REFS: 'result', DEREFS: 'none' ]
        /// </summary>
        public static CUDDNode PlusInfinity()
        {
            return new CUDDNode(PlatformInvoke.DD_PlusInfinity(manager));
        }

        /// <summary>
        /// Return the minus infinity constant
        /// [ REFS: 'result', DEREFS: 'none' ]
        /// </summary>
        /// <returns></returns>
        public static CUDDNode MinusInfinity()
        {
            return new CUDDNode(PlatformInvoke.DD_MinusInfinity(manager));
        }

        /// <summary>
        /// Get the ith variable, if not exist then create it
        /// Note that when you create variable i, then all variable from 0 to i-1 are also created
        /// [ REFS: 'result', DEREFS: 'none' ]
        /// </summary>
        public static CUDDNode Var(int i)
        {
            return new CUDDNode(PlatformInvoke.DD_Var(manager, i));
        }

        /// <summary>
        /// Return min of ADD
        /// [ REFS: 'none', DEREFS: 'none' ]
        /// </summary>
        public static double FindMin(CUDDNode dd)
        {
            return PlatformInvoke.DD_FindMin(manager, dd.Ptr);
        }

        /// <summary>
        /// Return max of ADD
        /// [ REFS: 'none', DEREFS: 'none' ]
        /// </summary>
        public static double FindMax(CUDDNode dd)
        {
            return PlatformInvoke.DD_FindMax(manager, dd.Ptr);
        }

        /// <summary>
        /// Return the first cube making the ADD not 0
        /// [ REFS: 'result', DEREFS: 'dd' ]
        /// </summary>
        public static CUDDNode RestrictToFirst(CUDDNode dd, CUDDVars vars)
        {
            return new CUDDNode(PlatformInvoke.DD_RestrictToFirst(manager, dd.Ptr, vars.GetArrayPointer(), vars.GetNumVars()));
        }

        /// <summary>
        /// Return number of nodes
        /// [ REFS: 'none', DEREFS: 'none' ]
        /// </summary>
        public static int GetNumNodes(CUDDNode dd)
        {
            return PlatformInvoke.DD_GetNumNodes(manager, dd.Ptr);
        }

        public static List<int> GetNumNodes(List<CUDDNode> dds)
        {
            List<int> result = new List<int>();
            foreach (CUDDNode dd in dds)
            {
                result.Add(CUDD.GetNumNodes(dd));
            }
            return result;
        }
        /// <summary>
        /// Return number of terminals
        /// [ REFS: 'none', DEREFS: 'none' ]
        /// </summary>
        public static int GetNumTerminals(CUDDNode dd)
        {
            return PlatformInvoke.DD_GetNumTerminals(manager, dd.Ptr);
        }

        /// <summary>
        /// Return number of minterm which made the ADD != 0
        /// [ REFS: 'none', DEREFS: 'none' ]
        /// </summary>
        public static double GetNumMinterms(CUDDNode dd, int numVars)
        {
            return PlatformInvoke.DD_GetNumMinterms(manager, dd.Ptr, numVars);
        }

        /// <summary>
        /// returns number of paths in dd
        /// [ REFS: 'none', DEREFS: 'none' ]
        /// </summary>
        public static double GetNumPaths(CUDDNode dd)
        {
            return PlatformInvoke.DD_GetNumPaths(manager, dd.Ptr);
        }

        /// <summary>
        /// Finds the variables on which a DD depends.
        /// Returns a ADD consisting of the product of the variables.
        /// [ REFS: 'result', DEREFS: 'none' ]
        /// </summary>
        public static CUDDNode GetSupport(CUDDNode dd)
        {
            return new CUDDNode(PlatformInvoke.DD_GetSupport(manager, dd.Ptr));
        }

        /// <summary>
        /// Return the int value corresponding the binary representation of the current minterm
        /// [ REFS: 'none', DEREFS: 'none' ]
        /// </summary>
        public static int MinTermToInt(CUDDNode dd, CUDDVars vars)
        {
            return PlatformInvoke.Cudd_MinTermToInt(manager, dd.Ptr, vars.GetArrayPointer(), vars.GetNumVars());
        }


        /// <summary>
        /// Check whether subSet is a subset of set
        /// [ REFS: '', DEREFS: 'none' ]
        /// </summary>
        public static bool IsSubSet(CUDDNode set, CUDDNode subSet)
        {
            CUDD.Ref(subSet);
            CUDD.Ref(set);
            CUDDNode temp = CUDD.Function.And(subSet, set);
            bool result = temp.Equals(subSet);
            CUDD.Deref(temp);
            return result;
        }

        /// <summary>
        /// Returns the memory in use by the manager measured in bytes.
        /// </summary>
        /// <returns></returns>
        public static int ReadMemoryInUse()
        {
            return (int) PlatformInvoke.Cudd_ReadMemoryInUse(manager);
        }

        /// <summary>
        /// When there is exception, most of the cases are because of newPermuation not contains fully number of bool variables
        /// Check how many bool are created
        /// </summary>
        /// <param name="newPermutation"></param>
        /// <returns></returns>
        public static int ShuffleHeap(List<int> newPermutation)
        {
            IntPtr newPermutationPtr = PlatformInvoke.DDV_AllocateIntArray(newPermutation.Count);
            for(int i = 0; i < newPermutation.Count; i++)
            {
                PlatformInvoke.DDV_SetElementIntArray(newPermutationPtr, i, newPermutation[i]);
            }
            int result = PlatformInvoke.Cudd_ShuffleHeap(manager, newPermutationPtr);

            PlatformInvoke.DDV_FreeIntArray(newPermutationPtr);

            return result;
        }

        /// <summary>
        /// [Compares two ADDs for equality within tolerance.]
        /// Description [Compares two ADDs for equality within tolerance. Two
        /// ADDs are reported to be equal if the maximum difference between them
        /// (the sup norm of their difference) is less than or equal to the
        /// tolerance parameter. 
        /// </summary>
        /// <param name="add1"></param>
        /// <param name="add2"></param>
        /// <returns></returns>
        public static bool IsEqual(CUDDNode add1, CUDDNode add2)
        {
            if(CUDD.TERMINATION_CRITERIA > 0)
            {
                return (PlatformInvoke.Cudd_EqualSupNormRel(manager, add1.Ptr, add2.Ptr, TERMINATION_EPSILON, 0) > 0);                
            }
            else
            {
                return (PlatformInvoke.Cudd_EqualSupNorm(manager, add1.Ptr, add2.Ptr, TERMINATION_EPSILON, 0) > 0);                       
            }
        }

        public static ODDNode BuildODD(CUDDNode reach, CUDDVars rowVars)
        {
            return new ODDNode(PlatformInvoke._Z9build_oddP9DdManagerP6DdNodePS2_i(manager, reach.Ptr, rowVars.GetArrayPointer(), rowVars.GetNumVars()));
        }
    }
}
