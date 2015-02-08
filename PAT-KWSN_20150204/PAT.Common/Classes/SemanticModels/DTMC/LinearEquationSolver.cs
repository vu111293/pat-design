using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
//using MathWorks.MATLAB.NET.Arrays;
using PAT.Common.Classes.SemanticModels.DTMC;
//using InverseMul;

namespace PAT.Common.Classes.SemanticModels.DTMC
{
    public class LinearEquationsSolver
    {
        // public const double EPSILON=0.00001;
        public const double EPSILON = 0.000001;
        /// <summary>
        /// This method is based Guassian-Jordan Elimiantion methods
        /// </summary>
        /// <param name="a">a r*c matrix, assume r less or equal to c</param>
        /// <returns>a matrix with first r*r matrix being identity matrix </returns>
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


        public static SparseMatrix GaussianJordenElimination(SparseMatrix smatrix)
        {
            //int r = smatrix.Nrows;
            //for (int i = 0; i < r; i++)
            //{
            //    //row value divide pivot element
            //    var currentRow = smatrix.Rows[i];
            //    if (Math.Abs(currentRow.diag) > EPSILON)
            //    {
            //        if (Math.Abs(currentRow.diag - 1) > EPSILON)
            //        {
            //            for (int k = 0; k < currentRow.val.Count; k++)
            //            {
            //                currentRow.val[k] = currentRow.val[k]/currentRow.diag;
            //            }
            //        }

            //        var colIndexCurrentRow = currentRow.col.BinarySearch(i);
            //        //using current row to deduct every other rows in smatrix if corresponding element in row i is not zero

            //        for (int j = 0; j < r; j++)
            //        {
            //            if(j==i)
            //            {
            //                continue;
            //            }
            //            var pivotRow = smatrix.Rows[j];
            //            int colIndexPivot = pivotRow.col.BinarySearch(i);
            //            if (colIndexPivot >= 0 && Math.Abs(pivotRow.val[colIndexPivot] - 0) > EPSILON)
            //                //index of col for column value in dense matrix equal to pivot element. if value is zero, it will not be found
            //            {
            //                double t = -pivotRow.val[colIndexPivot];
            //                //var hashset1 = new HashSet<int>(pivotRow.col);
            //                //hashset1.UnionWith(new HashSet<int>(currentRow.col));
            //                var r3 = new Row
            //                             {
            //                                 diag = pivotRow.diag,
            //                                 diagCol = pivotRow.diagCol,
            //                                // val = new List<double>(hashset1.Count),
            //                                // col = hashset1.ToList(),
            //                             };

                           

            //                //foreach (int i1 in hashset1)
            //                //{
            //                //    colValuePair.Add(i1,0);
            //                //} 
                         
            //                //add elements in colValuePair 
            //                var hashset1 = new HashSet<int>();
            //               for (int inneri = colIndexCurrentRow; inneri < currentRow.col.Count; inneri++)
            //               {
            //                   hashset1.Add(currentRow.col[inneri]);
            //               }

            //               //add workingRow to r3
            //               var hashset2 = new HashSet<int>();
            //               for (int inneri =colIndexPivot; inneri < pivotRow.col.Count; inneri++)
            //               {
            //                   hashset2.Add(pivotRow.col[inneri]);
            //               }

            //                hashset2.UnionWith(hashset1);
            //                var colValuePair = new SortedDictionary<int, double>();
            //                foreach (int i1 in hashset2)
            //                {
            //                    colValuePair.Add(i1,0);
            //                }

            //                //add currentRow*t to r3
            //                //inneri starts from colValue as it only needs to update elements after colValue
            //                for (int inneri = colIndexCurrentRow; inneri < currentRow.col.Count; inneri++)
            //                {
            //                    colValuePair[currentRow.col[inneri]] += currentRow.val[inneri]*t;
            //                }

            //                //add workingRow to r3
            //                for (int inneri = colIndexPivot; inneri < pivotRow.col.Count; inneri++)
            //                {
            //                    colValuePair[pivotRow.col[inneri]] += pivotRow.val[inneri];
            //                }

            //                //remove new introduced zeros //Comments: this is not productive. 
            //                //var hashsetZeros = new HashSet<int>();
            //                //foreach (KeyValuePair<int, double> keyValuePair in colValuePair)
            //                //{
            //                //    if (Math.Abs(keyValuePair.Value) < EPSILON)
            //                //    {
            //                //        hashsetZeros.Add(keyValuePair.Key);
            //                //    }
            //                //}
            //                //foreach (int hashsetZero in hashsetZeros)
            //                //{
            //                //    colValuePair.Remove(hashsetZero);
            //                //}

            //                r3.col = new List<int>(colValuePair.Keys);
            //                r3.val = new List<double>(colValuePair.Values);

            //                //update diag elements in that row. After row adding, the pivot element/diag info is redundant.  
            //               // r3.diag = r3.col.Contains(r3.diagCol) ? colValuePair[r3.diagCol] : 0;
            //                smatrix.Rows[j] = r3;
            //                Debug.WriteLine(smatrix);
            //            }
            //        }
            //    }
            //}
            //return smatrix;

            //the algo can be simplified because of the property for this matrix. The pivot position is positive and nozero.
            //we also assume the first element in the column gives the right index (pivot colum index) 
            //for each elimiantion operation 
            //there will no redundant row, therefore, after elimiantion, there will no be the case for all elements in a row being 
            //zeros

            int r = smatrix.Nrows;
            for (int i = 0; i < r; i++)
            {
                //row value divide pivot element
                var currentRow = smatrix.Rows[i];
                //if (Math.Abs(currentRow.diag) > EPSILON)
                if(currentRow.col[0]==i) //assert current.Row.col.count>0;
                {
                    if (Math.Abs(currentRow.val[0] - 1) > EPSILON)
                    {
                        for (int k = 1; k < currentRow.val.Count; k++)
                        {
                            currentRow.val[k] = currentRow.val[k] / currentRow.val[0];
                        }
                    }

                    //remove the pivot position element, dia, diacol is redundant now and remove them
                    currentRow.col.RemoveAt(0);
                    currentRow.val.RemoveAt(0);
                    
                   // var colIndexCurrentRow = currentRow.col.BinarySearch(i); //by right, this should always be the first element in the row
                   // var colIndexCurrentRow = currentRow.col[0];
                    //using current row to deduct every other rows in smatrix if corresponding element in row i is not zero

                    for (int j = 0; j < r; j++)
                    {
                        if (j == i)
                        {
                            continue;
                        }
                        var pivotRow = smatrix.Rows[j];
                        //int colIndexPivot = pivotRow.col.BinarySearch(i);
                       //int colIndexPivot = pivotRow.col[0];
                       // if (colIndexPivot >= 0 && Math.Abs(pivotRow.val[colIndexPivot] - 0) > EPSILON)
                        if(pivotRow.col[0]==i) //this actually guarantees the value nonzero, 
                        //index of col for column value in dense matrix equal to pivot element. if value is zero, it will not be found
                        {
                            double t = -pivotRow.val[0];
                            
                            pivotRow.col.RemoveAt(0);
                            pivotRow.val.RemoveAt(0);
                            //diag, diagcol are also redundant. 

                            //var hashset1 = new HashSet<int>(pivotRow.col);
                            //hashset1.UnionWith(new HashSet<int>(currentRow.col));
                            var r3 = new Row
                            {
                                diag = pivotRow.diag,
                                diagCol = pivotRow.diagCol,
                                // val = new List<double>(hashset1.Count),
                                // col = hashset1.ToList(),
                            };


                            //add elements in colValuePair 
                            var hashset1 = new HashSet<int>(currentRow.col);
                            hashset1.UnionWith(new HashSet<int>(pivotRow.col));
                            var colValuePair = new SortedDictionary<int, double>();
                            foreach (int i1 in hashset1)
                            {
                                colValuePair.Add(i1,0);
                            }

                            //add currentRow*t to r3
                            //inneri starts from colValue as it only needs to update elements after colValue
                            for (int inneri = 0; inneri < currentRow.col.Count; inneri++)
                            {
                                if (Math.Abs(currentRow.val[inneri]) > EPSILON) //make sure zero is not added
                                {
                                    colValuePair[currentRow.col[inneri]] += currentRow.val[inneri] * t;
                                }
                            }

                            //add workingRow to r3
                            for (int inneri = 0; inneri < pivotRow.col.Count; inneri++)
                            {
                                double m = colValuePair[pivotRow.col[inneri]] + pivotRow.val[inneri];
                                if (Math.Abs(m) > EPSILON) //make sure zero is not added
                                {
                                    colValuePair[pivotRow.col[inneri]] = m;
                                }
                            }

                            r3.col = new List<int>(colValuePair.Keys);
                            r3.val = new List<double>(colValuePair.Values);

                            //update diag elements in that row. After row adding, the pivot element/diag info is redundant.  
                            // r3.diag = r3.col.Contains(r3.diagCol) ? colValuePair[r3.diagCol] : 0;
                            smatrix.Rows[j] = r3;
                            Debug.WriteLine(smatrix);
                        }
                    }
                }
            }
            return smatrix;
        }

