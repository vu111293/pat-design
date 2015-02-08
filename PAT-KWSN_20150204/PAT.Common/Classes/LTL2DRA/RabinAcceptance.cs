using System;
using System.Collections.Generic;
using PAT.Common.Classes.LTL2DRA.common;


namespace PAT.Common.Classes.LTL2DRA
{
      /** The 3 different colors for RabinAcceptance */
  public enum RabinColor
  {
      RABIN_WHITE=0, 
      RABIN_GREEN=1, 
      RABIN_RED=2
  }


  public class RabinAcceptance
  {
      // members
      /** The number of acceptance pairs */
      public int _acceptance_count;


      /** A vector of BitSet* representing the L part of the acceptance pairs. */
      public List<BitSet> _acceptance_L;
      /** A vector of BitSet* representing the U part of the acceptance pairs. */
      public List<BitSet> _acceptance_U;

      public bool _is_compact;

      /**
      * Constructor
   * @param number_of_initial_pairs The initial numbers of pairs to allocate
   */
      public RabinAcceptance()
          : this(0)
      {

      }

      public RabinAcceptance(int number_of_initial_pairs)
      {
          _is_compact = true;
          if (number_of_initial_pairs > 0)
          {
              newAcceptancePairs(number_of_initial_pairs);
          }

          //added by ly
          _acceptance_L = new List<BitSet>();
          _acceptance_U = new List<BitSet>();

      }

      public bool isCompact()
      {
          return _is_compact;
      }

      ///**
      //* Make this RabinAcceptance compact (part of interface AcceptanceCondition).
      //*/
      public void makeCompact()
      {
          //    if (isCompact())
          //    {
          //        return;
          //    }

          //    // Compress Acceptance-Pairs 
          //    int pair_to = 0;
          //    for (int pair_from = 0;
          //         pair_from < _acceptance_L.Count;
          //         pair_from++)
          //    {
          //        if (_acceptance_L[pair_from] != null) ////!=false
          //        {
          //            if (pair_from == pair_to)
          //            {
          //                // nothing to do
          //            }
          //            else
          //            {
          //                _acceptance_L[pair_to] = _acceptance_L[pair_from];
          //                _acceptance_U[pair_to] = _acceptance_U[pair_from];
          //            }

          //            ++pair_to;
          //        }
          //    }

          //    int new_acceptance_count = pair_to;

          //    //todo: need to do this??
          //    //_acceptance_L.resize(new_acceptance_count);
          //    //_acceptance_U.resize(new_acceptance_count);

          //    //bool[] newAcceptance_L = new bool[new_acceptance_count];
          //    //_acceptance_L.CopyTo(newAcceptance_L, 0);
          //    //_acceptance_L = new BitArray(newAcceptance_L);

          //    //bool[] newAcceptance_U = new bool[new_acceptance_count];
          //    //_acceptance_U.CopyTo(newAcceptance_U, 0);
          //    //_acceptance_U = new BitArray(newAcceptance_U);

          //    _is_compact = true;
      }

      /** Update the acceptance condition upon renaming of states acording
*  to the mapping (part of AcceptanceCondition interface).
*  Assumes that states can only get a lower name.
* @param mapping vector with mapping a[i] -> j
*/
      public void moveStates(List<int> mapping)
      {
          if (!isCompact())
          {
              makeCompact();
          }

          for (int i = 0; i < _acceptance_L.Count; ++i)
          {
              move_acceptance_bits(_acceptance_L[i], mapping);
              move_acceptance_bits(_acceptance_U[i], mapping);
          }
      }

      public override string ToString()
      {
          return "Acceptance-Pairs: " + _acceptance_L.Count;
      }

      public int size()
      {
          return _acceptance_L.Count;
      }


      /**
* Print the Acc-Sig: line for a state (part of interface AcceptanceCondition).
* @param out the output stream.
* @param state_index the state
*/
      public string outputAcceptanceForState(int state_index)
      {
          string returnString = "Acc-Sig:";
          for (int pair_index = 0; pair_index < size(); pair_index++)
          {
              if (isStateInAcceptance_L(pair_index, state_index))
              {
                  returnString += " +" + pair_index;
              }

              if (isStateInAcceptance_U(pair_index, state_index))
              {
                  returnString += " -" + pair_index;
              }
          }
          return returnString;
      }
      /**
       * Add a state (part of interface AcceptanceCondition).
       * @param state_index the index of the added state.
       */
      public void addState(int state_index)
      {
          // TODO: Assert that state_index > highest set bit
          ;
      }

