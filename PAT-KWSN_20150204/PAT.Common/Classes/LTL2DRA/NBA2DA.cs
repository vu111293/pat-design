using System.Collections.Generic;
using PAT.Common.Classes.LTL2DRA.exception;

namespace PAT.Common.Classes.LTL2DRA
{
    public class NBA2DA
    {
        /** Save detailed information on the Safra trees in the states? */
        public bool _detailed_states;


        /** Constructor */
        public NBA2DA(bool detailedStates)
        {
            _detailed_states = detailedStates;
            ; // nop
        }
        public NBA2DA()
        {
            _detailed_states = false;
        }

        /**
         * Generate a DA using the Algorithm
         * Throws LimitReachedException if a limit is set (>0) and
         * there are more states in the generated DA than the limit. 
         * @param algo the algorithm 
         * @param da_result the DA where the result is stored 
         *        (has to have same APSet as the nba)
         * @param limit a limit for the number of states (0 disables the limit).
         */
        public void convert(SafraAlgorithm algo, DRA da_result, int limit)
        {
            /** The hash map from DA_State to StateInterface */
            StateMapper<StateInterface, DA_State> state_mapper = new StateMapper<StateInterface, DA_State>();
            APSet ap_set = da_result.getAPSet();////////////************where dose this dra have ap_set?


            if (algo.checkEmpty())
            {
                da_result.constructEmpty();
                return;
            }

            //typedef typename DA_t::state_type da_state_t;
            //typedef typename Algorithm_t::state_t algo_state_t;
            //typedef typename Algorithm_t::result_t algo_result_t;

            //* Creates new acceptance pairs according to da_result's acceptance. 
            algo.prepareAcceptance(da_result.acceptance());////*********don't understand well

            StateInterface start = algo.getStartState();
            DA_State start_state = da_result.newState();/////************?? what is in this newState?
            start.generateAcceptance(start_state.acceptance());
            if (_detailed_states)
            {
                start_state.setDescription(start.toHTML());
            }

            state_mapper.add(start, start_state);
            da_result.setStartState(start_state);

            //typedef std::pair<algo_state_t, da_state_t*> unprocessed_value;

            Stack<KeyValuePair<StateInterface, DA_State>> unprocessed = new Stack<KeyValuePair<StateInterface, DA_State>>();
            unprocessed.Push(new KeyValuePair<StateInterface, DA_State>(start, start_state));

            while (unprocessed.Count > 0)
            {
                KeyValuePair<StateInterface, DA_State> top = unprocessed.Pop();
                //unprocessed.pop();      

                StateInterface cur = top.Key;//safratreeNode
                DA_State from = top.Value;//DA_state

                //for (APSet::element_iterator it_elem = ap_set.all_elements_begin(); it_elem != ap_set.all_elements_end(); ++it_elem)
                for (int it_elem = ap_set.all_elements_begin(); it_elem != ap_set.all_elements_end(); ++it_elem)///from 0 to 2^ap_set.size
                {
                    APElement elem = new APElement(it_elem);/////////set simpleBitset = it_elem

                    ResultStateInterface<SafraTree> result = algo.delta(cur as SafraTree, elem);/////get a new safraTree through elem


                    DA_State to = state_mapper.find(result.getState());

                    if (to == null)////////////////result is not in state mapper.
                    {
                        to = da_result.newState();
                        result.getState().generateAcceptance(to.acceptance());

                        if (_detailed_states)
                        {
                            to.setDescription(result.getState().toHTML());
                        }

                        state_mapper.add(result.getState(), to);
                        unprocessed.Push(new KeyValuePair<StateInterface, DA_State>(result.getState(), to));
                    }

                    from.edges().set(elem, to);//////////////add this edge.
                    if (limit != 0 && da_result.size() > limit)
                    {
                        throw new LimitReachedException("");
                    }
                }
            }
        }

        /**
 * Generate a DA using the Algorithm
 * Throws LimitReachedException if a limit is set (>0) and
 * there are more states in the generated DA than the limit. 
 * @param algo the algorithm 
 * @param da_result the DA where the result is stored 
 *        (has to have same APSet as the nba)
 * @param limit a limit for the number of states (0 disables the limit).
 */
        public void convert(DAUnionAlgorithm algo, DRA da_result, int limit)
        {
            StateMapper<StateInterface, DA_State> state_mapper = new StateMapper<StateInterface, DA_State>();
            APSet ap_set = da_result.getAPSet();

            if (algo.checkEmpty())
            {
                da_result.constructEmpty();
                return;
            }

            //typedef typename DA_t::state_type da_state_t;
            //typedef typename Algorithm_t::state_t algo_state_t;
            //typedef typename Algorithm_t::result_t algo_result_t;

            algo.prepareAcceptance(da_result.acceptance());

            StateInterface start = algo.getStartState();
            DA_State start_state = da_result.newState();
            start.generateAcceptance(start_state.acceptance());
            if (_detailed_states)
            {
                start_state.setDescription(start.toHTML());                
            }

            state_mapper.add(start, start_state);
            da_result.setStartState(start_state);

            //typedef std::pair<algo_state_t, da_state_t*> unprocessed_value;

            Stack<KeyValuePair<StateInterface, DA_State>> unprocessed = new Stack<KeyValuePair<StateInterface, DA_State>>();
            unprocessed.Push(new KeyValuePair<StateInterface, DA_State>(start, start_state));

            while (unprocessed.Count > 0)
            {
                KeyValuePair<StateInterface, DA_State> top = unprocessed.Pop();
                //unprocessed.pop();      

                StateInterface cur = top.Key;
                DA_State from = top.Value;

                //for (APSet::element_iterator it_elem = ap_set.all_elements_begin(); it_elem != ap_set.all_elements_end(); ++it_elem)
                for (int it_elem = ap_set.all_elements_begin(); it_elem != ap_set.all_elements_end(); ++it_elem)
                {
                    APElement elem = new APElement(it_elem);

                    ResultStateInterface<UnionState> result = algo.delta(cur as UnionState, elem);


                    DA_State to = state_mapper.find(result.getState());

                    if (to == null)
                    {
                        to = da_result.newState();
                        result.getState().generateAcceptance(to.acceptance());

                        if (_detailed_states)
                        {
                            to.setDescription(result.getState().toHTML());                            
                        }

                        state_mapper.add(result.getState(), to);
                        unprocessed.Push(new KeyValuePair<StateInterface, DA_State>(result.getState(), to));
                    }

                    from.edges().set(elem, to);

                    if (limit != 0 && da_result.size() > limit)
                    {
                        throw new LimitReachedException("");
                    }
                }
            }

        }
    }
}
