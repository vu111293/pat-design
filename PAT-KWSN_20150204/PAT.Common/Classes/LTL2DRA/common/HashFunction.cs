using System;

namespace PAT.Common.Classes.LTL2DRA.common
{

    public interface HashFunction
    {
        void hash(char c);
        void hash(int i);
        void hash(bool b);
    }

    public class StdHashFunction : HashFunction
    {

        /** Constructor. */
        public StdHashFunction() // seeding
        {
            _hash = 5381;
        }

        /** Hash a single char */
        public void hash(char c)
        {
            HASH(c);
        }

        /** Hash an unsigned int */
        public void hash(int i)
        {
            for (int t = 0; t < sizeof(int); t++)
            {
                HASH(Convert.ToChar(i & 0xFF));
                i >>= 8;
            }
        }

        /** Hash a boolean */
        public void hash(bool b)
        {
            HASH(b?'+':'-');
        }

        /** Get current value of the hash */
        public int value() { return _hash; }


        /** Hash a single byte */
        private void HASH(char c)
        {
            _hash = ((_hash << 5) + _hash) ^ c;
        }

        /** The current hash value */
        private int _hash;
    }

}

