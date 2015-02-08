using PAT.Common.Classes.LTL2DRA.exception;


namespace PAT.Common.Classes.LTL2DRA
{
    public class APSet
    {
        /** Maximum size of the APSet */
        const int MAX_AP = 31;

        /** Dummy constructor to prevent copying. */
        //APSet(APSet other);
        /** Dummy operator= to prevent copying. */
        //APSet operator=(APSet const& other);

        /** The storage for the atomic propositions. */
        public string[] array;
        /** The size of the APSet. */
        int array_size;

        /**
        * Constructor.
        */
        public APSet()
        {
            array = new string[MAX_AP];
            array_size = 0;
        }


        /**
  * Adds a new AP to the set.
  * @param name the name of the AP
  * @return the index of the added AP
  */
        public int addAP(string name)
        {
            int new_index = array_size;

            if (new_index >= MAX_AP)
            {
                throw new IllegalArgumentException("Can't add AP, APSet is full");
            }

            array[new_index] = name; // make copy of name

            array_size = new_index + 1;
            return new_index;
        }

        /**
 * Gets the name of a certain AP.
 * @param index index of the AP
 * @return string-ref with the name
 */
        public string getAP(int index)
        {
            if (index < 0 || index >= array_size)
            {
                throw new IndexOutOfBoundsException("Index out of bounds!");
            }

            return array[index];
        }

        /**
         * Searches for an existing AP in the APSet and returns the index.
         * @return the index of the AP, or -1 if not found.
         */
        public int find(string s)
        {
            for (int i = 0; i < size(); i++)
            {
                if (array[i] == s)
                {
                    return i;
                }
            }
            return -1;
        }

        /**
         * Get the size of this set
         * @return the number of APs in this set.
         */
        public int size()
        {
            return array_size;
        }

        /**
         * Get the size of the powerset 2^APSet
         * @return the size of 2^AP
         */
        public int powersetSize()
        {
            return (1 << size());
        }

        public int all_elements_begin()
        {
            return 0;
        }

        public int all_elements_end()
        {
            return 1 << size();//////////////////////2^size
        }

        /** 
         * Equality check.
         * @param other the other APSet
         * @return <b>true</b> if this and the other APSet are equal
         */
        public static bool operator ==(APSet one, APSet other)
        {
            bool? val = Ultility.NullCheck(one, other);
            if(val != null)
            {
                return val.Value;
            }

            if (one.size() != other.size())
            {
                return false;
            }

            for (int i = 0; i < one.size(); i++)
            {
                if (!(one.getAP(i) == other.getAP(i)))
                {
                    return false;
                }
            }

            return true;
        }

        public static bool operator !=(APSet one, APSet other)
        {
            return !(one == other);
        }

   //     /**
   //* Create a new APSet with the same number of 
   //* atomic propositions, but named 'p0', 'p1', 'p2', ...
   //* The caller takes ownership of the memory of the created APSet.
   //* @return APSet* to newly created APSet
   //*/
   //     public APSet createCanonical()
   //     {
   //         APSet canonical = new APSet();

   //         for (int i = 0; i < size(); i++)
   //         {
   //             canonical.addAP("p" + i); //)+ boost::lexical_cast<std::string>(
   //         }

   //         return canonical;
   //     }


    }
}