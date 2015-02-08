using System;
using System.Collections.Generic;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Actions;
using ICSharpCode.TextEditor.Document;


namespace PAT.GUI.Docking
{
    public class CommentRegion : AbstractMenuCommand
    {
        public override void Run()
        {
            ITextEditorControlProvider viewContent = WorkbenchSingleton.ActiveControl as ITextEditorControlProvider;

            if(Common.Ultility.Ultility.IsUnixOS)
            {
                viewContent = FormMain.CurrentActiveTab;
            }

            if (viewContent == null)
            {
                return;
            }

            TextEditorControl textarea = viewContent.TextEditorControl;
            if (textarea.Document.HighlightingStrategy.Name != "MoML")
            {
                new ICSharpCode.TextEditor.Actions.ToggleComment().Execute(textarea.ActiveTextAreaControl.TextArea);
            }
            else
            {
                new ToggleComment().Execute(textarea.ActiveTextAreaControl.TextArea);    
            }

            
        }
    }
    public class ToggleComment : AbstractEditAction
    {
        /// <remarks>
        /// Executes this edit action
        /// </remarks>
        /// <param name="textArea">The <see cref="ItextArea"/> which is used for callback purposes</param>
        public override void Execute(TextArea textArea)
        {
            if (textArea.Document.ReadOnly)
            {
                return;
            }

            if (textArea.SelectionManager.HasSomethingSelected)
            {
                bool insideMoScript = false;

                List<KeyValuePair<int, int>> values = new List<KeyValuePair<int, int>>();
                int index = textArea.Document.TextContent.IndexOf("<moscript>", 0, StringComparison.CurrentCultureIgnoreCase);
                while (index != -1)
                {
                    int endindex = textArea.Document.TextContent.IndexOf("</moscript>", index + 1, StringComparison.CurrentCultureIgnoreCase);
                    if (endindex != -1)
                    {
                        KeyValuePair<int, int> pair = new KeyValuePair<int, int>(index, endindex);
                        values.Add(pair);
                    }

                    index = textArea.Document.TextContent.IndexOf("<moscript>", index + 1, StringComparison.CurrentCultureIgnoreCase);
                }

                foreach (ISelection selection in textArea.SelectionManager.SelectionCollection)
                {
                    foreach (KeyValuePair<int, int> pair in values)
                    {
                        if (selection.Offset > pair.Key && selection.EndOffset < pair.Value)
                        {
                            insideMoScript = true;
                            break;
                        }
                    }
                }

                if (insideMoScript)
                {
                    if (textArea.Document.HighlightingStrategy.Properties.ContainsKey("LineComment"))
                    {
                        new ToggleLineComment().Execute(textArea);
                    }
                }
                else
                {
                    if (textArea.Document.HighlightingStrategy.Properties.ContainsKey("BlockCommentBegin") && textArea.Document.HighlightingStrategy.Properties.ContainsKey("BlockCommentBegin"))
                    {
                        new ToggleBlockComment().Execute(textArea);
                    }
                }

            }


        }
    }
}
