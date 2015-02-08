using System;
using System.Diagnostics;
using PAT.Common.Classes.CUDDLib;

namespace PAT.Common.Classes.SemanticModels.Prob
{
    public class NondetAlgo
    {
        /// <summary>
        /// Return pmin(b1 U b2) = 0
        /// </summary>
        /// <param name="trans01"></param>
        /// <param name="reach"></param>
        /// <param name="nondetMask"></param>
        /// <param name="allRowVars"></param>
        /// <param name="allColVars"></param>
        /// <param name="nondetVars"></param>
        /// <param name="b1"></param>
        /// <param name="b2"></param>
        /// <returns></returns>
        public static CUDDNode Prob0E(CUDDNode trans01, CUDDNode reach, CUDDNode nondetMask, CUDDVars allRowVars, CUDDVars allColVars, CUDDVars nondetVars, CUDDNode b1, CUDDNode b2)
        {
            DateTime startTime = DateTime.Now;
            int numberOfIterations = 0;

            CUDD.Ref(b2);
            CUDDNode sol = b2;

            while (true)
            {
                numberOfIterations++;

                CUDD.Ref(sol, trans01);
                CUDDNode tmp = CUDD.Function.And(CUDD.Variable.SwapVariables(sol, allRowVars, allColVars), trans01);

                tmp = CUDD.Abstract.ThereExists(tmp, allColVars);

                CUDD.Ref(nondetMask);
                tmp = CUDD.Function.Or(tmp, nondetMask);

                tmp = CUDD.Abstract.ForAll(tmp, nondetVars);

                CUDD.Ref(b1);
                tmp = CUDD.Function.And(b1, tmp);

                CUDD.Ref(b2);
                tmp = CUDD.Function.Or(b2, tmp);

                if (tmp.Equals(sol))
                {
                    CUDD.Deref(tmp);
                    break;
                }
                CUDD.Deref(sol);
                sol = tmp;
            }


            CUDD.Ref(reach);
            sol = CUDD.Function.And(reach, CUDD.Function.Not(sol));

            DateTime endTime = DateTime.Now;
            double runningTime = (endTime - startTime).TotalSeconds;
            Debug.WriteLine("Prob0E: " + numberOfIterations + " iterations in " + runningTime + " seconds");

            return sol;
        }

        /// <summary>
        /// Return Pmax(b1 U b2) = 0
        /// </summary>
        /// <param name="trans01"></param>
        /// <param name="reach"></param>
        /// <param name="allRowVars"></param>
        /// <param name="allColVars"></param>
        /// <param name="nondetvars"></param>
        /// <param name="b1"></param>
        /// <param name="b2"></param>
        /// <returns></returns>
        public static CUDDNode Prob0A(CUDDNode trans01, CUDDNode reach, CUDDVars allRowVars, CUDDVars allColVars, CUDDVars nondetvars, CUDDNode b1, CUDDNode b2)
        {
            DateTime startTime = DateTime.Now;
            int numberOfIterations = 0;

            CUDD.Ref(b2);
            CUDDNode sol = b2;

            while (true)
            {
                numberOfIterations++;

                CUDD.Ref(sol, trans01);
                CUDDNode tmp = CUDD.Function.And(CUDD.Variable.SwapVariables(sol, allRowVars, allColVars), trans01);

                tmp = CUDD.Abstract.ThereExists(tmp, allColVars);

                tmp = CUDD.Abstract.ThereExists(tmp, nondetvars);

                CUDD.Ref(b1);
                tmp = CUDD.Function.And(b1, tmp);

                CUDD.Ref(b2);
                tmp = CUDD.Function.Or(b2, tmp);

                if (tmp.Equals(sol))
                {
                    CUDD.Deref(tmp);
                    break;
                }

                CUDD.Deref(sol);
                sol = tmp;
            }


            CUDD.Ref(reach);
            sol = CUDD.Function.And(reach, CUDD.Function.Not(sol));

            DateTime endTime = DateTime.Now;
            double runningTime = (endTime - startTime).TotalSeconds;
            Debug.WriteLine("Prob0A: " + numberOfIterations + " iterations in " + runningTime + " seconds");

            return sol;
        }

