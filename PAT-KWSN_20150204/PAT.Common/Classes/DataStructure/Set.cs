using System.Collections.Generic;

namespace PAT.Common.Classes.DataStructure
{
    public class Set<T> : List<T>
    {
        public new virtual bool Add(T item)
        {
            foreach (T state in this)
            {
                if (state.ToString() == item.ToString())
                {
                    return false;
                }
            }

            base.Add(item);
            return true;
        }

        public bool SubsetOf(Set<T> set)
        {
            foreach (T element in this)
            {
                if(!set.Contains(element))
                {
                    return false;
                }
            }
            return true;
        }

        public static bool SubsetOf(List<string> set1, List<string> set2)
        {
            foreach (string element in set1)
            {
                if (!set2.Contains(element))
                {
                    return false;
                }
            }
            return true;            
        }
    }
}
