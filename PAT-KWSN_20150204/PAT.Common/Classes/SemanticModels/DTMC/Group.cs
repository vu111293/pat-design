using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PAT.Common.Classes.SemanticModels.DTMC
{
    public class Group
    {
        public HashSet<Row> RowsInSameGroup = new HashSet<Row>();
        //GL:
        public int Index;
        public override string ToString()
        {
            return RowsInSameGroup.Aggregate("", (current, row) => current + (row + "\n"));
        }


    }
}
