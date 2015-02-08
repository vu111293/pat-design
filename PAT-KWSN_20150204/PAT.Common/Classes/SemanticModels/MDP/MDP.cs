using System;
using System.Collections.Generic;
using System.Linq;
using PAT.Common.Classes.DataStructure;
using PAT.Common.Classes.Expressions.ExpressionClass;
using PAT.Common.Classes.ModuleInterface;
using PAT.Common.Classes.Ultility;
using System.Diagnostics;

namespace PAT.Common.Classes.SemanticModels.MDP
{
    public class MDP
    {
        public Dictionary<string, MDPState> States;
        public MDPState InitState;

        private int Precision;
        private double MAX_DIFFERENCE;
        public List<MDPState> TargetStates;
        private List<MDPState> TargetStates_backup = new List<MDPState>();

        public MDP(int precision, double maxdiffer)
        {
            Precision = precision;
            MAX_DIFFERENCE = maxdiffer;
            States = new Dictionary<string, MDPState>(Ultility.Ultility.MC_INITIAL_SIZE);
            TargetStates = new List<MDPState>();
        }

        public void SetInit(MDPState init)
        {
            InitState = init;
        }

        public MDP(MDPState init, int precision, double maxdiffer)
        {
            Precision = precision;
            MAX_DIFFERENCE = maxdiffer;
            States = new Dictionary<string, MDPState>(Ultility.Ultility.MC_INITIAL_SIZE);
            InitState = init;
            States.Add(init.ID, InitState);
            TargetStates = new List<MDPState>();
        }

        public void AddTargetStates(MDPState target)
        {
            TargetStates.Add(target);
            //target.Distributions.Clear();
            target.ReachTarget = true;
            target.CurrentProb = 1;
        }

        public void AddZenoStates(MDPState zeno)
        {
            //TargetStates.Add(target);
            //target.Distributions.Clear();
            zeno.CurrentProb = -1;
        }

        public static string GetNextUnvisitedState(HashSet<string> visited, List<string> currentSCC)
        {
            foreach (string state in currentSCC)
            {
                if (!visited.Contains(state))
                {
                    return state;
                }
            }

            System.Diagnostics.Debug.Assert(false);
            return null;
        }

        public void AddState(MDPState state)
        {
            States.Add(state.ID, state);
        }

        public void AddDistribution(string sourceState, Distribution distribution)
        {
            MDPState state;

            if (States.TryGetValue(sourceState, out state))
            {
                state.AddDistribution(distribution);
            }
        }

        public override string ToString()
        {
            string toReturn = "";

            foreach (KeyValuePair<string, MDPState> keyValuePair in States)
            {
                toReturn += keyValuePair.Value.ToString() + "\r\n";
            }

            return toReturn;
        }

        public void ResetNonTargetState()
        {
            foreach (KeyValuePair<string, MDPState> state in States)
            {
                state.Value.CurrentProb = 0;
                state.Value.CurrentReward = 0;
            }

            //TargetStates = TargetStates_backup;

            foreach (MDPState targetState in TargetStates)
            {
                targetState.CurrentProb = 1;
            }
        }

        public void BackUpTargetStates()
        {
            TargetStates_backup.AddRange(TargetStates);
        }

        public double MaxProbability(VerificationOutput VerificationOutput, bool isZeno = false, bool isAntiChain = false)
        {
            if (TargetStates.Count == 0)
            {
                return 0;
            }

            if (TargetStates.Contains(InitState))
            {
                return 1;
            }

            if (isAntiChain)
            {
                return MaxProbabilityWorkAntiChain(VerificationOutput);
            }
            else if (isZeno)
            {
                return MaxProbabilityWork_Zeno(VerificationOutput);
            }
            //return BuildQuotientMDP(VerificationOutput).MaxProbabilityWork();
            //return BuildQuotientMDPBisimulation(VerificationOutput).MaxProbabilityWork(VerificationOutput);
            //return yourQuotient(VerificationOutput).MaxProbabilityWork(VerificationOutput);
            return MaxProbabilityWork(VerificationOutput);
        }

        public double MaxProbabilityWorkAntiChain(VerificationOutput VerificationOutput)
        {

            //calculate the states whose maximul prob is 1
            //HashSet<MDPState> MaxProbOne = new HashSet<MDPState>(maxProbOne());
            //if (MaxProbOne.Contains(InitState)) return 1;
            //foreach (MDPState state in MaxProbOne)
            //{
            //    AddTargetStates(state);
            //    state.Distributions.Clear();//note here may affect prob
            //}

            HashSet<MDPState> working = new HashSet<MDPState>(TargetStates);
            double maxDifference = 1;
            //int counter = 0;

            while (maxDifference > MAX_DIFFERENCE)
            {
                //counter++;
                VerificationOutput.MDPIterationNumber++;

                maxDifference = 0;

                //get the nodes which should be re-calculated.
                HashSet<MDPState> newWorking = new HashSet<MDPState>();
                foreach (MDPState state in working)
                {
                    foreach (MDPState mdpState in state.Pre)
                    {
                        //if (nonSafe.Contains(mdpState)) //note changed here
                        //{
                        newWorking.Add(mdpState);
                        //}
                    }
                }

                List<MDPState> toRemove = new List<MDPState>();
                foreach (MDPState node in newWorking)
                {
                    double newMax = 0;

                    foreach (Distribution distribution in node.Distributions)
                    {
                        double result = 0;
                        foreach (KeyValuePair<double, MDPState> pair in distribution.States)
                        {
                            result += pair.Key * pair.Value.CurrentProb;
                        }
                        newMax = Math.Max(newMax, result);
                        //newMax = Math.Max(newMax, DistributionProbCalc(node, distribution));//note self-loop detected
                    }

                    if (node.CurrentProb < newMax)
                    {
                        maxDifference = Math.Max(maxDifference, (newMax - node.CurrentProb) / node.CurrentProb);///relative difference
                        node.CurrentProb = newMax;
                        //note: add for subset
                        node.AntichainUpdate = false;
                        foreach (MDPState subnode in node.Sub)
                        {
                            if (subnode.CurrentProb < newMax)
                            {
                                subnode.AntichainUpdate = true;
                                subnode.CurrentProb = newMax;
                            }
                        }
                        //note: add for subset
                    }
                    else if (node.CurrentProb == newMax)
                    {
                        if (!node.AntichainUpdate)
                        {
                            toRemove.Add(node);
                        }
                        else
                        {
                            node.AntichainUpdate = false;
                        }
                    }

                }

                foreach (MDPState i in toRemove)
                {
                    newWorking.Remove(i);
                }

                working = newWorking;
            }
            //System.Console.Write(counter);

            return Ultility.Ultility.RoundProbWithPrecision(InitState.CurrentProb, Precision);
        }

        public double MaxProbabilityWork(VerificationOutput VerificationOutput)
        {

            //calculate the states whose maximul prob is 1
            //HashSet<MDPState> MaxProbOne = new HashSet<MDPState>(maxProbOne());
            //if (MaxProbOne.Contains(InitState)) return 1;
            //foreach (MDPState state in MaxProbOne)
            //{
            //    AddTargetStates(state);
            //    state.Distributions.Clear();//note here may affect prob
            //}



            HashSet<MDPState> working = new HashSet<MDPState>(TargetStates);
            double maxDifference = 1;
            //int counter = 0;

            while (maxDifference > MAX_DIFFERENCE)
            {
                //counter++;
                //VerificationOutput.MDPIterationNumber++;

                maxDifference = 0;

                //get the nodes which should be re-calculated.
                HashSet<MDPState> newWorking = new HashSet<MDPState>();
                foreach (MDPState state in working)
                {
                    foreach (MDPState mdpState in state.Pre)
                    {
                        //if (nonSafe.Contains(mdpState)) //note changed here
                        //{
                        newWorking.Add(mdpState);
                        //}
                    }
                }

                List<MDPState> toRemove = new List<MDPState>();
                foreach (MDPState node in newWorking)
                {
                    double newMax = 0;
                    VerificationOutput.MDPIterationNumber++;
                    foreach (Distribution distribution in node.Distributions)
                    {
                        double result = 0;
                        foreach (KeyValuePair<double, MDPState> pair in distribution.States)
                        {
                            result += pair.Key * pair.Value.CurrentProb;
                        }
                        newMax = Math.Max(newMax, result);
                        //newMax = Math.Max(newMax, DistributionProbCalc(node, distribution));//note self-loop detected
                    }

                    if (node.CurrentProb < newMax)
                    {
                        maxDifference = Math.Max(maxDifference, (newMax - node.CurrentProb) / node.CurrentProb);///relative difference
                        node.CurrentProb = newMax;

                        ////note: add for subset
                        //foreach(MDPState subnode in node.Sub)
                        //{
                        //    if(subnode.CurrentProb < newMax)
                        //    {
                        //        subnode.CurrentProb = newMax;
                        //    }
                        //}
                        ////note: add for subset
                    }
                    else
                    {
                        toRemove.Add(node);
                    }
                }

                foreach (MDPState i in toRemove)
                {
                    newWorking.Remove(i);
                }

                working = newWorking;
            }
            //System.Console.Write(counter);

            return Ultility.Ultility.RoundProbWithPrecision(InitState.CurrentProb, Precision);
        }

