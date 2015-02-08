using System.Collections.Generic;
using System.Linq;
using PAT.Common.Classes.Assertion;
using PAT.Common.Classes.LTS;
using PAT.Common.Classes.Ultility;

namespace PAT.Common.Classes.DataStructure
{
    public partial class Automata
    {

        public static void TraceInclusionCheck(ConfigurationBase currentImpl, Automata spec, VerificationOutput VerificationOutput)
        {
            FAState[] states = spec.States.Values.ToArray();
            //bool[] isFinal = new bool[states.Length];
            bool[,] fsim = new bool[states.Length, states.Length];
            
            // sim[u][v]=true iff v in sim(u) iff v simulates u

            //for (int i = 0; i < states.Length; i++)
            //{
            //    isFinal[i] = spec.F.Contains(states[i]);
            //}

            for (int i = 0; i < states.Length; i++)
            {
                for (int j = i; j < states.Length; j++)
                {
                    fsim[i, j] = states[j].covers(states[i]); //(!isFinal[i] || isFinal[j]) && 
                    fsim[j, i] = states[i].covers(states[j]); //(isFinal[i] || !isFinal[j]) && 
                }
            }

            Dictionary<string, HashSet<FAState>> rel_spec = FastFSimRelNBW(spec, fsim);


            StringHashTable Visited = new StringHashTable(Ultility.Ultility.MC_INITIAL_SIZE);
            List<ConfigurationBase> toReturn = new List<ConfigurationBase>();

            Stack<ConfigurationBase> pendingImpl = new Stack<ConfigurationBase>(1024);
            Stack<NormalizedFAState> pendingSpec = new Stack<NormalizedFAState>(1024);

            //The following are for identifying a counterexample trace. 
            Stack<int> depthStack = new Stack<int>(1024);
            depthStack.Push(0);
            List<int> depthList = new List<int>(1024);
            //The above are for identifying a counterexample trace. 

            //implementation initial state
            pendingImpl.Push(currentImpl);

            //specification initial state
            NormalizedFAState currentSpec = new NormalizedFAState(spec.InitialState, rel_spec);


#if TEST
            pendingSpec.Push(currentSpec.TauReachable());
#else
            pendingSpec.Push(currentSpec);
#endif

            while (pendingImpl.Count > 0)
            {
                currentImpl = pendingImpl.Pop();
                currentSpec = pendingSpec.Pop();

                string ID = currentImpl.GetID() + Constants.SEPARATOR + currentSpec.GetID();
                if (Visited.ContainsKey(ID))
                {
                    continue;
                }

                Visited.Add(ID);

                //The following are for identifying a counterexample trace. 
                int depth = depthStack.Pop();

                while (depth > 0 && depthList[depthList.Count - 1] >= depth)
                {
                    int lastIndex = depthList.Count - 1;
                    depthList.RemoveAt(lastIndex);
                    toReturn.RemoveAt(lastIndex);
                }

                toReturn.Add(currentImpl);
                depthList.Add(depth);

                //If the specification has no corresponding state, then it implies that the trace is allowed by the 
                //implementation but not the specification -- which means trace-refinement is failed.
                if (currentSpec.States.Count == 0)
                {
                    VerificationOutput.NoOfStates = Visited.Count;
                    VerificationOutput.CounterExampleTrace = toReturn;
                    VerificationOutput.VerificationResult = VerificationResultType.INVALID;
                    return;
                }

                ConfigurationBase[] nextImpl = currentImpl.MakeOneMove();
                VerificationOutput.Transitions += nextImpl.Length;

                for (int k = 0; k < nextImpl.Length; k++)
                {
                    ConfigurationBase next = nextImpl[k];

                    if (next.Event != Constants.TAU)
                    {
                        NormalizedFAState nextSpec = currentSpec.Next(next.Event, rel_spec);

                        pendingImpl.Push(next);
                        pendingSpec.Push(nextSpec);
                        depthStack.Push(depth + 1);
                    }
                    else
                    {
                        pendingImpl.Push(next);
                        pendingSpec.Push(currentSpec);
                        depthStack.Push(depth + 1);
                    }
                }
            }

            VerificationOutput.NoOfStates = Visited.Count;
            VerificationOutput.VerificationResult = VerificationResultType.VALID;
            //return null;

        }




