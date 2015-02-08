using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using ltl2ba;

namespace PAT.Common.Classes.LTL2DRA
{
    public class LTLPrefixParser
    {

        /**
 * Parse the formula and return an LTLFormula_ptr
 * @param formula the formula string
 * @param predefined_apset if specified, use this APSet and don't 
 *                         add new atomic propositions
 * @return the LTLFormula_ptr
 */
        public static LTLFormula parse(ltl2ba.Node LTLHeadNode)
        {
            return parse(LTLHeadNode, null);
        }

        public static LTLFormula parse(ltl2ba.Node LTLHeadNode, APSet predefined_apset)
        {
            //    boost::char_separator<char> sep(" ");
            //ltl_seperator sep; tokenizer tokens(formula, sep);

            APSet apset = new APSet();

            //tokenizer::iterator it = tokens.begin();

            //LTLNode ltl = parse(apset, predefined_apset);

            //if (it != tokens.end())
            //{
            //    THROW_EXCEPTION(Exception, "Unexpected character(s) at end of LTL formula: '" + *it + "'");
            //}


            LTLNode ltl = TranslateLTL(LTLHeadNode, apset, predefined_apset);

            APSet apset_ = predefined_apset ?? apset;

            return new LTLFormula(ltl, apset_);
        }

        private static LTLNode TranslateLTL(ltl2ba.Node CurrentNode, APSet apset, APSet predefined_apset)
        {
            if (CurrentNode == null)
            {
                return null;
            }

            type_t nodeType = type_t.T_TRUE;
            switch ((Operator) CurrentNode.ntyp)
            {
                case Operator.ALWAYS:
                    nodeType = type_t.T_GLOBALLY;
                    break;
                case Operator.AND:
                    nodeType = type_t.T_AND;
                    break;
                case Operator.EQUIV:
                    nodeType = type_t.T_EQUIV;
                    break;
                case Operator.EVENTUALLY:
                    nodeType = type_t.T_FINALLY;
                    break;
                case Operator.FALSE:
                    nodeType = type_t.T_FALSE;
                    break;
                case Operator.IMPLIES:
                    nodeType = type_t.T_IMPLICATE;
                    break;
                case Operator.NOT:
                    nodeType = type_t.T_NOT;
                    break;
                case Operator.OR:
                    nodeType = type_t.T_OR;
                    break;
                case Operator.TRUE:
                    nodeType = type_t.T_TRUE;
                    break;
                case Operator.U_OPER:
                    nodeType = type_t.T_UNTIL;
                    break;
                case Operator.V_OPER:
                    nodeType = type_t.T_RELEASE;
                    break;
                case Operator.NEXT:
                    nodeType = type_t.T_NEXTSTEP;
                    break;
                case Operator.PREDICATE:
                    nodeType = type_t.T_AP;

                    string ap = CurrentNode.sym.name;
                    char ch = ap[0];

                    if (ch == '"')
                    {
                        //	std::cerr << ap << std::endl;
                        Debug.Assert(ap[ap.Length - 1] == '"'); // last char is "

                        if (ap.Length <= 2)
                        {
                            // empty ap!
                            throw new Exception("LTL-Parse-Error: empty quoted string");
                        }


                        ap = ap.Substring(1, ap.Length - 2); // cut quotes
                    }
                    else if ((ch >= 'a' && ch <= 'z') || (ch >= 'A' && ch <= 'Z'))
                    {
                        // nop
                    }
                    else
                    {
                        throw new Exception("LTL-Parse-Error");
                    }

                    int ap_i; // the AP index

                    if (predefined_apset != null)
                    {
                        if ((ap_i = predefined_apset.find(ap)) != -1)
                        {
                            return new LTLNode(ap_i);
                        }
                        else
                        {
                            // not found in predefined APSet!
                            //std::cerr << "[" << (int)s[2] << "]" << std::endl;
                            throw new Exception("Can't parse formula with this APSet!");
                        }
                    }
                    else
                    {
                        if ((ap_i = apset.find(ap)) != -1)
                        {
                            // AP exists already
                            return new LTLNode(ap_i);
                        }
                        else
                        {
                            // create new AP
                            ap_i = apset.addAP(ap);
                            return new LTLNode(ap_i);
                        }
                    }
                    break;


                default:
                    break;

            }



            LTLNode newNode = new LTLNode(nodeType, TranslateLTL(CurrentNode.lft, apset, predefined_apset),
                                          TranslateLTL(CurrentNode.rgt, apset, predefined_apset));


            return newNode;
        }


        //   /**
        // * Functor for boost::tokenizer
        // */
        //public class ltl_seperator {

        //  void reset() {}

        //  //template <typename InputIterator, typename Token>
        //  bool operator()(InputIterator next, InputIterator end, Token tok) {

        //    tok.clear();

        //    while (next != end &&
        //       *next == ' ') {
        //  ++next;  // skip whitespace
        //    }

        //    if (next == end) {return false;}

        //    if (*next=='"') {
        //  tok+=*next;
        //  ++next;

        //  // start of quoted string
        //  while (next != end &&
        //         *next != '"') {
        //    tok+=*next;
        //    ++next;
        //  }

        //  if (next == end) {
        //    // no matching end of quote!
        //    THROW_EXCEPTION(Exception, "Missing final quote!");
        //  }

        //  tok+=*next; // eat final "
        //  ++next; 
        //  return true;
        //    } else {
        //  // read until end or first whitespace
        //  while (next != end &&
        //         *next != ' ') {
        //    tok+=*next;
        //    ++next;
        //  }

        //      return true;
        //    }
        //  }
        //}

