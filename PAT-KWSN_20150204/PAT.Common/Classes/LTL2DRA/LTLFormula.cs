using System;
using PAT.Common.Classes.LTL2DRA.exception;

namespace PAT.Common.Classes.LTL2DRA
{
    public enum type_t
    {
        T_AP,
        T_TRUE,
        T_FALSE,

        T_NOT,
        T_AND,
        T_OR,
        T_XOR,
        T_IMPLICATE,
        T_EQUIV,

        T_NEXTSTEP,

        T_GLOBALLY,
        T_FINALLY,

        T_UNTIL,
        T_WEAKUNTIL,

        T_RELEASE,
        T_WEAKRELEASE,

        T_BEFORE
    }

    /** A node in the syntax tree of an LTL formula. Memory management is
 * automatic using boost::shared_ptr */
    public class LTLNode
    {
        /** Operator type */
        private type_t _type;

        /** The left child */
        private LTLNode _left;

        /** The right child */
        private LTLNode _right;

        /** Index of AP (when type==T_AP)*/
        private int _ap;

        /** Constructor for a node containing an atomic proposition (index into APSet) */
        public LTLNode(int ap)
        {
            _type = type_t.T_AP;
            _left = null;
            _right = null;
            _ap = ap;
        }
        public LTLNode(type_t type, LTLNode left) : this(type, left, null)
        {

        }

        /** Constructor for an operator of type */
        public LTLNode(type_t type, LTLNode left, LTLNode right)
        {
            _type = type;
            _left = left;
            _right = right;
        }

        /** Get the type for the node */
        public type_t getType() { return _type; }

        /** Get the left child node */
        public LTLNode getLeft() { return _left; }

        /** Get the right child node */
        public LTLNode getRight() { return _right; }

        /** Get the index of the AP (when type==T_AP)*/
        public int getAP() { return _ap; }

        /** Deep copy of the node */
        public LTLNode copy()
        {
            if (_type == type_t.T_AP)
            {
                return new LTLNode(_ap);
            }
            else if (_type == type_t.T_TRUE || _type == type_t.T_FALSE)
            {
                return new LTLNode(_type, null);
            }
            else if (_right == null)
            {
                return new LTLNode(_type, _left.copy());
            }
            else
            {
                return new LTLNode(_type, _left.copy(), _right.copy());
            }
        }

