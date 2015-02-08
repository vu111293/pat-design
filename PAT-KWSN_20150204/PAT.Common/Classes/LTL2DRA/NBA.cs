using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Msagl.Drawing;
using PAT.Common.Classes.LTL2DRA.common;
using PAT.Common.Classes.LTL2DRA.exception;

namespace PAT.Common.Classes.LTL2DRA
{
    //APElement, EdgeContainerExplicit_APElement
    //<Label, EdgeContainer>
    public class NBA //: NBA_I
    {

        /** Number of states */
        //private int _state_count;

        /** Storage for the states */
        public List<NBA_State> _index;

        /** The underlying APSet */
        private APSet _apset;

        /** The start states */
        private NBA_State _start_state;

        /** The states that are accepting (final) */
        private BitSet _final_states;

        public NBA(APSet apset)
        {
            //_state_count = 0;
            _apset = apset;
            _start_state = null;

            //ly: added to the source
            _index = new List<NBA_State>();
            _final_states = new BitSet();
        }

        /** Get number of states. */
        public int size() { return _index.Count; }

        /** Array index operator, get the state with index i. */
        public NBA_State this[int i]
        {
            get { return _index[i]; }
            set { this._index[i] = value; }
        }

        /** Get the size of the underlying APSet. */
        public int getAPSize() { return _apset.size(); }

        /** Get a const reference to the underlying APSet. */
        public APSet getAPSet() { return _apset; }

        /** Get a const pointer to the underlying APSet. */
        public APSet getAPSet_cp() { return _apset; }

        /** Switch the APSet to another with the same number of APs. */
        public void switchAPSet(APSet new_apset)
        {
            if (new_apset.size() != _apset.size())
            {
                throw new IllegalArgumentException("New APSet has to have the same size as the old APSet!"); //IllegalArgument
            }
            _apset = new_apset;
        }


        /** Get the index for a state. */
        public int getIndexForState(NBA_State state)
        {
            return _index.IndexOf(state); //.get_index
        }

        /** Set the start state. */
        public void setStartState(NBA_State state)
        {
            _start_state = state;
        }

        /**
   * Get the start state.
   * @return the start state, or NULL if it wasn't set.
   */
        public NBA_State getStartState()
        {
            return _start_state;
        }

        /** Get the set of final (accepting) states in the NBA */
        public BitSet getFinalStates() { return _final_states; }


        /** Return number of states. */
        public int getStateCount() 
        {
            return this._index.Count;
            //_state_count
        }

        // -- NBA_I virtual functions -----------
        /** 
         * Create a new state.
         * @return the index of the new state
         */
        public int nba_i_newState()
        {
            return newState().getName();
        }

        /**
 * Add an edge from state <i>from</i> to state <i>to</i>
 * for the edges covered by the APMonom.
 * @param from the index of the 'from' state
 * @param m the APMonom
 * @param to the index of the 'to' state
 */
        public void nba_i_addEdge(int from, APMonom m, int to)
        {
            this[from].addEdge(m, this[to]);
        }

        /**
 * Get the underlying APSet 
 * @return a const pointer to the APSet
 */
        public APSet nba_i_getAPSet()
        {
            return getAPSet_cp();
        }
        /**
         * Set the state as the start state.
         * @param state the state index
         */
        public void nba_i_setStartState(int state)
        {
            //throw new NotImplementedException();
            setStartState(this[state]);
        }

        /** 
 * Set the final flag (accepting) for a state.
 * @param state the state index
 * @param final the flag
 */
        public void nba_i_setFinal(int state, bool final)
        {
            //throw new NotImplementedException();
            this[state].setFinal(final);
        }


        /**
         * Add a new state.
         * @return a pointer to the newly generated state
         */
        public NBA_State newState()
        {
            //_state_count++;
            NBA_State state = new NBA_State(this, _index.Count);

            _index.Add(state);
            return state;
        }

