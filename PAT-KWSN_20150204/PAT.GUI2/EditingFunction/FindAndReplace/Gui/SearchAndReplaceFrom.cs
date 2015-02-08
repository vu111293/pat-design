using System;
using System.Windows.Forms;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Document;


namespace SearchAndReplace
{
    public enum SearchAndReplaceMode
    {
        Search,
        Replace
    }

    public partial class SearchAndReplaceFrom : Form
    {
        static SearchAndReplaceFrom Instance;
        Keys searchKeyboardShortcut = Keys.None;
        Keys replaceKeyboardShortcut = Keys.None;

        public static void ShowSingleInstance(SearchAndReplaceMode searchAndReplaceMode)
        {
            if (Instance == null)
            {
                Instance = new SearchAndReplaceFrom(searchAndReplaceMode);
                Instance.Show(WorkbenchSingleton.MainForm); //
            }
            else
            {
                if (searchAndReplaceMode == SearchAndReplaceMode.Search)
                {
                    Instance.ToolStripButton_Find.PerformClick();
                }
                else
                {
                    Instance.ToolStripButton_Replace.PerformClick();
                }
                Instance.Focus();
            }
        }
        public SearchAndReplaceFrom(SearchAndReplaceMode searchAndReplaceMode)
        {
            InitializeComponent();

            this.Owner = WorkbenchSingleton.MainForm;
            this.FormBorderStyle = FormBorderStyle.FixedToolWindow;
            this.ShowInTaskbar = false;
            this.TopMost = false;
            this.KeyPreview = true;

            this.ToolStripButton_Find.Checked = searchAndReplaceMode == SearchAndReplaceMode.Search;
            this.ToolStripButton_Replace.Checked = searchAndReplaceMode == SearchAndReplaceMode.Replace;

            SetSearchAndReplaceMode();

            FormLocationHelper.Apply(this, "ICSharpCode.SharpDevelop.Gui.SearchAndReplaceDialog.Location", false);
        }

        private void Button_FindNext_Click(object sender, EventArgs e)
        {
            WritebackOptions();
            if (IsSelectionSearch)
            {
                if (IsTextSelected(selection))
                {
                    FindNextInSelection();
                }
            }
            else
            {
                using (AsynchronousWaitDialog monitor = AsynchronousWaitDialog.ShowWaitDialog("Search", true))
                {
                    SearchReplaceManager.FindNext(monitor);
                }
            }
            Focus();
        }

        private void Button_Find_Click(object sender, EventArgs e)
        {
            Button_FindNext_Click(sender, e);
            //WritebackOptions();
            //if (IsSelectionSearch)
            //{
            //    if (IsTextSelected(selection))
            //    {
            //        RunAllInSelection(0);
            //    }
            //}
            //else
            //{
            //    using (AsynchronousWaitDialog monitor = AsynchronousWaitDialog.ShowWaitDialog("Search", true))
            //    {
            //        SearchInFilesManager.FindAll(monitor);
            //    }
            //}
        }

        private void Button_BookmarkAll_Click(object sender, EventArgs e)
        {
            WritebackOptions();
            if (IsSelectionSearch)
            {
                if (IsTextSelected(selection))
                {
                    RunAllInSelection(1);
                }
            }
            else
            {
                using (AsynchronousWaitDialog monitor = AsynchronousWaitDialog.ShowWaitDialog("Search", true))
                {
                    SearchReplaceManager.MarkAll(monitor);
                }
            }
        }

        private void Button_Replace_Click(object sender, EventArgs e)
        {
            WritebackOptions();
            if (IsSelectionSearch)
            {
                if (IsTextSelected(selection))
                {
                    ReplaceInSelection();
                }
            }
            else
            {
                using (AsynchronousWaitDialog monitor = AsynchronousWaitDialog.ShowWaitDialog("Search", true))
                {
                    SearchReplaceManager.Replace(monitor);
                }
            }
            Focus();
        }

        private void Button_ReplaceAll_Click(object sender, EventArgs e)
        {
            WritebackOptions();
            if (IsSelectionSearch)
            {
                if (IsTextSelected(selection))
                {
                    RunAllInSelection(2);
                }
            }
            else
            {
                using (AsynchronousWaitDialog monitor = AsynchronousWaitDialog.ShowWaitDialog("Search", true))
                {
                    SearchReplaceManager.ReplaceAll(monitor);
                }
            }
        }

        void SearchButtonClick(object sender, EventArgs e)
        {
            if (!this.ToolStripButton_Find.Checked)
            {
                EnableSearchMode(true);
            }
        }

