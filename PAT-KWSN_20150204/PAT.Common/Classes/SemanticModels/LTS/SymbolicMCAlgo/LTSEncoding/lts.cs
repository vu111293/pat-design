using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PAT.Common.Classes.SemanticModels.LTS.BDD
{
    public class Lts
    {
        /// <summary>
        /// dataPre[label][target]
        /// Set of states having a transition a, sorting by target states
        /// </summary>
        public List<int>[][] dataPre;

        public int numSymbols;
        public int numStates;
        public int numTrans;
        public List<int> states;

        public Lts(int numberSymbols, int numberStates)
        {
            dataPre = new List<int>[numberSymbols][];
            for(int i = 0; i < numberSymbols; i++)
            {
                dataPre[i] = new List<int>[numberStates];
                for(int j = 0; j < numberStates; j++)
                {
                    dataPre[i][j] = new List<int>();
                }
            }

            numSymbols = numberSymbols;
            numStates = numberStates;
            numTrans = 0;

            states = new List<int>();
            for(int i = 0; i < numStates; i++)
            {
                states.Add(i);
            }
        }

        public void NewTransition(int q, int a, int r)
        {
            dataPre[a][r].Add(q);
            numTrans++;
        }

        /// <summary>
        /// Return set of states having transition a to q
        /// </summary>
        /// <param name="q"></param>
        /// <param name="a"></param>
        /// <returns></returns>
        public List<int> Pre(int q, int a)
        {
            return dataPre[a][q];
        }

        /// <summary>
        /// Set of states having transition a, sorting by source state
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public List<int>[] DataPost(int a)
        {
            List<int>[] arr = new List<int>[numStates];
            for (int i = 0; i < numStates; i++ )
            {
                arr[i] = new List<int>();
            }

            for (int r = 0; r < dataPre[a].Length; r++)
            {
                List<int> pre_r = dataPre[a][r];

                foreach (var q in pre_r)
                {
                    arr[q].Add(r);
                }
            }

            return arr;
        }
    }
}
