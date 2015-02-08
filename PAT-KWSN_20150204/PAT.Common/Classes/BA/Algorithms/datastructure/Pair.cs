using System;

namespace PAT.Common.Classes.BA.Algorithms.datastructure
{
	
	public class Pair<E1, E2> 
	{

		virtual public E1 Left
		{
			get
			{
				return this.e1;
			}
			
		}
		virtual public E2 Right
		{
			get
			{
				return this.e2;
			}
			
		}
		//UPGRADE_ISSUE: The following fragment of code could not be parsed and was not converted. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1156'"
		
		
		private E1 e1;
		
		private E2 e2;
		
		public Pair(E1 e1, E2 e2)
		{
			
			this.e1 = e1;
			this.e2 = e2;
		}
		//UPGRADE_ISSUE: The following fragment of code could not be parsed and was not converted. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1156'"
		//Override
		public override System.String ToString()
		{
			System.String str1 = this.e1.Equals(default(E1)) ?"null":this.e1.ToString();
			System.String str2 = this.e2.Equals(default(E2)) ?"null":this.e2.ToString();
			return "(" + str1 + ", " + str2 + ")";
		}
		//UPGRADE_ISSUE: The following fragment of code could not be parsed and was not converted. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1156'"
		//Override
		public override int GetHashCode()
		{
			return this.e1.GetHashCode() ^ this.e2.GetHashCode() * 29;
		}

        public override bool Equals(object obj)
        {
            try
            {
                Pair<E1, E2> temp = obj as Pair<E1, E2>;
                if(temp != null)
                {
                    return this.e1.Equals(temp.Left) && this.e2.Equals(temp.Right);    
                }
                
            }
            catch (InvalidCastException e)
            {
                //return false;
            }

            return false;
        }
		//UPGRADE_ISSUE: The following fragment of code could not be parsed and was not converted. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1156'"
		
	}
}