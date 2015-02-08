using System;
using System.Collections.Generic;
using PAT.ModelChecking.Expressions.DenotationalClass;
using PAT.ModelChecking.Expressions.ExpressionClass;
using Environment=PAT.ModelChecking.Expressions.DenotationalClass.Environment;

namespace PAT.ModelChecking.Expressions
{
    public sealed class EvaluatorDenotational
    {

        private static Value EvalPrimAppl(String op, Value x1, Value x2)
        {
            switch (op)
            {
                case "<":
                    return new BoolValue(((IntValue) x1).Value < ((IntValue) x2).Value);
                case "<=":
                    return new BoolValue(((IntValue)x1).Value <= ((IntValue)x2).Value);
                case ">":
                    return new BoolValue(((IntValue) x1).Value > ((IntValue) x2).Value);
                case ">=":
                    return new BoolValue(((IntValue)x1).Value >= ((IntValue)x2).Value);
                case "==":
                    return new BoolValue(((IntValue) x1).Value == ((IntValue) x2).Value);
                case "!=":
                    return new BoolValue(((IntValue)x1).Value != ((IntValue)x2).Value);
                case "&&":
                    return new BoolValue(((BoolValue) x1).Value && ((BoolValue) x2).Value);
                case "||":
                    return new BoolValue(((BoolValue) x1).Value || ((BoolValue) x2).Value);
                case "!":
                    return new BoolValue(!((BoolValue) x1).Value);
                case "+" :
                    return new IntValue(((IntValue) x1).Value + ((IntValue) x2).Value);
                case "-" :
                    return new IntValue(((IntValue) x1).Value - ((IntValue) x2).Value);
                case "*" :
                    return new IntValue(((IntValue) x1).Value * ((IntValue) x2).Value);
                case "/" :
                    if(((IntValue) x2).Value == 0)
                    {
                        throw new PAT.ModelChecking.Expressions.ExpressionClass.ArithmeticException("Divide by Zero");
                    }
                    else
                    {
                        return new IntValue(((IntValue) x1).Value/((IntValue) x2).Value);
                    }
                case "mod":
                    if (((IntValue)x2).Value == 0)
                    {
                        throw new PAT.ModelChecking.Expressions.ExpressionClass.ArithmeticException("Modulo by Zero");
                    }
                    else
                    {
                        int tmp = ((IntValue)x1).Value;
                        while (tmp < 0)
                        {
                            tmp += ((IntValue)x2).Value;
                        }
                        return new IntValue(tmp % ((IntValue)x2).Value);
                    }                   
                //case "empty" :
                //    return new BoolValue(((RecordValue) x1).Empty);
                //case "hasproperty":
                //    return new BoolValue(((RecordValue)x1).HasProperty(((PropertyValue)x2).PropertyName));
                case ".":
                    return ((RecordValue)x1).Access(((IntValue)x2).Value);
                case "~":
                    return new IntValue(-((IntValue) x1).Value);
                default:
                    object[] paras = null;
                    if (x2 != null)
                    {
                        paras = new object[] { ((IntValue)x1).Value, ((IntValue)x2).Value };
                    }
                    else
                    {
                        paras = new object[] { ((IntValue)x1).Value };
                    }

                    if (Ultility.Ultility.MathMethods.ContainsKey(op + paras.Length))
                    {
                        try
                        {
                            object resultv = Ultility.Ultility.MathMethods[op + paras.Length].Invoke(null, paras);

                            if (resultv is bool)
                            {
                                return new BoolValue((bool)resultv);
                            }
                            else
                            {
                                return new IntValue(Convert.ToInt32(resultv));
                            }
                        }
                        catch (Exception ex)
                        {
                            throw new PAT.ModelChecking.Expressions.ExpressionClass.RuntimeException(ex.Message);
                        }
                    }
                    break;
            }
            
            throw new PAT.ModelChecking.Expressions.ExpressionClass.RuntimeException("Invalid Operator:" + op + "!");
        }

        


