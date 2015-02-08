using System;
using System.Collections.Generic;
using PAT.Common.Classes.Assertion;
using PAT.Common.Classes.DataStructure;
using PAT.Common.Classes.ModuleInterface;
using PAT.Common.Classes.Ultility;

namespace PAT.Common.Classes.SemanticModels.MDP
{
    public class MDPStat
    {
        public Dictionary<string, MDPStateStat> States;
        public MDPStateStat InitState;

        private int Precision;
        private double MAX_DIFFERENCE;
        private List<MDPStateStat> TargetStates;

        private List<MDPStateStat> PminZeroStates;   //GL
        public HashSet<MDPStateStat> NoneZeroStates = null;//GL

        public MDPStat(int precision, double maxdiffer)
        {
            Precision = precision;
            MAX_DIFFERENCE = maxdiffer;
            States = new Dictionary<string, MDPStateStat>(Ultility.Ultility.MC_INITIAL_SIZE);
            TargetStates = new List<MDPStateStat>();
            PminZeroStates = new List<MDPStateStat>();   //GL
        }

        public void SetInit(MDPStateStat init)
        {
            InitState = init;
        }

        public MDPStat(MDPStateStat init, int precision, double maxdiffer)
        {
            Precision = precision;
            MAX_DIFFERENCE = maxdiffer;
            States = new Dictionary<string, MDPStateStat>(Ultility.Ultility.MC_INITIAL_SIZE);
            InitState = init;
            States.Add(init.ID, InitState);
            TargetStates = new List<MDPStateStat>();
            PminZeroStates = new List<MDPStateStat>();   //GL
        }

