using System.Diagnostics;
using PAT.Common.Classes.CUDDLib;
using PAT.Common.Classes.SemanticModels.Prob.Encoding;
using PAT.Common.Classes.SemanticModels.Prob.SystemStructure;

namespace PAT.Common.Classes.SemanticModels.Prob.Verification
{
    public class NondetModelChecker: ExpressionToBDD
    {
        // Extra (MDP) model info
        protected CUDDNode nondetMask;
        protected CUDDVars allNondetVars;

        public NondetModelChecker(NonDetModel model)
            : base(model)
        {
            allNondetVars = model.allNondetVars;

            FilterStates();

            CUDD.Ref(trans01, reach);
            nondetMask = CUDD.Function.And(CUDD.Function.Not(CUDD.Abstract.ThereExists(trans01, allColVars)), reach);

            Debug.WriteLine("States: " + CUDD.GetNumMinterms(reach, allRowVars.GetNumVars()) + " (" + CUDD.GetNumMinterms(start, allRowVars.GetNumVars()) + " initial)");
            Debug.WriteLine("Transitions: " + CUDD.GetNumMinterms(trans01, allRowVars.GetNumVars()*2 + allNondetVars.GetNumVars()));
            Debug.WriteLine("Transition matrix: " + CUDD.Print.GetInfoString(trans, allRowVars.GetNumVars() * 2 + allNondetVars.GetNumVars()) +
                            ", vars: " + allRowVars.GetNumVars() + "r/" + allColVars.GetNumVars() + "c/" + allNondetVars.GetNumVars() + "nd");
        }

        private void FilterStates()
        {
            CUDD.Ref(this.trans);
            this.trans01 = CUDD.Convert.GreaterThan(this.trans, 0);

            //
            // remove nondeterminism var from trans01
            CUDD.Ref(trans01);
            CUDDNode trans01WithoutNondetVars = CUDD.Abstract.ThereExists(trans01, allNondetVars);

            CUDD.Ref(start);
            reach = SuccessorsStart(start, trans01WithoutNondetVars);
            CUDD.Deref(trans01WithoutNondetVars);

            // remove non-reachable states from transition matrix
            CUDD.Ref(reach);
            trans = CUDD.Function.Times(reach, trans);
            CUDD.Ref(reach);
            trans = CUDD.Function.Times(SwapRowColVars(reach), trans);

            CUDD.Ref(trans);
            trans01 = CUDD.Convert.GreaterThan(trans, 0);

            //filter state
            for (int i = 0; i < stateRewards.Count; i++)
            {
                CUDD.Ref(reach);
                stateRewards[i] = CUDD.Function.Times(reach, stateRewards[i]);

                CUDD.Ref(reach);
                transRewards[i] = CUDD.Function.Times(reach, transRewards[i]);

                CUDD.Ref(reach);
                transRewards[i] = CUDD.Function.Times(SwapRowColVars(reach), transRewards[i]);
            }
        }

        public override void Close()
        {
            CUDD.Deref(nondetMask);
            allNondetVars.Deref();

            base.Close();
        }

        /// <summary>
        /// [ REFS: 'result', DEREFS: '' ]
        /// </summary>
        /// <param name="b"></param>
        /// <param name="min"></param>
        /// <returns></returns>
        public CUDDNode ComputeNextProb(CUDDNode b, bool min)
        {
            //Reference to b in the calling place and create a new copy
            CUDD.Ref(b, reach);
            b = CUDD.Function.And(b, reach);


            CUDDNode probs = CUDD.Matrix.MatrixMultiplyVector(trans, b, allRowVars, allColVars);
            if (min)
            {
                //at a state, suppose we have 2 choices, but actually in total we need 2 bit for 3 choices. 
                //therefore the other choice is with prob 0
                //we will promote it to 1 via nondetMask then MinAbstract will work
                CUDD.Ref(nondetMask);
                probs = CUDD.Function.Maximum(probs, nondetMask);

                probs = CUDD.Abstract.MinAbstract(probs, allNondetVars);
            }
            else
            {
                probs = CUDD.Abstract.MaxAbstract(probs, allNondetVars);
            }

            CUDD.Deref(b);
            //
            CUDD.Ref(start);
            return CUDD.Function.Times(probs, start);
        }

