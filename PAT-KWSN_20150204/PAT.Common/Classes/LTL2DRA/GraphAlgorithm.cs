using System.Collections.Generic;
using PAT.Common.Classes.LTL2DRA.common;

namespace PAT.Common.Classes.LTL2DRA
{


    public class SCCs
    {
        public List<BitSet> _sccs;
        public List<int> _state_to_scc;
        public List<BitSet> _dag;
        public List<int> _topological_order;
        public List<BitSet> _reachability;

        /** Constructor */
        public SCCs()
        {
            _sccs = new List<BitSet>();
            _state_to_scc = new List<int>();
            _dag = new List<BitSet>();
            _topological_order = new List<int>();
            _reachability = new List<BitSet>();
        }

        public BitSet this[int i]
        {
            get
            {
                return _sccs[i];
            }
            set
            {
                _sccs[i] = value;
            }
        }
        /** Get the number of SCCs */
        public int countSCCs()
        {
            return _sccs.Count;
        }

        /** Get the SCC index for state */
        public int state2scc(int state)
        {
            return _state_to_scc[state];
        }

        /** Get a vector with a topological order of the states*/
        public List<int> topologicalOrder()
        {
            return _topological_order;
        }

        /** Get a set of SCCs that are successors of the SCC scc_index */
        public BitSet successors(int scc_index)
        {
            return _dag[scc_index];
        }

        /** Return true, if state_to is reachable from state_from */
        public bool stateIsReachable(int state_from, int state_to)
        {
            return isReachable(state2scc(state_from), state2scc(state_to));
        }

        /** Return true, if SCC scc_to is reachable from SCC_fromom */
        bool isReachable(int scc_from, int scc_to)
        {
            return _reachability[scc_from].get(scc_to);
        }

        public List<BitSet> getReachabilityForAllStates()
        {
            //std::vector<BitSet>* v=new std::vector<BitSet>;
            //v->resize(_state_to_scc.size());

            List<BitSet> v = new List<BitSet>();
            Ultility.resizeExact(v, _state_to_scc.Count);


            for (int i = 0; i < _state_to_scc.Count; ++i)
            {
                int scc = state2scc(i);
                BitSet reachable_sccs = _reachability[scc];

                BitSet reachable_states = new BitSet();

                //for (BitSetIterator it(reachable_sccs); it != BitSetIterator::end(reachable_sccs); ++it)
                for (int it = BitSetIterator.start(reachable_sccs); it != BitSetIterator.end(reachable_sccs); it = BitSetIterator.increment(reachable_sccs, it))
                {
                    // union with all states from the reachable scc
                    reachable_states.Union(_sccs[it]);
                }

                v[i] = reachable_states;

                //std::cerr << "from "<<i<<": "<<reachable_states<<std::endl;
            }

            return v;
        }

        /** Add a new SCC */
        public int addSCC(BitSet scc)
        {
            _sccs.Add(scc);
            return _sccs.Count - 1;
        }

        /** Set the SCC for a state */
        public void setState2SCC(int state, int scc)
        {
            if (_state_to_scc.Count <= state)
            {
                //_state_to_scc.resize(state + 1);
                for (int i = _state_to_scc.Count; i < state + 1; i++)
                {
                    _state_to_scc.Add(0);
                }
            }
            _state_to_scc[state] = scc;
        }

    }

    /** Provide access for a given Graph to the successors 
 *  for a state v, using the successors_begin and successors_end
 *  calls on the Graph::state_type* */



    /**
 * Perform graph algorithms
 */


    public class GraphAlgorithms
    {
        /** Calculate the SCCs for Graph graph and save in result. */
        //public static void calculateSCCs(NBA graph, SCCs result)
        //{
        //    calculateSCCs(graph, result, false);
        //}

        /** Calculate the SCCs for Graph graph and save in result. */
        public static void calculateSCCs(NBA graph, SCCs result, bool disjoint, APElement label)
        { // =false, SuccessorAccess successor_access=SuccessorAccess()
            SCC_DFS.calculateSCCs(graph, result, disjoint, label); //, successor_access
        }
    }



    /** Helper class to calculate the SCCs*/
    public class SCC_DFS
    {


        /** The graph */
        public NBA _graph;

        /** The SCCs */
        public SCCs _result;

        /** Calculate the SCCs for Graph graph and save in result. */
        public static void calculateSCCs(NBA graph, SCCs result, bool disjoint, APElement label) //, SuccessorAccess& successor_access
        {
            SCC_DFS scc_dfs = new SCC_DFS(graph, result, label); 
            scc_dfs.calculate(disjoint);
        }

        /** Dummy constructor to restrict creation */
        private SCC_DFS() { }

        /** Constructor */
        public SCC_DFS(NBA graph, SCCs result, APElement label)  //
        {
            _graph = graph;
            _result = result;
            //_successor_access = successor_access; 
            _labelMark = label;

            /** The DFS stack */
            _stack = new Stack<int>();

            /** The SCC_DFS_Data for every state (state index -> DFS_DATA) */
            _dfs_data = new List<SCC_DFS_Data>();
        }

