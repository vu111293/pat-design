using System;
using System.Collections.Generic;
using System.Diagnostics;
using PAT.Common.Classes.Expressions.ExpressionClass;

namespace PAT.Common.Classes.SemanticModels.LTS.BDD
{
    /// <summary>
    /// Implement the orginal LRT algorithm in the papger Optimizing an LTS-simulation algorithm, page 2
    /// </summary>
    public class Simulation
    {
        /// <summary>
        /// All states are maped to integer numbers from 0 to n-1
        /// </summary>
        private static Dictionary<State, int> stateMapping;

        /// <summary>
        /// All transitions are maped to integer numbers from 0 to m-1
        /// </summary>
        private static Dictionary<string, int> labelMapping;

        private static List<List<Block>> partitionPerLoc = new List<List<Block>>();
        private static List<SimRel> relationPerLoc = new List<SimRel>();
    
        /// <summary>
        /// Store resulted partitions of a single time
        /// </summary>
        private static List<Block> partition;

        /// <summary>
        /// Store block of states in different locations
        /// </summary>
        private static List<Block> tempParition;
 
        private static SimRel relation;
        private static Lts lts;


        /// <summary>
        /// Block which the state belonging to
        /// </summary>
        private static Block[] state2Block;

        /// <summary>
        /// delta[a] set of target states
        /// </summary>
        private static HashSet<int>[] delta;

        /// <summary>
        /// delta1[a] set of source states
        /// </summary>
        private static HashSet<int>[] delta1;

        private static bool[] tmp;

        private static List<int> removeLabels = new List<int>();
        private static List<Block> removeBlocks = new List<Block>();

        private static string lastProcessName = string.Empty;

        private static void ResetData()
        {
            stateMapping = new Dictionary<State, int>();
            labelMapping = new Dictionary<string, int>();

            removeLabels = new List<int>();
            removeBlocks = new List<Block>();

            tempParition = new List<Block>();
            partition = new List<Block>();
            relation = new SimRel(1);
        }

        public static void EraseLastProcName()
        {
            lastProcessName = string.Empty;
        }

        public static void Partition(SymbolicLTS actionbasedLTS)
        {
            //get the last result
            if(actionbasedLTS.Name == lastProcessName)
            {
                actionbasedLTS.partitionPerLoc = partitionPerLoc;
                actionbasedLTS.relationPerLoc = relationPerLoc;
                return;
            }

            EraseLastProcName();
            lastProcessName = actionbasedLTS.Name;

            partitionPerLoc = new List<List<Block>>();
            relationPerLoc = new List<SimRel>();

            foreach (var pair in actionbasedLTS.mapOldLoc2NewStates)
            {
                Initalize(pair.Value);
                Compute();

                List<Block> newPartition = new List<Block>();
                foreach (var block in partition)
                {
                    if(!tempParition.Contains(block))
                    {
                        newPartition.Add(block);
                    }
                }

                partitionPerLoc.Add(newPartition);
                relationPerLoc.Add(relation);
            }

            actionbasedLTS.partitionPerLoc = partitionPerLoc;
            actionbasedLTS.relationPerLoc = relationPerLoc;
        }

        private static void Convert(SymbolicLTS actionbasedLTS)
        {
            //Use integer numbers for state and labels
            foreach (var state in actionbasedLTS.States)
            {
                stateMapping.Add(state, stateMapping.Count);
            }

            foreach (var transition in actionbasedLTS.Transitions)
            {
                string label = transition.GetTransitionLabel();

                if(!labelMapping.ContainsKey(label))
                {
                    labelMapping.Add(label, labelMapping.Count);
                }
            }


            int nStates = stateMapping.Count;
            int nLabels = labelMapping.Count;


            Lts result = new Lts(nLabels, nStates);
            delta = new HashSet<int>[nLabels];
            delta1 = new HashSet<int>[nLabels];

            for (int i = 0; i < nLabels; i++)
            {
                delta[i] = new HashSet<int>();
                delta1[i] = new HashSet<int>();
            }

            foreach (var transition in actionbasedLTS.Transitions)
            {
                int q = stateMapping[transition.FromState];
                int a = labelMapping[transition.GetTransitionLabel()];
                int r = stateMapping[transition.ToState];
                result.NewTransition(q, a, r);

                //
                delta[a].Add(r);
                delta1[a].Add(q);
            }

            lts = result;
        }

