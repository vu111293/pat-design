using System.Collections.Generic;
using PAT.Common.Classes.CUDDLib;
using PAT.Common.Classes.Expressions.ExpressionClass;
using PAT.Common.Classes.LTS;
using PAT.Common.Classes.Ultility;

namespace PAT.Common.Classes.SemanticModels.LTS.BDD
{
    public class Transition
    {
        public ParallelDefinition[] Selects;
        public Expression GuardCondition;

        public Event Event;

        public Expression ProgramBlock;

        public bool HasLocalVariable;

        public State FromState;
        public State ToState;

        public Transition(Event e, ParallelDefinition[] selects, Expression Guard, Expression assignment, State from, State to) //string[] localvar, 
        {
            Event = e;
            Selects = selects;
            ProgramBlock = assignment;
            //LocalVariables = localvar;
            GuardCondition = Guard;
            FromState = from;
            ToState = to;
        }

        public override string ToString()
        {
            return "\"" + FromState + "\"--" + GetTransitionLabel() + "-->\"" + ToState + "\"";
        }

        private string label = null;
        public string GetTransitionLabel()
        {
            if (label == null)
            {
                string program = "";

                if (Selects != null)
                {
                    program += "{" + PAT.Common.Classes.Ultility.Ultility.PPStringList(Selects) + "}";
                }
                if (ProgramBlock != null)
                {
                    program += "{" + ProgramBlock + "}";
                }

                string guard = "";

                if (GuardCondition != null)
                {
                    guard += "[" + GuardCondition + "]";
                }

                label = guard + Event + program;
            }

            return label;
        }

        public List<Transition> ClearConstantExtended(List<State> newStates, Dictionary<string, Expression> constMapping)
        {
            List<Transition> transitions = new List<Transition>();
            if (Selects == null)
            {
                transitions.Add(ClearConstant(newStates, constMapping, false));
            }
            else
            {
                List<ParallelDefinition> newDefinitions = new List<ParallelDefinition>();
                foreach (ParallelDefinition definition in Selects)
                {
                    ParallelDefinition newPD = definition.ClearConstant(constMapping);
                    newDefinitions.Add(newPD);
                }

                foreach (ParallelDefinition pd in newDefinitions)
                {
                    pd.DomainValues.Sort();
                }

                List<List<Expression>> list = new List<List<Expression>>();
                foreach (int v in newDefinitions[0].DomainValues)
                {
                    List<Expression> l = new List<Expression>(newDefinitions.Count);
                    l.Add(new IntConstant(v));
                    list.Add(l);
                }

                for (int i = 1; i < newDefinitions.Count; i++)
                {
                    List<List<Expression>> newList = new List<List<Expression>>();
                    List<int> domain = newDefinitions[i].DomainValues;

                    for (int j = 0; j < list.Count; j++)
                    {
                        foreach (int i1 in domain)
                        {
                            List<Expression> cList = new List<Expression>(list[j]);
                            cList.Add(new IntConstant(i1));
                            newList.Add(cList);
                        }
                    }
                    list = newList;
                }

                foreach (List<Expression> constants in list)
                {
                    Dictionary<string, Expression> constMappingNew = new Dictionary<string, Expression>(constMapping);
                    for (int i = 0; i < constants.Count; i++)
                    {
                        Expression constant = constants[i];
                        constMappingNew.Add(newDefinitions[i].Parameter, constant);
                    }

                    Transition newProcess = ClearConstant(newStates, constMappingNew, false);
                    transitions.Add(newProcess);
                }
            }

            return transitions;
        }


        public Transition ClearConstant(List<State> newStates, Dictionary<string, Expression> constMapping, bool checkSelect)
        {
            ParallelDefinition[] newSelects = null;

            if (checkSelect && Selects != null)
            {
                newSelects = new ParallelDefinition[Selects.Length];
                for (int i = 0; i < Selects.Length; i++)
                {
                    newSelects[i] = newSelects[i].ClearConstant(constMapping);
                }
            }

            State newFrom = null, newTo = null;
            foreach (State state in newStates)
            {
                if (state.Name == FromState.Name)
                {
                    newFrom = state;
                }

                if (state.Name == ToState.Name)
                {
                    newTo = state;
                }
            }

            return new Transition(Event.ClearConstant(constMapping), newSelects, GuardCondition == null ? GuardCondition : GuardCondition.ClearConstant(constMapping), ProgramBlock == null ? ProgramBlock : ProgramBlock.ClearConstant(constMapping), newFrom, newTo);
        }

        public void GetGlobalVariables(List<string> returnList)
        {
            if (Selects != null)
            {
                for (int i = 0; i < Selects.Length; i++)
                {
                    Common.Classes.Ultility.Ultility.AddList(returnList, Selects[i].GetGlobalVariables());
                }
            }

            if (GuardCondition != null)
            {
                Common.Classes.Ultility.Ultility.AddList(returnList, GuardCondition.GetVars());
            }

            if (Event.ExpressionList != null)
            {
                foreach (Expression expression in Event.ExpressionList)
                {
                    Common.Classes.Ultility.Ultility.AddList(returnList, expression.GetVars());
                }
            }

            if (ProgramBlock != null)
            {
                Common.Classes.Ultility.Ultility.AddList(returnList, ProgramBlock.GetVars());
            }
        }

        public void GetChannels(List<string> vars)
        {
            if (Event is ChannelInputEvent || Event is ChannelOutputEvent)
            {
                vars.Add(Event.BaseName);
            }
        }

