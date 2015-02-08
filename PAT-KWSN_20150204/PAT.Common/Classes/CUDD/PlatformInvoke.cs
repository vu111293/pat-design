using System;
using System.Runtime.InteropServices;

namespace PAT.Common.Classes.CUDDLib
{
    /// <summary>
    /// Interface of CUDD in C#
    /// </summary>
    public class PlatformInvoke
    {
        #region Import from C++
        //cudd's own

        [DllImport(@"CUDDHelper.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern void Cudd_Quit(IntPtr manager);

        [DllImport(@"CUDDHelper.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern void Cudd_Ref(IntPtr dd);

        [DllImport(@"CUDDHelper.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern void Cudd_RecursiveDeref(IntPtr manager, IntPtr dd);

        [DllImport(@"CUDDHelper.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern int DDN_IsConstant(IntPtr dd);

        [DllImport(@"CUDDHelper.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern int DDN_GetIndex(IntPtr dd);

        [DllImport(@"CUDDHelper.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern int DDN_GetReference(IntPtr dd);

        [DllImport(@"CUDDHelper.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern double DDN_GetValue(IntPtr dd);

        [DllImport(@"CUDDHelper.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern IntPtr DDN_GetThen(IntPtr dd);

        [DllImport(@"CUDDHelper.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern IntPtr DDN_GetElse(IntPtr dd);

        [DllImport(@"CUDDHelper.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern IntPtr DDV_AllocateNodes(int n);

        [DllImport(@"CUDDHelper.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern void DDV_SetElement(IntPtr nodes, IntPtr node, int i);

        [DllImport(@"CUDDHelper.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern void DDV_FreeArray(IntPtr a);

        [DllImport(@"CUDDHelper.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern int Cudd_PrintMinterm(IntPtr manager, IntPtr ddNode);

        [DllImport(@"CUDDHelper.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern int Cudd_CheckKeys(IntPtr manager);

        [DllImport(@"CUDDHelper.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern int Cudd_CheckZeroRef(IntPtr manager);

        [DllImport(@"CUDDHelper.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern int Cudd_DebugCheck(IntPtr manager);

        [DllImport(@"CUDDHelper.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern void Cudd_SetStdoutToFile(IntPtr dd, string fileName);

        [DllImport(@"CUDDHelper.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern void Cudd_FlushStdOut(IntPtr dd);

        [DllImport(@"CUDDHelper.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern int Cudd_MinTermToInt(IntPtr manager, IntPtr dd, IntPtr vars, int num_vars);

        //prism
        [DllImport(@"CUDDHelper.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern IntPtr DD_ThereExists(IntPtr ddman, IntPtr dd, IntPtr vars, int num_vars);

        [DllImport(@"CUDDHelper.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern IntPtr DD_ForAll(IntPtr ddman, IntPtr dd, IntPtr vars, int num_vars);

        [DllImport(@"CUDDHelper.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern IntPtr DD_SumAbstract(IntPtr ddman, IntPtr dd, IntPtr vars, int num_vars);

        [DllImport(@"CUDDHelper.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern IntPtr DD_ProductAbstract(IntPtr ddman, IntPtr dd, IntPtr vars, int num_vars);
        
        [DllImport(@"CUDDHelper.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern IntPtr DD_AndExists(IntPtr ddman, IntPtr f, IntPtr g, IntPtr vars, int num_vars);

        [DllImport(@"CUDDHelper.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern IntPtr DD_MinAbstract(IntPtr ddman, IntPtr dd, IntPtr vars, int num_vars);

        [DllImport(@"CUDDHelper.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern IntPtr DD_MaxAbstract(IntPtr ddman, IntPtr dd, IntPtr vars, int num_vars);

        [DllImport(@"CUDDHelper.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern IntPtr DD_Create(IntPtr ddman);

        [DllImport(@"CUDDHelper.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern IntPtr DD_Constant(IntPtr ddman, double value);

        [DllImport(@"CUDDHelper.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern IntPtr DD_PlusInfinity(IntPtr ddman);

        [DllImport(@"CUDDHelper.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern IntPtr DD_MinusInfinity(IntPtr ddman);

        [DllImport(@"CUDDHelper.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern IntPtr DD_Var(IntPtr ddman, int i);

        [DllImport(@"CUDDHelper.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern IntPtr DD_Not(IntPtr ddman, IntPtr dd);

        [DllImport(@"CUDDHelper.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern IntPtr DD_Or(IntPtr ddman, IntPtr dd1, IntPtr dd2);

        [DllImport(@"CUDDHelper.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern IntPtr DD_And(IntPtr ddman, IntPtr dd1, IntPtr dd2);

        [DllImport(@"CUDDHelper.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern IntPtr DD_Xor(IntPtr ddman, IntPtr dd1, IntPtr dd2);

        [DllImport(@"CUDDHelper.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern IntPtr DD_Implies(IntPtr ddman, IntPtr dd1, IntPtr dd2);

        [DllImport(@"CUDDHelper.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern IntPtr DD_Different(IntPtr ddman, IntPtr dd1, IntPtr dd2);

        [DllImport(@"CUDDHelper.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern IntPtr DD_Plus(IntPtr ddman, IntPtr dd1, IntPtr dd2);

        [DllImport(@"CUDDHelper.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern IntPtr DD_Minus(IntPtr ddman, IntPtr dd1, IntPtr dd2);

        [DllImport(@"CUDDHelper.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern IntPtr DD_Times(IntPtr ddman, IntPtr dd1, IntPtr dd2);

        [DllImport(@"CUDDHelper.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern IntPtr DD_Divide(IntPtr ddman, IntPtr dd1, IntPtr dd2);

        [DllImport(@"CUDDHelper.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern IntPtr DD_BitwiseAnd(IntPtr ddman, IntPtr dd1, IntPtr dd2);

        [DllImport(@"CUDDHelper.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern IntPtr DD_BitwiseOr(IntPtr ddman, IntPtr dd1, IntPtr dd2);

        [DllImport(@"CUDDHelper.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern IntPtr DD_Minimum(IntPtr ddman, IntPtr dd1, IntPtr dd2);

        [DllImport(@"CUDDHelper.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern IntPtr DD_Maximum(IntPtr ddman, IntPtr dd1, IntPtr dd2);

        [DllImport(@"CUDDHelper.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern IntPtr DD_Modulo(IntPtr ddman, IntPtr dd1, IntPtr dd2);

        [DllImport(@"CUDDHelper.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern IntPtr DD_ModuloNonNegative(IntPtr ddman, IntPtr dd1, IntPtr dd2);

        [DllImport(@"CUDDHelper.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern IntPtr DD_Agreement(IntPtr ddman, IntPtr dd1, IntPtr dd2);

        [DllImport(@"CUDDHelper.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern IntPtr DD_Diff(IntPtr ddman, IntPtr dd1, IntPtr dd2);

        [DllImport(@"CUDDHelper.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern IntPtr DD_ApplyEqual(IntPtr ddman, IntPtr dd1, IntPtr dd2);

        [DllImport(@"CUDDHelper.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern IntPtr DD_ApplyNotEqual(IntPtr ddman, IntPtr dd1, IntPtr dd2);

        [DllImport(@"CUDDHelper.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern IntPtr DD_ApplyGreater(IntPtr ddman, IntPtr dd1, IntPtr dd2);

        [DllImport(@"CUDDHelper.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern IntPtr DD_ApplyGreaterEqual(IntPtr ddman, IntPtr dd1, IntPtr dd2);

        [DllImport(@"CUDDHelper.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern IntPtr DD_ApplyLess(IntPtr ddman, IntPtr dd1, IntPtr dd2);

        [DllImport(@"CUDDHelper.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern IntPtr DD_ApplyLessEqual(IntPtr ddman, IntPtr dd1, IntPtr dd2);

        [DllImport(@"CUDDHelper.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern IntPtr DD_SetNZ(IntPtr ddman, IntPtr dd1, IntPtr dd2);

        [DllImport(@"CUDDHelper.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern IntPtr DD_Thresholds(IntPtr ddman, IntPtr dd1, IntPtr dd2);

        [DllImport(@"CUDDHelper.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern IntPtr DD_Log(IntPtr ddman, IntPtr dd);

        [DllImport(@"CUDDHelper.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern IntPtr DD_Restrict(IntPtr ddman, IntPtr dd, IntPtr cube);

        [DllImport(@"CUDDHelper.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern IntPtr DD_ITE(IntPtr ddman, IntPtr dd1, IntPtr dd2, IntPtr dd3);

        [DllImport(@"CUDDHelper.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern IntPtr DD_InitialiseCUDD(int max_mem, double epsilon);

        [DllImport(@"CUDDHelper.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern void DD_SetCUDDMaxMem(IntPtr ddman, int max_mem);

        [DllImport(@"CUDDHelper.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern void DD_SetCUDDEpsilon(IntPtr ddman, double epsilon);

        [DllImport(@"CUDDHelper.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern void DD_PrintCacheInfo(IntPtr ddman);

        [DllImport(@"CUDDHelper.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern void DD_CloseDownCUDD(IntPtr ddman);

        [DllImport(@"CUDDHelper.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern int Cudd_CheckZeroRefVerbose(IntPtr manager);

        [DllImport(@"CUDDHelper.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern int DD_GetNumNodes(IntPtr ddman, IntPtr dd);

        [DllImport(@"CUDDHelper.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern int DD_GetNumTerminals(IntPtr ddman, IntPtr dd);

        [DllImport(@"CUDDHelper.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern double DD_GetNumMinterms(IntPtr ddman, IntPtr dd, int num_vars);

        [DllImport(@"CUDDHelper.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern double DD_GetNumPaths(IntPtr ddman, IntPtr dd);

        [DllImport(@"CUDDHelper.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern void DD_PrintInfo(IntPtr ddman, IntPtr dd, int num_vars);

        [DllImport(@"CUDDHelper.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern void DD_PrintInfoBrief(IntPtr ddman, IntPtr dd, int num_vars);

        [DllImport(@"CUDDHelper.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern void DD_PrintSupport(IntPtr ddman, IntPtr dd);

        [DllImport(@"CUDDHelper.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern IntPtr DD_GetSupport(IntPtr ddman, IntPtr dd);

        [DllImport(@"CUDDHelper.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern void DD_PrintTerminals(IntPtr ddman, IntPtr dd);

        [DllImport(@"CUDDHelper.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern void DD_PrintTerminalsAndNumbers(IntPtr ddman, IntPtr dd, int num_vars);

        [DllImport("CUDDHelper.dll")]
        public static extern int Cudd_bddPrintInfo(IntPtr manager, string fileName);

        [DllImport(@"CUDDHelper.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern IntPtr DD_SetVectorElement(IntPtr ddman, IntPtr dd, IntPtr vars, int num_vars, int index, double value);

        [DllImport(@"CUDDHelper.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern IntPtr DD_SetMatrixElement(IntPtr ddman, IntPtr dd, IntPtr rvars, int num_rvars, IntPtr cvars, int num_cvars, int rindex, int cindex, double value);

        [DllImport(@"CUDDHelper.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern IntPtr DD_Set3DMatrixElement(IntPtr ddman, IntPtr dd, IntPtr rvars, int num_rvars, IntPtr cvars, int num_cvars, IntPtr lvars, int num_lvars, int rindex, int cindex, int lindex, double value);

        [DllImport(@"CUDDHelper.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern double DD_GetVectorElement(IntPtr ddman, IntPtr dd, IntPtr vars, int num_vars, int x);

        [DllImport(@"CUDDHelper.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern IntPtr DD_Identity(IntPtr ddman, IntPtr rvars, IntPtr cvars, int num_vars);

        [DllImport(@"CUDDHelper.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern IntPtr DD_MatrixMultiply(IntPtr ddman, IntPtr dd1, IntPtr dd2, IntPtr vars, int num_vars, int method);

        [DllImport(@"CUDDHelper.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern IntPtr DD_Transpose(IntPtr ddman, IntPtr dd, IntPtr row_vars, IntPtr col_vars, int num_vars);

        [DllImport(@"CUDDHelper.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern void DD_PrintVector(IntPtr ddman, IntPtr dd, IntPtr vars, int num_vars, int accuracy);

        [DllImport(@"CUDDHelper.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern void DD_PrintMatrix(IntPtr ddman, IntPtr dd, IntPtr rvars, int num_rvars, IntPtr cvars, int num_cvars, int accuracy);

        [DllImport(@"CUDDHelper.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern void DD_PrintVectorFiltered(IntPtr ddman, IntPtr dd, IntPtr filter, IntPtr vars, int num_vars);

        [DllImport(@"CUDDHelper.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern IntPtr DD_Threshold(IntPtr ddman, IntPtr dd, double threshold);

        [DllImport(@"CUDDHelper.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern IntPtr DD_StrictThreshold(IntPtr ddman, IntPtr dd, double threshold);

        [DllImport(@"CUDDHelper.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern IntPtr DD_GreaterThan(IntPtr ddman, IntPtr dd, double threshold);

        [DllImport(@"CUDDHelper.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern IntPtr DD_GreaterThanEquals(IntPtr ddman, IntPtr dd, double threshold);

        [DllImport(@"CUDDHelper.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern IntPtr DD_LessThan(IntPtr ddman, IntPtr dd, double threshold);

        [DllImport(@"CUDDHelper.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern IntPtr DD_LessThanEquals(IntPtr ddman, IntPtr dd, double threshold);

        [DllImport(@"CUDDHelper.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern IntPtr DD_Equals(IntPtr ddman, IntPtr dd, double value);

        [DllImport(@"CUDDHelper.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern IntPtr DD_Interval(IntPtr ddman, IntPtr dd, double lower, double upper);

        [DllImport(@"CUDDHelper.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern IntPtr DD_RoundOff(IntPtr ddman, IntPtr dd, int places);

        [DllImport(@"CUDDHelper.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern double DD_FindMin(IntPtr ddman, IntPtr dd);

        [DllImport(@"CUDDHelper.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern double DD_FindMax(IntPtr ddman, IntPtr dd);

        [DllImport(@"CUDDHelper.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern IntPtr DD_RestrictToFirst(IntPtr ddman, IntPtr dd, IntPtr vars, int num_vars);

        [DllImport(@"CUDDHelper.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern IntPtr DD_PermuteVariables(IntPtr ddman, IntPtr dd, IntPtr old_vars, IntPtr new_vars, int num_vars);

        [DllImport(@"CUDDHelper.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern IntPtr DD_SwapVariables(IntPtr ddman, IntPtr dd, IntPtr old_vars, IntPtr new_vars, int num_vars);

        [DllImport(@"CUDDHelper.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern IntPtr DD_VariablesGreaterThan(IntPtr ddman, IntPtr x_vars, IntPtr y_vars, int num_vars);

        [DllImport(@"CUDDHelper.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern IntPtr DD_VariablesGreaterThanEquals(IntPtr ddman, IntPtr x_vars, IntPtr y_vars, int num_vars);

        [DllImport(@"CUDDHelper.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern IntPtr DD_VariablesLessThan(IntPtr ddman, IntPtr x_vars, IntPtr y_vars, int num_vars);

        [DllImport(@"CUDDHelper.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern IntPtr DD_VariablesLessThanEquals(IntPtr ddman, IntPtr x_vars, IntPtr y_vars, int num_vars);

        [DllImport(@"CUDDHelper.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern IntPtr DD_VariablesEquals(IntPtr ddman, IntPtr x_vars, IntPtr y_vars, int num_vars);

        [DllImport(@"CUDDHelper.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern uint Cudd_ReadMemoryInUse(IntPtr ddman);

        [DllImport(@"CUDDHelper.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern IntPtr DDV_AllocateIntArray(int numberOfElements);

        [DllImport(@"CUDDHelper.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern void DDV_SetElementIntArray(IntPtr intArray, int index, int value);

        [DllImport(@"CUDDHelper.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern void DDV_FreeIntArray(IntPtr intArray);

        [DllImport(@"CUDDHelper.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern int Cudd_ShuffleHeap(IntPtr ddman, IntPtr permutation);

        [DllImport(@"CUDDHelper.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern int Cudd_ReadReorderings (IntPtr ddman);

        [DllImport(@"CUDDHelper.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern int Cudd_ReadPerm(IntPtr ddman, int i);

        [DllImport(@"CUDDHelper.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern int Cudd_ReadInvPerm(IntPtr ddman, int i);

        [DllImport(@"CUDDHelper.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern int Cudd_ManuallyReorder(IntPtr ddman, int minsize);

        [DllImport(@"CUDDHelper.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern int Cudd_PrintBDDTree(IntPtr dd, IntPtr f, string fileName);

        [DllImport(@"CUDDHelper.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern int Cudd_EqualSupNorm(IntPtr dd, IntPtr f, IntPtr g, double tolerance, int pr);

        [DllImport(@"CUDDHelper.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int Cudd_EqualSupNormRel(IntPtr dd, IntPtr f, IntPtr g, double tolerance, int pr);
        [DllImport(@"odd.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr _Z9build_oddP9DdManagerP6DdNodePS2_i(IntPtr ddman, IntPtr dd, IntPtr vars, int num_vars);

        #endregion Import from C++
    }
}