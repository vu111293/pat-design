using System;

namespace ltl2ba
{
    class trans
    {
        //internal static int tl_terse = 0, tl_errs = 0;
        //static int Stack_mx = 0, Max_Red = 0, Total = 0;

        static String dumpbuf = "";

        internal static int only_nxt(Node n)
        {
                switch (n.ntyp) {
                case (short)Operator.NEXT:
                        return 1;
                case (short)Operator.OR:
                case (short)Operator.AND:
                        return (only_nxt(n.rgt) != 0 && only_nxt(n.lft) != 0) ? 1 : 0;
                default:
                        return 0;
                }
        }

        internal static void sdump(Node n)
        {
	        switch (n.ntyp) {
	        case (short)Operator.PREDICATE:	dumpbuf += n.sym.name;
			        break;
            case (short)Operator.U_OPER: dumpbuf += "U";
			        goto common2;
            case (short)Operator.V_OPER: dumpbuf += "V";
			        goto common2;
            case (short)Operator.OR: dumpbuf += "|";
			        goto common2;
            case (short)Operator.AND: dumpbuf += "&";
        common2:		sdump(n.rgt);
        common1:		sdump(n.lft);
			        break;
            case (short)Operator.NEXT: dumpbuf += "X";
			        goto common1;

            case (short)Operator.NOT: dumpbuf += "!";
			        goto common1;
            case (short)Operator.TRUE: dumpbuf += "T";
			        break;
            case (short)Operator.FALSE: dumpbuf += "F";
			        break;
	        default:	dumpbuf += "?";
			        break;
	        }
        }
        internal static Symbol DoDump(Node n)
        {
            if (n == null) return util.ZS;

            if (n.ntyp == (short)Operator.PREDICATE)
                return n.sym;

            dumpbuf = "";
            sdump(n);
            return lex.tl_lookup(dumpbuf);
        }
        internal static void trans1(Node p)
        {
            if (p == null || main.tl_errs != 0) return;

            //   if (tl_verbose || tl_terse) {	
            //     fprintf(tl_out, "\t/* Normlzd: ");
            //     dump(p);
            //     fprintf(tl_out, " */\n");
            //   }
            if (main.tl_terse != 0)
                return;

            alternating.mk_alternating(p);
            generalized.mk_generalized();
            buchi.mk_buchi();
        }
    }
}