        /// <summary>
        /// Return P(phi1 Until &lt;= k phi2)
        /// [ REFS: 'result', DEREFS: '' ]
        /// </summary>
        /// <param name="b1"></param>
        /// <param name="b2"></param>
        /// <param name="bound"></param>
        /// <returns></returns>
        public CUDDNode ComputeBoundedUntil(CUDDNode b1, CUDDNode b2, int bound, bool min)
        {
            //Reference old b1, b2 from calling, and create new copies b1, b2
            CUDD.Ref(b1, reach);
            b1 = CUDD.Function.And(b1, reach);

            CUDD.Ref(b2, reach);
            b2 = CUDD.Function.And(b2, reach);

            CUDDNode yes, no, maybe;
            CUDDNode probs;

            if(b2.Equals(CUDD.ZERO))
            {
                yes = CUDD.Constant(0);

                CUDD.Ref(reach);
                no = reach;

                maybe = CUDD.Constant(0);
            }
            else if(b1.Equals(CUDD.ZERO))
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

                if(yes.Equals(reach))
                {
                    no = CUDD.Constant(0);
                }
                else
                {
                    if (min)
                    {
                        no = NondetAlgo.Prob0E(trans01, reach, nondetMask, allRowVars, allColVars, allNondetVars, b1, b2);
                    }
                    else
                    {
                        no = NondetAlgo.Prob0A(trans01, reach, allRowVars, allColVars, allNondetVars, b1, b2);
                    }
                }

                CUDD.Ref(reach, yes, no);
                maybe = CUDD.Function.And(reach, CUDD.Function.Not(CUDD.Function.Or(yes, no)));
            }

            // print out yes/no/maybe
            Debug.WriteLine("yes = " + CUDD.GetNumMinterms(yes, allRowVars.GetNumVars()));
            Debug.WriteLine("no = " + CUDD.GetNumMinterms(no, allRowVars.GetNumVars()));
            Debug.WriteLine("maybe = " + CUDD.GetNumMinterms(maybe, allRowVars.GetNumVars()));

            if(maybe.Equals(CUDD.ZERO))
            {
                CUDD.Ref(yes);
                probs = yes;
            }
            else
            {
                probs = NondetAlgo.NondetBoundedUntil(trans, nondetMask, allRowVars, allColVars, allNondetVars, yes, maybe, bound, min);
            }

            //
            CUDD.Deref(yes, no, maybe, b1, b2);

