namespace PAT.GUI.Forms.GraphDiff
{
    partial class DifferenceDisplayForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DifferenceDisplayForm));
            this.toolStripContainer1 = new System.Windows.Forms.ToolStripContainer();
            this.Panel_Hiding = new System.Windows.Forms.Panel();
            this.Panel_MatchResult = new System.Windows.Forms.Panel();
            this.ListView_Trace = new System.Windows.Forms.ListView();
            this.columnHeader7 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader8 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader9 = new System.Windows.Forms.ColumnHeader();
            this.Button_Direction = new System.Windows.Forms.Button();
            this.DockContainer = new Fireball.Docking.DockContainer();
            this.SimulatorViewer = new Microsoft.Msagl.GraphViewerGdi.GViewer();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.Label_Processes = new System.Windows.Forms.ToolStripLabel();
            this.ComboBox_MatchType = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.ComboBox_MatchResult = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripSplitButton1 = new System.Windows.Forms.ToolStripSplitButton();
            this.Button_MatchStateStructure = new System.Windows.Forms.ToolStripMenuItem();
            this.Button_MatchProcessParameters = new System.Windows.Forms.ToolStripMenuItem();
            this.Button_MatchEventDetails = new System.Windows.Forms.ToolStripMenuItem();
            this.Button_MatchIfguardExpression = new System.Windows.Forms.ToolStripMenuItem();
            this.Button_Variables = new System.Windows.Forms.ToolStripSplitButton();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.Button_GenerateGraph = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.Button_Reset = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator7 = new System.Windows.Forms.ToolStripSeparator();
            this.Button_Settings = new System.Windows.Forms.ToolStripDropDownButton();
            this.Button_PopupDelay = new System.Windows.Forms.ToolStripMenuItem();
            this.Button_5seconds = new System.Windows.Forms.ToolStripMenuItem();
            this.Button_10seconds = new System.Windows.Forms.ToolStripMenuItem();
            this.Button_20seconds = new System.Windows.Forms.ToolStripMenuItem();
            this.Button_40seconds = new System.Windows.Forms.ToolStripMenuItem();
            this.Button_60seconds = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.Button_MatchResult = new System.Windows.Forms.ToolStripButton();
            this.ToolTip_Graph = new System.Windows.Forms.ToolTip(this.components);
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.StatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.From = new System.Windows.Forms.ColumnHeader();
            this.FromState = new System.Windows.Forms.ColumnHeader();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
            this.Event = new System.Windows.Forms.ColumnHeader();
            this.State = new System.Windows.Forms.ColumnHeader();
            this.Process = new System.Windows.Forms.ColumnHeader();
            this.Panel_ToolbarCover = new System.Windows.Forms.Panel();
            this.ImageList = new System.Windows.Forms.ImageList(this.components);
            this.statusStrip2 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel2 = new System.Windows.Forms.ToolStripStatusLabel();
            this.StatusLabel_LeftUnmatched = new System.Windows.Forms.ToolStripStatusLabel();
            this.StatusLabel_RightUnmatched = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripContainer1.BottomToolStripPanel.SuspendLayout();
            this.toolStripContainer1.ContentPanel.SuspendLayout();
            this.toolStripContainer1.TopToolStripPanel.SuspendLayout();
            this.toolStripContainer1.SuspendLayout();
            this.Panel_MatchResult.SuspendLayout();
            this.DockContainer.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.statusStrip2.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStripContainer1
            // 
            // 
            // toolStripContainer1.BottomToolStripPanel
            // 
            this.toolStripContainer1.BottomToolStripPanel.Controls.Add(this.statusStrip2);
            // 
            // toolStripContainer1.ContentPanel
            // 
            this.toolStripContainer1.ContentPanel.Controls.Add(this.Panel_Hiding);
            this.toolStripContainer1.ContentPanel.Controls.Add(this.Panel_MatchResult);
            this.toolStripContainer1.ContentPanel.Controls.Add(this.Button_Direction);
            this.toolStripContainer1.ContentPanel.Controls.Add(this.DockContainer);
            resources.ApplyResources(this.toolStripContainer1.ContentPanel, "toolStripContainer1.ContentPanel");
            resources.ApplyResources(this.toolStripContainer1, "toolStripContainer1");
            this.toolStripContainer1.Name = "toolStripContainer1";
            // 
            // toolStripContainer1.TopToolStripPanel
            // 
            this.toolStripContainer1.TopToolStripPanel.Controls.Add(this.toolStrip1);
            // 
            // Panel_Hiding
            // 
            resources.ApplyResources(this.Panel_Hiding, "Panel_Hiding");
            this.Panel_Hiding.Name = "Panel_Hiding";
            // 
            // Panel_MatchResult
            // 
            this.Panel_MatchResult.Controls.Add(this.ListView_Trace);
            resources.ApplyResources(this.Panel_MatchResult, "Panel_MatchResult");
            this.Panel_MatchResult.Name = "Panel_MatchResult";
            // 
            // ListView_Trace
            // 
            this.ListView_Trace.CheckBoxes = true;
            this.ListView_Trace.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader7,
            this.columnHeader8,
            this.columnHeader9});
            resources.ApplyResources(this.ListView_Trace, "ListView_Trace");
            this.ListView_Trace.FullRowSelect = true;
            this.ListView_Trace.GridLines = true;
            this.ListView_Trace.MultiSelect = false;
            this.ListView_Trace.Name = "ListView_Trace";
            this.ListView_Trace.OwnerDraw = true;
            this.ListView_Trace.ShowItemToolTips = true;
            this.ListView_Trace.UseCompatibleStateImageBehavior = false;
            this.ListView_Trace.View = System.Windows.Forms.View.Details;
            this.ListView_Trace.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.ListView1MouseDoubleClick);
            this.ListView_Trace.DrawColumnHeader += new System.Windows.Forms.DrawListViewColumnHeaderEventHandler(this.listView1_DrawColumnHeader);
            this.ListView_Trace.MouseClick += new System.Windows.Forms.MouseEventHandler(this.ListView1MouseClick);
            this.ListView_Trace.RetrieveVirtualItem += new System.Windows.Forms.RetrieveVirtualItemEventHandler(this.ListView1RetrieveVirtualItem);
            this.ListView_Trace.DrawSubItem += new System.Windows.Forms.DrawListViewSubItemEventHandler(this.ListView1DrawSubItem);
            // 
            // columnHeader7
            // 
            resources.ApplyResources(this.columnHeader7, "columnHeader7");
            // 
            // columnHeader8
            // 
            resources.ApplyResources(this.columnHeader8, "columnHeader8");
            // 
            // columnHeader9
            // 
            resources.ApplyResources(this.columnHeader9, "columnHeader9");
            // 
            // Button_Direction
            // 
            this.Button_Direction.FlatAppearance.BorderSize = 0;
            resources.ApplyResources(this.Button_Direction, "Button_Direction");
            this.Button_Direction.Name = "Button_Direction";
            this.ToolTip_Graph.SetToolTip(this.Button_Direction, resources.GetString("Button_Direction.ToolTip"));
            this.Button_Direction.UseVisualStyleBackColor = true;
            this.Button_Direction.Click += new System.EventHandler(this.Button_Direction_Click);
            // 
            // DockContainer
            // 
            this.DockContainer.ActiveAutoHideContent = null;
            this.DockContainer.Controls.Add(this.SimulatorViewer);
            resources.ApplyResources(this.DockContainer, "DockContainer");
            this.DockContainer.Name = "DockContainer";
            // 
            // SimulatorViewer
            // 
            this.SimulatorViewer.AsyncLayout = false;
            resources.ApplyResources(this.SimulatorViewer, "SimulatorViewer");
            this.SimulatorViewer.BackwardEnabled = true;
            this.SimulatorViewer.BuildHitTree = true;
            this.SimulatorViewer.ForwardEnabled = true;
            this.SimulatorViewer.Graph = null;
            this.SimulatorViewer.LayoutAlgorithmSettingsButtonVisible = true;
            this.SimulatorViewer.MouseHitDistance = 0.05;
            this.SimulatorViewer.Name = "SimulatorViewer";
            this.SimulatorViewer.NavigationVisible = true;
            this.SimulatorViewer.NeedToCalculateLayout = true;
            this.SimulatorViewer.PanButtonPressed = false;
            this.SimulatorViewer.SaveAsImageEnabled = true;
            this.SimulatorViewer.SaveAsMsaglEnabled = true;
            this.SimulatorViewer.SaveButtonVisible = true;
            this.SimulatorViewer.SaveGraphButtonVisible = true;
            this.SimulatorViewer.SaveInVectorFormatEnabled = true;
            this.SimulatorViewer.ToolBarIsVisible = true;
            this.SimulatorViewer.ZoomF = 1;
            this.SimulatorViewer.ZoomFraction = 0.5;
            this.SimulatorViewer.ZoomWindowThreshold = 0.05;
            this.SimulatorViewer.SelectionChanged += new System.EventHandler(this.SimulatorViewer_SelectionChanged);
            // 
            // toolStrip1
            // 
            resources.ApplyResources(this.toolStrip1, "toolStrip1");
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.Label_Processes,
            this.ComboBox_MatchType,
            this.toolStripSeparator2,
            this.toolStripLabel1,
            this.ComboBox_MatchResult,
            this.toolStripSeparator1,
            this.toolStripSplitButton1,
            this.Button_Variables,
            this.toolStripSeparator4,
            this.Button_GenerateGraph,
            this.toolStripSeparator3,
            this.Button_Reset,
            this.toolStripSeparator7,
            this.Button_Settings,
            this.toolStripSeparator5,
            this.Button_MatchResult});
            this.toolStrip1.Name = "toolStrip1";
            // 
            // Label_Processes
            // 
            this.Label_Processes.Name = "Label_Processes";
            resources.ApplyResources(this.Label_Processes, "Label_Processes");
            // 
            // ComboBox_MatchType
            // 
            this.ComboBox_MatchType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ComboBox_MatchType.Items.AddRange(new object[] {
            resources.GetString("ComboBox_MatchType.Items"),
            resources.GetString("ComboBox_MatchType.Items1")});
            this.ComboBox_MatchType.Name = "ComboBox_MatchType";
            resources.ApplyResources(this.ComboBox_MatchType, "ComboBox_MatchType");
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            resources.ApplyResources(this.toolStripSeparator2, "toolStripSeparator2");
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Name = "toolStripLabel1";
            resources.ApplyResources(this.toolStripLabel1, "toolStripLabel1");
            // 
            // ComboBox_MatchResult
            // 
            this.ComboBox_MatchResult.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ComboBox_MatchResult.Items.AddRange(new object[] {
            resources.GetString("ComboBox_MatchResult.Items"),
            resources.GetString("ComboBox_MatchResult.Items1")});
            this.ComboBox_MatchResult.Name = "ComboBox_MatchResult";
            resources.ApplyResources(this.ComboBox_MatchResult, "ComboBox_MatchResult");
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            resources.ApplyResources(this.toolStripSeparator1, "toolStripSeparator1");
            // 
            // toolStripSplitButton1
            // 
            this.toolStripSplitButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripSplitButton1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.Button_MatchStateStructure,
            this.Button_MatchProcessParameters,
            this.Button_MatchEventDetails,
            this.Button_MatchIfguardExpression});
            resources.ApplyResources(this.toolStripSplitButton1, "toolStripSplitButton1");
            this.toolStripSplitButton1.Name = "toolStripSplitButton1";
            // 
            // Button_MatchStateStructure
            // 
            this.Button_MatchStateStructure.Checked = true;
            this.Button_MatchStateStructure.CheckOnClick = true;
            this.Button_MatchStateStructure.CheckState = System.Windows.Forms.CheckState.Checked;
            this.Button_MatchStateStructure.Name = "Button_MatchStateStructure";
            resources.ApplyResources(this.Button_MatchStateStructure, "Button_MatchStateStructure");
            // 
            // Button_MatchProcessParameters
            // 
            this.Button_MatchProcessParameters.CheckOnClick = true;
            this.Button_MatchProcessParameters.Name = "Button_MatchProcessParameters";
            resources.ApplyResources(this.Button_MatchProcessParameters, "Button_MatchProcessParameters");
            // 
            // Button_MatchEventDetails
            // 
            this.Button_MatchEventDetails.CheckOnClick = true;
            this.Button_MatchEventDetails.Name = "Button_MatchEventDetails";
            resources.ApplyResources(this.Button_MatchEventDetails, "Button_MatchEventDetails");
            // 
            // Button_MatchIfguardExpression
            // 
            this.Button_MatchIfguardExpression.CheckOnClick = true;
            this.Button_MatchIfguardExpression.Name = "Button_MatchIfguardExpression";
            resources.ApplyResources(this.Button_MatchIfguardExpression, "Button_MatchIfguardExpression");
            // 
            // Button_Variables
            // 
            this.Button_Variables.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            resources.ApplyResources(this.Button_Variables, "Button_Variables");
            this.Button_Variables.Name = "Button_Variables";
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            resources.ApplyResources(this.toolStripSeparator4, "toolStripSeparator4");
            // 
            // Button_GenerateGraph
            // 
            resources.ApplyResources(this.Button_GenerateGraph, "Button_GenerateGraph");
            this.Button_GenerateGraph.Name = "Button_GenerateGraph";
            this.Button_GenerateGraph.Click += new System.EventHandler(this.Button_GenerateGraph_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            resources.ApplyResources(this.toolStripSeparator3, "toolStripSeparator3");
            // 
            // Button_Reset
            // 
            resources.ApplyResources(this.Button_Reset, "Button_Reset");
            this.Button_Reset.Name = "Button_Reset";
            this.Button_Reset.Click += new System.EventHandler(this.Button_Reset_Click);
            // 
            // toolStripSeparator7
            // 
            this.toolStripSeparator7.Name = "toolStripSeparator7";
            resources.ApplyResources(this.toolStripSeparator7, "toolStripSeparator7");
            // 
            // Button_Settings
            // 
            this.Button_Settings.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.Button_PopupDelay});
            resources.ApplyResources(this.Button_Settings, "Button_Settings");
            this.Button_Settings.Name = "Button_Settings";
            // 
            // Button_PopupDelay
            // 
            this.Button_PopupDelay.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.Button_5seconds,
            this.Button_10seconds,
            this.Button_20seconds,
            this.Button_40seconds,
            this.Button_60seconds});
            resources.ApplyResources(this.Button_PopupDelay, "Button_PopupDelay");
            this.Button_PopupDelay.Name = "Button_PopupDelay";
            // 
            // Button_5seconds
            // 
            this.Button_5seconds.Name = "Button_5seconds";
            resources.ApplyResources(this.Button_5seconds, "Button_5seconds");
            this.Button_5seconds.Tag = "5";
            this.Button_5seconds.Click += new System.EventHandler(this.secondsToolStripMenuItem_Click);
            // 
            // Button_10seconds
            // 
            this.Button_10seconds.Name = "Button_10seconds";
            resources.ApplyResources(this.Button_10seconds, "Button_10seconds");
            this.Button_10seconds.Tag = "10";
            this.Button_10seconds.Click += new System.EventHandler(this.secondsToolStripMenuItem_Click);
            // 
            // Button_20seconds
            // 
            this.Button_20seconds.Checked = true;
            this.Button_20seconds.CheckState = System.Windows.Forms.CheckState.Checked;
            this.Button_20seconds.Name = "Button_20seconds";
            resources.ApplyResources(this.Button_20seconds, "Button_20seconds");
            this.Button_20seconds.Tag = "20";
            this.Button_20seconds.Click += new System.EventHandler(this.secondsToolStripMenuItem_Click);
            // 
            // Button_40seconds
            // 
            this.Button_40seconds.Name = "Button_40seconds";
            resources.ApplyResources(this.Button_40seconds, "Button_40seconds");
            this.Button_40seconds.Tag = "40";
            this.Button_40seconds.Click += new System.EventHandler(this.secondsToolStripMenuItem_Click);
            // 
            // Button_60seconds
            // 
            this.Button_60seconds.Name = "Button_60seconds";
            resources.ApplyResources(this.Button_60seconds, "Button_60seconds");
            this.Button_60seconds.Tag = "60";
            this.Button_60seconds.Click += new System.EventHandler(this.secondsToolStripMenuItem_Click);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            resources.ApplyResources(this.toolStripSeparator5, "toolStripSeparator5");
            // 
            // Button_MatchResult
            // 
            this.Button_MatchResult.CheckOnClick = true;
            resources.ApplyResources(this.Button_MatchResult, "Button_MatchResult");
            this.Button_MatchResult.Name = "Button_MatchResult";
            this.Button_MatchResult.CheckStateChanged += new System.EventHandler(this.Button_DataStorePane_CheckStateChanged);
            // 
            // ToolTip_Graph
            // 
            this.ToolTip_Graph.AutoPopDelay = 10000000;
            this.ToolTip_Graph.InitialDelay = 500;
            this.ToolTip_Graph.ReshowDelay = 100;
            this.ToolTip_Graph.ShowAlways = true;
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.StatusLabel});
            resources.ApplyResources(this.statusStrip1, "statusStrip1");
            this.statusStrip1.Name = "statusStrip1";
            // 
            // StatusLabel
            // 
            this.StatusLabel.Name = "StatusLabel";
            resources.ApplyResources(this.StatusLabel, "StatusLabel");
            // 
            // From
            // 
            resources.ApplyResources(this.From, "From");
            // 
            // FromState
            // 
            resources.ApplyResources(this.FromState, "FromState");
            // 
            // columnHeader1
            // 
            resources.ApplyResources(this.columnHeader1, "columnHeader1");
            // 
            // columnHeader2
            // 
            resources.ApplyResources(this.columnHeader2, "columnHeader2");
            // 
            // Event
            // 
            resources.ApplyResources(this.Event, "Event");
            // 
            // State
            // 
            resources.ApplyResources(this.State, "State");
            // 
            // Process
            // 
            resources.ApplyResources(this.Process, "Process");
            // 
            // Panel_ToolbarCover
            // 
            resources.ApplyResources(this.Panel_ToolbarCover, "Panel_ToolbarCover");
            this.Panel_ToolbarCover.Name = "Panel_ToolbarCover";
            // 
            // ImageList
            // 
            this.ImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ImageList.ImageStream")));
            this.ImageList.TransparentColor = System.Drawing.Color.Transparent;
            this.ImageList.Images.SetKeyName(0, "Simulate");
            this.ImageList.Images.SetKeyName(1, "Stop");
            this.ImageList.Images.SetKeyName(2, "Play");
            // 
            // statusStrip2
            // 
            resources.ApplyResources(this.statusStrip2, "statusStrip2");
            this.statusStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel2,
            this.StatusLabel_LeftUnmatched,
            this.StatusLabel_RightUnmatched});
            this.statusStrip2.Name = "statusStrip2";
            // 
            // toolStripStatusLabel2
            // 
            this.toolStripStatusLabel2.Name = "toolStripStatusLabel2";
            resources.ApplyResources(this.toolStripStatusLabel2, "toolStripStatusLabel2");
            // 
            // StatusLabel_LeftUnmatched
            // 
            resources.ApplyResources(this.StatusLabel_LeftUnmatched, "StatusLabel_LeftUnmatched");
            this.StatusLabel_LeftUnmatched.Name = "StatusLabel_LeftUnmatched";
            // 
            // StatusLabel_RightUnmatched
            // 
            this.StatusLabel_RightUnmatched.Name = "StatusLabel_RightUnmatched";
            resources.ApplyResources(this.StatusLabel_RightUnmatched, "StatusLabel_RightUnmatched");
            // 
            // DifferenceDisplayForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.toolStripContainer1);
            this.Controls.Add(this.statusStrip1);
            this.DoubleBuffered = true;
            this.Name = "DifferenceDisplayForm";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.DifferenceDisplayForm_Load);
            this.Shown += new System.EventHandler(this.DifferenceDisplayForm_Shown);
            this.toolStripContainer1.BottomToolStripPanel.ResumeLayout(false);
            this.toolStripContainer1.BottomToolStripPanel.PerformLayout();
            this.toolStripContainer1.ContentPanel.ResumeLayout(false);
            this.toolStripContainer1.TopToolStripPanel.ResumeLayout(false);
            this.toolStripContainer1.TopToolStripPanel.PerformLayout();
            this.toolStripContainer1.ResumeLayout(false);
            this.toolStripContainer1.PerformLayout();
            this.Panel_MatchResult.ResumeLayout(false);
            this.DockContainer.ResumeLayout(false);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.statusStrip2.ResumeLayout(false);
            this.statusStrip2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel StatusLabel;
        private System.Windows.Forms.ColumnHeader From;
        private System.Windows.Forms.ColumnHeader FromState;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader Event;
        private System.Windows.Forms.ColumnHeader State;
        private System.Windows.Forms.ColumnHeader Process;
        private System.Windows.Forms.ToolStripContainer toolStripContainer1;
        private Fireball.Docking.DockContainer DockContainer;
        public Microsoft.Msagl.GraphViewerGdi.GViewer SimulatorViewer;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripLabel Label_Processes;
        private System.Windows.Forms.ToolStripButton Button_GenerateGraph;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripButton Button_Reset;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton Button_MatchResult;
        private System.Windows.Forms.Panel Panel_ToolbarCover;
        private System.Windows.Forms.ImageList ImageList;
        private System.Windows.Forms.Button Button_Direction;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolTip ToolTip_Graph;
        private System.Windows.Forms.Panel Panel_Hiding;
        private System.Windows.Forms.ToolStripDropDownButton Button_Settings;
        private System.Windows.Forms.ToolStripMenuItem Button_PopupDelay;
        private System.Windows.Forms.ToolStripMenuItem Button_5seconds;
        private System.Windows.Forms.ToolStripMenuItem Button_10seconds;
        private System.Windows.Forms.ToolStripMenuItem Button_20seconds;
        private System.Windows.Forms.ToolStripMenuItem Button_40seconds;
        private System.Windows.Forms.ToolStripMenuItem Button_60seconds;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator7;
        private System.Windows.Forms.ToolStripComboBox ComboBox_MatchType;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private System.Windows.Forms.ToolStripComboBox ComboBox_MatchResult;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.Panel Panel_MatchResult;
        private System.Windows.Forms.ListView ListView_Trace;
        private System.Windows.Forms.ColumnHeader columnHeader7;
        private System.Windows.Forms.ColumnHeader columnHeader8;
        private System.Windows.Forms.ColumnHeader columnHeader9;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripSplitButton Button_Variables;
        private System.Windows.Forms.ToolStripSplitButton toolStripSplitButton1;
        private System.Windows.Forms.ToolStripMenuItem Button_MatchProcessParameters;
        private System.Windows.Forms.ToolStripMenuItem Button_MatchEventDetails;
        private System.Windows.Forms.ToolStripMenuItem Button_MatchStateStructure;
        private System.Windows.Forms.ToolStripMenuItem Button_MatchIfguardExpression;
        private System.Windows.Forms.StatusStrip statusStrip2;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel2;
        private System.Windows.Forms.ToolStripStatusLabel StatusLabel_LeftUnmatched;
        private System.Windows.Forms.ToolStripStatusLabel StatusLabel_RightUnmatched;
    }
}