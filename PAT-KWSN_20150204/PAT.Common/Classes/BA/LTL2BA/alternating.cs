namespace ltl2ba
{
    class alternating
    {
        static Node[] label;
        internal static string[] sym_table;
        internal static ATrans[] transition;
        //struct rusage tr_debut, tr_fin;
        //struct timeval t_diff;
        internal static int node_id = 1, sym_id = 0, node_size;
        internal static int sym_size;
        internal static int[] final_set;
        static int astate_count = 0, atrans_count = 0;

        internal static int calculate_node_size(Node p) /* returns the number of temporal nodes */
        {
              switch(p.ntyp) {
              case (short)Operator.AND:
              case (short)Operator.OR:
              case (short)Operator.U_OPER:
              case (short)Operator.V_OPER:
                return(calculate_node_size(p.lft) + calculate_node_size(p.rgt) + 1);
              case (short)Operator.NEXT:
                return(calculate_node_size(p.lft) + 1);
              default:
                return 1;
                break;
              }
        }

        internal static int calculate_sym_size(Node p) /* returns the number of predicates */
        {
              switch(p.ntyp) {
              case (short)Operator.AND:
              case (short)Operator.OR:
              case (short)Operator.U_OPER:
              case (short)Operator.V_OPER:
                return(calculate_sym_size(p.lft) + calculate_sym_size(p.rgt));
              case (short)Operator.NEXT:
                return(calculate_sym_size(p.lft));
              case (short)Operator.NOT:
              case (short)Operator.PREDICATE:
                return 1;
              default:
                return 0;
              }
        }
        internal static ATrans dup_trans(ATrans trans)  /* returns the copy of a transition */
        {
            ATrans result;
            if (trans == null) return trans;
            result = mem.emalloc_atrans();
            set.copy_set(trans.to, result.to, 0);
            set.copy_set(trans.pos, result.pos, 1);
            set.copy_set(trans.neg, result.neg, 1);
            return result;
        }
        internal static void do_merge_trans(ref ATrans result, ATrans trans1, ATrans trans2)
        { /* merges two transitions */
            if (trans1 == null || trans2 == null)
            {
                mem.free_atrans(result, 0);
                result = null;
                return;
            }
            if (result == null)
                result = mem.emalloc_atrans();
            set.do_merge_sets((result).to, trans1.to, trans2.to, 0);
            set.do_merge_sets((result).pos, trans1.pos, trans2.pos, 1);
            set.do_merge_sets((result).neg, trans1.neg, trans2.neg, 1);
            if (set.empty_intersect_sets((result).pos, (result).neg, 1) == 0)
            {
                mem.free_atrans(result, 0);
                result = null;
            }
        }
        internal static ATrans merge_trans(ATrans trans1, ATrans trans2) /* merges two transitions */
        {
            ATrans result = mem.emalloc_atrans();
            
            do_merge_trans(ref result, trans1, trans2);
            return result;
        }
        internal static int already_done(Node p) /* finds the id of the node, if already explored */
        {
            int i;
            for (i = 1; i < node_id; i++)
                if (cache.isequal(p, label[i]) != 0)
                    return i;
            return -1;
        }
        internal static int get_sym_id(string s) /* finds the id of a predicate, or attributes one */
        {
            int i;
            for (i = 0; i < sym_id; i++)
                if (s.CompareTo(sym_table[i]) == 0)
                    return i;
            sym_table[sym_id] = s;
            return sym_id++;
        }
        internal static ATrans boolean(Node p) /* computes the transitions to boolean nodes . next & init */
        {
              ATrans t1, t2, lft, rgt, result = null;
              int id;
              switch(p.ntyp) {
              case (short)Operator.TRUE:
                result = mem.emalloc_atrans();
                set.clear_set(result.to,  0);
                set.clear_set(result.pos, 1);
                set.clear_set(result.neg, 1);
                break;
              case (short)Operator.FALSE:
                break;
              case (short)Operator.AND:
                lft = boolean(p.lft);
                rgt = boolean(p.rgt);
                for(t1 = lft; t1 != null; t1 = t1.nxt) {
                  for(t2 = rgt; t2 != null; t2 = t2.nxt) {
	            ATrans tmp = merge_trans(t1, t2);
	            if(tmp != null) {
	              tmp.nxt = result;
	              result = tmp;
	            }
                  }
                }
                mem.free_atrans(lft, 1);
                mem.free_atrans(rgt, 1);
                break;
              case (short)Operator.OR:
                lft = boolean(p.lft);
                for(t1 = lft; t1 != null; t1 = t1.nxt) {
                  ATrans tmp = dup_trans(t1);
                  tmp.nxt = result;
                  result = tmp;
                }
                mem.free_atrans(lft, 1);
                rgt = boolean(p.rgt);
                for(t1 = rgt; t1 != null; t1 = t1.nxt) {
                  ATrans tmp = dup_trans(t1);
                  tmp.nxt = result;
                  result = tmp;
                }
                mem.free_atrans(rgt, 1);
                break;
              default:
                build_alternating(p);
                result = mem.emalloc_atrans();
                set.clear_set(result.to, 0);
                set.clear_set(result.pos, 1);
                set.clear_set(result.neg, 1);
                set.add_set(result.to, already_done(p));
                break;
              }
              return result;
        }
        internal static ATrans build_alternating(Node p) /* builds an alternating automaton for p */
        {
              ATrans t1, t2, t = null;
              int node = already_done(p);
              if(node >= 0) return transition[node];

              switch (p.ntyp) {

              case (short)Operator.TRUE:
                t = mem.emalloc_atrans();
                set.clear_set(t.to, 0);
                set.clear_set(t.pos, 1);
                set.clear_set(t.neg, 1);
                break;
              case (short)Operator.FALSE:
                break;

              case (short)Operator.PREDICATE:
                t = mem.emalloc_atrans();
                set.clear_set(t.to, 0);
                set.clear_set(t.pos, 1);
                set.clear_set(t.neg, 1);
                set.add_set(t.pos, get_sym_id(p.sym.name));
                break;

              case (short)Operator.NOT:
                t = mem.emalloc_atrans();
                set.clear_set(t.to, 0);
                set.clear_set(t.pos, 1);
                set.clear_set(t.neg, 1);
                set.add_set(t.neg, get_sym_id(p.lft.sym.name));
                break;

              case (short)Operator.NEXT:                                            
                t = boolean(p.lft);
                break;

              case (short)Operator.U_OPER:    /* p U q <. q || (p && X (p U q)) */
                for(t2 = build_alternating(p.rgt); t2 != null; t2 = t2.nxt) {
                  ATrans tmp = dup_trans(t2);  /* q */
                  tmp.nxt = t;
                  t = tmp;
                }
                for(t1 = build_alternating(p.lft); t1 != null; t1 = t1.nxt) {
                  ATrans tmp = dup_trans(t1);  /* p */
                  set.add_set(tmp.to, node_id);  /* X (p U q) */
                  tmp.nxt = t;
                  t = tmp;
                }
                set.add_set(final_set, node_id);
                break;

              case (short)Operator.V_OPER:    /* p V q <. (p && q) || (p && X (p V q)) */
                for(t1 = build_alternating(p.rgt); t1 != null; t1 = t1.nxt) {
                  ATrans tmp;

                  for(t2 = build_alternating(p.lft); t2 != null; t2 = t2.nxt) {
	            tmp = merge_trans(t1, t2);  /* p && q */
	            if(tmp != null) {
	              tmp.nxt = t;
	              t = tmp;
	            }
                  }

                  tmp = dup_trans(t1);  /* p */
                  set.add_set(tmp.to, node_id);  /* X (p V q) */
                  tmp.nxt = t;
                  t = tmp;
                }
                break;

              case (short)Operator.AND:
                t = null;
                for(t1 = build_alternating(p.lft); t1 != null; t1 = t1.nxt) {
                  for(t2 = build_alternating(p.rgt); t2 != null; t2 = t2.nxt) {
	            ATrans tmp = merge_trans(t1, t2);
	            if(tmp != null) {
	              tmp.nxt = t;
	              t = tmp;
	            }
                  }
                }
                break;

              case (short)Operator.OR:
                t = null;
                for(t1 = build_alternating(p.lft); t1 != null; t1 = t1.nxt) {
                  ATrans tmp = dup_trans(t1);
                  tmp.nxt = t;
                  t = tmp;
                }
                for(t1 = build_alternating(p.rgt); t1 != null; t1 = t1.nxt) {
                  ATrans tmp = dup_trans(t1);
                  tmp.nxt = t;
                  t = tmp;
                }
                break;

              default:
                break;
              }

              transition[node_id] = t;
              label[node_id++] = p;
              return(t);
        }
        internal static void simplify_atrans(ref ATrans trans) /* simplifies the transitions */
        {
              ATrans t, father = null;
              for(t = trans; t != null;) {
                ATrans t1;
                for(t1 = trans; t1 != null; t1 = t1.nxt) {
                  if((t1 != t) && 
	             set.included_set(t1.to,  t.to,  0) != 0 &&
                 set.included_set(t1.pos, t.pos, 1) != 0 &&
                 set.included_set(t1.neg, t.neg, 1) != 0)
	            break;
                }
                if(t1 != null) {
                  if (father != null)
	            father.nxt = t.nxt;
                  else
	            trans = t.nxt;
                  mem.free_atrans(t, 0);
                  if (father != null)
	            t = father.nxt;
                  else
	            t = trans;
                  continue;
                }
                atrans_count++;
                father = t;
                t = t.nxt;
              }
        }
        internal static void simplify_astates() /* simplifies the alternating automaton */
        {
              ATrans t;
              int i;
              int[] acc = set.make_set(-1, 0); /* no state is accessible initially */

              for(t = transition[0]; t != null; t = t.nxt, i = 0)
                set.merge_sets(acc, t.to, 0); /* all initial states are accessible */

              for(i = node_id - 1; i > 0; i--) {
                  if (set.in_set(acc, i) == 0)
                  { /* frees unaccessible states */
                  label[i] = util.ZN;
                  mem.free_atrans(transition[i], 1);
                  transition[i] = null;
                  continue;
                }
                astate_count++;
                simplify_atrans(ref transition[i]);
                for(t = transition[i]; t != null; t = t.nxt)
                    set.merge_sets(acc, t.to, 0);
              }

              mem.tfree(acc);
        }
        internal static void mk_alternating(Node p) /* generates an alternating automaton for p */
        {
            //if(tl_stats) getrusage(RUSAGE_SELF, &tr_debut);

            node_size = calculate_node_size(p) + 1; /* number of states in the automaton */
            //label = (Node**)tl_emalloc(node_size * sizeof(Node*));
            label = new Node[node_size];
            //transition = (ATrans**)tl_emalloc(node_size * sizeof(ATrans*));
            transition = new ATrans[node_size];
            node_size = node_size / (8 * sizeof(int)) + 1;

            sym_size = calculate_sym_size(p); /* number of predicates */
            if (sym_size != 0) sym_table = new string[sym_size]; //(char**)tl_emalloc(sym_size * sizeof(char*));
            sym_size = sym_size / (8 * sizeof(int)) + 1;

            final_set = set.make_set(-1, 0);
            transition[0] = boolean(p); /* generates the alternating automaton */

            //   if(tl_verbose) {
            //     //fprintf(tl_out, "\nAlternating automaton before simplification\n");
            //     print_alternating();
            //   }

            if (main.tl_simp_diff != 0)
            {
                simplify_astates(); /* keeps only accessible states */
                //     if(tl_verbose) {
                //       //fprintf(tl_out, "\nAlternating automaton after simplification\n");
                //       print_alternating();
                //     }
            }
            /*
            if(tl_stats) {
              getrusage(RUSAGE_SELF, &tr_fin);
              timeval_subtract (&t_diff, &tr_fin.ru_utime, &tr_debut.ru_utime);
              fprintf(tl_out, "\nBuilding and simplification of the alternating automaton: %i.%06is",
                  t_diff.tv_sec, t_diff.tv_usec);
              fprintf(tl_out, "\n%i states, %i transitions\n", astate_count, atrans_count);
            }*/

            cache.releasenode(1, p);
            mem.tfree(label);
        }
    }
}
