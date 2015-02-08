using System;
using System.Collections.Generic;
using PAT.Common.Classes.Assertion.Algorithms.automata;
using PAT.Common.Classes.BA.Algorithms.automata;
using PAT.Common.Classes.BA.Algorithms.datastructure;

namespace PAT.Common.Classes.BA.Algorithms.algorithms
{
	
	
	/// <summary> </summary>
	/// <author>  Yu-Fang Chen
	/// 
	/// </author>
	public class InclusionOpt
	{
	
        internal bool opt2 = true;
		internal bool debug_Renamed_Field = false;
		bool quotient_var = true;
		public int removedCnt;
		public bool included_Renamed_Field = true;
        private Dictionary<String, HashSet<int>> Tail = new Dictionary<String, HashSet<int>>();
        private Dictionary<String, HashSet<int>> Head = new Dictionary<String, HashSet<int>>();
		private long runTime;
		private bool stop = false;
		private HashSet < Pair < FAState, FAState >> rel_spec, rel_system;
		internal FiniteAutomaton spec, system;
		internal System.String showSim_var;


        string showSim(HashSet<Pair<FAState, FAState>> sim)
        {
            String result = "";

            foreach (Pair<FAState, FAState> p in sim)
            {
                if (p.Left.ID != p.Right.ID)
                {
                    result += ("(" + p.Left + "," + p.Right + ")");
                }

            }
            return result;
        }

	public InclusionOpt(FiniteAutomaton system, FiniteAutomaton spec){
		this.spec=spec;
		this.system=system;
	}
	/**
	 * Simplify a finite automaton by merging simulation equivalent states
	 * @param fa: a finite automaton
	 * @param Sim: some simulation rel_specation over states in the spec automaton
	 * 
	 * @return an equivalent finite automaton
	 */
	private FiniteAutomaton quotient(FiniteAutomaton fa, HashSet<Pair<FAState,FAState>> rel) {
		FiniteAutomaton result=new FiniteAutomaton();
		Dictionary<FAState,FAState> map=new Dictionary<FAState,FAState>();
		Dictionary<FAState,FAState> reducedMap=new Dictionary<FAState,FAState>();

	    foreach (FAState state in fa.states)
	    {
	        map.Add(state, state);
	        foreach (FAState state2 in fa.states)
	        {
				if(rel.Contains(new Pair<FAState,FAState>(state,state2)) &&
					rel.Contains(new Pair<FAState,FAState>(state2,state))){
					map.Add(state,state2);
				}
			}			
		}

		FAState init=result.createState();
		reducedMap.Add(map[fa.InitialState], init);
	    result.InitialState = init;

        foreach (FAState state in fa.states)
	    {
            if(!reducedMap.ContainsKey(map[state])){
				reducedMap.Add(map[state], result.createState());
			}
			if(fa.F.Contains(state)){
				result.F.Add(reducedMap[map[state]]);
			}
	        foreach (KeyValuePair<string, HashSet<FAState>> sym_it in state.next)
	        {
	            String sym=sym_it.Key;
	            foreach (FAState to in sym_it.Value)
	            {
	                if(!reducedMap.ContainsKey(map[to])){
						reducedMap.Add(map[to], result.createState());
					}
					result.addTransition(reducedMap[map[state]], reducedMap[map[to]], sym);
				}
			}
		}
		HashSet<Pair<FAState,FAState>> newrel_spec=new HashSet<Pair<FAState,FAState>>();

	    foreach (Pair<FAState, FAState> sim in rel)
	    {
	        newrel_spec.Add(new Pair<FAState,FAState>(reducedMap[map[sim.Left]],reducedMap[map[sim.Right]]));
		}
		rel.Clear();
		rel = new HashSet<Pair<FAState,FAState>>(newrel_spec);
		
		return result;
	}	
	private bool double_graph_test(Pair<Arc,HashSet<Arc>> g, Pair<Arc,HashSet<Arc>> h, int init){
		Arc g_arc=g.Left;
		Arc h_arc=h.Left;
		
		if(!(rel_system.Contains(new Pair<FAState,FAState>(new FAState(h_arc.From),new FAState(h_arc.To))) &&
			 rel_system.Contains(new Pair<FAState,FAState>(new FAState(h_arc.From),new FAState(g_arc.To))) &&
			 system.InitialState.id==g_arc.From && system.F.Contains(new FAState(h_arc.From))
		)){
			return true;
		}else{
			bool result = lasso_finding_test(g.Right,h.Right,init);
			if(!result){
				//debug("g_arc: "+g_arc);
				//debug("h_arc: "+h_arc);
				//debug("init: "+system.InitialState);
				//debug("final: "+system.F);
			}
			return result;
		}
	}	
		
		
	