      // ---- Rabin/Streett acceptance specific

      /**
       * Creates a new acceptance pair.
       * @return the index of the new acceptance pair.
       */
      public int newAcceptancePair()
      {
          BitSet l = new BitSet();
          BitSet u = new BitSet();

          _acceptance_L.Add(l);
          _acceptance_U.Add(u);

          _acceptance_count++;
          return _acceptance_L.Count - 1;
      }


      /**
      * Creates count new acceptance pairs.
      * @return the index of the first new acceptance pair.
      */
      public int newAcceptancePairs(int count)
      {
          int rv = _acceptance_L.Count;

          for (int i = 0; i < count; i++)
          {
              newAcceptancePair();
          }

          return rv;
      }

      /**
 * Delete an acceptance pair.
 */
      public void removeAcceptancePair(int pair_index)
      {
          if (_acceptance_L[pair_index] != null)
          {
              _acceptance_count--;
          }

          _acceptance_L.RemoveAt(pair_index);
          _acceptance_U.RemoveAt(pair_index);

          //delete _acceptance_L[pair_index];
          //delete _acceptance_U[pair_index];

          //_acceptance_L[pair_index] = 0;
          //_acceptance_U[pair_index] = 0;

          _is_compact = false;
      }



      /**
 * Get a reference to the BitSet representing L[pair_index], 
 * allowing changes to this set.
 */
      public BitSet getAcceptance_L(int pair_index)
      {
          return _acceptance_L[pair_index];
      }

      /**
       * Get a reference to the BitSet representing U[pair_index], 
       * allowing changes to this set.
       */
      public BitSet getAcceptance_U(int pair_index)
      {
          return _acceptance_U[pair_index];
      }

      /**
    * Get the L part of the acceptance signature for a state (changes to the
    * BitSet do not affect the automaton).
    */
      public BitSet getAcceptance_L_forState(int state_index)
      {
          BitSet result = new BitSet(_acceptance_L.Count, false);
          getBitSetForState(state_index, _acceptance_L, result);


          return result;
      }

      /**
   * Get the U part of the acceptance signature for a state (changes to the
   * BitSet do not affect the automaton).
   */
      public BitSet getAcceptance_U_forState(int state_index)
      {
          BitSet result = new BitSet(_acceptance_U.Count, false);
          getBitSetForState(state_index, _acceptance_U, result);

          return result;
      }

      /** Is a certain state in L[pair_index]? */
      public bool isStateInAcceptance_L(int pair_index, int state_index)
      {
          return _acceptance_L[pair_index].get(state_index);
      }

      /** Is a certain state in U[pair_index]? */
      public bool isStateInAcceptance_U(int pair_index, int state_index)
      {
          return _acceptance_U[pair_index].get(state_index);
      }

      /** Set L[pair_index] for this state to value. */
      public void stateIn_L(int pair_index, int state_index, bool value) //=true
      {
          getAcceptance_L(pair_index).set(state_index, value);
      }

      public void stateIn_L(int pair_index, int state_index) //=true
      {
          stateIn_L(pair_index, state_index, true);
      }

      /** Set U[pair_index] for this state to value. */
      public void stateIn_U(int pair_index, int state_index, bool value) //=true
      {
          getAcceptance_U(pair_index).set(state_index, value);
      }

      public void stateIn_U(int pair_index, int state_index) //=true
      {
          stateIn_U(pair_index, state_index, true);
      }


      /** Calculate the BitSet for a state from the acceptance pairs, store
       *  result in result.
       *  @param state_index the state
       *  @param acc the BitSetVector (either _L or _U)
       *  @param result the Bitset where the results are stored, has to be clear 
       *                at the beginning!
       */
      private void getBitSetForState(int state_index, List<BitSet> acc, BitSet result)
      {

          for (int i = 0; i < acc.Count; i++)
          {
              if (acc[i] != null)
              {
                  if (acc[i].get(state_index))
                  {
                      //result[i] = true;
                      result.set(i);
                  }
              }
          }
      }

