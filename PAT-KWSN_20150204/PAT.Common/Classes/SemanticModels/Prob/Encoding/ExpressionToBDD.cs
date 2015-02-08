using System;
using System.Collections.Generic;
using System.Diagnostics;
using PAT.Common.Classes.CUDDLib;
using PAT.Common.Classes.Expressions.ExpressionClass;
using PAT.Common.Classes.SemanticModels.Prob.ExpressionClass;
using PAT.Common.Classes.SemanticModels.Prob.SystemStructure;

namespace PAT.Common.Classes.SemanticModels.Prob.Encoding
{
    public class ExpressionToBDD
    {
        public VariableList varList;

        /// <summary>
        /// And of all row-condition on all vars in range
        /// </summary>
        public CUDDNode allRowVarRanges;

        /// <summary>
        /// Identity of all variable
        /// </summary>
        public List<CUDDNode> varIdentities;

        /// <summary>
        /// Encoding of all variable
        /// </summary>
        public List<CUDDNode> varEncodings = new List<CUDDNode>();

        /// <summary>
        /// Prob transition matrix
        /// </summary>
        public CUDDNode trans;

        /// <summary>
        /// Init state
        /// </summary>
        public CUDDNode start;

        /// <summary>
        /// State reward of all reward structures
        /// </summary>
        public List<CUDDNode> stateRewards = new List<CUDDNode>();

        /// <summary>
        /// transition rewards of all reward structures
        /// </summary>
        public List<CUDDNode> transRewards = new List<CUDDNode>();

        /// <summary>
        /// All row vars
        /// </summary>
        public CUDDVars allRowVars;

        /// <summary>
        /// All col vars
        /// </summary>
        public CUDDVars allColVars;

        public CUDDNode trans01;
        public CUDDNode reach;

        public  ExpressionToBDD(VariableList varList, List<CUDDNode> varEncodings)
        {
            this.varList = varList;
            this.varEncodings = varEncodings;
        }

        protected ExpressionToBDD(ProbModel model)
        {
            this.allRowVars = model.allRowVars;
            this.allColVars = model.allColVars;
            this.allRowVarRanges = model.allRowVarRanges;
            this.varIdentities = model.varIdentities;
            this.varList = model.varList;
            this.varEncodings = model.varEncodings;
            this.trans = model.trans;
            this.start = model.start;

            this.stateRewards = model.stateRewards;
            this.transRewards = model.transRewards;
        }

        public CUDDNode EncodeExpression(Expression exp)
        {
            if(exp is BoolConstant)
            {
                return EncodeBoolConstannt(exp as BoolConstant);
            }
            else if (exp is IntConstant)
            {
                return EncodeIntConstant(exp as IntConstant);
            }
            else if (exp is DoubleConstant)
            {
                return EncodeDoubleConstant(exp as DoubleConstant);
            }
            else if (exp is FuncNary)
            {
                return EncodeNary(exp as FuncNary);
            }
            else if (exp is PrimitiveApplication)
            {
                return EncodePrimitive(exp as PrimitiveApplication);
            }
            else if (exp is Variable)
            {
                return EncodeVar(exp as Variable);
            }
            else
            {
                throw new Exception("Invalid Expression!");
            }

        }

        private CUDDNode EncodeBoolConstannt (BoolConstant exp)
        {
            return (exp.Value) ? CUDD.Constant(1) : CUDD.Constant(0);
        }

        private CUDDNode EncodeIntConstant(IntConstant exp)
        {
            return CUDD.Constant(exp.Value);
        }

        private CUDDNode EncodeDoubleConstant(DoubleConstant exp)
        {
            return CUDD.Constant(exp.Value);
        }

        private CUDDNode EncodeNary(FuncNary exp)
        {
            CUDDNode dd = EncodeExpression(exp.expressions[0]);

            for (int i = 1; i < exp.expressions.Count; i++ )
            {
                switch (exp.Operator)
                {
                    case FunctionName.Min:
                        dd = CUDD.Function.Minimum(dd, EncodeExpression(exp.expressions[i]));
                        break;
                    case FunctionName.Max:
                        dd = CUDD.Function.Maximum(dd, EncodeExpression(exp.expressions[i]));
                        break;
                    default:
                        throw new Exception("Invalid Expression!");
                }
            }

            return dd;
        }