        //for assignment attached with events only
        public static Value Evaluate(Expression expression, Environment env, List<string> VariablesToWrite, List<string> VariablesToRead)
        {

            Stack<Expression> workingStack = new Stack<Expression>(16);
            Stack<Value> valueStack = new Stack<Value>(16);

            workingStack.Push(expression);
            bool firstMeet = true;

            while (workingStack.Count > 0)
            {
                Expression expr = workingStack.Pop();
                if(expr == null)
                {
                    firstMeet = false;
                    expr = workingStack.Pop();
                }
                else
                {
                    firstMeet = true;
                }

                switch (expr.ExpressionType)
                {
                    case ExpressionType.Variable:

                        try
                        {
                            string varName = ((Variable)expr).VarName;
                            VariablesToRead.Add(varName);
                            valueStack.Push(env[varName]);
                            break;
                        }
                        catch (KeyNotFoundException)
                        {
                            throw new EvaluatingException("Access the non existing variable: " + (expr as Variable).VarName);
                        }
                        catch (Exception ex)
                        {
                            throw new EvaluatingException("Variable evaluation exception for variable '" + (expr as Variable).VarName + "':" + ex.Message);
                        }


                    case ExpressionType.BoolConstant:
                        valueStack.Push(new BoolValue(((BoolConstant)expr).BoolValue));
                        break;
                    case ExpressionType.IntConstant:
                        valueStack.Push(new IntValue(((IntConstant)expr).IntValue));
                        break;
                    case ExpressionType.Record:

                        int size = ((Record)expr).Associations.Length;

                        if(firstMeet)
                        {
                            workingStack.Push(expr);
                            workingStack.Push(null);
                            Expression[] ass = ((Record)expr).Associations;
                            for (int i = 0; i < size; i++)
                            {
                                workingStack.Push(ass[i]);
                            }    
                        }
                        else
                        {
                            
                            Value[] values = new Value[size];
                            for (int i = 0; i < size; i++)
                            {
                                values[i] = valueStack.Pop();
                            }
                            RecordValue rv = new RecordValue(values);
                            valueStack.Push(rv);
                        }
                        
                        break;
                    case ExpressionType.PrimitiveApplication:
                        
                            PrimitiveApplication newexp = ((PrimitiveApplication) expr);
                             if(firstMeet)
                             {
                                 workingStack.Push(expr);
                                 workingStack.Push(null);
                                 workingStack.Push(newexp.Argument1);
                                 
                                 if (newexp.Argument2 != null)
                                 {
                                     workingStack.Push(newexp.Argument2);
                                 }
                             }
                             else
                             {

                                 Value x1 = valueStack.Pop();
                                 Value x2 = null; 
                                 if (newexp.Argument2 != null)
                                 {
                                     x2 = valueStack.Pop();
                                     if (newexp.Operator == ".")
                                     {
                                         VariablesToRead.Remove(newexp.Argument1.ToString());
                                         VariablesToRead.Add(newexp.Argument1 + "[" + x2 + "]");
                                     }
                                 }
                                 valueStack.Push(EvalPrimAppl(newexp.Operator, x1, x2));
                             }

                            break;
                        
                    case ExpressionType.Assignment:

                            String lhs = ((Assignment)expr).LeftHandSide;
                            if (firstMeet)
                            {
                                workingStack.Push(expr);
                                workingStack.Push(null);

                                //Assign the rhs to lhs                                
                                VariablesToWrite.Add(lhs);

                                workingStack.Push(((Assignment) expr).RightHandSide);                                
                            }
                            else
                            {
                                env[lhs] = valueStack.Pop();               

                            }
                            break;
                    case ExpressionType.PropertyAssignment:
                            PropertyAssignment pa = (PropertyAssignment)expr;

                            if (firstMeet)
                            {                                
                                workingStack.Push(expr);
                                workingStack.Push(null);

                                workingStack.Push(pa.RecordExpression);
                                workingStack.Push(pa.PropertyExpression);
                                workingStack.Push(pa.RightHandExpression);
                            }
                            else
                            {
                                RecordValue rec = (RecordValue)valueStack.Pop();
                                IntValue pro = (IntValue)valueStack.Pop();

                                string tmp = pa.RecordExpression + "[" + pro.Value.ToString() + "]";
                                VariablesToWrite.Add(tmp);
                                VariablesToRead.Remove(pa.RecordExpression.ToString());

                                Value rhs = valueStack.Pop();
                                rec.Values[pro.Value] = rhs;                                
                            }
                        break;
                    case ExpressionType.If:
                        
                            if (firstMeet)
                            {
                                workingStack.Push(expr);
                                workingStack.Push(null);

                                workingStack.Push(((If)expr).Condition);
                            }
                            else
                            {
                                BoolValue cond = valueStack.Pop() as BoolValue;
                                if (cond.Value)
                                {
                                    //return Evaluate(((If)expr).ThenPart, env, VariablesToWrite, VariablesToRead);
                                    workingStack.Push(((If)expr).ThenPart);
                                }
                                else if (((If)expr).ElsePart != null)
                                {
                                    //return Evaluate(((If)expr).ElsePart, env, VariablesToWrite, VariablesToRead);
                                    workingStack.Push(((If)expr).ElsePart);
                                }
                            }
                            break;
                        
                    case ExpressionType.Sequence:
                        workingStack.Push(((Sequence)expr).FirstPart);
                        workingStack.Push(((Sequence)expr).SecondPart);                        
                        break;
                    case ExpressionType.While:

                        if (firstMeet)
                        {
                            workingStack.Push(expr);
                            workingStack.Push(null);

                            workingStack.Push(((While) expr).Test);
                        }
                        else
                        {
                            BoolValue cond = valueStack.Pop() as BoolValue;
                            if(cond.Value)
                            {
                                workingStack.Push(expr);
                                workingStack.Push(((While)expr).Body);
                                
                            }
                        }
                        break;                    
                }
            }

            if (valueStack.Count > 0)
            {
                return valueStack.Pop();
            }
            else
            {
                return null;
            }
        }

