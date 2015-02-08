using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using PAT.Common.Classes.CUDDLib;
using PAT.Common.Classes.SemanticModels.LTS.BDD;

namespace PAT.Common.Classes.Expressions.ExpressionClass
{
    public sealed class While : Expression
    {      
        public Expression Test, Body;
        public While(Expression t, Expression b)
        {
            Test = t;
            Body = b;
            this.HasVar = t.HasVar || b.HasVar;
            ExpressionType = ExpressionType.While;

            expressionID = "W@" + Test.ExpressionID + "{" + Body.ExpressionID + "}";
        }


        public override String ToString()
        {
            return "while (" + Test.ToString() + ") {" + Body.ToString() + "}";
        }

        //public override String GetID()
        //{
        //    return "W@" + Test.GetID() + "{" + Body.GetID() + "}";
        //}

        public override List<string> GetVars()
        {
            List<string> vars = Test.GetVars();
            Ultility.Ultility.AddList(vars, Body.GetVars());
            return vars;
        }

        public override Expression ClearConstant(Dictionary<string, Expression> constMapping)
        {
            return new While(Test.ClearConstant(constMapping), Body.ClearConstant(constMapping));
        }

        public override ExpressionBDDEncoding TranslateBoolExpToBDD(Model model)
        {
            ExpressionBDDEncoding result = new ExpressionBDDEncoding();
            result.GuardDDs.Add(CUDD.Constant(1));

            result = TranslateStatementToBDD(result, model);

            return result;
        }

        /// <summary>
        /// [ REFS: 'result', DEREFS: 'resultBefore' ]
        /// </summary>
        public override ExpressionBDDEncoding TranslateStatementToBDD(ExpressionBDDEncoding resultBefore, Model model)
        {
            ExpressionBDDEncoding result = new ExpressionBDDEncoding();

            CUDDNode conditionDD = CUDD.Function.Or(this.Test.TranslateBoolExpToBDD(model).GuardDDs);

            do
            {
                ExpressionBDDEncoding tempResult = new ExpressionBDDEncoding();

                List<List<int>> updatedVariablesBefore = new List<List<int>>();
                for (int index = 0; index < resultBefore.Count(); index++)
                {
                    updatedVariablesBefore.Add(model.GetColSupportedVars(resultBefore.GuardDDs[index]));
                }

                

                List<int> usedVariablesInCondition = model.GetRowSupportedVars(conditionDD);


                for (int j1 = 0; j1 < resultBefore.Count(); j1++)
                {
                    foreach (int index in usedVariablesInCondition)
                    {
                        if (updatedVariablesBefore[j1].Contains(index))
                        {
                            conditionDD = CUDD.Variable.SwapVariables(conditionDD, model.GetRowVars(index), model.GetColVars(index));
                        }
                    }

                    //Add configuration making the While condition true
                    CUDD.Ref(resultBefore.GuardDDs[j1], conditionDD);
                    CUDDNode transition = CUDD.Function.And(resultBefore.GuardDDs[j1], conditionDD);
                    tempResult.AddNodeToGuard(transition);

                    //Add configuration making the While condition false
                    CUDD.Ref(resultBefore.GuardDDs[j1], conditionDD);
                    CUDDNode falseTransition = CUDD.Function.And(resultBefore.GuardDDs[j1], CUDD.Function.Not(conditionDD));
                    result.AddNodeToGuard(falseTransition);
                }

                //There is no any configuration making the While condition true
                if (tempResult.Count() > 0)
                {
                    resultBefore.DeRef();
                    resultBefore = this.Body.TranslateStatementToBDD(tempResult, model);
                }
                else
                {
                    break;
                }
            } while (true);

            resultBefore.DeRef();

            return result;
        }

        public override bool HasExternalLibraryCall()
        {
            return Test.HasExternalLibraryCall() || Body.HasExternalLibraryCall();
        }

        #if DEBUG
        public override void GenerateMSIL(ILGenerator ilGenerator, TypeBuilder typeBuilder)
        {
            Label checkCondition = ilGenerator.DefineLabel();
            Label whileBody = ilGenerator.DefineLabel();

            ilGenerator.Emit(OpCodes.Br_S, checkCondition);

            ilGenerator.MarkLabel(whileBody);
            Body.GenerateMSIL(ilGenerator, typeBuilder);

            ilGenerator.MarkLabel(checkCondition);
            Test.GenerateMSIL(ilGenerator, typeBuilder);

            ilGenerator.Emit(OpCodes.Brtrue, whileBody);
        }
#endif
    }
}