using System;
using System.Collections.Generic;
using PAT.Common.Classes.Assertion.Algorithms.automata;
using PAT.Common.Classes.BA.Algorithms.automata;
using PAT.Common.Classes.BA.Algorithms.comparator;
using PAT.Common.Classes.BA.Algorithms.datastructure;

namespace PAT.Common.Classes.BA.Algorithms.algorithms
{
    public class Simulation
    {
        /**
         * Compute forward simulation relation of a Buchi automaton
         * @param omega: a Buchi automaton
         * @param FSim: the maximal bound of simulation relation
         * 
         * @return maximal simulation relation on states of the input automaton with FSim
         */
        public static StatePairComparator StatePairComparator = new StatePairComparator();

        public HashSet<Pair<FAState, FAState>> FSimRelNBW(FiniteAutomaton omega, HashSet<Pair<FAState, FAState>> FSim)
        {
            HashSet<Pair<FAState, FAState>> nextFSim = new HashSet<Pair<FAState, FAState>>();
            bool changed = true;
            while (changed)
            {
                changed = false;
                foreach (Pair<FAState, FAState> pair in FSim)
                {
                    if (NextStateSimulated(FSim, omega, pair.Left, pair.Right))
                    {
                        nextFSim.Add(new Pair<FAState, FAState>(pair.Left, pair.Right));
                    }
                    else
                    {
                        changed = true;
                    }
                }

                FSim = nextFSim;
                nextFSim = new HashSet<Pair<FAState, FAState>>();
            }
            return FSim;
        }

    public HashSet<Pair<FAState, FAState>> FastFSimRelNBW(FiniteAutomaton omega, bool[,] fsim) 
	{
		//implement the HHK algorithm

		int n_states = omega.states.Count;
		int n_symbols = omega.alphabet.Count;
		FAState[] states = omega.states.ToArray();
		List<String> symbols=new List<String>(omega.alphabet);
		
		// fsim[u][v]=true iff v in fsim(u) iff v forward-simulates u
		
		int[,,] pre = new int[n_symbols,n_states,n_states];
		int[,,] post = new int[n_symbols,n_states,n_states];
		int[,] pre_len = new int[n_symbols,n_states];
		int[,] post_len = new int[n_symbols,n_states];
		
		//state[post[s][q][r]] is in post_s(q) for 0<=r<adj_len[s][q]
		//state[pre[s][q][r]] is in pre_s(q) for 0<=r<adj_len[s][q]
		for(int s=0;s<n_symbols;s++)
		{
			string a = symbols[s];
			for(int p=0; p<n_states; p++)
				for(int q=0; q<n_states; q++)		
				{
					HashSet<FAState> next = states[p].next[a]; 
					if(next!=null && next.Contains(states[q]))
					{
						//if p --a--> q, then p is in pre_a(q), q is in post_a(p) 
						pre[s,q,pre_len[s,q]++] = p;
						post[s,p,post_len[s,p]++] = q;
					}
				}
		}
		int[] todo = new int[n_states*n_symbols];
		int todo_len = 0;
		
		int[,,] remove = new int[n_symbols,n_states,n_states];
		int[,] remove_len = new int[n_symbols,n_states];
		for(int a=0; a<n_symbols; a++)
		{
			for(int p=0; p<n_states; p++)
				if(pre_len[a,p]>0) // p is in a_S
				{	
					//Sharpen_S_a:
                    for (int q = 0; q < n_states; q++)	// {all q} --> S_a 
                    {
                        bool skipQ = false;
                        if (post_len[a, q] > 0) /// q is in S_a 
                        {
                            for (int r = 0; r < post_len[a, q]; r++)
                            {
                                if (fsim[p,post[a, q, r]]) // q is in pre_a(sim(p))
                                {
                                    skipQ = true;
                                    break;
                                }
                                //continue Sharpen_S_a; // skip q						
                            }
                            if (!skipQ)
                            {
                                remove[a, p, remove_len[a, p]++] = q;
                            }

                        }
                    }
				    if(remove_len[a,p]>0)
						todo[todo_len++] = a*n_states + p;
				}
		}

		int[] swap = new int[n_states];
		int swap_len = 0;
		bool using_swap = false;
		
		while(todo_len>0)
		{
			todo_len--;
			int v = todo[todo_len] % n_states;
			int a = todo[todo_len] / n_states;
			int len = (using_swap? swap_len : remove_len[a,v]);
			remove_len[a,v] = 0;
			
			for(int j=0; j<pre_len[a,v]; j++)
			{
				int u = pre[a,v,j];
				
				for(int i=0; i<len; i++)			
				{
					int w = (using_swap? swap[i] : remove[a,v,i]);
					if(fsim[u,w]) 
					{
						fsim[u,w] = false;					
						for(int b=0; b<n_symbols; b++)
						{
                            if(pre_len[b,u]>0)
							{
								//Sharpen_pre_b_w:
								for(int k=0; k<pre_len[b,w]; k++)
								{
								    bool skipww = false;
									int ww = pre[b,w,k];
									for(int r=0; r<post_len[b,ww]; r++)
									{
                                        if(fsim[u,post[b,ww,r]]) 	// ww is in pre_b(sim(u))
											//continue Sharpen_pre_b_w;	// skip ww
                                        {
                                            skipww = true;
                                            break;
                                        }
									}
								
                                    if(!skipww)
                                    {
                                        if (b == a && u == v && !using_swap)
                                            swap[swap_len++] = ww;
                                        else
                                        {
                                            if (remove_len[b, u] == 0)
                                            {
                                                todo[todo_len++] = b*n_states + u;
                                            }
                                            remove[b, u, remove_len[b, u]++] = ww;
                                        }
                                    }



								}
							}
                        }
					}//End of if(fsim[u][w])
				}				
			}			
			if(swap_len>0)
			{	
				if(!using_swap)
				{	
					todo[todo_len++] = a*n_states + v;	
					using_swap = true; 
				}else{
					swap_len = 0;
					using_swap = false;
				}
			}
			
		}

		HashSet<Pair<FAState,FAState>> FSim2 = new HashSet<Pair<FAState,FAState>>();
		for(int p=0; p<n_states; p++)	
			for(int q=0; q<n_states; q++)
				if(fsim[p,q]) // q is in sim(p), q simulates p
					FSim2.Add(new Pair<FAState, FAState>(states[p],states[q]));
		return FSim2;
		
		
    }


