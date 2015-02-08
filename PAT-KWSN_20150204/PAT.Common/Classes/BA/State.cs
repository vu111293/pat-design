namespace PAT.Common.Classes.BA
{
    public sealed class State
    {
        public string Name;
        public bool isFinal;
        public bool isInitial;

        public State(string name, bool final, bool initial)
        {
            Name = name;
            isFinal = final;
            isInitial = initial;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}