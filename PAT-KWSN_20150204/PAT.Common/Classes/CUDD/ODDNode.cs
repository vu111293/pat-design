using System;

namespace PAT.Common.Classes.CUDDLib
{
    public class ODDNode
    {
        private IntPtr ptr;

        public ODDNode(IntPtr p)
        {
            ptr = p;
        }

        public ODDNode(ODDNode odd)
        {
            ptr = odd.ptr;
        }

        /// <summary>
        /// Use to check 2 ODDNode are the same. Often used to check a node is a Zero or One
        /// [ REFS: 'none', DEREFS: 'none']
        /// </summary>
        public override bool Equals(object obj)
        {
            ODDNode dd = obj as ODDNode;
            return (this.ptr == dd.ptr);
        }

        public override int GetHashCode()
        {
            return (int)this.ptr;
        }

        public static bool operator ==(ODDNode a, ODDNode b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(ODDNode a, ODDNode b)
        {
            return !(a.Equals(b));
        }


        public bool IsZero()
        {
            return this.ptr == IntPtr.Zero;
        }

        public String toString()
        {
            return "" + ptr;
        }
    }
}