        /**
         * Generate formula in positive normal form,
         * with only TRUE, FALSE, AND, OR, X, U, V, G, F as operators
         */
        public LTLNode toPNF()
        {
            return toPNF(false);
        }
        public LTLNode toPNF(bool negate)
        {
            switch (_type)
            {
                case type_t.T_XOR:
                    throw new Exception("XOR not yet supported!");
                case type_t.T_IMPLICATE:
                    {
                        if (negate)
                        {
                            // ! (a->b) = !(!a | b) = (a & ! b)
                            return new LTLNode(type_t.T_AND, getLeft().toPNF(), getRight().toPNF(true));
                        }
                        else
                        {
                            // (a->b) = (!a | b)
                            return new LTLNode(type_t.T_OR, getLeft().toPNF(true), getRight().toPNF());
                        }
                        break;
                    }
                case type_t.T_EQUIV:
                    {
                        if (negate)
                        {
                            // ! (a<->b) == (a & !b) || (!a & b)
                            return new
                                LTLNode(type_t.T_OR,
                                new LTLNode(type_t.T_AND, getLeft().toPNF(), getRight().toPNF(true)),
                                new LTLNode(type_t.T_AND, getLeft().toPNF(true), getRight().toPNF()));
                        }
                        else
                        {
                            // (a<->b) = (!a && !b) || (a && b)
                            return new LTLNode(type_t.T_OR,
                                               new LTLNode(type_t.T_AND, getLeft().toPNF(true), getRight().toPNF(true)),
                                               new LTLNode(type_t.T_AND, getLeft().toPNF(), getRight().toPNF()));
                        }
                    }
                case type_t.T_NOT:
                    if (negate)
                    {
                        // double negation
                        return getLeft().toPNF();////////////////////?? shouldn't be getRight().toPNF()?            !(!a) = a
                    }
                    else
                    {
                        if (getLeft().getType() == type_t.T_AP)
                        {
                            // we are in front of an AP, negation is ok
                            return new LTLNode(type_t.T_NOT, getLeft());
                        }
                        else
                        {
                            return getLeft().toPNF(true);
                        }
                    }
                case type_t.T_AP:
                    if (negate)
                    {
                        return new LTLNode(type_t.T_NOT, new LTLNode(_ap));
                    }
                    else
                    {
                        return new LTLNode(_ap);
                    }
                case type_t.T_TRUE:
                    if (negate)
                    {
                        return new LTLNode(type_t.T_FALSE, null);
                    }
                    else
                    {
                        return new LTLNode(type_t.T_TRUE, null);
                    }
                case type_t.T_FALSE:
                    if (negate)
                    {
                        return new LTLNode(type_t.T_TRUE, null);
                    }
                    else
                    {
                        return new LTLNode(type_t.T_FALSE, null);
                    }
                case type_t.T_AND:
                    // ! (a & b) = (!a | !b)
                    if (negate)
                    {
                        return new LTLNode(type_t.T_OR, getLeft().toPNF(true), getRight().toPNF(true));
                    }
                    else
                    {
                        return new LTLNode(type_t.T_AND, getLeft().toPNF(), getRight().toPNF());
                    }
                case type_t.T_OR:
                    if (negate)
                    {
                        // ! (a | b) = (!a & !b)
                        return new LTLNode(type_t.T_AND, getLeft().toPNF(true), getRight().toPNF(true));
                    }
                    else
                    {
                        return new LTLNode(type_t.T_OR, getLeft().toPNF(), getRight().toPNF());
                    }
                case type_t.T_NEXTSTEP:
                    // ! (X a) = X (!a)
                    if (negate)
                    {
                        return new LTLNode(type_t.T_NEXTSTEP, getLeft().toPNF(true));
                    }
                    else
                    {
                        return new LTLNode(type_t.T_NEXTSTEP, getLeft().toPNF());
                    }
                case type_t.T_FINALLY:
                    // ! (F a) = G (!a) = f V !a
                    if (negate)
                    {
                        return new LTLNode(type_t.T_RELEASE, new LTLNode(type_t.T_FALSE, null), getLeft().toPNF(true));
                    }
                    else
                    { // F a = t U a
                        return new LTLNode(type_t.T_UNTIL, new LTLNode(type_t.T_TRUE, null), getLeft().toPNF());
                    }
                case type_t.T_GLOBALLY:
                    // ! (G a) = F (!a) = t U ! a
                    if (negate)
                    {
                        return new LTLNode(type_t.T_UNTIL, new LTLNode(type_t.T_TRUE, null), getLeft().toPNF(true));
                    }
                    else
                    { // f V a
                        return new LTLNode(type_t.T_RELEASE, new LTLNode(type_t.T_FALSE, null), getLeft().toPNF());
                    }
                case type_t.T_UNTIL:
                    // ! (a U b) = (!a V !b)
                    if (negate)
                    {
                        return new LTLNode(type_t.T_RELEASE, getLeft().toPNF(true), getRight().toPNF(true));
                    }
                    else
                    {
                        return new LTLNode(type_t.T_UNTIL, getLeft().toPNF(), getRight().toPNF());
                    }
                case type_t.T_RELEASE:
                    // ! (a R b) = (!a U !b)
                    if (negate)
                    {
                        return new LTLNode(type_t.T_UNTIL, getLeft().toPNF(true), getRight().toPNF(true));
                    }
                    else
                    {
                        return new LTLNode(type_t.T_RELEASE,
                                       getLeft().toPNF(),
                                       getRight().toPNF());
                    }
                case type_t.T_WEAKUNTIL:
                    {
                        LTLNode weak_until = new LTLNode(type_t.T_UNTIL, getLeft(),
                                                         new LTLNode(type_t.T_OR, getRight(),
                                                              new LTLNode(type_t.T_GLOBALLY, getLeft(), null)));
                        return weak_until.toPNF(negate);
                    }
                case type_t.T_WEAKRELEASE:
                    throw new Exception("Operator WEAKRELEASE not yet supported");
                case type_t.T_BEFORE:
                    throw new Exception("Operator BEFORE not yet supported");
            }

            throw new Exception("Implementation error");
        }