        /** A class for saving DFS state information */
        public class SCC_DFS_Data
        {
            public int dfs_nr;
            public int root_index;
            public bool inComponent;
        }


        APElement _labelMark;

        /** The current DFS number */
        int current_dfs_nr;

        /** The DFS stack */
        public Stack<int> _stack;

        /** The SCC_DFS_Data for every state (state index -> DFS_DATA) */
        public List<SCC_DFS_Data> _dfs_data;

        /** The current scc number */
        int scc_nr;

        /** Calculate the SCCs*/
        public void calculate(bool disjoint)
        {
            current_dfs_nr = 0;
            //_dfs_data.Clear();

            // Ensure there are as many entries as there are graph-states
            //_dfs_data.resize(_graph.size());
            //Ultility.resize(_dfs_data, _graph.size());
            Ultility.resizeExact(_dfs_data, _graph.size());

            scc_nr = 0;

            NBA_State start_state = _graph.getStartState();
            if (start_state == null)
            {
                return;
            }

            if (!disjoint)
            {
                int start_idx = start_state.getName();
                visit(start_idx);
            }
            else
            {
                // The Graph may be disjoint -> restart DFS on every not yet visited state 
                for (int v = 0; v < _graph.size(); ++v)
                {
                    if (_dfs_data[v] == null) //.get() 
                    {
                        // not yet visited
                        visit(v);
                    }
                }
            }

            calculateDAG();
        }

        /** Visit a state (perform DFS) */
        public void visit(int v)
        {
            SCC_DFS_Data sdd = new SCC_DFS_Data();
            sdd.dfs_nr = current_dfs_nr++;
            sdd.root_index = v;
            sdd.inComponent = false;

            _stack.Push(v);

            //todo: be careful of the following swap
            //_dfs_data[v].reset(sdd);
            //Ultility.swap(_dfs_data[v], sdd);
            _dfs_data[v] = sdd;

            //for (typename SuccessorAccess::successor_iterator  succ_it=_successor_access.begin(_graph, v);succ_it!=_successor_access.end(_graph, v); ++succ_it) 
            if(_labelMark == null)
            {
                for (KeyValuePair<APElement, BitSet> it_set = _graph[v].edges_begin(); !_graph[v].edges_end(); it_set = _graph[v].increment())
                {
                    for (int succ_it = BitSetIterator.start(it_set.Value); succ_it != BitSetIterator.end(it_set.Value); succ_it = BitSetIterator.increment(it_set.Value, succ_it))
                    {
                        int w = succ_it;

                        if (_dfs_data[w] == null) //.get() 
                        {
                            // not yet visited
                            visit(w);
                        }

                        SCC_DFS_Data sdd_w = _dfs_data[w]; //.get();
                        if (sdd_w.inComponent == false)
                        {
                            int dfs_nr_root_v = _dfs_data[sdd.root_index].dfs_nr;
                            int dfs_nr_root_w = _dfs_data[sdd_w.root_index].dfs_nr;

                            if (dfs_nr_root_v > dfs_nr_root_w)
                            {
                                sdd.root_index = sdd_w.root_index;
                            }
                        }
                    }
                }    
            }
            else
            {
                BitSet it_set = _graph[v].getEdge(_labelMark);
                for (int succ_it = BitSetIterator.start(it_set); succ_it != BitSetIterator.end(it_set); succ_it = BitSetIterator.increment(it_set, succ_it))
                {
                    int w = succ_it;

                    if (_dfs_data[w] == null) //.get() 
                    {
                        // not yet visited
                        visit(w);
                    }

                    SCC_DFS_Data sdd_w = _dfs_data[w]; //.get();
                    if (sdd_w.inComponent == false)
                    {
                        int dfs_nr_root_v = _dfs_data[sdd.root_index].dfs_nr;
                        int dfs_nr_root_w = _dfs_data[sdd_w.root_index].dfs_nr;

                        if (dfs_nr_root_v > dfs_nr_root_w)
                        {
                            sdd.root_index = sdd_w.root_index;
                        }
                    }
                }
            }
            


            if (sdd.root_index == v)
            {
                BitSet set = new BitSet();

                int w;
                do
                {
                    //w=_stack.Peek(); //.top();
                    w = _stack.Pop();

                    set.set(w);
                    _result.setState2SCC(w, scc_nr);

                    SCC_DFS_Data sdd_w = _dfs_data[w];//.get();
                    sdd_w.inComponent = true;
                } while (w != v);

                scc_nr = _result.addSCC(set) + 1;
            }
        }

