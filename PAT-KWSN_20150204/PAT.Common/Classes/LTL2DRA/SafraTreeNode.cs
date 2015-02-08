using System.Text;
using PAT.Common.Classes.LTL2DRA.common;

namespace PAT.Common.Classes.LTL2DRA
{
    public class SafraTreeNode
    {
        /** The node name */
        public int _id;

        /** The label of the node (powerset) */
        public BitSet _labeling;

        /** The final flag */
        public bool _final_flag;

        /** The parent node */
        public SafraTreeNode _parent;

        /** The older brother */
        public SafraTreeNode _olderBrother;

        /** The younger brother */
        public SafraTreeNode _youngerBrother;

        /** The oldest child */
        public SafraTreeNode _oldestChild;

        /** The youngest child */
        public SafraTreeNode _youngestChild;

        /** The number of children */
        public int _childCount;

        /** 
        * Constructor 
        * @param id the name of the node
        */
        public SafraTreeNode(int id)
        {
            this._id = id;
            //_final_flag(false),
            // _parent(0),
            //_olderBrother(0),
            //_youngerBrother(0),
            //_oldestChild(0),
            //_youngestChild(0),
            //_childCount(0)
            _labeling = new BitSet();
        }

        /** Get the name of the node*/
        public int getID()
        {
            return _id;
        }

        /** Get the final flag */
        public bool hasFinalFlag()
        {
            return _final_flag;
        }

        public BitSet getLabeling()
        {
            return _labeling;
        }

        /** 
        * Get the older brother.
        * @return the older brother, or NULL if node is oldest child.
        */
        public SafraTreeNode getOlderBrother()
        {
            return _olderBrother;
        }

        /** 
        * Get the youngest child.
        * @return the youngest child, or NULL if node has no children
        */
        public SafraTreeNode getYoungestChild()
        {
            return _youngestChild;
        }

        /** 
        * Get the oldest child.
        * @return the oldest child, or NULL if node has no children
        */
        public SafraTreeNode getOldestChild()
        {
            return _oldestChild;
        }

        /** 
        * Get the parent of the node.
        * @return the parent, or NULL if node has no parent (is root node)
        */
        public SafraTreeNode getParent()
        {
            return _parent;
        }



        /** 
         * Get the younger brother.
         * @return the younger brother, or NULL if node is youngest child.
         */
        public SafraTreeNode getYoungerBrother()
        {
            return _youngerBrother;
        }

        /** Get the number of children. */
        public int getChildCount()
        {
            return _childCount;
        }



        /** 
        * Set the final flag.
        * @param finalFlag the value
        */
        public void setFinalFlag(bool finalFlag)
        {
            _final_flag = finalFlag;
        }

        public void setFinalFlag() { _final_flag = true; }

        private static bool NULL_OR_EQUALID(SafraTreeNode a, SafraTreeNode b)
        {
            return (a == null && b == null) || ((a != null && b != null) && (a._id == b._id));
        }

        //add this code to class ThreeDPoint as defined previously
        //
        public static bool operator ==(SafraTreeNode a, SafraTreeNode b)
        {
            bool? val = Ultility.NullCheck(a, b);
            if (val != null)
            {
                return val.Value;
            }

            if (!(a._id == b._id))
            {
                return false;
            }
            if (!(a._final_flag == b._final_flag))
            {
                return false;
            }
            if (!(a._childCount == b._childCount))
            {
                return false;
            }
            if (!(a._labeling == b._labeling))
            {
                return false;
            }
            if (!NULL_OR_EQUALID(a._parent, b._parent))
            {
                return false;
            }
            if (!NULL_OR_EQUALID(a._olderBrother, b._olderBrother))
            {
                return false;
            }
            if (!NULL_OR_EQUALID(a._youngerBrother, b._youngerBrother))
            {
                return false;
            }
            if (!NULL_OR_EQUALID(a._oldestChild, b._oldestChild))
            {
                return false;
            }
            if (!NULL_OR_EQUALID(a._youngestChild, b._youngestChild))
            {
                return false;
            }

            return true;
        }

        public static bool operator !=(SafraTreeNode a, SafraTreeNode b)
        {
            return !(a == b);
        }