        void ReplaceButtonClick(object sender, EventArgs e)
        {
            if (!this.ToolStripButton_Replace.Checked)
            {
                EnableSearchMode(false);
            }
        }
		

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            base.OnClosing(e);
            Instance = null;
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.KeyData == Keys.Escape)
            {
                Close();
            }
            else if (searchKeyboardShortcut == e.KeyData && ! this.ToolStripButton_Find.Checked)
            {
                EnableSearchMode(true);
            }
            else if (replaceKeyboardShortcut == e.KeyData && !this.ToolStripButton_Replace.Checked)
            {
                EnableSearchMode(false);
            }
        }

        void EnableSearchMode(bool enable)
        {
            ToolStripButton_Find.Checked = enable;
            ToolStripButton_Replace.Checked = !enable;
            SetSearchAndReplaceMode();
            Focus();
        }

        void SetSearchAndReplaceMode()
        {
            SearchAndReplaceMode = this.ToolStripButton_Find.Checked ? SearchAndReplaceMode.Search : SearchAndReplaceMode.Replace;
            //searchAndReplacePanel.SearchAndReplaceMode = 
            //if (searchButton.Checked)
            //{
            //    this.ClientSize = new Size(430, 335);
            //}
            //else
            //{
            //    this.ClientSize = new Size(430, 385);
            //}
        }

        SearchAndReplaceMode  searchAndReplaceMode;
		ISelection selection;
		TextEditorControl textEditor;
		bool ignoreSelectionChanges;
		bool findFirst;
		
		public SearchAndReplaceMode SearchAndReplaceMode {
			get {
				return searchAndReplaceMode;
			}
			set {
				searchAndReplaceMode = value;
				SuspendLayout();
				//Controls.Clear();
				switch (searchAndReplaceMode) {
					case SearchAndReplaceMode.Search:
                        //SetupFromXmlStream(this.GetType().Assembly.GetManifestResourceStream("PAT.GUI.EditingFunction.FindAndReplace.Resources.FindPanel.xfrm"));
                        //Get<Button>("bookmarkAll").Click += BookmarkAllButtonClicked;
                        //Get<Button>("findAll").Click += FindAllButtonClicked;
                        this.ComboBox_ReplaceWith.Visible = false;
                        this.Label_ReplaceWith.Visible = false;

                        this.Button_FindNext.Visible = false;
				        this.Button_Find.Visible = true;
                        this.Button_BookmarkAll.Visible = true;
                        this.Button_Replace.Visible = false;
                        this.Button_ReplaceAll.Visible = false;

                        this.AcceptButton = Button_Find;
						break;
					case SearchAndReplaceMode.Replace:
                        //SetupFromXmlStream(this.GetType().Assembly.GetManifestResourceStream("PAT.GUI.EditingFunction.FindAndReplace.Resources.ReplacePanel.xfrm"));
                        //Get<Button>("replace").Click += ReplaceButtonClicked;
                        //Get<Button>("replaceAll").Click += ReplaceAllButtonClicked;
                        this.ComboBox_ReplaceWith.Visible = true;
                        this.Label_ReplaceWith.Visible = true;

                        this.Button_FindNext.Visible = true;
				        this.Button_Find.Visible = false;
                        this.Button_BookmarkAll.Visible = false;
                        this.Button_Replace.Visible = true;
                        this.Button_ReplaceAll.Visible = true;
                        this.AcceptButton = Button_FindNext;

						break;
				}
				
				this.ComboBox_FindWhat.TextChanged += FindPatternChanged;
				//ControlDictionary["findNextButton"].Click     += FindNextButtonClicked;
				//ControlDictionary["lookInBrowseButton"].Click += LookInBrowseButtonClicked;
				//((Form)Parent).AcceptButton = (Button)ControlDictionary["findNextButton"];
				SetOptions();
				EnableButtons(HasFindPattern);
				//RightToLeftConverter.ReConvertRecursive(this);
				ResumeLayout(false);
			}
		}
		

		
        //protected override void Dispose(bool disposing)
        //{
        //    RemoveSelectionChangedHandler();
        //    RemoveActiveWindowChangedHandler();
        //    base.Dispose(disposing);
        //}
		
		public DocumentIteratorType DocumentIteratorType {
			get {
				return (DocumentIteratorType)(this.ComboBox_LookIn.SelectedIndex);
			}
			set {
                ComboBox_LookIn.SelectedIndex = (int)value;
			}
		}
		
		void LookInBrowseButtonClicked(object sender, EventArgs e)
		{
			ComboBox lookinComboBox = ComboBox_LookIn;
			using (FolderBrowserDialog dlg = FileService.CreateFolderBrowserDialog("${res:Dialog.NewProject.SearchReplace.LookIn.SelectDirectory}", lookinComboBox.Text)) {
				if (dlg.ShowDialog() == DialogResult.OK) {
					lookinComboBox.SelectedIndex = customDirectoryIndex;
					lookinComboBox.Text = dlg.SelectedPath;
				}
			}
		}
		
		void WritebackOptions()
		{
			SearchOptions.FindPattern = this.ComboBox_FindWhat.Text;
			
			if (searchAndReplaceMode == SearchAndReplaceMode.Replace) {
				SearchOptions.ReplacePattern = this.ComboBox_ReplaceWith.Text;
			}
			
			if (this.ComboBox_LookIn.DropDownStyle == ComboBoxStyle.DropDown) {
				SearchOptions.LookIn = ComboBox_LookIn.Text;
			}
			//SearchOptions.LookInFiletypes = Get<ComboBox>("fileTypes").Text;
			SearchOptions.MatchCase = this.CheckBox_MatchCase.Checked;
            SearchOptions.MatchWholeWord = this.CheckBox_MatchWholeWord.Checked;
			//SearchOptions.IncludeSubdirectories = Get<CheckBox>("includeSubFolder").Checked;
			
			SearchOptions.SearchStrategyType = (SearchStrategyType)ComboBox_Use.SelectedIndex;
			if (ComboBox_LookIn.DropDownStyle == ComboBoxStyle.DropDown) {
				SearchOptions.DocumentIteratorType = DocumentIteratorType.Directory;
			} else {
				SearchOptions.DocumentIteratorType = (DocumentIteratorType)ComboBox_LookIn.SelectedIndex;
			}
		}
		
		const int customDirectoryIndex = 3;
		
		void SetOptions()
		{
			ComboBox_FindWhat.Text = SearchOptions.FindPattern;
			ComboBox_FindWhat.Items.Clear();
			
			ComboBox_FindWhat.Text = SearchOptions.FindPattern;
			ComboBox_FindWhat.Items.Clear();
			foreach (string findPattern in SearchOptions.FindPatterns) {
				ComboBox_FindWhat.Items.Add(findPattern);
			}
			
			if (searchAndReplaceMode == SearchAndReplaceMode.Replace) {
				ComboBox_ReplaceWith.Text = SearchOptions.ReplacePattern;
				ComboBox_ReplaceWith.Items.Clear();
				foreach (string replacePattern in SearchOptions.ReplacePatterns) {
					ComboBox_ReplaceWith.Items.Add(replacePattern);
				}
			}
			
			ComboBox_LookIn.Text = SearchOptions.LookIn;
            //string[] lookInTexts = {
            //    // must be in the same order as the DocumentIteratorType enum
            //    "${res:Dialog.NewProject.SearchReplace.LookIn.CurrentDocument}",
            //    "${res:Dialog.NewProject.SearchReplace.LookIn.CurrentSelection}",
            //    //"${res:Dialog.NewProject.SearchReplace.LookIn.AllOpenDocuments}",
            //    //"${res:Dialog.NewProject.SearchReplace.LookIn.WholeProject}",
            //    //"${res:Dialog.NewProject.SearchReplace.LookIn.WholeSolution}"
            //};
            //foreach (string lookInText in lookInTexts) {
            //    ComboBox_LookIn.Items.Add(StringParser.Parse(lookInText));
            //}

			//ComboBox_LookIn.Items.Add(SearchOptions.LookIn);

			//ComboBox_LookIn.SelectedIndexChanged += new EventHandler(LookInSelectedIndexChanged);
			
			if (IsMultipleLineSelection(GetCurrentTextSelection())) {
				DocumentIteratorType = DocumentIteratorType.CurrentSelection;
			} else {
				if (SearchOptions.DocumentIteratorType == DocumentIteratorType.CurrentSelection) {
					SearchOptions.DocumentIteratorType = DocumentIteratorType.CurrentDocument;
				}
				DocumentIteratorType = SearchOptions.DocumentIteratorType;
			}
			
			//Get<ComboBox>("fileTypes").Text         = SearchOptions.LookInFiletypes;
			CheckBox_MatchCase.Checked      = SearchOptions.MatchCase;
			CheckBox_MatchWholeWord.Checked = SearchOptions.MatchWholeWord;
			//Get<CheckBox>("includeSubFolder").Checked = SearchOptions.IncludeSubdirectories;
			
			//this.ComboBox_Use.Items.Clear();
			//ComboBox_Use.Items.Add(StringParser.Parse("${res:Dialog.NewProject.SearchReplace.SearchStrategy.Standard}"));
			//ComboBox_Use.Items.Add(StringParser.Parse("${res:Dialog.NewProject.SearchReplace.SearchStrategy.RegexSearch}"));
			//ComboBox_Use.Items.Add(StringParser.Parse("${res:Dialog.NewProject.SearchReplace.SearchStrategy.WildcardSearch}"));
			switch (SearchOptions.SearchStrategyType) {
				case SearchStrategyType.RegEx:
					ComboBox_Use.SelectedIndex = 1;
					break;
				case SearchStrategyType.Wildcard:
					ComboBox_Use.SelectedIndex = 2;
					break;
				default:
					ComboBox_Use.SelectedIndex = 0;
					break;
			}
		}
		
		void LookInSelectedIndexChanged(object sender, EventArgs e)
		{
			if (ComboBox_LookIn.SelectedIndex == customDirectoryIndex) {
				ComboBox_LookIn.DropDownStyle = ComboBoxStyle.DropDown;
				//Get<CheckBox>("includeSubFolder").Enabled = true;
				//Get<ComboBox>("fileTypes").Enabled = true;
				//Get<Label>("lookAtTypes").Enabled = true;
			} else {
				ComboBox_LookIn.DropDownStyle = ComboBoxStyle.DropDownList;
				//Get<CheckBox>("includeSubFolder").Enabled = false;
				//Get<ComboBox>("fileTypes").Enabled = false;
				//Get<Label>("lookAtTypes").Enabled = false;
			}
			if (IsSelectionSearch) {
				InitSelectionSearch();
			} else {
				RemoveSelectionSearchHandlers();
			}
		}
		
		bool IsSelectionSearch {
			get {
				return DocumentIteratorType == DocumentIteratorType.CurrentSelection;
			}
		}
		
		/// <summary>
		/// Checks whether the selection spans two or more lines.
		/// </summary>
		/// <remarks>Maybe the ISelection interface should have an
		/// IsMultipleLine method?</remarks>
		static bool IsMultipleLineSelection(ISelection selection)
		{
			if (IsTextSelected(selection)) {
				return selection.SelectedText.IndexOf('\n') != -1;
			}
			return false;
		}
		
		static bool IsTextSelected(ISelection selection)
		{
			if (selection != null) {
				return !selection.IsEmpty;
			}
			return false;
		}
		
		void FindNextInSelection()
		{
			int startOffset = Math.Min(selection.Offset, selection.EndOffset);
			int endOffset = Math.Max(selection.Offset, selection.EndOffset);
			
			if (findFirst) {
				SetCaretPosition(textEditor.ActiveTextAreaControl.TextArea, startOffset);
			}
			
			try {
				ignoreSelectionChanges = true;
				if (findFirst) {
					findFirst = false;
					SearchReplaceManager.FindFirstInSelection(startOffset, endOffset - startOffset, null);
				} else {
					findFirst = !SearchReplaceManager.FindNextInSelection(null);
					if (findFirst) {
						SearchReplaceUtilities.SelectText(textEditor, startOffset, endOffset);
					}
				}
			} finally {
				ignoreSelectionChanges = false;
			}
		}
		
		/// <summary>
		/// Returns the first ISelection object from the currently active text editor
		/// </summary>
		static ISelection GetCurrentTextSelection()
		{
			TextEditorControl textArea = SearchReplaceUtilities.GetActiveTextEditor();
			if (textArea != null) {
				SelectionManager selectionManager = textArea.ActiveTextAreaControl.SelectionManager;
				if (selectionManager.HasSomethingSelected) {
					return selectionManager.SelectionCollection[0];
				}
			}
			return null;
		}
		
		void WorkbenchActiveViewContentChanged(object source, EventArgs e)
		{
			TextEditorControl activeTextEditorControl = SearchReplaceUtilities.GetActiveTextEditor();
			if (activeTextEditorControl != this.textEditor) {
				AddSelectionChangedHandler(activeTextEditorControl);
				TextSelectionChanged(source, e);
			}
		}
		
		void AddSelectionChangedHandler(TextEditorControl textEditor)
		{
			RemoveSelectionChangedHandler();
			
			this.textEditor = textEditor;
			if (textEditor != null) {
				this.textEditor.ActiveTextAreaControl.SelectionManager.SelectionChanged += TextSelectionChanged;
			}
		}
		
		void RemoveSelectionChangedHandler()
		{
			if (textEditor != null) {
				textEditor.ActiveTextAreaControl.SelectionManager.SelectionChanged -= TextSelectionChanged;
			}
		}
		
		void RemoveActiveWindowChangedHandler()
		{
			WorkbenchSingleton.DockContainer.ActiveDocumentChanged -= WorkbenchActiveViewContentChanged;
		}
		
		/// <summary>
		/// When the selected text is changed make sure the 'Current Selection'
		/// option is not selected if no text is selected.
		/// </summary>
		/// <remarks>The text selection can change either when the user
		/// selects different text in the editor or the active window is
		/// changed.</remarks>
		void TextSelectionChanged(object source, EventArgs e)
		{
			if (!ignoreSelectionChanges) {
				//LoggingService.Debug("TextSelectionChanged.");
				selection = GetCurrentTextSelection();
				findFirst = true;
			}
		}
		
		void SetCaretPosition(TextArea textArea, int offset)
		{
			textArea.Caret.Position = textArea.Document.OffsetToPosition(offset);
		}
		
		void InitSelectionSearch()
		{
			findFirst = true;
			selection = GetCurrentTextSelection();
			AddSelectionChangedHandler(SearchReplaceUtilities.GetActiveTextEditor());
            WorkbenchSingleton.DockContainer.ActiveDocumentChanged += WorkbenchActiveViewContentChanged;
		}
		
		void RemoveSelectionSearchHandlers()
		{
			RemoveSelectionChangedHandler();
			RemoveActiveWindowChangedHandler();
		}
		
		/// <summary>
		/// action: 0 = find, 1 = mark, 2 = replace
		/// </summary>
		void RunAllInSelection(int action)
		{
			const IProgressMonitor monitor = null;
			
			int startOffset = Math.Min(selection.Offset, selection.EndOffset);
			int endOffset = Math.Max(selection.Offset, selection.EndOffset);
			
			SearchReplaceUtilities.SelectText(textEditor, startOffset, endOffset);
			SetCaretPosition(textEditor.ActiveTextAreaControl.TextArea, startOffset);
			
			try {
				ignoreSelectionChanges = true;
				if (action == 0)
					SearchInFilesManager.FindAll(startOffset, endOffset - startOffset, monitor);
				else if (action == 1)
					SearchReplaceManager.MarkAll(startOffset, endOffset - startOffset, monitor);
				else if (action == 2)
					SearchReplaceManager.ReplaceAll(startOffset, endOffset - startOffset, monitor);
				SearchReplaceUtilities.SelectText(textEditor, startOffset, endOffset);
			} finally {
				ignoreSelectionChanges = false;
			}
		}
		
		void ReplaceInSelection()
		{
			int startOffset = Math.Min(selection.Offset, selection.EndOffset);
			int endOffset = Math.Max(selection.Offset, selection.EndOffset);
			
			if (findFirst) {
				SetCaretPosition(textEditor.ActiveTextAreaControl.TextArea, startOffset);
			}
			
			try {
				ignoreSelectionChanges = true;
				if (findFirst) {
					findFirst = false;
					SearchReplaceManager.ReplaceFirstInSelection(startOffset, endOffset - startOffset, null);
				} else {
					findFirst = !SearchReplaceManager.ReplaceNextInSelection(null);
					if (findFirst) {
						SearchReplaceUtilities.SelectText(textEditor, startOffset, endOffset);
					}
				}
			} finally {
				ignoreSelectionChanges = false;
			}
		}
		
		/// <summary>
		/// Enables the various find, bookmark and replace buttons
		/// depending on whether any find string has been entered. The buttons
		/// are disabled otherwise.
		/// </summary>
		void EnableButtons(bool enabled)
		{
			if (searchAndReplaceMode == SearchAndReplaceMode.Replace) {
				this.Button_Replace.Enabled = enabled;
                this.Button_ReplaceAll.Enabled = enabled;
			} else {
                this.Button_BookmarkAll.Enabled = enabled;
                this.Button_Find.Enabled = enabled;
			}
			this.Button_FindNext.Enabled = enabled;
		}
		
		/// <summary>
		/// Returns true if the string entered in the find or replace text box
		/// is not an empty string.
		/// </summary>
		bool HasFindPattern {
			get {
				return ComboBox_FindWhat.Text.Length != 0;
			}
		}
		
		/// <summary>
		/// Updates the enabled/disabled state of the search and replace buttons
		/// after the search or replace text has changed.
		/// </summary>
		void FindPatternChanged(object source, EventArgs e)
		{
			EnableButtons(HasFindPattern);
		}
    }
}
