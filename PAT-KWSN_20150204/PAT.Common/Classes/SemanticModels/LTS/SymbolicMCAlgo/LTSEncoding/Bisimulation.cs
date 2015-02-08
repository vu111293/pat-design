using System.Collections.Generic;
using System.Linq;

namespace PAT.Common.Classes.SemanticModels.LTS.BDD
{
    /// <summary>
    /// Implement the bisimulation algorithm from book Principle of Model Checking, page 487
    /// </summary>
    public class Bisimulation
    {
        /// <summary>
        /// All states are maped to integer numbers from 0 to n-1
        /// </summary>
        private static Dictionary<State, int> stateMapping;

        /// <summary>
        /// All transitions are maped to integer numbers from 0 to m-1
        /// </summary>
        private static Dictionary<string, int> labelMapping;

        private static Lts lts;



        /// <summary>
        /// Store C in Pi where exists C' in PiOld, C \subset C' and |C| le |C'| / 2
        /// </summary>
        private static Block child;
 
        public static void BisimulationPartition(SymbolicLTS actionbasedLTS)
        {
            ClearStaticData();
            Convert(actionbasedLTS);

            List<Block> partition = Compute();

            BuildQuotientGraph(partition, actionbasedLTS);
        }

        private static List<Block> Compute()
        {
            List<Block> piOld = new List<Block>(){new Block(lts.states)};

            List<Block> pi = Refine(piOld, lts.states);

            do
            {
                piOld = pi;

                foreach (var C in piOld)
                {
                    pi = Refine(pi, C.states);
                }
            } while (pi.Count != piOld.Count);

            return pi;
        }

        private static List<Block> Refine(List<Block> partition,  List<int> C , List<int> CPrimeMinusC)
        {
            return Refine(Refine(partition, C), CPrimeMinusC);
        }

        private static List<Block> Refine(List<Block> partition, List<int> C)
        {
            List<Block> result = partition;

            for(int i = 0; i < lts.numSymbols; i++)
            {
                result = Refine(result, C, i);
            }

            return result;
        }


        /// <summary>
        /// Refine(partition, C, label) = U Refine(B, C, lable) where B \in parition
        /// Refine(B, C, lable) = {B \interesect Pre(label, C), B \ Pre(label, C)}
        /// </summary>
        /// <param name="partition"></param>
        /// <param name="C"></param>
        /// <param name="label"></param>
        /// <returns></returns>
        private static List<Block> Refine(List<Block> partition, List<int> C, int label)
        {
            bool[] tmp = new bool[lts.numStates];
            bool preCEmpty = true;
            foreach (var cState in C)
            {
                foreach (var preC in lts.Pre(cState, label))
                {
                    preCEmpty = false;
                    tmp[preC] = true;
                }
            }

            //
            if(preCEmpty)
            {
                return partition;
            }

            List<Block> newPartition = new List<Block>();

            foreach (var B in partition)
            {
                if (B.states.Count > 1)
                {
                    List<int> intersect = new List<int>();
                    List<int> minus = new List<int>();

                    foreach (var bState in B.states)
                    {
                        if (tmp[bState])
                        {
                            intersect.Add(bState);
                        }
                        else
                        {
                            minus.Add(bState);
                        }
                    }

                    if (intersect.Count > 0)
                    {
                        Block intersectBlock = new Block(intersect);
                        newPartition.Add(intersectBlock);

                        if (intersect.Count * 2 <= B.states.Count)
                        {
                            child = intersectBlock;
                        }
                    }

                    if (minus.Count > 0)
                    {
                        Block minusBlock = new Block(minus);
                        newPartition.Add(minusBlock);

                        if (minus.Count * 2 <= B.states.Count)
                        {
                            child = minusBlock;
                        }
                    }
                }
                else
                {
                    newPartition.Add(B);
                }
            }

            return newPartition;
        }



        private static void ClearStaticData()
        {
            stateMapping = new Dictionary<State, int>();
            labelMapping = new Dictionary<string, int>();

            child = null;
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

                if (!labelMapping.ContainsKey(label))
                {
                    labelMapping.Add(label, labelMapping.Count);
                }
            }


            int nStates = stateMapping.Count;
            int nLabels = labelMapping.Count;


