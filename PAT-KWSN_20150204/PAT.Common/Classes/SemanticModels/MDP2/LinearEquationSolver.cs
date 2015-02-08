using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using PAT.Common.Classes.SemanticModels.MDP;

//using MathWorks.MATLAB.NET.Arrays;
//using InverseMul;

namespace PAT.Common.Classes.SemanticModels.MDP2
{
    public class LinearEquationsSolver
    {
        // public const double EPSILON=0.00001;
        public const double EPSILON = 0.000001;

        /// <summary>
        /// read scc and output to sparse matrix structure
        /// </summary>
        /// <param name="scc">scc states</param>
        /// <param name="output">output states</param>
        /// <returns></returns>
        public static SparseMatrixMDP Read2MatrixToSparseMatrix(List<MDPState> scc, List<MDPState> output)
        {
            int inputSize = scc.Count;
            int outputSize = output.Count;

            int column = inputSize + outputSize;

            var matrix = new SparseMatrixMDP(inputSize, column);

            var dtmcState2Number = new Dictionary<MDPState, int>(column);

            int i = 0;
            foreach (MDPState mdpState in scc)
            {
                dtmcState2Number.Add(mdpState, i);
                i++;
            }
            foreach (MDPState mdpState in output)
            {
                dtmcState2Number.Add(mdpState, i);
                i++;
            }

            //assign rows to matrix
          var groupIndex = 0;

            //foreach (MDPState dtmcState in scc)
            //{
              //  var rowCoord = dtmcState2Number[dtmcState];
            for(int rowCoord=0;rowCoord<scc.Count;rowCoord++)
            {   //construct group block
                MDPState dtmcState = scc[rowCoord];
                var group = new Group(){Index = groupIndex};
                
                int rowIndex = 0;

                
                for (int i2 = dtmcState.Distributions.Count - 1; i2 >= 0; i2--)
                {
                    var dis = dtmcState.Distributions[i2];
                    var compressedRow = new SortedDictionary<int, double> {{rowCoord, 1}};
                    foreach (KeyValuePair<double, MDPState> pair in dis.States)
                    {

                        int colCoord = dtmcState2Number[pair.Value];
                        if (colCoord == rowCoord)
                        {
                            compressedRow[rowCoord] += -pair.Key;
                        }
                        else
                        {
                            compressedRow.Add(colCoord, -pair.Key);
                        }
                    }

                    var row = new Row(new List<double>(compressedRow.Values), new List<int>(compressedRow.Keys))
                                  {
                                      Index = rowIndex,
                                      SelectionMemoryInGroups = new Dictionary<int, int> { { groupIndex, rowIndex } },
                                      WorkingGroup = new BitArray(inputSize),
                                      diag = 1,
                                      diagCol = groupIndex,
                                  };
                    row.WorkingGroup.Set(groupIndex,true);
                        //TODO: set dia, diagCol might be redundant.
                    rowIndex = rowIndex + 1;
                    group.RowsInSameGroup.Add(row);
                    //dtmcState.Distributions.RemoveAt(i2);
                }

                groupIndex++;
                matrix.Groups.Add(group);
            }

            return matrix;
        }

     
        public static SparseMatrixMDP GaussianJordenElimination(SparseMatrixMDP smatrixMdp, bool first)
        {
            var r = smatrixMdp.Ngroup;
           // Debug.WriteLine(smatrixMdp);

            //reduce 
            if(first)
            {
                foreach (var group in smatrixMdp.Groups)
                {
                    group.RowsInSameGroup = LinearEquationsSolver.EliminateRedundantRowsSubExtreme(group.RowsInSameGroup);
                }
            }
//            Debug.WriteLine(smatrixMdp);

            var groups = smatrixMdp.Groups;
            for (int i = 0; i < r; i++) //operating on each group, i can take as pivot position
            {
                var currentGroup = groups[i];
                //rows in ith group should normalized according to the element in its pivot position (ith group, ith column) 
                //Debug.Assert(groups != null, "groups != null");
                
             //  var initialZeroRows = new HashSet<Row>();
                
                foreach (Row currentRow1 in currentGroup.RowsInSameGroup)
                {
                    //     Debug.Assert(currentRow1.col[0] == i, "zero value");
                    if (currentRow1.col[0] == i) //TODO:I think this info is redundant
                    {
                        double currentFirstValue = currentRow1.val[0];
                        if (Math.Abs(currentFirstValue - 1) > EPSILON)
                        {
                            for (int k = 1; k < currentRow1.val.Count; k++)
                            {
                                currentRow1.val[k] = currentRow1.val[k]/currentFirstValue;
                            }
                        }

                        currentRow1.col.RemoveAt(0);
                        currentRow1.val.RemoveAt(0);
                        currentRow1.diag = 1;
                    } //else
                    // {
                    //    initialZeroRows.Add(currentRow1);
                    // }
                }

                currentGroup.RowsInSameGroup =
                    LinearEquationsSolver.EliminateRedundantRowsSubExtreme(currentGroup.RowsInSameGroup);

                
                    //using current row to deduct every rows in other group if corresponding element (column i in dense matrix) is not zero
                    //follow upper triangular form.
                    for (int j = 0; j < r; j++) //scan all groups
                    {  
                        if (j == i)
                        {
                            continue;
                        }
                        //update rows in group j
                        var pivotGroup = groups[j];
                        var removeCollection = new HashSet<Row>();
                        var addCollection = new HashSet<Row>();

                        foreach (Row pivotRow in pivotGroup.RowsInSameGroup)
                        {

                            if (pivotRow.col[0] == i)
                            //index of col for column value in dense matrix equal to pivot element. if value is zero, it will not be found
                            {
                                double t = -pivotRow.val[0];
                                pivotRow.col.RemoveAt(0);
                                pivotRow.val.RemoveAt(0);

                                if(currentGroup.RowsInSameGroup.Count>1)
                                {
                                    foreach (Row currentRow in currentGroup.RowsInSameGroup)
                                    {
                                        // bool flag;

                                        if (!CheckDictionarySubset(pivotRow.SelectionMemoryInGroups,
                                                                   currentRow.SelectionMemoryInGroups,pivotRow.WorkingGroup,currentRow.WorkingGroup))
                                        {
                                            continue;
                                        }

                                        var r3 = new Row
                                        {
                                            diag = pivotRow.diag,
                                            diagCol = pivotRow.diagCol,
                                            Index = pivotRow.Index,
                                        };
                                        //add elements in colValuePair 
                                       // var hashset1 = new HashSet<int>(currentRow.col);
                                        //hashset1.UnionWith(pivotRow.col);
                                        var colValuePair =
                                            new SortedDictionary<int, double>();

                                        for (int inneri = 0;
                                             inneri < currentRow.col.Count;
                                             inneri++)
                                        {
                                            //if (Math.Abs(currentRow.val[inneri]) > EPSILON)
                                            //{
                                                colValuePair[currentRow.col[inneri]] = currentRow.val[inneri] * t;
                                           // }

                                            if(currentRow.col[inneri]==r3.diagCol)
                                            {
                                                r3.diag = r3.diag + currentRow.val[inneri] * t;
                                            }
                                        }

                                        //add workingRow to r3
                                        //var hashset2 = new HashSet<int>();
                                        for (int inneri = 0; inneri < pivotRow.col.Count; inneri++)
                                        {
                                            if (colValuePair.ContainsKey(pivotRow.col[inneri]))
                                            {
                                                double m = colValuePair[pivotRow.col[inneri]] + pivotRow.val[inneri];
                                                if (Math.Abs(m) > EPSILON)
                                                {
                                                    colValuePair[pivotRow.col[inneri]] = m;
                                                }
                                            }else
                                            {
                                                colValuePair[pivotRow.col[inneri]] = pivotRow.val[inneri];
                                            }


                                        }

                                        r3.col = new List<int>(colValuePair.Keys);
                                        r3.val = new List<double>(colValuePair.Values);
                                        r3.SelectionMemoryInGroups = new Dictionary<int, int>(pivotRow.SelectionMemoryInGroups);
                                        r3.WorkingGroup=new BitArray(pivotRow.WorkingGroup);
                                        foreach (KeyValuePair<int, int> d in currentRow.SelectionMemoryInGroups)
                                        {
                                            r3.SelectionMemoryInGroups[d.Key] = d.Value;
                                            r3.WorkingGroup.Set(d.Key,true);
                                        }

                                        addCollection.Add(r3);
                                        if (pivotRow.SelectionMemoryInGroups.ContainsKey(currentGroup.Index))
                                        {
                                            break;
                                        }
                                    }
                                }else
                                {
                                    foreach (Row currentRow in currentGroup.RowsInSameGroup)
                                    {
                                        
                                        var r3 = new Row
                                        {
                                            diag = pivotRow.diag,
                                            diagCol = pivotRow.diagCol,
                                            Index = pivotRow.Index,
                                        };
                                        //add elements in colValuePair 
                                        var hashset1 = new HashSet<int>(currentRow.col);
                                        hashset1.UnionWith(new HashSet<int>(pivotRow.col));
                                        var colValuePair =
                                            new SortedDictionary<int, double>(hashset1.ToDictionary(item => item, item => 0.0));

                                        for (int inneri = 0;
                                             inneri < currentRow.col.Count;
                                             inneri++)
                                        {
                                            //if (Math.Abs(currentRow.val[inneri]) > EPSILON)
                                            //{
                                            colValuePair[currentRow.col[inneri]] = currentRow.val[inneri] * t;
                                            // }

                                            if (currentRow.col[inneri] == r3.diagCol)
                                            {
                                                r3.diag = r3.diag + currentRow.val[inneri] * t;
                                            }

                                        }

                                        for (int inneri = 0; inneri < pivotRow.col.Count; inneri++)
                                        {
                                            double m = colValuePair[pivotRow.col[inneri]] + pivotRow.val[inneri];
                                            if (Math.Abs(m) > EPSILON)
                                            {
                                                colValuePair[pivotRow.col[inneri]] = m;
                                            }

                                        }

                                        r3.col = new List<int>(colValuePair.Keys);
                                        r3.val = new List<double>(colValuePair.Values);
                                        r3.SelectionMemoryInGroups = new Dictionary<int, int>(pivotRow.SelectionMemoryInGroups);
                                        r3.WorkingGroup = new BitArray(pivotRow.WorkingGroup);
                                        foreach (KeyValuePair<int, int> d in currentRow.SelectionMemoryInGroups)
                                        {
                                            r3.SelectionMemoryInGroups[d.Key] = d.Value;
                                            r3.WorkingGroup.Set(d.Key, true);
                                        }
                                        addCollection.Add(r3);
                                      
                                    }
                                }

                                removeCollection.Add(pivotRow);

                            }
                        }

                        //update rows in current group
                        foreach (Row row in removeCollection)
                        {
                            pivotGroup.RowsInSameGroup.Remove(row);
                        }

                        //clear redundant rows in addCollection 
                        //pivotGroup.RowsInSameGroup.UnionWith(EliminateRedundantRows(addCollection));
                        //var reducedRows = EliminateRedundantRowsSubExtreme(addCollection);
                        
                        pivotGroup.RowsInSameGroup.UnionWith(addCollection);
                        if (pivotGroup.RowsInSameGroup.Count > smatrixMdp.Ncols) //r is not correct bound. 
                        {
                            //if (pivotGroup.Index > currentGroup.Index)
                            //{
                            //    pivotGroup.RowsInSameGroup =
                            //EliminateRedundantRowsSubExtremeWithNormalization(pivotGroup.RowsInSameGroup);
                            //}else
                            //{
                            //    pivotGroup.RowsInSameGroup =
                            //EliminateRedundantRowsSubExtreme(pivotGroup.RowsInSameGroup);
                            //}

                            pivotGroup.RowsInSameGroup = pivotGroup.Index > currentGroup.Index ? EliminateRedundantRowsSubExtremeWithNormalization(pivotGroup.RowsInSameGroup) : EliminateRedundantRowsSubExtreme(pivotGroup.RowsInSameGroup);

                        }
                        
                    }
                //Debug.WriteLine(smatrixMdp);
            }

            foreach (var group in smatrixMdp.Groups)
            {
                group.RowsInSameGroup = LinearEquationsSolver.EliminateRedundantRowsSubExtreme(group.RowsInSameGroup);
            }

            return smatrixMdp;
        }



