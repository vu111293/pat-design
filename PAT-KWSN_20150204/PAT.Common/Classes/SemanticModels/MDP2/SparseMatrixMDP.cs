using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using PAT.Common.Classes.SemanticModels.DTMC;

namespace PAT.Common.Classes.SemanticModels.MDP2
{
    public class SparseMatrixMDP
    {
        public int Ngroup;
        public int Ncols;

        //number non-zero off-diagonal elements for each row
       // public List<int> succ = new List<int>(); 

        //conbtains a list of groups, each group points to list of rows, means
        //number of groups equal to number of states in SCC and number of rows 
        //equal to the number of distributions in the state
        public List<Group> Groups = new List<Group>(); 


        public SparseMatrixMDP(int nr,int nc)
        {
            Ngroup = nr;
            Ncols = nc;
        }

        //After building up all the groups, it needs to initilize the SelectionMemoryInGroups for each rows. 
        //-1 indicates no specific rows have assigned
        
        

        public override string ToString()
        {
            return Groups.Aggregate("", (current, @group) => current + (@group + "\n"));
        }
    }
}