        public static SparseMatrixMDP GaussianJordenElimination(SparseMatrixMDP smatrixMdp)
        {
            var r = smatrixMdp.Ngroup;
            var groups = smatrixMdp.Groups;
            for (int i = 0; i < r; i++) //operating on each group, i can take as pivot position
            {
                var currentGroup = groups[i];
                //rows in ith group should normalized according to the element in its pivot position (ith group, ith column) 
                Debug.Assert(groups != null, "groups != null");
                foreach (Row currentRow1 in currentGroup.RowsInSameGroup)
                {
                    if (currentRow1.col[0] == i) //TODO:I think this info is redundant
                    {
                        if (Math.Abs(currentRow1.val[0] - 1) > EPSILON)
                        {
                            for (int k = 1; k < currentRow1.val.Count; k++)
                            {
                                currentRow1.val[k] = currentRow1.val[k]/currentRow1.val[0];
                            }
                        }
                        currentRow1.col.RemoveAt(0);
                        currentRow1.val.RemoveAt(0);
                    }
                }
                //currentRow1.colIndexCurrentRow = currentRow1.col.BinarySearch(i);

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
                            //var hashset1 = new HashSet<int>(pivotRow.col);
                            //hashset1.UnionWith(new HashSet<int>(currentRow.col));
                            pivotRow.col.RemoveAt(0);
                            pivotRow.val.RemoveAt(0);

                            foreach (Row currentRow in currentGroup.RowsInSameGroup)
                            {
                               // bool flag;

                                if (!CheckDictionarySubset(pivotRow.SelectionMemoryInGroups,
                                                           currentRow.SelectionMemoryInGroups))
                                {
                                    continue;
                                }

                                var r3 = new Row
                                             {
                                                 diag = pivotRow.diag,
                                                 diagCol = pivotRow.diagCol,
                                                 // val = new List<double>(hashset1.Count),
                                                 // col = hashset1.ToList(),
                                                 Index = pivotRow.Index,
                                             };
                                //add elements in colValuePair 
                                var hashset1 = new HashSet<int>(currentRow.col);
                                hashset1.UnionWith(new HashSet<int>(pivotRow.col));
                                var colValuePair =
                                    new Dictionary<int, double>(hashset1.ToDictionary(item => item, item => 0.0));

                                for (int inneri = 0;
                                     inneri < currentRow.col.Count;
                                     inneri++)
                                {
                                    if (Math.Abs(currentRow.val[inneri]) > EPSILON)
                                    {
                                        colValuePair[currentRow.col[inneri]] += currentRow.val[inneri]*t;
                                    }

                                }

                                //add workingRow to r3
                                //var hashset2 = new HashSet<int>();
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
                              //  r3.SelectionMemoryInGroups = new Dictionary<Group, Row>(pivotRow.SelectionMemoryInGroups);
                              //  r3.SelectionMemoryInGroups[currentGroup] = currentRow;

                                r3.SelectionMemoryInGroups = new Dictionary<int,int>(pivotRow.SelectionMemoryInGroups);
                                r3.SelectionMemoryInGroups[currentGroup.Index] = currentRow.Index;
                                //update diag elements in that row. After row adding, the pivot element/diag info is redundant.  
                                // r3.diag = r3.col.Contains(r3.diagCol) ? colValuePair[r3.diagCol] : 0;
                                //pivotGroup.RowsInSameGroup.Add(r3);
                                addCollection.Add(r3);
                                if(pivotRow.SelectionMemoryInGroups.ContainsKey(currentGroup.Index))
                                {
                                    break;
                                } 
                                //If pivotRow contains currentGroup index, and currentRow can work with pivotRow, 
                                //that means the rest of rows in the group are not that useful.
                            }
                            removeCollection.Add(pivotRow);

                        }
                    }

