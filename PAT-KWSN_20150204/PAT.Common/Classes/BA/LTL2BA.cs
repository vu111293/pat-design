using System;
using System.Collections.Generic;
using Antlr.Runtime;
using ltl2ba;
using Microsoft.Msagl.Drawing;
using PAT.Common.Classes.LTL2DRA;
using PAT.Common.Classes.Ultility;
using Graph=Microsoft.Msagl.Drawing.Graph;
using Node=Microsoft.Msagl.Drawing.Node;


namespace PAT.Common.Classes.BA
{
    public sealed class LTL2BA
    {
        /// <summary> State number of the initial state returned by ltl2ba.</summary>
        private const int STATE_ID_INITIAL_STATE = -1;

        /// <summary>the unique proposition meaning <i>any letter</i> </summary>		
        private static readonly Proposition SIGMA_PROPOSITION = new Proposition("\u03A3", false);

        /// <summary>
        /// Constructs a new automaton for the given formula. 
        /// </summary>
        /// <param name="formula"></param>
        /// <param name="options"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static BuchiAutomata FormulaToBA(string formula, string options, IToken token)
        {
            if (formula.Length > 4095)
            {
                throw new ParsingException("LTL2BA limitation: formula must not be longer than 4095 characters", token);
            }

            //formula = formula.Replace(" tau ", Common.Classes.Ultility.Constants.TAU);

            try
            {
                ltl2ba.main.ConvertFormula(formula, options);
                BuchiAutomata BA = BuildBA();

                //check whether it is syntaically safe.
                ltl2ba.Node LTL = ParseLTL(formula, options, token);
                BA.SyntacticSafety = LivenessChecking.HasSyntaxSafeOperator(LTL);

                return BA;
            }
            catch (Exception ex)
            {
                throw new ParsingException("Invalid LTL formula: " + ex.Message, token);
            }
        }

        /// <summary>
        /// Constructs a new automaton for the given formula. 
        /// </summary>
        /// <param name="formula"></param>
        /// <param name="options"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static ltl2ba.Node ParseLTL(string formula, string options, IToken token)
        {
            if (formula.Length > 4095)
            {
                throw new ParsingException("LTL2BA limitation: formula must not be longer than 4095 characters", token);
            }

            try
            {
                return ltl2ba.main.ParseLTL(formula, options);
            }
            catch (Exception ex)
            {
                throw new ParsingException("Invalid LTL formula: " + ex.Message, token);
            }
        }