	private bool lasso_finding_test(HashSet<Arc> g, HashSet<Arc> h, int init){
		if(!Head.ContainsKey(g.ToString())){
			HashSet<int> H=new HashSet<int>();

		    foreach (Arc arc_g in g)
		    {
		        if(arc_g.From==init){
					H.Add(arc_g.To);
				}
			}
			Head.Add(g.ToString(), H);
		}
		if(!Tail.ContainsKey(h.ToString())){
			FiniteAutomaton fa=new FiniteAutomaton();
			OneToOneTreeMap<int,FAState> st=new OneToOneTreeMap<int,FAState>();
		    foreach (Arc arc_h in h)
		    {
		        if(!st.containsKey(arc_h.From))
					st.put(arc_h.From, fa.createState());
				if(!st.containsKey(arc_h.To))
					st.put(arc_h.To, fa.createState());
				fa.addTransition(st.getValue(arc_h.From), st.getValue(arc_h.To), arc_h.Label?"1":"0");
			}
			SCC s=new SCC(fa);
			HashSet<int> T=new HashSet<int>();
            foreach (FAState state in s.getResult())
		    {
		        T.Add(st.getKey(state));
			}

			int TailSize=0;
			HashSet<Arc> isolatedArcs=h;
			while(TailSize!=T.Count){
                TailSize = T.Count;

				HashSet<Arc> isolatedArcsTemp=new HashSet<Arc>();

			    foreach (Arc arc in isolatedArcs)
			    {
			        if(!T.Contains(arc.To)){
						isolatedArcsTemp.Add(arc);
					}else{
						T.Add(arc.From);
					}
				}
				isolatedArcs=isolatedArcsTemp;
			}
			Tail.Add(h.ToString(), T);
		}
        HashSet<int> intersection = new HashSet<int>(Head[g.ToString()]);
		//intersection.retainAll(Tail[h.ToString()]);

        intersection.IntersectWith(Tail[h.ToString()]);
		
        //if(debug){
        //    if(intersection.isEmpty()){
        //        //debug("g_graph:"+g+", Head: "+Head.get(g.ToString()));
        //        //debug("h_graph:"+h+", Tail: "+Tail.get(h.ToString()));
        //    }
        //}
		
		return intersection.Count > 0;
	}
	
	private Pair<Arc, HashSet<Arc>> min(Pair<Arc, HashSet<Arc>> supergraph){
		
        HashSet<Arc> result=new HashSet<Arc>();
	    foreach (Arc cur in supergraph.Right)
	    {
	        bool canAdd=true;
            foreach (Arc other in supergraph.Right)
            {
                if (cur.From == other.From)
                {
                    if (!cur.Label || other.Label)
                    {
                        if (cur.To != other.To)
                        {
                            if (rel_spec.Contains(new Pair<FAState, FAState>(new FAState(cur.To), new FAState(other.To))))
                            {
                                canAdd = false;
                                break;
                            }
                        }
                    }
                }
            }
	        if(canAdd){
				result.Add(cur);
			}
		}
		return new Pair<Arc, HashSet<Arc>>(supergraph.Left,result);
	}
	