        public static SparseMatrixMDP GaussianJordenEliminationForTreeStructure(SparseMatrixMDP smatrixMdp, bool first)
        {
            var r = smatrixMdp.Ngroup;
            // Debug.WriteLine(smatrixMdp);

            //reduce 
            //if (first)
            //{
            //    foreach (var group in smatrixMdp.Groups)
            //    {
            //        group.RowsInSameGroup = LinearEquationsSolver.EliminateRedundantRowsSubExtreme(group.RowsInSameGroup);
            //    }
            //}
            var groups = smatrixMdp.Groups;
            for (int i = 0; i < r; i++) //operating on each group, i can take as pivot position
            {
                var currentGroup = groups[i];
                //rows in ith group should normalized according to the element in its pivot position (ith group, ith column) 
                foreach (Row currentRow1 in currentGroup.RowsInSameGroup)
                {
                    //     Debug.Assert(currentRow1.col[0] == i, "zero value");
                    if (currentRow1.col[0] == i) //TODO:I think this info is redundant
                    {
                        double currentFirstValue = currentRow1.val[0];
                        if (Math.Abs(currentFirstValue - 1) > EPSILON)
                        {
                            for (int k = 1; k < currentRow1.val.Count; k++)
                            {
                                currentRow1.val[k] = currentRow1.val[k] / currentFirstValue;
                            }
                        }

                        currentRow1.col.RemoveAt(0);
                        currentRow1.val.RemoveAt(0);
                        currentRow1.diag = 1;
                    } 
                }

                currentGroup.RowsInSameGroup =
                    LinearEquationsSolver.EliminateRedundantRowsSubExtreme(currentGroup.RowsInSameGroup);


                //using current row to deduct every rows in other group if corresponding element (column i in dense matrix) is not zero
                //follow upper triangular form.
                for (int j = 0; j < r; j++) //scan all groups
                {
                    if (j == i)
                    {
                        continue;
                    }
                    //update rows in group j
                    var pivotGroup = groups[j];
                    var removeCollection = new HashSet<Row>();
                    var addCollection = new HashSet<Row>();

                    foreach (Row pivotRow in pivotGroup.RowsInSameGroup)
                    {

                        if (pivotRow.col[0] == i)
                        //index of col for column value in dense matrix equal to pivot element. if value is zero, it will not be found
                        {
                            double t = -pivotRow.val[0];
                            pivotRow.col.RemoveAt(0);
                            pivotRow.val.RemoveAt(0);

                            //if (currentGroup.RowsInSameGroup.Count > 1)
                            //{
                                foreach (Row currentRow in currentGroup.RowsInSameGroup)
                                {
                                    // bool flag;

                                    //if (!CheckDictionarySubset(pivotRow.SelectionMemoryInGroups,
                                    //                           currentRow.SelectionMemoryInGroups, pivotRow.WorkingGroup, currentRow.WorkingGroup))
                                    //{
                                    //    continue;
                                    //}

                                    var r3 = new Row
                                    {
                                        diag = pivotRow.diag,
                                        diagCol = pivotRow.diagCol,
                                        Index = pivotRow.Index,
                                    };
                                    //add elements in colValuePair 
                                    // var hashset1 = new HashSet<int>(currentRow.col);
                                    //hashset1.UnionWith(pivotRow.col);
                                    var colValuePair =
                                        new SortedDictionary<int, double>();

                                    for (int inneri = 0;
                                         inneri < currentRow.col.Count;
                                         inneri++)
                                    {
                                        //if (Math.Abs(currentRow.val[inneri]) > EPSILON)
                                        //{
                                        colValuePair[currentRow.col[inneri]] = currentRow.val[inneri] * t;
                                        // }

                                        if (currentRow.col[inneri] == r3.diagCol)
                                        {
                                            r3.diag = r3.diag + currentRow.val[inneri] * t;
                                        }
                                    }

                                    //add workingRow to r3
                                    //var hashset2 = new HashSet<int>();
                                    for (int inneri = 0; inneri < pivotRow.col.Count; inneri++)
                                    {
                                        if (colValuePair.ContainsKey(pivotRow.col[inneri]))
                                        {
                                            double m = colValuePair[pivotRow.col[inneri]] + pivotRow.val[inneri];
                                            if (Math.Abs(m) > EPSILON)
                                            {
                                                colValuePair[pivotRow.col[inneri]] = m;
                                            }
                                        }
                                        else
                                        {
                                            colValuePair[pivotRow.col[inneri]] = pivotRow.val[inneri];
                                        }


                                    }

                                    r3.col = new List<int>(colValuePair.Keys);
                                    r3.val = new List<double>(colValuePair.Values);
                                    //r3.SelectionMemoryInGroups = new Dictionary<int, int>(pivotRow.SelectionMemoryInGroups);
                                   // r3.WorkingGroup = new BitArray(pivotRow.WorkingGroup);
                                  //  foreach (KeyValuePair<int, int> d in currentRow.SelectionMemoryInGroups)
                                  //  {
                                     //   r3.SelectionMemoryInGroups[d.Key] = d.Value;
                                  //      r3.WorkingGroup.Set(d.Key, true);
                                  //  }

                                    addCollection.Add(r3);
                                    //if (pivotRow.SelectionMemoryInGroups.ContainsKey(currentGroup.Index))
                                    //{
                                    //    break;
                                    //}
                                }
                            //}
                            removeCollection.Add(pivotRow);

                        }
                    }

                    //update rows in current group
                    foreach (Row row in removeCollection)
                    {
                        pivotGroup.RowsInSameGroup.Remove(row);
                    }

                    //clear redundant rows in addCollection 
                    //pivotGroup.RowsInSameGroup.UnionWith(EliminateRedundantRows(addCollection));
                    //var reducedRows = EliminateRedundantRowsSubExtreme(addCollection);

                    pivotGroup.RowsInSameGroup.UnionWith(addCollection);
                    //if (pivotGroup.RowsInSameGroup.Count > smatrixMdp.Ncols) //r is not correct bound. 
                    //{  
                    //    pivotGroup.RowsInSameGroup = pivotGroup.Index > currentGroup.Index ? EliminateRedundantRowsSubExtremeWithNormalization(pivotGroup.RowsInSameGroup) : EliminateRedundantRowsSubExtreme(pivotGroup.RowsInSameGroup);

                    //}

                }
                //Debug.WriteLine(smatrixMdp);
            }

            foreach (var group in smatrixMdp.Groups)
            {
                if(group.RowsInSameGroup.Count>smatrixMdp.Ncols)
                {

                    group.RowsInSameGroup = LinearEquationsSolver.EliminateRedundantRowsSubExtreme(group.RowsInSameGroup);
                }
            }

            return smatrixMdp;
        }




