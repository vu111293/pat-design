using PAT.Common.Classes.BA.Algorithms.automata;

namespace PAT.Common.Classes.BA.Algorithms.datastructure
{

    public class State_Label : System.IComparable<State_Label>
	{
		virtual public FAState State
		{
			get
			{
				return st;
			}
			
		}
		virtual public System.String Label
		{
			get
			{
				return this.l;
			}
			
		}
		//UPGRADE_ISSUE: The following fragment of code could not be parsed and was not converted. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1156'"
		//< State_Label >
		
		private FAState st;
		private string l;
		
		public State_Label(FAState st, System.String l)
		{
			this.st = st;
			this.l = l;
		}

		//UPGRADE_ISSUE: The following fragment of code could not be parsed and was not converted. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1156'"
		//Override
		public override System.String ToString()
		{
			return "<" + st + ", " + l + ">";
		}
		
        //public virtual int compareTo(State_Label other)
        //{
        //    if (other.st.compareTo(st) != 0)
        //    {
        //        return other.st.compareTo(st);
        //    }
        //    else if (String.CompareOrdinal(other.l, l) != 0)
        //    {
        //        return String.CompareOrdinal(other.l, l);
        //    }
        //    else
        //    {
        //        return 0;
        //    }
        //}
		//UPGRADE_ISSUE: The following fragment of code could not be parsed and was not converted. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1156'"
		//Override
		public override int GetHashCode()
		{
			return st.GetHashCode() + l.GetHashCode();
		}
		//UPGRADE_TODO: The following method was automatically generated and it must be implemented in order to preserve the class logic. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1232'"
        public System.Int32 CompareTo(State_Label other)
        {
            //return 0;
            if (other.st.CompareTo(st) != 0)
            {
                return other.st.CompareTo(st);
            }
            else if (other.l.CompareTo(l) != 0)
            {
                return other.l.CompareTo(l);
            }
            else
            {
                return 0;
            }
		}
	}
}