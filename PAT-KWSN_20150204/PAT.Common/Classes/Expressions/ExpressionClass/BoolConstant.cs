using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using PAT.Common.Classes.SemanticModels.LTS.BDD;
using PAT.Common.Classes.CUDDLib;

namespace PAT.Common.Classes.Expressions.ExpressionClass
{
    public sealed class BoolConstant : ExpressionValue
    {
        public bool Value;
        public string Const;

        public BoolConstant(bool v)
        {
            Value = v;
            expressionID = Value ? "T" : "F";
        }

        public BoolConstant(bool v, string constName)
        {
            Value = v;
            Const = constName;
            expressionID = Value ? "T" : "F";
        }

        public override String ToString()
        {
            if (Const == null)
            {
                return Value ? "true" : "false"; ;            
            }

            return Const;             
        }

        public override ExpressionValue GetClone()
        {
            return this;
        }

        //public override string GetID()
        //{
        //    return ExpressionID; // Value ? "T" : "F";           
        //}


        /// 
        /// <summary>
        /// Only used in boolean expression (and, or, not): true & a = 7
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public override ExpressionBDDEncoding TranslateBoolExpToBDD(Model model)
        {
            ExpressionBDDEncoding result = new ExpressionBDDEncoding();
            if (Value)
            {
                result.GuardDDs.Add(CUDD.Constant(1));
            }
            return result;

        }

        /// 
        /// <summary>
        /// Use when the boolen constant is used as expression: a = true
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public override ExpressionBDDEncoding TranslateIntExpToBDD(Model model)
        {
            ExpressionBDDEncoding result = new ExpressionBDDEncoding();
            result.GuardDDs.Add(CUDD.Constant(1));
            if (Value)
            {
                result.ExpressionDDs.Add(CUDD.Constant(1));
            }
            else
            {
                result.ExpressionDDs.Add(CUDD.Constant(0));
            }
            return result;

        }

        /// <summary>
        /// Used as a special statement which does nothing
        /// </summary>
        public override ExpressionBDDEncoding TranslateStatementToBDD(ExpressionBDDEncoding resultBefore, Model model)
        {
            if (Value)
            {
                return resultBefore;
            }
            else
            {
                return new ExpressionBDDEncoding();
            }
        }
#if DEBUG
        public override void GenerateMSIL(ILGenerator ilGenerator, TypeBuilder typeBuilder)
        {
            if (Value)
            {
                ilGenerator.Emit(OpCodes.Ldc_I4_1);

            }
            else
            {
                ilGenerator.Emit(OpCodes.Ldc_I4_0);

            }
        }
#endif
    }
}