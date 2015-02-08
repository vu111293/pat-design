using System;
using System.Collections.Generic;
using System.Text;
using PAT.Common.Classes.Ultility;

namespace PAT.Common.Classes.DataStructure
{
    public class Automata
    {
        public Dictionary<int, FAState> States;
        public HashSet<FAState> InitialState;
        public HashSet<String> Alphabet;

        public Automata()
        {
            States = new Dictionary<int, FAState>();
            InitialState = new HashSet<FAState>();
            Alphabet = new HashSet<String>();
        }

        public void SetInitialState (FAState init)
        {
            InitialState.Add(init);
        }

        public FAState AddState()
        {
            int id = States.Count;
            FAState newState = new FAState(id);
            States.Add(id, newState);
            return newState;
        }

        /// <summary>
        /// Add a transition into the automata
        /// </summary>
        /// <param name="source"></param>
        /// <param name="evt"></param>
        /// <param name="target"></param>
        /// <returns>Whether the target is a new state</returns>
        public void AddTransition(FAState source, string evt, FAState target)
        {
            source.AddTransition(evt, target);
        }

        public override string ToString()
        {
            StringBuilder toReturn = new StringBuilder();

            foreach (KeyValuePair<int, FAState> keyValuePair in States)
            {
                FAState state = keyValuePair.Value;

                foreach (KeyValuePair<string, HashSet<FAState>> item in state.Post)
                {
                    foreach (FAState st in item.Value)
                    {
                        toReturn.AppendLine(state.ToString() + "-"+ item.Key + "->" + st.ToString());
                    }
                }
            }

            return toReturn.ToString();
        }

        public DeterministicAutomata DeterminizeWithRefusalsAndDiv()
        {
            Dictionary<string, DeterministicFAState> Visited = new Dictionary<string, DeterministicFAState>(Ultility.Ultility.MC_INITIAL_SIZE);
            Stack<NormalizedFAState> pending = new Stack<NormalizedFAState>(1024);

            NormalizedFAState current = (new NormalizedFAState(InitialState)).TauReachable();
            pending.Push(current);

            DeterministicAutomata toReturn = new DeterministicAutomata();
            Visited.Add(current.GetID(), toReturn.AddInitialState());

            while (pending.Count > 0)
            {
                current = pending.Pop();
                DeterministicFAState currentState = Visited[current.GetID()];

                if (current.IsDiv())
                {
                    toReturn.InitialState.IsDivergent = true;
                }
                else
                {
                    currentState.SetNegatedRefusals(current.GetFailuresNegate());

                    Dictionary<string, HashSet<FAState>> nexts = new Dictionary<string, HashSet<FAState>>();

                    foreach (FAState state in current.States)
                    {
                        foreach (KeyValuePair<string, HashSet<FAState>> pair in state.Post)
                        {
                            if (pair.Key != Constants.TAU)
                            {

                                HashSet<FAState> states;
                                if (!nexts.TryGetValue(pair.Key, out states))
                                {
                                    states = new HashSet<FAState>();
                                    nexts.Add(pair.Key, states);
                                }

                                foreach (FAState faState in pair.Value)
                                {
                                    states.Add(faState);
                                }
                            }
                        }
                    }

                    foreach (KeyValuePair<string, HashSet<FAState>> keyValuePair in nexts)
                    {
                        NormalizedFAState next = new NormalizedFAState(keyValuePair.Value);
                        NormalizedFAState newState = next.TauReachable();

                        DeterministicFAState target;
                        if (!Visited.TryGetValue(newState.GetID(), out target))
                        {
                            target = toReturn.AddState();
                            Visited.Add(newState.GetID(), target);
                            pending.Push(newState);
                        }

                        toReturn.AddTransition(currentState, keyValuePair.Key, target);
                    }                    
                }
            }

            return toReturn;
        }