                    //update rows in current group
                    foreach (Row row in removeCollection)
                    {
                        pivotGroup.RowsInSameGroup.Remove(row);
                    }
                    foreach (Row row in addCollection)
                    {
                        pivotGroup.RowsInSameGroup.Add(row);
                    }
                }
                Debug.WriteLine(smatrixMdp);
            }
            return smatrixMdp;
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
        /// Read Input, Output and Inner MDPStates into a matrix
        /// </summary>
        /// <param name="input">Input MDP States to SCC</param>
        /// <param name="output">Output MDP States to SCC</param>
        /// <param name="inner">SCC MDP States exclude Input part</param>
        /// <param name="NumberMapMDPState">Usered for map back to MDPState from matrix </param>
        /// <returns>two dimentioanl arrary: matrix</returns>
        /// There is no overlap between Input, Output, Inner
        public static double[,] Read2Matrix(HashSet<DTMCState> input, HashSet<DTMCState> output, HashSet<DTMCState> inner, out Dictionary<int, DTMCState> NumberMapDTMCState)
        {
            int inputSize = input.Count;
            int outputSize = output.Count;
            int innerSize = inner.Count;

            int column = inputSize + outputSize + innerSize;
            int row = inputSize + innerSize;
            var matrix = new double[row, column];  //default value all zeros

            var dtmcState2Number = new Dictionary<DTMCState, int>(column);
            var number2dtmcState = new Dictionary<int, DTMCState>(column);

            //assign coordination first
            int i = 0;
            foreach (DTMCState dtmcState in input)
            {
                dtmcState2Number.Add(dtmcState, i);
                number2dtmcState.Add(i, dtmcState);
                i++;
            }
            foreach (DTMCState dtmcState in inner)
            {
                dtmcState2Number.Add(dtmcState, i);
                number2dtmcState.Add(i, dtmcState);
                i++;
            }
            foreach (DTMCState dtmcState in output)
            {
                dtmcState2Number.Add(dtmcState, i);
                number2dtmcState.Add(i, dtmcState);
                i++;
            }

            // to update rows of matrix
            foreach (DTMCState dtmcState in input)
            {
                int rowCoord = dtmcState2Number[dtmcState];
                matrix[rowCoord, rowCoord] = 1;
                //foreach (Distribution distriution in mdpState.Distributions)
                //{
                    foreach (KeyValuePair<double, DTMCState> pair in dtmcState.Transitions)
                    {
                        int colCoord = dtmcState2Number[pair.Value];
                        matrix[rowCoord, colCoord] += -pair.Key;
                    }
                //}
            }
            foreach (DTMCState dtmcState in inner)
            {
                int rowCoord = dtmcState2Number[dtmcState];
                matrix[rowCoord, rowCoord] = 1;
                //foreach (var distriution in mdpState.Distributions)
                //{
                    foreach (KeyValuePair<double, DTMCState> pair in dtmcState.Transitions)
                    {
                        int colCoord = dtmcState2Number[pair.Value];
                        matrix[rowCoord, colCoord] += -pair.Key;
                    }
                //}
            }

            NumberMapDTMCState = number2dtmcState;
            return matrix;
        }

        public static SparseMatrix Read2MatrixToSparseMatrix(List<DTMCState> scc, List<DTMCState> output)
        {
            int inputSize = scc.Count;
            int outputSize = output.Count;

            int column = inputSize + outputSize;

            var matrix = new SparseMatrix(inputSize,column);

            var dtmcState2Number = new Dictionary<DTMCState, int>(column);
            //var number2dtmcState = new Dictionary<int, DTMCState>(column);

            //assign coordination first
            int i = 0;
            foreach (DTMCState mdpState in scc)
            {
                dtmcState2Number.Add(mdpState, i);
                i++;
            }
            foreach (DTMCState mdpState in output)
            {
                dtmcState2Number.Add(mdpState, i);
                i++;
            }

            //assign rows to matrix
            foreach (DTMCState dtmcState in scc)
            {
                int rowCoord = dtmcState2Number[dtmcState];
                //update every valued rows elements
                var compressedRow = new SortedDictionary<int, double> {{rowCoord, 1}};

                foreach (KeyValuePair<double, DTMCState> pair in dtmcState.Transitions)
                {
                    int colCoord = dtmcState2Number[pair.Value];
                    if(colCoord==rowCoord)
                    {
                        compressedRow[rowCoord] += -pair.Key;
                    }else
                    {
                       compressedRow.Add(colCoord, -pair.Key);
                    }
                }
                var row = new Row(new List<double>(compressedRow.Values), new List<int>(compressedRow.Keys)) { diag = compressedRow[rowCoord], diagCol = rowCoord };
                matrix.Rows.Add(row);
                //update diagnoal elements

            }

            return matrix;
        }

        public static double[,] Read2Matrix(List<DTMCState> scc, List<DTMCState> output)//, out Dictionary<int, DTMCState> NumberMapDTMCState)
        {
            int inputSize = scc.Count;
            int outputSize = output.Count;

            int column = inputSize + outputSize;
            int row = inputSize;
            var matrix = new double[row, column];  //default value all zeros

            var dtmcState2Number = new Dictionary<DTMCState, int>(column);
            //var number2dtmcState = new Dictionary<int, DTMCState>(column);

            //assign coordination first
            int i = 0;
            foreach (DTMCState mdpState in scc)
            {
                dtmcState2Number.Add(mdpState, i);
                //number2dtmcState.Add(i, mdpState);
                i++;
            }
            foreach (DTMCState mdpState in output)
            {
                dtmcState2Number.Add(mdpState, i);
                //number2dtmcState.Add(i, mdpState);
                i++;
            }

            // to update rows of matrix
            foreach (DTMCState dtmcState in scc)
            {
                int rowCoord = dtmcState2Number[dtmcState];
                matrix[rowCoord, rowCoord] = 1;
                //foreach (Distribution distriution in mdpState.Distributions)
                //{
                    foreach (KeyValuePair<double, DTMCState> pair in dtmcState.Transitions)
                    {
                        int colCoord = dtmcState2Number[pair.Value];
                        matrix[rowCoord, colCoord] += -pair.Key;
                    }
                //}
            }
            //NumberMapDTMCState = number2dtmcState;
            return matrix;
        }

        public static double[] Read2MatrixUsedForMatlab(List<DTMCState> scc, List<DTMCState> output)//, out Dictionary<int, DTMCState> NumberMapDTMCState)
        {
            int inputSize = scc.Count;
            int outputSize = output.Count;

            int column = inputSize + outputSize;
            int row = inputSize;
            var matrix = new double[row*column];  //default value all zeros

            var dtmcState2Number = new Dictionary<DTMCState, int>(column);
            //var number2dtmcState = new Dictionary<int, DTMCState>(column);

            //assign coordination first
            int i = 0;
            foreach (DTMCState mdpState in scc)
            {
                dtmcState2Number.Add(mdpState, i);
                //number2dtmcState.Add(i, mdpState);
                i++;
            }
            foreach (DTMCState mdpState in output)
            {
                dtmcState2Number.Add(mdpState, i);
                //number2dtmcState.Add(i, mdpState);
                i++;
            }

            // to update rows of matrix
            foreach (DTMCState dtmcState in scc)
            {
                int rowCoord = dtmcState2Number[dtmcState];
                matrix[rowCoord*column+ rowCoord] = 1;
                //foreach (Distribution distriution in mdpState.Distributions)
                //{
                foreach (KeyValuePair<double, DTMCState> pair in dtmcState.Transitions)
                {
                    int colCoord = dtmcState2Number[pair.Value];
                    matrix[rowCoord*column+ colCoord] += -pair.Key;
                }
                //}
            }
            //NumberMapDTMCState = number2dtmcState;
            return matrix;
        }

        //TODO:
        /// <summary>
        /// This function is to added into MDP class. The function is performed on every identified SCC 
        /// </summary>
        /// <param name="mdp"></param>
        public static void AbstractionBySccReduction()
        {
            //TODO: from a MDP or SCC, obtain Input, Output, Inner

            //Step1: Load to matrix

            //Step2: Perform Guanssian-Jordan Elimination

            //Step3: update MDP by 1)deleting Inner nodes and distributions 2)adding new distribution between Input to Output


        }

        /// <summary>
        /// Check if set1 contains set2
        /// </summary>
        /// <param name="set1"></param>
        /// <param name="set2"></param>
        /// <returns></returns>
        //public static bool CheckDictionarySubset(Dictionary<Group,Row> set1, Dictionary<Group,Row>set2  )
        //{
        //   // bool any = false;
        //    foreach (KeyValuePair<Group, Row> keyValuePair in set2)
        //    {
        //        Group j = keyValuePair.Key;
        //        if (set1.ContainsKey(j))
        //        {
        //            return set1[j].Index == keyValuePair.Value.Index;
        //        }
        //    }
        //    return true;
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
        public static bool CheckDictionarySubset(Dictionary<int, int> set1, Dictionary<int, int> set2) //, out bool flag)
        {
            // bool any = false;
            //flag = false;
            foreach (KeyValuePair<int, int> keyValuePair in set2)
            {
                int j = keyValuePair.Key;
                if (set1.ContainsKey(j))
                {
                    if (set1[j] != keyValuePair.Value)
                    {
                        return false;
                    }
                }
            }
            //flag = true; //means, set1 doesn't contain any keys in set2
            return true;
        }

