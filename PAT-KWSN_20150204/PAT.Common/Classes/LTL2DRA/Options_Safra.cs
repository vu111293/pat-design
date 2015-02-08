namespace PAT.Common.Classes.LTL2DRA
{
    /**
    * Options for Safra's algorithm
    */
    public class Options_Safra
    {

        /** Optimize accepting true loops */
        public bool opt_accloop;

        /** Optimize all successor accepting */
        public bool opt_accsucc;

        /** Renaming optimization (templates) */
        public bool opt_rename;

        /** Try to reorder Safra trees */
        public bool opt_reorder;

        /** Use stuttering */
        public bool stutter;

        /** Check for stutter insensitive */
        public bool partial_stutter_check;

        /* Perform stutter closure on NBA before conversion */
        public bool stutter_closure;

        /** Check for DBA */
        public bool dba_check;

        /** Provide statistics */
        public bool stat;

        /** Optimize accepting true loops in union construction */
        public bool union_trueloop;

        /** Constructor */
        public Options_Safra()
        {
            opt_none();

            dba_check = false;
            //    tree_verbose=false;
            stat = false;
            union_trueloop = true;

            stutter = false;
            partial_stutter_check = false;
            stutter_closure = false;
        }

        /** Enable all opt_ options */
        public void opt_all()
        {
            opt_accloop
              = opt_accsucc
              = opt_rename
              = opt_reorder
              = true;
        }

        /** Disable all opt_ options */
        public void opt_none()
        {
            opt_accloop
              = opt_accsucc
              = opt_rename
              = opt_reorder
              = false;
        }
    }
}
