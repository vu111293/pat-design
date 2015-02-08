using System.Windows.Forms;

namespace PAT.Common.GUI
{
    partial class SimulationForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SimulationForm));
            this.toolStripContainer1 = new System.Windows.Forms.ToolStripContainer();
            this.Panel_Hiding = new System.Windows.Forms.Panel();
            this.Button_Export = new System.Windows.Forms.Button();
            this.Button_Direction = new System.Windows.Forms.Button();
            this.SplitContainer = new System.Windows.Forms.SplitContainer();
            this.Button_SimulateTrace = new System.Windows.Forms.ToolStripButton();
            this.GroupBox_EnabledEvents = new System.Windows.Forms.GroupBox();
            this.ListView_EnabledEvents = new System.Windows.Forms.ListView();
            this.columnHeader4 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader5 = new System.Windows.Forms.ColumnHeader();
            this.GroupBox_EventTrace = new System.Windows.Forms.GroupBox();
            this.ListView_Trace = new System.Windows.Forms.ListView();
            this.columnHeader10 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader7 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader8 = new System.Windows.Forms.ColumnHeader();
            this.SimulatorViewer = new Microsoft.Msagl.GraphViewerGdi.GViewer();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.Label_Processes = new System.Windows.Forms.ToolStripLabel();
            this.ComboBox_Process = new System.Windows.Forms.ToolStripComboBox();
            this.Button_Simulate = new System.Windows.Forms.ToolStripButton();
            this.Button_PlayTrace = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.Button_GenerateGraph = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.Button_Reset = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator7 = new System.Windows.Forms.ToolStripSeparator();
            this.Button_Settings = new System.Windows.Forms.ToolStripDropDownButton();
            this.Button_CounterExample = new System.Windows.Forms.ToolStripMenuItem();
            this.Button_DisplayCounterexample = new System.Windows.Forms.ToolStripMenuItem();
            this.Button_DisplaySCC = new System.Windows.Forms.ToolStripMenuItem();
            this.Button_AnimationSpeed = new System.Windows.Forms.ToolStripMenuItem();
            this.Button_SpeedVerySlow = new System.Windows.Forms.ToolStripMenuItem();
            this.Button_SpeedSlow = new System.Windows.Forms.ToolStripMenuItem();
            this.Button_SpeedNormal = new System.Windows.Forms.ToolStripMenuItem();
            this.Button_SpeedFast = new System.Windows.Forms.ToolStripMenuItem();
            this.Button_SpeedVeryFast = new System.Windows.Forms.ToolStripMenuItem();
            this.Button_PopupDelay = new System.Windows.Forms.ToolStripMenuItem();
            this.Button_5seconds = new System.Windows.Forms.ToolStripMenuItem();
            this.Button_10seconds = new System.Windows.Forms.ToolStripMenuItem();
            this.Button_20seconds = new System.Windows.Forms.ToolStripMenuItem();
            this.Button_40seconds = new System.Windows.Forms.ToolStripMenuItem();
            this.Button_60seconds = new System.Windows.Forms.ToolStripMenuItem();
            this.hideTauTransitionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.displayAllEnabledStatesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.Button_InteractionPane = new System.Windows.Forms.ToolStripButton();
            this.Button_StatePane = new System.Windows.Forms.ToolStripButton();
            this.MenuStrip_Trace = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.cpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyEventToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolTip_Graph = new System.Windows.Forms.ToolTip(this.components);
            this.Timer_Replay = new System.Windows.Forms.Timer(this.components);
            this.Timer_SimulateTrace = new System.Windows.Forms.Timer(this.components);
            this.Timer_Run = new System.Windows.Forms.Timer(this.components);
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
            this.TableLayoutPanel = new System.Windows.Forms.Panel();
            this.toolStripContainer1.ContentPanel.SuspendLayout();
            this.toolStripContainer1.TopToolStripPanel.SuspendLayout();
            this.toolStripContainer1.SuspendLayout();
            this.SplitContainer.Panel1.SuspendLayout();
            this.SplitContainer.Panel2.SuspendLayout();
            this.SplitContainer.SuspendLayout();
            this.GroupBox_EnabledEvents.SuspendLayout();
            this.GroupBox_EventTrace.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.MenuStrip_Trace.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.TableLayoutPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStripContainer1
            // 
            // 
            // toolStripContainer1.ContentPanel
            // 
            this.toolStripContainer1.ContentPanel.Controls.Add(this.Panel_Hiding);
            this.toolStripContainer1.ContentPanel.Controls.Add(this.Button_Export);
            this.toolStripContainer1.ContentPanel.Controls.Add(this.TableLayoutPanel);
            this.toolStripContainer1.ContentPanel.Controls.Add(this.Button_Direction);
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
            // Button_Export
            // 
            this.Button_Export.FlatAppearance.BorderSize = 0;
            resources.ApplyResources(this.Button_Export, "Button_Export");
            this.Button_Export.Name = "Button_Export";
            this.Button_Export.UseVisualStyleBackColor = true;
            this.Button_Export.Click += new System.EventHandler(this.Button_Export_Click);
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
            // SplitContainer
            // 
            resources.ApplyResources(this.SplitContainer, "SplitContainer");
            this.SplitContainer.Name = "SplitContainer";
            // 
            // SplitContainer.Panel1
            // 
            this.SplitContainer.Panel1.Controls.Add(this.GroupBox_EnabledEvents);
            // 
            // SplitContainer.Panel2
            // 
            this.SplitContainer.Panel2.Controls.Add(this.GroupBox_EventTrace);
            // 
            // GroupBox_EnabledEvents
            // 
            this.GroupBox_EnabledEvents.Controls.Add(this.ListView_EnabledEvents);
            resources.ApplyResources(this.GroupBox_EnabledEvents, "GroupBox_EnabledEvents");
            this.GroupBox_EnabledEvents.Name = "GroupBox_EnabledEvents";
            this.GroupBox_EnabledEvents.TabStop = false;
            // 
            // ListView_EnabledEvents
            // 
            this.ListView_EnabledEvents.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader4,
            this.columnHeader5});
            resources.ApplyResources(this.ListView_EnabledEvents, "ListView_EnabledEvents");
            this.ListView_EnabledEvents.FullRowSelect = true;
            this.ListView_EnabledEvents.GridLines = true;
            this.ListView_EnabledEvents.HideSelection = false;
            this.ListView_EnabledEvents.MultiSelect = false;
            this.ListView_EnabledEvents.Name = "ListView_EnabledEvents";
            this.ToolTip_Graph.SetToolTip(this.ListView_EnabledEvents, resources.GetString("ListView_EnabledEvents.ToolTip"));
            this.ListView_EnabledEvents.UseCompatibleStateImageBehavior = false;
            this.ListView_EnabledEvents.View = System.Windows.Forms.View.Details;
            this.ListView_EnabledEvents.DoubleClick += new System.EventHandler(this.ListView_EnabledEvents_DoubleClick);
            // 
            // columnHeader4
            // 
            resources.ApplyResources(this.columnHeader4, "columnHeader4");
            // 
            // columnHeader5
            // 
            resources.ApplyResources(this.columnHeader5, "columnHeader5");
            // 
            // GroupBox_EventTrace
            // 
            this.GroupBox_EventTrace.Controls.Add(this.ListView_Trace);
            resources.ApplyResources(this.GroupBox_EventTrace, "GroupBox_EventTrace");
            this.GroupBox_EventTrace.Name = "GroupBox_EventTrace";
            this.GroupBox_EventTrace.TabStop = false;
            // 
            // ListView_Trace
            // 
            this.ListView_Trace.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader10,
            this.columnHeader7,
            this.columnHeader8});
            resources.ApplyResources(this.ListView_Trace, "ListView_Trace");
            this.ListView_Trace.FullRowSelect = true;
            this.ListView_Trace.GridLines = true;
            this.ListView_Trace.HideSelection = false;
            this.ListView_Trace.MultiSelect = false;
            this.ListView_Trace.Name = "ListView_Trace";
            this.ToolTip_Graph.SetToolTip(this.ListView_Trace, resources.GetString("ListView_Trace.ToolTip"));
            this.ListView_Trace.UseCompatibleStateImageBehavior = false;
            this.ListView_Trace.View = System.Windows.Forms.View.Details;
            this.ListView_Trace.SelectedIndexChanged += new System.EventHandler(this.ListView_Trace_SelectedIndexChanged);
            this.ListView_Trace.MouseUp += new System.Windows.Forms.MouseEventHandler(this.ListView_Trace_MouseUp);
            // 
            // columnHeader10
            // 
            resources.ApplyResources(this.columnHeader10, "columnHeader10");
            // 
            // columnHeader7
            // 
            resources.ApplyResources(this.columnHeader7, "columnHeader7");
            // 
            // columnHeader8
            // 
            resources.ApplyResources(this.columnHeader8, "columnHeader8");
            // 
            // SimulatorViewer
            // 
            this.SimulatorViewer.AsyncLayout = false;
            resources.ApplyResources(this.SimulatorViewer, "SimulatorViewer");
            this.SimulatorViewer.Dock = DockStyle.Fill;
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
            this.ComboBox_Process,
            this.Button_Simulate,
            this.Button_PlayTrace,
             this.Button_SimulateTrace,
            this.toolStripSeparator2,
            this.Button_GenerateGraph,
            this.toolStripSeparator4,
            this.Button_Reset,
            this.toolStripSeparator7,
            this.Button_Settings,
            this.toolStripSeparator5,
            this.Button_InteractionPane,
            this.Button_StatePane});
            this.toolStrip1.Name = "toolStrip1";
            // 
            // Label_Processes
            // 
            this.Label_Processes.Name = "Label_Processes";
            resources.ApplyResources(this.Label_Processes, "Label_Processes");
            // 
            // ComboBox_Process
            // 
            this.ComboBox_Process.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ComboBox_Process.Name = "ComboBox_Process";
            resources.ApplyResources(this.ComboBox_Process, "ComboBox_Process");
            this.ComboBox_Process.SelectedIndexChanged += new System.EventHandler(this.ComboBox_Process_SelectedIndexChanged);
            // 
            // Button_Simulate
            // 
            resources.ApplyResources(this.Button_Simulate, "Button_Simulate");
            this.Button_Simulate.Name = "Button_Simulate";
            this.Button_Simulate.Click += new System.EventHandler(this.Button_Random_Click);
            // 
            // Button_PlayTrace
            // 
            resources.ApplyResources(this.Button_PlayTrace, "Button_PlayTrace");
            this.Button_PlayTrace.Name = "Button_PlayTrace";
            this.Button_PlayTrace.Click += new System.EventHandler(this.Button_Replay_Click);
            // 
            // Button_SimulateTrace
            // 
            resources.ApplyResources(this.Button_SimulateTrace, "Button_SimulateTrace");
            this.Button_SimulateTrace.Name = "Button_SimulateTrace";
            this.Button_SimulateTrace.Click += new System.EventHandler(this.Button_SimulateTrace_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            resources.ApplyResources(this.toolStripSeparator2, "toolStripSeparator2");
            // 
            // Button_GenerateGraph
            // 
            resources.ApplyResources(this.Button_GenerateGraph, "Button_GenerateGraph");
            this.Button_GenerateGraph.Name = "Button_GenerateGraph";
            this.Button_GenerateGraph.Click += new System.EventHandler(this.Button_GenerateGraph_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            resources.ApplyResources(this.toolStripSeparator4, "toolStripSeparator4");
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
            this.Button_CounterExample,
            this.Button_AnimationSpeed,
            this.Button_PopupDelay,
            this.hideTauTransitionToolStripMenuItem,
            this.displayAllEnabledStatesToolStripMenuItem});
            resources.ApplyResources(this.Button_Settings, "Button_Settings");
            this.Button_Settings.Name = "Button_Settings";
            // 
            // Button_CounterExample
            // 
            this.Button_CounterExample.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.Button_DisplayCounterexample,
            this.Button_DisplaySCC});
            resources.ApplyResources(this.Button_CounterExample, "Button_CounterExample");
            this.Button_CounterExample.Name = "Button_CounterExample";
            // 
            // Button_DisplayCounterexample
            // 
            this.Button_DisplayCounterexample.Checked = true;
            this.Button_DisplayCounterexample.CheckOnClick = true;
            this.Button_DisplayCounterexample.CheckState = System.Windows.Forms.CheckState.Checked;
            this.Button_DisplayCounterexample.Name = "Button_DisplayCounterexample";
            resources.ApplyResources(this.Button_DisplayCounterexample, "Button_DisplayCounterexample");
            this.Button_DisplayCounterexample.Click += new System.EventHandler(this.hignlightCounterexampleToolStripMenuItem_Click);
            // 
            // Button_DisplaySCC
            // 
            this.Button_DisplaySCC.CheckOnClick = true;
            this.Button_DisplaySCC.Name = "Button_DisplaySCC";
            resources.ApplyResources(this.Button_DisplaySCC, "Button_DisplaySCC");
            this.Button_DisplaySCC.Click += new System.EventHandler(this.hignlightCounterexampleToolStripMenuItem_Click);
            // 
            // Button_AnimationSpeed
            // 
            this.Button_AnimationSpeed.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.Button_SpeedVerySlow,
            this.Button_SpeedSlow,
            this.Button_SpeedNormal,
            this.Button_SpeedFast,
            this.Button_SpeedVeryFast});
            resources.ApplyResources(this.Button_AnimationSpeed, "Button_AnimationSpeed");
            this.Button_AnimationSpeed.Name = "Button_AnimationSpeed";
            // 
            // Button_SpeedVerySlow
            // 
            this.Button_SpeedVerySlow.Name = "Button_SpeedVerySlow";
            resources.ApplyResources(this.Button_SpeedVerySlow, "Button_SpeedVerySlow");
            this.Button_SpeedVerySlow.Tag = "4";
            this.Button_SpeedVerySlow.Click += new System.EventHandler(this.toolStripMenuItem_SpeedVerySlow_Click_1);
            // 
            // Button_SpeedSlow
            // 
            this.Button_SpeedSlow.Name = "Button_SpeedSlow";
            resources.ApplyResources(this.Button_SpeedSlow, "Button_SpeedSlow");
            this.Button_SpeedSlow.Tag = "2";
            this.Button_SpeedSlow.Click += new System.EventHandler(this.toolStripMenuItem_SpeedVerySlow_Click_1);
            // 
            // Button_SpeedNormal
            // 
            this.Button_SpeedNormal.Checked = true;
            this.Button_SpeedNormal.CheckState = System.Windows.Forms.CheckState.Checked;
            this.Button_SpeedNormal.Name = "Button_SpeedNormal";
            resources.ApplyResources(this.Button_SpeedNormal, "Button_SpeedNormal");
            this.Button_SpeedNormal.Tag = "1";
            this.Button_SpeedNormal.Click += new System.EventHandler(this.toolStripMenuItem_SpeedVerySlow_Click_1);
            // 
            // Button_SpeedFast
            // 
            this.Button_SpeedFast.Name = "Button_SpeedFast";
            resources.ApplyResources(this.Button_SpeedFast, "Button_SpeedFast");
            this.Button_SpeedFast.Tag = "0.5";
            this.Button_SpeedFast.Click += new System.EventHandler(this.toolStripMenuItem_SpeedVerySlow_Click_1);
            // 
            // Button_SpeedVeryFast
            // 
            this.Button_SpeedVeryFast.Name = "Button_SpeedVeryFast";
            resources.ApplyResources(this.Button_SpeedVeryFast, "Button_SpeedVeryFast");
            this.Button_SpeedVeryFast.Tag = "0.25";
            this.Button_SpeedVeryFast.Click += new System.EventHandler(this.toolStripMenuItem_SpeedVerySlow_Click_1);
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
            // hideTauTransitionToolStripMenuItem
            // 
            this.hideTauTransitionToolStripMenuItem.CheckOnClick = true;
            this.hideTauTransitionToolStripMenuItem.Name = "hideTauTransitionToolStripMenuItem";
            resources.ApplyResources(this.hideTauTransitionToolStripMenuItem, "hideTauTransitionToolStripMenuItem");
            this.hideTauTransitionToolStripMenuItem.CheckStateChanged += new System.EventHandler(this.hideTauTransitionToolStripMenuItem_CheckStateChanged);
            //
            // displayAllEnabledStatesToolStripMenuItem
            // 
            this.displayAllEnabledStatesToolStripMenuItem.CheckOnClick = true;
            this.displayAllEnabledStatesToolStripMenuItem.Checked = true;
            this.displayAllEnabledStatesToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.displayAllEnabledStatesToolStripMenuItem.Name = "displayAllEnabledStatesToolStripMenuItem";
            resources.ApplyResources(this.displayAllEnabledStatesToolStripMenuItem, "displayAllEnabledStatesToolStripMenuItem");
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            resources.ApplyResources(this.toolStripSeparator5, "toolStripSeparator5");
            // 
            // Button_InteractionPane
            // 
            this.Button_InteractionPane.CheckOnClick = true;
            resources.ApplyResources(this.Button_InteractionPane, "Button_InteractionPane");
            this.Button_InteractionPane.Name = "Button_InteractionPane";
            this.Button_InteractionPane.CheckStateChanged += new System.EventHandler(this.Button_EventPane_CheckStateChanged);
            // 
            // Button_StatePane
            // 
            this.Button_StatePane.CheckOnClick = true;
            resources.ApplyResources(this.Button_StatePane, "Button_StatePane");
            this.Button_StatePane.Name = "Button_StatePane";
            this.Button_StatePane.CheckStateChanged += new System.EventHandler(this.Button_State_CheckStateChanged);
            // 
            // MenuStrip_Trace
            // 
            this.MenuStrip_Trace.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem1,
            this.toolStripSeparator1,
            this.cpToolStripMenuItem,
            this.copyEventToolStripMenuItem,
            this.toolStripSeparator6,
            this.saveToolStripMenuItem});
            this.MenuStrip_Trace.Name = "MenuStrip_Trace";
            resources.ApplyResources(this.MenuStrip_Trace, "MenuStrip_Trace");
            // 
            // toolStripMenuItem1
            // 
            resources.ApplyResources(this.toolStripMenuItem1, "toolStripMenuItem1");
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Click += new System.EventHandler(this.toolStripMenuItem1_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            resources.ApplyResources(this.toolStripSeparator1, "toolStripSeparator1");
            // 
            // cpToolStripMenuItem
            // 
            this.cpToolStripMenuItem.Name = "cpToolStripMenuItem";
            resources.ApplyResources(this.cpToolStripMenuItem, "cpToolStripMenuItem");
            this.cpToolStripMenuItem.Click += new System.EventHandler(this.cpToolStripMenuItem_Click);
            // 
            // copyEventToolStripMenuItem
            // 
            this.copyEventToolStripMenuItem.Name = "copyEventToolStripMenuItem";
            resources.ApplyResources(this.copyEventToolStripMenuItem, "copyEventToolStripMenuItem");
            this.copyEventToolStripMenuItem.Click += new System.EventHandler(this.copyEventToolStripMenuItem_Click);
            // 
            // toolStripSeparator6
            // 
            this.toolStripSeparator6.Name = "toolStripSeparator6";
            resources.ApplyResources(this.toolStripSeparator6, "toolStripSeparator6");
            // 
            // saveToolStripMenuItem
            // 
            resources.ApplyResources(this.saveToolStripMenuItem, "saveToolStripMenuItem");
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
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
            // TableLayoutPanel
            // 
            resources.ApplyResources(this.TableLayoutPanel, "TableLayoutPanel");
            this.TableLayoutPanel.Name = "TableLayoutPanel";
            // 
            // SimulationForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.toolStripContainer1);
            this.Controls.Add(this.statusStrip1);
            this.DoubleBuffered = true;
            this.Name = "SimulationForm";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SimulationForm_FormClosing);
            this.toolStripContainer1.ContentPanel.ResumeLayout(false);
            this.toolStripContainer1.TopToolStripPanel.ResumeLayout(false);
            this.toolStripContainer1.TopToolStripPanel.PerformLayout();
            this.toolStripContainer1.ResumeLayout(false);
            this.toolStripContainer1.PerformLayout();
            this.SplitContainer.Panel1.ResumeLayout(false);
            this.SplitContainer.Panel2.ResumeLayout(false);
            this.SplitContainer.ResumeLayout(false);
            this.GroupBox_EnabledEvents.ResumeLayout(false);
            this.GroupBox_EventTrace.ResumeLayout(false);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.MenuStrip_Trace.ResumeLayout(false);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.TableLayoutPanel.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ContextMenuStrip MenuStrip_Trace;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.Timer Timer_Replay;
        private System.Windows.Forms.Timer Timer_SimulateTrace;
        private System.Windows.Forms.Timer Timer_Run;
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
        private System.Windows.Forms.SplitContainer SplitContainer;
        private System.Windows.Forms.GroupBox GroupBox_EnabledEvents;
        private System.Windows.Forms.ListView ListView_EnabledEvents;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.GroupBox GroupBox_EventTrace;
        private System.Windows.Forms.ListView ListView_Trace;
        private System.Windows.Forms.ColumnHeader columnHeader7;
        private System.Windows.Forms.ColumnHeader columnHeader8;
        public Microsoft.Msagl.GraphViewerGdi.GViewer SimulatorViewer;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripLabel Label_Processes;
        private System.Windows.Forms.ToolStripComboBox ComboBox_Process;
        private System.Windows.Forms.ToolStripButton Button_GenerateGraph;
        private System.Windows.Forms.ToolStripButton Button_Simulate;
        private System.Windows.Forms.ToolStripButton Button_Reset;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton Button_InteractionPane;
        private System.Windows.Forms.Panel Panel_ToolbarCover;
        private System.Windows.Forms.ToolStripButton Button_PlayTrace;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ImageList ImageList;
        private System.Windows.Forms.Button Button_Direction;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolTip ToolTip_Graph;
        private System.Windows.Forms.ToolStripMenuItem copyEventToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cpToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator6;
        private System.Windows.Forms.Panel Panel_Hiding;
        private System.Windows.Forms.ToolStripDropDownButton Button_Settings;
        private System.Windows.Forms.ToolStripMenuItem Button_PopupDelay;
        private System.Windows.Forms.ToolStripMenuItem Button_5seconds;
        private System.Windows.Forms.ToolStripMenuItem Button_10seconds;
        private System.Windows.Forms.ToolStripMenuItem Button_20seconds;
        private System.Windows.Forms.ToolStripMenuItem Button_AnimationSpeed;
        private System.Windows.Forms.ToolStripMenuItem Button_SpeedVerySlow;
        private System.Windows.Forms.ToolStripMenuItem Button_SpeedSlow;
        private System.Windows.Forms.ToolStripMenuItem Button_SpeedNormal;
        private System.Windows.Forms.ToolStripMenuItem Button_SpeedFast;
        private System.Windows.Forms.ToolStripMenuItem Button_SpeedVeryFast;
        private System.Windows.Forms.ToolStripMenuItem Button_40seconds;
        private System.Windows.Forms.ToolStripMenuItem Button_60seconds;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator7;
        private System.Windows.Forms.ColumnHeader columnHeader10;
        private System.Windows.Forms.ToolStripMenuItem hideTauTransitionToolStripMenuItem;
        private System.Windows.Forms.ToolStripButton Button_StatePane;
        private System.Windows.Forms.ToolStripMenuItem Button_CounterExample;
        private System.Windows.Forms.ToolStripMenuItem Button_DisplayCounterexample;
        private System.Windows.Forms.ToolStripMenuItem Button_DisplaySCC;
        private System.Windows.Forms.Button Button_Export;
        private System.Windows.Forms.Panel TableLayoutPanel;
        private ToolStripButton Button_SimulateTrace;
        private ToolStripMenuItem displayAllEnabledStatesToolStripMenuItem;
    }
}