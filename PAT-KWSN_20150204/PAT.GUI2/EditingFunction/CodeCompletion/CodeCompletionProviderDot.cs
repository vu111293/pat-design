using System.Collections.Generic;
using System.Windows.Forms;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Gui.CompletionWindow;
using PAT.GUI.Docking;
using Dom = ICSharpCode.SharpDevelop.Dom;

namespace PAT.GUI.EditingFunction.CodeCompletion
{
    class CodeCompletionProviderDot : ICompletionDataProvider
	{
        EditorTabItem mainForm;

        public CodeCompletionProviderDot(EditorTabItem mainForm)
		{
			this.mainForm = mainForm;
		}
		
		public ImageList ImageList {
			get {
				return mainForm.imageList1;
			}
		}
		
		public string PreSelection {
			get {
				return null;
			}
		}
		
		public int DefaultIndex {
			get {
				return -1;
			}
		}

        public CompletionDataProviderKeyResult ProcessKey(char key)
		{
            return CompletionDataProviderKeyResult.NormalKey;
            //if (char.IsLetterOrDigit(key) || key == '_') {
            //    return CompletionDataProviderKeyResult.NormalKey;
            //} else {
            //    // key triggers insertion of selected items
            //    return CompletionDataProviderKeyResult.InsertionKey;
            //}
		}
		
		/// <summary>
		/// Called when entry should be inserted. Forward to the insertion action of the completion data.
		/// </summary>
		public bool InsertAction(ICompletionData data, TextArea textArea, int insertionOffset, char key)
		{
			textArea.Caret.Position = textArea.Document.OffsetToPosition(insertionOffset);
			return data.InsertAction(textArea, key);
		}
		
		public ICompletionData[] GenerateCompletionData(string fileName, TextArea textArea, char charTyped)
		{


			// We can return code-completion items like this:
		    ExpressionResult expressionResult = FindExpression(textArea);
            string expression = (expressionResult.Expression ?? "").Trim();

            if (charTyped == '.')
            {
                if (!string.IsNullOrEmpty(expression))
                {
                    //List<ICompletionData> data = new List<ICompletionData>();
                    //expression = expression + ".";
                    //foreach (KeyValuePair<string, List<FuncStruct>> pair in PredefinedFuncTable.symbolTableList)
                    //{
                    //    if (pair.Key.StartsWith(expression))
                    //    {
                    //        string funName = pair.Key.Replace(expression, "");
                    //        List<FuncStruct> funcs = pair.Value;
                    //        if (funcs.Count == 1)
                    //        {
                    //            FuncStruct func = funcs[0];
                    //            data.Add(new DefaultCompletionData(funName, func.Description, func.Icon));
                    //        }
                    //        else
                    //        {
                    //            string descriptions = "";
                    //            foreach (FuncStruct func in funcs)
                    //            {
                    //                descriptions += func.Description + "\r\n";
                    //            }

                    //            data.Add(new DefaultCompletionData(funName, descriptions, funcs[0].Icon));
                    //        }

                    //    }
                    //}

                    //return data.ToArray();
                }
            }
            else if(char.IsLower(charTyped))
            {
                if (string.IsNullOrEmpty(expression))
                {
                    //List<ICompletionData> data = new List<ICompletionData>();
                    
                    //foreach (KeyValuePair<string, List<FuncStruct>> pair in PredefinedFuncTable.symbolTableList)
                    //{
                    //    if (!pair.Key.Contains("."))
                    //    {
                    //        string funName = pair.Key;
                    //        List<FuncStruct> funcs = pair.Value;
                    //        if (funcs.Count == 1)
                    //        {
                    //            FuncStruct func = funcs[0];
                    //            data.Add(new DefaultCompletionData(funName, func.Description, func.Icon));
                    //        }
                    //        else
                    //        {
                    //            string descriptions = "";
                    //            foreach (FuncStruct func in funcs)
                    //            {
                    //                descriptions += func.Description + "\r\n";
                    //            }

                    //            data.Add(new DefaultCompletionData(funName, descriptions, funcs[0].Icon));
                    //        }

                    //    }
                    //}

                    //return data.ToArray();
                }

            }

		    return null;

			
            //NRefactoryResolver resolver = new NRefactoryResolver(mainForm.myProjectContent.Language);
            //Dom.ResolveResult rr = resolver.Resolve(FindExpression(textArea),
            //                                        mainForm.parseInformation,
            //                                        textArea.MotherTextEditorControl.Text);
            //List<ICompletionData> resultList = new List<ICompletionData>();
            //if (rr != null) {
            //    ArrayList completionData = rr.GetCompletionData(mainForm.myProjectContent);
            //    if (completionData != null) {
            //        AddCompletionData(resultList, completionData);
            //    }
            //}
            //return resultList.ToArray();
		}
		
		/// <summary>
		/// Find the expression the cursor is at.
		/// Also determines the context (using statement, "new"-expression etc.) the
		/// cursor is at.
		/// </summary>
		Dom.ExpressionResult FindExpression(TextArea textArea)
		{
			Dom.IExpressionFinder finder;
			
			finder = new Dom.CSharp.CSharpExpressionFinder(mainForm.parseInformation);
			
			Dom.ExpressionResult expression = finder.FindExpression(textArea.Document.TextContent, textArea.Caret.Offset);
			if (expression.Region.IsEmpty) {
				expression.Region = new Dom.DomRegion(textArea.Caret.Line + 1, textArea.Caret.Column + 1);
			}
			return expression;
		}
		
        //void AddCompletionData(List<ICompletionData> resultList, ArrayList completionData)
        //{
        //    // used to store the method names for grouping overloads
        //    Dictionary<string, CodeCompletionData> nameDictionary = new Dictionary<string, CodeCompletionData>();
			
        //    // Add the completion data as returned by SharpDevelop.Dom to the
        //    // list for the text editor
        //    foreach (object obj in completionData) {
        //        if (obj is string) {
        //            // namespace names are returned as string
        //            resultList.Add(new DefaultCompletionData((string)obj, "namespace " + obj, 5));
        //        } else if (obj is Dom.IClass) {
        //            Dom.IClass c = (Dom.IClass)obj;
        //            resultList.Add(new CodeCompletionData(c));
        //        } else if (obj is Dom.IMember) {
        //            Dom.IMember m = (Dom.IMember)obj;
        //            if (m is Dom.IMethod && ((m as Dom.IMethod).IsConstructor)) {
        //                // Skip constructors
        //                continue;
        //            }
        //            // Group results by name and add "(x Overloads)" to the
        //            // description if there are multiple results with the same name.
					
        //            CodeCompletionData data;
        //            if (nameDictionary.TryGetValue(m.Name, out data)) {
        //                data.AddOverload();
        //            } else {
        //                nameDictionary[m.Name] = data = new CodeCompletionData(m);
        //                resultList.Add(data);
        //            }
        //        } else {
        //            // Current ICSharpCode.SharpDevelop.Dom should never return anything else
        //            throw new NotSupportedException();
        //        }
        //    }
        //}
	}
}