        public double MaxProbabilityWork_Zeno(VerificationOutput VerificationOutput)
        {

            //calculate the states whose maximul prob is 1
            //HashSet<MDPState> MaxProbOne = new HashSet<MDPState>(maxProbOne());
            //if (MaxProbOne.Contains(InitState)) return 1;
            //foreach (MDPState state in MaxProbOne)
            //{
            //    AddTargetStates(state);
            //    state.Distributions.Clear();//note here may affect prob
            //}



            HashSet<MDPState> working = new HashSet<MDPState>(TargetStates);
            double maxDifference = 1;
            //int counter = 0;

            while (maxDifference > MAX_DIFFERENCE)
            {
                //counter++;
                //VerificationOutput.MDPIterationNumber++;

                maxDifference = 0;

                //get the nodes which should be re-calculated.
                HashSet<MDPState> newWorking = new HashSet<MDPState>();
                foreach (MDPState state in working)
                {
                    foreach (MDPState mdpState in state.Pre)
                    {
                        //if (nonSafe.Contains(mdpState)) //note changed here
                        //{
                        if (mdpState.CurrentProb >= 0)
                        {
                            newWorking.Add(mdpState);
                        }
                        //}
                    }
                }

                List<MDPState> toRemove = new List<MDPState>();
                foreach (MDPState node in newWorking)
                {
                    double newMax = 0;
                    bool notAllZeno = false;
                    VerificationOutput.MDPIterationNumber++;
                    foreach (Distribution distribution in node.Distributions)
                    {
                        double result = 0;
                        foreach (KeyValuePair<double, MDPState> pair in distribution.States)
                        {
                            if (pair.Key * pair.Value.CurrentProb >= 0)
                            {
                                result += pair.Key * pair.Value.CurrentProb;
                            }
                            else
                            {
                                result = -1;
                                break;
                            }
                        }
                        if (result < 0) continue;
                        newMax = Math.Max(newMax, result);
                        notAllZeno = true;
                        //newMax = Math.Max(newMax, DistributionProbCalc(node, distribution));//note self-loop detected
                    }

                    if (!notAllZeno)
                    {
                        node.CurrentProb = -1;
                        maxDifference = 1;
                        continue;
                    }

                    if (node.CurrentProb < newMax)
                    {
                        maxDifference = Math.Max(maxDifference, (newMax - node.CurrentProb) / node.CurrentProb);///relative difference
                        node.CurrentProb = newMax;

                        ////note: add for subset
                        //foreach(MDPState subnode in node.Sub)
                        //{
                        //    if(subnode.CurrentProb < newMax)
                        //    {
                        //        subnode.CurrentProb = newMax;
                        //    }
                        //}
                        ////note: add for subset
                    }
                    else
                    {
                        toRemove.Add(node);
                    }
                }

                foreach (MDPState i in toRemove)
                {
                    newWorking.Remove(i);
                }

                working = newWorking;
            }
            //System.Console.Write(counter);

            return Ultility.Ultility.RoundProbWithPrecision(InitState.CurrentProb, Precision);
        }

        public double MinProbability(VerificationOutput VerificationOutput, bool isAntiChain = false)
        {
            if (TargetStates.Count == 0)
            {
                return 0;
            }

            if (TargetStates.Contains(InitState))
            {
                return 1;
            }

            if (isAntiChain)
            {
                return MinProbabilityWorkAntiChain(VerificationOutput);
            }

            //return BuildQuotientMDP(VerificationOutput).MinProbabilityWork();
            //return BuildQuotientMDPBisimulation(VerificationOutput).MinProbabilityWork(VerificationOutput);
            return MinProbabilityWork(VerificationOutput);
        }

        public double MinProbabilityWorkAntiChain(VerificationOutput VerificationOutput)
        {

            //calculate the states whose minimal probability is 1
            //HashSet<MDPState> MinProbOne = new HashSet<MDPState>(minProbOne());
            //if (MinProbOne.Contains(InitState)) return 1;
            //foreach (MDPState state in MinProbOne)
            //{
            //    AddTargetStates(state);
            //}

            //TargetStates.AddRange(MinProbOne);

            //HashSet<MDPState> MinProbNotZero = new HashSet<MDPState>(minProbNotZero());
            //if (!MinProbNotZero.Contains(InitState)) return 0;

            HashSet<MDPState> working = new HashSet<MDPState>(TargetStates);

            double maxDifference = 1;
            while (maxDifference > MAX_DIFFERENCE)
            {
                //VerificationOutput.MDPIterationNumber++;

                maxDifference = 0;
                //get the nodes which should be re-calculated.
                HashSet<MDPState> newWorking = new HashSet<MDPState>();
                foreach (MDPState state in working)
                {
                    foreach (MDPState mdpState in state.Pre)
                    {
                        //if (MinProbNotZero.Contains(mdpState)) //note changed here
                        //{
                        newWorking.Add(mdpState);
                        //}
                    }
                }

                List<MDPState> toRemove = new List<MDPState>();
                //HashSet<MDPState> toAdd = new HashSet<MDPState>();

                foreach (MDPState node in newWorking)
                {
                    if (node.CurrentProb == 1)
                    {
                        if (node.AntichainUpdate)
                        {
                            node.AntichainUpdate = false;
                        }
                        else
                        {
                            toRemove.Add(node);
                        }
                        continue;
                    }

                    double newMin = 1;
                    VerificationOutput.MDPIterationNumber++;
                    foreach (Distribution distribution in node.Distributions)
                    {
                        double result = 0;
                        foreach (KeyValuePair<double, MDPState> pair in distribution.States)
                        {
                            result += pair.Key * pair.Value.CurrentProb;
                        }

                        newMin = Math.Min(newMin, result);
                        //newMin = Math.Min(newMin, DistributionProbCalc(node, distribution)); //note self-loop detected
                    }


                    if (node.CurrentProb < newMin)
                    {
                        maxDifference = Math.Max(maxDifference, (newMin - node.CurrentProb) / node.CurrentProb);///relative difference
                        node.CurrentProb = newMin;
                        //note: add for subset
                        node.AntichainUpdate = false;
                        foreach (MDPState subnode in node.Sub)
                        {

                            if (subnode.CurrentProb < newMin)
                            {
                                subnode.AntichainUpdate = true;
                                subnode.CurrentProb = newMin;
                            }

                        }
                        //note: add for subset
                    }
                    else if (node.CurrentProb == newMin)
                    {
                        if (!node.AntichainUpdate)
                        {
                            toRemove.Add(node);
                        }
                        else
                        {
                            node.AntichainUpdate = false;
                        }
                    }

                }

                //newWorking.Union(toAdd);

                foreach (MDPState i in toRemove)
                {
                    newWorking.Remove(i);
                }

                working = newWorking;
            }

            return Ultility.Ultility.RoundProbWithPrecision(InitState.CurrentProb, Precision);
        }

