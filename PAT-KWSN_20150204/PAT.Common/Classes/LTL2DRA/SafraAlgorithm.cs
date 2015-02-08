namespace PAT.Common.Classes.LTL2DRA
{
    public interface AlgorithmInterface<T, RT>
    {
        bool checkEmpty();
        void prepareAcceptance(RabinAcceptance acceptance);

        T getStartState();
        RT delta(T from_state, APElement elem);

    }

    public class SafraAlgorithm : AlgorithmInterface<SafraTree, ResultStateInterface<SafraTree>>
    {
        public Options_Safra _options;
        public NBAAnalysis _nba_analysis;
        public NBA _nba;
        public int _NODES;

        //List<BitSet> _next;

        /** Cacheing the STVisitor_reorder_children, as it's initialization is complex. */
        STVisitor_check_children_horizontal.STVisitor_check_children_vertical.STVisitor_reorder_children stv_reorder;

        public SafraAlgorithm(NBA nba, Options_Safra options)
        {

            _options = options;
            _nba_analysis = new NBAAnalysis(nba);
            _nba = nba;
            _NODES = 2 * nba.getStateCount();////////////ensure the number of nodes is enough

            stv_reorder = null;
            //{
            //    _next.resize(nba.getStateCount());
            //}
        }


        // typedef SafraTreeTemplate_ptr result_t;
        //  typedef SafraTree state_t;

        public ResultStateInterface<SafraTree> delta(SafraTree tree, APElement elem)
        {
            return process(tree, elem);
        }

        public SafraTree getStartState()///////////get the start safratree in DRA
        {
            SafraTree start = new SafraTree(_NODES);
            if (_nba.getStartState() != null)
            {
                start.getRootNode().getLabeling().set(_nba.getStartState().getName());
            }

            return start;
        }

        public void prepareAcceptance(RabinAcceptance acceptance)
        {
            acceptance.newAcceptancePairs(_NODES);
        }

        /** 
 * Get the next Safra tree using the
 * transition function as described by Safra.
 * @param tree the original tree
 * @param elem the edge label
 * @return a SafraTreeTemplate containing the new tree and 
 *         bookkeeping which states were created/deleted.
 */
        SafraTreeTemplate process(SafraTree tree, APElement elem)
        {

            // Make copy of original tree
            SafraTree cur = new SafraTree(tree);

            SafraTreeTemplate tree_template = new SafraTreeTemplate(cur);

            STVisitor_reset_final_flag stv_reset_flag = new STVisitor_reset_final_flag();
            cur.walkTreePostOrder(stv_reset_flag);

////#if NBA2DRA_POWERSET_FIRST
//  STVisitor_powerset stv_powerset = new STVisitor_powerset(_nba, elem);
//  cur.walkTreePostOrder(stv_powerset);

//  STVisitor_check_finalset stv_final = new STVisitor_check_finalset(_nba_analysis.getFinalStates(), tree_template);
//  cur.walkTreePostOrder(stv_final);
//#else
            /////////generate new child
            STVisitor_check_finalset stv_final = new STVisitor_check_finalset(_nba_analysis.getFinalStates(), tree_template);
            cur.walkTreePostOrder(stv_final);
            //////////change the old label to new label using powerset
            STVisitor_powerset stv_powerset = new STVisitor_powerset(_nba, elem);
            cur.walkTreePostOrder(stv_powerset);
//#endif


            /*
   * Optimization: ACCEPTING_TRUE_LOOPS
   */
            if (_options.opt_accloop)///////////how to decide the value?
            {
                if (cur.getRootNode() != null)
                {
                    SafraTreeNode root = cur.getRootNode();

                    if (_nba_analysis.getStatesWithAcceptingTrueLoops().intersects(root.getLabeling()))
                    {
                        // True Loop
                        STVisitor_remove_subtree stv_remove = new STVisitor_remove_subtree(tree_template);
                        cur.walkChildrenPostOrder(stv_remove, root);

                        root.getLabeling().clear();

                        int canonical_true_loop = _nba_analysis.getStatesWithAcceptingTrueLoops().nextSetBit(0);
                        root.getLabeling().set(canonical_true_loop);
                        root.setFinalFlag(true);

                        return tree_template;
                    }
                }
            }

            //check if the younger child has the same labels of older ones. If so, remove the same labels from younger child
            STVisitor_check_children_horizontal stv_horizontal = new STVisitor_check_children_horizontal();/////////note sth wrong
            cur.walkTreePostOrder(stv_horizontal);
            //check if a node becomes empty. If so, remove it.
            STVisitor_check_children_horizontal.STVisitor_check_children_vertical.STVisitor_remove_empty stv_empty = new STVisitor_check_children_horizontal.STVisitor_check_children_vertical.STVisitor_remove_empty(tree_template);
            cur.walkTreePostOrder(stv_empty);
            //check if a node's children's labels union is same with itself's. If so, remove the children and mark this node.
            STVisitor_check_children_horizontal.STVisitor_check_children_vertical stv_vertical = new STVisitor_check_children_horizontal.STVisitor_check_children_vertical(tree_template);
            cur.walkTreePostOrder(stv_vertical);


            /*
             * Optimization: REORDER
             */
            if (_options.opt_reorder)////////
            {
                if (stv_reorder == null)
                {
                    stv_reorder = new STVisitor_check_children_horizontal.STVisitor_check_children_vertical.STVisitor_reorder_children(_nba_analysis.getReachability(), cur.getNodeMax());
                }

                cur.walkTreePostOrder(stv_reorder);
            }



            /*
             * Optimization: ALL SUCCESSORS ARE ACCEPTING
             */
            if (_options.opt_accsucc)////////
            {
                STVisitor_check_children_horizontal.STVisitor_check_children_vertical.STVisitor_check_for_succ_final stv_succ = new STVisitor_check_children_horizontal.STVisitor_check_children_vertical.STVisitor_check_for_succ_final(_nba_analysis.getStatesWithAllSuccAccepting(), tree_template);

                cur.walkTreePostOrder(stv_succ);
                if (stv_succ.wasSuccessful())
                {
#if NBA2DRA_MERGE
      if (stv_succ.wasMerged()) {
	SafrasAlgorithmInternal::STVisitor_remove_empty stv_empty;
	cur.walkTreePostOrder(stv_empty);
	
	SafrasAlgorithmInternal::STVisitor_check_children_vertical stv_vertical;
	cur.walkTreePostOrder(stv_vertical);
      }
#endif   // NBA2DRA_MERGE
                }
            }

            return tree_template;
        }
        public bool checkEmpty()
        {
            if (_nba.size() == 0 || _nba.getStartState() == null)
            {
                return true;
            }
            return false;
        }



    }
}
