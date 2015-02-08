using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using PAT.Common.Classes.CUDDLib;
using PAT.Common.Classes.SemanticModels.LTS.BDD;

namespace PAT.Common.Classes.Expressions.ExpressionClass
{
    public sealed class If : Expression
    {    
        public Expression Condition, ThenPart, ElsePart;

        /// <summary>
        /// If then else expression
        /// </summary>
        /// <param name="c"></param>
        /// <param name="t"></param>
        /// <param name="e">Give null if the else part does not exist</param>
        public If(Expression c, Expression t, Expression e)
        {
            Condition = c;
            ThenPart = t;
            ElsePart = e;
            
            if (e != null)
            {
                this.HasVar = c.HasVar || t.HasVar || e.HasVar;

                expressionID = "I@" + Condition.ExpressionID + "{" + ThenPart.ExpressionID + "^" + ElsePart.ExpressionID + "}";
            }
            else
            {
                this.HasVar = c.HasVar || t.HasVar;
                expressionID = "I@" + Condition.ExpressionID + "{" + ThenPart.ExpressionID + "}";
            }

            ExpressionType = ExpressionType.If;
        }

        public override String ToString()
        {
            if (ElsePart != null)
            {
                return " if (" + Condition + ") {" + ThenPart + "} else {" + ElsePart + "}";
            }
            else
            {
                return " if (" + Condition + ") {" + ThenPart + "}";
            }
        }

        //public override String GetID()
        //{
        //    if (ElsePart != null)
        //    {
        //        return "I@" + Condition.GetID() + "{" + ThenPart.GetID() + "^" + ElsePart.GetID() + "}";
        //    }
        //    else
        //    {
        //        return "I@" + Condition.GetID() + "{" + ThenPart.GetID() + "}";
        //    }
        //}

        public override List<string> GetVars()
        {
            List<string> vars = this.Condition.GetVars();
            Ultility.Ultility.AddList(vars, this.ThenPart.GetVars());
            if (ElsePart != null)
            {
                Ultility.Ultility.AddList(vars, this.ElsePart.GetVars());
            }
            return vars;
        }

        public override Expression ClearConstant(Dictionary<string, Expression> constMapping)
        {
            return new If(Condition.ClearConstant(constMapping), ThenPart.ClearConstant(constMapping), ElsePart == null ? null : ElsePart.ClearConstant(constMapping));
        }

        /// <summary>
        /// Encode the If as a single statement
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public override ExpressionBDDEncoding TranslateBoolExpToBDD(Model model)
        {
            ExpressionBDDEncoding result = new ExpressionBDDEncoding();
            result.GuardDDs.Add(CUDD.Constant(1));

            result = TranslateStatementToBDD(result, model);

            return result;
        }

