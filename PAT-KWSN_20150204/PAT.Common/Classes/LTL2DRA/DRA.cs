using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Msagl.Drawing;
using PAT.Common.Classes.BA;
using PAT.Common.Classes.Expressions;
using PAT.Common.Classes.Expressions.ExpressionClass;
using PAT.Common.Classes.LTL2DRA.common;
using PAT.Common.Classes.Ultility;

namespace PAT.Common.Classes.LTL2DRA
{
    //APElement, EdgeContainerExplicit_APElement
    //Label, EdgeContainer
    public class DRA : DA
    {

        /** Return a string giving the type of the automaton. */
        private string typeID()
        {
            if (_isStreett)
            {
                return "DSA";
            }
            else
            {
                return "DRA";
            }
        }

        /** Marker, is this DRA considered as a Streett automaton? */
        private bool _isStreett;

        public DRA(APSet ap_set)
            : base(ap_set)
        {
            _isStreett = false;
        }


        /** Create a new instance of the automaton. */
        public override DA createInstance(APSet ap_set)
        {
            return new DRA(ap_set);
        }

        /** Make this automaton into an never accepting automaton */
        public void constructEmpty()
        {
            DA_State n = this.newState();
            setStartState(n);

            //for (APSet::element_iterator it_elem= DA<Label,EdgeContainer,RabinAcceptance>::getAPSet().all_elements_begin();it_elem!=DA<Label,EdgeContainer,RabinAcceptance>::getAPSet().all_elements_end();++it_elem) 
            APSet ap_set = getAPSet();
            for (int it_elem = ap_set.all_elements_begin(); it_elem != ap_set.all_elements_end(); ++it_elem)
            {
                APElement label = new APElement(it_elem);
                n.edges().addEdge(label, n);
            }
        }

        /**
        * Optimizes the acceptance condition.
        * This function may delete acceptance pairs,
        * which can invalidate iterators.
        */
        public void optimizeAcceptanceCondition()
        {

            int id = 0;
            //acceptance_pair_iterator it=this.acceptance().acceptance_pair_begin();
            while (id < this.acceptance()._acceptance_count) ////it!=this.acceptance().acceptance_pair_end()
            {
                // L = L \ U
                if (acceptance().getAcceptance_L(id).intersects(acceptance().getAcceptance_U(id)))
                {
                    BitSet L_minus_U = acceptance().getAcceptance_L(id);
                    L_minus_U.Minus(acceptance().getAcceptance_U(id));

                    acceptance().getAcceptance_L(id).Assign(L_minus_U);
                }

                // remove if L is empty
                if (this.acceptance().getAcceptance_L(id).isEmpty())
                {
                    // no state is in L(id) -> remove
                    this.acceptance().removeAcceptancePair(id);
                }
                else
                {
                    // increment iterator here so we can eventually delete
                    // acceptance pair without side effects
                    ++id;                    
                }

            }
        }

        /** Is this DRA considered as a Streett automaton? */
        public bool isStreett() { return _isStreett; }

        public void considerAsStreett()
        {
            considerAsStreett(true);
        }

        /** Consider this DRA as a Streett automaton. */
        public void considerAsStreett(bool flag) //=true
        {
            _isStreett = flag;
        }


        //trueloop_check=true,bool detailed_states=false
        public static DA calculateUnion(DRA dra1, DRA dra2, bool trueloop_check)
        {
            return calculateUnion(dra1, dra2, trueloop_check, false);
        }

        public static DA calculateUnion(DRA dra1, DRA dra2, bool trueloop_check, bool detailed_states)
        {
            if (dra1.isStreett() || dra2.isStreett())
            {
                throw new Exception("Can not calculate union for Streett automata");
            }

            return DAUnionAlgorithm.calculateUnion(dra1, dra2, trueloop_check, detailed_states);
        }

