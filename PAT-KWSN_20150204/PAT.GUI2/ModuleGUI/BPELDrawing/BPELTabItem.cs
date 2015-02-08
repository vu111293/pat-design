using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Xml;
using Fireball.Docking;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Document;
using PAT.Common;
using PAT.Common.GUI.BPELModule;
using PAT.Common.GUI.Drawing;
using PAT.GUI.EditingFunction.CodeCompletion;
using PAT.GUI.Properties;
using Tools.Diagrams;
using PAT.GUI.Docking;
using CanvasItemData = PAT.Common.GUI.Drawing.LTSCanvas.CanvasItemData;
using Parsing = PAT.Common.GUI.BPELModule.Parsing;

namespace PAT.GUI.LTSDrawing
{
    public class BPELTabItem : EditorTabItem
    {
        private ContextMenuStrip contextMenuStrip1;
        private IContainer components;

        private ToolStripMenuItem MenuButton_NewState;
        private ToolStripMenuItem MenuButton_AddLink;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripMenuItem MenuButton_SetInitial;
        private ToolStripMenuItem MenuButton_Delete;
        private SplitContainer splitContainer1;
        private ToolStripContainer toolStripContainer1;
        private ToolStrip toolStrip1;
        private ToolStripButton Button_AddNewState;
        private ToolStripButton Button_Delete;
        private ToolStripButton Button_AddLink;
        private ToolStripSeparator toolStripSeparator3;
        private ToolStripButton Button_ExportBMP;
        private ToolStripButton Button_ExpandAllCommand;
        private ToolStripButton Button_CollapseAllCommand;
        private ToolStripButton Button_MatchAllWidthsCommand;
        private ToolStripButton Button_ShrinkAllWidthsCommand;
        private ToolStripButton Button_ZoomIn;
        private ToolStripButton Button_ZoomOut;
        private ImageList imageList2;
        private ContextMenuStrip contextMenuStrip2;
        private ToolStripMenuItem addProcessToolStripMenuItem;
        private ToolStripMenuItem deleteProcessToolStripMenuItem;
        private ToolStripMenuItem processDetailsToolStripMenuItem;
        private TreeView TreeView_Structure;
        private ToolStripButton Button_AutoRange;
        private LTSCanvas Canvas;

        private System.Windows.Forms.TreeNode AssertNode;
        private System.Windows.Forms.TreeNode FilesNode;
        private ToolStripButton Button_AddNewNail;
        private ToolStripMenuItem MenuButton_Edit;
        private ToolStripMenuItem duplicateProcessToolStripMenuItem;
        private ToolStripMenuItem addExistingFileToolStripMenuItem1;
        private ToolStripMenuItem deleteFileToolStripMenuItem1;
        private OpenFileDialog openFileDialog1;
        private ToolStripMenuItem MenuButton_NewNail;

        public BPELTabItem(string moduleName)
        {

            InitializeComponent();

            AssertNode = new System.Windows.Forms.TreeNode(Parsing.ASSERTION_NODE_NAME);
            FilesNode = new System.Windows.Forms.TreeNode(Parsing.FILES_NODE_NAME);
            AssertNode.Tag = new FileTag("", FileTag.FileType.assertion);
            System.Windows.Forms.TreeNode MainTreeNode = new System.Windows.Forms.TreeNode("BPEL Model", new System.Windows.Forms.TreeNode[] {
            AssertNode,
            FilesNode});

            MainTreeNode.Name = "Root";
            MainTreeNode.Text = "BPEL Model";
            this.TreeView_Structure.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            MainTreeNode});


            AssertNode.Name = Parsing.ASSERTION_NODE_NAME;
            AssertNode.StateImageIndex = 0;
            AssertNode.Text = Parsing.ASSERTION_NODE_NAME;
            FilesNode.Name = Parsing.FILES_NODE_NAME;
            FilesNode.StateImageIndex = 1;
            FilesNode.Text = Parsing.FILES_NODE_NAME;


            AddEventHandlerForButtons();

            textEditorControl = new SharpDevelopTextAreaControl();
            textEditorControl.Dock = DockStyle.Fill;
            textEditorControl.ContextMenuStrip = EditorContextMenuStrip;
            textEditorControl.BorderStyle = BorderStyle.Fixed3D;
            textEditorControl.Visible = false;

            this.splitContainer1.Panel2.Controls.Add(textEditorControl);


            //Canvas.AutoArrange();

            //this.toolStripContainer1.ContentPanel.Controls.Add(Canvas);

            //this.TabText = project.Project.ProjectName + " Diagram";

            //this.Padding = new Padding(1, 2, 1, 1);
            //this.DockableAreas = DockAreas.Document;

            ////this.Dock = DockStyle.Fill;
            //this.Icon = Ultility.GetIcon("Diagram");

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
            //ForceFoldingUpdate();
            //textEditorControl.InitializeFormatter();
            //textEditorControl.ActivateQuickClassBrowserOnDemand();


            textEditorControl.Document.FoldingManager.FoldingStrategy = new FoldingStrategy();

            // Highlight the matching bracket or not...
            this.textEditorControl.ShowMatchingBracket = true;

            this.textEditorControl.BracketMatchingStyle = BracketMatchingStyle.Before;


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

            //try
            //{

            //    IHighlightingStrategy strategy = HighlightingStrategyFactory.CreateHighlightingStrategy(".csp");
            //    textEditorControl.Document.HighlightingStrategy = strategy;
            //    textEditorControl.InitializeAdvancedHighlighter();
            //    ModuleName = "Labeled Transition Systems";
            //    this.Icon = Ultility.GetIcon(ModuleName);
            //    FileExtension = ModuleName + " (*.lts)|*.lts|All File (*.*)|*.*";


            //}
            //catch (HighlightingDefinitionInvalidException ex)
            //{
            //    MessageBox.Show("Error: file format is not supported!", Common.Ultility.Ultility.APPLICATION_NAME, MessageBoxButtons.OK, MessageBoxIcon.Error);
            //}

            this.TreeView_Structure.HideSelection = false;
            splitContainer1.SplitterDistance = 100;

            addProcessToolStripMenuItem.PerformClick();

            this.TreeView_Structure.ExpandAll();

            Button_AddNewNail.Visible = false;