        /// <summary>
        /// return Pmax(b1 U b2) = 1
        /// </summary>
        /// <param name="trans01"></param>
        /// <param name="reach"></param>
        /// <param name="allRowVars"></param>
        /// <param name="allColVars"></param>
        /// <param name="nondetVars"></param>
        /// <param name="b1"></param>
        /// <param name="b2"></param>
        /// <param name="no"></param>
        /// <returns></returns>
        public static CUDDNode Prob1E(CUDDNode trans01, CUDDNode reach, CUDDVars allRowVars, CUDDVars allColVars, CUDDVars nondetVars, CUDDNode b1, CUDDNode b2, CUDDNode no)
        {
            DateTime startTime = DateTime.Now;
            int numberOfIterations = 0;

            CUDD.Ref(reach, no);
            CUDDNode u = CUDD.Function.And(reach, CUDD.Function.Not(no));

            bool uDone = false;
            while (!uDone)
            {

                CUDDNode v = CUDD.Constant(0);
                bool vDone = false;

                while (!vDone)
                {
                    numberOfIterations++;

                    CUDD.Ref(u);
                    CUDDNode tmp = CUDD.Variable.SwapVariables(u, allRowVars, allColVars);

                    CUDD.Ref(trans01);
                    tmp = CUDD.Abstract.ForAll(CUDD.Function.Implies(trans01, tmp), allColVars);

                    CUDD.Ref(v);
                    CUDDNode tmp2 = CUDD.Variable.SwapVariables(v, allRowVars, allColVars);

                    CUDD.Ref(trans01);
                    tmp2 = CUDD.Abstract.ThereExists(CUDD.Function.And(tmp2, trans01), allColVars);

                    tmp = CUDD.Function.And(tmp, tmp2);
                    tmp = CUDD.Abstract.ThereExists(tmp, nondetVars);

                    CUDD.Ref(b1);
                    tmp = CUDD.Function.And(b1, tmp);

                    CUDD.Ref(b2);
                    tmp = CUDD.Function.Or(b2, tmp);

                    if (tmp.Equals(v))
                    {
                        vDone = true;
                    }

                    CUDD.Deref(v);
                    v = tmp;
                }

                if (v == u)
                {
                    uDone = true;
                }

                CUDD.Deref(u);
                u = v;
            }

            DateTime endTime = DateTime.Now;
            double runningTime = (endTime - startTime).TotalSeconds;
            Debug.WriteLine("Prob1E: " + numberOfIterations + " iterations in " + runningTime + " seconds");

            return u;
        }

        /// <summary>
        /// Return Pmin(phi1 U phi2) = 1
        /// </summary>
        /// <param name="trans01"></param>
        /// <param name="reach"></param>
        /// <param name="nondetMask"></param>
        /// <param name="allRowVars"></param>
        /// <param name="allColVars"></param>
        /// <param name="nondetVar"></param>
        /// <param name="no"></param>
        /// <param name="b2"></param>
        /// <returns></returns>
        public static CUDDNode Prob1A(CUDDNode trans01, CUDDNode reach, CUDDNode nondetMask, CUDDVars allRowVars, CUDDVars allColVars, CUDDVars nondetVar, CUDDNode no, CUDDNode b2)
        {
            DateTime startTime = DateTime.Now;
            int numberOfIterations = 0;

            CUDD.Ref(reach, no);
            CUDDNode notNo = CUDD.Function.And(reach, CUDD.Function.Not(no));

            CUDD.Ref(b2, notNo);
            CUDDNode sol = CUDD.Function.Or(b2, notNo);

            while (true)
            {
                numberOfIterations++;

                CUDD.Ref(sol);
                CUDDNode tmp = CUDD.Variable.SwapVariables(sol, allRowVars, allColVars);

                CUDD.Ref(trans01);
                tmp = CUDD.Abstract.ForAll(CUDD.Function.Implies(trans01, tmp), allColVars);

                CUDD.Ref(nondetMask);
                tmp = CUDD.Function.Or(tmp, nondetMask);
                tmp = CUDD.Abstract.ForAll(tmp, nondetVar);

                CUDD.Ref(notNo);
                tmp = CUDD.Function.And(notNo, tmp);

                CUDD.Ref(b2);
                tmp = CUDD.Function.Or(b2, tmp);

                if (tmp.Equals(sol))
                {
                    CUDD.Deref(tmp);
                    break;
                }
                CUDD.Deref(sol);
                sol = tmp;
            }

            DateTime endTime = DateTime.Now;
            double runningTime = (endTime - startTime).TotalSeconds;
            Debug.WriteLine("Prob1A: " + numberOfIterations + " iterations in " + runningTime + " seconds");

            //
            CUDD.Deref(notNo);
            return sol;
        }