        //bool trueloop_check=true, bool detailed_states=false
        public static DA calculateUnionStuttered(DRA dra1, DRA dra2, StutterSensitivenessInformation stutter_information, bool trueloop_check, bool detailed_states)
        {
            if (dra1.isStreett() ||
                dra2.isStreett())
            {
                throw new Exception("Can not calculate union for Streett automata");
            }

            return DAUnionAlgorithm.calculateUnionStuttered(dra1, dra2, stutter_information, trueloop_check, detailed_states);
        }

        /**
 * Print the DA in DOT format to the output stream.
 * This functions expects that the DA is compact.
 * @param da_type a string specifying the type of automaton ("DRA", "DSA").
 * @param out the output stream 
 */
        public Graph AutomatonToDot()
        {

            Graph g = new Graph("graph");
            g.Directed = true;


            APSet ap_set = getAPSet();
            Dictionary<int, string> stateToNumber = new Dictionary<int, string>();

            for (int i_state = 0; i_state < this.size(); i_state++)
            {
                //out << "\"" << i_state << "\" [";

                Node d = formatStateForDOT(i_state, g);
                stateToNumber.Add(i_state, d.Id);
            }

            //out << "]\n"; // close parameters for state

            for (int i_state = 0; i_state < this.size(); i_state++)
            {
                // transitions

                DA_State cur_state = this.get(i_state);
                if (cur_state.hasOnlySelfLoop())
                {
                    // get first to-state, as all the to-states are the same
                    DA_State to = cur_state.edges().get(new APElement(ap_set.all_elements_begin()));

                    //out << "\"" << i_state << "\" -> \"" << to->getName();
                    //out << "\" [label=\" true\", color=blue]\n";

                    Edge edge = g.AddEdge(stateToNumber[i_state], "\u03A3", stateToNumber[to.getName()]);
                    //edge.Attr.Color = Color.Blue;

                }
                else
                {
                    //for (APSet::element_iterator el_it=ap_set.all_elements_begin();el_it!=ap_set.all_elements_end();++el_it) 
                    for (int el_it = ap_set.all_elements_begin(); el_it != ap_set.all_elements_end(); ++el_it)
                    {
                        APElement label = new APElement(el_it);
                        DA_State to_state = cur_state.edges().get(label);
                        int to_state_index = to_state.getName();
                        //out << "\"" << i_state << "\" -> \"" << to_state_index;
                        //out << "\" [label=\" " << label.toString(getAPSet(), false) << "\"]\n";


                        Edge edge = g.AddEdge(stateToNumber[i_state], label.toString(getAPSet(), false), stateToNumber[to_state_index]);
                        //edge.Attr.Color = Color.Blue;
                    }
                }
            }

            return g;
        }


        /** Output state label for DOT printing. 
         * @param out the output stream
         * @param state_index the state index
         */
        public Node formatStateForDOT(int state_index, Graph g)
        {
            DA_State cur_state = this.get(state_index);

            bool has_pos = false, has_neg = false;

            //std::ostringstream acc_sig;
            string acc_sig = "";

            for (int pair_index = 0; pair_index < this.acceptance().size(); pair_index++)
            {
                if (this.acceptance().isStateInAcceptance_L(pair_index, state_index))
                {
                    acc_sig += " +" + pair_index;
                    has_pos = true;
                }

                if (this.acceptance().isStateInAcceptance_U(pair_index, state_index))
                {
                    acc_sig += " -" + pair_index;
                    has_neg = true;
                }
            }

            string label = state_index + "\r\n" +acc_sig.Trim();


            //out << "label= \"" << state_index;
            //if (acc_sig.str().length()!=0) {
            //  out  << "\\n" << acc_sig.str();
            //}
            Node d = g.AddNode(label);

            //if (!has_pos && !has_neg)
            //{
            //    //out << ", shape=circle";
            //    d.Attr.Shape = Shape.DoubleCircle;
            //}
            //else
            //{
            //    //out << ", shape=box";
            //    d.Attr.Shape = Shape.Box;
            //}
            d.Attr.Shape = Shape.Box;

            if (this.getStartState() == cur_state)
            {
                ////out << ", style=filled, color=black, fillcolor=grey";
                //d.Attr.AddStyle(Style.Filled);
                //d.Attr.Color = Color.Black;
                //d.Attr.FillColor = Color.Gray;

                Node temp = g.AddNode("init-" + label);

                //temp.Attr.Shape = Shape.InvHouse;
                temp.Attr.LineWidth = 0;
                temp.Attr.Color = Color.White;
                temp.LabelText = "";
                g.AddEdge("init-" + label, label);
                d.Attr.FillColor = Color.Gray;
            }

            return d;
        }

