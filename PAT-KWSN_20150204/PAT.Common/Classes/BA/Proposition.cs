using System;

namespace PAT.Common.Classes.BA
{
    public sealed class Proposition
    {        
        public String Label;
        public bool Negated;
        public String FullLabel;
        
        public bool IsSigmal
        {
            get { return Label == "\u03A3"; }
        }
		
        public Proposition(bool bool_Renamed, bool isNegated):this(bool_Renamed?"true":"false", isNegated)
        {
        }
		
        public Proposition(String label, bool isNegated)
        {
            this.Label = label;//.Replace(Ultility.Ultility.DOT_PREFIX, ".").Replace(Ultility.Ultility.EVENT_PREFIX, "");
            this.Negated = isNegated;
            this.FullLabel = (this.Negated ? "!" : "") + this.Label;
        }
		
        public override String ToString()
        {
            return this.FullLabel;
        }
    }
}