using System;
using System.Collections.Generic;

namespace PAT.Common.Classes.CUDDLib
{
    /// <summary>
    /// Manage variables and their lower-bound and upper-bound value and the module it belongs
    /// </summary>
    public class VariableList
    {
        private List<string> names = new List<string>();
        private List<int> lows = new List<int>();
        private List<int> highs = new List<int>();
        private List<int> inits = new List<int>();
        private List<int> moduleIndexes = new List<int>();

        public void AddNewVariable(string name, int low, int high)
        {
            names.Add(name);
            lows.Add(low);
            highs.Add(high);
            moduleIndexes.Add(-1);
        }

        public void AddNewVariable(string name, int low, int high, int init, int moduleIndex)
        {
            names.Add(name);
            lows.Add(low);
            highs.Add(high);
            inits.Add(init);
            moduleIndexes.Add(moduleIndex);
        }

        public int GetVarLow(int index)
        {
            return lows[index];
        }

        public int GetVarHigh(int  index)
        {
            return highs[index];
        }

        public int GetVarLow(string name)
        {
            int index = GetVarIndex(name);
            return lows[index];
        }

        public int GetVarHigh(string name)
        {
            int index = GetVarIndex(name);
            return highs[index];
        }

        public int GetVarInit(string name)
        {
            int index = GetVarIndex(name);
            return inits[index];
        }


        public int GetVarInit(int index)
        {
            return inits[index];
        }

        public int GetVarIndex(string name)
        {
            return names.IndexOf(name);
        }

        public int GetModuleIndex(int index)
        {
            return moduleIndexes[index];
        }


        /// <summary>
        /// Check whether a variable exists
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool ContainsVar(string name)
        {
            return names.Contains(name);
        }


        /// <summary>
        /// Return number of bits used to represent this variable
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public int GetNumberOfBits(string name)
        {
            int range = GetVarHigh(name) - GetVarLow(name) + 1;
            return (int)Math.Ceiling(Math.Log(range, 2));
        }

        /// <summary>
        /// Return number of bits used to represent this variable
        /// </summary>
        /// <param name="index">Index of that variable</param>
        /// <returns></returns>
        public int GetNumberOfBits(int index)
        {
            int range = highs[index] - lows[index] + 1;
            return (int)Math.Ceiling(Math.Log(range, 2));
        }

        public int GetNumberOfVar()
        {
            return names.Count;
        }
    }
}