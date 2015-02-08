using System;
using System.Collections.Generic;
using PAT.Common.Classes.LTL2DRA.common;

namespace PAT.Common.Classes.LTL2DRA
{
    //APElement, EdgeContainerExplicit_APElement
    //<Label, EdgeContainer>
    public class NBA_State//<Label, EdgeContainer>
    {

        /** The state owning this EdgeManager */
        //private NBA_State _state;
        
        /** The EdgeContainer */
       // EdgeContainerExplicit_APElement<BitSet> _container;


        /** The automaton */
        private NBA _graph;

        /** Is this state accepting */
        private bool _isFinal;

        /** A description. */
        private string _description;

        /** The EdgeManager*/
        public NBA_State_EdgeManager _edge_manager;

        public int Index;

        /**
 * Constructor.
 * @param graph The automaton (NBA) that contains this state.
 */
        public NBA_State(NBA graph, int index)
        {
            _graph = graph;
            _isFinal = false;
            _edge_manager = new NBA_State_EdgeManager(this, graph.getAPSet());
            Index = index;
        }

        /** 
 * Add an edge from this state to the other state
 * @param label the label for the edge
 * @param state the target state 
 */
        public void addEdge(APElement label, NBA_State state)
        {
            _edge_manager.addEdge(label, state);
        }

        /** 
 * Add edge(s) from this state to the other state
 * @param monom an APMonom for the label(s)
 * @param to_state the target state
 */
        public void addEdge(APMonom monom, NBA_State to_state)
        {
            _edge_manager.addEdge(monom, to_state);
        }

        /*
edge_container_type& edges() {return _edges;}
const edge_container_type& edges() const {return _edges;}
*/

        /** 
         * Get the target states of the labeled edge.
         * @return a pointer to a BitSet with the indizes of the target states.
         */
        public BitSet getEdge(APElement label)
        {
            return _edge_manager.getEdge(label);
        }

   //     /** 
   //* Get the target states of the labeled edge.
   //* @return a pointer to a BitSet with the indizes of the target states.
   //*/
   //     public BitSet getEdge(APMonom monom)
   //     {
   //         return _edge_manager.getEdge(monom);
   //     }

        /** Get the name (index) of this state. */
        public int getName()
        {
            //return _graph.getIndexForState(this);
            return Index;
        }


        /** Is this state accepting (final)? */
        public bool isFinal() { return _isFinal; }

        /** Set the value of the final flag for this state */
        public void setFinal(bool final)
        {
            _isFinal = final;
            _graph.getFinalStates().set(_graph.getIndexForState(this), final);
        }

          /** Returns an iterator over the edges pointing to the first edge. */
        public KeyValuePair<APElement, BitSet> edges_begin()
        {
            return _edge_manager.getEdgeContainer().begin();
        }

        /** Returns an iterator over the edges pointing after the last edge. */
  public bool edges_end() {
      return _edge_manager.getEdgeContainer().end();
  }

  /** Returns an iterator over the edges pointing after the last edge. */
  public KeyValuePair<APElement, BitSet> increment()
  {
      return _edge_manager.getEdgeContainer().increment();
  }

        /**
 * Returns an iterator over the names of the successors, pointing to the first.
 * Note: A state can occur multiple times!
 */


        ///**
        // * Returns an iterator over the names of the successors, pointing after the last.
        // * Note: A state can occur multiple times!
        // */
        //public successor_iterator successors_end()
        //{
        //    return successor_iterator(edges_end(), edges_end());
        //}

        /** Set the description for this state. */
        public void setDescription(string s) { _description = s; }
        /** Get the description for this state. */
        public string getDescription() { return _description; }

        /** Check if this state has a description. */
        public bool hasDescription() { return !string.IsNullOrEmpty(_description); }

        /** Get the automaton owning this state. */
        public NBA getGraph() { return _graph; }


    }

/** The EdgeManager for the NBA_State */

public struct NBA_State_EdgeManager//,<State>   APElement, <BitSet> 
{

  /** The type of the EdgeContainer*/
  //typedef  edge_container_type;


  /** The state owning this EdgeManager */
  private NBA_State _state;
  /** The EdgeContainer */
  public EdgeContainerExplicit_APElement<BitSet> _container; //<BitSet>

  

  /**
   * Constructor.
   * @param state the NBA_State owning this EdgeManager
   * @param apset the underlying APSet   
   */
  public NBA_State_EdgeManager(NBA_State state, APSet apset)
  {

      _state = state;
      _container = new EdgeContainerExplicit_APElement<BitSet>(apset.size());

      //for (APSet::element_iterator eit=apset.all_elements_begin(); eit!=apset.all_elements_end(); ++eit) {
      for (int i = apset.all_elements_begin(); i != apset.all_elements_end(); i++)
      {
          _container.addEdge(new APElement(i), new BitSet());
      }

  }


    ///** Destructor */
  //~NBA_State_EdgeManager() {
  //  const APSet& ap_set=_state.getGraph().getAPSet();
  //  for (APSet::element_iterator eit=ap_set.all_elements_begin();
  //   eit!=ap_set.all_elements_end();
  //   ++eit) {
  //    delete _container.get(*eit);
  //  }    
  //}

  /** Get the target states */
  public BitSet getEdge(APElement label) {
    return _container.get(label);
  }

  ///** Get the target states */
  //public BitSet getEdge(APMonom monom) {
  //  throw new Exception("Not implemented!");
  //}

  /** Add an edge. */
  public void addEdge(APElement label, NBA_State state) {
    _container.get(label).set(state.getName());///////////////note here
    //_container.addEdgeDebug(label.getBitSet(), state.getName());
  }

  /** Add an edge. */
  public void addEdge(APMonom label, NBA_State state) {
    APSet ap_set=_state.getGraph().getAPSet();

      APMonom2APElements start = APMonom2APElements.begin(ap_set, label);
        //for (APMonom2APElements it=APMonom2APElements::begin(ap_set, label);it!=APMonom2APElements::end(ap_set, label);++it) 
      while (!start.equal(APMonom2APElements.end(ap_set, label)))///////////////***********note sth wrong here don't skip sth extra
      {
          APElement it = start._cur_e;
          addEdge(it, state);
          start.increment();
      }
  }

  /** Get the EdgeContainer. */
  public EdgeContainerExplicit_APElement<BitSet> getEdgeContainer()
  {
    return _container;
  }
}


}