        public static CUDDNode NondetBoundedUntil(CUDDNode trans, CUDDNode nondetMask, CUDDVars allRowVars, CUDDVars allColVars, CUDDVars nondetVars, CUDDNode yes, CUDDNode maybe, int bound, bool min)
        {
            DateTime startTime = DateTime.Now;

            CUDDNode a, sol, tmp;

            CUDD.Ref(trans, maybe);
            a = CUDD.Function.Times(trans, maybe);

            CUDD.Ref(yes);
            sol = yes;

            for (int i = 0; i < bound; i++)
            {
                tmp = CUDD.Matrix.MatrixMultiplyVector(a, sol, allRowVars, allColVars);

                if (min)
                {
                    CUDD.Ref(nondetMask);
                    tmp = CUDD.Function.Maximum(tmp, nondetMask);

                    tmp = CUDD.Abstract.MinAbstract(tmp, nondetVars);
                }
                else
                {
                    tmp = CUDD.Abstract.MaxAbstract(tmp, nondetVars);
                }

                CUDD.Ref(yes);
                tmp = CUDD.Function.Maximum(tmp, yes);

                CUDD.Deref(sol);
                sol = tmp;
            }

            DateTime endTime = DateTime.Now;
            double runningTime = (endTime - startTime).TotalSeconds;
            Debug.WriteLine("NondetBoundedUntil: " + bound + " iterations in " + runningTime + " seconds");

            //
            CUDD.Deref(a);

            return sol;
        }

        public static CUDDNode NondetUntil(CUDDNode trans, CUDDNode nondetMask, CUDDVars allRowVars, CUDDVars allColVars, CUDDVars nondetVars, CUDDNode yes, CUDDNode maybe, bool min)
        {
            DateTime startTime = DateTime.Now;
            int numberOfIterations = 0;

            CUDDNode a, sol, tmp;

            CUDD.Ref(trans, maybe);
            a = CUDD.Function.Times(trans, maybe);

            CUDD.Ref(yes);
            sol = yes;

            while (true)
            {
                numberOfIterations++;

                tmp = CUDD.Matrix.MatrixMultiplyVector(a, sol, allRowVars, allColVars);

                if (min)
                {
                    CUDD.Ref(nondetMask);
                    tmp = CUDD.Function.Maximum(tmp, nondetMask);

                    tmp = CUDD.Abstract.MinAbstract(tmp, nondetVars);
                }
                else
                {
                    tmp = CUDD.Abstract.MaxAbstract(tmp, nondetVars);
                }

                CUDD.Ref(yes);
                tmp = CUDD.Function.Maximum(tmp, yes);

                //check convergence
                if(CUDD.IsEqual(tmp, sol))
                {
                    CUDD.Deref(tmp);
                    break;
                }

                CUDD.Deref(sol);
                sol = tmp;
            }

            DateTime endTime = DateTime.Now;
            double runningTime = (endTime - startTime).TotalSeconds;
            Debug.WriteLine("NondetUntil: " + numberOfIterations + " iterations in " + runningTime + " seconds");

            //
            CUDD.Deref(a);

            return sol;
        }

