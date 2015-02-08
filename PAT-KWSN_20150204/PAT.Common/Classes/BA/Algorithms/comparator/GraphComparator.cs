using PAT.Common.Classes.BA.Algorithms.datastructure;

namespace PAT.Common.Classes.BA.Algorithms.comparator
{


    //public class GraphComparator implements Comparator<TreeSet<Arc>> {
    public class GraphComparator : System.Collections.Generic.IComparer<BASortedSet<Arc>>
	{
		public GraphComparator()
		{
			//InitBlock();
		}
        //private void  InitBlock()
        //{
        //    return arg0.toString().compareTo(arg1.toString());
        //}
		//UPGRADE_ISSUE: The following fragment of code could not be parsed and was not converted. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1156'"
		//< TreeSet < Arc >>
		//public int compare;
		//UPGRADE_ISSUE: The following fragment of code could not be parsed and was not converted. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1156'"
		//(TreeSet < Arc > arg0, TreeSet < Arc > arg1)
		//UPGRADE_TODO: The following method was automatically generated and it must be implemented in order to preserve the class logic. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1232'"

        public int Compare(BASortedSet<Arc> x, BASortedSet<Arc> y)
		{
            return x.ToString().CompareTo(y.ToString());
		}
	}
}