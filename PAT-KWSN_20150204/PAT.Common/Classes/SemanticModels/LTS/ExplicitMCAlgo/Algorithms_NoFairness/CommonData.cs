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
        //number of cores
        public const int CORES = 1;

        //final counter example
        public List<ConfigurationBase> finalTrace;
        public int finalLoopIndex;

        public bool isGlobalStop;
        public object globalCounterExampleLocker;

        //MultiCore Nested DFS
        ConcurrentDictionary<string, bool> globalRedStates;
        ConcurrentDictionary<string, int> globalAcceptingCounter;

        //Improved MultiCore Nested DFS
        ConcurrentDictionary<string, bool> globalBlueRedStates;

        //MuiltiCore Improved Tarjan
        ConcurrentDictionary<string, bool> globalFoundSCCs;

        //Queue fair Tarjan
        ConcurrentQueue<SCC>[] queueSCCArray;

        //visited times
        ConcurrentDictionary<string, int> allVisitedStates;

        //all states
        //ConcurrentDictionary<string, bool> allVisitedStates;

        //generation permutation
        int[] Permutation(int len, Random rand)
        {
            List<int> iList = new List<int>(len);
            int[] iArray = new int[len];
            for (int i = 0; i < len; i++)
            {
                iList.Add(i);
            }
            while (iList.Count > 0)
            {
                int curLen = iList.Count;
                int r = rand.Next(curLen);
                iArray[curLen - 1] = iList[r];
                iList.RemoveAt(r);
            }
            return iArray;
        }

        //write visited times to files
        public void writeOverlapToFile(ConcurrentDictionary<string, int> allVisitedStates)
        {
            int sum = allVisitedStates.Count;
            Dictionary<int, int> countTimes = new Dictionary<int, int>(1024);
            foreach (KeyValuePair<string, int> kv in allVisitedStates)
            {
                //count states with the same visited times
                int times = kv.Value;
                if (!countTimes.ContainsKey(times))
                {
                    countTimes.Add(times, 0);
                }
                countTimes[times]++;
            }

            //get the path of overlap file
            string path = System.Environment.CurrentDirectory;
            path += @"\Overlap.txt";

            //write to file
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

    public sealed class SCC
    {
        public Dictionary<string, LocalPair> component;
        public Dictionary<string, List<string>> transitionTable;
        public List<ConfigurationBase> trace;
        public int loopIndex;

        public SCC(Dictionary<string, LocalPair> component, Dictionary<string, List<string>> transitionTable, List<ConfigurationBase> trace, int loopIndex)
        {
            this.component = component;
            this.transitionTable = transitionTable;
            this.trace = trace;
            this.loopIndex = loopIndex;
        }
    }
}