        public static CUDDNode NondetInstReward(CUDDNode trans, CUDDNode stateReward, CUDDNode nondetMask, CUDDVars allRowVars, CUDDVars allColVars, CUDDVars nondetVars, int bound, bool min, CUDDNode init)
        {
            DateTime startTime = DateTime.Now;

            CUDDNode newNondetMask, sol, tmp;

            CUDD.Ref(nondetMask);
            newNondetMask = CUDD.Function.ITE(nondetMask, CUDD.PlusInfinity(), CUDD.Constant(0));

            CUDD.Ref(stateReward);
            sol = stateReward;

            for(int i = 0; i < bound; i++)
            {
                tmp = CUDD.Matrix.MatrixMultiplyVector(trans, sol, allRowVars, allColVars);

                if(min)
                {
                    CUDD.Ref(newNondetMask);
                    tmp = CUDD.Function.Maximum(tmp, newNondetMask);

                    tmp = CUDD.Abstract.MinAbstract(tmp, nondetVars);
                }
                else
                {
                    tmp = CUDD.Abstract.MaxAbstract(tmp, nondetVars);
                }

                //
                CUDD.Deref(sol);
                sol = tmp;
            }
            
            DateTime endTime = DateTime.Now;
            double runningTime = (endTime - startTime).TotalSeconds;
            Debug.WriteLine("NondetInstReward: " + bound + " iterations in " + runningTime + " seconds");

            //
            CUDD.Deref(newNondetMask);
            return sol;
        }

        public static CUDDNode NondetReachReward(CUDDNode trans, CUDDNode reach, CUDDNode stateReward, CUDDNode transReward, CUDDNode nondetMask, CUDDVars allRowVars, CUDDVars allColVars, CUDDVars nondetVars, CUDDNode infReward, CUDDNode maybeReward, bool min)
        {
            DateTime startTime = DateTime.Now;
            int numberOfIterations = 0;

            CUDDNode a, allReward, newNondetMask, sol, tmp;

            // filter out rows (goal states and infinity states) from matrix
            CUDD.Ref(trans, maybeReward);
            a = CUDD.Function.Times(trans, maybeReward);

            // also remove goal and infinity states from state rewards vector
            CUDD.Ref(stateReward, maybeReward);
            CUDDNode tempStateReward = CUDD.Function.Times(stateReward, maybeReward);

            // multiply transition rewards by transition probs and sum rows
            // (note also filters out unwanted states at the same time)
            CUDD.Ref(transReward, a);
            CUDDNode tempTransReward = CUDD.Function.Times(transReward, a);
            tempTransReward = CUDD.Abstract.SumAbstract(tempTransReward, allColVars);

            // combine state and transition rewards
            allReward = CUDD.Function.Plus(tempStateReward, tempTransReward);

            // need to change mask because rewards are not necessarily in the range 0..1
            CUDD.Ref(nondetMask);
            newNondetMask = CUDD.Function.ITE(nondetMask, CUDD.PlusInfinity(), CUDD.Constant(0));

            // initial solution is infinity in 'inf' states, zero elsewhere
            // note: ok to do this because cudd matrix-multiply (and other ops)
            // treat 0 * inf as 0, unlike in IEEE 754 rules
            CUDD.Ref(infReward);
            sol = CUDD.Function.ITE(infReward, CUDD.PlusInfinity(), CUDD.Constant(0));

            while(true)
            {
                numberOfIterations++;

                tmp = CUDD.Matrix.MatrixMultiplyVector(a, sol, allRowVars, allColVars);

                // add rewards
                CUDD.Ref(allReward);
                tmp = CUDD.Function.Plus(tmp, allReward);

                if(min)
                {
                    CUDD.Ref(newNondetMask);
                    tmp = CUDD.Function.Maximum(tmp, newNondetMask);
                    tmp = CUDD.Abstract.MinAbstract(tmp, nondetVars);
                }
                else
                {
                    tmp = CUDD.Abstract.MaxAbstract(tmp, nondetVars);
                }

                CUDD.Ref(infReward);
                tmp = CUDD.Function.ITE(infReward, CUDD.PlusInfinity(), tmp);

                if (CUDD.IsEqual(tmp, sol))
                {
                    CUDD.Deref(tmp);
                    break;
                }

                CUDD.Deref(sol);
                sol = tmp;
            }
            
            DateTime endTime = DateTime.Now;
            double runningTime = (endTime - startTime).TotalSeconds;
            Debug.WriteLine("NondetReachReward: " + numberOfIterations + " iterations in " + runningTime + " seconds");

            //
            CUDD.Deref(a, allReward, newNondetMask);
            return sol;
        }
    }
}
