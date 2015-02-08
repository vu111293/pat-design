using System;
using ltl2ba;
using PAT.Common.Classes.BA;

namespace PAT.Common.Classes.LTL2DRA
{
    public class BA2DRAConverter
    {



        /** The output format */
        //enum {OUT_v2, OUT_NBA, OUT_DOT, OUT_PLUGIN} flag_output;
        



        public static DRA ConvertBA2DRA(BuchiAutomata BA, Node LTLHeadNode)
        {

            /** Flag: Convert LTL->DRA->NBA? */
            //bool flag_dra2nba;

            /** Flag: Print the NBA afert LTL->NBA? */
            //bool flag_print_ltl_nba;

            /** Flag: Use limiting with scheduler? */
            bool flag_sched_limits;

            /** The limiting factor for the scheduler (alpha) */
            double alpha;

            /** The options for Safra's algorithm */
            Options_Safra opt_safra = new Options_Safra();

            /** The options for LTL2DSTAR */
            LTL2DSTAR_Options opt_ltl2dstar = new LTL2DSTAR_Options();

            opt_ltl2dstar.opt_safra = opt_safra;

            //            std::map<std::string, std::string> defaults;
            //defaults["--ltl2nba"]="--ltl2nba=spin:ltl2ba";
            //defaults["--automata"]="--automata=rabin";
            //defaults["--output"]="--output=automaton";
            //defaults["--detailed-states"]="--detailed-states=no";
            //defaults["--safra"]="--safra=all";
            //defaults["--bisimulation"]="--bisimulation=yes";
            //defaults["--opt-acceptance"]="--opt-acceptance=yes";
            //defaults["--union"]="--union=yes";
            //defaults["--alpha"]="--alpha=10.0";
            //defaults["--stutter"]="--stutter=yes";
            //defaults["--partial-stutter"]="--partial-stutter=no";
            ////      defaults["--scheck"]=""; // scheck disabled
            /// 
            /// 
            // default values...
            //flag_dra2nba = false;
            flag_sched_limits = false;

            alpha = 1.0;

            // options not yet covered
            //flag_print_ltl_nba = false;
            //flag_stat_nba = false;

            //if (isRabin)
            //{
            opt_ltl2dstar.automata = automata_type.RABIN;
            //}
            //else
            //{
            //    opt_ltl2dstar.automata = automata_type.STREETT;
            //}
            opt_safra.opt_all();
            opt_safra.stutter = false;
            opt_ltl2dstar.bisim = false;


            opt_safra.opt_rename = false;

            LTLFormula ltl = LTLPrefixParser.parse(LTLHeadNode);
            //APSet ap_set = ltl.getAPSet();

            //Debug.Assert(ltl2nba != null);
            LTL2DRA ltl2dra = new LTL2DRA(opt_safra); //, ltl2nba.get()


            //if (opt_ltl2dstar.automata == automata_type.ORIGINAL_NBA)
            //{
            //    // We just generate the NBA for the LTL formula
            //    // and print it

            //    NBA nba = ltl2dra.ltl2nba(ltl);
            //    if (nba == null)
            //    {
            //        throw new Exception("Can't generate NBA for LTL formula");
            //    }

            //    //if (flag_output==OUT_DOT) {
            //    //  nba->print_dot(out);
            //    //} else {
            //    //  nba->print_lbtt(out);
            //    //}
            //    //return 0;
            //}



            LTL2DSTAR_Scheduler ltl2dstar_sched = new LTL2DSTAR_Scheduler(ltl2dra, flag_sched_limits, alpha);

            //ltl2dstar_sched.flagStatNBA(flag_stat_nba);
            ltl2dstar_sched.flagStatNBA(false);

            opt_ltl2dstar.opt_safra = opt_safra;
            DRA dra = ltl2dstar_sched.calculate(ltl, BA, opt_ltl2dstar);

            //if (!dra.isCompact()) {
            //  dra.makeCompact();
            //}

            if (dra == null)
            {
                throw new Exception("Couldn't generate DRA!");
            }

            //if (!dra.isCompact()) {
            //  dra.makeCompact();
            //}


            return dra;
        }
    }
}