        //public Automata BuildGraphHiddenTauOld(Dictionary<string, HashSet<FAState>> rel_spec, HashSet<FAState> initialStates)
        //{
        //    Queue<FAState> workingQueue = new Queue<FAState>(initialStates);
            
            

        //    while(workingQueue.Count > 0)
        //    {
        //        FAState current = workingQueue.Dequeue();
        //        HashSet<FAState> pres = current.Pre[Constants.TAU];
                
        //        foreach (FAState state in pres)
        //        {
        //            if(rel_spec.ContainsKey(state.ID))
        //            {
        //                rel_spec[state.ID].Add(current);
        //            }
        //        }
                

        //    }

        //}

        /**
* Compute forward simulation relation of a Buchi automaton
* @param omega: a Buchi automaton
* @param FSim: the maximal bound of simulation relation
* 
* @return maximal simulation relation on states of the input automaton with FSim
*/
        //public static StatePairComparator StatePairComparator = new StatePairComparator();

        //public HashSet<Pair<FAState, FAState>> FSimRelNBW(FiniteAutomaton omega, HashSet<Pair<FAState, FAState>> FSim)
        //{
        //    HashSet<Pair<FAState, FAState>> nextFSim = new HashSet<Pair<FAState, FAState>>();
        //    bool changed = true;
        //    while (changed)
        //    {
        //        changed = false;
        //        foreach (Pair<FAState, FAState> pair in FSim)
        //        {
        //            if (NextStateSimulated(FSim, omega, pair.Left, pair.Right))
        //            {
        //                nextFSim.Add(new Pair<FAState, FAState>(pair.Left, pair.Right));
        //            }
        //            else
        //            {
        //                changed = true;
        //            }
        //        }

        //        FSim = nextFSim;
        //        nextFSim = new HashSet<Pair<FAState, FAState>>();
        //    }
        //    return FSim;
        //}

