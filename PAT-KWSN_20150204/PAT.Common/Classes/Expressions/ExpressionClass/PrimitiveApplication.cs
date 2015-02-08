using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using PAT.Common.Classes.CUDDLib;
using PAT.Common.Classes.SemanticModels.LTS.BDD;

namespace PAT.Common.Classes.Expressions.ExpressionClass
{
    public sealed class PrimitiveApplication : Expression
    {
        public const string AND = "&&";
        public const string OR = "||";
        public const string NOT = "!";
        public const string IMPLIES = "=>";

        public const string PLUS = "+";
        public const string MINUS = "-";
        public const string TIMES = "*";
        public const string DIVIDE = "/";
        public const string MOD = "mod";
        public const string NEGATIVE = "~";

        public const string EQUAL = "==";
        public const string NOT_EQUAL = "!=";
        public const string GREATER = ">";
        public const string GREATER_EQUAL = ">=";
        public const string LESS = "<";
        public const string LESS_EQUAL = "<=";

        /// <summary>
        /// To get an element i.th in an array A, we create new expression PrimitiveApplication(ARRAY, new Variable(A), new Variable(i))
        /// </summary>
        public const string ARRAY = ".";
        public const string ARRAYPRIME = ".'";
        public const string MIN = "Min";
        public const string MAX = "Max";

        public string Operator;
        public Expression Argument1;
        public Expression Argument2;

        public PrimitiveApplication(String op, Expression a1)
        {
            ExpressionType = ExpressionType.PrimitiveApplication;
            Operator = op;
            Argument1 = a1;
            this.HasVar = a1.HasVar;
            expressionID = Operator + Argument1.ExpressionID;             
        }

        public PrimitiveApplication(String op, Expression a1, Expression a2)
        {
            ExpressionType = ExpressionType.PrimitiveApplication;
            Operator = op;
            Argument1 = a1;
            Argument2 = a2;
            this.HasVar = a1.HasVar || a2.HasVar;

            if (Operator == MOD)
            {
                expressionID = Argument1.ExpressionID + "%" + Argument2.ExpressionID;
            }
            else
            {
                expressionID = Argument1.ExpressionID + Operator + Argument2.ExpressionID;
            }
        }

        public override String ToString()
        {
            if (Argument2 == null)
            {
                if (Operator == NEGATIVE)
                {
                    return "-(" + Argument1 + ")";
                }
                else
                {
                    return Operator + "(" + Argument1 + ")";
                }
            }
            else
            {
                if (Operator == ARRAY)
                {
                    return Argument1 + "[" + Argument2 + "]";                   
                }
                else if (Operator == MOD)
                {
                    return "(" + Argument1 + " % " + Argument2 + ")";
                }
                else
                {
                    return "(" + Argument1 + " " + Operator + " " + Argument2 + ")";
                }
            }
        }

        //public override String GetID()
        //{
        //    if (Argument2 == null)
        //    {
        //        return Operator + Argument1.GetID();                    
        //    }
        //    else
        //    {
        //        if (Operator == MOD)
        //        {
        //            return Argument1.GetID() + "%" + Argument2.GetID();
        //        }
        //        else 
        //        {
        //            return Argument1.GetID() + Operator + Argument2.GetID();
        //        }
        //    }
        //}

        
        public override List<string> GetVars()
        {           
            if (Operator == ARRAY)
            {
                List<string> vars = new List<string>(16);
                vars.Add(Argument1.ToString());
                if (Argument2 != null)
                {
                    Ultility.Ultility.AddList(vars, this.Argument2.GetVars());
                }
                return vars;
            }
            else
            {
                List<string> vars = this.Argument1.GetVars();
                if (Argument2 != null)
                {
                    Ultility.Ultility.AddList(vars, this.Argument2.GetVars());
                }
                return vars;
            }            
        }