      /** 
      * Move the bits set in acc to the places specified by mapping.
      */
      private void move_acceptance_bits(BitSet acc, List<int> mapping)
      {
          int i = 0;
          while (i != -1)
          {
              int j = mapping[i];
              // :: j is always <= i
              if (j > i)
              {
                  throw new Exception("Wrong mapping in move_acceptance_bits");
              }

              if (i == j)
              {
                  // do nothing
              }
              else
              {
                  // move bit from i->j
                  //acc[j] = true;
                  //acc[i] = false;
                  acc.set(j);
                  acc.clear(i);
              }
              //i=acc.nextSetBit(i+1);
              //i++;
              i = acc.nextSetBit(i + 1);
          }
      }
  }


      /** A class storing the acceptance signature for a state
   * (for every acceptance pair one color).
   */
  public class RabinSignature {
  
    /** The L part */
    public BitSet _L;
    /** The U part */
    public BitSet _U;    
    /** The number of acceptance pairs */
    public int _size;

    /** Constructor 
     * @param size the number of acceptance pairs 
     */
    public RabinSignature(int size)
    {
        _size = size;

        //added by ly
        this._L = new BitSet(size, false);
        this._U = new BitSet(size, false);
    }

    /** Constructor
     * @param other another RabinSignature
     */
    public RabinSignature(RabinSignature other) 
    {
         //_L(other._L), _U(other._U), _size(other._size)
        this._L = new BitSet(other._L);
        this._U = new BitSet(other._U);
        this._size = other._size;
        
    }

    /** Constructor
     * @param L the L part of the acceptance signature.
     * @param U the U part of the acceptance signature.
     * @param size the number of acceptance pairs
     */
    public RabinSignature(BitSet L, BitSet U, int size)
    {
         // : _L(L), _U(U), _size(size) 
        this._L = new BitSet(L);
        this._U = new BitSet(U);
        this._size = size;
    }
    
    /** Constructor for getting the acceptance signature for a Tree.
     * @param tree the Tree, get acceptance signature from 
     *    tree.generateAcceptance(*this).
     */
    //template <typename Tree>
    public RabinSignature(StateInterface tree)
    {
        //added by ly
        this._L = new BitSet();
        this._U = new BitSet();

      tree.generateAcceptance(this);
        _size = 0;        
    }

    /** Clear the acceptance signature */
    public void clear() {
      _L.clear(); // = new List<bool>(0); //.Clear();
      _U.clear(); // = new List<bool>(0); //.clear();
    }

    /** Get the L part of this acceptance signature */
    public BitSet getL()  {return _L;}    
    /** Get the U part of this acceptance signature */
    public BitSet getU() {return _U;}

    ///** Get the L part of this acceptance signature */
    //BitArray getL() {return _L;}
    ///** Get the U part of this acceptance signature */
    //BitArray getU() {return _U;}

    /** Set index to value in the L part of this acceptance signature. */
    public void setL(int index, bool value) { //=true
      _L.set(index, value);
    }

    /** Set index to value. in the U part of this acceptance signature. */
    public void setU(int index, bool value) { //=true
      _U.set(index , value);
    }

    /** Set the L and U parts according to RabinColor c. 
     * @param i The pair index
     * @param c the RabinColor
     */
    public void setColor(int i, RabinColor c)
    {
        switch (c)
        {
            case RabinColor.RABIN_RED:
                _U.set(i, true);
                _L.set(i, false);
                break;

            case RabinColor.RABIN_GREEN:
                _U.set(i,  false);
                _L.set(i,  true);
                break;

            case RabinColor.RABIN_WHITE:
                _U.set(i, false);
                _L.set(i, false);
                break;
        }
    }

      /** Get the RabinColor for a pair i */
    public RabinColor getColor(int i)  {
      return _U.get(i) ? RabinColor.RABIN_RED : (_L.get(i) ? RabinColor.RABIN_GREEN : RabinColor.RABIN_WHITE);
    }

    /** Get string representation of this signature. */
    public override string ToString()
    {
        string a = "{";
        for (int i = 0; i < size(); i++)
        {
            switch (getColor(i))
            {
                case RabinColor.RABIN_RED:
                    a += '-' + i; //boost::lexical_cast<std::string>(i);
                    break;
                case RabinColor.RABIN_GREEN:
                    a += '+' + i; //boost::lexical_cast<std::string>(i);
                    break;
                case RabinColor.RABIN_WHITE:
                    break;
            }
        }
        a += "}";

        return a;
    }

      /** Compare to other signature for equality. */
    public static bool operator ==(RabinSignature one, RabinSignature other)
    {
        bool? val = Ultility.NullCheck(one, other);
        if (val != null)
        {
            return val.Value;
        }

        return one._L == other._L && one._U == other._U;
    }

