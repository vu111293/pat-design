using System;
using System.Collections.Generic;
using System.Text;
using PAT.Common.Classes.BA;
using PAT.Common.Classes.LTL2DRA.parsers;
using PAT.Common.Classes.Ultility;

namespace PAT.Common.Classes.LTL2DRA
{
    public class LTL2NBA
    {
        /** 
         * Convert an LTL formula to an NBA
         * @param ltl
         * @return a pointer to the created NBA (caller gets ownership).
         */

        public static NBA ltl2nba(LTLFormula ltl, BuchiAutomata buchiAutomata)
        {

            // Create canonical APSet (with 'p0', 'p1', ... as AP)
            //LTLFormula ltl_canonical = ltl.copy();
            //APSet canonical_apset = ltl.getAPSet().createCanonical();
           // ltl_canonical.switchAPSet(canonical_apset);


            //AnonymousTempFile spin_outfile;
            //std::vector<std::string> arguments;
            //arguments.push_back("-f");
            //arguments.push_back(ltl_canonical->toStringInfix());

            //arguments.insert(arguments.end(), _arguments.begin(),_arguments.end());

            //const char *program_path=_path.c_str();

            //RunProgram spin(program_path,
            //        arguments,
            //        false,
            //        0,
            //        &spin_outfile,
            //        0);

            //int rv=spin.waitForTermination();
            //if (rv==0) {
            //  NBA_t *result_nba(new NBA_t(canonical_apset));

            //  FILE *f=spin_outfile.getInFILEStream();
            //  if (f==NULL) {
            //throw Exception("");
            //  }

            //  int rc=nba_parser_promela::parse(f, result_nba);
            //  fclose(f);

            //  if (rc!=0) {
            //throw Exception("Couldn't parse PROMELA file!");
            //  }

            NBA result_nba = new NBA(ltl.getAPSet());

            ////////////////////////////////////////////////////////////
            //todo: create the NBA from the BA
            //
            //
            ////////////////////////////////////////////////////////////
            NBABuilder builder = new NBABuilder(result_nba);
            //int current_state = 0;
            //bool current_state_valid=false;

            //foreach (string state in buchiAutomata.States)
            //{

            //    if (buchiAutomata.InitialStates.Contains(state))
            //    {
            //        int current_state = builder.findOrAddState(state);
            //        if (buchiAutomata.InitialStates.Contains(state))
            //        {
            //            builder.setStartState(current_state);
            //        }
            //    }
            //}

            foreach (string state in buchiAutomata.States)
            {
                //if (!buchiAutomata.InitialStates.Contains(state))
                {
                    ////s.AppendLine(state);
                    //if (current_state_valid) {
                    //    builder.addAdditionalNameToState(state, current_state);			
                    //} 
                    //else
                    //{
                    int current_state = builder.findOrAddState(state);
                    //std::string& label=$1;
                    //if (label.find("accept") != std::string::npos) {
                    if (state.EndsWith(Constants.ACCEPT_STATE))
                    {
                        builder.setFinal(current_state);
                    }
                    //if (label.find("accept_all") != std::string ::npos)
                    //{
                    //    // dirty hack: accept_all + skip -> trueloop
                    //    builder.setFinal(current_state);
                    //    builder.addEdge(current_state, current_state, std::string ("t"));
                    //}

                    if (buchiAutomata.InitialStates.Contains(state))
                    {
                        builder.setStartState(current_state);
                    }
                    //current_state_valid = true;
                    //}    
                }                
            }

            //s.AppendLine("Transitions");
            foreach (Transition transition in buchiAutomata.Transitions)
            {
                int from = builder.findOrAddState(transition.FromState);
                int to = builder.findOrAddState(transition.ToState);
                builder.addEdge(from, to, transition.labels);
            }


            // switch back to original APSet
            //result_nba.switchAPSet(ltl.getAPSet());


            //todo:
            //construct the NBA here

            return result_nba;

        }
    }
}