        public override Expression ClearConstant(Dictionary<string, Expression> constMapping)
        {
            if (Argument2 != null)
            {
                return new PrimitiveApplication(Operator, Argument1.ClearConstant(constMapping), Argument2.ClearConstant(constMapping));
            }
            else
            {
                return new PrimitiveApplication(Operator, Argument1.ClearConstant(constMapping));
            }
        }

        /// <summary>
        /// Encode boolean expression
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public override ExpressionBDDEncoding TranslateBoolExpToBDD(Model model)
        {
            if (IsBinaryExpression())
            {
                ExpressionBDDEncoding result = new ExpressionBDDEncoding();
                if (IsBinaryLogicalExpression())
                {
                    result = TranslateLogicalBinaryExpression(model);
                }
                else if (IsRelationalExpression())
                {
                    result = TranslateRelationalBinaryExpression(model);
                }

                return result;
            }
            else if (this.Operator == NOT)
            {
                ExpressionBDDEncoding result = new ExpressionBDDEncoding();

                ExpressionBDDEncoding operandBddEncoding = this.Argument1.TranslateBoolExpToBDD(model);
                result.AddNodeToGuard(CUDD.Function.Not(operandBddEncoding.GuardDDs));

                return result;
            }
            //
            throw new Exception("Unknown operator");
        }

        /// <summary>
        /// Encode arithmetic expression whose returned value is integer
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public override ExpressionBDDEncoding TranslateIntExpToBDD(Model model)
        {
            if (IsBinaryExpression() && IsBinaryNumericalExpression())
            {
                return TranslateNumericalBinaryExpression(model);
            }
            else if (IsBinaryExpression() && IsArrayExpression())
            {
                return TranslateArrayExpression(model);
            }
            else if (!IsBinaryExpression() && this.Operator == NEGATIVE)
            {
                ExpressionBDDEncoding result = new ExpressionBDDEncoding();
                ExpressionBDDEncoding operandBddEncoding = this.Argument1.TranslateIntExpToBDD(model);
                for (int i = 0; i < operandBddEncoding.Count(); i++)
                {
                    result.GuardDDs.Add(operandBddEncoding.GuardDDs[i]);
                    result.ExpressionDDs.Add(CUDD.Function.Minus(CUDD.Constant(0), operandBddEncoding.ExpressionDDs[i]));
                }
                return result;
            }
            else
            {
                ExpressionBDDEncoding operandBddEncoding = this.Argument1.TranslateBoolExpToBDD(model);
                ExpressionBDDEncoding result = new ExpressionBDDEncoding();
                for (int i = 0; i < operandBddEncoding.Count(); i++)
                {
                    result.ExpressionDDs.Add(operandBddEncoding.GuardDDs[i]);
                    result.GuardDDs.Add(CUDD.Constant(1));
                }

                return result;
            }
        }

        /// <summary>
        /// Can support array of array
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private ExpressionBDDEncoding TranslateArrayExpression(Model model)
        {
            if (this.Argument2 is IntConstant)
            {
                int indexValue = ((IntConstant)this.Argument2).Value;
                if(Operator == ARRAY)
                {
                    return new Variable(this.Argument1 + Model.NAME_SEPERATOR + indexValue).TranslateIntExpToBDD(model);
                }
                else
                {
                    return new VariablePrime(this.Argument1 + Model.NAME_SEPERATOR + indexValue).TranslateIntExpToBDD(model);
                }
            }

            ExpressionBDDEncoding indexBddEncoding = this.Argument2.TranslateIntExpToBDD(model);

            //Get the array range
            int min = model.GetArrayRange(this.Argument1.ExpressionID)[0];
            int max = model.GetArrayRange(this.Argument1.ExpressionID)[1];

            ExpressionBDDEncoding result = new ExpressionBDDEncoding();
            //a[i]
            for (int i = min; i <= max; i++)
            {
                for(int j = 0; j < indexBddEncoding.Count(); j++)
                {
                    //index = i & guard of index
                    CUDD.Ref(indexBddEncoding.ExpressionDDs[j], indexBddEncoding.GuardDDs[j]);
                    CUDDNode guard = CUDD.Function.And(CUDD.Function.Equal(indexBddEncoding.ExpressionDDs[j], CUDD.Constant(i)),
                                          indexBddEncoding.GuardDDs[j]);

                    if(guard != CUDD.ZERO)
                    {
                        //a[i]
                        result.GuardDDs.Add(guard);
                        Expression arrayExpression;
                        if(Operator == ARRAY)
                        {
                            arrayExpression = new Variable(this.Argument1.ExpressionID + Model.NAME_SEPERATOR + i);
                        }
                        else
                        {
                            arrayExpression = new VariablePrime(this.Argument1.ExpressionID + Model.NAME_SEPERATOR + i);
                        }
                        result.ExpressionDDs.Add(arrayExpression.TranslateIntExpToBDD(model).ExpressionDDs[0]);
                    }
                    else
                    {
                        CUDD.Deref(guard);
                    }
                }
            }

            //dereference later because 2 loops above
            indexBddEncoding.DeRef();

            return result;
        }

