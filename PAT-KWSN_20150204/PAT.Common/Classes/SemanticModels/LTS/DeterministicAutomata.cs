using System;
using System.Collections.Generic;

namespace PAT.Common.Classes.DataStructure
{
    public class DeterministicAutomata
    {
        public Dictionary<int, DeterministicFAState> States;
        public DeterministicFAState InitialState;
        public HashSet<String> Alphabet;

        public DeterministicAutomata()
        {
            States = new Dictionary<int, DeterministicFAState>();
            Alphabet = new HashSet<String>();
        }

        public DeterministicFAState GetInitState()
        {
            return InitialState;
        }

        public void SetIsUnstable(int id, bool isUnstable)
        {
            States[id].IsUnstable = isUnstable;
        }

        public DeterministicFAState AddInitialState()
        {
            DeterministicFAState toReturn = new DeterministicFAState(0);
            States.Add(0, toReturn);
            InitialState = toReturn;
            return toReturn;
        }

        public DeterministicFAState AddState ()
        {
            int id = States.Count;
            DeterministicFAState toReturn = new DeterministicFAState(States.Count);
            States.Add(id, toReturn);
            return toReturn;
        }

        public void AddTransition(DeterministicFAState source, string evt, DeterministicFAState target)
        {
            source.AddTransition(evt,target);
        }

        public override string ToString()
        {
            string toReturn = "";

            foreach (KeyValuePair<int, DeterministicFAState> keyValuePair in States)
            {
                toReturn += keyValuePair.Value.ToString();
            }

            return toReturn;
        }
    }

    public class DeterministicFAState
    {
        public Dictionary<string, DeterministicFAState> Post;
        public int ID;
        public bool IsUnstable;
        public bool IsDivergent;
        public List<List<string>> NegatedRefusals;

        public DeterministicFAState(int id)
        {
            ID = id;
            Post = new Dictionary<string, DeterministicFAState>();
        }

        public override string ToString()
        {
            string toReturn = "";

            foreach (KeyValuePair<string, DeterministicFAState> keyValuePair in Post)
            {
                    toReturn += ID + " -" + keyValuePair.Key + "-> " + keyValuePair.Value.ID + "\r\n";
            }

            return toReturn;
        }

        /// <summary>
        /// return true iff deterministic
        /// </summary>
        /// <param name="evt"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public void AddTransition (string evt, DeterministicFAState target)
        {
            Post.Add(evt, target);
        }

        public DeterministicFAState Next(string evt)
        {
            if (Post.ContainsKey(evt))
            {
                return Post[evt];
            }

            return null;
        }

        public int GetID ()
        {
            return ID;
        }

        public void SetNegatedRefusals (List<List<string>> negatedRefusals)
        {
            NegatedRefusals = negatedRefusals;
        }
    }


    //these classes are used to check subset relation between the deterministic Automata, and used for Probabilistic 
    //refinement checking
    public class DeterministicAutomata_Subset
    {
        public Dictionary<int, DeterministicFAState_Subset> States;
        public DeterministicFAState_Subset InitialState;
        public HashSet<String> Alphabet;

        public DeterministicAutomata_Subset()
        {
            States = new Dictionary<int, DeterministicFAState_Subset>();
            Alphabet = new HashSet<String>();
        }

        public DeterministicFAState_Subset GetInitState()
        {
            return InitialState;
        }

        public void SetIsUnstable(int id, bool isUnstable)
        {
            States[id].IsUnstable = isUnstable;
        }

        public DeterministicFAState_Subset AddInitialState()
        {
            DeterministicFAState_Subset toReturn = new DeterministicFAState_Subset(0);
            States.Add(0, toReturn);
            InitialState = toReturn;
            return toReturn;
        }

        public DeterministicFAState_Subset AddState()
        {
            int id = States.Count;
            DeterministicFAState_Subset toReturn = new DeterministicFAState_Subset(States.Count);
            States.Add(id, toReturn);
            return toReturn;
        }

        public void AddTransition(DeterministicFAState_Subset source, string evt, DeterministicFAState_Subset target)
        {
            source.AddTransition(evt, target);
        }

        public override string ToString()
        {
            string toReturn = "";

            foreach (KeyValuePair<int, DeterministicFAState_Subset> keyValuePair in States)
            {
                toReturn += keyValuePair.Value.ToString();
            }

            return toReturn;
        }
    }
    public class DeterministicFAState_Subset
    {
        public Dictionary<string, DeterministicFAState_Subset> Post;
        public int ID;
        public bool IsUnstable;
        public bool IsDivergent;
        public List<List<string>> NegatedRefusals;
        public HashSet<DeterministicFAState_Subset> Sub;

        public DeterministicFAState_Subset(int id)
        {
            ID = id;
            Post = new Dictionary<string, DeterministicFAState_Subset>();
            Sub = new HashSet<DeterministicFAState_Subset>();
        }

        public override string ToString()
        {
            string toReturn = "";

            foreach (KeyValuePair<string, DeterministicFAState_Subset> keyValuePair in Post)
            {
                toReturn += ID + " -" + keyValuePair.Key + "-> " + keyValuePair.Value.ID + "\r\n";
            }

            return toReturn;
        }

        /// <summary>
        /// return true iff deterministic
        /// </summary>
        /// <param name="evt"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public void AddTransition(string evt, DeterministicFAState_Subset target)
        {
            Post.Add(evt, target);
        }

        public DeterministicFAState_Subset Next(string evt)
        {
            if (Post.ContainsKey(evt))
            {
                return Post[evt];
            }

            return null;
        }

        public int GetID()
        {
            return ID;
        }

        public void SetNegatedRefusals(List<List<string>> negatedRefusals)
        {
            NegatedRefusals = negatedRefusals;
        }
    }
}
