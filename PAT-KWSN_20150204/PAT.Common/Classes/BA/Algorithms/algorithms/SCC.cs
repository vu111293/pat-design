using System;
using System.Collections.Generic;
using PAT.Common.Classes.Assertion.Algorithms.automata;
using PAT.Common.Classes.BA.Algorithms.automata;

namespace PAT.Common.Classes.BA.Algorithms.algorithms
{
	
	public class SCC
	{

		int index = 0;
		Stack < FAState > S = new Stack < FAState >();
		FiniteAutomaton fa;
		Dictionary<int, int> v_index = new Dictionary<int, int>();
		Dictionary<int, int> v_lowlink = new Dictionary<int, int>();

		HashSet<FAState> OneSCC = new HashSet<FAState>();
		
		public HashSet< FAState > getResult()
		{
		    return OneSCC;
		}
		
		public SCC(FiniteAutomaton fa)
		{		
			this.fa = fa;
		    foreach (FAState st in fa.states)
		    {		        
				if (!v_index.ContainsKey(st.id))
				{
					tarjan(st);
				}
		    }
		}
		
		internal virtual void  tarjan(FAState v)
		{
			v_index.Add(v.id, index);
            v_lowlink.Add(v.id, index);
			index++;
			S.Push(v);

		    foreach (KeyValuePair<string, HashSet<FAState>> pair in v.next)
		    {
		        //System.String next = pair.Key;
		        foreach (FAState v_prime in pair.Value)
		        {
                    if (!v_index.ContainsKey(v_prime.id))
                    {
                        tarjan(v_prime);
                        v_lowlink.Add(v.id, Math.Min(v_lowlink[v.id], v_lowlink[v_prime.id]));
                    }
                    else if (S.Contains(v_prime))
                    {
                        v_lowlink.Add(v.id, Math.Min(v_lowlink[v.id], v_index[v_prime.id]));
                    }
		        }
		    }

			if (v_lowlink[v.id] == v_index[v.id])
			{
				HashSet<FAState> SCC = new HashSet<FAState>();
				while (S.Count > 0)
				{
					FAState t = S.Pop();
					SCC.Add(t);

					if (t.id == v.id)
					{
					    break;
					}
				}

			    foreach (FAState st in SCC)
			    {
                    if (st.next.ContainsKey("1"))
                    {
                        HashSet<FAState> states = st.next["1"];
                        
                        //states..retainAll(SCC);
                        if (states.Overlaps(SCC))
                        {
                            foreach (FAState state in SCC)
                            {
                                OneSCC.Add(state);
                            }
                            
                            ////is 1-SCC
                            ////UPGRADE_NOTE: There is an untranslated Statement.  Please refer to original code. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1153'"
                            //while (SCC_it2.hasNext())
                            //{
                            //    OneSCC.add(SCC_it2.next());
                            //}
                            break;
                        }
                    }

			    }
			}
		}
		
		/*
		procedure tarjan(v)
		index = index + 1
		S.push(v)                       // Push v on the stack
		forall (v, v') in E do          // Consider successors of v
		if (v'.index is undefined)    // Was successor v' visited?
		tarjan(v')                // Recurse
		v.lowlink = min(v.lowlink, v'.lowlink)
		else if (v' is in S)          // Was successor v' in stack S? 
		v.lowlink = min(v.lowlink, v'.index)
		if (v.lowlink == v.index)       // Is v the root of an SCC?
		print "SCC:"
		repeat
		v' = S.pop
		print v'
		until (v' == v)
		*/
	}
}