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
using PAT.Common.GUI.Drawing;
using PAT.Common.GUI.PNModule;
using PAT.GUI.EditingFunction.CodeCompletion;
using PAT.GUI.Properties;
using Tools.Diagrams;
using PAT.GUI.Docking;
using CanvasItemData = PAT.Common.GUI.Drawing.LTSCanvas.CanvasItemData;

namespace PAT.GUI.PNDrawing
{
    public class PNTabItem : EditorTabItem
    {
        private ContextMenuStrip contextMenuStrip1;
        private IContainer components;

        private ToolStripMenuItem MenuButton_NewState;
        private ToolStripMenuItem MenuButton_AddLink;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripMenuItem MenuButton_SetInitial;
        private ToolStripMenuItem MenuButton_SetError;
        private ToolStripMenuItem MenuButton_SetAcceptance;
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
        private TreeView TreeView_Structure;
        private ToolStripButton Button_AutoRange;
        private PNCanvas Canvas;

        private System.Windows.Forms.TreeNode DeclarationNode;
        private System.Windows.Forms.TreeNode ModelNode;
        private XmlElement Topology;

        private ToolStripButton Button_AddNewNail;
        private ToolStripButton Button_AddNewTransition;
        private ContextMenuStrip contextMenuStrip2;
        private ToolStripMenuItem addProcessToolStripMenuItem;
        private ToolStripMenuItem deleteProcessToolStripMenuItem;
        private ToolStripMenuItem processDetailToolStripMenuItem;
        private ToolStripMenuItem duplicateProcessToolStripMenuItem;
        private ToolStripMenuItem MenuButton_NewNail;

        public PNTabItem(string moduleName)
        {
            InitializeComponent();

            DeclarationNode = new TreeNode("Declaration");
            ModelNode = new TreeNode("Models");
            TreeNode treeNode8 = new TreeNode("Petri Nets Model",
                new TreeNode[] {
                    DeclarationNode,
                    ModelNode
                });

            MenuButton_SetError.Visible = false;

            treeNode8.Text = "Petri Nets";
            this.TreeView_Structure.Nodes.AddRange(new TreeNode[] { treeNode8 });

            DeclarationNode.StateImageIndex = 0;
            ModelNode.Name = "Models";
            ModelNode.StateImageIndex = 1;

            AddEventHandlerForButtons();

            textEditorControl = new SharpDevelopTextAreaControl();
            textEditorControl.Dock = DockStyle.Fill;
            textEditorControl.ContextMenuStrip = EditorContextMenuStrip;
            textEditorControl.BorderStyle = BorderStyle.Fixed3D;
            textEditorControl.Visible = true;

            this.splitContainer1.Panel2.Controls.Add(textEditorControl);

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

            this.TreeView_Structure.HideSelection = false;
            splitContainer1.SplitterDistance = 100;

            addProcessToolStripMenuItem.PerformClick();

            this.TreeView_Structure.ExpandAll();

            Button_AddNewNail.Visible = false;

            //show the declaration
            TreeView_Structure.SelectedNode = DeclarationNode;
            TreeView_Structure_NodeMouseDoubleClick(null, new TreeNodeMouseClickEventArgs(DeclarationNode, MouseButtons.Left, 2, 0, 0));
        }

        private void AddEventHandlerForButtons()
        {
            foreach (ToolStripItem button in this.toolStrip1.Items)
            {
                if (button is ToolStripButton)
                    ((ToolStripButton)button).CheckStateChanged += new EventHandler(button_CheckStateChanged);
            }
        }

        protected void textEditorControl_TextChanged(object sender, EventArgs e)
        {
            textEditorControl.Document.FoldingManager.UpdateFoldings(null, null);
            if (this.TreeView_Structure.SelectedNode != null)
            {
                this.TreeView_Structure.SelectedNode.Tag = textEditorControl.Text;
            }
            SetDirty();
        }

