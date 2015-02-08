namespace PAT.Common.Classes.BA.Algorithms.datastructure
{
	
	public class BASortedSet<T> : System.Collections.Generic.SortedDictionary<T, bool>
	{

        public BASortedSet()
		{
		
		}
		
        public void Add(T item)
        {
            this.Add(item, false);
        }
	}
}