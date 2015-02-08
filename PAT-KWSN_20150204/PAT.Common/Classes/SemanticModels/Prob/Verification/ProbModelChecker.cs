using System.Diagnostics;
using PAT.Common.Classes.CUDDLib;
using PAT.Common.Classes.SemanticModels.Prob.Encoding;
using PAT.Common.Classes.SemanticModels.Prob.SystemStructure;

namespace PAT.Common.Classes.SemanticModels.Prob.Verification
{
    public class ProbModelChecker: ExpressionToBDD
    {
        public ProbModelChecker(ProbModel model):base(model)
        {
            FilterStates();

            //
            Debug.WriteLine("States: " + CUDD.GetNumMinterms(reach, allRowVars.GetNumVars()) + " (" + CUDD.GetNumMinterms(start, allRowVars.GetNumVars()) + " initial)");
            Debug.WriteLine("Transitions: " + CUDD.GetNumMinterms(trans01, allRowVars.GetNumVars()*2));
            Debug.WriteLine("Transition matrix: " + CUDD.Print.GetInfoString(trans, allRowVars.GetNumVars() * 2) + ", vars: " + allRowVars.GetNumVars() + "r/" + allColVars.GetNumVars() + "c");
        }

        private void FilterStates()
        {
            CUDD.Ref(this.trans);
            trans01 = CUDD.Convert.GreaterThan(this.trans, 0);

            //
            CUDD.Ref(start);
            reach = SuccessorsStart(start, trans01);

            // remove non-reachable states from transition matrix
            CUDD.Ref(reach);
            trans = CUDD.Function.Times(reach, trans);
            CUDD.Ref(reach);
            trans = CUDD.Function.Times(SwapRowColVars(reach), trans);

            CUDD.Ref(trans);
            trans01 = CUDD.Convert.GreaterThan(trans, 0);

            //filter state
            for(int i = 0; i < stateRewards.Count; i++)
            {
                CUDD.Ref(reach);
                stateRewards[i] = CUDD.Function.Times(reach, stateRewards[i]);

                CUDD.Ref(reach);
                transRewards[i] = CUDD.Function.Times(reach, transRewards[i]);

                CUDD.Ref(reach);
                transRewards[i] = CUDD.Function.Times(SwapRowColVars(reach), transRewards[i]);
            }
        }


        /// <summary>
        /// [ REFS: 'result', DEREFS: '' ]
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        public CUDDNode ComputeNextProb(CUDDNode b)
        {
            //Reference to b in the calling place and create a new copy
            CUDD.Ref(b, reach);
            b = CUDD.Function.And(b, reach);

            CUDDNode allProbs = CUDD.Matrix.MatrixMultiplyVector(trans, b, allRowVars, allColVars);

            
            //
            CUDD.Deref(b);
            CUDD.Ref(start);
            return CUDD.Function.Times(allProbs, start);
        }

        /// <summary>
        /// Return P(phi1 Until &lt;= k phi2)
        /// [ REFS: 'result', DEREFS: '' ]
        /// </summary>
        /// <param name="b1"></param>
        /// <param name="b2"></param>
        /// <param name="k"></param>
        /// <returns></returns>
        public CUDDNode ComputeBoundedUntil(CUDDNode b1, CUDDNode b2, int k)
        {
            //Reference old b1, b2 from calling, and create new copies b1, b2
            CUDD.Ref(b1, reach);
            b1 = CUDD.Function.And(b1, reach);

            CUDD.Ref(b2, reach);
            b2 = CUDD.Function.And(b2, reach);

            //
            CUDDNode yes, no, maybe, probs;

            if (b2.Equals(CUDD.ZERO))
            {
                yes = CUDD.Constant(0);

                CUDD.Ref(reach);
                no = reach;

                maybe = CUDD.Constant(0);
            }
            else if (b1.Equals(CUDD.ZERO))
            {
                CUDD.Ref(b2);
                yes = b2;

                CUDD.Ref(reach, b2);
                no = CUDD.Function.And(reach, CUDD.Function.Not(b2));

                maybe = CUDD.Constant(0);
            }
            else
            {
                CUDD.Ref(b2);
                yes = b2;

                if (yes.Equals(reach))
                {
                    no = CUDD.Constant(0);
                }
                else
                {
                    no = ProbAlgo.Prob0(trans01, reach, allRowVars, allColVars, b1, yes);
                }

                CUDD.Ref(reach, yes, no);
                maybe = CUDD.Function.And(reach, CUDD.Function.Not(CUDD.Function.Or(yes, no)));
            }

            // print out yes/no/maybe
            Debug.WriteLine("yes = " + CUDD.GetNumMinterms(yes, allRowVars.GetNumVars()));
            Debug.WriteLine("no = " + CUDD.GetNumMinterms(no, allRowVars.GetNumVars()));
            Debug.WriteLine("maybe = " + CUDD.GetNumMinterms(maybe, allRowVars.GetNumVars()));

            if (maybe.Equals(CUDD.ZERO))
            {
                CUDD.Ref(yes);
                probs = yes;
            }
            else
            {
                probs = ProbAlgo.ProbBoundedUntil(trans, allRowVars, allColVars, yes, maybe, k);
            }

            //
            CUDD.Deref(yes, no, maybe, b1, b2);

             //
            CUDD.Ref(start);
            return CUDD.Function.Times(probs, start);
        }

        /// <summary>
        /// Return P(F phi1)
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        public CUDDNode ComputeFuture(CUDDNode b)
        {
            return ComputeUntil(CUDD.Constant(1), b);
        }


