using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Document;
using ICSharpCode.TextEditor.Gui.CompletionWindow;
using ICSharpCode.TextEditor.Util;
using PAT.Common.Classes.Expressions.ExpressionClass;
using PAT.GUI.Docking;
using Dom = ICSharpCode.SharpDevelop.Dom;

namespace PAT.GUI.EditingFunction.CodeCompletion
{

    
	class CodeCompletionProvider : ICompletionDataProvider
	{
        EditorTabItem mainForm;
	    private string preSelection;
        private int  defaultIndex = -1;

        public CodeCompletionProvider(EditorTabItem mainForm, string key)
		{
			this.mainForm = mainForm;
            preSelection = key;
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
				return defaultIndex;
			}
		}


        public CompletionDataProviderKeyResult ProcessKey(char key)
		{
            return CompletionDataProviderKeyResult.NormalKey;
            //if (char.IsLetterOrDigit(key) || key == '_' || key == ' ') {
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
            if (string.IsNullOrEmpty(expression))
            {
                if (char.IsLetter(charTyped) || charTyped == '#')
                {
                    wordTyped = "";
                    return GenerateCompletionList(charTyped);
                }
            }

            return null;
        }

        private string wordTyped; 

	    public ICompletionData[] GenerateCompletionList(char key)
	    {
            List<ICompletionData> data = new List<ICompletionData>();
            if(key == '#')
            {
                defaultIndex = 0;
                data.Add(new DefaultCompletionData("#import", "import a library (under Lib folder of PAT installation path): #import \"PAT.Lib.Hashtable\";", 1));
                data.Add(new DefaultCompletionData("#include", "include a model (with absolute path or relative path) to the current model: #include \"C:\\example.csp\";", 1));
                data.Add(new DefaultCompletionData("#assert", "define an assertion: #assert process property;", 1));
                data.Add(new DefaultCompletionData("#define", "define a constant or condition: #assert process property", 1));
                data.Add(new DefaultCompletionData("#alphabet", "define an alphabet of a process: #alphabet P {event1, event2,...};", 1));

                if (mainForm.CodeEditor.Document.HighlightingStrategy.Name == "Parameterized Real-Time System Model")
                {
                    data.Add(new DefaultCompletionData("#synthesize", "synthesize the constraints of a model which satisfies a property.", 1));
                }
                else if (mainForm.CodeEditor.Document.HighlightingStrategy.Name == "Probability CSP Model" || mainForm.CodeEditor.Document.HighlightingStrategy.Name == "Probabilistic Real-Time System Model")
                {
                    data.Add(new DefaultCompletionData("#reward", "decleare the rewards of the model.", 1));
                }  
                
            }
            else
            {
                wordTyped = wordTyped + key;
                defaultIndex = -1;
               
                DefaultHighlightingStrategy strategy = mainForm.CodeEditor.Document.HighlightingStrategy as DefaultHighlightingStrategy;

                List<LookupTable.Node> keywords = new List<LookupTable.Node>();
                foreach (HighlightRuleSet rule in strategy.Rules)
                {
                    foreach (LookupTable.Node node in rule.KeyWords.GetAllWords())
                    {
                        if(!keywords.Contains(node))
                        {
                            keywords.Add(node);
                        }
                    }                    
                }

                if(mainForm.Specification != null)
                {
                    if(mainForm.Specification.DeclarationDatabase != null)
                    {
                        foreach (KeyValuePair<string, Expression> pair in mainForm.Specification.DeclarationDatabase)
                        {
                            if (pair.Key.StartsWith(wordTyped) && defaultIndex == -1)
                            {
                                defaultIndex = data.Count;
                            }
                            data.Add(new DefaultCompletionData(pair.Key, pair.Value.ToString(), 3));
                        }    
                    }

                    if (mainForm.Specification.GlobalConstantDatabase != null)
                    {
                        foreach (KeyValuePair<string, Expression> pair in mainForm.Specification.GlobalConstantDatabase)
                        {
                            if (pair.Key.StartsWith(wordTyped) && defaultIndex == -1)
                            {
                                defaultIndex = data.Count;
                            }
                            data.Add(new DefaultCompletionData(pair.Key, pair.Value.ToString(), 6));
                        }
                    }


                    foreach (string pair in mainForm.Specification.GetAllProcessNames())
                    {
                        if (pair.StartsWith(wordTyped) && defaultIndex == -1)
                        {
                            defaultIndex = data.Count;
                        }
                        data.Add(new DefaultCompletionData(pair, "Process " + pair, 0));
                    }


                    foreach (string pair in mainForm.Specification.GetGlobalVarNames())
                    {
                        if (pair.StartsWith(wordTyped) && defaultIndex == -1)
                        {
                            defaultIndex = data.Count;
                        }
                        data.Add(new DefaultCompletionData(pair, "Global variable " + pair, 1));
                    }

                    foreach (string pair in mainForm.Specification.GetChannelNames())
                    {
                        if (pair.StartsWith(wordTyped) && defaultIndex == -1)
                        {
                            defaultIndex = data.Count;
                        }
                        data.Add(new DefaultCompletionData(pair, "Global channel "+ pair, 2));
                    }
                }

                Color c = Color.FromArgb(43, 145, 175);
                foreach (LookupTable.Node keyword in keywords)
                {
                    if (keyword.word.Length > 1 && keyword.word != "import" && keyword.word != "define" && keyword.word != "alphabet")
                    {
                        if (keyword.word.StartsWith(wordTyped) && defaultIndex == -1)
                        {
                            defaultIndex = data.Count;
                        }
                        
                        if(((HighlightColor)keyword.color).Color == Color.MidnightBlue)
                        {
                            data.Add(new DefaultCompletionData(keyword.word, keyword.description, 0));
                        }
                        else if (((HighlightColor)keyword.color).Color == c)
                        {
                            data.Add(new DefaultCompletionData(keyword.word, keyword.description, 5));
                        }
                        else
                        {
                            data.Add(new DefaultCompletionData(keyword.word, keyword.description, 4));    
                        }
                        
                    }
                }
            }

            if (defaultIndex != -1)
            {
                return data.ToArray();
            }

	        return null;
	    }

	    /// <summary>
		/// Find the expression the cursor is at.
		/// Also determines the context (using statement, "new"-expression etc.) the
		/// cursor is at.
		/// </summary>
		Dom.ExpressionResult FindExpression(TextArea textArea)
		{
			Dom.IExpressionFinder finder = new Dom.CSharp.CSharpExpressionFinder(mainForm.parseInformation);
            
			Dom.ExpressionResult expression = finder.FindExpression(textArea.Document.TextContent, textArea.Caret.Offset);
			if (expression.Region.IsEmpty) {
				expression.Region = new Dom.DomRegion(textArea.Caret.Line + 1, textArea.Caret.Column + 1);
			}

            //the previous char must be empty.
            if (textArea.Caret.Offset > 0)
            {
                string text = textArea.Document.GetText(textArea.Caret.Offset - 1, 1);
                //if (char.IsLetterOrDigit(text[0]) || text == "'" || text == ".")
                if (text != " " && text != "\n")
                {
                    expression.Expression = "xxx";
                }
                else
                {
                    expression.Expression = "";
                }
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