        private static HashSet<Row> EliminateRedundantRowsSubExtremeWithNormalization(HashSet<Row> rowsInSameGroup)
        {
            foreach (Row row in rowsInSameGroup)
            {
                double factor = row.diag;
                //if (Math.Abs(factor - 1) > EPSILON)
                //{
                    if (Math.Abs(factor) < EPSILON)
                    {
                        Debug.WriteLine("Let me know" + factor);
                    }

                    for (int i = 0; i < row.val.Count; i++)
                    {
                        row.val[i] = row.val[i]/factor;
                    }
                //}

            }
            return EliminateRedundantRowsSubExtreme(rowsInSameGroup);
        }

        //public static double[,] UseMatlabSolveLinearEquation(int r, int c, double[] a)
        //{
        //    var obj = new InverseMul.InverseMul();
        //    MWNumericArray ma = new MWNumericArray(r, c, a);
        //    MWArray[] agIn = new MWArray[] {ma};
        //    MWArray[] agOut = null;

        //    obj.inversionNonSquare(1, ref agOut, agIn);

        //    MWNumericArray item = (MWNumericArray)agOut[0];
        //    return (double[,])item.ToArray(MWArrayComponent.Real);
        //}

       
        /// <summary>
        /// Check if set1 contains set2
        /// </summary>
        /// <param name="set1"></param>
        /// <param name="set2"></param>
        ///// <param name="flag">use to check a situation that if the return result if true AND set2 contains set1 AND values are equal. 
        ///// for the case that, set2 doesn't contain set1.keys. Result can still return true. But flag = false. Usage is that, if flag =true, only 
        ///// one row has to be checked. Otherwise, all the row needs to be checked</param>
        /// <returns></returns>
        public static bool CheckDictionarySubset(Dictionary<int, int> set1, Dictionary<int, int> set2, BitArray b1,BitArray b2) //, out bool flag)
        {
            // bool any = false;
            //flag = false;;
            
            for (int i = 0; i < b1.Count; i++)
            {
                if (b1[i] & b2[i])
                {
                    if (set1[i] != set2[i])
                    {
                        return false;
                    }
                }

            }
            return true;
        }


