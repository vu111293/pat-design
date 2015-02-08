using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Fireball.CodeEditor.SyntaxFiles;
using Fireball.Docking;
using Fireball.Syntax;
using Fireball.Syntax.SyntaxDocumentParsers;
using Fireball.Windows.Forms;
using PAT.Common.Ultility;
using PAT.GUI.Forms;

namespace PAT.GUI.Docking
{
    public class EditorTabItemOld : DockableWindow
    {
        public delegate void TabActivitedHandler(EditorTabItem tab);
        public event TabActivitedHandler TabActivited;

        public ContextMenuStrip EditorContextMenuStrip;
        private IContainer components;
        private ToolStripMenuItem mnuUndo;
        private ToolStripMenuItem mnuRedo;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripMenuItem mnuCut;
        private ToolStripMenuItem mnuCopy;
        private ToolStripMenuItem mnuPaste;
        private ToolStripMenuItem mnuSelectAll;
        private CodeEditorControl _EditorControl = null;
        private static int counter = 1;

        public event EventHandler FileSaved;

        public event EventHandler<LanguageChangedEventArgs> LanguageChanged;

        public string LanguageName;

        public string FileExtension;
        private ToolStripSeparator toolStripSeparator3;
        private ToolStripSeparator toolStripSeparator4;
        private ToolStripMenuItem mnuComment;
        private ToolStripMenuItem mnuUnComment;
        private ToolStripMenuItem mnuFindUsage;
        private ToolStripMenuItem mnuGoToDeclarition;
        private ToolStripSeparator toolStripSeparator2;
        private ToolStripMenuItem mnuRename;

        public ToolStripMenuItem WindowMenuItem;


