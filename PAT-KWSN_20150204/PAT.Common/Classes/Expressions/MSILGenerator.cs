using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using PAT.Common.Classes.DataStructure;
using PAT.Common.Classes.Expressions.ExpressionClass;
using PAT.Common.Classes.LTS;
using PAT.Common.Classes.SemanticModels.LTS.BDD;

namespace PAT.Common.Classes.Expressions
{
    public class MSILGenerator
    {
        public static TypeBuilder tb;

        private static string var1 = "a";
        private static string var2 = "b";

        public static void TestMSIL()
        {
            string assemblyName = "MSILGeneration";
            string modName = "MSILGeneration.dll";
            string typeName = "MSILForExpression";

            AppDomain appDomain = AppDomain.CurrentDomain;
            AssemblyName aname = new AssemblyName(assemblyName);
            AssemblyBuilder assemBuilder = appDomain.DefineDynamicAssembly(aname, AssemblyBuilderAccess.Run);
            ModuleBuilder modBuilder = assemBuilder.DefineDynamicModule(modName);
            tb = modBuilder.DefineType(typeName, TypeAttributes.Public);

            AddField(var1, typeof(int));
            tb.CreateType();

            Test1();
        }

        public static void Test1()
        {
            int length = 100;
            int loop = 1;

            Valuation valuation = new Valuation();
            Expression exp = new IntConstant(1);
            for(int i = 0; i < length; i++)
            {
                exp = new PrimitiveApplication(PrimitiveApplication.PLUS, exp, new IntConstant(1));
            }

            Console.WriteLine(DateTime.Now.ToLongTimeString() + DateTime.Now.Millisecond);
            for(int i = 0; i < loop; i++)
            {
                EvaluatorDenotational.Evaluate(exp, valuation);
            }

            Console.WriteLine(DateTime.Now.ToLongTimeString() + DateTime.Now.Millisecond);

            DynamicMethod method = GetDynamicMethodOfExpression(exp);

            for (int i = 0; i < loop; i++)
            {
                 method.Invoke(null, null);
            }
            Console.WriteLine(DateTime.Now.ToLongTimeString() + DateTime.Now.Millisecond);

            Console.WriteLine("---------------------");
            //---------------------------------------------------
            exp = new Variable(var1);
            for (int i = 0; i < length; i++)
            {
                exp = new PrimitiveApplication(PrimitiveApplication.PLUS, exp, new Variable(var1));
            }

            valuation.Variables = new StringDictionaryWithKey<ExpressionValue>();
            valuation.Variables.Add(var1, new IntConstant(1));

            Console.WriteLine(DateTime.Now.ToLongTimeString() + DateTime.Now.Millisecond);
            for (int i = 0; i < loop; i++)
            {
                EvaluatorDenotational.Evaluate(exp, valuation);
            }

            Console.WriteLine(DateTime.Now.ToLongTimeString() + DateTime.Now.Millisecond);

            method = GetDynamicMethodOfExpression(exp);

            for (int i = 0; i < loop; i++)
            {
                UpdateVarsBasedOnValuation(valuation);

                object value = method.Invoke(null, null);
                Console.WriteLine(value);
                UpdateValuationBasedOnClassValues(valuation);
            }
            Console.WriteLine(DateTime.Now.ToLongTimeString() + DateTime.Now.Millisecond);

            
        }


        /// <summary>
        /// Test evaluate expression without reading variable value
        /// </summary>
        private static void Test()
        {
            Expression exp;
            ExpressionValue value;
            int valueOfVar = 0;

            exp = new PrimitiveApplication(PrimitiveApplication.PLUS, new PrimitiveApplication(PrimitiveApplication.TIMES, new IntConstant(2), new IntConstant(3)),
                                                      new IntConstant(4));
            value = EvaluateExpression(exp);
            Console.WriteLine(value.ExpressionID);
            //
            exp = new Variable(var1);
            value = EvaluateExpression(exp);
            Console.WriteLine(value.ExpressionID);
            //
            exp = new Sequence(new Assignment(var1, new IntConstant(10)), new Variable(var1));
            value = EvaluateExpression(exp);
            Console.WriteLine(value.ExpressionID);
            //
            exp = new Sequence(new Assignment(var1, new IntConstant(10)), new Variable(var1));

            SetValue(var1, 5);
            valueOfVar = (int) GetValue(var1);
            value = EvaluateExpression(exp);
            valueOfVar = (int) GetValue(var1);
            Console.WriteLine(value.ExpressionID);
            Console.WriteLine(valueOfVar);
            //
            exp = new Sequence(new Assignment(var1, new PrimitiveApplication(PrimitiveApplication.PLUS, new Variable(var1), new IntConstant(10))), new Variable(var1));
            SetValue(var1, 5);
            value = EvaluateExpression(exp);
            valueOfVar = (int) GetValue(var1);
            Console.WriteLine(value.ExpressionID);
            Console.WriteLine(valueOfVar);

            //
            exp = new Sequence(new PropertyAssignment(new Variable(var2), new IntConstant(0),
                new PrimitiveApplication(PrimitiveApplication.PLUS, new PrimitiveApplication(PrimitiveApplication.ARRAY, new Variable(var2), new IntConstant(0))
                , new IntConstant(10))), new PrimitiveApplication(PrimitiveApplication.ARRAY, new Variable(var2), new IntConstant(0)));
            SetValue(var2, new int[2] { 20, 30 });
            value = EvaluateExpression(exp);
            int[] temp = (int[])GetValue(var2);
            Console.WriteLine(value.ExpressionID);
            Console.WriteLine(temp);
        }

