using System.Collections.Generic;
using PAT.Common.Classes.Expressions.ExpressionClass;

namespace PAT.Common.Classes.SemanticModels.LTS.BDD
{
    public class State
    {
        public List<Transition> OutgoingTransitions = new List<Transition>();
        public List<Transition> IncomingTransition = new List<Transition>();

        public string Name;
        public bool HavePriority = false;
        public string Label;

        /// <summary>
        /// Integer type
        /// </summary>
        public string ID;

        public State(string name, string id)
        {
            Name = name;
            ID = id;
        }

        public State(string name, string id, bool HavePriority)
        {
            Name = name;
            ID = id;
            this.HavePriority = HavePriority;
        }

        public State(string name, string id, string label)
        {
            Name = name;
            ID = id;
            Label = label;
        }

        public State ClearConstant(Dictionary<string, Expression> constMapping)
        {
            return new State(Name, ID);
        }

        public override string ToString()
        {
            return Name;
        }

        public override int GetHashCode()
        {
            return ID.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return ID == (obj as State).ID;
        }

        public void AddTransition (Transition tran)
        {
            if (OutgoingTransitions == null)
            {
                OutgoingTransitions = new List<Transition>();
            }

            OutgoingTransitions.Add(tran);
        }
    }
}