        public double MinProbabilityWork(VerificationOutput VerificationOutput)
        {

            //calculate the states whose minimal probability is 1
            //HashSet<MDPState> MinProbOne = new HashSet<MDPState>(minProbOne());
            //if (MinProbOne.Contains(InitState)) return 1;
            //foreach (MDPState state in MinProbOne)
            //{
            //    AddTargetStates(state);
            //}

            //TargetStates.AddRange(MinProbOne);

            //HashSet<MDPState> MinProbNotZero = new HashSet<MDPState>(minProbNotZero());
            //if (!MinProbNotZero.Contains(InitState)) return 0;


            HashSet<MDPState> working = new HashSet<MDPState>(TargetStates);

            double maxDifference = 1;
            while (maxDifference > MAX_DIFFERENCE)
            {
                //VerificationOutput.MDPIterationNumber++;

                maxDifference = 0;
                //get the nodes which should be re-calculated.
                HashSet<MDPState> newWorking = new HashSet<MDPState>();
                foreach (MDPState state in working)
                {
                    foreach (MDPState mdpState in state.Pre)
                    {
                        //if (MinProbNotZero.Contains(mdpState)) //note changed here
                        //{
                        newWorking.Add(mdpState);
                        //}
                    }
                }

                List<MDPState> toRemove = new List<MDPState>();

                foreach (MDPState node in newWorking)
                {
                    if (node.CurrentProb == 1)
                    {
                        toRemove.Add(node);
                        continue;
                    }
                    double newMin = 1;
                    VerificationOutput.MDPIterationNumber++;
                    foreach (Distribution distribution in node.Distributions)
                    {
                        double result = 0;
                        foreach (KeyValuePair<double, MDPState> pair in distribution.States)
                        {
                            result += pair.Key * pair.Value.CurrentProb;
                        }

                        newMin = Math.Min(newMin, result);
                        //newMin = Math.Min(newMin, DistributionProbCalc(node, distribution)); //note self-loop detected
                    }

                    if (node.CurrentProb < newMin)
                    {
                        maxDifference = Math.Max(maxDifference, (newMin - node.CurrentProb) / node.CurrentProb);///relative difference
                        node.CurrentProb = newMin;
                        ////note: add for subset
                        //foreach (MDPState subnode in node.Sub)
                        //{
                        //    if (subnode.CurrentProb < newMin)
                        //    {
                        //        subnode.CurrentProb = newMin;
                        //    }
                        //}
                        ////note: add for subset
                    }
                    else
                    {
                        toRemove.Add(node);
                    }
                }

                foreach (MDPState i in toRemove)
                {
                    newWorking.Remove(i);
                }

                working = newWorking;

            }

            return Ultility.Ultility.RoundProbWithPrecision(InitState.CurrentProb, Precision);
        }

        //This method calculate the probability through each distribution; self-loop structure is removed to reduce iterations
        public double DistributionProbCalc(MDPState node, Distribution distr)
        {
            double result = 0;
            //bool hasNewValues = false;
            //check the self loop probability; If selfloop exists, e.g. s->s with 0.5, s->t with 0.5, can be transferred to
            //s->t with 1; this could reduce the iterations in some cases.
            //bool hasSelfLoop = false;
            //record the self-loop probability. 

            //record the remaining transition probability
            double nonSelfLoopProb = 0.0;
            for (int i = 0; i < distr.States.Count; i++)
            {
                KeyValuePair<double, MDPState> pair = distr.States[i];
                if (pair.Value == node)
                {
                    //selfLoopProb += pair.Key;
                    //hasSelfLoop = true;
                    //the self loop is removed in this distribution
                    distr.States.Remove(pair);
                    //i-- is used to keep the loop correct after removing one element
                    i--;
                }
                else
                {
                    nonSelfLoopProb += pair.Key;
                }

            }

            if (distr.States.Count == 0)
            {
                result = node.CurrentProb;
            }
            else
            {
                foreach (KeyValuePair<double, MDPState> pair in distr.States)
                {
                    //re-calculate the transition probability after removing self-loop(including the case that no self loop is removed; then nonSelfLoopProb=1)
                    result += pair.Value.CurrentProb * (pair.Key / nonSelfLoopProb);
                }
            }

            return result;
        }

        //re-calculate the distribution after removing selfloop
        //public Distribution DistrbutionReCalculate(List<string> newSCC, MDPState node, Distribution distr)
        //{

        //    double nonSelfLoopProb = 0.0;
        //    for (int i = 0; i < distr.States.Count; i++)
        //    {
        //        KeyValuePair<double, MDPState> pair = distr.States[i];
        //        if (newSCC.Contains(pair.Value.ID))
        //        {
        //            //selfLoopProb += pair.Key;
        //            //hasSelfLoop = true;
        //            //the self loop is removed in this distribution
        //            distr.States.Remove(pair);
        //            //i-- is used to keep the loop correct after removing one element
        //            i--;
        //        }
        //        else
        //        {
        //            nonSelfLoopProb += pair.Key;
        //        }

        //    }
        //    foreach (KeyValuePair<double, MDPState> pair in distr.States)
        //    {
        //        //KeyValuePair<double, MDPState> newPair = new KeyValuePair<double, MDPState>(pair.Key / nonSelfLoopProb, pair.Value);
        //        distr.AddProbStatePair(pair.Key / nonSelfLoopProb, pair.Value);
        //        distr.States.Remove(pair);
        //    }
        //    return distr;
        //}

        //this method is used to remove all loops in a DTMC.



        public MDP BuildQuotientMDPBisimulation(VerificationOutput VerificationOutput)
        {
            MDP toReturn = new MDP(Precision, MAX_DIFFERENCE);

            //calculate the nonsafe states, whose maximal prob is not 0
            Stack<MDPState> NonSafe = new Stack<MDPState>(TargetStates);
            Stack<MDPState> helper = new Stack<MDPState>(TargetStates);

            //backward checking from target states
            while (helper.Count != 0)
            {
                MDPState t = helper.Pop();
                foreach (MDPState s in t.Pre)
                {
                    bool addState = false;

                    //check each distribution; as long as s has a post state in NonSafe, then s should be added.
                    foreach (Distribution distribution in s.Distributions)
                    {
                        foreach (KeyValuePair<double, MDPState> pair in distribution.States)
                        {
                            if (NonSafe.Contains(pair.Value))
                            {
                                addState = true;
                                //s.Distributions.Remove(distribution);
                                break;
                            }
                        }

                        if (addState)
                        {
                            break;
                        }
                    }

                    if (addState && !NonSafe.Contains(s))
                    {
                        helper.Push(s);
                        NonSafe.Push(s);
                    }

                }
            }

            //note here remaining doesn't include the target states and initial states
            HashSet<MDPState> remaining = new HashSet<MDPState>();
            foreach (MDPState mdpState in NonSafe)
            {
                if (!TargetStates.Contains(mdpState) && InitState != mdpState)
                {
                    remaining.Add(mdpState);
                }
            }

            //add Initial
            MDPState Initial = new MDPState(InitState.ID);
            toReturn.AddState(Initial);
            toReturn.SetInit(Initial);

            //add target
            MDPState Target = new MDPState("target");
            toReturn.AddState(Target);
            toReturn.AddTargetStates(Target);

            //add safe
            MDPState Safe = new MDPState("safe");
            toReturn.AddState(Safe);

            //add remaining
            //MDPState Remaining = new MDPState("remaining");
            //toReturn.AddState(Remaining);

            //add selfloop at Target.
            //Target.AddDistribution(new Distribution(Constants.TAU, Target));

            //add selfloop at Safe.
            //Safe.AddDistribution(new Distribution(Constants.TAU, Safe));

            //add the group distributions in Remaining

            List<HashSet<MDPState>> SeperatingRemaining = new List<HashSet<MDPState>>();

            SeperatingRemaining.Add(remaining);

            bool refinement = true;

            while (refinement)//if the former iteration has splitted some groups
            {
                refinement = false;

                for (int i = 0; i < SeperatingRemaining.Count; i++)
                {
                    HashSet<MDPState> mdpStates = SeperatingRemaining[i];

                    if (mdpStates.Count > 1)
                    {
                        Dictionary<string, HashSet<MDPState>> groups = SeperateGroup(SeperatingRemaining, mdpStates, Initial, Target, Safe);

                        if (groups.Count > 1)
                        {
                            SeperatingRemaining.RemoveAt(i--);

                            foreach (KeyValuePair<string, HashSet<MDPState>> keyValuePair in groups)
                            {
                                SeperatingRemaining.Add(keyValuePair.Value);
                            }

                            refinement = true;
                        }
                    }
                }
                //List<HashSet<MDPState>> NewSeperating = new List<HashSet<MDPState>>();

                //foreach (HashSet<MDPState> mdpStates in SeperatingRemaining)
                //{

                //    if (mdpStates.Count > 1)
                //        {
                //            int counter = 0;

                //            foreach (HashSet<MDPState> grouped in SeperateGroup(SeperatingRemaining, mdpStates, Initial, Target, Safe).Values)
                //            {
                //                counter++;
                //                NewSeperating.Add(grouped);
                //            }

                //            if (counter > 1)
                //            {
                //                refinement = true;
                //            }

                //        }
                //        else
                //        {
                //            NewSeperating.Add(mdpStates);
                //        }
                //    }


                ////todo: reduce the loop number
                //SeperatingRemaining = NewSeperating;

            }


            //add distributions after all the refinement
            foreach (HashSet<MDPState> mdpStates in SeperatingRemaining)
            {
                //add representative state of each groupd states
                MDPState Grouped = StateFromHashset(mdpStates);
                toReturn.AddState(new MDPState(Grouped.ID));

            }


            foreach (MDPState state in toReturn.States.Values)
            {
                if (state.ID == "target" || state.ID == "safe")
                {
                    continue;
                }

                HashSet<Distribution> DistrToAdd = new HashSet<Distribution>();

                foreach (Distribution Distr in this.States[state.ID].Distributions)
                {
                    Distribution newDistr = calcGroupedDistr(SeperatingRemaining, Distr, Initial, Target, Safe, toReturn);
                    DistrToAdd.Add(newDistr);
                }

                foreach (Distribution distribution in DistrToAdd)
                {
                    toReturn.AddDistribution(state.ID, distribution);
                }
            }
            //foreach (HashSet<MDPState> mdpStates in SeperatingRemaining)
            //{
            //    MDPState Grouped = StateFromHashset(mdpStates);
            //    FinalgroupedDistribution(SeperatingRemaining, Grouped, Target, Safe, toReturn);//calculate the grouped distributions
            //}

            //note: add initial state's distributions after refinement
            //FinalgroupedDistribution(SeperatingRemaining, InitState, Target, Safe, toReturn);
            return toReturn;
        }