//#if DEBUG
//        public static void ToDot(List<DTMCState> scc, int i, int j)
//        {
            
//            //report index of SCC states
//            //Dictionary<MDPState,int> stateIndex = new Dictionary<MDPState, int>(scc.Count);
//            //int stateID = 0;
//            //foreach (var mdpState in scc)
//            //{
//            //    stateIndex.Add(mdpState,stateID);
//            //    stateID++;
//            //}

//            StringBuilder info = new StringBuilder();

//            string fileName = j+"_"+i+"cutGraph.dot";

//            info.Append("digraph " + j + "00" + i + "{ \n");
//            info.Append("rankdir=LR;\n");
            
//            foreach (MDPState state in scc)
//            {
//                //info.Append(state.toDot().ToString());

//                //foreach ( trans in state.)
//                //{
//                //    info.Append(trans.toDot().ToString());
//                //}
//                string name = state.ID.Replace("$", "00").Replace("\u03C4", "t");
//                foreach (Distribution dis in state.Distributions)
//                {
//                    foreach (KeyValuePair<double, MDPState> keyValuePair in dis.States)
//                    {
//                        info.Append(name + " ->" + keyValuePair.Value.ID.Replace("$", "00").Replace("\u03C4", "t") + "[label=" + dis.Event + keyValuePair.Key * 1000 + "];\n");
//                    }
//                }
//                //a->{b;c;d}[label="coo0.33l"];
                
