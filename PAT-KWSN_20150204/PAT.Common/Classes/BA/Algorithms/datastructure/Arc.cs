namespace PAT.Common.Classes.BA.Algorithms.datastructure
{

    public class Arc : System.IComparable<Arc>

	{
		virtual public int From
		{
			get
			{
				return this.e1;
			}
			
		}
		virtual public int To
		{
			get
			{
				return this.e2;
			}
			
		}
		virtual public bool Label
		{
			get
			{
				return this.l;
			}
			
		}
		//UPGRADE_ISSUE: The following fragment of code could not be parsed and was not converted. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1156'"
		//< Arc >
		
		private int e1, e2;
		private bool l;
		
		public Arc(int e1, bool l, int e2)
		{
			this.e1 = e1;
			this.e2 = e2;
			this.l = l;
		}
		//UPGRADE_ISSUE: The following fragment of code could not be parsed and was not converted. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1156'"
		//Override
		public override string ToString()
		{
			return "<" + e1 + ", " + l + ", " + e2 + ">";
		}

        public int CompareTo(Arc other)
		{
			if (other.e1 != e1)
			{
				return other.e1 - e1;
			}
			else if (other.e2 != e2)
			{
				return other.e2 - e2;
			}
			else if (other.l != l)
			{
				if (other.l)
					return 1;
				else
					return - 1;
			}
			else
			{
				return 0;
			}
		}

        ////UPGRADE_TODO: The following method was automatically generated and it must be implemented in order to preserve the class logic. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1232'"
        //virtual public System.Int32 CompareTo(System.Object obj)
        //{
        //    return 0;
        //}
	}
}