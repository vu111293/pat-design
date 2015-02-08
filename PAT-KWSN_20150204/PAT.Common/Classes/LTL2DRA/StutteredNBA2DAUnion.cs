using System;
using System.Collections.Generic;
using System.Diagnostics;
using PAT.Common.Classes.LTL2DRA;
using PAT.Common.Classes.LTL2DRA.common;

namespace PAT.Common.Classes.LTL2DRA
{





    /** Calculate stuttered DA_t given Algorithm_t */
    //template <typename Algorithm_t,typename DA_t>
    public class StutteredNBA2DAUnion
    {

        /** Generate detailed descriptions for the states? */
        public bool _detailed_states;
        /** Information which symbols may be stuttered */
        public StutterSensitivenessInformation _stutter_information;


        //typedef typename DA_t::acceptance_condition_type Acceptance;

        /** Constructor.
         * detailed_states Generate detailed descriptions for the states? 
         * stutter_information Information which symbols may be stuttered 
         */
        public StutteredNBA2DAUnion(bool detailed_states, StutterSensitivenessInformation stutter_information)
        {
            _detailed_states = detailed_states;
            _stutter_information = stutter_information;
            Debug.Assert(_stutter_information != null);
        }



        /**
         * Perform the stuttered conversion.
         * Throws LimitReachedException if a limit is set (>0) and
         * there are more states in the generated DRA than the limit. 
         * @param algo the underlying algorithm to be used
         * @param da_result the DRA where the result is stored 
         *        (has to have same APSet as the nba)
         * @param limit a limit for the number of states (0 disables the limit).
         */
        public void convert(DAUnionAlgorithm algo, DA da_result, int limit)
        {
            StutteredConvertorUnion conv = new StutteredConvertorUnion(algo, da_result as DRA, limit, _detailed_states, _stutter_information);
            conv.convert();
        }

    }


    /** 
     * Converting an NBA using Algorithm_t into a DA
     */
    public class StutteredConvertorUnion
    {


        // ---- members ----
        /** The result DA */
        public DRA _da_result;
        /** Limit for the number of states */
        public int _limit;
        /** The algorithm to use */
        public DAUnionAlgorithm _algo;
        /** Generate detailed descriptions? */
        public bool _detailed_states;
        /** A state mapper for the already generated states */
        StateMapper<TreeWithAcceptance, DA_State> _state_mapper; //int, 
        /** Information about which symbols are safe to stutter */
        StutterSensitivenessInformation _stutter_information;
        /** A stack for the unprocessed states */
        Stack<KeyValuePair<TreeWithAcceptance, DA_State>> _unprocessed;


        /** Constructor.
         * @param algo The Algorithm_t to use
         * @param da_result The result automaton
         * @param limit Limit the number of states in the result automaton?
         * @param detailed_states Generate detailed descriptions?
         * @param stutter_information Information about which symbols may be stuttered
         */
        public StutteredConvertorUnion(DAUnionAlgorithm algo, DRA da_result, int limit, bool detailed_states, StutterSensitivenessInformation stutter_information)
        {
            _da_result = da_result;
            _limit = limit;
            _algo = algo;
            _detailed_states = detailed_states;
            _stutter_information = stutter_information;

            //added by ly
            _state_mapper = new StateMapper<TreeWithAcceptance, DA_State>();
            _unprocessed = new Stack<KeyValuePair<TreeWithAcceptance, DA_State>>();
        }

        //typedef typename DA_t::state_type da_state_t;
        //typedef typename Algorithm_t::state_t algo_state_t;
        //typedef typename Algorithm_t::result_t algo_result_t;
        //typedef TreeWithAcceptance<algo_state_t, typename Acceptance::signature_type> stuttered_state_t;
        //typedef typename stuttered_state_t::ptr stuttered_state_ptr_t;

        //typedef std::pair<stuttered_state_ptr_t, da_state_t*> unprocessed_value_t;
        //typedef std::stack<unprocessed_value_t> unprocessed_stack_t;

