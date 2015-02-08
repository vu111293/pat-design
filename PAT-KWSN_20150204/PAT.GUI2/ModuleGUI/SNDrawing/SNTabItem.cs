using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using System.Xml;
using Fireball.Docking;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Document;
using PAT.Common;
using PAT.Common.Classes.Expressions.ExpressionClass;
using PAT.Common.GUI.Drawing;
using PAT.Common.GUI.SNModule;
using PAT.GUI.EditingFunction.CodeCompletion;
using Tools.Diagrams;
using PAT.GUI.Docking;
using CanvasItemData = PAT.Common.GUI.Drawing.LTSCanvas.CanvasItemData;
using PAT.Common.Ultility;

namespace PAT.GUI.SNDrawing
{
    public class SNTabItem : EditorTabItem
    {
        public static string NCPath = Path.Combine(Ultility.ModuleFolderPath, "NESC");
        public static string NCLibPath = Path.Combine(NCPath, "Lib");
        public static string NCExamplePath = Path.Combine(NCPath, "NesCExamples");
        private ContextMenuStrip contextMenuStrip1;
        private IContainer components;

        private ToolStripMenuItem menuButton_NewSensor;
        private ToolStripMenuItem MenuButton_AddLink;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripMenuItem MenuButton_SetInitial;
        //public ImageList imageList1;
        private ToolStripMenuItem MenuButton_Delete;
        private SplitContainer splitContainer1;
        private ToolStripContainer toolStripContainer1;
        private ToolStrip toolStrip1;
        private ToolStripButton button_AddNewSensor;
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
        private ToolStripMenuItem openWindowsExplorerToolStripMenuItem;
        private ToolStripMenuItem showStateMachineToolStripMenuItem;
        private ToolStripMenuItem showTinyOSLibraryToolStripMenuItem;
        private ToolStripMenuItem generateRTSToolStripMenuItem;
        private ToolStripMenuItem porOverheadToolStripMenuItem;
        private ToolStripMenuItem openModelInWindowsExplorerToolStripMenuItem;
        //private ToolStripMenuItem deleteSensorToolStripMenuItem;
        //private ToolStripMenuItem sensorDetailsToolStripMenuItem;
        private TreeView TreeView_Structure;
        private ToolStripButton Button_AutoRange;
        private LTSCanvas Canvas;

        //private TreeNode NetworkNode;
        private TreeNode SensorsNode;
        private TreeNode AssertionNode;

        private ToolStripButton Button_AddNewNail;
        private ToolStripMenuItem MenuButton_NewNail;

        private MethodInfo GenerateCanvas_Method = null;
        private MethodInfo POROverhead_Method = null;
        private MethodInfo RTSGeneration_Method = null;

        /// <summary>
        /// The list of data links in the network, in the form of:
        /// src -> dst
        /// </summary>
        private List<string> LinkIDList = new List<string>();

        public SNTabItem(string moduleName)
        {
            InitializeComponent();
            AddEventHandlerForButtons();

            textEditorControl = new SharpDevelopTextAreaControl();
            textEditorControl.Dock = DockStyle.Fill;
            textEditorControl.ContextMenuStrip = EditorContextMenuStrip;
            textEditorControl.BorderStyle = BorderStyle.Fixed3D;
            textEditorControl.Visible = false;

            splitContainer1.Panel2.Controls.Add(textEditorControl);


            //Canvas.AutoArrange();

            //toolStripContainer1.ContentPanel.Controls.Add(Canvas);

            //TabText = project.Project.ProjectName + " Diagram";

            //Padding = new Padding(1, 2, 1, 1);
            //DockableAreas = DockAreas.Document;

            ////Dock = DockStyle.Fill;
            //Icon = Ultility.GetIcon("Diagram");

            TabText = "Document " + counter;
            counter++;

            textEditorControl.FileNameChanged += _EditorControl_FileNameChanged;
            textEditorControl.TextChanged += textEditorControl_TextChanged;
            textEditorControl.Tag = this;


            Padding = new Padding(2, 2, 2, 2);
            DockableAreas = DockAreas.Document;

            secondaryViewContentCollection = new SecondaryViewContentCollection(this);
            InitFiles();

            file = FileService.CreateUntitledOpenedFile(TabText, new byte[] { });
            file.CurrentView = this;
            textEditorControl.FileName = file.FileName;
            files.Clear();
            files.Add(file);

            SetSyntaxLanguage(moduleName);
            //ForceFoldingUpdate();
            //textEditorControl.InitializeFormatter();
            //textEditorControl.ActivateQuickClassBrowserOnDemand();


            textEditorControl.Document.FoldingManager.FoldingStrategy = new FoldingStrategy();

            // Highlight the matching bracket or not...
            textEditorControl.ShowMatchingBracket = true;

            textEditorControl.BracketMatchingStyle = BracketMatchingStyle.Before;


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

            TreeView_Structure.HideSelection = false;
            splitContainer1.SplitterDistance = 100;

            //Button_AddNewSensor_Click();


            //addSensorToolStripMenuItem.PerformClick();
            InitNetwork();

            TreeView_Structure.ExpandAll();

            Button_AddNewNail.Visible = false;
        }

        private void InitNetwork()
        {
            if (Canvas != null)
            {
                return;
            }

            Canvas = new LTSCanvas();
            Canvas.Node.Name = "SN";
            Canvas.Node = TreeView_Structure.Nodes[0];
            TreeView_Structure.Nodes[0].Tag = Canvas;
            Canvas.Dock = DockStyle.Fill;
            Canvas.ContextMenuStrip = contextMenuStrip1;
            Canvas.CanvasItemSelected += Canvas_CanvasItemSelected;
            Canvas.CanvasItemsSelected += Canvas_CanvasItemsSelected;
            Canvas.CanvasRouteSelected += Canvas_CanvasRouteSelected;
            Canvas.ItemDoubleClick += Canvas_ItemDoubleClick;
            Canvas.RouteDoubleClick += Canvas_RouteDoubleClick;
            Canvas.LayoutChanged += Canvas_LayoutChanged;
            Canvas.SaveCurrentCanvas += Canvas_SaveCurrentCanvas;

            Button_AddLink.Checked = false;
            button_AddNewSensor.Checked = false;
            Button_Delete.Enabled = false;
            //Canvas.Visible = false;

            //splitContainer1.Panel2.Controls.Add(Canvas);
            toolStripContainer1.ContentPanel.Controls.Add(Canvas);
            textEditorControl.Visible = false;
            toolStripContainer1.Visible = true;

            //add the first 
            int id = Canvas.StateCounter - 1;
            string nodeID = "Sensor" + (id);
            StateItem classitem = new StateItem(false, nodeID);
            Canvas.StateCounter++;

            classitem.X = 100 / Canvas.Zoom;
            classitem.Y = 100 / Canvas.Zoom;

            //classitem.Initialize();

            AddCanvasItem(classitem);
            Canvas.Refresh();

            Canvas.Visible = true;

            TreeNode newNode = SensorsNode.Nodes.Add(nodeID);
            newNode.Name = nodeID;
            newNode.Text = nodeID;
            newNode.Tag = new NodeData(nodeID, id.ToString());

            //button_AddNewSensor.PerformClick();
            //button_AddNewSensor.Checked = false;

            TreeView_Structure.SelectedNode = TreeView_Structure.Nodes[0];

            SetDirty();
        }

        private void AddEventHandlerForButtons()
        {
            foreach (ToolStripItem button in toolStrip1.Items)
            {
                if (button is ToolStripButton)
                {
                    ((ToolStripButton)button).CheckStateChanged += button_CheckStateChanged;
                }
            }
        }

        private void contextMenuStrip2_Opening(object sender, CancelEventArgs e)
        {
            openWindowsExplorerToolStripMenuItem.Enabled = false;
            openWindowsExplorerToolStripMenuItem.Visible = false;
            porOverheadToolStripMenuItem.Enabled = false;
            porOverheadToolStripMenuItem.Visible = false;
            showStateMachineToolStripMenuItem.Enabled = false;
            showStateMachineToolStripMenuItem.Visible = false;
            generateRTSToolStripMenuItem.Enabled = false;
            generateRTSToolStripMenuItem.Visible = false;
            showTinyOSLibraryToolStripMenuItem.Enabled = false;
            showTinyOSLibraryToolStripMenuItem.Visible = false;
            openModelInWindowsExplorerToolStripMenuItem.Enabled = false;
            openModelInWindowsExplorerToolStripMenuItem.Visible = false;
            if (TreeView_Structure.SelectedNode != null)
            {
                var selectedNode = TreeView_Structure.SelectedNode;
                if (selectedNode.Parent != null && selectedNode.Parent.Parent != null
                    && selectedNode.Parent.Parent.Text == Parsing.SENSORS_NODE_NAME)
                {
                    openWindowsExplorerToolStripMenuItem.Enabled = true;
                    openWindowsExplorerToolStripMenuItem.Visible = true;
                }
                else if (selectedNode.Parent != null && selectedNode.Parent.Text == Parsing.SENSORS_NODE_NAME)
                {
                    showStateMachineToolStripMenuItem.Enabled = true;
                    showStateMachineToolStripMenuItem.Visible = true;
                    showTinyOSLibraryToolStripMenuItem.Enabled = true;
                    showTinyOSLibraryToolStripMenuItem.Visible = true;
                    generateRTSToolStripMenuItem.Enabled = true;
                    generateRTSToolStripMenuItem.Visible = true;
                }
                else if (selectedNode.Text == Parsing.SENSORNETWORK_NODE_NAME)
                {
                    porOverheadToolStripMenuItem.Enabled = true;
                    porOverheadToolStripMenuItem.Visible = true;
                    openModelInWindowsExplorerToolStripMenuItem.Enabled = true;
                    openModelInWindowsExplorerToolStripMenuItem.Visible = true;
                }
            }
        }

        //openModelInWindowsExplorerToolStripMenuItem_Click
        private void openModelInWindowsExplorerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string appPath = fileName;