            Lts result = new Lts(nLabels, nStates);
            
            foreach (var transition in actionbasedLTS.Transitions)
            {
                int q = stateMapping[transition.FromState];
                int a = labelMapping[transition.GetTransitionLabel()];
                int r = stateMapping[transition.ToState];
                result.NewTransition(q, a, r);
            }

            lts = result;
        }

        private static void BuildQuotientGraph(List<Block> partition, SymbolicLTS actionbasedLTS)
        {
            bool allSimpleBlocks = true;
            foreach (var block in partition)
            {
                if (block.states.Count > 1)
                {
                    allSimpleBlocks = false;
                    break;
                }
            }

            //if bisimulation is not helpful
            if (allSimpleBlocks)
            {
                return;
            }

            List<State> newStatesOfActionbasedLTS = new List<State>();

            Dictionary<State, State> mapStatesToBlockRepresentative = new Dictionary<State, State>();
            foreach (var block in partition)
            {
                if (block.states.Count > 1)
                {
                    State blockRepresentative = actionbasedLTS.AddState();
                    newStatesOfActionbasedLTS.Add(blockRepresentative);

                    //add this blockRepresentative to mapOldLoc2NewStates of all locations of element's locations
                    foreach (var state in block.states)
                    {
                        string location = actionbasedLTS.States[state].Label;
                        if(!actionbasedLTS.mapOldLoc2NewStates[location].Contains(blockRepresentative))
                        {
                            actionbasedLTS.mapOldLoc2NewStates[location].Add(blockRepresentative);
                        }
                    }


                    //set name as concatenation of all elements' names
                    //update actionbasedLTS.mapOldLoc2NewStates by removing states in partition
                    string newName = "(";
                    foreach (var state in block.states)
                    {
                        newName += actionbasedLTS.States[state].Name + "+";

                        mapStatesToBlockRepresentative.Add(actionbasedLTS.States[state], blockRepresentative);

                        //update mapOldLoc2NewStates
                        actionbasedLTS.mapOldLoc2NewStates[actionbasedLTS.States[state].Label].Remove(
                            actionbasedLTS.States[state]);
                    }

                    blockRepresentative.Name = newName + ")";
                }
                else
                {
                    newStatesOfActionbasedLTS.Add(actionbasedLTS.States[block.states[0]]);
                }

                //in whatever case, clear incoming and outgoing data
                foreach (var state in block.states)
                {
                    actionbasedLTS.States[state].OutgoingTransitions.Clear();
                    actionbasedLTS.States[state].IncomingTransition.Clear();
                }
            }


            List<string> allTransitions = new List<string>();
            List<Transition> transitionsToAdd = new List<Transition>();

            foreach (var transition in actionbasedLTS.Transitions)
            {
                if (mapStatesToBlockRepresentative.ContainsKey(transition.FromState))
                {
                    transition.FromState = mapStatesToBlockRepresentative[transition.FromState];
                }

                if (mapStatesToBlockRepresentative.ContainsKey(transition.ToState))
                {
                    transition.ToState = mapStatesToBlockRepresentative[transition.ToState];
                }

                if (!allTransitions.Contains(transition.ToString()))
                {
                    allTransitions.Add(transition.ToString());
                    transitionsToAdd.Add(transition);
                }
            }

            actionbasedLTS.States = newStatesOfActionbasedLTS;
            actionbasedLTS.SetTransitions(transitionsToAdd);

            if (mapStatesToBlockRepresentative.ContainsKey(actionbasedLTS.InitialState))
            {
                actionbasedLTS.InitialState = mapStatesToBlockRepresentative[actionbasedLTS.InitialState];
            }
        }

        /// <summary>
        /// Return A\B
        /// </summary>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <returns></returns>
        private static List<int> MinusSet(List<int> A, List<int> B )
        {
            List<int> result = new List<int>();

            foreach (var a in A)
            {
                if(!B.Contains(a))
                {
                    result.Add(a);
                }
            }

            return result;
        }

        private class Block
        {
            public List<int> states;
 
            public Block(List<int> states)
            {
                this.states = states;
            }

            public override string ToString()
            {
                string result = string.Empty;

                foreach (var state in states)
                {
                    result += state + ".";
                }

                return result;
            }
        }
    }
}
