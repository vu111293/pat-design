using System.Collections.Generic;
using System.Diagnostics;
using PAT.Common.Classes.LTL2DRA.common;

namespace PAT.Common.Classes.LTL2DRA
{
    public class NBAStutterClosure
    {
          /** Calculate the stutter closure for the NBA, for all symbols. 
   * @param nba the NBA
   */

        public static NBA stutter_closure(NBA nba)
        {
            APSet apset = nba.getAPSet_cp();

            NBA nba_result_ptr = new NBA(apset);
            NBA result = nba_result_ptr;

            int element_count = apset.powersetSize();

            Debug.Assert(nba.getStartState() != null);
            int start_state = nba.getStartState().getName();

            for (int i = 0; i < nba.size(); i++)
            {
                int st = result.nba_i_newState();
                Debug.Assert(st == i);

                if (st == start_state)
                {
                    result.setStartState(result[st]);
                }

                if (nba[st].isFinal())
                {
                    result[st].setFinal(true);
                }
            }

            for (int i = 0; i < nba.size(); i++)
            {
                for (int j = 0; j < element_count; j++)
                {
                    int st = result.nba_i_newState();
                    Debug.Assert(st == nba.size() + (i*element_count) + j);
                    result[st].addEdge(new APElement(j), result[i]);
                    result[st].addEdge(new APElement(j), result[st]);
                }
            }

            List<List<BitSet>> reachable = new List<List<BitSet>>();
            //reachable.resize(element_count);
            Ultility.resize(reachable, element_count);

            for (int j = 0; j < element_count; j++)
            {
                //NBAEdgeSuccessors edge_successor = new NBAEdgeSuccessors(new APElement(j));
                SCCs scc = new SCCs();
                GraphAlgorithms.calculateSCCs(nba, scc, true, new APElement(j)); //,edge_successor

                reachable[j] = scc.getReachabilityForAllStates();

#if VERBOSE
      std::cerr << "SCCs for " << APElement(j).toString(*apset) << std::endl;
      std::cerr << scc << std::endl; 
      
      std::cerr << " Reachability: "<< std::endl;
      std::vector<BitSet>& reach=*reachable[j];
      for (unsigned int t=0; t < reach.size(); t++) {
      	std::cerr << t << " -> " << reach[t] << std::endl;
      }
      
      std::cerr << "  ---\n";
#endif
            }


            for (int i = 0; i < nba.size(); i++)
            {
                NBA_State from = result[i];

                for (int j = 0; j < element_count; j++)
                {
                    BitSet result_to = new BitSet();

                    BitSet to = nba[i].getEdge(new APElement(j));
                    //for (BitSetIterator it=BitSetIterator(*to);it!=BitSetIterator::end(*to);++it) 
                    for (int it = BitSetIterator.start(to); it != BitSetIterator.end(to); it = BitSetIterator.increment(to, it))
                    {
                        int to_state = it;

                        // We can go directly to the original state
                        result_to.set(to_state);
                        // We can also go to the corresponding stutter state instead
                        int stutter_state = nba.size() + (to_state*element_count) + j;
                        result_to.set(stutter_state);

                        // ... and then we can go directly to all the states that are j-reachable from to
                        result_to.Union(reachable[j][to_state]);
                    }

                    from.getEdge(new APElement(j)).Assign(result_to);
                }
            }

            //for (int i=0; i<reachable.size(); ++i) {
            //  delete reachable[i];
            //  }

            return nba_result_ptr;
        }


        
  /** Calculate the stutter closure for the NBA, for a certain symbol.
   * @param nba the NBA
   * @param label the symbol for which to perform the stutter closure
   */

        public static NBA stutter_closure(NBA nba, APElement label)
        {
            APSet apset = nba.getAPSet_cp();

            NBA nba_result_ptr = new NBA(apset);
            NBA result = nba_result_ptr;

            int element_count = apset.powersetSize();

            Debug.Assert(nba.getStartState() != null);
            int start_state = nba.getStartState().getName();

            for (int i = 0; i < nba.size(); i++)
            {
                int st = result.nba_i_newState();
                Debug.Assert(st == i);

                if (st == start_state)
                {
                    result.setStartState(result[st]);
                }

                if (nba[st].isFinal())
                {
                    result[st].setFinal(true);
                }
            }

            for (int i = 0; i < nba.size(); i++)
            {
                int st = result.nba_i_newState();
                Debug.Assert(st == nba.size() + i);
                result[st].addEdge(label, result[i]);
                result[st].addEdge(label, result[st]);
            }

            //List<BitSet> reachable = null;

            //NBAEdgeSuccessors edge_successor = new NBAEdgeSuccessors(label);
            SCCs scc = new SCCs();
            GraphAlgorithms.calculateSCCs(nba, scc, true, label); //,edge_successor

            List<BitSet> reachable = scc.getReachabilityForAllStates();

            //    std::cerr << "SCCs for " << label.toString(*apset) << std::endl;
            //    std::cerr << scc << std::endl; 

            //    std::cerr << " Reachability: "<< std::endl;
            //    for (unsigned int t=0; t < reachable->size(); t++) {
            //      std::cerr << t << " -> " << (*reachable)[t] << std::endl;
            //    }

            //    std::cerr << "  ---\n";

            for (int i = 0; i < nba.size(); i++)
            {
                NBA_State from = result[i];

                for (int j = 0; j < element_count; j++)
                {
                    BitSet result_to = new BitSet();

                    BitSet to = nba[i].getEdge(new APElement(j));
                    if (j != label.bitset)
                    {
                        result_to = to;
                    }
                    else
                    {
                        //for (BitSetIterator it=BitSetIterator(*to);it!=BitSetIterator::end(*to);++it) 
                        for (int it = BitSetIterator.start(to); it != BitSetIterator.end(to); it = BitSetIterator.increment(to, it))
                        {
                            int to_state = it;

                            // We can go directly to the original state
                            result_to.set(to_state);
                            // We can also go to the corresponding stutter state instead
                            int stutter_state = nba.size() + to_state;
                            result_to.set(stutter_state);

                            // ... and then we can go directly to all the states that are j-reachable from to
                            result_to.Union(reachable[to_state]);
                        }
                    }

                    from.getEdge(new APElement(j)).Assign(result_to);
                }
            }

            //delete reachable;

            return nba_result_ptr;
        }

    }

    
  ///** The successors reachable via a certain label */
  
  //public class NBAEdgeSuccessors {
  
  //  //typedef BitSetIterator successor_iterator;

  //  public NBAEdgeSuccessors(APElement label)
  //  {
  //      _label = label;
  //  }
    
  //  public int begin(NBA graph, int v) {
  //    return BitSetIterator.start(graph[v].getEdge(_label));
  //  }

  //  public int end(NBA graph, int v) {
  //    return BitSetIterator.end(graph[v].getEdge(_label));
  //  }

  //private APElement _label;
  //}
}
