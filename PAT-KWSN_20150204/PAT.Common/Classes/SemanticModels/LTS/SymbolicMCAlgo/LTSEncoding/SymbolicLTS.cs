using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using PAT.Common.Classes.CUDDLib;
using PAT.Common.Classes.DataStructure;
using PAT.Common.Classes.Expressions.ExpressionClass;
using PAT.Common.Classes.LTS;
using PAT.Common.Classes.Ultility;
using System;

namespace PAT.Common.Classes.SemanticModels.LTS.BDD
{
    public class SymbolicLTS
    {
        public State InitialState;
        public string Name;
        public List<Transition> Transitions = new List<Transition>();
        public List<State> States = new List<State>();

        /// <summary>
        /// Parameter information is used to add new boolean variables. UpperBound, LowerBound and Arguments are used to identify the parameter ranges.
        /// </summary>
        public List<string> Parameters = new List<string>();
        public List<Expression> Arguments = new List<Expression>();
        public Dictionary<string, int> ParameterUpperBound = new Dictionary<string, int>();
        public Dictionary<string, int> ParameterLowerBound = new Dictionary<string, int>();

        public EventCollection AlphabetEvents;
        public HashSet<string> Alphabets;

        public int StateIDCounter;

        //after static analysis, AlphabetsCalculable is true if and only if Alphabets are not null.
        public bool AlphabetsCalculable;

        /// <summary>
        /// Used for initial simulation partition. Simulation happens on states having the same location
        /// state ID of original state to list of new states
        /// </summary>
        public Dictionary<string, List<State>> mapOldLoc2NewStates = new Dictionary<string, List<State>>();

        /// <summary>
        /// state ID of new state to its valuation
        /// </summary>
        public Dictionary<string, int[]> mapNewState2Valuation = new Dictionary<string, int[]>();

        public SymbolicLTS()
        {

        }

        public SymbolicLTS(string name, List<string> vars, List<State> states)
        {
            Name = name;
            InitialState = states[0];
            States = states;
            if(vars != null)
            {
                Parameters = vars;
            }

            AlphabetsCalculable = true;
            Alphabets = new HashSet<string>();
        }

        /// <summary>
        /// To Perform the static analysis on a single definition first.
        /// </summary>
        public void StaticAnalysis()
        {
            if (AlphabetEvents != null)
            {
                if (AlphabetEvents.ContainsVariable())
                {
                    AlphabetsCalculable = false;
                    Alphabets = null;
                }
                else
                {
                    Alphabets = new HashSet<string>(new EventCollection(AlphabetEvents).EventNames);
                    AlphabetsCalculable = true;
                }
            }
            else
            {
                if (AlphabetsCalculable)
                {
                    Alphabets = GetAlphabets(null);
                }
            }
        }

        public HashSet<string> GetAlphabets(Dictionary<string, string> visitedDefinitionRefs)
        {
            HashSet<string> list = new HashSet<string>();

            foreach (Transition transition in Transitions)
            {
                Event evt = transition.Event;
                if (evt.ExpressionList != null)
                {
                    foreach (Expression expression in evt.ExpressionList)
                    {
                        if (expression.HasVar)
                        {
                            StringBuilder sb = new StringBuilder();
                            sb.AppendLine("ERROR - PAT FAILED to calculate the alphabet.");
                            sb.AppendLine("CAUSE - Event " + evt + " contains global variables!");
                            sb.AppendLine(
                                "REMEDY - 1) Avoid using global variables in events 2) Or manually specify the alphabet of the relevant process using the following syntax: \n\r\t #alphabet someProcess {X}; \n\rwhere X is a set of event names.");
                            throw new RuntimeException(sb.ToString());
                        }
                    }
                }

                if (transition.ProgramBlock == null)
                {
                    string name = evt.GetEventID(null);

                    if (name != Constants.TAU) // && !list.Contains(name)
                    {
                        list.Add(name);
                    }
                }
            }

            return list;
        }

