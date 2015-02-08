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
	public class InclusionAnti
	{
        //bool debug_Renamed_Field = false;
		public int removedCnt;
		public bool included_Renamed_Field = true;
		private Dictionary < String, HashSet < int >> Tail = new Dictionary < String, HashSet < int >>();
		private Dictionary < String, HashSet < int >> Head = new Dictionary < String, HashSet < int >>();
        //private long runTime;
        //private bool stop = false;
		internal FiniteAutomaton spec, system;


	public InclusionAnti(FiniteAutomaton system, FiniteAutomaton spec){
		this.spec=spec;
		this.system=system;
	}

	private bool double_graph_test(Pair<Arc,HashSet<Arc>> g, Pair<Arc,HashSet<Arc>> h, int init){
		Arc g_arc=g.Left;
		Arc h_arc=h.Left;
		if(!(g_arc.To==h_arc.From && h_arc.To==h_arc.From &&
			 system.InitialState.id==g_arc.From && system.F.Contains(new FAState(h_arc.From))
		)){
			return true;
		}else{
			bool result = lasso_finding_test(g.Right,h.Right,init);
			if(!result){
				////debug("g_arc: "+g_arc);
				////debug("h_arc: "+h_arc);
				////debug("init: "+system.InitialState);
				////debug("final: "+system.F);
			}
			return result;
		}
	}	
		
		
	
	private bool lasso_finding_test(HashSet<Arc> g, HashSet<Arc> h, int init)
    {
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
                if (!st.containsKey(arc_h.From))
                    st.put(arc_h.From, fa.createState());
                if (!st.containsKey(arc_h.To))
                    st.put(arc_h.To, fa.createState());
                fa.addTransition(st.getValue(arc_h.From), st.getValue(arc_h.To), arc_h.Label ? "1" : "0");
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
				TailSize=T.Count;

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
	
	
	private List<Pair<Arc, HashSet<Arc>>> buildSingleCharacterSuperGraphs()
    {
		List<Pair<Arc, HashSet<Arc>>> supergraphs=new List<Pair<Arc, HashSet<Arc>>>();
		
		//Iterator<String> symbol_it=system.getAllTransitionSymbols().iterator();

        foreach (string sym in system.getAllTransitionSymbols())
	    {
	        HashSet<Arc> graph=new HashSet<Arc>();
			
			//String sym=symbol_it.next();

	        foreach (FAState from in spec.states)
	        {
	            if(from.next.ContainsKey(sym)){
					HashSet<FAState> to_it=from.next[sym];
	                foreach (FAState to in to_it)
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
                    HashSet<FAState> to_it = from.next[sym];
                    foreach (FAState to in to_it)
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
							supergraphs.Add(supergraph);
                            //supergraphs.RemoveAll(toRemove);
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
		HashSet<Arc> f=new HashSet<Arc>();

        foreach (Arc arc_g in g.Right)
	    {
	        foreach (Arc arc_h in h.Right)
	        {
	            if(arc_g.To==arc_h.From){
					if(arc_g.Label||arc_h.Label){
						f.Add(new Arc(arc_g.From,true,arc_h.To));
						f.Remove(new Arc(arc_g.From,false,arc_h.To));
					}else{
						if(!f.Contains(new Arc(arc_g.From,true,arc_h.To))){
							f.Add(new Arc(arc_g.From,false,arc_h.To));
						}
					}
				}				
			}			
		}
		return new Pair<Arc, HashSet<Arc>>(new Arc(g.Left.From,false,h.Left.To),f);
	}

    bool smallerThan(Pair<Arc, HashSet<Arc>> old, Pair<Arc, HashSet<Arc>> old2)
    {
        Arc old_arc = old.Left;
        Arc old2_arc = old2.Left;
        if (!(old_arc.From == old2_arc.From && old_arc.To == old2_arc.To))
            return false;


        foreach (Arc arc_g in old.Right)
        {
            bool has_larger = false;
            foreach (Arc arc_h in old2.Right)
            {
                if (arc_g.From == arc_h.From)
                {
                    if (!arc_g.Label || arc_h.Label)
                    {
                        if (arc_g.To == arc_h.To)
                        {
                            has_larger = true;
                            break;
                        }
                    }
                }
            }
            if (!has_larger)
            {
                return false;
            }
        }
        return true;
    }

    public void inclusionTest()
    {
        ////debug("Spec.:");
        ////debug(spec.ToString());
        ////debug("System:");
        ////debug(system.ToString());
        this.included_Renamed_Field = included();

    }

	    //public long getCpuTime( ) {
    //    ThreadMXBean bean = ManagementFactory.getThreadMXBean( );
    //    return bean.isCurrentThreadCpuTimeSupported( ) ?
    //        bean.getCurrentThreadCpuTime( ) : 0L;
    //}
	
    //public bool isIncluded(){
    //    return included;
    //}
	
    //public void run(){

    //    runTime=getCpuTime();
    //        inclusionTest();
    //    runTime=getCpuTime()-runTime;
    //}
	
    //public long getRunTime(){
    //    return runTime;
    //}



        private bool included()
        {

            List<Pair<Arc, HashSet<Arc>>> Q1 = this.buildSingleCharacterSuperGraphs();

            foreach (Pair<Arc, HashSet<Arc>> g in Q1)
            {
                if (!double_graph_test(g, g, spec.InitialState.id))
                {
                    return false;
                }
            }
            //new SuperGraphComparator()
            HashSet<Pair<Arc, HashSet<Arc>>> Next = new HashSet<Pair<Arc, HashSet<Arc>>>(Q1);
            HashSet<Pair<Arc, HashSet<Arc>>> Processed = new HashSet<Pair<Arc, HashSet<Arc>>>();
            //Next.addAll(Q1);
            while (Next.Count > 0)
            {
                //if(stop)
                //	break;

                HashSet<Pair<Arc, HashSet<Arc>>>.Enumerator Enumerator = Next.GetEnumerator();
                Pair<Arc, HashSet<Arc>> g = Enumerator.Current;

                foreach (Pair<Arc, HashSet<Arc>> h in Processed)
                {
                    if (!double_graph_test(g, h, spec.InitialState.id) || !double_graph_test(h, g, spec.InitialState.id))
                        return false;
                }

                Next.Remove(g);
                Processed.Add(g);
                //debug("Processed:"+Processed);
                //debug("Next:"+Next);

                foreach (Pair<Arc, HashSet<Arc>> h in Q1)
                {
                    List<Pair<Arc, HashSet<Arc>>> toRemove = new List<Pair<Arc, HashSet<Arc>>>();
                    //Pair<Arc, HashSet<Arc>> h=Q1_it.next();
                    if (composable(g, h))
                    {
                        Pair<Arc, HashSet<Arc>> f = compose(g, h);
                        bool discard = false;

                        //debug("f:"+f +"="+g+";"+h);

                        foreach (Pair<Arc, HashSet<Arc>> p in Processed)
                        {

                            if (smallerThan(f, p))
                            {
                                toRemove.Add(p);
                            }
                            if (smallerThan(p, f))
                            {
                                discard = true;
                                break;
                            }
                        }
                        if (discard)
                            continue;

                        foreach (Pair<Arc, HashSet<Arc>> p in Next)
                        {

                            if (smallerThan(f, p))
                            {
                                toRemove.Add(p);
                            }
                            if (smallerThan(p, f))
                            {
                                discard = true;
                                break;
                            }
                        }
                        if (discard)
                            continue;
                        if (!double_graph_test(f, f, spec.InitialState.id))
                            return false;

                        foreach (Pair<Arc, HashSet<Arc>> pair in toRemove)
                        {
                            Processed.Remove(pair);
                            Next.Remove(pair);
                        }

                        //Processed.removeAll(toRemove);
                        //Next.removeAll(toRemove);
                        
                        
                        Next.Add(f);
                    }
                }
            }
            return true;
        }

	    private bool composable(Pair<Arc, HashSet<Arc>> g, Pair<Arc, HashSet<Arc>> h) 
    {
		if(g.Left.To==h.Left.From)
			return true;
		else
			return false;
	}
    //public void stopIt(){
    //    stop=true;
    //}
	}
}