    /**
* Compute forward simulation relation of a Buchi automaton using Henzinger, Henzinger, Kopke FOCS 1995
* @param omega: a Buchi automaton
* @param FSim: maximum simulation relation
* 
* @return simulation relation on states of the input automaton
*/
    public HashSet<Pair<FAState, FAState>> FastFSimRelNBW2(FiniteAutomaton omega, HashSet<Pair<FAState, FAState>> FSim) {

		Dictionary<State_Label, HashSet<FAState>> Remove=new Dictionary<State_Label, HashSet<FAState>>();

        Dictionary<String, int> symMap = new Dictionary<String, int>();
		int [,,] counter = new int[omega.states.Count, omega.states.Count, omega.alphabet.Count];
		for(int i=0;i<omega.states.Count;i++){
            for (int j = 0; j < omega.states.Count; j++)
            {
                for (int k = 0; k < omega.alphabet.Count; k++)
                {
					counter[i,j, k]=0;
				}
			}
		}

        foreach (FAState v in omega.states)
        {
            HashSet<String> sym_it = omega.getAllTransitionSymbols();

            int sym_index = 0;
            foreach (string sym in sym_it)
            {
                symMap.Add(sym, sym_index);
                sym_index++;
                HashSet<FAState> allStates = new HashSet<FAState>(omega.states);
                Remove.Add(new State_Label(v, sym), allStates);
            }
        }

        foreach (Pair<FAState, FAState> cur in FSim)
        {
            FAState v = cur.Left;
            FAState sim_v = cur.Right;

            foreach (KeyValuePair<string, HashSet<FAState>> symbol_it in sim_v.pre)
            {
                String symbol = symbol_it.Key;

                foreach (FAState from in symbol_it.Value)
                {
                    State_Label label = new State_Label(v, symbol);
                    if (Remove.ContainsKey(label))
                    {
                        Remove[label].Remove(from);
                    }

                    counter[from.id, v.id, symMap[symbol]]++;
                }
            }
        }

        while(Remove.Count > 0)
        {

            Dictionary<State_Label, HashSet<FAState>>.Enumerator iterator = Remove.GetEnumerator();

            State_Label key = iterator.Current.Key;
            HashSet<FAState> remove = iterator.Current.Value;
			Remove.Remove(key);

			FAState v= key.State;
			String symbol=key.Label;
			
            if(!v.pre.ContainsKey(symbol))
				continue;

            foreach (FAState u in v.pre[symbol])
            {

                foreach (FAState w in remove)
                {
                    
                	if(FSim.Contains(new Pair<FAState,FAState>(u,w))){
						FSim.Remove(new Pair<FAState,FAState>(u,w));

                	    foreach (KeyValuePair<string, HashSet<FAState>> symbol_it in w.pre)
                	    {
                	    
							String w_symbol=symbol_it.Key;
                	        foreach (FAState w_pre in symbol_it.Value)
                	        {
                	            counter[w_pre.id,u.id,symMap[w_symbol]]--;
								if(counter[w_pre.id,u.id,symMap[w_symbol]]==0)
								{
								    State_Label label = new State_Label(u, w_symbol);
                                    if (!Remove.ContainsKey(label))
                                    {
                                        HashSet<FAState> emptyStates = new HashSet<FAState>();
                                        Remove.Add(label, emptyStates);
									}
									Remove[label].Add(w_pre);
								}
							}
						}
					}
				}
			}
		}
		return FSim;
	}

