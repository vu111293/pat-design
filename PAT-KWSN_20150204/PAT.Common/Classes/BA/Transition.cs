using System.Collections.Generic;

namespace PAT.Common.Classes.BA
{
    public sealed class Transition
    {
        public string FromState, ToState;
        public List<Proposition> labels;

        public Transition(string from, string to)
        {
            this.FromState = from;
            this.ToState = to;
        }

        public Transition(List<Proposition> labels, string from, string to)
        {
            this.labels = labels;
            this.FromState = from;
            this.ToState = to;
        }

        public override string ToString()
        {
            return FromState + " --(" + Ultility.Ultility.PPStringList(labels) + ")--> " + ToState;
        }
    }
}