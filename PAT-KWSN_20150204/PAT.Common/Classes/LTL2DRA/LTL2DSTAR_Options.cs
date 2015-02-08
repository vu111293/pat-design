namespace PAT.Common.Classes.LTL2DRA
{
    public enum automata_type { STREETT, RABIN, RABIN_AND_STREETT, ORIGINAL_NBA }
    /**
     * Options for the LTL2DSTAR scheduler.
     */
    public class LTL2DSTAR_Options
    {

        /** Constructor */
        public LTL2DSTAR_Options()
        {
            // Defaults...
            allow_union = true;
            recursive_union = true;

            optimizeAcceptance = true;

            bisim = false;
            recursive_bisim = true;

            safety = false;

            automata = automata_type.RABIN;
            only_union = false;
            only_safety = false;

            detailed_states = false;
            verbose_scheduler = false;
        }

        /** Disable all options */
        public void allFalse()
        {
            allow_union
              = recursive_union

              = safety

              = optimizeAcceptance
              = bisim
              = recursive_bisim

              = only_union
              = only_safety
              = detailed_states
              = verbose_scheduler
              = false;
        }

        /** Change options for next level of recursion */
        public void recursion()
        {
            allow_union = allow_union && recursive_union;
            only_union = false;

            bisim = bisim && recursive_bisim;
        }

        /** Safra Options */
        public Options_Safra opt_safra;

        /** Allow union construction */
        public bool allow_union;

        /** Allow union construction on next levels */
        public bool recursive_union;

        /** Allow using scheck for (co-)safety LTL formulas */
        public bool safety;

        /** Allow optimization of acceptance conditions */
        public bool optimizeAcceptance;

        /** Allow bisimulation */
        public bool bisim;

        /** Allow bisimulation on all levels. */
        public bool recursive_bisim;

        /** Provide detailed internal description in the states */
        public bool detailed_states;

        /** Type of the automata that should be generated */
        public automata_type automata;

        /** Use union construction exclusively */
        public bool only_union;

        /** Use scheck exclusively */
        public bool only_safety;

        /** Debug information from the scheduler */
        public bool verbose_scheduler;

        /** Path to scheck */
        //public string scheck_path;
    }
}
