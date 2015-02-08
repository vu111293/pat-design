namespace PAT.Common.Classes.LTL2DRA
{
    // template <class SafraTreeVisitor>

    public class SafraTreeWalker//<SafraTreeVisitor>
    {
        public SafraTreeVisitor _visitor;

        /** Constructor.
        *  @param visitor the visitor functor
        */
        public SafraTreeWalker(SafraTreeVisitor visitor)
        {
            _visitor = visitor;
        }


        /** 
   * Walk the tree post-order and call visit() on each node.
   * @param tree the SafraTree
   */
        public void walkTreePostOrder(SafraTree tree)
        {
            if (tree.getRootNode() == null)
            {
                return;
            }
            walkSubTreePostOrder(tree, tree.getRootNode());
        }


        public void walkSubTreePostOrder(SafraTree tree, SafraTreeNode top)
        {
            walkSubTreePostOrder(tree, top, true);
        }
        /** 
   * Walk the subtree rooted at *top post-order and call visit() on each node.
   * @param tree the SafraTree
   * @param top the current subroot 
   * @param visit_top if true, *top is visited too
   */
        public void walkSubTreePostOrder(SafraTree tree, SafraTreeNode top, bool visit_top)
        {
            if (top.getChildCount() > 0)
            {
                SafraTreeNode it = top.children_begin();
                //SafraTreeNode it = top._oldestChild;
                //while (it._youngestChild != null)
                while (it != top.children_end())
                {
                    SafraTreeNode cur_child = it;

                    // Increment iterator *before* recursion & visit to account
                    // for possible deletion of this child
                    //it = it.getYoungerBrother();
                    it = it.increment();
                    walkSubTreePostOrder(tree, cur_child, true);
                }
            }

            if (visit_top)
            {
                _visitor.visit(tree, top);
            }
        }
    }
}   