        /// <summary>
        /// Return P(F (&lt;= k) phi1)
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        public CUDDNode ComputeBoundedFuture(CUDDNode b, int k)
        {
            return ComputeBoundedUntil(CUDD.Constant(1), b, k);
        }


        public CUDDNode ComputeUntil(CUDDNode b1, CUDDNode b2)
        {
            //Reference old b1, b2 from calling, and create new copies b1, b2
            CUDD.Ref(b1, reach);
            b1 = CUDD.Function.And(b1, reach);

            CUDD.Ref(b2, reach);
            b2 = CUDD.Function.And(b2, reach);

            //
            CUDDNode yes, no, maybe, probs;

            if (b2.Equals(CUDD.ZERO))
            {
                yes = CUDD.Constant(0);

                CUDD.Ref(reach);
                no = reach;

                maybe = CUDD.Constant(0);
            }
            else if (b1.Equals(CUDD.ZERO))
            {
                CUDD.Ref(b2);
                yes = b2;

                CUDD.Ref(reach, b2);
                no = CUDD.Function.And(reach, CUDD.Function.Not(b2));

                maybe = CUDD.Constant(0);
            }
            else
            {
                no = ProbAlgo.Prob0(trans01, reach, allRowVars, allColVars, b1, b2);

                yes = ProbAlgo.Prob1(trans01, reach, allRowVars, allColVars, b1, b2, no);
                
                CUDD.Ref(reach, yes, no);
                maybe = CUDD.Function.And(reach, CUDD.Function.Not(CUDD.Function.Or(yes, no)));
            }

            // print out yes/no/maybe
            Debug.WriteLine("yes = " + CUDD.GetNumMinterms(yes, allRowVars.GetNumVars()));
            Debug.WriteLine("no = " + CUDD.GetNumMinterms(no, allRowVars.GetNumVars()));
            Debug.WriteLine("maybe = " + CUDD.GetNumMinterms(maybe, allRowVars.GetNumVars()));

            if (maybe.Equals(CUDD.ZERO))
            {
                CUDD.Ref(yes);
                probs = yes;
            }
            else
            {
                probs = ProbAlgo.ProbUntil(trans, reach, allRowVars, allColVars, yes, maybe);
            }

            //
            CUDD.Deref(yes, no, maybe, b1, b2);

             //
            CUDD.Ref(start);
            return CUDD.Function.Times(probs, start);
        }

        public CUDDNode ComputeCumulReward(int rewardStructIndex, int bound)
        {
            CUDDNode result = CUDD.Constant(0);

            if (bound > 0)
            {
                CUDDNode allRewards = ProbAlgo.ProbCumulReward(trans, stateRewards[rewardStructIndex], transRewards[rewardStructIndex], allRowVars, allColVars, bound);

                CUDD.Deref(result);
                CUDD.Ref(start);
                result = CUDD.Function.Times(allRewards, start);
            }

            //
            return result;
        }

        public CUDDNode ComputeInstReward(int rewardStructIndex, int bound)
        {
            CUDD.Ref(stateRewards[rewardStructIndex]);
            CUDDNode stateReward = stateRewards[rewardStructIndex];

            CUDDNode reward;
            if(bound == 0)
            {
                CUDD.Ref(stateReward);
                reward = stateReward;
            }
            else
            {
                reward = ProbAlgo.ProbInstReward(trans, stateReward, allRowVars, allColVars, bound);
            }

            //
            CUDD.Deref(stateReward);

            CUDD.Ref(start);
            return CUDD.Function.Times(reward, start);
        }

        public CUDDNode ComputeReachReward(CUDDNode b, int rewardStructIndex)
        {
            //Reference to b in the calling place and create a new copy
            CUDD.Ref(b, reach);
            b = CUDD.Function.And(b, reach);

            //
            CUDDNode inf, maybe, reward;

            if(b.Equals(CUDD.ZERO))
            {
                CUDD.Ref(reach);
                inf = reach;

                maybe = CUDD.Constant(0);
            }
            else if(b.Equals(reach))
            {
                inf = CUDD.Constant(0);
                maybe = CUDD.Constant(0);
            }
            else
            {
                CUDDNode no = ProbAlgo.Prob0(trans01, reach, allRowVars, allColVars, reach, b);

                CUDDNode prob1 = ProbAlgo.Prob1(trans01, reach, allRowVars, allColVars, reach, b, no);
                CUDD.Deref(no);

                CUDD.Ref(reach);
                inf = CUDD.Function.And(reach, CUDD.Function.Not(prob1));

                CUDD.Ref(reach, inf, b);
                maybe = CUDD.Function.And(reach, CUDD.Function.Not(CUDD.Function.Or(inf, b)));
            }

            // print out yes/no/maybe
            Debug.WriteLine("goal = " + CUDD.GetNumMinterms(b, allRowVars.GetNumVars()));
            Debug.WriteLine("inf = " + CUDD.GetNumMinterms(inf, allRowVars.GetNumVars()));
            Debug.WriteLine("maybe = " + CUDD.GetNumMinterms(maybe, allRowVars.GetNumVars()));

            if(maybe.Equals(CUDD.ZERO))
            {
                CUDD.Ref(inf);
                reward = CUDD.Function.ITE(inf, CUDD.PlusInfinity(), CUDD.Constant(0));
            }
            else
            {
                reward = ProbAlgo.ProbReachReward(trans, stateRewards[rewardStructIndex], transRewards[rewardStructIndex], reach, allRowVars, allColVars, maybe, inf);
            }

            CUDD.Deref(inf, maybe, b);

            CUDD.Ref(start);
            return CUDD.Function.Times(reward, start);
        }

    }
}
