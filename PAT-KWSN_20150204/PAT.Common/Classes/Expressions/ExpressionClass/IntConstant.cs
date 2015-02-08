using System;
using System.Reflection.Emit;
using PAT.Common.Classes.SemanticModels.LTS.BDD;
using PAT.Common.Classes.CUDDLib;

namespace PAT.Common.Classes.Expressions.ExpressionClass
{
    public sealed class IntConstant : ExpressionValue
    {
        public int Value;
        public string Const;

        public IntConstant(int v)
        {
            Value = v;
            expressionID = Value.ToString();
        }

        public IntConstant(int v, string constName)
        {
            Value = v;
            Const = constName;
            expressionID = Value.ToString();
        }

        public override String ToString()
        {
            if (Const == null)
            {
                return Value.ToString();
            }
            return Const;            
        }

        public override ExpressionValue GetClone()
        {
            return this;
        }

        //public override string GetID()
        //{
        //    return Value.ToString();
        //}

        /// <summary>
        /// Return the encoding of the integer value
        /// </summary>
        public override ExpressionBDDEncoding TranslateIntExpToBDD(Model model)
        {
            ExpressionBDDEncoding result = new ExpressionBDDEncoding();
            result.GuardDDs.Add(CUDD.Constant(1));
            result.ExpressionDDs.Add(CUDD.Constant(Value));
            return result;
        }
#if DEBUG
        public override void GenerateMSIL(ILGenerator ilGenerator, TypeBuilder typeBuilder)
        {
            switch (Value)
            {
                case 0:
                    ilGenerator.Emit(OpCodes.Ldc_I4_0);
                    break;
                case 1:
                    ilGenerator.Emit(OpCodes.Ldc_I4_1);
                    break;
                case 2:
                    ilGenerator.Emit(OpCodes.Ldc_I4_2);
                    break;
                case 3:
                    ilGenerator.Emit(OpCodes.Ldc_I4_3);
                    break;
                case 4:
                    ilGenerator.Emit(OpCodes.Ldc_I4_4);
                    break;
                case 5:
                    ilGenerator.Emit(OpCodes.Ldc_I4_5);
                    break;
                case 6:
                    ilGenerator.Emit(OpCodes.Ldc_I4_6);
                    break;
                case 7:
                    ilGenerator.Emit(OpCodes.Ldc_I4_7);
                    break;
                case 8:
                    ilGenerator.Emit(OpCodes.Ldc_I4_8);
                    break;
                default:
                    ilGenerator.Emit(OpCodes.Ldc_I4, Value);
                    break;
            }
        }
#endif
    }
}