        //      #define NULL_OR_EQUALID(a,b) ((a==0 && b==0) || ((a!=0 && b!=0) && (a.getID()==b.getID())))

        ///** Equality operator. Does not do a deep compare */
        //bool operator==(const SafraTreeNode& other) {
        //  if (!(_id==other._id)) {return false;}
        //  if (!(_final_flag==other._final_flag)) {return false;}
        //  if (!(_childCount==other._childCount)) {return false;}
        //  if (!(_labeling==other._labeling)) {return false;}
        //  if (!NULL_OR_EQUALID(_parent, other._parent)) {return false;}
        //  if (!NULL_OR_EQUALID(_olderBrother, other._olderBrother)) {return false;}
        //  if (!NULL_OR_EQUALID(_youngerBrother, other._youngerBrother)) {return false;}
        //  if (!NULL_OR_EQUALID(_oldestChild, other._oldestChild)) {return false;}
        //  if (!NULL_OR_EQUALID(_youngestChild, other._youngestChild)) {return false;}

        //  return true;
        //}

        /** 
        * Equality operator ignoring the name of the nodes, doing a deep compare
        * (checks that all children are also structurally equal.
        */
        public bool structural_equal_to(SafraTreeNode other)
        {
            if (!(_final_flag == other._final_flag))
            {
                return false;
            }
            if (!(_childCount == other._childCount))
            {
                return false;
            }
            if (!(_labeling == other._labeling))
            {
                return false;
            }

            if (_childCount > 0)
            {
                SafraTreeNode this_child = this._oldestChild;
                SafraTreeNode other_child = other._oldestChild;

                do
                {
                    if (!this_child.structural_equal_to(other_child))
                    {
                        return false;
                    }

                    this_child = this_child._youngerBrother;
                    other_child = other_child._youngerBrother;
                } while (this_child != null && other_child != null);

                System.Diagnostics.Debug.Assert(this_child == null && other_child == null);
            }
            return true;
        }

        // LEG = LESS, EQUAL, GREATER
        enum LEG { LESS, EQUAL, GREATER }

        //private static LEG CMP(SafraTreeNode a,  SafraTreeNode b)
        //{
        //    return ((a < b) ? LEG.LESS : (a == b ? LEG.EQUAL : LEG.GREATER));
        //}

        private static LEG CMP(int a, int b)
        {
            return ((a < b) ? LEG.LESS : (a == b ? LEG.EQUAL : LEG.GREATER));
        }


        private static bool CMP_ID(SafraTreeNode a, SafraTreeNode b)
        {
            if (a == null)
            {
                if (b != null)
                {
                    return true;
                }
            }
            else
            {
                if (b == null)
                {
                    return false;
                }

                if (a._id != b._id)
                {
                    return a._id < b._id;
                }
            }

            return false;
        }


        private static LEG CMP(BitSet a, BitSet b)
        {
            if (a < b)
            {
                return LEG.LESS;
            }
            else
            {
                if (a == b)
                {
                    return LEG.EQUAL;
                }
                else
                {
                    return LEG.GREATER;
                }
            }
        }

        /** Less-than operator. Does not do deep compare */
        public static bool operator <(SafraTreeNode a, SafraTreeNode b)
        {

            LEG cmp = CMP(a._id, b._id);
            if (cmp != LEG.EQUAL)
            {
                return (cmp == LEG.LESS);
            }

            cmp = CMP(a._final_flag ? 1 : 0, b._final_flag ? 1 : 0);
            if (cmp != LEG.EQUAL)
            {
                return (cmp == LEG.LESS);
            }

            cmp = CMP(a._childCount, b._childCount);
            if (cmp != LEG.EQUAL)
            {
                return (cmp == LEG.LESS);
            }

            cmp = CMP(a._labeling, b._labeling);
            if (cmp != LEG.EQUAL)
            {
                return (cmp == LEG.LESS);
            }

            CMP_ID(a._parent, b._parent);
            CMP_ID(a._olderBrother, b._olderBrother);
            CMP_ID(a._youngerBrother, b._youngerBrother);
            CMP_ID(a._oldestChild, b._oldestChild);
            CMP_ID(a._youngestChild, b._youngestChild);

            return false;
        }

