using System;


namespace PAT.Common.Classes.SemanticModels.LTS.BDD
{
    public class Counter
    {
        /// <summary>
        /// data[label][state]
        /// </summary>
        private int[,] data;

        public Counter(int numberSymbols, int numberStates)
        {
            data = new int[numberSymbols, numberStates];
        }

        public bool IsZero(int a, int q)
        {
            return data[a, q] == 0;
        }

        public void Decr(int a, int q)
        {
            data[a, q]--;
        }

        public void Incr(int a, int q)
        {
            data[a, q]++;
        }

        public void CopyCounter(Counter c)
        {
            for(int i = 0; i < data.GetLength(0); i++)
            {
                for (int j = 0; j < data.GetLength(1); j++ )
                {
                    data[i, j] = c.data[i, j];
                }
            }
        }
    }
}
