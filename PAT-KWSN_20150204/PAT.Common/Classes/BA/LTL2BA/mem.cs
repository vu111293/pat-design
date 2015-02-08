namespace ltl2ba
{
    class mem
    {
        //public static ulong All_mem;
        //static ATrans atrans_list = null;
        //static GTrans gtrans_list = null;
        //static BTrans btrans_list = null;
        //static int aallocs = 0, afrees = 0, apool = 0;
        //static int gallocs = 0, gfrees = 0, gpool = 0;
        //static int ballocs = 0, bfrees = 0, bpool = 0;
        //static M[] freelist = new M[util.A_LARGE];
        //static long[] req = new long[util.A_LARGE];
        //static long[,] event1 = new long[util.NREVENT, util.A_LARGE];
        
        internal static ATrans emalloc_atrans()
        {
            ATrans result = new ATrans();
            result.pos = set.new_set(1);
            result.neg = set.new_set(1);
            result.to = set.new_set(0);
           
            return result;
        }
        internal static void free_atrans(ATrans t, int rec)
        {
            
        }
        internal static void tfree(object v)
        {

        }
        internal static void free_btrans(BTrans t, BTrans sentinel, int fly)
        {
        }
        internal static void free_gtrans(GTrans t, GTrans sentinel, int fly)
        {
        }
        internal static BTrans emalloc_btrans()
        {
            BTrans result = new BTrans();
            result.pos = set.new_set(1);
            result.neg = set.new_set(1);
           
            return result;
        }
        internal static GTrans emalloc_gtrans()
        {
            GTrans result = new GTrans();
            result.pos = set.new_set(1);
            result.neg = set.new_set(1);
            result.final = set.new_set(0);
            
            return result;
        }

        //internal static void free_all_atrans()
        //{
        //}

        //internal static void a_stats()
        //{
        //}
    }

    //[StructLayout(LayoutKind.Explicit)]
    //class M
    //{
    //    [FieldOffset(0)] long size;
    //    [FieldOffset(0)] M link;
    //}

    //static partial class util
    //{
    //    internal static int A_LARGE { get { return 80; } }
    //    internal static int A_USER { get { return 0x55000000; } }
    //    internal static int NOTOOBIG { get { return 32768; } }
    //    internal static int POOL { get { return 0; } }
    //    internal static int ALLOC { get { return 1; } }
    //    internal static int FREE { get { return 2; } }
    //    internal static int NREVENT { get { return 3; } }
    //}
}
