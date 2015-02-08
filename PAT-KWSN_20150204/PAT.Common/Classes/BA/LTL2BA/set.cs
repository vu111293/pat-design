namespace ltl2ba
{
    class set
    {
        
        //* extern char **sym_table;
        static int mod = 8 * sizeof(int);

        internal static int set_size(int t)
        {
            return t == 1 ? alternating.sym_size : (t == 2 ? generalized.scc_size : alternating.node_size);
        }
        internal static int[] new_set(int type) /* creates a new set */
        {
            return new int[set_size(type)];
        }
        internal static int[] clear_set(int[] l, int type) /* clears the set */
        {
            int i;
            for (i = 0; i < set_size(type); i++)
            {
                l[i] = 0;
            }
            return l;
        }
        internal static int[] make_set(int n, int type) /* creates the set {n}, or the empty set if n = -1 */
        {
            int[] l = clear_set(new_set(type), type);
            if (n == -1) return l;
            l[n / mod] = 1 << (n % mod);
            return l;
        }
        internal static void copy_set(int[] from, int[] to, int type) /* copies a set */
        {
            int i;
            for (i = 0; i < set_size(type); i++)
                to[i] = from[i];
        }
        internal static int[] dup_set(int[] l, int type) /* duplicates a set */
        {
            int i;
            int[] m = new_set(type);
            for(i = 0; i < set_size(type); i++)
                m[i] = l[i];
             return m;
        }
        internal static void merge_sets(int[] l1, int[] l2, int type) /* puts the union of the two sets in l1 */
        {
            int i;
            for (i = 0; i < set_size(type); i++)
                l1[i] = l1[i] | l2[i];
        }
        internal static void do_merge_sets(int[] l, int[] l1, int[] l2, int type) /* makes the union of two sets */
        {
            int i;
            for (i = 0; i < set_size(type); i++)
                l[i] = l1[i] | l2[i];
        }
        internal static int[] intersect_sets(int[] l1, int[] l2, int type) /* makes the intersection of two sets */
        {
            int i;
            int[] l = new_set(type);
            for(i = 0; i < set_size(type); i++)
                l[i] = l1[i] & l2[i];
            return l;
        }
        internal static int empty_intersect_sets(int[] l1, int[] l2, int type) /* tests intersection of two sets */
        {
            int i, test = 0;
            for (i = 0; i < set_size(type); i++)
                test |= l1[i] & l2[i];
            return test == 0 ? 1 : 0;
        }
        internal static void add_set(int[] l, int n) /* adds an element to a set */
        {
            l[n / mod] |= 1 << (n % mod);
        }
        internal static void rem_set(int[] l, int n) /* removes an element from a set */
        {
            l[n / mod] &= (-1 - (1 << (n % mod)));
        }
        internal static int empty_set(int[] l, int type) /* tests if a set is the empty set */
        {
            int i, test = 0;
            for (i = 0; i < set_size(type); i++)
                test |= l[i];
            return test == 0 ? 1 : 0;
        }
        internal static int same_sets(int[] l1, int[] l2, int type) /* tests if two sets are identical */
        {
            int i, test = 1;
            for (i = 0; i < set_size(type); i++)
                test &= ((l1[i] == l2[i]) ? 1 : 0);
            return test;
        }
        internal static int included_set(int[] l1, int[] l2, int type)
        {                    /* tests if the first set is included in the second one */
            int i, test = 0;
            for (i = 0; i < set_size(type); i++)
                test |= (l1[i] & ~l2[i]);
            return test == 0 ? 1 : 0;
        }
        internal static int in_set(int[] l, int n) /* tests if an element is in a set */
        {
            return (l[n / mod] & (1 << (n % mod)));
        }
        internal static int[] list_set(int[] l, int type) /* transforms a set into a list */
        {
            int i, j, size = 1; int[] list;
            for(i = 0; i < set_size(type); i++)
                for(j = 0; j < mod; j++) 
                    if((l[i] & (1 << j)) != 0)
	                    size++;
            list = new int[size];
            list[0] = size;
            size = 1;
            for(i = 0; i < set_size(type); i++)
                for(j = 0; j < mod; j++) 
                    if((l[i] & (1 << j)) != 0)
	                    list[size++] = mod * i + j;
            return list;
        }
    }
}
