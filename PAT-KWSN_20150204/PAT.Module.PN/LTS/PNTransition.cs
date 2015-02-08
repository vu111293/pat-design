using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PAT.Common.Classes.CUDDLib;
using PAT.Common.Classes.Expressions;
using PAT.Common.Classes.Expressions.ExpressionClass;
using PAT.Common.Classes.LTS;
using PAT.Common.Classes.SemanticModels.LTS.BDD;
using PAT.Common.Classes.Ultility;

namespace PAT.PN.LTS
{
    /// <summary>
    /// This class represent a transition & its arcs in the petrinet.
    /// </summary>
    public class PNTransition
    {
        /// <summary>
        /// Copy from PAT.Common.Classes.SemanticModels.LTS.BDD.Transition
        /// </summary>
        public ParallelDefinition[] Selects;

        /// <summary>
        /// TL: Hold the condition for trasition to fired: number tokens is required in each Places
        /// </summary>
        public Expression GuardCondition;

        public Event Event;

        /// <summary>
        /// TL: Hold the action when transition is fired: add/subtract tokens in each places
        /// </summary>
        public Expression ProgramBlock;

        public bool HasLocalVariable;

        public PNPlace FromPNPlace;
        public PNPlace ToPNPlace;

        public PNTransition(Event e, ParallelDefinition[] selects, Expression guard, Expression assignment, PNPlace from, PNPlace to) //string[] localvar, 
        {
            Event = e;
            Selects = selects;
            ProgramBlock = assignment;
            GuardCondition = guard;
            FromPNPlace = from;
            ToPNPlace = to;
        }

        public override string ToString()
        {
            return "\"" + FromPNPlace + "\"--" + GetTransitionLabel() + "-->\"" + ToPNPlace + "\"";
        }

        private string label = null;
        public string GetTransitionLabel()
        {
            if (label == null)
            {
                string program = "";

                if (Selects != null)
                    program += "{" + Common.Classes.Ultility.Ultility.PPStringList(Selects) + "}";
                if (ProgramBlock != null)
                    program += "{" + ProgramBlock + "}";

                string guard = "";
                if (GuardCondition != null)
                    guard += "[" + GuardCondition + "]";

                label = guard + Event + program;
            }

            return label;
        }

        public List<PNTransition> ClearConstantExtended(List<PNPlace> newPNPlaces, Dictionary<string, Expression> constMapping)
        {
            var transitions = new List<PNTransition>();
            if (Selects == null)
            {
                transitions.Add(ClearConstant(newPNPlaces, constMapping, false));
            }
            else
            {
                var newDefinitions = new List<ParallelDefinition>();
                foreach (var definition in Selects)
                {
                    var newPD = definition.ClearConstant(constMapping);
                    newDefinitions.Add(newPD);
                }

                foreach (ParallelDefinition pd in newDefinitions)
                    pd.DomainValues.Sort();

                var list = new List<List<Expression>>();
                foreach (int v in newDefinitions[0].DomainValues)
                {
                    var l = new List<Expression>(newDefinitions.Count);
                    l.Add(new IntConstant(v));
                    list.Add(l);
                }

                for (int i = 1; i < newDefinitions.Count; i++)
                {
                    var newList = new List<List<Expression>>();
                    var domain = newDefinitions[i].DomainValues;

                    for (int j = 0; j < list.Count; j++)
                    {
                        foreach (int i1 in domain)
                        {
                            var cList = new List<Expression>(list[j]);
                            cList.Add(new IntConstant(i1));
                            newList.Add(cList);
                        }
                    }
                    list = newList;
                }

                foreach (List<Expression> constants in list)
                {
                    var constMappingNew = new Dictionary<string, Expression>(constMapping);
                    for (var i = 0; i < constants.Count; i++)
                    {
                        Expression constant = constants[i];
                        constMappingNew.Add(newDefinitions[i].Parameter, constant);
                    }

                    var newProcess = ClearConstant(newPNPlaces, constMappingNew, false);
                    transitions.Add(newProcess);
                }
            }

            return transitions;
        }


