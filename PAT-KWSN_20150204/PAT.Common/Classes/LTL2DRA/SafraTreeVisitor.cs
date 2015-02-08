using System.Collections.Generic;
using PAT.Common.Classes.LTL2DRA.common;

namespace PAT.Common.Classes.LTL2DRA
{
    public interface SafraTreeVisitor
    {
        void visit(SafraTree tree, SafraTreeNode node);
    }

    /**
    * Safra tree visitor that resets the final flag on the Safra tree node.
    */
    public class STVisitor_reset_final_flag : SafraTreeVisitor
    {
        /** Node visitor */
        public void visit(SafraTree tree, SafraTreeNode node)
        {
            node.setFinalFlag(false);
        }
    }

    /**
  * Safra tree visitor that creates a new child node if
  * the label of the node and the set of final states in the
  * NBA intersect.
  */
    public class STVisitor_check_finalset : SafraTreeVisitor
    {


        private BitSet _final_states;
        private SafraTreeTemplate _tree_template;

        /**
         * Constructor.
         * @param final_states the states that are accepting (final) in the NBA
         * @param tree_template the tree template to keep track of new nodes
         */
        public STVisitor_check_finalset(BitSet final_states, SafraTreeTemplate tree_template)
        {
            _final_states = final_states;
            _tree_template = tree_template;
        }

        /** */
        public void visit(SafraTree tree, SafraTreeNode node)
        {
            if (_final_states.intersects(node.getLabeling()))//////////if this node has the state in accepting states of NBA
            {
                BitSet q_and_f = new BitSet(_final_states);
                q_and_f.Intersect(node.getLabeling());////////////////get the intersect of the label and the accepting states

                SafraTreeNode new_child = tree.newNode();
                node.addAsYoungestChild(new_child);

                _tree_template.setRenameable(new_child.getID());

                new_child.getLabeling().Assign(q_and_f);
            }
        }
    }

    /**
 * Safra tree visitor that performs the powerset construction
 * on the label of the Safra tree node.
 */
    public class STVisitor_powerset: SafraTreeVisitor
    {


        private NBA _nba;
        private APElement _elem;

        /**
         * Constructor.
         */
        public STVisitor_powerset(NBA nba, APElement elem)
        {
            _nba = nba;
            _elem = elem;
        }

        /** Node visitor */
        public void visit(SafraTree tree, SafraTreeNode node)
        {

            BitSet new_labeling = new BitSet();

            BitSet old_labeling = node.getLabeling();
            for (int i = old_labeling.nextSetBit(0); i != -1; i = old_labeling.nextSetBit(i + 1))
            {
                new_labeling.Union(_nba[i].getEdge(_elem));//////////////generate new label.
            }

            node.getLabeling().Assign(new_labeling);
        }


    }

    /**
 * A Safra tree visitor that subtracts (minus operator) a BitSet from
 * the label of the tree node.
 */
    public class STVisitor_subtract_labeling : SafraTreeVisitor
    {
        private BitSet _bitset;
        public STVisitor_subtract_labeling(BitSet bitset)
        {
            _bitset = bitset;
        }
        public void visit(SafraTree tree, SafraTreeNode node)
        {
            node.getLabeling().Minus(_bitset);
        }

    }

    /**
 * A Safra tree visitor that removes all 
 * children of the node.
 */
    public class STVisitor_remove_subtree : SafraTreeVisitor
    {
        private SafraTreeTemplate _tree_template;

        public STVisitor_remove_subtree(SafraTreeTemplate tree_template)
        {
            _tree_template = tree_template;
        }

        /** Node visitor */
        public void visit(SafraTree tree, SafraTreeNode node)
        {
            int id = node.getID();
            if (_tree_template.isRenameable(id))
            {
                // this node was created recently, so we only delete it from
                // the renameableNames, but don't mark it in restrictedNames
                _tree_template.setRenameable(id, false);
            }
            else
            {
                _tree_template.setRestricted(id);
            }

            tree.remove(node);
        }
    }

    /**
     * A Safra tree visitor that modifies the
     * children so that all children have
     * disjoint labels
     */
    public class STVisitor_check_children_horizontal : SafraTreeVisitor
    {