        public DeterministicAutomata DeterminizeWithRefusals()
        {
            Dictionary<string, DeterministicFAState> Visited = new Dictionary<string, DeterministicFAState>(Ultility.Ultility.MC_INITIAL_SIZE);
            Stack<NormalizedFAState> pending = new Stack<NormalizedFAState>(1024);

            NormalizedFAState current = (new NormalizedFAState(InitialState)).TauReachable();
            pending.Push(current);

            DeterministicAutomata toReturn = new DeterministicAutomata();
            Visited.Add(current.GetID(), toReturn.AddInitialState());
            toReturn.InitialState.SetNegatedRefusals(current.GetFailuresNegate());

            while (pending.Count > 0)
            {
                current = pending.Pop();
                DeterministicFAState currentState = Visited[current.GetID()];

                Dictionary<string, HashSet<FAState>> nexts = new Dictionary<string, HashSet<FAState>>();

                foreach (FAState state in current.States)
                {
                    foreach (KeyValuePair<string, HashSet<FAState>> pair in state.Post)
                    {
                        if (pair.Key != Constants.TAU)
                        {
                            HashSet<FAState> states;
                            if (!nexts.TryGetValue(pair.Key, out states))
                            {
                                states = new HashSet<FAState>();
                                nexts.Add(pair.Key, states);
                            }

                            foreach (FAState faState in pair.Value)
                            {
                                states.Add(faState);
                            }
                        }
                    }
                }

                foreach (KeyValuePair<string, HashSet<FAState>> keyValuePair in nexts)
                {
                    NormalizedFAState next = new NormalizedFAState(keyValuePair.Value);
                    NormalizedFAState newState = next.TauReachable();

                    DeterministicFAState target;
                    if (!Visited.TryGetValue(newState.GetID(), out target))
                    {
                        target = toReturn.AddState();
                        target.SetNegatedRefusals(newState.GetFailuresNegate());
                        Visited.Add(newState.GetID(), target);
                        pending.Push(newState);
                    }

                    toReturn.AddTransition(currentState, keyValuePair.Key, target);
                }
            }

            return toReturn;
        }

        public DeterministicAutomata Determinize()
        {
            Dictionary<string, DeterministicFAState> Visited = new Dictionary<string, DeterministicFAState>(Ultility.Ultility.MC_INITIAL_SIZE);
            Stack<NormalizedFAState> pending = new Stack<NormalizedFAState>(1024);

            NormalizedFAState current = (new NormalizedFAState(InitialState)).TauReachable();
            pending.Push(current);

            DeterministicAutomata toReturn = new DeterministicAutomata();
            Visited.Add(current.GetID(), toReturn.AddInitialState());

            while (pending.Count > 0)
            {
                current = pending.Pop();
                DeterministicFAState currentState = Visited[current.GetID()];

                Dictionary<string, HashSet<FAState>> nexts = new Dictionary<string, HashSet<FAState>>();

                foreach (FAState state in current.States)
                {
                    foreach (KeyValuePair<string, HashSet<FAState>> pair in state.Post)
                    {
                        if (pair.Key != Constants.TAU)
                        {
                            HashSet<FAState> states;
                            if (!nexts.TryGetValue(pair.Key, out states))
                            {
                                states = new HashSet<FAState>();
                                nexts.Add(pair.Key, states);
                            }

                            foreach (FAState faState in pair.Value)
                            {
                                states.Add(faState);
                            }                            
                        }
                    }
                }

                foreach (KeyValuePair<string, HashSet<FAState>> keyValuePair in nexts)
                {
                    NormalizedFAState next = new NormalizedFAState(keyValuePair.Value);
                    NormalizedFAState newState = next.TauReachable();

                    DeterministicFAState target;
                    if (!Visited.TryGetValue(newState.GetID(), out target))
                    {
                        target = toReturn.AddState();
                        Visited.Add(newState.GetID(), target);
                        pending.Push(newState);
                    }

                    toReturn.AddTransition(currentState, keyValuePair.Key, target);
                }
            }

            return toReturn;
        }

