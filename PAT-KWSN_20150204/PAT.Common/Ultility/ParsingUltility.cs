using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using Antlr.Runtime;
using Antlr.Runtime.Tree;
using PAT.Common.Classes.Expressions;
using PAT.Common.Classes.Expressions.ExpressionClass;
using PAT.Common.Classes.LTS;
using PAT.Common.Classes.ModuleInterface;


namespace PAT.Common.Ultility
{
    public class ParsingUltility
    {
        public const int LTL_CHANNEL_TOKEN = -1;
        public const int LTL_COMPOUND_EVENT = -2;

        public const double MINIMUM_DIFFERENCE = 0.000001; //0.000001;

        public static void TestIsBooleanExpression(Expression p, IToken ID1, string expression, Valuation valuation, Dictionary<string, Expression> ConstantDatabase)
        {

            if (ConstantDatabase.Count > 0)
            {
                p = p.ClearConstant(ConstantDatabase);
            }

            if (p is PrimitiveApplication)
            {
                string Operator = (p as PrimitiveApplication).Operator;
                if (Operator == "+" || Operator == "-" || Operator == "*" || Operator == "/" || Operator == "~" || Operator == "." || Operator == "mod") //Operator == "." || 
                {
                    throw new ParsingException("The expression " + p + " " + expression + " must be a boolean expression!", ID1);
                }
                else if (Operator == "||" || Operator == "&&" || Operator == "!" )
                {
                    //recursive test the component inside
                    TestIsBooleanExpression((p as PrimitiveApplication).Argument1, ID1, expression, valuation, ConstantDatabase);
                    if((p as PrimitiveApplication).Argument2 != null)
                    {
                        TestIsBooleanExpression((p as PrimitiveApplication).Argument2, ID1, expression, valuation, ConstantDatabase);
                    }
                }
                else if (Operator == ">=" || Operator == "<=" || Operator == ">" || Operator == "<")
                {
                    //recursive test the component inside
                    TestIsIntExpression((p as PrimitiveApplication).Argument1, ID1, expression, valuation, ConstantDatabase);
                    if ((p as PrimitiveApplication).Argument2 != null)
                    {
                        TestIsIntExpression((p as PrimitiveApplication).Argument2, ID1, expression, valuation, ConstantDatabase);
                    }
                }
            }
            else if (p is Variable)
            {
                if (valuation != null && valuation.Variables != null && valuation.Variables.ContainsKey(p.ExpressionID) && !(valuation.Variables[p.ExpressionID] is BoolConstant))
                {
                    throw new ParsingException(string.Format(Resources.The_variable__0__must_be_a_boolean_variable_, p + " " + expression), ID1);
                }
            }
            else if (p is StaticMethodCall)
            {
                StaticMethodCall call = p as StaticMethodCall;

                switch (call.MethodName)
                {
                    case Common.Classes.Ultility.Constants.cfull:
                    case Common.Classes.Ultility.Constants.cempty:
                        return;
                }

                string key = call.MethodName+ call.Arguments.Length;

                if (Ultility.CSharpMethods.ContainsKey(key))
                {
                    if(Ultility.CSharpMethods[key].ReturnType.Name == "Boolean")
                    {
                        return;
                    }
                }
                else {
                    throw new ParsingException(string.Format("The call {0} is not defined.", call.MethodName), ID1);
                }
                throw new ParsingException(string.Format(Resources.The_static_method_call__0__must_return_a_boolean_value_, p), ID1);
            }
            else if (p is ClassMethodCall)
            {
                ClassMethodCall call = p as ClassMethodCall;
                if(valuation.Variables.ContainsKey(call.Variable))
                {
                    MethodInfo methodInfo = valuation.Variables[call.Variable].GetType().GetMethod(call.MethodName);
                    if (methodInfo != null)
                    {
                        if (methodInfo.ReturnType.Name == "Boolean")
                        {
                            return;
                        }
                    }    
                }
                else
                {
                    throw new ParsingException(string.Format("The call {0} may not be defined for variable {1}.", call.MethodName, call.Variable), ID1);
                }
                
                throw new ParsingException(string.Format(Resources.The_static_method_call__0__must_return_a_boolean_value_, p), ID1);
                
            }
            else if (p is Assignment)
            {
                Assignment assign = p as Assignment;
                TestIsBooleanExpression(assign.RightHandSide, ID1, expression, valuation, ConstantDatabase);
            }
            else if (p is PropertyAssignment)
            {
                PropertyAssignment assign = p as PropertyAssignment;
                TestIsBooleanExpression(assign.RightHandExpression, ID1, expression, valuation, ConstantDatabase);
            }
            else if (!(p is BoolConstant) && !(p is If) && !(p is While))
            {
                throw new ParsingException(string.Format(Resources.The_expression__0__must_be_a_boolean_expression_, p + " " + expression), ID1);
            }
        }

