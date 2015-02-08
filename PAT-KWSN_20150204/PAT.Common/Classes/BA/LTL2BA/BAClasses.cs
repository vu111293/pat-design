using System;

namespace ltl2ba
{
    public class Symbol
    {
        public String name;
        public Symbol next;
    }

    public class Node
    {
        public short ntyp;
        public Symbol sym;
        public Node lft;
        public Node rgt;
        //public Node nxt;
    }
    public class Graph
    {
        public Symbol name;
        public Symbol incoming;
        public Symbol outgoing;
        public Symbol oldstring;
        public Symbol nxtstring;
        public Node New;
        public Node Old;
        public Node Other;
        public Node Next;
        public byte[] isred = new byte[64], isgrn = new byte[64];
        public byte redcnt, grncnt;
        public byte reachable;
        public Graph nxt;
    }
    class ValueWrapper<T>
    {
        public T value;
    }
    public class Mapping
    {
        public string from;
        public Graph to;
        public Mapping nxt;
    }
    public class ATrans 
    {
        public int[] to;
        public int[] pos;
        public int[] neg;
        public ATrans nxt;
    }
    public class AProd 
    {
        public int astate;
        public ATrans prod;
        public ATrans trans;
        public AProd nxt;
        public AProd prv;
    }
    public class GTrans 
    {
        public int[] pos;
        public int[] neg;
        public GState to;
        public int[] final;
        public GTrans nxt;
    }
    public class GState 
    {
        public int id;
        public int incoming;
        public int[] nodes_set;
        public GTrans trans;
        public GState nxt;
        public GState prv;
    }
    public class BTrans 
    {
        public BState to;
        public int[] pos;
        public int[] neg;
        public BTrans nxt;
    }
    public class BState 
    {
        public GState gstate;
        public int id;
        public int incoming;
        public int final;
        public BTrans trans;
        public BState nxt;
        public BState prv;
    }
    public class GScc 
    {
        public GState gstate;
        public int rank;
        public int theta;
        public GScc nxt;
    }
    public class BScc 
    {
        public BState bstate;
        public int rank;
        public int theta;
        public BScc nxt;
    }
    public enum Operator : short 
    {
        ALWAYS=257,
	    AND,		/* 258 */
	    EQUIV,		/* 259 */
	    EVENTUALLY,	/* 260 */
	    FALSE,		/* 261 */
	    IMPLIES,	/* 262 */
	    NOT,		/* 263 */
	    OR,		    /* 264 */
	    PREDICATE,	/* 265 */
	    TRUE,		/* 266 */
	    U_OPER,		/* 267 */
	    V_OPER,		/* 268 */
        NEXT		/* 269 */
    }

    static partial class util
    {
        public static Node ZN { get {return null;} }
        public static Symbol ZS { get {return null;} }
        public static int Nhash { get {return 255;} }
        public static Node True { get { return cache.tl_nn((int)Operator.TRUE, ZN, ZN); } }
        public static Node False { get { return cache.tl_nn((int)Operator.FALSE, ZN, ZN); } }
        public static Node Not(Node a) { return rewrt.push_negation(cache.tl_nn((int)Operator.NOT, a, ZN)); }
        public static Node rewrite(Node n) { return rewrt.canonical(rewrt.right_linked(n)); }
        //public static void Debug(object o) { if (false) Console.WriteLine(o); }
        //public static void Assert(bool x, int y) { if (!x) { main.tl_explain(y); main.fatal(": assertion failed\n", null); } }
    }
}
