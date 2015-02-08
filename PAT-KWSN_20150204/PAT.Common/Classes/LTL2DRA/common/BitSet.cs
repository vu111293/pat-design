using System;
using System.Collections.Generic;
using System.Diagnostics;


namespace PAT.Common.Classes.LTL2DRA.common
{
    public class BitSet
    {
        public List<bool> bitset;

        public BitSet()
        {
            bitset = new List<bool>();
            //bitset.Add(false); note
        }
        //public BitSet(int count)
        //{
        //    bitset = new List<bool>();
        //    for (int i = 0; i < count;i++ )
        //    {
        //        bitset.Add(false);
        //    }
                
        //}

        public BitSet(int length, bool defaultBool)
        {
            bitset = new List<bool>();
            for (int i = 0; i < length; i++)
            {
                bitset.Add(defaultBool);
            }
        }

        public int Count
        {
            get
            {
                return bitset.Count;
            }
        }

        public BitSet(BitSet bit)
        {
            bitset = new List<bool>(bit.bitset);
        }

        public bool this[int i]
        {
            get
            {
                return this.bitset[i];
            }
            set
            {
                this.bitset[i] = value;
            }
        }

        public void Assign(BitSet other)
        {
            // Handle self-assignment

            //if (other != this)
            //{
                bitset = new List<bool>(other.bitset);
            //}
        }

        public static bool operator ==(BitSet one, BitSet other)
        {
            bool? val = Ultility.NullCheck(one, other);
            if (val != null)
            {
                return val.Value;
            }

            int i;
            for (i = 0; i < one.bitset.Count && i < other.bitset.Count; i++)
            {
                if (one.bitset[i] != other.bitset[i])
                {
                    return false;
                }
            }

            for (int j = i; j < one.bitset.Count; j++)
            {
                if (one.bitset[j] != false) { return false; }
            }

            for (int j = i; j < other.bitset.Count; j++)
            {
                if (other.bitset[j] != false) { return false; }
            }

            return true;
        }

        public static bool operator !=(BitSet one, BitSet other)
        {
            return !(one == other);
        }

        public static bool operator <(BitSet one, BitSet other)
        {
            int i;
            if (one.Count > other.Count)
            {
                for (i = other.Count; i < one.Count; i++)
                {
                    if (one.bitset[i])
                    {
                        return false;
                    }
                }
            }
            else if (one.Count < other.Count)
            {
                for (i = one.Count; i < other.Count; i++)
                {
                    if (other.bitset[i])
                    {
                        return true;
                    }
                }
            }

            for (i = Math.Min(one.Count, other.Count); i > 0; --i)
            {
                if (one.bitset[i - 1] == true && other.bitset[i - 1] == false)
                {
                    return false;
                }
                else if (one.bitset[i - 1] == false && other.bitset[i - 1] == true)
                {
                    return true;
                }
            }

            // we are here if both bitsets are equal
            return false;
        }

        public static bool operator >(BitSet a, BitSet b)
        {
            if (a == b || a < b)
            {
                return false;
            }
            return true;
        }

        // add a bool into BitSet
        public void Add(bool a)
        {
            bitset.Add(a);
        }


        /**
        * Performs logical And operation with another BitSet.
        * @param other the other BitSet. a and b then change a.
        */
        public void And(BitSet b)
        {
            for (int i = 0; i < Count; i++)
            {
                if (i >= b.Count)
                {
                    bitset[i] = false;
                }
                else
                {
                    bitset[i] &= b.bitset[i];
                }
            }
        }

        /**
        * Performs intersection test with other BitSet.
        * @param other the other BitSet.
        * @return <b>true</b> if there is an index <i>i</i> such that bit 
        *         <i>i</i> is set in both BitSets.
        */
        public bool intersects(BitSet b)
        {
            for (int i = 0; i < Count && i < b.Count; i++)
            {
                if ((bitset[i] & b.bitset[i]) != false)
                {
                    return true;
                }
            }
            return false;
        }