        /// <summary>
        /// eliminate redundant rows in sparse matrix structure, based on extreme points selection. 
        /// </summary>
        /// <param name="rowsIntheSameGroup"></param>
        /// <returns></returns>
        public static HashSet<Row> EliminateRedundantRowsSubExtreme(HashSet<Row> rowsIntheSameGroup)
        {
            if(rowsIntheSameGroup.Count<=1)
            {
                return rowsIntheSameGroup;
            }

            var clonedRows = Row.CloneSetRows(rowsIntheSameGroup);
            var i = 0;
            var rows1 = clonedRows.ToDictionary(item => (i++), item => item);
            var j = 0;
            var row2 = rowsIntheSameGroup.ToDictionary(item => (j++), item => item);

            var resultingRows = new HashSet<int>();
            var workingstack = new Stack<Dictionary<int, Row>>();

            workingstack.Push(rows1);
            //rows with same extreme col value. The reductions is done based on the rest columns.
            while (workingstack.Count != 0)
            {
                var rows = workingstack.Pop();
                if (rows.Count > 1)
                {
                    var colMaxValue = new Dictionary<int, double>();
                    var colMaxRow = new Dictionary<int, Dictionary<int, Row>>();
                    var colIndexHashSet = new HashSet<int>();

                    bool flag2 = true;
                    foreach (KeyValuePair<int, Row> keyValuePair in rows)
                    {
                        if (flag2)
                        {
                            foreach (int i1 in keyValuePair.Value.col)
                            {
                                if (colIndexHashSet.Contains(i1))
                                {
                                    flag2 = false;
                                }
                            }
                        }

                        colIndexHashSet.UnionWith(keyValuePair.Value.col);

                    }
                    if (flag2)
                    {
                        resultingRows.UnionWith(rows.Keys);
                        continue;
                    }

                    var rowsIndex = new List<int>(rows.Keys);
                    foreach (int colIndex in colIndexHashSet)
                    {
                        var flag = false;
                        
                        for(int i2=rowsIndex.Count-1;i2>=0;i2--)  //rows
                        {
                            var currentRowIndex = rowsIndex[i2];
                            var currentRow1 = rows[currentRowIndex];

                            if(currentRow1.col.Contains(colIndex))
                            {
                                var valIndex = currentRow1.col.BinarySearch(colIndex);
                                var colValue = currentRow1.val[valIndex];
                                if (colValue < 0)
                                { 
                                    if (colMaxValue.ContainsKey(colIndex))
                                    {
                                        if (colMaxValue[colIndex] > colValue)
                                        {
                                            colMaxValue[colIndex] = colValue;
                                            colMaxRow[colIndex] = new Dictionary<int, Row> { { currentRowIndex, currentRow1 } };
                                            //can also be equal
                                        }
                                        else if (Math.Abs(colMaxValue[colIndex] - colValue) < EPSILON)
                                        {
                                            colMaxRow[colIndex].Add(currentRowIndex, currentRow1);
                                        }

                                    }
                                    else
                                    {
                                        colMaxValue[colIndex] = colValue;
                                        colMaxRow[colIndex] = new Dictionary<int, Row> { { currentRowIndex, currentRow1 } };

                                    }
                                    flag = true;

                                }else
                                {
                                    currentRow1.col.RemoveAt(valIndex); //if positive, like, 1, remove. 
                                    currentRow1.val.RemoveAt(valIndex);
                                }

                            }
                        }
                        //remove the rows that already added to colMaxRow[colIndex]

                        if (flag)
                        {
                            //if (colMaxRow.ContainsKey(colIndex))
                           // {
                                foreach (int key in colMaxRow[colIndex].Keys)
                                {
                                    rows.Remove(key);
                                    rowsIndex.Remove(key);
                                }
                            //
                        }

                    }

                   

                   // var rowsIndex = new List<int>(colMaxRow.Keys);
                    foreach (KeyValuePair<int, Dictionary<int, Row>> keyValuePair in colMaxRow)
                    {
                        Dictionary<int, Row> rowsDictionary = keyValuePair.Value;
                        //List<int> rowsIndex2 = new List<int>(keyValuePair.Value.Keys.Count);
                        //List<int> rowsIndex3 = new List<int>(keyValuePair.Value.Keys.Count);
                        //foreach (KeyValuePair<int, Row> intRowPair in keyValuePair.Value)
                        //{
                        //    if (intRowPair.Value.col.Count == 1)
                        //    {
                        //        rowsIndex2.Add(intRowPair.Key);
                        //    }

                        //}
                        
                        //////if there is only one element in a row, and rows with the same values then, remove 
                        //////List<int> rowsIndex=new List<int>(keyValuePair.Value.Keys);
                        //for (int inneri = rowsIndex2.Count - 1; inneri >= 1; inneri--)
                        //{

                        //    var inneriIndex = rowsIndex2[inneri];
                        //    var row = rowsDictionary[inneriIndex];
                        //    for (int innerj = inneri - 1; innerj >= 0; innerj--)
                        //    {
                        //        var innerjIndex = rowsIndex2[innerj];
                        //        var rowj = rowsDictionary[innerjIndex];
                        //        //if all equal.then, this is a identical /redundant row, remove
                        //        if (row.col[0]==rowj.col[0]&&Math.Abs(row.val[0] - rowj.val[0]) < EPSILON)
                        //        {
                        //            rowsDictionary.Remove(inneriIndex);
                        //            break;
                        //        }
                        //    }
                        //}


                       //rowsIndex = new List<int>(keyValuePair.Value.Keys);
                       //// remove similar rows
                       //for (int inneri = rowsDictionary.Count - 1; inneri >= 1; inneri--)
                       //{
                          
                       //    var inneriIndex = rowsIndex[inneri];
                       //    var row = rowsDictionary[inneriIndex];
                       //    if(row.col.Count>2)
                       //    {
                       //        continue;
                       //    }
                       //    //if (row.col[0] == i)
                       //    //{
                       //    for (int innerj = inneri - 1; innerj >= 0; innerj--)
                       //    {
                       //        var innerjIndex = rowsIndex[innerj];
                       //        var rowj = rowsDictionary[innerjIndex];
                       //        if (row.col.Count != rowj.col.Count)
                       //        {
                       //            continue;
                       //        }
                       //        bool flag = true;
                       //        for (int k = 0; k < row.col.Count; k++)
                       //        {

                       //            if (row.col[k] == rowj.col[k])
                       //            //GL:TODO:can improve, try int compare
                       //            {
                       //                if (Math.Abs(row.val[k] - rowj.val[k]) > EPSILON)
                       //                {
                       //                    flag = false;
                       //                    break;
                       //                }

                       //            }
                       //            else
                       //            {
                       //                flag = false;
                       //                break;
                       //            }
                       //        }
                       //        //if all equal.then, this is a identical /redundant row, remove
                       //        if (flag)
                       //        {
                       //            rowsDictionary.Remove(inneriIndex);
                       //            break;
                       //        }
                       //    }

                       //    //find sub extreme points and remove rest
                       //}

                        //recursive if number of rows larger than 1
                       if (rowsDictionary.Count > 1)
                       {

                           var nonZeroElementDictionary = new Dictionary<int, Row>();
                           foreach (KeyValuePair<int, Row> rowPair in rowsDictionary)
                           {
                               var index=rowPair.Value.col.BinarySearch(keyValuePair.Key);
                               rowPair.Value.col.RemoveAt(index);
                               rowPair.Value.val.RemoveAt(index);
                               if (rowPair.Value.col.Count > 0)
                               {
                                   nonZeroElementDictionary.Add(rowPair.Key, rowPair.Value);
                               }
                               else 
                               {
                                   resultingRows.Add(rowPair.Key);
                                   break;  //only one-elements rows, keep one to delete same rows
                               }
                           }
                           if(nonZeroElementDictionary.Count>0)
                           {
                               workingstack.Push(nonZeroElementDictionary);
                           }
                       }
                       else
                       {
                           Debug.Assert(rowsDictionary.Keys.Count > 0, "true1");
                           resultingRows.UnionWith(rowsDictionary.Keys);
                       }

                    }
                }
                else
                {
                    Debug.Assert(rows.Keys.Count > 0, "true2");
                    resultingRows.UnionWith(rows.Keys);
                }


            }

            var returnRows = new HashSet<Row>();

            foreach (int k in resultingRows)
            {
                returnRows.Add(row2[k]);

            }
            Debug.Assert(resultingRows.Count>0,"true");
            return returnRows;
        }

       