            //
            CUDD.Ref(start);
            return CUDD.Function.Times(probs, start);

        }

    
        /// <summary>
        /// 
        /// </summary>
        /// <param name="b1"></param>
        /// <param name="b2"></param>
        /// <param name="min"></param>
        /// <returns></returns>
        public CUDDNode ComputeUntil(CUDDNode b1, CUDDNode b2, bool min)
        {
            //Reference old b1, b2 from calling, and create new copies b1, b2
            CUDD.Ref(b1, reach);
            b1 = CUDD.Function.And(b1, reach);

            CUDD.Ref(b2, reach);
            b2 = CUDD.Function.And(b2, reach);

            CUDDNode yes, no, maybe;
            CUDDNode probs;

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

                if (min)
                {
                    no = NondetAlgo.Prob0E(trans01, reach, nondetMask, allRowVars, allColVars, allNondetVars, b1, b2);
                    yes = NondetAlgo.Prob1A(trans01, reach, nondetMask, allRowVars, allColVars, allNondetVars, no, b2);
                }
                else
                {
                    no = NondetAlgo.Prob0A(trans01, reach, allRowVars, allColVars, allNondetVars, b1, b2);
                    yes = NondetAlgo.Prob1E(trans01, reach, allRowVars, allColVars, allNondetVars, b1, b2, no);
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
                probs = NondetAlgo.NondetUntil(trans, nondetMask, allRowVars, allColVars, allNondetVars, yes, maybe, min);
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
        public CUDDNode ComputeFuture(CUDDNode b, bool min)
        {
            return ComputeUntil(CUDD.Constant(1), b, min);
        }


        /// <summary>
        /// Return P(F (&lt;= k) phi1)
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        public CUDDNode ComputeBoundedFuture(CUDDNode b, int k, bool min)
        {
            return ComputeBoundedUntil(CUDD.Constant(1), b, k, min);
        }


        public CUDDNode ComputeInstReward(int rewardStructIndex, int bound, bool min)
        {
            CUDD.Ref(stateRewards[rewardStructIndex]);
            CUDDNode stateReward = stateRewards[rewardStructIndex];

            CUDDNode reward;
            if (bound == 0)
            {
                CUDD.Ref(stateReward);
                reward = stateReward;
            }
            else
            {
                reward = NondetAlgo.NondetInstReward(trans, stateReward, nondetMask, allRowVars, allColVars, allNondetVars, bound, min, start);
            }

            //
            CUDD.Deref(stateReward);

            //
            CUDD.Ref(start);
            return CUDD.Function.Times(reward, start);
        }

        public CUDDNode ComputeReachReward(CUDDNode b, int rewardStructIndex, bool min)
        {
            //Reference to b in the calling place and create a new copy
            CUDD.Ref(b, reach);
            b = CUDD.Function.And(b, reach);

            CUDDNode inf, maybe, prob1, no, reward;

            if (b.Equals(CUDD.ZERO))
            {
                CUDD.Ref(reach);
                inf = reach;

                maybe = CUDD.Constant(0);
            }
            else if (b.Equals(reach))
            {
                inf = CUDD.Constant(0);
                maybe = CUDD.Constant(0);
            }
            else
            {
                if (!min)
                {
                    no = NondetAlgo.Prob0E(trans01, reach, nondetMask, allRowVars, allColVars, allNondetVars, reach, b);

                    prob1 = NondetAlgo.Prob1A(trans01, reach, nondetMask, allRowVars, allColVars, allNondetVars, no, b);

                    CUDD.Deref(no);
                    CUDD.Ref(reach);
                    inf = CUDD.Function.And(reach, CUDD.Function.Not(prob1));
                }
                else
                {
                    no = NondetAlgo.Prob0A(trans01, reach, allRowVars, allColVars, allNondetVars, reach, b);

                    prob1 = NondetAlgo.Prob1E(trans01, reach, allRowVars, allColVars, allNondetVars, reach, b, no);

                    CUDD.Deref(no);
                    CUDD.Ref(reach);
                    inf = CUDD.Function.And(reach, CUDD.Function.Not(prob1));
                }

                CUDD.Ref(reach, inf, b);
                maybe = CUDD.Function.And(reach, CUDD.Function.Not(CUDD.Function.Or(inf, b)));
            }

            // print out yes/no/maybe
            Debug.WriteLine("goal = " + CUDD.GetNumMinterms(b, allRowVars.GetNumVars()));
            Debug.WriteLine("inf = " + CUDD.GetNumMinterms(inf, allRowVars.GetNumVars()));
            Debug.WriteLine("maybe = " + CUDD.GetNumMinterms(maybe, allRowVars.GetNumVars()));

            if (maybe.Equals(CUDD.ZERO))
            {
                CUDD.Ref(inf);
                reward = CUDD.Function.ITE(inf, CUDD.PlusInfinity(), CUDD.Constant(0));
            }
            else
            {
                reward = NondetAlgo.NondetReachReward(trans, reach, stateRewards[rewardStructIndex], transRewards[rewardStructIndex], nondetMask, allRowVars, allColVars,
                                                      allNondetVars, b, inf, maybe, min);
            }

            CUDD.Deref(inf, maybe, b);

             //
            CUDD.Ref(start);
            return CUDD.Function.Times(reward, start);
        }
    }
}