        /** Less-than operator. Does not do deep compare */
        public static bool operator >(SafraTreeNode a, SafraTreeNode b)
        {
            if (a == b || a < b)
            {
                return false;
            }
            return true;
        }

        /** 
 * Less-than operator ignoring the name of the nodes, doing a deep compare
 * (applies recursively on the children).
 */
        public bool structural_less_than(SafraTreeNode other)
        {
            return (this.structural_cmp(other) == LEG.LESS);
        }



        /** Do a structural comparison */
        private LEG structural_cmp(SafraTreeNode other)
        {

            LEG cmp = CMP(_final_flag ? 1 : 0, other._final_flag ? 1 : 0);
            if (cmp != LEG.EQUAL)
            {
                return cmp;
            }

            cmp = CMP(_childCount, other._childCount);
            if (cmp != LEG.EQUAL)
            {
                return cmp;
            }

            cmp = CMP(_labeling, other._labeling);
            if (cmp != LEG.EQUAL)
            {
                return cmp;
            }

            // if we are here, this and other have the same number of children
            if (_childCount > 0)
            {
                SafraTreeNode this_child = this._oldestChild;
                SafraTreeNode other_child = other._oldestChild;

                do
                {
                    cmp = this_child.structural_cmp(other_child);
                    if (cmp != LEG.EQUAL)
                    {
                        return cmp;
                    }

                    this_child = this_child._youngerBrother;
                    other_child = other_child._youngerBrother;
                } while (this_child != null && other_child != null);

                // assert that there was really the same number of children
                System.Diagnostics.Debug.Assert(this_child == null && other_child == null);
            }

            // when we are here, all children were equal
            return LEG.EQUAL;
        }

        /** Add a node as the youngest child */
        public void addAsYoungestChild(SafraTreeNode other)
        {
            System.Diagnostics.Debug.Assert(other._parent == null);
            System.Diagnostics.Debug.Assert(other._olderBrother == null);
            System.Diagnostics.Debug.Assert(other._youngerBrother == null);

            if (_youngestChild != null)
            {
                System.Diagnostics.Debug.Assert(_youngestChild._youngerBrother == null);
                _youngestChild._youngerBrother = other;
                other._olderBrother = _youngestChild;
            }

            other._parent = this;
            _youngestChild = other;
            if (_oldestChild == null)
            {
                _oldestChild = other;
            }
            _childCount++;
        }

        /** Add a node as the oldest child */
        public void addAsOldestChild(SafraTreeNode other)
        {
            System.Diagnostics.Debug.Assert(other._parent == null);
            System.Diagnostics.Debug.Assert(other._olderBrother == null);
            System.Diagnostics.Debug.Assert(other._youngerBrother == null);

            if (_oldestChild != null)
            {
                System.Diagnostics.Debug.Assert(_oldestChild._olderBrother == null);
                _oldestChild._olderBrother = other;
                other._youngerBrother = _oldestChild;
            }

            other._parent = this;
            _oldestChild = other;
            if (_youngestChild == null)
            {
                _youngestChild = other;
            }
            _childCount++;
        }

        /** Remove this node from the tree (relink siblings). The node is not allowed to have children! */
        public void removeFromTree()
        {
            System.Diagnostics.Debug.Assert(_childCount == 0);

            if (_parent == null)
            {
                // Root-Node or already removed from tree, nothing to do
                return;
            }

            // Relink siblings
            if (_olderBrother != null)
            {
                _olderBrother._youngerBrother = _youngerBrother;
            }
            if (_youngerBrother != null)
            {
                _youngerBrother._olderBrother = _olderBrother;
            }

            // Relink child-pointers in _parent
            if (_parent._oldestChild == this)
            {
                // this node is oldest child
                _parent._oldestChild = this._youngerBrother;
            }

            if (_parent._youngestChild == this)
            {
                // this node is youngest child
                _parent._youngestChild = this._olderBrother;
            }

            _parent._childCount = _parent._childCount - 1;

            _youngerBrother = null;
            _olderBrother = null;
            _parent = null;
        }

