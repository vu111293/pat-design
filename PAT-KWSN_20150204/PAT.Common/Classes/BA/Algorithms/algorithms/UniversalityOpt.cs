/* Written by Chen Yu-Fang and Hong Chih-Duo                              */
/* Copyright (c) 2010  Academia Sinica	                                  */
/*                                                                        */
/* This program is free software; you can redistribute it and/or modify   */
/* it under the terms of the GNU General Public License as published by   */
/* the Free Software Foundation; either version 2 of the License, or      */
/* (at your option) any later version.                                    */
/*                                                                        */
/* This program is distributed in the hope that it will be useful,        */
/* but WITHOUT ANY WARRANTY; without even the implied warranty of         */
/* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the          */
/* GNU General Public License for more details.                           */
/*                                                                        */
/* You should have received a copy of the GNU General Public License      */
/* along with this program; if not, write to the Free Software            */
/* Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA*/
using System;
//UPGRADE_TODO: The type 'java.lang.management.ManagementFactory' could not be found. If it was not included in the conversion, there may be compiler issues. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1262'"
using ManagementFactory = java.lang.management.ManagementFactory;
//UPGRADE_TODO: The type 'java.lang.management.ThreadMXBean' could not be found. If it was not included in the conversion, there may be compiler issues. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1262'"
using ThreadMXBean = java.lang.management.ThreadMXBean;
using FiniteAutomaton = cav2010.automata.FiniteAutomaton;
using FAState = cav2010.automata.FAState;
using GraphComparator = cav2010.comparator.GraphComparator;
using StatePairComparator = cav2010.comparator.StatePairComparator;
using Arc = cav2010.datastructure.Arc;
using OneToOneTreeMap = cav2010.datastructure.OneToOneTreeMap;
using Pair = cav2010.datastructure.Pair;
namespace cav2010.algorithms
{
	
	
	/// <summary> </summary>
	/// <author>  Yu-Fang Chen
	/// 
	/// </author>
	public class UniversalityOpt:SupportClass.ThreadClass
	{
		private void  InitBlock()
		{
			if (!Head.containsKey(g.toString()))
			{
				//UPGRADE_ISSUE: The following fragment of code could not be parsed and was not converted. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1156'"
				TreeSet < Integer > H = new TreeSet < Integer >();
				//UPGRADE_NOTE: There is an untranslated Statement.  Please refer to original code. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1153'"
				while (arc_g_it.hasNext())
				{
					Arc arc_g = arc_g_it.next();
					if (arc_g.From == init)
					{
						H.add(arc_g.To);
					}
				}
				Head.put(g.toString(), H);
			}
			if (!Tail.containsKey(h.toString()))
			{
				FiniteAutomaton fa = new FiniteAutomaton();
				//UPGRADE_ISSUE: The following fragment of code could not be parsed and was not converted. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1156'"
				OneToOneTreeMap < Integer, cav2010.automata.FAState > st = new OneToOneTreeMap < Integer, cav2010.automata.FAState >();
				//UPGRADE_NOTE: There is an untranslated Statement.  Please refer to original code. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1153'"
				while (arc_h_it.hasNext())
				{
					Arc arc_h = arc_h_it.next();
					if (!st.containsKey(arc_h.From))
						st.put(arc_h.From, fa.createState());
					if (!st.containsKey(arc_h.To))
						st.put(arc_h.To, fa.createState());
					fa.addTransition(st.getValue(arc_h.From), st.getValue(arc_h.To), arc_h.Label?"1":"0");
				}
				SCC s = new SCC(fa);
				//UPGRADE_ISSUE: The following fragment of code could not be parsed and was not converted. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1156'"
				TreeSet < Integer > T = new TreeSet < Integer >();
				//UPGRADE_NOTE: There is an untranslated Statement.  Please refer to original code. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1153'"
				while (s_it.hasNext())
				{
					T.add(st.getKey(s_it.next()));
				}
				int TailSize = 0;
				//UPGRADE_NOTE: There is an untranslated Statement.  Please refer to original code. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1153'"
				while (TailSize != T.size())
				{
					TailSize = T.size();
					//UPGRADE_ISSUE: The following fragment of code could not be parsed and was not converted. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1156'"
					TreeSet < Arc > isolatedArcsTemp = new TreeSet < Arc >();
					//UPGRADE_NOTE: There is an untranslated Statement.  Please refer to original code. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1153'"
					while (arc_it.hasNext())
					{
						Arc arc = arc_it.next();
						if (!T.contains(arc.To))
						{
							isolatedArcsTemp.add(arc);
						}
						else
						{
							T.add(arc.From);
						}
					}
					isolatedArcs = isolatedArcsTemp;
				}
				Tail.put(h.toString(), T);
			}
			//UPGRADE_ISSUE: The following fragment of code could not be parsed and was not converted. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1156'"
			TreeSet < Integer > intersection = new TreeSet < Integer >();
			intersection.addAll(Head.get_Renamed(g.toString()));
			intersection.retainAll(Tail.get_Renamed(h.toString()));
			
			if (debug_Renamed_Field)
			{
				if (intersection.isEmpty())
				{
					debug("g:" + g + ", Head: " + Head.get_Renamed(g.toString()));
					debug("h:" + h + ", Tail: " + Tail.get_Renamed(h.toString()));
				}
			}
			
			return !intersection.isEmpty();
			//UPGRADE_ISSUE: The following fragment of code could not be parsed and was not converted. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1156'"
			TreeSet < Arc > result = new TreeSet < Arc >();
			//UPGRADE_NOTE: There is an untranslated Statement.  Please refer to original code. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1153'"
			while (arc_it1.hasNext())
			{
				Arc cur = arc_it1.next();
				bool canAdd = true;
				//UPGRADE_NOTE: There is an untranslated Statement.  Please refer to original code. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1153'"
				while (arc_it2.hasNext())
				{
					Arc other = arc_it2.next();
					if (cur.From == other.From)
					{
						if (!cur.Label || other.Label)
						{
							if (cur.To != other.To)
							{
								//UPGRADE_ISSUE: The following fragment of code could not be parsed and was not converted. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1156'"
								if(rel.contains(new Pair < FAState, FAState >(new FAState(cur.getTo()), new FAState(other.getTo()))) && 
								result.contains(other))
								{
									canAdd = false;
									break;
								}
							}
						}
					}
				}
				if (canAdd)
				{
					result.add(cur);
				}
			}
			return result;
			//UPGRADE_ISSUE: The following fragment of code could not be parsed and was not converted. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1156'"
			ArrayList < TreeSet < Arc >> graphs = new ArrayList < TreeSet < Arc >>();
			
			//UPGRADE_NOTE: There is an untranslated Statement.  Please refer to original code. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1153'"
			while (symbol_it.hasNext())
			{
				//UPGRADE_ISSUE: The following fragment of code could not be parsed and was not converted. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1156'"
				TreeSet < Arc > graph = new TreeSet < Arc >();
				
				System.String sym = symbol_it.next();
				//UPGRADE_NOTE: There is an untranslated Statement.  Please refer to original code. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1153'"
				while (from_it.hasNext())
				{
					FAState from = from_it.next();
					if (from.getNext(sym) != null)
					{
						//UPGRADE_NOTE: There is an untranslated Statement.  Please refer to original code. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1153'"
						while (to_it.hasNext())
						{
							FAState to = to_it.next();
							if (input.F.contains(from) || input.F.contains(to))
							{
								graph.add(new Arc(from.id, true, to.id));
							}
							else
							{
								graph.add(new Arc(from.id, false, to.id));
							}
						}
					}
				}
				
				//UPGRADE_ISSUE: The following fragment of code could not be parsed and was not converted. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1156'"
				ArrayList < TreeSet < Arc >> toRemove = new ArrayList < TreeSet < Arc >>();
				bool canAdd = true;
				//UPGRADE_NOTE: There is an untranslated Statement.  Please refer to original code. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1153'"
				while (old_it.hasNext())
				{
					//UPGRADE_NOTE: There is an untranslated Statement.  Please refer to original code. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1153'"
					if (smallerThan(old, graph, rel))
					{
						canAdd = false;
						break;
					}
					else if (smallerThan(graph, old, rel))
					{
						toRemove.add(old);
					}
				}
				if (canAdd)
				{
					if (opt2)
						graphs.add(min(graph));
					else
						graphs.add(graph);
					graphs.removeAll(toRemove);
				}
			}
			return graphs;
			//UPGRADE_ISSUE: The following fragment of code could not be parsed and was not converted. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1156'"
			TreeSet < Arc > f = new TreeSet < Arc >();
			//UPGRADE_NOTE: There is an untranslated Statement.  Please refer to original code. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1153'"
			while (arc_g_it.hasNext())
			{
				Arc arc_g = arc_g_it.next();
				//UPGRADE_NOTE: There is an untranslated Statement.  Please refer to original code. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1153'"
				while (arc_h_it.hasNext())
				{
					Arc arc_h = arc_h_it.next();
					if (arc_g.To == arc_h.From)
					{
						if (arc_g.Label || arc_h.Label)
						{
							f.add(new Arc(arc_g.From, true, arc_h.To));
							f.remove(new Arc(arc_g.From, false, arc_h.To));
						}
						else
						{
							if (!f.contains(new Arc(arc_g.From, true, arc_h.To)))
							{
								f.add(new Arc(arc_g.From, false, arc_h.To));
							}
						}
					}
				}
			}
			return f;
			//UPGRADE_NOTE: There is an untranslated Statement.  Please refer to original code. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1153'"
			while (arc_g_it.hasNext())
			{
				Arc arc_g = arc_g_it.next();
				bool has_larger = false;
				//UPGRADE_NOTE: There is an untranslated Statement.  Please refer to original code. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1153'"
				while (arc_h_it.hasNext())
				{
					Arc arc_h = arc_h_it.next();
					if (arc_g.From == arc_h.From)
					{
						if (!arc_g.Label || arc_h.Label)
						{
							//UPGRADE_ISSUE: The following fragment of code could not be parsed and was not converted. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1156'"
							if(rel.contains(new Pair < FAState, FAState >(new FAState(arc_g.getTo()), new FAState(arc_h.getTo()))))
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
			
			FAState[] states = input.states.toArray(new FAState[0]);
			bool[] isFinal = new bool[states.Length];
			bool[][] fsim = new bool[states.Length][];
			for (int i = 0; i < states.Length; i++)
			{
				fsim[i] = new bool[states.Length];
			}
			// sim[u][v]=true iff v in sim(u) iff v simulates u
			
			for (int i = 0; i < states.Length; i++)
			{
				isFinal[i] = input.F.contains(states[i]);
			}
			for (int i = 0; i < states.Length; i++)
			{
				for (int j = i; j < states.Length; j++)
				{
					fsim[i][j] = (!isFinal[i] || isFinal[j]) && states[j].covers(states[i]);
					fsim[j][i] = (isFinal[i] || !isFinal[j]) && states[i].covers(states[j]);
				}
			}
			Simulation sim = new Simulation();
			//UPGRADE_ISSUE: The following fragment of code could not be parsed and was not converted. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1156'"
			Set < Pair < FAState, FAState >> FSim = sim.FastFSimRelNBW(input, fsim);
			
			return FSim;
		}
		virtual public long CpuTime
		{
			get
			{
				ThreadMXBean bean = ManagementFactory.getThreadMXBean();
				return bean.isCurrentThreadCpuTimeSupported()?bean.getCurrentThreadCpuTime():0L;
			}
			
		}
		virtual public bool Universal
		{
			get
			{
				return universal_Renamed_Field;
			}
			
		}
		virtual public long RunTime
		{
			get
			{
				return runTime;
			}
			
		}
		internal bool opt2 = true;
		internal bool debug_Renamed_Field = false;
		internal bool quotient_Renamed_Field = false;
		public int removedCnt;
		public bool universal_Renamed_Field = true;
		//UPGRADE_ISSUE: The following fragment of code could not be parsed and was not converted. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1156'"
		private TreeMap < String, TreeSet < Integer >> Tail = new TreeMap < String, TreeSet < Integer >>();
		//UPGRADE_ISSUE: The following fragment of code could not be parsed and was not converted. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1156'"
		private TreeMap < String, TreeSet < Integer >> Head = new TreeMap < String, TreeSet < Integer >>();
		private long runTime;
		private bool stop = false;
		//UPGRADE_ISSUE: The following fragment of code could not be parsed and was not converted. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1156'"
		private Set < Pair < cav2010.automata.FAState, cav2010.automata.FAState >> rel;
		internal FiniteAutomaton input;
		
		internal virtual void  debug(System.String out_Renamed)
		{
			if (debug_Renamed_Field)
				System.Console.Out.WriteLine(out_Renamed);
		}
		public UniversalityOpt(FiniteAutomaton input)
		{
			InitBlock();
			this.input = input;
		}
		/// <summary> Simplify a finite automaton by merging simulation equivalent states</summary>
		/// <param name="fa:">a finite automaton
		/// </param>
		/// <param name="Sim:">some simulation relation over states in the input automaton
		/// 
		/// </param>
		/// <returns> an equivalent finite automaton
		/// </returns>
		private FiniteAutomaton quotient(FiniteAutomaton fa)
		{
			FiniteAutomaton result = new FiniteAutomaton();
			//UPGRADE_ISSUE: The following fragment of code could not be parsed and was not converted. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1156'"
			TreeMap < FAState, FAState > map = new TreeMap < FAState, FAState >();
			//UPGRADE_ISSUE: The following fragment of code could not be parsed and was not converted. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1156'"
			TreeMap < FAState, FAState > reducedMap = new TreeMap < FAState, FAState >();
			
			//UPGRADE_NOTE: There is an untranslated Statement.  Please refer to original code. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1153'"
			while (state_it.hasNext())
			{
				FAState state = state_it.next();
				map.put(state, state);
				//UPGRADE_NOTE: There is an untranslated Statement.  Please refer to original code. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1153'"
				while (state_it2.hasNext())
				{
					FAState state2 = state_it2.next();
					//UPGRADE_ISSUE: The following fragment of code could not be parsed and was not converted. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1156'"
					if(rel.contains(new Pair < FAState, FAState >(state, state2)) && 
					rel.contains(new Pair < FAState, FAState >(state2, state)))
					{
						map.put(state, state2);
					}
				}
			}
			
			FAState init = result.createState();
			reducedMap.put(map.get_Renamed(fa.InitialState), init);
			result.InitialState = init;
			
			state_it = fa.states.iterator();
			while (state_it.hasNext())
			{
				FAState state = state_it.next();
				if (!reducedMap.containsKey(map.get_Renamed(state)))
				{
					reducedMap.put(map.get_Renamed(state), result.createState());
				}
				if (fa.F.contains(state))
				{
					result.F.add(reducedMap.get_Renamed(map.get_Renamed(state)));
				}
				//UPGRADE_NOTE: There is an untranslated Statement.  Please refer to original code. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1153'"
				while (sym_it.hasNext())
				{
					System.String sym = sym_it.next();
					//UPGRADE_NOTE: There is an untranslated Statement.  Please refer to original code. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1153'"
					while (to_it.hasNext())
					{
						FAState to = to_it.next();
						if (!reducedMap.containsKey(map.get_Renamed(to)))
						{
							reducedMap.put(map.get_Renamed(to), result.createState());
						}
						result.addTransition(reducedMap.get_Renamed(map.get_Renamed(state)), reducedMap.get_Renamed(map.get_Renamed(to)), sym);
					}
				}
			}
			//UPGRADE_ISSUE: The following fragment of code could not be parsed and was not converted. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1156'"
			Set < Pair < FAState, FAState >> newRel = new TreeSet < Pair < FAState, FAState >>(new StatePairComparator());
			//UPGRADE_ISSUE: The following fragment of code could not be parsed and was not converted. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1156'"
			Iterator < Pair < FAState, FAState >> sim_it = rel.iterator();
			while (sim_it.hasNext())
			{
				//UPGRADE_ISSUE: The following fragment of code could not be parsed and was not converted. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1156'"
				Pair < FAState, FAState > sim = sim_it.next();
				//UPGRADE_ISSUE: The following fragment of code could not be parsed and was not converted. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1156'"
				newRel.add(new Pair < FAState, FAState >(reducedMap.get(map.get(sim.getLeft())), reducedMap.get(map.get(sim.getRight()))));
			}
			rel.clear();
			rel = newRel;
			
			return result;
		}
		private bool lasso_finding_test;
		//UPGRADE_ISSUE: The following fragment of code could not be parsed and was not converted. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1156'"
		(TreeSet < Arc > g, TreeSet < Arc > h, int init)
		
		//UPGRADE_ISSUE: The following fragment of code could not be parsed and was not converted. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1156'"
		private TreeSet < Arc > min(TreeSet < Arc > original)
		
		
		//UPGRADE_ISSUE: The following fragment of code could not be parsed and was not converted. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1156'"
		private ArrayList < TreeSet < Arc >> buildSingleCharacterGraphs()
		
		//UPGRADE_ISSUE: The following fragment of code could not be parsed and was not converted. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1156'"
		private TreeSet < Arc > compose(TreeSet < Arc > g, TreeSet < Arc > h)
		
		internal bool smallerThan;
		//UPGRADE_ISSUE: The following fragment of code could not be parsed and was not converted. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1156'"
		(TreeSet < Arc > g, TreeSet < Arc > h, Set < Pair < FAState, FAState >> rel)
		public virtual void  universalAS()
		{
			rel = computeSim();
			if (quotient_Renamed_Field)
				input = quotient(input);
			debug(input.ToString());
			debug("Sim=" + rel.toString());
			this.universal_Renamed_Field = universal();
		}
		//UPGRADE_ISSUE: The following fragment of code could not be parsed and was not converted. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1156'"
		Override
		override public void  Run()
		{
			
			runTime = CpuTime;
			universalAS();
			runTime = CpuTime - runTime;
		}
		
		
		//UPGRADE_ISSUE: The following fragment of code could not be parsed and was not converted. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1156'"
		Set < Pair < FAState, FAState >> computeSim()
		
		private bool universal()
		{
			
			//UPGRADE_NOTE: There is an untranslated Statement.  Please refer to original code. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1153'"
			//UPGRADE_NOTE: There is an untranslated Statement.  Please refer to original code. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1153'"
			while (Q1_it.hasNext())
			{
				//UPGRADE_NOTE: There is an untranslated Statement.  Please refer to original code. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1153'"
				if (!lasso_finding_test(g, g, input.InitialState.id))
				{
					return false;
				}
			}
			//UPGRADE_ISSUE: The following fragment of code could not be parsed and was not converted. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1156'"
			TreeSet < TreeSet < Arc >> Next = new TreeSet < TreeSet < Arc >>(new GraphComparator());
			//UPGRADE_ISSUE: The following fragment of code could not be parsed and was not converted. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1156'"
			TreeSet < TreeSet < Arc >> Processed = new TreeSet < TreeSet < Arc >>(new GraphComparator());
			Next.addAll(Q1);
			while (!Next.isEmpty())
			{
				if (stop)
					break;
				
				//UPGRADE_NOTE: There is an untranslated Statement.  Please refer to original code. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1153'"
				//UPGRADE_NOTE: There is an untranslated Statement.  Please refer to original code. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1153'"
				while (Processed_it.hasNext())
				{
					//UPGRADE_NOTE: There is an untranslated Statement.  Please refer to original code. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1153'"
					if (!lasso_finding_test(g, h, input.InitialState.id) || !lasso_finding_test(h, g, input.InitialState.id))
						return false;
				}
				Next.remove(g);
				Processed.add(g);
				debug("Processed:" + Processed);
				debug("Next:" + Next);
				
				Q1_it = Q1.iterator();
				while (Q1_it.hasNext())
				{
					//UPGRADE_ISSUE: The following fragment of code could not be parsed and was not converted. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1156'"
					ArrayList < TreeSet < Arc >> toRemove = new ArrayList < TreeSet < Arc >>();
					//UPGRADE_NOTE: There is an untranslated Statement.  Please refer to original code. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1153'"
					//UPGRADE_NOTE: There is an untranslated Statement.  Please refer to original code. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1153'"
					bool discard = false;
					
					debug("f:" + f + "=" + g + ";" + h);
					
					Processed_it = Processed.iterator();
					while (Processed_it.hasNext())
					{
						//UPGRADE_NOTE: There is an untranslated Statement.  Please refer to original code. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1153'"
						if (smallerThan(f, p, rel))
						{
							toRemove.add(p);
						}
						if (smallerThan(p, f, rel))
						{
							discard = true;
							break;
						}
					}
					if (discard)
						continue;
					//UPGRADE_NOTE: There is an untranslated Statement.  Please refer to original code. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1153'"
					while (Next_it.hasNext())
					{
						//UPGRADE_NOTE: There is an untranslated Statement.  Please refer to original code. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1153'"
						if (smallerThan(f, p, rel))
						{
							toRemove.add(p);
						}
						if (smallerThan(p, f, rel))
						{
							discard = true;
							break;
						}
					}
					if (discard)
						continue;
					if (!lasso_finding_test(f, f, input.InitialState.id))
						return false;
					Processed.removeAll(toRemove);
					Next.removeAll(toRemove);
					if (opt2)
						Next.add(min(f));
					else
						Next.add(f);
				}
			}
			return true;
		}
		
		public virtual void  stopIt()
		{
			stop = true;
		}
	}
}