//            }

//            info.Append("\n}\n");

//            try
//            { 
//                StreamWriter fwriter = File.CreateText("d:\\test\\" + fileName);
//                fwriter.Write(info.ToString());
//                fwriter.Close();
//            }
//            catch (Exception)
//            {
//                System.Windows.Forms.MessageBox.Show("Writing File Error!");
//            }

//        }
//#endif

//#if DEBUG
//        public static void ToDotWithIndex(HashSet<DTMCState> scc, int i, int j, HashSet<DTMCState> outputs )
//        {

//            //report index of SCC states
//            Dictionary<DTMCState, int> stateIndex = new Dictionary<DTMCState, int>(scc.Count);
//            int stateID = 0;
//            foreach (var mdpState in scc)
//            {
//                stateIndex.Add(mdpState, stateID);
//                stateID++;
//            }

//            foreach (var mdpState in outputs)
//            {
//                stateIndex.Add(mdpState, stateID);
//                stateID++;
//            }

//            StringBuilder info = new StringBuilder();

//            string fileName = j + "_" + i + "cutGraph.dot";

//            info.Append("digraph " + j + "00" + i + "{ \n");
//            info.Append("rankdir=LR;\n");

//            foreach (DTMC state in scc)
//            {
//                //info.Append(state.toDot().ToString());