        /**
        * Performs logical Or operation with another BitSet.
        * @param other the other BitSet.
        */
        public void Or(BitSet b)
        {
             grow(b.Count-1);
            for (int i = 0; i < b.Count; i++)
            {
                bitset[i] |= b.bitset[i];
            }
        }

        /**
        * Performs logical And Not operation with another BitSet.
        * @param other the other BitSet.
        */
        public void AndNot(BitSet b)
        {
            for (int i = 0; i < Count && i < b.Count; i++)
            {
                bitset[i] = bitset[i] & !b.bitset[i];
            }
        }

        //public void Minus(BitSet other)
        //{
        //    for (int i = 0; i < Count && i < other.Count; i++)
        //    {
        //        bitset[i] = bitset[i] & !other[i];
        //    }
        //}


        /**
        * Clears the BitSet. All bits are set to <code>false</code>.
        */
        public void clear()
        {

            bitset.Clear();
            //bitset.Add(false);
        }

        /**
        * Set the bit at the specified index to <code>false</code>.
        * @param bitIndex the index
        */
        public void clear(int bitIndex)
        {
            bitset[bitIndex] = false;
        }

        /// <summary>
        /// //////Check the index
        /// </summary>
        /// <param name="index"></param>
        public void checkIndex(int index)
        {
            Debug.Assert(index >= 0);
        }

        /**
        * Set the bits between the specified indexes (inclusive) to
        * <code>false</code>.
        * @param fromIndex the lower index
        * @param toIndex the upper index
        */
        public void clear(int fromIndex, int toIndex)
        {
            checkIndex(fromIndex);
            checkIndex(toIndex);

            if (toIndex < fromIndex)
            {
                throw new Exception("toIndex < fromIndex");
            }

            for (int i = fromIndex; i <= toIndex; i++)
            {
                clear(i);
            }
        }

        /**
        * Calculate the cardinality of the BitSet.
        * @return the number of set bits in the BitSet.
        */
        public int cardinality()
        {
            int c = 0;
            for (int i = 0; i < bitset.Count; i++)
            {
                if (get(i))
                {
                    c++;
                }
            }
            return c;
        }

        /**
        * Is the bit at bitIndex set?
        * @param bitIndex the index
        * @return <b>true</b> if the bit at bitIndex is set.
        */
        public bool get(int bitIndex)
        {
            checkIndex(bitIndex);

            if (bitIndex >= bitset.Count)
            {
                // Requested bit is outside of storage, defaults to false
                return false;
            }
            return bitset[bitIndex];

        }



        /**
        * Flip bit at index.
        * @param bitIndex the index.
        */
        public void flip(int bitIndex)
        {
            checkIndex(bitIndex);

            //this[bitIndex] = !this[bitIndex];
            set(bitIndex, !get(bitIndex));
        }

        /**
        * Flip the bits between the specified indexes (inclusive).
        * @param fromIndex the lower index
        * @param toIndex the upper index
        */
        public void flip(int fromIndex, int toIndex)
        {
            checkIndex(fromIndex);
            checkIndex(toIndex);

            if (fromIndex > toIndex)
            {
                throw new Exception("toIndex < fromIndex");
            }

            for (int i = fromIndex; i <= toIndex; i++)
            {
                flip(i);
            }
        }

        /**
        * Checks if the BitSet is empty.
        * @return <b>true</b> if the cardinality of the BitSet is 0
        */
        public bool isEmpty()
        {
            for (int i = 0; i < bitset.Count; i++)
            {
                if (bitset[i] != false)
                {
                    return false;
                }
            }
            return true;
        }

        /**
        * Return the index of the next clear bit at an index >= fromIndex.
        * @return the index of the next clear bit
        */
        public int nextClearBit(int fromIndex)
        {
            if (fromIndex < 0)
            {
                return -1;
            }

            if (fromIndex >= Count)
            {
                return fromIndex;
            }

            while (get(fromIndex))
            {
                ++fromIndex;
            }
            return fromIndex;
        }

