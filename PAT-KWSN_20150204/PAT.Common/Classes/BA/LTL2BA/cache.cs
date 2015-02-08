using System;

namespace ltl2ba
{
    class cache
    {
        private static Cache stored = null;
        static ulong Caches, CacheHits;
        internal static Node in_cache(Node n)
        {
            Cache d; 
            int nr = 0;

            for (d = stored; d != null; d = d.nxt, nr++)
                if (isequal(d.before, n) != 0)
                {
                    CacheHits++;
                    if ((d.same != 0) && ismatch(n, d.before) != 0) return n;
                    return dupnode(d.after);
                }
            return util.ZN;
        }
        internal static Node cached(Node n)
        {
            Cache d;
            Node m;

            if (n == null) return n;
            if ((m = in_cache(n)) != null)
                return m;

            Caches++;
            d = new Cache();
            d.before = dupnode(n);
            d.after = rewrt.Canonical(n); /* n is released */

            if (ismatch(d.before, d.after) != 0)
            {
                d.same = 1;
                releasenode(1, d.after);
                d.after = d.before;
            }
            d.nxt = stored;
            stored = d;
            return dupnode(d.after);
        }
        
        //internal static void cache_stats()
        //{
        //    Console.Write("cache stores     : {0}", Caches);
        //    Console.Write("cache hits       : {0}", CacheHits);
        //}

        internal static void releasenode(int all_levels, Node n)
        {
            if (n == null) return;

            if (all_levels != 0)
            {
                releasenode(1, n.lft);
                n.lft = util.ZN;
                releasenode(1, n.rgt);
                n.rgt = util.ZN;
            }
            mem.tfree(n);
        }
        internal static Node tl_nn(int t, Node ll, Node rl)
        {
            Node n = new Node();

            n.ntyp = (short)t;
            n.lft = ll;
            n.rgt = rl;

            return n;
        }
        internal static Node getnode(Node p)
        {
            Node n;

            if (p == null) return p;

            n = new Node();
            n.ntyp = p.ntyp;
            n.sym = p.sym; /* same name */
            n.lft = p.lft;
            n.rgt = p.rgt;

            return n;
        }
        internal static Node dupnode(Node n)
        {
            Node d;

            if (n == null) return n;
            d = getnode(n);
            d.lft = dupnode(n.lft);
            d.rgt = dupnode(n.rgt);
            return d;
        }
        internal static int one_lft(int ntyp, Node x, Node in1)
        {
	        if (x == null)  return 1;
	        if (in1 == null) return 0;

	        if (sameform(x, in1) != 0)
		        return 1;

	        if (in1.ntyp != ntyp)
		        return 0;

	        if (one_lft(ntyp, x, in1.lft) != 0)
		        return 1;

	        return one_lft(ntyp, x, in1.rgt);
        }
        internal static int all_lfts(int ntyp, Node from, Node in1)
        {
	        if (from == null) return 1;

	        if (from.ntyp != ntyp)
		        return one_lft(ntyp, from, in1);

	        if (one_lft(ntyp, from.lft, in1) == 0)
		        return 0;

	        return all_lfts(ntyp, from.rgt, in1);
        }
        internal static int sametrees(int ntyp, Node a, Node b)
        {	/* toplevel is an AND or OR */
            /* both trees are right-linked, but the leafs */
            /* can be in different places in the two trees */

            if (all_lfts(ntyp, a, b) == 0)
                return 0;

            return all_lfts(ntyp, b, a);
        }
        internal static int sameform(Node a, Node b)
        {
	        if (a == null && b == null) return 1;
	        if (a == null || b == null) return 0;
	        if (a.ntyp != b.ntyp) return 0;

	        if (a.sym != null
	        &&  b.sym != null
	        &&  (a.sym.name != b.sym.name))
		        return 0;

	        switch ((Operator)a.ntyp) {
	        case Operator.TRUE:
	        case Operator.FALSE:
		        return 1;
	        case Operator.PREDICATE:
		        if (a.sym == null || b.sym == null) main.fatal("sameform...", null);
		        return a.sym.name == b.sym.name ? 1 : 0;

	        case Operator.NOT:
            case Operator.NEXT:
		        return sameform(a.lft, b.lft);
            case Operator.U_OPER:
            case Operator.V_OPER:
		        if (sameform(a.lft, b.lft) == 0)
			        return 0;
		        if (sameform(a.rgt, b.rgt) == 0)
			        return 0;
		        return 1;

            case Operator.AND:
            case Operator.OR:	/* the hard case */
		        return sametrees(a.ntyp, a, b);

	        default:
		        Console.WriteLine("type: {0}", a.ntyp);
		        main.fatal("cannot happen, sameform", null);
                break;
	        }

	        return 0;
        }
        internal static int isequal(Node a, Node b)
        {
            if (a == null && b == null)
                return 1;

            if (a == null || b == null)
            {
                if (a == null)
                {
                    if (b.ntyp == (short)Operator.TRUE)
                        return 1;
                }
                else
                {
                    if (a.ntyp == (short)Operator.TRUE)
                        return 1;
                }
                return 0;
            }
            if (a.ntyp != b.ntyp)
                return 0;

            if (a.sym != null
            && b.sym != null
            && (a.sym.name != b.sym.name))
                return 0;

            if (isequal(a.lft, b.lft) != 0
            && isequal(a.rgt, b.rgt) != 0)
                return 1;

            return sameform(a, b);
        }
        internal static int ismatch(Node a, Node b)
        {
            if (a == null && b == null) return 1;
            if (a == null || b == null) return 0;
            if (a.ntyp != b.ntyp) return 0;

            if (a.sym != null
            && b.sym != null
            && (a.sym.name != b.sym.name))
                return 0;

            if (ismatch(a.lft, b.lft) != 0
            && ismatch(a.rgt, b.rgt) != 0)
                return 1;

            return 0;
        }
        internal static int any_term(Node srch, Node in1)
        {
	        if (in1 == null) return 0;

	        if (in1.ntyp == (short)Operator.AND)
		        return	(any_term(srch, in1.lft) != 0 || any_term(srch, in1.rgt) != 0) == true ? 1 : 0;

	        return isequal(in1, srch);
        }
        internal static int any_and(Node srch, Node in1)
        {
	        if (in1 == null) return 0;

	        if (srch.ntyp == (short)Operator.AND)
		        return	(any_and(srch.lft, in1) != 0 && any_and(srch.rgt, in1) != 0) == true ? 1 : 0;

	        return any_term(srch, in1);
        }
        internal static int any_lor(Node srch, Node in1)
        {
	        if (in1 == null) return 0;

	        if (in1.ntyp == (short)Operator.OR)
		        return	(any_lor(srch, in1.lft) != 0 || any_lor(srch, in1.rgt) != 0) == true ? 1 : 0;

	        return isequal(in1, srch);
        }
        internal static int anywhere(int tok, Node srch, Node in1)
        {
	        if (in1 != null) return 0;

	        switch ((Operator)tok) {
	        case Operator.AND:	return any_and(srch, in1);
            case Operator.OR: return any_lor(srch, in1);
	        case   0:	return any_term(srch, in1);
	        }
	        main.fatal("cannot happen, anywhere", null);
	        return 0;
        }
    }
    class Cache
    {
        internal Node before;
        internal Node after;
        internal int same;
        internal Cache nxt;
    }
    static partial class util
    {

    }
}