        public DeterministicAutomata_Subset DeterminizeSubset()
        {
            Dictionary<string, DeterministicFAState_Subset> Visited = new Dictionary<string, DeterministicFAState_Subset>(Ultility.Ultility.MC_INITIAL_SIZE);
            HashSet<NormalizedFAState> VisitedNormalizedState = new HashSet<NormalizedFAState>();
            Stack<NormalizedFAState> pending = new Stack<NormalizedFAState>(1024);

            NormalizedFAState current = (new NormalizedFAState(InitialState)).TauReachable();
            VisitedNormalizedState.Add(current);
            pending.Push(current);

            DeterministicAutomata_Subset toReturn = new DeterministicAutomata_Subset();
            Visited.Add(current.GetID(), toReturn.AddInitialState());

            while (pending.Count > 0)
            {
                current = pending.Pop();
                DeterministicFAState_Subset currentState = Visited[current.GetID()];

                Dictionary<string, HashSet<FAState>> nexts = new Dictionary<string, HashSet<FAState>>();

                foreach (FAState state in current.States)
                {
                    foreach (KeyValuePair<string, HashSet<FAState>> pair in state.Post)
                    {
                        if (pair.Key != Constants.TAU)
                        {
                            HashSet<FAState> states;
                            if (!nexts.TryGetValue(pair.Key, out states))
                            {
                                states = new HashSet<FAState>();
                                nexts.Add(pair.Key, states);
                            }

                            foreach (FAState faState in pair.Value)
                            {
                                states.Add(faState);
                            }
                        }
                    }
                }

                foreach (KeyValuePair<string, HashSet<FAState>> keyValuePair in nexts)
                {
                    NormalizedFAState next = new NormalizedFAState(keyValuePair.Value);
                    NormalizedFAState newState = next.TauReachable();
                    
                    //foreach (NormalizedFAState state in )
                    //{
                    //    if(newState.States.IsProperSupersetOf(state.States))
                    //    {
                    //        newState.sub.Add(state);
                    //    }
                    //}

                    DeterministicFAState_Subset target;
                    if (!Visited.TryGetValue(newState.GetID(), out target))
                    {
                        VisitedNormalizedState.Add(newState);
                        target = toReturn.AddState();
                        Visited.Add(newState.GetID(), target);
                        //foreach(var deterministicFaState in Visited)
                        //{
                            
                        //}
                        pending.Push(newState);
                    }

                    toReturn.AddTransition(currentState, keyValuePair.Key, target);

                    foreach(var nstate in VisitedNormalizedState)
                    {
                        if(newState.States.IsProperSupersetOf(nstate.States))
                        {
                            Visited[newState.GetID()].Sub.Add(Visited[nstate.GetID()]);
                        }
                        else if(newState.States.IsProperSubsetOf(nstate.States))
                        {
                            Visited[nstate.GetID()].Sub.Add(Visited[newState.GetID()]);
                        }

                    }
                }
            }

            return toReturn;
        }
    }

    public struct NormalizedFAState
    {
        public HashSet<FAState> States;
        private String ID;
        //public HashSet<NormalizedFAState> sub;
        public NormalizedFAState(FAState faState)
        {
            States = new HashSet<FAState>();
            States.Add(faState);
            ID = null;
            //sub = new HashSet<NormalizedFAState>();
        }

        public NormalizedFAState(HashSet<FAState> faStates)
        {
            States = faStates;
            ID = null;
            //sub = new HashSet<NormalizedFAState>();
        }

        public bool IsDiv()
        {
            foreach (FAState faState in States)
            {
                if (faState.IsDiv)
                {
                    return true;
                }
            }

            return false;
        }

        public NormalizedFAState NextWithTauReachable(string evt)
        {
            HashSet<FAState> toReturn = new HashSet<FAState>();

            foreach (FAState st in States)
            {
                HashSet<FAState> next = st.Next(evt);

                if (next != null)
                {
                    foreach (FAState state in next)
                    {
                        toReturn.Add(state);
                    }
                }
            }

            Stack<FAState> working = new Stack<FAState>();
            Dictionary<int,bool> visited = new Dictionary<int,bool>(toReturn.Count * 5);

            foreach (FAState state in toReturn)
            {
                 working.Push(state);
                 visited.Add(state.ID, false);
            }

            while (working.Count > 0)
            {
                HashSet<FAState> nextStates = working.Pop().Next(Constants.TAU);

                if (nextStates != null)
                {
                    foreach (FAState next in nextStates)
                    {
                        if (!visited.ContainsKey(next.ID))
                        {
                            visited.Add(next.ID, false);
                            working.Push(next);
                            toReturn.Add(next);
                        }
                    }
                }
            }

            return new NormalizedFAState(toReturn);
        }

