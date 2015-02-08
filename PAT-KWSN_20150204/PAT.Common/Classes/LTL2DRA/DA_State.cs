using System.Collections.Generic;
using System.Diagnostics;

namespace PAT.Common.Classes.LTL2DRA
{
    //APElement, EdgeContainerExplicit_APElement
    //Label, EdgeContainer
    public class DA_State
    {

        /** The type of the automaton containing this state */
        //typedef DA<Label, EdgeContainer,AcceptanceCondition> graph_type;

        ///** The type of the edges in the DA. */
        //typedef typename graph_type::edge_type edge_type;

        ///** The type of the states in the DA (ie this DA_State class). */
        //typedef typename graph_type::state_type state_type;

        ///** The type of the EdgeContainer for the DA_State. */
        //typedef EdgeContainer<DA_State> edge_container_type;

        ///** The type of an iterator over all the edges of this state. */
        //typedef typename edge_container_type::iterator edge_iterator;


        /** The automaton of which this state is a part. */
        private DA _graph;

        /** The edges */
        public EdgeContainerExplicit_APElement<DA_State> _edges;  //<DA_State>

        /** A description */
        string _description;

        public int Index;

        /** 
 * Constructor.
 * @param graph The automaton (DA) that contains this state.
 */
        public DA_State(DA graph, int index)
        {
            _graph = graph;

            _edges = new EdgeContainerExplicit_APElement<DA_State>(graph.getAPSize());

            Index = index;
        }


        /** Get the EdgeContainer to access the edges. */
        public EdgeContainerExplicit_APElement<DA_State> edges()
        {
            return _edges;
        }


        /** Get the name (index) of this state. */
        public int getName()
        {
            //return _graph.getIndexForState(this);
            return Index;
        }

        /** Set an description for the state */
        public void setDescription(string s)
        {
            _description = s;
        }

        /**
 * Get an description for the state (previously set using setDescription()).
 * Should only be called after verifying that the state hasDescription()
 * @return a const string ref to the description
 */
        public string getDescription()
        {
            Debug.Assert(hasDescription());
            return _description;
        }

        /**
  * Check wheter the state has a description.
  */
        public bool hasDescription()
        {
            return !string.IsNullOrEmpty(_description);
        }


        /**
         * Checks if all transitions originating in this state
         * leed back to itself. 
         */
        //todo: implemented later.
        public bool hasOnlySelfLoop()
        {
            //for (edge_iterator eit = edges().begin(); eit != edges().end(); ++eit)
            for (KeyValuePair<APElement, DA_State> eit = edges().begin(); !edges().end(); eit = edges().increment())
            {
                if (this != eit.Value)
                {
                    return false;
                }
            }

            return true;
        }
        /** Get the AcceptanceForState access functor for this state */
        public AcceptanceForState acceptance()
        {
            AcceptanceForState acc = new AcceptanceForState(_graph.acceptance(), this.getName());
            return acc;
        }

        /**
 * Returns an iterator over the names of the successors, pointing to the first.
 * Note: A state can occur multiple times!
 */
        public int successors_begin()
        {
            return _edges.begin().Value.getName();
        }

        /**
         * Returns an iterator over the names of the successors, pointing after the last.
         * Note: A state can occur multiple times!
         */
        public bool successors_end()
        {
            return _edges.end();
        }

        /**
         * Returns an iterator over the names of the successors, pointing after the last.
         * Note: A state can occur multiple times!
         */
        public int increment()
        {
            return _edges.increment().Value.getName();
        }
    }
}