        //----------------------following functions are not used any more in the project-----------------------------------//

        /// <summary>
        /// This method is based Guassian-Jordan Elimiantion methods
        /// </summary>
        /// <param name="a">a r*c matrix, assume r less or equal to c</param>
        /// <returns>a matrix with first r*r matrix being identity matrix </returns>
        /// This function is no longer used in our project, as we are now using Gaussian Jordan based on sparse matrix
        public static double[,] GaussianJordanElimination(double[,] a)
        {
            // r: #of rows; c: #of coloumns
            int r = a.GetLength(0);
            int c = a.GetLength(1);

            for (int i = 0; i < r; i++)
            {
                for (int j = i; j < r; j++)  //assume c>=r
                {
                    if (Math.Abs(a[j, j] - 0) > EPSILON)
                    //if(a[i,j] != 0.0)
                    {

                        //Step1: devide pivot element
                        if (Math.Abs(a[j, j] - 1) > EPSILON)
                        //if (a[i, j] != 1.0)
                        {
                            //make diagnoal position element to 1
                            double t = 1.0 / a[j, j];
                            //  for (int k = j+1; k < c; k++)
                            for (int k = j; k < c; k++)
                            {
                                a[j, k] = t * a[j, k];
                            }
                        }

                        //Step2: eliminate that element for rest of columns

                        for (int l = 0; l < r; l++)
                        {
                            if (l != j)
                            {
                                double t = -a[l, j];
                                if (Math.Abs(t - 0) > EPSILON)
                                //if (t != 0.0 && Math.Abs(t - 0) <= EPSILON)
                                {
                                    for (int k = j; k < c; k++)
                                    {
                                        a[l, k] = a[l, k] + t * a[j, k];
                                    }
                                }
                            }

                        }
                    }
                }
            }
            return a;
        }

