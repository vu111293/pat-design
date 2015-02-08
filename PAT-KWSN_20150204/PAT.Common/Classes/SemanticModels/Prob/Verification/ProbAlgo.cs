using System;
using System.Collections.Generic;
using System.Diagnostics;
using PAT.Common.Classes.CUDDLib;

namespace PAT.Common.Classes.SemanticModels.Prob.Verification
{
    public class ProbAlgo
    {

        /// <summary>
        /// Return states having probability 0 of b1 U b2
        /// </summary>
        /// <param name="trans01"></param>
        /// <param name="reach"></param>
        /// <param name="allRowVars"></param>
        /// <param name="allColVars"></param>
        /// <param name="b1"></param>
        /// <param name="b2"></param>
        /// <returns></returns>
        public static CUDDNode Prob0(CUDDNode trans01, CUDDNode reach, CUDDVars allRowVars, CUDDVars allColVars, CUDDNode b1, CUDDNode b2)
        {
            DateTime startTime = DateTime.Now;
            int numberOfIterations = 0;

            CUDD.Ref(b2);
            CUDDNode sol = b2;

            while (true)
            {
                numberOfIterations++;

                CUDD.Ref(sol);
                CUDDNode tmp = CUDD.Variable.SwapVariables(sol, allRowVars, allColVars);

                CUDD.Ref(trans01);
                tmp = CUDD.Function.And(tmp, trans01);

                tmp = CUDD.Abstract.ThereExists(tmp, allColVars);

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
            sol = CUDD.Function.And(CUDD.Function.Not(sol), reach);

            DateTime endTime = DateTime.Now;
            double runningTime = (endTime - startTime).TotalSeconds;
            Debug.WriteLine("Prob0: " + numberOfIterations + " iterations in " + runningTime + " seconds");

            return sol;
        }

        /// <summary>
        /// Return state having probability 1 of b1 U b2
        /// </summary>
        /// <param name="trans01"></param>
        /// <param name="reach"></param>
        /// <param name="allRowVars"></param>
        /// <param name="allColVars"></param>
        /// <param name="b1"></param>
        /// <param name="b2"></param>
        /// <param name="no"></param>
        /// <returns></returns>
        public static CUDDNode Prob1(CUDDNode trans01, CUDDNode reach, CUDDVars allRowVars, CUDDVars allColVars, CUDDNode b1, CUDDNode b2, CUDDNode no)
        {
            DateTime startTime = DateTime.Now;
            int numberOfIterations = 0;

            CUDD.Ref(no);
            CUDDNode sol = no;

            while (true)
            {
                numberOfIterations++;

                CUDD.Ref(sol);
                CUDDNode tmp = CUDD.Variable.SwapVariables(sol, allRowVars, allColVars);

                CUDD.Ref(trans01);
                tmp = CUDD.Function.And(tmp, trans01);

                tmp = CUDD.Abstract.ThereExists(tmp, allColVars);

                CUDD.Ref(b1);
                tmp = CUDD.Function.And(b1, tmp);

                CUDD.Ref(b2);
                tmp = CUDD.Function.And(CUDD.Function.Not(b2), tmp);

                CUDD.Ref(no);
                tmp = CUDD.Function.Or(no, tmp);

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
            Debug.WriteLine("Prob1: " + numberOfIterations + " iterations in " + runningTime + " seconds");

            return sol;
        }
        
        public static CUDDNode ProbBoundedUntil(CUDDNode trans, CUDDVars allRowVars, CUDDVars allColVars, CUDDNode yes, CUDDNode maybe, int bound)
        {
            DateTime startTime = DateTime.Now;

            CUDDNode a, sol, tmp;

            CUDD.Ref(trans, maybe);
            a = CUDD.Function.Times(trans, maybe);

            CUDD.Ref(yes);
            sol = yes;

            for (int i = 1; i <= bound; i++)
            {
                tmp = CUDD.Matrix.MatrixMultiplyVector(a, sol, allRowVars, allColVars);

                CUDD.Ref(yes);
                tmp = CUDD.Function.Maximum(tmp, yes);

                CUDD.Deref(sol);
                sol = tmp;
            }

            DateTime endTime = DateTime.Now;
            double runningTime = (endTime - startTime).TotalSeconds;
            Debug.WriteLine("ProbBoundedUntil: " + bound + " iterations in " + runningTime + " seconds");

            //
            CUDD.Deref(a);

            return sol;
        }

        public static CUDDNode ProbUntil(CUDDNode trans, CUDDNode reach, CUDDVars allRowVars, CUDDVars allColVars, CUDDNode yes, CUDDNode maybe)
        {
            CUDD.Ref(trans, maybe);
            CUDDNode a = CUDD.Function.Times(trans, maybe);

            CUDDNode tmp = CUDD.Matrix.Identity(allRowVars, allColVars);
            CUDD.Ref(reach);
            tmp = CUDD.Function.And(tmp, reach);

            a = CUDD.Function.Minus(tmp, a);

            CUDD.Ref(yes);
            CUDDNode b = yes;

            CUDDNode sol = Jacobi(a, b, b, reach, allRowVars, allColVars, 1);

            CUDD.Deref(a, b);
            return sol;

        }

        public static CUDDNode ProbCumulReward(CUDDNode trans, CUDDNode stateReward, CUDDNode transReward, CUDDVars allRowVars, CUDDVars allColVars, int bound)
        {
            DateTime startTime = DateTime.Now;

            // multiply transition rewards by transition probs and sum rows
            // then combine state and transition rewards and put in a vector
            CUDD.Ref(transReward, trans);
            CUDDNode allReward = CUDD.Function.Times(transReward, trans);

            allReward = CUDD.Abstract.SumAbstract(allReward, allColVars);

            CUDD.Ref(stateReward);
            allReward = CUDD.Function.Plus(stateReward, allReward);

            // initial solution is zero
            CUDDNode sol = CUDD.Constant(0);

            for (int i = 0; i < bound; i++)
            {
                CUDDNode tmp = CUDD.Matrix.MatrixMultiplyVector(trans, sol, allRowVars, allColVars);

                // add in (combined state and transition) rewards
                CUDD.Ref(allReward);
                tmp = CUDD.Function.Plus(tmp, allReward);

                CUDD.Deref(sol);
                sol = tmp;
            }

            //
            CUDD.Deref(allReward);

            DateTime endTime = DateTime.Now;
            double runningTime = (endTime - startTime).TotalSeconds;
            Debug.WriteLine("ProbCumulReward: " + bound + " iterations in " + runningTime + " seconds");

            //
            return sol;
        }

        public static CUDDNode ProbInstReward(CUDDNode trans, CUDDNode stateReward, CUDDVars allRowVars, CUDDVars allColVars, int bound)
        {
            DateTime startTime = DateTime.Now;

            // initial solution is the state rewards
            CUDD.Ref(stateReward);
            CUDDNode sol = stateReward;

            for (int iters = 0; iters < bound; iters++)
            {
                CUDDNode tmp = CUDD.Matrix.MatrixMultiplyVector(trans, sol, allRowVars, allColVars);
                CUDD.Deref(sol);
                sol = tmp;
            }

            DateTime endTime = DateTime.Now;
            double runningTime = (endTime - startTime).TotalSeconds;
            Debug.WriteLine("ProbInstReward: " + bound + " iterations in " + runningTime + " seconds");

            return sol;
        }

        public static CUDDNode ProbReachReward(CUDDNode trans, CUDDNode stateReward, CUDDNode transReward, CUDDNode reach, CUDDVars allRowVars, CUDDVars allColVars, CUDDNode maybe, CUDDNode inf)
        {
            CUDD.Ref(trans, maybe);
            CUDDNode a = CUDD.Function.Times(trans, maybe);

            CUDD.Ref(stateReward, maybe);
            CUDDNode newStateReward = CUDD.Function.Times(stateReward, maybe);

            CUDD.Ref(transReward, a);
            CUDDNode newTransRewards = CUDD.Function.Times(transReward, a);
            newTransRewards = CUDD.Abstract.SumAbstract(newTransRewards, allColVars);

            CUDDNode allReward = CUDD.Function.Plus(newStateReward, newTransRewards);

            //
            CUDDNode tmp = CUDD.Matrix.Identity(allRowVars, allColVars);
            CUDD.Ref(reach);
            tmp = CUDD.Function.And(tmp, reach);
            a = CUDD.Function.Minus(tmp, a);

            CUDDNode sol = Jacobi(a, allReward, allReward, reach, allRowVars, allColVars, 1);

            // set reward for infinity states to infinity
            CUDD.Ref(inf);
            sol = CUDD.Function.ITE(inf, CUDD.PlusInfinity(), sol);

            //
            CUDD.Deref(a, allReward);
            return sol;
        }

        /// <summary>
        /// Solve the linear equation system Ax = b with Jacobi
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="init">initial solution</param>
        /// <param name="reach"></param>
        /// <param name="allRowVars"></param>
        /// <param name="allColVars"></param>
        /// <param name="omega">jor parameter</param>
        /// <returns></returns>
        public static CUDDNode Jacobi(CUDDNode a, CUDDNode b, CUDDNode init, CUDDNode reach, CUDDVars allRowVars, CUDDVars allColVars, double omega)
        {
            DateTime startTime = DateTime.Now;
            int numberOfIterations = 0;

            CUDDNode id = CUDD.Matrix.Identity(allRowVars, allColVars);

            CUDD.Ref(reach);
            id = CUDD.Function.And(id, reach);

            CUDD.Ref(id, a);
            CUDDNode diags = CUDD.Function.Times(id, a);
            diags = CUDD.Abstract.SumAbstract(diags, allColVars);

            //
            CUDD.Ref(id, a);
            CUDDNode newA = CUDD.Function.ITE(id, CUDD.Constant(0), a);

            newA = CUDD.Function.Times(CUDD.Constant(-1), newA);

            // divide a,b by diagonal
            CUDD.Ref(diags);
            newA = CUDD.Function.Divide(newA, diags);

            CUDD.Ref(b, diags);
            CUDDNode newB = CUDD.Function.Divide(b, diags);


            // print out some memory usage
            Debug.WriteLine("Iteration matrix MTBDD... [nodes = " + CUDD.GetNumNodes(newA) + "]");
            Debug.WriteLine("Diagonals MTBDD... [nodes = " + CUDD.GetNumNodes(diags) + "]");


            CUDD.Ref(init);
            CUDDNode sol = init;

            while(true)
            {
                numberOfIterations++;

                CUDDNode tmp = CUDD.Matrix.MatrixMultiplyVector(newA, sol, allRowVars, allColVars);

                CUDD.Ref(newB);
                tmp = CUDD.Function.Plus(tmp, newB);

                if(omega != 1)
                {
                    tmp = CUDD.Function.Times(tmp, CUDD.Constant(omega));

                    CUDD.Ref(sol);
                    tmp = CUDD.Function.Plus(tmp, CUDD.Function.Times(sol, CUDD.Constant(1 - omega)));
                }

                if(CUDD.IsEqual(tmp, sol))
                {
                    CUDD.Deref(tmp);
                    break;
                }

                CUDD.Deref(sol);
                sol = tmp;
            }

            CUDD.Deref(id, diags, newA, newB);

            DateTime endTime = DateTime.Now;
            double runningTime = (endTime - startTime).TotalSeconds;
            Debug.WriteLine("Jacobi: " + numberOfIterations + " iterations in " + runningTime + " seconds");

            return sol;
        }
    }
}
