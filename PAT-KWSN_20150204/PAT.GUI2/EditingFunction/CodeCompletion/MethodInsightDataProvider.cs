using System;
using System.Collections.Generic;
using System.Linq;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Document;
using ICSharpCode.TextEditor.Gui.InsightWindow;
using PAT.GUI.Docking;

namespace PAT.GUI.EditingFunction.CodeCompletion
{
	public class MethodInsightDataProvider : IInsightDataProvider
	{
		string    fileName = null;
		IDocument document = null;
		TextArea textArea  = null;
		protected List<IMethodOrProperty> methods  = new List<IMethodOrProperty>();
        EditorTabItem mainForm;

		public List<IMethodOrProperty> Methods {
			get {
				return methods;
			}
		}
		
		public int InsightDataCount {
			get {
				return methods.Count;
			}
		}
		
		int defaultIndex = -1;
		
		public int DefaultIndex {
			get {
				return defaultIndex;
			}
			set {
				defaultIndex = value;
			}
		}
		
		public string GetInsightData(int number)
		{
			IMember method = methods[number];
			IAmbience conv = AmbienceService.GetCurrentAmbience();
			conv.ConversionFlags = ConversionFlags.StandardConversionFlags| ConversionFlags.UseFullyQualifiedMemberNames;
			//string documentation = method.Documentation;
			string text = conv.Convert(method).Replace("XXX.", "").Replace("Method ", "Process ");
		    return text;// +"\n" + CodeCompletionData.ConvertDocumentation(documentation);
		}
		
		int lookupOffset;
		bool setupOnlyOnce;
		
		/// <summary>
		/// Creates a MethodInsightDataProvider looking at the caret position.
		/// </summary>
        public MethodInsightDataProvider(EditorTabItem form)
		{
			this.lookupOffset = -1;
		    this.mainForm = form;
		}
		
		/// <summary>
		/// Creates a MethodInsightDataProvider looking at the specified position.
		/// </summary>
		public MethodInsightDataProvider(int lookupOffset, bool setupOnlyOnce)
		{
			this.lookupOffset = lookupOffset;
			this.setupOnlyOnce = setupOnlyOnce;
		}


	    private static IClass dummyClass = new DefaultClass(DefaultCompilationUnit.DummyCompilationUnit, "XXX");

		int initialOffset;

        public void SetupDataProvider(string fileName, TextArea textArea)
        {
            if (setupOnlyOnce && this.textArea != null) return;
            IDocument document = textArea.Document;
            this.fileName = fileName;
            this.document = document;
            this.textArea = textArea;
            int useOffset = (lookupOffset < 0) ? textArea.Caret.Offset : lookupOffset;
            initialOffset = useOffset;


            ExpressionResult expressionResult = new ExpressionResult(TextUtilities.GetExpressionBeforeOffset(textArea, useOffset));
            if (expressionResult.Expression == null) // expression is null when cursor is in string/comment
                return;
            
            expressionResult.Expression = expressionResult.Expression.Trim();

            string expression = (expressionResult.Expression ?? "").Trim();
           
            if (!string.IsNullOrEmpty(expression))
            {
                if (mainForm.Specification != null)
                {
                    DefaultMethod m = new DefaultMethod(dummyClass, expression);

                    string[] parameters = mainForm.Specification.GetParameterNames(expression);
                    if(parameters != null)
                    {
                        foreach (string parameter in parameters)
                        {
                            m.Parameters.Add(new DefaultParameter(parameter, varClass.DefaultReturnType, DomRegion.Empty));
                        }


                        //// for the insight window, remove first parameter and mark the
                        //// method as normal - this is required to show the list of
                        //// parameters the method expects.
                        m.IsExtensionMethod = false;

                        methods.Add(m);    
                    }                    
                }
            }
            //    foreach (KeyValuePair<string, List<FuncStruct>> pair in PredefinedFuncTable.symbolTableList)
            //    {
            //        if (pair.Key == expression)
            //        {
            //            string funName = pair.Key;


            //            foreach (FuncStruct func in pair.Value)
            //            {
            //                if (func.getFuncName() != null)
            //                {
            //                    DefaultMethod m = new DefaultMethod(dummyClass, funName);
            //                    m.ReturnType = GetType(func.getReturnType());

            //                    //m.Documentation = func.Description;
            //                    List<int> types = func.getParamTypes();
            //                    List<string> names = func.getParamNames();

            //                    for (int i = 0; i < types.Count; i++)
            //                    {
            //                        int type = types[i];
            //                        string name = names[i];
            //                        m.Parameters.Add(new DefaultParameter(name, GetType(type), DomRegion.Empty));
            //                    }

            //                    //// for the insight window, remove first parameter and mark the
            //                    //// method as normal - this is required to show the list of
            //                    //// parameters the method expects.
            //                    m.IsExtensionMethod = false;
            //                    //m.Parameters.RemoveAt(0);
            //                    methods.Add(m);
            //                }
            //            }
            //        }
            //    }
            //}

            //if (setupOnlyOnce && this.textArea != null) return;
            //IDocument document = textArea.Document;
            //this.fileName = fileName;
            //this.document = document;
            //this.textArea = textArea;
            //int useOffset = (lookupOffset < 0) ? textArea.Caret.Offset : lookupOffset;
            //initialOffset = useOffset;


            //IExpressionFinder expressionFinder = ParserService.GetExpressionFinder(fileName);
            //ExpressionResult expressionResult;
            //if (expressionFinder == null)
            //    expressionResult = new ExpressionResult(TextUtilities.GetExpressionBeforeOffset(textArea, useOffset));
            //else
            //    expressionResult = expressionFinder.FindExpression(textArea.Document.TextContent, useOffset);

            //if (expressionResult.Expression == null) // expression is null when cursor is in string/comment
            //    return;
            //expressionResult.Expression = expressionResult.Expression.Trim();

            //if (LoggingService.IsDebugEnabled) {
            //    if (expressionResult.Context == ExpressionContext.Default)
            //        LoggingService.DebugFormatted("ShowInsight for >>{0}<<", expressionResult.Expression);
            //    else
            //        LoggingService.DebugFormatted("ShowInsight for >>{0}<<, context={1}", expressionResult.Expression, expressionResult.Context);
            //}

            //int caretLineNumber = document.GetLineNumberForOffset(useOffset);
            //int caretColumn     = useOffset - document.GetLineSegment(caretLineNumber).Offset;
            //// the parser works with 1 based coordinates
            //SetupDataProvider(fileName, document, expressionResult, caretLineNumber + 1, caretColumn + 1);
        }

