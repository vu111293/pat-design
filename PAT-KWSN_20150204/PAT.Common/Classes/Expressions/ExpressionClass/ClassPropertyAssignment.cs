using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace PAT.Common.Classes.Expressions.ExpressionClass
{
    public sealed class ClassPropertyAssignment : Expression
    {
        public ClassProperty ClassProperty;
        public Expression RightHandExpression;

        public ClassPropertyAssignment(ClassProperty property, Expression rhs)
        {
            ClassProperty = property;
            RightHandExpression = rhs;
            ExpressionType = ExpressionType.ClassPropertyAssignment;
            HasVar =  ClassProperty.HasVar || rhs.HasVar;
            expressionID = ClassProperty.ExpressionID + "=" + RightHandExpression.ExpressionID;
        }

        public override String ToString()
        {
            return ClassProperty + "=" + RightHandExpression + ";";
        }

        public override List<string> GetVars()
        {
            List<string> vars = ClassProperty.GetVars();
            Ultility.Ultility.AddList(vars, RightHandExpression.GetVars());
            return vars;
        }

        public override Expression ClearConstant(Dictionary<string, Expression> constMapping)
        {
            return new ClassPropertyAssignment(ClassProperty.ClearConstant(constMapping) as ClassProperty, RightHandExpression.ClearConstant(constMapping));        
        }

        public override bool HasExternalLibraryCall()
        {
            return true;
        }

        //public override ExpressionBDDEncoding TranslateExpressionToBDD(Model model)
        //{
        //    ExpressionBDDEncoding result = new ExpressionBDDEncoding();

        //    ExpressionBDDEncoding variableBddEncoding = new PrimitiveApplication(PrimitiveApplication.ARRAY, this.RecordExpression,
        //                                                                            this.PropertyExpression).TranslateExpressionToBDD(model);
        //    ExpressionBDDEncoding valueBddEncoding = this.RightHandExpression.TranslateExpressionToBDD(model);
        //    for (int i = 0; i < variableBddEncoding.Count(); i++)
        //    {
        //        //becaue this is update, so variable should be gotten from column variable
        //        CUDD.Ref(variableBddEncoding.ExpressionDDs[i]);
        //        CUDDNode expressionDD = model.SwapRowColVars(variableBddEncoding.ExpressionDDs[i]);

        //        for (int j = 0; j < valueBddEncoding.Count(); j++)
        //        {
        //            CUDD.Ref(variableBddEncoding.GuardDDs[i], valueBddEncoding.GuardDDs[j]);
        //            CUDDNode guardDD = CUDD.Function.And(variableBddEncoding.GuardDDs[i], valueBddEncoding.GuardDDs[j]);
        //            if (guardDD.Equals(CUDD.ZERO))
        //            {
        //                CUDD.Deref(guardDD);
        //                continue;
        //            }

        //            CUDD.Ref(valueBddEncoding.ExpressionDDs[j], expressionDD);
        //            CUDDNode assignmentDD = CUDD.Function.Equal(valueBddEncoding.ExpressionDDs[j], expressionDD);

        //            guardDD = CUDD.Function.And(guardDD, assignmentDD);
        //            result.AddNodeToGuard(guardDD);
        //        }
        //        CUDD.Deref(expressionDD);
        //    }
        //    //remove unused expression
        //    variableBddEncoding.DeRef();
        //    valueBddEncoding.DeRef();

        //    return result;
        //}


        ///// <summary>
        ///// [ REFS: 'result', DEREFS: 'resultBefore' ]
        ///// </summary>
        //public override ExpressionBDDEncoding TranslateStatementToBDD(ExpressionBDDEncoding resultBefore, Model model)
        //{
        //    ExpressionBDDEncoding newUpdate = this.TranslateExpressionToBDD(model);

        //    ExpressionBDDEncoding tempResult = new ExpressionBDDEncoding();

        //    List<List<int>> updatedVariablesInNewUpdate = new List<List<int>>();
        //    for (int index = 0; index < newUpdate.Count(); index++)
        //    {
        //        updatedVariablesInNewUpdate.Add(model.GetColSupportedVars(newUpdate.GuardDDs[index]));
        //    }

        //    List<List<int>> usedVariableInNewUpdate = new List<List<int>>();
        //    for (int index = 0; index < newUpdate.Count(); index++)
        //    {
        //        usedVariableInNewUpdate.Add(model.GetRowSupportedVars(newUpdate.GuardDDs[index]));
        //    }

        //    List<List<int>> updatedVariablesBefore = new List<List<int>>();
        //    for (int index = 0; index < resultBefore.Count(); index++)
        //    {
        //        updatedVariablesBefore.Add(model.GetColSupportedVars(resultBefore.GuardDDs[index]));
        //    }

        //    for (int j1 = 0; j1 < resultBefore.Count(); j1++)
        //    {
        //        for (int j2 = 0; j2 < newUpdate.Count(); j2++)
        //        {
        //            //a variable is already updated, now is updated again and it also apprears in the value expression
        //            if (Assignment.IsComplexUpdate(updatedVariablesBefore[j1], updatedVariablesInNewUpdate[j2], usedVariableInNewUpdate[j2]))
        //            {
        //                model.CreateTemporaryVar();
        //                Expression newUpdate1;
        //                Expression newUpdate2;

        //                PropertyAssignment assignment = this as PropertyAssignment;
        //                newUpdate1 = new Assignment(Model.TEMPORARY_VARIABLE, this.RightHandExpression);
        //                newUpdate2 = new PropertyAssignment(this.RecordExpression, this.PropertyExpression, new Variable(Model.TEMPORARY_VARIABLE));

        //                resultBefore.Ref();
        //                ExpressionBDDEncoding tempResult1 = newUpdate1.TranslateStatementToBDD(resultBefore, model);
        //                ExpressionBDDEncoding tempResult2 = newUpdate2.TranslateStatementToBDD(tempResult1, model);

        //                //Remove the temporary variable from transition
        //                for (int i = 0; i < tempResult2.Count(); i++)
        //                {
        //                    tempResult2.GuardDDs[i] = CUDD.Abstract.ThereExists(tempResult2.GuardDDs[i], model.GetRowVars(model.GetVarIndex(Model.TEMPORARY_VARIABLE)));
        //                    tempResult2.GuardDDs[i] = CUDD.Abstract.ThereExists(tempResult2.GuardDDs[i], model.GetColVars(model.GetVarIndex(Model.TEMPORARY_VARIABLE)));

        //                    tempResult.AddNodeToGuard(tempResult2.GuardDDs[i]);
        //                }
        //            }
        //            else
        //            {
        //                //swap row, col updated variable in result in the new update command expression
        //                CUDDNode update2 = newUpdate.GuardDDs[j2];
        //                CUDD.Ref(update2);
        //                foreach (int index in updatedVariablesBefore[j1])
        //                {
        //                    if (usedVariableInNewUpdate[j2].Contains(index))
        //                    {
        //                        update2 = CUDD.Variable.SwapVariables(update2, model.GetColVars(index), model.GetRowVars(index));
        //                    }
        //                }

        //                //Restrict updated variable in new update of the old update
        //                CUDDNode update1 = resultBefore.GuardDDs[j1];
        //                CUDD.Ref(update1);
        //                foreach (int index in updatedVariablesInNewUpdate[j2])
        //                {
        //                    if (updatedVariablesBefore[j1].Contains(index))
        //                    {
        //                        update1 = CUDD.Abstract.ThereExists(update1, model.GetColVars(index));
        //                    }
        //                }

        //                CUDDNode transition = CUDD.Function.And(update1, update2);
        //                tempResult.AddNodeToGuard(transition);
        //            }
        //        }
        //    }

        //    resultBefore.DeRef();
        //    newUpdate.DeRef();

        //    return tempResult;
        //}
#if DEBUG
        public override void GenerateMSIL(ILGenerator ilGenerator, TypeBuilder typeBuilder)
        {
            FieldInfo fb = typeBuilder.GetField(ClassProperty.Variable.ExpressionID);
            ilGenerator.Emit(OpCodes.Ldsfld, fb);

            RightHandExpression.GenerateMSIL(ilGenerator, typeBuilder);

            Type type = fb.GetType();
            FieldInfo property = typeBuilder.GetField(ClassProperty.PropertyName);
            ilGenerator.Emit(OpCodes.Stsfld, property);
        }
#endif
    }
}