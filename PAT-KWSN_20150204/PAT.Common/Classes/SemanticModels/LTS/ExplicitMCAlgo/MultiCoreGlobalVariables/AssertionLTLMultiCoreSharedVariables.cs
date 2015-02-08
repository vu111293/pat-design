using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using PAT.Common.Classes.BA;
using PAT.Common.Classes.DataStructure;
using PAT.Common.Classes.LTS;
using PAT.Common.Classes.ModuleInterface;
using PAT.Common.Classes.Ultility;

namespace PAT.Common.Classes.SemanticModels.LTS.Assertion
{
    public partial class AssertionLTL
    {        
        private Object MultiCoreLock; // lock for multicore algorithm
        private bool StopMutliCoreThreads;

        // Variables to store result
        private Dictionary<string, List<string>> MultiCoreOutgoingTransitionTable;
        private Dictionary<string, LocalPair> MultiCoreResultedLoop;
        private Stack<LocalPair> MultiCoreLocalTaskStack;
        
        private int MultiCoreSeed = 0; // Help to generate different random variable

        // Generate a random number from 0 to n-1 using the rand variable
        private int[] generatePermutation(int n, Random rand)
        {
            int[] permutation = new int[n];
            for (int i = 0; i < n; i++)
            {
                permutation[i] = i;
            }

            int randIndex, temp;
            for (int i = n - 1; i >= 0; i--)
            {
                randIndex = rand.Next(i + 1);
                temp = permutation[i];
                permutation[i] = permutation[randIndex];
                permutation[randIndex] = temp;
            }

            return permutation;
        }
    }
}