        public PNTransition ClearConstant(List<PNPlace> newPNPlaces, Dictionary<string, Expression> constMapping, bool checkSelect)
        {
            ParallelDefinition[] newSelects = null;

            if (checkSelect && Selects != null)
            {
                newSelects = new ParallelDefinition[Selects.Length];
                for (int i = 0; i < Selects.Length; i++)
                    newSelects[i] = newSelects[i].ClearConstant(constMapping);
            }

            PNPlace newFrom = null, newTo = null;
            foreach (PNPlace PNPlace in newPNPlaces)
            {
                if (PNPlace.Name == FromPNPlace.Name)
                    newFrom = PNPlace;

                if (PNPlace.Name == ToPNPlace.Name)
                    newTo = PNPlace;
            }

            return new PNTransition(Event.ClearConstant(constMapping), newSelects, GuardCondition == null ? GuardCondition : GuardCondition.ClearConstant(constMapping), ProgramBlock == null ? ProgramBlock : ProgramBlock.ClearConstant(constMapping), newFrom, newTo);
        }

        public void GetGlobalVariables(List<string> returnList)
        {
            if (Selects != null)
            {
                for (int i = 0; i < Selects.Length; i++)
                    Common.Classes.Ultility.Ultility.AddList(returnList, Selects[i].GetGlobalVariables());
            }

            if (GuardCondition != null)
                Common.Classes.Ultility.Ultility.AddList(returnList, GuardCondition.GetVars());

            if (Event.ExpressionList != null)
            {
                foreach (Expression expression in Event.ExpressionList)
                    Common.Classes.Ultility.Ultility.AddList(returnList, expression.GetVars());
            }

            if (ProgramBlock != null)
                Common.Classes.Ultility.Ultility.AddList(returnList, ProgramBlock.GetVars());
        }

        public void GetChannels(List<string> vars)
        {
            if (Event is ChannelInputEvent || Event is ChannelOutputEvent)
                vars.Add(Event.BaseName);
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
            Expression guardExpressions = Expression.EQ(new Variable(processVariableName),
                new IntConstant(encoder.stateIndexOfCurrentProcess[this.FromPNPlace.ID]));

            guardExpressions = Expression.CombineGuard(guardExpressions, GuardCondition);
            Expression eventUpdateExpression;
            if (this.Event is BDDEncoder.EventChannelInfo)
            {
                int channelIndex = encoder.GetChannelIndex(this.Event.BaseName, (this.Event as BDDEncoder.EventChannelInfo).type);
                eventUpdateExpression = new Assignment(Model.EVENT_NAME, new IntConstant(channelIndex));
            }
            else
                eventUpdateExpression = encoder.GetEventExpression(this.Event);

            Assignment PNPlaceUpdateExpression = new Assignment(processVariableName,
                new IntConstant(encoder.stateIndexOfCurrentProcess[this.ToPNPlace.ID]));
            Sequence updateExpressions = new Sequence(eventUpdateExpression, PNPlaceUpdateExpression);

            if (this.ProgramBlock != null)
                updateExpressions = new Sequence(updateExpressions, this.ProgramBlock);

            List<int> unchangedVars = new List<int>(localVars);
            if (!isSynchronized)
                unchangedVars.AddRange(encoder.model.GlobalVarIndex);

            return encoder.model.EncodeTransition(guardExpressions, updateExpressions, unchangedVars);
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
            Expression guard = Expression.EQ(new Variable(processVariableName),
                new IntConstant(encoder.stateIndexOfCurrentProcess[this.FromPNPlace.ID]));
            guard = Expression.CombineGuard(guard, GuardCondition);

            List<Expression> parameters = encoder.GetParaExpInEvent(this.Event);
            int channelEventIndex = encoder.GetEventIndex(this.Event.BaseName, parameters.Count);

            guard = Expression.AND(guard, new Assignment(Model.EVENT_NAME, new IntConstant(channelEventIndex)));
            for (int i = 0; i < parameters.Count; i++)
            {
                //assign event parameter to the values in the event expression
                guard = Expression.AND(guard, new Assignment(encoder.model.eventParameterVariables[i], parameters[i]));
            }

            guard = Expression.AND(guard, new Assignment(processVariableName,
                new IntConstant(encoder.stateIndexOfCurrentProcess[this.ToPNPlace.ID])));

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
