using System.Collections.Generic;
using PAT.Common.Classes.BA;
using PAT.Common.Classes.CUDDLib;
using PAT.Common.Classes.DataStructure;
using PAT.Common.Classes.Expressions.ExpressionClass;
using PAT.Common.Classes.Expressions;
using PAT.Common.Classes.LTS;
using PAT.Common.Classes.Ultility;

namespace PAT.Common.Classes.SemanticModels.LTS.BDD
{
    public class BDDEncoder
    {
        public Model model;

        /// <summary>
        /// Used to trace back the event name. All events of the system are encode in 1 BDD variable
        /// event name.number of parameters.corresponding index
        /// </summary>
        public List<EventChannelInfo> allEventIndex = new List<EventChannelInfo>();

        /// <summary>
        /// Mapping state ID to the int index of the current encoded process
        /// </summary>
        public Dictionary<string, int> stateIndexOfCurrentProcess = new Dictionary<string, int>();

        /// <summary>
        /// Get the event name, channel name from the currentStateDD in the column form
        /// [ REFS: '', DEREFS: '']
        /// </summary>
        /// <param name="lastValuation"></param>
        /// <param name="currentValuation"></param>
        /// <param name="currentStateDD"></param>
        /// <returns></returns>
        public string GetEventChannelName(Valuation lastValuation, Valuation currentValuation, CUDDNode currentStateDD)
        {
            int eventIndex = model.GetColVarValue(currentStateDD, Model.EVENT_NAME);
            EventChannelInfo eventChannelInfo = this.allEventIndex[eventIndex];

            if (eventChannelInfo.type == EventChannelInfo.EventType.EVENT)
            {
                return GetEventName(eventChannelInfo, currentStateDD);
            }
            else
            {
                return GetChannelName(eventChannelInfo, currentStateDD, lastValuation, currentValuation);
            }
        }


        private string GetEventName(EventChannelInfo eventChannelInfo, CUDDNode currentStateDD)
        {
            string eventName = eventChannelInfo.name;
            int parameterLength = eventChannelInfo.numberOfParameters;

            for (int i = 0; i < parameterLength; i++)
            {
                int parameterValue = model.GetColVarValue(currentStateDD, model.eventParameterVariables[i]);
                eventName += "." + parameterValue;
            }

            return eventName;
        }

        private string GetChannelName(EventChannelInfo eventChannelInfo, CUDDNode currentStateDD, Valuation lastValuation, Valuation currentValuation)
        {
            string channelName = eventChannelInfo.name;

            if (eventChannelInfo.type == EventChannelInfo.EventType.ASYNC_CHANNEL_INPUT)
            {
                channelName += "?";
            }
            else if (eventChannelInfo.type == EventChannelInfo.EventType.ASYNC_CHANNEL_OUTPUT)
            {
                channelName += "!";
            }

            switch (eventChannelInfo.type)
            {
                case EventChannelInfo.EventType.ASYNC_CHANNEL_INPUT:
                    ChannelQueue channelBuffer1 = lastValuation.Channels[eventChannelInfo.name];
                    foreach (ExpressionValue elementValue in channelBuffer1.Peek())
                    {
                        int value = int.Parse(elementValue.ExpressionID);
                        channelName += value + ".";
                    }
                    break;
                case EventChannelInfo.EventType.ASYNC_CHANNEL_OUTPUT:
                    ChannelQueue channelBuffer2 = currentValuation.Channels[eventChannelInfo.name];
                    foreach (ExpressionValue elementValue in channelBuffer2.ToArray()[channelBuffer2.Size - 1])
                    {
                        int value = int.Parse(elementValue.ExpressionID);
                        channelName += value + ".";
                    }
                    break;
            }

            channelName = channelName.TrimEnd('.');
            return channelName;
        }