        /** 
 * Remove states from the set of accepting (final) states when this is redundant.
 * @param sccs the SCCs of the NBA
 */
        public void removeRedundantFinalStates(SCCs sccs)
        {
            for (int scc = 0; scc < sccs.countSCCs(); ++scc)
            {
                if (sccs[scc].cardinality() == 1)
                {
                    int state_id = sccs[scc].nextSetBit(0);
                    NBA_State state = this[state_id];

                    if (state.isFinal())
                    {
                        if (!sccs.stateIsReachable(state_id, state_id))
                        {
                            // The state is final and has no self-loop
                            //  -> the final flag is redundant
                            state.setFinal(false);
                            //	  std::cerr << "Removing final flag for " << state_id << std::endl;
                        }
                    }
                }
            }
        }


        /**
 * Checks if the NBA is deterministic (every edge has at most one target state).
 */
        public bool isDeterministic()
        {
            //for (iterator state_it = begin(); state_it != end(); ++state_it)
            for (int i = 0; i < this._index.Count; i++)
            {
                NBA_State state = this._index[i];
                //for (edge_iterator edge_it = state.edges_begin(); edge_it != state.edges_end(); ++edge_it)
                //BitSet[] edges = state._edge_manager._container._storage;
                //foreach (BitSet edge in edges)
                for (KeyValuePair<APElement, BitSet> edge = state.edges_begin(); !state.edges_end(); edge = state.increment())
                {
                    //edge_type edge = edge_it;
                    if (edge.Value.cardinality() > 1)
                    {
                        return false;
                    }
                }
            }
            return true;
        }



        public static NBA product_automaton(NBA nba_1, NBA nba_2)
        {
            Debug.Assert(nba_1.getAPSet() == nba_2.getAPSet());
            NBA product_nba = new NBA(nba_1.getAPSet_cp());

            APSet apset = nba_1.getAPSet();
            Debug.Assert(apset == nba_2.getAPSet());

            for (int s_1 = 0; s_1 < nba_1.size(); s_1++)
            {
                for (int s_2 = 0; s_2 < nba_2.size(); s_2++)
                {
                    for (int copy = 0; copy < 2; copy++)
                    {
                        int s_r = product_nba.nba_i_newState();
                        Debug.Assert(s_r == (s_1 * nba_2.size() + s_2) * 2 + copy);

                        int to_copy = copy;

                        if (copy == 0 && nba_1[s_1].isFinal())
                        {
                            to_copy = 1;
                        }
                        if (copy == 1 && nba_2[s_2].isFinal())
                        {
                            product_nba[s_r].setFinal(true);
                            to_copy = 0;
                        }

                        //for (typename APSet::element_iterator it=apset.all_elements_begin();it!=apset.all_elements_end();++it) 
                        for (int it = apset.all_elements_begin(); it != apset.all_elements_end(); it++)
                        {
                            APElement label = new APElement(it);
                            BitSet to_s1 = nba_1[s_1].getEdge(label);
                            BitSet to_s2 = nba_2[s_2].getEdge(label);
                            BitSet to_set = new BitSet();
                            //for (BitSetIterator it_e_1 = BitSetIterator(*to_s1); it_e_1 != BitSetIterator::end(*to_s1); ++it_e_1)
                            //for (int it_e_1 = 0; it_e_1 != to_s1.bitset.Count; ++it_e_1)
                            for (int it_e_1 = BitSetIterator.start(to_s1); it_e_1 != BitSetIterator.end(to_s1); it_e_1 = BitSetIterator.increment(to_s1, it_e_1))
                            {
                                //for (BitSetIterator it_e_2 = BitSetIterator(*to_s2); it_e_2 != BitSetIterator::end(*to_s2); ++it_e_2)
                                //for (int it_e_2 = 0; it_e_2 != to_s2.bitset.Count; ++it_e_2)
                                for (int it_e_2 = BitSetIterator.start(to_s2); it_e_2 != BitSetIterator.end(to_s2); it_e_2 = BitSetIterator.increment(to_s2, it_e_2))
                                {
                                    int to = it_e_1 * nba_2.size() + it_e_2 * 2 + to_copy;
                                    to_set.set(to);
                                }
                            }

                            product_nba[s_r].getEdge(label).Assign(to_set);
                        }
                    }
                }
            }

            int start_1 = nba_1.getStartState().getName();
            int start_2 = nba_2.getStartState().getName();
            product_nba.setStartState(product_nba[start_1 * nba_2.size() + start_2]);

            return product_nba;
        }

        
    }
}