        /// <summary>
        /// Encode transition, if it is synchronized, then we don't add constraint of unchanged global variables
        /// Parallel process only synchorinize event which does not change global variable or each transition changes same to global variables
        /// 3 kinds of transition: normal event, async channel input and async channel output
        /// </summary>
        /// <param name="encoder"></param>
        /// <param name="processVariableName"></param>
        /// <param name="localVars">Local of the current SymbolicLTS is unchanged</param>
        /// <param name="isSynchronized"></param>
        /// <returns></returns>
        public List<CUDDNode> Encode(BDDEncoder encoder, string processVariableName, List<int> localVars, bool isSynchronized)
        {
            Expression guardExpressions = Expression.EQ(
                                                        new Variable(processVariableName), new IntConstant(encoder.stateIndexOfCurrentProcess[this.FromState.ID]));

            guardExpressions = Expression.CombineGuard(guardExpressions, GuardCondition);


            Expression eventUpdateExpression;
            if (this.Event is BDDEncoder.EventChannelInfo)
            {
                int channelIndex = encoder.GetChannelIndex(this.Event.BaseName, (this.Event as BDDEncoder.EventChannelInfo).type);
                eventUpdateExpression = new Assignment(Model.EVENT_NAME, new IntConstant(channelIndex));
            }
            else
            {
                eventUpdateExpression = encoder.GetEventExpression(this.Event);
            }

            Assignment stateUpdateExpression = new Assignment(processVariableName, new IntConstant(encoder.stateIndexOfCurrentProcess[this.ToState.ID]));
            Sequence updateExpressions = new Sequence(eventUpdateExpression, stateUpdateExpression);

            if (this.ProgramBlock != null)
            {
                updateExpressions = new Sequence(updateExpressions, this.ProgramBlock);
            }

            List<int> unchangedVars = new List<int>(localVars);
            if (!isSynchronized)
            {
                unchangedVars.AddRange(encoder.model.GlobalVarIndex);
            }

            return encoder.model.EncodeTransition(guardExpressions, updateExpressions, unchangedVars);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="encoder"></param>
        /// <param name="processVariableName"></param>
        /// <param name="localVars">Local of the current SymbolicLTS is unchanged</param>
        /// <returns></returns>
        public List<CUDDNode> EncodeSyncChannelInTransition(BDDEncoder encoder, string processVariableName, List<int> localVars)
        {
            Expression guard = Expression.EQ(
                                                        new Variable(processVariableName), new IntConstant(encoder.stateIndexOfCurrentProcess[this.FromState.ID]));

            guard = Expression.CombineGuard(guard, GuardCondition);

            List<Expression> parameters = encoder.GetParaExpInEvent(this.Event);
            int channelEventIndex = encoder.GetEventIndex(this.Event.BaseName, parameters.Count);

            guard = Expression.AND(guard, new Assignment(Model.EVENT_NAME, new IntConstant(channelEventIndex)));
            guard = Expression.AND(guard, new Assignment(processVariableName, new IntConstant(encoder.stateIndexOfCurrentProcess[this.ToState.ID])));

            //channel input has not program block
            for (int i = 0; i < parameters.Count; i++)
            {
                if (parameters[i] is IntConstant)
                {
                    //eventParameterVariables[i]' = exps[i]
                    guard = Expression.AND(guard, new Assignment(encoder.model.eventParameterVariables[i], parameters[i]));
                }
                else
                {
                    //eventParameterVariables[i]' = exps[i]'
                    guard = Expression.AND(guard,
                                 Expression.EQ(new VariablePrime(encoder.model.eventParameterVariables[i]),
                                                          new VariablePrime(parameters[i].expressionID)));

                }
            }

            List<CUDDNode> transitions = guard.TranslateBoolExpToBDD(encoder.model).GuardDDs;
            return encoder.model.AddVarUnchangedConstraint(transitions, localVars);
        }

        /// <summary>
        /// Encode sync channel out transition as event!a.b.c
        /// </summary>
        /// <param name="encoder"></param>
        /// <param name="processVariableName"></param>
        /// <param name="localVars">Local of the current SymbolicLTS is unchanged</param>
        /// <returns></returns>
        public List<CUDDNode> EncodeSyncChannelOutTransition(BDDEncoder encoder, string processVariableName, List<int> localVars)
        {
            Expression guard = Expression.EQ(
                                                       new Variable(processVariableName), new IntConstant(encoder.stateIndexOfCurrentProcess[this.FromState.ID]));

            guard = Expression.CombineGuard(guard, GuardCondition);

            List<Expression> parameters = encoder.GetParaExpInEvent(this.Event);
            int channelEventIndex = encoder.GetEventIndex(this.Event.BaseName, parameters.Count);

            guard = Expression.AND(guard, new Assignment(Model.EVENT_NAME, new IntConstant(channelEventIndex)));
            for (int i = 0; i < parameters.Count; i++)
            {
                //assign event parameter to the values in the event expression
                guard = Expression.AND(guard, new Assignment(encoder.model.eventParameterVariables[i], parameters[i]));
            }

            guard = Expression.AND(guard, new Assignment(processVariableName, new IntConstant(encoder.stateIndexOfCurrentProcess[this.ToState.ID])));

            List<CUDDNode> transitions = guard.TranslateBoolExpToBDD(encoder.model).GuardDDs;
            return encoder.model.AddVarUnchangedConstraint(transitions, localVars);
        }

        public bool IsTau()
        {
            return Event.BaseName == Constants.TAU;
        }

        public bool IsTick()
        {
            return Event.BaseName == Constants.TOCK;
        }

        public bool IsTermination()
        {
            return Event.BaseName == Constants.TERMINATION;
        }
    }
}