	    //protected virtual void SetupDataProvider(string fileName, IDocument document, ExpressionResult expressionResult, int caretLineNumber, int caretColumn)
        //{
        //    bool constructorInsight = false;
        //    if (expressionResult.Context == ExpressionContext.Attribute) {
        //        constructorInsight = true;
        //    } else if (expressionResult.Context.IsObjectCreation) {
        //        constructorInsight = true;
        //        expressionResult.Context = ExpressionContext.Type;
        //    } else if (expressionResult.Context == ExpressionContext.BaseConstructorCall) {
        //        constructorInsight = true;
        //    }
			
        //    ResolveResult results = ParserService.Resolve(expressionResult, caretLineNumber, caretColumn, fileName, document.TextContent);
        //    if (results == null)
        //        return;
        //    LanguageProperties language = ParserService.CurrentProjectContent.Language;
        //    TypeResolveResult trr = results as TypeResolveResult;
        //    if (trr == null && language.AllowObjectConstructionOutsideContext) {
        //        if (results is MixedResolveResult)
        //            trr = (results as MixedResolveResult).TypeResult;
        //    }
        //    if (trr != null && !constructorInsight) {
        //        if (language.AllowObjectConstructionOutsideContext)
        //            constructorInsight = true;
        //    }
        //    if (constructorInsight) {
        //        if (trr != null || expressionResult.Context == ExpressionContext.BaseConstructorCall) {
        //            if (results.ResolvedType != null) {
        //                methods.AddRange(GetConstructorMethods(results.ResolvedType.GetMethods()));
        //            }
        //        }
        //    } else {
        //        MethodGroupResolveResult result = results as MethodGroupResolveResult;
        //        if (result == null)
        //            return;
        //        bool classIsInInheritanceTree = false;
        //        if (result.CallingClass != null)
        //            classIsInInheritanceTree = result.CallingClass.IsTypeInInheritanceTree(result.ContainingType.GetUnderlyingClass());
				
