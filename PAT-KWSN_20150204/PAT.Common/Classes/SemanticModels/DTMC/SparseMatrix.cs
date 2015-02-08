using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PAT.Common.Classes.SemanticModels.DTMC;

namespace PAT.Common.Classes.SemanticModels.DTMC
{
    public class SparseMatrix
    {
        public int Nrows;
        public int Ncols;

        //number non-zero off-diagonal elements for each row
       // public List<int> succ = new List<int>(); 

        //row structure 
        public List<Row> Rows = new List<Row>(); 


        public SparseMatrix(int nr,int nc)
        {
            Nrows = nr;
            Ncols = nc;
        }

        //public void AddRow(Row r, int noOffDiag)
        //{
        //    //succ.Add(noOffDiag);
        //    rows.Add(r);
        //}

        public override string ToString()
        {
            return Rows.Aggregate("", (current, row) => current + (row.ToString() + "\n"));
        }
    }
}