        //public static HashSet<Row> EliminateRedundantRows(HashSet<Row> rowsIntheSameGroup)
        //{

        //    var colMaxValue = new Dictionary<int, double>();
        //    var colMaxRow = new Dictionary<int, List<Row>>();
        //    foreach (Row currentRow1 in rowsIntheSameGroup)
        //    {

        //        //following code for the usage of row reduction
        //        for (int k = 0; k < currentRow1.val.Count; k++)
        //        {
        //            int colIndex = currentRow1.col[k];
        //            double colValue = currentRow1.val[k];
        //            if (colMaxValue.ContainsKey(colIndex))
        //            {
        //                if (colMaxValue[colIndex] > colValue)
        //                {
        //                    colMaxValue[colIndex] = colValue;
        //                    colMaxRow[colIndex] = new List<Row>() { currentRow1 }; //can also be equal
        //                }
        //                else if (Math.Abs(colMaxValue[colIndex] - colValue) < EPSILON)
        //                {
        //                    colMaxRow[colIndex].Add(currentRow1);
        //                }

        //            }
        //            else
        //            {
        //                colMaxValue[colIndex] = colValue;
        //                colMaxRow[colIndex] = new List<Row>() { currentRow1 };
        //            }
        //        }
        //    }

        //    var unionAllMaxRow = new HashSet<Row>();
        //    foreach (KeyValuePair<int, List<Row>> keyValuePair in colMaxRow)
        //    {
        //        List<Row> rowslist = keyValuePair.Value;
        //        for (int inneri = rowslist.Count - 1; inneri >= 1; inneri--)
        //        {
        //            var row = rowslist[inneri];
        //            //if (row.col[0] == i)
        //            //{
        //            for (int innerj = inneri - 1; innerj >= 0; innerj--)
        //            {
        //                var rowj = rowslist[innerj];
        //                if (row.col.Count != rowj.col.Count)
        //                {
        //                    continue;
        //                }
        //                bool flag = true;
        //                for (int k = 0; k < row.col.Count; k++)
        //                {
        //                    if (row.col[k] == rowj.col[k] & Math.Abs(row.val[k] - rowj.val[k]) > EPSILON)
        //                    //GL:TODO:can improve, try int compare
        //                    {
        //                        flag = false;
        //                        break;
        //                    }
        //                }
        //                //if all equal.then, this is a identical /redundant row, remove
        //                if (flag)
        //                {
        //                    rowslist.Remove(row);
        //                    break;
        //                }
        //            }
        //        }
        //        unionAllMaxRow.UnionWith(rowslist);
        //    }

