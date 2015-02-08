using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using PAT.Common.Classes.Expressions.ExpressionClass;
using PAT.Common.Classes.LTS;
using ArithmeticException=PAT.Common.Classes.Expressions.ExpressionClass.ArithmeticException;


namespace PAT.Common.Classes.Expressions
{
    public sealed class EvaluatorDenotational
    {
        private static ExpressionValue EvalPrimAppl(PrimitiveApplication application, ExpressionValue x1, Expression x2Exp, Valuation env)
        {
            try
            {
                ExpressionValue x2;
                switch (application.Operator)
                {
                    case "<":
                        x2 = Evaluate(x2Exp, env);
                        return new BoolConstant(((IntConstant)x1).Value < ((IntConstant)x2).Value);
                    case "<=":
                        x2 = Evaluate(x2Exp, env);
                        return new BoolConstant(((IntConstant)x1).Value <= ((IntConstant)x2).Value);
                    case ">":
                        x2 = Evaluate(x2Exp, env);
                        return new BoolConstant(((IntConstant)x1).Value > ((IntConstant)x2).Value);
                    case ">=":
                        x2 = Evaluate(x2Exp, env);
                        return new BoolConstant(((IntConstant)x1).Value >= ((IntConstant)x2).Value);
                    case "==":
                        x2 = Evaluate(x2Exp, env);
                        return new BoolConstant(x1.ExpressionID == x2.ExpressionID);
                    case "!=":
                        x2 = Evaluate(x2Exp, env);
                        //return new BoolConstant(((IntConstant)x1).Value != ((IntConstant)x2).Value);
                        return new BoolConstant(x1.ExpressionID != x2.ExpressionID);
                    case "&&":
                        if(((BoolConstant)x1).Value)
                        {
                            x2 = Evaluate(x2Exp, env);
                            return new BoolConstant(((BoolConstant)x2).Value);
                        }
                        else
                        {
                            return new BoolConstant(false);
                        }                        
                    case "||":
                        if (!((BoolConstant)x1).Value)
                        {
                            x2 = Evaluate(x2Exp, env);
                            return new BoolConstant(((BoolConstant)x2).Value);
                        }
                        else
                        {
                            return new BoolConstant(true);
                        }
                    case "xor":                      
                            x2 = Evaluate(x2Exp, env);
                            return new BoolConstant(((BoolConstant)x1).Value ^ ((BoolConstant)x2).Value);                        
                    case "!":
                        return new BoolConstant(!((BoolConstant)x1).Value);
                    case "+":
                        x2 = Evaluate(x2Exp, env);
                        return new IntConstant(((IntConstant)x1).Value + ((IntConstant)x2).Value);
                    case "-":
                        x2 = Evaluate(x2Exp, env);
                        return new IntConstant(((IntConstant)x1).Value - ((IntConstant)x2).Value);
                    case "*":
                        x2 = Evaluate(x2Exp, env);
                        return new IntConstant(((IntConstant)x1).Value * ((IntConstant)x2).Value);
                    case "/":
                        x2 = Evaluate(x2Exp, env);
                        if (((IntConstant)x2).Value == 0)
                        {
                            throw new ArithmeticException("Divide by Zero on " + application.ToString());
                        }
                        else
                        {
                            return new IntConstant(((IntConstant)x1).Value / ((IntConstant)x2).Value);
                        }
                    case "mod":
                        x2 = Evaluate(x2Exp, env);
                        if (((IntConstant)x2).Value == 0)
                        {
                            throw new ArithmeticException("Modulo by Zero on " + application.ToString());
                        }
                        else
                        {
                            int int_X1 = ((IntConstant) x1).Value;
                            int int_X2 = ((IntConstant) x2).Value;

                            int tmp = int_X1 % int_X2;

                            return new IntConstant((tmp >= 0) ? tmp : (tmp + int_X2));
                        }
                    //case "empty" :
                    //    return new Value(((RecordValue) x1).Empty);
                    //case "hasproperty":
                    //    return new Value(((RecordValue)x1).HasProperty(((PropertyValue)x2).PropertyName));
                    case ".":
                        RecordValue record = (RecordValue)x1;
                        x2 = Evaluate(x2Exp, env);
                        int index = ((IntConstant)x2).Value;
                        if (index < 0)
                        {
                            throw new NegativeArraySizeException("Access negative index " + index + " for variable " + application.Argument1.ToString() + " in expression " + application.ToString());
                        }
                        else if (index >= record.Associations.Length)
                        {
                            throw new IndexOutOfBoundsException("Index " + index + " is out of range for variable " + application.Argument1.ToString() + " in expression " + application.ToString());
                        }

                        return record.Associations[index];
                    case "~":
                        return new IntConstant(-((IntConstant)x1).Value);
                    
                    //Bitwise operators used by NESC module
                    case "<<"://bitwise left shift
                        x2 = Evaluate(x2Exp, env);
                        return new IntConstant(((IntConstant)x1).Value << ((IntConstant)x2).Value);
                    case ">>"://bitwise right shift
                        x2 = Evaluate(x2Exp, env);
                        return new IntConstant(((IntConstant)x1).Value >> ((IntConstant)x2).Value);
                    case "&"://bitwise AND
                        x2 = Evaluate(x2Exp, env);
                        return new IntConstant(((IntConstant)x1).Value & ((IntConstant)x2).Value);
                    case "^"://bitwise XOR
                        x2 = Evaluate(x2Exp, env);
                        return new IntConstant(((IntConstant)x1).Value ^ ((IntConstant)x2).Value);
                    case "|"://bitwise OR
                        x2 = Evaluate(x2Exp, env);
                        return new IntConstant(((IntConstant)x1).Value | ((IntConstant)x2).Value);

                }

            }
            catch (InvalidCastException ex)
            {
                throw new RuntimeException("Invalid Cast Exception for " + application.ToString() + ": " + ex.Message.Replace("PAT.Common.Classes.Expressions.ExpressionClass.", ""));
            }
            //catch (Exception ex1)
            //{
            //    throw new RuntimeException("Invalid Primitive Operation: " + application.ToString() + "!");
            //}

            throw new RuntimeException("Invalid Primitive Operation: " +application.ToString() + "!");
        }

