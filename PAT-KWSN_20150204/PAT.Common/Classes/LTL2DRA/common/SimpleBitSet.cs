using System;

namespace PAT.Common.Classes.LTL2DRA.common
{
    public class SimpleBitSet
    {
        public int bitset = 0;
        /**
        * Constructor
        */
        public SimpleBitSet()
        {
            bitset = 0;
        }
        public SimpleBitSet(int value)
        {
            bitset = value;
        }
        
        /**
         * Returns the bit at index.
         */
        public bool get(int index)
        {
            // Argument check
            if (index >= 32)
            {
                throw new Exception("Index out of range!");
            }

            int mask = 1 << index;
            return (bitset & mask) != 0;
        }


        public int get()
        {
            return bitset;
        }


        /**
         * Sets the bit at index to value.
         */
        public void set(int index, bool value)
        {
            // Argument check
            if (index >= 32)
            {
                throw new Exception("Index out of range!");
            }

            int mask = 1 << index;
            mask = ~mask; // negate
            bitset &= mask; // set bit at index to 0
            if (value)
            {
                bitset |= 1 << index;
            }
        }

        /**
         * Sets the value of this bitset
         * @param values integer representation of the bitset
         */
        public void set(int values)
        {
            bitset = values;
        }


        /** Get the integer representation of this bitset */
        public int getBitSet() { return bitset; }

        public int intValue() {
          return getBitSet();
        }

        public static bool operator ==(SimpleBitSet one, SimpleBitSet other)
        {
            bool? val = Ultility.NullCheck(one, other);
            if (val != null)
            {
                return val.Value;
            }
            
            return (one.bitset == other.bitset);
        }

        public static bool operator !=(SimpleBitSet one, SimpleBitSet other)
        {
            return !(one.bitset == other.bitset);
        }

        //todo: check for the correctness 
        // In my opinion, this is correct
        public BitSet getBitSetObject()
        {
            BitSet bs = new BitSet();
            for (int i = 0; i < 32; i++)
            {
                int mask = 1 << i;
                if (mask <= bitset)
                {
                    if ((bitset & mask) != 0)
                    {
                        bs.Add(true);
                    }
                    else
                    {
                        bs.Add(false);
                    }
                }
                else
                {

                    break;
                }

            }
            return bs;
        }
    }
}