        public void AddTargetStates(MDPStateStat target)
        {
            TargetStates.Add(target);
            target.CurrentProb = 1;
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

        public void AddState(MDPStateStat state)
        {
            States.Add(state.ID, state);
        }

        public void AddDistribution(string sourceState, DistributionStat distribution)
        {
            MDPStateStat state;

            if (States.TryGetValue(sourceState, out state))
            {
                state.AddDistribution(distribution);
            }
        }

        //GL
        public List<MDPStateStat> getTargetStates()
        {
            return TargetStates;
        }

        //GL
        public void setNonZeroStates()
        {
            NoneZeroStates = this.CollectPminZeroStates();
        }

        //GL
        public HashSet<MDPStateStat> getNonZeroStates()
        {
            return this.NoneZeroStates;
        }

        //GL
        public HashSet<MDPStateStat> CollectPminZeroStates()
        {
            bool done = false;
            HashSet<MDPStateStat> target = new HashSet<MDPStateStat>(TargetStates);
            HashSet<MDPStateStat> reached = new HashSet<MDPStateStat>();
            reached.UnionWith(target);
            HashSet<MDPStateStat> copyTarget = new HashSet<MDPStateStat>(); //updated

            while (done == false)
            {

                HashSet<MDPStateStat> postreached = MDPStateStat.cloneHashSet(reached);
                List<MDPStateStat> preTarget = new List<MDPStateStat>();
                HashSet<MDPStateStat> intsPre = null;

                foreach (MDPStateStat state in reached)  //updated
                {
                    if (copyTarget.Contains(state)) { continue; }  //updated
                    preTarget = state.Pre;
                    if (intsPre == null)
                    {
                        intsPre = new HashSet<MDPStateStat>(preTarget);
                    }
                    else
                    {
                        //intsPre.IntersectWith(preTarget);
                        intsPre.UnionWith(preTarget);
                    }
                }

                // intsPre2 = this.ListStateFromHashset(intsPre);
                if (intsPre == null) { done = true; break; }  //updated
                HashSet<MDPStateStat> enLargeTarget = MDPStateStat.cloneHashSet(intsPre);

                foreach (MDPStateStat st in intsPre)
                {
                    foreach (DistributionStat ds in st.Distributions)
                    {
                        // HashSet<MDPStateStat> nextStates = new HashSet<KeyValuePair> (ds.States);
                        HashSet<MDPStateStat> endStates = new HashSet<MDPStateStat>();
                        endStates = ds.getEndStates();
                        if (!reached.Overlaps(endStates))  //updated
                        {
                            enLargeTarget.Remove(st);
                            goto firstforloop;
                        }
                    }
                firstforloop: ;
                }
                copyTarget = MDPStateStat.cloneHashSet(reached);  //updated
                postreached.UnionWith(enLargeTarget);
                if (reached.SetEquals(postreached)) done = true;
                reached = postreached;
            }
            return reached;
        }

        public override string ToString()
        {
            string toReturn = "";

            foreach (KeyValuePair<string, MDPStateStat> keyValuePair in States)
            {
                toReturn += keyValuePair.Value.ToString() + "\r\n";
            }

            return toReturn;
        }

        public void ResetNonTargetStateProbability()
        {
            foreach (KeyValuePair<string, MDPStateStat> state in States)
            {
                state.Value.CurrentProb = 0;
            }

            foreach (MDPStateStat targetState in TargetStates)
            {
                targetState.CurrentProb = 1;
            }
        }

        public double MaxProbability(VerificationOutput VerificationOutput)
        {
            if (TargetStates.Count == 0)
            {
                return 0;
            }

            if (TargetStates.Contains(InitState))
            {
                return 1;
            }

            //return BuildQuotientMDP(VerificationOutput).MaxProbabilityWork();
            //return BuildQuotientMDPBisimulation(VerificationOutput).MaxProbabilityWork(VerificationOutput);
            return MaxProbabilityWork(VerificationOutput);
        }

        public double MaxProbabilityWork(VerificationOutput VerificationOutput)
        {
            ////calculate the nonsafe states, whose maximal prob is not 0
            //Stack<MDPStateStat> NonSafe = new Stack<MDPStateStat>(TargetStates);
            //Stack<MDPStateStat> helper = new Stack<MDPStateStat>(TargetStates);

            ////backward checking from target states
            //while (helper.Count != 0)
            //{
            //    MDPStateStat t = helper.Pop();
            //    foreach (MDPStateStat s in t.Pre)
            //    {
            //        bool addState = false;

            //        //check each distribution; as long as s has a post state in NonSafe, then s should be added.
            //        foreach (DistributionStat distribution in s.Distributions)
            //        {
            //            foreach (KeyValuePair<double, MDPStateStat> pair in distribution.States)
            //            {
            //                if (NonSafe.Contains(pair.Value))
            //                {
            //                    addState = true;
            //                    //s.Distributions.Remove(distribution);
            //                    break;
            //                }
            //            }

            //            if (addState)
            //            {
            //                break;
            //            }
            //        }

            //        if (addState && !NonSafe.Contains(s))
            //        {
            //            helper.Push(s);
            //            NonSafe.Push(s);
            //        }

            //    }
            //}

            //HashSet<MDPStateStat> nonSafe = new HashSet<MDPStateStat>(NonSafe);

            HashSet<MDPStateStat> working = new HashSet<MDPStateStat>(TargetStates);
            double maxDifference = 1;
            //int counter = 0;

            while (maxDifference > MAX_DIFFERENCE)
            {
                //counter++;
                VerificationOutput.MDPIterationNumber++;

                maxDifference = 0;

                //get the nodes which should be re-calculated.
                HashSet<MDPStateStat> newWorking = new HashSet<MDPStateStat>();
                foreach (MDPStateStat state in working)
                {
                    foreach (MDPStateStat mdpState in state.Pre)
                    {
                        //if (nonSafe.Contains(mdpState)) //note changed here
                        //{
                        newWorking.Add(mdpState);
                        //}
                    }
                }

                List<MDPStateStat> toRemove = new List<MDPStateStat>();
                foreach (MDPStateStat node in newWorking)
                {
                    double newMax = 0;

                    foreach (DistributionStat distribution in node.Distributions)
                    {
                        double result = 0;
                        bool hasNewValues = false;
                        foreach (KeyValuePair<double, MDPStateStat> pair in distribution.States)
                        {
                            result += pair.Value.CurrentProb * pair.Key;
                            hasNewValues = true;
                        }

                        if (hasNewValues)
                        {
                            newMax = Math.Max(newMax, result);
                        }
                    }

                    if (node.CurrentProb < newMax)
                    {
                        maxDifference = Math.Max(maxDifference, (newMax - node.CurrentProb) / node.CurrentProb);///relative difference
                        node.CurrentProb = newMax;
                    }
                    else
                    {
                        toRemove.Add(node);
                    }
                }

                foreach (MDPStateStat i in toRemove)
                {
                    newWorking.Remove(i);
                }

                working = newWorking;
            }
            //System.Console.Write(counter);

            return Ultility.Ultility.RoundProbWithPrecision(InitState.CurrentProb, Precision);
        }


        public double MinProbability(VerificationOutput VerificationOutput)
        {
            if (TargetStates.Count == 0)
            {
                return 0;
            }

            if (TargetStates.Contains(InitState))
            {
                return 1;
            }

            //return BuildQuotientMDP(VerificationOutput).MinProbabilityWork();
            //return BuildQuotientMDPBisimulation(VerificationOutput).MinProbabilityWork(VerificationOutput);
            return MinProbabilityWork(VerificationOutput);
        }


        public double MinProbabilityWork(VerificationOutput VerificationOutput)
        {
            ////calculate the nonsafe states, whose minimal prob is not 0
            //Stack<MDPStateStat> NonSafe = new Stack<MDPStateStat>(TargetStates);
            //Stack<MDPStateStat> helper = new Stack<MDPStateStat>(TargetStates);

            ////backward checking from target states
            //while (helper.Count != 0)
            //{
            //    MDPStateStat t = helper.Pop();
            //    foreach (MDPStateStat s in t.Pre)
            //    {
            //        bool addState = true;

            //        foreach (DistributionStat distribution in s.Distributions)
            //        {
            //            bool keepDistr = false;

            //            //check each distribution; if states in s.distribution are not included in NonSafe, then s should not be added.
            //            foreach (KeyValuePair<double, MDPStateStat> pair in distribution.States)
            //            {

            //                if (NonSafe.Contains(pair.Value))
            //                {
            //                    keepDistr = true;
            //                    //s.Distributions.Remove(distribution);
            //                    break;
            //                }

            //            }

            //            if (!keepDistr)
            //            {
            //                addState = false;
            //                break;
            //            }
            //        }

            //        if (addState && !NonSafe.Contains(s))//add a state to NonSafe
            //        {
            //            helper.Push(s);
            //            NonSafe.Push(s);
            //        }

            //    }
            //}

            //HashSet<MDPStateStat> nonSafe = new HashSet<MDPStateStat>(NonSafe);

            HashSet<MDPStateStat> working = new HashSet<MDPStateStat>(TargetStates);

            double maxDifference = 1;
            while (maxDifference > MAX_DIFFERENCE)
            {
                VerificationOutput.MDPIterationNumber++;

                maxDifference = 0;
                //get the nodes which should be re-calculated.
                HashSet<MDPStateStat> newWorking = new HashSet<MDPStateStat>();
                foreach (MDPStateStat state in working)
                {
                    foreach (MDPStateStat mdpState in state.Pre)
                    {
                        //if (nonSafe.Contains(mdpState)) //note changed here
                        //{
                        newWorking.Add(mdpState);
                        //}
                    }
                }

                List<MDPStateStat> toRemove = new List<MDPStateStat>();

                foreach (MDPStateStat node in newWorking)
                {
                    double newMin = 1;

                    foreach (DistributionStat distribution in node.Distributions)
                    {
                        double result = 0;
                        bool hasNewValues = false;
                        foreach (KeyValuePair<double, MDPStateStat> pair in distribution.States)
                        {
                            result += pair.Value.CurrentProb * pair.Key;
                            hasNewValues = true;
                        }

                        if (hasNewValues)
                        {
                            newMin = Math.Min(newMin, result);
                        }
                    }

                    if (node.CurrentProb < newMin)
                    {
                        maxDifference = Math.Max(maxDifference, (newMin - node.CurrentProb) / node.CurrentProb);///relative difference
                        node.CurrentProb = newMin;
                    }
                    else
                    {
                        toRemove.Add(node);
                    }
                }

                foreach (MDPStateStat i in toRemove)
                {
                    newWorking.Remove(i);
                }

                working = newWorking;
            }

            return Ultility.Ultility.RoundProbWithPrecision(InitState.CurrentProb, Precision);
        }

        public MDPStat BuildQuotientMDPBisimulation(VerificationOutput VerificationOutput)
        {
            MDPStat toReturn = new MDPStat(Precision, MAX_DIFFERENCE);

            //calculate the nonsafe states, whose maximal prob is not 0
            Stack<MDPStateStat> NonSafe = new Stack<MDPStateStat>(TargetStates);
            Stack<MDPStateStat> helper = new Stack<MDPStateStat>(TargetStates);

            //backward checking from target states
            while (helper.Count != 0)
            {
                MDPStateStat t = helper.Pop();
                foreach (MDPStateStat s in t.Pre)
                {
                    bool addState = false;

                    //check each distribution; as long as s has a post state in NonSafe, then s should be added.
                    foreach (DistributionStat distribution in s.Distributions)
                    {
                        foreach (KeyValuePair<double, MDPStateStat> pair in distribution.States)
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
            HashSet<MDPStateStat> remaining = new HashSet<MDPStateStat>();
            foreach (MDPStateStat mdpState in NonSafe)
            {
                if (!TargetStates.Contains(mdpState) && InitState != mdpState)
                {
                    remaining.Add(mdpState);
                }
            }

            //add Initial
            MDPStateStat Initial = new MDPStateStat(InitState.ID);
            toReturn.AddState(Initial);
            toReturn.SetInit(Initial);

            //add target
            MDPStateStat Target = new MDPStateStat("target");
            toReturn.AddState(Target);
            toReturn.AddTargetStates(Target);

            //add safe
            MDPStateStat Safe = new MDPStateStat("safe");
            toReturn.AddState(Safe);

            //add remaining
            //MDPStateStat Remaining = new MDPStateStat("remaining");
            //toReturn.AddState(Remaining);

            //add selfloop at Target.
            //Target.AddDistribution(new DistributionStat(Constants.TAU, Target));

            //add selfloop at Safe.
            //Safe.AddDistribution(new DistributionStat(Constants.TAU, Safe));

            //add the group distributions in Remaining

            List<HashSet<MDPStateStat>> SeperatingRemaining = new List<HashSet<MDPStateStat>>();

            SeperatingRemaining.Add(remaining);

            bool refinement = true;
            while (refinement)
            {
                refinement = false;

                for (int i = 0; i < SeperatingRemaining.Count; i++)
                {
                    HashSet<MDPStateStat> mdpStates = SeperatingRemaining[i];

                    if (mdpStates.Count > 1)
                    {
                        Dictionary<string, HashSet<MDPStateStat>> groups = SeperateGroup(SeperatingRemaining, mdpStates, Initial, Target, Safe);

                        if (groups.Count > 1)
                        {
                            SeperatingRemaining.RemoveAt(i--);

                            foreach (KeyValuePair<string, HashSet<MDPStateStat>> keyValuePair in groups)
                            {
                                SeperatingRemaining.Add(keyValuePair.Value);
                            }

                            refinement = true;
                        }
                    }
                }
                //List<HashSet<MDPStateStat>> NewSeperating = new List<HashSet<MDPStateStat>>();

                //foreach (HashSet<MDPStateStat> mdpStates in SeperatingRemaining)
                //{

                //    if (mdpStates.Count > 1)
                //        {
                //            int counter = 0;

                //            foreach (HashSet<MDPStateStat> grouped in SeperateGroup(SeperatingRemaining, mdpStates, Initial, Target, Safe).Values)
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
            foreach (HashSet<MDPStateStat> mdpStates in SeperatingRemaining)
            {
                MDPStateStat Grouped = StateFromHashset(mdpStates);
                //MDPStateStat Grouped = mdpStates[0];
                toReturn.AddState(new MDPStateStat(Grouped.ID));
                //FinalgroupedDistribution(SeperatingRemaining, mdpStates, Target, Safe, toReturn);
            }

            foreach (HashSet<MDPStateStat> mdpStates in SeperatingRemaining)
            {
                FinalgroupedDistribution(SeperatingRemaining, mdpStates, Target, Safe, toReturn);
            }

            //note: add initial state's distributions after refinement
            FinalgroupedDistribution(SeperatingRemaining, InitState, Target, Safe, toReturn);
            return toReturn;
        }

        public void FinalgroupedDistribution(List<HashSet<MDPStateStat>> AfterRefined, HashSet<MDPStateStat> mdpStates, MDPStateStat Target, MDPStateStat Safe, MDPStat Returned)
        {

            foreach (MDPStateStat State in mdpStates)
            {
                FinalgroupedDistribution(AfterRefined, State, Target, Safe, Returned);
            }
        }

        public void FinalgroupedDistribution(List<HashSet<MDPStateStat>> AfterRefined, MDPStateStat mdpState, MDPStateStat Target, MDPStateStat Safe, MDPStat Returned)
        {
            HashSet<DistributionStat> DistrToAdd = new HashSet<DistributionStat>();

            foreach (DistributionStat Distr in mdpState.Distributions)
            {
                DistributionStat newDistr = calcGroupedDistr(AfterRefined, Distr, Returned.InitState, Target, Safe, Returned);
                DistrToAdd.Add(newDistr);
            }

            foreach (DistributionStat distribution in DistrToAdd)
            {
                Returned.AddDistribution(mdpState.ID, distribution);
            }
        }

        public Dictionary<string, HashSet<MDPStateStat>> SeperateGroup(List<HashSet<MDPStateStat>> seperation, HashSet<MDPStateStat> mdpStates, MDPStateStat Initial, MDPStateStat Target, MDPStateStat Safe)
        {
            Dictionary<string, HashSet<MDPStateStat>> record = new Dictionary<string, HashSet<MDPStateStat>>();

            foreach (MDPStateStat State in mdpStates)
            {
                string dis = "";
                foreach (DistributionStat Distr in State.Distributions)
                {
                    dis += calcGroupedDistr(seperation, Distr, Initial, Target, Safe, this).ToString();
                    //dis += State.ID;
                }

                if (record.Count == 0)
                {
                    HashSet<MDPStateStat> states = new HashSet<MDPStateStat>();
                    states.Add(State);
                    record.Add(dis, states);
                }
                else
                {
                    if (record.ContainsKey(dis))
                    {
                        //todo: too slow here..
                        record[dis].Add(State);
                        continue;
                    }

                    HashSet<MDPStateStat> states = new HashSet<MDPStateStat>();
                    states.Add(State);
                    record.Add(dis, states);
                }
            }

            return record;
        }

        public DistributionStat calcGroupedDistr(List<HashSet<MDPStateStat>> ListHashMDP, DistributionStat Distr, MDPStateStat Initial, MDPStateStat Target, MDPStateStat Safe, MDPStat mdp)
        {
            DistributionStat toReturn = new DistributionStat(Distr.Event);

            double toInit = 0;
            double toTarget = 0;
            double[] combined = new double[ListHashMDP.Count];
            //MDPStateStat[] groupstates = new MDPStateStat[seperation.Count];

            foreach (KeyValuePair<double, MDPStateStat> transition in Distr.States)
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
                    MDPStateStat state = mdp.States[StateFromHashset(ListHashMDP[j]).ID];
                    toReturn.AddProbStatePair(Math.Round(combined[j], 5), state);
                    //toReturn.AddProbStatePair(Math.Round(combined[j], 5), StateFromHashset(ListHashMDP[j]));
                }

            }

            return toReturn;
        }

        public MDPStateStat StateFromHashset(HashSet<MDPStateStat> states)
        {
            if (states.Count != 0)
            {
                List<MDPStateStat> States = new List<MDPStateStat>(states);
                return States[0];
            }

            return null;
        }

        public MDPStat BuildQuotientMDP(VerificationOutput VerificationOutput)
        {
            //return this;

            MDPStat toReturn = new MDPStat(Precision, MAX_DIFFERENCE);

            //todo change to set
            List<KeyValuePair<string, string>> BoundaryOneTransition = new List<KeyValuePair<string, string>>();

            //todo change to set
            List<DistributionStat> ProbTransitions = new List<DistributionStat>();

            Dictionary<string, List<DistributionStat>> GlobalProbTransitions = new Dictionary<string, List<DistributionStat>>();

            StringDictionary<bool> visited = new StringDictionary<bool>(States.Count);
            List<KeyValuePair<HashSet<string>, MDPStateStat>> sccs = new List<KeyValuePair<HashSet<string>, MDPStateStat>>();

            Dictionary<string, int> preorder = new Dictionary<string, int>();
            Dictionary<string, int> lowlink = new Dictionary<string, int>();
            //HashSet<string> scc_found = new HashSet<string>();
            Stack<MDPStateStat> TaskStack = new Stack<MDPStateStat>();

            //Dictionary<string, List<string>> OutgoingTransitionTable = new Dictionary<string, List<string>>();
            Stack<MDPStateStat> stepStack = new Stack<MDPStateStat>(1024);

            visited.Add(InitState.ID, false);
            TaskStack.Push(InitState);

            //# Preorder counter 
            int preor = 0;

            do
            {
                while (TaskStack.Count > 0)
                {
                    MDPStateStat pair = TaskStack.Peek();
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

                    List<DistributionStat> list = pair.Distributions;
                    List<MDPStateStat> nonProbTrans = new List<MDPStateStat>();
                    List<DistributionStat> ProbTrans = new List<DistributionStat>();

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
                        MDPStateStat step = nonProbTrans[k];
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

                            MDPStateStat newstate = new MDPStateStat(toReturn.States.Count.ToString());
                            if (scc.Count > 1 || (scc.Count == 1 && selfLoop))
                            {
                                newstate.AddDistribution(new DistributionStat(Constants.TAU, newstate)); //add self loop: sun jun                                
                            }
                            sccs.Add(new KeyValuePair<HashSet<string>, MDPStateStat>(scc, newstate));

                            toReturn.AddState(newstate);

                            if (scc.Contains(InitState.ID))
                            {
                                toReturn.SetInit(newstate);
                            }

                            foreach (MDPStateStat state in TargetStates)
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
                    foreach (DistributionStat step in ProbTransitions)
                    {
                        foreach (KeyValuePair<double, MDPStateStat> pair in step.States)
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
                MDPStateStat source = null;
                MDPStateStat target = null;

                foreach (KeyValuePair<HashSet<string>, MDPStateStat> sccstate in sccs)
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

                toReturn.AddDistribution(source.ID, new DistributionStat(Constants.TAU, target));
                VerificationOutput.ReducedMDPTransitions++;

            }

            foreach (KeyValuePair<string, List<DistributionStat>> pair in GlobalProbTransitions)
            {
                MDPStateStat source = null;

                foreach (KeyValuePair<HashSet<string>, MDPStateStat> sccstate in sccs)
                {
                    if (sccstate.Key.Contains(pair.Key))
                    {
                        source = sccstate.Value;
                        break;
                    }
                }

                foreach (DistributionStat distribution in pair.Value)
                {
                    DistributionStat disNew = new DistributionStat(distribution.Event);
                    foreach (KeyValuePair<double, MDPStateStat> state in distribution.States)
                    {
                        foreach (KeyValuePair<HashSet<string>, MDPStateStat> sccstate in sccs)
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
    }

    //public class MyDictionary : Dictionary<object , object >
    //{
    //    public override int GetHashCode()
    //    {
    //        base.
    //        return base.GetHashCode();
    //    }
    //}

    public class MDPStateStat
    {
        public string ID;
        public List<DistributionStat> Distributions;
        public List<MDPStateStat> Pre;
        public double CurrentProb;
        public DistributionStat action;  //GL

        public MDPStateStat(string id)
        {
            ID = id;
            Distributions = new List<DistributionStat>();
            Pre = new List<MDPStateStat>();
            CurrentProb = 0;
        }

        public void AddDistribution(DistributionStat distribution)
        {
            Distributions.Add(distribution);

            foreach (KeyValuePair<double, MDPStateStat> pair in distribution.States)
            {
                pair.Value.Pre.Add(this);
            }
        }

        //GL
        public void assignAction(DistributionStat ds)  //GL
        {
            this.action = ds;
            ds.taken = true;
        }

        //GL
        public static HashSet<MDPStateStat> cloneHashSet(HashSet<MDPStateStat> states)
        {
            HashSet<MDPStateStat> pp = new HashSet<MDPStateStat>();
            foreach (MDPStateStat ss in states)
            {
                pp.Add(ss);
            }
            return pp;
        }

        public override String ToString()
        {
            string toReturn = ID + ": ";

            foreach (DistributionStat distribution in Distributions)
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
            if (obj is MDPStateStat)
            {
                return ((MDPStateStat)obj).ID == ID;
            }

            return false;
        }
    }

    public class DistributionStat   //GL: structs is changed to class
    {
        public string Event;
        public List<KeyValuePair<double, MDPStateStat>> States;
        public bool taken;//GL

        public DistributionStat(string evt)
        {
            Event = evt;
            States = new List<KeyValuePair<double, MDPStateStat>>();
            taken = false;//GL
        }

        public DistributionStat(string evt, MDPStateStat target)
        {
            Event = evt;
            States = new List<KeyValuePair<double, MDPStateStat>>();
            States.Add(new KeyValuePair<double, MDPStateStat>(1, target));
            taken = false; // GL
        }

        public bool IsTrivial()
        {
            return States.Count == 1;
        }

        //GL
        public void setTaken(bool bl)
        {
            taken = bl;
        }

        //GL
        public HashSet<MDPStateStat> getEndStates()
        {
            HashSet<MDPStateStat> endStates = new HashSet<MDPStateStat>();
            foreach (KeyValuePair<double, MDPStateStat> pair in States)
            {
                endStates.Add(pair.Value);
            }
            return endStates;
        }

        public void AddProbStatePair(double prob, MDPStateStat state)
        {
            States.Add(new KeyValuePair<double, MDPStateStat>(prob, state));
        }

        public override String ToString()
        {
            String toReturn = "-" + Event + "> ";

            foreach (KeyValuePair<double, MDPStateStat> keyValuePair in States)
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
            if (obj is MDPStateStat)
            {
                return ((MDPStateStat)obj).ToString() == ToString();
            }

            return false;
        }
    }
}