        //public void FinalgroupedDistribution(List<HashSet<MDPState>> AfterRefined, HashSet<MDPState> mdpStates, MDPState Target, MDPState Safe, MDP Returned)
        //{

        //    foreach (MDPState State in mdpStates)
        //    {
        //        FinalgroupedDistribution(AfterRefined, State, Target, Safe, Returned);
        //    }
        //}

        //public void FinalgroupedDistribution(List<HashSet<MDPState>> AfterRefined, MDPState mdpState, MDPState Target, MDPState Safe, MDP Returned)
        //{
        //    HashSet<Distribution> DistrToAdd = new HashSet<Distribution>();

        //    foreach (Distribution Distr in mdpState.Distributions)
        //    {
        //        Distribution newDistr = calcGroupedDistr(AfterRefined, Distr, Returned.InitState, Target, Safe, Returned);
        //        DistrToAdd.Add(newDistr);
        //    }

        //    foreach (Distribution distribution in DistrToAdd)
        //    {
        //        Returned.AddDistribution(mdpState.ID, distribution);
        //    }
        //}

        public Dictionary<string, HashSet<MDPState>> SeperateGroup(List<HashSet<MDPState>> seperation, HashSet<MDPState> mdpStates, MDPState Initial, MDPState Target, MDPState Safe)
        {
            Dictionary<string, HashSet<MDPState>> record = new Dictionary<string, HashSet<MDPState>>();

            foreach (MDPState State in mdpStates)
            {
                string disstring = "";

                HashSet<Distribution> dis = new HashSet<Distribution>();

                foreach (Distribution Distr in State.Distributions)
                {

                    dis.Add(calcGroupedDistr(seperation, Distr, Initial, Target, Safe, this));
                    //disstring += calcGroupedDistr(seperation, Distr, Initial, Target, Safe, this).ToString();
                    //dis += State.ID;
                }

                foreach (var distribution in dis)
                {
                    disstring += distribution.ToString();
                }

                if (record.Count == 0)
                {
                    HashSet<MDPState> states = new HashSet<MDPState>();
                    states.Add(State);
                    record.Add(disstring, states);
                }
                else
                {
                    if (record.ContainsKey(disstring))
                    {
                        //todo: too slow here..
                        record[disstring].Add(State);
                        continue;
                    }

                    HashSet<MDPState> states = new HashSet<MDPState>();
                    states.Add(State);
                    record.Add(disstring, states);
                }
            }

            return record;
        }

        public Distribution calcGroupedDistr(List<HashSet<MDPState>> ListHashMDP, Distribution Distr, MDPState Initial, MDPState Target, MDPState Safe, MDP mdp)
        {
            Distribution toReturn = new Distribution(Distr.Event);

            double toInit = 0;
            double toTarget = 0;
            double[] combined = new double[ListHashMDP.Count];
            //MDPState[] groupstates = new MDPState[seperation.Count];

            foreach (KeyValuePair<double, MDPState> transition in Distr.States)
            {
                if (TargetStates.Contains(transition.Value))
                {
                    toTarget += transition.Key;
                }
                else if (InitState == transition.Value)
                {
                    toInit += transition.Key;
                }
                else
                {
                    for (int i = 0; i < ListHashMDP.Count; i++)
                    {
                        if (ListHashMDP[i].Contains(transition.Value))
                        {
                            combined[i] += transition.Key;
                            break;
                        }
                    }
                }

            }

            double toSafe = 1 - toTarget - toInit;

            for (int i = 0; i < ListHashMDP.Count; i++)
            {
                toSafe -= combined[i];
            }

            if (toInit != 0)
            {
                toReturn.AddProbStatePair(Math.Round(toInit, 5), Initial);
            }

            if (toTarget != 0)
            {
                toReturn.AddProbStatePair(Math.Round(toTarget, 5), Target);
            }

            if (toSafe != 0)
            {
                toReturn.AddProbStatePair(Math.Round(toSafe, 5), Safe);
            }

            //toReturn.AddProbStatePair(toSafe, Safe);
            for (int j = 0; j < ListHashMDP.Count; j++)
            {
                if (combined[j] != 0)
                {
                    MDPState state = mdp.States[StateFromHashset(ListHashMDP[j]).ID];
                    toReturn.AddProbStatePair(Math.Round(combined[j], 5), state);
                    //toReturn.AddProbStatePair(Math.Round(combined[j], 5), StateFromHashset(ListHashMDP[j]));
                }

            }

            return toReturn;
        }

        public MDPState StateFromHashset(HashSet<MDPState> states)
        {
            if (states.Count != 0)
            {
                List<MDPState> States = new List<MDPState>(states);
                return States[0];
            }

            return null;
        }