        /** Node visitor */
        public void visit(SafraTree tree, SafraTreeNode node)
        {
            if (node.getChildCount() <= 1)
            {
                return;
            }

            BitSet already_seen = new BitSet();
            bool first = true;
            //for (SafraTreeNode::child_iterator it=node->children_begin();it!=node->children_end();++it)
            SafraTreeNode it = node.children_begin();
            while (it != node.children_end())
            {
                SafraTreeNode cur_child = it;
                if (first)
                {
                    already_seen = new BitSet(cur_child.getLabeling());////////////get the NBA states in child
                    first = false;
                    it = it.increment();////////note added
                }
                else
                {
                    BitSet current = new BitSet(cur_child.getLabeling());

                    BitSet intersection = new BitSet(already_seen); // make copy
                    if (intersection.intersects(current))
                    {
                        // There are some labels, which occur in older brothers,
                        // remove them from current node and its children
                        STVisitor_subtract_labeling stv_sub = new STVisitor_subtract_labeling(intersection);
                        tree.walkSubTreePostOrder(stv_sub, cur_child);
                    }

                    already_seen.Union(current);
                    it = it.increment();
                }
            }
        }

        /**
         * A Safra tree visitor that ensures that 
         * the union of the labels of the children
         * are a proper subset of the label of the
         * parents. Otherwise, the children are
         * removed and the final flag is set on
         * the tree node.
         */
        public class STVisitor_check_children_vertical : SafraTreeVisitor
        {

            private SafraTreeTemplate _tree_template;
            public STVisitor_check_children_vertical(SafraTreeTemplate tree_template)
            {
                _tree_template = tree_template;
            }

            /** Node visitor */
            public void visit(SafraTree tree, SafraTreeNode node)
            {
                if (node.getChildCount() == 0) { return; }

                BitSet labeling_union = new BitSet();
                //for (SafraTreeNode::child_iterator it=node->children_begin();it!=node->children_end();++it) 
                SafraTreeNode it = node.children_begin();
                while (it != node.children_end())
                {
                    labeling_union.Union(it.getLabeling());
                    it = it.increment();
                }

                if (labeling_union == node.getLabeling())
                {
                    // The union of the labelings of the children is exactly the 
                    // same as the labeling of the parent ->
                    //  remove children
                    STVisitor_remove_subtree stv_remove = new STVisitor_remove_subtree(_tree_template);
                    tree.walkChildrenPostOrder(stv_remove, node);

                    node.setFinalFlag(true);///////////should be "+ i", means in Li
                }
            }


            /**
             * Safra tree visitor that attempts
             * to reorder the independant children 
             * into a canonical order.
             * Two children are independet if
             * their is no state that is reachable by 
             * states in both labels.
             */
            public class STVisitor_reorder_children : SafraTreeVisitor
            {

                List<BitSet> _nba_reachability;
                BitSet[] _node_reachability;
                int[] _node_order;
                int _n;
                /**
                 * Constructor
                 * nba_reachability A vector of BitSets (state index -> BitSet) of states
                 *                  in the NBA that are reachable from a state.
                 * N                the maximum number of nodes in the Safra tree
                 */
                public STVisitor_reorder_children(List<BitSet> nba_reachability, int N)
                {
                    _nba_reachability = nba_reachability;
                    _n = N;
                    _node_order = new int[N];
                    _node_reachability = new BitSet[N];

                    //added by ly
                    for (int i = 0; i < N; i++)
                    {
                        _node_reachability[i] = new BitSet();
                    }
                }