        /** 
       * Check if the formula rooted in this node is 
       * syntactically safe. Formula has to be in PNF.
       */
        public bool isSafe()
        {
            switch (getType())
            {
                case type_t.T_UNTIL:
                case type_t.T_FINALLY:
                    return false;
                case type_t.T_WEAKUNTIL:
                    throw new Exception("Operator WEAKUNTIL not yet supported");
                case type_t.T_WEAKRELEASE:
                    throw new Exception("Operator WEAKRELEASE not yet supported");
                case type_t.T_BEFORE:
                    throw new Exception("Operator BEFORE not yet supported");
                default:
                    // this level ok, check next level
                    break;
            }

            if (getLeft() != null)
            {
                if (!getLeft().isSafe())
                {
                    return false;
                }
            }

            if (getRight() != null)
            {
                if (!getRight().isSafe())
                {
                    return false;
                }
            }

            return true;
        }


        /** 
       * Check if the formula rooted in this node is 
       * syntactically co-safe. Formula has to be in PNF.
       */
        public bool isCoSafe()
        {
            switch (getType())
            {
                case type_t.T_RELEASE:
                case type_t.T_GLOBALLY:
                    return false;
                case type_t.T_WEAKUNTIL:
                    throw new Exception("Operator WEAKUNTIL not yet supported");
                case type_t.T_WEAKRELEASE:
                    throw new Exception("Operator WEAKRELEASE not yet supported");
                case type_t.T_BEFORE:
                    throw new Exception("Operator BEFORE not yet supported");
                default:
                    // this level ok, check next level
                    break;
            }

            if (getLeft() != null)
            {
                if (!getLeft().isCoSafe())
                {
                    return false;
                }
            }

            if (getRight() != null)
            {
                if (!getRight().isCoSafe())
                {
                    return false;
                }
            }

            return true;
        }


        /**
       * Recursively if the formula rooted at the current node is
       * NextStep-free.
       */
        public bool hasNextStep()
        {
            switch (getType())
            {
                case type_t.T_NEXTSTEP:
                    return true;
                default:
                    // this level ok, check next level
                    break;
            }

            if (getLeft() != null)
            {
                if (getLeft().hasNextStep())
                {
                    return true;
                }
            }

            if (getRight() != null)
            {
                if (getRight().hasNextStep())
                {
                    return true;
                }
            }

            return false;
        }


        /**
       * Generate formula in DNF. Formula has to have only 
       * AND, OR, TRUE, FALSE, ! as operators, and has to
       * be in PNF
       */
        public LTLNode toDNF()
        {
            switch (_type)
            {
                case type_t.T_TRUE:
                    return new LTLNode(type_t.T_TRUE, null);
                case type_t.T_FALSE:
                    return new LTLNode(type_t.T_FALSE, null);
                case type_t.T_NOT:
                    return new LTLNode(type_t.T_NOT, getLeft().toDNF());
                case type_t.T_AP:
                    return new LTLNode(getAP());
                case type_t.T_OR:
                    return new LTLNode(type_t.T_OR,
                               getLeft().toDNF(),
                               getRight().toDNF());
                case type_t.T_AND:
                    {
                        LTLNode left = getLeft().toDNF();
                        LTLNode right = getRight().toDNF();

                        if (left.getType() == type_t.T_OR)
                        {

                            LTLNode a = left.getLeft();
                            LTLNode b = left.getRight();

                            if (right.getType() == type_t.T_OR)
                            {

                                LTLNode c = right.getLeft();
                                LTLNode d = right.getRight();

                                LTLNode a_c = new LTLNode(type_t.T_AND, a, c),
                                  b_c = new LTLNode(type_t.T_AND, b, c),
                                  a_d = new LTLNode(type_t.T_AND, a, d),
                                  b_d = new LTLNode(type_t.T_AND, b, d);

                                return new LTLNode(type_t.T_OR,
                                           new LTLNode(type_t.T_OR, a_c, b_c).toDNF(),
                                           new LTLNode(type_t.T_OR, a_d, b_d).toDNF());
                            }
                            else
                            {
                                LTLNode
                                  a_c = new LTLNode(type_t.T_AND, a, right),
                                  b_c = new LTLNode(type_t.T_AND, b, right);

                                return new LTLNode(type_t.T_OR, a_c.toDNF(), b_c.toDNF());
                            }
                        }
                        else if (right.getType() == type_t.T_OR)
                        {
                            LTLNode a, b;
                            a = right.getLeft();
                            b = right.getRight();

                            LTLNode
                              a_c = new LTLNode(type_t.T_AND, left, a),
                              b_c = new LTLNode(type_t.T_AND, left, b);

                            return new LTLNode(type_t.T_OR, a_c.toDNF(), b_c.toDNF());
                        }
                        else
                        {
                            return new LTLNode(type_t.T_AND, left, right);
                        }
                    }
                default:
                    throw new Exception("Illegal operator for DNF!");
            }
        }