        public override string TabText
        {
            get
            {
                return DockHandler.TabText;
            }
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

        public EditorTabItemOld()
        {
            InitializeComponent();

            _EditorControl = new CodeEditorControl();
            //_EditorControl.ShowGutterMargin = true;
            //_EditorControl.ShowTabGuides = true;

            _EditorControl.Dock = DockStyle.Fill;

            _EditorControl.ContextMenuStrip = EditorContextMenuStrip;

            _EditorControl.Name = "codeeditor";

            this.Controls.Add(_EditorControl);

            this.Text = "Document " + counter;
            counter++;

            _EditorControl.FileNameChanged += new EventHandler(_EditorControl_FileNameChanged);

            _EditorControl.FileSavedChanged += new EventHandler(_EditorControl_FileSaved);

            _EditorControl.FontName = "Courier New";

            _EditorControl.FontSize = 10;

            _EditorControl.LineNumberForeColor = Color.SteelBlue;
            _EditorControl.LineNumberBorderColor = Color.SteelBlue;

            this.CodeEditor.Saved = true;

            this.Padding = new Padding(1, 2, 1, 1);
            this.DockableAreas = DockAreas.Document;

            WindowMenuItem = new ToolStripMenuItem();

            //this.mnuGoToDeclarition.Visible = false;
            this.mnuFindUsage.Visible = false;
        }

        void EditorTabItem_MouseDown(object sender, MouseEventArgs e)
        {
            this.MainMenuStrip.Show();
        }



        void _EditorControl_FileSaved(object sender, EventArgs e)
        {
            if (FileSaved != null)
                FileSaved(sender, e);
        }

        void _EditorControl_FileNameChanged(object sender, EventArgs e)
        {
            this.Text = Path.GetFileName(this._EditorControl.FileName);
        }

        public CodeEditorControl CodeEditor
        {
            get
            {
                return _EditorControl;
            }
        }

        public void Open(string filename)
        {
            this._EditorControl.Open(filename);

            //changes by liuyang
            //CodeEditorSyntaxLoader.SetSyntax(this._EditorControl, filename);

            if (LanguageChanged != null)
                LanguageChanged(this, new LanguageChangedEventArgs(this._EditorControl.Document.Parser.Language));

            this.ToolTipText = filename;

        }

        public void Save(string filename)
        {
            this._EditorControl.Save(filename);

            if (this._EditorControl.Document.Parser.Language == null)
            {
                CodeEditorSyntaxLoader.SetSyntax(this._EditorControl, filename);
                if (LanguageChanged != null)
                    LanguageChanged(this, new LanguageChangedEventArgs(this._EditorControl.Document.Parser.Language));
            }
            this.ToolTipText = filename;
        }


        public void SetSyntaxLanguage(Language language)
        {
            
             FileType ft = language.FileTypes[0] as FileType;
            LanguageName = ft.Extension.Substring(1).ToUpper();
            FileExtension = ft.Name + " (*" + ft.Extension + ")|*" + ft.Extension + "|All File (*.*)|*.*";

            this._EditorControl.Document.Parser.Init(language);

            this.Icon = Ultility.GetIcon(language.Name);

            if (LanguageChanged != null)
                LanguageChanged(this, new LanguageChangedEventArgs(language));
        }

        public void SetSyntaxLanguage(SyntaxLanguage syntaxLanguage)
        {
            CodeEditorSyntaxLoader.SetSyntax(this._EditorControl, syntaxLanguage);
            if (LanguageChanged != null)
                LanguageChanged(this,
                                new LanguageChangedEventArgs(this._EditorControl.Document.Parser.Language));
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EditorTabItem));
            this.EditorContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.mnuGoToDeclarition = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuFindUsage = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuRename = new System.Windows.Forms.ToolStripMenuItem();
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
            this.EditorContextMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // EditorContextMenuStrip
            // 
            this.EditorContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuGoToDeclarition,
            this.mnuFindUsage,
            this.mnuRename,
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
            this.EditorContextMenuStrip.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.contextMenuStrip1_ItemClicked);
            this.EditorContextMenuStrip.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip1_Opening);
            // 
            // mnuGoToDeclarition
            // 
            this.mnuGoToDeclarition.Name = "mnuGoToDeclarition";
            resources.ApplyResources(this.mnuGoToDeclarition, "mnuGoToDeclarition");
            // 
            // mnuFindUsage
            // 
            this.mnuFindUsage.Name = "mnuFindUsage";
            resources.ApplyResources(this.mnuFindUsage, "mnuFindUsage");
            // 
            // mnuRename
            // 
            this.mnuRename.Name = "mnuRename";
            resources.ApplyResources(this.mnuRename, "mnuRename");
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
            // 
            // mnuUnComment
            // 
            this.mnuUnComment.Image = global::PAT.GUI.Properties.Resources.uncomment;
            this.mnuUnComment.Name = "mnuUnComment";
            resources.ApplyResources(this.mnuUnComment, "mnuUnComment");
            // 
            // EditorTabItem
            // 
            resources.ApplyResources(this, "$this");
            this.Name = "EditorTabItem";
            this.EditorContextMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        private void contextMenuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if (e.ClickedItem == mnuCopy)
                _EditorControl.Copy();
            else if (e.ClickedItem == mnuCut)
                _EditorControl.Cut();
            else if (e.ClickedItem == mnuPaste)
                _EditorControl.Paste();
            else if (e.ClickedItem == mnuUndo)
                _EditorControl.Undo();
            else if (e.ClickedItem == mnuRedo)
                _EditorControl.Redo();
            else if (e.ClickedItem == mnuSelectAll)
                _EditorControl.SelectAll();
            else if (e.ClickedItem == mnuComment)
            {
                Comment();
            }
            else if(e.ClickedItem == mnuUnComment)
            {
                UnComment(); 
            }
            else if (e.ClickedItem == mnuGoToDeclarition)
            {
                if (e.ClickedItem.Tag != null)
                {
                    string word = e.ClickedItem.Tag.ToString();
                    string text = _EditorControl.Document.Text;
                    int index = text.IndexOf(word);
                    int totalIndex = 0;
                    while (index > 0)
                    {
                        totalIndex += index;

                        string preText = text.Substring(0, index).TrimEnd(' ', '\r', '\n');
                        if (preText.EndsWith("define"))
                        {
                            _EditorControl.Selection.SelStart = totalIndex;
                            _EditorControl.Selection.SelLength = word.Length;

                            _EditorControl.GotoLine(_EditorControl.Selection.Bounds.FirstRow);
                            _EditorControl.ScrollIntoView();

                            _EditorControl.Selection.SelStart = totalIndex;
                            _EditorControl.Selection.SelLength = word.Length;
                            break;
                        }
                        else
                        {
                            totalIndex += (text.Length - index); 
                            text = text.Substring(index + word.Length);
                            text = text.TrimStart('(', ')', ' ', '\r', '\n');
                            totalIndex -= text.Length; 
                            if (text.StartsWith("="))
                            {

                                _EditorControl.Selection.SelStart = totalIndex;
                                _EditorControl.Selection.SelLength = word.Length;

                                _EditorControl.GotoLine(_EditorControl.Selection.Bounds.FirstRow);
                                _EditorControl.ScrollIntoView();

                                _EditorControl.Selection.SelStart = totalIndex;
                                _EditorControl.Selection.SelLength = word.Length;

                                break;
                            }
                            else
                            {
                                int index2 = text.IndexOf(")");
                                if (index2 > 0)
                                {
                                    string temp = text.Substring(0, index2);
                                    string[] parts = temp.Split(new char[] { ',', ' ', '\r', '\n' },
                                                                StringSplitOptions.RemoveEmptyEntries);
                                    bool isParamter = true;
                                    foreach (string s in parts)
                                    {
                                        if (!Common.Ultility.Ultility.IsAValidName(s))
                                        {
                                            isParamter = false;
                                            break;
                                        }
                                    }

                                    if (isParamter)
                                    {
                                        totalIndex += text.Length; 
                                        text = text.Substring(index2).TrimStart(')', ' ', '\r', '\n');
                                        totalIndex -= text.Length; 

                                        if (text.StartsWith("="))
                                        {

                                            _EditorControl.Selection.SelStart = totalIndex;
                                            _EditorControl.Selection.SelLength = word.Length;

                                            _EditorControl.GotoLine(_EditorControl.Selection.Bounds.FirstRow);
                                            _EditorControl.ScrollIntoView();

                                            _EditorControl.Selection.SelStart = totalIndex;
                                            _EditorControl.Selection.SelLength = word.Length;

                                            break;
                                        }
                                    }
                                }
                            }

                        }
                        
                        index = text.IndexOf(word);
                    }
                }
            }
            //else if (e.ClickedItem == mnuFindUsage)
            //{
            //    UnComment();
            //}
            else if (e.ClickedItem == mnuRename)
            {
                if(e.ClickedItem.Tag == null)
                {
                    string word = this._EditorControl.Selection.Text;

                    bool enable = Common.Ultility.Ultility.IsAValidName(word);
                    Word formatedWord = _EditorControl.Document.GetFormatWordFromPos(this._EditorControl.Selection.Bounds.FirstColumn + 1, this._EditorControl.Selection.Bounds.FirstRow);
                    if (formatedWord != null)
                    {
                        if (formatedWord.Style.ForeColor == Color.Black && enable)
                        {
                            Rename(word);
                        }
                    }

                }
                else
                {
                    Rename(e.ClickedItem.Tag.ToString());    
                }
                
            }
        }

        private void Rename(string oldName)
        {
            RenameForm renameForm = new RenameForm(oldName);
            if(renameForm.ShowDialog() == DialogResult.OK)
            {
                string newName = renameForm.TextBox_NewName.Text;
                    
                _EditorControl.Caret.Position.X = 0;
                _EditorControl.Caret.Position.Y = 0;
                while (_EditorControl.ActiveViewControl.SelectNext(oldName, true, true, false, true))
                {
                    this._EditorControl.ActiveViewControl.ReplaceSelection(newName);
                }

                _EditorControl.Selection.ClearSelection();    
            }
        }

        public void Comment()
        {
            if ((this._EditorControl.Document.Parser != null) && (this._EditorControl.Document.Parser is DefaultParser))
            {
                DefaultParser parser = this._EditorControl.Document.Parser as DefaultParser;
                if (parser.Language != null)
                {
                    this._EditorControl.Selection.Indent("//");
                }
            }
        }

        public void UnComment()
        {
            if ((this._EditorControl.Document.Parser != null) && (this._EditorControl.Document.Parser is DefaultParser))
            {
                DefaultParser parser = this._EditorControl.Document.Parser as DefaultParser;
                if (parser.Language != null)
                {
                    this._EditorControl.Selection.Outdent("//");
                }
            }
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            mnuCut.Enabled = _EditorControl.CanCopy;
            mnuCopy.Enabled = _EditorControl.CanCopy;
            mnuPaste.Enabled = _EditorControl.CanPaste;
            mnuRedo.Enabled = _EditorControl.CanRedo;
            mnuUndo.Enabled = _EditorControl.CanUndo;
            mnuSelectAll.Enabled = _EditorControl.CanSelect;

            string word = this._EditorControl.Selection.Text;

            bool enable = Common.Ultility.Ultility.IsAValidName(word);
           Word formatedWord = _EditorControl.Document.GetFormatWordFromPos(this._EditorControl.Selection.Bounds.FirstColumn+1, this._EditorControl.Selection.Bounds.FirstRow);
           if (formatedWord != null)
            {
                enable = formatedWord.Style.ForeColor == Color.Black && enable;
                this.mnuRename.Tag = word;
                this.mnuGoToDeclarition.Tag = word;
                this.mnuFindUsage.Tag = word;
            }
            
            this.mnuRename.Enabled = enable;
            this.mnuGoToDeclarition.Enabled = enable;
            this.mnuFindUsage.Enabled = enable;
        }
    }

    public class LanguageChangedEventArgs : EventArgs
    {
        private Language _Language = null;

        public LanguageChangedEventArgs(Language language)
        {
            _Language = language;
        }

        public Language Language
        {
            get
            {
                return _Language;
            }
        }
    }
}