        private static void AddField(string name, Type type)
        {
            FieldBuilder fieldBuilder = tb.DefineField(name, type, FieldAttributes.Static | FieldAttributes.Public);
        }

        private static void AddFields(Valuation valuation)
        {
            if (valuation.Variables != null && valuation.Variables.Count > 0)
            {
                foreach (StringDictionaryEntryWithKey<ExpressionValue> pair in valuation.Variables._entries)
                {
                    if (pair != null)
                    {
                        if (pair.Value is RecordValue)
                        {
                            AddField(pair.Key, typeof (int[]));
                        }
                        else
                        {
                            AddField(pair.Key, typeof(int));
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Set fields to values as valuation
        /// </summary>
        /// <param name="valuation"></param>
        private static void UpdateVarsBasedOnValuation(Valuation valuation)
        {
            if (valuation.Variables != null)
            {
                foreach (StringDictionaryEntryWithKey<ExpressionValue> pair in valuation.Variables._entries)
                {
                    if (pair != null)
                    {
                        if (pair.Value is RecordValue)
                        {
                            RecordValue arrayOfValues = pair.Value as RecordValue;
                            int[] values = new int[arrayOfValues.Associations.Length];

                            for (int i = 0; i < values.Length; i++)
                            {
                                values[i] = int.Parse(arrayOfValues.Associations[i].ExpressionID);
                            }
                            SetValue(pair.Key, values);
                        }
                        else if (pair.Value is BoolConstant)
                        {
                            int value = (pair.Value as BoolConstant).Value ? 1 : 0;
                            SetValue(pair.Key, value);
                        }
                        else
                        {
                            SetValue(pair.Key, int.Parse(pair.Value.ExpressionID));
                        }
                    }
                }
            }
        }

        private static void UpdateValuationBasedOnClassValues(Valuation valuation)
        {
            if (valuation.Variables != null)
            {
                foreach (StringDictionaryEntryWithKey<ExpressionValue> pair in valuation.Variables._entries)
                {
                    if (pair != null)
                    {
                        if (pair.Value is RecordValue)
                        {
                            RecordValue recordValue = (RecordValue)pair.Value;

                            int[] values = (int[])GetValue(pair.Key);
                            for (int i = 0; i < values.Length; i++)
                            {
                                if (recordValue.Associations[0] is IntConstant)
                                {
                                    recordValue.Associations[i] = new IntConstant(values[i]);
                                }
                                else
                                {
                                    recordValue.Associations[i] = new BoolConstant(values[i] > 0);
                                }
                            }
                        }
                        else if (pair.Value is BoolConstant)
                        {
                            int value = (int)GetValue(pair.Key);
                            pair.Value = new BoolConstant(value > 0);
                        }
                        else
                        {
                            int value = (int)GetValue(pair.Key);
                            pair.Value = new IntConstant(value);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Update the valuation after evaluating the expression
        /// </summary>
        /// <param name="funcName"></param>
        /// <param name="valuation"></param>
        private static void RunProgramBlock(Expression expression, Valuation valuation)
        {
            UpdateVarsBasedOnValuation(valuation);

            EvaluateExpression(expression);

            UpdateValuationBasedOnClassValues(valuation);
        }

        private static ExpressionValue EvaluateExpression(Expression expression)
        {
            DynamicMethod dynMeth = GetDynamicMethodOfExpression(expression);
            int value = (int) dynMeth.Invoke(null, null);

            return new IntConstant(value);
        }

        private static DynamicMethod GetDynamicMethodOfExpression(Expression expression)
        {
            DynamicMethod dynMeth = new DynamicMethod(string.Empty, typeof(int), null, typeof(void));

            ILGenerator gen = dynMeth.GetILGenerator();
            expression.GenerateMSIL(gen, tb);
            gen.Emit(OpCodes.Ret);

            return dynMeth;
        }

        private static object GetValue(string name)
        {
            return tb.GetField(name).GetValue(null);
        }

        private static void SetValue(string name, object value)
        {
            tb.GetField(name).SetValue(null, value);
        }
    }
}