        public static void TestIsIntExpression(Expression p, IToken ID1, string expression, Valuation valuation, Dictionary<string, Expression> ConstantDatabase)
        {
            if (ConstantDatabase.Count > 0)
            {
                p = p.ClearConstant(ConstantDatabase);
            }

            if (p is PrimitiveApplication)
            {
                string Operator = (p as PrimitiveApplication).Operator;
                if (Operator == "||" || Operator == "&&" || Operator == "==" || Operator == "!=" || Operator == ">" ||
                    Operator == "<" || Operator == "!" || Operator == ">=" || Operator == "<=")
                {
                    throw new ParsingException(string.Format(Resources.The_expression__0__must_be_an_integer_expression_,  p + " " + expression), ID1);
                }
                //else
                //{
                //    //recursive test the component inside
                //    TestIsIntExpression((p as PrimitiveApplication).Argument1, ID1, expression, valuation, ConstantDatabase);
                //    if ((p as PrimitiveApplication).Argument2 != null)
                //    {
                //        TestIsIntExpression((p as PrimitiveApplication).Argument2, ID1, expression, valuation, ConstantDatabase);
                //    }
                //}
            }
            else if (p is Variable)
            {
                if (valuation != null && valuation.Variables != null && valuation.Variables.ContainsKey(p.ExpressionID) && !(valuation.Variables[p.ExpressionID] is IntConstant))
                {
                    throw new ParsingException(string.Format(Resources.The_variable__0__must_be_an_integer_variable_, p + " " + expression), ID1);
                }
            }
            else if (p is StaticMethodCall)
            {
                StaticMethodCall call = p as StaticMethodCall;
                switch (call.MethodName)
                {
                    case Common.Classes.Ultility.Constants.ccount:
                    case Common.Classes.Ultility.Constants.csize:
                        return;
                }                              

                string key = call.MethodName + call.Arguments.Length;

                if (Ultility.CSharpMethods.ContainsKey(key))
                {
                    if (Ultility.CSharpMethods[key].ReturnType.Name == "Int32")
                    {
                        return;
                    }
                }
                else
                {
                    throw new ParsingException(string.Format("The call {0} is not defined.", call.MethodName), ID1);
                }

                throw new ParsingException(string.Format(Resources.The_static_method_call__0__must_return_an_integer_value_, ID1.Text), ID1);

            }
            else if (p is ClassMethodCall)
            {
                ClassMethodCall call = p as ClassMethodCall;
                if (valuation.Variables.ContainsKey(call.Variable))
                {
                    MethodInfo methodInfo = valuation.Variables[call.Variable].GetType().GetMethod(call.MethodName);
                    if (methodInfo != null)
                    {
                        if (methodInfo.ReturnType.Name == "Int32")
                        {
                            return;
                        }
                    }
                }
                else
                {
                    throw new ParsingException(string.Format("The call {0} may not be defined for variable {1}.", call.MethodName, call.Variable), ID1);
                }
                
                throw new ParsingException(string.Format(Resources.The_method_call__0__must_return_an_integer_value_, p), ID1);
                
            }
            else if (p is Assignment)
            {
                Assignment assign = p as Assignment;
                TestIsIntExpression(assign.RightHandSide,ID1, expression, valuation, ConstantDatabase);
            }
            else if (p is PropertyAssignment)
            {
                PropertyAssignment assign = p as PropertyAssignment;
                TestIsIntExpression(assign.RightHandExpression, ID1, expression, valuation, ConstantDatabase);
            }
            else if (!(p is IntConstant) && !(p is ClassMethodCall) && !(p is If) && !(p is While) )
            {
                throw new ParsingException(string.Format(Resources.The_expression__0__must_be_an_integer_expression_,  p + " " + expression), ID1);
            }
        }