//                //foreach ( trans in state.)
//                //{
//                //    info.Append(trans.toDot().ToString());
//                //}
//                string name = stateIndex[state].ToString(CultureInfo.InvariantCulture);// +state.ID.Replace("$", "00").Replace("\u03C4", "t");
//                foreach (Distribution dis in state.Distributions)
//                {
//                    foreach (KeyValuePair<double, MDPState> keyValuePair in dis.States)
//                    {
//                        //info.Append(name + " ->" + stateIndex[keyValuePair.Value] + "9" + keyValuePair.Value.ID.Replace("$", "00").Replace("\u03C4", "t") + "[label=" + dis.Event + keyValuePair.Key * 1000 + "];\n");
//                        info.Append(name + " ->" + stateIndex[keyValuePair.Value] + "[label=" + dis.Event + keyValuePair.Key * 1000 + "];\n");
//                    }
//                }
//                //a->{b;c;d}[label="coo0.33l"];

//            }

//            info.Append("\n}\n");

//            try
//            {
//                StreamWriter fwriter = File.CreateText("d:\\test1\\" + fileName);
//                fwriter.Write(info.ToString());
//                fwriter.Close();
//            }
//            catch (Exception)
//            {
//                System.Windows.Forms.MessageBox.Show("Writing File Error!");
//            }

//        }
//#endif
    }


}
