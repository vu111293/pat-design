using System;

namespace ltl2ba
{
    public class main
    {
        //static int tl_stats = 0; /* time and size stats */
        internal static int tl_simp_log = 1; /* logical simplification */
        internal static int tl_simp_diff = 1; /* automata simplification */
        internal static int tl_simp_fly = 1; /* on the fly simplification */
        internal static int tl_simp_scc = 1; /* use scc simplification */
        internal static int tl_fjtofj = 1; /* 2eme fj */
        internal static int tl_errs = 0;
        internal static int	tl_terse     = 0;
        //static ulong All_Mem = 0;

        static string uform = "";
        static string error_msg = "";
        static int	hasuform, cnt;

        public static Node LTLHeadNode = null;

        //static string out1 = "";

        //internal static void alldone(int estatus)
        //{
        //        if (out1.Length > 0)
        //             System.IO.File.Delete(out1);
        //        System.Valuation.Exit(estatus);
        //}

        //internal static char[] emalloc(int n)
        //{
        //    char[] tmp;

        //    if ((tmp = new char[n]) == null)
        //        main.fatal("not enough memory", null);
           
        //    return tmp;
        //}

        internal static int tl_Getchar()
        {
	        if (cnt < hasuform)
		        return uform[cnt++];
	        cnt++;
	        return -1;
        }

        internal static void tl_UnGetchar()
        {
	        if (cnt > 0) cnt--;
        }
        
        //internal static void tl_endstats()
        //{	//int Stack_mx;
        //    //Console.WriteLine("\ntotal memory used: {0}", All_Mem);
        //    /*printf("largest stack sze: %9d\n", Stack_mx);*/
        //    /*cache_stats();*/
        //    //mem.a_stats();
        //}

        //internal static void tl_explain(int n)
        //{
        //    switch (n) {
        //    case (short)Operator.ALWAYS:	Console.Write("[]"); break;
        //    case (short)Operator.EVENTUALLY: Console.Write("<>"); break;
        //    case (short)Operator.IMPLIES:	Console.Write("->"); break;
        //    case (short)Operator.EQUIV:	Console.Write("<->"); break;
        //    case (short)Operator.PREDICATE:	Console.Write("predicate"); break;
        //    case (short)Operator.OR:	Console.Write("||"); break;
        //    case (short)Operator.AND:	Console.Write("&&"); break;
        //    case (short)Operator.NOT:	Console.Write("!"); break;
        //    case (short)Operator.U_OPER:	Console.Write("U"); break;
        //    case (short)Operator.V_OPER:	Console.Write("V"); break;
        //    case (short)Operator.NEXT:	Console.Write("X"); break;
        //    case (short)Operator.TRUE:	Console.Write("true"); break;
        //    case (short)Operator.FALSE:	Console.Write("false"); break;
        //    case ';':	Console.Write("end of formula"); break;
        //    default:	Console.Write("{0}", n); break;
        //    }
        //}

        internal static void tl_explain_detail(int n)
        {
	        switch (n) {
	        case (short)Operator.ALWAYS:	error_msg += "[]"; break;
	        case (short)Operator.EVENTUALLY: error_msg += "<>"; break;
	        case (short)Operator.IMPLIES:	error_msg += "->"; break;
	        case (short)Operator.EQUIV:	error_msg += "<->"; break;
	        case (short)Operator.PREDICATE:	error_msg += "predicate"; break;
	        case (short)Operator.OR:	error_msg += "||"; break;
	        case (short)Operator.AND:	error_msg += "&&"; break;
	        case (short)Operator.NOT:	error_msg += "!"; break;
	        case (short)Operator.U_OPER:	error_msg += "U"; break;
	        case (short)Operator.V_OPER:	error_msg += "V"; break;
	        case (short)Operator.NEXT:	error_msg += "X"; break;
	        case (short)Operator.TRUE:	error_msg += "true"; break;
	        case (short)Operator.FALSE:	error_msg += "false"; break;
	        case ';':	error_msg += "end of formula"; break;
	        default:	error_msg = string.Format("%c", n); break;
	        }
        }
        internal static void non_fatal(string s1, string s2)
        {
	        if (s2 != null)
	        {
		        error_msg = string.Format(s1, s2);
	        }
	        else
	        {
		        error_msg += s1;
	        }

            if (parse.tl_yychar != -1 && parse.tl_yychar != 0)
	        {	
		        error_msg += ", saw '";

                tl_explain_detail(parse.tl_yychar);
        		
		        error_msg += "'";
	        }

	        error_msg = "\nformula: ";
	        error_msg += uform;
	        error_msg +=  "\n--------";
        	
	        for (int i = 0; i < cnt; i++)
	        {
		        error_msg += "-"; //-
	        }
            
	        error_msg += "^\n";
	        tl_errs++;
        }