        /**
         * Calls Functor::operator(APMonom& m) for each monom of the formula. 
         * Formula has to be in DNF!
         */
        //template <class Functor>
        public void forEachMonom(EdgeCreator f)
        {
            if (getType() == type_t.T_OR)
            {
                getLeft().forEachMonom(f);
                getRight().forEachMonom(f);
            }
            else
            {
                APMonom m = this.toMonom();
                f.apply(m);
            }
        }

        /** Returns an APMonom representing the formula rooted at
         * this node. Formula has to be in DNF. */
        APMonom toMonom()
        {
            APMonom result = APMonom.TRUE;

            switch (getType())
            {
                case type_t.T_AND:
                    {
                        APMonom left = getLeft().toMonom();
                        APMonom right = getRight().toMonom();

                        result = left & right;
                        return result;
                    }
                case type_t.T_NOT:
                    switch (getLeft().getType())
                    {
                        case type_t.T_AP:
                            result.setValue(getLeft().getAP(), false);
                            return result;
                        case type_t.T_FALSE:
                            result = APMonom.TRUE;
                            return result;
                        case type_t.T_TRUE:
                            result = APMonom.FALSE;
                            return result;
                        default:
                            throw new Exception("Formula not in DNF!");
                    }
                case type_t.T_AP:
                    result.setValue(getAP(), true);
                    return result;
                case type_t.T_FALSE:
                    result = APMonom.FALSE;
                    return result;
                case type_t.T_TRUE:
                    result = APMonom.TRUE;
                    return result;
                default:
                    throw new Exception("Formula not in DNF!");
            }
        }



        ///** Helper function to get LTLNode_p from LTLNode* */
        //public LTLNode ptr(LTLNode p) {
        //  return LTLNode_p(p);
        //}

    }

    public class LTLFormula
    {
        /** The root node */
        public LTLNode _root;
        /** The underlying APSet */
        public APSet _apset;
        /**
         * Constructor
         * @param root the root node
         * @param apset the underlying APSet
         */
        public LTLFormula(LTLNode root, APSet apset)
        {
            _root = root;
            _apset = apset;
        }

        /** Copy constructor (not deep) */
        public LTLFormula(LTLFormula other)
        {
            _root = other._root;
            _apset = other._apset;
        }


        /** Deep copy */
        public LTLFormula copy()
        {
            LTLFormula copy_p = new LTLFormula(_root.copy(), _apset);
            return copy_p;
        }

        /** Get root node */
        public LTLNode getRootNode() { return _root; }

        /** Get APSet */
        public APSet getAPSet() { return _apset; }

        /**
         * Switch the APSet to another with the same number of APs.
         */
        public void switchAPSet(APSet new_apset)
        {
            if (new_apset.size() != _apset.size())
            {
                throw new IllegalArgumentException("New APSet has to have the same size as the old APSet!");
            }

            _apset = new_apset;
        }


        /**
         * Get a LTLFormula_ptr for the subformula rooted at subroot
         */
        public LTLFormula getSubFormula(LTLNode subroot)
        {
            LTLFormula sub = new LTLFormula(subroot, _apset);
            return sub;
        }

        /** Return an LTLFormula_ptr of the negation of this formula */
        public LTLFormula negate()
        {
            LTLNode new_root = new LTLNode(type_t.T_NOT, getRootNode());
            return new LTLFormula(new_root, _apset);
        }

        // TODO:
        //bool isInPNF() {

        /** Return true if the formula is syntactically safe (has to be in PNF) */
        public bool isSafe()
        {
            return getRootNode().isSafe();
        }