        public MDP BuildQuotientMDP(VerificationOutput VerificationOutput)
        {
            //return this;

            MDP toReturn = new MDP(Precision, MAX_DIFFERENCE);

            //todo change to set
            List<KeyValuePair<string, string>> BoundaryOneTransition = new List<KeyValuePair<string, string>>();

            //todo change to set
            List<Distribution> ProbTransitions = new List<Distribution>();

            Dictionary<string, List<Distribution>> GlobalProbTransitions = new Dictionary<string, List<Distribution>>();

            StringDictionary<bool> visited = new StringDictionary<bool>(States.Count);
            List<KeyValuePair<HashSet<string>, MDPState>> sccs = new List<KeyValuePair<HashSet<string>, MDPState>>();

            Dictionary<string, int> preorder = new Dictionary<string, int>();
            Dictionary<string, int> lowlink = new Dictionary<string, int>();
            //HashSet<string> scc_found = new HashSet<string>();
            Stack<MDPState> TaskStack = new Stack<MDPState>();

            //Dictionary<string, List<string>> OutgoingTransitionTable = new Dictionary<string, List<string>>();
            Stack<MDPState> stepStack = new Stack<MDPState>(1024);

            visited.Add(InitState.ID, false);
            TaskStack.Push(InitState);

            //# Preorder counter 
            int preor = 0;

            do
            {
                while (TaskStack.Count > 0)
                {
                    MDPState pair = TaskStack.Peek();
                    string v = pair.ID;

                    if (visited.GetContainsKey(v) && visited.GetContainsKey(v))
                    {
                        TaskStack.Pop();
                        continue;
                    }

                    if (!preorder.ContainsKey(v))
                    {
                        preorder.Add(v, preor);
                        preor++;
                    }

                    bool done = true;

                    List<Distribution> list = pair.Distributions;
                    List<MDPState> nonProbTrans = new List<MDPState>();
                    List<Distribution> ProbTrans = new List<Distribution>();

                    for (int i = 0; i < list.Count; i++)
                    {
                        if (list[i].IsTrivial())
                        {
                            nonProbTrans.Add(list[i].States[0].Value);
                        }
                        else
                        {
                            ProbTrans.Add(list[i]);
                        }
                    }

                    if (ProbTrans.Count > 0 && !GlobalProbTransitions.ContainsKey(v))
                    {
                        GlobalProbTransitions.Add(v, ProbTrans);
                        ProbTransitions.AddRange(ProbTrans);
                    }

                    for (int k = nonProbTrans.Count - 1; k >= 0; k--)
                    {
                        MDPState step = nonProbTrans[k];
                        string tmp = step.ID;

                        if (visited.ContainsKey(tmp))
                        {
                            //if this node is still not visited
                            if (!preorder.ContainsKey(tmp))
                            {
                                //only put the first one to the work list stack.
                                //if there are more than one node to be visited, 
                                //simply ignore them and keep its event step in the list.
                                if (done)
                                {
                                    TaskStack.Push(step);
                                    done = false;
                                }
                            }
                        }
                        else
                        {
                            visited.Add(tmp, false);
                            //OutgoingTransitionTable.Add(tmp, new List<string>(8));

                            //only put the first one into the stack.
                            if (done)
                            {
                                TaskStack.Push(step);
                                done = false;
                            }
                        }
                    }

                    if (done)
                    {
                        int lowlinkV = preorder[v];
                        int preorderV = preorder[v];

                        bool selfLoop = false;
                        for (int j = 0; j < nonProbTrans.Count; j++)
                        {
                            string w = nonProbTrans[j].ID;

                            if (w == v)
                            {
                                selfLoop = true;
                            }

                            if (!visited.GetContainsKey(w))
                            {
                                if (preorder[w] > preorderV)
                                {
                                    lowlinkV = Math.Min(lowlinkV, lowlink[w]);
                                }
                                else
                                {
                                    lowlinkV = Math.Min(lowlinkV, preorder[w]);
                                }
                            }
                            else //in this case, there is a tau transition leading to an SCC; must add the transition into the toReturn automaton
                            {
                                BoundaryOneTransition.Add(new KeyValuePair<string, string>(v, w));
                            }
                        }

                        lowlink[v] = lowlinkV;

                        TaskStack.Pop();

                        HashSet<string> scc = new HashSet<string>();

                        if (lowlinkV == preorderV)
                        {
                            scc.Add(v);
                            visited.SetValue(v, true);

                            while (stepStack.Count > 0 && preorder[stepStack.Peek().ID] > preorderV)
                            {
                                string s = stepStack.Pop().ID;

                                scc.Add(s);
                                visited.SetValue(s, true);
                            }

                            MDPState newstate = new MDPState(toReturn.States.Count.ToString());
                            if (scc.Count > 1 || (scc.Count == 1 && selfLoop))
                            {
                                newstate.AddDistribution(new Distribution(Constants.TAU, newstate)); //add self loop: sun jun                                
                            }
                            sccs.Add(new KeyValuePair<HashSet<string>, MDPState>(scc, newstate));

                            toReturn.AddState(newstate);

                            if (scc.Contains(InitState.ID))
                            {
                                toReturn.SetInit(newstate);
                            }

                            foreach (MDPState state in TargetStates)
                            {
                                if (scc.Contains(state.ID))
                                {
                                    toReturn.AddTargetStates(newstate);
                                }
                            }
                        }
                        else
                        {
                            stepStack.Push(pair);
                        }
                    }
                }

                if (ProbTransitions.Count > 0)
                {
                    foreach (Distribution step in ProbTransitions)
                    {
                        foreach (KeyValuePair<double, MDPState> pair in step.States)
                        {
                            string stateID = pair.Value.ID;
                            if (!visited.ContainsKey(stateID))
                            {
                                TaskStack.Push(pair.Value);
                                visited.Add(stateID, false);
                            }
                        }
                    }
                    ProbTransitions.Clear();
                }
            } while (TaskStack.Count > 0);


            foreach (KeyValuePair<string, string> pair in BoundaryOneTransition)
            {
                MDPState source = null;
                MDPState target = null;

                foreach (KeyValuePair<HashSet<string>, MDPState> sccstate in sccs)
                {
                    if (sccstate.Key.Contains(pair.Key))
                    {
                        source = sccstate.Value;
                    }

                    if (sccstate.Key.Contains(pair.Value))
                    {
                        target = sccstate.Value;
                    }
                }

                toReturn.AddDistribution(source.ID, new Distribution(Constants.TAU, target));
                VerificationOutput.ReducedMDPTransitions++;

            }

            foreach (KeyValuePair<string, List<Distribution>> pair in GlobalProbTransitions)
            {
                MDPState source = null;

                foreach (KeyValuePair<HashSet<string>, MDPState> sccstate in sccs)
                {
                    if (sccstate.Key.Contains(pair.Key))
                    {
                        source = sccstate.Value;
                        break;
                    }
                }

                foreach (Distribution distribution in pair.Value)
                {
                    Distribution disNew = new Distribution(distribution.Event);
                    foreach (KeyValuePair<double, MDPState> state in distribution.States)
                    {
                        foreach (KeyValuePair<HashSet<string>, MDPState> sccstate in sccs)
                        {
                            if (sccstate.Key.Contains(state.Value.ID))
                            {
                                disNew.AddProbStatePair(state.Key, sccstate.Value);
                                VerificationOutput.ReducedMDPTransitions++;
                                break;
                            }
                        }
                    }

                    toReturn.AddDistribution(source.ID, disNew);
                }
            }
            VerificationOutput.ReducedMDPStates = toReturn.States.Count;
            return toReturn;
        }



        //=====================Reward Calculation=================================

        //calculate the states whose maximal prob to target states are 1
        public HashSet<MDPState> maxProbOne()
        {
            HashSet<MDPState> toReturn = new HashSet<MDPState>(States.Values);
            bool done = false;
            while (!done)
            {
                HashSet<MDPState> working = new HashSet<MDPState>(TargetStates);
                HashSet<MDPState> visited = new HashSet<MDPState>(TargetStates);

                bool done1 = false;
                while (!done1)
                {
                    int visit = visited.Count;
                    HashSet<MDPState> newWorking = new HashSet<MDPState>();
                    foreach (MDPState state in working)
                    {
                        foreach (MDPState mdpState in state.Pre)
                        {
                            newWorking.Add(mdpState);
                        }
                    }

                    foreach (MDPState state in newWorking)
                    {
                        int toRemove = 0;
                        foreach (Distribution distr in state.Distributions)
                        {
                            bool existToTarget = false;
                            int toRemove1 = toRemove;

                            foreach (var pair in distr.States)
                            {
                                if (visited.Contains(pair.Value))
                                {
                                    existToTarget = true;
                                }
                                if (!toReturn.Contains(pair.Value))
                                {
                                    toRemove++;
                                    break;
                                }
                            }

                            if (toRemove1 == toRemove && existToTarget)
                            {
                                visited.Add(state);
                                break;
                            }
                        }

                    }

                    if (visit == visited.Count)
                    {
                        done1 = true;
                    }
                    else
                    {
                        //working.UnionWith(visited);
                        working = newWorking;
                    }

                }

                if (toReturn.SetEquals(visited))
                {
                    done = true;
                }
                else
                {
                    toReturn = visited;
                }
            }
            return toReturn;
        }

