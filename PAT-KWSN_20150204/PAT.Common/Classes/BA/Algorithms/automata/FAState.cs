using System;
using System.Collections.Generic;
using PAT.Common.Classes.Assertion.Algorithms.automata;

namespace PAT.Common.Classes.BA.Algorithms.automata
{

    public class FAState : System.IComparable
    {

        public Dictionary<String, HashSet<FAState>> next;
        public Dictionary<String, HashSet<FAState>> pre;
        public int id;


        virtual public int ID
        {
            get
            {
                return id;
            }
        }

        public FAState(int i)
        {
            id = i;
            next = new Dictionary<String, HashSet<FAState>>();
            pre = new Dictionary<String, HashSet<FAState>>();
        }
        ////UPGRADE_ISSUE: The following fragment of code could not be parsed and was not converted. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1156'"
        //public Iterator < String > nextIt()
        ////UPGRADE_ISSUE: The following fragment of code could not be parsed and was not converted. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1156'"
        //public Iterator < String > preIt()

        public override System.String ToString()
        {
            return "S" + id;
        }
        ////UPGRADE_ISSUE: The following fragment of code could not be parsed and was not converted. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1156'"
        //public Set < FAState > getNext(String a)
        ////UPGRADE_ISSUE: The following fragment of code could not be parsed and was not converted. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1156'"
        //public Set < FAState > getPre(String a)

        public virtual void addNext(System.String a, FAState b, FiniteAutomaton auto)
        {
            if (!next.ContainsKey(a))
            {
                //UPGRADE_ISSUE: The following fragment of code could not be parsed and was not converted. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1156'"
                HashSet<FAState> S = new HashSet<FAState>();
                S.Add(b);
                next.Add(a, S);
            }
            else
                next[a].Add(b);
        }
        /// <param name="s">sate
        /// </param>
        /// <returns> if the set of out-going transitions of this state covers that of s 
        /// </returns>
        public virtual bool covers(FAState s)
        {
            foreach (string key in s.next.Keys)
            {
                if (!next.ContainsKey(key))
                {
                    return false;
                }
            }

            return true;

            ////UPGRADE_NOTE: There is an untranslated Statement.  Please refer to original code. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1153'"
            //while (it.hasNext())
            //{
            //    System.String ss = it.next();
            //    if (!next.containsKey(ss))
            //        return false;
            //}
            //return true;
        }

        public void addPre(string a, FAState n)
        {
            if (!pre.ContainsKey(a))
            {
                //UPGRADE_ISSUE: The following fragment of code could not be parsed and was not converted. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1156'"
                pre.Add(a, new HashSet<FAState>() { n });
            }
            else
            {
                pre[a].Add(n);
            }
        }
        public int CompareTo(System.Object obj)
        {
            FAState o = obj as FAState;
            return o.ID - id;
        }
        public override bool Equals(System.Object o)
        {
            return ((FAState)o).ID == id;
        }
        ////UPGRADE_TODO: The following method was automatically generated and it must be implemented in order to preserve the class logic. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1232'"
        //virtual public System.Int32 CompareTo(System.Object obj)
        //{
        //    return 0;
        //}
        //UPGRADE_NOTE: The following method implementation was automatically added to preserve functionality. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1306'"
        //public override int GetHashCode()
        //{
        //    return base.GetHashCode();
        //}
    }
}