        public static object GetValueFromExpression(ExpressionValue x1)
        {
            if (x1 is IntConstant)
            {
                return ((IntConstant) x1).Value;
            }
            else if (x1 is RecordValue)
            {
                ExpressionValue[] vals = ((RecordValue) x1).Associations;
                int[] values = new int[vals.Length];
                for (int i = 0; i < vals.Length; i++)
                {
                    values[i] = ((IntConstant)vals[i]).Value;
                }
                return values;
            }
            else if (x1 is BoolConstant)
            {
                return ((BoolConstant)x1).Value;
            }
            else if (x1 is NullConstant)
            {
                return null;
            }

            return x1;
            
            //else
            //{
            //    throw new RuntimeException("Call expression only accept int, boolean or array variables.");
            //}
        }

        // evaluate starts evaluation with fresh environment
        public static ExpressionValue Evaluate(Expression exp, Valuation env)
        {
            switch (exp.ExpressionType)
            {
               
                case ExpressionType.Constant:
                    return exp as ExpressionValue;

                case ExpressionType.Variable:
                    // Look up variable in environment; we assume
                    // that value is found
                    try
                    {
                        return env.Variables[exp.expressionID];
                    }
                    catch (KeyNotFoundException)
                    {
                        throw new EvaluatingException("Access the non existing variable: " + exp.expressionID);
                    }
                    catch (Exception ex)
                    {
                        throw new EvaluatingException("Variable evaluation exception for variable '" + exp.expressionID + "':" + ex.Message);
                    }
                case ExpressionType.Record:
                    {
                        Expression[] ass = ((Record)exp).Associations;
                        ExpressionValue[] values = new ExpressionValue[ass.Length];

                        for (int i = 0; i < ass.Length; i++)
                        {
                            //rv.Put(association.Property, store.Extend(Eval(association.Expression, env)));
                            //rv.Put(Eval(association, env));
                            values[i] = Evaluate(ass[i], env);
#if !OPTIMAL_FOR_EXP
                            if(values[i] == null)
                            {
                                throw new RuntimeException("Invalid expression assignment: " + exp);
                            }
#endif

                        }
                        RecordValue rv = new RecordValue(values);
                        return rv;
                    }
                case ExpressionType.PrimitiveApplication:
                    // First evaluate the first argument, then the second, and
                    // then evaluate using evalPrimAppl.
                    {
                        PrimitiveApplication newexp = exp as PrimitiveApplication;

                        ExpressionValue x1 = Evaluate(newexp.Argument1, env);
                        Debug.Assert(x1 != null);
#if !OPTIMAL_FOR_EXP
                        if (x1 == null)
                        {
                            throw new RuntimeException("Invalid expression assignment: " + exp);
                        }
#endif                        
                        return EvalPrimAppl(newexp, x1, newexp.Argument2, env);
                    }
                case ExpressionType.Assignment:
                    {
                        //Assign the rhs to lhs
                        String lhs = ((Assignment) exp).LeftHandSide;
                        Expression rhs = ((Assignment) exp).RightHandSide;
                        ExpressionValue rhsV = Evaluate(rhs, env);
#if !OPTIMAL_FOR_EXP                        
                        if (rhsV == null)
                        {
                            throw new RuntimeException("Invalid expression assignment: " + exp);
                        }

                        Valuation.CheckVariableRange(lhs, rhsV);
#endif
                       
                        env.Variables[lhs] = rhsV;
                        return rhsV;
                    }
                case ExpressionType.PropertyAssignment:
                    {
                        try
                        {
                            PropertyAssignment pa = (PropertyAssignment) exp;
                            RecordValue rec = (RecordValue) Evaluate(pa.RecordExpression, env);
                            IntConstant pro = (IntConstant) Evaluate(pa.PropertyExpression, env);
                            ExpressionValue rhs = Evaluate(pa.RightHandExpression, env);

                            //rec.Put(pro.PropertyName, store.Extend(rhs));
                            int index = pro.Value;
                            if (index < 0)
                            {
                                throw new NegativeArraySizeException("Access negative index " + index + " for variable " + pa.RecordExpression.ToString());
                            }
                            else if (index >= rec.Associations.Length)
                            {
                                throw new IndexOutOfBoundsException("Index " + index + " is out of range for variable " + pa.RecordExpression.ToString());
                            }
#if !OPTIMAL_FOR_EXP
                            if (rhs == null)
                            {
                                throw new RuntimeException("Invalid expression assignment: " + exp);
                            }

                            Valuation.CheckVariableRange(pa.RecordExpression.ToString(), rhs);
#endif

                            rec.Associations[index] = rhs;

                            //Note:Important!!! must recalculate the ID here, otherwise ID is obsolete and the verification result is wrong
                            rec.GetID();

                            return rhs;
                        }
                        catch (InvalidCastException ex)
                        {
                            throw new RuntimeException("Invalid Cast Exception for " + exp + ": " + ex.Message.Replace("PAT.Common.Classes.Expressions.ExpressionClass.", ""));
                        }
                    }
                case ExpressionType.If:
                    // Conditionals are evaluated by evaluating the then-part or 
                    // else-part depending of the result of evaluating the condition.
                    {
                        ExpressionValue cond = Evaluate(((If)exp).Condition, env);
                        if (((BoolConstant) cond).Value)
                        {
                            return Evaluate(((If) exp).ThenPart, env);
                        }
                        else if (((If)exp).ElsePart != null)
                        {
                            return Evaluate(((If) exp).ElsePart, env);
                        }
                        else
                        {
                            return null;
                        }
                    }
                case ExpressionType.Sequence:

                    // firstPart;secondPart
                    Expression fP = ((Sequence) exp).FirstPart;
                    Expression sP = ((Sequence) exp).SecondPart;

                    Evaluate(fP, env);
                    return Evaluate(sP, env);

                case ExpressionType.While:

                    Expression test = ((While) exp).Test;
                    Expression body = ((While) exp).Body;

                    // the value of test may not be a Value.
                    // here we assume it is always a Value, which
                    // may cause run time exception due to non-Value. 
                    if (((BoolConstant) Evaluate(test, env)).Value)
                    {
                        // test is ture
                        Evaluate(body, env); // body serves to change the store	
                        return Evaluate(exp, env); // evaluate the While again 
                    }
                    else
                    {
                        return null;
                    }
                case ExpressionType.StaticMethodCall:
                    try
                    {
                        StaticMethodCall methodCall = (StaticMethodCall)exp;

                        if (methodCall.Arguments.Length > 0)
                        {
                            ChannelQueue queue;
                            string cname = null;

                            if ((methodCall.Arguments[0] is Variable))
                            {
                                cname = (methodCall.Arguments[0] as Variable).ExpressionID;
                            }
                            else if (methodCall.Arguments[0] is PrimitiveApplication)
                            {
                                PrimitiveApplication pa = (methodCall.Arguments[0] as PrimitiveApplication);
                                ExpressionValue ind = Evaluate(pa.Argument2, env);
                                cname = pa.Argument1 + "[" + ind + "]";
                            }


                            switch (methodCall.MethodName)
                            {
                                case Common.Classes.Ultility.Constants.cfull:
                                    if (env.Channels.TryGetValue(cname, out queue))
                                    {
                                        return new BoolConstant(queue.IsFull());
                                    }
                                    else
                                    {
                                        throw new RuntimeException("Channel " + cname +
                                                                   " is not used in the model. Therefore it is meaningless to query channel information using " +
                                                                   methodCall + ".");
                                    }

                                case Common.Classes.Ultility.Constants.cempty:

                                    if (env.Channels.TryGetValue(cname, out queue))
                                    {
                                        return new BoolConstant(queue.IsEmpty());
                                    }
                                    else
                                    {
                                        throw new RuntimeException("Channel " + cname +
                                                                   " is not used in the model. Therefore it is meaningless to query channel information using " +
                                                                   methodCall + ".");
                                    }

                                case Common.Classes.Ultility.Constants.ccount:
                                    if (env.Channels.TryGetValue(cname, out queue))
                                    {
                                        return new IntConstant(queue.Count);
                                    }
                                    else
                                    {
                                        throw new RuntimeException("Channel " + cname +
                                                                   " is not used in the model. Therefore it is meaningless to query channel information using " +
                                                                   methodCall + ".");
                                    }


                                case Common.Classes.Ultility.Constants.csize:
                                    if (env.Channels.TryGetValue(cname, out queue))
                                    {
                                        return new IntConstant(queue.Size);
                                    }
                                    else
                                    {
                                        throw new RuntimeException("Channel " + cname +
                                                                   " is not used in the model. Therefore it is meaningless to query channel information using " +
                                                                   methodCall + ".");
                                    }

                                case Common.Classes.Ultility.Constants.cpeek:
                                    if (env.Channels.TryGetValue(cname, out queue))
                                    {
                                        if (queue.Count == 0)
                                        {
                                            throw new IndexOutOfBoundsException("Channel " + cname +
                                                                                "'s buffer is empty!");
                                        }

                                        return new RecordValue(queue.Peek());
                                    }
                                    else
                                    {
                                        throw new RuntimeException("Channel " + cname +
                                                                   " is not used in the model. Therefore it is meaningless to query channel information using " +
                                                                   methodCall + ".");
                                    }
                            }
                        }

                        object[] paras = new object[methodCall.Arguments.Length];
                        for (int i = 0; i < paras.Length; i++)
                        {
                            ExpressionValue x1 = Evaluate(methodCall.Arguments[i], env);
                            paras[i] = GetValueFromExpression(x1);
                        }

                        string key = methodCall.MethodName + paras.Length;
                        if (Common.Ultility.Ultility.CSharpMethods.ContainsKey(key))
                        {
                            object resultv = Common.Ultility.Ultility.CSharpMethods[key].Invoke(null, paras);

                            if (Common.Ultility.Ultility.CSharpMethods[key].ReturnType.Name == "Void")
                            {
                                return null;
                            }

                            if (resultv is bool)
                            {
                                return new BoolConstant((bool)resultv);
                            }
                            else if (resultv is int || resultv is short || resultv is byte || resultv is double)
                            {
                                return new IntConstant(Convert.ToInt32(resultv));
                            }
                            else if (resultv is int[])
                            {
                                int[] list = resultv as int[];
                                ExpressionValue[] vals = new ExpressionValue[list.Length];

                                for (int i = 0; i < vals.Length; i++)
                                {
                                    vals[i] = new IntConstant(list[i]);
                                }
                                return new RecordValue(vals);
                            }
                            else if (resultv is ExpressionValue)
                            {
                                return resultv as ExpressionValue;
                            }

                            return new NullConstant();
                            //the following check is not necessary, since we will only keep bool, int and int[] methods
                            //else
                            //{
                            //     throw new Expressions.ExpressionClass.RuntimeException("Call expression can only return int, short, byte, bool or int[] types. Please check your methods.");
                            //}
                        }

                        throw new RuntimeException("Invalid Method Call: " + methodCall + "! Make sure you have defined the method in the library.");

                    }
                    catch(TargetInvocationException ex)
                    {
                        if (ex.InnerException != null)
                        {
                            RuntimeException exception =
                                new RuntimeException("Exception happened at expression " + exp + ": " +
                                                     ex.InnerException.Message);
                            exception.InnerStackTrace = ex.InnerException.StackTrace;
                            throw exception;
                        }
                        else
                        {
                            throw new RuntimeException("Exception happened at expression " + exp + ": " + ex.Message);
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new RuntimeException("Exception happened at expression " + exp + ": " + ex.Message);
                    }
                case ExpressionType.ClassMethodCall:
                    try
                    {
                        ClassMethodCall methodCall = (ClassMethodCall)exp;
                        ExpressionValue variable = env.Variables[methodCall.Variable];

                        if (variable == null)
                        {
                            throw new RuntimeException("Exception happened at expression " + exp + ": variable " + methodCall.Variable + "'s value is null");
                        }

                        object[] paras = new object[methodCall.Arguments.Length];
                        for (int i = 0; i < paras.Length; i++)
                        {
                            ExpressionValue x1 = Evaluate(methodCall.Arguments[i], env);
                            paras[i] = GetValueFromExpression(x1);
                        }

                        MethodInfo methodInfo = variable.GetType().GetMethod(methodCall.MethodName);

                        if (methodInfo != null)
                        {
                            object resultv = methodInfo.Invoke(variable, BindingFlags.InvokeMethod, null, paras, CultureInfo.InvariantCulture);

                            if (methodInfo.ReturnType.Name == "Void")
                            {
                                return null;
                            }


                            if (resultv is bool)
                            {
                                return new BoolConstant((bool)resultv);
                            }
                            else if (resultv is int || resultv is short || resultv is byte || resultv is double)
                            {
                                return new IntConstant(Convert.ToInt32(resultv));
                            }
                            else if (resultv is int[])
                            {
                                int[] list = resultv as int[];
                                ExpressionValue[] vals = new ExpressionValue[list.Length];

                                for (int i = 0; i < vals.Length; i++)
                                {
                                    vals[i] = new IntConstant(list[i]);
                                }
                                return new RecordValue(vals);
                            }
                            else if (resultv is ExpressionValue)
                            {
                                return resultv as ExpressionValue;
                            }
                            else if (resultv == null)
                            {
                                return new NullConstant();
                            }

                            //return null;

                            //the following check is not necessary, since we will only keep bool, int and int[] methods
                            throw new RuntimeException("Call expression can only return int, short, byte, bool or int[] types. Please check your statement: " + methodCall.ToString() + ".");
                        }

                        throw new RuntimeException("Invalid Method Call: " + methodCall + "! Make sure you have defined the method in the library.");

                    }
                    catch (TargetInvocationException ex)
                    {
                        if(ex.InnerException != null)
                        {
                            RuntimeException exception =
                                new RuntimeException("Exception happened at expression " + exp + ": " +
                                                     ex.InnerException.Message);
                            exception.InnerStackTrace = ex.InnerException.StackTrace;
                            throw exception;    
                        }
                        else
                        {
                            throw new RuntimeException("Exception happened at expression " + exp + ": " + ex.Message);
                        }
                        
                    }
                    catch (Exception ex)
                    {
                        throw new RuntimeException("Exception happened at expression " + exp + ": " + ex.Message);
                    }
                case ExpressionType.ClassMethodCallInstance:
                    try
                    {
                        ClassMethodCallInstance methodCall = (ClassMethodCallInstance) exp;
                        ExpressionValue variable = Evaluate(methodCall.Variable, env); 

                        object[] paras = new object[methodCall.Arguments.Length];
                        for (int i = 0; i < paras.Length; i++)
                        {
                            ExpressionValue x1 = Evaluate(methodCall.Arguments[i], env);
                            paras[i] = GetValueFromExpression(x1);
                        }

                        MethodInfo methodInfo = variable.GetType().GetMethod(methodCall.MethodName);

                        if (methodInfo != null)
                        {
                            object resultv = methodInfo.Invoke(variable, paras);

                            if (methodInfo.ReturnType.Name == "Void")
                            {
                                return null;
                            }

                            if (resultv is bool)
                            {
                                return new BoolConstant((bool)resultv);
                            }
                            else if (resultv is int || resultv is short || resultv is byte || resultv is double)
                            {
                                return new IntConstant(Convert.ToInt32(resultv));
                            }
                            else if (resultv is int[])
                            {
                                int[] list = resultv as int[];
                                ExpressionValue[] vals = new ExpressionValue[list.Length];

                                for (int i = 0; i < vals.Length; i++)
                                {
                                    vals[i] = new IntConstant(list[i]);
                                }
                                return new RecordValue(vals);
                            }
                            else if (resultv is ExpressionValue)
                            {
                                return resultv as ExpressionValue;
                            }
                            else if (resultv == null)
                            {
                                return new NullConstant();
                            }

                            //return null;

                            //the following check is not necessary, since we will only keep bool, int and int[] methods
                            throw new RuntimeException("Call expression can only return int, short, byte, bool or int[] types. Please check your statement: " + methodCall.ToString() + ".");
                        }

                        throw new RuntimeException("Invalid Method Call: " + methodCall + "! Make sure you have defined the method in the library.");

                    }
                    catch (TargetInvocationException ex)
                    {
                        if (ex.InnerException != null)
                        {
                            RuntimeException exception =
                                new RuntimeException("Exception happened at expression " + exp + ": " +
                                                     ex.InnerException.Message);
                            exception.InnerStackTrace = ex.InnerException.StackTrace;
                            throw exception;
                        }
                        else
                        {
                            throw new RuntimeException("Exception happened at expression " + exp + ": " + ex.Message);
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new RuntimeException("Exception happened at expression " + exp + ": " + ex.Message);
                    }
                case ExpressionType.ClassProperty:
                    try
                    {
                        ClassProperty property = (ClassProperty)exp;
                        ExpressionValue variable = Evaluate(property.Variable, env);

                        PropertyInfo propertyInfo = variable.GetType().GetProperty(property.PropertyName);

                        object resultv = null;
                        if (propertyInfo != null)
                        {
                            resultv = propertyInfo.GetValue(variable, null);
                        }
                        else
                        {
                            FieldInfo fieldInfo = variable.GetType().GetField(property.PropertyName);
                            if (fieldInfo != null)
                            {
                                resultv = fieldInfo.GetValue(variable);
                            }
                        }

                        if (resultv != null)
                        {
                            if (resultv is bool)
                            {
                                return new BoolConstant((bool)resultv);
                            }
                            else if (resultv is int || resultv is short || resultv is byte || resultv is double)
                            {
                                return new IntConstant(Convert.ToInt32(resultv));
                            }
                            else if (resultv is int[])
                            {
                                int[] list = resultv as int[];
                                ExpressionValue[] vals = new ExpressionValue[list.Length];

                                for (int i = 0; i < vals.Length; i++)
                                {
                                    vals[i] = new IntConstant(list[i]);
                                }
                                return new RecordValue(vals);
                            }
                            else if (resultv is ExpressionValue)
                            {
                                return resultv as ExpressionValue;
                            }
                            //else if (resultv == null)
                            //{
                            //    return new NullConstant();
                            //}

                            //return null;

                            //the following check is not necessary, since we will only keep bool, int and int[] methods
                            throw new RuntimeException("Call expression can only return int, short, byte, bool or int[] types. Please check your statement: " + property.ToString() + ".");
                        }

                        throw new RuntimeException("Invalid Property Accessing: " + property + "! Make sure you have defined the method in the library.");

                    }
                    catch (TargetInvocationException ex)
                    {
                        if (ex.InnerException != null)
                        {
                            RuntimeException exception =
                                new RuntimeException("Exception happened at expression " + exp + ": " +
                                                     ex.InnerException.Message);
                            exception.InnerStackTrace = ex.InnerException.StackTrace;
                            throw exception;
                        }
                        else
                        {
                            throw new RuntimeException("Exception happened at expression " + exp + ": " + ex.Message);
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new RuntimeException("Exception happened at expression " + exp + ": " + ex.Message);
                    }
                case ExpressionType.ClassPropertyAssignment:
                    {
                        try
                        {
                            ClassPropertyAssignment pa = (ClassPropertyAssignment)exp;
                            ExpressionValue rhs = Evaluate(pa.RightHandExpression, env);
#if !OPTIMAL_FOR_EXP
                            if (rhs == null)
                            {
                                throw new RuntimeException("Invalid expression assignment: " + exp);
                            }
#endif                            
                            ClassProperty property = pa.ClassProperty;
                            ExpressionValue variable = Evaluate(property.Variable, env);

                            PropertyInfo propertyInfo = variable.GetType().GetProperty(property.PropertyName);

                            if (propertyInfo != null)
                            {
                                propertyInfo.SetValue(variable, GetValueFromExpression(rhs), null);
                            }
                            else
                            {
                                FieldInfo fieldInfo = variable.GetType().GetField(property.PropertyName);
                                if (fieldInfo != null)
                                {
                                    fieldInfo.SetValue(variable, GetValueFromExpression(rhs));
                                }
                                else
                                {
                                    throw new RuntimeException("Invalid expression assignment: " + exp);
                                }
                            }                            

                            return rhs;
                        }
                        catch (InvalidCastException ex)
                        {
                            throw new RuntimeException("Invalid Cast Exception for " + exp + ": " + ex.Message.Replace("PAT.Common.Classes.Expressions.ExpressionClass.", ""));
                        }
                    }
                 case ExpressionType.Let:
                    LetDefinition definition = exp as LetDefinition;
                    ExpressionValue rhv = Evaluate(definition.RightHandExpression, env);
                    env.ExtendDestructive(definition.Variable, rhv);
                    return null;
                 case ExpressionType.NewObjectCreation:
                    try
                    {
                        NewObjectCreation methodCall = (NewObjectCreation)exp;
                        
                        object[] paras = new object[methodCall.Arguments.Length];
                        for (int i = 0; i < paras.Length; i++)
                        {
                            ExpressionValue x1 = Evaluate(methodCall.Arguments[i], env);
                            paras[i] = GetValueFromExpression(x1);
                        }

                        Type classType;
                        
                        if (Common.Ultility.Ultility.CSharpDataType.TryGetValue(methodCall.ClassName, out classType))
                        {
                            object resultv = Activator.CreateInstance(classType, paras);

                            if (resultv is ExpressionValue)
                            {
                                return resultv as ExpressionValue;
                            }

                            //return null;

                            //the following check is not necessary, since we will only keep bool, int and int[] methods
                            throw new RuntimeException("Only object of class inheriting from ExpressionValue can be created. Please check your statement: " + methodCall .ToString() + ".");
                        }

                        throw new RuntimeException("Invalid Object Creation: " + methodCall + "! Make sure you have defined the class in the library.");

                    }
                    catch (Exception ex)
                    {
                        throw new RuntimeException("Exception happened at expression " + exp + ": " + ex.Message);
                    }
                 //case ExpressionType.UserDefinedDataType:
                 //   return exp as ;
                /* case ExpressionType.Let:
                // Evaluate body with respect to environment extended by binding of leftHand to rightHand
                {
                 Valuation newenv = env.GetVariableClone();
                 foreach (LetDefinition def in ((Let) exp).Definitions)
                 {
                     Value rhv = Evaluate(def.RightHandExpression, env);
                     //newenv = Extend(newenv, def.Variable, rhv);
                     //newenv = newenv.Extend(def.Variable, rhv);
                     newenv.ExtendDestructive(def.Variable, rhv);
                 }
                 return Evaluate(((Let) exp).Body, newenv);
                }
                case ExpressionType.Fun:
                return new FunValue(env, ((Fun) exp).Formals, ((Fun) exp).Body);
                case ExpressionType.RecFun:
                // For recursive functions, we need to place an environment
                // in the function that has a binding of the function variable
                // to the function itself. For this, we obtain a clone of the
                // environment, making sure that a destructive change will
                // not have any effect on the original environment. Then, we
                // place the clone in the function value. After that, we
                // destructively change the environment by a binding of the
                // function variable to the constructed function value.
                {
                 Valuation newEnv = env.GetVariableClone(); // (Valuation)env.Clone();
                 Value result = new FunValue(newEnv, ((RecFun) exp).Formals, ((RecFun) exp).body);
                 //ExtendDestructive(newEnv, ((RecFun)exp).FunVar, result);
                 newEnv.ExtendDestructive(((RecFun) exp).FunVar, result);
                 return result;
                }
                case ExpressionType.Application:
                // Apply the function value resulting from evaluating the operator
                // (we assume that this is a function value) to
                // the value resulting from evaluating the operand.
                // Note that we do not need to distinguish functions from
                // recursive functions. Both are represented by function values,
                // recursive functions have a binding of their function variable
                // to themselves in their environment.
                {
                 FunValue fun = (FunValue) Evaluate(((Application) exp).Operator, env);
                 Valuation newenv = (Valuation) fun.Valuation;

                 List<Expression> ops = ((Application) exp).Operands;
                 List<string> fe = fun.Formals;

                 for (int i = 0; i < ops.Count; i++)
                 {
                     Value argvalue = Evaluate(ops[i], env);
                     //newenv = Extend(newenv, fe[i], argvalue);
                     newenv = newenv.Extend((String) fe[i], argvalue);
                 }
                 return Evaluate(fun.Body, newenv);
                }*/
            }
   
            // (exp instanceof NotUsed)
            // NotUsed is used as argument2 of PrimitiveApplication.
            // We assume the resulting value will not be used,
            // thus any value will do, here.

            return new BoolConstant(true);
        }
    }
}