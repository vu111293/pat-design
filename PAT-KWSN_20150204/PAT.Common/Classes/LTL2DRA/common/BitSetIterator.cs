
namespace PAT.Common.Classes.LTL2DRA.common
{
    public class BitSetIterator
    {
        /** Constructor, pointing after the last set bit. */
        public static int start(BitSet bitset)
        {
            return increment(bitset, -1);
        }

        /** Constructor, pointing after the last set bit. */
        public static int end(BitSet bitset)
        {
            //return BitSetIterator(bitset, -1);
            //return bitset.Count;
            return -1;
        }


        /** Increment iterator */
        public static int increment(BitSet bitset, int index)
        {
            return bitset.nextSetBit(index + 1);
        }
  
    }
}