        /// <summary>
        /// Return the event Index based on eventName & parameter Length. If not exist, add new event to allEventIndex dictionary
        /// </summary>
        /// <param name="eventName"></param>
        /// <param name="parameterLength"></param>
        /// <returns></returns>
        public int GetEventIndex(string eventName, int parameterLength)
        {
            for (int i = 0; i < this.allEventIndex.Count; i++)
            {
                EventChannelInfo eventChannel = this.allEventIndex[i];
                if (eventChannel.type == EventChannelInfo.EventType.EVENT && eventChannel.name == eventName && eventChannel.numberOfParameters == parameterLength)
                {
                    return i;
                }
            }

            this.allEventIndex.Add(new EventChannelInfo(eventName, parameterLength, EventChannelInfo.EventType.EVENT));
            //
            return this.allEventIndex.Count - 1;
        }

        /// <summary>
        /// Return the update event expression when the Event is an Event object
        /// </summary>
        /// <param name="Event"></param>
        /// <returns></returns>
        public Expression GetEventExpression(Event Event)
        {
            List<Expression> parameters = this.GetParaExpInEvent(Event);

            int eventIndex = this.GetEventIndex(Event.BaseName, parameters.Count);

            Expression eventUpdateExpression = new Assignment(Model.EVENT_NAME, new IntConstant(eventIndex));

            if (parameters.Count > 0)
            {
                for (int i = 0; i < parameters.Count; i++)
                {
                    eventUpdateExpression = new Sequence(eventUpdateExpression, new Assignment(this.model.eventParameterVariables[i], parameters[i]));
                }
            }

            return eventUpdateExpression;
        }

        /// <summary>
        /// Get the expression list from event.
        /// Event.a.b return [a, b]
        /// Event.1.2 return [1, 2]
        /// </summary>
        /// <param name="Event"></param>
        /// <returns></returns>
        public List<Expression> GetParaExpInEvent(Event Event)
        {
            List<Expression> parameters = new List<Expression>();

            if (Event.ExpressionList != null)
            {
                parameters = new List<Expression>(Event.ExpressionList);
            }
            else if (Event.EventID != null && Event.EventID.Contains("."))
            {
                string[] temp = Event.EventID.Split('.');

                for (int i = 1; i < temp.Length; i++)
                {
                    parameters.Add(new IntConstant(int.Parse(temp[i])));
                }
            }

            //
            return parameters;
        }

        public int GetChannelIndex(string channelName, EventChannelInfo.EventType type)
        {
            for (int i = 0; i < this.allEventIndex.Count; i++)
            {
                EventChannelInfo eventChannel = this.allEventIndex[i];
                if (eventChannel.type == type && eventChannel.name == channelName)
                {
                    return i;
                }
            }

            EventChannelInfo channel = new EventChannelInfo(channelName, 0, type);
            this.allEventIndex.Add(channel);
            //
            return this.allEventIndex.Count - 1;
        }

        /// <summary>
        /// Add global variables and environment variables
        /// </summary>
        /// <param name="valuation"></param>
        /// <param name="assertion"></param>
        public BDDEncoder(Valuation valuation)
        {
            model = new Model();

            //4 more events for Tau, Terminate, and temp, and tock
            Model.NUMBER_OF_EVENT += 4;
            //each event with global update, add Model.NUMBER_OF_EVENT to its index
            model.AddSingleCopyVar(Model.EVENT_NAME, 0, Model.NUMBER_OF_EVENT - 1);
            
            this.allEventIndex.Add(new EventChannelInfo(Constants.TAU, 0, EventChannelInfo.EventType.EVENT));
            this.allEventIndex.Add(new EventChannelInfo(Constants.TERMINATION, 0, EventChannelInfo.EventType.EVENT));
            this.allEventIndex.Add(new EventChannelInfo(Constants.TOCK, 0, EventChannelInfo.EventType.EVENT));
            
            for (int i = 0; i < Model.MAX_NUMBER_EVENT_PARAMETERS; i++)
            {
                string varName = Model.EVENT_NAME + Model.NAME_SEPERATOR + i;
                model.eventParameterVariables.Add(varName);
                model.AddSingleCopyVar(varName, Model.MIN_EVENT_INDEX[i], Model.MAX_EVENT_INDEX[i]);
            }

            //
            AddGlobalVars(valuation);
            AddGlobalChannel(valuation);
        }

