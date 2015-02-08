using System.Diagnostics;
using ltl2ba;
using PAT.Common.Classes.Ultility;


namespace PAT.Common.Classes.BA
{
    public class LivenessChecking
    {
        /// <summary>
        /// Given a Buchi automaton, return true if and only if the automaton is a liveness property.
        /// A Buchi automaton is a liveness property if and only if its close automata accepts all words.
        /// </summary>
        /// <param name="ba"></param>
        /// <returns></returns>
        public static bool isLiveness(BuchiAutomata ba)
        {
            //return true;
            if (ba.SyntacticSafety)
            {
                return false;
            }

            foreach (string state in ba.States)
            {
                if(!state.EndsWith(Constants.ACCEPT_STATE))
                {
                    return true;
                }
            }

            return false;
        }

        public static bool HasSyntaxSafeOperator(ltl2ba.Node CurrentNode)
        {
            if (CurrentNode == null)
            {
                return true;
            }

            bool currentSafe = true;
            switch ((Operator)CurrentNode.ntyp)
            {
                case Operator.ALWAYS:
                    break;
                case Operator.AND:
                    break;
                case Operator.EQUIV:
                    break;
                case Operator.EVENTUALLY:
                    currentSafe = false;
                    break;
                case Operator.FALSE:
                    break;
                case Operator.IMPLIES:
                    if (CurrentNode.lft != null)
                    {
                        
                        if ((Operator)CurrentNode.lft.ntyp != Operator.PREDICATE)
                        {
                            currentSafe = false;
                        }
                    }
                    break;
                case Operator.NOT:
                    Debug.Assert(CurrentNode.rgt == null);

                    if (CurrentNode.lft != null)
                    {
                        if ((Operator)CurrentNode.lft.ntyp != Operator.PREDICATE)
                        {
                            currentSafe = false;
                        }
                    }
                    break;
                case Operator.OR:
                    break;
                case Operator.TRUE:
                    break;
                case Operator.U_OPER:
                    currentSafe = false;
                    break;
                case Operator.V_OPER:                   
                    break;
                case Operator.NEXT:
                    break;
                case Operator.PREDICATE:
                    break;
                default:
                    break;
            }
            
            if(currentSafe)
            {
                return HasSyntaxSafeOperator(CurrentNode.lft) && HasSyntaxSafeOperator(CurrentNode.rgt);
            }           
            return false;
        }
    }
}
