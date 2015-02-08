using System.Collections.Generic;

namespace PAT.Common.Classes.BA.Algorithms.datastructure
{

    public class OneToOneTreeMap<K, V> : OneToOneMap<K, V>
    {


        //UPGRADE_ISSUE: The following fragment of code could not be parsed and was not converted. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1156'"
        private Dictionary<K, V> mapKey;
        //UPGRADE_ISSUE: The following fragment of code could not be parsed and was not converted. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1156'"
        private Dictionary<V, K> mapValue;

        public OneToOneTreeMap()
        {
            //UPGRADE_ISSUE: The following fragment of code could not be parsed and was not converted. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1156'"
            mapKey = new Dictionary<K, V>();
            //UPGRADE_ISSUE: The following fragment of code could not be parsed and was not converted. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1156'"
            mapValue = new Dictionary<V, K>();
        }

        public virtual void put(K key, V value_Renamed)
        {
            if (mapKey.ContainsKey(key))
            {
                mapKey.Remove(key);
            }
            if (mapValue.ContainsKey(value_Renamed))
            {
                mapValue.Remove(value_Renamed);
            }
            mapKey.Add(key, value_Renamed);
            mapValue.Add(value_Renamed, key);
        }

        public virtual void removeKey(K key)
        {
            mapValue.Remove(mapKey[key]);
            mapKey.Remove(key);
        }

        public virtual void removeValue(V value_Renamed)
        {
            mapKey.Remove(mapValue[value_Renamed]);
            mapValue.Remove(value_Renamed);
        }

        public virtual bool containsValue(V value_Renamed)
        {
            return mapValue.ContainsKey(value_Renamed);
        }
        public virtual bool containsKey(K key)
        {
            return mapKey.ContainsKey(key);
        }
        public virtual V getValue(K key)
        {
            //return mapKey.get_Renamed(key);
            return mapKey[key];
        }

        public virtual K getKey(V value_Renamed)
        {
            //return mapValue.get_Renamed(value_Renamed);
            return mapValue[value_Renamed];
        }
    }
}