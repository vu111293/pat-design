namespace ltl2ba
{
    class parse
    {
        internal static int tl_yychar = 0;
        internal static Node tl_yylval;
        static int[,] prec = {
	                                { (int)Operator.U_OPER,  (int)Operator.V_OPER, 0, 0},  /* left associative */
	                                { (int)Operator.OR, (int)Operator.AND, (int)Operator.IMPLIES, (int)Operator.EQUIV, },	/* left associative */
                                 };
        internal static int implies(Node a, Node b)
        {
            return
              (cache.isequal(a, b) != 0 ||
               b.ntyp == (short)Operator.TRUE ||
               a.ntyp == (short)Operator.FALSE ||
               (b.ntyp == (short)Operator.AND && implies(a, b.lft) != 0 && implies(a, b.rgt) != 0) ||
               (a.ntyp == (short)Operator.OR && implies(a.lft, b) != 0 && implies(a.rgt, b) != 0) ||
               (a.ntyp == (short)Operator.AND && (implies(a.lft, b) != 0 || implies(a.rgt, b) != 0)) ||
               (b.ntyp == (short)Operator.OR && (implies(a, b.lft) != 0 || implies(a, b.rgt) != 0)) ||
               (b.ntyp == (short)Operator.U_OPER && implies(a, b.rgt) != 0) ||
               (a.ntyp == (short)Operator.V_OPER && implies(a.rgt, b) != 0) ||
               (a.ntyp == (short)Operator.U_OPER && implies(a.lft, b) != 0 && implies(a.rgt, b) != 0) ||
               (b.ntyp == (short)Operator.V_OPER && implies(a, b.lft) != 0 && implies(a, b.rgt) != 0) ||
               ((a.ntyp == (short)Operator.U_OPER || a.ntyp == (short)Operator.V_OPER) && a.ntyp == b.ntyp && implies(a.lft, b.lft) != 0 && implies(a.rgt, b.rgt) != 0)) ? 1 : 0;
        }
        internal static Node bin_simpler(Node ptr)
        {	Node a, b;

	        if (ptr != null)
	        switch (ptr.ntyp) {
	        case (short)Operator.U_OPER:
		        if (ptr.rgt.ntyp == (short)Operator.TRUE
		        ||  ptr.rgt.ntyp == (short)Operator.FALSE
		        ||  ptr.lft.ntyp == (short)Operator.FALSE)
		        {	ptr = ptr.rgt;
			        break;
		        }
		        if (implies(ptr.lft, ptr.rgt) != 0) /* NEW */
		        {	ptr = ptr.rgt;
		                break;
		        }
		        if (ptr.lft.ntyp == (short)Operator.U_OPER
		        &&  cache.isequal(ptr.lft.lft, ptr.rgt) != 0)
		        {	/* (p U q) U p = (q U p) */
			        ptr.lft = ptr.lft.rgt;
			        break;
		        }
		        if (ptr.rgt.ntyp == (short)Operator.U_OPER
		        &&  implies(ptr.lft, ptr.rgt.lft) != 0)
		        {	/* NEW */
			        ptr = ptr.rgt;
			        break;
		        }

		        /* X p U X q == X (p U q) */
		        if (ptr.rgt.ntyp == (short)Operator.NEXT
		        &&  ptr.lft.ntyp == (short)Operator.NEXT)
		        {	ptr = cache.tl_nn((short)Operator.NEXT,
				        cache.tl_nn((short)Operator.U_OPER,
					        ptr.lft.lft,
					        ptr.rgt.lft), util.ZN);
		                break;
		        }

		        /* NEW : F X p == X F p */
		        if (ptr.lft.ntyp == (short)Operator.TRUE &&
		            ptr.rgt.ntyp == (short)Operator.NEXT) {
		          ptr = cache.tl_nn((short)Operator.NEXT, cache.tl_nn((short)Operator.U_OPER, util.True, ptr.rgt.lft), util.ZN);
		          break;
		        }

		        /* NEW : F G F p == G F p */
		        if (ptr.lft.ntyp == (short)Operator.TRUE &&
		            ptr.rgt.ntyp == (short)Operator.V_OPER &&
		            ptr.rgt.lft.ntyp == (short)Operator.FALSE &&
		            ptr.rgt.rgt.ntyp == (short)Operator.U_OPER &&
		            ptr.rgt.rgt.lft.ntyp == (short)Operator.TRUE) {
		          ptr = ptr.rgt;
		          break;
		        }

		        /* NEW */
		        if (ptr.lft.ntyp != (short)Operator.TRUE && 
		            implies(rewrt.push_negation(cache.tl_nn((short)Operator.NOT, cache.dupnode(ptr.rgt), util.ZN)), 
			            ptr.lft) != 0)
		        {       ptr.lft = util.True;
		                break;
		        }
		        break;
	        case (short)Operator.V_OPER:
		        if (ptr.rgt.ntyp == (short)Operator.FALSE
		        ||  ptr.rgt.ntyp == (short)Operator.TRUE
		        ||  ptr.lft.ntyp == (short)Operator.TRUE)
		        {	ptr = ptr.rgt;
			        break;
		        }
		        if (implies(ptr.rgt, ptr.lft) != 0)
		        {	/* p V p = p */	
			        ptr = ptr.rgt;
			        break;
		        }
		        /* F V (p V q) == F V q */
		        if (ptr.lft.ntyp == (short)Operator.FALSE
		        &&  ptr.rgt.ntyp == (short)Operator.V_OPER)
		        {	ptr.rgt = ptr.rgt.rgt;
			        break;
		        }

		        /* NEW : G X p == X G p */
		        if (ptr.lft.ntyp == (short)Operator.FALSE &&
		            ptr.rgt.ntyp == (short)Operator.NEXT) {
		          ptr = cache.tl_nn((short)Operator.NEXT, cache.tl_nn((short)Operator.V_OPER, util.False, ptr.rgt.lft), util.ZN);
		          break;
		        }

		        /* NEW : G F G p == F G p */
		        if (ptr.lft.ntyp == (short)Operator.FALSE &&
		            ptr.rgt.ntyp == (short)Operator.U_OPER &&
		            ptr.rgt.lft.ntyp == (short)Operator.TRUE &&
		            ptr.rgt.rgt.ntyp == (short)Operator.V_OPER &&
		            ptr.rgt.rgt.lft.ntyp == (short)Operator.FALSE) {
		          ptr = ptr.rgt;
		          break;
		        }

		        /* NEW */
		        if (ptr.rgt.ntyp == (short)Operator.V_OPER
		        &&  implies(ptr.rgt.lft, ptr.lft) != 0)
		        {	ptr = ptr.rgt;
			        break;
		        }

		        /* NEW */
		        if (ptr.lft.ntyp != (short)Operator.FALSE && 
		            implies(ptr.lft, 
			            rewrt.push_negation(cache.tl_nn((short)Operator.NOT, cache.dupnode(ptr.rgt), util.ZN))) != 0)
		        {       ptr.lft = util.False;
		                break;
		        }
		        break;

	        case (short)Operator.NEXT:
		        /* NEW : X G F p == G F p */
		        if (ptr.lft.ntyp == (short)Operator.V_OPER &&
		            ptr.lft.lft.ntyp == (short)Operator.FALSE &&
		            ptr.lft.rgt.ntyp == (short)Operator.U_OPER &&
		            ptr.lft.rgt.lft.ntyp == (short)Operator.TRUE) {
		          ptr = ptr.lft;
		          break;
		        }
		        /* NEW : X F G p == F G p */
		        if (ptr.lft.ntyp == (short)Operator.U_OPER &&
		            ptr.lft.lft.ntyp == (short)Operator.TRUE &&
		            ptr.lft.rgt.ntyp == (short)Operator.V_OPER &&
		            ptr.lft.rgt.lft.ntyp == (short)Operator.FALSE) {
		          ptr = ptr.lft;
		          break;
		        }
		        break;
	        case (short)Operator.IMPLIES:
		        if (implies(ptr.lft, ptr.rgt) != 0)
		          {	ptr = util.True;
			        break;
		        }
		        ptr = cache.tl_nn((short)Operator.OR, util.Not(ptr.lft), ptr.rgt);
		        ptr = util.rewrite(ptr);
		        break;
	        case (short)Operator.EQUIV:
		        if (implies(ptr.lft, ptr.rgt) != 0 &&
		            implies(ptr.rgt, ptr.lft) != 0)
		          {	ptr = util.True;
			        break;
		        }
		        a = util.rewrite(cache.tl_nn((short)Operator.AND,
			        cache.dupnode(ptr.lft),
			        cache.dupnode(ptr.rgt)));
		        b = util.rewrite(cache.tl_nn((short)Operator.AND,
			        util.Not(ptr.lft),
			        util.Not(ptr.rgt)));
		        ptr = cache.tl_nn((short)Operator.OR, a, b);
		        ptr = util.rewrite(ptr);
		        break;
	        case (short)Operator.AND:
		        /* p && (q U p) = p */
		        if (ptr.rgt.ntyp == (short)Operator.U_OPER
		        &&  cache.isequal(ptr.rgt.rgt, ptr.lft) != 0)
		        {	ptr = ptr.lft;
			        break;
		        }
		        if (ptr.lft.ntyp == (short)Operator.U_OPER
		        &&  cache.isequal(ptr.lft.rgt, ptr.rgt) != 0)
		        {	ptr = ptr.rgt;
			        break;
		        }

		        /* p && (q V p) == q V p */
		        if (ptr.rgt.ntyp == (short)Operator.V_OPER
		        &&  cache.isequal(ptr.rgt.rgt, ptr.lft) != 0)
		        {	ptr = ptr.rgt;
			        break;
		        }
		        if (ptr.lft.ntyp == (short)Operator.V_OPER
		        &&  cache.isequal(ptr.lft.rgt, ptr.rgt) != 0)
		        {	ptr = ptr.lft;
			        break;
		        }

		        /* (p U q) && (r U q) = (p && r) U q*/
		        if (ptr.rgt.ntyp == (short)Operator.U_OPER
		        &&  ptr.lft.ntyp == (short)Operator.U_OPER
		        &&  cache.isequal(ptr.rgt.rgt, ptr.lft.rgt) != 0)
		        {	ptr = cache.tl_nn((short)Operator.U_OPER,
				        cache.tl_nn((short)Operator.AND, ptr.lft.lft, ptr.rgt.lft),
				        ptr.lft.rgt);
			        break;
		        }

		        /* (p V q) && (p V r) = p V (q && r) */
		        if (ptr.rgt.ntyp == (short)Operator.V_OPER
		        &&  ptr.lft.ntyp == (short)Operator.V_OPER
		        &&  cache.isequal(ptr.rgt.lft, ptr.lft.lft) != 0)
		        {	ptr = cache.tl_nn((short)Operator.V_OPER,
				        ptr.rgt.lft,
				        cache.tl_nn((short)Operator.AND, ptr.lft.rgt, ptr.rgt.rgt));
			        break;
		        }

		        /* X p && X q == X (p && q) */
		        if (ptr.rgt.ntyp == (short)Operator.NEXT
		        &&  ptr.lft.ntyp == (short)Operator.NEXT)
		        {	ptr = cache.tl_nn((short)Operator.NEXT,
                        cache.tl_nn((short)Operator.AND,
					        ptr.rgt.lft,
					        ptr.lft.lft), util.ZN);
			        break;
		        }

		        /* (p V q) && (r U q) == p V q */
		        if (ptr.rgt.ntyp == (short)Operator.U_OPER
		        &&  ptr.lft.ntyp == (short)Operator.V_OPER
                && cache.isequal(ptr.lft.rgt, ptr.rgt.rgt) != 0)
		        {	ptr = ptr.lft;
			        break;
		        }

                if (cache.isequal(ptr.lft, ptr.rgt) != 0	/* (p && p) == p */
		        ||  ptr.rgt.ntyp == (short)Operator.FALSE	/* (p && F) == F */
		        ||  ptr.lft.ntyp == (short)Operator.TRUE	/* (T && p) == p */
		        ||  implies(ptr.rgt, ptr.lft) != 0)/* NEW */
		        {	ptr = ptr.rgt;
			        break;
		        }	
		        if (ptr.rgt.ntyp == (short)Operator.TRUE	/* (p && T) == p */
		        ||  ptr.lft.ntyp == (short)Operator.FALSE	/* (F && p) == F */
		        ||  implies(ptr.lft, ptr.rgt) != 0)/* NEW */
		        {	ptr = ptr.lft;
			        break;
		        }
        		
		        /* NEW : F G p && F G q == F G (p && q) */
		        if (ptr.lft.ntyp == (short)Operator.U_OPER &&
		            ptr.lft.lft.ntyp == (short)Operator.TRUE &&
		            ptr.lft.rgt.ntyp == (short)Operator.V_OPER &&
		            ptr.lft.rgt.lft.ntyp == (short)Operator.FALSE &&
		            ptr.rgt.ntyp == (short)Operator.U_OPER &&
		            ptr.rgt.lft.ntyp == (short)Operator.TRUE &&
		            ptr.rgt.rgt.ntyp == (short)Operator.V_OPER &&
		            ptr.rgt.rgt.lft.ntyp == (short)Operator.FALSE)
		          {
                      ptr = cache.tl_nn((short)Operator.U_OPER, util.True,
                        cache.tl_nn((short)Operator.V_OPER, util.False,
                              cache.tl_nn((short)Operator.AND, ptr.lft.rgt.rgt,
					            ptr.rgt.rgt.rgt)));
		            break;
		          }

		        /* NEW */
		        if (implies(ptr.lft,
                        rewrt.push_negation(cache.tl_nn((short)Operator.NOT, cache.dupnode(ptr.rgt), util.ZN))) != 0
		         || implies(ptr.rgt,
                        rewrt.push_negation(cache.tl_nn((short)Operator.NOT, cache.dupnode(ptr.lft), util.ZN))) != 0)
		        {       ptr = util.False;
		                break;
		        }
		        break;

	        case (short)Operator.OR:
		        /* p || (q U p) == q U p */
		        if (ptr.rgt.ntyp == (short)Operator.U_OPER
                && cache.isequal(ptr.rgt.rgt, ptr.lft) != 0)
		        {	ptr = ptr.rgt;
			        break;
		        }

		        /* p || (q V p) == p */
		        if (ptr.rgt.ntyp == (short)Operator.V_OPER
                && cache.isequal(ptr.rgt.rgt, ptr.lft) != 0)
		        {	ptr = ptr.lft;
			        break;
		        }

		        /* (p U q) || (p U r) = p U (q || r) */
		        if (ptr.rgt.ntyp == (short)Operator.U_OPER
		        &&  ptr.lft.ntyp == (short)Operator.U_OPER
                && cache.isequal(ptr.rgt.lft, ptr.lft.lft) != 0)
		        {	ptr = cache.tl_nn((short)Operator.U_OPER,
				        ptr.rgt.lft,
                        cache.tl_nn((short)Operator.OR, ptr.lft.rgt, ptr.rgt.rgt));
			        break;
		        }

                if (cache.isequal(ptr.lft, ptr.rgt) != 0	/* (p || p) == p */
		        ||  ptr.rgt.ntyp == (short)Operator.FALSE	/* (p || F) == p */
		        ||  ptr.lft.ntyp == (short)Operator.TRUE	/* (T || p) == T */
		        ||  implies(ptr.rgt, ptr.lft) != 0)/* NEW */
		        {	ptr = ptr.lft;
			        break;
		        }	
		        if (ptr.rgt.ntyp == (short)Operator.TRUE	/* (p || T) == T */
		        ||  ptr.lft.ntyp == (short)Operator.FALSE	/* (F || p) == p */
		        ||  implies(ptr.lft, ptr.rgt) != 0)/* NEW */
		        {	ptr = ptr.rgt;
			        break;
		        }

		        /* (p V q) || (r V q) = (p || r) V q */
		        if (ptr.rgt.ntyp == (short)Operator.V_OPER
		        &&  ptr.lft.ntyp == (short)Operator.V_OPER
                && cache.isequal(ptr.lft.rgt, ptr.rgt.rgt) != 0)
                {
                    ptr = cache.tl_nn((short)Operator.V_OPER,
                        cache.tl_nn((short)Operator.OR, ptr.lft.lft, ptr.rgt.lft),
				        ptr.rgt.rgt);
			        break;
		        }

		        /* (p V q) || (r U q) == r U q */
		        if (ptr.rgt.ntyp == (short)Operator.U_OPER
		        &&  ptr.lft.ntyp == (short)Operator.V_OPER
                && cache.isequal(ptr.lft.rgt, ptr.rgt.rgt) != 0)
		        {	ptr = ptr.rgt;
			        break;
		        }		
        		
		        /* NEW : G F p || G F q == G F (p || q) */
		        if (ptr.lft.ntyp == (short)Operator.V_OPER &&
		            ptr.lft.lft.ntyp == (short)Operator.FALSE &&
		            ptr.lft.rgt.ntyp == (short)Operator.U_OPER &&
		            ptr.lft.rgt.lft.ntyp == (short)Operator.TRUE &&
		            ptr.rgt.ntyp == (short)Operator.V_OPER &&
		            ptr.rgt.lft.ntyp == (short)Operator.FALSE &&
		            ptr.rgt.rgt.ntyp == (short)Operator.U_OPER &&
		            ptr.rgt.rgt.lft.ntyp == (short)Operator.TRUE)
		          {
                      ptr = cache.tl_nn((short)Operator.V_OPER, util.False,
                        cache.tl_nn((short)Operator.U_OPER, util.True,
                              cache.tl_nn((short)Operator.OR, ptr.lft.rgt.rgt,
					            ptr.rgt.rgt.rgt)));
		            break;
		          }

		        /* NEW */
                if (implies(rewrt.push_negation(cache.tl_nn((short)Operator.NOT, cache.dupnode(ptr.rgt), util.ZN)),
			            ptr.lft) != 0
                 || implies(rewrt.push_negation(cache.tl_nn((short)Operator.NOT, cache.dupnode(ptr.lft), util.ZN)),
			            ptr.rgt) != 0)
		        {       ptr = util.True;
		                break;
		        }
		        break;
	        }
	        return ptr;
        }
        internal static Node bin_minimal(Node ptr)
        {       if (ptr != null)
	        switch (ptr.ntyp) {
	        case (short)Operator.IMPLIES:
		        return cache.tl_nn((short)Operator.OR, util.Not(ptr.lft), ptr.rgt);
	        case (short)Operator.EQUIV:
		        return cache.tl_nn((short)Operator.OR, 
			             cache.tl_nn((short)Operator.AND,cache.dupnode(ptr.lft),cache.dupnode(ptr.rgt)),
			             cache.tl_nn((short)Operator.AND,util.Not(ptr.lft),util.Not(ptr.rgt)));
	        }
	        return ptr;
        }
        internal static Node tl_factor()
        {	
            Node ptr = util.ZN;

	        switch (tl_yychar) {
	        case '(':
		        ptr = tl_formula();
		        if (tl_yychar != ')')
			        main.tl_yyerror("expected ')'");
		        tl_yychar = lex.tl_yylex();
		        goto simpl;
	        case (short)Operator.NOT:
		        ptr = tl_yylval;
		        tl_yychar = lex.tl_yylex();
		        ptr.lft = tl_factor();
		        ptr = rewrt.push_negation(ptr);
		        goto simpl;
	        case (short)Operator.ALWAYS:
		        tl_yychar = lex.tl_yylex();

		        ptr = tl_factor();

		        if(main.tl_simp_log != 0) {
		          if (ptr.ntyp == (short)Operator.FALSE
		              ||  ptr.ntyp == (short)Operator.TRUE)
		            break;	/* [] false == false */
        		  
		          if (ptr.ntyp == (short)Operator.V_OPER)
		            {	if (ptr.lft.ntyp == (short)Operator.FALSE)
		              break;	/* [][]p = []p */
        		    
		            ptr = ptr.rgt;	/* [] (p V q) = [] q */
		            }
		        }

		        ptr = cache.tl_nn((short)Operator.V_OPER, util.False, ptr);
		        goto simpl;
       
	        case (short)Operator.NEXT:
		        tl_yychar = lex.tl_yylex();

		        ptr = tl_factor();

		        if ((ptr.ntyp == (short)Operator.TRUE || ptr.ntyp == (short)Operator.FALSE)&& main.tl_simp_log != 0)
			        break;	/* X true = true , X false = false */

		        ptr = cache.tl_nn((short)Operator.NEXT, ptr, util.ZN);
		        goto simpl;
        
	        case (short)Operator.EVENTUALLY:
		        tl_yychar = lex.tl_yylex();

		        ptr = tl_factor();

		        if(main.tl_simp_log != 0) {
		          if (ptr.ntyp == (short)Operator.TRUE
		              ||  ptr.ntyp == (short)Operator.FALSE)
		            break;	/* <> true == true */

		          if (ptr.ntyp == (short)Operator.U_OPER
		              &&  ptr.lft.ntyp == (short)Operator.TRUE)
		            break;	/* <><>p = <>p */

		          if (ptr.ntyp == (short)Operator.U_OPER)
		            {	/* <> (p U q) = <> q */
		              ptr = ptr.rgt;
		              /* fall thru */
		            }
		        }

		        ptr = cache.tl_nn((short)Operator.U_OPER, util.True, ptr);
	        simpl:
		        if (main.tl_simp_log != 0) 
		          ptr = bin_simpler(ptr);
		        break;
	        case (short)Operator.PREDICATE:
		        ptr = tl_yylval;
		        tl_yychar = lex.tl_yylex();
		        break;
	        case (short)Operator.TRUE:
	        case (short)Operator.FALSE:
		        ptr = tl_yylval;
		        tl_yychar = lex.tl_yylex();
		        break;
	        }
	        if (ptr == null) main.tl_yyerror("expected predicate");
//#if X0
//            Console.WriteLine("factor:	");
//            main.tl_explain(ptr.ntyp);
//            Console.WriteLine("\n");
//#endif
	        return ptr;
        }
        internal static Node tl_level(int nr)
        {	int i; 
            Node ptr = util.ZN;

	        if (nr < 0)
		        return tl_factor();

	        ptr = tl_level(nr-1);
        again:
	        for (i = 0; i < 4; i++)
		        if (tl_yychar == prec[nr, i])
		        {	tl_yychar = lex.tl_yylex();
			        ptr = cache.tl_nn(prec[nr, i],
				        ptr, tl_level(nr-1));
			        if(main.tl_simp_log != 0) ptr = bin_simpler(ptr);
			        else ptr = bin_minimal(ptr);
			        goto again;
		        }
	        if (ptr == null) main.tl_yyerror("syntax error");
//#if X0
//            Console.WriteLine("level %d:	", nr);
//            main.tl_explain(ptr.ntyp);
//            Console.WriteLine("\n");
//#endif
	        return ptr;
        }
        internal static Node tl_formula()
        {	tl_yychar = lex.tl_yylex();
	        return tl_level(1);	/* 2 precedence levels, 1 and 0 */	
        }


        internal static void tl_parse()
        {
            Node n = tl_formula();
            //         if (tl_verbose)
            // 	{	printf("formula: ");
            // 		put_uform();
            // 		printf("\n");
            // 	}

            trans.trans1(n);
        }

        //create the LTL only
        internal static Node ltl_parse()
        {
            return tl_formula();
        }
                    

    }
}
