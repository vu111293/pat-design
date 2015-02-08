using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PAT.Common.Classes.SemanticModels.LTS.BDD
{
    public class SimRel
    {
        /// <summary>
        /// data[i][j] if block i has relation with block i
        /// </summary>
        public bool[,] data;
        public int size;

        /// <summary>
        /// Store the next index of new block
        /// </summary>
        public int index;

        public SimRel(int initSize)
        {
            data = new bool[initSize,initSize];
            
            size = initSize;
            index = 0;
        }

        /// <summary>
        /// Return the new block index
        /// </summary>
        /// <returns></returns>
        public int NewEntry()
        {
            if (index == size)
            {
                int newSize = size * 2;
                bool[,] a = new bool[newSize, newSize];

                for (int i = 0; i < size; i++)
                {
                    for (int j = 0; j < size; j++)
                    {
                        a[i, j] = data[i, j];
                    }
                }

                data = a;
                size = newSize;

            }
            index++;

            return index - 1;
        }

        public bool GetRelation(int index1, int index2)
        {
            return data[index1, index2];
        }

        public void SetRelation(int index1, int index2, bool value)
        {
            data[index1, index2] = value;
        }
    }
}