        //calculate the states whose minimal prob to target states are 1
        public HashSet<MDPState> minProbOne()
        {
            HashSet<MDPState> toReturn = new HashSet<MDPState>(States.Values);
            bool done = false;
            while (!done)
            {
                HashSet<MDPState> working = new HashSet<MDPState>(TargetStates);
                HashSet<MDPState> visited = new HashSet<MDPState>(TargetStates);

                bool done1 = false;
                while (!done1)
                {
                    int visit = visited.Count;
                    HashSet<MDPState> newWorking = new HashSet<MDPState>();
                    foreach (MDPState state in working)
                    {
                        foreach (MDPState mdpState in state.Pre)
                        {
                            newWorking.Add(mdpState);
                        }
                    }

                    foreach (MDPState state in newWorking)
                    {
                        bool toRemove = false;

                        foreach (Distribution distr in state.Distributions)
                        {
                            bool existToTarget = false;
                            //distr.States.Value.Contains(state);
                            foreach (var pair in distr.States)
                            {
                                if (visited.Contains(pair.Value))
                                {
                                    existToTarget = true;
                                }

                                if (!toReturn.Contains(pair.Value))
                                {
                                    toRemove = true;
                                    break;
                                }
                            }
                            //existing a distribution that all successive states are not going to targets.
                            if (!existToTarget || toRemove)
                            {
                                toRemove = true;
                                break;
                            }
                        }

                        if (!toRemove)
                        {
                            //if (!isReward)
                            //{
                            //    this.AddTargetStates(state);
                            //}
                            visited.Add(state);
                        }

                    }

                    if (visit == visited.Count)
                    {
                        done1 = true;
                    }
                    else
                    {
                        //working.UnionWith(visited);
                        working = newWorking;
                    }

                }

                if (toReturn.SetEquals(visited))
                {
                    done = true;
                }
                else
                {
                    toReturn = visited;
                }
            }
            return toReturn;
        }


        //calculate the nonsafe states, whose maximal prob to target states are not 0
        public HashSet<MDPState> maxProbNotZero()
        {
            HashSet<MDPState> toReturn = new HashSet<MDPState>(TargetStates);
            HashSet<MDPState> visited = new HashSet<MDPState>(TargetStates);


            //backward checking from target states
            while (visited.Count != 0)
            {
                foreach (MDPState t in visited)
                {

                    visited.Remove(t);

                    foreach (var s in t.Pre)
                    {
                        if (!toReturn.Contains(s))
                        {
                            visited.Add(s);
                            toReturn.Add(s);
                        }
                    }

                    break;//just check the first element in visited
                }
                //MDPState t = visited.Pop();
                //foreach (MDPState s in t.Pre)
                //{
                //bool addState = false;

                ////check each distribution; as long as s has a post state in NonSafe, then s should be added.
                //foreach (Distribution distribution in s.Distributions)
                //{
                //    foreach (KeyValuePair<double, MDPState> pair in distribution.States)
                //    {
                //        if (toReturn.Contains(pair.Value))
                //        {
                //            addState = true;
                //            //s.Distributions.Remove(distribution);
                //            break;
                //        }
                //    }

                //    if (addState)
                //    {
                //        break;
                //    }
                //}


                //}
            }

            return toReturn;
        }

        //calculate the nonsafe states, whose minimal prob to target states are not 0
        public HashSet<MDPState> minProbNotZero()
        {
            //backward checking from target states
            HashSet<MDPState> working = new HashSet<MDPState>(TargetStates);
            HashSet<MDPState> visited = new HashSet<MDPState>(TargetStates);

            bool done = false;
            while (!done)
            {
                int visit = visited.Count;
                HashSet<MDPState> newWorking = new HashSet<MDPState>();
                foreach (MDPState state in working)
                {
                    foreach (MDPState mdpState in state.Pre)
                    {
                        newWorking.Add(mdpState);
                    }
                }

                foreach (MDPState state in newWorking)
                {
                    int keepDistr = 0;
                    bool toremove = false;
                    foreach (Distribution distr in state.Distributions)
                    {
                        int DistrRecord = keepDistr;
                        foreach (var pair in distr.States)
                        {
                            if (visited.Contains(pair.Value))
                            {
                                keepDistr++;
                                break;
                            }
                        }

                        if (DistrRecord == keepDistr)
                        {
                            toremove = true;
                            break;
                        }
                    }

                    if (!toremove)
                    {
                        visited.Add(state);
                    }

                }

                if (visit == visited.Count)
                {
                    done = true;
                }
                else
                {
                    //working.UnionWith(visited);
                    working = newWorking;
                }

            }

            return visited;

        }

        public double MaxReward(VerificationOutput VerificationOutput, RewardConfig reward)
        {
            if (TargetStates.Count == 0)
            {
                return double.PositiveInfinity;
            }

            if (TargetStates.Contains(InitState))
            {
                return 0;
            }

            return MaxRewardWork(VerificationOutput, reward);
        }

        public double MaxRewardWork(VerificationOutput verificationOutput, RewardConfig reward)
        {
            //note here should calculate MinProbNotZero instead of MaxProbNotZero
            //note to test the efficiency of MaxPorbNotZero and MaxProbNotZero
            //HashSet<MDPState> minProbNotZero = new HashSet<MDPState>(MinProbNotZero());

            //note here calculate the states whose minimal prob to targets are 1;
            HashSet<MDPState> minProbOne = new HashSet<MDPState>(this.minProbOne());

            //if Initial state is not in MinProbNotZero, then return Rmax = infinity
            if (!minProbOne.Contains(InitState))
            {
                return double.PositiveInfinity;
            }

            HashSet<MDPState> working = new HashSet<MDPState>(TargetStates);

            HashSet<MDPState> visited = new HashSet<MDPState>(TargetStates);

            double maxDifference = 1;
            //int counter = 0;

            while (maxDifference > MAX_DIFFERENCE || visited.Count < minProbOne.Count)
            {
                //counter++;
                verificationOutput.MDPIterationNumber++;

                maxDifference = 0;

                //get the nodes which should be re-calculated.
                HashSet<MDPState> newWorking = new HashSet<MDPState>();
                foreach (MDPState state in working)
                {
                    foreach (MDPState mdpState in state.Pre)
                    {
                        //if a pre-state is in minProbZero, then Rmax must be infinity because from initial state there is a finite trace to this state.
                        if (!minProbOne.Contains(mdpState))
                        {
                            return double.PositiveInfinity;
                        }

                        newWorking.Add(mdpState);
                    }
                }

                visited.UnionWith(newWorking);

                List<MDPState> toRemove = new List<MDPState>();

                foreach (MDPState node in newWorking)
                {
                    double newMaxReward = 0;

                    foreach (Distribution distribution in node.Distributions)
                    {
                        double result = 0;
                        bool hasNewValues = false;
                        foreach (KeyValuePair<double, MDPState> pair in distribution.States)
                        {
                            //if there is a state which is not in nonsafe, which means from that state we cannot arrive the target states, then the result should be infinity.
                            if (!minProbOne.Contains(pair.Value))
                            {
                                return double.PositiveInfinity;
                            }

                            KeyValuePair<Expression, double> value;
                            if (reward.EventToRewardMapping.TryGetValue(distribution.Event, out value))
                            {
                                result += (value.Value + pair.Value.CurrentReward) * pair.Key;
                            }
                            else
                            {
                                result += pair.Value.CurrentReward * pair.Key;
                            }



                            hasNewValues = true;
                        }

                        if (hasNewValues)
                        {
                            newMaxReward = Math.Max(newMaxReward, result);
                        }
                    }

                    if (node.CurrentReward < newMaxReward)
                    {
                        maxDifference = Math.Max(maxDifference, (newMaxReward - node.CurrentReward) / node.CurrentReward);///relative difference
                        node.CurrentReward = newMaxReward;
                    }
                    else if (node.CurrentReward != 0)
                    {
                        toRemove.Add(node);
                    }
                }

                foreach (MDPState i in toRemove)
                {
                    newWorking.Remove(i);
                }

                working = newWorking;
            }

            return Ultility.Ultility.RoundProbWithPrecision(InitState.CurrentReward, Precision);

        }

        public double MinReward(VerificationOutput VerificationOutput, RewardConfig reward)
        {
            if (TargetStates.Count == 0)
            {
                return double.PositiveInfinity;
            }

            if (TargetStates.Contains(InitState))
            {
                return 0;
            }

            return MinRewardWork(VerificationOutput, reward);
        }