        //for assignment attached with events only
        public static Value Evaluate(Expression expression, Environment env)
        {

            Stack<Expression> workingStack = new Stack<Expression>(16);
            Stack<Value> valueStack = new Stack<Value>(16);

            workingStack.Push(expression);
            bool firstMeet = true;

            while (workingStack.Count > 0)
            {
                Expression expr = workingStack.Pop();
                if (expr == null)
                {
                    firstMeet = false;
                    expr = workingStack.Pop();
                }
                else
                {
                    firstMeet = true;
                }

                switch (expr.ExpressionType)
                {
                    case ExpressionType.Variable:

                        try
                        {
                            string varName = ((Variable)expr).VarName;
                            valueStack.Push(env[varName]);
                            break;
                        }
                        catch (KeyNotFoundException)
                        {
                            throw new EvaluatingException("Access the non existing variable: " + (expr as Variable).VarName);
                        }
                        catch (Exception ex)
                        {
                            throw new EvaluatingException("Variable evaluation exception for variable '" + (expr as Variable).VarName + "':" + ex.Message);
                        }


                    case ExpressionType.BoolConstant:
                        valueStack.Push(new BoolValue(((BoolConstant)expr).BoolValue));
                        break;
                    case ExpressionType.IntConstant:
                        valueStack.Push(new IntValue(((IntConstant)expr).IntValue));
                        break;
                    case ExpressionType.Record:

                        int size = ((Record)expr).Associations.Length;

                        if (firstMeet)
                        {
                            workingStack.Push(expr);
                            workingStack.Push(null);
                            Expression[] ass = ((Record)expr).Associations;
                            for (int i = 0; i < size; i++)
                            {
                                workingStack.Push(ass[i]);
                            }
                        }
                        else
                        {

                            Value[] values = new Value[size];
                            for (int i = 0; i < size; i++)
                            {
                                values[i] = valueStack.Pop();
                            }
                            RecordValue rv = new RecordValue(values);
                            valueStack.Push(rv);
                        }

                        break;
                    case ExpressionType.PrimitiveApplication:

                        PrimitiveApplication newexp = ((PrimitiveApplication)expr);
                        if (firstMeet)
                        {
                            workingStack.Push(expr);
                            workingStack.Push(null);
                            workingStack.Push(newexp.Argument1);

                            if (newexp.Argument2 != null)
                            {
                                workingStack.Push(newexp.Argument2);
                            }
                        }
                        else
                        {

                            Value x1 = valueStack.Pop();
                            Value x2 = null;
                            if (newexp.Argument2 != null)
                            {
                                x2 = valueStack.Pop();                                
                            }
                            valueStack.Push(EvalPrimAppl(newexp.Operator, x1, x2));
                        }

                        break;

                    case ExpressionType.Assignment:

                        String lhs = ((Assignment)expr).LeftHandSide;
                        if (firstMeet)
                        {
                            workingStack.Push(expr);
                            workingStack.Push(null);

                            workingStack.Push(((Assignment)expr).RightHandSide);
                        }
                        else
                        {
                            env[lhs] = valueStack.Pop();

                        }
                        break;
                    case ExpressionType.PropertyAssignment:
                        PropertyAssignment pa = (PropertyAssignment)expr;

                        if (firstMeet)
                        {
                            workingStack.Push(expr);
                            workingStack.Push(null);

                            workingStack.Push(pa.RecordExpression);
                            workingStack.Push(pa.PropertyExpression);
                            workingStack.Push(pa.RightHandExpression);
                        }
                        else
                        {
                            RecordValue rec = (RecordValue)valueStack.Pop();
                            IntValue pro = (IntValue)valueStack.Pop();
                            
                            Value rhs = valueStack.Pop();
                            rec.Values[pro.Value] = rhs;
                        }
                        break;
                    case ExpressionType.If:

                        if (firstMeet)
                        {
                            workingStack.Push(expr);
                            workingStack.Push(null);

                            workingStack.Push(((If)expr).Condition);
                        }
                        else
                        {
                            BoolValue cond = valueStack.Pop() as BoolValue;
                            if (cond.Value)
                            {
                                //return Evaluate(((If)expr).ThenPart, env, VariablesToWrite, VariablesToRead);
                                workingStack.Push(((If)expr).ThenPart);
                            }
                            else if (((If)expr).ElsePart != null)
                            {
                                //return Evaluate(((If)expr).ElsePart, env, VariablesToWrite, VariablesToRead);
                                workingStack.Push(((If)expr).ElsePart);
                            }
                        }
                        break;

                    case ExpressionType.Sequence:
                        workingStack.Push(((Sequence)expr).FirstPart);
                        workingStack.Push(((Sequence)expr).SecondPart);
                        break;
                    case ExpressionType.While:

                        if (firstMeet)
                        {
                            workingStack.Push(expr);
                            workingStack.Push(null);

                            workingStack.Push(((While)expr).Test);
                        }
                        else
                        {
                            BoolValue cond = valueStack.Pop() as BoolValue;
                            if (cond.Value)
                            {
                                workingStack.Push(expr);
                                workingStack.Push(((While)expr).Body);

                            }
                        }
                        break;
                }
            }

            if (valueStack.Count > 0)
            {
                return valueStack.Pop();
            }
            else
            {
                return null;
            }
        }
    }
}