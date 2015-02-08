using System.Collections.Generic;

namespace PAT.Common.Classes.LTL2DRA
{

    //SafraTreeTemplate, SafraTree, DA_State
    //template <typename ResultType, typename KeyType, typename StateType>
    public class StateMapper<KeyType, StateType>
    {

        /** The hash map from StateType to MappedStateType */
        Dictionary<KeyType, StateType> _map;
        /** The number of mappings */
        int _count;

        /** Constructor. */
        public StateMapper()
        {
            _count = 0;
            _map = new Dictionary<KeyType, StateType>();
        }

        /** Clear the mapping */
        public void clear()
        {
            _map.Clear();
            _count = 0;
        }

        /** Add a mapping. 
 * @param key the key
 * @param state the state
 */
        public StateType add(KeyType key, StateType state)
        {
            //_map[key] = state;

            _map.Add(key, state);
             ++_count;
            return state;
        }

        /** Find a mapping. 
 * @param key the key
 * @return the state (or the NULL pointer if not found)
 */
        public StateType find(KeyType key)
        {
            if (_map.ContainsKey(key))
            {
                return _map[key];

            }
            else
            {
                return default(StateType);
            }

            //typename map_type::const_iterator it;
            //it=_map.find(key);
            //if (it == _map.end()) {
            //  return 0;
            //} else {
            //  return (*it).second;
            //}
        }


        ///** Find a mapping using ResultType. 
        // * @param result
        // * @return the state (or the NULL pointer if not found)
        // */
        //public StateType find(SafraTreeTemplate result)
        //{
        //    return find(result.getState());
        //}

        /** Get number of mappings.
         * @return the number of mappings
         */
        public int size()
        {
            return _count;
        }


    }
}
