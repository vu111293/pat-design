using System.Diagnostics;
using PAT.Common.Classes.LTL2DRA.common;

namespace PAT.Common.Classes.LTL2DRA
{
    public abstract class StateInterface
    {
        public abstract RabinSignature generateAcceptance();
        public abstract void generateAcceptance(RabinSignature acceptance);
        public abstract void generateAcceptance(AcceptanceForState acceptance);

        public static bool operator <(StateInterface one, StateInterface other)
        {
            return false;
        }

        public static bool operator >(StateInterface one, StateInterface other)
        {
            return false;
        }

        public abstract void hashCode(HashFunction hashfunction);

        public abstract string toHTML();
    }

    public class SafraTree : StateInterface
    {
        /** The maximum number of nodex */
        private int MAX_NODES;
        /** An array to store the nodes */
        private SafraTreeNode[] _nodes;

        /** 
        * Constructor.
        * @param N the maximum number of nodes.
        */
        public SafraTree(int N)
        {
            if (N == 0)
            {
                MAX_NODES = 1;
            }
            else
            {
                MAX_NODES = N;
            }


            _nodes = new SafraTreeNode[MAX_NODES];
            //for (int i = 0; i < MAX_NODES; i++)
            //{
            //    _nodes[i] = null;
            //}

            // create root-node
            newNode(0);
        }

        /** Copy constructor. */
        public SafraTree(SafraTree other)
        {

            MAX_NODES = other.MAX_NODES;

            _nodes = new SafraTreeNode[MAX_NODES];
            for (int i = 0; i < MAX_NODES; i++)
            {
                //_nodes[i] = null;
                if (other._nodes[i] != null)
                {
                    _nodes[i] = newNode(i);
                    _nodes[i]._labeling.Assign(other._nodes[i].getLabeling());
                    _nodes[i].setFinalFlag(other._nodes[i].hasFinalFlag());
                }
            }

            copySubTree(_nodes[0], other._nodes[0]);
        }



        /** Get the root node of the tree. */
        public SafraTreeNode getRootNode()
        {
            return _nodes[0];
        }


        /** Create a new node. The name is the next free node name. */
        public SafraTreeNode newNode()
        {
            for (int i = 0; i < MAX_NODES; i++)
            {
                if (_nodes[i] == null)
                {
                    return newNode(i);
                }
            }
            return null;
        }

        /** Create a new node with name <i>id</i>. */
        public SafraTreeNode newNode(int id)
        {
            Debug.Assert(id < MAX_NODES);
            Debug.Assert(_nodes[id] == null);

            _nodes[id] = new SafraTreeNode(id);
            return _nodes[id];
        }

        /** 
 * Remove a SafraTreeNode from the tree, 
 * the node can have no children.
 */
        public void remove(SafraTreeNode node)
        {
            Debug.Assert(_nodes[node.getID()] == node);
            remove(node.getID());
        }

        /** 
        * Remove the SafraTreeNode <i>id</i> from the tree,
        * the node can have no children.
        */
        public void remove(int id)
        {
            Debug.Assert(id < MAX_NODES);
            _nodes[id].removeFromTree();
            //delete _nodes[id];
            _nodes[id] = null;
        }
        /**
 * Remove all children of the SafraTreeNode <i>id</i>.
 */
        public void removeAllChildren(int id)
        {
            Debug.Assert(id < MAX_NODES);

            SafraTreeNode n = _nodes[id];
            SafraTreeNode child;
            while ((child = n.getOldestChild()) != null)
            {
                removeAllChildren(child.getID());
                remove(child.getID());
            }
        }

        /** 
         * Walk the tree post-order, calling the function 
         * void visit(SafraTree& tree, SafraTreeNode *node) 
         * in the SafraTreeVisitor on each node.
         */

        public void walkTreePostOrder(SafraTreeVisitor visitor)
        {
            SafraTreeWalker walker = new SafraTreeWalker(visitor);
            walker.walkTreePostOrder(this);
        }


        /** 
         * Walk the subtree rooted under node *top post-order, 
         * calling the function void visit(SafraTree& tree, SafraTreeNode *node) 
         * in the SafraTreeVisitor on each node.
         */

        public void walkSubTreePostOrder(SafraTreeVisitor visitor, SafraTreeNode top)
        {
            SafraTreeWalker walker = new SafraTreeWalker(visitor);
            walker.walkSubTreePostOrder(this, top);
        }

