using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using PAT.Common.Classes.SemanticModels.LTS.BDD;

namespace PAT.Common.Classes.Expressions.ExpressionClass
{
    public sealed class LetDefinition : Expression
    {
        public string Variable;
        public Expression RightHandExpression;
       
        public LetDefinition(String v, Expression rhe) 
        {
            Variable = v; 
            RightHandExpression = rhe;
            HasVar = true;
            ExpressionType = ExpressionType.Let;

            expressionID = Variable + "=" + RightHandExpression.ExpressionID;

        }

        public override String ToString()
        {
            return "var " +Variable + " = " + RightHandExpression + ";";
        }

        //public override String GetID()
        //{
        //    return Variable + "=" + RightHandExpression.GetID();
        //}

        public override List<string> GetVars()
        {
            List<string> vars = this.RightHandExpression.GetVars();
            if (!vars.Contains(Variable))
            {
                vars.Add(Variable);
            }
            return vars;
        }

        public override Expression ClearConstant(Dictionary<string, Expression> constMapping)
        {
            return new LetDefinition(Variable, RightHandExpression.ClearConstant(constMapping));
        }

        public override bool HasExternalLibraryCall()
        {
            return RightHandExpression.HasExternalLibraryCall();
        }

        /// <summary>
        /// Create a new local variable
        /// </summary>
        public override ExpressionBDDEncoding TranslateStatementToBDD(ExpressionBDDEncoding resultBefore, Model model)
        {
            //Create new local variable if not exists
            if (!model.ContainsVar(this.Variable))
            {
                model.AddLocalVar(this.Variable, Model.BDD_INT_LOWER_BOUND, Model.BDD_INT_UPPER_BOUND);
            }

            //If there is an initialization then do it based on the resultBefore
            if (this.RightHandExpression == null)
            {
                return resultBefore;
            }
            else
            {
                return (new Assignment(this.Variable, RightHandExpression)).TranslateStatementToBDD(resultBefore, model);
            }
        }
#if DEBUG
        public override void GenerateMSIL(ILGenerator ilGenerator, TypeBuilder typeBuilder)
        {
            LocalBuilder newVar = ilGenerator.DeclareLocal(typeof (int));
            newVar.SetLocalSymInfo(Variable);

            RightHandExpression.GenerateMSIL(ilGenerator, typeBuilder);

            ilGenerator.Emit(OpCodes.Stloc, newVar);
        }
#endif
    }
}