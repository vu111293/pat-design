using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using PAT.Common.Classes.BA;
using PAT.Common.Classes.DataStructure;
using PAT.Common.Classes.LTS;
using PAT.Common.Classes.ModuleInterface;
using PAT.Common.Classes.Ultility;
using System.Collections.Concurrent;
using System.IO;

namespace PAT.Common.Classes.SemanticModels.LTS.Assertion
{
    public partial class AssertionLTL
    {
        private int CORES = Environment.ProcessorCount;

        // data used to obtain result
        private Dictionary<string, List<string>> finalOutgoingTransitionTable;
        private Dictionary<string, LocalPair> finalAcceptingCycle;
        private Stack<LocalPair> finalLocalTaskStack;

        // control variable
        private bool isStop;
        private Object reportLocker;

        // shared data between threads for improved SCC-based algorithms
        ConcurrentDictionary<string, bool> sharedSCCStates;

        // shared data between threads for Nested DFS-based Shared Red States algorithms
        ConcurrentDictionary<string, bool> sharedRedStates;
        ConcurrentDictionary<string, int> sharedAcceptingCountRedDFS;

        // shared data between threads for Combination Parallel Nested-DFS algorithms
        ConcurrentDictionary<string, bool> sharedBlueRedStates; // being in this shared variable is global blue, if global red then bool variable is true

        // data for overlap calculation
        ConcurrentDictionary<string, int> visitedTimes;
        
        // data for 1 core algorithm
        int expendedNodesNumber;

        // set number of cores
        public int setCores(int expectedCores)
        {
            if (expectedCores > CORES)
            {
                expectedCores = CORES;
            }
            return expectedCores;
        }

        // common function
        public void writeOverlap(ConcurrentDictionary<string, int> visitedTimes)
        {
            // total visited states
            int sum = 0;
            Dictionary<int, int> countTimes = new Dictionary<int, int>(1024);
            foreach (KeyValuePair<string, int> kv in visitedTimes)
            {
                // increase number of states
                sum++;

                // count states with the same visited times
                int times = kv.Value;
                if (!countTimes.ContainsKey(times))
                {
                    countTimes.Add(times, 0);
                }
                countTimes[times]++;
            }

            // get the path of overlap file
            string path = System.Environment.CurrentDirectory;
            path += @"\Overlap.txt";

            // write to file
            using (StreamWriter w = File.AppendText(path))
            {
                foreach (KeyValuePair<int, int> kv in countTimes)
                {
                    w.WriteLine(kv.Key + ": " + kv.Value + "/" + sum);
                }
                w.WriteLine("--------------------");
                w.Flush();
                w.Close();
            }
        }

        // write number of expended to file
        public void writeExpendedNodesNumber()
        {
            // get the path of overlap file
            string path = System.Environment.CurrentDirectory;
            path += @"\Overlap.txt";
            using (StreamWriter w = File.AppendText(path))
            {
                w.WriteLine(expendedNodesNumber);
                w.WriteLine("--------------------");
                w.Flush();
                w.Close();
            }
        }

        public sealed class Color
        {
            private bool b1, b2;
            public Color()
            {
                b1 = false; b2 = false;
            }

            // set
            public void setWhite()
            {
                b1 = false; b2 = false;
            }
            public void setCyan()
            {
                b1 = false; b2 = true;
            }
            public void setBlue()
            {
                b1 = true; b2 = false;
            }
            public void setPink()
            {
                b1 = true; b2 = true;
            }

            // compare
            public bool isWhite()
            {
                return (!b1 && !b2);
            }
            public bool isCyan()
            {
                return (!b1 && b2);
            }
            public bool isBlue()
            {
                return (b1 && !b2);
            }
            public bool isPink()
            {
                return (b1 && b2);
            }
        }
    }
}