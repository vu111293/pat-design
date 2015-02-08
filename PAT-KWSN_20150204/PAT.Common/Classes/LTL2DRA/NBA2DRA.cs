using PAT.Common.Classes.LTL2DRA.common;

namespace PAT.Common.Classes.LTL2DRA
{
    public class NBA2DRA
    {

        /** The options */
        private Options_Safra _options;
        /** Save detailed information on the Safra trees in the states? */
        private bool _detailed_states;

        // stuttering
        StutterSensitivenessInformation _stutter_information;

        /** Constructor */
        public NBA2DRA()
        {
            _detailed_states = false;
            ; // nop
        }

        /** Constructor.
 * @param options Options_Safra specifying whether to stutter, etc...
 * @param detailedStates generate detailed descriptions for the states?
 * @param stutter_information Information about the symbols that may be stuttered
 */
        //,=false StutterSensitivenessInformation::ptr stutter_information=StutterSensitivenessInformation::ptr()
        public NBA2DRA(Options_Safra options)
            : this(options, false, new StutterSensitivenessInformation())
        {

        }
        public NBA2DRA(Options_Safra options, bool detailedStates, StutterSensitivenessInformation stutter_information)
        {
            ; // nop
            _options = options;
            _detailed_states = detailedStates;
            _stutter_information = stutter_information;
        }

          /**
          * Convert an NBA to an DRA (having APElements as edge labels).
          * Throws LimitReachedException if a limit is set (>0) and
          * there are more states in the generated DRA than the limit. 
          * @param nba the NBA
          * @param dra_result the DRA where the result is stored 
          *        (has to have same APSet as the nba)
          * @param limit a limit for the number of states (0 disables the limit).
          */
        //template < typename NBA_t, typename DRA_t>
        public void convert(NBA nba, DRA dra_result)  //=0
        {
            convert(nba, dra_result, 0);
        }
        public void convert(NBA nba, DRA dra_result, int limit)  //=0
        {
            if (nba.size() == 0 || nba.getStartState() == null)
            {
                // the NBA is empty -> construct DRA that is empty

                dra_result.constructEmpty();
                return;
            }

            if (_options.dba_check && nba.isDeterministic())
            {
                DBA2DRA.dba2dra(nba, dra_result);
                return;
            }

            if (_options.stutter_closure)
            {
                if (_stutter_information != null && !_stutter_information.isCompletelyInsensitive())
                {
                    //std::cerr <<
                    //"WARNING: NBA might not be 100% stutter insensitive, applying stutter closure can create invalid results!" <<
                    //std::endl;
                }

                NBA nba_closed = NBAStutterClosure.stutter_closure(nba);

                if (can_stutter())
                {
                    convert_safra_stuttered(nba_closed, dra_result, limit);
                    return;
                }

                convert_safra(nba_closed, dra_result, limit);
                return;
            }


            if (can_stutter())
            {
                convert_safra_stuttered(nba, dra_result, limit);
                return;
            }

            convert_safra(nba, dra_result, limit);

            return;

        }

        /** 
 * Is stuttering allowed?
 */
        public bool can_stutter()
        {
            if (_stutter_information == null)
            {
                return false;
            }

            if (_options.stutter && _stutter_information.isCompletelyInsensitive())
            {
                return true;
            }

            if (_options.stutter && _stutter_information.isPartiallyInsensitive())
            {
                return true;
            }

            return false;
        }



        /**
   * Convert the NBA to a DRA using Safra's algorithm
   * @param nba the NBA
   * @param dra_result the result DRA
   * @param limit limit for the size of the DRA
   */
        public void convert_safra(NBA nba, DRA dra_result, int limit)  //=0
        {

            SafraAlgorithm safras_algo = new SafraAlgorithm(nba, _options);

            if (!_options.opt_rename)
            {
                NBA2DA nba2da = new NBA2DA(_detailed_states);

                nba2da.convert(safras_algo, dra_result, limit);
            }
            else
            {
                //typedef typename SafrasAlgorithm<NBA_t>::result_t result_t;
                //typedef typename SafrasAlgorithm<NBA_t>::state_t key_t;

                //<safra_t,DRA_t, StateMapperFuzzy<result_t, key_t, typename DRA_t::state_type, SafraTreeCandidateMatcher> >
                NBA2DA nba2da_fuzzy = new NBA2DA(_detailed_states);

                nba2da_fuzzy.convert(safras_algo, dra_result, limit);
            }
        }

        /**
 * Convert the NBA to a DRA using Safra's algorithm, using stuttering
 * @param nba the NBA
 * @param dra_result the result DRA
 * @param limit limit for the size of the DRA
 */
        //template < typename NBA_t, typename DRA_t >
        void convert_safra_stuttered(NBA nba, DRA dra_result, int limit)  //=0
        {

            SafraAlgorithm safras_algo = new SafraAlgorithm(nba, _options);

            StutteredNBA2DA nba2dra_stuttered = new StutteredNBA2DA(_detailed_states, _stutter_information);

            nba2dra_stuttered.convert(safras_algo, dra_result, limit);
        }


    }


    /**
     * Provides CandidateMatcher for SafraTrees
     */
    public class SafraTreeCandidateMatcher
    {

        public static bool isMatch(SafraTreeTemplate temp, SafraTree tree)
        {
            return temp.matches(tree);
        }

        public static bool abstract_equal_to(SafraTree t1, SafraTree t2)
        {
            return t1.structural_equal_to(t2);
        }

        public static bool abstract_less_than(SafraTree t1, SafraTree t2)
        {
            return t1.structural_less_than(t2);
        }

        //template <typename HashFunction>
        public static void abstract_hash_code(StdHashFunction hash, SafraTree t)
        {
            //todo: implemented this method
            t.hashCode(hash, true);
        }
    }
}