        public void SetTransitions(List<Transition> transitions)
        {
            Transitions = transitions;
            foreach (Transition transition in transitions)
            {
                transition.FromState.OutgoingTransitions.Add(transition);
                transition.ToState.IncomingTransition.Add(transition);
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("Process \"" + Name + "\"(" + Common.Classes.Ultility.Ultility.PPStringList(Parameters) + ")[\"" + InitialState + "\"]:");

            foreach (Transition transition in Transitions)
            {
                sb.AppendLine(transition.ToString());
            }
            sb.AppendLine(";");
            return sb.ToString();
        }

        public List<string> GetGlobalVariables()
        {
            List<string> toReturn = new List<string>();
            foreach (Transition transition in Transitions)
            {
                transition.GetGlobalVariables(toReturn);
            }

            foreach (string x in Parameters)
            {
                toReturn.Remove(x);
            }

            return toReturn;
        }

        public void GetChannels(List<string> vars)
        {
            foreach (Transition transition in Transitions)
            {
                transition.GetChannels(vars);
            }
        }

        /// <summary>
        /// Encode the graph
        /// make sure that the local variables' names are unique.
        /// </summary>
        /// <param name="encoder"></param>
        /// <param name="isUsingSimulation">true to use LU simulation in TA module</param>
        /// <returns></returns>
        public AutomataBDD Encode(BDDEncoder encoder, bool isUsingSimulation)
        {
            AutomataBDD processAutomataBDD = new AutomataBDD();

            string processVariableName = this.AddLocalVariables(encoder);

            //Set local variables and initial condition
            SetLocalVarsAndInit(encoder, processAutomataBDD, processVariableName);

            //Encode transitions
            EncodeTransitions(encoder, processAutomataBDD, processVariableName);

            if (isUsingSimulation)
            {
                EncodeSimulationRel(encoder, processAutomataBDD, processVariableName);
            }

            return processAutomataBDD;
        }

        private void EncodeSimulationRel(BDDEncoder encoder, AutomataBDD processBDD, string stateVar)
        {
            int stateVarIndex = encoder.model.GetVarIndex(stateVar);
            CUDDVars rowStateVars = encoder.model.GetRowVars(stateVarIndex);
            CUDDVars colStateVars = encoder.model.GetColVars(stateVarIndex);


            CUDDNode result = CUDD.Constant(0);

            foreach (var pair in mapOldLoc2NewStates)
            {
                List<State> statesInLoc = pair.Value;

                for(int i = 0; i < statesInLoc.Count; i++)
                {
                    for(int j = 0; j < statesInLoc.Count; j++)
                    {
                        if(ClockValuation.IsLUSimulated(mapNewState2Valuation[statesInLoc[i].ID], mapNewState2Valuation[statesInLoc[j].ID], pair.Key))
                        {
                            CUDDNode simulated = CUDD.Matrix.SetVectorElement(CUDD.Constant(0), rowStateVars, encoder.stateIndexOfCurrentProcess[statesInLoc[i].ID], 1);
                            CUDDNode simulating = CUDD.Matrix.SetVectorElement(CUDD.Constant(0), colStateVars, encoder.stateIndexOfCurrentProcess[statesInLoc[j].ID], 1);

                            result = CUDD.Function.Or(result, CUDD.Function.And(simulated, simulating));
                        }
                    }
                }
            }

            processBDD.SimulationRel = result;
        }

        private void EncodeTransitions(BDDEncoder encoder, AutomataBDD processBDD, string stateVar)
        {
            foreach (Transition transition in this.Transitions)
            {
                //Depend on what kind of transition to add the encoded transition to AutomataBDD
                if (transition.Event is ChannelInputEvent)
                {
                    List<CUDDNode> transitionBDDs = transition.Encode(encoder, stateVar, processBDD.variableIndex, true);
                    processBDD.channelInTransitionBDD.AddRange(transitionBDDs);
                }
                else if (transition.Event is ChannelOutputEvent)
                {
                    List<CUDDNode> transitionBDDs = transition.Encode(encoder, stateVar, processBDD.variableIndex, true);
                    processBDD.channelOutTransitionBDD.AddRange(transitionBDDs);
                }
                else if (transition.Event is BDDEncoder.EventChannelInfo && (transition.Event as BDDEncoder.EventChannelInfo).type == BDDEncoder.EventChannelInfo.EventType.SYNC_CHANNEL_INPUT)
                {
                    List<CUDDNode> transitionBDDs = transition.EncodeSyncChannelInTransition(encoder, stateVar, processBDD.variableIndex);
                    processBDD.channelInTransitionBDD.AddRange(transitionBDDs);
                }
                else if (transition.Event is BDDEncoder.EventChannelInfo && (transition.Event as BDDEncoder.EventChannelInfo).type == BDDEncoder.EventChannelInfo.EventType.SYNC_CHANNEL_OUTPUT)
                {
                    List<CUDDNode> transitionBDDs = transition.EncodeSyncChannelOutTransition(encoder, stateVar, processBDD.variableIndex);
                    processBDD.channelOutTransitionBDD.AddRange(transitionBDDs);
                }
                else if (transition.IsTick())
                {
                    List<CUDDNode> transitionBDDs = transition.Encode(encoder, stateVar, processBDD.variableIndex, false);
                    processBDD.Ticks.AddRange(transitionBDDs);
                }
                else
                {
                    List<CUDDNode> transitionBDDs = transition.Encode(encoder, stateVar, processBDD.variableIndex, false);
                    processBDD.transitionBDD.AddRange(transitionBDDs);
                }
            }
        }

        public void SetLocalVarsAndInit(BDDEncoder encoder, AutomataBDD processBDD, string stateVar)
        {
            processBDD.variableIndex.Add(encoder.model.GetVarIndex(stateVar));
            //
            foreach (string parameter in this.Parameters)
            {
                processBDD.variableIndex.Add(encoder.model.GetVarIndex(parameter));
            }

            //Set initial expression
            processBDD.initExpression = Expression.EQ(new Variable(stateVar),
                                                                         new IntConstant(encoder.stateIndexOfCurrentProcess[this.InitialState.ID]));
            for (int i = 0; i < this.Arguments.Count; i++)
            {
                processBDD.initExpression = Expression.AND(processBDD.initExpression,
                                                                             Expression.EQ(new Variable(this.Parameters[i]), this.Arguments[i]));
            }
        }

        /// <summary>
        /// Add local variable including state, and parameters
        /// Return the variable name encoding states
        /// </summary>
        /// <param name="encoder"></param>
        public string AddLocalVariables(BDDEncoder encoder)
        {
            RenameLocalVars();

            for (int i = 0; i < this.Parameters.Count; i++)
            {
                string parameter = this.Parameters[i];

                int min = 0;
                int max = 0;

                if (ParameterUpperBound.ContainsKey(parameter) && ParameterLowerBound.ContainsKey(parameter))
                {
                    min = ParameterLowerBound[parameter];
                    max = ParameterUpperBound[parameter];
                }
                else
                {
                    if(this.Arguments[i] is IntConstant)
                    {
                        IntConstant tempExp = (IntConstant) this.Arguments[i];

                        min = tempExp.Value;
                        max = tempExp.Value;
                    }
                    else
                    {
                        throw new Exception("Symbolic Model Checking only support constant parameters!");
                    }
                }

                //In its old transition encoding, we don't make sure this variable must be unchanged.
                //We also need to add this variable to VaribleIndex of the AutomataBDD because previous process does not know this variable
                //if global then later processes will set this variable as unchange.
                encoder.model.AddLocalVar(parameter, min, max);

            }

            const string STATE = "state";
            //
            string processVariableName = Name + Model.NAME_SEPERATOR + STATE + Model.GetNewTempVarName();
            encoder.model.AddLocalVar(processVariableName, 0, this.States.Count - 1);

            //
            encoder.stateIndexOfCurrentProcess = new Dictionary<string, int>();
            //collect the state index
            foreach (State state in this.States)
            {
                encoder.stateIndexOfCurrentProcess.Add(state.ID, encoder.stateIndexOfCurrentProcess.Count);
            }

            return processVariableName;
        }

        /// <summary>
        /// Rename local variable to be unique
        /// </summary>
        /// <returns></returns>
        private void RenameLocalVars()
        {
            if (Parameters.Count == 0)
            {
                return ;
            }

            //Rename parameter to be unique
            Dictionary<string, Expression> newLocalVariableNameMapping = new Dictionary<string, Expression>();
            List<string> parameters = new List<string>();
            Dictionary<string, int> parameterUpper = new Dictionary<string, int>(this.ParameterUpperBound);
            Dictionary<string, int> parameterLower = new Dictionary<string, int>(this.ParameterLowerBound);

            //
            foreach (string para in this.Parameters)
            {
                //Just build the unique name
                string newName = para + Model.GetNewTempVarName();

                //Update ParameterLowerBound, ParameterUpperBound
                if (parameterUpper.ContainsKey(para))
                {
                    parameterUpper.Add(newName, ParameterUpperBound[para]);
                }

                if (parameterLower.ContainsKey(para))
                {
                    parameterLower.Add(newName, ParameterLowerBound[para]);
                }

                //
                newLocalVariableNameMapping.Add(para, new Variable(newName));
                parameters.Add(newName);
            }

            List<Transition> newTransition = new List<Transition>();
            for (int i = 0; i < Transitions.Count; i++)
            {
                newTransition.AddRange(Transitions[i].ClearConstantExtended(States, newLocalVariableNameMapping));
            }

            this.Parameters = parameters;
            this.ParameterUpperBound = parameterUpper;
            this.ParameterLowerBound = parameterLower;

            this.SetTransitions(newTransition);
        }

        public void CollectEvent(List<string> allEvents)
        {
            foreach (Transition transition in this.Transitions)
            {
                int paraLength = 0;
                if (transition.Event.ExpressionList != null)
                {
                    paraLength = transition.Event.ExpressionList.Length;
                }
                else if (transition.Event.EventID != null && transition.Event.EventID != transition.Event.BaseName)
                {
                    paraLength = transition.Event.EventID.Split('.').Length - 1;
                }

                string eventName = transition.Event.BaseName + Model.NAME_SEPERATOR + paraLength;

                if (!allEvents.Contains(eventName) && transition.Event.BaseName != Constants.TAU && transition.Event.BaseName != Constants.TERMINATION && transition.Event.BaseName != Constants.TOCK)
                {
                    allEvents.Add(eventName);
                }
            }
        }

        /// <summary>
        /// Remove unreachable states and useful transitions
        /// </summary>
        private void RemoveUnreachableStates()
        {
            //get all states that can be reached from initStates by event-transitions which are not labeled with Terminiation
            List<State> reachableStates = new List<State>();
            reachableStates.Add(this.InitialState);
            Stack<State> working = new Stack<State>();
            working.Push(this.InitialState);

            while (working.Count > 0)
            {
                State current = working.Pop();
                List<Transition> trans = new List<Transition>(current.OutgoingTransitions);

                foreach (Transition transition in trans)
                {
                    if (!reachableStates.Contains(transition.ToState))
                    {
                        working.Push(transition.ToState);
                        reachableStates.Add(transition.ToState);
                    }
                }
            }

            List<State> removedState = new List<State>();
            foreach (State state in this.States)
            {
                if (!reachableStates.Contains(state))
                {
                    removedState.Add(state);
                }
            }

            //Remove
            foreach (State state in removedState)
            {
                RemoveState(state);
            }
        }

        #region ultility functions
        public State AddState()
        {
            //make sure new name not conflict with old name
            const string temp = "#-#";
            State toReturn = new State(temp + StateIDCounter.ToString(), temp + StateIDCounter.ToString());

            StateIDCounter++;
            States.Add(toReturn);
            return toReturn;
        }

        public void AddTransition(Transition toAdd)
        {
            Transitions.Add(toAdd);
            toAdd.FromState.OutgoingTransitions.Add(toAdd);
            toAdd.ToState.IncomingTransition.Add(toAdd);
        }

        /// <summary>
        /// Remove transition for list and its source state's outgoing list
        /// </summary>
        /// <param name="toRemove"></param>
        public void RemoveTransition(Transition toRemove)
        {
            Transitions.Remove(toRemove);
            toRemove.FromState.OutgoingTransitions.Remove(toRemove);
            toRemove.ToState.IncomingTransition.Remove(toRemove);
        }

        /// <summary>
        /// Remove state from list
        /// Remove its outgoing transition from list
        /// </summary>
        /// <param name="toRemove"></param>
        public void RemoveState(State toRemove)
        {
            States.Remove(toRemove);
            foreach (var trans in toRemove.OutgoingTransitions)
            {
                Transitions.Remove(trans);
            }

            foreach (var trans in toRemove.IncomingTransition)
            {
                Transitions.Remove(trans);
            }
        }

        public State GetStateByID(string ID)
        {
            foreach (var state in States)
            {
                if(state.ID == ID)
                {
                    return state;
                }
            }
            throw new Exception("State not exists!");
        }
        #endregion ultility functions
    }
}
