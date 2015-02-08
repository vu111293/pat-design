using System.Collections.Generic;
using System.Diagnostics;
using PAT.Common.Classes.LTL2DRA.common;

namespace PAT.Common.Classes.LTL2DRA
{
    public class NBAAnalysis
    {
        /** The analysed NBA */
        private NBA _nba;

        /** Information about the SCCs of the NBA (cached) */
        private SCCs _sccs;
        /** Information about the states where all the successor states are accepting (cached) */
        private BitSet _allSuccAccepting;
        /** Information about the states that have an accepting true self-loop (cached) */
        private BitSet _accepting_true_loops;
        /** Information about the reachability of states (cached) */
        private List<BitSet> _reachability;


        /** Constructor.
         * @param nba the NBA to be analyzed
         */
        public NBAAnalysis(NBA nba)
        {
            _nba = nba;
        }

        /** Get the SCCs for the NBA 
 * @return the SCCs
 */
        public SCCs getSCCs()
        {
            if (_sccs == null)
            {
                _sccs = new SCCs();
                GraphAlgorithms.calculateSCCs(_nba, _sccs, false, null);
            }
            return _sccs;
        }


        /** Get the states for which all successor states are accepting.
        * @return BitSet with the information
        */
        public BitSet getStatesWithAllSuccAccepting()
        {
            if (_allSuccAccepting == null)
            {
                calculateStatesWithAllSuccAccepting();
            }
            return _allSuccAccepting;
        }

        /** Get the states with accepting true self loops
 * @return BitSet with the information
 */
        public BitSet getStatesWithAcceptingTrueLoops()
        {
            if (_accepting_true_loops == null)
            {
                calculateAcceptingTrueLoops();
            }
            return _accepting_true_loops;
        }

        /** Checks to see if NBA has only accepting (final) states.
 * @return true iff all states are accepting
 */
        public bool areAllStatesFinal()
        {
            //for (typename NBA_t::iterator it=_nba.begin(); it!=_nba.end(); ++it) 
            for (int i = 0; i < _nba._index.Count; i++)
            {
                NBA_State it = _nba[i];
                if (!it.isFinal())
                {
                    return false;
                }
            }

            return true;
        }

        /** Get the accepting states from the NBA
 * @return BitSet with the information
 */
        public BitSet getFinalStates()
        {
            return _nba.getFinalStates();
        }

        /** Get the reachability analysis for the NBA
 * @return vector of BitSets representing the set of state which are reachable from a given state.
 */
        public List<BitSet> getReachability()
        {
            if (_reachability == null)
            {
                _reachability = getSCCs().getReachabilityForAllStates();
            }

            return _reachability;
        }

        /** Check if the NBA is empty.
 * @return true iff the NBA has no accepting run.
 */
        public bool emptinessCheck()
        {
            SCCs sccs = getSCCs();

#if VERBOSE
    std::cerr << sccs << "\n";

    std::cerr << " Reachability: "<< std::endl;
    std::vector<BitSet>* reachable=sccs.getReachabilityForAllStates();
    for (unsigned int t=0; t < reachable->size(); t++) {
      std::cerr << t << " -> " << (*reachable)[t] << std::endl;
    }
    delete reachable;
#endif

            for (int scc = 0; scc < sccs.countSCCs(); ++scc)
            {
                BitSet states_in_scc = sccs[scc];

                // check to see if there is an accepting state in this SCC
                //for (BitSetIterator it=BitSetIterator(states_in_scc); it!=BitSetIterator::end(states_in_scc); ++it) 
                for (int it = BitSetIterator.start(states_in_scc); it != BitSetIterator.end(states_in_scc); it = BitSetIterator.increment(states_in_scc, it))
                {
                    int state = it;

#if VERBOSE
	 std::cerr << "Considering state " << state << std::endl;
#endif
                    if (_nba[state].isFinal())
                    {
                        // check to see if this SCC is a trivial SCC (can't reach itself)

#if VERBOSE	  
	  std::cerr << " +final";
	  std::cerr << " " << states_in_scc.cardinality();
#endif

                        if (states_in_scc.cardinality() == 1)
                        {
                            // there is only one state in this scc ...

#if VERBOSE	    
	    std::cerr << " +single";
#endif

                            if (sccs.stateIsReachable(state, state) == false)
                            {
                                // ... and it doesn't loop to itself
                                // -> can not guarantee accepting run

#if VERBOSE
	      std::cerr << " -no_loop" << std::endl;
#endif
                                continue;
                            }
                        }

                        // if we are here, the SCC has more than 1 state or 
                        // exactly one self-looping state
                        //  -> accepting run

#if VERBOSE
	  std::cerr << "+acc" << std::endl;
#endif

                        // check that SCC can be reached from initial state
                        Debug.Assert(_nba.getStartState() != null);
                        if (sccs.stateIsReachable(_nba.getStartState().getName(), state))
                        {
#if VERBOSE
	    std::cerr << "Found accepting state = "<< state << std::endl;
#endif
                            return false;
                        }
#if VERBOSE
	  std::cerr << "Not reachable!"<< std::endl;
#endif
                        continue;
                    }
                }
            }
            return true;
        }


