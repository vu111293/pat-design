using System;
using System.Collections.Generic;
using PAT.Common.Classes.BA.Algorithms.automata;
using PAT.Common.Classes.BA.Algorithms.datastructure;

namespace PAT.Common.Classes.BA.Algorithms.comparator
{
    public class StringSetPairComparator : System.Collections.Generic.IComparer<Pair<string, BASortedSet<FAState>>>
    {
        public int Compare(Pair<String, BASortedSet<FAState>> s1, Pair<String, BASortedSet<FAState>> s2)
        {
            if (s1.Left.CompareTo(s2.Left) != 0)
            {
                return s1.Left.CompareTo(s2.Left);
            }
            else
            {
                return compare(s1.Right, s2.Right);
            }
        }

        public int compare(BASortedSet<FAState> s1, BASortedSet<FAState> s2)
        {
            if (s1.Count > s2.Count)
            {
                return 1;
            }
            else if (s1.Count < s2.Count)
            {
                return -1;
            }
            else
            {

                SortedDictionary<FAState, bool>.Enumerator iter1 = s1.GetEnumerator();
                SortedDictionary<FAState, bool>.Enumerator iter2 = s2.GetEnumerator();

                while (iter1.MoveNext())
                {
                    iter2.MoveNext();

                    FAState st1 = iter1.Current.Key;
                    FAState st2 = iter2.Current.Key;
                    if (st1.ID > st2.ID)
                    {
                        return 1;
                    }
                    else if (st1.ID < st2.ID)
                    {
                        return -1;
                    }
                }
                return 0;
            }
        }
    }
}