        /**
   * Swap the places of two child nodes 
   */
        public void swapChildren(SafraTreeNode a, SafraTreeNode b)
        {
            System.Diagnostics.Debug.Assert(a._parent == b._parent && a._parent == this);

            if (a == b)
            {
                return;
            }

            if (_oldestChild == a)
            {
                _oldestChild = b;
            }
            else if (_oldestChild == b)
            {
                _oldestChild = a;
            }

            if (_youngestChild == a)
            {
                _youngestChild = b;
            }
            else if (_youngestChild == b)
            {
                _youngestChild = a;
            }

            SafraTreeNode a_left = a._olderBrother, b_left = b._olderBrother, a_right = a._youngerBrother, b_right = b._youngerBrother;

            if (a_left != null)
            {
                a_left._youngerBrother = b;
            }
            if (b_left != null)
            {
                b_left._youngerBrother = a;
            }
            if (a_right != null)
            {
                a_right._olderBrother = b;
            }
            if (b_right != null)
            {
                b_right._olderBrother = a;
            }

            a._olderBrother = b_left;
            a._youngerBrother = b_right;
            b._olderBrother = a_left;
            b._youngerBrother = a_right;

            if (a_right == b)
            {
                // a & b are direct neighbours, a to the left of b
                a._olderBrother = b;
                b._youngerBrother = a;
            }
            else if (b_right == a)
            {
                // a & b are direct neighbours, b to the left of a
                a._youngerBrother = b;
                b._olderBrother = a;
            }
        }


        /** Calculate the height of the subtree rooted at this node. */
        public int treeHeight()
        {
            int height = 0;

            if (this._childCount > 0)
            {

                SafraTreeNode it = this._oldestChild;
                while (it != null)
                {
                    SafraTreeNode cur_child = it;
                    int child_height = cur_child.treeHeight();
                    if (child_height > height)
                    {
                        height = child_height;
                    }
                    it = it._youngerBrother;
                }
            }

            return height + 1;
        }


        /** Calculate the width of the subtree rooted at this node. */
        public int treeWidth()
        {
            int width = 0;

            if (this._childCount > 0)
            {
                SafraTreeNode it = this._oldestChild;

                while (it != null)
                {
                    SafraTreeNode cur_child = it;
                    width += cur_child.treeWidth();

                    it = it._youngerBrother;

                }
            }
            else
            {
                width = 1;
            }

            return width;
        }

        public SafraTreeNode children_begin()
        {
            return getOldestChild();
        }

        public SafraTreeNode children_end()
        {
            return null;
        }

        public SafraTreeNode increment()
        {
            return getYoungerBrother();
        }

        /** 
 * Calculate a hashvalue using HashFunction for this node.
 * @param hashfunction the HashFunction functor
 * @param only_structure Ignore naming of the nodes?
 */
        public void hashCode(HashFunction hashfunction, bool only_structure)  //=false
        {
            if (!only_structure)
            {
                hashfunction.hash(getID());
            }

            getLabeling().hashCode(hashfunction);
            hashfunction.hash(hasFinalFlag());

            if (getChildCount() > 0)
            {
                //for (child_iterator cit=children_begin();cit!=children_end();++cit) 
                for (SafraTreeNode cit = children_begin(); cit != children_end(); cit = cit.increment())
                {
                    cit.hashCode(hashfunction, only_structure);
                }
            }
        }

          /** Print HTML version of this node to output stream */
        public string toHTML()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("<TABLE><TR>");

            if (getChildCount() <= 1)
            {
                sb.Append("<TD>");
            }
            else
            {
                sb.Append("<TD COLSPAN=\"");
                sb.Append(getChildCount());
                sb.Append("\">");
            }

            sb.Append(getID());
            sb.Append(" ");
            sb.Append(_labeling);
            if (_final_flag)
            {
                sb.Append("!");
            }
            sb.Append("</TD></TR>");
            if (getChildCount() > 0)
            {
                sb.Append("<TR>");
                //for (child_iterator it = children_begin(); it != children_end(); ++it)
                for (SafraTreeNode cit = children_begin(); cit != children_end(); cit = cit.increment())
                {
                    sb.Append("<TD>");
                    sb.Append(cit.toHTML());
                    sb.Append("</TD>");
                }
                sb.Append("</TR>");
            }
            sb.Append("</TABLE>");
            return sb.ToString();
        }
    }
}
