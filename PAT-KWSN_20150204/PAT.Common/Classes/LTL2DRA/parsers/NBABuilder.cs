using System.Collections.Generic;
using System.Diagnostics;
using PAT.Common.Classes.BA;

namespace PAT.Common.Classes.LTL2DRA.parsers
{
    public class NBABuilder
    {

        /** The type of a map from names to state indizes */
        //typedef std::map<name_t, unsigned int> name2state_map_t;
        /** A map from names to state indizes */
        private Dictionary<string, int> name2state;

        /** The interface to the NBA */
        NBA _nba;
        /** Should all states be final?*/
        bool _all_states_are_final;

        /** 
   * Constructor
   * @param nba the interface to the NBA
   */
        public NBABuilder(NBA nba)
        {
            _nba = nba;
            _all_states_are_final = false;

            name2state = new Dictionary<string, int>();
        }

        /**
 * Find or add a state with a certain name.
 * @param name the name of the state
 * @return the index of the state
 */
        public int findOrAddState(string name)
        {
            //name2state_map_t::iterator it = name2state.find(name);

            if (!name2state.ContainsKey(name))
            {
                int new_state = _nba.nba_i_newState();
                if (_all_states_are_final)
                {
                    _nba.nba_i_setFinal(new_state, true);
                }
                name2state.Add(name, new_state);
                return new_state;
            }
            else
            {
                return name2state[name]; // it.second;
            }
        }

        /**
  * Add an additional name to an already existing state.
  * @param name the name
  * @param state the index of the state
  */
        public void addAdditionalNameToState(string name, int state)
        {
            name2state[name] = state;
        }

        ///**
        //* Add an edge from a state to a state with labeling specified
        //* by a formula in propositional logic in prefix form.
        //* @param from  the 'from' state index
        //* @param to    the 'to' state index
        //* @param guard the formula
        //*/
        //public void addEdge(int from, int to, string guard)
        //{

        //    EdgeCreator ec = new EdgeCreator(from, to, _nba);

        //    LTLFormula ltl_guard= LTLPrefixParser.parse(guard, _nba.nba_i_getAPSet());
        //    //    std::cerr << ltl_guard->toStringPrefix() << std::endl;

        //    LTLFormula guard_dnf=ltl_guard.toDNF();
        //    //    std::cerr << guard_dnf->toStringPrefix() << std::endl;

        //    guard_dnf.forEachMonom(ec);
        //}

        /**
 * Add an edge from a state to a state with labeling specified
 * by a formula in propositional logic in prefix form.
 * @param from  the 'from' state index
 * @param to    the 'to' state index
 * @param guard the formula
 */
        public void addEdge(int from, int to, List<Proposition> labels)
        {
            Debug.Assert(labels.Count > 0);
            LTLNode root = createNodeFromLabel(labels[labels.Count - 1]);
            if(labels.Count > 1)
            {
                for (int i = labels.Count-2; i >= 0; i--)
                {
                    root = new LTLNode(type_t.T_AND, createNodeFromLabel(labels[i]), root);
                }
            }

            EdgeCreator ec = new EdgeCreator(from, to, _nba);

            LTLFormula ltl_guard = new LTLFormula(root, _nba.nba_i_getAPSet());
            //    std::cerr << ltl_guard->toStringPrefix() << std::endl;

            LTLFormula guard_dnf = ltl_guard.toDNF();
            //    std::cerr << guard_dnf->toStringPrefix() << std::endl;

            guard_dnf.forEachMonom(ec);
        }

        private LTLNode createNodeFromLabel(Proposition p)
        {
            if (p.IsSigmal)
            {
                return new LTLNode(type_t.T_TRUE, null, null);
            }

            if (p.Negated)
            {
                int ap_i = _nba.nba_i_getAPSet().find(p.Label);
                return new LTLNode(type_t.T_NOT, new LTLNode(ap_i));
            }
            else
            {
                int ap_i = _nba.nba_i_getAPSet().find(p.Label);
                return new LTLNode(ap_i);
            }
        }


        /**
         * Set the start state
         * @param state the state index
         */
        public void setStartState(int state)
        {

            // TODO: Check if start state was already set..
            _nba.nba_i_setStartState(state);
        }

        /**
         * Set the final flag of a state
         * @param state the state index
         * @param value the value of the flag
         */
        public void setFinal(int state, bool value)
        {
            _nba.nba_i_setFinal(state, value);
        }

        public void setFinal(int state)
        {
            _nba.nba_i_setFinal(state, true);
        }

        /**
        * Mark that the NBA has only accepting (final) states,
        * all states newly created will be automatically final.
        */
        public void setAllStatesAreFinal()
        {
            _all_states_are_final = true;
        }

        /** Check if an atomic proposition is valid */
        public bool isAP(string ap)
        {
            return (_nba.nba_i_getAPSet().find(ap) != -1);
        }


    }

      /** Functor to create the edges. */
    //struct EdgeCreator
    //{
    //    int _from, _to;
    //    NBA_I _nba;

    //    public EdgeCreator(int from, int to, NBA_I nba)
    //    {
    //        _from = from;
    //        _to = to;
    //        _nba = nba;
    //    }

    //    //todo: what is () operator
    //    public void apply(APMonom m) {
    //     _nba.nba_i_addEdge(_from, m, _to);
    //    }
    //}
}