        public static Dictionary<string, HashSet<FAState>> FastFSimRelNBW(Automata spec, bool[,] fsim)
        {
            //implement the HHK algorithm

            int n_states = spec.States.Count;
            int n_symbols = spec.Alphabet.Count;
            FAState[] states = spec.States.Values.ToArray();

            List<string> symbols = new List<string>(spec.Alphabet);

            // fsim[u][v]=true iff v in fsim(u) iff v forward-simulates u

            int[, ,] pre = new int[n_symbols, n_states, n_states];
            int[, ,] post = new int[n_symbols, n_states, n_states];
            int[,] pre_len = new int[n_symbols, n_states];
            int[,] post_len = new int[n_symbols, n_states];

            //state[post[s][q][r]] is in post_s(q) for 0<=r<adj_len[s][q]
            //state[pre[s][q][r]] is in pre_s(q) for 0<=r<adj_len[s][q]
            for (int s = 0; s < n_symbols; s++)
            {
                string a = symbols[s];
                for (int p = 0; p < n_states; p++)
                    for (int q = 0; q < n_states; q++)
                    {
                        if (states[p].Post.ContainsKey(a))
                        {
                            HashSet<FAState> next = states[p].Post[a];
                            if (next != null && next.Contains(states[q]))
                            {
                                //if p --a--> q, then p is in pre_a(q), q is in post_a(p) 
                                pre[s, q, pre_len[s, q]++] = p;
                                post[s, p, post_len[s, p]++] = q;
                            }
                        }
                    }
            }
            int[] todo = new int[n_states * n_symbols];
            int todo_len = 0;

            int[, ,] remove = new int[n_symbols, n_states, n_states];
            int[,] remove_len = new int[n_symbols, n_states];
            for (int a = 0; a < n_symbols; a++)
            {
                for (int p = 0; p < n_states; p++)
                    if (pre_len[a, p] > 0) // p is in a_S
                    {
                        //Sharpen_S_a:
                        for (int q = 0; q < n_states; q++) // {all q} --> S_a 
                        {
                            bool skipQ = false;
                            if (post_len[a, q] > 0) /// q is in S_a 
                            {
                                for (int r = 0; r < post_len[a, q]; r++)
                                {
                                    if (fsim[p, post[a, q, r]]) // q is in pre_a(sim(p))
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
                        if (remove_len[a, p] > 0)
                            todo[todo_len++] = a * n_states + p;
                    }
            }

            int[] swap = new int[n_states];
            int swap_len = 0;
            bool using_swap = false;

            while (todo_len > 0)
            {
                todo_len--;
                int v = todo[todo_len] % n_states;
                int a = todo[todo_len] / n_states;
                int len = (using_swap ? swap_len : remove_len[a, v]);
                remove_len[a, v] = 0;

                for (int j = 0; j < pre_len[a, v]; j++)
                {
                    int u = pre[a, v, j];

                    for (int i = 0; i < len; i++)
                    {
                        int w = (using_swap ? swap[i] : remove[a, v, i]);
                        if (fsim[u, w])
                        {
                            fsim[u, w] = false;
                            for (int b = 0; b < n_symbols; b++)
                            {
                                if (pre_len[b, u] > 0)
                                {
                                    //Sharpen_pre_b_w:
                                    for (int k = 0; k < pre_len[b, w]; k++)
                                    {
                                        bool skipww = false;
                                        int ww = pre[b, w, k];
                                        for (int r = 0; r < post_len[b, ww]; r++)
                                        {
                                            if (fsim[u, post[b, ww, r]]) // ww is in pre_b(sim(u))
                                            //continue Sharpen_pre_b_w;	// skip ww
                                            {
                                                skipww = true;
                                                break;
                                            }
                                        }

                                        if (!skipww)
                                        {
                                            if (b == a && u == v && !using_swap)
                                                swap[swap_len++] = ww;
                                            else
                                            {
                                                if (remove_len[b, u] == 0)
                                                {
                                                    todo[todo_len++] = b * n_states + u;
                                                }
                                                remove[b, u, remove_len[b, u]++] = ww;
                                            }
                                        }



                                    }
                                }
                            }
                        } //End of if(fsim[u][w])
                    }
                }

                if (swap_len > 0)
                {
                    if (!using_swap)
                    {
                        todo[todo_len++] = a * n_states + v;
                        using_swap = true;
                    }
                    else
                    {
                        swap_len = 0;
                        using_swap = false;
                    }
                }

            }

            //HashSet<Pair<FAState, FAState>> FSim2 = new HashSet<Pair<FAState, FAState>>();
            //for (int p = 0; p < n_states; p++)
            //    for (int q = 0; q < n_states; q++)
            //        if (fsim[p, q]) // q is in sim(p), q simulates p
            //            FSim2.Add(new Pair<FAState, FAState>(states[p], states[q]));


            Dictionary<string, HashSet<FAState>> Fsim3 = new Dictionary<string, HashSet<FAState>>();

            for (int p = 0; p < n_states; p++)
            {
                for (int q = 0; q < n_states; q++)
                {
                    if (fsim[p, q] && q != p) // q is in sim(p), q simulates p
                    {
                        //FSim2.Add(new Pair<FAState, FAState>(states[p], states[q]));
                        HashSet<FAState> list = null;
                        if (Fsim3.TryGetValue(states[p].ID, out list))
                        {
                            list.Add(states[q]);
                        }
                        else
                        {
                            list = new HashSet<FAState>();
                            list.Add(states[q]);
                            Fsim3.Add(states[p].ID, list);
                        }
                    }
                }
            }

            return Fsim3;
        }


    }
}