        public double MinRewardWork(VerificationOutput verificationOutput, RewardConfig reward)
        {
            //note here should calculate MaxProbNotZero instead of MinProbNotZero
            //HashSet<MDPState> maxProbNotZero = new HashSet<MDPState>(MaxProbNotZero());
            HashSet<MDPState> maxProbOne = new HashSet<MDPState>(this.maxProbOne());
            if (!maxProbOne.Contains(InitState)) return double.PositiveInfinity;

            HashSet<MDPState> working = new HashSet<MDPState>(TargetStates);

            HashSet<MDPState> visited = new HashSet<MDPState>(TargetStates);

            double maxDifference = 1;
            while (maxDifference > MAX_DIFFERENCE || visited.Count < maxProbOne.Count)
            {
                verificationOutput.MDPIterationNumber++;

                maxDifference = 0;
                //get the nodes which should be re-calculated.
                HashSet<MDPState> newWorking = new HashSet<MDPState>();
                foreach (MDPState state in working)
                {
                    foreach (MDPState mdpState in state.Pre)
                    {
                        //if (nonSafe.Contains(mdpState)) //note changed here
                        //{
                        newWorking.Add(mdpState);
                        //}
                    }
                }

                visited.UnionWith(newWorking);

                List<MDPState> toRemove = new List<MDPState>();

                foreach (MDPState node in newWorking)
                {
                    double newMinReward = double.PositiveInfinity;

                    foreach (Distribution distribution in node.Distributions)
                    {
                        double result = 0;
                        //bool hasNewValues = false;
                        foreach (KeyValuePair<double, MDPState> pair in distribution.States)
                        {
                            if (!maxProbOne.Contains(pair.Value))
                            {
                                result = double.PositiveInfinity;
                            }
                            else
                            {
                                KeyValuePair<Expression, double> value;
                                if (reward.EventToRewardMapping.TryGetValue(distribution.Event, out value))
                                {
                                    result += (value.Value + pair.Value.CurrentReward) * pair.Key;
                                }
                                else
                                {
                                    result += pair.Value.CurrentReward * pair.Key;
                                }

                            }

                            //hasNewValues = true;
                        }

                        //if (hasNewValues)
                        //{
                        newMinReward = Math.Min(newMinReward, result);
                        //}
                    }

                    if (node.CurrentReward < newMinReward) //+ node.StateReward
                    {
                        maxDifference = Math.Max(maxDifference, (newMinReward - node.CurrentReward) / node.CurrentReward);///relative difference + node.StateReward 
                        node.CurrentReward = newMinReward; ;// + node.StateReward
                    }
                    else if (node.CurrentReward != 0)
                    {
                        toRemove.Add(node);
                    }
                }

                foreach (MDPState i in toRemove)
                {
                    newWorking.Remove(i);
                }

                working = newWorking;
            }

            //return Ultility.Ultility.RoundProbWithPrecision(InitState.CurrentProb, Precision);

            return Ultility.Ultility.RoundProbWithPrecision(InitState.CurrentReward, Precision);

        }


        //====================================================================================

        #region

        public MDP ComputeGCPP(VerificationOutput VerificationOutput)
        {
            MDP toReturn = new MDP(Precision, MAX_DIFFERENCE);

            //add Initial
            MDPState Initial = new MDPState(InitState.ID);
            toReturn.AddState(Initial);
            toReturn.SetInit(Initial);

            //add target
            foreach (MDPState targetstate in this.TargetStates)
            {
                AddTargetStates(targetstate);
            }

            //note here remaining doesn't include the target states and initial states
            HashSet<MDPState> remaining = new HashSet<MDPState>();
            foreach (MDPState mdpState in States.Values)
            {
                if (!TargetStates.Contains(mdpState) && InitState != mdpState)
                {
                    remaining.Add(mdpState);
                }
            }

            //instantiate GCPP
            GCPP gcpp = new GCPP();
            Dictionary<string, int> table = new Dictionary<string, int>();

            foreach (MDPState mdpState in remaining)
            {
                gcpp.MDPTable.Add(mdpState.ID, mdpState);
                table.Add(mdpState.ID, 0);
            }

            int n = 0;
            foreach (MDPState mdpState1 in remaining)
            {
                foreach (MDPState mdpState2 in remaining)
                {
                    List<MDPState> list = new List<MDPState>();
                    if (mdpState1 != mdpState2 && table[mdpState1.ID] == table[mdpState2.ID] && PropositionEquals(mdpState1, mdpState2))
                    {
                        list.Add(mdpState1);
                        list.Add(mdpState2);
                        table[mdpState1.ID] = 1;
                        table[mdpState2.ID] = 1;
                        gcpp.PartOrder.Add(n, n);
                        gcpp.Partition.Add(n, list);
                        gcpp.PartitionGetKey.Add(list, n++);
                    }
                }
            }


            gcpp.partition = null;
            gcpp.partorder = null;

            while (gcpp.Partition != gcpp.partition || gcpp.PartOrder != gcpp.partorder)
            {
                gcpp.partition = gcpp.Partition;
                gcpp.partorder = gcpp.PartOrder;

                foreach (List<MDPState> B in gcpp.partition.Values)
                {
                    //GCPP item = new GCPP();
                    gcpp = Split(gcpp, B);
                    //∑:=∑\{B}∪{∑B}
                    gcpp.Partition.Remove(gcpp.PartitionGetKey[B]);
                    foreach (KeyValuePair<int, List<MDPState>> pair in gcpp.partition)
                    {
                        gcpp.Partition.Add(n, pair.Value);
                        gcpp.PartitionGetKey.Add(pair.Value, n);
                    }
                    //≤:=≤∪≤B
                    //   ∪{(B',X)|X∈∑:B'∈∑B:(B,X)∈≤}
                    //   ∪{(X,B')|X∈∑:B'∈∑B:(X,B)∈≤}
                    //    \{(B,X),(X,B)|X∈∑:(X,B),(B,X)∈≤}
                    foreach (KeyValuePair<int, int> pair in gcpp.partorder)
                    {
                        gcpp.PartOrder.Add(pair.Key, pair.Value);
                    }
                    foreach (List<MDPState> X in gcpp.Partition.Values)
                    {
                        if (SamePartOrder(X, B, gcpp.PartOrder, gcpp.PartitionGetKey) && SamePartOrder(B, X, gcpp.PartOrder, gcpp.PartitionGetKey))
                        {
                            var query1 = from p in gcpp.PartOrder
                                         where p.Key == gcpp.PartitionGetKey[B] && p.Value == gcpp.PartitionGetKey[X]
                                         select p.Key;
                            gcpp.PartOrder.Remove(query1.FirstOrDefault());

                            var query2 = from p in gcpp.PartOrder
                                         where p.Key == gcpp.PartitionGetKey[X] && p.Value == gcpp.PartitionGetKey[B]
                                         select p.Key;
                            gcpp.PartOrder.Remove(query2.FirstOrDefault());
                        }
                        foreach (List<MDPState> B1 in gcpp.partition.Values)
                        {
                            if (SamePartOrder(B, X, gcpp.PartOrder, gcpp.PartitionGetKey))
                            {
                                gcpp.PartOrder.Add(gcpp.PartitionGetKey[B1], gcpp.PartitionGetKey[X]);
                            }
                            if (SamePartOrder(X, B, gcpp.PartOrder, gcpp.PartitionGetKey))
                            {
                                gcpp.PartOrder.Add(gcpp.PartitionGetKey[X], gcpp.PartitionGetKey[B1]);
                            }
                        }
                    }

                }
            }


            return toReturn;
        }

        public bool PropositionEquals(MDPState state1, MDPState state2)
        {
            return true;
        }