        #region InitializeComponent
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PNTabItem));
            this.toolStripContainer1 = new System.Windows.Forms.ToolStripContainer();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.Button_AddNewState = new System.Windows.Forms.ToolStripButton();
            this.Button_AddLink = new System.Windows.Forms.ToolStripButton();
            this.Button_AddNewTransition = new System.Windows.Forms.ToolStripButton();
            this.Button_Delete = new System.Windows.Forms.ToolStripButton();
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
            this.MenuButton_Delete = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.MenuButton_SetInitial = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuButton_SetError = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuButton_SetAcceptance = new System.Windows.Forms.ToolStripMenuItem();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.TreeView_Structure = new System.Windows.Forms.TreeView();
            this.contextMenuStrip2 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.addProcessToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteProcessToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.processDetailToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.duplicateProcessToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.imageList2 = new System.Windows.Forms.ImageList(this.components);
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
            this.Button_AddLink,
            this.Button_AddNewTransition,
            this.Button_Delete,
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
            // Button_AddLink
            // 
            this.Button_AddLink.CheckOnClick = true;
            this.Button_AddLink.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.Button_AddLink.Image = global::PAT.GUI.Properties.Resources.link;
            resources.ApplyResources(this.Button_AddLink, "Button_AddLink");
            this.Button_AddLink.Name = "Button_AddLink";
            this.Button_AddLink.Click += new System.EventHandler(this.Button_AddLink_Click);
            // 
            // Button_AddNewTransition
            // 
            this.Button_AddNewTransition.CheckOnClick = true;
            this.Button_AddNewTransition.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.Button_AddNewTransition, "Button_AddNewTransition");
            this.Button_AddNewTransition.Name = "Button_AddNewTransition";
            // 
            // Button_Delete
            // 
            this.Button_Delete.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.Button_Delete, "Button_Delete");
            this.Button_Delete.Image = global::PAT.GUI.Properties.Resources.delete;
            this.Button_Delete.Name = "Button_Delete";
            this.Button_Delete.Click += new System.EventHandler(this.Button_Delete_Click);
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
            this.MenuButton_Delete,
            this.toolStripSeparator1,
            this.MenuButton_SetInitial,
            this.MenuButton_SetError,
            this.MenuButton_SetAcceptance});
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
            // MenuButton_SetError
            // 
            this.MenuButton_SetError.Name = "MenuButton_SetError";
            resources.ApplyResources(this.MenuButton_SetError, "MenuButton_SetError");
            this.MenuButton_SetError.Click += new System.EventHandler(this.MenuButton_Error_Click);
            // 
            // MenuButton_SetAcceptance
            // 
            this.MenuButton_SetAcceptance.Name = "MenuButton_SetAcceptance";
            resources.ApplyResources(this.MenuButton_SetAcceptance, "MenuButton_SetAcceptance");
            this.MenuButton_SetAcceptance.Click += new System.EventHandler(this.MenuButton_Committed_Click);
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
            this.addProcessToolStripMenuItem,
            this.deleteProcessToolStripMenuItem,
            this.processDetailToolStripMenuItem,
            this.duplicateProcessToolStripMenuItem});
            this.contextMenuStrip2.Name = "contextMenuStrip2";
            resources.ApplyResources(this.contextMenuStrip2, "contextMenuStrip2");
            this.contextMenuStrip2.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip2_Opening);
            // 
            // addProcessToolStripMenuItem
            // 
            this.addProcessToolStripMenuItem.Name = "addProcessToolStripMenuItem";
            resources.ApplyResources(this.addProcessToolStripMenuItem, "addProcessToolStripMenuItem");
            this.addProcessToolStripMenuItem.Click += new System.EventHandler(this.addProcessToolStripMenuItem_Click);
            // 
            // deleteProcessToolStripMenuItem
            // 
            this.deleteProcessToolStripMenuItem.Name = "deleteProcessToolStripMenuItem";
            resources.ApplyResources(this.deleteProcessToolStripMenuItem, "deleteProcessToolStripMenuItem");
            this.deleteProcessToolStripMenuItem.Click += new System.EventHandler(this.deleteProcessToolStripMenuItem_Click);
            // 
            // processDetailToolStripMenuItem
            // 
            this.processDetailToolStripMenuItem.Name = "processDetailToolStripMenuItem";
            resources.ApplyResources(this.processDetailToolStripMenuItem, "processDetailToolStripMenuItem");
            this.processDetailToolStripMenuItem.Click += new System.EventHandler(this.processDetailsToolStripMenuItem_Click);
            // 
            // duplicateProcessToolStripMenuItem
            // 
            this.duplicateProcessToolStripMenuItem.Name = "duplicateProcessToolStripMenuItem";
            resources.ApplyResources(this.duplicateProcessToolStripMenuItem, "duplicateProcessToolStripMenuItem");
            this.duplicateProcessToolStripMenuItem.Click += new System.EventHandler(this.duplicateProcessToolStripMenuItem_Click);
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
            // PNTabItem
            // 
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.splitContainer1);
            this.Name = "PNTabItem";
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

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e) { }

        private void Button_AutoRange_Click(object sender, EventArgs e)
        {
            Canvas.AutoArrange();
        }

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
                p = this.Canvas.LastMouseClickPosition;
            else
                p = this.Canvas.lastRightClickPosition;

            PNPlace classitem = new PNPlace(string.Format("P{0}", Canvas.PlaceCounter), 0, "", 0);
            Canvas.PlaceCounter++;

            classitem.X = p.X;
            classitem.Y = p.Y;

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
                //Add from menu bar
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
                SetDirty();
            }
        }

        public List<CanvasItemData> SelectedItems = new List<CanvasItemData>();
        private Route SelectedRoute;

        public static bool HighLightControlUsingRed = false;

        private void Canvas_CanvasItemSelected(object sender, CanvasItemEventArgs e)
        {
            if (this.Button_AddLink.Checked)
            {
                if (this.SelectedItems.Count > 0 && e.CanvasItem != null)
                {
                    CanvasItemData SelectedItem = this.SelectedItems[0];
                    if (SelectedItem.Item is StateItem && e.CanvasItem.Item is StateItem
                        && SelectedItem.Item.GetType() != e.CanvasItem.Item.GetType())
                    {
                        StateItem sourceState = SelectedItem.Item as StateItem;
                        StateItem targetState = e.CanvasItem.Item as StateItem;
                        PNArc r = new PNArc(SelectedItem.Item, targetState);
                        if (sourceState is PNTransition)
                            (sourceState as PNTransition).OutputPlaces.Add(targetState as PNPlace);
                        else
                            (targetState as PNTransition).InputPlaces.Add(sourceState as PNPlace);

                        this.AddLink(r);
                        SetDirty();
                    }

                    e.CanvasItem.Item.HandleMouseUp(new PointF());
                    Button_AddLink.Checked = false;
                }
                else if (this.SelectedItems.Count == 0 && e.CanvasItem != null)
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
                }

                CanvasItemData SelectedItem = e.CanvasItem;
                if (SelectedItem == null)
                {
                    if (Button_AddNewState.Checked)
                    {
                        Button_AddNewState_Click(sender, e);
                        Button_AddNewState.Checked = false;
                    }

                    if (Button_AddNewTransition.Checked)
                    {
                        Button_AddNewTransition_Click(sender, e);
                        Button_AddNewTransition.Checked = false;
                    }
                }
            }

            SelectedRoute = null;
            this.Button_Delete.Enabled = (this.SelectedItems.Count > 0);
            //if (SelectedItems.Count == 1 && SelectedItems[0].Item is PNPlace)
            //{
            //}
        }

        private void Canvas_CanvasItemsSelected(object sender, CanvasItemsEventArgs e)
        {
            this.SelectedItems.Clear();
            this.SelectedItems.AddRange(e.CanvasItem);
            this.Button_Delete.Enabled = (this.SelectedItems.Count > 0);
        }

        private void Canvas_ItemDoubleClick(object sender, CanvasItemEventArgs e)
        {
            if (e.CanvasItem.Item is PNPlace)
            {
                PNPlace editedState = e.CanvasItem.Item as PNPlace;
                PlaceEditingForm form = new PlaceEditingForm(editedState);
                string lastName = editedState.Name;
                if (form.ShowDialog() == DialogResult.OK)
                {
                    this.StoreCurrentCanvas();
                    SetDirty();
                    if (!this.Canvas.CheckStateNameDuplicate())
                        editedState.Name = lastName;
                }

                Canvas.Refresh();
            }

            if (e.CanvasItem.Item is PNTransition)
            {
                PNTransition editedState = e.CanvasItem.Item as PNTransition;
                TransitionEditingForm form = new TransitionEditingForm(editedState);
                string lastName = editedState.Name;
                if (form.ShowDialog() == DialogResult.OK)
                {
                    this.StoreCurrentCanvas();
                    SetDirty();
                    if (!this.Canvas.CheckStateNameDuplicate())
                        editedState.Name = lastName;
                }

                Canvas.Refresh();
            }
        }

        private void Canvas_RouteDoubleClick(object sender, CanvasRouteEventArgs e)
        {
            EditingArc(e.CanvasItem as PNArc);
        }

        private void EditingArc(PNArc arc)
        {
            ArcEditingForm form = new ArcEditingForm(arc);
            if (form.ShowDialog() == DialogResult.OK)
            {
                // do something here
                SetDirty();
            }
            Canvas.Refresh();
        }

        [System.Obsolete("Return wrong value when the canvas is bigger than the screen. Use PNCanvas.LastMouseClickPosition")]
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
                    RemoveCanvasRoute(SelectedRoute);
                    Canvas.Refresh();
                    SetDirty();
                }
            }

            this.Button_Delete.Enabled = false;
        }

        private void TreeView_Structure_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Node.Parent != null && e.Node.Parent.Text == "Models")
            {
                toolStripContainer1.Visible = true;
                if (Canvas != null && Canvas.Visible)
                    Canvas.Visible = false;

                Canvas = (e.Node.Tag as PNCanvas);
                if (Canvas != null)
                {
                    Canvas.Visible = true;
                    this.Button_AddLink.Checked = false;
                    this.Button_AddNewState.Checked = false;
                    this.Button_Delete.Enabled = Canvas.itemSelected;
                }

                textEditorControl.Visible = false;
            }
            else if (e.Node.Text == "Declaration")
            {
                toolStripContainer1.Visible = false;
                textEditorControl.Visible = true;
            }

            RaiseIsDirtyChanged();
        }

        private void MenuButton_Initial_Click(object sender, EventArgs e) { }

        private void MenuButton_Committed_Click(object sender, EventArgs e) { }

        private void MenuButton_Error_Click(object sender, EventArgs e) { }

        private void contextMenuStrip2_Opening(object sender, CancelEventArgs e)
        {
            if (TreeView_Structure.SelectedNode != null)
            {
                addProcessToolStripMenuItem.Enabled = false;
                deleteProcessToolStripMenuItem.Enabled = false;
                processDetailToolStripMenuItem.Enabled = false;
                duplicateProcessToolStripMenuItem.Enabled = false;

                if (TreeView_Structure.SelectedNode.Text == "Models")
                    addProcessToolStripMenuItem.Enabled = true;
                else if (TreeView_Structure.SelectedNode.Parent != null && TreeView_Structure.SelectedNode.Parent.Text == "Models")
                {
                    deleteProcessToolStripMenuItem.Enabled = true;
                    processDetailToolStripMenuItem.Enabled = true;
                    duplicateProcessToolStripMenuItem.Enabled = true;
                }
            }
        }

        private int pcounter = 1;

        private void addProcessToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Canvas != null)
                Canvas.Visible = false;

            TreeNode node = TreeView_Structure.Nodes[0].Nodes["Models"].Nodes.Add("M" + pcounter);
            pcounter++;

            Canvas = new PNCanvas();
            Canvas.Node = node;
            node.Tag = Canvas;
            TreeView_Structure.SelectedNode = node;

            AddNewProcess(Canvas);

            //add the first 
            PNPlace classitem = new PNPlace("Place " + Canvas.StateCounter, 1, "", 0);
            Canvas.StateCounter++;
            classitem.X = 100 / Canvas.Zoom;
            classitem.Y = 100 / Canvas.Zoom;
            AddCanvasItem(classitem);

            Canvas.Refresh();
            SetDirty();
        }

        private void AddNewProcess(PNCanvas Canvas)
        {
            Canvas.Dock = DockStyle.Fill;
            Canvas.ContextMenuStrip = contextMenuStrip1;
            Canvas.CanvasItemSelected += new EventHandler<CanvasItemEventArgs>(Canvas_CanvasItemSelected);
            Canvas.CanvasItemsSelected += new EventHandler<CanvasItemsEventArgs>(Canvas_CanvasItemsSelected);
            Canvas.CanvasRouteSelected += new EventHandler<CanvasRouteEventArgs>(Canvas_CanvasRouteSelected);
            Canvas.ItemDoubleClick += new EventHandler<CanvasItemEventArgs>(Canvas_ItemDoubleClick);
            Canvas.RouteDoubleClick += new EventHandler<CanvasRouteEventArgs>(Canvas_RouteDoubleClick);
            Canvas.LayoutChanged += new EventHandler(Canvas_LayoutChanged);
            Canvas.SaveCurrentCanvas += new EventHandler(Canvas_SaveCurrentCanvas);

            this.Button_AddLink.Checked = false;
            this.Button_AddNewState.Checked = false;
            this.Button_Delete.Enabled = false;

            toolStripContainer1.ContentPanel.Controls.Add(Canvas);
            toolStripContainer1.Visible = false;
        }

        private void Canvas_LayoutChanged(object sender, EventArgs e)
        {
            SetDirty();
        }

        protected new void SetDirty()
        {
            base.SetDirty();

            if (Canvas != null)
                this.Canvas.RefreshPictureBox();
        }

        private void Canvas_SaveCurrentCanvas(object sender, EventArgs e)
        {
            this.StoreCurrentCanvas();
        }

        private void processDetailsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (TreeView_Structure.SelectedNode != null)
            {
                if (TreeView_Structure.SelectedNode.Parent != null && TreeView_Structure.SelectedNode.Parent.Text == Parsing.MODELS_NODE_NAME)
                {
                    PNCanvas canvas = TreeView_Structure.SelectedNode.Tag as PNCanvas;
                    string lastText = canvas.Node.Text;
                    ModelEditingForm form = new ModelEditingForm(canvas);
                    if (form.ShowDialog() == DialogResult.OK)
                    {
                        form.UpdateData();
                        SetDirty();
                        if (!CheckProcessNameDuplicate())
                            canvas.Node.Text = lastText;
                    }
                }
            }
        }

        private bool CheckProcessNameDuplicate()
        {
            List<string> processNames = new List<string>();
            foreach (TreeNode node in TreeView_Structure.Nodes[0].Nodes[1].Nodes)
            {
                if (processNames.Contains(node.Text))
                {
                    MessageBox.Show(Resources.No_duplicated_process_names_are_allowed,
                        Resources.Duplicate_Process_Name, MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                    return false;
                }

                processNames.Add(node.Text);
            }

            return true;
        }

        private void deleteProcessToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (TreeView_Structure.SelectedNode != null)
            {
                if (TreeView_Structure.SelectedNode.Parent != null && TreeView_Structure.SelectedNode.Parent.Text == "Models")
                {
                    if (MessageBox.Show(Resources.Are_you_sure_you_want_to_delete_the_selected_process_, Resources.Confirmation, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        PNCanvas cavas = TreeView_Structure.SelectedNode.Tag as PNCanvas;
                        if (cavas != null)
                            toolStripContainer1.ContentPanel.Controls.Remove(cavas);
                        toolStripContainer1.Visible = false;
                        textEditorControl.Visible = false;

                        TreeView_Structure.SelectedNode.Remove();
                        TreeView_Structure.SelectedNode = null;

                        SetDirty();
                    }
                }
            }
        }

        private void duplicateProcessToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (TreeView_Structure.SelectedNode != null)
            {
                if (TreeView_Structure.SelectedNode.Parent != null && TreeView_Structure.SelectedNode.Parent.Text == "Models")
                {
                    PNCanvas currentCanvas = TreeView_Structure.SelectedNode.Tag as PNCanvas;
                    MessageBox.Show(currentCanvas.Node.Text);
                    PNCanvas duplicatedCanvas = currentCanvas.Dup();

                    TreeNode node = TreeView_Structure.Nodes[0].Nodes["Models"].Nodes.Add(duplicatedCanvas.Node.Text);
                    duplicatedCanvas.Node = node;
                    node.Tag = duplicatedCanvas;
                    TreeView_Structure.SelectedNode = node;

                    AddNewProcess(duplicatedCanvas);

                    duplicatedCanvas.Refresh();
                    this.Canvas = duplicatedCanvas;
                    SetDirty();
                }
            }
        }

        public override bool CanUndo
        {
            get
            {
                if (textEditorControl.Visible)
                    return this.CodeEditor.EnableUndo;

                if (Canvas != null && Canvas.Visible)
                    return (this.Canvas.currentStateIndex > 0);

                return false;
            }
        }

        public override bool CanRedo
        {
            get
            {
                if (textEditorControl.Visible)
                    return this.CodeEditor.EnableRedo;

                if (Canvas != null && Canvas.Visible)
                    return (this.Canvas.currentStateIndex < this.Canvas.undoStack.Count - 1);

                return false;
            }
        }

        public override bool CanCut
        {
            get
            {
                if (textEditorControl.Visible)
                    return this.EnableCut;

                return false;
            }
        }

        public override bool CanCopy
        {
            get
            {
                if (textEditorControl.Visible)
                    return this.EnableCopy;

                return false;
            }
        }

        public override bool CanPaste
        {
            get
            {
                if (textEditorControl.Visible)
                    return this.EnablePaste;

                return false;
            }
        }

        public override bool CanSelectAll
        {
            get
            {
                if (textEditorControl.Visible)
                    return this.CodeEditor.CanSelect;

                return false;
            }
        }

        public override bool CanFind
        {
            get
            {
                if (textEditorControl.Visible)
                    return true;

                return false;
            }
        }

        public override bool CanPrint
        {
            get
            {
                if (textEditorControl.Visible)
                    return true;

                return false;
            }
        }

        public override void Undo()
        {
            if (textEditorControl.Visible)
                this.CodeEditor.Undo();
            else if (Canvas != null && Canvas.Visible)
                Button_Undo_Click(null, null);
        }

        public override void Redo()
        {
            if (textEditorControl.Visible)
                this.CodeEditor.Redo();
            else if (Canvas != null && Canvas.Visible)
                Button_Redo_Click(null, null);
        }

        public override void Cut()
        {
            textEditorControl.ActiveTextAreaControl.TextArea.ClipboardHandler.Cut(null, null);
        }

        public override void Copy()
        {
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
                textEditorControl.ActiveTextAreaControl.TextArea.ClipboardHandler.Delete(null, null);
            else if (Canvas != null && Canvas.Visible)
                this.Button_Delete_Click(null, null);
        }

        public override bool CanDelete
        {
            get
            {
                if (textEditorControl.Visible)
                    return true;

                if (Canvas != null && Canvas.Visible)
                    return this.SelectedItems.Count > 0;

                return true;
            }
        }

        public override void ZoomOut()
        {
            if (textEditorControl.Visible)
                base.ZoomOut();
            else if (Canvas != null && Canvas.Visible)
                Canvas.Zoom *= 0.9f;
        }

        public override void ZoomIn()
        {
            if (textEditorControl.Visible)
                base.ZoomIn();
            else if (Canvas != null && Canvas.Visible)
                Canvas.Zoom *= 1.1f;
        }

        public override void Zoom100()
        {
            if (textEditorControl.Visible)
                base.Zoom100();
            else if (Canvas != null && Canvas.Visible)
                Canvas.Zoom = 1.0f;
        }

        public override void SplitWindow()
        {
            if (textEditorControl.Visible)
                textEditorControl.Split();
        }

        public override void SetText(string text)
        {
            PNModel pn = PNModel.LoadLTSFromXML(text);

            DeclarationNode.Tag = pn.Declaration;
            textEditorControl.Text = pn.Declaration;

            ModelNode.Nodes.Clear();
            toolStripContainer1.ContentPanel.Controls.Clear();

            Topology = pn.Topology;

            foreach (PNCanvas canvas in pn.Canvases)
            {
                canvas.Dock = DockStyle.Fill;
                canvas.ContextMenuStrip = contextMenuStrip1;
                canvas.CanvasItemSelected += new EventHandler<CanvasItemEventArgs>(Canvas_CanvasItemSelected);
                canvas.CanvasItemsSelected += new EventHandler<CanvasItemsEventArgs>(Canvas_CanvasItemsSelected);
                canvas.CanvasRouteSelected += new EventHandler<CanvasRouteEventArgs>(Canvas_CanvasRouteSelected);
                canvas.ItemDoubleClick += new EventHandler<CanvasItemEventArgs>(Canvas_ItemDoubleClick);
                canvas.RouteDoubleClick += new EventHandler<CanvasRouteEventArgs>(Canvas_RouteDoubleClick);
                canvas.LayoutChanged += new EventHandler(Canvas_LayoutChanged);
                canvas.SaveCurrentCanvas += new EventHandler(Canvas_SaveCurrentCanvas);
                canvas.Visible = false;

                toolStripContainer1.ContentPanel.Controls.Add(canvas);
                ModelNode.Nodes.Add(canvas.Node);
                canvas.undoStack.Clear();
                this.StoreCanvas(canvas);
            }

            toolStripContainer1.Visible = false;
            textEditorControl.Visible = false;

            if (ModelNode.Nodes.Count > 0)
            {
                TreeView_Structure.SelectedNode = ModelNode.Nodes[0];
                TreeView_Structure_NodeMouseDoubleClick(null, new TreeNodeMouseClickEventArgs(ModelNode.Nodes[0], MouseButtons.Left, 2, 0, 0));
            }
        }

        public override string GetText()
        {
            XmlDocument doc = GetDoc();
            return doc.InnerXml;
        }

        public override string FileName
        {
            get { return fileName; }
        }

        private string fileName;
        public override void Save(string filename)
        {
            if (!string.IsNullOrEmpty(filename) && filename != fileName)
                fileName = filename;

            XmlDocument doc = GetDoc();
            doc.Save(fileName);

            HaveFileName = true;
        }



        private XmlDocument GetDoc()
        {
            string declare = "";
            if (this.DeclarationNode.Tag != null)
                declare = this.DeclarationNode.Tag.ToString();

            List<PNCanvas> canvases = new List<PNCanvas>();
            foreach (TreeNode processNode in ModelNode.Nodes)
            {
                if (processNode.Tag is PNCanvas)
                {
                    PNCanvas canvas = (processNode.Tag as PNCanvas);
                    canvases.Add(canvas);
                }
            }

            PNModel pn = new PNModel(declare, canvases, Topology);

            return pn.GenerateXML();
        }

        public override void Open(string filename)
        {
            this.ToolTipText = filename;
            TabText = Path.GetFileName(filename);
            HaveFileName = true;
            fileName = filename;

            StreamReader streamReader = new StreamReader(filename);
            string text = streamReader.ReadToEnd();
            streamReader.Close();
            SetText(text);

            pcounter = 1;
        }

        private void Button_AutoRange_Click_1(object sender, EventArgs e)
        {
            SaveFileDialog svd = new SaveFileDialog();
            svd.Filter = "Bitmap (*.bmp)|*.bmp|All File (*.*)|*.*";

            if (svd.ShowDialog() == DialogResult.OK)
                Canvas.SaveToImage(svd.FileName);
        }

        public override void HandleParsingException(ParsingException ex)
        {
            try
            {
                if (ex is GraphParsingException)
                {
                    string pname = (ex as GraphParsingException).ProcessName;
                    foreach (TreeNode node in ModelNode.Nodes)
                    {
                        if (node.Text == pname)
                        {
                            TreeView_Structure.SelectedNode = node;
                            TreeView_Structure_NodeMouseDoubleClick(null, new TreeNodeMouseClickEventArgs(node, MouseButtons.Left, 2, 0, 0));
                            break;
                        }
                    }
                }
                else
                {
                    TreeView_Structure.SelectedNode = DeclarationNode;
                    TreeView_Structure_NodeMouseDoubleClick(null, new TreeNodeMouseClickEventArgs(DeclarationNode, MouseButtons.Left, 2, 0, 0));

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

        private void StoreCurrentCanvas()
        {
            this.Canvas.undoStack.RemoveRange(this.Canvas.currentStateIndex + 1, (this.Canvas.undoStack.Count - 1) - (this.Canvas.currentStateIndex + 1) + 1);
            this.Canvas.undoStack.Add(this.Canvas.Clone());
            this.Canvas.currentStateIndex = this.Canvas.undoStack.Count - 1;
        }

        private void StoreCanvas(PNCanvas canvas)
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

        private void AddLink(PNArc route)
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
                    this.Canvas.RemoveCanvasItem(SelectedItem.Item);
            }

            foreach (CanvasItemData SelectedItem in this.SelectedItems)
            {
                if (SelectedItem.Item is LabelItem)
                    this.Canvas.RemoveCanvasItem(SelectedItem.Item);
            }

            foreach (CanvasItemData SelectedItem in this.SelectedItems)
            {
                if (SelectedItem.Item is StateItem)
                    this.Canvas.RemoveCanvasItem(SelectedItem.Item);
            }

            this.Canvas.FinishMultiObjectAction();
            this.StoreCurrentCanvas();
        }

        private void Button_AddNewTransition_Click(object sender, EventArgs e)
        {
            PointF p = Point.Empty;
            //Add from menu bar
            if (Button_AddNewTransition.Checked)
                p = this.Canvas.LastMouseClickPosition;
            else
                p = this.Canvas.lastRightClickPosition;

            PNTransition classitem = new PNTransition(string.Format("T{0}", Canvas.TransitionCounter), "", 0);
            Canvas.TransitionCounter++;

            classitem.X = p.X;
            classitem.Y = p.Y;

            AddCanvasItem(classitem);
            Canvas.Refresh();
            SetDirty();
        }
    }
}