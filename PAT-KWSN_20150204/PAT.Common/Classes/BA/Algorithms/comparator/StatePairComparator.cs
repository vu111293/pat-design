using PAT.Common.Classes.BA.Algorithms.automata;
using PAT.Common.Classes.BA.Algorithms.datastructure;

namespace PAT.Common.Classes.BA.Algorithms.comparator
{

	
	public class StatePairComparator  : System.Collections.Generic.IComparer<Pair<FAState, FAState>>
	{
       
		public System.Int32 Compare(Pair < FAState, FAState > o1, Pair < FAState, FAState > o2)
		{
			if (o1.Left.ID > o2.Left.ID)
			{
				return 1;
			}
			else if (o1.Left.ID < o2.Left.ID)
			{
				return - 1;
			}
			else if (o1.Right.ID > o2.Right.ID)
			{
				return 1;
			}
			else if (o1.Right.ID < o2.Right.ID)
			{
				return - 1;
			}
			else
			{
				return 0;
			}
		}
	}
}