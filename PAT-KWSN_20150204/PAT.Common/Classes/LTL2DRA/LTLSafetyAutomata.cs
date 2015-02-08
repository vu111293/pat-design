using PAT.Common.Classes.BA;

namespace PAT.Common.Classes.LTL2DRA
{
    public class LTLSafetyAutomata
    {

        /** Only check syntactically safe formulas */
        private bool _only_syn;

        //typedef NBA<APElement, EdgeContainerExplicit_APElement> NBA_t;


        /** Constructor */
        public LTLSafetyAutomata()
            : this(true)
        {

        }
        public LTLSafetyAutomata(bool only_syntactically_safe)  //=true
        {
            _only_syn = only_syntactically_safe;
        }

        /** 
         * Generate a DRA for an LTL formula using scheck
         * @param ltl the formula
         * @param scheck_path the path to the scheck executable
         * @return a shared_ptr to the generated DRA (on failure returns a ptr to 0)
         */
        //template <class DRA>
        //typename DRA::shared_ptr 
        public DRA ltl2dra(LTLFormula ltl, BuchiAutomata buchiAutomata)
        {
            LTLFormula ltl_;
            LTLFormula ltl_for_scheck = null;

            bool safe = false;

            if (ltl.isSafe())
            {
                safe = true;
                ltl_ = ltl.negate();
                ltl_for_scheck = ltl_;
            }
            else if (ltl.isCoSafe())
            {
                ltl_for_scheck = ltl;
            }
            else
            {
                if (_only_syn)
                {
                    // Not syntactically safe -> abort
                    //typename
                    //DRA::shared_ptr p;
                    //return p;
                    return null;
                }
            }

            //    std::cerr<< "Calling scheck with " 
            //	     <<ltl_for_scheck->toStringPrefix() << " : " << safe << std::endl;

            //NBA nba = ltl2dba(ltl_for_scheck, buchiAutomata); //, scheck_path, _only_syn

            NBA nba = LTL2NBA.ltl2nba(ltl_for_scheck, buchiAutomata); //, scheck_path, _only_syn

            if (nba == null)
            {
                //typename
                //DRA::shared_ptr p;
                //return p;

                return null;
            }

            //    nba->print(std::cerr);

            // safe -> negate DRA
            return DBA2DRA.dba2dra(nba, safe);
            //    return dba2dra<DRA>(*nba, safe);
            // nba is auto-destructed
            //<NBA_t,DRA>
        }


        //public static NBA ltl2dba(LTLFormula ltl, BuchiAutomata buchiAutomata) //, string scheck_path, bool only_syn
        //{
        //    //AnonymousTempFile scheck_outfile;
        //    //AnonymousTempFile scheck_infile;
        //    //std::vector<std::string> arguments;
        //    //arguments.push_back("-d");

        //    //if (only_syn) {
        //    //  arguments.push_back("-s");
        //    //} else {
        //    //  THROW_EXCEPTION(Exception, "Checking for pathological safety not yet supported!");
        //    //  //      arguments.push_back("-p");
        //    //  //      arguments.push_back( --path-to-lbtt-type-ltl2nba-translator-- );
        //    //}


        //    // Create canonical APSet (with 'p0', 'p1', ... as AP)
        //    LTLFormula ltl_canonical = ltl.copy();
        //    APSet canonical_apset = ltl.getAPSet().createCanonical();
        //    ltl_canonical.switchAPSet(canonical_apset);

        //    //std::ostream& so=scheck_infile.getOStream();
        //    //so << ltl_canonical->toStringPrefix() << std::endl;
        //    //so.flush();

        //    //const char *program_path=scheck_path.c_str();

        //    //RunProgram scheck(program_path,
        //    //          arguments,
        //    //          false,
        //    //          &scheck_infile,
        //    //          &scheck_outfile,
        //    //          0);

        //    //int rv=scheck.waitForTermination();
        //    //if (rv==0) {
        //    //  FILE *f=scheck_outfile.getInFILEStream();
        //    //  if (f==NULL) {
        //    //throw Exception("");
        //    //  }

        //    //std::auto_ptr<NBA_t> 
        //    NBA nba_result = new NBA(ltl_canonical.getAPSet());

        //    ////////////////////////////////////////////////////////////
        //    //todo: create the NBA from the BA
        //    //
        //    //
        //    ////////////////////////////////////////////////////////////

        //    //  int rc=nba_parser_lbtt::parse(f, nba_result.get());
        //    //  fclose(f);

        //    //  if (rc!=0) {
        //    //THROW_EXCEPTION(Exception, "scheck: Couldn't parse LBTT file!");
        //    //  }

        //    // switch back to original APSet
        //    nba_result.switchAPSet(ltl.getAPSet());

        //    // release auto_ptr so the NBA is not deleted,
        //    // return pointer to it to calling function
        //    return nba_result; //.release()
        //    //} else {
        //    //  std::cerr << "scheck error code: "<< rv << std::endl;
        //    //  return 0;
        //    //}
        //}
    }
}