        /** 
         * Calculates BitSet which specifies which states in the NBA 
         * only have accepting successors.
         */
        public void calculateStatesWithAllSuccAccepting()
        {
            _allSuccAccepting = new BitSet();
            BitSet result = _allSuccAccepting;
            SCCs sccs = getSCCs();

            List<bool> scc_all_final = new List<bool>();
            Ultility.resize(scc_all_final, sccs.countSCCs());

            for (int i = 0; i < scc_all_final.Count; i++)
            {
                scc_all_final[i] = false;
            }

            for (int i = sccs.countSCCs(); i > 0; --i)
            {
                // go backward in topological order...
                int scc = (sccs.topologicalOrder())[i - 1];

                BitSet states_in_scc = sccs[scc];

                // check to see if all states in this SCC are final
                scc_all_final[scc] = true;
                //for (BitSetIterator it=BitSetIterator(states_in_scc);it!=BitSetIterator::end(states_in_scc);++it) 
                for (int it = BitSetIterator.start(states_in_scc); it != BitSetIterator.end(states_in_scc); it = BitSetIterator.increment(states_in_scc, it))
                {
                    if (!_nba[it].isFinal())
                    {
                        scc_all_final[scc] = false;
                        break;
                    }
                }


                bool might_be_final = false;
                if (scc_all_final[scc] == false)
                {
                    if (states_in_scc.length() == 1)
                    {
                        // there is only one state in this scc ...
                        int state = states_in_scc.nextSetBit(0);

                        if (sccs.stateIsReachable(state, state) == false)
                        {
                            // ... and it doesn't loop to itself
                            might_be_final = true;
                        }
                    }
                }

                if (scc_all_final[scc] == true || might_be_final)
                {
                    // Check to see if all successors are final...
                    bool all_successors_are_final = true;
                    BitSet scc_succ = sccs.successors(scc);

                    //for (BitSetIterator it=BitSetIterator(scc_succ); it!=BitSetIterator::end(scc_succ); ++it) {
                    for (int it = BitSetIterator.start(scc_succ); it != BitSetIterator.end(scc_succ); it = BitSetIterator.increment(scc_succ, it))
                    {
                        if (!scc_all_final[it])
                        {
                            all_successors_are_final = false;
                            break;
                        }
                    }

                    if (all_successors_are_final)
                    {
                        // Add all states in this SCC to the result-set
                        result.Or(states_in_scc);

                        if (might_be_final)
                        {
                            scc_all_final[scc] = true;
                        }
                    }
                }
            }
        }

        /** 
      * Calculate the set of states that are accepting and have a true self loop.
      */
        public void calculateAcceptingTrueLoops()
        {
            _accepting_true_loops = new BitSet();
            //BitSet isAcceptingTrueLoop= *_accepting_true_loops;
            BitSet isAcceptingTrueLoop = _accepting_true_loops;////////changed
            SCCs sccs = getSCCs();

            for (int scc = 0; scc < sccs.countSCCs(); ++scc)
            {
                if (sccs[scc].cardinality() == 1)
                {
                    int state_id = sccs[scc].nextSetBit(0);
                    NBA_State state = _nba[state_id];

                    if (!state.isFinal())
                    {
                        // not final, consider next
                        continue;
                    }

                    if (!sccs.successors(scc).isEmpty())//////////note here
                    {
                        // there are edges leaving this state, consider next
                        continue;
                    }

                    bool no_empty_to = true;
                    if (sccs.stateIsReachable(state_id, state_id))
                    {
                        // state has at least one self-loop
                        // we have to check that there is no edge with empty To

                        //for (typename NBA_t::edge_iterator eit=state->edges_begin(); eit!=state->edges_end(); ++eit) 
                        //BitSet[] edges = state._edge_manager._container._storage;

                        //foreach (BitSet eit in edges)
                        for (KeyValuePair<APElement, BitSet> eit = state.edges_begin(); !state.edges_end(); eit = state.increment())
                        {
                            BitSet edge = eit.Value;
                            if (edge.isEmpty())
                            {
                                // not all edges lead back to the state...
                                no_empty_to = false;
                                break;
                            }
                        }

                        if (no_empty_to)
                        {
                            // When we are here the state is a final true loop
                            isAcceptingTrueLoop.set(state_id);
                            //	  std::cerr << "True Loop: " << state_id << std::endl;
                        }
                    }
                }
            }
        }
    }
}