        /** Convert the NBA to the DA */
        public void convert()
        {
            APSet ap_set = _da_result.getAPSet();

            if (_algo.checkEmpty())
            {
                _da_result.constructEmpty();
                return;
            }

            _algo.prepareAcceptance(_da_result.acceptance());

            TreeWithAcceptance s_start = new TreeWithAcceptance(_algo.getStartState());
            DA_State start_state = _da_result.newState();
            s_start.generateAcceptance(start_state.acceptance());
            if (_detailed_states)
            {
                //start_state->setDescription(s_start->toHTML());
                start_state.setDescription("hahahahah");
            }

            _state_mapper.add(s_start, start_state);
            _da_result.setStartState(start_state);

            Stack<KeyValuePair<TreeWithAcceptance, DA_State>> unprocessed;
            _unprocessed.Push(new KeyValuePair<TreeWithAcceptance, DA_State>(s_start, start_state));

            bool all_insensitive = _stutter_information.isCompletelyInsensitive();
            BitSet partial_insensitive = _stutter_information.getPartiallyInsensitiveSymbols();

            while (_unprocessed.Count > 0)
            {
                KeyValuePair<TreeWithAcceptance, DA_State> top = _unprocessed.Pop();
                //_unprocessed.pop();      

                TreeWithAcceptance from = top.Key;
                DA_State da_from = top.Value;

                //for (APSet::element_iterator it_elem=ap_set.all_elements_begin();it_elem!=ap_set.all_elements_end();++it_elem) 
                for (int it_elem = ap_set.all_elements_begin(); it_elem != ap_set.all_elements_end(); ++it_elem)
                {
                    APElement elem = new APElement(it_elem);

                    if (da_from.edges().get(elem) == null)
                    {
                        // the edge was not yet calculated...

                        if (!all_insensitive && partial_insensitive.get(it_elem) == null)
                        {

                            // can't stutter for this symbol, do normal step
                            UnionState next_tree = _algo.delta(from.getTree() as UnionState, elem).getState();
                            TreeWithAcceptance next_state = new TreeWithAcceptance(next_tree);
                            add_transition(da_from, next_state, elem);

                            continue;
                        }

                        // normal stuttering...

                        calc_delta(from, da_from, elem);

                        if (_limit != 0 && _da_result.size() > _limit)
                        {
                            //THROW_EXCEPTION(LimitReachedException, "");
                            throw new Exception("LimitReachedException");

                        }
                    }
                }
            }
        }



        // --- private functions ----

        /** Add a transition from DA states da_from to stuttered_state to for edge elem.
         * If the state does not yet exist in the DA, create it.
         * @param da_from the from state in the DA
         * @param to the target state 
         * @param elem the edge label
         */
        DA_State add_transition(DA_State da_from, TreeWithAcceptance to, APElement elem)
        {
            DA_State da_to = _state_mapper.find(to);

            if (da_to == null)
            {
                da_to = _da_result.newState();
                to.generateAcceptance(da_to.acceptance());
                if (_detailed_states)
                {
                    //    da_to.setDescription(to->toHTML());
                }

                _state_mapper.add(to, da_to);
                _unprocessed.Push(new KeyValuePair<TreeWithAcceptance, DA_State>(to, da_to));
            }

#if STUTTERED_VERBOSE
      std::cerr << da_from->getName() << " -> " << da_to->getName() << std::endl;
#endif

            da_from.edges().set(elem, da_to);

            return da_to;
        }

        //typedef std::vector<algo_state_t> intermediate_state_vector_t;

        /**
         * Calculate Acceptance for RabinAcceptance conditon
         */
        private bool calculate_acceptance(List<StateInterface> state_vector, int cycle_point, RabinSignature prefix_signature, RabinSignature cycle_signature)
        {
            int states = state_vector.Count;

            state_vector[cycle_point].generateAcceptance(cycle_signature); // start
            for (int i = cycle_point + 1; i < states; i++)
            {
                cycle_signature.maxMerge(state_vector[i].generateAcceptance());
            }

            if (prefix_signature != null)
            {
                prefix_signature = cycle_signature;
                for (int i = 1; i < cycle_point; i++)
                {
                    prefix_signature.maxMerge(state_vector[i].generateAcceptance());
                }
            }

            if (prefix_signature != null)
            {
                // check if prefix can be ommited
                RabinSignature p0_signature = new RabinSignature(prefix_signature.getSize());
                state_vector[0].generateAcceptance(p0_signature);

                for (int j = 0; j < prefix_signature.getSize(); j++)
                {
                    if (prefix_signature.getColor(j) <= cycle_signature.getColor(j) ||
                        prefix_signature.getColor(j) <= p0_signature.getColor(j))
                    {
                        // acceptance pair j is ok
                        ;
                    }
                    else
                    {
                        return false;
                    }
                }
                // all acceptance pairs are ok, return true
                return true;
            }

            return false;
        }


        /** Store a prefix and a cycle state */
        public struct prefix_and_cycle_state_t
        {

            public TreeWithAcceptance prefix_state;
            public TreeWithAcceptance cycle_state;

            public prefix_and_cycle_state_t(TreeWithAcceptance prefix_, TreeWithAcceptance cycle_)
            {
                prefix_state = prefix_;
                cycle_state = cycle_;
            }

        }