    /**
    * Compute backward simulation relation of a Buchi automaton
    * @param omega: a Buchi automaton
    * @param BSim: the maximal bound of simulation relation
    * 
    * @return maximal simulation relation on states of the input automaton with BSim
    */
    public HashSet<Pair<FAState, FAState>> BSimRelNBW(FiniteAutomaton omega, HashSet<Pair<FAState, FAState>> BSim)
    {
        HashSet<Pair<FAState, FAState>> nextBSim = new HashSet<Pair<FAState, FAState>>();
        bool changed = true;
        while (changed)
        {
            changed = false;

            foreach (Pair<FAState, FAState> pair in BSim)
            {
                if (PreStateSimulated(BSim, omega, pair.Left, pair.Right))
                {
                    nextBSim.Add(new Pair<FAState, FAState>(pair.Left, pair.Right));
                }
                else
                {
                    changed = true;
                }
            }

            BSim = nextBSim;
            nextBSim = new HashSet<Pair<FAState, FAState>>();
        }
        return BSim;
    }

    private bool NextStateSimulated(HashSet<Pair<FAState, FAState>> sim, FiniteAutomaton omega, FAState p, FAState q)
    {

        foreach (KeyValuePair<string, HashSet<FAState>> symbol_it in p.next)
        {
            String a = symbol_it.Key;
            if (symbol_it.Value == null)
            {
                return false;
            }

            foreach (FAState p_next in symbol_it.Value)
            {
                bool hasSimulatingState = false;

                foreach (FAState q_next in q.next[a])
                {
                    if (sim.Contains(new Pair<FAState, FAState>(p_next, q_next)))
                    {
                        hasSimulatingState = true;
                        break;
                    }
                }

                if (!hasSimulatingState)
                {
                    return false;
                }
            }
        }

        return true;
    }	


    private bool PreStateSimulated(HashSet<Pair<FAState, FAState>> sim, FiniteAutomaton omega, FAState p, FAState q)
    {
        foreach (KeyValuePair<string, HashSet<FAState>> keyValuePair in p.pre)
        {
            String a = keyValuePair.Key;
            if (keyValuePair.Value == null)
            {
                return false;
            }

            foreach (FAState p_pre in keyValuePair.Value)
            {
                bool hasSimulatingState = false;

                foreach (FAState q_pre in q.pre[a])
                {
                    if (sim.Contains(new Pair<FAState, FAState>(p_pre, q_pre)))
                    {
                        hasSimulatingState = true;
                        break;
                    }
                }

                if (!hasSimulatingState)
                {
                    return false;
                }
            }
        }

        return true;
    }
    }
}
