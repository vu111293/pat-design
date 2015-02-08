using PAT.Common.Classes.BA;
using PAT.Common.Classes.LTL2DRA.common;

namespace PAT.Common.Classes.LTL2DRA
{
    public class StutterSensitivenessInformation
    {

        public bool _completelyStutterInsensitive;
        public bool _partiallyStutterInsensitive;

        public bool _hasCheckedLTL;
        public bool _hasCheckedNBAs;
        public BitSet _partiallyInsensitive;

        public static bool _enableTimekeeping;
        public static bool _printInfo;
        //public static TimeKeeper _timekeeper;



        /** Constructor */
        public StutterSensitivenessInformation()
        {
            _completelyStutterInsensitive = false;
            _partiallyStutterInsensitive = false;
            _hasCheckedLTL = false;
            _hasCheckedNBAs = false;
        }

        /** Check the LTLFormula syntacticly for
        *  stutter insensitiveness */
        //public void checkLTL(bool hasNextStep) //LTLFormula ltl
        public void checkLTL(LTLFormula ltl) //
        {
            if (ltl.hasNextStep())
            //if(hasNextStep)
            {
                _completelyStutterInsensitive = false;
                _partiallyStutterInsensitive = false;
            }
            else
            {
                _completelyStutterInsensitive = true;
                _partiallyStutterInsensitive = false;
            }

            _hasCheckedLTL = true;
        }

        /** Check for partial stutter insensitiveness using 
  *  the nba and the complement nba */

        public void checkNBAs(NBA nba, NBA nba_complement)
        {

            APSet apset = nba.getAPSet_cp();

            bool nba_is_smaller = (nba.size() < nba_complement.size());

            //if (_printInfo) {
            //  std::cerr << "Checking for insensitiveness" << std::endl;
            //}
            bool one_insensitive = false;
            bool all_insensitive = true;
            //for (APSet::element_iterator it=apset->all_elements_begin(); it!=apset->all_elements_end();++it) 
            for (int it = apset.all_elements_begin(); it != apset.all_elements_end(); ++it)
            {
                APElement elem = new APElement(it);

                if (_partiallyInsensitive.get(it))
                {
                    // don't recheck something we already now is stutter insensitive
                    one_insensitive = true;
                    continue;
                }

                //  if (_printInfo) {
                //std::cerr << "Checking " << elem.toString(*apset) << ": ";
                //std::cerr.flush();
                //  }

                bool insensitive;
                if (nba_is_smaller)
                {
                    insensitive = is_stutter_insensitive(nba, nba_complement, elem);
                }
                else
                {
                    insensitive = is_stutter_insensitive(nba_complement, nba, elem);
                }
                if (insensitive)
                {
                    _partiallyInsensitive.set(it);
                    one_insensitive = true;
                    //if (_printInfo) {
                    //  std::cerr << "+" << std::endl;
                    //}
                }
                else
                {
                    all_insensitive = false;
                    //if (_printInfo) {
                    //  std::cerr << "-" << std::endl;
                    //}
                }
            }
            _hasCheckedNBAs = true;
            _partiallyStutterInsensitive = one_insensitive;


        }

        /** Check for partial stutter insensitiveness for a LTL formula.
 *  @param ltl the LTL formula
 *  @param llt2nba the LTL2NBA translator, has to provide function ltl2nba(ltl)
 */
        public void checkPartial(LTLFormula ltl, BuchiAutomata ba, LTL2DRA ltl2nba)
        {
            checkNBAs(ltl2nba.ltl2nba(ltl, ba), ltl2nba.ltl2nba(ltl.negate().toPNF(), ba));//true
        }

        /** Check for partial stutter insensitiveness for a LTL formula, using an 
 *  already calculated NBA.
 *  @param nba an NBA for the positive formula
 *  @param ltl_neg the negated LTL formula (in PNF)
 *  @param llt2nba the LTL2NBA translator, has to provide function ltl2nba(ltl)
 */

        public void checkPartial(NBA nba, BuchiAutomata ba, LTLFormula ltl_neg, LTL2DRA ltl2nba)
        {
            checkNBAs(nba, ltl2nba.ltl2nba(ltl_neg, ba));
        }


        /** Return true iff all symbols are completely stutter
  * insensitive. */
        public bool isCompletelyInsensitive()
        {
            return _completelyStutterInsensitive;
        }

        /** Return true iff some (or all) symbols are stutter insensitive,
         *  result is undefined if isCompletelyInsensitive()==true
         */
        public bool isPartiallyInsensitive()
        {
            return _partiallyStutterInsensitive;
        }

        /** Get the set of the stutter insensitive symbols from 2^APSet */
        public BitSet getPartiallyInsensitiveSymbols()
        {
            return _partiallyInsensitive;
        }




        /** Static, enable printing of verbose information */
        public static void enablePrintInfo()
        {
            _printInfo = true;
        }

        // -- private member functions

        /** Check that symbol label is stutter insensitive, 
         *  using nba and complement_nba */
        public bool is_stutter_insensitive(NBA nba, NBA nba_complement, APElement label)
        {

            NBA stutter_closed_nba = NBAStutterClosure.stutter_closure(nba, label);

            NBA product = NBA.product_automaton(stutter_closed_nba, nba_complement);

            NBAAnalysis analysis_product = new NBAAnalysis(product);
            bool empty = analysis_product.emptinessCheck();
            //  std::cerr << "NBA is " << (empty ? "empty" : "not empty") << std::endl;

            return empty;
        }

    }

}