        private CUDDNode EncodePrimitive(PrimitiveApplication exp)
        {
            if(exp.Argument2 != null)
            {
                return EncodeBinary(exp);
            }
            else
            {
                return EncodeUnary(exp);
            }
        }

        private CUDDNode EncodeBinary(PrimitiveApplication exp)
        {

            CUDDNode dd1 = EncodeExpression(exp.Argument1);
            CUDDNode dd2 = EncodeExpression(exp.Argument2);

            CUDDNode dd;
            switch (exp.Operator)
            {
                case PrimitiveApplication.AND:
                    dd = CUDD.Function.And(dd1, dd2);
                    break;
                case PrimitiveApplication.OR:
                    dd = CUDD.Function.Or(dd1, dd2);
                    break;
                case PrimitiveApplication.IMPLIES:
                    dd = CUDD.Function.Implies(dd1, dd2);
                    break;
                case PrimitiveApplication.PLUS:
                    dd = CUDD.Function.Plus(dd1, dd2);
                    break;
                case PrimitiveApplication.MINUS:
                    dd = CUDD.Function.Minus(dd1, dd2);
                    break;
                case PrimitiveApplication.TIMES:
                    dd = CUDD.Function.Times(dd1, dd2);
                    break;
                case PrimitiveApplication.DIVIDE:
                    dd = CUDD.Function.Divide(dd1, dd2);
                    break;
                case PrimitiveApplication.MOD:
                    dd = CUDD.Function.Modulo(dd1, dd2);
                    break;
                case PrimitiveApplication.EQUAL:
                    dd = CUDD.Function.Equal(dd1, dd2);
                    break;
                case PrimitiveApplication.NOT_EQUAL:
                    dd = CUDD.Function.NotEqual(dd1, dd2);
                    break;
                case PrimitiveApplication.GREATER:
                    dd = CUDD.Function.Greater(dd1, dd2);
                    break;
                case PrimitiveApplication.GREATER_EQUAL:
                    dd = CUDD.Function.GreaterEqual(dd1, dd2);
                    break;
                case PrimitiveApplication.LESS:
                    dd = CUDD.Function.Less(dd1, dd2);
                    break;
                case PrimitiveApplication.LESS_EQUAL:
                    dd = CUDD.Function.LessEqual(dd1, dd2);
                    break;
                default:
                    throw new Exception("Invalid Expression!");

            }

            return dd;
        }

        private CUDDNode EncodeUnary(PrimitiveApplication exp)
        {
            CUDDNode dd1 = EncodeExpression(exp.Argument1);

            CUDDNode dd;
            switch (exp.Operator)
            {
                case  PrimitiveApplication.NOT:
                    dd = CUDD.Function.Not(dd1);
                    break;
                case PrimitiveApplication.MINUS:
                    dd = CUDD.Function.Minus(CUDD.Constant(0), dd1);
                    break;
                default:
                    throw new Exception("Invalid Expression!");  
            }
            return dd;
        }

        private CUDDNode EncodeVar(Variable exp)
        {
            int varIndex = varList.GetVarIndex(exp.ExpressionID);

            CUDD.Ref(varEncodings[varIndex]);
            CUDDNode varDD = varEncodings[varIndex];

            return varDD;
        }

        /// <summary>
        /// Swap row and column variables
        /// [ REFS: 'result', DEREFS: 'dd']
        /// </summary>
        /// <param name="dd"></param>
        /// <returns></returns>
        public CUDDNode SwapRowColVars(CUDDNode dd)
        {
            return CUDD.Variable.SwapVariables(dd, allRowVars, allColVars);
        }