        /// <summary>
        /// List of states in the same location
        /// </summary>
        /// <param name="states"></param>
        private static void Initalize(List<State> states)
        {
            ResetData();

            //Use integer numbers for state and labels
            List<Transition> transitions = new List<Transition>();
            List<int> statesInSameLoc = new List<int>();
            List<int> statesInDifferentLoc = new List<int>();
            foreach (var state in states)
            {
                statesInSameLoc.Add(stateMapping.Count);
                stateMapping.Add(state, stateMapping.Count);

                transitions.AddRange(state.OutgoingTransitions);
            }

            foreach (var transition in transitions)
            {
                string label = transition.GetTransitionLabel();

                if (!labelMapping.ContainsKey(label))
                {
                    labelMapping.Add(label, labelMapping.Count);
                }

                if(!stateMapping.ContainsKey(transition.ToState))
                {
                    statesInDifferentLoc.Add(stateMapping.Count);
                    stateMapping.Add(transition.ToState, stateMapping.Count);
                }
            }


            int nStates = stateMapping.Count;
            int nLabels = labelMapping.Count;


            Lts result = new Lts(nLabels, nStates);
            delta = new HashSet<int>[nLabels];
            delta1 = new HashSet<int>[nLabels];

            for (int i = 0; i < nLabels; i++)
            {
                delta[i] = new HashSet<int>();
                delta1[i] = new HashSet<int>();
            }

            foreach (var transition in transitions)
            {
                int q = stateMapping[transition.FromState];
                int a = labelMapping[transition.GetTransitionLabel()];
                int r = stateMapping[transition.ToState];
                result.NewTransition(q, a, r);

                //
                delta[a].Add(r);
                delta1[a].Add(q);
            }

            lts = result;

            //
            //Line 1
            state2Block = new Block[lts.numStates];

            Block blockOfStateSameLoc = new Block(lts.numSymbols, lts.numStates, statesInSameLoc);
            blockOfStateSameLoc.index = relation.NewEntry();

            foreach (var state in statesInSameLoc)
            {
                state2Block[state] = blockOfStateSameLoc;
            }

            partition.Add(blockOfStateSameLoc);

            relation.SetRelation(blockOfStateSameLoc.index, blockOfStateSameLoc.index, true);

            foreach (var state in statesInDifferentLoc)
            {
                Block blockSingleState = new Block(lts.numSymbols, lts.numStates, new List<int>(){state});
                blockSingleState.index = relation.NewEntry();

                state2Block[state] = blockOfStateSameLoc;

                partition.Add(blockSingleState);
                tempParition.Add(blockSingleState);
                relation.SetRelation(blockSingleState.index, blockSingleState.index, true);
            }

            //Line 2-4
            foreach (var B in partition)
            {
                for (int a = 0; a < lts.numSymbols; a++)
                {
                    List<int>[] post_a = lts.DataPost(a);
                    List<int>[] pre_a = lts.dataPre[a];

                    //Line 3
                    foreach (var v in delta1[a])
                    {
                        //transition v-a-q
                        foreach (var q in post_a[v])
                        {
                            //q has relation to B
                            if (relation.GetRelation(B.index, GetBlock(q).index))
                            {
                                B.relCount.Incr(a, v);
                            }
                        }
                    }

                    tmp = new bool[lts.numStates];
                    for (int i = 0; i < tmp.Length; i++)
                    {
                        tmp[i] = true;
                    }

                    HashSet<int> remove = new HashSet<int>();
                    foreach (var c in partition)
                    {
                        if (relation.GetRelation(B.index, c.index))
                        {
                            foreach (var r in c.states)
                            {
                                foreach (var q in pre_a[r])
                                {
                                    //q-a-r
                                    //state q has transition a to a state in block c having relation with B
                                    tmp[q] = false;
                                }
                            }
                        }
                    }

                    for (int i = 0; i < tmp.Length; i++)
                    {
                        if (tmp[i])
                        {
                            remove.Add(i);
                        }
                    }

                    B.remove[a] = remove;
                    if (remove.Count > 0)
                    {
                        NewTask(a, B);
                    }
                }
            }

        }

