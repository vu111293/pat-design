using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using PAT.Common.Classes.CUDDLib;
using PAT.Common.Classes.SemanticModels.LTS.BDD;


namespace PAT.Common.Classes.Expressions.ExpressionClass
{
    public sealed class Assignment : Expression
    {
        public string LeftHandSide;
        public Expression RightHandSide;
        
        public Assignment(string l, Expression r)
        {
            LeftHandSide = l;
            RightHandSide = r;
            ExpressionType = ExpressionType.Assignment;
            HasVar = true;

            expressionID = LeftHandSide + "=" + RightHandSide.ExpressionID;
        }

        public override string ToString()
        {
            return LeftHandSide + " = " + RightHandSide.ToString() + ";";
        }

        public override List<string> GetVars()
        {
            List<string> vars = this.RightHandSide.GetVars();
            if (!vars.Contains(LeftHandSide))
            {
                vars.Add(LeftHandSide);
            }

            return vars;
        }

        public override Expression ClearConstant(Dictionary<string, Expression> constMapping)
        {
            return new Assignment((constMapping.ContainsKey(LeftHandSide)? constMapping[LeftHandSide].ExpressionID: LeftHandSide), RightHandSide.ClearConstant(constMapping));
        }

        /// <summary>
        /// Return only guards for complete boolean expression, no expression
        /// Use this when we want to encode a single assignment or the assignment does not depend other assignments.
        /// </summary>
        public override ExpressionBDDEncoding TranslateBoolExpToBDD(Model model)
        {
            ExpressionBDDEncoding result = new ExpressionBDDEncoding();
            ExpressionBDDEncoding variableBddEncoding = new VariablePrime(this.LeftHandSide).TranslateIntExpToBDD(model);

            ExpressionBDDEncoding valueBddEncoding = this.RightHandSide.TranslateIntExpToBDD(model);

            for (int i = 0; i < variableBddEncoding.Count(); i++)
            {
                for (int j = 0; j < valueBddEncoding.Count(); j++)
                {
                    CUDD.Ref(variableBddEncoding.GuardDDs[i], valueBddEncoding.GuardDDs[j]);
                    CUDDNode guardDD = CUDD.Function.And(variableBddEncoding.GuardDDs[i], valueBddEncoding.GuardDDs[j]);
                    if (guardDD.Equals(CUDD.ZERO))
                    {
                        CUDD.Deref(guardDD);
                        continue;
                    }

                    CUDD.Ref(variableBddEncoding.ExpressionDDs[i], valueBddEncoding.ExpressionDDs[j]);
                    CUDDNode assignmentDD = CUDD.Function.Equal(variableBddEncoding.ExpressionDDs[i], valueBddEncoding.ExpressionDDs[j]);

                    guardDD = CUDD.Function.And(guardDD, assignmentDD);
                    result.AddNodeToGuard(guardDD);
                }
            }
            //remove unused expression
            variableBddEncoding.DeRef();
            valueBddEncoding.DeRef();

            return result;
        }

