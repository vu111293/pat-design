namespace ltl2ba
{
    class rewrt
    {
        static Node can = util.ZN;

        internal static Node right_linked(Node n)
        {
            if (n == null) return n;

            if (n.ntyp == (short)Operator.AND || n.ntyp == (short)Operator.OR)
                while (n.lft != null && n.lft.ntyp == n.ntyp)
                {
                    Node tmp = n.lft;
                    n.lft = tmp.rgt;
                    tmp.rgt = n;
                    n = tmp;
                }

            n.lft = right_linked(n.lft);
            n.rgt = right_linked(n.rgt);

            return n;
        }
        internal static Node canonical(Node n)
        {
            Node m;	/* assumes input is right_linked */

            if (n == null) return n;
            if ((m = cache.in_cache(n)) != null)
                return m;

            n.rgt = canonical(n.rgt);
            n.lft = canonical(n.lft);

            return cache.cached(n);
        }
        internal static Node push_negation(Node n)
        {	
            Node m;

            //util.Assert(n.ntyp == (short)Operator.NOT, n.ntyp);
            System.Diagnostics.Debug.Assert(n.ntyp == (short)Operator.NOT);

	        switch (n.lft.ntyp) 
            {
	        case (short)Operator.TRUE:
		        cache.releasenode(0, n.lft);
		        n.lft = util.ZN;
                n.ntyp = (short)Operator.FALSE;
		        break;
            case (short)Operator.FALSE:
		        cache.releasenode(0, n.lft);
		        n.lft = util.ZN;
                n.ntyp = (short)Operator.TRUE;
		        break;
            case (short)Operator.NOT:
		        m = n.lft.lft;
		        cache.releasenode(0, n.lft);
		        n.lft = util.ZN;
		        cache.releasenode(0, n);
		        n = m;
		        break;
            case (short)Operator.V_OPER:
                n.ntyp = (short)Operator.U_OPER;
		        goto same;
            case (short)Operator.U_OPER:
                n.ntyp = (short)Operator.V_OPER;
		        goto same;
            case (short)Operator.NEXT:
                n.ntyp = (short)Operator.NEXT;
                n.lft.ntyp = (short)Operator.NOT;
		        n.lft = push_negation(n.lft);
		        break;
            case (short)Operator.AND:
                n.ntyp = (short)Operator.OR;
		        goto same;
            case (short)Operator.OR:
                n.ntyp = (short)Operator.AND;

        same:		m = n.lft.rgt;
		        n.lft.rgt = util.ZN;

		        n.rgt = util.Not(m);
                n.lft.ntyp = (short)Operator.NOT;
		        m = n.lft;
		        n.lft = push_negation(m);
		        break;
	        }

	        return util.rewrite(n);
        }
        internal static void addcan(int tok, Node n)
        {
            Node m, prev = util.ZN;
            //ValueWrapper<Node> ptr = new ValueWrapper<Node>();;
            Node ptr = null;
            Node N;
            Symbol s, t; int cmp;

            if (n == null) return;

            if (n.ntyp == tok)
            {
                addcan(tok, n.rgt);
                addcan(tok, n.lft);
                return;
            }
            //#if X0
            //            if ((tok == (short)Operator.AND && n.ntyp == (short)Operator.TRUE)
            //            ||  (tok == (short)Operator.OR  && n.ntyp == (short)Operator.FALSE))
            //                return;
            //#endif
            N = cache.dupnode(n);
            if (can == null)
            {
                can = N;
                return;
            }

            s = trans.DoDump(N);
            if (can.ntyp != tok)	/* only one element in list so far */
            {
                ptr = can;
                goto insert;
            }

            /* there are at least 2 elements in list */
            prev = util.ZN;
            for (m = can; m.ntyp == tok && m.rgt != null; prev = m, m = m.rgt)
            {
                t = trans.DoDump(m.lft);
                cmp = s.name.CompareTo(t.name);
                if (cmp == 0)	/* duplicate */
                    return;
                if (cmp < 0)
                {
                    if (prev == null)
                    {
                        can = cache.tl_nn(tok, N, can);
                        return;
                    }
                    else
                    {
                        ptr = prev.rgt;
                        goto insert;
                    }
                }
            }

            /* new entry goes at the end of the list */
            ptr = prev.rgt;
        insert:
            t = trans.DoDump(ptr);
            cmp = s.name.CompareTo(t.name);
            if (cmp == 0)	/* duplicate */
                return;
            if (cmp < 0)
            {
                //ptr.value = cache.tl_nn(tok, N, ptr);
                if (ptr == can)
                    can = cache.tl_nn(tok, N, ptr);
                else
                    prev.rgt = cache.tl_nn(tok, N, ptr);
            }
            else
            {
                //ptr.value = cache.tl_nn(tok, ptr, N);
                if (ptr == can)
                    can = cache.tl_nn(tok, ptr, N);
                else
                    prev.rgt = cache.tl_nn(tok, ptr, N);
            }
        }
        internal static void marknode(int tok, Node m)
        {
	        if (m.ntyp != tok)
	        {	
                cache.releasenode(0, m.rgt);
		        m.rgt = util.ZN;
	        }
	        m.ntyp = -1;
        }
        internal static Node Canonical(Node n)
        {	Node m, p, k1, k2, prev, dflt = util.ZN;
	        int tok;

	        if (n == null) return n;

	        tok = n.ntyp;
	        if (tok != (short)Operator.AND && tok != (short)Operator.OR)
		        return n;

	        can = util.ZN;
	        addcan(tok, n);
//#if X1
//            //Debug("\nA0: "); Dump(can); 
//            //Debug("\nA1: "); Dump(n); Debug("\n");
//#endif
	        cache.releasenode(1, n);

	        /* mark redundant nodes */
	        if (tok == (short)Operator.AND)
	        {	for (m = can; m != null; m = (m.ntyp == (short)Operator.AND) ? m.rgt : util.ZN)
		        {	k1 = (m.ntyp == (short)Operator.AND) ? m.lft : m;
			        if (k1.ntyp == (short)Operator.TRUE)
			        {	marknode((short)Operator.AND, m);
				        dflt = util.True;
				        continue;
			        }
			        if (k1.ntyp == (short)Operator.FALSE)
			        {	cache.releasenode(1, can);
				        can = util.False;
				        goto out1;
		        }	}
		        for (m = can; m != null; m = (m.ntyp == (short)Operator.AND) ? m.rgt : util.ZN)
		        for (p = can; p != null; p = (p.ntyp == (short)Operator.AND) ? p.rgt : util.ZN)
		        {	if (p == m
			        ||  p.ntyp == -1
			        ||  m.ntyp == -1)
				        continue;
			        k1 = (m.ntyp == (short)Operator.AND) ? m.lft : m;
			        k2 = (p.ntyp == (short)Operator.AND) ? p.lft : p;

			        if (cache.isequal(k1, k2) != 0)
			        {	marknode((short)Operator.AND, p);
				        continue;
			        }
			        if (cache.anywhere((short)Operator.OR, k1, k2) != 0)
			        {	marknode((short)Operator.AND, p);
				        continue;
			        }
			        if (k2.ntyp == (short)Operator.U_OPER
			        &&  cache.anywhere((short)Operator.AND, k2.rgt, can) != 0)
			        {	marknode((short)Operator.AND, p);
				        continue;
			        }	/* q && (p U q) = q */
	        }	}
	        if (tok == (short)Operator.OR)
	        {	for (m = can; m != null; m = (m.ntyp == (short)Operator.OR) ? m.rgt : util.ZN)
		        {	k1 = (m.ntyp == (short)Operator.OR) ? m.lft : m;
			        if (k1.ntyp == (short)Operator.FALSE)
			        {	marknode((short)Operator.OR, m);
				        dflt = util.False;
				        continue;
			        }
			        if (k1.ntyp == (short)Operator.TRUE)
			        {	cache.releasenode(1, can);
				        can = util.True;
				        goto out1;
		        }	}
		        for (m = can; m != null; m = (m.ntyp == (short)Operator.OR) ? m.rgt : util.ZN)
		        for (p = can; p != null; p = (p.ntyp == (short)Operator.OR) ? p.rgt : util.ZN)
		        {	if (p == m
			        ||  p.ntyp == -1
			        ||  m.ntyp == -1)
				        continue;
			        k1 = (m.ntyp == (short)Operator.OR) ? m.lft : m;
			        k2 = (p.ntyp == (short)Operator.OR) ? p.lft : p;

			        if (cache.isequal(k1, k2) != 0)
			        {	marknode((short)Operator.OR, p);
				        continue;
			        }
			        if (cache.anywhere((short)Operator.AND, k1, k2) != 0)
			        {	marknode((short)Operator.OR, p);
				        continue;
			        }
			        if (k2.ntyp == (short)Operator.V_OPER
			        &&  k2.lft.ntyp == (short)Operator.FALSE
			        &&  cache.anywhere((short)Operator.AND, k2.rgt, can) != 0)
			        {	marknode((short)Operator.OR, p);
				        continue;
			        }	/* p || (F V p) = p */
	        }	}
	        for (m = can, prev = util.ZN; m != null; )	/* remove marked nodes */
	        {	if (m.ntyp == -1)
		        {	k2 = m.rgt;
			        cache.releasenode(0, m);
			        if (prev == null)
			        {	m = can = can.rgt;
			        } else
			        {	m = prev.rgt = k2;
				        /* if deleted the last node in a chain */
				        if (prev.rgt == null && prev.lft != null
				        &&  (prev.ntyp == (short)Operator.AND || prev.ntyp == (short)Operator.OR))
				        {	k1 = prev.lft;
					        prev.ntyp = prev.lft.ntyp;
					        prev.sym = prev.lft.sym;
					        prev.rgt = prev.lft.rgt;
					        prev.lft = prev.lft.lft;
					        cache.releasenode(0, k1);
				        }
			        }
			        continue;
		        }
		        prev = m;
		        m = m.rgt;
	        }
        out1:
//#if X1
//            //Debug("A2: "); Dump(can); Debug("\n");
//#endif
	        if (can == null)
	        {	if (dflt == null)
			        main.fatal("cannot happen, Canonical", null);
		        return dflt;
	        }

	        return can;
        }
    }
}
