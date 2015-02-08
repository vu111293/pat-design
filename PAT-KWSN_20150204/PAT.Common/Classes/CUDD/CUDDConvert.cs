namespace PAT.Common.Classes.CUDDLib
{
    public partial class CUDD
    {
        /// <summary>
        /// Convert all kinds of conversion from ADD to BDD
        /// </summary>
        public class Convert
        {
            /// <summary>
            /// Convert ADD to 0-1 ADD based on the interval (threshold, +inf)
            /// [ REFS: 'result', DEREFS: dd ]
            /// </summary>
            public static CUDDNode GreaterThan(CUDDNode dd, double threshold)
            {
                return new CUDDNode(PlatformInvoke.DD_GreaterThan(manager, dd.Ptr, threshold));
            }

            /// <summary>
            /// Convert ADD to 0-1 ADD based on the interval [threshold, +inf)
            /// [ REFS: 'result', DEREFS: dd ]
            /// </summary>
            public static CUDDNode GreaterThanEquals(CUDDNode dd, double threshold)
            {
                return new CUDDNode(PlatformInvoke.DD_GreaterThanEquals(manager, dd.Ptr, threshold));
            }

            /// <summary>
            /// Convert ADD to 0-1 ADD based on the interval (-inf, threshold)
            /// [ REFS: 'result', DEREFS: dd ]
            /// </summary>
            public static CUDDNode LessThan(CUDDNode dd, double threshold)
            {
                return new CUDDNode(PlatformInvoke.DD_LessThan(manager, dd.Ptr, threshold));
            }

            /// <summary>
            /// Convert ADD to 0-1 ADD based on the interval (-inf, threshold]
            /// [ REFS: 'result', DEREFS: dd ]
            /// </summary>
            public static CUDDNode LessThanEquals(CUDDNode dd, double threshold)
            {
                return new CUDDNode(PlatformInvoke.DD_LessThanEquals(manager, dd.Ptr, threshold));
            }

            /// <summary>
            /// Convert ADD to 0-1 ADD based on the interval [threshold, threshold]
            /// [ REFS: 'result', DEREFS: dd ]
            /// </summary>
            public static CUDDNode Equals(CUDDNode dd, double threshold)
            {
                return new CUDDNode(PlatformInvoke.DD_Equals(manager, dd.Ptr, threshold));
            }

            /// <summary>
            /// Convert ADD to 0-1 ADD based on the interval [lower, upper]
            /// [ REFS: 'result', DEREFS: dd ]
            /// </summary>
            public static CUDDNode Interval(CUDDNode dd, double lower, double upper)
            {
                return new CUDDNode(PlatformInvoke.DD_Interval(manager, dd.Ptr, lower, upper));
            }
        }
    }
}