        public List<List<string>> GetFailuresNegate()
        {
            List<List<string>> toReturn = new List<List<string>>();
            foreach (FAState state in States)
            {
                if (!state.IsUnstable())
                {
                    toReturn.Add(state.NegatedRefusal);
                }
            }

            return toReturn;
        }

        public NormalizedFAState TauReachable()
        {
            Stack<FAState> working = new Stack<FAState>();
            Dictionary<int,bool> visited = new Dictionary<int,bool>(States.Count * 5);
            HashSet<FAState> toReturn = new HashSet<FAState>();
            

            foreach (FAState state in States)
            {
                if (!visited.ContainsKey(state.ID))
                {
                    working.Push(state);
                    visited.Add(state.ID,false);
                    toReturn.Add(state);
                }
            }

            while (working.Count > 0)
            {
                HashSet<FAState> nextStates = working.Pop().Next(Constants.TAU);

                if (nextStates != null)
                {
                    foreach (FAState next in nextStates)
                    {
                        if (!visited.ContainsKey(next.ID))
                        {
                            visited.Add(next.ID,false);
                            working.Push(next);
                            toReturn.Add(next);
                        }
                    }
                }
            }

            return new NormalizedFAState(toReturn);
        }

        public string GetID()
        {
            if (ID != null)
            {
                return ID;
            }

            if (States == null || States.Count == 0)
            {
                return "";
            }

            List<int> IDs = new List<int>();

            foreach (FAState state in States)
            {
                IDs.Add(state.ID);
            }

            IDs.Sort();

            StringBuilder toReturn = new StringBuilder(); 
            toReturn.Append(IDs[0]);

            for (int i = 1; i < IDs.Count; i++)
            {
                toReturn.Append(Constants.SEPARATOR);
                toReturn.Append(IDs[i]);
            }

            ID = toReturn.ToString();
            return ID;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            foreach (FAState item in States)
            {
                sb.Append(item.ToString() + ",");
            }

            return sb.ToString();
        }

    }

    public class FAState
    {
        public Dictionary<string, HashSet<FAState>> Pre;
        public Dictionary<string, HashSet<FAState>> Post;
        public int ID;
        public bool IsDiv;
        public List<string> NegatedRefusal; 

        public FAState(int id)
        {
            ID = id;
            Pre = new Dictionary<string, HashSet<FAState>>();
            Post = new Dictionary<string, HashSet<FAState>>();
        }

        public bool IsUnstable ()
        {
            return NegatedRefusal == null;
        }

        public override string ToString()
        {
            return ID.ToString();
        }

        public override bool Equals(object obj)
        {
            return (obj as FAState).ID == ID;
        }

        public override int GetHashCode()
        {
            return ID;
        }

        public void SetNegatedReusal(List<string> negatedRefusal)
        {
            NegatedRefusal = negatedRefusal;
        }

        public void AddTransition (string evt, FAState target)
        {
            if (Post.ContainsKey(evt))
            {
                Post[evt].Add(target);
            }
            else
            {
                HashSet<FAState> states = new HashSet<FAState>();
                states.Add(target);
                Post.Add(evt, states);
            }

            if (target.Pre.ContainsKey(evt))
            {
                target.Pre[evt].Add(target);
            }
            else
            {
                HashSet<FAState> states = new HashSet<FAState>();
                states.Add(target);
                target.Pre.Add(evt, states);
            }
        }

        public HashSet<FAState> Next(string evt)
        {
            if (Post.ContainsKey(evt))
            {
                return Post[evt];
            }
            else
            {
                return null;
            }
        }
    }
}
