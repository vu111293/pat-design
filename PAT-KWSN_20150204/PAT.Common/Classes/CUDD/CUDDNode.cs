using System;

namespace PAT.Common.Classes.CUDDLib
{
    /// <summary>
    /// Wrapper of a boolean expression
    /// </summary>
    public class CUDDNode
    {
        public IntPtr Ptr;

        public CUDDNode(IntPtr p)
        {
            this.Ptr = p;
        }

        public CUDDNode(CUDDNode dd)
        {
            this.Ptr = dd.Ptr;
        }

        /// <summary>
        /// Check whether the represented boolean expression is actually a constant
        /// </summary>
        /// <returns></returns>
        public bool IsConstant()
        {
            return CUDD.IsConstant(this);
        }

        public int GetIndex()
        {
            return CUDD.GetIndex(this);
        }

        public double GetValue()
        {
            return CUDD.GetValue(this);
        }

        /// <summary>
        /// [ REFS: 'result', DEREFS: 'this']
        /// </summary>
        /// <returns></returns>
        public CUDDNode GetThen()
        {
            CUDDNode result = CUDD.GetThen(this);
            CUDD.Ref(result);
            CUDD.Deref(this);
            return result;
        }

        /// <summary>
        /// [ REFS: 'result', DEREFS: 'this']
        /// </summary>
        /// <returns></returns>
        public CUDDNode GetElse()
        {
            CUDDNode result = CUDD.GetElse(this);
            CUDD.Ref(result);
            CUDD.Deref(this);
            return result;
        }

        /// <summary>
        /// Use to check 2 CUDDNode are the same. Often used to check a node is a Zero or One
        /// [ REFS: 'none', DEREFS: 'none']
        /// </summary>
        public override bool Equals(object obj)
        {
            CUDDNode dd = obj as CUDDNode;
            return (this.Ptr == dd.Ptr);
        }

        public override int GetHashCode()
        {
            return (int)this.Ptr;
        }

        public static bool operator ==(CUDDNode a, CUDDNode b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(CUDDNode a, CUDDNode b)
        {
            return !(a.Equals(b));
        }

        public void Print()
        {
            CUDD.Print.PrintMinterm(this);
        }

        public bool IsZero()
        {
            return this.Ptr == IntPtr.Zero;
        }
    }
}