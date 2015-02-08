using System.Collections.Generic;
using PAT.Common.Classes.LTL2DRA.common;

namespace PAT.Common.Classes.LTL2DRA
{
    //SafraTreeTemplate, SafraTree, DA_State
    //template <typename ResultType, typename KeyType, typename StateType>
    public class StateMapperFuzzy
    {

        /** The hash map from StateType to MappedStateType */
        public Dictionary<AbstractedKeyType, ValueList> _map;
        public int _count;

        /** Constructor. */
        public StateMapperFuzzy()
        {
            _map = new Dictionary<AbstractedKeyType, ValueList>();
            _count = 0;
        }

        /** Clear the mapping. */
        public void clear()
        {
            //// delete all Lists...
            //for (typename map_type::iterator it = _map.begin();
            //it != _map.end();
            //++it)
            //{
            //    ValueList* list = (*it).second;
            //    if (list != 0)
            //    {
            //        list->destroyList();
            //        delete list;
            //    }
            //}
            _map.Clear();
            _count = 0;
        }

        /** 
 * Search for a mapping, fuzzily
 * @param result the query
 * @return the corresponding state or NULL otherwise
 */
        public DA_State find(SafraTreeTemplate result)
        {
            //map_type::const_iterator it;

            AbstractedKeyType search_key = new AbstractedKeyType(result.getState());

            //it = _map.find(search_key);

            if (_map.ContainsKey(search_key))
            {

                ValueList list = _map[search_key];

                int count = 0;
                while (list != null)
                {
                    // check to see if we are compatible

                    if (SafraTreeCandidateMatcher.isMatch(result, list._key))
                    {
                        //std::cerr << "Found: "<< count << std::endl;
                        return list._state;
                    }

                    //	std::cerr << "Tree: "<< *list->_tree;

                    list = list._next;
                    count++;
                }
                //      std::cerr << "Not found: "<< count << std::endl;
            }

            // not found
            return null;
        }


        /** 
         * Add a mapping
         * @param key the key
         * @param state the state
         */
        public void add(SafraTree key, DA_State state)
        {
            AbstractedKeyType akey = new AbstractedKeyType(key);
            ValueList list = new ValueList();

            list._key = key;
            list._state = state;
            list._next = null;

            //typename map_type::value_type value (akey, list);

            //std::pair<typename  map_type:: iterator,bool > result = _map.insert(value);



            if (_map.ContainsKey(akey))
            {
                // there is already an element with this structure
                // -> insert list into current list

                ValueList head = _map[akey];
                list._next = head._next;
                head._next = list;

                _map[akey] = head;
            }
            else
            {
                _map.Add(akey, list);
            }

            _count++;
        }

        /** Get the number of trees */
        public int size() { return _count; }

        /** Print statistics */
        //public void print_stats(std::ostream& out) {
        //  _map.print_stats(out);
        //}
    }

      /** 
   * A structure that abstracts the Keytype to its abstracted properties
   */
    public class AbstractedKeyType
    {

        SafraTree _key;

        public AbstractedKeyType(SafraTree key)
        {
            _key = key;
        }

        public static bool operator ==(AbstractedKeyType one, AbstractedKeyType other)
        {
            bool? val = Ultility.NullCheck(one, other);
            if (val != null)
            {
                return val.Value;
            }

            return SafraTreeCandidateMatcher.abstract_equal_to(one._key, other._key);
        }

        public static bool operator !=(AbstractedKeyType one, AbstractedKeyType other)
        {
            return !(one._key == other._key);
        }

        public static bool operator <(AbstractedKeyType one, AbstractedKeyType other)
        {
            return SafraTreeCandidateMatcher.abstract_less_than(one._key, other._key);
        }



        public static bool operator >(AbstractedKeyType one, AbstractedKeyType other)
        {
            if (one == other || one < other)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public int hashCode()
        {
            StdHashFunction hash = new StdHashFunction();
            SafraTreeCandidateMatcher.abstract_hash_code(hash, _key);
            return hash.value();
        }

        public override int GetHashCode()
        {
            return hashCode();
        }
        public override bool Equals(object obj)
        {
            return this.GetHashCode() == obj.GetHashCode();
        }
    }




      /** A linked list of KeyTypes that are structurally equal */
  public class ValueList {
      public SafraTree _key;
      public DA_State _state;
      public ValueList _next;

      public ValueList()
      {
          _state = null;
          _next = null;
          _key = null;
      }

      //public void destroyList() {
    //  ValueList* list=_next;
    //  while (list != 0)
    //  {
    //      ValueList* cur = list;
    //      list = list->_next;
    //      delete cur;
    //  }
    //}
    
    
  }
}