        public GCPP Split(GCPP obj, List<MDPState> states)
        {
            Dictionary<int, List<MDPState>> PartitionB = new Dictionary<int, List<MDPState>>();
            Dictionary<List<MDPState>, int> PartitionBGetKey = new Dictionary<List<MDPState>, int>();
            Dictionary<int, int> PartOrderB = new Dictionary<int, int>();
            Dictionary<int, List<MDPState>> partitionB = new Dictionary<int, List<MDPState>>();
            Dictionary<int, int> partorderB = new Dictionary<int, int>();

            int n = 0;
            foreach (MDPState mdpState in states)
            {
                List<MDPState> set = new List<MDPState>();
                set.Add(mdpState);
                PartOrderB.Add(n, n);
                PartitionB.Add(n, set);
                PartitionBGetKey.Add(set, n++);
            }

            partitionB = null;
            partorderB = null;

            while (PartitionB != partitionB || PartOrderB != partorderB)
            {
                partitionB = PartitionB;
                partorderB = PartOrderB;

                foreach (List<MDPState> B1 in partitionB.Values)
                {
                    foreach (List<MDPState> B2 in partitionB.Values)
                    {
                        if (B1 != B2)
                        {
                            if (CanSim(obj, B1[0], B2[0]) && CanSim(obj, B2[0], B1[0]))
                            {
                                //∑B:=∑B\{B1，B2}∪{B1∪B2}
                                PartitionB.Remove(PartitionBGetKey[B1]);
                                PartitionB.Remove(PartitionBGetKey[B1]);

                                List<MDPState> list = new List<MDPState>();
                                foreach (MDPState state in B1)
                                {
                                    list.Add(state);
                                }
                                foreach (MDPState state in B2)
                                {
                                    list.Add(state);
                                }
                                PartitionB.Add(n, list);
                                PartitionBGetKey.Add(list, n++);

                                //≤B:={≤B∪{X,B1∪B2}|X∈∑:(X,B1)∈≤B∨X∈∑:(X,B2)}
                                //   ∪{(B1∪B2,X)|X∈∑:(X,B2)∈≤B∨X∈∑:(X,B1)
                                //    \{(Bi,X),(X,Bi)|X∈∑:(Bi,X),(X,Bi)∈≤B∧i∈{1,2}}
                                foreach (List<MDPState> statelist in obj.partition.Values)
                                {
                                    if (SamePartOrder(statelist, B1, PartOrderB, PartitionBGetKey) || SamePartOrder(statelist, B2, PartOrderB, PartitionBGetKey))
                                    {
                                        PartOrderB.Add(PartitionBGetKey[statelist], PartitionBGetKey[list]);
                                    }
                                    if (SamePartOrder(statelist, B2, PartOrderB, PartitionBGetKey) || SamePartOrder(statelist, B1, PartOrderB, PartitionBGetKey))
                                    {
                                        PartOrderB.Add(PartitionBGetKey[list], PartitionBGetKey[statelist]);
                                    }
                                    if (SamePartOrder(B1, statelist, PartOrderB, PartitionBGetKey) && SamePartOrder(statelist, B1, PartOrderB, PartitionBGetKey))
                                    {
                                        var query1 = from p in PartOrderB
                                                     where p.Key == PartitionBGetKey[B1] && p.Value == PartitionBGetKey[statelist]
                                                     select p.Key;
                                        PartOrderB.Remove(query1.FirstOrDefault());

                                        var query2 = from p in PartOrderB
                                                     where p.Value == PartitionBGetKey[statelist] && p.Key == PartitionBGetKey[B1]
                                                     select p.Key;
                                        PartOrderB.Remove(query2.FirstOrDefault());
                                    }
                                    if (SamePartOrder(B2, statelist, PartOrderB, PartitionBGetKey) && SamePartOrder(statelist, B2, PartOrderB, PartitionBGetKey))
                                    {
                                        var query1 = from p in PartOrderB
                                                     where p.Key == PartitionBGetKey[B2] && p.Value == PartitionBGetKey[statelist]
                                                     select p.Key;
                                        PartOrderB.Remove(query1.FirstOrDefault());

                                        var query2 = from p in PartOrderB
                                                     where p.Value == PartitionBGetKey[statelist] && p.Key == PartitionBGetKey[B2]
                                                     select p.Key;
                                        PartOrderB.Remove(query2.FirstOrDefault());
                                    }
                                }
                            }
                            else if (CanSim(obj, B1[0], B2[0]))
                            {
                                PartOrderB.Add(PartitionBGetKey[B2], PartitionBGetKey[B1]);
                            }
                            else if (CanSim(obj, B2[0], B1[0]))
                            {
                                PartOrderB.Add(PartitionBGetKey[B1], PartitionBGetKey[B2]);
                            }
                        }
                    }
                }
            }

            obj.partition = PartitionB;
            obj.partorder = PartOrderB;
            return obj;
        }

        public bool CanSim(GCPP GCPPobj, MDPState state1, MDPState state2)
        {
            if (!PropositionEquals(state1, state2))
            {
                return false;
            }
            //else if ()
            //{
            //}

            return true;
        }

        public bool SamePartOrder(List<MDPState> list1, List<MDPState> list2, Dictionary<int, int> partorder, Dictionary<List<MDPState>, int> partitionGetKey)
        {
            foreach (KeyValuePair<int, int> pair in partorder)
            {
                if (partitionGetKey[list1] == pair.Key && partitionGetKey[list2] == pair.Value)
                {
                    return true;
                }
            }
            return false;

        }

        public class GCPP
        {
            public Dictionary<string, MDPState> MDPTable;   //record the relation pair of state and its id
            public Dictionary<int, List<MDPState>> Partition;
            public Dictionary<List<MDPState>, int> PartitionGetKey;
            public Dictionary<int, int> PartOrder;
            public Dictionary<int, List<MDPState>> partition;
            public Dictionary<int, int> partorder;

            public GCPP()
            {
                MDPTable = new Dictionary<string, MDPState>();
                Partition = new Dictionary<int, List<MDPState>>();
                PartitionGetKey = new Dictionary<List<MDPState>, int>();
                PartOrder = new Dictionary<int, int>();
                partition = new Dictionary<int, List<MDPState>>();
                partorder = new Dictionary<int, int>();
            }
        }


        #endregion


    }

    public class MDPState
    {
        public string ID;
        public List<Distribution> Distributions;
        public List<MDPState> Pre;
        public double CurrentProb;
        public double CurrentReward;
        public HashSet<MDPState> Sub;
        public bool AntichainUpdate = false;
        public bool TrivialPre = true;
        public bool ReachTarget = false;
        public int SCCIndex = -1;

        public MDPState(string id)
        {
            ID = id;
            Distributions = new List<Distribution>();
            Pre = new List<MDPState>();
            CurrentProb = 0;
            CurrentReward = 0;
            Sub = new HashSet<MDPState>();
        }

        public void AddDistribution(Distribution distribution)
        {
            Distributions.Add(distribution);

            foreach (KeyValuePair<double, MDPState> pair in distribution.States)
            {
                if(pair.Value.Pre.Contains(this)) continue;
                pair.Value.Pre.Add(this);
            }
        }

        public void RemoveDistribution(Distribution distribution)
        {
            Distributions.Remove(distribution);

            foreach (KeyValuePair<double, MDPState> pair in distribution.States)
            {
                pair.Value.Pre.Remove(this);
            }
        }

        public void RemoveDistributionAt(int index)
        {
            foreach (KeyValuePair<double, MDPState> pair in Distributions[index].States)
            {
                pair.Value.Pre.Remove(this);
            }
            Distributions.RemoveAt(index);
        }

        public void RemoveDistributions()
        {
            for(int i = Distributions.Count - 1; i >=0; i--)
            {
                RemoveDistributionAt(i);
            }
        }

        public override String ToString()
        {
            string toReturn = ID + ": ";

            foreach (Distribution distribution in Distributions)
            {
                toReturn += distribution.ToString();
            }

            return toReturn;
        }

        public override int GetHashCode()
        {
            return ID.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj is MDPState)
            {
                return ((MDPState)obj).ID == ID;
            }

            return false;
        }
    }

    public class Distribution
    {
        public string Event;
        public List<KeyValuePair<double, MDPState>> States;
        public bool inMatrix;

        public Distribution(string evt)
        {
            Event = evt;
            States = new List<KeyValuePair<double, MDPState>>();
            inMatrix = false;
        }

        public Distribution(string evt, MDPState target)
        {
            Event = evt;
            States = new List<KeyValuePair<double, MDPState>>();
            States.Add(new KeyValuePair<double, MDPState>(1, target));
            inMatrix = false;
        }

        public bool IsTrivial()
        {
            return States.Count == 1;
        }

        public void AddProbStatePair(double prob, MDPState state)
        {
            States.Add(new KeyValuePair<double, MDPState>(prob, state));
        }

        public override String ToString()
        {
            String toReturn = "-" + Event + "> ";

            foreach (KeyValuePair<double, MDPState> keyValuePair in States)
            {
                toReturn += "[" + keyValuePair.Key + "]" + keyValuePair.Value.ID + "; ";
            }

            return toReturn;
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj is MDPState)
            {
                return ((MDPState)obj).ToString() == ToString();
            }

            return false;
        }
    }
}
