using System.Collections.Generic;
using PAT.Common.Classes.LTL2DRA.exception;

namespace PAT.Common.Classes.LTL2DRA
{
    public class EdgeContainerExplicit_APElement<T>
    {


        /** The size of the APSet */
        private int _sizeAP;
        /** The number of edges */
        private long _arraySize;
        /** The storage area for the edges */
        public T[] _storage;

        /** The storage area for the edges */
        //public List<int>[] _storageDebug;

        /** 
   * Constructor.
   * @param sizeAP the number of atomic propositions in the APSet
   */
        public EdgeContainerExplicit_APElement() : this(1)
        {

        }

        public EdgeContainerExplicit_APElement(int sizeAP)
        {
            _sizeAP = sizeAP;
            _arraySize = 1 << sizeAP;
            _storage = new T[_arraySize];
            //for (int i=0;i<_arraySize;i++) {
            //  _storage[i]=0;
            //}

            //_storageDebug = new List<int>[_arraySize];
        }


        //public void addEdgeDebug(int label, int to)
        //{
        //    if(_storageDebug[label] == null)
        //    {
        //        List<int> list = new List<int>();
        //        list.Add(to);
        //        _storageDebug[label] = list;
        //    }
        //    else
        //    {
        //        _storageDebug[label].Add(to);
        //    }
        //}



        /** Add an edge that doesn't already exist */
        //public void addEdge(APElement label, BitSet to) {addEdge(label, to);}

        /** Add an edge that doesn't already exist */
        public void addEdge(APElement label, T to)
        {
            if (get(label) != null) //
            {
                throw new IllegalArgumentException("Trying to add edge which already exists!");
            }

            set(label, to);
        }

        /** Remove an edge */
        public void removeEdge(APElement label)
        {
            if (get(label) == null)
            {
                throw new IllegalArgumentException("Trying to remove non-existing edge!");
            }
            set(label, default(T));
        }

        /** Get the target of the edge labeled with label*/
        public T get(APElement label)
        {
            if (label.getBitSet() >= _arraySize)
            {
                throw new IndexOutOfBoundsException(""); //, 
            }

            return _storage[label.getBitSet()];
        }

        public void set(APElement label, T to)
        {
            if (label.getBitSet() >= _arraySize)
            {
                throw new IndexOutOfBoundsException(""); //IndexOutOfBoundsException, 
            }

            _storage[label.getBitSet()] = to;
        }

        /** 
 * Returns an iterator pointing to the first edge. 
 * This iterator iterates over all the 2^AP edges,
 * irrespective that some may not have a target
 * (dereferencing will return NULL for these).
 */
        private int _i;
        public KeyValuePair<APElement, T> begin()
        {
            _i = -1;
            return increment(); // _storage[0];
            
        }
        /** Returns an iterator pointing after the last edge. */
        public bool end() { return _i == _arraySize; } //return default(T); 

        public KeyValuePair<APElement, T> increment()
        {
            _i++;
            while (_i < _arraySize && this.get(new APElement(_i)) == null)
            {
                _i++;   
            }


            if (_i  < _arraySize)
            {
                return new KeyValuePair<APElement,T>(new APElement(_i), _storage[_i]);
            }
            else
            {
                return new KeyValuePair<APElement, T>(new APElement(_i), default(T));
                //return default(T);
            }
        }
    }


}

