using System;
using System.Collections.Generic;
using System.Text;
using Antlr.Runtime;
using PAT.Common.Classes.Expressions.ExpressionClass;
using PAT.Common.Classes.ModuleInterface;
using PAT.Common.Classes.Ultility;

namespace PAT.Common.Classes.BA
{
    public sealed class BuchiAutomata
    {
        public List<string> InitialStates;
        public string[] States;
        public Transition[] Transitions;
        public bool HasAcceptState;
        public string Name;
        public Dictionary<string, Expression> DeclarationDatabase;
        public List<string> VisibleVariables;       
 
        public Dictionary<string, Transition[]> fromTransitions;
        public bool HasXOperator;
        public bool SyntacticSafety;

        public BuchiAutomata(List<string> initials, string[] states, Transition[] transitions, string[] fairset)
        {
            Transitions = transitions;
            States = states;
            this.InitialStates = initials;
            HasAcceptState = fairset.Length > 0;
            fromTransitions = new Dictionary<string, Transition[]>();

            //reorder transitions for easy referencing; removing infeasible transitions.
            foreach (string state in States)
            {
                List<Transition> temp = new List<Transition>();

                foreach (Transition transition in Transitions)
                {
                    if (transition.FromState == state)
                    {
                        temp.Add(transition);
                    }
                }

                List<Transition> trans = new List<Transition>();
                foreach (Transition transition in temp)
                {
                    if (transition.ToState.EndsWith(Constants.ACCEPT_STATE))
                    {
                        trans.Add(transition);
                    }
                }

                foreach (Transition transition in temp)
                {
                    if (!transition.ToState.EndsWith(Constants.ACCEPT_STATE))
                    {
                        trans.Add(transition);
                    }
                }

                fromTransitions.Add(state, trans.ToArray());
            }
        }

        /// <summary>
        /// initialize the visible vars and events
        /// </summary>
        /// <param name="declare"></param>
        public void Initialize(Dictionary<string, Expression> declare)
        {
            //Get visible events and variables
            DeclarationDatabase = declare;
            VisibleVariables = new List<string>();
            
            foreach (Transition transition in Transitions)
            {
                foreach (Proposition label in transition.labels)
                {
                    if (!label.IsSigmal)
                    {
                        if (DeclarationDatabase.ContainsKey(label.Label))                        
                        {
                            Ultility.Ultility.Union(VisibleVariables, DeclarationDatabase[label.Label].GetVars());

                            //try
                            //{
                                PAT.Common.Ultility.ParsingUltility.TestIsBooleanExpression(
                                DeclarationDatabase[label.Label],
                                new CommonToken(null, -1, -1, -1, -1),
                                " used in LTL proposition " + label.Label,
                                null,
                                new Dictionary<string, Expression>()
                                );
                            //}
                            //catch (Exception)
                            //{
                            //}                            
                        }
                    }
                }
            }
        }

        public List<string> MakeOneMove(string state, ConfigurationBase config)
        {
            List<string> returnList = new List<string>();
            Transition[] trans = fromTransitions[state];
            string evt = config.Event;

            foreach (Transition tran in trans)
            {
                bool toAdd = true;
                foreach (Proposition label in tran.labels)
                {
                    //If the transition is labelled with Sigma, there should not be any other labels. 
                    if (label.IsSigmal)
                    {
                        returnList.Add(tran.ToState);
                        break;
                    }

                    string labelstring = label.Label;

                    Expression exp;

                    //If the labed is negated, e.g., !eat0.
                    if (label.Negated)
                    {
                        //if (!DeclarationDatabase.ContainsKey(labelstring)) //If the label is an event.
                        if(!DeclarationDatabase.TryGetValue(labelstring, out exp))
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
                            //if (config.ImplyCondition(DeclarationDatabase[labelstring]))
                            if (config.ImplyCondition(exp))
                            {
                                toAdd = false;
                                break;
                            }
                        }
                    }
                    else //if (!label.Negated)
                    {
                        //if (!DeclarationDatabase.ContainsKey(labelstring)) //If the label is an event.
                        if (!DeclarationDatabase.TryGetValue(labelstring, out exp))
                        {
                            if (labelstring != evt)
                            {
                                toAdd = false;
                                break;
                            }
                        }
                        else //If the label is a proposition.
                        {
                            if (!config.ImplyCondition(exp))
                            {
                                toAdd = false;
                                break;
                            }
                        }
                    }
                }

                if (toAdd && !returnList.Contains(tran.ToState))
                {
                    returnList.Add(tran.ToState);
                }
            }

            return returnList;
        }

        public override string ToString()
        {
            StringBuilder s = new StringBuilder();
            s.AppendLine("States");
            foreach (string state in States)
            {
                s.AppendLine(state);
            }
            s.AppendLine("Transitions");
            foreach (Transition transition in Transitions)
            {
                if (this.InitialStates.Contains(transition.FromState))
                {
                    s.Append("-->");
                }
                s.AppendLine(transition.ToString());
            }

            return s.ToString();
        }
    }

    public sealed class EventBAPairSafety
    {
        public ConfigurationBase configuration;
        public List<string> States;

        public EventBAPairSafety(ConfigurationBase e, List<string> s)
        {
            configuration = e;
            States = s;
        }

        public string GetCompressedState()
        {
            return configuration.GetIDWithEvent() + "*" + Ultility.Ultility.PPStringList(States);
        }

        public static EventBAPairSafety GetInitialPairs(BuchiAutomata BA, ConfigurationBase initialStep)
        {
            List<string> intialBAStates = new List<string>();
            //List<string> existed = new List<string>();

            foreach (string s in BA.InitialStates)
            {
                List<string> next = BA.MakeOneMove(s, initialStep);

                foreach (string var in next)
                {
                    if (!intialBAStates.Contains(var))
                    {
                        //existed.Add(var);
                        intialBAStates.Add(var);
                    }
                }
            }

            return new EventBAPairSafety(initialStep, intialBAStates);
        }

        public  EventBAPairSafety[] Next(BuchiAutomata BA, ConfigurationBase[] steps)
        {
            EventBAPairSafety[] product = new EventBAPairSafety[steps.Length]; // * BA.States.Length);

            for (int i = 0; i < steps.Length; i++)
            {
                List<string> targetStates = new List<string>();

                foreach (string state in States)
                {
                    List<string> states = BA.MakeOneMove(state, steps[i]);
                    Ultility.Ultility.Union(targetStates, states);
                }

                product[i] = new EventBAPairSafety(steps[i], targetStates);
            }

            return product;
        }
    }
}