        /// <summary>
        /// Return variable values after the If statement is executed based on resultBefore
        /// [ REFS: 'result', DEREFS: 'resultBefore' ]
        /// </summary>
        public override ExpressionBDDEncoding TranslateStatementToBDD(ExpressionBDDEncoding resultBefore, Model model)
        {
            ExpressionBDDEncoding tempResult1 = new ExpressionBDDEncoding();
            ExpressionBDDEncoding tempResult2 = new ExpressionBDDEncoding();

            List<List<int>> updatedVariablesBefore = new List<List<int>>();
            for (int index = 0; index < resultBefore.Count(); index++)
            {
                updatedVariablesBefore.Add(model.GetColSupportedVars(resultBefore.GuardDDs[index]));
            }

            ExpressionBDDEncoding conditionDD = this.Condition.TranslateBoolExpToBDD(model);
            List<List<int>> usedVariablesInCondition = new List<List<int>>();
            for (int index = 0; index < conditionDD.Count(); index++)
            {
                usedVariablesInCondition.Add(model.GetRowSupportedVars(conditionDD.GuardDDs[index]));
            }

            for (int j1 = 0; j1 < resultBefore.Count(); j1++)
            {
                for (int j2 = 0; j2 < conditionDD.Count(); j2++)
                {
                    CUDDNode condition = conditionDD.GuardDDs[j2];
                    CUDD.Ref(condition);
                    foreach (int index in usedVariablesInCondition[j2])
                    {
                        if (updatedVariablesBefore[j1].Contains(index))
                        {
                            condition = CUDD.Variable.SwapVariables(condition, model.GetRowVars(index), model.GetColVars(index));
                        }
                    }

                    CUDD.Ref(resultBefore.GuardDDs[j1]);
                    CUDDNode transition = CUDD.Function.And(resultBefore.GuardDDs[j1], condition);

                    tempResult1.AddNodeToGuard(transition);
                }
            }

            //the condition is not always false (a = 1; if(a > 1))
            if (tempResult1.Count() > 0)
            {
                tempResult1 = this.ThenPart.TranslateStatementToBDD(tempResult1, model);
            }

            //
            tempResult2 = new ExpressionBDDEncoding();
            for (int j1 = 0; j1 < resultBefore.Count(); j1++)
            {
                for (int j2 = 0; j2 < conditionDD.Count(); j2++)
                {
                    CUDDNode condition = conditionDD.GuardDDs[j2];
                    CUDD.Ref(condition);
                    condition = CUDD.Function.Not(condition);
                    foreach (int index in usedVariablesInCondition[j2])
                    {
                        if (updatedVariablesBefore[j1].Contains(index))
                        {
                            condition = CUDD.Variable.SwapVariables(condition, model.GetRowVars(index), model.GetColVars(index));
                        }
                    }

                    CUDD.Ref(resultBefore.GuardDDs[j1]);
                    CUDDNode transition = CUDD.Function.And(resultBefore.GuardDDs[j1], condition);

                    tempResult2.AddNodeToGuard(transition);
                }
            }

            if (this.ElsePart != null && tempResult2.Count() > 0)
            {
                tempResult2 = this.ElsePart.TranslateStatementToBDD(tempResult2, model);
            }

            conditionDD.DeRef();

            //Combine
            if (tempResult1.Count() == 0 && tempResult2.Count() == 0)//condition always false, no else part
            {
                return resultBefore;
            }
            else if (tempResult1.Count() == 0 && tempResult2.Count() > 0)//condition always false, having else part
            {
                resultBefore.DeRef();
                return tempResult2;
            }
            else
            {
                resultBefore.DeRef();
                tempResult1.AddNodeToGuard(tempResult2.GuardDDs);
                return tempResult1;
            }
        }

        public override bool HasExternalLibraryCall()
        {
            if (ElsePart != null)
            {
                return Condition.HasExternalLibraryCall() || ThenPart.HasExternalLibraryCall() ||
                       ElsePart.HasExternalLibraryCall();
            }
            else
            {
                return Condition.HasExternalLibraryCall() || ThenPart.HasExternalLibraryCall();
            }
        }
#if DEBUG
        public override void GenerateMSIL(ILGenerator ilGenerator, TypeBuilder typeBuilder)
        {
            Condition.GenerateMSIL(ilGenerator, typeBuilder);

            ilGenerator.Emit(OpCodes.Ldc_I4_0);
            ilGenerator.Emit(OpCodes.Ceq);

            if(ElsePart == null)
            {
                Label labelAfterIf = ilGenerator.DefineLabel();

                ilGenerator.Emit(OpCodes.Brtrue, labelAfterIf);

                ThenPart.GenerateMSIL(ilGenerator, typeBuilder);

                ilGenerator.MarkLabel(labelAfterIf);
            }
            else
            {
                Label labelForFalse = ilGenerator.DefineLabel();
                Label labelAfterIf = ilGenerator.DefineLabel();

                ilGenerator.Emit(OpCodes.Brtrue, labelForFalse);

                ThenPart.GenerateMSIL(ilGenerator, typeBuilder);

                ilGenerator.Emit(OpCodes.Br_S, labelAfterIf);

                ilGenerator.MarkLabel(labelForFalse);
                ElsePart.GenerateMSIL(ilGenerator, typeBuilder);

                ilGenerator.MarkLabel(labelAfterIf);
            }
        }
#endif
    }
}