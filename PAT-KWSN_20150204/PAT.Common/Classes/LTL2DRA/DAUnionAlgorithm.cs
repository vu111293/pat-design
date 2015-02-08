using System;
using PAT.Common.Classes.LTL2DRA.common;
using PAT.Common.Classes.LTL2DRA.exception;

namespace PAT.Common.Classes.LTL2DRA
{
    /**
 * Specialized UnionAcceptanceCalculator for RabinAcceptance, for calculating the acceptance in the union automaton.
 * This approach merges the acceptance signatures of the two states in the union tuple, the union is provided by
 * the semantics of the Rabin acceptance condition (There <i>exists</i> an acceptance pair ....)
 */

    public struct UnionAcceptanceCalculator
    {
        /** RabinAcceptance signature type. */
        //typedef RabinAcceptance::signature_type signature_t;
        /** shared_ptr of signature type. */
        //typedef boost::shared_ptr<signature_t> signature_ptr;

        /** The acceptance condition of the first automaton. */
        public RabinAcceptance _acc_1;
        /** The acceptance condition of the second automaton. */
        public RabinAcceptance _acc_2;

        /** The size of the acceptance condition in the original automaton. */
        public int _acc_size_1, _acc_size_2;
        /**
         * Constructor. 
         * @param acc_1 The RabinAcceptance condition from automaton 1
         * @param acc_2 The RabinAcceptance condition from automaton 2
         */
        public UnionAcceptanceCalculator(RabinAcceptance acc_1, RabinAcceptance acc_2)
        {
            _acc_1 = acc_1;
            _acc_2 = acc_2;
            _acc_size_1 = _acc_1.size();
            _acc_size_2 = _acc_2.size();
        }

        /**
       * Prepares the acceptance condition in the result union automaton. If the two automata have k1 and k2 
       * acceptance pairs, this function allocates k1+k2 acceptance pairs in the result automaton.
       * @param acceptance_result The acceptance condition in the result automaton.
       */
        public void prepareAcceptance(RabinAcceptance acceptance_result)
        {
            acceptance_result.newAcceptancePairs(_acc_size_1 + _acc_size_2);
        }


        /**
         * Calculate the acceptance signature for the union of two states.
         * @param da_state_1 index of the state in the first automaton
         * @param da_state_2 index of the state in the second automaton
         * @return A shared_ptr Rabin acceptance signature
         */
        public RabinSignature calculateAcceptance(int da_state_1, int da_state_2)
        {
            RabinSignature signature_p = new RabinSignature(_acc_size_1 + _acc_size_2);
            RabinSignature signature = signature_p;

            for (int i = 0; i < _acc_size_1; i++)
            {
                if (_acc_1.isStateInAcceptance_L(i, da_state_1))
                {
                    signature.setL(i, true);
                }
                if (_acc_1.isStateInAcceptance_U(i, da_state_1))
                {
                    signature.setU(i, true);
                }
            }

            for (int j = 0; j < _acc_size_2; j++)
            {
                if (_acc_2.isStateInAcceptance_L(j, da_state_2))
                {
                    signature.setL(j + _acc_size_1, true);
                }
                if (_acc_2.isStateInAcceptance_U(j, da_state_2))
                {
                    signature.setU(j + _acc_size_1, true);
                }
            }

            return signature_p;
        }
    }


    /** A state representing a union state from two DA. */
    public class UnionState : StateInterface
    {
        /** Index of the state from the first automaton */
        public int da_state_1;
        /** Index of the state from the second automaton */
        public int da_state_2;
        /** A shared_ptr with the acceptance signature of this state */
        public RabinSignature signature;
        /** A shared_ptr to a string containing a detailed description of this state */
        public string description;

        /** Constructor.
         * @param da_state_1_ index of the state in the first automaton
         * @param da_state_2_ index of the state in the second automaton
         * @param acceptance_calculator UnionAcceptanceCalculator
         */
        //template <typename AcceptanceCalc>
        public UnionState(int da_state_1_, int da_state_2_, UnionAcceptanceCalculator acceptance_calculator)
        {
            da_state_1 = da_state_1_;
            da_state_2 = da_state_2_;
            signature = acceptance_calculator.calculateAcceptance(da_state_1, da_state_2);
        }

        /** Compare this state to another for equality.
       * @param other the other UnionState
       * @returns true iff the two states are equal
       */
        public static bool operator ==(UnionState one, UnionState other)
        {
            bool? val = Ultility.NullCheck(one, other);
            if (val != null)
            {
                return val.Value;
            }

            return (one.da_state_1 == other.da_state_1 && one.da_state_2 == other.da_state_2);
            // we don't have to check the signature as there is a 
            // 1-on-1 mapping between <da_state_1, da_state_2> -> signature
        }
        public static bool operator !=(UnionState one, UnionState other)
        {
            return !(one == other);
        }