        //    return unionAllMaxRow;

        //}


        
#if DEBUG
        public static void ToDot(List<MDPState> scc, int i, int j)
        {
            
            //report index of SCC states
            //Dictionary<MDPState,int> stateIndex = new Dictionary<MDPState, int>(scc.Count);
            //int stateID = 0;
            //foreach (var mdpState in scc)
            //{
            //    stateIndex.Add(mdpState,stateID);
            //    stateID++;
            //}

            StringBuilder info = new StringBuilder();

            string fileName = j+"_"+i+"cutGraph.dot";

            info.Append("digraph " + j + "00" + i + "{ \n");
            info.Append("rankdir=LR;\n");
            
            foreach (MDPState state in scc)
            {
                //info.Append(state.toDot().ToString());

                //foreach ( trans in state.)
                //{
                //    info.Append(trans.toDot().ToString());
                //}
                string name = state.ID.Replace("$", "00").Replace("\u03C4", "t");
                foreach (Distribution dis in state.Distributions)
                {
                    foreach (KeyValuePair<double, MDPState> keyValuePair in dis.States)
                    {
                        info.Append(name + " ->" + keyValuePair.Value.ID.Replace("$", "00").Replace("\u03C4", "t") + "[label=" + dis.Event + keyValuePair.Key * 1000 + "];\n");
                    }
                }
                //a->{b;c;d}[label="coo0.33l"];
                
            }

            info.Append("\n}\n");

            try
            {
                StreamWriter fwriter = File.CreateText("d:\\test\\" + fileName);
                fwriter.Write(info.ToString());
                fwriter.Close();
            }
            catch (Exception)
            {
                System.Windows.Forms.MessageBox.Show("Writing File Error!");
            }

        }
#endif

#if DEBUG
        public static void ToDotWithIndex(HashSet<MDPState> scc, int i, int j, HashSet<MDPState> outputs )
        {

            //report index of SCC states
            Dictionary<MDPState, int> stateIndex = new Dictionary<MDPState, int>(scc.Count);
            int stateID = 0;
            foreach (var mdpState in scc)
            {
                stateIndex.Add(mdpState, stateID);
                stateID++;
            }

            foreach (var mdpState in outputs)
            {
                stateIndex.Add(mdpState, stateID);
                stateID++;
            }

            StringBuilder info = new StringBuilder();

            string fileName = j + "_" + i + "cutGraph.dot";

            info.Append("digraph " + j + "00" + i + "{ \n");
            info.Append("rankdir=LR;\n");

            foreach (MDPState state in scc)
            {
                //info.Append(state.toDot().ToString());

                //foreach ( trans in state.)
                //{
                //    info.Append(trans.toDot().ToString());
                //}
                string name = stateIndex[state].ToString(CultureInfo.InvariantCulture);// +state.ID.Replace("$", "00").Replace("\u03C4", "t");
                foreach (Distribution dis in state.Distributions)
                {
                    foreach (KeyValuePair<double, MDPState> keyValuePair in dis.States)
                    {
                        //info.Append(name + " ->" + stateIndex[keyValuePair.Value] + "9" + keyValuePair.Value.ID.Replace("$", "00").Replace("\u03C4", "t") + "[label=" + dis.Event + keyValuePair.Key * 1000 + "];\n");
                        info.Append(name + " ->" + stateIndex[keyValuePair.Value] + "[label=" + dis.Event + keyValuePair.Key * 1000 + "];\n");
                    }
                }
                //a->{b;c;d}[label="coo0.33l"];

            }

            info.Append("\n}\n");

            try
            {
                StreamWriter fwriter = File.CreateText("d:\\test1\\" + fileName);
                fwriter.Write(info.ToString());
                fwriter.Close();
            }
            catch (Exception)
            {
                System.Windows.Forms.MessageBox.Show("Writing File Error!");
            }

        }
#endif
    }



}