        /// <summary>
        /// P ◦ R is the set of all successors of states in the set P
        /// [ REFS: 'result', DEREFS:states]
        /// </summary>
        /// <param name="states"></param>
        /// <param name="transitions"></param>
        /// <returns></returns>
        public CUDDNode Successors(CUDDNode states, CUDDNode transitions)
        {
            CUDD.Ref(transitions);
            CUDDNode temp = CUDD.Function.And(states, transitions);


            CUDDNode successors = CUDD.Abstract.ThereExists(temp, allRowVars);
            successors = SwapRowColVars(successors);

            return successors;
        }

        /// <summary>
        /// return all states reachable from start and Start
        /// P ◦ R*
        /// [ REFS: 'result', DEREFS:start]
        /// </summary>
        /// <param name="start"></param>
        /// <param name="transition"></param>
        /// <returns></returns>
        public CUDDNode SuccessorsStart(CUDDNode start, CUDDNode transition)
        {
            DateTime startTime = DateTime.Now;
            int numberOfIterations = 0;

            CUDD.Ref(start);
            CUDDNode allReachableStates = start;

            CUDDNode currentStep = start;

            while (true)
            {
                numberOfIterations++;

                currentStep = Successors(currentStep, transition);

                CUDD.Ref(allReachableStates, currentStep);
                CUDDNode allReachableTemp = CUDD.Function.Or(allReachableStates, currentStep);

                if (allReachableTemp.Equals(allReachableStates))
                {
                    CUDD.Deref(currentStep, allReachableTemp);

                    DateTime endTime = DateTime.Now;
                    double runningTime = (endTime - startTime).TotalSeconds;
                    Debug.WriteLine("Reachability: " + numberOfIterations + " iterations in " + runningTime + " seconds");

                    return allReachableStates;
                }
                else
                {
                    currentStep = CUDD.Function.Different(currentStep, allReachableStates);
                    allReachableStates = allReachableTemp;
                }
            }
        }


        /// <summary>
        /// [ REFS: '', DEREFS:]
        /// </summary>
        /// <param name="allProb"></param>
        /// <param name="state"></param>
        public double GetMinProb(CUDDNode allProb, CUDDNode filter)
        {
            double result = 0;

            CUDD.Ref(filter, reach);
            CUDDNode newFilter = CUDD.Function.And(filter, reach);

            if(newFilter.Equals(CUDD.ZERO))
            {
                result = double.MaxValue;
            }
            else
            {
                CUDD.Ref(newFilter, allProb);
                CUDDNode tmp = CUDD.Function.ITE(newFilter, allProb, CUDD.PlusInfinity());

                result = CUDD.FindMin(tmp);
                CUDD.Deref(tmp);
            }

            CUDD.Deref(newFilter);

            return result;
        }

        /// <summary>
        /// [ REFS: '', DEREFS:]
        /// </summary>
        /// <param name="allProb"></param>
        /// <param name="state"></param>
        public double GetMaxProb(CUDDNode allProb, CUDDNode filter)
        {
            double result = 0;

            CUDD.Ref(filter, reach);
            CUDDNode newFilter = CUDD.Function.And(filter, reach);

            if (newFilter.Equals(CUDD.ZERO))
            {
                result = double.MinValue;
            }
            else
            {
                CUDD.Ref(newFilter, allProb);
                CUDDNode tmp = CUDD.Function.ITE(newFilter, allProb, CUDD.MinusInfinity());

                result = CUDD.FindMax(tmp);
                CUDD.Deref(tmp);
            }

            CUDD.Deref(newFilter);

            return result;
        }

        public void GetResult(CUDDNode allProb, CUDDNode filter)
        {
            double min = GetMinProb(allProb, filter);
            double max = GetMaxProb(allProb, filter);

            Debug.WriteLine("[" + min + "," + max + "]");
        }

        public virtual void Close()
        {
            allRowVars.Deref();
            allColVars.Deref();
            CUDD.Deref(allRowVarRanges, trans, start, trans01, reach);
            CUDD.Deref(varIdentities, varEncodings, stateRewards, transRewards);

            CUDD.CloseDownCUDD();
        }
    }
}