        //        foreach (IMethod method in result.ContainingType.GetMethods()) {
        //            if (language.NameComparer.Equals(method.Name, result.Name)) {
        //                if (method.IsAccessible(result.CallingClass, classIsInInheritanceTree)) {
        //                    methods.Add(method);
        //                }
        //            }
        //        }
        //        if (methods.Count == 0 && result.CallingClass != null && language.SupportsExtensionMethods) {
        //            ArrayList list = new ArrayList();
        //            ResolveResult.AddExtensions(language, list, result.CallingClass, result.ContainingType);
        //            foreach (IMethodOrProperty mp in list) {
        //                if (language.NameComparer.Equals(mp.Name, result.Name) && mp is IMethod) {
        //                    DefaultMethod m = (DefaultMethod)mp.CreateSpecializedMember();
        //                    // for the insight window, remove first parameter and mark the
        //                    // method as normal - this is required to show the list of
        //                    // parameters the method expects.
        //                    m.IsExtensionMethod = false;
        //                    m.Parameters.RemoveAt(0);
        //                    methods.Add(m);
        //                }
        //            }
        //        }
        //    }
        //}
		
		IEnumerable<IMethodOrProperty> GetConstructorMethods(List<IMethod> methods)
		{
			return from method in methods
				where method.IsConstructor && !method.IsStatic
				select (IMethodOrProperty)method;
		}
		
		public bool CaretOffsetChanged()
		{
			bool closeDataProvider = textArea.Caret.Offset <= initialOffset;
			int brackets = 0;
			int curlyBrackets = 0;
			if (!closeDataProvider) {
				bool insideChar   = false;
				bool insideString = false;
				for (int offset = initialOffset; offset < Math.Min(textArea.Caret.Offset, document.TextLength); ++offset) {
					char ch = document.GetCharAt(offset);
					switch (ch) {
						case '\'':
							insideChar = !insideChar;
							break;
						case '(':
							if (!(insideChar || insideString)) {
								++brackets;
							}
							break;
						case ')':
							if (!(insideChar || insideString)) {
								--brackets;
							}
							if (brackets <= 0) {
								return true;
							}
							break;
						case '"':
							insideString = !insideString;
							break;
						case '}':
							if (!(insideChar || insideString)) {
								--curlyBrackets;
							}
							if (curlyBrackets < 0) {
								return true;
							}
							break;
						case '{':
							if (!(insideChar || insideString)) {
								++curlyBrackets;
							}
							break;
						case ';':
							if (!(insideChar || insideString)) {
								return true;
							}
							break;
					}
				}
			}
			
			return closeDataProvider;
		}
		
		public bool CharTyped()
		{
//			int offset = document.Caret.Offset - 1;
//			if (offset >= 0) {
//				return document.GetCharAt(offset) == ')';
//			}
			return false;
		}

        public const int BOOLEAN = 10000;
        public const int STRING = 10001;
        public const int CHAR = 10003;
        public const int BYTE = 10004;
        public const int FLOAT = 10005;
        public const int INT = 10006;
        public const int LONG = 10007;
        public const int ARRAY = 10008;
        public const int SIGNAL = 10009;

        private static IClass booleanClass = new DefaultClass(DefaultCompilationUnit.DummyCompilationUnit, "boolean");
        private static IClass stringClass = new DefaultClass(DefaultCompilationUnit.DummyCompilationUnit, "string");
        private static IClass charClass = new DefaultClass(DefaultCompilationUnit.DummyCompilationUnit, "char");
        private static IClass byteClass = new DefaultClass(DefaultCompilationUnit.DummyCompilationUnit, "byte");
        private static IClass floatClass = new DefaultClass(DefaultCompilationUnit.DummyCompilationUnit, "float");
        private static IClass intClass = new DefaultClass(DefaultCompilationUnit.DummyCompilationUnit, "int");
        private static IClass longClass = new DefaultClass(DefaultCompilationUnit.DummyCompilationUnit, "long");
        private static IClass arrayClass = new DefaultClass(DefaultCompilationUnit.DummyCompilationUnit, "array");
        private static IClass signalClass = new DefaultClass(DefaultCompilationUnit.DummyCompilationUnit, "signal");

        private static IClass varClass = new DefaultClass(DefaultCompilationUnit.DummyCompilationUnit, "var");

        private IReturnType GetType(int type)
        {
            switch(type)
            {
                case BOOLEAN:
                    return booleanClass.DefaultReturnType;
                case STRING:
                    return booleanClass.DefaultReturnType;
                case CHAR:
                    return charClass.DefaultReturnType;
                case BYTE:
                    return byteClass.DefaultReturnType;
                case FLOAT:
                    return floatClass.DefaultReturnType;
                case INT:
                    return intClass.DefaultReturnType;
                case LONG:
                    return longClass.DefaultReturnType;
                case ARRAY:
                    return arrayClass.DefaultReturnType;
                case SIGNAL:
                    return signalClass.DefaultReturnType;
            }

            return null;            
        }
	}
}
