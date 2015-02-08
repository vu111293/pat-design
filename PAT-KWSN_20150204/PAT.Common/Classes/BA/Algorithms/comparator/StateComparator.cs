using PAT.Common.Classes.BA.Algorithms.automata;

namespace PAT.Common.Classes.BA.Algorithms.comparator
{
    public class StateComparator : System.Collections.Generic.IComparer<FAState>
    {
        public int Compare(FAState arg0, FAState arg1)
        {
            if (arg0.ID < arg1.ID)
                return -1;
            else if (arg0.ID == arg1.ID)
                return 0;
            else
                return 1;
        }
    }
}