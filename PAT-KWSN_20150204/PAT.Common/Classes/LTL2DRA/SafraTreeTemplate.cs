using System.Diagnostics;
using PAT.Common.Classes.LTL2DRA.common;

namespace PAT.Common.Classes.LTL2DRA
{
    public interface ResultStateInterface<T>
    {
        /** Get the SafraTree */
        T getState();
  
    }

    public class SafraTreeTemplate : ResultStateInterface<SafraTree>
    {
        SafraTree _safraTree;
        BitSet _renameableNames;
        BitSet _restrictedNames;

        /**
        * Constructor.
        * @param safraTree the SafraTree
        */
        public SafraTreeTemplate(SafraTree safraTree)
        {
            _safraTree = safraTree;
            //todo. check whether the two bitarray needs to be initialized or not.
            _renameableNames= new BitSet();
            _restrictedNames = new BitSet();
        }


        /** Get the SafraTree */
        public SafraTree getSafraTree()
        {
            return _safraTree;
        }

        /** Get the SafraTree */
        public SafraTree getState()
        {
            return _safraTree;
        }
        /** Get the names of nodes that may be renamed. */
        public BitSet renameableNames()
        {
            return _renameableNames;
        }

        /** Get the names that can are not allowed to be used in the Safra tree */
        public BitSet restrictedNames()
        {
            return _restrictedNames;
        }
        /** Set the 'renameable' flag for a name */
        public void setRenameable(int name, bool flag)
        {
            _renameableNames.set(name, flag);
        }

        public void setRenameable(int name)
        {
            _renameableNames.set(name, true);

        }

        /** Get the 'renameable' flag for a name */
        public bool isRenameable(int name)
        {
            return _renameableNames.get(name);
        }

        /** Set the 'restricted' flag for a name */
        public void setRestricted(int name, bool flag)
        {
            _restrictedNames.set(name, flag);
        }
        public void setRestricted(int name)
        {
            _restrictedNames.set(name, true);
        }

        /** Get the 'restricted' flag for a name */
        public bool isRestricted(int name)
        {
            return _restrictedNames.get(name);
        }

        /**
  * Return true if this tree (taking into account the renameableNames and the restrictedNames) 
  * can be renamed to match the SafraTree other.
  * Can only be called for trees that are structural_equal!!!
  */
        public bool matches(SafraTree other)
        {
            SafraTreeNode this_root = _safraTree.getRootNode();
            SafraTreeNode other_root = other.getRootNode();

            if (this_root == null || other_root == null)
            {
                Debug.Assert(this_root == null && other_root == null);
                return true;
            }

            return matches(this_root, other_root);
        }


        /**
         * Compare two subtrees to see if they match (taking into account the renameableNames
         * and the restrictedNames).
         */
        public bool matches(SafraTreeNode this_node, SafraTreeNode other_node)
        {
            Debug.Assert(this_node != null && other_node != null);

            if (this_node == null || other_node == null)
            {
                return false;
            }

            if (!renameableNames().get(this_node.getID()))
            {
                // this is not a new node, so we require a perfect match..
                if (other_node.getID() != this_node.getID())
                {
                    return false;
                }
            }
            else
            {
                // we are flexible with the id, as long as the id wasn't removed
                //  in the tree
                if (restrictedNames().get(other_node.getID()))
                {
                    return false;
                }
            }

            Debug.Assert(this_node.getLabeling() == other_node.getLabeling());
            Debug.Assert(this_node.hasFinalFlag() == other_node.hasFinalFlag());

            // this node looks good, now the children
            SafraTreeNode this_child = this_node.getOldestChild();
            SafraTreeNode other_child = other_node.getOldestChild();

            while (this_child != null && other_child != null)
            {
                if (!matches(this_child, other_child))
                {
                    return false;
                }

                this_child = this_child.getYoungerBrother();
                other_child = other_child.getYoungerBrother();
            }
            Debug.Assert(this_child == null && other_child == null);

            return true;
        }

    }
}