        private ExpressionBDDEncoding TranslateRelationalBinaryExpression(Model model)
        {
            ExpressionBDDEncoding result = new ExpressionBDDEncoding();
            ExpressionBDDEncoding operandTranslation1 = this.Argument1.TranslateIntExpToBDD(model);
            ExpressionBDDEncoding operandTranslation2 = this.Argument2.TranslateIntExpToBDD(model);

            CUDDNode guardOfResult = CUDD.Constant(0);
            for (int i = 0; i < operandTranslation1.Count(); i++)
            {
                for (int j = 0; j < operandTranslation2.Count(); j++)
                {
                    CUDD.Ref(operandTranslation1.GuardDDs[i], operandTranslation2.GuardDDs[j]);
                    CUDDNode guardDD = CUDD.Function.And(operandTranslation1.GuardDDs[i], operandTranslation2.GuardDDs[j]);
                    if (guardDD.Equals(CUDD.ZERO))
                    {
                        CUDD.Deref(guardDD);
                        continue;
                    }
                    CUDDNode expressionDD;
                    CUDD.Ref(operandTranslation1.ExpressionDDs[i], operandTranslation2.ExpressionDDs[j]);
                    switch (this.Operator)
                    {
                        case EQUAL:
                            expressionDD = CUDD.Function.Equal(operandTranslation1.ExpressionDDs[i], operandTranslation2.ExpressionDDs[j]);
                            break;
                        case NOT_EQUAL:
                            expressionDD = CUDD.Function.NotEqual(operandTranslation1.ExpressionDDs[i], operandTranslation2.ExpressionDDs[j]);
                            break;
                        case GREATER:
                            expressionDD = CUDD.Function.Greater(operandTranslation1.ExpressionDDs[i], operandTranslation2.ExpressionDDs[j]);
                            break;
                        case GREATER_EQUAL:
                            expressionDD = CUDD.Function.GreaterEqual(operandTranslation1.ExpressionDDs[i], operandTranslation2.ExpressionDDs[j]);
                            break;
                        case LESS:
                            expressionDD = CUDD.Function.Less(operandTranslation1.ExpressionDDs[i], operandTranslation2.ExpressionDDs[j]);
                            break;
                        case LESS_EQUAL:
                            expressionDD = CUDD.Function.LessEqual(operandTranslation1.ExpressionDDs[i], operandTranslation2.ExpressionDDs[j]);
                            break;
                        default:
                            throw new Exception("Unknown operator");
                    }
                    guardDD = CUDD.Function.And(guardDD, expressionDD);
                    if (guardDD.Equals(CUDD.ZERO))
                    {
                        CUDD.Deref(guardDD);
                        continue;
                    }
                    guardOfResult = CUDD.Function.Or(guardOfResult, guardDD);
                }
            }

            result.AddNodeToGuard(guardOfResult);
            //Remove all old guard, expression
            operandTranslation1.DeRef();
            operandTranslation2.DeRef();
            return result;
        }

