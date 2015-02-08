using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Windows.Forms;
using Fireball.Docking;
using ICSharpCode.Core;
using ICSharpCode.Core.WinForms;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Document;
using PAT.Common;
using PAT.Common.Classes.ModuleInterface;
using PAT.Common.Ultility;
using PAT.GUI.EditingFunction.CodeCompletion;
using ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor;
using PAT.GUI.Forms;
using PAT.GUI.Properties;
using HelpProvider = ICSharpCode.SharpDevelop.HelpProvider;

namespace PAT.GUI.Docking
{
    public class EditorTabItem : DockableWindow, ITextEditorControlProvider, IViewContent, IMementoCapable, IPrintable, IEditable, IUndoHandler, IPositionable, IParseInformationListener, IClipboardHandler, IContextHelpProvider//, IToolsHost
    {
        public delegate void TabActivitedHandler(EditorTabItem tab);
        public event TabActivitedHandler TabActivited;

        public delegate void FindUsagesHandler(string name, List<ParsingException> usages, SpecificationBase spec);
        public event FindUsagesHandler FindUsages;

        public delegate void GoToDeclaritionHandler(ParsingException declaration);
        public event GoToDeclaritionHandler GoToDeclarition;

        public ContextMenuStrip EditorContextMenuStrip;
        private IContainer components;
        private ToolStripMenuItem mnuUndo;
        private ToolStripMenuItem mnuRedo;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripMenuItem mnuCut;
        private ToolStripMenuItem mnuCopy;
        private ToolStripMenuItem mnuPaste;
        private ToolStripMenuItem mnuSelectAll;
        protected static int counter = 1;

        public event EventHandler FileSaved;

        public string FileExtension;
        private ToolStripSeparator toolStripSeparator3;
        private ToolStripMenuItem mnuComment;
        private ToolStripMenuItem mnuUnComment;
        private ToolStripMenuItem mnuFindUsage;
        private ToolStripMenuItem mnuOpenImportFile;
        private ToolStripMenuItem mnuGoToDeclarition;
        private ToolStripSeparator toolStripSeparator2;
        private ToolStripMenuItem mnuRename;

        public ToolStripMenuItem WindowMenuItem = new ToolStripMenuItem();
        public ImageList imageList1;
        public string ModuleName;
        public SpecificationBase Specification;

        public ProjectContentRegistry pcRegistry;
        public DefaultProjectContent myProjectContent;
        public ParseInformation parseInformation = new ParseInformation();
        public bool HaveFileName;

        public override string TabText
        {
            get { return DockHandler.TabText; }
            set
            {
                DockHandler.TabText = value;
                WindowMenuItem.Text = value;
            }
        }

        public override void ActivateUpdate()
        {
            TabActivited(this);
        }

        protected EditorTabItem()
        {
            InitializeComponent();
        }

        public EditorTabItem(string moduleName)
        {
            InitializeComponent();

            textEditorControl = new SharpDevelopTextAreaControl();
            textEditorControl.Dock = DockStyle.Fill;
            textEditorControl.ContextMenuStrip = EditorContextMenuStrip;
            textEditorControl.BorderStyle = BorderStyle.Fixed3D;

            this.Controls.Add(textEditorControl);
            this.TabText = Resources.Document_ + counter;
            counter++;

            textEditorControl.FileNameChanged += new EventHandler(_EditorControl_FileNameChanged);
            textEditorControl.TextChanged += new EventHandler(textEditorControl_TextChanged);
            textEditorControl.Tag = this;

            this.Padding = new Padding(2, 2, 2, 2);
            this.DockableAreas = DockAreas.Document;

            secondaryViewContentCollection = new SecondaryViewContentCollection(this);
            InitFiles();

            file = FileService.CreateUntitledOpenedFile(TabText, new byte[] { });
            file.CurrentView = this;
            textEditorControl.FileName = file.FileName;

            files.Clear();
            files.Add(file);

            this.SetSyntaxLanguage(moduleName);

            textEditorControl.Document.FoldingManager.FoldingStrategy = new FoldingStrategy();

            // Highlight the matching bracket or not...
            this.textEditorControl.ShowMatchingBracket = true;

            this.textEditorControl.BracketMatchingStyle = BracketMatchingStyle.Before;

            //there is no code completion for Mono
            if (Common.Ultility.Ultility.IsWindowsOS)
            {
                HostCallbackImplementation.Register(this);
                CodeCompletionKeyHandler.Attach(this, textEditorControl);
                ToolTipProvider.Attach(this, textEditorControl);

                pcRegistry = new ProjectContentRegistry(); // Default .NET 2.0 registry

                // Persistence lets SharpDevelop.Dom create a cache file on disk so that
                // future starts are faster.
                // It also caches XML documentation files in an on-disk hash table, thus
                // reducing memory usage.
                pcRegistry.ActivatePersistence(Path.Combine(Path.GetTempPath(), "CSharpCodeCompletion"));

                myProjectContent = new DefaultProjectContent();
                myProjectContent.Language = LanguageProperties.CSharp;
            }
        }

        #region NewEditor
        public virtual string FileName
        {
            get { return file.FileName; }
        }

        protected SharpDevelopTextAreaControl textEditorControl;
        public SharpDevelopTextAreaControl CodeEditor
        {
            get { return textEditorControl; }
        }

        private ToolStripSeparator toolStripSeparator4;

        protected void textEditorControl_TextChanged(object sender, EventArgs e)
        {
            textEditorControl.Document.FoldingManager.UpdateFoldings(null, null);
            SetDirty();
        }

        protected void SetDirty()
        {
            if (!TabText.EndsWith("*"))
                TabText = TabText + "*";

            RaiseIsDirtyChanged();
        }

        void _EditorControl_FileSaved(object sender, EventArgs e)
        {
            if (FileSaved != null)
                FileSaved(sender, e);
        }

        protected void _EditorControl_FileNameChanged(object sender, EventArgs e)
        {
            this.TabText = Path.GetFileName(this.textEditorControl.FileName);
        }

        protected OpenedFile file;
        public virtual void Open(string filename)
        {
            file = FileService.GetOrCreateOpenedFile(filename);
            files.Clear();
            files.Add(file);

            file.CurrentView = this;
            this.Load(file, file.OpenRead());

            textEditorControl.InitializeFormatter();
            textEditorControl.ActivateQuickClassBrowserOnDemand();
            file.CloseIfAllViewsClosed();

            this.ToolTipText = filename;
            TabText = Path.GetFileName(filename);
            HaveFileName = true;
        }