	private List<Pair<Arc, HashSet<Arc>>> buildSingleCharacterSuperGraphs(){

		List<Pair<Arc, HashSet<Arc>>> supergraphs=new List<Pair<Arc, HashSet<Arc>>>();

	    foreach (string sym in system.getAllTransitionSymbols())
	    {
	        HashSet<Arc> graph=new HashSet<Arc>();

	        foreach (FAState from in spec.states)
	        {
	            if(from.next.ContainsKey(sym)){

	                foreach (FAState to in from.next[sym])
	                {
	                    if(spec.F.Contains(from)||spec.F.Contains(to)){
							graph.Add(new Arc(from.id,true,to.id));
						}else{
							graph.Add(new Arc(from.id,false,to.id));
						}
					}
				}
			}


            foreach (FAState from in spec.states)
            {
                if (from.next.ContainsKey(sym))
                {


                    foreach (FAState to in from.next[sym])
                    {
						Arc left_arc=new Arc(from.id,false,to.id);
						Pair<Arc, HashSet<Arc>> supergraph=new Pair<Arc, HashSet<Arc>>(left_arc,graph);
						List<Pair<Arc, HashSet<Arc>>> toRemove=new List<Pair<Arc, HashSet<Arc>>>();
						bool canAdd=true;

                        foreach (Pair<Arc, HashSet<Arc>> old in supergraphs)
                        {
                            if(smallerThan(old, supergraph)){
								canAdd=false;
								break;
							}else if(smallerThan(supergraph, old)){
								toRemove.Add(old);
							}
						}
						if(canAdd){
							if(opt2)
								supergraphs.Add(min(supergraph));
							else
								supergraphs.Add(supergraph);

                            //supergraphs.removeAll(toRemove);
						    foreach (Pair<Arc, HashSet<Arc>> pair in toRemove)
						    {
						        supergraphs.Remove(pair);
						    }                            
						}
					}
				}
			}
		}
		return supergraphs;
	}

    private Pair<Arc, HashSet<Arc>> compose(Pair<Arc, HashSet<Arc>> g, Pair<Arc, HashSet<Arc>> h)
    {
        HashSet<Arc> f = new HashSet<Arc>();
        foreach (Arc arc_g in g.Right)
        {
            foreach (Arc arc_h in h.Right)
            {

                if (arc_g.To == arc_h.From)
                {
                    if (arc_g.Label || arc_h.Label)
                    {
                        f.Add(new Arc(arc_g.From, true, arc_h.To));
                        f.Remove(new Arc(arc_g.From, false, arc_h.To));
                    }
                    else
                    {
                        if (!f.Contains(new Arc(arc_g.From, true, arc_h.To)))
                        {
                            f.Add(new Arc(arc_g.From, false, arc_h.To));
                        }
                    }
                }
            }
        }
        return new Pair<Arc, HashSet<Arc>>(new Arc(g.Left.From, false, h.Left.To), f);
    }

	    bool smallerThan(Pair<Arc, HashSet<Arc>> old, Pair<Arc, HashSet<Arc>> old2){
		Arc old_arc=old.Left;
		Arc old2_arc=old2.Left;
		if(!(old_arc.From==old2_arc.From && 
		rel_system.Contains(new Pair<FAState,FAState>(new FAState(old2_arc.To),new FAState(old_arc.To))))){
			return false;
		}

	    foreach (Arc arc_g in old.Right)
	    {
	        bool has_larger=false;

	        foreach (Arc arc_h in old2.Right)
	        {
	            if(arc_g.From==arc_h.From){
					if(!arc_g.Label||arc_h.Label){
						if(rel_spec.Contains(new Pair<FAState,FAState>(new FAState(arc_g.To),new FAState(arc_h.To)))){
							has_larger=true;
							break;
						}
					}
				}
			}			
			if(!has_larger){
				return false;
			}
		}
		return true;
	}
	public bool inclusionTest(){
		this.computeSim();
		if(quotient_var){
			spec=quotient(spec, rel_spec);
			system=quotient(system, rel_system);
		}
		//debug("Spec.:");
		//debug(spec.ToString());
		//debug("Sim="+showSim(rel_spec));
		//debug("System:");
		//debug(system.ToString());
		//debug("Sim="+showSim(rel_system));
		return included();
		
	}

	
	void computeSim(){

		FAState[] states=spec.states.ToArray();		
		bool[] isFinal = new bool[states.Length];
		bool[,] fsim = new bool[states.Length,states.Length];
		// sim[u][v]=true iff v in sim(u) iff v simulates u
		
		for(int i=0;i<states.Length;i++){			
			isFinal[i] = spec.F.Contains(states[i]);
		}
		for(int i=0;i<states.Length;i++){
			for(int j=i;j<states.Length;j++){
				fsim[i,j] = (!isFinal[i] || isFinal[j]) && states[j].covers(states[i]);
				fsim[j,i] = (isFinal[i] || !isFinal[j]) && states[i].covers(states[j]);
			}
		}
		Simulation sim = new Simulation();
		rel_spec = sim.FastFSimRelNBW(spec, fsim);

		states=system.states.ToArray();		
		isFinal = new bool[states.Length];
		fsim = new bool[states.Length,states.Length];
		
		for(int i=0;i<states.Length;i++){			
			isFinal[i] = system.F.Contains(states[i]);
		}
		for(int i=0;i<states.Length;i++){
			for(int j=i;j<states.Length;j++){
				fsim[i,j] = (!isFinal[i] || isFinal[j]) && states[j].covers(states[i]);
				fsim[j,i] = (isFinal[i] || !isFinal[j]) && states[i].covers(states[j]);
			}
		}
		sim = new Simulation();
		rel_system = sim.FastFSimRelNBW(system,fsim);
		
	}	
	
