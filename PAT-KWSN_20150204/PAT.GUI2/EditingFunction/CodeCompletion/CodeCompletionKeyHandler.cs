using System;
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Gui.CompletionWindow;
using ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor;
using PAT.GUI.Docking;
using ICSharpCode.NRefactory;

namespace PAT.GUI.EditingFunction.CodeCompletion
{
    class CodeCompletionKeyHandler : NRefactoryCodeCompletionBinding
	{
        EditorTabItem mainForm;
		TextEditorControl editor;
		CodeCompletionWindow codeCompletionWindow;

        private CodeCompletionKeyHandler(EditorTabItem mainForm, TextEditorControl editor)
            : base(SupportedLanguage.CSharp)
		{
			this.mainForm = mainForm;
			this.editor = editor;
		}

        public static CodeCompletionKeyHandler Attach(EditorTabItem mainForm, TextEditorControl editor)
		{
			CodeCompletionKeyHandler h = new CodeCompletionKeyHandler(mainForm, editor);
			
			editor.ActiveTextAreaControl.TextArea.KeyEventHandler += h.TextAreaKeyEventHandler;
			
			// When the editor is disposed, close the code completion window
			editor.Disposed += h.CloseCodeCompletionWindow;
			
			return h;
		}
		
		/// <summary>
		/// Return true to handle the keypress, return false to let the text area handle the keypress
		/// </summary>
        bool TextAreaKeyEventHandler(char key)
		{
		    try
		    {


		        if (codeCompletionWindow != null)
		        {
		            // If completion window is open and wants to handle the key, don't let the text area
		            // handle it
		            System.Diagnostics.Debug.WriteLine("---" + key);

		            if (codeCompletionWindow.ProcessKeyEvent(key))
		            {
		                System.Diagnostics.Debug.WriteLine("---" + key + "===");
		                return true;
		            }
		            else
		            {
		                if (codeCompletionWindow != null && codeCompletionWindow.dataProvider is CodeCompletionProvider)
		                {
		                    System.Diagnostics.Debug.WriteLine("---" + key + "===inin");
		                    ICompletionData[] data = (codeCompletionWindow.dataProvider as CodeCompletionProvider).GenerateCompletionList(key);
		                    if (data == null)
		                    {
		                        System.Diagnostics.Debug.WriteLine("---" + key + "===close");
		                        codeCompletionWindow.Close();
		                        //codeCompletionWindow = null;

		                    }
		                    return false;
		                }
		            }
		        }

		        //bool insideMoScript = false;

		        //List<KeyValuePair<int, int>> values = new List<KeyValuePair<int, int>>();
		        //int index = editor.Document.TextContent.IndexOf("<moscript>", 0, StringComparison.CurrentCultureIgnoreCase);
		        //while (index != -1)
		        //{
		        //    int endindex = editor.Document.TextContent.IndexOf("</moscript>", index + 1, StringComparison.CurrentCultureIgnoreCase);
		        //    if (endindex != -1)
		        //    {
		        //        KeyValuePair<int, int> pair = new KeyValuePair<int, int>(index, endindex);
		        //        values.Add(pair);
		        //    }

		        //    index = editor.Document.TextContent.IndexOf("<moscript>", index + 1, StringComparison.CurrentCultureIgnoreCase);
		        //}

		        //foreach (KeyValuePair<int, int> pair in values)
		        //{
		        //    if (editor.ActiveTextAreaControl.Caret.Offset > pair.Key && editor.ActiveTextAreaControl.Caret.Offset < pair.Value)
		        //    {
		        //        insideMoScript = true;
		        //        break;
		        //    }
		        //}


		        //if (insideMoScript)
		        //{
		        //if (key == '.')
		        //{
		        //    ICompletionDataProvider completionDataProvider = new CodeCompletionProviderDot(mainForm);

		        //    codeCompletionWindow = CodeCompletionWindow.ShowCompletionWindow(
		        //        mainForm, // The parent window for the completion window
		        //        editor, // The text editor to show the window for
		        //        "x.cs", // Filename - will be passed back to the provider
		        //        completionDataProvider, // Provider to get the list of possible completions
		        //        key // Key pressed - will be passed to the provider
		        //        );



		        //    if (codeCompletionWindow != null)
		        //    {
		        //        // ShowCompletionWindow can return null when the provider returns an empty list
		        //        codeCompletionWindow.Closed += new EventHandler(CloseCodeCompletionWindow);

		        //    }
		        //}

		        try
		        {
                    var seg = editor.Document.GetLineSegment(editor.ActiveTextAreaControl.Caret.Line);
                    string textline = editor.Document.GetText(seg);
		            int index = textline.IndexOf("//");

                    if (index != -1 && index < editor.ActiveTextAreaControl.Caret.Offset)
                    {
                        return false;
                    }
		        }
		        catch (Exception)
		        {
		            
		        }
		        
		      
                if (char.IsLetter(key) || key == '#')
		        {

		            ICompletionDataProvider completionDataProvider = new CodeCompletionProvider(mainForm, key.ToString());

		            codeCompletionWindow = CodeCompletionWindow.ShowCompletionWindow(
		                mainForm, // The parent window for the completion window
                        editor, // The text editor to show the window for
		                "x.cs", // Filename - will be passed back to the provider
		                completionDataProvider, // Provider to get the list of possible completions
		                key // Key pressed - will be passed to the provider
		                );



		            if (codeCompletionWindow != null)
		            {
		                // ShowCompletionWindow can return null when the provider returns an empty list
		                codeCompletionWindow.Closed += new EventHandler(CloseCodeCompletionWindow);
		                codeCompletionWindow.SetPre();
		                //return true;
		            }
		        }
                else if (key == '(' && mainForm.Specification != null)
                {
                    //if (EnableMethodInsight && CodeCompletionOptions.InsightEnabled)
                    {
                        mainForm.CodeEditor.ShowInsightWindow(new MethodInsightDataProvider(mainForm));
                        return false;
                    }
                }
                if (key == ',') //&& CodeCompletionOptions.InsightRefreshOnComma && CodeCompletionOptions.InsightEnabled
                {
                    mainForm.CodeEditor.ShowInsightWindow(new MethodInsightDataProvider(mainForm));
                    return false;
                }


		        // }
		    }
		    catch (Exception ex)
		    {
		        string msg = ex.Message;
		    }

		    return false;
		}

        void CloseCodeCompletionWindow(object sender, EventArgs e)
		{
			if (codeCompletionWindow != null) {
				codeCompletionWindow.Closed -= new EventHandler(CloseCodeCompletionWindow);
				codeCompletionWindow.Dispose();
				codeCompletionWindow = null;
			}
		}
	}
}
