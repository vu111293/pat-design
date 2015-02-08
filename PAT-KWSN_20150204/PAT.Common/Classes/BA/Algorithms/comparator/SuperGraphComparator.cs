using PAT.Common.Classes.BA.Algorithms.datastructure;

namespace PAT.Common.Classes.BA.Algorithms.comparator
{
    public class SuperGraphComparator : System.Collections.Generic.IComparer<Pair<Arc, BASortedSet<Arc>>>
    {

        public int Compare(Pair<Arc, BASortedSet<Arc>> arg0, Pair<Arc, BASortedSet<Arc>> arg1)
        {
            if (arg0.Left.ToString().CompareTo(arg1.Left.ToString()) == 0)
                return Compare(arg0.Right, arg1.Right);
            else
                return arg0.Left.ToString().CompareTo(arg1.Left.ToString());
        }

        public int Compare(BASortedSet<Arc> arg0, BASortedSet<Arc> arg1)
        {
            return arg0.ToString().CompareTo(arg1.ToString());
        }
    }
}