            //show the declaration
            TreeView_Structure.SelectedNode = AssertNode;
            TreeView_Structure_NodeMouseDoubleClick(null, new TreeNodeMouseClickEventArgs(AssertNode, MouseButtons.Left, 2, 0, 0));
        }

        private void AddEventHandlerForButtons()
        {
            foreach (ToolStripItem button in this.toolStrip1.Items)
            {
                if (button is ToolStripButton)
                {
                    ((ToolStripButton)button).CheckStateChanged += new EventHandler(button_CheckStateChanged);
                }
            }
        }


        protected void textEditorControl_TextChanged(object sender, EventArgs e)
        {
            textEditorControl.Document.FoldingManager.UpdateFoldings(null, null);
            //if (this.TreeView_Structure.SelectedNode != null)
            //{
            //    this.TreeView_Structure.SelectedNode.Tag = textEditorControl.Text;
            //}
            //if(this.LastSelectedNode!=null)
            //{
            //    this.LastSelectedNode.Tag =textEditorControl.Text;
            //}
            FileTag.UpdateTag(this.LastSelectedNode,textEditorControl.Text);
            TextEditorChanged = true;
            SetDirty();
        }

        #region InitializeComponent
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BPELTabItem));
            this.toolStripContainer1 = new System.Windows.Forms.ToolStripContainer();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.Button_AddNewState = new System.Windows.Forms.ToolStripButton();
            this.Button_Delete = new System.Windows.Forms.ToolStripButton();
            this.Button_AddLink = new System.Windows.Forms.ToolStripButton();
            this.Button_AddNewNail = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.Button_ExportBMP = new System.Windows.Forms.ToolStripButton();
            this.Button_AutoRange = new System.Windows.Forms.ToolStripButton();
            this.Button_ExpandAllCommand = new System.Windows.Forms.ToolStripButton();
            this.Button_CollapseAllCommand = new System.Windows.Forms.ToolStripButton();
            this.Button_MatchAllWidthsCommand = new System.Windows.Forms.ToolStripButton();
            this.Button_ShrinkAllWidthsCommand = new System.Windows.Forms.ToolStripButton();
            this.Button_ZoomIn = new System.Windows.Forms.ToolStripButton();
            this.Button_ZoomOut = new System.Windows.Forms.ToolStripButton();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.MenuButton_NewState = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuButton_AddLink = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuButton_NewNail = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuButton_Edit = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuButton_Delete = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.MenuButton_SetInitial = new System.Windows.Forms.ToolStripMenuItem();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.TreeView_Structure = new System.Windows.Forms.TreeView();
            this.contextMenuStrip2 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.addExistingFileToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteFileToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.imageList2 = new System.Windows.Forms.ImageList(this.components);
            this.addProcessToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteProcessToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.processDetailsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.duplicateProcessToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.toolStripContainer1.TopToolStripPanel.SuspendLayout();
            this.toolStripContainer1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.contextMenuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.contextMenuStrip2.SuspendLayout();
            this.SuspendLayout();
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
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
            // toolStripContainer1
            // 
            // 
            // toolStripContainer1.ContentPanel
            // 
            resources.ApplyResources(this.toolStripContainer1.ContentPanel, "toolStripContainer1.ContentPanel");
            resources.ApplyResources(this.toolStripContainer1, "toolStripContainer1");
            this.toolStripContainer1.Name = "toolStripContainer1";
            // 
            // toolStripContainer1.TopToolStripPanel
            // 
            this.toolStripContainer1.TopToolStripPanel.Controls.Add(this.toolStrip1);
            // 
            // toolStrip1
            // 
            resources.ApplyResources(this.toolStrip1, "toolStrip1");
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.Button_AddNewState,
            this.Button_Delete,
            this.Button_AddLink,
            this.Button_AddNewNail,
            this.toolStripSeparator3,
            this.Button_ExportBMP,
            this.Button_AutoRange,
            this.Button_ExpandAllCommand,
            this.Button_CollapseAllCommand,
            this.Button_MatchAllWidthsCommand,
            this.Button_ShrinkAllWidthsCommand,
            this.Button_ZoomIn,
            this.Button_ZoomOut});
            this.toolStrip1.Name = "toolStrip1";
            // 
            // Button_AddNewState
            // 
            this.Button_AddNewState.CheckOnClick = true;
            this.Button_AddNewState.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.Button_AddNewState.Image = global::PAT.GUI.Properties.Resources.plus_circle;
            this.Button_AddNewState.Name = "Button_AddNewState";
            resources.ApplyResources(this.Button_AddNewState, "Button_AddNewState");
            // 
            // Button_Delete
            // 
            this.Button_Delete.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.Button_Delete, "Button_Delete");
            this.Button_Delete.Image = global::PAT.GUI.Properties.Resources.delete;
            this.Button_Delete.Name = "Button_Delete";
            this.Button_Delete.Click += new System.EventHandler(this.Button_Delete_Click);
            // 
            // Button_AddLink
            // 
            this.Button_AddLink.CheckOnClick = true;
            this.Button_AddLink.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.Button_AddLink, "Button_AddLink");
            this.Button_AddLink.Name = "Button_AddLink";
            this.Button_AddLink.Click += new System.EventHandler(this.Button_AddLink_Click);
            // 
            // Button_AddNewNail
            // 
            this.Button_AddNewNail.CheckOnClick = true;
            this.Button_AddNewNail.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.Button_AddNewNail.Image = global::PAT.GUI.Properties.Resources.nail;
            resources.ApplyResources(this.Button_AddNewNail, "Button_AddNewNail");
            this.Button_AddNewNail.Name = "Button_AddNewNail";
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            resources.ApplyResources(this.toolStripSeparator3, "toolStripSeparator3");
            // 
            // Button_ExportBMP
            // 
            this.Button_ExportBMP.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.Button_ExportBMP, "Button_ExportBMP");
            this.Button_ExportBMP.Name = "Button_ExportBMP";
            this.Button_ExportBMP.Click += new System.EventHandler(this.Button_AutoRange_Click_1);
            // 
            // Button_AutoRange
            // 
            this.Button_AutoRange.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.Button_AutoRange, "Button_AutoRange");
            this.Button_AutoRange.Name = "Button_AutoRange";
            this.Button_AutoRange.Click += new System.EventHandler(this.Button_AutoRange_Click);
            // 
            // Button_ExpandAllCommand
            // 
            this.Button_ExpandAllCommand.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.Button_ExpandAllCommand, "Button_ExpandAllCommand");
            this.Button_ExpandAllCommand.Name = "Button_ExpandAllCommand";
            // 
            // Button_CollapseAllCommand
            // 
            this.Button_CollapseAllCommand.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.Button_CollapseAllCommand, "Button_CollapseAllCommand");
            this.Button_CollapseAllCommand.Name = "Button_CollapseAllCommand";
            // 
            // Button_MatchAllWidthsCommand
            // 
            this.Button_MatchAllWidthsCommand.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.Button_MatchAllWidthsCommand, "Button_MatchAllWidthsCommand");
            this.Button_MatchAllWidthsCommand.Name = "Button_MatchAllWidthsCommand";
            // 
            // Button_ShrinkAllWidthsCommand
            // 
            this.Button_ShrinkAllWidthsCommand.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.Button_ShrinkAllWidthsCommand, "Button_ShrinkAllWidthsCommand");
            this.Button_ShrinkAllWidthsCommand.Name = "Button_ShrinkAllWidthsCommand";
            // 
            // Button_ZoomIn
            // 
            this.Button_ZoomIn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.Button_ZoomIn, "Button_ZoomIn");
            this.Button_ZoomIn.Name = "Button_ZoomIn";
            this.Button_ZoomIn.Click += new System.EventHandler(this.Button_ZoomIn_Click);
            // 
            // Button_ZoomOut
            // 
            this.Button_ZoomOut.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.Button_ZoomOut, "Button_ZoomOut");
            this.Button_ZoomOut.Name = "Button_ZoomOut";
            this.Button_ZoomOut.Click += new System.EventHandler(this.Button_ZoomOut_Click);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MenuButton_NewState,
            this.MenuButton_AddLink,
            this.MenuButton_NewNail,
            this.MenuButton_Edit,
            this.MenuButton_Delete,
            this.toolStripSeparator1,
            this.MenuButton_SetInitial});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            resources.ApplyResources(this.contextMenuStrip1, "contextMenuStrip1");
            this.contextMenuStrip1.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip1_Opening);
            // 
            // MenuButton_NewState
            // 
            this.MenuButton_NewState.Image = global::PAT.GUI.Properties.Resources.plus_circle;
            this.MenuButton_NewState.Name = "MenuButton_NewState";
            resources.ApplyResources(this.MenuButton_NewState, "MenuButton_NewState");
            this.MenuButton_NewState.Click += new System.EventHandler(this.Button_AddNewState_Click);
            // 
            // MenuButton_AddLink
            // 
            this.MenuButton_AddLink.Image = global::PAT.GUI.Properties.Resources.link;
            resources.ApplyResources(this.MenuButton_AddLink, "MenuButton_AddLink");
            this.MenuButton_AddLink.Name = "MenuButton_AddLink";
            // 
            // MenuButton_NewNail
            // 
            this.MenuButton_NewNail.Image = global::PAT.GUI.Properties.Resources.nail;
            this.MenuButton_NewNail.Name = "MenuButton_NewNail";
            resources.ApplyResources(this.MenuButton_NewNail, "MenuButton_NewNail");
            this.MenuButton_NewNail.Click += new System.EventHandler(this.Button_AddNewNail_Click);
            // 
            // MenuButton_Edit
            // 
            this.MenuButton_Edit.Name = "MenuButton_Edit";
            resources.ApplyResources(this.MenuButton_Edit, "MenuButton_Edit");
            this.MenuButton_Edit.Click += new System.EventHandler(this.Button_Edit_Click);
            // 
            // MenuButton_Delete
            // 
            this.MenuButton_Delete.Image = global::PAT.GUI.Properties.Resources.delete;
            this.MenuButton_Delete.Name = "MenuButton_Delete";
            resources.ApplyResources(this.MenuButton_Delete, "MenuButton_Delete");
            this.MenuButton_Delete.Click += new System.EventHandler(this.Button_Delete_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            resources.ApplyResources(this.toolStripSeparator1, "toolStripSeparator1");
            // 
            // MenuButton_SetInitial
            // 
            resources.ApplyResources(this.MenuButton_SetInitial, "MenuButton_SetInitial");
            this.MenuButton_SetInitial.Name = "MenuButton_SetInitial";
            this.MenuButton_SetInitial.Click += new System.EventHandler(this.MenuButton_Initial_Click);
            // 
            // splitContainer1
            // 
            resources.ApplyResources(this.splitContainer1, "splitContainer1");
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.TreeView_Structure);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.toolStripContainer1);
            // 
            // TreeView_Structure
            // 
            this.TreeView_Structure.ContextMenuStrip = this.contextMenuStrip2;
            resources.ApplyResources(this.TreeView_Structure, "TreeView_Structure");
            this.TreeView_Structure.HideSelection = false;
            this.TreeView_Structure.Name = "TreeView_Structure";
            this.TreeView_Structure.StateImageList = this.imageList2;
            this.TreeView_Structure.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.TreeView_Structure_NodeMouseClick);
            this.TreeView_Structure.NodeMouseDoubleClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.TreeView_Structure_NodeMouseDoubleClick);
            // 
            // contextMenuStrip2
            // 
            this.contextMenuStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addExistingFileToolStripMenuItem1,
            this.deleteFileToolStripMenuItem1});
            this.contextMenuStrip2.Name = "contextMenuStrip2";
            resources.ApplyResources(this.contextMenuStrip2, "contextMenuStrip2");
            this.contextMenuStrip2.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip2_Opening);
            // 
            // addExistingFileToolStripMenuItem1
            // 
            this.addExistingFileToolStripMenuItem1.Name = "addExistingFileToolStripMenuItem1";
            resources.ApplyResources(this.addExistingFileToolStripMenuItem1, "addExistingFileToolStripMenuItem1");
            this.addExistingFileToolStripMenuItem1.Click += new System.EventHandler(this.addExistingFileToolStripMenuItem1_Click);
            // 
            // deleteFileToolStripMenuItem1
            // 
            this.deleteFileToolStripMenuItem1.Name = "deleteFileToolStripMenuItem1";
            resources.ApplyResources(this.deleteFileToolStripMenuItem1, "deleteFileToolStripMenuItem1");
            this.deleteFileToolStripMenuItem1.Click += new System.EventHandler(this.deleteFileToolStripMenuItem1_Click);
            // 
            // imageList2
            // 
            this.imageList2.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList2.ImageStream")));
            this.imageList2.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList2.Images.SetKeyName(0, "declare.png");
            this.imageList2.Images.SetKeyName(1, "channel.png");
            this.imageList2.Images.SetKeyName(2, "templates.png");
            this.imageList2.Images.SetKeyName(3, "questionMark.png");
            // 
            // addProcessToolStripMenuItem
            // 
            this.addProcessToolStripMenuItem.Name = "addProcessToolStripMenuItem";
            resources.ApplyResources(this.addProcessToolStripMenuItem, "addProcessToolStripMenuItem");
            // 
            // deleteProcessToolStripMenuItem
            // 
            this.deleteProcessToolStripMenuItem.Name = "deleteProcessToolStripMenuItem";
            resources.ApplyResources(this.deleteProcessToolStripMenuItem, "deleteProcessToolStripMenuItem");
            // 
            // processDetailsToolStripMenuItem
            // 
            this.processDetailsToolStripMenuItem.Name = "processDetailsToolStripMenuItem";
            resources.ApplyResources(this.processDetailsToolStripMenuItem, "processDetailsToolStripMenuItem");
            // 
            // duplicateProcessToolStripMenuItem
            // 
            this.duplicateProcessToolStripMenuItem.Name = "duplicateProcessToolStripMenuItem";
            resources.ApplyResources(this.duplicateProcessToolStripMenuItem, "duplicateProcessToolStripMenuItem");
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // BPELTabItem
            // 
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.splitContainer1);
            this.Name = "BPELTabItem";
            this.toolStripContainer1.TopToolStripPanel.ResumeLayout(false);
            this.toolStripContainer1.TopToolStripPanel.PerformLayout();
            this.toolStripContainer1.ResumeLayout(false);
            this.toolStripContainer1.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.contextMenuStrip1.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.contextMenuStrip2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        public class FileTag
        {
            public string content = "";
            public string FilePath = "";
            public enum FileType
            {
                files,
                assertion
            }
            public FileTag(string str, FileType type)
            {
                if (type == FileType.files)
                {
                    FileStream fileStream=null;
                    StreamReader sReader = null;
                    try
                    {
                  
                    this.FilePath = str;
                  fileStream = new FileStream(this.FilePath, FileMode.Open, FileAccess.Read);
                    sReader = new StreamReader(fileStream);
                    this.content = sReader.ReadToEnd();
                    }
                    finally
                    {
                        if (sReader != null)
                            sReader.Close();
                        if (fileStream != null)
                            fileStream.Close();
                    }

            }
                else if (type == FileType.assertion)
                {
                    this.content = str;
                }
            }

            public static void UpdateTag(TreeNode lastSelectedNode, string text)
            {
                if (lastSelectedNode == null)
                {
                    throw new Exception("impossible");
                }
                FileTag f = null;
                f = (FileTag) lastSelectedNode.Tag;
                f.content = text;
            }

            public static string GetTagText(TreeNode treeNode)
            {

                if (treeNode.Tag != null)
                {

                    FileTag f = (FileTag) treeNode.Tag;
                    return f.content;



                }
                return "";
            }

            public static string GetTagPath(TreeNode treeNode)
            {

                if (treeNode.Tag != null)
                {

                    FileTag f = (FileTag)treeNode.Tag;
                    return f.FilePath;



                }
                return "";
            }
        }


        void TreeView_Structure_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Button == MouseButtons.Right && e.Clicks == 1)
            {
                this.TreeView_Structure.SelectedNode = e.Node;
                contextMenuStrip2.Show(e.Location);
            }
        }

        private void button_CheckStateChanged(object sender, EventArgs e)
        {
            foreach (ToolStripItem button in this.toolStrip1.Items)
            {
                ToolStripButton buttonTemp = button as ToolStripButton;
                if (buttonTemp != null && buttonTemp != sender)
                {
                    buttonTemp.Checked = false;
                }
            }
            if (!this.Button_AddLink.Checked)
            {
                this.Canvas.temporaryNails.Clear();
                SetDirty();
            }
        }

        #endregion

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            this.MenuButton_Delete.Enabled = this.Button_Delete.Enabled;
            this.MenuButton_SetInitial.Enabled = (this.SelectedItems.Count == 1);
            this.MenuButton_NewNail.Enabled = (this.SelectedRoute != null);
            this.MenuButton_Edit.Enabled = this.MenuButton_Delete.Enabled;
        }

        private void Button_AutoRange_Click(object sender, EventArgs e)
        {
            Canvas.AutoArrange();
        }

        //private void Button_ExpandAllCommand_Click(object sender, EventArgs e)
        //{
        //    Canvas.ExpandAll();
        //}

        //private void Button_CollapseAllCommand_Click(object sender, EventArgs e)
        //{
        //    Canvas.CollapseAll();
        //}

        //private void Button_MatchAllWidthsCommand_Click(object sender, EventArgs e)
        //{
        //    Canvas.MatchAllWidths();
        //}

        //private void Button_ShrinkAllWidthsCommand_Click(object sender, EventArgs e)
        //{
        //    Canvas.ShrinkAllWidths();
        //}

        private void Button_ZoomIn_Click(object sender, EventArgs e)
        {
            Canvas.Zoom *= 1.1f;

        }

        private void Button_ZoomOut_Click(object sender, EventArgs e)
        {
            Canvas.Zoom *= 0.9f;
        }

        private void Button_AddNewState_Click(object sender, EventArgs e)
        {
            PointF p = Point.Empty;
            //Add from menu bar
            if (this.Button_AddNewState.Checked)
            {
                p = this.Canvas.LastMouseClickPosition;
            }
            else
            {
                p = this.Canvas.lastRightClickPosition;
            }

            StateItem classitem = new StateItem(false, "State " + Canvas.StateCounter);
            Canvas.StateCounter++;


            classitem.X = p.X;
            classitem.Y = p.Y;

            //classitem.Initialize();

            AddCanvasItem(classitem);
            Canvas.Refresh();
            SetDirty();
        }

        private void Button_AddNewNail_Click(object sender, EventArgs e)
        {
            //Add from context menu
            Route route;
            PointF p = PointF.Empty;

            if (this.Button_AddNewNail.Checked)
            {
                //Add from menubar
                route = (e as CanvasRouteEventArgs).CanvasItem;
                p = this.Canvas.LastMouseClickPosition;
            }
            else
            {
                //Add from context menu
                route = this.SelectedRoute;
                p = this.Canvas.lastRightClickPosition;
            }
            if (route != null)
            {
                NailItem nailItem = new NailItem(route);
                nailItem.X = p.X;
                nailItem.Y = p.Y;

                this.AddCanvasItem(nailItem);
                Canvas.Refresh();
                SetDirty();
            }
        }

        private void Button_Undo_Click(object sender, EventArgs e)
        {
            if (this.Canvas.currentStateIndex > 0)
            {
                this.Canvas.currentStateIndex--;
                XmlElement top = this.Canvas.undoStack[this.Canvas.currentStateIndex];
                this.Canvas.Restore(top);
                //this.Canvas.Refresh();
                SetDirty();
            }
        }

        private void Button_Redo_Click(object sender, EventArgs e)
        {
            if (this.Canvas.currentStateIndex < this.Canvas.undoStack.Count - 1)
            {
                this.Canvas.currentStateIndex++;
                XmlElement top = this.Canvas.undoStack[this.Canvas.currentStateIndex];
                this.Canvas.Restore(top);
                //this.Canvas.Refresh();
                SetDirty();
            }
        }

        public List<CanvasItemData> SelectedItems = new List<CanvasItemData>();
        private Route SelectedRoute;

        public static bool HighLightControlUsingRed = false;

        private void Canvas_CanvasItemSelected(object sender, CanvasItemEventArgs e)
        {
            SelectedRoute = null;
            if (this.Button_AddLink.Checked)
            {
                if (this.SelectedItems.Count > 0 && e.CanvasItem != null)
                {
                    CanvasItemData SelectedItem = this.SelectedItems[0];
                    if (SelectedItem.Item is StateItem && e.CanvasItem.Item is StateItem)
                    {
                        StateItem sourceState = SelectedItem.Item as StateItem;
                        StateItem targetState = e.CanvasItem.Item as StateItem;
                        Route r = new Route(SelectedItem.Item, targetState);
                        this.AddLink(r);
                        SetDirty();
                    }

                    e.CanvasItem.Item.HandleMouseUp(new PointF());
                    Button_AddLink.Checked = false;
                }
                else if (this.SelectedItems.Count == 0 && e.CanvasItem != null && e.CanvasItem.Item is StateItem)
                {
                    //Select the starting state of route
                    this.SelectedItems.Clear();
                    this.SelectedItems.Add(e.CanvasItem);
                    Canvas.temporaryNails.Add((e.CanvasItem.Item as StateItem).Center());
                }
                else if (this.SelectedItems.Count > 0 && e.CanvasItem == null)
                {
                    //Click on canvas to create a new nail
                    Canvas.temporaryNails.Add(this.Canvas.LastMouseClickPosition);
                }

            }
            else
            {
                this.SelectedItems.Clear();
                if (e.CanvasItem != null)
                {
                    this.SelectedItems.Add(e.CanvasItem);
                    e.CanvasItem.Item.HandleSelected(this.Canvas);
                    if (e.CanvasItem.Item is Transition)
                    {
                        Route route = (e.CanvasItem.Item as Transition).FindSelectedRouteBasedOnTransition(this.Canvas);
                        this.SelectedRoute = route;
                    }
                }
                CanvasItemData SelectedItem = e.CanvasItem;
                if (SelectedItem == null && this.Button_AddNewState.Checked)
                {
                    Button_AddNewState_Click(sender, e);
                    Button_AddNewState.Checked = false;
                }
            }
            this.Button_Delete.Enabled = (this.SelectedItems.Count > 0);
        }

        private void Canvas_CanvasItemsSelected(object sender, CanvasItemsEventArgs e)
        {
            this.SelectedItems.Clear();
            this.SelectedItems.AddRange(e.CanvasItem);
            this.Button_Delete.Enabled = (this.SelectedItems.Count > 0);
        }

        private void EditingLink(Route route)
        {
            TransitionEditingForm form = new TransitionEditingForm(route, this.Canvas.itemsList);
            if (form.ShowDialog() == DialogResult.OK)
            {
                this.StoreCurrentCanvas();
                SetDirty();
            }

            Canvas.Refresh();
        }

        private void Canvas_ItemDoubleClick(object sender, CanvasItemEventArgs e)
        {
            if (e.CanvasItem.Item is StateItem)
            {
                StateItem editedState = e.CanvasItem.Item as StateItem;
                string lastName = editedState.Name;
                StateEditingForm form = new StateEditingForm(editedState);
                if (form.ShowDialog() == DialogResult.OK)
                {
                    this.StoreCurrentCanvas();
                    SetDirty();
                    if (!this.Canvas.CheckStateNameDuplicate())
                    {
                        editedState.Name = lastName;
                    }
                }
                Canvas.Refresh();
            }
        }

        private void Canvas_RouteDoubleClick(object sender, CanvasRouteEventArgs e)
        {
            EditingLink(e.CanvasItem);
        }

        [System.Obsolete("Return wrong value when the canvas is bigger than the screen. Use LTSCanvas.LastMouseClickPosition")]
        public Point GetCurrentMousePosition()
        {
            Point screenPos = MousePosition;
            return Canvas.PointToClient(screenPos);
        }


        void Canvas_CanvasRouteSelected(object sender, CanvasRouteEventArgs e)
        {
            this.SelectedItems.Clear();
            if (this.Button_AddNewNail.Checked)
            {
                Button_AddNewNail_Click(sender, e);
                Button_AddNewNail.Checked = false;

            }
            else
            {
                SelectedRoute = e.CanvasItem;
                this.Button_Delete.Enabled = true;
            }
        }

        private void Button_AddLink_Click(object sender, EventArgs e)
        {
            if (Button_AddLink.Checked)
            {
                if (this.SelectedItems.Count > 0)
                {
                    this.SelectedItems[0].Focused = false;
                    this.SelectedItems.Clear();
                }
                Button_AddNewState.Checked = false;
                HighLightControlUsingRed = true;
            }
            else
            {
                HighLightControlUsingRed = false;
            }
        }

        private void Button_Delete_Click(object sender, EventArgs e)
        {
            if (this.SelectedItems.Count > 0)
            {
                RemoveMultiItems();
                Canvas.Refresh();
                SetDirty();
            }
            else if (SelectedRoute != null)
            {
                if (SelectedRoute.From is StateItem && SelectedRoute.To is StateItem)
                {
                    //StateItem sourcePage = SelectedRoute.From as StateItem;
                    //StateItem targetPage = SelectedRoute.To as StateItem;

                    //EditingLink(sourcePage, targetPage, SelectedRoute, true);
                    RemoveCanvasRoute(SelectedRoute);
                    Canvas.Refresh();
                    SetDirty();
                }
            }
            this.Button_Delete.Enabled = false;
        }

        private void Button_Edit_Click(object sender, EventArgs e)
        {
            if (this.SelectedItems.Count > 0)
            {
                if (this.SelectedItems[0].Item is StateItem)
                {
                    this.Canvas_ItemDoubleClick(null, new CanvasItemEventArgs(this.SelectedItems[0]));
                }
                else if (this.SelectedItems[0].Item is Transition)
                {
                    Route route = (this.SelectedItems[0].Item as Transition).FindSelectedRouteBasedOnTransition(this.Canvas);
                    this.Canvas_RouteDoubleClick(null, new CanvasRouteEventArgs(route));
                }
            }
        }
        private TreeNode LastSelectedNode;
        //private void SaveSourceCode()
        //{
        //    string appFilePath = LastSelectedNode.Tag as string;

        //    if (File.Exists(appFilePath))
        //    {
        //        try
        //        {
        //            File.WriteAllText(@appFilePath, textEditorControl.Text);
        //        }
        //        catch (DirectoryNotFoundException)
        //        {

        //        }
        //        catch (FileNotFoundException)
        //        {

        //        }
        //    }
        //}
        private void ActivateTextEditorByLastSelectedNode()
        {
            textEditorControl.Visible = false;
            textEditorControl.TextChanged -= textEditorControl_TextChanged;
            textEditorControl.Text = FileTag.GetTagText(LastSelectedNode);
            textEditorControl.TextChanged += textEditorControl_TextChanged;

            if (Canvas != null)
                Canvas.Visible = false;
            toolStripContainer1.Visible = false;
            textEditorControl.Visible = true;
        }
        private void TreeView_Structure_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {

            //if (LastSelectedNode != null && LastSelectedNode.Parent != null
            //   && LastSelectedNode.Parent.Text == Parsing.FILES_NODE_NAME
            //  )
            //{
            //    if (!(e.Node.Parent != null
            //        && e.Node.Parent.Text == Parsing.FILES_NODE_NAME                 
            //        && e.Node.Text == LastSelectedNode.Text
            //        && e.Node.Name == LastSelectedNode.Name))
            //    {
            //        if (TextEditorChanged)
            //        {
            //            TextEditorChanged = false;
            //            if (MessageBox.Show("Do you want to save the changes of " + LastSelectedNode.Name + "?",
            //                "Yes", MessageBoxButtons.YesNo) == DialogResult.Yes)
            //            {
            //                SaveSourceCode();
            //            }

            //        }

            //    }
            //}
            //else if (LastSelectedNode != null
            //   && LastSelectedNode.Text == Parsing.ASSERTION_NODE_NAME
            //   && e.Node.Text != Parsing.ASSERTION_NODE_NAME)
            //{
            //    if (TextEditorChanged)
            //    {
            //        TextEditorChanged = false;
            //        if (MessageBox.Show("Do you want to save the changes of Assertions?",
            //            "Yes", MessageBoxButtons.YesNo) == DialogResult.Yes)
            //        {
            //            LastSelectedNode.Tag = textEditorControl.Text;
            //        }

            //    }
            //}

            LastSelectedNode = e.Node;

            //if (e.Node.Parent != null && e.Node.Parent.Text == Parsing.FILES_NODE_NAME)
            //{
            //    toolStripContainer1.Visible = false;

            //    string appFilePath = e.Node.Tag as string;
            //    string appText = "";

            //    if (!Path.IsPathRooted(appFilePath))
            //    {
            //        appFilePath = Path.Combine(appFilePath, appFilePath);
            //    }

            //    StreamReader sReader = null;

            //    try
            //    {
            //        FileStream fileStream = new FileStream(appFilePath, FileMode.Open, FileAccess.Read);
            //        sReader = new StreamReader(fileStream);
            //        appText = sReader.ReadToEnd();
            //    }
            //    catch (DirectoryNotFoundException DNF)
            //    {
            //        appText = e.Node.Tag.ToString();
            //    }
            //    catch (FileNotFoundException FNF)
            //    {
            //        //throw new ComponentDefinitionFileNotFoundException(componentDefName, FileName, token);
            //        appText = appFilePath;
            //    }
            //    finally
            //    {
            //        if (sReader != null)
            //        {
            //            sReader.Close();
            //        }
            //    }

                //textEditorControl.Visible = false;
                //textEditorControl.TextChanged -= textEditorControl_TextChanged;
                //textEditorControl.Text = FileTag.GetTagText(LastSelectedNode);
                //textEditorControl.TextChanged += textEditorControl_TextChanged;

                //if (Canvas != null)
                //    Canvas.Visible = false;
                //toolStripContainer1.Visible = false;
                //textEditorControl.Visible = true;
            ActivateTextEditorByLastSelectedNode();

                //}else if (e.Node.Parent != null && e.Node.Text == Parsing.NETWORK_NODE_NAME)
            //}
            //else if (e.Node.Parent == null
            //   || (e.Node.Parent != null && e.Node.Parent.Parent == null && e.Node.Text == Parsing.SENSORS_NODE_NAME))
            //else if (e.Node.Text == Parsing.ASSERTION_NODE_NAME)
            //{
            //    toolStripContainer1.Visible = false;
            //    textEditorControl.Visible = false;
            //    textEditorControl.TextChanged -= textEditorControl_TextChanged;
            //    if (AssertNode.Tag != null)
            //    {
            //        textEditorControl.Text = AssertNode.Tag.ToString();
            //    }
            //    else
            //    {
            //        textEditorControl.Text = "//Assertions";
            //    }
            //    textEditorControl.TextChanged += textEditorControl_TextChanged;

            //    if (Canvas != null)
            //        Canvas.Visible = false;

            //    textEditorControl.Visible = true;
            //}
            //else
            //{
            //    textEditorControl.Visible = false;
            //}

            RaiseIsDirtyChanged();
        }

        private void MenuButton_Initial_Click(object sender, EventArgs e)
        {
            if (this.SelectedItems.Count > 0)
            {
                CanvasItemData SelectedItem = this.SelectedItems[0];
                if (SelectedItem.Item is StateItem)
                {
                    foreach (CanvasItem state in Canvas.itemsData.Keys)
                    {
                        if (state is StateItem && state != null)
                        {
                            (state as StateItem).IsInitialState = false;
                        }
                    }
                    (SelectedItem.Item as StateItem).IsInitialState = true;
                    Canvas.Refresh();
                    SetDirty();
                }
            }
        }


        //private void contextMenuStrip2_Opening(object sender, CancelEventArgs e)
        //{
        //    if (TreeView_Structure.SelectedNode != null)
        //    {
        //        addProcessToolStripMenuItem.Enabled = false;
        //        deleteProcessToolStripMenuItem.Enabled = false;
        //        processDetailsToolStripMenuItem.Enabled = false;

        //        if (TreeView_Structure.SelectedNode.Text == Parsing.PROCESSES_NODE_NAME)
        //        {
        //            addProcessToolStripMenuItem.Enabled = true;

        //        }
        //        else if (TreeView_Structure.SelectedNode.Parent != null && TreeView_Structure.SelectedNode.Parent.Text == Parsing.PROCESSES_NODE_NAME)
        //        {
        //            deleteProcessToolStripMenuItem.Enabled = true;
        //            processDetailsToolStripMenuItem.Enabled = true;
        //        }
        //    }
        //}



        private int pcounter = 1;

        //private void addProcessToolStripMenuItem_Click(object sender, EventArgs e)
        //{
        //    if (Canvas != null)
        //    {
        //        Canvas.Visible = false;
        //    }

        //    TreeNode node = TreeView_Structure.Nodes[0].Nodes[Parsing.PROCESSES_NODE_NAME].Nodes.Add("P" + pcounter);
        //    pcounter++;

        //    Canvas = new LTSCanvas();
        //    Canvas.Node = node;
        //    node.Tag = Canvas;
        //    TreeView_Structure.SelectedNode = node;

        //    AddNewProcess(Canvas);

        //    //add the first 
        //    StateItem classitem = new StateItem(true, "State " + Canvas.StateCounter);
        //    Canvas.StateCounter++;
        //    classitem.X = 100 / Canvas.Zoom;
        //    classitem.Y = 100 / Canvas.Zoom;
        //    AddCanvasItem(classitem);

        //    Canvas.Refresh();
        //    SetDirty();
        //}

        //private void Canvas_LayoutChanged(object sender, EventArgs e)
        //{
        //    SetDirty();
        //}

        protected new void SetDirty()
        {
            base.SetDirty();
            if (Canvas != null)
            {
                this.Canvas.RefreshPictureBox();
            }
        }

        //private void Canvas_SaveCurrentCanvas(object sender, EventArgs e)
        //{
        //    this.StoreCurrentCanvas();
        //}

        //private void processDetailsToolStripMenuItem_Click(object sender, EventArgs e)
        //{
        //    if (TreeView_Structure.SelectedNode != null)
        //    {
        //        if (TreeView_Structure.SelectedNode.Parent != null && TreeView_Structure.SelectedNode.Parent.Text == Parsing.PROCESSES_NODE_NAME)
        //        {
        //            LTSCanvas canvas = TreeView_Structure.SelectedNode.Tag as LTSCanvas;
        //            string lastText = canvas.Node.Text;
        //            string lastParameters = canvas.Parameters;
        //            ProcessEditingForm form = new ProcessEditingForm(canvas);
        //            if (form.ShowDialog() == DialogResult.OK)
        //            {
        //                form.UpdateData();
        //                SetDirty();
        //                if (!CheckProcessNameDuplicate())
        //                {
        //                    canvas.Node.Text = lastText;
        //                    canvas.Parameters = lastParameters;
        //                }
        //            }
        //        }
        //    }
        //}

        private bool CheckProcessNameDuplicate()
        {
            List<string> processNames = new List<string>();
            foreach (TreeNode node in TreeView_Structure.Nodes[0].Nodes[1].Nodes)
            {
                if (processNames.Contains(node.Text))
                {
                    MessageBox.Show(Resources.No_duplicated_process_names_are_allowed, Resources.Duplicate_Process_Name, MessageBoxButtons.OK,
                                                                                            MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                    return false;
                }
                else
                {
                    processNames.Add(node.Text);
                }
            }
            return true;
        }

        //private void deleteProcessToolStripMenuItem_Click(object sender, EventArgs e)
        //{
        //    if (TreeView_Structure.SelectedNode != null)
        //    {
        //        if (TreeView_Structure.SelectedNode.Parent != null && TreeView_Structure.SelectedNode.Parent.Text == Parsing.PROCESSES_NODE_NAME)
        //        {
        //            //if (TreeView_Structure.SelectedNode.Parent.Nodes.Count == 1)
        //            //{
        //            //    MessageBox.Show(Resources.At_least_one_process_is_needed_in_the_model_, Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
        //            //    return;
        //            //}

        //            if (MessageBox.Show(Resources.Are_you_sure_you_want_to_delete_the_selected_process_, Resources.Confirmation, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
        //            {
        //                LTSCanvas cavas = TreeView_Structure.SelectedNode.Tag as LTSCanvas;
        //                if (cavas != null)
        //                {
        //                    toolStripContainer1.ContentPanel.Controls.Remove(cavas);
        //                }
        //                toolStripContainer1.Visible = false;
        //                textEditorControl.Visible = false;

        //                TreeView_Structure.SelectedNode.Remove();
        //                TreeView_Structure.SelectedNode = null;

        //                SetDirty();
        //            }
        //        }
        //    }
        //}



        public override bool CanUndo
        {
            get
            {
                if (textEditorControl.Visible)
                {
                    return this.CodeEditor.EnableUndo;
                }
                else if (Canvas != null && Canvas.Visible)
                {
                    return (this.Canvas.currentStateIndex > 0);
                }
                return false;
            }
        }

        public override bool CanRedo
        {
            get
            {
                if (textEditorControl.Visible)
                {
                    return this.CodeEditor.EnableRedo;
                }
                else if (Canvas != null && Canvas.Visible)
                {
                    return (this.Canvas.currentStateIndex < this.Canvas.undoStack.Count - 1);
                }
                return false;

            }
        }

        public override bool CanCut
        {
            get
            {
                if (textEditorControl.Visible)
                {
                    return this.EnableCut;
                }
                return false;

            }
        }

        public override bool CanCopy
        {
            get
            {
                if (textEditorControl.Visible)
                {
                    return this.EnableCopy;
                }
                return false;

            }
        }
        public override bool CanPaste
        {
            get
            {
                if (textEditorControl.Visible)
                {
                    return this.EnablePaste;// textEditorControl.ActiveTextAreaControl.TextArea.ClipboardHandler.EnablePaste;
                }
                return false;

            }
        }

        public override bool CanSelectAll
        {
            get
            {
                if (textEditorControl.Visible)
                {
                    return this.CodeEditor.CanSelect;
                }
                return false;

            }
        }

        public override bool CanFind
        {
            get
            {
                if (textEditorControl.Visible)
                {
                    return true;
                }
                return false;

            }
        }

        public override bool CanPrint
        {
            get
            {
                if (textEditorControl.Visible)
                {
                    return true;
                }
                return false;
            }
        }

        public override void Undo()
        {
            if (textEditorControl.Visible)
            {
                this.CodeEditor.Undo();
            }
            else if (Canvas != null && Canvas.Visible)
            {
                Button_Undo_Click(null, null);
            }

        }

        public override void Redo()
        {
            if (textEditorControl.Visible)
            {
                this.CodeEditor.Redo();
            }
            else if (Canvas != null && Canvas.Visible)
            {
                Button_Redo_Click(null, null);
            }
        }

        public override void Cut()
        {

            //this.CodeEditor.Cut();
            textEditorControl.ActiveTextAreaControl.TextArea.ClipboardHandler.Cut(null, null);

        }

        public override void Copy()
        {

            //this.CodeEditor.Copy();
            textEditorControl.ActiveTextAreaControl.TextArea.ClipboardHandler.Copy(null, null);


        }

        public override void Paste()
        {

            textEditorControl.ActiveTextAreaControl.TextArea.ClipboardHandler.Paste(null, null);

        }

        public override void SelectAll()
        {

            textEditorControl.ActiveTextAreaControl.TextArea.ClipboardHandler.SelectAll(null, null);

        }

        public override void Delete()
        {
            if (textEditorControl.Visible)
            {
                textEditorControl.ActiveTextAreaControl.TextArea.ClipboardHandler.Delete(null, null);
            }
            else if (Canvas != null && Canvas.Visible)
            {
                this.Button_Delete_Click(null, null);
            }
        }

        public override bool CanDelete
        {
            get
            {
                if (textEditorControl.Visible)
                {
                    return true;
                }
                else if (Canvas != null && Canvas.Visible)
                {
                    return this.SelectedItems.Count > 0;
                }

                return true;
            }
        }


        public override void ZoomOut()
        {
            if (textEditorControl.Visible)
            {
                base.ZoomOut();
            }
            else if (Canvas != null && Canvas.Visible)
            {
                Canvas.Zoom *= 0.9f;
            }
        }


        public override void ZoomIn()
        {

            if (textEditorControl.Visible)
            {
                base.ZoomIn();
            }
            else if (Canvas != null && Canvas.Visible)
            {
                Canvas.Zoom *= 1.1f;
            }
        }

        public override void Zoom100()
        {
            if (textEditorControl.Visible)
            {
                base.Zoom100();
            }
            else if (Canvas != null && Canvas.Visible)
            {
                Canvas.Zoom = 1.0f;
            }
        }


        public override void SplitWindow()
        {
            if (textEditorControl.Visible)
            {
                textEditorControl.Split();
            }
        }

      private void addFiles(List<string> files )
      {
         
          foreach (string filepath in files)
          {
              string filename = Path.GetFileName(filepath);
              TreeNode node = FilesNode.Nodes.Add(filename);
              node.Tag = new FileTag(filepath,FileTag.FileType.files);
              node.Name = filename;
          }
          FilesNode.ExpandAll();
      }

        public override void SetText(string text)
        {
            BPELModel bpel = BPELModel.LoadLTSFromXML(text);

            AssertNode.Tag = new FileTag(bpel.Assertion, FileTag.FileType.assertion);
            //textEditorControl.Text = bpel.Assertion;

            FilesNode.Nodes.Clear();
            toolStripContainer1.ContentPanel.Controls.Clear();

            addFiles(bpel.getFileName());
            TreeView_Structure.SelectedNode = AssertNode;
            LastSelectedNode = AssertNode;
            ActivateTextEditorByLastSelectedNode();
           
          

            toolStripContainer1.Visible = false;
            //textEditorControl.Visible = false;

            //if (FilesNode.Nodes.Count > 0)
            //{
            //    TreeView_Structure.SelectedNode = FilesNode.Nodes[0];
            //    TreeView_Structure_NodeMouseDoubleClick(null, new TreeNodeMouseClickEventArgs(FilesNode.Nodes[0], MouseButtons.Left, 2, 0, 0));
            //}
        }

        public override string GetText()
        {
            XmlDocument doc = GetDoc(true);
            return doc.InnerXml;
        }
        public override string FileName
        {
            get
            {
                return fileName;
            }
        }

        //todo 
        private string fileName;
        private bool TextEditorChanged;

        public override void Save(string filename)
        {
            //Newly added

            foreach(TreeNode tr in this.FilesNode.Nodes)
            {
                string appFilePath = FileTag.GetTagPath(tr);
                string appText = FileTag.GetTagText(tr);
                try
                {
                    File.WriteAllText(@appFilePath, appText);
                }
                catch (Exception e)
                {
                    int i = 1;
                }
            }

              //var CurrentNode = TreeView_Structure.SelectedNode;
              //if (CurrentNode.Parent != null &&
              //    CurrentNode.Parent.Text == Parsing.FILES_NODE_NAME)
              //{
              //    if (TextEditorChanged)
              //    {
              //        TextEditorChanged = false;
              //        string appFilePath = FileTag.GetTagPath(CurrentNode);
              //        string appText = FileTag.GetTagText(CurrentNode);

              //        if (File.Exists(appFilePath))
              //        {
              //            try
              //            {
              //                //File.WriteAllText(@appFilePath, textEditorControl.Text)
              //                File.WriteAllText(@appFilePath, appText);
              //            }
              //            catch (DirectoryNotFoundException)
              //            {

              //            }
              //            catch (FileNotFoundException)
              //            {

              //            }
              //        }
              //    }
              //}
              //else if (CurrentNode.Text == Parsing.ASSERTION_NODE_NAME)
              //{
                  //if (TextEditorChanged)
                  //{
                  //    TextEditorChanged = false;
                  //    CurrentNode.Tag = textEditorControl.Text;
                  //}
              //}

            //Newly added

            if (!string.IsNullOrEmpty(filename) && filename != fileName)
            {
                fileName = filename;
            }
            XmlDocument doc = GetDoc(false);
            doc.Save(fileName);


            HaveFileName = true;
        }

        private XmlDocument GetDoc(bool dirtyVersion)
        {
            string declare = "";
            if (this.AssertNode.Tag != null)
            {
                declare = FileTag.GetTagText(this.AssertNode);
            }
            BPELModel lts=null;

            if (dirtyVersion)
            {
                Dictionary<string,string> dic=new Dictionary<string, string>();
                foreach (TreeNode node in FilesNode.Nodes)
                {
                    dic.Add(FileTag.GetTagPath(node),FileTag.GetTagText(node));
                }
                lts = new BPELModel(declare, dic);
            }
            else
            {
                List<string> files = new List<string>();
                foreach (TreeNode node in FilesNode.Nodes)
                {

                    files.Add(FileTag.GetTagPath(node));

                }
                lts = new BPELModel(declare, files);

            }
            return lts.GenerateXML();
          
        }

        //todo
        public override void Open(string filename)
        {
            //file = FileService.GetOrCreateOpenedFile(filename);
            //files.Clear();
            //files.Add(file);
            ////TextEditorDisplayBindingWrapper b2 = CreateWrapper(file);
            ////file.ForceInitializeView(this); // load file to initialize folding etc.

            //file.CurrentView = this;
            //this.Load(file, file.OpenRead());

            ////this.SetSyntaxLanguage(file.FileName);

            //textEditorControl.InitializeFormatter();
            //textEditorControl.ActivateQuickClassBrowserOnDemand();
            //file.CloseIfAllViewsClosed();

            this.ToolTipText = filename;
            TabText = Path.GetFileName(filename);
            HaveFileName = true;
            fileName = filename;

            StreamReader streamReader = new StreamReader(filename);
            string text = streamReader.ReadToEnd();
            streamReader.Close();
            SetText(text);

            pcounter = 1;

            //TreeView_Structure.SelectedNode = AssertNode;
            //TreeView_Structure_NodeMouseDoubleClick(null, new TreeNodeMouseClickEventArgs(AssertNode, MouseButtons.Left, 2, 0, 0));

        }

        private void Button_AutoRange_Click_1(object sender, EventArgs e)
        {
            SaveFileDialog svd = new SaveFileDialog();
            svd.Filter = "Bitmap (*.bmp)|*.bmp|All File (*.*)|*.*";

            if (svd.ShowDialog() == DialogResult.OK)
            {
                Canvas.SaveToImage(svd.FileName);
            }
        }

        public override void HandleParsingException(ParsingException ex)
        {
            try
            {
                if (ex is GraphParsingException)
                {
                    string pname = (ex as GraphParsingException).ProcessName;
                    foreach (TreeNode node in FilesNode.Nodes)
                    {
                        if (node.Text == pname)
                        {
                            TreeView_Structure.SelectedNode = node;
                            TreeView_Structure_NodeMouseDoubleClick(null,
                                                                    new TreeNodeMouseClickEventArgs(node, MouseButtons.Left,
                                                                                                    2, 0, 0));
                            break;
                        }
                    }
                }
                else
                {

                    TreeView_Structure.SelectedNode = AssertNode;
                    TreeView_Structure_NodeMouseDoubleClick(null,
                                                            new TreeNodeMouseClickEventArgs(AssertNode, MouseButtons.Left,
                                                                                            2, 0, 0));


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
            catch
            {

            }
        }

        //public override void SetSyntaxLanguage(string languageName)
        //{
        //    //try
        //    //{

        //    //    IHighlightingStrategy strategy = HighlightingStrategyFactory.CreateHighlightingStrategy(languageName);
        //    //    textEditorControl.Document.HighlightingStrategy = strategy;
        //    //    textEditorControl.InitializeAdvancedHighlighter();
        //    //    ModuleName = strategy.Name;
        //    //    this.Icon = Ultility.GetModuleIcon(languageName);
        //    //    FileExtension = strategy.Name + " (*" + string.Join(";", strategy.Extensions) + ")|*" +
        //    //                    string.Join(";", strategy.Extensions) + "|All File (*.*)|*.*";


        //    //}
        //    //catch (HighlightingDefinitionInvalidException ex)
        //    //{
        //    //    //textEditorControl.Document.HighlightingStrategy = HighlightingStrategyFactory.CreateHighlightingStrategy(DefaultLanguageExtension);
        //    //    MessageBox.Show("Error: file format is not supported!", Common.Ultility.Ultility.APPLICATION_NAME, MessageBoxButtons.OK, MessageBoxIcon.Error);
        //    //}
        //}

        private void StoreCurrentCanvas()
        {
            this.Canvas.undoStack.RemoveRange(this.Canvas.currentStateIndex + 1, (this.Canvas.undoStack.Count - 1) - (this.Canvas.currentStateIndex + 1) + 1);
            this.Canvas.undoStack.Add(this.Canvas.Clone());
            this.Canvas.currentStateIndex = this.Canvas.undoStack.Count - 1;
        }

        private void StoreCanvas(LTSCanvas canvas)
        {
            canvas.undoStack.RemoveRange(canvas.currentStateIndex + 1, (canvas.undoStack.Count - 1) - (canvas.currentStateIndex + 1) + 1);
            canvas.undoStack.Add(canvas.Clone());
            canvas.currentStateIndex = canvas.undoStack.Count - 1;
        }

        private void AddCanvasItem(CanvasItem item)
        {
            this.Canvas.AddCanvasItem(item);
            StoreCurrentCanvas();
        }

        private void AddLink(Route route)
        {
            this.Canvas.AddLink(route);
            StoreCurrentCanvas();
        }

        private void RemoveCanvasItem(CanvasItem item)
        {
            this.Canvas.RemoveCanvasItem(item);
            StoreCurrentCanvas();
        }

        public void RemoveCanvasRoute(Route route)
        {
            this.Canvas.RemoveCanvasRoute(route);
            StoreCurrentCanvas();
        }

        private void RemoveMultiItems()
        {
            foreach (CanvasItemData SelectedItem in this.SelectedItems)
            {
                if (SelectedItem.Item is NailItem)
                {
                    this.Canvas.RemoveCanvasItem(SelectedItem.Item);
                }
            }

            foreach (CanvasItemData SelectedItem in this.SelectedItems)
            {
                if (SelectedItem.Item is Transition)
                {
                    this.Canvas.RemoveCanvasItem(SelectedItem.Item);
                }
            }

            foreach (CanvasItemData SelectedItem in this.SelectedItems)
            {
                if (SelectedItem.Item is LabelItem)
                {
                    this.Canvas.RemoveCanvasItem(SelectedItem.Item);
                }
            }

            foreach (CanvasItemData SelectedItem in this.SelectedItems)
            {
                if (SelectedItem.Item is StateItem)
                {
                    this.Canvas.RemoveCanvasItem(SelectedItem.Item);
                }
            }
            this.Canvas.FinishMultiObjectAction();
            this.StoreCurrentCanvas();
        }

        //private void duplicateProcessToolStripMenuItem_Click(object sender, EventArgs e)
        //{
        //    if (TreeView_Structure.SelectedNode != null)
        //    {
        //        if (TreeView_Structure.SelectedNode.Parent != null && TreeView_Structure.SelectedNode.Parent.Text == Parsing.PROCESSES_NODE_NAME)
        //        {
        //            LTSCanvas currentCanvas = TreeView_Structure.SelectedNode.Tag as LTSCanvas;
        //            LTSCanvas duplicatedCanvas = currentCanvas.Duplicate();

        //            TreeNode node = TreeView_Structure.Nodes[0].Nodes[Parsing.PROCESSES_NODE_NAME].Nodes.Add(currentCanvas.Node.Text + "-copy");
        //            duplicatedCanvas.Node = node;
        //            node.Tag = duplicatedCanvas;
        //            TreeView_Structure.SelectedNode = node;

        //            AddNewProcess(duplicatedCanvas);

        //            duplicatedCanvas.Refresh();
        //            SetDirty();
        //        }
        //    }
        //}

        /// <summary>
        /// Do seetings for added process
        /// </summary>
        /// <param name="addedCanvas"></param>
        //private void AddNewProcess(LTSCanvas addedCanvas)
        //{
        //    addedCanvas.Dock = DockStyle.Fill;
        //    addedCanvas.ContextMenuStrip = contextMenuStrip1;
        //    addedCanvas.CanvasItemSelected += new EventHandler<CanvasItemEventArgs>(Canvas_CanvasItemSelected);
        //    addedCanvas.CanvasItemsSelected += new EventHandler<CanvasItemsEventArgs>(Canvas_CanvasItemsSelected);
        //    addedCanvas.CanvasRouteSelected += new EventHandler<CanvasRouteEventArgs>(Canvas_CanvasRouteSelected);
        //    addedCanvas.ItemDoubleClick += new EventHandler<CanvasItemEventArgs>(Canvas_ItemDoubleClick);
        //    addedCanvas.RouteDoubleClick += new EventHandler<CanvasRouteEventArgs>(Canvas_RouteDoubleClick);
        //    addedCanvas.LayoutChanged += new EventHandler(Canvas_LayoutChanged);
        //    addedCanvas.SaveCurrentCanvas += new EventHandler(Canvas_SaveCurrentCanvas);

        //    this.Button_AddLink.Checked = false;
        //    this.Button_AddNewState.Checked = false;
        //    this.Button_Delete.Enabled = false;

        //    toolStripContainer1.ContentPanel.Controls.Add(addedCanvas);
        //    textEditorControl.Visible = false;
        //    toolStripContainer1.Visible = true;
        //}

        


        private void addExistingFileToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            //To add for add existing file
        //    this.openFileDialog1.Filter =
        //"BPEL Related Files (*.BPEL;*.WSDL;*.XSD)|*.BPEL;*.WSDL;*.XSD|" +
        //"All files (*.*)|*.*";
            this.openFileDialog1.Filter =
                "BPEL Related Files (*.BPEL)|*.BPEL";
            this.openFileDialog1.Multiselect = true;
            this.openFileDialog1.Title = "Select Files";

            this.openFileDialog1.FileName = "";
            DialogResult dr = this.openFileDialog1.ShowDialog();
            
            if (dr == System.Windows.Forms.DialogResult.OK)
            {
                // Read the files
                this.addFiles(new List<string>(openFileDialog1.FileNames));
                SetDirty();
            }


        }

        private void deleteFileToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            //To add for deleting file
            TreeNode selectedNode = TreeView_Structure.SelectedNode;
            if (selectedNode != null)
            {
                
                if (selectedNode.Parent != null && selectedNode.Parent.Text ==Parsing.FILES_NODE_NAME)
                {
                  
                   
                        //File.Delete(selectedNode.Tag as string);
                        if(FilesNode.GetNodeCount(false)>0)
                        {
                            TreeView_Structure.SelectedNode=FilesNode.Nodes[0];
                        }else
                        {
                            TreeView_Structure.SelectedNode = AssertNode;
                        }
                   
                    selectedNode.Remove();
                    SetDirty();

                }
        }
        }

        private void contextMenuStrip2_Opening(object sender, CancelEventArgs e)
        {
            if (TreeView_Structure.SelectedNode != null)
            {
                addExistingFileToolStripMenuItem1.Enabled = true;
                deleteFileToolStripMenuItem1.Enabled = false;


                if (TreeView_Structure.SelectedNode.Parent != null && TreeView_Structure.SelectedNode.Parent.Text == Parsing.FILES_NODE_NAME)
                {
                    addExistingFileToolStripMenuItem1.Enabled = false;
                    deleteFileToolStripMenuItem1.Enabled = true;

                }
                
            }
        }
    }
}