        /// <summary>
        /// Based on the list of global variables in the Valuation, add them to the model
        /// </summary>
        /// <param name="valuation"></param>
        public void AddGlobalVars(Valuation valuation)
        {
            if (valuation.Variables != null && valuation.Variables.Count > 0)
            {
                foreach (StringDictionaryEntryWithKey<ExpressionValue> pair in valuation.Variables._entries)
                {
                    if (pair != null)
                    {
                        int lowerBound = Model.BDD_INT_LOWER_BOUND;
                        if (Valuation.VariableLowerBound.ContainsKey(pair.Key))
                        {
                            lowerBound = Valuation.VariableLowerBound.GetContainsKey(pair.Key);
                        }

                        int upperBound = Model.BDD_INT_UPPER_BOUND;
                        if (Valuation.VariableUpperLowerBound.ContainsKey(pair.Key))
                        {
                            upperBound = Valuation.VariableUpperLowerBound.GetContainsKey(pair.Key);
                        }

                        if (pair.Value is RecordValue)
                        {
                            RecordValue array = pair.Value as RecordValue;
                            this.model.AddGlobalArray(pair.Key, array.Associations.Length, lowerBound, upperBound);
                        }
                        else if (pair.Value is BoolConstant)
                        {
                            this.model.AddGlobalVar(pair.Key, 0, 1);
                        }
                        else
                        {
                            this.model.AddGlobalVar(pair.Key, lowerBound, upperBound);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Add channel variable to model from channel delcaration in Valuation
        /// </summary>
        /// <param name="valuation"></param>
        public void AddGlobalChannel(Valuation valuation)
        {
            if (valuation.Channels != null && valuation.Channels.Count > 0)
            {
                foreach (KeyValuePair<string, ChannelQueue> pair in valuation.Channels)
                {
                    model.AddGlobalChannel(pair.Key, pair.Value.Size);
                }
            }
        }

        public Expression InitializeGlobalVariables(Valuation valuation)
        {
            Expression initialVariables = new BoolConstant(true);
            //continue build inital state
            //default value of global variables
            if (valuation.Variables != null)
            {
                foreach (StringDictionaryEntryWithKey<ExpressionValue> pair in valuation.Variables._entries)
                {
                    if (pair != null)
                    {
                        if (!(pair.Value is WildConstant))
                        {
                            if (pair.Value is RecordValue)
                            {
                                RecordValue array = pair.Value as RecordValue;
                                for (int i = 0; i < array.Associations.Length; i++)
                                {
                                    if (!(array.Associations[i] is WildConstant))
                                    {
                                        Expression initialVariable = Expression.EQ(
                                            new Variable(pair.Key + Model.NAME_SEPERATOR + i),
                                            new IntConstant(int.Parse(array.Associations[i].ExpressionID)));

                                        initialVariables = Expression.AND(initialVariables, initialVariable);
                                    }
                                    else
                                    {
                                        string variableName = pair.Key + Model.NAME_SEPERATOR + i;
                                        Expression lowerBound = Expression.GE(new Variable(variableName),
                                                                                                                             new IntConstant(model.GetVarLowerBound(variableName)));
                                        Expression upperBound = Expression.LE(new Variable(variableName),
                                                                                                                             new IntConstant(model.GetVarUpperBound(variableName)));

                                        initialVariables = Expression.AND(initialVariables, lowerBound);
                                        initialVariables = Expression.AND(initialVariables, upperBound);
                                    }
                                }
                            }
                            else if (pair.Value is BoolConstant)
                            {
                                int value = (pair.Value as BoolConstant).Value ? 1 : 0;
                                Expression initialVariable = Expression.EQ(new Variable(pair.Key), new IntConstant(value));
                                initialVariables = Expression.AND(initialVariables, initialVariable);
                            }
                            else
                            {
                                Expression initialVariable = Expression.EQ(new Variable(pair.Key),
                                                                                    new IntConstant(int.Parse(pair.Value.ExpressionID)));
                                initialVariables = Expression.AND(initialVariables, initialVariable);
                            }
                        }
                        else
                        {
                            Expression lowerBound = Expression.GE(new Variable(pair.Key), new IntConstant(model.GetVarLowerBound(pair.Key)));
                            Expression upperBound = Expression.LE(new Variable(pair.Key), new IntConstant(model.GetVarUpperBound(pair.Key)));

                            initialVariables = Expression.AND(initialVariables, lowerBound);
                            initialVariables = Expression.AND(initialVariables, upperBound);
                        }
                    }
                }
            }

            if (valuation.Channels != null)
            {
                foreach (KeyValuePair<string, ChannelQueue> pair in valuation.Channels)
                {
                    //initialize the top index of channel is 0
                    Expression initialVariable = Expression.EQ(new Variable(Model.GetTopVarChannel(pair.Key)), new IntConstant(0));
                    initialVariables = Expression.AND(initialVariables, initialVariable);

                    //initialize the cound of channel buffer is 0
                    initialVariable = Expression.EQ(new Variable(Model.GetCountVarChannel(pair.Key)), new IntConstant(0));
                    initialVariables = Expression.AND(initialVariables, initialVariable);
                }
            }

            return initialVariables;
        }

        /// <summary>
        /// Return Valuation in Configuration of given BDD configuration in the column form
        /// [ REFS: '', DEREFS: '']
        /// </summary>
        /// <param name="currentStateDD">current BDD configuration</param>
        /// <param name="initialValuation">based on Initial Valuation to get the global variables</param>
        /// <returns>Corresponding Valuation of the BDD configuration</returns>
        public Valuation GetValuationFromBDD(CUDDNode currentStateDD, Valuation initialValuation)
        {
            Valuation currentValuation = initialValuation.GetClone();

            if (currentValuation.Variables != null && currentValuation.Variables.Count > 0)
            {
                foreach (StringDictionaryEntryWithKey<ExpressionValue> pair in currentValuation.Variables._entries)
                {
                    if (pair != null)
                    {
                        if (pair.Value is RecordValue)
                        {
                            RecordValue array = pair.Value as RecordValue;
                            ExpressionValue[] arrayValue = new ExpressionValue[array.Associations.Length];

                            for (int i = 0; i < array.Associations.Length; i++)
                            {
                                string variableName = pair.Key + Model.NAME_SEPERATOR + i.ToString();
                                int value = model.GetColVarValue(currentStateDD, variableName);
                                arrayValue[i] = new IntConstant(value);
                            }
                            pair.Value = new RecordValue(arrayValue);
                        }
                        else if (pair.Value is BoolConstant)
                        {
                            string variableName = pair.Key;
                            int value = model.GetColVarValue(currentStateDD, variableName);
                            pair.Value = new BoolConstant(value == 1);
                        }
                        else
                        {
                            string variableName = pair.Key;
                            int value = model.GetColVarValue(currentStateDD, variableName);
                            pair.Value = new IntConstant(value);
                        }
                    }
                }
            }

            if (currentValuation.Channels != null && currentValuation.Channels.Count > 0)
            {
                List<string> channelNames = new List<string>(currentValuation.Channels.Keys);


                foreach (string channelName in channelNames)
                {
                    int count = model.GetColVarValue(currentStateDD, Model.GetCountVarChannel(channelName));
                    int top = model.GetColVarValue(currentStateDD, Model.GetTopVarChannel(channelName));

                    ChannelQueue currentQueue = new ChannelQueue(count);

                    int firstElement = 0;
                    if (top >= count)
                    {
                        firstElement = top - count;
                    }
                    else
                    {
                        firstElement = top - count + model.mapChannelToSize[channelName];
                    }

                    for (int i = 0; i < count; i++)
                    {
                        int elementSize = model.GetColVarValue(currentStateDD, Model.GetArrayOfSizeElementChannel(channelName) + Model.NAME_SEPERATOR + firstElement);
                        ExpressionValue[] elementValues = new ExpressionValue[elementSize];
                        //Find values in the message
                        for (int j = 0; j < elementSize; j++)
                        {
                            int subElementIndex = firstElement * Model.MAX_MESSAGE_LENGTH + j;
                            int value = model.GetColVarValue(currentStateDD, channelName + Model.NAME_SEPERATOR + subElementIndex.ToString());
                            elementValues[j] = new IntConstant(value);
                        }

                        //Add element to queue
                        currentQueue.Enqueue(elementValues);

                        //update to the next element
                        firstElement = (firstElement + 1) % model.mapChannelToSize[channelName];
                    }

                    currentValuation.Channels[channelName] = currentQueue;

                }
            }

            return currentValuation;
        }

        #region Encode Buchi Automata of LTL property
        public AutomataBDD EncodeBA(BuchiAutomata buchi)
        {
            //
            string processVariableName = Model.GetNewTempVarName();
            this.model.AddLocalVar(processVariableName, 0, buchi.States.Length - 1);

            //
            this.stateIndexOfCurrentProcess = new Dictionary<string, int>();
            //collect the state index
            foreach (string state in buchi.States)
            {
                this.stateIndexOfCurrentProcess.Add(state, this.stateIndexOfCurrentProcess.Count);
            }

            AutomataBDD processAutomataBDD = new AutomataBDD();

            //Set variable
            processAutomataBDD.variableIndex.Add(this.model.GetVarIndex(processVariableName));


            //Set initial expression
            processAutomataBDD.initExpression = new BoolConstant(false);

            foreach (string initState in buchi.InitialStates)
            {
                processAutomataBDD.initExpression = Expression.OR(processAutomataBDD.initExpression,
                                                            Expression.EQ(new Variable(processVariableName),
                                                                            new IntConstant(this.stateIndexOfCurrentProcess[initState])));
            }

            //set acceptance expression
            processAutomataBDD.acceptanceExpression = new BoolConstant(false);
            foreach (string state in buchi.States)
            {
                if (state.EndsWith(Constants.ACCEPT_STATE))
                {
                    processAutomataBDD.acceptanceExpression = Expression.OR(processAutomataBDD.acceptanceExpression,
                                                            Expression.EQ(new Variable(processVariableName),
                                                                            new IntConstant(this.stateIndexOfCurrentProcess[state])));
                }
            }

            //Encode transition
            foreach (BA.Transition transition in buchi.Transitions)
            {
                List<CUDDNode> transitionBDDs = this.EncodeTransitionBA(transition, buchi.DeclarationDatabase, processVariableName);

                processAutomataBDD.transitionBDD.AddRange(transitionBDDs);
            }

            //
            return processAutomataBDD;
        }

        private List<CUDDNode> EncodeTransitionBA(BA.Transition transition, Dictionary<string, Expression> declarationDatabase, string processVariableName)
        {
            //
            Expression transitionDD = Expression.EQ(new Variable(processVariableName),
                                                            new IntConstant(this.stateIndexOfCurrentProcess[transition.FromState]));
            transitionDD = Expression.AND(transitionDD, new Assignment(processVariableName, new IntConstant(this.stateIndexOfCurrentProcess[transition.ToState])));

            

            List<CUDDNode> transBDD = new List<CUDDNode> { CUDD.Constant(1) };

            foreach (Proposition label in transition.labels)
            {
                List<CUDDNode> temp;

                if (!label.IsSigmal)
                {
                    //label is a propostion
                    if (declarationDatabase != null && declarationDatabase.ContainsKey(label.Label))
                    {
                        Expression proposition = (label.Negated)
                                                     ? Expression.NOT(declarationDatabase[label.Label])
                                                     : declarationDatabase[label.Label];
                        temp = EncodeProposition(transitionDD, proposition);
                    }
                    else
                    {
                        //label is an event
                        //Because we mark event with update with # at the begininig
                        //At this step, we don't know whether the event has update, therefore we check both state
                        Expression eventExpression = Expression.OR(
                                                                              GetEventExpression(label.Label),
                                                                              GetEventExpression(Model.NAME_SEPERATOR +
                                                                                                 label.Label));
                        if(label.Negated)
                        {
                            eventExpression = Expression.NOT(eventExpression);
                        }

                        transitionDD = Expression.AND(transitionDD, eventExpression);
                        temp = transitionDD.TranslateBoolExpToBDD(model).GuardDDs;
                    }
                }
                else
                {
                    temp = transitionDD.TranslateBoolExpToBDD(model).GuardDDs;
                }

                transBDD = CUDD.Function.And(transBDD, temp);
            }

            //
            return transBDD;
        }

        /// <summary>
        /// Return the event update expression when the event is a string like "event.0"
        /// </summary>
        /// <param name="eventName"></param>
        /// <returns></returns>
        private Expression GetEventExpression(string eventName)
        {
            string[] eventsTemp = eventName.Split('.', '[', ']');
            List<string> events = new List<string>();
            for (int i = 0; i < eventsTemp.Length; i++)
            {
                if (!string.IsNullOrEmpty(eventsTemp[i]))
                {
                    events.Add(eventsTemp[i]);
                }
            }
            
            Expression eventExpression = new Assignment(Model.EVENT_NAME, new IntConstant(this.GetEventIndex(events[0], events.Count - 1)));

            for (int i = 1; i < events.Count; i++)
            {
                eventExpression = new Sequence(eventExpression, new Assignment(this.model.eventParameterVariables[i - 1], new IntConstant(int.Parse(events[i]))));
            }

            return eventExpression;
        }

        /// <summary>
        /// Encode buchi automaton for proposition. Proposition is implemented as to be true at the destination state
        /// </summary>
        /// <param name="guardDD"></param>
        /// <param name="proposition"></param>
        /// <returns></returns>
        private List<CUDDNode> EncodeProposition(Expression guardDD, Expression proposition)
        {
            List<CUDDNode> transition = guardDD.TranslateBoolExpToBDD(model).GuardDDs;

            List<CUDDNode> propositionDD = model.SwapRowColVars(proposition.TranslateBoolExpToBDD(model).GuardDDs);
            return CUDD.Function.And(transition, propositionDD);
        }

        #endregion Encode Buchi Automata of LTL property

        /// <summary>
        /// Return the CUDDNode of the alphabet
        /// </summary>
        /// <param name="alphabets"></param>
        /// <returns></returns>
        public CUDDNode GetAlphabetInBDD(HashSet<string> alphabets)
        {
            Expression result = new BoolConstant(false);
            foreach (var alphabet in alphabets)
            {
                result = Expression.OR(result, GetEventExpression(alphabet));
            }

            return CUDD.Function.Or(result.TranslateBoolExpToBDD(model).GuardDDs);
        }

        public class EventChannelInfo : Event
        {
            public enum EventType { EVENT, ASYNC_CHANNEL_INPUT, ASYNC_CHANNEL_OUTPUT, SYNC_CHANNEL_INPUT, SYNC_CHANNEL_OUTPUT };

            /// <summary>
            /// Name of event or channel
            /// </summary>
            public string name;
            /// <summary>
            /// Used for only event, later get parameters value from event#0, event#1...
            /// </summary>
            public int numberOfParameters;

            public EventType type;

            public EventChannelInfo(string name, int numberOfParameters, EventType type)
                : base(name)
            {
                this.name = name;
                this.numberOfParameters = numberOfParameters;
                this.type = type;
            }
        }
    }
}
