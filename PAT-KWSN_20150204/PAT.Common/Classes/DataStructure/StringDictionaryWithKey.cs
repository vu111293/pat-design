using System;
using System.Collections;
using System.Diagnostics;

namespace PAT.Common.Classes.DataStructure
{
    /// <summary>
    /// A fast, memory efficient StringDictionary.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class StringDictionaryWithKey<T>
    {
        /// Some primes to help speed up prime calculation
        /// </summary>
        private static uint[] _primes = new uint[]   {
                                                           2, 3, 5, 7, 11, 13, 17, 19, 23, 29, 31, 37, 41, 43, 47, 53, 59, 61, 67, 71, 73, 79, 83, 89, 97, 101, 103, 107, 109, 113, 127, 131, 137, 139, 149, 151, 157, 163, 167, 173, 179, 181, 191, 193, 197, 199, 211, 223, 227, 229, 233, 239, 241, 251, 257, 263, 269, 271, 277, 281, 283, 293, 307, 311, 313, 317, 331, 337, 347, 349, 353, 359, 367, 373, 379, 383, 389, 397, 401, 409, 419, 421, 431, 433, 439, 443, 449, 457, 461, 463, 467, 479, 487, 491, 499, 503, 509, 521, 523, 541, 547, 557, 563, 569, 571, 577, 587, 593, 599, 601, 607, 613, 617, 619, 631, 641, 643, 647, 653, 659, 661, 673, 677, 683, 691, 701, 709, 719, 727, 733, 739, 743, 751, 757, 761, 769, 773, 787, 797, 809, 811, 821, 823, 827, 829, 839, 853, 857, 859, 863, 877, 881, 883, 887, 907, 911, 919, 929, 937, 941, 947, 953, 967, 971, 977, 983, 991, 997, 1009, 1013, 1019, 1021, 1031, 1033, 1039, 1049, 1051, 1061, 1063, 1069, 1087, 1091, 1093, 1097, 1103, 1109, 1117, 1123, 1129, 1151, 1153, 1163, 1171, 1181, 1187, 1193, 1201, 1213, 1217, 1223, 1229, 1231, 1237, 1249, 1259, 1277, 1279, 1283, 1289, 1291, 1297, 1301, 1303, 1307, 1319, 1321, 1327, 1361, 1367, 1373, 1381, 1399, 1409, 1423, 1427, 1429, 1433, 1439, 1447, 1451, 1453, 1459, 1471, 1481, 1483, 1487, 1489, 1493, 1499, 1511, 1523, 1531, 1543, 1549, 1553, 1559, 1567, 1571, 1579, 1583, 1597, 1601, 1607, 1609, 1613, 1619, 1621, 1627, 1637, 1657, 1663, 1667, 1669, 1693, 1697, 1699, 1709, 1721, 1723, 1733, 1741, 1747, 1753, 1759, 1777, 1783, 1787, 1789, 1801, 1811, 1823, 1831, 1847, 1861, 1867, 1871, 1873, 1877, 1879, 1889, 1901, 1907, 1913, 1931, 1933, 1949, 1951, 1973, 1979, 1987, 1993, 1997, 1999, 2003, 2011, 2017, 2027, 2029
                                                       };


        #region Constructors
        public StringDictionaryWithKey() // : this(0)
        {
            _capacity = 8; // (capacity > _minCapacity ? GetNextCapacity(capacity) : _minCapacity);
            _capacityLess1 = _capacity - 1;
            // Minimum capacity of StringDictionary = 1024 entries
            _entries = new StringDictionaryEntryWithKey<T>[_capacity];
            // Initialize the space map
            _spaceMap = new BitArray(_capacity);
        }

        public StringDictionaryWithKey(StringDictionaryWithKey<T> copyDictionary)
        {
            _capacity = copyDictionary._capacity;
            _capacityLess1 = copyDictionary._capacityLess1;
            _entries = new StringDictionaryEntryWithKey<T>[_capacity];
            //copyDictionary._entries.CopyTo(_entries, 0);
            _spaceMap = new BitArray(copyDictionary._spaceMap);
            Count = copyDictionary.Count;
        }

        public void ReinitilizeSpaceMap()
        {
            _spaceMap =  new BitArray(_spaceMap); // new BitArray(copyDictionary._spaceMap);
        }

        public T this[string key]
        {
            get
            {
                return this.GetContainsKey(key);
            }
            set
            {
                this.SetValue(key, value);
            }
        }


        ///// <summary>
        ///// Capacity specifies the initial size of the StringDictionary. The StringDictionary grows too accomodate more entries as they are added.
        ///// </summary>
        ///// <param name="capacity"></param>
        //public StringDictionaryWithKey(int capacity)
        //{
        //    _capacity = (capacity > _minCapacity ? GetNextCapacity(capacity) : _minCapacity);
        //    _capacityLess1 = _capacity - 1;
        //    // Minimum capacity of StringDictionary = 1024 entries
        //    _entries = new StringDictionaryEntryWithKey<T>[_capacity];
        //    // Initialize the space map
        //    _spaceMap = new BitArray(_capacity);
        //}

        #endregion

        #region Private Members
        // Used for thread synchronization
        private object _syncRoot = new object();
        // Number of items present in StringDictionary
        public int Count;
        // StringDictionary entries
        public StringDictionaryEntryWithKey<T>[] _entries;
        // The number of hashes to use by default. A higher number will provide higher load factor. A lower number will provide greater performance.
        private const int _numHashes = 15;
        // Mininum Capacity of StringDictionary
        //private const int _minCapacity = 4;
        // The space map for the dictionary
        private BitArray _spaceMap;
        // The present capacity
        int _capacity;
        int _capacityLess1;
        #endregion

        #region Public Methods
        /// <summary>
        /// This methods adds a given Key value pair to the StringDictionary. Keys need to be unique. Values need not be unique.
        /// Complexity Average Case: O(log((n + 1/)n)), Worst Case: O(n)
        /// </summary>
        /// <param name="key"></param>
        /// <param name="keyValue"></param>
        public void Add(string key, T keyValue)
        {

            //GetHashCodes(key, out hashA, out hashB);
            uint hashA = 0, hashB = 0;
            for (int i = 0; i < key.Length; i++)
            {
                hashA = ((hashA << 5) + hashA ^ (uint)key[i]);
                hashB = ((hashB << 6) + (hashB << 16) - hashB + (uint)key[i]);
            }

            uint hash;
            bool inserted = false;

            lock (_syncRoot)
            {
                for (int i = 0; i < _numHashes; i++)
                {
                    hash = hashA + _primes[i] * hashB;
                    int index = (int)(hash & _capacityLess1);
                    if (!_spaceMap[index])
                    {
                        _entries[index] = new StringDictionaryEntryWithKey<T>(hashA, hashB, keyValue, key);
                        inserted = true;
                        _spaceMap[index] = true;
                        break;
                    }
                }
                if (!inserted)
                {
                    while (!inserted)
                    {
                        Resize();
                        for (int i = 0; i < _numHashes; i++)
                        {
                            hash = hashA + _primes[i] * hashB;
                            if (!_spaceMap[(int)(hash & _capacityLess1)])
                            {
                                _entries[hash & _capacityLess1] = new StringDictionaryEntryWithKey<T>(hashA, hashB, keyValue, key);
                                inserted = true;
                                _spaceMap[(int)(hash & _capacityLess1)] = true;
                                break;
                            }
                        }
                    }
                }
                Count++;
            }
        }


        /// <summary>
        /// Determines whether a given key exists in the StringDictionary. 
        /// Complexity O(1)
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool ContainsKey(string key)
        {
            uint hashA = 0, hashB = 0;
            for (int i = 0; i < key.Length; i++)
            {
                hashA = ((hashA << 5) + hashA ^ (uint)key[i]);
                hashB = ((hashB << 6) + (hashB << 16) - hashB + (uint)key[i]);
            }

            int index;
            uint hash;

            lock (_syncRoot)
            {

                for (int i = 0; i < _numHashes; i++)
                {
                    hash = hashA + _primes[i] * hashB;
                    index = (int)(hash & _capacityLess1);
                    if (!_spaceMap[index])
                    {
                        return false;
                        //break;
                    }
                    else if (_entries[index].HashA == hashA && _entries[index].HashB == hashB)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Determines whether a given key exists in the StringDictionary. 
        /// Complexity O(1)
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public T GetContainsKey(string key)
        {
            //GetHashCodes(key, out hashA, out hashB);
            uint hashA = 0, hashB = 0;
            for (int i = 0; i < key.Length; i++)
            {
                hashA = ((hashA << 5) + hashA ^ (uint)key[i]);
                hashB = ((hashB << 6) + (hashB << 16) - hashB + (uint)key[i]);
            }

            int index;
            uint hash;

            lock (_syncRoot)
            {
                for (int i = 0; i < _numHashes; i++)
                {
                    hash = hashA + _primes[i] * hashB;
                    index = (int)(hash & _capacityLess1);
                    if (!_spaceMap[index])
                    {
                        return default(T);
                        //break;
                    }
                    else if (_entries[index].HashA == hashA && _entries[index].HashB == hashB)
                    {
                        return _entries[index].Value;
                    }
                }
            }
            return default(T);
            /*
            if (index >= 0)
            {
                return _entries[index].Value;
            }
            else
            {
                return default(T);
            }*/
        }

        /// <summary>
        /// Determines whether a given key exists in the StringDictionary. 
        /// Complexity O(1)
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool SetValue(string key, T value)
        {
            //GetHashCodes(key, out hashA, out hashB);
            uint hashA = 0, hashB = 0;
            for (int i = 0; i < key.Length; i++)
            {
                hashA = ((hashA << 5) + hashA ^ (uint)key[i]);
                hashB = ((hashB << 6) + (hashB << 16) - hashB + (uint)key[i]);
            }

            int index;
            uint hash;
            lock (_syncRoot)
            {
                for (int i = 0; i < _numHashes; i++)
                {
                    hash = hashA + _primes[i] * hashB;
                    index = (int)(hash & _capacityLess1);
                    if (!_spaceMap[index]) // || _entries[index] == null
                    {
                        break;
                    }
                    else if (_entries[index].HashA == hashA && _entries[index].HashB == hashB)
                    {
                        _entries[index].Value = value;
                        return true;
                    }
                }
            }
            return false;
        }


        ///// <summary>
        ///// Determines whether a given key exists in the StringDictionary. 
        ///// Complexity O(1)
        ///// </summary>
        ///// <param name="key"></param>
        ///// <returns></returns>
        //public bool Remove(string key)
        //{
        //    //GetHashCodes(key, out hashA, out hashB);
        //    uint hashA = 0, hashB = 0;
        //    for (int i = 0; i < key.Length; i++)
        //    {
        //        hashA = ((hashA << 5) + hashA ^ (uint)key[i]);
        //        hashB = ((hashB << 6) + (hashB << 16) - hashB + (uint)key[i]);
        //    }

        //    int index;
        //    uint hash;
        //    lock (_syncRoot)
        //    {
        //        for (int i = 0; i < _numHashes; i++)
        //        {
        //            hash = hashA + _primes[i] * hashB;
        //            index = (int)(hash & _capacityLess1);
        //            if (!_spaceMap[index])
        //            {
        //                break;
        //            }
        //            else if (_entries[index].HashA == hashA && _entries[index].HashB == hashB)
        //            {
        //                _entries[index] = null;
        //                _spaceMap[index] = false;
        //                return true;
        //            }
        //        }
        //    }
        //    return false;
        //}

        //private void GetHashCodes(string stringToHash, out uint hashCodeA, out uint hashCodeB)
        //{
        //    uint hashA = 0, hashB = 0;
        //    for (int i = 0; i < stringToHash.Length; i++)
        //    {
        //        hashA = ((hashA << 5) + hashA ^ (uint)stringToHash[i]);
        //        hashB = ((hashB << 6) + (hashB << 16) - hashB + (uint)stringToHash[i]);
        //    }
        //    hashCodeA = hashA;
        //    hashCodeB = hashB;
        //}


        /// <summary>
        /// Clears all the Key-Value pairs from the StringDictionary.
        /// Complexity O(1)
        /// </summary>
        public void Clear()
        {
            _entries = new StringDictionaryEntryWithKey<T>[_capacity];
            _spaceMap.SetAll(false);
            Count = 0;
        }


        /// <summary>
        /// The number of hashes to generate for insertions / lookups in the StringDictionary.
        /// The default value is 31. Possible values range from 5 to 277 inclusive. A lower value leads to greater performance at the cost of a low load factor.
        /// A higher value leads to lesser performance but higher load factor.
        /// </summary>
        //public int NumHashes
        //{
        //    get { return _numHashes; }
        //    set { _numHashes = ((value < 16) && (value > 278) ? value : 31); }
        //}
        // Indexer
        //public T this[string key]
        //{
        //    get
        //    {
        //        //int index;
        //        //if ((index = ContainsKeyInternal(key)) >= 0)
        //        //    return _entries[index].Value;
        //        //else
        //        //    return default(T);
        //        return this.GetContainsKey(key);
        //    }
        //    //set
        //    //{
        //    //    int index;
        //    //    if ((index = ContainsKeyInternal(key)) >= 0)
        //    //        _entries[index].Value = value;
        //    //    else
        //    //        throw new KeyNotFoundException("Key not found. Key: " + key);
        //    //}
        //}
        #endregion

        #region Utility Functions


        /// <summary>
        ///// The internal method used by ContainsKey(string) method.
        ///// </summary>
        ///// <param name="key"></param>
        ///// <returns></returns>
        //private int ContainsKeyInternal(string key)
        //{
        //    uint hashA, hashB;
        //    uint hash;
        //    GetHashCodes(key, out hashA, out hashB);
        //    for (int i = 0; i < _numHashes; i++)
        //    {
        //        hash = hashA + _primes[i] * hashB;
        //        if (!_spaceMap[(int)(hash & _capacityLess1)])
        //            break;
        //        else if (_entries[hash & _capacityLess1].HashA == hashA && _entries[hash & _capacityLess1].HashB == hashB)
        //            return (int)(hash & _capacityLess1);
        //    }
        //    return -1;
        //}

        /// <summary>
        /// The internal method called to resize the _entries array.
        /// Complexity O(n)
        /// </summary>
        private void Resize()
        {
            start:
            _capacity = _capacity << 1;
            _capacityLess1 = _capacity - 1;
            StringDictionaryEntryWithKey<T>[] newEntries = new StringDictionaryEntryWithKey<T>[_capacity];
            BitArray newSpaceMap = new BitArray(_capacity);

            uint hash;

            for (int i = 0; i < _entries.Length; i++)
            {
                if (_spaceMap[i])
                {
                    bool rehashed = false;
                    for (int j = 0; j < _numHashes; j++)
                    {
                        hash = _entries[i].HashA + _primes[j] * _entries[i].HashB;
                        if (!newSpaceMap[(int)(hash & _capacityLess1)])
                        {
                            newEntries[hash & _capacityLess1] = _entries[i];
                            newSpaceMap[(int)(hash & _capacityLess1)] = true;
                            rehashed = true;
                            break;
                        }
                    }

                    if(!rehashed)
                    {
                        //Debug.Assert(false, "rehash goto accured.");
                        goto start;
                    }
                }
            }

            _entries = newEntries;
            _spaceMap = newSpaceMap;
        }


        /// <summary>
        /// Returns the next capacity greater than the given number. Capacities are powers of 2.
        /// </summary>
        /// <param name="currentCapacity"></param>
        /// <returns></returns>
        private static int GetNextCapacity(int currentCapacity)
        {
            for (int i = 2; i < 31; i++)
            {
                if ((2 << i) >= currentCapacity)
                    return 2 << i;
            }
            throw new Exception("Capacity could not be found for currentCapacity: ");
        }

        /// <summary>
        #endregion
    }

    public sealed class StringDictionaryEntryWithKey<T>
    {
        #region Properties

        public uint HashA;
        public uint HashB;
        public T Value;
        public string Key;

        #endregion

        public StringDictionaryEntryWithKey(uint hashA, uint hashB, T val, string key)
        {
            HashA = hashA;
            HashB = hashB;
            Value = val;
            Key = key;
        }
    }
}