                /** Node visitor */
                public void visit(SafraTree tree, SafraTreeNode node)
                {
                    if (node.getChildCount() <= 1)
                    {
                        return;
                    }

                    int i = 0;
                    //for (SafraTreeNode::child_iterator it= node->children_begin(); it!=node->children_end();++it) 
                    SafraTreeNode it = node.children_begin();
                    while (it != node.children_end())
                    {
                        BitSet reachable_this = _node_reachability[it.getID()];
                        reachable_this.clear();
                        _node_order[it.getID()] = i++;

                        BitSet label_this = it.getLabeling();
                        //for (BitSetIterator label_it(label_this); label_it != BitSetIterator::end(label_this); ++label_it)
                        for (int label_it = 0; label_it < label_this.Count; label_it++)
                        {
                            reachable_this.Union(_nba_reachability[label_it]);
                        }

                        //      std::cerr << "reachability_this: "<<reachable_this << std::endl; 
                        it = it.increment();

                    }


                    // reorder...
                    //    std::cerr << "Sorting!" << std::endl;

                    // Bubble sort, ough!
                    bool finished = false;
                    while (!finished)
                    {
                        finished = true;

                        for (SafraTreeNode a = node.getOldestChild(); a != null && a.getYoungerBrother() != null; a = a.getYoungerBrother())
                        {

                            SafraTreeNode b = a.getYoungerBrother();

                            BitSet reach_a = _node_reachability[a.getID()];
                            BitSet reach_b = _node_reachability[b.getID()];

                            if (reach_a.intersects(reach_b))
                            {
                                // a and b are not independant...
                                // --> keep relative order...
                                System.Diagnostics.Debug.Assert(_node_order[a.getID()] < _node_order[b.getID()]);
                            }
                            else
                            {
                                // a and b are independant...
                                if (!(a.getLabeling() < b.getLabeling()))
                                {
                                    // swap
                                    node.swapChildren(a, b);
                                    a = b;
                                    finished = false;
                                }
                            }
                        }
                    }
                }
            }


            /**
             * A Safra tree visitor that removes tree nodes
             * with empty labels.
             */
            public class STVisitor_remove_empty : SafraTreeVisitor
            {

                private SafraTreeTemplate _tree_template;
                public STVisitor_remove_empty(SafraTreeTemplate tree_template)
                {
                    _tree_template = tree_template;
                }

                /** Node visitor */
                public void visit(SafraTree tree, SafraTreeNode node)
                {
                    if (node.getLabeling().isEmpty())
                    {
                        int id = node.getID();
                        if (_tree_template.isRenameable(id))
                        {
                            // this node was created recently, so we only delete it in
                            // renameableNames, but don't mark it in restrictedNodes
                            _tree_template.setRenameable(id, false);
                        }
                        else
                        {
                            _tree_template.setRestricted(id);
                        }

                        tree.remove(node);
                    }
                }


            }


            /**
             * A Safra tree visitor that checks if all
             * the successor states in the NBA of the label
             * are accepting. If this is the case, all
             * children are removed, and the final flag is set.
             */
            public class STVisitor_check_for_succ_final : SafraTreeVisitor
            {

                private bool _success;
                private BitSet _nba_states_with_all_succ_final;
                private SafraTreeTemplate _tree_template;


                /** 
                 * Constructor 
                 * @param nba_states_with_all_succ_final A BitSet with the indizes of the
                 *                                       NBA states that only have accepting (final)
                 *                                       successors.
                 * @param tree_template                  SafraTreeTemplate to keep track of removed nodes
                 */
                public STVisitor_check_for_succ_final(BitSet nba_states_with_all_succ_final, SafraTreeTemplate tree_template)
                {
                    _success = false;
                    _nba_states_with_all_succ_final = nba_states_with_all_succ_final;
                    _tree_template = tree_template;
                }

                /** Returns true if the condition was triggered. */
                public bool wasSuccessful() { return _success; }

                /** Node visitor */
                public void visit(SafraTree tree, SafraTreeNode node)
                {

                    bool all_final = true;
                    //for (BitSetIterator it=BitSetIterator(node->getLabeling());it!=BitSetIterator::end(node->getLabeling());++it) 
                    //BitSet label_this = node.getLabeling();
                    //for (int it = 0; it < label_this.Count; it++)
                    for (int it = BitSetIterator.start(node.getLabeling()); it != BitSetIterator.end(node.getLabeling()); it = BitSetIterator.increment(node.getLabeling(), it)) 
                    {
                        ////	if (!_nba_states_with_all_succ_final.get(*it)) {
                        if (!_nba_states_with_all_succ_final.get(it))
                        {
                            all_final = false;
                            break;
                        }
                    }

                    if (all_final)
                    {
                        // remove all children of node & set final flag
                        STVisitor_remove_subtree stv_remove = new STVisitor_remove_subtree(_tree_template);
                        tree.walkChildrenPostOrder(stv_remove, node);

                        node.setFinalFlag();

                        _success = true;
                    }
                }
            }
        }
    }
}