        /** Compare this state to another for less-than-relationship. Uses the natural order on
       * the two indizes (da_state_1, da_state_2)
       * @param other the other UnionState
       * @returns true iff this state is 'smaller' than the other
       */
        public static bool operator <(UnionState one, UnionState other)
        {
            if (one.da_state_1 < other.da_state_1)
                return true;

            if (one.da_state_1 == other.da_state_1)
            {
                return (one.da_state_2 < other.da_state_2);
                // we don't have to check the signature as there is a 
                // 1-on-1 mapping between <da_state_1, da_state_2> -> signature
            }
            return false;
        }

        public static bool operator >(UnionState one, UnionState other)
        {
            if (one == other || one < other)
            {
                return false;
            }
            return true;
        }

        /** Copy acceptance signature for this state
       * @param acceptance (<b>out</b>) AcceptanceForState for the state in the result automaton 
       */
        public override void generateAcceptance(AcceptanceForState acceptance)
        {
            acceptance.setSignature(signature);
        }

        /** Copy acceptance signature for this state
         * @param acceptance (<b>out</b>) acceptance signature for the state in the result automaton 
         */
        public override void generateAcceptance(RabinSignature acceptance)
        {
            acceptance = signature;
        }

        /** Return the acceptance acceptance signature for this state
         * @return the acceptance signature for this state
         */
        public override RabinSignature generateAcceptance()
        {
            return signature;
        }

        /**
         * Set the detailed description for this state
         * @param description_ the description
         */
        public void setDescription(string description_)
        {
            description = description_;
        }

        /** Generate a simple representation of this state 
         * @return a string with the representation
         */
        public string toString()
        {
            return "(" + da_state_1 + "," + da_state_1 + ")";
        }

        /** Return the detailed description 
         * @return the detailed description
         */
        public override string toHTML()
        {
            if (string.IsNullOrEmpty(description))
            {
                throw new Exception("No description");
            }
            else
            {
                return description;
            }
        }

        /** Calculates the hash for the union state. 
         * @param hash the HashFunction functor used for the calculation
         */
        //template <class HashFunction>
        public override void hashCode(HashFunction hash) {
          hash.hash(da_state_1);
          hash.hash(da_state_2);
          // we don't have to consider the signature as there is a 
          // 1-on-1 mapping between <da_state_1, da_state_2> -> signature
        }

        public override int GetHashCode()
        {
            //int hash = 17; // Suitable nullity checks etc, of course :)   
            //hash = hash * 23 + da_state_1.GetHashCode();
            //hash = hash * 23 + da_state_2.GetHashCode();
            //return hash;
            
            StdHashFunction hash = new StdHashFunction();
            hashCode(hash);
            return hash.value();

        }

        public override bool Equals(object obj)
        {
            return this.GetHashCode() == obj.GetHashCode();
        }
    }

    /** A simple wrapper for UnionState_Result to accomodate the plugging in with fuzzy matching */
    public class UnionState_Result : ResultStateInterface<UnionState>
    {
        UnionState state;

        public UnionState_Result(UnionState state_)
        {
            state = state_;
        }

        public UnionState getState()
        {
            return state;
        }
    };


    public class DAUnionAlgorithm : AlgorithmInterface<UnionState, ResultStateInterface<UnionState>>
    {
        //      public:
        ///** shared_ptr for DA_t type*/
        //typedef typename boost::shared_ptr<DA_t> DA_ptr;
        ///** state_type from DA_t */
        //typedef typename DA_t::state_type da_state_t;
        ///** acceptance condition type from DA_t */
        //typedef typename DA_t::acceptance_condition_type da_acceptance_t;
        ///** acceptance signature type from DA_t */
        //typedef typename da_acceptance_t::signature_type da_signature_t;


        /** The first DA */
        private DA _da_1;
        /** The second DA */
        private DA _da_2;
        /** The result DA */
        private DA _result_da;

        /** The acceptance calculator */
        private UnionAcceptanceCalculator _acceptance_calculator;

        /** Perform trueloop check? */
        private bool _trueloop_check;
        /** Generate detailed descriptions? */
        private bool _detailed_states;

        /** Constructor. 
   * @param da_1 The first DA
   * @param da_2 the second DA
   * @param trueloop_check Check for trueloops?
   * @param detailed_states Generate detailed descriptions of the states? */
        //bool trueloop_check=true, bool detailed_states=false
        public DAUnionAlgorithm(DA da_1, DA da_2, bool trueloop_check, bool detailed_states)
        {

            _da_1 = da_1;
            _da_2 = da_2;
            _acceptance_calculator = new UnionAcceptanceCalculator(da_1.acceptance(), da_2.acceptance());
            _trueloop_check = trueloop_check;
            _detailed_states = detailed_states;

            if (!(_da_1.getAPSet() == _da_2.getAPSet()))
            {
                throw new IllegalArgumentException("Can't create union of DAs: APSets don't match");
            }

            APSet combined_ap = da_1.getAPSet();

            if (!_da_1.isCompact() || !_da_2.isCompact())
            {
                throw new IllegalArgumentException("Can't create union of DAs: Not compact");
            }

            _result_da = da_1.createInstance(combined_ap);
        }