        private ExpressionBDDEncoding TranslateLogicalBinaryExpression(Model model)
        {
            ExpressionBDDEncoding result = new ExpressionBDDEncoding();

            ExpressionBDDEncoding operandTranslation1 = this.Argument1.TranslateBoolExpToBDD(model);
            ExpressionBDDEncoding operandTranslation2 = this.Argument2.TranslateBoolExpToBDD(model);

            List<CUDDNode> operand1 = (operandTranslation1.Count() > 0)? operandTranslation1.GuardDDs: new List<CUDDNode>();
            List<CUDDNode> operand2 = (operandTranslation2.Count() > 0) ? operandTranslation2.GuardDDs : new List<CUDDNode>();

            switch (this.Operator)
            {
                case AND:
                    result.GuardDDs = CUDD.Function.And(operand1, operand2);
                    break;
                case OR:
                    result.GuardDDs.AddRange(operand1);
                    result.GuardDDs.AddRange(operand2);
                    break;
                default:
                    throw new Exception("Unknown operator");
            }

            return result;
        }

        private ExpressionBDDEncoding TranslateNumericalBinaryExpression(Model model)
        {
            ExpressionBDDEncoding result = new ExpressionBDDEncoding();
            ExpressionBDDEncoding operandTranslation1 = this.Argument1.TranslateIntExpToBDD(model);
            ExpressionBDDEncoding operandTranslation2 = this.Argument2.TranslateIntExpToBDD(model);

            for (int i = 0; i < operandTranslation1.Count(); i++)
            {
                for (int j = 0; j < operandTranslation2.Count(); j++)
                {
                    CUDD.Ref(operandTranslation1.GuardDDs[i], operandTranslation2.GuardDDs[j]);
                    CUDDNode guard = CUDD.Function.And(operandTranslation1.GuardDDs[i], operandTranslation2.GuardDDs[j]);
                    if (guard.Equals(CUDD.ZERO))
                    {
                        CUDD.Deref(guard);
                        continue;
                    }

                    CUDDNode ex;

                    CUDD.Ref(operandTranslation1.ExpressionDDs[i], operandTranslation2.ExpressionDDs[j]);
                    switch (this.Operator)
                    {
                        case PLUS:
                            ex = CUDD.Function.Plus(operandTranslation1.ExpressionDDs[i], operandTranslation2.ExpressionDDs[j]);
                            break;
                        case MINUS:
                            ex = CUDD.Function.Minus(operandTranslation1.ExpressionDDs[i], operandTranslation2.ExpressionDDs[j]);
                            break;
                        case TIMES:
                            ex = CUDD.Function.Times(operandTranslation1.ExpressionDDs[i], operandTranslation2.ExpressionDDs[j]);
                            break;
                        case DIVIDE:
                            ex = CUDD.Function.Divide(operandTranslation1.ExpressionDDs[i], operandTranslation2.ExpressionDDs[j]);
                            break;
                        case MOD:
                            ex = CUDD.Function.Modulo(operandTranslation1.ExpressionDDs[i], operandTranslation2.ExpressionDDs[j]);
                            break;
                        case MIN:
                            ex = CUDD.Function.Minimum(operandTranslation1.ExpressionDDs[i], operandTranslation2.ExpressionDDs[j]);
                            break;
                        case MAX:
                            ex = CUDD.Function.Maximum(operandTranslation1.ExpressionDDs[i], operandTranslation2.ExpressionDDs[j]);
                            break;
                        default:
                            throw new Exception("Unknown operator");
                    }
                    result.GuardDDs.Add(guard);
                    result.ExpressionDDs.Add(ex);
                }
            }

            //Remove all old guard, expression
            operandTranslation1.DeRef();
            operandTranslation2.DeRef();
            return result;
        }


        public bool IsBinaryExpression()
        {
            return (this.Argument2 != null);
        }

        public bool IsBinaryLogicalExpression()
        {
            return (this.Operator == AND || this.Operator == OR);
        }