  //      public static LTLNode LTLNode_p()
  //      {
  //          return null;
  //      }

  //      private static int tl_yychar = 0;
  //      private static LTLNode parse(APSet apset,APSet predefined_apset) //(iterator iterator, tokenizer tokenizer, APSet apset,APSet predefined_apset) 
  //{
      
  // //if (iterator==tokenizer.end()) {
  //  //  THROW_EXCEPTION(Exception, "LTL-Parse-Exception!");
  //  //}

  //  //std::string s=*iterator; 
  //  //++iterator;

  //  //if (s.length()==0) {
  //  //  THROW_EXCEPTION(Exception, "LTL-Parse-Exception!");
  //  //}

  //  //    std::cerr << s << " ";
  //          string s = tl_yychar;

  //   char ch=s[0];
  //  if (s.Length==1) { 
  //   switch (ch) {
  //    case 't':
  //  return new LTLNode(type_t.T_TRUE, LTLNode_p(), LTLNode_p());
  //    case 'f':
  //  return new LTLNode(type_t.T_FALSE, LTLNode_p(), LTLNode_p());
  //    case '!': {
  //  LTLNode left=parse(iterator, tokenizer, apset, predefined_apset);
  //  return new LTLNode(type_t.T_NOT, left);
  //    }			

  //    case '|': {
  //  LTLNode left=parse(iterator, tokenizer, apset, predefined_apset);
  //  LTLNode right=parse(iterator, tokenizer, apset, predefined_apset);
  //  return new LTLNode(type_t.T_OR, left, right);
  //    }			

  //    case '&': {
  //  LTLNode left=parse(iterator, tokenizer, apset, predefined_apset);
  //  LTLNode right=parse(iterator, tokenizer, apset, predefined_apset);
  //  return new LTLNode(type_t.T_AND, left, right);
  //    }			

  //    case '^': {
  //  LTLNode left=parse(iterator, tokenizer, apset, predefined_apset);
  //  LTLNode right=parse(iterator, tokenizer, apset, predefined_apset);
  //  return new LTLNode(type_t.T_XOR, left, right);
  //    }			

  //    case 'i': {
  //  LTLNode left=parse(iterator, tokenizer, apset, predefined_apset);
  //  LTLNode right=parse(iterator, tokenizer, apset, predefined_apset);
  //  return new LTLNode(type_t.T_IMPLICATE, left, right);
  //    }			

  //    case 'e': {
  //  LTLNode left=parse(iterator, tokenizer, apset, predefined_apset);
  //  LTLNode right=parse(iterator, tokenizer, apset, predefined_apset);
  //  return new LTLNode(type_t.T_EQUIV, left, right);
  //    }			

  //    case 'X': {
  //  LTLNode left=parse(iterator, tokenizer, apset, predefined_apset);
  //  return new LTLNode(type_t.T_NEXTSTEP, left);
  //    }			

  //    case 'G': {
  //  LTLNode left=parse(iterator, tokenizer, apset, predefined_apset);
  //  return new LTLNode(type_t.T_GLOBALLY, left);
  //    }			

  //    case 'F': {
  //  LTLNode left=parse(iterator, tokenizer, apset, predefined_apset);
  //  return new LTLNode(type_t.T_FINALLY, left);
  //    }			

  //    case 'U': {
  //  LTLNode left=parse(iterator, tokenizer, apset, predefined_apset);
  //  LTLNode right=parse(iterator, tokenizer, apset, predefined_apset);
  //  return new LTLNode(type_t.T_UNTIL, left, right);
  //    }			

  //    case 'V': {
  //  LTLNode left=parse(iterator, tokenizer, apset, predefined_apset);
  //  LTLNode right=parse(iterator, tokenizer, apset, predefined_apset);
  //  return new LTLNode(type_t.T_RELEASE, left, right);
  //    }			

  //    case 'W': {
  //  LTLNode left=parse(iterator, tokenizer, apset, predefined_apset);
  //  LTLNode right=parse(iterator, tokenizer, apset, predefined_apset);
  //  return new LTLNode(type_t.T_WEAKUNTIL, left, right);
  //    }			
  //    }
  //  }

  //  // no operator =>
  //  // possible atomic proposition
    
  //  string ap=s;
    
  //  if (ch=='"') {
  //    //	std::cerr << ap << std::endl;
  //    Debug.Assert(ap[ap.Length-1]=='"'); // last char is "
      
  //    if (ap.Length <=2) {
  //  // empty ap!
  //  throw new Exception("LTL-Parse-Error: empty quoted string");
  //    }

      
  //    ap=ap.Substring(1,ap.Length-2); // cut quotes
  //  } else if ((ch>='a' && ch<='z') ||
  //         (ch>='A' && ch<='Z')) {
  //    ;  // nop
  //  } else {
  //    throw new Exception("LTL-Parse-Error");
  //  }
    
  //  int ap_i; // the AP index
    
  //  if (predefined_apset!=null) {
  //    if ((ap_i=predefined_apset.find(ap))!=-1) {
  //  return new LTLNode(ap_i);
  //    } else {
  //  // not found in predefined APSet!
  //  //std::cerr << "[" << (int)s[2] << "]" << std::endl;
  //  throw new Exception("Can't parse formula with this APSet!");
  //    }
  //  } else {
  //    if ((ap_i=apset.find(ap))!=-1) {
  //  // AP exists already
  //  return new LTLNode(ap_i);
  //    } else {
  //  // create new AP
  //  ap_i=apset.addAP(ap);
  //  return new LTLNode(ap_i);
  //    }
  //  }
  //  Debug.Assert(false);  // Unreachable
  //}

    }
}