        /** 
         * Walk the subtree rooted under node *top (only the children, not *top itself) 
         * post-order, calling the function void visit(SafraTree& tree, SafraTreeNode *node) 
         * in the SafraTreeVisitor on each node.
         */
        public void walkChildrenPostOrder(SafraTreeVisitor visitor, SafraTreeNode top)
        {

            SafraTreeWalker walker = new SafraTreeWalker(visitor);
            // = don't visit top
            walker.walkSubTreePostOrder(this, top, false);
        }

        /**
 * Calculate the height of the tree.
 */
        public int treeHeight()
        {
            if (getRootNode() != null)
            {
                return getRootNode().treeHeight();
            }

            return 0;
        }


        /**
   * Calculate the width of the tree.
   */
        public int treeWidth()
        {
            if (getRootNode() != null)
            {
                return getRootNode().treeWidth();
            }

            return 0;
        }


        /**
         * Equality operator.
         */
        public static bool operator ==(SafraTree one, SafraTree other)
        {
            bool? val = Ultility.NullCheck(one, other);
            if (val != null)
            {
                return val.Value;
            }

            if (other.MAX_NODES != one.MAX_NODES)
            {
                return false;
            }

            for (int i = 0; i < one.MAX_NODES; i++)
            {
                if (one._nodes[i] == null)
                {
                    if (other._nodes[i] != null)
                    {
                        return false;
                    }
                }
                else
                {
                    if (other._nodes[i] == null)
                    {
                        return false;
                    }

                    if (!(one._nodes[i] == other._nodes[i]))
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        /**
         * Equality operator.
         */
        public static bool operator !=(SafraTree one, SafraTree other)
        {
            return !(one == other);
        }


        /**
         * Checks equality when ignoring the node names.
         */
        public bool structural_equal_to(SafraTree other)
        {
            if (other.MAX_NODES != MAX_NODES)
            {
                return false;
            }

            SafraTreeNode this_root = this.getRootNode();
            SafraTreeNode other_root = other.getRootNode();

            if (this_root == null || other_root == null)
            {
                // return true if both are 0
                return (this_root == other_root);
            }

            return this_root.structural_equal_to(other_root);
        }

        /**
 * Less-than operator when ignoring the node names.
 */
        public bool structural_less_than(SafraTree other)
        {
            if (other.MAX_NODES < MAX_NODES)
            {
                return true;
            }

            SafraTreeNode this_root = this.getRootNode();
            SafraTreeNode other_root = other.getRootNode();

            if (this_root == null)
            {
                if (other_root != null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                // this_root !=0 
                if (other_root == null)
                {
                    return false;
                }

                return this_root.structural_less_than(other_root);
            }
        }

        /**
 * Less-than operator
 */
        public static bool operator <(SafraTree one, SafraTree other)
        {
            if (one.MAX_NODES < other.MAX_NODES)
            {
                return true;
            }

            for (int i = 0; i < one.MAX_NODES; i++)
            {
                if (one._nodes[i] == null && other._nodes[i] == null)
                {
                    ;
                }
                else if (one._nodes[i] == null)
                {
                    return true;
                }
                else if (other._nodes[i] == null)
                {
                    return false;
                }
                else
                {
                    if (one._nodes[i] < other._nodes[i])
                    {
                        return true;
                    }
                    else if (one._nodes[i] == other._nodes[i])
                    {
                        ;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            return false;
        }
        public static bool operator >(SafraTree one, SafraTree other)
        {
            if (one == other || one < other)
            {
                return false;
            }
            return true;
        }

        /** Get the maximum number of nodes. */
        public int getNodeMax()
        {
            return MAX_NODES;
        }



        public SafraTreeNode this[int i]
        {
            get { return this._nodes[i]; }
            set { this._nodes[i] = value; }
        }

        /** Returns a string representation of the SafraTree */
        public override string ToString()
        {
            if (this.getRootNode() == null)
            {
                return "<empty>" + "\r\n";
            }
            else
            {
                return this.printSubTree(0, getRootNode());
            }
        }


        /**
 * Calculate a hash value using HashFunction
 * @param hashfunction the HashFunction
 * @param only_structure ignore the nameing of the nodes
 */
        public override void hashCode(HashFunction hashfunction) //only_structure=false
        {
            hashCode(hashfunction, false);
        }

        public void hashCode(HashFunction hashfunction, bool only_structure) //only_structure=false
        {
            SafraTreeNode root = getRootNode();

            if (root != null)
            {
                root.hashCode(hashfunction, only_structure);
            }
        }

        public override int GetHashCode()
        {
            StdHashFunction hash = new StdHashFunction();
            hashCode(hash);
            int value = hash.value();
            return value;
        }


        public override bool Equals(object obj)
        {
            return this.GetHashCode() == obj.GetHashCode();
        }


        /**
         * Generate the appropriate acceptance signature for Rabin Acceptance for this tree  
         */
        public override void generateAcceptance(AcceptanceForState acceptance)
        {
            for (int i = 0; i < getNodeMax(); i++)
            {
                SafraTreeNode stn = this[i];
                if (stn == null)
                {
                    acceptance.addTo_U(i);
                }
                else
                {
                    if (stn.hasFinalFlag())
                    {
                        acceptance.addTo_L(i);
                    }
                }
            }
        }

        public override void generateAcceptance(RabinSignature acceptance)
        {
            acceptance.setSize(getNodeMax());
            for (int i = 0; i < getNodeMax(); i++)
            {
                SafraTreeNode stn = this[i];
                if (stn == null)
                {
                    acceptance.setColor(i, RabinColor.RABIN_RED);
                }
                else
                {
                    if (stn.hasFinalFlag())
                    {
                        acceptance.setColor(i, RabinColor.RABIN_GREEN);
                    }
                    else
                    {
                        acceptance.setColor(i, RabinColor.RABIN_WHITE);
                    }
                }
            }
        }

        public override RabinSignature generateAcceptance()
        {
            RabinSignature s = new RabinSignature(getNodeMax());
            generateAcceptance(s);
            return s;
        }

        /**
  * Copy the subtree (the children) of *other
  * to *top, becoming the children of *top
  */
        public void copySubTree(SafraTreeNode top, SafraTreeNode other)
        {
            if (other == null)
            {
                return;
            }

            //for (SafraTreeNode::child_iterator it=other->children_begin();it!=other->children_end();++it) {
            //  SafraTreeNode *n=_nodes[(*it)->getID()], *n_o=*it;
            //  top->addAsYoungestChild(n);
            //  copySubTree(n, n_o);
            //}
            SafraTreeNode it = other.children_begin();
            while (it != other.children_end())
            {
                SafraTreeNode n = _nodes[it.getID()];
                top.addAsYoungestChild(n);
                copySubTree(n, it);

                it = it.increment();
            }
        }

        /**
  * Print the subtree rooted at node *top to the output stream
  * @param out the output stream
  * @param prefix the number of spaces ' ' in front of each node
  * @param top the current tree sub root
  */
        public string printSubTree(int prefix, SafraTreeNode top)
        {
            string returnString = "";
            for (int i = 0; i < prefix; i++)
            {
                returnString += " ";
            }
            returnString += top.ToString() + "\r\n";

            //for (SafraTreeNode::child_iterator it=top->children_begin();
            // it!=top->children_end();
            // ++it) {
            //  printSubTree(out, prefix+1, *it);
            //}

            SafraTreeNode it = top.children_begin();
            while (it != top.children_end())
            {
                printSubTree(prefix + 1, it);

                it = it.increment();
            }
            return returnString;
        }

        //        /** Print the SafraTree on an output stream. */
        //friend std::ostream& operator<<(std::ostream& out, 
        //                SafraTree& st) {
        //  if (st.getRootNode()==0) {
        //    out << "<empty>" << std::endl;
        //  } else {
        //    st.printSubTree(out, 0, st.getRootNode());
        //  }
        //  return out;
        //}

        ///** Returns a string representation of the SafraTree */
        //std::string toString() {
        //  std::ostringstream buf;
        //  buf << *this;
        //  return buf.str();
        //}

        /** Returns a string representation in HTML of the SafraTree */
        public override string toHTML()
        {
            if (getRootNode() == null)
            {
                return "<TABLE><TR><TD>[empty]</TD></TR></TABLE>";
            }
            else
            {

                return getRootNode().toHTML();
            }
        }

    }
}