        /** Calculate the Directed Acyclical Graph (DAG) */
        public void calculateDAG()
        {
            //_result._dag.Clear();
            //_result._dag.resize(_result.countSCCs());
            Ultility.resizeExact(_result._dag, _result.countSCCs());
        
            //_result._reachability.resize(_result.countSCCs());
            Ultility.resizeExact(_result._reachability, _result.countSCCs());

            List<int> in_degree = new List<int>(_result.countSCCs());
            Ultility.resizeExact(in_degree, _result.countSCCs());

            for (int scc = 0; scc < _result.countSCCs(); ++scc)
            {
                _result._reachability[scc] = new BitSet();
                _result._dag[scc] = new BitSet();
                
                BitSet states_in_scc = _result[scc];

                //for (BitSetIterator it=BitSetIterator(states_in_scc); it!=BitSetIterator::end(states_in_scc); ++it) 
                for (int it = BitSetIterator.start(states_in_scc); it != BitSetIterator.end(states_in_scc); it = BitSetIterator.increment(states_in_scc, it))
                {

                    int from_state = it;

                    if (_labelMark == null)
                    {
                        //for (typename SuccessorAccess::successor_iterator succ_it=_successor_access.begin(_graph, from_state);succ_it!=_successor_access.end(_graph, from_state); ++succ_it) 
                        for (KeyValuePair<APElement, BitSet> it_set = _graph[from_state].edges_begin(); !_graph[from_state].edges_end(); it_set = _graph[from_state].increment())
                        {
                            for (int succ_it = BitSetIterator.start(it_set.Value); succ_it != BitSetIterator.end(it_set.Value); succ_it = BitSetIterator.increment(it_set.Value, succ_it))
                            {

                                int to_state = succ_it;
                                int to_scc = _result.state2scc(to_state);

                                if (to_scc != scc)
                                {
                                    // Only successor in the DAG if not the same scc
                                    if (!_result._dag[scc].get(to_scc))
                                    {
                                        // This SCC is a new successor, increment in_degree
                                        in_degree[to_scc]++;
                                        _result._dag[scc].set(to_scc);
                                    }
                                }

                                // Reachability
                                _result._reachability[scc].set(to_scc);
                            }
                        }
                    }
                    else
                    {
                        BitSet it_set = _graph[from_state].getEdge(_labelMark);

                        //for (typename SuccessorAccess::successor_iterator succ_it=_successor_access.begin(_graph, from_state);succ_it!=_successor_access.end(_graph, from_state); ++succ_it) 
                       
                        for (int succ_it = BitSetIterator.start(it_set); succ_it != BitSetIterator.end(it_set); succ_it = BitSetIterator.increment(it_set, succ_it))
                        {

                            int to_state = succ_it;
                            int to_scc = _result.state2scc(to_state);

                            if (to_scc != scc)
                            {
                                // Only successor in the DAG if not the same scc
                                if (!_result._dag[scc].get(to_scc))
                                {
                                    // This SCC is a new successor, increment in_degree
                                    in_degree[to_scc]++;
                                    _result._dag[scc].set(to_scc);
                                }
                            }

                            // Reachability
                            _result._reachability[scc].set(to_scc);
                        }
                    }
                }
            }

            bool progress = true;
            int cnt = 0;
            
            //_result._topological_order.Clear();
            //_result._topological_order.resize(_result.countSCCs());
            Ultility.resizeExact(_result._topological_order, _result.countSCCs());


            List<int> sort = new List<int>(_result.countSCCs());
            Ultility.resizeExact(sort, _result.countSCCs());

            while (progress)
            {
                progress = false;

                for (int scc = 0; scc < _result.countSCCs(); ++scc)
                {
                    if (in_degree[scc] == 0)
                    {
                        sort[scc] = cnt++;
                        progress = true;
                        in_degree[scc] = -1;

                        //for (BitSetIterator it_neighbors= BitSetIterator(_result._dag[scc]); it_neighbors!=BitSetIterator::end(_result._dag[scc]); ++it_neighbors) 
                        for (int it_neighbors = BitSetIterator.start(_result._dag[scc]); it_neighbors != BitSetIterator.end(_result._dag[scc]); it_neighbors = BitSetIterator.increment(_result._dag[scc], it_neighbors))
                        {
                            int scc_to = it_neighbors;
                            in_degree[scc_to]--;
                        }
                    }
                }
            }

            for (int i = 0; i < _result.countSCCs(); i++)
            {
                _result._topological_order[sort[i]] = i;
            }


            // traverse SCCs in reverse topological order
            for (int i = _result.countSCCs(); i > 0; --i)
            {
                int cur_scc = _result._topological_order[i - 1];

                BitSet reaches = _result._reachability[cur_scc];

                //for (BitSetIterator it_neighbors= BitSetIterator(_result._dag[cur_scc]); it_neighbors!=BitSetIterator::end(_result._dag[cur_scc]);++it_neighbors) {
                for (int it_neighbors = BitSetIterator.start(_result._dag[cur_scc]); it_neighbors != BitSetIterator.end(_result._dag[cur_scc]); it_neighbors = BitSetIterator.increment(_result._dag[cur_scc], it_neighbors))
                {
                    int scc_to = it_neighbors;
                    reaches.Union(_result._reachability[scc_to]);
                }
            }
        }
    }
}