        /** Return true if the formula is syntactically co-safe (has to be in PNF) */
        public bool isCoSafe()
        {
            return getRootNode().isCoSafe();
        }

        /** Return true if the formula has at least one NextStep operator */
        public bool hasNextStep()
        {
            return getRootNode().hasNextStep();
        }

        /** Return this formula in PNF */
        public LTLFormula toPNF()
        {
            return new LTLFormula(getRootNode().toPNF(), _apset);
        }

        /** Return this formula in DNF (no temporal operators allowed) */
        public LTLFormula toDNF()
        {
            return new LTLFormula(getRootNode().toPNF().toDNF(), _apset);
        }

        /**
         * Calls Functor::operator(APMonom& m) for each monom of the formula. 
         * Formula has to be in DNF!
         */
        //template <class Functor>
        public void forEachMonom(EdgeCreator f)
        {
            getRootNode().forEachMonom(f);
        }

        ///** Print this formula in infix format (SPIN) to out */
        //void printInfix(std::ostream &out) {
        //  printInfix(getRootNode(), out);
        //}

        ///** Print this formula in prefix format to out */
        //void printPrefix(std::ostream &out) {
        //  printPrefix(getRootNode(), out);
        //}

        ///** Get a string with this formula in infix format (SPIN) */
        //std::string toStringInfix() {
        //  std::stringstream sstream;
        //  printInfix(sstream);
        //  return sstream.str();
        //}

        ///** Get a string with this formula in prefix format*/
        //std::string toStringPrefix() {
        //  std::stringstream sstream;
        //  printPrefix(sstream);
        //  return sstream.str();
        //}


        ///** Print in prefix format */
        //void printPrefix(LTLNode_p ltl,
        //          std::ostream& out) {
        //  switch (ltl->getType()) {
        //  case LTLNode::T_AP:
        //    out << _apset->getAP(ltl->getAP());
        //    break;
        //  case LTLNode::T_TRUE:
        //    out << "t";
        //    break;
        //  case LTLNode::T_FALSE:
        //    out << "f";
        //    break;
        //  case LTLNode::T_NOT:
        //    out << "! ";
        //    printPrefix(ltl->getLeft(), out);
        //    break;
        //  case LTLNode::T_AND:
        //    out << "& ";
        //    printPrefix(ltl->getLeft(), out);
        //    printPrefix(ltl->getRight(), out);
        //    break;
        //  case LTLNode::T_OR:
        //    out << "| ";
        //    printPrefix(ltl->getLeft(), out);
        //    printPrefix(ltl->getRight(), out);
        //    break;
        //  case LTLNode::T_XOR:
        //    out << "^ ";
        //    printPrefix(ltl->getLeft(), out);
        //    printPrefix(ltl->getRight(), out);
        //    break;
        //  case LTLNode::T_IMPLICATE:
        //    out << "i ";
        //    printPrefix(ltl->getLeft(), out);
        //    printPrefix(ltl->getRight(), out);
        //    break;
        //  case LTLNode::T_EQUIV:
        //    out << "e ";
        //    printPrefix(ltl->getLeft(), out);
        //    printPrefix(ltl->getRight(), out);
        //    break;
        //  case LTLNode::T_NEXTSTEP:
        //    out << "X ";
        //    printPrefix(ltl->getLeft(), out);
        //    break;
        //  case LTLNode::T_GLOBALLY:
        //    out << "G ";
        //    printPrefix(ltl->getLeft(), out);
        //    break;
        //  case LTLNode::T_FINALLY:
        //    out << "F ";
        //    printPrefix(ltl->getLeft(), out);
        //    break;
        //  case LTLNode::T_UNTIL:
        //    out << "U ";
        //    printPrefix(ltl->getLeft(), out);
        //    printPrefix(ltl->getRight(), out);
        //    break;
        //  case LTLNode::T_RELEASE:
        //    out << "V ";
        //    printPrefix(ltl->getLeft(), out);
        //    printPrefix(ltl->getRight(), out);
        //    break;
        //  case LTLNode::T_WEAKUNTIL:
        //    out << "| U ";
        //    printPrefix(ltl->getLeft(), out);
        //    printPrefix(ltl->getRight(), out);
        //    out << "G ";
        //    printPrefix(ltl->getLeft(), out);
        //    break;
        //  case LTLNode::T_WEAKRELEASE:
        //  case LTLNode::T_BEFORE:
        //    THROW_EXCEPTION(Exception, "Not yet implemented");      
        //  default:
        //    THROW_EXCEPTION(Exception, "Illegal operator");
        //  }
        //  out << " ";
        //}

