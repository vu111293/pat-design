using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PAT.Common.Classes.SemanticModels.LTS.BDD
{
    public class Block
    {
        public int index;
        public List<int> states;

        /// <summary>
        /// Remove set by label of this block
        /// </summary>
        public HashSet<int>[] remove;

        public Counter relCount;

        public Block child;

        public Block(int nSymbols, int nStates, List<int> states)
        {
            index = -1;
            this.states = states;
            remove = new HashSet<int>[nSymbols];
            relCount = new Counter(nSymbols, nStates);

            child = this;
        }

        public HashSet<int> CopyRow(int label)
        {
            return new HashSet<int>(remove[label]);
        }
    }
}
