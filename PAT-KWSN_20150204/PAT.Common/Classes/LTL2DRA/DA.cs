using System.Collections.Generic;
using PAT.Common.Classes.LTL2DRA.exception;

namespace PAT.Common.Classes.LTL2DRA
{
    //APElement, EdgeContainerExplicit_APElement
    //Label, EdgeContainer
    public class DA
    {

        // Members

        /** The number of states. */
        //private int _state_count;

        /** The storage index for the states. */
        protected List<DA_State> _index;

        /** The underlying APset. */
        protected APSet _ap_set;

        /** The start state. */
        private DA_State _start_state;

        /** Flag to mark that the automaton is compact. */
        private bool _is_compact;

        /** A comment */
        private string _comment;

        /** The acceptance condition for this automaton. */
        private RabinAcceptance _acceptance;

        /** Create a new instance of the automaton. */
        public virtual DA createInstance(APSet ap_set)
        {
            return null;
        }

        /** The number of states in the automaton.*/
        public int size() { return _index.Count; }

        /**
        * Get the state with index i.
        */
        public DA_State get(int i)
        {
            return _index[i];
        }

        /**
         * Array index operator, get the state with index i.
         */
        public DA_State this[int i]
        {
            get
            {
                return _index[i];
            }
        }

        /**
         * Get the size of the underlying APSet.
         */
        public int getAPSize() { return _ap_set.size(); }

        /**
         * Get a const reference to the underlying APSet.
         */
        public APSet getAPSet() { return _ap_set; }

        ///**
        // * Get a const pointer to the underlying APSet.
        // */
        //APSet getAPSet_cp() const {return _ap_set;}

        /**
      * Switch the APSet to another with the same number of APs.
      */
        public void switchAPSet(APSet new_apset)
        {
            if (new_apset.size() != _ap_set.size())
            {
                throw new IllegalArgumentException("New APSet has to have the same size as the old APSet!");
            }

            _ap_set = new_apset;
        }

        ///**
        //* Get the index for a state.
        //*/
        //public int getIndexForState(DA_State state)
        //{
        //    //return _index.get_index(state);

        //    for (int i = 0; i < _index.Count; i++)
        //    {
        //        DA_State s = _index[i];
        //        if (state == s)
        //        {
        //            return i;
        //        }
        //    }

        //    return -1;
        //}

        /** Set the start state. */
        public void setStartState(DA_State state)
        {
            _start_state = state;
        }

        /**
         * Get the start state.
         * @return the start state, or NULL if it wasn't set.
         */
        public DA_State getStartState()
        {
            return _start_state;
        }

        /** Output state label for DOT printing. 
         * @param out the output stream
         * @param state_index the state index
         */
        public virtual string formatStateForDOT(int state_index)
        {
            return "label = \"" + state_index + "\"";
        }

        /** Checks if the automaton is compact. */
        public bool isCompact()
        {
            //return _is_compact && acceptance().isCompact();
            return true;
        }

        /** Set a comment for the automaton. */
        public void setComment(string comment)
        {
            _comment = comment;
        }

        /** Get the comment for the automaton. */
        public string getComment()
        {
            return _comment;
        }

        /** Return reference to the acceptance condition for this automaton.
  * @return reference to the acceptance condition
  */
        public RabinAcceptance acceptance()
        {
            return _acceptance;
        }

        /**
        * Constructor.
 * @param ap_set the underlying APSet.
 */
        //template <typename Label, template <typename N> class EdgeContainer, typename AcceptanceCondition>
        public DA(APSet ap_set)
        {
            //_state_count = 0;
            _ap_set = ap_set;
            _start_state = null;
            _is_compact = true;

            //added by ly
            _index = new List<DA_State>();
            //_start_state
            //_acceptance??
            _acceptance = new RabinAcceptance();
        }


        public DA_State newState()
        {
            DA_State state = new DA_State(this, _index.Count);

            _index.Add(state);
            _acceptance.addState(state.getName());
            return state;
        }

        //       /**
        //* Reorder states and acceptance conditions so that
        //* the automaton becomes compact.
        //*/

        //   public void makeCompact() {
        // acceptance().makeCompact();

        // if (!_is_compact) {
        //   pair<bool, std::vector<unsigned int> > r=_index.compact();

        //   bool moved=r.first;
        //   std::vector<unsigned int>& mapping=r.second;

        //   if (moved) {
        //     acceptance().moveStates(mapping);
        //   }
        //   _is_compact=true;
        // }


    }
}
