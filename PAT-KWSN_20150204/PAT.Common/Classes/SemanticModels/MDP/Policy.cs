using System;
using System.Collections.Generic;
using System.Linq;

namespace PAT.Common.Classes.SemanticModels.MDP
{

    public class Policy
    {

        private int counter;
        private readonly int totalPolicies = -1;
        public MDPStat mdp;
        public List<KeyValuePair<MDPStateStat, DistributionStat>> adversary;
        public List<MDPStateStat> ListStates;

        public Policy()
        {
            counter = 0;
            adversary = new List<KeyValuePair<MDPStateStat, DistributionStat>>();
        }

        public Policy(MDPStat mdp)
        {
            this.mdp=mdp;
            counter = 0;
           // this.ListStates = getNoneDeadlockStates(mdp);
            ListStates = getStatesNeedPlan(mdp);
            totalPolicies = getTotalPolicies();  
        }

        //private List<MDPStateStat> getNoneDeadlockStates(MDPStat mdp)
        //{
        //    List<MDPStateStat>  States = new List<MDPStateStat>();

        //    foreach (var pair in mdp.States)
        //    {
        //        if (pair.Value.Distributions.Count() != 0) { States.Add(pair.Value);}
        //    }
        //    return States;
        //}

        private List<MDPStateStat> getStatesNeedPlan(MDPStat mdpsat)
        {
            var states = new List<MDPStateStat>(mdpsat.NoneZeroStates);
            foreach (MDPStateStat ss in mdpsat.NoneZeroStates)
            {
                if (ss.Distributions.Count() == 0) { states.Remove(ss); }
            }
            return states;
        }

        

        public void setCounter(int c)
        {
            counter = c;
        }

        public void reSetPlan()
        {
            counter = 0;
        }

        public int getTotalPolicies()
        {
            int ct = 1;
            foreach (MDPStateStat ss in ListStates)
            {       
                int value =ss.Distributions.Count();
                if (value != 0)
                {
                    ct *= value;
                }
            }
            return ct;
        }

        public int[] updatePolicy()
        {
            int[] positions = accessPlan(counter);
            int ct = 0;
            foreach (MDPStateStat ss in ListStates)
            {
                if (ss.Distributions.Count != 0)
                {
                    ss.assignAction(ss.Distributions[positions[ct]]);
                    ct++;
                }
            }
            counter = counter + 1;
            return positions;
        }

        private int[] accessPlan(int number)
        {
           // List<MDPStateStat> States = new List<MDPStateStat>(this.ListState);
            var positions = new int[ListStates.Count()];
            int ct = 0;

            while (number != 0)
            {
                int value = ListStates[ct].Distributions.Count();
                //quotient = Math.DivRem(dividend, divisor, out remainder);
                if (value != 0)
                {
                    number = Math.DivRem(number, value, out positions[ct]);
                    ListStates[ct].assignAction(ListStates[ct].Distributions[positions[ct]]);
                }
                ct++;
            }
            return positions;
        }

        public bool hasPolicy()
        {
            return counter < totalPolicies;
        }

        public bool hasNextPolicy()
        {
            return counter + 1 <= totalPolicies;
        }

        public string reportAdversary(int[] positions)
        {
            string toreturn = null;
            toreturn += "\n*******************policy" + counter + "*******************\n" + "init" ;
            int k = 0;
            foreach (MDPStateStat ss in ListStates)
            {
                int j = positions[k];
            //    toreturn += "->"+ss.Distributions[j].ToString();// +ss.action.Event + "\n";
                toreturn += "->"+ss.ID+":" + ss.action.Event + ss.action.ToString();//  + "\n";
                k++;
            }
            return toreturn;
        }
    }
}