        public bool IsBinaryNumericalExpression()
        {
            return (this.Operator == PLUS || this.Operator == MINUS || this.Operator == TIMES ||
                        this.Operator == DIVIDE || this.Operator == MOD  || this.Operator == MIN || this.Operator == MAX);
        }

        public bool IsRelationalExpression()
        {
            return (this.Operator == EQUAL || this.Operator == NOT_EQUAL || this.Operator == GREATER || this.Operator == GREATER_EQUAL ||
                        this.Operator == LESS || this.Operator == LESS_EQUAL);
        }

        public bool IsArrayExpression()
        {
            return (this.Operator == ARRAY) || (this.Operator == ARRAYPRIME);
        }

        public bool IsBooleanExpression()
        {
            return IsBinaryLogicalExpression() || IsRelationalExpression();
        }


        public override bool HasExternalLibraryCall()
        {
            if(IsBinaryExpression())
            {
                return Argument1.HasExternalLibraryCall() || Argument2.HasExternalLibraryCall();
            }
            else
            {
                return Argument1.HasExternalLibraryCall();
            }
        }
#if DEBUG
        public override void GenerateMSIL(ILGenerator ilGenerator, TypeBuilder typeBuilder)
        {
            Argument1.GenerateMSIL(ilGenerator, typeBuilder);

            if (Argument2 != null)
            {
                Argument2.GenerateMSIL(ilGenerator, typeBuilder);
            }

            switch (Operator)
            {
                case AND:
                    ilGenerator.Emit(OpCodes.Mul);
                    break;
                case OR:
                    ilGenerator.Emit(OpCodes.Add);
                    ilGenerator.Emit(OpCodes.Ldc_I4_0);
                    ilGenerator.Emit(OpCodes.Cgt);
                    break;
                case NOT:
                    ilGenerator.Emit(OpCodes.Ldc_I4_0);
                    ilGenerator.Emit(OpCodes.Ceq);
                    break;
                case PLUS:
                    ilGenerator.Emit(OpCodes.Add);
                    break;
                case MINUS:
                    ilGenerator.Emit(OpCodes.Sub);
                    break;
                case TIMES:
                    ilGenerator.Emit(OpCodes.Mul);
                    break;
                case DIVIDE:
                    ilGenerator.Emit(OpCodes.Div);
                    break;
                case MOD:
                    ilGenerator.Emit(OpCodes.Rem);
                    break;
                case NEGATIVE:
                    ilGenerator.Emit(OpCodes.Neg);
                    break;
                case EQUAL:
                    ilGenerator.Emit(OpCodes.Ceq);
                    break;
                case NOT_EQUAL:
                    ilGenerator.Emit(OpCodes.Ceq);
                    ilGenerator.Emit(OpCodes.Ldc_I4_0);
                    ilGenerator.Emit(OpCodes.Ceq);
                    break;
                case GREATER:
                    ilGenerator.Emit(OpCodes.Cgt);
                    break;
                case GREATER_EQUAL:
                    ilGenerator.Emit(OpCodes.Clt);
                    ilGenerator.Emit(OpCodes.Ldc_I4_0);
                    ilGenerator.Emit(OpCodes.Ceq);
                    break;
                case LESS:
                    ilGenerator.Emit(OpCodes.Clt);
                    break;
                case LESS_EQUAL:
                    ilGenerator.Emit(OpCodes.Cgt);
                    ilGenerator.Emit(OpCodes.Ldc_I4_0);
                    ilGenerator.Emit(OpCodes.Ceq);
                    break;
                case ARRAY:
                    ilGenerator.Emit(OpCodes.Ldelem_I4);
                    break;
                case MIN:
                    MethodInfo minFunc = typeof(Math).GetMethod("Min", new Type[] { typeof(int), typeof(int) });
                    ilGenerator.Emit(OpCodes.Call, minFunc);
                    break;
                case MAX:
                    MethodInfo maxFunc = typeof(Math).GetMethod("Max", new Type[] { typeof(int), typeof(int) });
                    ilGenerator.Emit(OpCodes.Call, maxFunc);
                    break;
            }
        }
#endif
    }
}