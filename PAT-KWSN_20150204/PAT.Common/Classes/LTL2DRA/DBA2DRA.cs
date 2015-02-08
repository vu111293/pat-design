using PAT.Common.Classes.LTL2DRA.common;
using PAT.Common.Classes.LTL2DRA.exception;

namespace PAT.Common.Classes.LTL2DRA
{
    public class DBA2DRA
    {
        /**
  * Convert a deterministic B點hi automaton
  * (a nondeterministic B點hi automaton NBA, where every transition
  * has at most one target state) to an equivalent deterministic 
  * Rabin automaton.
  * <p>
  * This involves generation of the appropriate acceptance condition
  * and making sure that the transition function is complete.
  * </p>
  * <p>
  * The DBA can also be complemented on the fly 
  * (by modifying the acceptance condition of the DRA). The resulting DRA can then be
  * regarded as a Streett automaton of the original DBA.
  * @param nba the NBA, the transitions have to be deterministic!
  * @param complement complement the DBA?
  * @return a shared_ptr to the created DRA
  */   public static DRA dba2dra(NBA nba)
        {
           return dba2dra(nba, false);
        }

        public static DRA dba2dra(NBA nba, bool complement)
        { 
            APSet ap_set = nba.getAPSet(); ;

            DRA dra_p = new DRA(ap_set);

            dba2dra(nba, dra_p, complement);
            return dra_p;
        }

        /** 
         * Internal helper function to perform the conversion from NBA to DRA.
         * @param nba the NBA (has to be deterministic)
         * @param dra_result the DRA into which the converted automaton is saved
         * @param complement complement the DBA?
         */
        public static void dba2dra(NBA nba, DRA dra_result)
        {
            dba2dra(nba, dra_result, false);
        }
        public static void dba2dra(NBA nba, DRA dra_result, bool complement)
        { 
            //complement=false
            DRA dra = dra_result;
            APSet ap_set = dra.getAPSet(); ;

            dra.acceptance().newAcceptancePair();

            for (int i = 0; i < nba.size(); i++)
            {
                dra.newState();

                if (complement)
                {
                    // Final states -> U_0, all states -> L_0
                    if (nba[i].isFinal())
                    {
                        dra.acceptance().stateIn_U(0, i);
                    }
                    dra.acceptance().stateIn_L(0, i);
                }
                else
                {
                    // Final states -> L_0, U_0 is empty
                    if (nba[i].isFinal())
                    {
                        dra.acceptance().stateIn_L(0, i);
                    }
                }
            }

            if (nba.getStartState() != null)
            {
                dra.setStartState(dra[nba.getStartState().getName()]);
            }

            DA_State sink_state = null;

            for (int i = 0; i < nba.size(); i++)
            {
                NBA_State nba_state = nba[i];
                DA_State dra_from = dra[i];

                //for (APSet::element_iterator label=ap_set->all_elements_begin();label!=ap_set->all_elements_end(); ++label) 
                for (int label = ap_set.all_elements_begin(); label != ap_set.all_elements_end(); ++label)
                {
                    BitSet to = nba_state.getEdge(new APElement(label));

                    int to_cardinality = 0;
                    if (to != null)
                    {
                        to_cardinality = to.cardinality();
                    }


                    DA_State dra_to = null;
                    if (to == null || to_cardinality == 0)
                    {
                        // empty to -> go to sink state
                        if (sink_state == null)
                        {
                            // we have to create the sink
                            sink_state = dra.newState();

                            // if we complement, we have to add the sink to
                            // L_0 
                            if (complement)
                            {
                                sink_state.acceptance().addTo_L(0);
                            }
                        }
                        dra_to = sink_state;
                    }
                    else if (to_cardinality == 1)
                    {
                        int to_index = to.nextSetBit(0);

                        //	  std::cerr << "to: " << to_index << std::endl;

                        dra_to = dra[to_index];
                    }
                    else
                    {
                        // to_cardinality>1 !
                        throw new IllegalArgumentException("NBA is no DBA!");
                    }

                    dra_from.edges().addEdge(new APElement(label), dra_to);
                }
            }

            if (sink_state != null)
            {
                // there is a sink state
                // make true-loop from sink state to itself
                //for (APSet::element_iterator label=ap_set->all_elements_begin();label!=ap_set->all_elements_end();++label) {
                for (int label = ap_set.all_elements_begin(); label != ap_set.all_elements_end(); ++label)
                {
                    sink_state.edges().addEdge(new APElement(label), sink_state);
                }
            }
        }
    }
}