        /** Calculate the prefix and the cycle state */
        prefix_and_cycle_state_t calculate_prefix_and_cycle_state(List<StateInterface> state_vector, int cycle_point)
        {
            //TreeWithAcceptance prefix_state, cycle_state; 
            TreeWithAcceptance prefix_state = null;
            TreeWithAcceptance cycle_state = null;
            int states = state_vector.Count; //.size();

            int smallest = cycle_point;
            for (int i = cycle_point + 1; i < states; i++)
            {
                if (state_vector[i] < state_vector[smallest])
                {
                    smallest = i;
                }
            }

#if STUTTERED_VERBOSE
      std::cerr << "Smallest: " << smallest << std::endl;
#endif

            cycle_state = new TreeWithAcceptance(state_vector[smallest]);
            if (!(cycle_point == 0 || cycle_point == 1))
            {
                prefix_state = new TreeWithAcceptance(state_vector[smallest]);
            }

            RabinSignature signature_prefix = null; //(typename Acceptance::signature_type*)NULL;
            if (prefix_state != null)
            {
                signature_prefix = prefix_state.getSignature();
            }
            RabinSignature signature_cycle = cycle_state.getSignature();


            bool omit_prefix = calculate_acceptance(state_vector, cycle_point, signature_prefix, signature_cycle);

            if (omit_prefix)
            {
                //prefix_state.reset();
            }

            //  if (_detailed_states) {
            //    std::ostringstream prefix_description;
            //    std::ostringstream cycle_description;

            //if (prefix_state) {
            //  prefix_description << "<TABLE><TR><TD>Prefix</TD><TD>Cycle (" << (smallest-cycle_point) <<")</TD></TR>"
            //             << "<TR><TD>";

            //  prefix_description << "<TABLE><TR>";
            //  for (unsigned int i=1;i<cycle_point;i++) {
            //    prefix_description << "<TD>" << state_vector[i]->toHTML() << "</TD>";
            //  }
            //  prefix_description << "</TR></TABLE></TD>";
            //}

            //cycle_description << "<TD><TABLE><TR>";
            //for (unsigned int i=cycle_point; i<state_vector.size();i++) {	  
            //  cycle_description << "<TD>";
            //  cycle_description << state_vector[i]->toHTML();
            //  cycle_description << "</TD>";
            //}
            //cycle_description << "</TR></TABLE></TD>";


            //if (prefix_state) {
            //  prefix_description << cycle_description.str();
            //  prefix_description << "</TR></TABLE>";

            //  prefix_state->setDescription(prefix_description.str());
            //}

            //cycle_description << "</TR></TABLE>";
            //cycle_state->setDescription("<TABLE><TR><TD>Cycle ("+
            //                boost::lexical_cast<std::string>(smallest-cycle_point) +
            //                ")</TD></TR><TR>" + cycle_description.str());
            //  }

            return new prefix_and_cycle_state_t(prefix_state, cycle_state);
        }


        /** Calculate and add transitions to the successor state.
         * @param from the source stuttered_state 
         * @param da_from the source DA state
         * @param elem the edge label
         */
        void calc_delta(TreeWithAcceptance from, DA_State da_from, APElement elem)
        {
            //StateMapper<SafraTree, int , ptr_hash<algo_state_t>> intermediate_state_map_t;  //, PtrComparator<algo_state_t> 

            Dictionary<StateInterface, int> state_map = new Dictionary<StateInterface, int>();
            List<StateInterface> state_vector = new List<StateInterface>();

            StateInterface start_tree = from.getTree();
            //state_map[start_tree] = null;
            state_map.Add(start_tree, 0);
            state_vector.Add(start_tree); //push_back

#if STUTTERED_VERBOSE
        std::cerr << "Calculate from state [" << da_from->getName() << "], " << (unsigned
        int )
        elem << ":" << std::endl;
        std::cerr << start_tree->toString() << std::endl;
#endif

            StateInterface cur_tree = start_tree;
            while (true)
            {
                StateInterface next_tree = _algo.delta(cur_tree as UnionState, elem).getState();

                //typename intermediate_state_map_t::iterator it;
                //int it = state_map.find(next_tree);
                //if (it == state_map.end())

                if (!state_map.ContainsKey(next_tree))
                {
                    // tree doesn't yet exist...
                    // add tree
                    //state_map[next_tree] = state_vector.size();
                    state_map.Add(next_tree, state_vector.Count);
                    state_vector.Add(next_tree); //push_back

                    cur_tree = next_tree;
                    continue;
                }
                else
                {
                    // found the cycle!
                    int cycle_point = state_map[next_tree];

#if STUTTERED_VERBOSE
	    std::cerr << "-----------------------\n";
	    for (unsigned int i=0;i<state_vector.size();i++) {
	    std::cerr << "[" << i << "] ";
	    if (cycle_point==i) {
	    std::cerr << "* ";
	    }
	    std::cerr << "\n" << state_vector[i]->toString() << std::endl;
	    }
	    std::cerr << "-----------------------\n";
#endif

                    prefix_and_cycle_state_t pac = calculate_prefix_and_cycle_state(state_vector, cycle_point);

                    //DA_State da_prefix = null;
                    DA_State da_cycle = null;

                    if (pac.prefix_state != null && !(pac.prefix_state == pac.cycle_state))
                    {
                        DA_State da_prefix = add_transition(da_from, pac.prefix_state, elem);
                        da_cycle = add_transition(da_prefix, pac.cycle_state, elem);
                    }
                    else
                    {
                        da_cycle = add_transition(da_from, pac.cycle_state, elem);
                    }

                    da_cycle.edges().set(elem, da_cycle);

                    return;
                }
            }
        }
    }



}