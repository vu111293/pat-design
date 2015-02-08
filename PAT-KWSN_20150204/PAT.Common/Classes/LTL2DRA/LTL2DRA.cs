using System;
using System.Diagnostics;
using PAT.Common.Classes.BA;
using PAT.Common.Classes.LTL2DRA.exception;

namespace PAT.Common.Classes.LTL2DRA
{
    public class LTL2DRA
    {

        /** The options for Safra's algorithm */
        private Options_Safra _safra_opt;

        /** The wrapper for the external LTL-to-Buechi translator */
        //private LTL2NBA _ltl2nba;

        /** Get the options for Safra's algorithm */
        public Options_Safra getOptions()
        {
            return _safra_opt;
        }


        public LTL2DRA(Options_Safra safra_opt) //, LTL2NBA ltl2nba
        {
            _safra_opt = safra_opt;
            //_ltl2nba = ltl2nba;
        }

        /**
 * Convert an LTL formula to a DRA.
 * @param ltl the LTL formula
 * @param options which operators are allowed
 * @return a shared_ptr to the DRA
 */
        private DRA ltl2dra(LTLFormula ltl, BuchiAutomata buchiAutomata,  LTL2DSTAR_Options options)
        {
            APSet ap_set = ltl.getAPSet();

            LTLFormula ltl_pnf = ltl.toPNF();

            if (options.allow_union && ltl_pnf.getRootNode().getType() == type_t.T_OR)
            {
                LTLFormula ltl_left = ltl_pnf.getSubFormula(ltl_pnf.getRootNode().getLeft());

                LTLFormula ltl_right = ltl_pnf.getSubFormula(ltl_pnf.getRootNode().getRight());

                LTL2DSTAR_Options rec_opt = options;
                rec_opt.recursion();

                DRA dra_left = ltl2dra(ltl_left, buchiAutomata, rec_opt);
                DRA dra_right = ltl2dra(ltl_right,buchiAutomata, rec_opt);

                return DRA.calculateUnion(dra_left, dra_right, _safra_opt.union_trueloop) as DRA;
            }

            if (options.safety)
            {
                LTLSafetyAutomata lsa = new LTLSafetyAutomata();

                DRA safety_dra = lsa.ltl2dra(ltl, buchiAutomata);

                if (safety_dra != null)
                {
                    return safety_dra;
                }
            }

            DRA dra = new DRA(ap_set);

            NBA nba = LTL2NBA.ltl2nba(ltl_pnf, buchiAutomata);

            if (nba == null)
            {
                throw new Exception("Couldn't create NBA from LTL formula");
            }

            NBA2DRA nba2dra = new NBA2DRA(_safra_opt);

            nba2dra.convert(nba, dra);

            if (options.optimizeAcceptance)
            {
                dra.optimizeAcceptanceCondition();
            }

            if (options.bisim)
            {
                DRAOptimizations dra_optimizer = new DRAOptimizations();
                dra = dra_optimizer.optimizeBisimulation(dra);
            }

            return dra;
        }


/**
 * Convert an LTL formula to an NBA using the specified LTL2NBA translator
 * @param ltl the formula
 * @param exception_on_failure if false, on error a null pointer is returned
 * @return a shared_ptr to the created NBA
 */
        public NBA ltl2nba(LTLFormula ltl, BuchiAutomata buchiAutomata)
         {
             return ltl2nba(ltl, buchiAutomata, false);
         }

         public NBA ltl2nba(LTLFormula ltl, BuchiAutomata buchiAutomata, bool exception_on_failure)
        {
            //Debug.Assert(_ltl2nba != null);

            NBA nba = LTL2NBA.ltl2nba(ltl, buchiAutomata);

            if (exception_on_failure && nba == null)
            {
                throw new Exception("Couldn't generate NBA from LTL formula!");
            }

            return nba;
        }

        /**
 * Convert an NBA to a DRA using Safra's algorithm.
 * If limit is specified (>0), the conversion is 
 * aborted with LimitReachedException when the number of 
 * states exceeds the limit.
 * @param nba the formula
 * @param limit a limit on the number of states (0 for no limit)
 * @param detailedStates save detailed interal information (Safra trees) 
 *                       in the generated states
 * @param stutter_information Information about the symbols that can be stuttered
 * @return a shared_ptr to the created DRA
 */
        public DRA nba2dra(NBA nba, int limit, bool detailedStates, StutterSensitivenessInformation stutter_information)
        {
            DRA dra = new DRA(nba.getAPSet_cp());

            NBA2DRA nba2dra = new NBA2DRA(_safra_opt, detailedStates, stutter_information);

            try
            {
                nba2dra.convert(nba, dra, limit);
            }
            catch (LimitReachedException e)
            {
                //dra.reset();
                // rethrow to notify caller
                //throw;
            }

            return dra;
        }


    }
}