        /**
        * Return the index of the next set bit at an index >= fromIndex.
        * @return the index of the next set bit (or -1 if there is no set bit).
        */
        public int nextSetBit(int fromIndex)
        {
            if (fromIndex < 0)
            {
                return -1;
            }

            if (fromIndex >= Count)
            {
                return -1;
            }

            for (int i = fromIndex; i < Count; i++)
            {
                if (get(i))
                {
                    return i;
                }
            }
            return -1;
        }

        /**
        * Get the highest set bit in a single storage unit.
        * @param s the storage unit
        * @return the index of the highest set bit (or -1 if there are none).
        */
        public int getHighestSetBit(BitSet s)
        {
            if (s == null)
            {
                return -1;
            }
            int i = s.Count;
            while (i >= 0)
            {
                if (s.bitset[i]) return i;
                i--;
            }
            return -1;
        }

        /**
        * Get the highest set bit in a single storage unit.
        * @param s the storage unit
        * @return the index of the highest set bit (or -1 if there are none).
        */
        public int getLowestSetBit(BitSet s)
        {
            if (s == null)
            {
                return -1;
            }
            int i = 0;
            while (i < s.Count)
            {
                if (s.bitset[i]) return i;
                i++;
            }
            return -1;
        }
        /**
        *Generate a string representing this BitSet.
        */
        public string ToString()
        {
            string returnString = "";
            for (int i = 0; i < Count; i++)
            {
                returnString += bitset[i].ToString();
            }
            return returnString;
        }

        /** Perform set operation Intersection */
        public void Intersect(BitSet other) { this.And(other); }
        /** Perform set operation Union */
        public void Union(BitSet other) { this.Or(other); }
        /** Perform set operation Minus */
        public void Minus(BitSet other) { this.AndNot(other); }////////////////what does Minus mean??

        /**
        * Set the bit at index bitIndex.
        * @param bitIndex the index.
        */
        public void set(int bitIndex)
        {
            set(bitIndex, true);
        }

        /**
        * Set the bit at index bitIndex to a certain value
        * @param bitIndex the index.
        * @param value the value
        */
        public void set(int bitIndex, bool value)
        {
            checkIndex(bitIndex);

            if (bitIndex >= Count)
            {
                if (value == false)
                {
                    // Bit not in storage, defaults to false: Nothing to do
                    return;
                }

                // We need to grow storage
                grow(bitIndex);
            }

            bitset[bitIndex] = value;

            //int storage_index = bitIndex / bits_per_storage;
            //int index_inside_storage = bitIndex % bits_per_storage;

            //if (value)
            //{
            //    storage[storage_index] |= (1L << index_inside_storage);
            //}
            //else
            //{
            //    storage[storage_index] &= ~(1L << index_inside_storage);
            //}
        }

        void grow(int maxIndex)
        {
            if (maxIndex >= Count)
            {
                // We need to grow it...
                int new_size = maxIndex + 1;

                for (int j = Count; j < new_size; j++)
                {
                    bitset.Add(false);
                }


            }
        }

        /**todo be careful. different with Count.
        * Get the length (the highest-set bit) of the BitSet.
        * @return the length
        */
        public int length()
        {
            for (int i = Count; i > 0; i--)
            {
                if (bitset[i - 1])
                {
                    return i;
                    // Implementation Failure!
                    //throw new Exception ("Implementation failure!");
                }
            }
            return 0;
        }

        public void hashCode(HashFunction hashfunction)
        {
            bool all_zero = true;
            for (int i = Count; i > 0; --i)
            {
                if (all_zero)
                {
                    if (bitset[i - 1] != false)
                    {
                        hashfunction.hash(bitset[i - 1]);
                        all_zero = false;
                    }
                }
                else
                {
                    hashfunction.hash(bitset[i - 1]);
                }
            }
        }
    }
}