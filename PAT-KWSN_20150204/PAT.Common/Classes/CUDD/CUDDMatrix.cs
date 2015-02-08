namespace PAT.Common.Classes.CUDDLib
{
    public partial class CUDD
    {
        /// <summary>
        /// Matrix in BDD
        /// </summary>
        public class Matrix
        {
            public const int CMU = 1;
            public const int BOULDER = 2;

            /// <summary>
            /// Set the value of i.th element in vector. i is allowed to be negative. -1 is the last element.
            /// Note that this is a function, not a procedure
            /// [ REFS: 'result', DEREFS: 'dd' ]
            /// </summary>
            public static CUDDNode SetVectorElement(CUDDNode dd, CUDDVars vars, int index, double value)
            {
                return new CUDDNode(PlatformInvoke.DD_SetVectorElement(manager, dd.Ptr, vars.GetArrayPointer(), vars.GetNumVars(), index, value));
            }

            /// <summary>
            /// Sets element in matrix dd and return value.
            /// Note that this is a function, not a procedure
            /// [ REFS: 'result', DEREFS: 'dd' ]
            /// </summary>
            public static CUDDNode SetMatrixElement(CUDDNode dd, CUDDVars rVars, CUDDVars cVars, int rIndex, int cIndex, double value)
            {
                return new CUDDNode(PlatformInvoke.DD_SetMatrixElement(manager, dd.Ptr, rVars.GetArrayPointer(), rVars.GetNumVars(), cVars.GetArrayPointer(), cVars.GetNumVars(), rIndex, cIndex, value));
            }

            /// <summary>
            /// // sets element in 3d matrix dd
            /// [ REFS: 'result', DEREFS: 'dd' ]
            /// </summary>
            public static CUDDNode Set3DMatrixElement(CUDDNode dd, CUDDVars rVars, CUDDVars cVars, CUDDVars lVars, int rIndex, int cIndex, int lIndex, double value)
            {
                return new CUDDNode(PlatformInvoke.DD_Set3DMatrixElement(manager, dd.Ptr, rVars.GetArrayPointer(), rVars.GetNumVars(), cVars.GetArrayPointer(), cVars.GetNumVars(), lVars.GetArrayPointer(), lVars.GetNumVars(), rIndex, cIndex, lIndex, value));
            }

            /// <summary>
            /// Get element in vector dd
            /// [ REFS: 'none', DEREFS: 'none' ]
            /// </summary>
            public static double GetVectorElement(CUDDNode dd, CUDDVars vars, int index)
            {
                return PlatformInvoke.DD_GetVectorElement(manager, dd.Ptr, vars.GetArrayPointer(), vars.GetNumVars(), index);
            }

            /// <summary>
            /// Generates 0-1 ADD for the function x = y
            /// where x, y are num_vars-bit numbers encoded by variables x_vars, y_vars
            /// [ REFS: 'result', DEREFS: 'none' ]
            /// </summary>
            public static CUDDNode Identity(CUDDVars rVars, CUDDVars cVars)
            {
                return new CUDDNode(PlatformInvoke.DD_Identity(manager, rVars.GetArrayPointer(), cVars.GetArrayPointer(), rVars.GetNumVars()));
            }

            /// <summary>
            /// Returns transpose of matrix dd
            /// [ REFS: 'result', DEREFS: 'dd' ]
            /// </summary>
            public static CUDDNode Transpose(CUDDNode dd, CUDDVars rVars, CUDDVars cVars)
            {
                return new CUDDNode(PlatformInvoke.DD_Transpose(manager, dd.Ptr, rVars.GetArrayPointer(), cVars.GetArrayPointer(), rVars.GetNumVars()));
            }

            /// <summary>
            /// Returns matrix multiplication of matrices dd1 and dd2
            /// [Calculates the product of two matrices represented as ADDs.]
            /// Description [Calculates the product of two matrices, A and B,
            /// represented as ADDs. This procedure implements the quasiring multiplication
            /// algorithm.  A is assumed to depend on variables x (rows) and z
            /// (columns).  B is assumed to depend on variables z (rows) and y
            /// (columns).  The product of A and B then depends on x (rows) and y
            /// (columns).  Only the z variables have to be explicitly identified;
            /// they are the "summation" variables.  Returns a pointer to the
            /// result if successful; NULL otherwise.]
            /// [ REFS: 'result', DEREFS: 'dd1, dd2' ]
            /// </summary>
            /// <param name="dd1"></param>
            /// <param name="dd2"></param>
            /// <param name="vars">variable are shared by both dd1, dd2. Normally dd1 row + column, dd2 only column</param>
            /// <param name="method"></param>
            /// <returns></returns>
            public static CUDDNode MatrixMultiply(CUDDNode dd1, CUDDNode dd2, CUDDVars vars, int method)
            {
                return new CUDDNode(PlatformInvoke.DD_MatrixMultiply(manager, dd1.Ptr, dd2.Ptr, vars.GetArrayPointer(), vars.GetNumVars(), method));
            }

            /// <summary>
            /// Return the rol-vars vector of the product of matrix and vector
            /// [ REFS: 'result', DEREFS: '' ]
            /// </summary>
            /// <param name="matrix">Matrix of rol * col vars</param>
            /// <param name="vector">Vector of rol vars</param>
            /// <returns></returns>
            public static CUDDNode MatrixMultiplyVector(CUDDNode matrix, CUDDNode vector, CUDDVars allRowVars, CUDDVars allColVars)
            {
                //Return matrix * vector
                CUDD.Ref(vector);
                CUDDNode temp = Variable.SwapVariables(vector, allRowVars, allColVars);

                CUDD.Ref(matrix);
                CUDDNode result = MatrixMultiply(matrix, temp, allColVars, BOULDER);

                return result;
            }
        }
    }
}