            if (!(string.IsNullOrEmpty(appPath)) && Path.IsPathRooted(appPath))
            {
                if (File.Exists(appPath))
                {
                    appPath = Path.GetDirectoryName(appPath);
                    Process.Start(@appPath);
                }
                else
                {
                    MessageBox.Show("File + " + appPath + "doesn't exist!", "Error!",
                     MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
            else
            {
                MessageBox.Show("Please save the model file first!", "Error!",
                     MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }


        }

        //showStateMachineToolStripMenuItem_Click
        private void showStateMachineToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //string appPath = TreeView_Structure.SelectedNode.Tag as string;
            //appPath = Path.GetDirectoryName(appPath);
            //if (Directory.Exists(appPath))
            //{
            //    Process.Start(@appPath);
            //}
            var sensorID = TreeView_Structure.SelectedNode.Name;
            var canvas = GetCanvasOfSensor(sensorID);
            if (canvas != null)
            {
                var form = new StateMachineForm(sensorID, canvas);
            }
            else
            {
                MessageBox.Show("The state machine for " + sensorID + " can not be generated!", "Error!",
                      MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        //showTinyOSLibraryToolStripMenuItem_Click
        private void showTinyOSLibraryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Directory.Exists(@NCLibPath))
            {
                Process.Start(@NCLibPath);
            }
        }

        //generateRTSToolStripMenuItem_Click
        private void generateRTSToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var sensorID = TreeView_Structure.SelectedNode.Name;
            string rtsModel = "";

            if (RTSGeneration_Method != null)
            {
                var para = new object[3];
                para[0] = sensorID;
                para[1] = this.GetText();
                para[2] = this.FileName;
                rtsModel = (string)RTSGeneration_Method.Invoke(null, para);
            }

            var rtsFile = sensorID + ".rts";
            rtsFile = Path.Combine(Path.GetDirectoryName(FileName), rtsFile);

            System.IO.File.WriteAllText(@rtsFile, rtsModel);

            MessageBox.Show(
                "The RTS model for " + sensorID + " has been generated successfully in " + rtsFile + ".",
                "Generate RTS Model",
                MessageBoxButtons.OK,
                MessageBoxIcon.Exclamation);
        }

        //porOverheadToolStripMenuItem
        private void porOverheadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //GetStaticAnalysisOverhead
            TimeSpan porOverhead = new TimeSpan();

            if (POROverhead_Method != null)
            {
                var para = new object[2];
                para[0] = this.GetText();
                para[1] = this.FileName;
                porOverhead = (TimeSpan)POROverhead_Method.Invoke(null, para);

            }

            MessageBox.Show("The POR overhead time is " + porOverhead.TotalMilliseconds + "ms.", "Static Analysis",
                      MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }

        private void openWindowsExplorerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string appPath = TreeView_Structure.SelectedNode.Tag as string;
            appPath = Path.GetDirectoryName(appPath);
            if (Directory.Exists(appPath))
            {
                Process.Start(@appPath);
            }
        }

        private TreeNode LastSelectedNode;
        private bool TextEditorChanged;

        private void SaveSourceCode()
        {
            string appFilePath = LastSelectedNode.Tag as string;

            if (File.Exists(appFilePath))
            {
                try
                {
                    File.WriteAllText(@appFilePath, textEditorControl.Text);
                }
                catch (DirectoryNotFoundException)
                {

                }
                catch (FileNotFoundException)
                {

                }
            }
        }

        //todo
        protected new void textEditorControl_TextChanged(object sender, EventArgs e)
        {
            textEditorControl.Document.FoldingManager.UpdateFoldings(null, null);
            TextEditorChanged = true;
            //if (TreeView_Structure.SelectedNode != null)
            //{
            //    TreeNode node = TreeView_Structure.SelectedNode;
            //    if(node.Parent != null && node.Parent.Parent != null && node.Parent.Parent.Text == Parsing.SENSORS_NODE_NAME)
            //    {
            //        string appFilePath = node.Tag as string;
            //        if (File.Exists(appFilePath)) {
            //            try
            //            {
            //                File.WriteAllText(@appFilePath, textEditorControl.Text);
            //            }
            //            catch (DirectoryNotFoundException)
            //            {
            //            }
            //            catch (FileNotFoundException)
            //            {
            //            }
            //        }
            //    }
            //    else
            //    {
            //        node.Tag = textEditorControl.Text;
            //    }
            //}
            SetDirty();
        }

        #region InitializeComponent
        private void InitializeComponent()
        {
            components = new Container();
            System.ComponentModel.ComponentResourceManager resources = new ComponentResourceManager(typeof(SNTabItem));
            //NetworkNode = new TreeNode("Network");
            SensorsNode = new TreeNode(Parsing.SENSORS_NODE_NAME);
            AssertionNode = new TreeNode(Parsing.ASSERTION_NODE_NAME);
            TreeNode treeNode8 = new TreeNode(Parsing.SENSORNETWORK_NODE_NAME, new[] {
            //NetworkNode,
                        AssertionNode,
            SensorsNode});
            contextMenuStrip1 = new ContextMenuStrip(components);
            menuButton_NewSensor = new ToolStripMenuItem();
            MenuButton_AddLink = new ToolStripMenuItem();
            MenuButton_Delete = new ToolStripMenuItem();
            toolStripSeparator1 = new ToolStripSeparator();
            MenuButton_SetInitial = new ToolStripMenuItem();
            splitContainer1 = new SplitContainer();
            TreeView_Structure = new TreeView();
            contextMenuStrip2 = new ContextMenuStrip(components);
            openWindowsExplorerToolStripMenuItem = new ToolStripMenuItem();
            showStateMachineToolStripMenuItem = new ToolStripMenuItem();
            showTinyOSLibraryToolStripMenuItem = new ToolStripMenuItem();
            generateRTSToolStripMenuItem = new ToolStripMenuItem();
            porOverheadToolStripMenuItem = new ToolStripMenuItem();
            openModelInWindowsExplorerToolStripMenuItem = new ToolStripMenuItem();
            //deleteSensorToolStripMenuItem = new ToolStripMenuItem();
            //sensorDetailsToolStripMenuItem = new ToolStripMenuItem();
            imageList2 = new ImageList(components);
            toolStripContainer1 = new ToolStripContainer();
            toolStrip1 = new ToolStrip();
            button_AddNewSensor = new ToolStripButton();
            Button_Delete = new ToolStripButton();
            Button_AddLink = new ToolStripButton();
            toolStripSeparator3 = new ToolStripSeparator();
            Button_ExportBMP = new ToolStripButton();
            Button_ExpandAllCommand = new ToolStripButton();
            Button_CollapseAllCommand = new ToolStripButton();
            Button_MatchAllWidthsCommand = new ToolStripButton();
            Button_ShrinkAllWidthsCommand = new ToolStripButton();
            Button_ZoomIn = new ToolStripButton();
            Button_ZoomOut = new ToolStripButton();
            Button_AutoRange = new ToolStripButton();
            Button_AddNewNail = new ToolStripButton();
            MenuButton_NewNail = new ToolStripMenuItem();
            contextMenuStrip1.SuspendLayout();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            contextMenuStrip2.SuspendLayout();
            toolStripContainer1.TopToolStripPanel.SuspendLayout();
            toolStripContainer1.SuspendLayout();
            toolStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // contextMenuStrip1
            // 
            contextMenuStrip1.Items.AddRange(new ToolStripItem[] {
            menuButton_NewSensor,
            MenuButton_AddLink,
            MenuButton_NewNail,
            MenuButton_Delete,
            toolStripSeparator1,
            MenuButton_SetInitial});
            contextMenuStrip1.Name = "contextMenuStrip1";
            contextMenuStrip1.Size = new Size(163, 120);
            contextMenuStrip1.Opening += contextMenuStrip1_Opening;
            // 
            // menuButton_NewSensor
            // 
            menuButton_NewSensor.Name = "menuButton_NewSensor";
            menuButton_NewSensor.Image = Properties.Resources.plus_circle;
            menuButton_NewSensor.Size = new Size(162, 22);
            menuButton_NewSensor.Text = "New Sensor";
            menuButton_NewSensor.Click += Button_AddNewSensor_Click;
            // 
            // MenuButton_AddLink
            // 
            MenuButton_AddLink.Image = Properties.Resources.link;
            MenuButton_AddLink.ImageTransparentColor = Color.Magenta;
            MenuButton_AddLink.Name = "MenuButton_AddLink";
            MenuButton_AddLink.Size = new Size(162, 22);
            MenuButton_AddLink.Text = "Add Link";
            MenuButton_AddLink.Visible = false;
            // 
            // MenuButton_Delete
            // 
            //Properties.Resources.delete = MenuButton_Delete.Image;
            MenuButton_Delete.Image = Properties.Resources.delete;
            MenuButton_Delete.Name = "MenuButton_Delete";
            MenuButton_Delete.Size = new Size(162, 22);
            MenuButton_Delete.Text = "Delete";
            MenuButton_Delete.Click += Button_Delete_Click;
            MenuButton_Delete.ShortcutKeys = Keys.Delete;
            MenuButton_Delete.ShortcutKeyDisplayString = "Delete";
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new Size(159, 6);
            // 
            // MenuButton_SetInitial
            // 
            MenuButton_SetInitial.Image = ((Image)(resources.GetObject("MenuButton_SetInitial.Image")));
            MenuButton_SetInitial.Name = "MenuButton_SetInitial";
            MenuButton_SetInitial.Size = new Size(162, 22);
            MenuButton_SetInitial.Text = "Set as Initial State";
            MenuButton_SetInitial.Click += MenuButton_Initial_Click;
            // 
            // splitContainer1
            // 
            splitContainer1.Dock = DockStyle.Fill;
            splitContainer1.Location = new Point(0, 0);
            splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(TreeView_Structure);
            splitContainer1.Panel1MinSize = 120;
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(toolStripContainer1);
            splitContainer1.Panel2MinSize = 100;
            splitContainer1.Size = new Size(617, 399);
            splitContainer1.SplitterDistance = 178;
            splitContainer1.TabIndex = 3;
            // 
            // TreeView_Structure
            // 
            TreeView_Structure.ContextMenuStrip = contextMenuStrip2;
            TreeView_Structure.Dock = DockStyle.Fill;
            TreeView_Structure.HideSelection = false;
            TreeView_Structure.Location = new Point(0, 0);
            TreeView_Structure.Name = "TreeView_Structure";
            //NetworkNode.Name = "Network";
            //NetworkNode.StateImageIndex = 0;
            //NetworkNode.Text = "Network";
            AssertionNode.Name = Parsing.ASSERTION_NODE_NAME;
            AssertionNode.StateImageIndex = 0;
            AssertionNode.Text = Parsing.ASSERTION_NODE_NAME;
            SensorsNode.Name = Parsing.SENSORS_NODE_NAME;
            SensorsNode.StateImageIndex = 1;
            SensorsNode.Text = Parsing.SENSORS_NODE_NAME;
            treeNode8.Name = Parsing.SENSORNETWORK_NODE_NAME;
            treeNode8.Text = Parsing.SENSORNETWORK_NODE_NAME;
            TreeView_Structure.Nodes.AddRange(new[] {
            treeNode8});
            TreeView_Structure.Size = new Size(178, 399);
            TreeView_Structure.StateImageList = imageList2;
            TreeView_Structure.TabIndex = 0;
            TreeView_Structure.NodeMouseClick += TreeView_Structure_NodeMouseClick;
            TreeView_Structure.NodeMouseDoubleClick += TreeView_Structure_NodeMouseDoubleClick;
            // 
            // contextMenuStrip2
            // 
            contextMenuStrip2.Items.Add(openWindowsExplorerToolStripMenuItem);
            contextMenuStrip2.Items.Add(showStateMachineToolStripMenuItem);
            contextMenuStrip2.Items.Add(showTinyOSLibraryToolStripMenuItem);
            contextMenuStrip2.Items.Add(generateRTSToolStripMenuItem);
            contextMenuStrip2.Items.Add(porOverheadToolStripMenuItem);
            contextMenuStrip2.Items.Add(openModelInWindowsExplorerToolStripMenuItem);
            contextMenuStrip2.Name = "contextMenuStrip2";
            contextMenuStrip2.Size = new Size(50, 15);
            contextMenuStrip2.Opening += contextMenuStrip2_Opening;
            // 
            // openWindowsExplorerToolStripMenuItem
            // 
            openWindowsExplorerToolStripMenuItem.Name = "openWindowsExplorerToolStripMenuItem";
            openWindowsExplorerToolStripMenuItem.Size = new Size(50, 15);
            openWindowsExplorerToolStripMenuItem.Text = "View File in Windows Explorer";
            openWindowsExplorerToolStripMenuItem.Click += openWindowsExplorerToolStripMenuItem_Click;

            //
            //showStateMachineToolStripMenuItem
            //
            showStateMachineToolStripMenuItem.Name = "showStateMachineToolStripMenuItem";
            showStateMachineToolStripMenuItem.Size = new Size(50, 15);
            showStateMachineToolStripMenuItem.Text = "Show State Machine";
            showStateMachineToolStripMenuItem.Click += showStateMachineToolStripMenuItem_Click;

            //showTinyOSLibraryToolStripMenuItem
            //
            showTinyOSLibraryToolStripMenuItem.Name = "showTinyOSLibraryToolStripMenuItem";
            showTinyOSLibraryToolStripMenuItem.Size = new Size(50, 15);
            showTinyOSLibraryToolStripMenuItem.Text = "View TinyOS Library In Windows Explorer";
            showTinyOSLibraryToolStripMenuItem.Click += showTinyOSLibraryToolStripMenuItem_Click;

            //generateRTSToolStripMenuItem
            //
            generateRTSToolStripMenuItem.Name = "generateRTSToolStripMenuItem";
            generateRTSToolStripMenuItem.Size = new Size(50, 15);
            generateRTSToolStripMenuItem.Text = "Generate RTS Model";
            generateRTSToolStripMenuItem.Click += generateRTSToolStripMenuItem_Click;

            //
            //porOverheadToolStripMenuItem
            //
            porOverheadToolStripMenuItem.Name = "porOverheadToolStripMenuItem";
            porOverheadToolStripMenuItem.Size = new Size(50, 15);
            porOverheadToolStripMenuItem.Text = "Calculate POR overhead time";
            porOverheadToolStripMenuItem.Click += porOverheadToolStripMenuItem_Click;

            //
            //openModelInWindowsExplorerToolStripMenuItem
            //
            openModelInWindowsExplorerToolStripMenuItem.Name = "openModelInWindowsExplorerToolStripMenuItem";
            openModelInWindowsExplorerToolStripMenuItem.Size = new Size(50, 15);
            openModelInWindowsExplorerToolStripMenuItem.Text = "View Model in Windows Explorer";
            openModelInWindowsExplorerToolStripMenuItem.Click += openModelInWindowsExplorerToolStripMenuItem_Click;


            // 
            // deleteSensorToolStripMenuItem
            // 
            //deleteSensorToolStripMenuItem.Name = "deleteSensorToolStripMenuItem";
            //deleteSensorToolStripMenuItem.Size = new Size(146, 22);
            //deleteSensorToolStripMenuItem.Text = "Delete Sensor";
            //deleteSensorToolStripMenuItem.Click += deleteSensorToolStripMenuItem_Click;
            // 
            // sensorDetailsToolStripMenuItem
            // 
            //sensorDetailsToolStripMenuItem.Name = "sensorDetailsToolStripMenuItem";
            //sensorDetailsToolStripMenuItem.Size = new Size(146, 22);
            //sensorDetailsToolStripMenuItem.Text = "Sensor Details";
            //sensorDetailsToolStripMenuItem.Click += sensorDetailsToolStripMenuItem_Click;
            // 
            // imageList2
            // 
            imageList2.ImageStream = ((ImageListStreamer)(resources.GetObject("imageList2.ImageStream")));
            imageList2.TransparentColor = Color.Transparent;
            imageList2.Images.SetKeyName(0, "declare.png");
            imageList2.Images.SetKeyName(1, "channel.png");
            imageList2.Images.SetKeyName(2, "templates.png");
            imageList2.Images.SetKeyName(3, "questionMark.png");
            // 
            // toolStripContainer1
            // 
            // 
            // toolStripContainer1.ContentPanel
            // 
            toolStripContainer1.ContentPanel.Size = new Size(435, 374);
            toolStripContainer1.Dock = DockStyle.Fill;
            toolStripContainer1.Location = new Point(0, 0);
            toolStripContainer1.Name = "toolStripContainer1";
            toolStripContainer1.Size = new Size(435, 399);
            toolStripContainer1.TabIndex = 3;
            // 
            // toolStripContainer1.TopToolStripPanel
            // 
            toolStripContainer1.TopToolStripPanel.Controls.Add(toolStrip1);
            // 
            // toolStrip1
            // 
            toolStrip1.Dock = DockStyle.None;
            toolStrip1.Items.AddRange(new ToolStripItem[] {
            button_AddNewSensor,
            Button_Delete,
            Button_AddLink,
            Button_AddNewNail,
            toolStripSeparator3,
            Button_ExportBMP,
            Button_AutoRange,
            Button_ExpandAllCommand,
            Button_CollapseAllCommand,
            Button_MatchAllWidthsCommand,
            Button_ShrinkAllWidthsCommand,
            Button_ZoomIn,
            Button_ZoomOut});
            toolStrip1.Location = new Point(3, 0);
            toolStrip1.Name = "toolStrip1";
            toolStrip1.Size = new Size(133, 25);
            toolStrip1.TabIndex = 1;
            // 
            // button_AddNewSensor
            // 
            button_AddNewSensor.CheckOnClick = true;
            button_AddNewSensor.DisplayStyle = ToolStripItemDisplayStyle.Image;
            button_AddNewSensor.Image = Properties.Resources.plus_circle;
            button_AddNewSensor.Name = "button_AddNewSensor";
            button_AddNewSensor.Size = new Size(23, 22);
            button_AddNewSensor.Text = "Add New Sensor";
            //button_AddNewSensor.Click += Button_AddNewSensor_Click;

            // 
            // Button_Delete
            // 
            Button_Delete.DisplayStyle = ToolStripItemDisplayStyle.Image;
            Button_Delete.Enabled = false;
            Button_Delete.Image = Properties.Resources.delete;
            Button_Delete.ImageTransparentColor = Color.Magenta;
            Button_Delete.Name = "Button_Delete";
            Button_Delete.Size = new Size(23, 22);
            Button_Delete.Text = "Delete";
            Button_Delete.Click += Button_Delete_Click;
            // 
            // Button_AddLink
            // 
            Button_AddLink.CheckOnClick = true;
            Button_AddLink.DisplayStyle = ToolStripItemDisplayStyle.Image;
            Button_AddLink.Image = ((Image)(resources.GetObject("Button_AddLink.Image")));
            Button_AddLink.ImageTransparentColor = Color.Magenta;
            Button_AddLink.Name = "Button_AddLink";
            Button_AddLink.Size = new Size(23, 22);
            Button_AddLink.Text = "Add Link";
            Button_AddLink.Click += Button_AddLink_Click;
            // 
            // toolStripSeparator3
            // 
            toolStripSeparator3.Name = "toolStripSeparator3";
            toolStripSeparator3.Size = new Size(6, 25);
            // 
            // Button_ExportBMP
            // 
            Button_ExportBMP.DisplayStyle = ToolStripItemDisplayStyle.Image;
            Button_ExportBMP.Image = ((Image)(resources.GetObject("Button_ExportBMP.Image")));
            Button_ExportBMP.Name = "Button_ExportBMP";
            Button_ExportBMP.Size = new Size(23, 22);
            Button_ExportBMP.Text = "Export Drawing as Bitmap";
            Button_ExportBMP.Click += Button_ExportBMP_Click;
            // 
            // Button_ExpandAllCommand
            // 
            Button_ExpandAllCommand.DisplayStyle = ToolStripItemDisplayStyle.Image;
            Button_ExpandAllCommand.Image = ((Image)(resources.GetObject("Button_ExpandAllCommand.Image")));
            Button_ExpandAllCommand.Name = "Button_ExpandAllCommand";
            Button_ExpandAllCommand.Size = new Size(23, 22);
            Button_ExpandAllCommand.Text = "Expand All";
            Button_ExpandAllCommand.Visible = false;
            // 
            // Button_CollapseAllCommand
            // 
            Button_CollapseAllCommand.DisplayStyle = ToolStripItemDisplayStyle.Image;
            Button_CollapseAllCommand.Image = ((Image)(resources.GetObject("Button_CollapseAllCommand.Image")));
            Button_CollapseAllCommand.Name = "Button_CollapseAllCommand";
            Button_CollapseAllCommand.Size = new Size(23, 22);
            Button_CollapseAllCommand.Text = "Collapse All";
            Button_CollapseAllCommand.Visible = false;
            // 
            // Button_MatchAllWidthsCommand
            // 
            Button_MatchAllWidthsCommand.DisplayStyle = ToolStripItemDisplayStyle.Image;
            Button_MatchAllWidthsCommand.Image = ((Image)(resources.GetObject("Button_MatchAllWidthsCommand.Image")));
            Button_MatchAllWidthsCommand.Name = "Button_MatchAllWidthsCommand";
            Button_MatchAllWidthsCommand.Size = new Size(23, 22);
            Button_MatchAllWidthsCommand.Text = "Match All Widths";
            Button_MatchAllWidthsCommand.Visible = false;
            // 
            // Button_ShrinkAllWidthsCommand
            // 
            Button_ShrinkAllWidthsCommand.DisplayStyle = ToolStripItemDisplayStyle.Image;
            Button_ShrinkAllWidthsCommand.Image = ((Image)(resources.GetObject("Button_ShrinkAllWidthsCommand.Image")));
            Button_ShrinkAllWidthsCommand.Name = "Button_ShrinkAllWidthsCommand";
            Button_ShrinkAllWidthsCommand.Size = new Size(23, 22);
            Button_ShrinkAllWidthsCommand.Text = "Shrink All Widths";
            Button_ShrinkAllWidthsCommand.Visible = false;
            // 
            // Button_ZoomIn
            // 
            Button_ZoomIn.DisplayStyle = ToolStripItemDisplayStyle.Image;
            Button_ZoomIn.Image = ((Image)(resources.GetObject("Button_ZoomIn.Image")));
            Button_ZoomIn.ImageTransparentColor = Color.Black;
            Button_ZoomIn.Name = "Button_ZoomIn";
            Button_ZoomIn.Size = new Size(23, 22);
            Button_ZoomIn.Text = "Zoom In";
            Button_ZoomIn.Visible = false;
            Button_ZoomIn.Click += Button_ZoomIn_Click;
            // 
            // Button_ZoomOut
            // 
            Button_ZoomOut.DisplayStyle = ToolStripItemDisplayStyle.Image;
            Button_ZoomOut.Image = ((Image)(resources.GetObject("Button_ZoomOut.Image")));
            Button_ZoomOut.ImageTransparentColor = Color.Black;
            Button_ZoomOut.Name = "Button_ZoomOut";
            Button_ZoomOut.Size = new Size(23, 22);
            Button_ZoomOut.Text = "Zoom Out";
            Button_ZoomOut.Visible = false;
            Button_ZoomOut.Click += Button_ZoomOut_Click;
            // 
            // Button_AutoRange
            // 
            Button_AutoRange.DisplayStyle = ToolStripItemDisplayStyle.Image;
            Button_AutoRange.Image = ((Image)(resources.GetObject("Button_AutoRange.Image")));
            Button_AutoRange.Name = "Button_AutoRange";
            Button_AutoRange.Size = new Size(23, 22);
            Button_AutoRange.Text = "Auto Range";
            Button_AutoRange.Visible = false;
            Button_AutoRange.Click += Button_AutoRange_Click;
            // 
            // Button_AddNewNail
            //
            Button_AddNewNail.CheckOnClick = true;
            Button_AddNewNail.DisplayStyle = ToolStripItemDisplayStyle.Image;
            Button_AddNewNail.Image = Properties.Resources.nail;
            Button_AddNewNail.ImageTransparentColor = Color.Magenta;
            Button_AddNewNail.Name = "Button_AddNewNail";
            Button_AddNewNail.Size = new Size(23, 22);
            Button_AddNewNail.Text = "New Nail";

            // 
            // MenuButton_NewNail
            // 
            MenuButton_NewNail.Name = "MenuButton_NewNail";
            MenuButton_NewNail.Size = new Size(162, 22);
            MenuButton_NewNail.Image = Properties.Resources.nail;
            MenuButton_NewNail.Text = "New Nail";
            MenuButton_NewNail.ToolTipText = "New Nail";
            MenuButton_NewNail.Click += Button_AddNewNail_Click;
            // 
            // LTSTabItem
            // 
            ClientSize = new Size(617, 399);
            Controls.Add(splitContainer1);
            Name = "SNTabItem";
            contextMenuStrip1.ResumeLayout(false);
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel2.ResumeLayout(false);
            splitContainer1.ResumeLayout(false);
            contextMenuStrip2.ResumeLayout(false);
            toolStripContainer1.TopToolStripPanel.ResumeLayout(false);
            toolStripContainer1.TopToolStripPanel.PerformLayout();
            toolStripContainer1.ResumeLayout(false);
            toolStripContainer1.PerformLayout();
            toolStrip1.ResumeLayout(false);
            toolStrip1.PerformLayout();
            ResumeLayout(false);

            InitializeCanvasMethod();
            InitializePOROverheadsMethod();
            InitializeGenerateRTSMethod();

        }

        private void InitializeGenerateRTSMethod()
        {
            string file = Path.Combine(Path.Combine(Common.Ultility.Ultility.ModuleFolderPath, "NESC"), "PAT.Module.NESC.dll");
            Assembly assembly = Assembly.LoadFrom(file);
            Module[] modules = assembly.GetModules(false);
            Type type = null;
            Type[] types = modules[0].GetTypes();
            if (types != null && types.Length > 0)
            {
                for (int i = 0; i < types.Length; i++)
                {
                    if (types[i].Name == "Specification")
                    {
                        type = types[i];
                        break;
                    }
                }
            }

            if (type != null)
            {
                RTSGeneration_Method = type.GetMethod("GenerateRTSModel");
            }
        }

        private void InitializePOROverheadsMethod()
        {
            string file = Path.Combine(Path.Combine(Common.Ultility.Ultility.ModuleFolderPath, "NESC"), "PAT.Module.NESC.dll");
            Assembly assembly = Assembly.LoadFrom(file);
            Module[] modules = assembly.GetModules(false);
            Type type = null;
            Type[] types = modules[0].GetTypes();
            if (types != null && types.Length > 0)
            {
                for (int i = 0; i < types.Length; i++)
                {
                    if (types[i].Name == "Specification")
                    {
                        type = types[i];
                        break;
                    }
                }
            }

            if (type != null)
            {
                POROverhead_Method = type.GetMethod("GetStaticAnalysisOverhead");
            }

        }


        private void InitializeCanvasMethod()
        {
            string file = Path.Combine(Path.Combine(Common.Ultility.Ultility.ModuleFolderPath, "NESC"), "PAT.Module.NESC.dll");
            Assembly assembly = Assembly.LoadFrom(file);
            Module[] modules = assembly.GetModules(false);
            Type type = null;
            Type[] types = modules[0].GetTypes();
            if (types != null && types.Length > 0)
            {
                for (int i = 0; i < types.Length; i++)
                {
                    if (types[i].Name == "Specification")
                    {
                        type = types[i];
                        break;
                    }
                }
            }

            if (type != null)
            {
                GenerateCanvas_Method = type.GetMethod("GenerateSymbolicLTSForSensor");
            }

        }

        private LTSCanvas GetCanvasOfSensor(string sensorID)
        {
            if (GenerateCanvas_Method != null)
            {
                var para = new object[3];
                para[0] = sensorID;
                para[1] = this.Text;
                para[2] = this.FileName;
                var resultv = GenerateCanvas_Method.Invoke(null, para) as LTSCanvas;
                return resultv;
            }
            return null;
        }

        void TreeView_Structure_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Button == MouseButtons.Right && e.Clicks == 1)
            {
                TreeView_Structure.SelectedNode = e.Node;
                contextMenuStrip2.Show(e.Location);
            }
            else
            {

                if (LastSelectedNode != null && LastSelectedNode.Parent != null
                && LastSelectedNode.Parent.Parent != null
                && LastSelectedNode.Parent.Parent.Text == Parsing.SENSORS_NODE_NAME)
                {//The last selected node is a nesc file node
                    if (!(e.Node.Parent != null
                        && e.Node.Parent.Text == LastSelectedNode.Parent.Text
                        && e.Node.Parent.Parent != null
                        && e.Node.Parent.Parent.Text == Parsing.SENSORS_NODE_NAME
                        && e.Node.Text == LastSelectedNode.Text
                        && e.Node.Name == LastSelectedNode.Name))
                    {//the node clicked is a nesC file node different from the current one
                        if (TextEditorChanged)
                        {
                            TextEditorChanged = false;
                            if (MessageBox.Show("Do you want to save the changes of " + LastSelectedNode.Name + "?",
                                "Yes", MessageBoxButtons.YesNo) == DialogResult.Yes)
                            {
                                SaveSourceCode();
                            }

                        }

                    }
                }
                else if (LastSelectedNode != null
                   && LastSelectedNode.Text == Parsing.ASSERTION_NODE_NAME
                   && e.Node.Text != Parsing.ASSERTION_NODE_NAME)
                {
                    if (TextEditorChanged)
                    {
                        TextEditorChanged = false;
                        if (MessageBox.Show("Do you want to save the changes of Assertions?",
                            "Yes", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        {
                            LastSelectedNode.Tag = textEditorControl.Text;
                        }

                    }
                }

                LastSelectedNode = e.Node;

                if (e.Node.Parent != null && e.Node.Parent.Parent != null && e.Node.Parent.Parent.Text == Parsing.SENSORS_NODE_NAME)
                {
                    toolStripContainer1.Visible = false;

                    string appFilePath = e.Node.Tag as string;
                    string appText = "";

                    if (!Path.IsPathRooted(appFilePath))
                    {
                        appFilePath = Path.Combine(appFilePath, appFilePath);
                    }

                    StreamReader sReader = null;

                    try
                    {
                        FileStream fileStream = new FileStream(appFilePath, FileMode.Open, FileAccess.Read);
                        sReader = new StreamReader(fileStream);
                        appText = sReader.ReadToEnd();
                    }
                    catch (DirectoryNotFoundException DNF)
                    {
                        appText = e.Node.Tag.ToString();
                    }
                    catch (FileNotFoundException FNF)
                    {
                        //throw new ComponentDefinitionFileNotFoundException(componentDefName, FileName, token);
                        appText = appFilePath;
                    }
                    finally
                    {
                        if (sReader != null)
                        {
                            sReader.Close();
                        }
                    }

                    textEditorControl.Visible = false;
                    textEditorControl.TextChanged -= textEditorControl_TextChanged;
                    textEditorControl.Text = appText;
                    textEditorControl.TextChanged += textEditorControl_TextChanged;

                    if (Canvas != null)
                        Canvas.Visible = false;

                    textEditorControl.Visible = true;

                    //}else if (e.Node.Parent != null && e.Node.Text == Parsing.NETWORK_NODE_NAME)
                }
                else if (e.Node.Parent == null
                   || (e.Node.Parent != null && e.Node.Parent.Parent == null && e.Node.Text == Parsing.SENSORS_NODE_NAME))
                {
                    toolStripContainer1.Visible = true;

                    if (Canvas != null && Canvas.Visible)
                        Canvas.Visible = false;

                    Canvas = (TreeView_Structure.Nodes[0].Tag as LTSCanvas);

                    if (Canvas != null)
                    {
                        Canvas.Visible = true;
                        Button_AddLink.Checked = false;
                        button_AddNewSensor.Checked = false;
                        Button_Delete.Enabled = Canvas.itemSelected;
                    }

                    textEditorControl.Visible = false;
                }
                else
                {
                    if (e.Node.Text == Parsing.ASSERTION_NODE_NAME)
                    {
                        toolStripContainer1.Visible = false;
                        textEditorControl.Visible = false;
                        textEditorControl.TextChanged -= textEditorControl_TextChanged;
                        if (AssertionNode.Tag != null)
                        {
                            textEditorControl.Text = AssertionNode.Tag.ToString();
                        }
                        else
                        {
                            textEditorControl.Text = "//Assertions";
                        }
                        textEditorControl.TextChanged += textEditorControl_TextChanged;

                        if (Canvas != null)
                            Canvas.Visible = false;

                        textEditorControl.Visible = true;
                    }
                }

                RaiseIsDirtyChanged();
            }
        }

        private void button_CheckStateChanged(object sender, EventArgs e)
        {
            foreach (ToolStripItem button in toolStrip1.Items)
            {
                ToolStripButton buttonTemp = button as ToolStripButton;
                if (buttonTemp != null && buttonTemp != sender)
                {
                    buttonTemp.Checked = false;
                }
            }
            if (!Button_AddLink.Checked)
            {
                Canvas.temporaryNails.Clear();
                SetDirty();
            }
        }

        #endregion

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            MenuButton_Delete.Enabled = Button_Delete.Enabled;
            MenuButton_SetInitial.Enabled = (SelectedItems.Count == 1);
            MenuButton_NewNail.Enabled = (SelectedRoute != null);
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

        private void Button_AddNewSensor_Click(object sender, EventArgs e)
        {
            PointF p = Point.Empty;
            //Add from menu bar
            if (button_AddNewSensor.Checked)
            {
                p = Canvas.LastMouseClickPosition;
            }
            else
            {
                p = Canvas.lastRightClickPosition;
            }

            int id = Canvas.StateCounter - 1;
            string nodeID = "Sensor" + id;
            StateItem classitem = new StateItem(false, nodeID);

            TreeNode newNode = SensorsNode.Nodes.Add(nodeID);
            newNode.Name = nodeID;
            newNode.Text = nodeID;
            newNode.Tag = new NodeData(nodeID, id.ToString());
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
            PointF p;

            if (Button_AddNewNail.Checked)
            {
                //Add from menubar
                route = ((CanvasRouteEventArgs)e).CanvasItem;
                p = Canvas.LastMouseClickPosition;
            }
            else
            {
                //Add from context menu
                route = SelectedRoute;
                p = Canvas.lastRightClickPosition;
            }
            if (route != null)
            {
                NailItem nailItem = new NailItem(route);
                nailItem.X = p.X;
                nailItem.Y = p.Y;

                AddCanvasItem(nailItem);
                Canvas.Refresh();
                SetDirty();
            }
        }

        private void Button_Undo_Click()
        {
            if (Canvas.currentStateIndex > 0)
            {
                Canvas.currentStateIndex--;
                XmlElement top = Canvas.undoStack[Canvas.currentStateIndex];
                Canvas.Restore(top);
                //Canvas.Refresh();
                SetDirty();
            }
        }

        private void Button_Redo_Click()
        {
            if (Canvas.currentStateIndex < Canvas.undoStack.Count - 1)
            {
                Canvas.currentStateIndex++;
                XmlElement top = Canvas.undoStack[Canvas.currentStateIndex];
                Canvas.Restore(top);
                //Canvas.Refresh();
                SetDirty();
            }
        }

        public List<CanvasItemData> SelectedItems = new List<CanvasItemData>();
        private Route SelectedRoute;

        public static bool HighLightControlUsingRed;

        private void Canvas_CanvasItemSelected(object sender, CanvasItemEventArgs e)
        {
            if (Button_AddLink.Checked)
            {
                if (SelectedItems.Count > 0 && e.CanvasItem != null)
                {
                    CanvasItemData SelectedItem = SelectedItems[0];
                    if (SelectedItem.Item is StateItem && e.CanvasItem.Item is StateItem)
                    {
                        StateItem sourceState = SelectedItem.Item as StateItem;
                        StateItem targetState = e.CanvasItem.Item as StateItem;

                        string src = sourceState.Name;
                        string tgt = targetState.Name;
                        string linkId = src + "->" + tgt;

                        if (LinkIDList != null)
                        {
                            if (LinkIDList.Contains(linkId))
                            {
                                throw new RuntimeException(
                                    "Only one link from sensor " + src + " to "
                                    + tgt + " is allowed in a network!"
                                    );
                            }
                        }

                        Route r = new Route(SelectedItem.Item, targetState);
                        r.Transition = new Transition();
                        //r.Transition.Event = "100";
                        //r.Transition.Guard = "0.9";

                        AddLink(r);
                        LinkIDList.Add(linkId);
                        SetDirty();
                    }

                    e.CanvasItem.Item.HandleMouseUp(new PointF());
                    Button_AddLink.Checked = false;
                }
                else if (SelectedItems.Count == 0 && e.CanvasItem != null)
                {
                    //Select the starting state of route
                    SelectedItems.Clear();
                    SelectedItems.Add(e.CanvasItem);
                    Canvas.temporaryNails.Add((e.CanvasItem.Item as StateItem).Center());
                }
                else if (SelectedItems.Count > 0 && e.CanvasItem == null)
                {
                    //Click on canvas to create a new nail
                    Canvas.temporaryNails.Add(Canvas.LastMouseClickPosition);
                }

            }
            else
            {
                SelectedItems.Clear();
                if (e.CanvasItem != null)
                {
                    SelectedItems.Add(e.CanvasItem);
                    e.CanvasItem.Item.HandleSelected(Canvas);
                }
                CanvasItemData SelectedItem = e.CanvasItem;
                if (SelectedItem == null && button_AddNewSensor.Checked)
                {
                    Button_AddNewSensor_Click(sender, e);
                    button_AddNewSensor.Checked = false;
                }
            }
            SelectedRoute = null;
            Button_Delete.Enabled = (SelectedItems.Count > 0);
        }

        private void Canvas_CanvasItemsSelected(object sender, CanvasItemsEventArgs e)
        {
            SelectedItems.Clear();
            SelectedItems.AddRange(e.CanvasItem);
            Button_Delete.Enabled = (SelectedItems.Count > 0);
        }

        //private void EditingLink(Transition transition, StateItem sourcePage, StateItem targetPage)
        //{
        //    LinkEditingForm form = new LinkEditingForm(transition, sourcePage.Name, targetPage.Name);
        //    if (form.ShowDialog() == DialogResult.OK)
        //    {
        //        StoreCurrentCanvas();
        //        SetDirty();
        //    }

        //    Canvas.Refresh();
        //}

        private void Canvas_ItemDoubleClick(object sender, CanvasItemEventArgs e)
        {
            if (e.CanvasItem.Item is StateItem)
            {//a sensor node is clicked
                StateItem editedState = e.CanvasItem.Item as StateItem;
                string lastName = editedState.Name;//the name of the sensor clicked on the canvas
                string lastApp = "";
                string lastRange = "";
                string lastTosID = "";
                string lastPredefVars = "";

                TreeNode sensor = null;

                //looking for the corresponding tree node in the SensorsNode Tree
                foreach (TreeNode node in SensorsNode.Nodes)
                {
                    if (node.Text == lastName)
                    {
                        sensor = node;
                        break;
                    }
                }

                if (sensor == null)
                {//the sensor in the canvas does not have a corresponding node in the SensorsNode Tree
                    //then add a new node to the SensorsNode Tree
                    sensor = SensorsNode.Nodes.Add(lastName);
                    sensor.Tag = new NodeData(lastName, "0");
                }

                //back the previous sensor property
                var sensorNodeData = (NodeData)sensor.Tag;
                lastApp = sensorNodeData.TopConfiguration;
                lastRange = sensorNodeData.SensorRanges;
                lastTosID = sensorNodeData.TOS_ID;
                lastPredefVars = sensorNodeData.PredefinedVars;


                //open a NodeEditing form to edit the properties of the sensor
                NodeEditingForm form = new NodeEditingForm(sensor, fileName);

                if (form.ShowDialog() == DialogResult.OK)
                {
                    StoreCurrentCanvas();
                    SetDirty();
                    //update the changes
                    form.UpdateData();
                    editedState.Name = sensor.Text;
                    if (!Canvas.CheckStateNameDuplicate())
                    {//the new sensor name is duplicated with some existing one
                        //restore the sensor with the previous properties
                        MessageBox.Show("Fail to update sensor because the sensor " + sensor.Name + " already exists!",
        "Error");

                        editedState.Name = lastName;
                        sensor.Text = lastName;
                        sensor.Name = lastName;
                        sensor.Tag = new NodeData(lastName, lastApp, lastRange, lastTosID, lastPredefVars);
                        editedState.Name = sensor.Text;

                    }
                    else
                    {
                        if (sensor.Text != lastName)
                        {
                            //rename the data link list
                            if (LinkIDList != null)
                            {
                                for (int i = 0; i < LinkIDList.Count; i++)
                                {
                                    string id = LinkIDList[i];

                                    if (id.StartsWith(lastName + "->"))
                                    {
                                        LinkIDList[i] = id.Replace(lastName + "->", sensor.Text + "->");
                                    }
                                    else if (id.EndsWith("->" + lastName))
                                    {
                                        LinkIDList[i] = id.Replace("->" + lastName, "->" + sensor.Text);
                                    }
                                }
                            }
                        }
                        string newApp = ((NodeData)sensor.Tag).TopConfiguration;
                        if (newApp != lastApp)
                        {
                            sensor.Nodes.Clear();

                            string appPath = newApp;
                            if (Path.IsPathRooted(appPath))
                            {
                                appPath = Path.GetDirectoryName(appPath);
                            }
                            else
                            {
                                appPath = Path.GetDirectoryName(fileName);
                            }

                            //get all files in the application path
                            var fileList = Directory.GetFiles(appPath, "*.h");
                            if (fileList != null && fileList.Length > 0)
                            {
                                Array.Sort(fileList);
                                foreach (var hFile in fileList)
                                {//add each header file (.h) to the current senser node
                                    TreeNode hNode = sensor.Nodes.Add(Path.GetFileName(hFile));
                                    hNode.Tag = hFile;
                                    hNode.Name = Path.GetFileName(hFile);
                                }
                            }

                            fileList = Directory.GetFiles(appPath, "*.nc");
                            if (fileList != null && fileList.Length > 0)
                            {//add each nesc file (.nc) to the current senser node
                                Array.Sort(fileList);
                                foreach (var ncFile in fileList)
                                {
                                    TreeNode ncNode = sensor.Nodes.Add(Path.GetFileName(ncFile));
                                    ncNode.Tag = ncFile;
                                    ncNode.Name = Path.GetFileName(ncFile);
                                }
                            }

                        }
                    }
                }

                Canvas.Refresh();
            }
        }

        private void Canvas_RouteDoubleClick(object sender, CanvasRouteEventArgs e)
        {
            //if (e.CanvasItem.From is StateItem && e.CanvasItem.To is StateItem)
            //{
            //    StateItem sourcePage = e.CanvasItem.From as StateItem;
            //    StateItem targetPage = e.CanvasItem.To as StateItem;
            //    //EditingLink(sourcePage, targetPage, e.CanvasItem);
            //    EditingLink(e.CanvasItem.Transition, sourcePage, targetPage);
            //}
        }

        [System.Obsolete("Return wrong value when the canvas is bigger than the screen. Use LTSCanvas.LastMouseClickPosition")]
        public Point GetCurrentMousePosition()
        {
            Point screenPos = MousePosition;
            return Canvas.PointToClient(screenPos);
        }


        void Canvas_CanvasRouteSelected(object sender, CanvasRouteEventArgs e)
        {
            SelectedItems.Clear();
            if (Button_AddNewNail.Checked)
            {
                Button_AddNewNail_Click(sender, e);
                Button_AddNewNail.Checked = false;

            }
            else
            {
                SelectedRoute = e.CanvasItem;
                Button_Delete.Enabled = true;
            }
        }

        private void Button_AddLink_Click(object sender, EventArgs e)
        {
            if (Button_AddLink.Checked)
            {
                if (SelectedItems.Count > 0)
                {
                    SelectedItems[0].Focused = false;
                    SelectedItems.Clear();
                }
                button_AddNewSensor.Checked = false;
                HighLightControlUsingRed = true;
            }
            else
            {
                HighLightControlUsingRed = false;
            }
        }

        private void Button_Delete_Click(object sender, EventArgs e)
        {
            if (SelectedItems.Count > 0)
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
                    string tr_id = ((StateItem)SelectedRoute.From).Name + "->" + ((StateItem)SelectedRoute.To).Name;
                    if (LinkIDList != null)
                        LinkIDList.Remove(tr_id);
                    RemoveCanvasRoute(SelectedRoute);
                    Canvas.Refresh();
                    SetDirty();
                }
            }
            Button_Delete.Enabled = false;
        }