    public static bool operator !=(RabinSignature a, RabinSignature b)
    {
        return !(a == b);
    }

      /** Compare less_than to other signature */
    public static bool operator <(RabinSignature a, RabinSignature other)
    {
        if (a._L < other._L)
        {
            return true;
        }
        else if (a._L == other._L)
        {
            return a._U < other._U;
        }
        return false;
    }

      /** Less-than operator. Does not do deep compare */
    public static bool operator >(RabinSignature a, RabinSignature b)
    {
        if (a == b || a < b)
        {
            return false;
        }
        return true;
    }

      /** Get the number of acceptance pairs */
    public int getSize()  {return _size;}
    /** Get the number of acceptance pairs */
    public int size()  {return _size;}

    /** Set the number of acceptance pairs */
    public void setSize(int size) 
    {
        _size=size;

        //added by ly
        this._L = new BitSet(size, false);
        this._U = new BitSet(size, false);

    }

    /** Merge this acceptance signature with other signature,
     *  for each tuple element calculate the maximum of the
     *  colors according to the order 
     * RABIN_WHITE < RABIN_GREEN < RABIN_RED */
    public void maxMerge(RabinSignature other)
    {
        for (int i = 0; i < _size; i++)
        {
            if (getColor(i) < other.getColor(i))
            {
                setColor(i, other.getColor(i));
            }
        }
    }

    //todo: check whether need to implement or not
      /**
     * Calculate a hash value using HashFunction
     * @param hashfunction the HashFunction
     */
    //template <class HashFunction>
    public void hashCode(HashFunction hashfunction) {
      _L.hashCode(hashfunction);
      _U.hashCode(hashfunction);
    }
    

  }

      /** Accessor for the acceptance signature for a state 
   *  (part of AcceptanceCondition interface)
   */
  public class AcceptanceForState {
  
    /** Reference to the underlying RabinAcceptance */
    public RabinAcceptance _acceptance;
    /** The state index for this accessor */
    public int _state_index;

  
    /** Constructor */
    public AcceptanceForState(RabinAcceptance acceptance,int state_index) 
    {
        _acceptance = acceptance;
        _state_index = state_index;
    }

    /** Add this state to L[pair_index] */
    public void addTo_L(int pair_index) {
      _acceptance.getAcceptance_L(pair_index).set(_state_index);
      _acceptance.getAcceptance_U(pair_index).set(_state_index, false);

      //  _acceptance.getAcceptance_L(pair_index)[_state_index] = true;
      //  _acceptance.getAcceptance_U(pair_index)[_state_index] = false;
    }
    
    /** Add this state to U[pair_index] */
   public void addTo_U(int pair_index) {
      _acceptance.getAcceptance_U(pair_index).set(_state_index);
      _acceptance.getAcceptance_L(pair_index).set(_state_index, false);

      //  _acceptance.getAcceptance_U(pair_index)[_state_index] = true;
      //  _acceptance.getAcceptance_L(pair_index)[_state_index] = false;
    }

    /** Is this state in L[pair_index] */
    public bool isIn_L(int pair_index)  {
      return _acceptance.isStateInAcceptance_L(pair_index, _state_index);
    }

    /** Is this state in U[pair_index] */
    public bool isIn_U(int pair_index) {
      return _acceptance.isStateInAcceptance_U(pair_index, _state_index);
    }
    
    /** Set L and U for this state according to RabinSignature */
    public void setSignature(RabinSignature signature) {
      for (int i=0;i<signature.size();i++) {
	if (signature.getL().get(i)) {
	  addTo_L(i);
	}
	if (signature.getU().get(i)) {
	  addTo_U(i);
	}
      }
    }

    /** Get number of acceptance pairs */
    public int size() {return _acceptance.size();}

    /** Get the signature for this state */
    public RabinSignature getSignature()
    {

        BitSet Larray = _acceptance.getAcceptance_L_forState(_state_index);
        BitSet Uarray = _acceptance.getAcceptance_U_forState(_state_index);

        BitSet llist = new BitSet(Larray);
        BitSet ulist = new BitSet(Uarray);

        //foreach (bool bit in Larray.bitset)
        //{
        //    llist.Add(bit);
        //}

        //foreach (bool bit in Uarray.bitset)
        //{
        //    ulist.Add(bit);
        //}

        return new RabinSignature(llist, ulist, _acceptance.size());
    }
  };
}