        public static void TestIsNonVoidExpression(Expression p, IToken ID1, Valuation valuation)
        {
            if (p is StaticMethodCall)
            {
                StaticMethodCall call = p as StaticMethodCall;
                if (call != null)
                {
                    string key = call.MethodName + call.Arguments.Length;

                    if (Ultility.CSharpMethods.ContainsKey(key))
                    {
                        if (Ultility.CSharpMethods[key].ReturnType.Name == "Void")
                        {
                            throw new ParsingException(string.Format("Expression {0} must have a return value! ", p),
                                                       ID1);
                        }
                    }
                }
            }
            else if (p is ClassMethodCall)
            {
                try
                {
                    ClassMethodCall call = (ClassMethodCall) p;
                    if (call != null && valuation.Variables.ContainsKey(call.Variable))
                    {
                        MethodInfo methodInfo = valuation.Variables[call.Variable].GetType().GetMethod(call.MethodName);
                        if (methodInfo != null)
                        {
                            if (methodInfo.ReturnType.Name == "Void")
                            {
                                throw new ParsingException(
                                    string.Format("Expression {0} must have a return value! ", p), ID1);
                            }
                        }
                    }
                }
                catch (ParsingException)
                {
                    throw;
                }
                catch (Exception)
                {
                    //throw;
                }
            }
        }


        public static int EvaluateExpression(Expression exp, IToken token, Dictionary<string, Expression> constantDB)
        {
            try
            {
                if (constantDB.Count > 0)
                {
                    exp = exp.ClearConstant(constantDB);
                }
                
                if(exp.HasVar)
                {
                    List<string> vars = exp.GetVars();
                    throw new ParsingException(string.Format(Resources.Variables___0___can_not_be_used_in_this_expression_,Classes.Ultility.Ultility.PPStringList(vars)) + exp, token);
                }

                ExpressionValue rhv = EvaluatorDenotational.Evaluate(exp, null);

                if (rhv is IntConstant)
                {
                    IntConstant v = rhv as IntConstant;
                    return v.Value;
                }
                else
                {
                    throw new ParsingException(Resources.The_expression_should_return_an_integer_value_, token);
                }
            }
            catch (Exception ex)
            {
                throw new ParsingException(ex.Message, token);
            }
        }

        public static IntConstant EvaluateIntExpression(Expression exp, IToken token, Dictionary<string, Expression> constantDB)
        {
            try
            {
                if (constantDB.Count > 0)
                {
                    exp = exp.ClearConstant(constantDB);
                }

                if (exp.HasVar)
                {
                    List<string> vars = exp.GetVars();
                    throw new ParsingException(string.Format(Resources.Variables___0___can_not_be_used_in_this_expression_, Classes.Ultility.Ultility.PPStringList(vars)) + exp, token);
                }

                ExpressionValue rhv = EvaluatorDenotational.Evaluate(exp, null);

                if (rhv is IntConstant)
                {
                   return rhv as IntConstant;
                }
                else
                {
                    throw new ParsingException(Resources.The_expression_should_return_an_integer_value_, token);
                }
            }
            catch (Exception ex)
            {
                throw new ParsingException(ex.Message, token);
            }
        }

        public static IToken GetVariableTokenInTree(CommonTree root, string variable)
        {

            if (root.Text == variable)
            {
                return root.Token;
            }

            if (root.Children != null)
            {
                foreach (CommonTree child in root.Children)
                {
                    IToken foundInChild = GetVariableTokenInTree(child, variable);
                    if (foundInChild != null)
                    {
                        return foundInChild;
                    }
                }
            }

            return null;
        }


        //public static void LoadPATLib(IToken libName)
        //{
        //    string dll = Ultility.LibFolderPath + "/" + libName.Text + ".dll";
        //    if(File.Exists(dll))
        //    {
        //        Ultility.LoadDLLLibrary(dll);
        //    }
        //    else
        //    {
        //        throw new ParsingException("The C# library " + dll + " can not be found!", libName);
        //    }
        //}