	private bool included(){
		
		List<Pair<Arc, HashSet<Arc>>> Q1=this.buildSingleCharacterSuperGraphs();

        foreach (Pair<Arc, HashSet<Arc>> g in Q1)
        {
            if (!double_graph_test(g, g, spec.InitialState.id))
            {
                return false;
            }
        }
        HashSet<Pair<Arc, HashSet<Arc>>> Next = new HashSet<Pair<Arc, HashSet<Arc>>>(Q1);
		HashSet<Pair<Arc, HashSet<Arc>>> Processed=new HashSet<Pair<Arc, HashSet<Arc>>>();
		
		while(Next.Count > 0){
            HashSet<Pair<Arc, HashSet<Arc>>>.Enumerator Enumerator = Next.GetEnumerator();
            Pair<Arc, HashSet<Arc>> g = Enumerator.Current;

            foreach (Pair<Arc, HashSet<Arc>> h in Processed)
            {
				if(!double_graph_test(g, h, spec.InitialState.id)||!double_graph_test(h, g, spec.InitialState.id))
					return false;
			}

            Next.Remove(g);

            Processed.Add(g);
			//debug("Processed:"+Processed);
			//debug("Next:"+Next);

            foreach (Pair<Arc, HashSet<Arc>> h in Q1)
            {
				List<Pair<Arc, HashSet<Arc>>> toRemove=new List<Pair<Arc, HashSet<Arc>>>();
				if(composable(g,h)){
				Pair<Arc, HashSet<Arc>> f=compose(g, h);
				bool discard=false;

				//debug("f:"+f +"="+g+";"+h);


                foreach (Pair<Arc, HashSet<Arc>> p in Processed)
                {

					if(smallerThan(f, p)){
						toRemove.Add(p);
					}
					if(smallerThan(p, f)){
						discard=true;
						break;
					}
				}
				if(discard)
					continue;

                foreach (Pair<Arc, HashSet<Arc>> p in Next)
                {

					if(smallerThan(f, p)){
						toRemove.Add(p);
					}
					if(smallerThan(p, f)){
						discard=true;
						break;
					}
				}
				if(discard)
					continue;
				if(!double_graph_test(f, f, spec.InitialState.id))
					return false;

				//Processed.removeAll(toRemove);
				//Next.removeAll(toRemove);

                foreach (Pair<Arc, HashSet<Arc>> pair in toRemove)
                {
                    Processed.Remove(pair);
                    Next.Remove(pair);
                }

				if(opt2)
					Next.Add(min(f));
				else
					Next.Add(f);
				}
			}
		}			
		return true;
	}

	private bool composable(Pair<Arc, HashSet<Arc>> g,Pair<Arc, HashSet<Arc>> h) {
		if(g.Left.To==h.Left.From)
			return true;
		else
			return false;
	}

	}
}