using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PAT.Common.Classes.SemanticModels.DTMC
{
    public class Row
    {
        public double diag; //diagonal element
        public int diagCol; //record diagonal element position
        public List<double> val= new List<double>(); //an array of non-zeror off-diagonal values  //we can also consider diagonal values
        public List<int> col = new List<int>();
        public int colIndexCurrentRow; //record index of pivot position element, only used for matrixMDP, to avoiding multiple calculations. 
        
        public Dictionary<int,int> SelectionMemoryInGroups; //this is only used for MDP setting
        //public Dictionary<Group,Row> SelectionMemoryInGroups; //this is only used for MDP setting
        public int Index; //this is unique when creating a row. The later generated row will reuse its "parents"'s row index. 

        public Row()
        {
        }

        public Row(Row r2)
        {
            diag = r2.diag;
            diagCol = r2.diagCol;
            val = new List<double>(r2.val);
            col = new List<int>(r2.col);

        }

        //don't specify diag, and diagcol
        public Row(List<double> va,List<int> co  )
        {
            val = va;
            col = co;
        }
        
        public Row DeepCopy(Row r2)
        {
            var r = new Row
                        {
                            diag = r2.diag,
                            diagCol = r2.diagCol,
                            val = new List<double>(r2.val),
                            col = new List<int>(r2.col)
                        };
            return r;
        }

        //adding r2 to r1, so diagonal element follow r2
        //assuming r2[r1.diagCol] is zero. This is true in our
        //pivoting calculation/elimination in general
        public static Row Addition (Row r1, Row r2, int denseNcol)
        {
            var r3 = new Row
                         {
                             diag = r1.diag,
                             diagCol = r1.diagCol,
                             col= new List<int>(),
                             val = new List<double>()
                         };
            //auxiliary dense vector helps to record position info
            bool[] auxiDense = new bool[denseNcol];
 
            //register location of nonzeros of r2. For zeros, take value as -1
            for(int i=0; i<r2.col.Count;i++)
            {
                auxiDense[r2.col[i]] = true;
            }

            //add matching nonzeros of r2 and r1 into r3;
            bool flag = false;//to detect diagnal position matching
            for (int i = 0; i < r1.col.Count;i++)
            {
                int j = r1.col[i];
                if(auxiDense[j])
                {
                    r3.col.Add(r1.col[i]);
                    r3.val.Add(r1.val[i]+r2.val[i]);
                }
                auxiDense[j] = false;
                if((!flag) && r1.col[i]==r2.diagCol)
                {
                    r3.col.Add(r1.col[i]);
                    r3.val.Add(r1.val[i] + r2.val[i]);
                    flag = true;
                }
            }
            
            //if diagonal element of r2 doesnot matching 
            if(!flag)
            {
                r3.col.Add(r2.diagCol);
                r3.val.Add(r2.diag);
            }


            for (int i = 0; i < r2.col.Count;i++)
            {
                int j = r2.col[i];
                if(auxiDense[j])
                {
                    r3.col.Add(r2.diagCol);
                    r3.val.Add(r2.diag);
                }
                auxiDense[j] = false;
            }

            return r3;
        } 

        
        public static Row Addition2 (Row r1, Row r2)
        {
            var hashset1 = new HashSet<int>(r1.col);
            hashset1.UnionWith(new HashSet<int>(r2.col));
            var r3 = new Row
            {
                diag = r1.diag,
                diagCol = r1.diagCol,
                //col = hashset1.ToList(),
                //val = new List<double>(hashset1.Count)

            };

           var mapCol2ValIndex = new SortedDictionary<int, double>();


            foreach (int i in hashset1)
            {
                mapCol2ValIndex.Add(i,0);
            }

            //copy r2 to r3
            for (int i = 0; i < r2.col.Count; i++)
            {
                mapCol2ValIndex[r2.col[i]]+=r2.val[i];
            }

            //copy r1 to r3
            for (int i = 0; i < r1.col.Count; i++)
            {
                mapCol2ValIndex[r1.col[i]] += r1.val[i];
            }
            r3.col = new List<int>(mapCol2ValIndex.Keys);
            r3.val = new List<double>(mapCol2ValIndex.Values);
            r3.diag = r3.col.Contains(r3.diagCol) ? mapCol2ValIndex[r3.diagCol] : 0;

            return r3;

            //var hashset1 = new HashSet<int> (r1.col);
            //hashset1.UnionWith(new HashSet<int>(r2.col));
            //var r3 = new Row
            //{
            //    diag = r1.diag,
            //    diagCol = r1.diagCol,
            //    col = hashset1.ToList(),
            //    val = new List<double>(hashset1.Count)

            //};

            //var mapCol2ValIndex = new SortedDictionary<int, int>();
            
            
            //for (int i = 0; i < r3.col.Count;i++)
            //{
            //    mapCol2ValIndex.Add(r3.col[i],i);
            //    r3.val.Add(0); //initialization
            //}

            ////copy r2 to r3
            //for (int i = 0; i < r2.col.Count;i++)
            //{
            //    int j = r2.col[i];
            //    int k = mapCol2ValIndex[j];
            //    r3.val[k] += r2.val[i];
            //}

            ////copy r1 to r3
            //for (int i = 0; i < r1.col.Count; i++)
            //{
            //    int j = r1.col[i];
            //    r3.val[mapCol2ValIndex[j]] += r1.val[i];
            //}

            //r3.diag = r3.col.Contains(r3.diagCol) ? r3.val[mapCol2ValIndex[r3.diagCol]] : 0;

            //return r3;

        }

        public override string  ToString()
        {
            string str = "";
            for (int i = 0; i < col.Count;i++)
            {
                str += col[i] + " -> " + val[i] + ";\t";
            }

            //foreach (KeyValuePair<int, int> selectionMemoryInGroup in SelectionMemoryInGroups)
            //{
            //    str += "(" + selectionMemoryInGroup.Key + "," + selectionMemoryInGroup.Value + "),";
            //}

            return str;
        }
    }
}
