using System;

namespace ltl2ba
{
    class generalized
    {
        static GState gstack, gremoved, gstates;
        internal static GState[] init;
        static GScc scc_stack;
        internal static int init_size = 0; 
        internal static int gstate_id = 1, gstate_count = 0, gtrans_count = 0;
        //static int *fin, *final, rank, scc_id, scc_size, *bad_scc;
        internal static int[] final;
        static int[] fin, bad_scc;
        static int rank, scc_id;
        internal static int scc_size;

        internal static void free_gstate(GState s) /* frees a state and its transitions */
        {
            mem.free_gtrans(s.trans.nxt, s.trans, 1);
            mem.tfree(s.nodes_set);
            mem.tfree(s);
        }
        internal static GState remove_gstate(GState s, GState s1) /* removes a state */
        {
              GState prv = s.prv;
              s.prv.nxt = s.nxt;
              s.nxt.prv = s.prv;
              mem.free_gtrans(s.trans.nxt, s.trans, 0);
              s.trans = null;
              mem.tfree(s.nodes_set);
              s.nodes_set = null;
              s.nxt = gremoved.nxt;
              gremoved.nxt = s;
              s.prv = s1;
              for(s1 = gremoved.nxt; s1 != gremoved; s1 = s1.nxt)
                if(s1.prv == s)
                  s1.prv = s.prv;
              return prv;
        } 
        internal static void copy_gtrans(GTrans from, GTrans to) /* copies a transition */
        {
              to.to = from.to;
              set.copy_set(from.pos,   to.pos,   1);
              set.copy_set(from.neg,   to.neg,   1);
              set.copy_set(from.final, to.final, 0);
        }
        internal static int same_gtrans(GState a, GTrans s, GState b, GTrans t, int use_scc) 
        { /* returns 1 if the transitions are identical */
              if((s.to != t.to) ||
                  set.same_sets(s.pos, t.pos, 1) == 0 ||
                  set.same_sets(s.neg, t.neg, 1) == 0)
                return 0; /* transitions differ */
              if(set.same_sets(s.final, t.final, 0) != 0)
                return 1; /* same transitions exactly */
              /* next we check whether acceptance conditions may be ignored */
              if( use_scc != 0 &&
                  ( set.in_set(bad_scc, a.incoming) != 0 ||
                    set.in_set(bad_scc, b.incoming) != 0 ||
                    (a.incoming != s.to.incoming) ||
                    (b.incoming != t.to.incoming) ) )
                return 1;
              return 0;
              /* below is the old test to check whether acceptance conditions may be ignored */
              //if(use_scc == 0)
                //return 0; /* transitions differ */
              //if( (a.incoming == b.incoming) && (a.incoming == s.to.incoming) )
                //return 0; /* same scc: acceptance conditions must be taken into account */
              /* if scc(a)=scc(b)>scc(s.to) then acceptance conditions need not be taken into account */
              /* if scc(a)>scc(b) and scc(a) is non-trivial then all_gtrans_match(a,b,use_scc) will fail */
              /* if scc(a) is trivial then acceptance conditions of transitions from a need not be taken into account */
              return 1; /* same transitions up to acceptance conditions */
        }
        internal static int simplify_gtrans() /* simplifies the transitions */
        {
              int changed = 0;
              GState s;
              GTrans t, t1;

              //if(tl_stats) getrusage(RUSAGE_SELF, &tr_debut);

              for(s = gstates.nxt; s != gstates; s = s.nxt) {
                t = s.trans.nxt;
                while(t != s.trans) { /* tries to remove t */
                  copy_gtrans(t, s.trans);
                  t1 = s.trans.nxt;
                  while ( !((t != t1) 
                      && (t1.to == t.to) 
                      && set.included_set(t1.pos, t.pos, 1) != 0
                      && set.included_set(t1.neg, t.neg, 1) != 0
                      && (set.included_set(t.final, t1.final, 0) != 0 /* acceptance conditions of t are also in t1 or may be ignored */
                          || (main.tl_simp_scc != 0 && ((s.incoming != t.to.incoming) || set.in_set(bad_scc, s.incoming) != 0)))) )
                    t1 = t1.nxt;
                  if(t1 != s.trans) { /* remove transition t */
                    GTrans free = t.nxt;
                    t.to = free.to;
                    set.copy_set(free.pos, t.pos, 1);
                    set.copy_set(free.neg, t.neg, 1);
                    set.copy_set(free.final, t.final, 0);
                    t.nxt = free.nxt;
                    if(free == s.trans) s.trans = t;
                    mem.free_gtrans(free, null, 0);
                    changed++;
                  }
                  else
                    t = t.nxt;
                }
              }
              /*
              if(tl_stats) {
                getrusage(RUSAGE_SELF, &tr_fin);
                timeval_subtract (&t_diff, &tr_fin.ru_utime, &tr_debut.ru_utime);
                fprintf(tl_out, "\nSimplification of the generalized Buchi automaton - transitions: %i.%06is",
		            t_diff.tv_sec, t_diff.tv_usec);
                fprintf(tl_out, "\n%i transitions removed\n", changed);
              }*/

              return changed;
        }
        internal static void retarget_all_gtrans()
        {             /* redirects transitions before removing a state from the automaton */
              GState s;
              GTrans t;
              int i;
              for (i = 0; i < init_size; i++)
                if (init[i] != null && init[i].trans == null) /* init[i] has been removed */
                  init[i] = init[i].prv;
              for (s = gstates.nxt; s != gstates; s = s.nxt)
                for (t = s.trans.nxt; t != s.trans; )
                  if (t.to.trans == null) { /* t.to has been removed */
	            t.to = t.to.prv;
	            if(t.to == null) { /* t.to has no transitions */
	              GTrans free = t.nxt;
	              t.to = free.to;
	              set.copy_set(free.pos, t.pos, 1);
                  set.copy_set(free.neg, t.neg, 1);
                  set.copy_set(free.final, t.final, 0);
	              t.nxt   = free.nxt;
	              if(free == s.trans) s.trans = t;
	              mem.free_gtrans(free, null, 0);
	            }
	            else
	              t = t.nxt;
                  }
                  else
	            t = t.nxt;
              while(gremoved.nxt != gremoved) { /* clean the 'removed' list */
                s = gremoved.nxt;
                gremoved.nxt = gremoved.nxt.nxt;
                if(s.nodes_set != null) mem.tfree(s.nodes_set);
                mem.tfree(s);
              }
        }
        internal static void mk_generalized() 
        { /* generates a generalized Buchi automaton from the alternating automaton */
              ATrans t;
              GState s;
              int i;

              //if(tl_stats) getrusage(RUSAGE_SELF, &tr_debut);

              fin = set.new_set(0);
              bad_scc = null; /* will be initialized in simplify_gscc */
              final = set.list_set(alternating.final_set, 0);

              gstack        = new GState(); /* sentinel */
              gstack.nxt   = gstack;
              gremoved      = new GState(); /* sentinel */
              gremoved.nxt = gremoved;
              gstates       = new GState(); /* sentinel */
              gstates.nxt  = gstates;
              gstates.prv  = gstates;

              for(t = alternating.transition[0]; t != null; t = t.nxt) { /* puts initial states in the stack */
                s = new GState();
                s.id = (set.empty_set(t.to, 0)) != 0 ? 0 : gstate_id++;
                s.incoming = 1;
                s.nodes_set = set.dup_set(t.to, 0);
                s.trans = mem.emalloc_gtrans(); /* sentinel */
                s.trans.nxt = s.trans;
                s.nxt = gstack.nxt;
                gstack.nxt = s;
                init_size++;
              }

              if(init_size != 0) init = new GState[init_size];
              init_size = 0;
              for(s = gstack.nxt; s != gstack; s = s.nxt)
                init[init_size++] = s;

              while(gstack.nxt != gstack) { /* solves all states in the stack until it is empty */
                s = gstack.nxt;
                gstack.nxt = gstack.nxt.nxt;
                if(s.incoming == 0) {
                  free_gstate(s);
                  continue;
                }
                make_gtrans(s);
              }

              retarget_all_gtrans();
            /*
              if(tl_stats) {
                getrusage(RUSAGE_SELF, &tr_fin);
                timeval_subtract (&t_diff, &tr_fin.ru_utime, &tr_debut.ru_utime);
                fprintf(tl_out, "\nBuilding the generalized Buchi automaton : %i.%06is",
		            t_diff.tv_sec, t_diff.tv_usec);
                fprintf(tl_out, "\n%i states, %i transitions\n", gstate_count, gtrans_count);
              }*/

              mem.tfree(gstack);
              /*for(i = 0; i < node_id; i++) /* frees the data from the alternating automaton */
              /*free_atrans(transition[i], 1);*/
              //mem.free_all_atrans();
              mem.tfree(alternating.transition);

              //if(tl_verbose) {
              //  fprintf(tl_out, "\nGeneralized Buchi automaton before simplification\n");
              //  print_generalized();
              //}

              if(main.tl_simp_diff != 0) {
                if (main.tl_simp_scc != 0) simplify_gscc();
                simplify_gtrans();
                if (main.tl_simp_scc != 0) simplify_gscc();
                while(simplify_gstates() != 0) { /* simplifies as much as possible */
                  if (main.tl_simp_scc != 0) simplify_gscc();
                  simplify_gtrans();
                  if (main.tl_simp_scc != 0) simplify_gscc();
                }
            //     
            //     if(tl_verbose) {
            //       fprintf(tl_out, "\nGeneralized Buchi automaton after simplification\n");
            //       print_generalized();
            //     }
              }
        }
        internal static int all_gtrans_match(GState a, GState b, int use_scc) 
        { /* decides if the states are equivalent */
              GTrans s, t;
              for (s = a.trans.nxt; s != a.trans; s = s.nxt) { 
                                            /* all transitions from a appear in b */
                copy_gtrans(s, b.trans);
                t = b.trans.nxt;
                while(same_gtrans(a, s, b, t, use_scc) == 0) t = t.nxt;
                if(t == b.trans) return 0;
              }
              for (t = b.trans.nxt; t != b.trans; t = t.nxt) { 
                                            /* all transitions from b appear in a */
                copy_gtrans(t, a.trans);
                s = a.trans.nxt;
                while(same_gtrans(a, s, b, t, use_scc) == 0) s = s.nxt;
                if(s == a.trans) return 0;
              }
              return 1;
        }
        internal static void make_gtrans(GState s) 
        { /* creates all the transitions from a state */
              int i, state_trans = 0, trans_exist = 1;
              int[] list;
              GState s1;
              GTrans t;
              ATrans t1, free;
              AProd prod = new AProd(); /* initialization */
              prod.nxt = prod;
              prod.prv = prod;
              prod.prod = mem.emalloc_atrans();
              set.clear_set(prod.prod.to,  0);
              set.clear_set(prod.prod.pos, 1);
              set.clear_set(prod.prod.neg, 1);
              prod.trans = prod.prod;
              prod.trans.nxt = prod.prod;
              list = set.list_set(s.nodes_set, 0);

              for(i = 1; i < list[0]; i++) {
                AProd p = new AProd();
                p.astate = list[i];
                p.trans = alternating.transition[list[i]];
                if(p.trans == null) trans_exist = 0;
                p.prod = alternating.merge_trans(prod.nxt.prod, p.trans);
                p.nxt = prod.nxt;
                p.prv = prod;
                p.nxt.prv = p;
                p.prv.nxt = p;
              }

              while(trans_exist != 0) { /* calculates all the transitions */
                AProd p = prod.nxt;
                t1 = p.prod;
                if(t1 != null) { /* solves the current transition */
                  GTrans trans, t2;
                  set.clear_set(fin, 0);
                  for(i = 1; i < final[0]; i++)
	            if(is_final(s.nodes_set, t1, final[i]) != 0)
	              set.add_set(fin, final[i]);
                  for(t2 = s.trans.nxt; t2 != s.trans;) {
	            if(main.tl_simp_fly != 0 &&
	               set.included_set(t1.to, t2.to.nodes_set, 0) != 0 &&
	               set.included_set(t1.pos, t2.pos, 1) != 0 &&
	               set.included_set(t1.neg, t2.neg, 1) != 0 &&
	               set.same_sets(fin, t2.final, 0) != 0) { /* t2 is redondant */
	              GTrans free1 = t2.nxt;
	              t2.to.incoming--;
	              t2.to = free1.to;
	              set.copy_set(free1.pos, t2.pos, 1);
	              set.copy_set(free1.neg, t2.neg, 1);
	              set.copy_set(free1.final, t2.final, 0);
	              t2.nxt   = free1.nxt;
	              if(free1 == s.trans) s.trans = t2;
	              mem.free_gtrans(free1, null, 0);
	              state_trans--;
	            }
	            else if(main.tl_simp_fly != 0 &&
		            set.included_set(t2.to.nodes_set, t1.to, 0) != 0 &&
		            set.included_set(t2.pos, t1.pos, 1) != 0 &&
		            set.included_set(t2.neg, t1.neg, 1) != 0 &&
		            set.same_sets(t2.final, fin, 0) != 0) {/* t1 is redondant */
	              break;
	            }
	            else {
	              t2 = t2.nxt;
	            }
                  }
                  if(t2 == s.trans) { /* adds the transition */
	            trans = mem.emalloc_gtrans();
	            trans.to = find_gstate(t1.to, s);
	            trans.to.incoming++;
	            set.copy_set(t1.pos, trans.pos, 1);
                set.copy_set(t1.neg, trans.neg, 1);
                set.copy_set(fin, trans.final, 0);
	            trans.nxt = s.trans.nxt;
	            s.trans.nxt = trans;
	            state_trans++;
                  }
                }
                if(p.trans == null)
                  break;
                while(p.trans.nxt == null) /* calculates the next transition */
                  p = p.nxt;
                if(p == prod)
                  break;
                p.trans = p.trans.nxt;
                alternating.do_merge_trans(ref p.prod, p.nxt.prod, p.trans);
                p = p.prv;
                while(p != prod) {
                  p.trans = alternating.transition[p.astate];
                  alternating.do_merge_trans(ref (p.prod), p.nxt.prod, p.trans);
                  p = p.prv;
                }
              }
              
              mem.tfree(list); /* free memory */
              while(prod.nxt != prod) {
                AProd p = prod.nxt;
                prod.nxt = p.nxt;
                mem.free_atrans(p.prod, 0);
                mem.tfree(p);
              }
              mem.free_atrans(prod.prod, 0);
              mem.tfree(prod);

              if(main.tl_simp_fly != 0) {
                if(s.trans == s.trans.nxt) { /* s has no transitions */
                  mem.free_gtrans(s.trans.nxt, s.trans, 1);
                  s.trans = null;
                  s.prv = null;
                  s.nxt = gremoved.nxt;
                  gremoved.nxt = s;
                  for(s1 = gremoved.nxt; s1 != gremoved; s1 = s1.nxt)
	            if(s1.prv == s)
	            s1.prv = null;
                  return;
                }
                
                gstates.trans = s.trans;
                s1 = gstates.nxt;
                while(all_gtrans_match(s, s1, 0) == 0)
                  s1 = s1.nxt;
                if(s1 != gstates) { /* s and s1 are equivalent */
                  mem.free_gtrans(s.trans.nxt, s.trans, 1);
                  s.trans = null;
                  s.prv = s1;
                  s.nxt = gremoved.nxt;
                  gremoved.nxt = s;
                  for(s1 = gremoved.nxt; s1 != gremoved; s1 = s1.nxt)
	            if(s1.prv == s)
	              s1.prv = s.prv;
                  return;
                }
              }

              s.nxt = gstates.nxt; /* adds the current state to 'gstates' */
              s.prv = gstates;
              s.nxt.prv = s;
              gstates.nxt = s;
              gtrans_count += state_trans;
              gstate_count++;
        }
        internal static GState find_gstate(int[] set, GState s) 
        { /* finds the corresponding state, or creates it */

              if(ltl2ba.set.same_sets(set, s.nodes_set, 0) != 0) return s; /* same state */

              s = gstack.nxt; /* in the stack */
              gstack.nodes_set = set;
              while (ltl2ba.set.same_sets(set, s.nodes_set, 0) == 0)
                s = s.nxt;
              if(s != gstack) return s;

              s = gstates.nxt; /* in the solved states */
              gstates.nodes_set = set;
              while (ltl2ba.set.same_sets(set, s.nodes_set, 0) == 0)
                s = s.nxt;
              if(s != gstates) return s;

              s = gremoved.nxt; /* in the removed states */
              gremoved.nodes_set = set;
              while (ltl2ba.set.same_sets(set, s.nodes_set, 0) == 0)
                s = s.nxt;
              if(s != gremoved) return s;

              s = new GState(); /* creates a new state */
              s.id = (ltl2ba.set.empty_set(set, 0)) != 0 ? 0 : gstate_id++;
            //   printf("%d \n", gstate_id);
            //   fflush(stdout);
              s.incoming = 0;
              s.nodes_set = ltl2ba.set.dup_set(set, 0);
              s.trans = mem.emalloc_gtrans(); /* sentinel */
              s.trans.nxt = s.trans;
              s.nxt = gstack.nxt;
              gstack.nxt = s;
              return s;
        }
        internal static int is_final(int[] from, ATrans at, int i) /*is the transition final for i ?*/
        {
              ATrans t;
              int in_to;
              if((main.tl_fjtofj != 0 && set.in_set(at.to, i) == 0) ||
                (main.tl_fjtofj == 0 && set.in_set(from,  i) == 0)) return 1;
              in_to = set.in_set(at.to, i);
              set.rem_set(at.to, i);
              for(t = alternating.transition[i]; t != null; t = t.nxt)
                if(set.included_set(t.to, at.to, 0) != 0 &&
                   set.included_set(t.pos, at.pos, 1) != 0 &&
                   set.included_set(t.neg, at.neg, 1) != 0) {
                  if(in_to != 0) set.add_set(at.to, i);
                  return 1;
                }
              if(in_to != 0) set.add_set(at.to, i);
              return 0;
        }
        internal static void simplify_gscc() 
        {
              GState s;
              GTrans t;
              int i;
              int[][] scc_final;
              rank = 1;
              scc_stack = null;
              scc_id = 1;

              if(gstates == gstates.nxt) return;

              for(s = gstates.nxt; s != gstates; s = s.nxt)
                s.incoming = 0; /* state color = white */

              for(i = 0; i < init_size; i++)
                if(init[i] != null && init[i].incoming == 0)
                  gdfs(init[i]);

              scc_final = new int[scc_id][];
              for(i = 0; i < scc_id; i++)
                scc_final[i] = set.make_set(-1,0);

              for(s = gstates.nxt; s != gstates; s = s.nxt)
                if(s.incoming == 0)
                  s = remove_gstate(s, null);
                else
                  for (t = s.trans.nxt; t != s.trans; t = t.nxt)
                    if(t.to.incoming == s.incoming)
                      set.merge_sets(scc_final[s.incoming], t.final, 0);

              scc_size = (scc_id + 1) / (8 * sizeof(int)) + 1;
              bad_scc=set.make_set(-1,2);

              for(i = 0; i < scc_id; i++)
                if(set.included_set(alternating.final_set, scc_final[i], 0) == 0)
                   set.add_set(bad_scc, i);

              for(i = 0; i < scc_id; i++)
                mem.tfree(scc_final[i]);
              mem.tfree(scc_final);
        }
        internal static int simplify_gstates() /* eliminates redundant states */
        {
              int changed = 0;
              GState a, b;

             // if(tl_stats) getrusage(RUSAGE_SELF, &tr_debut);

              for(a = gstates.nxt; a != gstates; a = a.nxt) {
                if(a.trans == a.trans.nxt) { /* a has no transitions */
                  a = remove_gstate(a, null);
                  changed++;
                  continue;
                }
                gstates.trans = a.trans;
                b = a.nxt;
                while(all_gtrans_match(a, b, main.tl_simp_scc) == 0) b = b.nxt;
                if(b != gstates) { /* a and b are equivalent */
                  /* if scc(a)>scc(b) and scc(a) is non-trivial then all_gtrans_match(a,b,use_scc) must fail */
                  if(a.incoming > b.incoming) /* scc(a) is trivial */
                    a = remove_gstate(a, b);
                  else /* either scc(a)=scc(b) or scc(b) is trivial */ 
                    remove_gstate(b, a);
                  changed++;
                }
              }
              retarget_all_gtrans();
            /*
              if(tl_stats) {
                getrusage(RUSAGE_SELF, &tr_fin);
                timeval_subtract (&t_diff, &tr_fin.ru_utime, &tr_debut.ru_utime);
                fprintf(tl_out, "\nSimplification of the generalized Buchi automaton - states: %i.%06is",
		            t_diff.tv_sec, t_diff.tv_usec);
                fprintf(tl_out, "\n%i states removed\n", changed);
              }
            */
              return changed;
        }
        internal static int gdfs(GState s) 
        {
              GTrans t;
              GScc c;
              GScc scc = new GScc();
              scc.gstate = s;
              scc.rank = rank;
              scc.theta = rank++;
              scc.nxt = scc_stack;
              scc_stack = scc;

              s.incoming = 1;

              for (t = s.trans.nxt; t != s.trans; t = t.nxt) {
                if (t.to.incoming == 0) {
                  int result = gdfs(t.to);
                  scc.theta = Math.Min(scc.theta, result);
                }
                else {
                  for(c = scc_stack.nxt; c != null; c = c.nxt)
	            if(c.gstate == t.to) {
	              scc.theta = Math.Min(scc.theta, c.rank);
	              break;
	            }
                }
              }
              if(scc.rank == scc.theta) {
                while(scc_stack != scc) {
                  scc_stack.gstate.incoming = scc_id;
                  scc_stack = scc_stack.nxt;
                }
                scc.gstate.incoming = scc_id++;
                scc_stack = scc.nxt;
              }
              return scc.theta;
        }
    }
}