        /// <summary>
        /// Return the variable values after the assignment based on the current variable values in resultBefore
        /// [ REFS: 'result', DEREFS: 'resultBefore' ]
        /// </summary>
        public override ExpressionBDDEncoding TranslateStatementToBDD(ExpressionBDDEncoding resultBefore, Model model)
        {
            ExpressionBDDEncoding newUpdate = this.TranslateBoolExpToBDD(model);

            ExpressionBDDEncoding tempResult = new ExpressionBDDEncoding();

            List<List<int>> updatedVariablesInNewUpdate = new List<List<int>>();
            for (int index = 0; index < newUpdate.Count(); index++)
            {
                updatedVariablesInNewUpdate.Add(model.GetColSupportedVars(newUpdate.GuardDDs[index]));
            }

            List<List<int>> usedVariableInNewUpdate = new List<List<int>>();
            for (int index = 0; index < newUpdate.Count(); index++)
            {
                usedVariableInNewUpdate.Add(model.GetRowSupportedVars(newUpdate.GuardDDs[index]));
            }

            List<List<int>> updatedVariablesBefore = new List<List<int>>();
            for (int index = 0; index < resultBefore.Count(); index++)
            {
                updatedVariablesBefore.Add(model.GetColSupportedVars(resultBefore.GuardDDs[index]));
            }

            for (int j1 = 0; j1 < resultBefore.Count(); j1++)
            {
                for (int j2 = 0; j2 < newUpdate.Count(); j2++)
                {
                    //a variable is already updated, now is updated again and it also apprears in the value expression
                    if (IsComplexUpdate(updatedVariablesBefore[j1], updatedVariablesInNewUpdate[j2], usedVariableInNewUpdate[j2]))
                    {
                        model.CreateTemporaryVar();
                        Expression newUpdate1;
                        Expression newUpdate2;

                        newUpdate1 = new Assignment(Model.TEMPORARY_VARIABLE, this.RightHandSide);
                        newUpdate2 = new Assignment(this.LeftHandSide, new Variable(Model.TEMPORARY_VARIABLE));

                        resultBefore.Ref();
                        ExpressionBDDEncoding tempResult1 = newUpdate1.TranslateStatementToBDD(resultBefore, model);
                        ExpressionBDDEncoding tempResult2 = newUpdate2.TranslateStatementToBDD(tempResult1, model);
                        for (int i = 0; i < tempResult2.Count(); i++)
                        {
                            tempResult2.GuardDDs[i] = CUDD.Abstract.ThereExists(tempResult2.GuardDDs[i], model.GetRowVars(Model.TEMPORARY_VARIABLE));
                            tempResult2.GuardDDs[i] = CUDD.Abstract.ThereExists(tempResult2.GuardDDs[i], model.GetColVars(Model.TEMPORARY_VARIABLE));

                            tempResult.AddNodeToGuard(tempResult2.GuardDDs[i]);
                        }
                    }
                    else
                    {
                        //swap row, col updated variable in result in the new update command expression
                        CUDDNode update2 = newUpdate.GuardDDs[j2];
                        CUDD.Ref(update2);
                        foreach (int index in updatedVariablesBefore[j1])
                        {
                            if (usedVariableInNewUpdate[j2].Contains(index))
                            {
                                update2 = CUDD.Variable.SwapVariables(update2, model.GetColVars(index), model.GetRowVars(index));
                            }
                        }

                        //Restrict updated variable in new update of the old update
                        CUDDNode update1 = resultBefore.GuardDDs[j1];
                        CUDD.Ref(update1);
                        foreach (int index in updatedVariablesInNewUpdate[j2])
                        {
                            if (updatedVariablesBefore[j1].Contains(index))
                            {
                                update1 = CUDD.Abstract.ThereExists(update1, model.GetColVars(index));
                            }
                        }

                        CUDDNode transition = CUDD.Function.And(update1, update2);
                        tempResult.AddNodeToGuard(transition);
                    }
                }
            }

            resultBefore.DeRef();
            newUpdate.DeRef();

            return tempResult;
        }

        /// <summary>
        /// a = 1; a = a + 1;
        /// </summary>
        public static bool IsComplexUpdate(List<int> updatedVariablesBefore, List<int> updatedVariablesInNewUpdate, List<int> usedVariableInNewUpdate)
        {
            return (updatedVariablesInNewUpdate.Count > 0 && usedVariableInNewUpdate.Contains(updatedVariablesInNewUpdate[0]) &&
                        updatedVariablesBefore.Contains(updatedVariablesInNewUpdate[0]));
        }



        public override bool HasExternalLibraryCall() 
        {
            return this.RightHandSide.HasExternalLibraryCall();
        }
#if DEBUG
        public override void GenerateMSIL(ILGenerator ilGenerator, TypeBuilder typeBuilder)
        {
            RightHandSide.GenerateMSIL(ilGenerator, typeBuilder);

            FieldInfo fb = typeBuilder.GetField(LeftHandSide);
            ilGenerator.Emit(OpCodes.Stsfld, fb);
        }
#endif
    }
}