        #region Newly added methods for the PAT usage

        public Dictionary<string, Expression> DeclarationDatabase;

        public List<int> MakeOneMove(int stateIndex, Valuation env, string evt)
        {
            DA_State state = this._index[stateIndex];
            List<int> returnList = new List<int>();

            if (state.hasOnlySelfLoop())
            {
                // get first to-state, as all the to-states are the same
                DA_State to = state.edges().get(new APElement(_ap_set.all_elements_begin()));
                returnList.Add(to.Index);
                return returnList;
            }

            //Transition[] trans = fromTransitions[state];

            //foreach (Transition tran in trans)
            for (int el_it = _ap_set.all_elements_begin(); el_it != _ap_set.all_elements_end(); ++el_it)
            {
                APElement label = new APElement(el_it);
                DA_State to_state = state.edges().get(label);
                int to_state_index = to_state.Index;

                //bool toAdd = true;
                //for (int i = 0; i < _ap_set.size(); i++)
                bool toAdd = true;
                for (int i = 0; i < _ap_set.size(); i++)
                {
                    ////If the transition is labelled with Sigma, there should not be any other labels. 
                    //if (label.IsSigmal)
                    //{
                    //    //if(!returnList.Contains(tran.ToState))
                    //    {
                    //        returnList.Add(tran.ToState);
                    //    }
                    //    break;
                    //}

                    string labelstring = _ap_set.getAP(i);
                    //If the labed is negated, e.g., !eat0.
                    if (!label.get(i))
                    {
                        if (!DeclarationDatabase.ContainsKey(labelstring)) //If the label is an event.
                        {
                            //if the label says that this event can not happen, the event is eat0 and the label is !eat0.
                            if (labelstring == evt)
                            {
                                toAdd = false;
                                break;
                            }
                        }
                        else //If the label is a proposition.
                        {
                            ExpressionValue v = EvaluatorDenotational.Evaluate(DeclarationDatabase[labelstring], env);

                            //liuyang: v must be a boolconstant, 20/04/2009
                            Debug.Assert(v is BoolConstant);
                            //if (v is BoolConstant)
                            //{
                            if ((v as BoolConstant).Value)
                            {
                                toAdd = false;
                                break;
                            }
                            //}
                        }
                    }
                    else //if (!label.Negated)
                    {
                        if (!DeclarationDatabase.ContainsKey(labelstring)) //If the label is an event.
                        {
                            if (labelstring != evt)
                            {
                                toAdd = false;
                                break;
                            }
                        }
                        else //If the label is a proposition.
                        {
                            ExpressionValue v = EvaluatorDenotational.Evaluate(DeclarationDatabase[labelstring], env);

                            //liuyang: v must be a boolconstant, 20/04/2009
                            Debug.Assert(v is BoolConstant);
                            //if (v is BoolConstant)
                            //{
                            if (!(v as BoolConstant).Value)
                            {
                                toAdd = false;
                                break;
                            }
                            //}
                        }
                    }                    
                }

                if (toAdd && !returnList.Contains(to_state_index))
                {
                    returnList.Add(to_state_index);
                }
            

            }

            return returnList;
        }

        #endregion

    }
}