        internal static void tl_yyerror(string s1)
        {
	        fatal(s1, null);
        }
        internal static void fatal(string s1, string s2)
        {
           non_fatal(s1, s2);
           throw new Exception(error_msg);
        }

        private static void Initilize(string options) 
        {
	        alternating.sym_size = 0;
	        buchi.bstates = null;
	        alternating.sym_table = null;	       
            error_msg = string.Empty;
            LTLHeadNode = null;
        	
	        //tl_stats     = 0; /* time and size stats */	
	        tl_simp_log  = 1; /* logical simplification */
	        tl_simp_diff = 1; /* automata simplification */
	        tl_simp_fly  = 1; /* on the fly simplification */
	        tl_simp_scc  = 1; /* use scc simplification */
	        tl_fjtofj    = 1; /* 2eme fj */
	        tl_errs      = 0;
	        tl_terse     = 0;

        	
	        for(int i=0;i<options.Length;i++) {
		        switch (options[i]) {
				        case 'a': tl_fjtofj = 0; break;
				        case 'c': tl_simp_scc = 0; break;
				        case 'o': tl_simp_fly = 0; break;
				        case 'p': tl_simp_diff = 0; break;
				        case 'l': tl_simp_log = 0; break;
		        }
	        }
            
	        generalized.init_size = 0;
            generalized.gstate_id = 1;
            generalized.gstate_count = 0;
            generalized.gtrans_count = 0;

        }
        public static void ConvertFormula(string formula, string options)
        {
            //initialize the options
            Initilize(options);

            hasuform = formula.Length;
            uform = formula;

            cnt = 0;
            alternating.sym_id = 0;
            alternating.node_id = 1;

            parse.tl_parse();
        }

        public static Node ParseLTL(string formula, string options)
        {
            //initialize the options
            Initilize(options);

            hasuform = formula.Length;
            uform = formula;

            cnt = 0;
            alternating.sym_id = 0;
            alternating.node_id = 1;

            return parse.ltl_parse();
        }

        public static BState GetBstates() 
        {
	        return buchi.bstates;
        }
        public static int GetAccept() 
        {
	        return buchi.b_accept;
        }
        public static int GetSymtemID() 
        {
	        return alternating.sym_id;
        }
        public static int GetSystemSize() 
        {
	        return alternating.sym_size;
        }
        public static string GetSystemString(int i) 
        {
	        return alternating.sym_table[i];		
        }

        public static int BtransPos(BTrans p)
        {
            return ((p).pos)[0];
        }
        public static int BtransNeg(BTrans p)
        {
            return p.neg[0];
        }

        //public static BState BstateNxt(BState p) 
        //{
        //    return p.nxt;
        //}
        //public static int BstateFinal(BState p)
        //{
        //    return p.final;
        //}
        //public static int BstateID(BState p)
        //{
        //    return p.id;
        //}
        //public static BTrans BstateFirstTrans(BState p)
        //{
        //    return p.trans;
        //}
        //public static BTrans BtransNxt(BTrans p) 
        //{
        //    return p.nxt;
        //}
        //public static BState BtransTo(BTrans p) 
        //{
        //    return p.to;
        //}


 
    }
}
