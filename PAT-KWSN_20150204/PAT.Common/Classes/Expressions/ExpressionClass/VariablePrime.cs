using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PAT.Common.Classes.SemanticModels.LTS.BDD;
using PAT.Common.Classes.CUDDLib;

namespace PAT.Common.Classes.Expressions.ExpressionClass
{
    /// <summary>
    /// This class is used for BDD to encode new variable value after transition
    /// </summary>
    public sealed class VariablePrime : Expression
    {
        public VariablePrime(String name)
        {
            //VarName = nam;
            HasVar = true;
            ExpressionType = ExpressionType.Variable;
            expressionID = name;
        }

        public override String ToString()
        {
            return expressionID;
        }

        //public override String GetID()
        //{
        //    return VarName;
        //}

        public override List<string> GetVars()
        {
            List<string> vars = new List<string>(1);
            vars.Add(expressionID);
            return vars;
        }

        public override Expression ClearConstant(Dictionary<string, Expression> constMapping)
        {
            if (constMapping.ContainsKey(expressionID))
            {
                return constMapping[expressionID];
            }

            return new VariablePrime(expressionID);
        }

        /// <summary>
        /// Encode as booleane expression, check whether the variable is true
        /// </summary>
        public override ExpressionBDDEncoding TranslateBoolExpToBDD(Model model)
        {
            return Expression.EQ(new VariablePrime(this.expressionID), new BoolConstant(true)).TranslateBoolExpToBDD(model);
        }

        /// <summary>
        /// Return variable expression
        /// </summary>
        public override ExpressionBDDEncoding TranslateIntExpToBDD(Model model)
        {
            ExpressionBDDEncoding result = new ExpressionBDDEncoding();
            result.GuardDDs.Add(CUDD.Constant(1));

            int variableIndex = model.GetVarIndex(this.expressionID);
            CUDDNode varDD = model.variableEncoding[variableIndex];
            CUDD.Ref(varDD);

            CUDDVars rowVars = model.GetRowVars(variableIndex);
            CUDDVars colVars = model.GetColVars(variableIndex);
            varDD = CUDD.Variable.SwapVariables(varDD, rowVars, colVars);

            result.ExpressionDDs.Add(varDD);

            return result;
        }
    }
}