        public static void LoadStandardLib(IToken libPath, string filePathOld)
        {
            string filePath = ParsingException.GetFileNameByLineNumber(libPath.Line);

            string dll = libPath.Text.Trim('"');

            if (dll == "PAT.Math")
            {
                Ultility.LoadMathLib();
                return;
            }
            if (File.Exists(dll))
            {
                Ultility.LoadDLLLibrary(dll);
            }
            else
            {
                string dlllocal = ""; 
                if (!string.IsNullOrEmpty(filePath))
                {
                    dlllocal = Path.Combine(Path.GetDirectoryName(filePath), dll) + ".dll";
                    if (File.Exists(dlllocal))
                    {
                        Ultility.LoadDLLLibrary(dlllocal);
                        return;
                    }
                    dlllocal = " or " + dlllocal;
                }

                dll = Path.Combine(Ultility.LibFolderPath, libPath.Text.Trim('"') + ".dll");
                if (File.Exists(dll))
                {
                    Ultility.LoadDLLLibrary(dll);
                }
                else
                {
                    throw new ParsingException(string.Format(Resources.The_C__library__0__can_not_be_found_, dll + dlllocal),
                                               libPath);
                }
            }
        }


        public static void LoadIncludeModel(IToken libPath, SpecificationBase Spec)
        {
            //string s = libPath.Text;
            try
            {
                string filePath = ParsingException.GetFileNameByLineNumber(libPath.Line);
                //s += "\r\n1" + filePath;

                if (filePath == null)
                {
                    throw new ParsingException(string.Format("Cannot get the current file path"), libPath);
                }

                string dll = libPath.Text.Trim('"');
                /**
                 * Notes: if the dll path is start with "..\",
                 * which means that get the file name from the super directory,
                 * we parse this syntax here and assign the dll directory the value 
                 * of the current file's super directory
                 */
                string dllFileDirectory = Path.GetDirectoryName(filePath);

                //s += "\r\n2" + dllFileDirectory;

                if (Common.Ultility.Ultility.IsWindowsOS)
                {
                    if (dll.StartsWith("../"))
                    {
                        throw new ParsingException(
                            string.Format("Please use char '\\' instead of '/' in the include file path"),
                            libPath);
                    }

                    while (dll.StartsWith("..\\"))
                    {
                        dllFileDirectory = dllFileDirectory.Substring(0, dllFileDirectory.LastIndexOf('\\'));
                        dll = dll.Substring(dll.IndexOf('\\') + 1);
                    }
                }
                else
                {
                    while (dll.StartsWith("../") || dll.StartsWith("..\\"))
                    {
                        dllFileDirectory = dllFileDirectory.Substring(0, dllFileDirectory.LastIndexOf('/'));
                        if (dll.Contains("\\"))
                        {
                            dll = dll.Substring(dll.IndexOf('\\') + 1);
                        }
                        else if (dll.Contains("/"))
                        {
                            dll = dll.Substring(dll.IndexOf('/') + 1);
                        }
                    }
                }


                //s += "\r\n3" ;
                if (File.Exists(dll))
                {
                    Spec.IncludeFiles.Add(Path.Combine(dllFileDirectory, dll));
                }
                else
                {
                    string dlllocal = "";
                    if (!string.IsNullOrEmpty(filePath))
                    {
                        dlllocal = Path.Combine(dllFileDirectory, dll);
                        //s += "\r\n4" + dlllocal;
                        if (File.Exists(dlllocal))
                        {
                            Spec.IncludeFiles.Add(dlllocal);
                            return;
                        }
                    }

                    throw new ParsingException(string.Format("Cannot find the including file: {0}", dll), libPath);
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show(s + "\r\n" + ex.Message + ex.StackTrace);
                //throw new ParsingException(s + "\r\n" + ex.Message + ex.StackTrace, libPath);
                throw new ParsingException(string.Format("Cannot find the including file: {0}", libPath.Text), libPath);
            }
        }

        public static Expression TestMethod(StaticMethodCall call, IToken ID1, Dictionary<string, ChannelQueue> ChannelDatabase, Dictionary<string, Expression> ConstantDatabase)
        {
            return TestMethod(call, ID1, ChannelDatabase, ConstantDatabase, null);
        }

        public static Expression TestMethod(StaticMethodCall call, IToken ID1, Dictionary<string, ChannelQueue> ChannelDatabase, Dictionary<string, Expression> ConstantDatabase, SpecificationBase Spec) //IToken methodName, int size
        {

            if (ChannelDatabase != null)
            {
                switch (call.MethodName)
                {
                    case Common.Classes.Ultility.Constants.cfull:
                    case Common.Classes.Ultility.Constants.cempty:
                    case Common.Classes.Ultility.Constants.ccount:
                    case Common.Classes.Ultility.Constants.csize:
                    case Common.Classes.Ultility.Constants.cpeek:

                        if (call.Arguments.Length != 1)
                        {
                            throw new ParsingException(call.MethodName + " can only take exactly one argument!", ID1);
                        }

                        string cname = call.Arguments[0].ToString();
                        if (!ChannelDatabase.ContainsKey(cname))
                        {
                            throw new ParsingException(cname + " is not a valid channel name!", ID1);
                        }
                        else if (ChannelDatabase[cname].Size == 0)
                        {
                            throw new ParsingException(call + " cannot invoke on a synchronous channel!", ID1);
                        }
                        return call;
                }

                foreach (Expression argument in call.Arguments)
                {
                    if (ChannelDatabase.ContainsKey(argument.ToString()))
                    {
                        throw new ParsingException("Channel name " + argument.ToString() + " cannot be used in method call!", ID1);
                    }
                }
            }
            string key = call.MethodName + call.Arguments.Length;           

            if (!Ultility.CSharpMethods.ContainsKey(key))
            {
                if (Spec == null || !Spec.MacroDefinition.ContainsKey(key))
                {
                    throw new ParsingException(string.Format(Resources.Can_NOT_find_the_method__0__with__1__parameters_in_the_imported_C__libraries_, call.MethodName, call.Arguments.Length), ID1);
                }
                else
                {
                 
                    Dictionary<string, Expression> constMapping = new Dictionary<string, Expression>();
                    List<string> para = Spec.MacroDefinition[key].Key;
                    for (int i = 0; i < para.Count; i++) 
                    {
                        constMapping.Add(para[i], call.Arguments[i]);
                    }

                    AddIntoUsageTable(Spec.UsageTable, call.MethodName, ID1); //+ "(" + Common.Classes.Ultility.Ultility.PPStringList(para) + ")"

                    return Spec.MacroDefinition[key].Value.ClearConstant(constMapping);
                }
            }
            else
            {
                return call;
            }
        }

        public static List<string> CheckWhetherIsChannelCall(IToken methodName, List<string> sourceVars, Dictionary<string, ChannelQueue> ChannelDatabase)
        {
            switch (methodName.Text)
            {
                case Common.Classes.Ultility.Constants.cfull:
                case Common.Classes.Ultility.Constants.cempty:
                case Common.Classes.Ultility.Constants.ccount:
                case Common.Classes.Ultility.Constants.csize:
                case Common.Classes.Ultility.Constants.cpeek:

                    if (sourceVars == null)
                    {
                        sourceVars = new List<string>();
                    }
                    
                    sourceVars.AddRange(ChannelDatabase.Keys);
                    break;
            }

            return sourceVars;
        }

        public static List<string> CheckWhetherIsChannelCall(IToken methodName, List<string> sourceVars, Dictionary<string, ChannelQueue> ChannelDatabase, Dictionary<string, int> ChannelArrayDatabase)
        {
            switch (methodName.Text)
            {
                case Common.Classes.Ultility.Constants.cfull:
                case Common.Classes.Ultility.Constants.cempty:
                case Common.Classes.Ultility.Constants.ccount:
                case Common.Classes.Ultility.Constants.csize:
                case Common.Classes.Ultility.Constants.cpeek:

                    if (sourceVars == null)
                    {
                        sourceVars = new List<string>();
                    }

                    sourceVars.AddRange(ChannelDatabase.Keys);
                    sourceVars.AddRange(ChannelArrayDatabase.Keys);
                    break;
            }

            return sourceVars;
        }

        public static void TestMethodDefined(IToken methodName, int size, ExpressionValue variable)
        {


            MethodInfo methodInfo = variable.GetType().GetMethod(methodName.Text);
            
            if (methodInfo == null)
            {
                throw new ParsingException(string.Format(Resources.Can_NOT_find_the_method__0__with__1__parameters_in_class__2__, methodName.Text, size, variable.GetType().Name), methodName);
            }

        }

        public static void TestFieldDefined(IToken fieldName, ExpressionValue variable)
        {
            FieldInfo fieldInfo = variable.GetType().GetField(fieldName.Text);

            if (fieldInfo == null)
            {
                PropertyInfo propertyInfo = variable.GetType().GetProperty(fieldName.Text);
                if (propertyInfo == null)
                {
                    throw new ParsingException(string.Format("Cannot find the field {0} in class {1}", fieldName.Text, variable.GetType().Name), fieldName);
                }
            }

        }


        public static void UpdateClockBounds(Expression expression, Dictionary<string, Expression> constantDB, int oldCeiling, int oldFloor, out int ceiling, out int floor)
        {
            try
            {
                if (constantDB.Count > 0)
                {
                    expression = expression.ClearConstant(constantDB);
                }
                
                if (!expression.HasVar)
                {
                    ExpressionValue rhv = EvaluatorDenotational.Evaluate(expression, null);

                    if (rhv is IntConstant)
                    {
                        IntConstant v = rhv as IntConstant;
                        ceiling = Math.Max(v.Value, oldCeiling);
                        floor = Math.Min(v.Value, oldFloor);

                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                
            }

            ceiling = oldCeiling;
            floor = oldFloor;
        }

        public static List<string> SymbolList = new List<string>()
                                                    {
                                                        "||", "&&", "==", "!=", ">", "<", "!", ">=", "<=", "+", "-", "*", "/", "%", "^", "true", "false",
                                                       "CALL_NODE", "CLASS_CALL_NODE", "LET_NODE", "LET_ARRAY_NODE", "ASSIGNMENT_NODE", "LOCAL_VAR_NODE", "if", "while",  "new",
                                                       "CLASS_CALL_INSTANCE_NODE", "LOCAL_ARRAY_NODE", "VAR_NODE",
                                                    };

        public static List<string> ExpressionList = new List<string>()
                                                    {
                                                        "CALL_NODE", "CLASS_CALL_NODE",  "ASSIGNMENT_NODE", "LOCAL_VAR_NODE", "if", "while", "new",
                                                       "CLASS_CALL_INSTANCE_NODE", "LOCAL_ARRAY_NODE" //"LET_NODE", "LET_ARRAY_NODE",
                                                    };

        public static void IsStateAValidOneForBlock(ParserRuleReturnScope<IToken> statement, CommonTree tree)
        {
            if (tree != null && !SymbolList.Contains(tree.Text))
            {
                if (tree.Token.TokenIndex == -1)
                {
                    throw new ParsingException(Resources.Only_a_sequential_program_or_C__method_calls_can_be_associated_with_an_event_, statement.Start as IToken);
                }
                else
                {
                    throw new ParsingException(Resources.Only_a_sequential_program_or_C__method_calls_can_be_associated_with_an_event_, tree.Token);
                }
            }
            return;
        }

        public static void IsVarAValidOneForExpression(ParserRuleReturnScope<IToken> expression, CommonTree tree, Dictionary<string, CommonTree> DeclarationDatabase)
        {
            bool invalid = false;
            if (tree != null)
            {
                if (tree.Text == "VAR_NODE")
                {
                    if (DeclarationDatabase.ContainsKey(tree.GetChild(0).Text))
                    {
                        IsVarAValidOneForExpression(expression, DeclarationDatabase[tree.GetChild(0).Text], DeclarationDatabase);
                    }
                    else
                    {
                        invalid = true;
                    }
                }
                else if (tree.Text == "BLOCK_NODE")
                {
                    foreach (CommonTree statement in tree.Children)
                    {
                        IsVarAValidOneForExpression(expression, statement, DeclarationDatabase);
                    }
                }
                else if (tree.Text == "CALL_NODE")
                {
                    string key = tree.GetChild(1).Text + (tree.ChildCount - 2);
                    if (DeclarationDatabase.ContainsKey(key))
                    {
                        IsVarAValidOneForExpression(expression, DeclarationDatabase[key], DeclarationDatabase);
                    }
                }
                else if (tree.Text == "if")
                {
                    IsVarAValidOneForExpression(expression, tree.GetChild(1) as CommonTree, DeclarationDatabase);
                    IsVarAValidOneForExpression(expression, tree.GetChild(2) as CommonTree, DeclarationDatabase);
                }
                else if (tree.Text == "while")
                {
                    IsVarAValidOneForExpression(expression, tree.GetChild(1) as CommonTree, DeclarationDatabase);
                }
                else if (!ExpressionList.Contains(tree.Text))
                {
                    invalid = true;
                }
                else if (tree.Text == "CLASS_CALL_NODE" && tree.ChildCount == 2)
                {
                    if (tree.GetChild(1).Text == "FIELDS_CALL_NODE")
                    {
                        invalid = true;
                    }
                }
            }

            if (invalid)
            {
                if (tree.Token.TokenIndex == -1)
                {
                    throw new ParsingException("Only assignment, call, if, while, local variable declaration, new object creation and object method call can be used as a statement", expression.Start as IToken);
                }
                else
                {
                    throw new ParsingException("Only assignment, call, if, while, local variable declaration, new object creation and object method call can be used as a statement", tree.Token);
                }
            }
            return;
        }


        public static void IsStateAValidOneForExpression(ParserRuleReturnScope<IToken> expression, CommonTree tree, Dictionary<string, CommonTree> DeclarationDatabase, List<CommonTree> varsToBeChecked, List<ParserRuleReturnScope<IToken>> expToBeChecked)
        {
            bool invalid = false;
            if (tree != null)
            {
                if (tree.Text == "VAR_NODE")
                {
                    if(DeclarationDatabase.ContainsKey(tree.GetChild(0).Text))
                    {
                        IsStateAValidOneForExpression(expression, DeclarationDatabase[tree.GetChild(0).Text], DeclarationDatabase, varsToBeChecked, expToBeChecked);
                    }
                    else
                    {
                        ////invalid = true; 
                        //return tree;
                        varsToBeChecked.Add(tree);
                        expToBeChecked.Add(expression);
                    }
                }
                else if (tree.Text == "BLOCK_NODE")
                {
                    foreach (CommonTree statement in tree.Children)
                    {
                        IsStateAValidOneForExpression(expression, statement, DeclarationDatabase, varsToBeChecked, expToBeChecked);
                    }
                }
                else if(tree.Text == "CALL_NODE")
                {
                    string key = tree.GetChild(0).Text + (tree.ChildCount - 2);
                    if (DeclarationDatabase.ContainsKey(key))
                    {
                        IsStateAValidOneForExpression(expression, DeclarationDatabase[key], DeclarationDatabase, varsToBeChecked, expToBeChecked);
                    }
                }
                else if (tree.Text == "if")
                {
                    IsStateAValidOneForExpression(expression, tree.GetChild(1) as CommonTree, DeclarationDatabase, varsToBeChecked, expToBeChecked);
                    IsStateAValidOneForExpression(expression, tree.GetChild(2) as CommonTree, DeclarationDatabase, varsToBeChecked, expToBeChecked);
                }
                else if (tree.Text == "while")
                {
                    IsStateAValidOneForExpression(expression, tree.GetChild(1) as CommonTree, DeclarationDatabase, varsToBeChecked, expToBeChecked);
                }
                else if(!ExpressionList.Contains(tree.Text))
                {
                    invalid = true;
                }
                else if (tree.Text == "CLASS_CALL_NODE" && tree.ChildCount == 2)
                {
                    if (tree.GetChild(1).Text == "FIELDS_CALL_NODE")
                    {
                        invalid = true;
                    }
                }                
            }

            if(invalid)
            {
                if (tree.Token.TokenIndex == -1)
                {
                    throw new ParsingException("Only assignment, call, if, while, local variable declaration, new object creation and object method call can be used as a statement", expression.Start as IToken);
                }
                else
                {
                    throw new ParsingException("Only assignment, call, if, while, local variable declaration, new object creation and object method call can be used as a statement", tree.Token);
                }
            }
            return;
        }

        public static void IsStateAValidOneForExpression(ParserRuleReturnScope<IToken> expression, CommonTree tree)
        {
            bool invalid = false;
            if (tree != null)
            {
                if (!ExpressionList.Contains(tree.Text))
                {
                    invalid = true;
                }
                else if (tree.Text == "CLASS_CALL_NODE" && tree.ChildCount == 2)
                {
                    if (tree.GetChild(1).Text == "FIELDS_CALL_NODE")
                    {
                        invalid = true;
                    }
                }

            }

            if (invalid)
            {
                if (tree.Token.TokenIndex == -1)
                {
                    throw new ParsingException("Only assignment, call, if, while, local variable declaration, new object creation and object method call can be used as a statement", expression.Start as IToken);
                }
                else
                {
                    throw new ParsingException("Only assignment, call, if, while, local variable declaration, new object creation and object method call can be used as a statement", tree.Token);
                }
            }
            return;
        }


        /// <summary>
        /// Input an expression token in tree structure, return a single token representing the whole expression
        /// </summary>
        /// <param name="rootToken"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        public static IToken GetExpressionToken(CommonTree rootToken, ITreeNodeStream input)
        {
            try
            {
                ITokenStream stream = ((Antlr.Runtime.Tree.CommonTreeNodeStream)(input)).TokenStream;

                int start = rootToken.TokenStartIndex;
                int end = rootToken.TokenStopIndex;
                IToken token1 = new CommonToken(); //(Token.DEFAULT_CHANNEL
                token1.CharPositionInLine = stream.Get(start).CharPositionInLine;
                token1.Line = stream.Get(start).Line;

                for (int i = start; i <= end; i++)
                {
                    token1.Text += stream.Get(i).Text;
                }
                return token1;
            }
            catch (Exception)
            {

            }

            return rootToken.Token;
        }

        /// <summary>
        /// Input an expression token in tree structure, return a single token representing the whole expression
        /// </summary>
        /// <param name="rootToken"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        public static void CheckIsParsingComplete(CommonTokenStream tokens, CommonTree tree)
        {
            if (!(tokens.Index == tokens.Count - 2 && tokens.Get(tokens.Index).Text == "<EOF>" && tokens.Get(tokens.Index + 1).Text == "<EOF>"))
            {
                if (tokens.Index != tokens.Count - 1 || tokens.Get(tokens.Index).Text != "<EOF>")
                {
                    throw new ParsingException("Unrecognized symbol.", tokens.Get(tokens.Index));
                }    
            }

            if (tree == null)
            {
                throw new ParsingException(
                    "Please check your first line of your model (not comments), it may start with invalid symbols.",
                    tokens.Get(0));
            }
        }

        public static void AddIntoUsageTable(Dictionary<string, List<ParsingException>> UsageTable, string keyword, IToken token)
        {
            if (UsageTable.ContainsKey(keyword))
            {
                if (token != null)
                {
                    UsageTable[keyword].Add(new ParsingException("", token));
                }
            }
            else
            {
                UsageTable.Add(keyword, new List<ParsingException>());
                if (token != null)
                {
                    UsageTable[keyword].Add(new ParsingException("", token));
                }
            }
        }

        public static void AddIntoUsageTable(Dictionary<string, List<ParsingException>> UsageTable, string keyword, IToken token, ParsingException definition)
        {
            if (UsageTable.ContainsKey(keyword))
            {
                if (token != null)
                {
                    UsageTable[keyword].Add(new ParsingException("", token, definition));
                }
            }
            else
            {
                UsageTable.Add(keyword, new List<ParsingException>());
                if (token != null)
                {
                    UsageTable[keyword].Add(new ParsingException("", token, definition));
                }
            }
        }

        public static void CheckExpressionWithGlobalVariable(Expression e1, List<string> parameters, CommonTree ParentTree, IToken defaultToken)
        {
            //if e has only one var, this means e is a input variable
            List<string> evars = e1.GetVars();
            if (evars.Count >= 1)
            {
                foreach (string var in evars)
                {
                    IToken token = Common.Ultility.ParsingUltility.GetVariableTokenInTree(ParentTree, var);

                    if (token == null)
                    {
                        throw new ParsingException("Parsing error in expression " + e1.ToString(), defaultToken);
                    }

                    //if not contained in the parameters, then the variable is in the global variables
                    if (!parameters.Contains(var))
                    {
                        throw new ParsingException("Only process parameter and constants can be used in expression " + e1 + "! Global variable is not supported here for performance reason!", token);
                    }
                }
            }
        }



        //public static void CheckParameterVarUsedInMathExpression(Expression e1, Dictionary<string, string> ParameterVariables, IToken token)
        //{
        //    if (e1 is Variable)
        //    {
        //        string name = ((Variable) e1).VarName;
        //        if (ParameterVariables.ContainsKey(name))
        //        {
        //            throw new ParsingException("Can not use parameter variable " + name + " in math expression!", token);
        //        }
        //    }
        //}  

        public static void CheckTimedExpressionWithGlobalVariable(Expression e1, IToken token, Dictionary<string, Expression> ConstantDatabase, List<IToken> GlobalVarNames)
        {
            if (ConstantDatabase.Count > 0)
            {
                e1 = e1.ClearConstant(ConstantDatabase);
            }

            List<string> evars = e1.GetVars();
            if (evars.Count >= 1)
            {
                foreach (string evar in evars)
                {
                    foreach (IToken gvar in GlobalVarNames)
                    {
                        if (gvar.Text == evar)
                        {
                            throw new ParsingException("Global variable (" + evar + ") cannot be used in timed process!", token);
                        }    
                    }
                    
                }                
            }
        }
    }
}