        private static void Compute()
        {
            while(removeLabels.Count > 0)
            {
                int a = removeLabels[0];
                Block b = removeBlocks[0];

                removeLabels.RemoveAt(0);
                removeBlocks.RemoveAt(0);

                OneRound(a, b);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="a">Label</param>
        /// <param name="B">Block</param>
        private static void OneRound(int a, Block B)
        {
            HashSet<int> remove = B.remove[a];
            B.remove[a] = new HashSet<int>();

            List<int> bPrev = B.states;

            Split(remove);

            //Find set of block D is a subset of Remove, line 15
            List<Block> removeList = new List<Block>();

            tmp = new bool[lts.numStates];
            for (int i = 0; i < lts.numStates; i++)
            {
                tmp[i] = false;
            }

            //tmp[x] if x in remove
            foreach (var x in remove)
            {
                tmp[x] = true;
            }


            //line 15
            foreach (var d in partition)
            {
                bool notChildOfRemove = false;
                foreach (var s in d.states)
                {
                    if(!tmp[s])
                    {
                        notChildOfRemove = true;
                        break;
                    }
                }

                if(!notChildOfRemove)
                {
                    removeList.Add(d);
                }

            }


            //line 14

            //later from state y, we will find the block containing this state
            //this block may contain some state and be used many time.
            //tmp makes sure that each block C is looped 1 time
            tmp = new bool[partition.Count];
            for (int i = 0; i < partition.Count; i++)
            {
                tmp[i] = true;
            }

            foreach (var x in bPrev)
            {
                //state y has transition a to x (in B)
                foreach (var y in lts.dataPre[a][x])
                {
                    //Now C \intersect delta1(a, B) \notempty
                    Block c = GetBlock(y);
                    if(tmp[c.index])
                    {
                        tmp[c.index] = false;

                        foreach (var d in removeList)
                        {
                            if(c.index == d.index)
                            {
                                throw new Exception();
                            }

                            //line 17
                            if(relation.GetRelation(c.index, d.index))
                            {
                                relation.SetRelation(c.index, d.index, false);

                                for (int b = 0; b < lts.numSymbols; b++)
                                {
                                    HashSet<int> vStates = new HashSet<int>();
                                    foreach (var dstate in d.states)
                                    {
                                        foreach (var v in lts.Pre(dstate, b))
                                        {
                                            if (!vStates.Contains(v))
                                            {
                                                vStates.Add(v);

                                                c.relCount.Decr(b, v);

                                                if (c.relCount.IsZero(b, v))
                                                {
                                                    if (c.remove[b].Count == 0)
                                                    {
                                                        NewTask(b, c);
                                                    }

                                                    c.remove[b].Add(v);
                                                }
                                            }
                                        }
                                    }
                                }

                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Line 9-13
        /// </summary>
        /// <param name="remove"></param>
        private static void Split(HashSet<int> remove)
        {
            List<Block> allBlocks = new List<Block>();

            foreach (var b in partition)
            {
                //For each block, we will split two new blocks: intersect and minus.
                //We will use the current block to store the minus set
                //Create new block to store the interesect set
                List<int> intersect = new List<int>();
                List<int> minus = new List<int>();

                foreach (var state in b.states)
                {
                    if(remove.Contains(state))
                    {
                        intersect.Add(state);
                    }
                    else
                    {
                        minus.Add(state);
                    }
                }

                if(minus.Count == 0 || intersect.Count == 0)
                {
                    allBlocks.Add(b);

                    //set child of b is b, no change
                    b.child = b;
                }
                else
                {
                    b.states = minus;

                    Block newBlock = new Block(lts.numSymbols, lts.numStates, intersect);
                    newBlock.index = relation.NewEntry();


                    //upate state2Block
                    foreach (var s in newBlock.states)
                    {
                        state2Block[s] = newBlock;
                    }

                    b.child = newBlock;

                    allBlocks.Add(b);
                    allBlocks.Add(newBlock);
                }
            }

            foreach (var b in partition)
            {
                //update relation for newBlock
                foreach (var c in partition)
                {
                    if(relation.GetRelation(b.index, c.index))
                    {
                        //Inherit from parent
                        relation.SetRelation(b.index, c.child.index, true);
                        relation.SetRelation(b.child.index, c.index, true);
                        relation.SetRelation(b.child.index, c.child.index, true);
                    }

                }

                if (b.child.index != b.index)
                {
                    //Copy remove
                    for (int i = 0; i < lts.numSymbols; i++)
                    {
                        b.child.remove[i] = b.CopyRow(i);

                        if (b.child.remove[i].Count > 0)
                        {
                            NewTask(i, b.child);
                        }
                    }

                    //Copy Counter
                    b.child.relCount.CopyCounter(b.relCount);
                }
            }

            partition = allBlocks;
        }

        /// <summary>
        /// Line 1-2
        /// </summary>
        /// <returns></returns>
        private static void SetInitPartition(SymbolicLTS actionbasedLTS)
        {
            //Line 1
            partition = new List<Block>();
            relation = new SimRel(1);

            state2Block = new Block[lts.numStates];

            foreach (var pair in actionbasedLTS.mapOldLoc2NewStates)
            {
                
                List<State> statesSameLoc = pair.Value;

                if (statesSameLoc.Count > 0)
                {
                    List<int> states = new List<int>();
                    Block newBlock = new Block(lts.numSymbols, lts.numStates, states);
                    newBlock.index = relation.NewEntry();

                    foreach (var state in statesSameLoc)
                    {
                        states.Add(stateMapping[state]);
                        state2Block[stateMapping[state]] = newBlock;
                    }

                    partition.Add(newBlock);

                    relation.SetRelation(newBlock.index, newBlock.index, true);
                }
            }

            //Line 2-4
            foreach (var B in partition)
            {
                for (int a = 0; a < lts.numSymbols; a++)
                {
                    List<int>[] post_a = lts.DataPost(a);
                    List<int>[] pre_a = lts.dataPre[a];

                    //Line 3
                    foreach (var v in delta1[a])
                    {
                        //transition v-a-q
                        foreach (var q in post_a[v])
                        {
                            //q has relation to B
                            if (relation.GetRelation(B.index, GetBlock(q).index))
                            {
                                B.relCount.Incr(a, v);
                            }
                        }
                    }

                    tmp = new bool[lts.numStates];
                    for (int i = 0; i < tmp.Length; i++)
                    {
                        tmp[i] = true;
                    }

                    HashSet<int> remove = new HashSet<int>();
                    foreach (var c in partition)
                    {
                        if (relation.GetRelation(B.index, c.index))
                        {
                            foreach (var r in c.states)
                            {
                                foreach (var q in pre_a[r])
                                {
                                    //q-a-r
                                    //state q has transition a to a state in block c having relation with B
                                    tmp[q] = false;
                                }
                            }
                        }
                    }

                    for (int i = 0; i < tmp.Length; i++)
                    {
                        if (tmp[i])
                        {
                            remove.Add(i);
                        }
                    }

                    B.remove[a] = remove;
                    if (remove.Count > 0)
                    {
                        NewTask(a, B);
                    }
                }
            }
        }

        /// <summary>
        /// List contains not empty Remove(a, B) 
        /// </summary>
        /// <param name="label"></param>
        /// <param name="b"></param>
        private static void NewTask(int label, Block b)
        {
            removeLabels.Add(label);
            removeBlocks.Add(b);
        }

        private static Block GetBlock(int s)
        {
            return state2Block[s];
        }
    }
}
