using System;

namespace ltl2ba
{
    class buchi
    {
        static BState bstack, bremoved;
        internal static BState bstates;
        static BScc scc_stack;
        internal static int b_accept, bstate_count = 0, btrans_count = 0, rank;

        internal static void free_bstate(BState s) /* frees a state and its transitions */
        {
            mem.free_btrans(s.trans.nxt, s.trans, 1);
            mem.tfree(s);
        }
        internal static BState remove_bstate(BState s, BState s1) /* removes a state */
        {
            BState prv = s.prv;
            s.prv.nxt = s.nxt;
            s.nxt.prv = s.prv;
            mem.free_btrans(s.trans.nxt, s.trans, 0);
            s.trans = null;
            s.nxt = bremoved.nxt;
            bremoved.nxt = s;
            s.prv = s1;
            for (s1 = bremoved.nxt; s1 != bremoved; s1 = s1.nxt)
                if (s1.prv == s)
                    s1.prv = s.prv;
            return prv;
        }
        internal static void copy_btrans(BTrans from, BTrans to)
        {
            to.to = from.to;
            set.copy_set(from.pos, to.pos, 1);
            set.copy_set(from.neg, to.neg, 1);
        }
        internal static int simplify_btrans() /* simplifies the transitions */
        {
              BState s;
              BTrans t, t1;
              int changed = 0;

             // if(tl_stats) getrusage(RUSAGE_SELF, &tr_debut);

              for (s = bstates.nxt; s != bstates; s = s.nxt)
                for (t = s.trans.nxt; t != s.trans;) {
                  t1 = s.trans.nxt;
                  copy_btrans(t, s.trans);
                  while((t == t1) || (t.to != t1.to) ||
                        set.included_set(t1.pos, t.pos, 1) == 0 ||
                        set.included_set(t1.neg, t.neg, 1) == 0)
                    t1 = t1.nxt;
                  if(t1 != s.trans) {
                    BTrans free = t.nxt;
                    t.to    = free.to;
                    set.copy_set(free.pos, t.pos, 1);
                    set.copy_set(free.neg, t.neg, 1);
                    t.nxt   = free.nxt;
                    if(free == s.trans) s.trans = t;
                    mem.free_btrans(free, null, 0);
                    changed++;
                  }
                  else
                    t = t.nxt;
                }
                  /*
              if(tl_stats) {
                getrusage(RUSAGE_SELF, &tr_fin);
                timeval_subtract (&t_diff, &tr_fin.ru_utime, &tr_debut.ru_utime);
                fprintf(tl_out, "\nSimplification of the Buchi automaton - transitions: %i.%06is",
		            t_diff.tv_sec, t_diff.tv_usec);
                fprintf(tl_out, "\n%i transitions removed\n", changed);

              }*/
              return changed;
        }
        internal static int same_btrans(BTrans s, BTrans t) /* returns 1 if the transitions are identical */
        {
            return ((s.to == t.to) &&
               set.same_sets(s.pos, t.pos, 1) != 0 &&
               set.same_sets(s.neg, t.neg, 1) != 0) ? 1 : 0;
        }
        internal static void remove_btrans(BState to)
        {             /* redirects transitions before removing a state from the automaton */
            BState s;
            BTrans t;
            int i;
            for (s = bstates.nxt; s != bstates; s = s.nxt)
                for (t = s.trans.nxt; t != s.trans; t = t.nxt)
                    if (t.to == to)
                    { /* transition to a state with no transitions */
                        BTrans free = t.nxt;
                        t.to = free.to;
                        set.copy_set(free.pos, t.pos, 1);
                        set.copy_set(free.neg, t.neg, 1);
                        t.nxt = free.nxt;
                        if (free == s.trans) s.trans = t;
                        mem.free_btrans(free, null, 0);
                    }
        }
        internal static void retarget_all_btrans()
        {             /* redirects transitions before removing a state from the automaton */
            BState s;
            BTrans t;
            for (s = bstates.nxt; s != bstates; s = s.nxt)
                for (t = s.trans.nxt; t != s.trans; t = t.nxt)
                    if (t.to.trans == null)
                    { /* t.to has been removed */
                        t.to = t.to.prv;
                        if (t.to == null)
                        { /* t.to has no transitions */
                            BTrans free = t.nxt;
                            t.to = free.to;
                            set.copy_set(free.pos, t.pos, 1);
                            set.copy_set(free.neg, t.neg, 1);
                            t.nxt = free.nxt;
                            if (free == s.trans) s.trans = t;
                            mem.free_btrans(free, null, 0);
                        }
                    }
            while (bremoved.nxt != bremoved)
            { /* clean the 'removed' list */
                s = bremoved.nxt;
                bremoved.nxt = bremoved.nxt.nxt;
                mem.tfree(s);
            }
        }
        internal static int all_btrans_match(BState a, BState b) /* decides if the states are equivalent */
        {	
              BTrans s, t;
              if (((a.final == b_accept) || (b.final == b_accept)) &&
                  (a.final + b.final != 2 * b_accept) && 
                  a.incoming >=0 && b.incoming >=0)
                return 0; /* the states have to be both final or both non final */

              for (s = a.trans.nxt; s != a.trans; s = s.nxt) { 
                                            /* all transitions from a appear in b */
                copy_btrans(s, b.trans);
                t = b.trans.nxt;
                while(same_btrans(s, t) == 0)
                  t = t.nxt;
                if(t == b.trans) return 0;
              }
              for (s = b.trans.nxt; s != b.trans; s = s.nxt) { 
                                            /* all transitions from b appear in a */
                copy_btrans(s, a.trans);
                t = a.trans.nxt;
                while(same_btrans(s, t) == 0)
                  t = t.nxt;
                if(t == a.trans) return 0;
              }
              return 1;
        }
        internal static int simplify_bstates() /* eliminates redundant states */
        {
              BState s, s1;
              int changed = 0;

              //if(tl_stats) getrusage(RUSAGE_SELF, &tr_debut);

              for (s = bstates.nxt; s != bstates; s = s.nxt) {
                if(s.trans == s.trans.nxt) { /* s has no transitions */
                  s = remove_bstate(s, null);
                  changed++;
                  continue;
                }
                bstates.trans = s.trans;
                bstates.final = s.final;
                s1 = s.nxt;
                while(all_btrans_match(s, s1) == 0)
                  s1 = s1.nxt;
                if(s1 != bstates) { /* s and s1 are equivalent */
                  if(s1.incoming == -1)
                    s1.final = s.final; /* get the good final condition */
                  s = remove_bstate(s, s1);
                  changed++;
                }
              }
              retarget_all_btrans();
            /*
              if(tl_stats) {
                getrusage(RUSAGE_SELF, &tr_fin);
                timeval_subtract (&t_diff, &tr_fin.ru_utime, &tr_debut.ru_utime);
                fprintf(tl_out, "\nSimplification of the Buchi automaton - states: %i.%06is",
		            t_diff.tv_sec, t_diff.tv_usec);
                fprintf(tl_out, "\n%i states removed\n", changed);
              }*/

              return changed;
        }
        internal static int bdfs(BState s)
        {
            BTrans t;
            BScc c;
            BScc scc = new BScc();
            scc.bstate = s;
            scc.rank = rank;
            scc.theta = rank++;
            scc.nxt = scc_stack;
            scc_stack = scc;

            s.incoming = 1;

            for (t = s.trans.nxt; t != s.trans; t = t.nxt)
            {
                if (t.to.incoming == 0)
                {
                    int result = bdfs(t.to);
                    scc.theta = Math.Min(scc.theta, result);
                }
                else
                {
                    for (c = scc_stack.nxt; c != null; c = c.nxt)
                        if (c.bstate == t.to)
                        {
                            scc.theta = Math.Min(scc.theta, c.rank);
                            break;
                        }
                }
            }
            if (scc.rank == scc.theta)
            {
                if (scc_stack == scc)
                { /* s is alone in a scc */
                    s.incoming = -1;
                    for (t = s.trans.nxt; t != s.trans; t = t.nxt)
                        if (t.to == s)
                            s.incoming = 1;
                }
                scc_stack = scc.nxt;
            }
            return scc.theta;
        }
        internal static void simplify_bscc()
        {
            BState s;
            rank = 1;
            scc_stack = null;

            if (bstates == bstates.nxt) return;

            for (s = bstates.nxt; s != bstates; s = s.nxt)
                s.incoming = 0; /* state color = white */

            bdfs(bstates.prv);

            for (s = bstates.nxt; s != bstates; s = s.nxt)
                if (s.incoming == 0)
                    remove_bstate(s, null);
        }
        internal static BState find_bstate(ref GState state, int final, BState s)
        {                       /* finds the corresponding state, or creates it */
            if ((s.gstate == state) && (s.final == final)) return s; /* same state */

            s = bstack.nxt; /* in the stack */
            bstack.gstate = state;
            bstack.final = final;
            while (!(s.gstate == state) || !(s.final == final))
                s = s.nxt;
            if (s != bstack) return s;

            s = bstates.nxt; /* in the solved states */
            bstates.gstate = state;
            bstates.final = final;
            while (!(s.gstate == state) || !(s.final == final))
                s = s.nxt;
            if (s != bstates) return s;

            s = bremoved.nxt; /* in the removed states */
            bremoved.gstate = state;
            bremoved.final = final;
            while (!(s.gstate == state) || !(s.final == final))
                s = s.nxt;
            if (s != bremoved) return s;

            s = new BState(); /* creates a new state */
            s.gstate = state;
            s.id = (state).id;
            s.incoming = 0;
            s.final = final;
            s.trans = mem.emalloc_btrans(); /* sentinel */
            s.trans.nxt = s.trans;
            s.nxt = bstack.nxt;
            bstack.nxt = s;
            return s;
        }
        internal static int next_final(int[] set, int fin) /* computes the 'final' value */
        {
            if ((fin != b_accept) && ltl2ba.set.in_set(set, generalized.final[fin + 1]) != 0)
                return next_final(set, fin + 1);
            return fin;
        }
        internal static void make_btrans(BState s) /* creates all the transitions from a state */
        {
            int state_trans = 0;
            GTrans t;
            BTrans t1;
            BState s1;
            if (s.gstate.trans != null)
                for (t = s.gstate.trans.nxt; t != s.gstate.trans; t = t.nxt)
                {
                    int fin = next_final(t.final, (s.final == b_accept) ? 0 : s.final);
                    BState to = find_bstate( ref t.to, fin, s);

                    for (t1 = s.trans.nxt; t1 != s.trans; )
                    {
                        if (main.tl_simp_fly != 0 &&
                           (to == t1.to) &&
                           set.included_set(t.pos, t1.pos, 1) != 0 &&
                           set.included_set(t.neg, t1.neg, 1) != 0)
                        { /* t1 is redondant */
                            BTrans free = t1.nxt;
                            t1.to.incoming--;
                            t1.to = free.to;
                            set.copy_set(free.pos, t1.pos, 1);
                            set.copy_set(free.neg, t1.neg, 1);
                            t1.nxt = free.nxt;
                            if (free == s.trans) s.trans = t1;
                            mem.free_btrans(free, null, 0);
                            state_trans--;
                        }
                        else if (main.tl_simp_fly != 0 &&
                            (t1.to == to) &&
                            set.included_set(t1.pos, t.pos, 1) != 0 &&
                            set.included_set(t1.neg, t.neg, 1) != 0) /* t is redondant */
                            break;
                        else
                            t1 = t1.nxt;
                    }
                    if (t1 == s.trans)
                    {
                        BTrans trans = mem.emalloc_btrans();
                        trans.to = to;
                        trans.to.incoming++;
                        set.copy_set(t.pos, trans.pos, 1);
                        set.copy_set(t.neg, trans.neg, 1);
                        trans.nxt = s.trans.nxt;
                        s.trans.nxt = trans;
                        state_trans++;
                    }
                }

            if (main.tl_simp_fly != 0)
            {
                if (s.trans == s.trans.nxt)
                { /* s has no transitions */
                    mem.free_btrans(s.trans.nxt, s.trans, 1);
                    s.trans = null;
                    s.prv = null;
                    s.nxt = bremoved.nxt;
                    bremoved.nxt = s;
                    for (s1 = bremoved.nxt; s1 != bremoved; s1 = s1.nxt)
                        if (s1.prv == s)
                            s1.prv = null;
                    return;
                }
                bstates.trans = s.trans;
                bstates.final = s.final;
                s1 = bstates.nxt;
                while (all_btrans_match(s, s1) == 0)
                    s1 = s1.nxt;
                if (s1 != bstates)
                { /* s and s1 are equivalent */
                    mem.free_btrans(s.trans.nxt, s.trans, 1);
                    s.trans = null;
                    s.prv = s1;
                    s.nxt = bremoved.nxt;
                    bremoved.nxt = s;
                    for (s1 = bremoved.nxt; s1 != bremoved; s1 = s1.nxt)
                        if (s1.prv == s)
                            s1.prv = s.prv;
                    return;
                }
            }
            s.nxt = bstates.nxt; /* adds the current state to 'bstates' */
            s.prv = bstates;
            s.nxt.prv = s;
            bstates.nxt = s;
            btrans_count += state_trans;
            bstate_count++;
        }
        internal static void mk_buchi()
        {/* generates a Buchi automaton from the generalized Buchi automaton */
            int i;
            BState s = new BState();
            GTrans t;
            BTrans t1;
            b_accept = generalized.final[0] - 1;

            // if(tl_stats) getrusage(RUSAGE_SELF, &tr_debut);

            bstack = new BState(); /* sentinel */
            bstack.nxt = bstack;
            bremoved = new BState(); /* sentinel */
            bremoved.nxt = bremoved;
            bstates = new BState(); /* sentinel */
            bstates.nxt = s;
            bstates.prv = s;

            s.nxt = bstates; /* creates (unique) inital state */
            s.prv = bstates;
            s.id = -1;
            s.incoming = 1;
            s.final = 0;
            s.gstate = null;
            s.trans = mem.emalloc_btrans(); /* sentinel */
            s.trans.nxt = s.trans;
            for (i = 0; i < generalized.init_size; i++)
                if (generalized.init[i] != null)
                    for (t = generalized.init[i].trans.nxt; t != generalized.init[i].trans; t = t.nxt)
                    {
                        int fin = next_final(t.final, 0);
                        BState to = find_bstate(ref t.to, fin, s);
                        for (t1 = s.trans.nxt; t1 != s.trans; )
                        {
                            if (main.tl_simp_fly != 0 &&
                               (to == t1.to) &&
                               set.included_set(t.pos, t1.pos, 1) != 0 &&
                               set.included_set(t.neg, t1.neg, 1) != 0)
                            { /* t1 is redondant */
                                BTrans free = t1.nxt;
                                t1.to.incoming--;
                                t1.to = free.to;
                                set.copy_set(free.pos, t1.pos, 1);
                                set.copy_set(free.neg, t1.neg, 1);
                                t1.nxt = free.nxt;
                                if (free == s.trans) s.trans = t1;
                                mem.free_btrans(free, null, 0);
                            }
                            else if (main.tl_simp_fly != 0 &&
                              (t1.to == to) &&
                              set.included_set(t1.pos, t.pos, 1) != 0 &&
                              set.included_set(t1.neg, t.neg, 1) != 0) /* t is redondant */
                                break;
                            else
                                t1 = t1.nxt;
                        }
                        if (t1 == s.trans)
                        {
                            BTrans trans = mem.emalloc_btrans();
                            trans.to = to;
                            trans.to.incoming++;
                            set.copy_set(t.pos, trans.pos, 1);
                            set.copy_set(t.neg, trans.neg, 1);
                            trans.nxt = s.trans.nxt;
                            s.trans.nxt = trans;
                        }
                    }

            while (bstack.nxt != bstack)
            { /* solves all states in the stack until it is empty */
                s = bstack.nxt;
                bstack.nxt = bstack.nxt.nxt;
                if (s.incoming == 0)
                {
                    free_bstate(s);
                    continue;
                }
                make_btrans(s);
            }

            retarget_all_btrans();
            /*
              if(tl_stats) {
                getrusage(RUSAGE_SELF, &tr_fin);
                timeval_subtract (&t_diff, &tr_fin.ru_utime, &tr_debut.ru_utime);
                fprintf(tl_out, "\nBuilding the Buchi automaton : %i.%06is",
                    t_diff.tv_sec, t_diff.tv_usec);
                fprintf(tl_out, "\n%i states, %i transitions\n", bstate_count, btrans_count);
              }*/

            //if(tl_verbose) {
            //  fprintf(tl_out, "\nBuchi automaton before simplification\n");
            //  print_buchi(bstates.nxt);
            //  if(bstates == bstates.nxt) 
            //    fprintf(tl_out, "empty automaton, refuses all words\n");  
            //}

            if (main.tl_simp_diff != 0)
            {
                simplify_btrans();
                if (main.tl_simp_scc != 0) simplify_bscc();
                while (simplify_bstates() != 0)
                { /* simplifies as much as possible */
                    simplify_btrans();
                    if (main.tl_simp_scc != 0) simplify_bscc();
                }

                //   if(tl_verbose) {
                //     fprintf(tl_out, "\nBuchi automaton after simplification\n");
                //     print_buchi(bstates.nxt);
                //     if(bstates == bstates.nxt) 
                //fprintf(tl_out, "empty automaton, refuses all words\n");
                //     fprintf(tl_out, "\n");
                //   }
            }

            //print_spin_buchi();
        }
    }
}