        ///** Print in infix (SPIN) format */
        //void printInfix(LTLNode_p ltl,
        //        std::ostream& out) {
        //  switch (ltl->getType()) {
        //  case LTLNode::T_AP:
        //    out << _apset->getAP(ltl->getAP());
        //    break;
        //  case LTLNode::T_TRUE:
        //    out << "true";
        //    break;
        //  case LTLNode::T_FALSE:
        //    out << "false";
        //    break;
        //  case LTLNode::T_NOT:
        //    out << "! (";
        //    printInfix(ltl->getLeft(), out);
        //    out << ")";
        //    break;
        //  case LTLNode::T_AND:
        //    out << "(";
        //    printInfix(ltl->getLeft(), out);
        //    out << ") && (";
        //    printInfix(ltl->getRight(), out);
        //    out << ")";
        //    break;
        //  case LTLNode::T_OR:
        //    out << "(";
        //    printInfix(ltl->getLeft(), out);
        //    out << ") || (";
        //    printInfix(ltl->getRight(), out);
        //    out << ")";
        //    break;
        //  case LTLNode::T_XOR:
        //    out << "(";
        //    printInfix(ltl->getLeft(), out);
        //    out << ") ^ (";
        //    printInfix(ltl->getRight(), out);
        //    out << ")";
        //    break;
        //  case LTLNode::T_IMPLICATE:
        //    out << "(";
        //    printInfix(ltl->getLeft(), out);
        //    out << ") -> (";
        //    printInfix(ltl->getRight(), out);
        //    out << ")";
        //    break;
        //  case LTLNode::T_EQUIV:
        //    out << "(";
        //    printInfix(ltl->getLeft(), out);
        //    out << ") <-> (";
        //    printInfix(ltl->getRight(), out);
        //    out << ")";
        //    break;
        //  case LTLNode::T_NEXTSTEP:
        //    out << "X (";
        //    printInfix(ltl->getLeft(), out);
        //    out << ")";
        //    break;
        //  case LTLNode::T_GLOBALLY:
        //    out << "[] (";
        //    printInfix(ltl->getLeft(), out);
        //    out << ")";
        //    break;
        //  case LTLNode::T_FINALLY:
        //    out << "<> (";
        //    printInfix(ltl->getLeft(), out);
        //    out << ")";
        //    break;
        //  case LTLNode::T_UNTIL:
        //    out << "(";
        //    printInfix(ltl->getLeft(), out);
        //    out << ") U (";
        //    printInfix(ltl->getRight(), out);
        //    out << ")";
        //    break;

        //  case LTLNode::T_RELEASE:
        //    out << "(";
        //    printInfix(ltl->getLeft(), out);
        //    out << ") V (";
        //    printInfix(ltl->getRight(), out);
        //    out << ")";
        //    break;
        //  case LTLNode::T_WEAKUNTIL:
        //    out << "(";
        //    out << "(";
        //    printInfix(ltl->getLeft(), out);
        //    out << ") U ((";
        //    printInfix(ltl->getRight(), out);
        //    out << ") || ([] (";
        //    printInfix(ltl->getLeft(), out);
        //    out << "))))";
        //    break;
        //  case LTLNode::T_WEAKRELEASE:
        //  case LTLNode::T_BEFORE:
        //    // TODO: implement
        //    THROW_EXCEPTION(Exception, "Not yet implemented");      
        //  default:
        //    THROW_EXCEPTION(Exception, "Illegal operator");
        //  }
        //}
    }

    /** Functor to create the edges. */
    public class EdgeCreator
    {
        private int _from, _to;
        private NBA _nba;

        public EdgeCreator(int from, int to, NBA nba)
        {
            _from = from;
            _to = to;
            _nba = nba;
        }

        public void apply(APMonom m)
        {
            _nba.nba_i_addEdge(_from, m, _to);
        }
    }
}