        private void TreeView_Structure_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (LastSelectedNode != null && LastSelectedNode.Parent != null
                && LastSelectedNode.Parent.Parent != null
                && LastSelectedNode.Parent.Parent.Text == Parsing.SENSORS_NODE_NAME)
            {
                if (!(e.Node.Parent != null
                    && e.Node.Parent.Text == LastSelectedNode.Parent.Text
                    && e.Node.Parent.Parent != null
                    && e.Node.Parent.Parent.Text == Parsing.SENSORS_NODE_NAME
                    && e.Node.Text == LastSelectedNode.Text
                    && e.Node.Name == LastSelectedNode.Name))
                {
                    if (TextEditorChanged)
                    {
                        TextEditorChanged = false;
                        if (MessageBox.Show("Do you want to save the changes of " + LastSelectedNode.Name + "?",
                            "Yes", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        {
                            SaveSourceCode();
                        }

                    }

                }
            }
            else if (LastSelectedNode != null
               && LastSelectedNode.Text == Parsing.ASSERTION_NODE_NAME
               && e.Node.Text != Parsing.ASSERTION_NODE_NAME)
            {
                if (TextEditorChanged)
                {
                    TextEditorChanged = false;
                    if (MessageBox.Show("Do you want to save the changes of Assertions?",
                        "Yes", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        LastSelectedNode.Tag = textEditorControl.Text;
                    }

                }
            }

            LastSelectedNode = e.Node;

            if (e.Node.Parent != null && e.Node.Parent.Parent != null && e.Node.Parent.Parent.Text == Parsing.SENSORS_NODE_NAME)
            {
                toolStripContainer1.Visible = false;

                string appFilePath = e.Node.Tag as string;
                string appText = "";

                if (!Path.IsPathRooted(appFilePath))
                {
                    appFilePath = Path.Combine(appFilePath, appFilePath);
                }

                StreamReader sReader = null;

                try
                {
                    FileStream fileStream = new FileStream(appFilePath, FileMode.Open, FileAccess.Read);
                    sReader = new StreamReader(fileStream);
                    appText = sReader.ReadToEnd();
                }
                catch (DirectoryNotFoundException DNF)
                {
                    appText = e.Node.Tag.ToString();
                }
                catch (FileNotFoundException FNF)
                {
                    //throw new ComponentDefinitionFileNotFoundException(componentDefName, FileName, token);
                    appText = appFilePath;
                }
                finally
                {
                    if (sReader != null)
                    {
                        sReader.Close();
                    }
                }

                textEditorControl.Visible = false;
                textEditorControl.TextChanged -= textEditorControl_TextChanged;
                textEditorControl.Text = appText;
                textEditorControl.TextChanged += textEditorControl_TextChanged;

                if (Canvas != null)
                    Canvas.Visible = false;

                textEditorControl.Visible = true;

                //}else if (e.Node.Parent != null && e.Node.Text == Parsing.NETWORK_NODE_NAME)
            }
            else if (e.Node.Parent == null
               || (e.Node.Parent != null && e.Node.Parent.Parent == null && e.Node.Text == Parsing.SENSORS_NODE_NAME))
            {
                toolStripContainer1.Visible = true;

                if (Canvas != null && Canvas.Visible)
                    Canvas.Visible = false;

                Canvas = (TreeView_Structure.Nodes[0].Tag as LTSCanvas);

                if (Canvas != null)
                {
                    Canvas.Visible = true;
                    Button_AddLink.Checked = false;
                    button_AddNewSensor.Checked = false;
                    Button_Delete.Enabled = Canvas.itemSelected;
                }

                textEditorControl.Visible = false;
            }
            else
            {
                if (e.Node.Text == Parsing.ASSERTION_NODE_NAME)
                {
                    toolStripContainer1.Visible = false;
                    textEditorControl.Visible = false;
                    textEditorControl.TextChanged -= textEditorControl_TextChanged;
                    if (AssertionNode.Tag != null)
                    {
                        textEditorControl.Text = AssertionNode.Tag.ToString();
                    }
                    else
                    {
                        textEditorControl.Text = "//Assertions";
                    }
                    textEditorControl.TextChanged += textEditorControl_TextChanged;

                    if (Canvas != null)
                        Canvas.Visible = false;

                    textEditorControl.Visible = true;
                }
            }

            RaiseIsDirtyChanged();
        }

        private void MenuButton_Initial_Click(object sender, EventArgs e)
        {
            if (SelectedItems.Count > 0)
            {
                CanvasItemData SelectedItem = SelectedItems[0];
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
        //        //addSensorToolStripMenuItem.Enabled = false;
        //        //addSensorToolStripMenuItem.Visible = false;

        //        //deleteSensorToolStripMenuItem.Enabled = false;
        //        //deleteSensorToolStripMenuItem.Visible = false;

        //        //sensorDetailsToolStripMenuItem.Enabled = false;
        //        //sensorDetailsToolStripMenuItem.Visible = false;

        //        if (TreeView_Structure.SelectedNode.Text == Parsing.SENSORS_NODE_NAME)
        //        {
        //            //addSensorToolStripMenuItem.Enabled = true;
        //            //addSensorToolStripMenuItem.Visible = true;

        //        }
        //        else if (TreeView_Structure.SelectedNode.Parent != null && TreeView_Structure.SelectedNode.Parent.Text == Parsing.SENSORS_NODE_NAME)
        //        {
        //            //deleteSensorToolStripMenuItem.Visible = true;
        //            //deleteSensorToolStripMenuItem.Enabled = true;
        //            //sensorDetailsToolStripMenuItem.Visible = true;
        //            //sensorDetailsToolStripMenuItem.Enabled = true;
        //        }
        //    }
        //}

        //private void addSensorToolStripMenuItem_Click(object sender, EventArgs e)
        //{

        //    int id = 0;
        //    if(Canvas != null)
        //        id = Canvas.StateCounter - 1;

        //    string nodeID = "Sensor" + id;
        //    TreeNode node = TreeView_Structure.Nodes[0].Nodes[Parsing.SENSORS_NODE_NAME].Nodes.Add(nodeID);

        //    node.Name = nodeID;
        //    node.Text = node.Name;
        //    node.Tag = new NodeData(node.Name, id.ToString());

        //    if (Canvas != null) {
        //        PointF p = Point.Empty;
        //        //Add from menu bar
        //        if (button_AddNewSensor.Checked)
        //        {
        //            p = Canvas.LastMouseClickPosition;
        //        }
        //        else
        //        {
        //            p = Canvas.lastRightClickPosition;
        //        }

        //        StateItem classitem = new StateItem(false, node.Name);

        //        Canvas.StateCounter++;

        //        classitem.X = p.X;
        //        classitem.Y = p.Y;

        //        //classitem.Initialize();

        //        AddCanvasItem(classitem);
        //        Canvas.Refresh();

        //        if (Canvas.Visible)
        //            Canvas.Visible = false;
        //    }

        //    textEditorControl.Text = "app";

        //    textEditorControl.Visible = true;
        //    toolStripContainer1.Visible = false;

        //    TreeView_Structure.SelectedNode = node;
        //    SetDirty();
        //}

        private void Canvas_LayoutChanged(object sender, EventArgs e)
        {
            SetDirty();
        }

        protected new void SetDirty()
        {
            base.SetDirty();
            if (Canvas != null)
            {
                Canvas.RefreshPictureBox();
            }
        }

        private void Canvas_SaveCurrentCanvas(object sender, EventArgs e)
        {
            StoreCurrentCanvas();
        }

        //private void sensorDetailsToolStripMenuItem_Click(object sender, EventArgs e)
        //{
        //    if (TreeView_Structure.SelectedNode != null)
        //    {
        //        if (TreeView_Structure.SelectedNode.Parent != null && TreeView_Structure.SelectedNode.Parent.Text == Parsing.SENSORS_NODE_NAME)
        //        {
        //            TreeNode sensorNode =
        //                TreeView_Structure.SelectedNode;
        //            string lastText = sensorNode.Tag.ToString();
        //            string lastID = sensorNode.Text;

        //            LTSCanvas canvas = NetworkNode.Tag as LTSCanvas;
        //            List<CanvasItemData> items = canvas.itemsList;
        //            StateItem sensorItem = null;

        //            foreach (var item in items)
        //            {
        //                if (item.Item is StateItem)
        //                {
        //                    StateItem sitem = item.Item as StateItem;
        //                    if (sitem.Name == lastID)
        //                    {
        //                        sensorItem = item.Item as StateItem;
        //                        break;
        //                    }
        //                }
        //            }

        //            NodeEditingForm form = new NodeEditingForm(sensorNode, fileName);
        //            if (form.ShowDialog() == DialogResult.OK)
        //            {
        //                form.UpdateData();
        //                sensorItem.Name = sensorNode.Text;
        //                SetDirty();
        //                if (!CheckSensorNameDuplicate())
        //                {
        //                    sensorItem.Name = lastID;
        //                    sensorNode.Name = lastID;
        //                    sensorNode.Text = lastID;
        //                    sensorNode.Tag = lastText;
        //                }else if(sensorNode.Text != lastID)
        //                {
        //                    if (LinkIDList != null)
        //                    {
        //                        for (int i = 0; i < LinkIDList.Count; i++)
        //                        {
        //                            string id = LinkIDList[i];

        //                            if (id.StartsWith(lastID + "->"))
        //                            {
        //                                LinkIDList[i] = id.Replace(lastID + "->", sensorNode.Text + "->");
        //                            }
        //                            else if (id.EndsWith("->" + lastID))
        //                            {
        //                                LinkIDList[i] = id.Replace("->" + lastID, "->" + sensorNode.Text);
        //                            }
        //                        }
        //                    }
        //                }
        //            }

        //            if(Canvas != null && Canvas.Visible == true)
        //                Canvas.Visible = false;

        //            textEditorControl.Visible = true;

        //            string filePath = Path.GetDirectoryName(FileName);
        //            string appFilePath = (sensorNode.Tag as NodeData).TopConfiguration;

        //            string appText = "";

        //            StreamReader sReader = null;

        //            try
        //            {
        //                FileStream fileStream = new FileStream(appFilePath, FileMode.Open, FileAccess.Read);
        //                sReader = new StreamReader(fileStream);
        //                appText = sReader.ReadToEnd();
        //            }
        //            catch (FileNotFoundException FNF)
        //            {
        //                appText = appFilePath;
        //            }
        //            finally
        //            {
        //                if (sReader != null)
        //                {
        //                    sReader.Close();
        //                }
        //            }
        //            textEditorControl.Text = appText;

        //            toolStripContainer1.Visible = false;
        //        }
        //    }
        //}

        //private bool CheckSensorNameDuplicate()
        //{
        //    List<string> sensorNames = new List<string>();
        //    foreach (TreeNode node in TreeView_Structure.Nodes[0].Nodes[1].Nodes)
        //    {
        //        if (sensorNames.Contains(node.Text))
        //        {
        //            MessageBox.Show("No duplicated sensor names are allowed", "Duplicate Sensor Name", MessageBoxButtons.OK,
        //                                                                                    MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
        //            return false;
        //        }
        //        else
        //        {
        //            sensorNames.Add(node.Text);
        //        }
        //    }
        //    return true;
        //}

        //private void deleteSensorToolStripMenuItem_Click(object sender, EventArgs e)
        //{
        //    if (TreeView_Structure.SelectedNode != null)
        //    {
        //        if (TreeView_Structure.SelectedNode.Parent != null && TreeView_Structure.SelectedNode.Parent.Text == Parsing.SENSORS_NODE_NAME)
        //        {
        //            //if (TreeView_Structure.SelectedNode.Parent.Nodes.Count == 1)
        //            //{
        //            //    MessageBox.Show("At least one sensor is needed in the model.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //            //    return;
        //            //}

        //            if (MessageBox.Show("Are you sure you want to delete the selected sensor?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
        //            {
        //                string sensorID = TreeView_Structure.SelectedNode.Text;
        //                LTSCanvas canvas = NetworkNode.Tag as LTSCanvas;
        //                List<CanvasItemData> items = canvas.itemsList;

        //                foreach(var item in items)
        //                {
        //                    if (item.Item is StateItem) {
        //                        StateItem sitem = item.Item as StateItem;
        //                        if (sitem.Name == sensorID)
        //                        {
        //                            sitem.RemovedFromCanvas(canvas);
        //                            break;
        //                        }
        //                    }
        //                }

        //                TreeView_Structure.SelectedNode.Remove();

        //                toolStripContainer1.Visible = true;
        //                textEditorControl.Visible = false;
        //                Canvas.Visible = true;
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
                    return CodeEditor.EnableUndo;
                }
                if (Canvas != null && Canvas.Visible)
                {
                    return (Canvas.currentStateIndex > 0);
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
                    return CodeEditor.EnableRedo;
                }
                if (Canvas != null && Canvas.Visible)
                {
                    return (Canvas.currentStateIndex < Canvas.undoStack.Count - 1);
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
                    return EnableCut;
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
                    return EnableCopy;
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
                    return EnablePaste;// textEditorControl.ActiveTextAreaControl.TextArea.ClipboardHandler.EnablePaste;
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
                    return CodeEditor.CanSelect;
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
                CodeEditor.Undo();
            }
            else if (Canvas != null && Canvas.Visible)
            {
                Button_Undo_Click();
            }

        }

        public override void Redo()
        {
            if (textEditorControl.Visible)
            {
                CodeEditor.Redo();
            }
            else if (Canvas != null && Canvas.Visible)
            {
                Button_Redo_Click();
            }
        }

        public override void Cut()
        {

            //CodeEditor.Cut();
            textEditorControl.ActiveTextAreaControl.TextArea.ClipboardHandler.Cut(null, null);

        }

        public override void Copy()
        {

            //CodeEditor.Copy();
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
                Button_Delete_Click(null, null);
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
                if (Canvas != null && Canvas.Visible)
                {
                    return SelectedItems.Count > 0;
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

        /// <summary>
        /// To initialize an SNTabItem instance from a .sn file (in xml format)
        /// </summary>
        /// <param name="text">the text string of the source .sn file</param>
        public override void SetText(string text)
        {
            //generate an SNModel instance from the .sn source code
            SNModel sn = SNModel.LoadSensorNetworkFromXML(text);

            //initialize the LinkIDList
            if (sn.LinkList != null)
            {
                LinkIDList = new List<string>(sn.LinkList);
            }

            Canvas = sn.Network;
            Canvas.Node = TreeView_Structure.Nodes[0];
            TreeView_Structure.Nodes[0].Tag = Canvas;
            TreeView_Structure.Nodes[0].Name = sn.SystemName;

            Canvas.Dock = DockStyle.Fill;
            Canvas.ContextMenuStrip = contextMenuStrip1;
            Canvas.CanvasItemSelected += Canvas_CanvasItemSelected;
            Canvas.CanvasItemsSelected += Canvas_CanvasItemsSelected;
            Canvas.CanvasRouteSelected += Canvas_CanvasRouteSelected;
            Canvas.ItemDoubleClick += Canvas_ItemDoubleClick;
            Canvas.RouteDoubleClick += Canvas_RouteDoubleClick;
            Canvas.LayoutChanged += Canvas_LayoutChanged;
            Canvas.SaveCurrentCanvas += Canvas_SaveCurrentCanvas;


            //ProcessNode.Nodes.Add(canvas.Node);
            Canvas.undoStack.Clear();
            StoreCanvas(Canvas);

            toolStripContainer1.ContentPanel.Controls.Clear();
            //toolStripContainer1.ContentPanel.Controls.Add(Canvas);
            toolStripContainer1.ContentPanel.Controls.Add(Canvas);

            if (sn.Assertion != null)
            {
                AssertionNode.Tag = sn.Assertion;
            }
            else { AssertionNode.Tag = " "; }


            SensorsNode.Nodes.Clear();

            foreach (var sensor in sn.Sensors)
            {
                //add "sensor" to the sensor tree
                TreeNode node = SensorsNode.Nodes.Add(sensor.Key);
                node.Tag = sensor.Value;
                node.Name = sensor.Key;

                string appPath = sensor.Value.TopConfiguration;
                if (!Path.IsPathRooted(appPath))
                {
                    appPath = Path.Combine(Path.GetDirectoryName(fileName), appPath);
                }
                appPath = Path.GetDirectoryName(appPath);

                //get all files in the application path
                var fileList = Directory.GetFiles(appPath, "*.h");
                if (fileList != null && fileList.Length > 0)
                {
                    Array.Sort(fileList);
                    foreach (var hFile in fileList)
                    {//add each header file (.h) to the current senser node
                        TreeNode hNode = node.Nodes.Add(Path.GetFileName(hFile));
                        hNode.Tag = hFile;
                        hNode.Name = Path.GetFileName(hFile);
                    }
                }

                fileList = Directory.GetFiles(appPath, "*.nc");
                if (fileList != null && fileList.Length > 0)
                {//add each nesc file (.nc) to the current senser node
                    Array.Sort(fileList);
                    foreach (var ncFile in fileList)
                    {
                        TreeNode ncNode = node.Nodes.Add(Path.GetFileName(ncFile));
                        ncNode.Tag = ncFile;
                        ncNode.Name = Path.GetFileName(ncFile);
                    }
                }
            }

            toolStripContainer1.Visible = true;
            Canvas.Visible = true;
            textEditorControl.Visible = false;
        }

        public override string GetText()
        {
            XmlDocument doc = GetDoc();
            return doc.InnerXml;
        }
        public override string FileName
        {
            get
            {
                if (fileName == null)
                {
                    //string file = Path.Combine(NCExamplePath, NetworkNode.Name);
                    string file = Path.Combine(NCExamplePath, TreeView_Structure.Nodes[0].Name + ".sn");
                    return file;
                }
                return fileName;
            }
        }

        private string fileName;

        /// <summary>
        /// Save the current SNTabItem instance as a .sn file
        /// </summary>
        /// <param name="filename">file name</param>
        public override void Save(string filename)
        {
            var CurrentNode = TreeView_Structure.SelectedNode;
            if (CurrentNode.Parent != null &&
                CurrentNode.Parent.Parent != null &&
                CurrentNode.Parent.Parent.Text == Parsing.SENSORS_NODE_NAME)
            {
                if (TextEditorChanged)
                {
                    TextEditorChanged = false;
                    string appFilePath = CurrentNode.Tag as string;

                    if (File.Exists(appFilePath))
                    {
                        try
                        {
                            File.WriteAllText(@appFilePath, textEditorControl.Text);
                        }
                        catch (DirectoryNotFoundException)
                        {

                        }
                        catch (FileNotFoundException)
                        {

                        }
                    }
                }
            }
            else if (CurrentNode.Text == Parsing.ASSERTION_NODE_NAME)
            {
                if (TextEditorChanged)
                {
                    TextEditorChanged = false;
                    CurrentNode.Tag = textEditorControl.Text;
                }
            }

            if (!string.IsNullOrEmpty(filename) && filename != fileName)
            {
                fileName = filename;
                string sname = Path.GetFileNameWithoutExtension(filename);
                TreeView_Structure.Nodes[0].Name = sname;
            }

            XmlDocument doc = GetDoc();
            doc.Save(fileName);

            HaveFileName = true;
        }

        private XmlDocument GetDoc()
        {
            LTSCanvas network = TreeView_Structure.Nodes[0].Tag as LTSCanvas;

            string assertion = "";

            if (AssertionNode.Tag != null)
                assertion = AssertionNode.Tag.ToString();

            Dictionary<string, NodeData> sensors = new Dictionary<string, NodeData>();
            foreach (TreeNode node in SensorsNode.Nodes)
            {
                sensors.Add(node.Text, node.Tag as NodeData);
            }

            SNModel sn = new SNModel(network, assertion, sensors, TreeView_Structure.Nodes[0].Name);

            sn.LinkList = LinkIDList;

            return sn.GenerateXML();
        }

        public override void Open(string filename)
        {
            ToolTipText = filename;
            TabText = Path.GetFileName(filename);
            HaveFileName = true;
            fileName = filename;

            StreamReader streamReader = new StreamReader(filename);
            string text = streamReader.ReadToEnd();
            streamReader.Close();
            SetText(text);
        }

        private void Button_ExportBMP_Click(object sender, EventArgs e)
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
                    foreach (TreeNode node in SensorsNode.Nodes)
                    {
                        if (node.Text == pname)
                        {
                            TreeView_Structure.SelectedNode = node;
                            TreeView_Structure_NodeMouseDoubleClick(node,
                                new TreeNodeMouseClickEventArgs(node, MouseButtons.Left,
                                                                                                    2, 0, 0));
                            break;
                        }
                    }
                }
                else
                {

                    TreeView_Structure.SelectedNode = TreeView_Structure.Nodes[0];
                    TreeView_Structure_NodeMouseDoubleClick(TreeView_Structure.Nodes[0],
                                                            new TreeNodeMouseClickEventArgs(null, MouseButtons.Left,
                                                                                            2, 0, 0));


                    if (ex.Line >= 1 && ex.CharPositionInLine >= 0 && ex.Text != null)
                    {
                        textEditorControl.ActiveTextAreaControl.JumpTo(ex.Line - 1);
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
        //    //    Icon = Ultility.GetModuleIcon(languageName);
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
            Canvas.undoStack.RemoveRange(Canvas.currentStateIndex + 1, (Canvas.undoStack.Count - 1) - (Canvas.currentStateIndex + 1) + 1);
            Canvas.undoStack.Add(Canvas.Clone());
            Canvas.currentStateIndex = Canvas.undoStack.Count - 1;
        }

        private void StoreCanvas(LTSCanvas canvas)
        {
            canvas.undoStack.RemoveRange(canvas.currentStateIndex + 1, (canvas.undoStack.Count - 1) - (canvas.currentStateIndex + 1) + 1);
            canvas.undoStack.Add(canvas.Clone());
            canvas.currentStateIndex = canvas.undoStack.Count - 1;
        }

        private void AddCanvasItem(CanvasItem item)
        {
            Canvas.AddCanvasItem(item);
            StoreCurrentCanvas();
        }

        private void AddLink(Route route)
        {
            Canvas.AddLink(route);
            StoreCurrentCanvas();
        }

        private void RemoveCanvasItem(CanvasItem item)
        {
            Canvas.RemoveCanvasItem(item);
            StoreCurrentCanvas();
        }

        public void RemoveCanvasRoute(Route route)
        {
            Canvas.RemoveCanvasRoute(route);
            StoreCurrentCanvas();
        }

        private void RemoveMultiItems()
        {
            foreach (CanvasItemData SelectedItem in SelectedItems)
            {
                if (SelectedItem.Item is NailItem)
                {
                    Canvas.RemoveCanvasItem(SelectedItem.Item);
                }
            }

            foreach (CanvasItemData SelectedItem in SelectedItems)
            {
                if (SelectedItem.Item is Transition)
                {
                    Canvas.RemoveCanvasItem(SelectedItem.Item);
                }
            }

            foreach (CanvasItemData SelectedItem in SelectedItems)
            {
                if (SelectedItem.Item is LabelItem)
                {
                    Canvas.RemoveCanvasItem(SelectedItem.Item);
                }
            }

            foreach (CanvasItemData SelectedItem in SelectedItems)
            {
                if (SelectedItem.Item is StateItem)
                {
                    string ID = ((StateItem)SelectedItem.Item).Name;

                    foreach (TreeNode node in SensorsNode.Nodes)
                    {
                        if (node.Text == ID)
                        {
                            node.Remove();
                            break;
                        }
                    }

                    if (LinkIDList != null)
                    {

                        var newList = new List<string>();

                        foreach (var lid in LinkIDList)
                        {
                            if (lid.StartsWith(ID + "->"))
                                continue;
                            if (lid.EndsWith("->" + ID))
                                continue;
                            newList.Add(lid);
                        }

                        LinkIDList = newList;
                    }

                    Canvas.RemoveCanvasItem(SelectedItem.Item);
                }
            }
            Canvas.FinishMultiObjectAction();
            StoreCurrentCanvas();
        }
    }
}