        public virtual void Save(string filename)
        {
            if (string.IsNullOrEmpty(filename) || filename == file.FileName)
                file.SaveToDisk();
            else
                file.SaveToDisk(filename);
            HaveFileName = true;
        }

        public void SetSyntaxLanguageFromFile(string file)
        {
            try
            {
                IHighlightingStrategy strategy = HighlightingStrategyFactory.CreateHighlightingStrategyForFile(file);
                if (strategy != null)
                {
                    textEditorControl.Document.HighlightingStrategy = strategy;
                    textEditorControl.InitializeAdvancedHighlighter();
                    ModuleName = strategy.Name;
                    this.Icon = Ultility.GetModuleIcon(strategy.Name);
                    FileExtension = strategy.Name + " (*" + string.Join(";", strategy.Extensions) + ")|*" +
                                    string.Join(";", strategy.Extensions) + "|All File (*.*)|*.*";
                }
            }
            catch (HighlightingDefinitionInvalidException)
            {
                MessageBox.Show(Resources.Error__file_format_is_not_supported_, Common.Ultility.Ultility.APPLICATION_NAME, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public virtual void SetSyntaxLanguage(string languageName)
        {
            try
            {
                IHighlightingStrategy strategy = HighlightingStrategyFactory.CreateHighlightingStrategy(languageName);
                textEditorControl.Document.HighlightingStrategy = strategy;
                textEditorControl.InitializeAdvancedHighlighter();
                ModuleName = strategy.Name;
                this.Icon = Ultility.GetModuleIcon(languageName);
                FileExtension = strategy.Name + " (*" + string.Join(";", strategy.Extensions) + ")|*" +
                                string.Join(";", strategy.Extensions) + "|All File (*.*)|*.*";
            }
            catch (HighlightingDefinitionInvalidException)
            {
                MessageBox.Show(Resources.Error__file_format_is_not_supported_, Common.Ultility.Ultility.APPLICATION_NAME, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void contextMenuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if (e.ClickedItem == mnuCopy)
                textEditorControl.ActiveTextAreaControl.TextArea.ClipboardHandler.Copy(null, null);
            else if (e.ClickedItem == mnuCut && !this.textEditorControl.IsReadOnly)
                textEditorControl.ActiveTextAreaControl.TextArea.ClipboardHandler.Cut(null, null);
            else if (e.ClickedItem == mnuPaste && !this.textEditorControl.IsReadOnly)
                textEditorControl.ActiveTextAreaControl.TextArea.ClipboardHandler.Paste(null, null);
            else if (e.ClickedItem == mnuUndo && !this.textEditorControl.IsReadOnly)
                textEditorControl.Undo();
            else if (e.ClickedItem == mnuRedo && !this.textEditorControl.IsReadOnly)
                textEditorControl.Redo();
            else if (e.ClickedItem == mnuSelectAll)
                textEditorControl.ActiveTextAreaControl.TextArea.ClipboardHandler.SelectAll(null, null);
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            try
            {
                mnuCut.Enabled = textEditorControl.ActiveTextAreaControl.TextArea.ClipboardHandler.EnableCut && !this.textEditorControl.IsReadOnly;
                mnuCopy.Enabled = textEditorControl.ActiveTextAreaControl.TextArea.ClipboardHandler.EnableCopy;
                mnuPaste.Enabled = textEditorControl.ActiveTextAreaControl.TextArea.ClipboardHandler.EnablePaste && !this.textEditorControl.IsReadOnly;
                mnuRedo.Enabled = textEditorControl.EnableRedo && !this.textEditorControl.IsReadOnly;
                mnuUndo.Enabled = textEditorControl.EnableUndo && !this.textEditorControl.IsReadOnly;
                mnuSelectAll.Enabled = textEditorControl.ActiveTextAreaControl.TextArea.ClipboardHandler.EnableSelectAll;

                SelectionManager selectionManager = textEditorControl.ActiveTextAreaControl.TextArea.SelectionManager;
                if (!string.IsNullOrEmpty(selectionManager.SelectedText))
                {
                    this.mnuGoToDeclarition.Enabled = true;
                    this.mnuFindUsage.Enabled = true;
                    this.mnuOpenImportFile.Enabled = true;
                }
                else
                {
                    this.mnuGoToDeclarition.Enabled = false;
                    this.mnuFindUsage.Enabled = false;
                    this.mnuOpenImportFile.Enabled = false;
                }
            }
            catch (Exception) { }
        }


        public void ToggleOutline()
        {
            ICSharpCode.TextEditor.Actions.ToggleAllFoldings action = new ICSharpCode.TextEditor.Actions.ToggleAllFoldings();
            action.Execute(this.TextEditorControl.ActiveTextAreaControl.TextArea);
        }

        #region Editing Functions
        public virtual bool CanUndo
        {
            get { return this.CodeEditor.EnableUndo; }
        }

        public virtual bool CanRedo
        {
            get { return this.CodeEditor.EnableRedo; }
        }

        public virtual bool CanCut
        {
            get { return this.EnableCut; }
        }

        public virtual bool CanCopy
        {
            get { return this.EnableCopy; }
        }

        public virtual bool CanDelete
        {
            get { return true; }
        }

        public virtual bool CanPaste
        {
            get { return this.EnablePaste; }
        }

        public virtual bool CanSelectAll
        {
            get { return this.CodeEditor.CanSelect; }
        }

        public virtual bool CanFind
        {
            get { return true; }
        }

        public virtual bool CanPrint
        {
            get { return true; }
        }

        public virtual void Undo()
        {
            this.CodeEditor.Undo();
        }

        public virtual void Redo()
        {
            this.CodeEditor.Redo();
        }

        public virtual void Cut()
        {
            textEditorControl.ActiveTextAreaControl.TextArea.ClipboardHandler.Cut(null, null);
        }

        public virtual void Copy()
        {
            textEditorControl.ActiveTextAreaControl.TextArea.ClipboardHandler.Copy(null, null);
        }

        public virtual void Paste()
        {
            textEditorControl.ActiveTextAreaControl.TextArea.ClipboardHandler.Paste(null, null);
        }

        public virtual void SelectAll()
        {
            textEditorControl.ActiveTextAreaControl.TextArea.ClipboardHandler.SelectAll(null, null);
        }

        public virtual void Delete()
        {
            textEditorControl.ActiveTextAreaControl.TextArea.ClipboardHandler.Delete(null, null);
        }

        public virtual void ZoomOut()
        {
            if (this.textEditorControl.TextEditorProperties.Font.Size > 2)
            {
                this.textEditorControl.TextEditorProperties.Font = new Font(this.textEditorControl.TextEditorProperties.Font.Name, this.textEditorControl.TextEditorProperties.Font.Size - 1);
                this.textEditorControl.OptionsChanged();
            }
        }

        public virtual void ZoomIn()
        {
            if (this.textEditorControl.TextEditorProperties.Font.Size < 100)
            {
                this.textEditorControl.TextEditorProperties.Font = new Font(this.textEditorControl.TextEditorProperties.Font.Name, this.textEditorControl.TextEditorProperties.Font.Size + 1);
                this.textEditorControl.OptionsChanged();
            }
        }

        public virtual void Zoom100()
        {
            if (this.textEditorControl.TextEditorProperties.Font.Size != 10)
            {
                this.textEditorControl.TextEditorProperties.Font = new Font(this.textEditorControl.TextEditorProperties.Font.Name, 10);
                this.textEditorControl.OptionsChanged();
            }
        }

        public virtual void SplitWindow()
        {
            textEditorControl.Split();
        }
        #endregion

        #region Function Inherited From SharpDevelopment
        public bool EnableUndo
        {
            get { return textEditorControl.EnableUndo; }
        }

        public bool EnableRedo
        {
            get { return textEditorControl.EnableRedo; }
        }

        // ParserUpdateThread uses the text property via IEditable, I had an exception
        // because multiple threads were accessing the GapBufferStrategy at the same time.
        public virtual string GetText()
        {
            return textEditorControl.Document.TextContent;
        }

        public virtual void SetText(string value)
        {
            textEditorControl.Document.Replace(0, textEditorControl.Document.TextLength, value);
        }

        public new string Text
        {
            get
            {
                if (WorkbenchSingleton.InvokeRequired)
                    return WorkbenchSingleton.SafeThreadFunction<string>(GetText);
                else
                    return GetText();
            }
            set
            {
                if (WorkbenchSingleton.InvokeRequired)
                    WorkbenchSingleton.SafeThreadCall(SetText, value);
                else
                    SetText(value);
            }
        }

        public PrintDocument PrintDocument
        {
            get { return textEditorControl.PrintDocument; }
        }

        public void ShowHelp()
        {
            // Resolve expression at cursor and show help
            TextArea textArea = textEditorControl.ActiveTextAreaControl.TextArea;
            IDocument doc = textArea.Document;
            IExpressionFinder expressionFinder = ParserService.GetExpressionFinder(textArea.MotherTextEditorControl.FileName);
            if (expressionFinder == null)
                return;

            LineSegment seg = doc.GetLineSegment(textArea.Caret.Line);
            string textContent = doc.TextContent;
            ExpressionResult expressionResult = expressionFinder.FindFullExpression(textContent, seg.Offset + textArea.Caret.Column);
            string expression = expressionResult.Expression;
            if (!string.IsNullOrEmpty(expression))
            {
                ResolveResult result = ParserService.Resolve(expressionResult, textArea.Caret.Line + 1, textArea.Caret.Column + 1, textEditorControl.FileName, textContent);
                TypeResolveResult trr = result as TypeResolveResult;
                if (trr != null)
                    HelpProvider.ShowHelp(trr.ResolvedClass);

                MemberResolveResult mrr = result as MemberResolveResult;
                if (mrr != null)
                    HelpProvider.ShowHelp(mrr.ResolvedMember);
            }
        }

        void TextAreaChangedEvent(object sender, DocumentEventArgs e)
        {
            this.PrimaryFile.MakeDirty();
            NavigationService.ContentChanging(this.textEditorControl, e);
        }

        public void RedrawContent()
        {
            textEditorControl.OptionsChanged();
            textEditorControl.Refresh();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (PrimaryFile != null && this.PrimaryFile.IsUntitled)
                    ParserService.ClearParseInformation(this.PrimaryFile.FileName);
                textEditorControl.Dispose();

                workbenchWindow = null;
                UnregisterOnActiveViewContentChanged();
                if (AutomaticallyRegisterViewOnFiles)
                    this.Files.Clear();
            }

            base.Dispose(disposing);
        }

        public bool IsReadOnly
        {
            get { return textEditorControl.IsReadOnly; }
            set { textEditorControl.IsReadOnly = value; }
        }

        public bool IsViewOnly
        {
            get { return false; }
        }

        public virtual void Save(OpenedFile file1, Stream stream)
        {
            if (!textEditorControl.CanSaveWithCurrentEncoding())
            {
                if (MessageService.AskQuestion("The file cannot be saved with the current encoding " +
                                               textEditorControl.Encoding.EncodingName + " without losing data." +
                                               "\nDo you want to save it using UTF-8 instead?"))
                {
                    textEditorControl.Encoding = System.Text.Encoding.UTF8;
                }
            }

            textEditorControl.SaveFile(stream);
        }

        public new void Load(OpenedFile file1, Stream stream)
        {
            if (!file1.IsUntitled)
                textEditorControl.IsReadOnly = (File.GetAttributes(file1.FileName) & FileAttributes.ReadOnly) == FileAttributes.ReadOnly;

            bool autodetectEncoding = true;
            textEditorControl.LoadFile(file1.FileName, stream, true, autodetectEncoding);
            textEditorControl.FileLoaded();
            file1.SetData(ParserService.DefaultFileEncoding.GetBytesWithPreamble(textEditorControl.Text));
            foreach (ICSharpCode.SharpDevelop.Bookmarks.SDBookmark mark in ICSharpCode.SharpDevelop.Bookmarks.BookmarkManager.GetBookmarks(file1.FileName))
            {
                mark.Document = textEditorControl.Document;
                textEditorControl.Document.BookmarkManager.AddMark(mark);
            }
        }

        public ICSharpCode.Core.Properties CreateMemento()
        {
            ICSharpCode.Core.Properties properties = new ICSharpCode.Core.Properties();
            properties.Set("CaretOffset", textEditorControl.ActiveTextAreaControl.Caret.Offset);
            properties.Set("VisibleLine", textEditorControl.ActiveTextAreaControl.TextArea.TextView.FirstVisibleLine);
            if (textEditorControl.HighlightingExplicitlySet)
                properties.Set("HighlightingLanguage", textEditorControl.Document.HighlightingStrategy.Name);

            return properties;
        }

        public void SetMemento(ICSharpCode.Core.Properties properties)
        {
            textEditorControl.ActiveTextAreaControl.Caret.Position = textEditorControl.Document.OffsetToPosition(Math.Min(textEditorControl.Document.TextLength, Math.Max(0, properties.Get("CaretOffset", textEditorControl.ActiveTextAreaControl.Caret.Offset))));
            string highlightingName = properties.Get("HighlightingLanguage", string.Empty);
            if (!string.IsNullOrEmpty(highlightingName))
            {
                if (highlightingName == textEditorControl.Document.HighlightingStrategy.Name)
                    textEditorControl.HighlightingExplicitlySet = true;
                else
                {
                    IHighlightingStrategy highlightingStrategy = HighlightingStrategyFactory.CreateHighlightingStrategy(highlightingName);
                    if (highlightingStrategy != null)
                    {
                        textEditorControl.HighlightingExplicitlySet = true;
                        textEditorControl.Document.HighlightingStrategy = highlightingStrategy;
                    }
                }
            }

            textEditorControl.ActiveTextAreaControl.TextArea.TextView.FirstVisibleLine = properties.Get("VisibleLine", 0);
        }

        public INavigationPoint BuildNavPoint()
        {
            int lineNumber = this.Line;
            LineSegment lineSegment = textEditorControl.Document.GetLineSegment(lineNumber);
            string txt = textEditorControl.Document.GetText(lineSegment);

            return new TextNavigationPoint(this.PrimaryFileName, lineNumber, this.Column, txt);
        }

        void CaretUpdate(object sender, EventArgs e)
        {
            CaretChanged(null, null);
            CaretModeChanged(null, null);
        }

        void CaretChanged(object sender, EventArgs e)
        {
            TextAreaControl activeTextAreaControl = textEditorControl.ActiveTextAreaControl;
            int line = activeTextAreaControl.Caret.Line;
            int col = activeTextAreaControl.Caret.Column;
            StatusBarService.SetCaretPosition(activeTextAreaControl.TextArea.TextView.GetVisualColumn(line, col) + 1, line + 1, col + 1);
            NavigationService.Log(this.BuildNavPoint());
        }

        void CaretModeChanged(object sender, EventArgs e)
        {
            StatusBarService.SetInsertMode(textEditorControl.ActiveTextAreaControl.Caret.CaretMode == CaretMode.InsertMode);
        }

        protected void OnFileNameChanged(OpenedFile file)
        {
            Debug.Assert(file == this.Files[0]);

            string oldFileName = textEditorControl.FileName;
            string newFileName = file.FileName;

            if (Path.GetExtension(oldFileName) != Path.GetExtension(newFileName))
            {
                if (textEditorControl.Document.HighlightingStrategy != null)
                {
                    textEditorControl.Document.HighlightingStrategy = HighlightingStrategyFactory.CreateHighlightingStrategyForFile(newFileName);
                    textEditorControl.Refresh();
                }
            }

            SetIcon();

            ParserService.ClearParseInformation(oldFileName);
            textEditorControl.FileName = newFileName;
            ParserService.ParseViewContent(this);
        }

        protected void OnWorkbenchWindowChanged()
        {
            SetIcon();
        }

        void SetIcon()
        {
            if (this.WorkbenchWindow != null)
            {
                System.Drawing.Icon icon = WinFormsResourceService.GetIcon(IconService.GetImageForFile(this.PrimaryFileName));
                if (icon != null)
                {
                    this.WorkbenchWindow.Icon = icon;
                }
            }
        }

        #region IPositionable implementation
        public void JumpTo(int line, int column)
        {
            textEditorControl.ActiveTextAreaControl.JumpTo(line, column);

            // we need to delay this call here because the text editor does not know its height if it was just created
            WorkbenchSingleton.SafeThreadAsyncCall(
                delegate
                {
                    textEditorControl.ActiveTextAreaControl.CenterViewOn(
                        line, (int)(0.3 * textEditorControl.ActiveTextAreaControl.TextArea.TextView.VisibleLineCount));
                });
        }

        public int Line
        {
            get { return textEditorControl.ActiveTextAreaControl.Caret.Line; }
        }

        public int Column
        {
            get { return textEditorControl.ActiveTextAreaControl.Caret.Column; }
        }
        #endregion

        public void ForceFoldingUpdate()
        {
            if (textEditorControl.TextEditorProperties.EnableFolding)
            {
                string fileName = file.FileName;
                ParseInformation parseInfo = ParserService.GetParseInformation(fileName);
                if (parseInfo == null)
                    parseInfo = ParserService.ParseFile(fileName, textEditorControl.Document.TextContent, false);

                textEditorControl.Document.FoldingManager.UpdateFoldings(fileName, parseInfo);
                UpdateClassMemberBookmarks(parseInfo);
            }
        }

        public void ParseInformationUpdated(ParseInformation parseInfo)
        {
            if (textEditorControl.TextEditorProperties.EnableFolding)
                WorkbenchSingleton.SafeThreadAsyncCall(ParseInformationUpdatedInvoked, parseInfo);
        }

        void ParseInformationUpdatedInvoked(ParseInformation parseInfo)
        {
            try
            {
                textEditorControl.Document.FoldingManager.UpdateFoldings(TitleName, parseInfo);
                UpdateClassMemberBookmarks(parseInfo);
                textEditorControl.ActiveTextAreaControl.TextArea.Refresh(textEditorControl.ActiveTextAreaControl.TextArea.FoldMargin);
                textEditorControl.ActiveTextAreaControl.TextArea.Refresh(textEditorControl.ActiveTextAreaControl.TextArea.IconBarMargin);
            }
            catch (Exception ex)
            {
                MessageService.ShowError(ex);
            }
        }

        void UpdateClassMemberBookmarks(ParseInformation parseInfo)
        {
            ICSharpCode.TextEditor.Document.BookmarkManager bm = textEditorControl.Document.BookmarkManager;
            bm.RemoveMarks(new Predicate<Bookmark>(IsClassMemberBookmark));
            if (parseInfo == null) return;
            Debug.Assert(textEditorControl.Document.TotalNumberOfLines >= 1);
            if (textEditorControl.Document.TotalNumberOfLines < 1)
                return;

            foreach (IClass c in parseInfo.MostRecentCompilationUnit.Classes)
                AddClassMemberBookmarks(bm, c);
        }

        void AddClassMemberBookmarks(ICSharpCode.TextEditor.Document.BookmarkManager bm, IClass c)
        {
            if (c.IsSynthetic) return;

            if (!c.Region.IsEmpty)
                bm.AddMark(new ICSharpCode.SharpDevelop.Bookmarks.ClassBookmark(textEditorControl.Document, c));

            foreach (IClass innerClass in c.InnerClasses)
                AddClassMemberBookmarks(bm, innerClass);

            foreach (IMethod m in c.Methods)
            {
                if (m.Region.IsEmpty || m.IsSynthetic) continue;
                bm.AddMark(new ICSharpCode.SharpDevelop.Bookmarks.MethodBookmark(textEditorControl.Document, m));
            }

            foreach (IProperty m in c.Properties)
            {
                if (m.Region.IsEmpty || m.IsSynthetic) continue;
                bm.AddMark(new ICSharpCode.SharpDevelop.Bookmarks.PropertyBookmark(textEditorControl.Document, m));
            }

            foreach (IField f in c.Fields)
            {
                if (f.Region.IsEmpty || f.IsSynthetic) continue;
                bm.AddMark(new ICSharpCode.SharpDevelop.Bookmarks.FieldBookmark(textEditorControl.Document, f));
            }

            foreach (IEvent e in c.Events)
            {
                if (e.Region.IsEmpty || e.IsSynthetic) continue;
                bm.AddMark(new ICSharpCode.SharpDevelop.Bookmarks.EventBookmark(textEditorControl.Document, e));
            }
        }

        bool IsClassMemberBookmark(Bookmark b)
        {
            return b is ICSharpCode.SharpDevelop.Bookmarks.ClassMemberBookmark || b is ICSharpCode.SharpDevelop.Bookmarks.ClassBookmark;
        }

        #region ICSharpCode.SharpDevelop.Gui.IClipboardHandler interface implementation
        public bool EnableCut
        {
            get { return !this.IsDisposed && textEditorControl.ActiveTextAreaControl.TextArea.ClipboardHandler.EnableCut; }
        }

        public bool EnableCopy
        {
            get { return !this.IsDisposed && textEditorControl.ActiveTextAreaControl.TextArea.ClipboardHandler.EnableCopy; }
        }

        public bool EnablePaste
        {
            get { return !this.IsDisposed && textEditorControl.ActiveTextAreaControl.TextArea.ClipboardHandler.EnablePaste; }
        }

        public bool EnableDelete
        {
            get { return !this.IsDisposed && textEditorControl.ActiveTextAreaControl.TextArea.ClipboardHandler.EnableDelete; }
        }

        public bool EnableSelectAll
        {
            get { return !this.IsDisposed && textEditorControl.ActiveTextAreaControl.TextArea.ClipboardHandler.EnableSelectAll; }
        }
        #endregion

        public override string ToString()
        {
            return "[" + GetType().Name + " " + this.PrimaryFileName + "]";
        }

        public IDocument GetDocumentForFile(OpenedFile file)
        {
            throw new NotImplementedException();
        }

        IWorkbenchWindow workbenchWindow;

        public Control Control
        {
            get { return this; }
        }

        IWorkbenchWindow IViewContent.WorkbenchWindow
        {
            get { return workbenchWindow; }
            set
            {
                if (workbenchWindow != value)
                {
                    workbenchWindow = value;
                    OnWorkbenchWindowChanged();
                }
            }
        }

        public IWorkbenchWindow WorkbenchWindow
        {
            get { return workbenchWindow; }
        }

        string tabPageText = "TabPageText";

        public event EventHandler TabPageTextChanged;

        public string TabPageText
        {
            get { return tabPageText; }
            set
            {
                if (tabPageText != value)
                {
                    tabPageText = value;

                    if (TabPageTextChanged != null)
                        TabPageTextChanged(this, EventArgs.Empty);
                }
            }
        }

        #region Secondary view content support
        public sealed class SecondaryViewContentCollection : ICollection<IViewContent>
        {
            readonly EditorTabItem parent;
            readonly List<IViewContent> list = new List<IViewContent>();

            public SecondaryViewContentCollection(EditorTabItem parent)
            {
                this.parent = parent;
            }

            public int Count
            {
                get { return list.Count; }
            }

            public bool IsReadOnly
            {
                get { return false; }
            }

            public void Add(IViewContent item)
            {
                if (item == null)
                    throw new ArgumentNullException("item");

                if (item.WorkbenchWindow != null && item.WorkbenchWindow != parent.WorkbenchWindow)
                    throw new ArgumentException("The view content already is displayed in another workbench window.");

                list.Add(item);
                if (parent.workbenchWindow != null)
                    parent.workbenchWindow.ViewContents.Add(item);
            }

            public void Clear()
            {
                if (parent.workbenchWindow != null)
                {
                    foreach (IViewContent vc in list)
                        parent.workbenchWindow.ViewContents.Remove(vc);
                }

                list.Clear();
            }

            public bool Contains(IViewContent item)
            {
                return list.Contains(item);
            }

            public void CopyTo(IViewContent[] array, int arrayIndex)
            {
                list.CopyTo(array, arrayIndex);
            }

            public bool Remove(IViewContent item)
            {
                if (list.Remove(item))
                {
                    if (parent.workbenchWindow != null)
                        parent.workbenchWindow.ViewContents.Remove(item);
                    return true;
                }

                return false;
            }

            public IEnumerator<IViewContent> GetEnumerator()
            {
                return list.GetEnumerator();
            }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return list.GetEnumerator();
            }
        }

        protected SecondaryViewContentCollection secondaryViewContentCollection;

        /// <summary>
        /// Gets the collection that stores the secondary view contents.
        /// </summary>
        public virtual ICollection<IViewContent> SecondaryViewContents
        {
            get
            {
                return secondaryViewContentCollection;
            }
        }

        /// <summary>
        /// Gets switching without a Save/Load cycle for <paramref name="file"/> is supported
        /// when switching from this view content to <paramref name="newView"/>.
        /// </summary>
        public virtual bool SupportsSwitchFromThisWithoutSaveLoad(OpenedFile file, IViewContent newView)
        {
            return newView == this;
        }

        /// <summary>
        /// Gets switching without a Save/Load cycle for <paramref name="file"/> is supported
        /// when switching from <paramref name="oldView"/> to this view content.
        /// </summary>
        public virtual bool SupportsSwitchToThisWithoutSaveLoad(OpenedFile file, IViewContent oldView)
        {
            return oldView == this;
        }

        /// <summary>
        /// Executes an action before switching from this view content to the new view content.
        /// </summary>
        public virtual void SwitchFromThisWithoutSaveLoad(OpenedFile file, IViewContent newView) { }

        /// <summary>
        /// Executes an action before switching from the old view content to this view content.
        /// </summary>
        public virtual void SwitchToThisWithoutSaveLoad(OpenedFile file, IViewContent oldView) { }
        #endregion

        #region Files
        protected FilesCollection files;
        ReadOnlyCollection<OpenedFile> filesReadonly;

        protected void InitFiles()
        {
            files = new FilesCollection(this);
            filesReadonly = new ReadOnlyCollection<OpenedFile>(files);
        }

        protected Collection<OpenedFile> Files
        {
            get { return files; }
        }

        IList<OpenedFile> IViewContent.Files
        {
            get { return filesReadonly; }
        }

        /// <summary>
        /// Gets the primary file being edited. Might return null if no file is edited.
        /// </summary>
        public virtual OpenedFile PrimaryFile
        {
            get
            {
                if (files.Count != 0)
                    return files[0];

                return null;
            }
        }

        /// <summary>
        /// Gets the name of the primary file being edited. Might return null if no file is edited.
        /// </summary>
        public virtual string PrimaryFileName
        {
            get
            {
                OpenedFile file = PrimaryFile;
                if (file != null)
                    return file.FileName;

                return null;
            }
        }

        protected bool AutomaticallyRegisterViewOnFiles = true;

        void RegisterFileEventHandlers(OpenedFile newItem)
        {
            newItem.FileNameChanged += OnFileNameChanged;
            newItem.IsDirtyChanged += OnIsDirtyChanged;
            if (AutomaticallyRegisterViewOnFiles)
                newItem.RegisterView(this);

            OnIsDirtyChanged(null, EventArgs.Empty); // re-evaluate this.IsDirty after changing the file collection
        }

        void UnregisterFileEventHandlers(OpenedFile oldItem)
        {
            oldItem.FileNameChanged -= OnFileNameChanged;
            oldItem.IsDirtyChanged -= OnIsDirtyChanged;
            if (AutomaticallyRegisterViewOnFiles)
                oldItem.UnregisterView(this);

            OnIsDirtyChanged(null, EventArgs.Empty); // re-evaluate this.IsDirty after changing the file collection
        }

        void OnFileNameChanged(object sender, EventArgs e)
        {
            OnFileNameChanged((OpenedFile)sender);
            if (titleName == null && files.Count > 0 && sender == files[0])
                OnTitleNameChanged(EventArgs.Empty);
        }

        public sealed class FilesCollection : Collection<OpenedFile>
        {
            EditorTabItem parent;
            public FilesCollection(EditorTabItem parent)
            {
                this.parent = parent;
            }

            protected override void InsertItem(int index, OpenedFile item)
            {
                base.InsertItem(index, item);
                parent.RegisterFileEventHandlers(item);
            }

            protected override void SetItem(int index, OpenedFile item)
            {
                parent.UnregisterFileEventHandlers(this[index]);
                base.SetItem(index, item);
                parent.RegisterFileEventHandlers(item);
            }

            protected override void RemoveItem(int index)
            {
                parent.UnregisterFileEventHandlers(this[index]);
                base.RemoveItem(index);
            }

            protected override void ClearItems()
            {
                foreach (OpenedFile item in this)
                {
                    parent.UnregisterFileEventHandlers(item);
                }
                base.ClearItems();
            }
        }
        #endregion

        #region TitleName
        public event EventHandler TitleNameChanged;

        void OnTitleNameChanged(EventArgs e)
        {
            if (TitleNameChanged != null)
                TitleNameChanged(this, e);
        }

        string titleName;
        string IViewContent.TitleName
        {
            get
            {
                if (titleName != null)
                    return titleName;

                if (files.Count > 0)
                    return Path.GetFileName(files[0].FileName);

                return "[Default Title]";
            }
        }

        public string TitleName
        {
            get { return titleName; }
            protected set
            {
                if (titleName != value)
                {
                    titleName = value;
                    OnTitleNameChanged(EventArgs.Empty);
                }
            }
        }
        #endregion

        #region IsDirty
        bool IsDirtyInternal
        {
            get
            {
                foreach (OpenedFile file in this.Files)
                {
                    if (file.IsDirty)
                        return true;
                }

                return false;
            }
        }

        bool isDirty;
        public virtual bool IsDirty
        {
            get { return this.TabText.EndsWith("*"); }
        }

        void OnIsDirtyChanged(object sender, EventArgs e)
        {
            bool newIsDirty = IsDirtyInternal;
            if (newIsDirty != isDirty)
            {
                isDirty = newIsDirty;
                RaiseIsDirtyChanged();
            }
        }

        /// <summary>
        /// Raise the IsDirtyChanged event. Call this method only if you have overridden the IsDirty property
        /// to implement your own handling of IsDirty.
        /// </summary>
        protected void RaiseIsDirtyChanged()
        {
            if (IsDirtyChanged != null)
                IsDirtyChanged(this, EventArgs.Empty);
        }

        public event EventHandler IsDirtyChanged;
        #endregion

        #region IsActiveViewContent
        EventHandler isActiveViewContentChanged;
        bool registeredOnViewContentChange;
        bool wasActiveViewContent;

        /// <summary>
        /// Gets if this view content is the active view content.
        /// </summary>
        public bool IsActiveViewContent
        {
            get
            {
                if (Common.Ultility.Ultility.IsUnixOS)
                    return FormMain.CurrentActiveTab == this;

                return WorkbenchSingleton.ActiveControl == this;
            }
        }

        /// <summary>
        /// Is raised when the value of the IsActiveViewContent property changes.
        /// </summary>
        protected event EventHandler IsActiveViewContentChanged
        {
            add
            {
                if (!registeredOnViewContentChange)
                {
                    // register WorkbenchSingleton.Workbench.ActiveViewContentChanged only on demand
                    wasActiveViewContent = IsActiveViewContent;
                    WorkbenchSingleton.Workbench.ActiveViewContentChanged += OnActiveViewContentChanged;
                    registeredOnViewContentChange = true;
                }

                isActiveViewContentChanged += value;
            }

            remove { isActiveViewContentChanged -= value; }
        }

        void UnregisterOnActiveViewContentChanged()
        {
            if (registeredOnViewContentChange)
            {
                WorkbenchSingleton.Workbench.ActiveViewContentChanged -= OnActiveViewContentChanged;
                registeredOnViewContentChange = false;
            }
        }

        void OnActiveViewContentChanged(object sender, EventArgs e)
        {
            bool isActiveViewContent = IsActiveViewContent;
            if (isActiveViewContent != wasActiveViewContent)
            {
                wasActiveViewContent = isActiveViewContent;
                if (isActiveViewContentChanged != null)
                    isActiveViewContentChanged(this, e);
            }
        }
        #endregion
        #endregion

        public TextEditorControl TextEditorControl
        {
            get { return this.textEditorControl; }
        }
        #endregion

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EditorTabItem));
            this.EditorContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.mnuGoToDeclarition = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuFindUsage = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuRename = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuOpenImportFile = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuUndo = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuRedo = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuCut = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuCopy = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuPaste = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuSelectAll = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuComment = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuUnComment = new System.Windows.Forms.ToolStripMenuItem();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.EditorContextMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // EditorContextMenuStrip
            // 
            this.EditorContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuGoToDeclarition,
            this.mnuFindUsage,
            this.mnuRename,
            this.mnuOpenImportFile,
            this.toolStripSeparator2,
            this.mnuUndo,
            this.mnuRedo,
            this.toolStripSeparator1,
            this.mnuCut,
            this.mnuCopy,
            this.mnuPaste,
            this.toolStripSeparator3,
            this.mnuSelectAll,
            this.toolStripSeparator4,
            this.mnuComment,
            this.mnuUnComment});
            this.EditorContextMenuStrip.Name = "EditorContextMenuStrip";
            resources.ApplyResources(this.EditorContextMenuStrip, "EditorContextMenuStrip");
            this.EditorContextMenuStrip.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip1_Opening);
            this.EditorContextMenuStrip.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.contextMenuStrip1_ItemClicked);
            // 
            // mnuGoToDeclarition
            // 
            this.mnuGoToDeclarition.Name = "mnuGoToDeclarition";
            resources.ApplyResources(this.mnuGoToDeclarition, "mnuGoToDeclarition");
            this.mnuGoToDeclarition.Click += new System.EventHandler(this.mnuGoToDeclarition_Click);
            // 
            // mnuFindUsage
            // 
            this.mnuFindUsage.Name = "mnuFindUsage";
            resources.ApplyResources(this.mnuFindUsage, "mnuFindUsage");
            this.mnuFindUsage.Click += new System.EventHandler(this.mnuFindUsage_Click);
            // 
            // mnuRename
            // 
            this.mnuRename.Name = "mnuRename";
            resources.ApplyResources(this.mnuRename, "mnuRename");
            this.mnuRename.Click += new System.EventHandler(this.mnuRename_Click);
            // 
            // mnuOpenImportFile
            // 
            this.mnuOpenImportFile.Image = global::PAT.GUI.Properties.Resources.open_spec;
            this.mnuOpenImportFile.Name = "mnuOpenImportFile";
            resources.ApplyResources(this.mnuOpenImportFile, "mnuOpenImportFile");
            this.mnuOpenImportFile.Click += new System.EventHandler(this.mnuOpenImportFile_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            resources.ApplyResources(this.toolStripSeparator2, "toolStripSeparator2");
            // 
            // mnuUndo
            // 
            this.mnuUndo.Image = global::PAT.GUI.Properties.Resources.undo;
            this.mnuUndo.Name = "mnuUndo";
            resources.ApplyResources(this.mnuUndo, "mnuUndo");
            // 
            // mnuRedo
            // 
            this.mnuRedo.Image = global::PAT.GUI.Properties.Resources.redo;
            this.mnuRedo.Name = "mnuRedo";
            resources.ApplyResources(this.mnuRedo, "mnuRedo");
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            resources.ApplyResources(this.toolStripSeparator1, "toolStripSeparator1");
            // 
            // mnuCut
            // 
            this.mnuCut.Image = global::PAT.GUI.Properties.Resources.cut;
            this.mnuCut.Name = "mnuCut";
            resources.ApplyResources(this.mnuCut, "mnuCut");
            // 
            // mnuCopy
            // 
            this.mnuCopy.Image = global::PAT.GUI.Properties.Resources.copy;
            this.mnuCopy.Name = "mnuCopy";
            resources.ApplyResources(this.mnuCopy, "mnuCopy");
            // 
            // mnuPaste
            // 
            this.mnuPaste.Image = global::PAT.GUI.Properties.Resources.paste;
            this.mnuPaste.Name = "mnuPaste";
            resources.ApplyResources(this.mnuPaste, "mnuPaste");
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            resources.ApplyResources(this.toolStripSeparator3, "toolStripSeparator3");
            // 
            // mnuSelectAll
            // 
            this.mnuSelectAll.Image = global::PAT.GUI.Properties.Resources.selection;
            this.mnuSelectAll.Name = "mnuSelectAll";
            resources.ApplyResources(this.mnuSelectAll, "mnuSelectAll");
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            resources.ApplyResources(this.toolStripSeparator4, "toolStripSeparator4");
            // 
            // mnuComment
            // 
            this.mnuComment.Image = global::PAT.GUI.Properties.Resources.comment;
            this.mnuComment.Name = "mnuComment";
            resources.ApplyResources(this.mnuComment, "mnuComment");
            this.mnuComment.Click += new System.EventHandler(this.mnuComment_Click);
            // 
            // mnuUnComment
            // 
            this.mnuUnComment.Image = global::PAT.GUI.Properties.Resources.uncomment;
            this.mnuUnComment.Name = "mnuUnComment";
            resources.ApplyResources(this.mnuUnComment, "mnuUnComment");
            this.mnuUnComment.Click += new System.EventHandler(this.mnuUnComment_Click);
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "process");
            this.imageList1.Images.SetKeyName(1, "variable");
            this.imageList1.Images.SetKeyName(2, "channel");
            this.imageList1.Images.SetKeyName(3, "declare");
            this.imageList1.Images.SetKeyName(4, "keyword");
            this.imageList1.Images.SetKeyName(5, "");
            this.imageList1.Images.SetKeyName(6, "Icons.16x16.Literal.png");
            this.imageList1.Images.SetKeyName(7, "variable");
            this.imageList1.Images.SetKeyName(8, "");
            this.imageList1.Images.SetKeyName(9, "");
            this.imageList1.Images.SetKeyName(10, "keyword");
            this.imageList1.Images.SetKeyName(11, "channel");
            this.imageList1.Images.SetKeyName(12, "define");
            // 
            // EditorTabItem
            // 
            resources.ApplyResources(this, "$this");
            this.Name = "EditorTabItem";
            this.EditorContextMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        public void Comment()
        {
            try
            {
                CommentRegion CommentRegion = new CommentRegion();
                CommentRegion.Run();
            }
            catch (Exception ex)
            {
                Ultility.LogException(ex, null);
            }
        }

        public void UnComment()
        {
            try
            {
                CommentRegion CommentRegion = new CommentRegion();
                CommentRegion.Run();
            }
            catch (Exception ex)
            {
                Ultility.LogException(ex, null);
            }
        }

        public void HideGoToDeclarition()
        {
            mnuGoToDeclarition.Visible = false;
            mnuFindUsage.Visible = false;
            mnuRename.Visible = false;
            mnuRename.ShortcutKeys = Keys.None;
            toolStripSeparator2.Visible = false;
        }

        private void mnuGoToDeclarition_Click(object sender, EventArgs e)
        {
            try
            {
                SelectionManager selectionManager = textEditorControl.ActiveTextAreaControl.TextArea.SelectionManager;
                if (!string.IsNullOrEmpty(selectionManager.SelectedText))
                {
                    if (Common.Ultility.Ultility.ModuleDictionary.ContainsKey(ModuleName))
                    {
                        ModuleFacadeBase module = Common.Ultility.Ultility.ModuleDictionary[ModuleName];
                        SpecificationBase spec = module.ParseSpecification(this.Text, "", FileName);

                        if (spec != null)
                        {
                            ParsingException exp = spec.GoToDeclarition(selectionManager.SelectedText);
                            GoToDeclarition(exp);
                        }
                    }
                }
            }
            catch (Exception) { }
        }

        private void mnuOpenImportFile_Click(object sender, EventArgs e)
        {
            try
            {
                SelectionManager selectionManager = textEditorControl.ActiveTextAreaControl.TextArea.SelectionManager;
                if (!string.IsNullOrEmpty(selectionManager.SelectedText))
                {
                    string file = selectionManager.SelectedText;
                    if (!File.Exists(file))
                    {
                        if (File.Exists(FileName))
                        {
                            file = Path.Combine(Path.GetDirectoryName(FileName), selectionManager.SelectedText);
                            file = ProcessFileType(file);
                        }

                        if (!File.Exists(file))
                        {
                            file = Path.Combine(Ultility.LibFolderPath, selectionManager.SelectedText);
                            file = ProcessFileType(file);
                        }
                    }

                    if (File.Exists(file))
                    {
                        if (file.EndsWith(".cs", StringComparison.CurrentCultureIgnoreCase))
                        {
                            CSharpLibraryForm oForm = new CSharpLibraryForm(file);
                            oForm.Show();
                        }
                        else
                        {
                            Process.Start(file);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Cannot open the file!", Common.Ultility.Ultility.APPLICATION_NAME, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Cannot open the file!", Common.Ultility.Ultility.APPLICATION_NAME, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string ProcessFileType(string file)
        {
            if (file.EndsWith(".dll", StringComparison.CurrentCultureIgnoreCase))
            {
                string filecs = file.Replace(".dll", ".cs", StringComparison.CurrentCultureIgnoreCase);
                if (File.Exists(filecs))
                    return filecs;
            }

            if (File.Exists(file + ".cs"))
                return file + ".cs";

            return file;
        }

        private void mnuFindUsage_Click(object sender, EventArgs e)
        {
            try
            {
                SelectionManager selectionManager = textEditorControl.ActiveTextAreaControl.TextArea.SelectionManager;
                if (!string.IsNullOrEmpty(selectionManager.SelectedText))
                {
                    if (Common.Ultility.Ultility.ModuleDictionary.ContainsKey(ModuleName))
                    {
                        ModuleFacadeBase module = Common.Ultility.Ultility.ModuleDictionary[ModuleName];
                        SpecificationBase spec = module.ParseSpecification(this.Text, "", FileName);

                        if (spec != null)
                        {
                            List<ParsingException> exp = spec.FindUsages(selectionManager.SelectedText);
                            FindUsages(selectionManager.SelectedText, exp, spec);
                        }
                    }
                }
            }
            catch (Exception) { }
        }

        private void mnuRename_Click(object sender, EventArgs e)
        {
            try
            {
                SelectionManager selectionManager = textEditorControl.ActiveTextAreaControl.TextArea.SelectionManager;
                string select = selectionManager.SelectedText;
                if (!string.IsNullOrEmpty(select))
                {
                    if (Common.Ultility.Ultility.ModuleDictionary.ContainsKey(ModuleName))
                    {
                        RenameForm renameForm = new RenameForm(select);
                        if (renameForm.ShowDialog() == DialogResult.OK)
                        {
                            string newName = renameForm.TextBox_NewName.Text;
                            int difference = newName.Length - select.Length;
                            ModuleFacadeBase module = Common.Ultility.Ultility.ModuleDictionary[ModuleName];
                            SpecificationBase spec = module.ParseSpecification(this.Text, "", FileName);

                            if (spec != null)
                            {
                                selectionManager.ClearSelection();

                                Dictionary<int, int> sameLineCounter = new Dictionary<int, int>();
                                List<ParsingException> exp = spec.RenameFindUsages(select);
                                for (int i = 0; i < exp.Count; i++)
                                {
                                    if (exp[i].Line > 0)
                                    {
                                        try
                                        {
                                            int sameLineNumber = 0;
                                            if (sameLineCounter.ContainsKey(exp[i].Line - 1))
                                            {
                                                sameLineNumber = sameLineCounter[exp[i].Line - 1] + 1;
                                                sameLineCounter[exp[i].Line - 1] = sameLineNumber;
                                            }
                                            else
                                            {
                                                sameLineCounter.Add(exp[i].Line - 1, sameLineNumber);
                                            }

                                            int lineOff = textEditorControl.ActiveTextAreaControl.TextArea.Document.GetLineSegment(exp[i].Line - 1).Offset;

                                            textEditorControl.ActiveTextAreaControl.TextArea.Document.Replace(lineOff + exp[i].CharPositionInLine + (difference * sameLineNumber), select.Length, newName);
                                        }
                                        catch (Exception) { }
                                    }
                                }

                                textEditorControl.ActiveTextAreaControl.TextArea.Refresh();
                            }
                        }
                    }
                }
            }
            catch (Exception) { }
        }

        private void mnuComment_Click(object sender, EventArgs e)
        {
            Comment();
        }

        private void mnuUnComment_Click(object sender, EventArgs e)
        {
            UnComment();
        }

        public virtual void HandleParsingException(ParsingException ex)
        {
            try
            {
                if (string.IsNullOrEmpty(ex.FileName) || ex.FileName == this.FileName)
                {
                    if (ex.Line >= 1 && ex.CharPositionInLine >= 0 && ex.Text != null)
                    {
                        this.textEditorControl.ActiveTextAreaControl.JumpTo(ex.Line - 1);
                        SelectionManager selectionManager =
                            textEditorControl.ActiveTextAreaControl.TextArea.SelectionManager;
                        selectionManager.ClearSelection();
                        selectionManager.SetSelection(new TextLocation(ex.CharPositionInLine, ex.Line - 1),
                                                      new TextLocation(ex.CharPositionInLine + ex.Text.Length,
                                                                       ex.Line - 1));
                        textEditorControl.Refresh();
                    }
                }
            }
            catch { }
        }

        public string GetLineText(int linenumber)
        {
            try
            {
                LineSegment line = textEditorControl.ActiveTextAreaControl.TextArea.Document.GetLineSegment(linenumber);
                return textEditorControl.ActiveTextAreaControl.TextArea.Document.GetText(line).Trim();
            }
            catch (Exception) { }

            return "";
        }
    }
}