        /** Get the resulting DA 
   * @return a shared_ptr to the resulting union DA.
   */
        public DA getResultDA()
        {
            return _result_da;
        }

        /** Calculate the successor state.
   * @param from_state The from state
   * @param elem The edge label 
   * @return result_t the shared_ptr of the successor state
   */
        public ResultStateInterface<UnionState> delta(UnionState from_state, APElement elem)
        {
            DA_State state1_to = _da_1[from_state.da_state_1].edges().get(elem);
            DA_State state2_to = _da_2[from_state.da_state_2].edges().get(elem);

            UnionState to = createState(state1_to.getName(), state2_to.getName());
            return new UnionState_Result(to);
        }

        /** Get the start state.
    * @return a shared_ptr to the start state 
    */
        public UnionState getStartState()
        {
            if (_da_1.getStartState() == null || _da_2.getStartState() == null)
            {
                throw new IllegalArgumentException("DA has no start state!");
            }

            return createState(_da_1.getStartState().getName(), _da_2.getStartState().getName());
        }

        /** Prepare the acceptance condition 
   * @param acceptance the acceptance condition in the result DA
   */
        public void prepareAcceptance(RabinAcceptance acceptance)
        {
            _acceptance_calculator.prepareAcceptance(acceptance);
        }

        /** Check if the automaton is a-priori empty */
        public bool checkEmpty()
        {
            return false;
        }

        /** Calculate the union of two DA. If the DAs are not compact, they are made compact.
   * @param da_1 The first DA
   * @param da_2 the second DA
   * @param trueloop_check Check for trueloops?
   * @param detailed_states Generate detailed descriptions of the states?
   * @return shared_ptr to result DA
   */
        //bool trueloop_check=true,bool detailed_states=false
        public static DA calculateUnion(DA da_1, DA da_2, bool trueloop_check, bool detailed_states)
        {
            //if (!da_1.isCompact()) {
            //  da_1.makeCompact();
            //}

            //if (!da_2.isCompact()) {
            //  da_2.makeCompact();
            //}

            DAUnionAlgorithm dua = new DAUnionAlgorithm(da_1, da_2, trueloop_check, detailed_states);

            NBA2DA generator = new NBA2DA(detailed_states);
            generator.convert(dua, dua.getResultDA() as DRA, 0);

            return dua.getResultDA();
        }

        /** Calculate the union of two DA, using stuttering if possible. If the DAs are not compact, they are made compact.
   * @param da_1 The first DA
   * @param da_2 the second DA
   * @param stutter_information information about the symbols where stuttering is allowed
   * @param trueloop_check Check for trueloops?
   * @param detailed_states Generate detailed descriptions of the states? */
        //bool trueloop_check=true, bool detailed_states=false
        public static DA calculateUnionStuttered(DA da_1, DA da_2, StutterSensitivenessInformation stutter_information, bool trueloop_check, bool detailed_states)
        {
            //if (!da_1.isCompact()) {
            //  da_1.makeCompact();
            //}

            //if (!da_2.isCompact()) {
            //  da_2.makeCompact();
            //}

            //typedef DAUnionAlgorithm<DA_t> algo_t;
            DAUnionAlgorithm dua = new DAUnionAlgorithm(da_1, da_2, trueloop_check, detailed_states);

            //<algo_t, DA_t> 
            StutteredNBA2DAUnion generator = new StutteredNBA2DAUnion(detailed_states, stutter_information);
            generator.convert(dua , dua.getResultDA(), 0);

            return dua.getResultDA();
        }


        /** Create a UnionState 
         * @param da_state_1
         * @param da_state_2
         * @return the corresponding UnionState
         */
        private UnionState createState(int da_state_1, int da_state_2)
        {
            UnionState state = new UnionState(da_state_1, da_state_2, _acceptance_calculator);

            //// Generate detailed description
            //if (_detailed_states) {
            //  std::ostringstream s;

            //  s << "<TABLE BORDER=\"1\" CELLBORDER=\"0\"><TR><TD>";

            //  if (_da_1[da_state_1]->hasDescription()) {
            //s << _da_1[da_state_1]->getDescription();
            //  } else {
            //s << da_state_1;
            //  }

            //  s << "</TD><TD>U</TD><TD>";

            //  if (_da_2[da_state_2]->hasDescription()) {
            //s << _da_2[da_state_2]->getDescription();
            //  } else {
            //s << da_state_2;
            //  }

            //  s << "</TD></TR></TABLE>";

            //  state->setDescription(s.str());
            //}

            return state;
        }
    }
}