        /// <summary> Builds the output objects from the internal state list.</summary>
        /// <throws>  IllegalArgumentException if formula has invalid syntax  </throws>
        private static BuchiAutomata BuildBA()
        {
            List<string> initial = new List<string>(4);
            PAT.Common.Classes.DataStructure.Set<string> states = new PAT.Common.Classes.DataStructure.Set<string>();
            List<Transition> transitions = new List<Transition>();
            List<string> accept = new List<string>();

            Dictionary<String, string> statesHash = new Dictionary<String, string>();

            //in the following, all "System.IntPtr" values are actually pointers in C !
            //dummy node in the circular list of states
            BState root = ltl2ba.main.GetBstates();

            //int count = 0;
            //if there was no error and so we have states
            if (root != null)
            {
                int id = ltl2ba.main.GetSymtemID();
                string[] allLabels = new string[id];

                for (int i = 0; i < id; i++)
                {
                    allLabels[i] = ltl2ba.main.GetSystemString(i).Trim('"');
                }

                //iterate over state list
                for (BState pSourceState = root.nxt; pSourceState != ltl2ba.main.GetBstates(); pSourceState = pSourceState.nxt)
                {
                    
                    bool isInitial = (pSourceState.id == STATE_ID_INITIAL_STATE);

                    bool isFinal = pSourceState.final == ltl2ba.main.GetAccept();

                    string label = pSourceState.id + (isInitial ? Constants.INIT_STATE : "") + (isFinal ? Constants.ACCEPT_STATE : "");

                    string sourceState;

                    //take care that equal states are unique
                    if (statesHash.ContainsKey(label))
                    {
                        sourceState = statesHash[label];
                    }
                    else
                    {
                        sourceState = label; //new State(, isFinal, isInitial);
                        statesHash.Add(label, sourceState);

                        states.Add(sourceState);

                        if (isInitial)
                        {
                            initial.Add(sourceState);
                        }

                        if (isFinal)
                        {
                            accept.Add(sourceState);
                        }
                    }

                    BTrans troot = pSourceState.trans;

                    for (BTrans pTransition = troot.nxt; pTransition != troot; pTransition = pTransition.nxt)
                    {
                        BState pTargetState = pTransition.to;

                        isInitial = (pTargetState.id == STATE_ID_INITIAL_STATE);
                        isFinal = (pTargetState.final == ltl2ba.main.GetAccept());

                        label = pTargetState.id + (isInitial ? Constants.INIT_STATE : "") + (isFinal ? Constants.ACCEPT_STATE : "");

                        string targetState;

                        //take care that equal states are unique
                        if (statesHash.ContainsKey(label))
                        {
                            targetState = statesHash[label];
                        }
                        else
                        {
                            targetState = label; // new State(label, isFinal, isInitial);
                            statesHash.Add(label, targetState);
                            states.Add(targetState);

                            if (isInitial)
                            {
                                initial.Add(targetState);
                            }

                            if (isFinal)
                            {
                                accept.Add(targetState);
                            }
                        }

                        //it is hashset in java before
                        PAT.Common.Classes.DataStructure.Set<Proposition> labels = new PAT.Common.Classes.DataStructure.Set<Proposition>();

                        if (ltl2ba.main.BtransPos(pTransition) == 0 && ltl2ba.main.BtransNeg(pTransition) == 0)
                        {
                            // we have a "Sigma" edge
                            labels.Add(SIGMA_PROPOSITION);
                        }
                        else
                        {
                            for (int i = 0; i < allLabels.Length; i++)
                            {
                                if ((ltl2ba.main.BtransPos(pTransition) & (1 << i)) > 0)
                                {                                    
                                    labels.Add(new Proposition(allLabels[i], false));
                                }
                                if ((ltl2ba.main.BtransNeg(pTransition) & (1 << i)) > 0)
                                {
                                    
                                    Proposition l = new Proposition(allLabels[i], true);
                                    labels.Add(l);
                                }
                            }
                        }
                        
                        PersonComparer p = new PersonComparer();
                        labels.Sort(p);

                        Transition transition = new Transition(labels, sourceState, targetState);
                        transitions.Add(transition);
                    }
                }
            }
            else
            {
                throw new ArgumentException("invalid formula");
            }

            //fair.Add(accept);
            return new BuchiAutomata(initial, states.ToArray(), transitions.ToArray(), accept.ToArray());            
        }
        private static LTLNode TranslateLTL(ltl2ba.Node LTLHeadNode)
        {
            type_t type = new type_t();
            LTLNode node = new LTLNode(type, TranslateLTL(LTLHeadNode.lft), TranslateLTL(LTLHeadNode.rgt));
            
            return node;
        }
        public static Graph AutomatonToDot(BuchiAutomata BA)
        {
            Graph g = new Graph("graph");
            g.Directed = true;
            //g.MinNodeWidth = 0;
            //g.MinNodeHeight = 0;

            Dictionary<string, string> stateToNumber = new Dictionary<string, string>();

            for (int i = 0; i <  BA.States.Length; i++)
            {
                string state = BA.States[i];
            
                //dot += ("s" + stateToNumber[state] + " [" + (isInitial ? "shape=box" : "") + ((isInitial && isFinal) ? "," : "") + (isFinal ? "style=dotted" : "") + "];\n");
                //Node d = g.AddNode("s" + stateToNumber[state.ToString()]);

                string label = "s" + i;
                Node d = g.AddNode(label);
                stateToNumber.Add(state, label);

                if (BA.InitialStates.Contains(state)) //.isInitial
                {
                    Node temp = g.AddNode("init-" + label);

                    //temp.Attr.Shape = Shape.InvHouse;
                    temp.Attr.LineWidth = 0;
                    temp.Attr.Color = Color.White;
                    temp.LabelText = "";
                    g.AddEdge("init-" + label, label);
                    d.Attr.FillColor = Color.Gray;
                }

                if (state.EndsWith(Constants.ACCEPT_STATE))
                {
                    d.Attr.Shape = Shape.DoubleCircle;
                    //d.Attr.AddStyle(Style.Dotted);
                }
            }

            foreach (Transition transition in BA.Transitions)
            {
                //dot += ("s" + stateToNumber[transition.getSourceState()] + " -> " + "s" + stateToNumber[transition.getTargetState()] + " [label=\"" + transition.getLabels() + "\"]" + ";\n");
                g.AddEdge(stateToNumber[transition.FromState.ToString()], Ultility.Ultility.PPStringList(transition.labels), stateToNumber[transition.ToState.ToString()]);
            }

            return g;
        }
    }

  
    public class PersonComparer : IComparer<Proposition>
    {
        #region IComparer Members

        public int Compare(Proposition x, Proposition y)
        {
            return x.ToString().CompareTo(y.ToString());
        }

        #endregion
    }
}