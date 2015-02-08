using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Fireball.Docking;
using Microsoft.Msagl.Drawing;
using PAT.Common.Classes.Assertion;
using PAT.Common.Classes.Expressions.ExpressionClass;
using PAT.Common.Classes.ModuleInterface;
using PAT.Common.Classes.SemanticModels.LTS.Assertion;
using PAT.Common.GUI.Simulator;
using Color = Microsoft.Msagl.Drawing.Color;
using PAT.Common.Classes.LTS;


namespace PAT.Common.GUI
{
    public partial class SimulationForm : Form, ResourceFormInterface
    {
        public const string INITIAL_STATE = "INIT-STATE";

        //localized string for the button text display
        private System.ComponentModel.ComponentResourceManager resources;
        private string STOP;
        private string PLAY_TRACE;
        private string SIMULATE_TRACE;
        private string SIMULATE;
        private string GENERATE_GRAPH;

        public SpecificationBase Spec;
        public Graph g;
        private LayerDirection Direction = LayerDirection.TB;

        private List<EventStepSim> CurrentEnableEventList = new List<EventStepSim>(100);
        public List<string> Traces = new List<string>(32);
        public Dictionary<string, ProcessData> Mapping = new Dictionary<string, ProcessData>(10000);
        private string InitialState;

        public Hashtable visited = new Hashtable(10000);
        private bool WarningFlag = false;
        private double AnimationSpeed = 1;
        private int ToolTipDisplayTime = 20000;
        private DockableWindow StatePane;
        private StateInfoControl StateInfoControl;

        private Fireball.Docking.DockContainer DockContainer;

        private DockableWindow EventWindow;

        //assertion stored the counter example information
        private AssertionBase Assertion;

        private Control SimulatorViewerDockWindow;

        public bool HideTauTransition = false;


        //====================Liuyang: 23, Jan, 2010============================
        //a new simulation datastore is created for each simulation form;
        //where the explicity ID is used rather than hashed ID, which can help user to understand more.
        //private DataStoreSimulation DataStoreSimulation = new DataStoreSimulation();
        //====================Liuyang: 23, Jan, 2010============================

        public SimulationForm()
        {
            InitializeComponent();

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
            resources = new System.ComponentModel.ComponentResourceManager(typeof (SimulationForm));


            if (Common.Ultility.Ultility.IsUnixOS)
            {
                StateInfoControl = new StateInfoControl();
                StateInfoControl.Dock = DockStyle.Fill;
                SplitContainer.Dock = DockStyle.Right;
                this.StateInfoControl.Dock = DockStyle.Left;

                this.TableLayoutPanel.Controls.Add(this.SimulatorViewer);
                this.TableLayoutPanel.Controls.Add(this.SplitContainer);
                this.TableLayoutPanel.Controls.Add(this.StateInfoControl);

                InitializeResourceText();

                Timer_Run.Tick += new EventHandler(Timer_Drawing_Tick);
                Timer_Replay.Tick += new EventHandler(Timer_Replay_Tick);
                Timer_SimulateTrace.Tick += new EventHandler(Timer_SimulateTrace_Tick);


                //show the event window by default.
                Button_InteractionPane.Checked = true;

                Panel_ToolbarCover.BringToFront();

                this.ListView_EnabledEvents.OwnerDraw = true;
                this.ListView_EnabledEvents.DrawItem += new DrawListViewItemEventHandler(ListView_EnabledEvents_DrawItem);
                ListView_EnabledEvents.DrawColumnHeader +=
                    new DrawListViewColumnHeaderEventHandler(ListView_EnabledEvents_DrawColumnHeader);


                SimulatorViewer.Resize += new EventHandler(SimulatorViewer_Resize);
                Button_StatePane.Checked = true;
                StatusLabel.Text = Resources.Ready;

            }
            else
            {
                this.DockContainer = new Fireball.Docking.DockContainer();
                // 
                // DockContainer
                // 
                this.DockContainer.ActiveAutoHideContent = null;
                resources.ApplyResources(this.DockContainer, "DockContainer");
                //this.DockContainer.Name = "DockContainer";


                this.toolStripContainer1.ContentPanel.Controls.Add(this.DockContainer);
                //toolStripContainer1.ContentPanel.Controls.Add(this.gra)
                //toolStripContainer1.ContentPanel.Controls.Remove(this.TableLayoutPanel);
                this.TableLayoutPanel.Visible = false;

                //initialize the event window
                this.toolStripContainer1.ContentPanel.Controls.Remove(SplitContainer);
                SplitContainer.Dock = DockStyle.Fill;
                EventWindow = new DockableWindow();
                EventWindow.DockableAreas = DockAreas.DockRight | DockAreas.DockLeft | DockAreas.Float;
                EventWindow.CloseButton = false;
                EventWindow.Controls.Add(SplitContainer);


                //initialize the data store window.
                StatePane = new DockableWindow();
                StatePane.DockableAreas = DockAreas.DockLeft | DockAreas.DockRight | DockAreas.Float;
                StateInfoControl = new StateInfoControl();
                StateInfoControl.Dock = DockStyle.Fill;
                StatePane.Controls.Add(StateInfoControl);
                StatePane.CloseButton = false;

                InitializeResourceText();

                Timer_Run.Tick += new EventHandler(Timer_Drawing_Tick);
                Timer_Replay.Tick += new EventHandler(Timer_Replay_Tick);
                Timer_SimulateTrace.Tick += new EventHandler(Timer_SimulateTrace_Tick);

                //show the event window by default.
                Button_InteractionPane.Checked = true;

                //release the simulator viewer from the dockContainer
                DockContainer.Controls.Remove(SimulatorViewer);

                //Create the dummy DocumentTab to initialize the Document DockWindow in a proper way
                //actually, there should be a better way to do this by initialize the Document DockWindow directly.
                //since we are not sure about the how the Dock control is implemented, so we play this trick here.
                DockContainer.DocumentStyle = DocumentStyles.DockingWindow;
                DockableWindow SimulatorViewerTab = new DockableWindow();
                SimulatorViewerTab.Show(DockContainer, DockState.Document);

                //show the simulator viewer in the SimulatorViewerDockWindow
                SimulatorViewerDockWindow = SimulatorViewerTab.Parent.Parent;
                SimulatorViewerDockWindow.Controls.Clear();
                SimulatorViewerDockWindow.Controls.Add(SimulatorViewer);
                SimulatorViewerDockWindow.Controls.Add(Panel_ToolbarCover);

                Panel_ToolbarCover.BringToFront();

                this.ListView_EnabledEvents.OwnerDraw = true;
                this.ListView_EnabledEvents.DrawItem += new DrawListViewItemEventHandler(ListView_EnabledEvents_DrawItem);
                ListView_EnabledEvents.DrawColumnHeader +=
                    new DrawListViewColumnHeaderEventHandler(ListView_EnabledEvents_DrawColumnHeader);


                SimulatorViewer.Resize += new EventHandler(SimulatorViewer_Resize);
                Button_StatePane.Checked = true;
                StatusLabel.Text = Resources.Ready;

                //Common.Ultility.Ultility.SimulationForms.Add(this);
            }

            //SimulatorViewer.Invalidate();
            SimulatorViewer.Refresh();
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

        private void SimulatorViewer_Resize(object sender, EventArgs e)
        {
            try
            {
                int offset = 0;

                if (Common.Ultility.Ultility.IsUnixOS)
                {
                    offset = 285;
                }
                Panel_Hiding.Left = this.SimulatorViewer.Parent.Left + offset;
                Button_Direction.Left = this.SimulatorViewer.Parent.Left + 277 + offset;
                Button_Export.Left = this.SimulatorViewer.Parent.Left + 278 + Button_Direction.Width + offset;
            }
            catch (Exception)
            {

            }
        }


        public void InitializeResourceText()
        {
            STOP = resources.GetString("Button_Stop.Text") ?? "Stop";
            PLAY_TRACE = resources.GetString("Button_PlayTrace.Text") ?? "Play Trace";
            SIMULATE_TRACE = resources.GetString("Button_SimulateTrace.Text") ?? "Simulate Trace";
            SIMULATE = resources.GetString("Button_Simulate.Text") ?? "Simulate";
            GENERATE_GRAPH = resources.GetString("Button_GenerateGraph.Text") ?? "Generate Graph";

            if (EventWindow != null)
            {
                EventWindow.Text = resources.GetString("InteractionPanel") ?? "Event and Trace Window";
                EventWindow.Icon =
                    Icon.FromHandle(((Bitmap) resources.GetObject("Button_InteractionPane.Image")).GetHicon());
                ;
            }

            if (StatePane != null)
            {
                StatePane.Text = resources.GetString("StatePane") ?? "State Process Window";
                StatePane.Icon = Icon.FromHandle(((Bitmap) resources.GetObject("Button_StatePane.Image")).GetHicon());
            }
        }


        private void ListView_EnabledEvents_DrawColumnHeader(object sender, DrawListViewColumnHeaderEventArgs e)
        {
            e.Graphics.FillRectangle(Brushes.GreenYellow, e.Bounds);
            e.DrawText();
            e.DrawDefault = true;
        }

        private void ListView_EnabledEvents_DrawItem(object sender, DrawListViewItemEventArgs e)
        {
            Rectangle foo = e.Bounds;
            foo.Offset(-10, 0);
            e.Graphics.FillRectangle(new SolidBrush(e.Item.BackColor), foo);
            e.DrawDefault = true;
        }

        public void SetSpec(string name, SpecificationBase spec)
        {

            this.Spec = spec;
            this.ComboBox_Process.Items.Clear();

            List<string> names = Spec.GetProcessNames();
            foreach (string s in names)
            {
                this.ComboBox_Process.Items.Add(s);
            }

            if (ComboBox_Process.Items.Count > 0)
            {
                ComboBox_Process.SelectedIndex = 0;
            }

            if (name != "")
            {
#if DEBUG
                this.Text = this.Text + " (Debug Model) - " + name; // +PAT.CSP.Ultility.Ultility.GetVersionNumber();
#else
             this.Text = this.Text + " - " + name;
// +PAT.CSP.Ultility.Ultility.GetVersionNumber();
#endif

            }
        }




        #region Counter Example Simuation

        public void SetSpec(string name, string simulateProcess, SpecificationBase spec, AssertionBase assertion)
        {
            this.Spec = spec;
            Assertion = assertion;

            this.ComboBox_Process.Items.Clear();
            this.ComboBox_Process.Items.Add(simulateProcess);
            ComboBox_Process.SelectedIndexChanged -= ComboBox_Process_SelectedIndexChanged;
            ComboBox_Process.SelectedIndex = 0;
            ComboBox_Process.SelectedIndexChanged += ComboBox_Process_SelectedIndexChanged;

            if (name != "")
            {
                this.Text = this.Text + " - " + name; // +PAT.CSP.Ultility.Ultility.GetVersionNumber();
            }

            DisplayCounterExample();
        }

        private void DisplayCounterExample()
        {
            if (CanGrabLock())
            {

                DisableControls();
                try
                {
                    int LoopIndex = 0;
                    if (Assertion is AssertionLTL && Assertion.VerificationOutput.LoopIndex >= 0)
                    {
                        LoopIndex = Assertion.VerificationOutput.LoopIndex;
                    }

                    SimulationInitialize(false);

                    this.CurrentEnableEventList.Clear();
                    this.ListView_EnabledEvents.Items.Clear();

                    if (Button_DisplayCounterexample.Checked)
                    {
                        string sourceProcess = InitialState;

                        for (int j = 1; j < Assertion.VerificationOutput.CounterExampleTrace.Count; j++)
                        {
                            Node n = null;
                            Edge edge = null;

                            //EventStepSim fromStep = CounterExampleTrace[j-1];
                            ConfigurationBase stepOld = Assertion.VerificationOutput.CounterExampleTrace[j];

                            EventStepSim step = new EventStepSim(stepOld);
                            step.SourceProcess = sourceProcess;

                            string stepString;

                            if (step.StepVisited(visited, out stepString))
                            {
                                //take out the duplicated edges
                                //bool hasMatch = false;
                                Edge matchEdge = null;

                                foreach (Edge e1 in g.Edges)
                                {
                                    if (e1.Source == step.SourceProcess && e1.Target == stepString &&
                                        e1.LabelText == step.Event && e1.Attr.Color == Color.DarkOrange)
                                    {
                                        //hasMatch = true;
                                        matchEdge = e1;
                                        break;
                                    }
                                }

                                if (matchEdge == null)
                                {
                                    edge = g.AddEdge(step.SourceProcess, step.Event, stepString);
                                    n = g.NodeMap[stepString] as Node;

                                    AddToTrace(edge.SourceNode.LabelText, step, edge.TargetNode.LabelText, stepString);
                                }
                                else
                                {
                                    edge = matchEdge;
                                    n = edge.TargetNode;
                                    AddToTrace(edge.SourceNode.LabelText, step, edge.TargetNode.LabelText, stepString);

                                    sourceProcess = stepString;
                                    //continue;
                                }
                            }
                            else
                            {
                                visited.Add(stepString, null);

                                //add the new node and set it to red.
                                n = g.AddNode(stepString);
                                n.LabelText = (g.NodeCount - 1).ToString();
                                n.UserData = step; //// step.ToFullString();

                                edge = g.AddEdge(step.SourceProcess, step.Event, stepString);

                                AddToTrace(edge.SourceNode.LabelText, step, n.LabelText, stepString);
                            }

                            sourceProcess = stepString;
                            string key = GetTraceEvent(this.ListView_Trace.Items.Count) + stepString;

                            //states in SCC are yellow colored.
                            if (LoopIndex > 0 && j >= LoopIndex)
                            {
                                edge.Label.FontColor = Color.DarkOrange;
                                edge.Attr.Color = Color.DarkOrange;
                                n.Attr.FillColor = Color.Yellow;
                            }
                            else
                            {
                                //change all nodes to white node!
                                foreach (Node mapN in g.NodeMap.Values)
                                {
                                    if (mapN.Attr.FillColor == Color.Red)
                                    {
                                        mapN.Attr.FillColor = Color.White;
                                    }
                                }

                                //the last node of the path is inside SCC, which should be in yellow color!
                                if (LoopIndex > 0 && j == LoopIndex - 1)
                                {
                                    n.Attr.FillColor = Color.Yellow;
                                }
                                else
                                {
                                    n.Attr.FillColor = Color.Red;
                                }
                            }

                            if (!Mapping.ContainsKey(key))
                            {
                                Mapping.Add(key,
                                            new ProcessData(step, CloneGraph(g), CloneEnabledEvent()));
                            }

                            UpdateStore(step);
                        }

                        SimulatorViewer.Graph = g;
                        SimulatorViewer.Invalidate();
                    }
                    //else
                    //{
                    //    List<string> FairSCC = (Assertion as AssertionLTL).FairSCC;
                    //    Dictionary<int, EventBAPair> SCC2EventStepMapping = (Assertion as AssertionLTL).SCC2EventStepMapping;
                    //    Dictionary<int, List<int>> OutgoingTransitionTable =
                    //        (Assertion as AssertionLTL).OutgoingTransitionTable;

                    //    string sourceProcess = InitialState;
                    //    //print out the prefix path, and mark the last node as green color (inside SCC)
                    //    for (int j = 1; j < LoopIndex; j++)
                    //    {
                    //        AddCounterExamplePrefixPathNode(ref sourceProcess, Assertion.VerificationOutput.CounterExampleTrace[j],
                    //                                        j == LoopIndex - 1);
                    //    }

                    //    for (int i = 0; i < FairSCC.Count; i++)
                    //    {
                    //        Node n = null;
                    //        Edge edge = null;

                    //        //get sourceProcess of the current SCC node
                    //        ConfigurationBase stepSJ = SCC2EventStepMapping[FairSCC[i]].configuration;
                    //        EventStepSim stepSJSJ = new EventStepSim(stepSJ);
                    //        stepSJSJ.StepVisited(visited, out sourceProcess);

                    //        //check all the transitions
                    //        List<int> outgoing = OutgoingTransitionTable[FairSCC[i]];
                    //        for (int j = 0; j < outgoing.Count; j++)
                    //        {
                    //            int w = outgoing[j];
                    //            if (FairSCC.Contains(w))
                    //            {
                    //                ConfigurationBase stepOld = SCC2EventStepMapping[w].configuration;

                    //                EventStepSim step = new EventStepSim(stepOld); //.Process, stepOld.Event, stepOld.HidenEvent, stepOld.GlobalEnv
                    //                step.SourceProcess = sourceProcess;

                    //                string stepString;
                    //                //string nextState = step.StepToString;

                    //                if (step.StepVisited(visited, out stepString))
                    //                {
                    //                    //ignore the transition which is in side the graph already!
                    //                    bool hasMatch = false;
                    //                    foreach (Edge e1 in g.Edges)
                    //                    {
                    //                        if (e1.Source == sourceProcess && e1.Target == stepString &&
                    //                            e1.LabelText == step.Event && e1.Attr.Color == Color.DarkGreen)
                    //                        {
                    //                            hasMatch = true;
                    //                            break;
                    //                        }
                    //                    }

                    //                    if (!hasMatch)
                    //                    {
                    //                        //liuyang added code for the bug start
                    //                        //this is really a special case, where the hidden events are different for the last tau step
                    //                        if (step.Config.Event == Constants.TAU)
                    //                        {
                    //                            //ConfigurationBase[] steps = CounterExampleTrace[i - 1].MakeOneMove();
                    //                            List<EventStepSim> steps = ((g.NodeMap[sourceProcess] as Node).UserData as EventStepSim).MakeOneMove(false);
                    //                            foreach (EventStepSim istep in steps)
                    //                            {
                    //                                if (istep.StepID == stepString)
                    //                                {
                    //                                    step = istep;
                    //                                    break;
                    //                                }
                    //                            }
                    //                        }

                    //                        edge = g.AddEdge(sourceProcess, step.Event, stepString);
                    //                        //liuyang added code for the bug end

                    //                        n = g.NodeMap[stepString] as Node;

                    //                        AddToTrace(edge.SourceNode.LabelText, step, edge.TargetNode.LabelText, stepString);
                    //                    }
                    //                    else
                    //                    {
                    //                        continue;
                    //                    }
                    //                }
                    //                else
                    //                {
                    //                    visited.Add(stepString, null);

                    //                    //add the new node and set it to red.
                    //                    n = g.AddNode(stepString);
                    //                    n.LabelText = (g.NodeCount - 1).ToString();
                    //                    n.UserData = step; //// nextState;
                    //                    edge = g.AddEdge(sourceProcess, step.Event, stepString);

                    //                    AddToTrace(edge.SourceNode.LabelText, step, n.LabelText, stepString);
                    //                }

                    //                //set the current node to be Green 
                    //                edge.Label.FontColor = Color.DarkGreen;
                    //                edge.Attr.Color = Color.DarkGreen;
                    //                n.Attr.FillColor = Color.LightGreen;


                    //                string key = GetTraceEvent(this.ListView_Trace.Items.Count) + stepString;
                    //                if (!Mapping.ContainsKey(key))
                    //                {
                    //                    Mapping.Add(key,
                    //                                new ProcessData(step, CloneGraph(g), CloneEnabledEvent()));
                    //                }

                    //                UpdateStore(step);
                    //            }
                    //        }
                    //    }

                    //    SimulatorViewer.Graph = g;
                    //    SimulatorViewer.Invalidate();
                    //}



                    StatusLabel.Text = string.Format(Resources.Graph_Generated___0__Nodes___1__Edges, (g.NodeCount - 1),
                                                     (g.EdgeCount - 1));

                    this.CurrentEnableEventList.Clear();
                    this.ListView_EnabledEvents.Items.Clear();

                    EnableControls();

                }
                catch (Exception ex)
                {
                    PrintErrorMsg(ex);
                }
            }
        }

        private void PrintErrorMsg(Exception ex)
        {
            EnableControls();
            if (ex is RuntimeException)
            {
                Ultility.Ultility.LogRuntimeException(ex as RuntimeException);
                //MessageBox.Show("Runtime exception!\r\n" + ex.Message,// + "\r\n" + ex.StackTrace, "PAT", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (ex is System.OutOfMemoryException)
            {
                PAT.Common.Classes.Expressions.ExpressionClass.OutOfMemoryException outex =
                    new PAT.Common.Classes.Expressions.ExpressionClass.OutOfMemoryException("");
                Ultility.Ultility.LogRuntimeException(outex);
                //MessageBox.Show("Runtime exception!\r\n" + ex.Message,// + "\r\n" + ex.StackTrace, "PAT", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                Ultility.Ultility.LogException(ex, Spec);
                //MessageBox.Show("Exception happened during the simulation: " + ex.Message, //"\r\n" + ex.StackTrace,"PAT", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AddCounterExamplePrefixPathNode(ref string sourceProcess, ConfigurationBase stepOld, bool firstSCC)
        {
            Node n = null;
            Edge edge = null;

            //EventStepSim fromStep = CounterExampleTrace[j-1];

            EventStepSim step = new EventStepSim(stepOld);

            step.SourceProcess = sourceProcess;

            //change all nodes to white node!
            foreach (Node mapN in g.NodeMap.Values)
            {
                if (mapN.Attr.FillColor == Color.Red)
                {
                    mapN.Attr.FillColor = Color.White;
                }
            }

            string stepString;
            ////string nextState = step.ToFullString();

            if (step.StepVisited(visited, out stepString))
            {
                Node srcNode = g.FindNode(step.SourceProcess);

                edge = null;
                foreach (Edge outEdge in srcNode.OutEdges)
                {
                    if (outEdge.LabelText == step.Event && outEdge.Target == stepString)
                    {
                        //duplicate edge is found
                        edge = outEdge;
                        break;
                    }
                }

                if (edge == null)
                {
                    edge = g.AddEdge(step.SourceProcess, step.Event, stepString);
                }

                n = edge.TargetNode; // g.NodeMap[stepString] as Node;

                AddToTrace(edge.SourceNode.LabelText, step, edge.TargetNode.LabelText, stepString);
            }
            else
            {
                visited.Add(stepString, null);

                //add the new node and set it to red.
                n = g.AddNode(stepString);
                n.LabelText = (g.NodeCount - 1).ToString();
                n.UserData = step; //// nextState;

                edge = g.AddEdge(step.SourceProcess, step.Event, stepString);

                AddToTrace(edge.SourceNode.LabelText, step, n.LabelText, stepString);
            }

            if (firstSCC)
            {
                n.Attr.FillColor = Color.LightGreen;
            }
            else
            {
                n.Attr.FillColor = Color.Red;
            }

            sourceProcess = stepString;

            string key = GetTraceEvent(this.ListView_Trace.Items.Count) + stepString;
            if (!Mapping.ContainsKey(key))
            {
                Mapping.Add(key, new ProcessData(step, CloneGraph(g), CloneEnabledEvent()));
            }

            UpdateStore(step);


        }

        #endregion

        private SimulationWorker GraphBuilder;
        private string CutNumberString = "";

        private void Button_GenerateGraph_Click(object sender, EventArgs e)
        {

            if (this.Button_GenerateGraph.Text == STOP)
            {
                if (GraphBuilder != null)
                {
                    Button_GenerateGraph.Enabled = false;
                    GraphBuilder.Cancel();
                }
            }
            else
            {

                if (CanGrabLock())
                {
                    DisableControls();

                    try
                    {

                        Button_GenerateGraph.Enabled = true;
                        Button_GenerateGraph.Text = STOP;

                        //P startingProcess = Spec.GetProcessDef();
                        CutNumberString = "";
                        //if (startingProcess.MustBeAbstracted())
                        //{

                        //    CutNumberForm cutNumberForm = new CutNumberForm();
                        //    if (cutNumberForm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                        //    {
                        //        Classes.Ultility.Ultility.CutNumber = (int) cutNumberForm.NumericUpDown_CutNumber.Value;
                        //        //startingProcess.SetCutNumber(new List<string>());

                        //        CutNumberString = " (Cut Number " + Classes.Ultility.Ultility.CutNumber + ")";

                        //    }
                        //    else
                        //    {
                        //        EnableControls();
                        //        return;
                        //    }
                        //}
                        //else
                        //{
                        //Classes.Ultility.Ultility.CutNumber = -1;
                        //}

                        this.ListView_Trace.Items.Clear();
                        this.ListView_EnabledEvents.Items.Clear();
                        Timer_Run.Stop();
                        Timer_Replay.Stop();
                        Timer_SimulateTrace.Stop();

                        this.SimulationInitialize(true);

                        ListView_EnabledEvents.Items.Clear();

                        EventStepSim initalStep =
                            new EventStepSim(Spec.SimulationInitialization(this.ComboBox_Process.Text));
                        //, Common.Ultility.Constants.INITIAL_EVENT, null, SpecProcess.GetEnvironment());


                        GraphBuilder = new SimulationWorker();
                        GraphBuilder.Initialize(this, initalStep, InitialState, ListView_Trace, g);

                        GraphBuilder.Failed += new System.Threading.ThreadExceptionEventHandler(GraphBuilder_Failed);
                        GraphBuilder.Cancelled += new EventHandler(GraphBuilder_Cancelled);
                        GraphBuilder.Action += new SimulationWorker.ActionEvent(GraphBuilder_Action);

                        if (Common.Ultility.Ultility.IsUnixOS)
                        {
                            GraphBuilder.InternalStart();
                            GraphBuilder_ReturnResult();
                        }
                        else
                        {
                            GraphBuilder.ReturnResult +=
                                new SimulationWorker.ReturnResultEvent(GraphBuilder_ReturnResult);
                            GraphBuilder.Print += new SimulationWorker.PrintingEvent(GraphBuilder_Print);
                            GraphBuilder.Start();
                        }

                    }
                    catch (Exception ex)
                    {
                        PrintErrorMsg(ex);
                    }
                }
            }
        }

        private void GraphBuilder_Print(string msg)
        {
            this.StatusLabel.Text = msg;
            if (msg == "Rendering Graph...")
            {
                Button_GenerateGraph.Enabled = false;
            }
        }

        private void GraphBuilder_Action(ListViewItem item, Graph graph)
        {
            ListView_Trace.Items.Add(item);
            Traces.Add(item.Tag.ToString());
        }

        private void GraphBuilder_Cancelled(object sender, EventArgs e)
        {
            try
            {
                Mapping = GraphBuilder.Mapping;

                SimulatorViewerDockWindow.Controls.Remove(SimulatorViewer);
                SimulatorViewer = GraphBuilder.SimulatorViewer;
                SimulatorViewer.Resize += new EventHandler(SimulatorViewer_Resize);
                SimulatorViewerDockWindow.Controls.Add(SimulatorViewer);

                StatusLabel.Text = Resources.Simulation_Cancelled;

                EnableControls();

                StateInfoControl.SetText("", null);
                //Classes.Ultility.Ultility.CutNumber = -1;
                Mapping = GraphBuilder.Mapping;
                Button_GenerateGraph.Text = GENERATE_GRAPH;
            }
            catch (Exception ex)
            {
                PrintErrorMsg(ex);
            }
        }

        private void GraphBuilder_Failed(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            try
            {
                Mapping = GraphBuilder.Mapping;

                SimulatorViewerDockWindow.Controls.Remove(SimulatorViewer);
                SimulatorViewer = GraphBuilder.SimulatorViewer;
                SimulatorViewer.Resize += new EventHandler(SimulatorViewer_Resize);

                SimulatorViewerDockWindow.Controls.Add(SimulatorViewer);

                StatusLabel.Text = Resources.Error_Happened;

                if (e.Exception is RuntimeException)
                {
                    Common.Ultility.Ultility.LogRuntimeException(e.Exception as RuntimeException);
                }
                else
                {
                    Common.Ultility.Ultility.LogException(e.Exception, Spec);
                    //MessageBox.Show("Exception happened in simulation", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }

                EnableControls();
                //Classes.Ultility.Ultility.CutNumber = -1;
                StateInfoControl.SetText("", null);


                Button_GenerateGraph.Text = GENERATE_GRAPH;
                GraphBuilder = null;
            }
            catch (Exception ex)
            {
                PrintErrorMsg(ex);
            }
        }

        private void GraphBuilder_ReturnResult()
        {
            try
            {
                Mapping = GraphBuilder.Mapping;

                if (Common.Ultility.Ultility.IsUnixOS)
                {
                    this.TableLayoutPanel.Controls.Clear();

                    SimulatorViewer = GraphBuilder.SimulatorViewer;
                    SimulatorViewer.Dock = DockStyle.Fill;
                    SimulatorViewer.Resize += new EventHandler(SimulatorViewer_Resize);

                    this.TableLayoutPanel.Controls.Add(this.SimulatorViewer);
                    this.TableLayoutPanel.Controls.Add(this.SplitContainer);
                    this.TableLayoutPanel.Controls.Add(this.StateInfoControl);

                }
                else
                {
                    SimulatorViewerDockWindow.Controls.Remove(SimulatorViewer);
                    SimulatorViewer = GraphBuilder.SimulatorViewer;
                    SimulatorViewer.Resize += new EventHandler(SimulatorViewer_Resize);

                    SimulatorViewerDockWindow.Controls.Add(SimulatorViewer);
                }

                if (GraphBuilder.ForceSimuationStop)
                {
                    MessageBox.Show(
                        string.Format(Resources.The_simulation_is_forced_to_stop_due_to_the_huge_space_size,
                                      Classes.Ultility.Ultility.SIMULATION_BOUND), Resources.Warning,
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                StatusLabel.Text =
                    string.Format(Resources.Graph_Generated___0__Nodes___1__Edges, (SimulatorViewer.Graph.NodeCount - 1),
                                  (SimulatorViewer.Graph.EdgeCount - 1)) + CutNumberString;

                //StatusLabel.Text = "Graph Generated: " +  + " Nodes, " +  + " Edges" + ;

                //SimulatorViewer.Graph.UserData = Classes.Ultility.Ultility.CutNumber;todo: what is the cutnumber used here for?
                EnableControls();
                //Classes.Ultility.Ultility.CutNumber = -1;
                StateInfoControl.SetText("", null);
                Button_GenerateGraph.Text = GENERATE_GRAPH;
                GraphBuilder = null;
            }
            catch (Exception ex)
            {
                PrintErrorMsg(ex);
            }
        }


        //private void BuildCompleteGraph(EventStepSim currentStep, string currentNode)
        //{

        //    if (visited.Count > Classes.Ultility.Ultility.SIMULATION_BOUND)
        //    {
        //        ForceSimuationStop = true;
        //        return;
        //    }
        //    List<EventStepSim> list = currentStep.MakeOneMove();

        //    foreach (EventStepSim step in list)
        //    {
        //        //change all nodes to white node!
        //        foreach (Node mapN in g.NodeMap.Values)
        //        {
        //            mapN.Attr.FillColor = Color.White;
        //        }

        //        string stepString;
        //        string nextState = step.ToFullString();
        //        step.SourceProcess = currentStep.Process.ToString();


        //        if (step.StepVisited(visited, out stepString))
        //        {
        //            Edge edge = g.AddEdge(currentNode, step.Event, stepString);
        //            (g.NodeMap[stepString] as Node).Attr.FillColor = Color.Red;

        //            //update the ListView_Trace and clone graph
        //            AddToTrace(edge.SourceNode.LabelText, step, edge.TargetNode.LabelText, stepString);

        //            string key = GetTraceEvent(this.ListView_Trace.Items.Count) + stepString;
        //            if (!Mapping.ContainsKey(key))
        //            {
        //                Mapping.Add(key, new ProcessData(step.GlobalEnv.GetClone(), CloneGraph(g), new List<EventStepSim>(0)));
        //            }

        //        }
        //        else
        //        {
        //            visited.Add(stepString, null);

        //            Edge e = g.AddEdge(currentNode, step.Event, stepString);
        //            e.TargetNode.LabelText = (g.NodeCount - 1).ToString();
        //            e.TargetNode.UserData = nextState;
        //            e.TargetNode.Attr.FillColor = Color.Red;

        //            //update the ListView_Trace and clone graph
        //            AddToTrace(e.SourceNode.LabelText, step, e.TargetNode.LabelText, stepString);
        //            string key = GetTraceEvent(this.ListView_Trace.Items.Count) + stepString;
        //            if (!Mapping.ContainsKey(key))
        //            {
        //                Mapping.Add(key, new ProcessData(step.GlobalEnv.GetClone(), CloneGraph(g), new List<EventStepSim>(0)));
        //            }

        //            BuildCompleteGraph(step, stepString);
        //        }
        //    }
        //}

        private void Button_Random_Click(object sender, EventArgs e)
        {
            try
            {

                if (Button_Simulate.Text == SIMULATE)
                {
                    StatusLabel.Text = Resources.Simulation_Starting;

                    Button_Simulate.Text = STOP;
                    Button_Simulate.Image = ImageList.Images["Stop"];

                    Timer_Run.Interval = (int) (AnimationSpeed*1000);

                    if (CanGrabLock())
                    {
                        DisableControls();
                        SimulationInitialize(true);
                    }

                    Button_Simulate.Enabled = true;
                    Timer_Run.Start();
                }
                else
                {
                    Button_Simulate.Text = SIMULATE;
                    Button_Simulate.Image = ImageList.Images["Simulate"];
                    Timer_Run.Stop();
                    EnableControls();
                }

            }
            catch (Exception ex)
            {
                PrintErrorMsg(ex);
            }

        }

        private void EnableControls()
        {
            this.Cursor = Cursors.Default;
            ComboBox_Process.Enabled = true;
            Button_Settings.Enabled = true;
            Button_Reset.Enabled = true;
            Button_InteractionPane.Enabled = true;
            Button_StatePane.Enabled = true;
            Button_PlayTrace.Enabled = true;
            Button_Simulate.Enabled = true;
            Button_GenerateGraph.Enabled = true;
            Button_SimulateTrace.Enabled = true;
            Button_CounterExample.Enabled = true;

            this.ListView_Trace.SelectedIndexChanged += new System.EventHandler(this.ListView_Trace_SelectedIndexChanged);
            this.ListView_EnabledEvents.DoubleClick += new System.EventHandler(this.ListView_EnabledEvents_DoubleClick);
            this.SimulatorViewer.SelectionChanged += new System.EventHandler(this.SimulatorViewer_SelectionChanged);
            this.ListView_Trace.MouseUp += new MouseEventHandler(ListView_Trace_MouseUp);
            this.SimulatorViewer.LayoutEditingEnabled = true;

            //this.SimulatorViewer.BackwardEnabled = true;
            //this.SimulatorViewer.ForwardEnabled = true;
            //this.SimulatorViewer.NavigationVisible = true;
            //this.SimulatorViewer.PanButtonPressed = true;
            //this.SimulatorViewer.SaveButtonVisible = true;
            //Panel_ToolbarCover.Visible = false;
            this.Button_Direction.Visible = true;

            //this.SimulatorViewer.ToolBarIsVisible = true;
            Panel_Hiding.Visible = false;
            Spec.UnLockSharedData();
            //PAT.CSP.Ultility.Ultility.UnLockSharedData(SpecProcess);

        }

        private void DisableControls()
        {
            this.Cursor = Cursors.WaitCursor;
            ComboBox_Process.Enabled = false;
            Button_Settings.Enabled = false;
            Button_Reset.Enabled = false;
            Button_InteractionPane.Enabled = false;
            Button_StatePane.Enabled = false;
            Button_PlayTrace.Enabled = false;
            Button_Simulate.Enabled = false;
            Button_SimulateTrace.Enabled = false;
            Button_GenerateGraph.Enabled = false;
            Button_CounterExample.Enabled = false;


            this.ListView_Trace.SelectedIndexChanged -= new System.EventHandler(this.ListView_Trace_SelectedIndexChanged);
            this.ListView_EnabledEvents.DoubleClick -= new System.EventHandler(this.ListView_EnabledEvents_DoubleClick);
            this.SimulatorViewer.SelectionChanged -= new System.EventHandler(this.SimulatorViewer_SelectionChanged);
            this.SimulatorViewer.LayoutEditingEnabled = false;
            this.ListView_Trace.MouseUp -= new MouseEventHandler(ListView_Trace_MouseUp);

            //this.SimulatorViewer.BackwardEnabled = false;
            //this.SimulatorViewer.ForwardEnabled = false;
            //this.SimulatorViewer.NavigationVisible = false;
            //this.SimulatorViewer.PanButtonPressed = false;
            //this.SimulatorViewer.SaveButtonVisible = false;
            //Panel_ToolbarCover.Visible = true;
            this.Button_Direction.Visible = false;
            //this.SimulatorViewer.ToolBarIsVisible = false;

            Panel_Hiding.Visible = true;
            Spec.LockSharedData(true);

            //====================Liuyang: 23, Jan, 2010============================
            //a special updates to use the explicity ID in the simulator
            //a new simulation datastore is used here.
            //DataStore.DataManager = DataStoreSimulation;
            //====================Liuyang: 23, Jan, 2010============================
        }


        private void Timer_Drawing_Tick(object sender, EventArgs e)
        {
            try
            {
                if (CurrentEnableEventList.Count > 0)
                {
                    int index = GetNextInteger();
                    EventStepSim step = CurrentEnableEventList[index];
                    MakeOneMove(step, index);

                    //StatusLabel.Text = "Graph Generated: " + (g.NodeCount - 1) + " Nodes, " + (g.EdgeCount - 1) + " Edges";
                    StatusLabel.Text = string.Format(Resources.Graph_Generated___0__Nodes___1__Edges, (g.NodeCount - 1),
                                                     (g.EdgeCount - 1));

                }
                else
                {
                    Button_Simulate.Text = SIMULATE;
                    Button_Simulate.Image = ImageList.Images["Simulate"];

                    Timer_Run.Stop();
                    EnableControls();
                    this.SimulatorViewer.Invalidate();
                    this.SimulatorViewer.Refresh();
                }
            }
            catch (Exception ex)
            {
                Button_Simulate.Text = SIMULATE;
                Button_Simulate.Image = ImageList.Images["Simulate"];
                Timer_Run.Stop();
                PrintErrorMsg(ex);
            }
        }

        /// <summary>
        /// pass the index to make sure the current step is removed from the CurrentEnableEventList
        /// </summary>
        /// <param name="step"></param>
        private void MakeOneMove(EventStepSim step, int stepIndex)
        {
            if (WarningFlag)
            {
                if (
                    MessageBox.Show(
                        Resources.The_execution_of_the_step_will_destroy_the_current_trace__Do_you_want_to_continue_,
                        Resources.Warning, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
                {
                    return;
                }
                WarningFlag = false;

                int index = this.ListView_Trace.SelectedIndices[0];

                while (this.ListView_Trace.Items.Count - 1 > index)
                {
                    Traces.RemoveAt(this.ListView_Trace.Items.Count - 1);
                    ListView_Trace.Items.RemoveAt(this.ListView_Trace.Items.Count - 1);
                }
            }

            if (this.ListView_Trace.SelectedIndices.Count > 0)
            {
                this.ListView_Trace.SelectedItems[0].Selected = false;
            }

            if(displayAllEnabledStatesToolStripMenuItem.Checked)
            {
                CurrentEnableEventList.RemoveAt(stepIndex);
            }
            else
            {
                CurrentEnableEventList.Clear();    
            }
            //CurrentEnableEventList.RemoveAt(stepIndex); //.Remove(step);
            

            //change all nodes to white node!
            foreach (Node mapN in g.NodeMap.Values)
            {
                mapN.Attr.FillColor = Color.White;
            }

            string stepString;
            //string nextState = step.ToFullString();

            if (displayAllEnabledStatesToolStripMenuItem.Checked)
            {
                for (int i = 0; i < CurrentEnableEventList.Count; i++)
                {
                    EventStepSim simStep = CurrentEnableEventList[i];
                    simStep.IsCurrentEvent = false;
                    CurrentEnableEventList[i] = simStep;
                }
            }

            if (step.StepVisited(visited, out stepString))
            {
                //Edge edge = g.AddEdge(step.SourceProcess, step.Event, stepString);
                //(g.NodeMap[stepString] as Node).Attr.FillColor = Color.Red;

                Node srcNode = g.FindNode(step.SourceProcess);

                Edge edge = null;
                foreach (Edge outEdge in srcNode.OutEdges)
                {
                    if (outEdge.LabelText == step.Event && outEdge.Target == stepString)
                    {
                        //duplicate edge is found
                        edge = outEdge;
                        break;
                    }
                }

                if (edge == null)
                {
                    foreach (Edge outEdge in srcNode.SelfEdges)
                    {
                        if (outEdge.LabelText == step.Event && outEdge.Target == stepString)
                        {
                            //duplicate edge is found
                            edge = outEdge;
                            break;
                        }
                    }
                }

                if (edge == null)
                {
                    edge = g.AddEdge(step.SourceProcess, step.Event, stepString);
                }

                edge.TargetNode.Attr.FillColor = Color.Red;

                AddToTrace(edge.SourceNode.LabelText, step, edge.TargetNode.LabelText, stepString);

                if (displayAllEnabledStatesToolStripMenuItem.Checked)
                {
                    for (int i = 0; i < CurrentEnableEventList.Count; i++)
                    {
                        EventStepSim simStep = CurrentEnableEventList[i];
                        if (simStep.SourceProcess == stepString)
                        {
                            simStep.IsCurrentEvent = true;
                            CurrentEnableEventList[i] = simStep;
                        }
                    }
                }

                List<EventStepSim> list = step.MakeOneMove(HideTauTransition);
                for (int i = 0; i < list.Count; i++)
                {
                    EventStepSim s = list[i];
                    s.SourceProcess = stepString;

                    s.IsUnvisitedStep = true;
                    foreach (Edge outEdge in edge.TargetNode.OutEdges)
                    {
                        if (outEdge.LabelText == s.Event && outEdge.Target == s.StepID)
                        {
                            s.IsUnvisitedStep = false;
                            break;
                        }
                    }


                    if (s.IsUnvisitedStep)
                    {
                        foreach (Edge outEdge in edge.TargetNode.SelfEdges)
                        {
                            if (outEdge.LabelText == s.Event && outEdge.Target == stepString)
                            {
                                s.IsUnvisitedStep = false;
                                break;
                            }
                        }
                    }


                    list[i] = s;
                }

                CurrentEnableEventList.AddRange(list);
            }
            else
            {
                visited.Add(stepString, null);

                //add the new node and set it to red.
                Node n = g.AddNode(stepString);
                n.LabelText = (g.NodeCount - 1).ToString();
                n.UserData = step; // nextState;
                n.Attr.FillColor = Color.Red;

                Edge edge = g.AddEdge(step.SourceProcess, step.Event, stepString);


                AddToTrace(edge.SourceNode.LabelText, step, n.LabelText, stepString);

                List<EventStepSim> list = step.MakeOneMove(HideTauTransition);
                for (int i = 0; i < list.Count; i++)
                {
                    EventStepSim s = list[i];
                    s.SourceProcess = stepString;

                    s.IsUnvisitedStep = true;
                    foreach (Edge outEdge in edge.TargetNode.OutEdges)
                    {
                        if (outEdge.LabelText == s.Event && outEdge.Target == s.StepID)
                        {
                            s.IsUnvisitedStep = false;
                            break;
                        }
                    }

                    if (s.IsUnvisitedStep)
                    {
                        foreach (Edge outEdge in edge.TargetNode.SelfEdges)
                        {
                            if (outEdge.LabelText == s.Event && outEdge.Target == stepString)
                            {
                                s.IsUnvisitedStep = false;
                                break;
                            }
                        }
                    }

                    list[i] = s;
                }

                CurrentEnableEventList.AddRange(list);
            }

            SimulatorViewer.Graph = g;
            SimulatorViewer.Invalidate();

            string key = GetTraceEvent(this.ListView_Trace.Items.Count) + stepString;
            if (!Mapping.ContainsKey(key))
            {
                Mapping.Add(key, new ProcessData(step, CloneGraph(g), CloneEnabledEvent()));
            }

            UpdateStore(step);

            FillCurrentEnabledList();

        }

        private void UpdateStore(EventStepSim step)
        {

            StateInfoControl.SetText(step.StepToString,
                                     Spec.MapConfigurationToImage(step.Config, (StateInfoControl.Width - 15)*2));
        }

        public void AddToTrace(string srcID, EventStepSim step, string id, string key)
        {
            ListViewItem item = new ListViewItem(new string[] {srcID, step.Event, id});
            item.Tag = step.StepToString;
            ListView_Trace.Items.Add(item);
            Traces.Add(key);
        }

        private int GetNextInteger()
        {
            Random r = new Random();
            int index = r.Next(0, CurrentEnableEventList.Count);
            return index;
        }

        private void Button_Reset_Click(object sender, EventArgs e)
        {
            try
            {
                this.ListView_Trace.Items.Clear();
                this.ListView_EnabledEvents.Items.Clear();
                Timer_Run.Stop();
                Timer_Replay.Stop();
                Timer_SimulateTrace.Stop();
                this.StateInfoControl.SetText("", null);

                Button_SpeedNormal.Checked = true;
                if (this.SimulatorViewer.Graph != null)
                {
                    //todo: clear nodes
                    this.SimulatorViewer.Graph.NodeMap.Clear();
                }

                if (CanGrabLock())
                {
                    DisableControls();
                    this.SimulationInitialize(true);
                    EnableControls();
                }

                StatusLabel.Text = Resources.Ready;
            }
            catch (Exception ex)
            {
                PrintErrorMsg(ex);
            }
        }

        private static Graph CloneGraph(Graph graph)
        {
            Graph Graph = new Graph(graph.Label.Text);
            //Graph.GraphAttr.Orientation = graph.GraphAttr.Orientation;
            Graph.Attr.LayerDirection = graph.Attr.LayerDirection;

            Debug.Assert(graph.Edges.Count > 0);

            foreach (Edge edge in graph.Edges)
            {
                Edge newEdge = Graph.AddEdge(edge.Source, edge.LabelText, edge.Target);
                if (edge.Source == INITIAL_STATE)
                {
                    newEdge.SourceNode.Attr.LineWidth = 0;
                    newEdge.SourceNode.Attr.Color = Color.White;
                }
                newEdge.SourceNode.LabelText = edge.SourceNode.LabelText;
                newEdge.SourceNode.UserData = edge.SourceNode.UserData;
                newEdge.TargetNode.LabelText = edge.TargetNode.LabelText;
                newEdge.TargetNode.Attr.FillColor = edge.TargetNode.Attr.FillColor;
                newEdge.TargetNode.UserData = edge.TargetNode.UserData;
                newEdge.Attr.Color = edge.Attr.Color;
            }

            return Graph;
        }


        private List<EventStepSim> CloneEnabledEvent()
        {
            List<EventStepSim> events = new List<EventStepSim>();
            foreach (EventStepSim sim in CurrentEnableEventList)
            {
                EventStepSim step = new EventStepSim(sim.Config);
                step.IsUnvisitedStep = sim.IsUnvisitedStep;
                step.IsCurrentEvent = sim.IsCurrentEvent;
                step.SourceProcess = sim.SourceProcess;
                step.StepToString = sim.StepToString;
                events.Add(step);
            }
            return events;
        }




        #region Mouse Event



        private object selectedObjectAttr;
        private object selectedObject;
        private Color selectedObjectColor;

        private void SimulatorViewer_SelectionChanged(object sender, EventArgs e)
        {
            lock (this)
            {
                string tooltipstring;
                if (selectedObject != null)
                {
                    if (selectedObject is Edge)
                    {
                        Edge edge = (selectedObject as Edge);
                        edge.Attr = selectedObjectAttr as EdgeAttr;
                        if (edge.Label != null)
                        {
                            edge.Label.FontColor = selectedObjectColor;
                        }
                    }
                    else if (selectedObject is Node)
                    {
                        (selectedObject as Node).Attr = selectedObjectAttr as NodeAttr;
                        (selectedObject as Node).Label.FontColor = selectedObjectColor;
                    }

                    selectedObject = null;
                }

                if (SimulatorViewer.SelectedObject == null)
                {
                    tooltipstring = "";
                    //label1.Text = "No object under the mouse";
                    //this.SimulatorViewer.SetToolTip(ToolTip_Graph, "");

                    //ToolTip_Graph.Show("", SimulatorViewer, Cursor.Position, 20000);
                    ToolTip_Graph.Hide(SimulatorViewer);
                }
                else
                {
                    selectedObject = SimulatorViewer.SelectedObject;

                    if (selectedObject is Edge)
                    {
                        Edge selectedEdge = SimulatorViewer.SelectedObject as Edge;
                        selectedObjectAttr = selectedEdge.Attr.Clone();
                        selectedEdge.Attr.Color = Color.Blue;
                        if (selectedEdge.Label != null)
                        {
                            selectedObjectColor = selectedEdge.Label.FontColor;
                            selectedEdge.Label.FontColor = Color.Blue;
                        }

                        //here you can use e.Attr.Id or e.UserData to get back to you data
                        //this.SimulatorViewer.SetToolTip(this.ToolTip_Graph, String.Format("edge " + selectedEdge.LabelText + " from {0} to {1}", selectedEdge.SourceNode.LabelText == "" ? "init" : selectedEdge.SourceNode.LabelText, selectedEdge.TargetNode.LabelText));
                        //String.Format("edge", selectedEdge.SourceNode.UserData, selectedEdge.TargetNode.UserData));

                        Point p = System.Windows.Forms.Control.MousePosition;

                        Point q = SimulatorViewer.PointToScreen(new Point(0, 0));

                        p.X = p.X - q.X + 15;
                        p.Y = p.Y - q.Y + 15;

                        ToolTip_Graph.Show(
                            String.Format("edge " + selectedEdge.LabelText + " from {0} to {1}",
                                          selectedEdge.SourceNode.LabelText == ""
                                              ? "init"
                                              : selectedEdge.SourceNode.LabelText, selectedEdge.TargetNode.LabelText),
                            SimulatorViewer, p, ToolTipDisplayTime);
                    }
                    else if (selectedObject is Node)
                    {
                        Node selectedNode = SimulatorViewer.SelectedObject as Node;
                        selectedObjectAttr = selectedNode.Attr.Clone();
                        selectedObjectColor = selectedNode.Label.FontColor;

                        if (selectedNode != null && selectedNode.Id != INITIAL_STATE)
                        {

                            selectedNode.Attr.Color = Color.Blue;
                            selectedNode.Label.FontColor = Color.Blue;
                            //here you can use e.Attr.Id to get back to your data
                            //this.SimulatorViewer.SetToolTip(this.ToolTip_Graph, String.Format("node {0}", selectedNode.UserData));

                            Point p = System.Windows.Forms.Control.MousePosition;

                            Point q = SimulatorViewer.PointToScreen(new Point(0, 0));

                            p.X = p.X - q.X + 15;
                            p.Y = p.Y - q.Y + 15;


                            //here you can use e.Attr.Id to get back to your data
                            //this.SimulatorViewer.SetToolTip(this.ToolTip_Graph, String.Format("node {0}", selectedNode.UserData));
                            ToolTip_Graph.Show(String.Format("{0}", selectedNode.UserData), SimulatorViewer, p,
                                               ToolTipDisplayTime);

                            //ToolTip_Graph.Show(String.Format("node {0}", selectedNode.UserData), SimulatorViewer, System.Windows.Forms.Control.MousePosition, ToolTipDisplayTime);
                        }
                    }
                }
                SimulatorViewer.Invalidate();


            }
        }

        #endregion

        private void ComboBox_Process_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (CanGrabLock())
            {
                DisableControls();

                try
                {
                    SimulationInitialize(true);
                    StatusLabel.Text = Resources.Ready;
                    EnableControls();
                }
                catch (RuntimeException ex)
                {
                    try
                    {
                        this.ListView_Trace.Items.Clear();
                        this.ListView_EnabledEvents.Items.Clear();
                        Timer_Run.Stop();
                        Timer_Replay.Stop();
                        Timer_SimulateTrace.Stop();
                        this.StateInfoControl.SetText("", null);

                        Button_SpeedNormal.Checked = true;

                        this.SimulatorViewer.Graph = new Graph("Graph");
                        this.SimulatorViewer.Invalidate();
                        this.SimulatorViewer.Refresh();

                        StatusLabel.Text = Resources.Error;
                    }
                    catch (Exception)
                    {

                    }

                    PrintErrorMsg(ex);
                }
                catch (Exception ex)
                {
                    PrintErrorMsg(ex);
                    this.Close();
                }
            }
        }

        private bool CanGrabLock()
        {
            if (Spec.GrabSharedDataLock()) //Ultility.Ultility.GrabSharedDataLock()
            {
                return true;
            }
            else
            {
                MessageBox.Show(Resources.Please_stop_verification_or_parsing_before_the_simulation_,
                                Common.Ultility.Ultility.APPLICATION_NAME, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }


        private void SimulationInitialize(bool moveFirsStep)
        {


            //clear the trace and visited table
            Traces.Clear();
            visited.Clear();
            ListView_Trace.Items.Clear();

            //get the starting process

            //create the initial step
            EventStepSim initialStep = new EventStepSim(Spec.SimulationInitialization(this.ComboBox_Process.Text));
            //new EventStepSim(startingProcess, Common.Ultility.Constants.INITIAL_EVENT, null, SpecProcess.GetEnvironment()));

            //use the process string as the definition ref string, this is the only special case needs to be taken care of
            //initialStep.ProcessToString = startingProcess.Def.ToString();
            //todo: tobe checked
            //initialStep.ProcessToString = startingProcess.ToString();

            InitialState = initialStep.StepID;

            //string initialLabel = initialStep.ToFullString();

            AddToTrace("0", initialStep, "1", InitialState);


            CurrentEnableEventList.Clear();

            if (moveFirsStep)
            {
                //try
                //{
                List<EventStepSim> list = initialStep.MakeOneMove(HideTauTransition);

                for (int i = 0; i < list.Count; i++)
                {
                    EventStepSim step = list[i];
                    step.SourceProcess = InitialState;
                    step.IsUnvisitedStep = true;
                    list[i] = step;
                }

                CurrentEnableEventList.AddRange(list);
                //}
                //catch (Exception ex)
                //{

                //}
            }

            visited.Add(InitialState, null);

            g = new Graph("Graph");
            //g.GraphAttr.Orientation = System.Windows.Forms.Orientation.Landscape;
            g.Attr.LayerDirection = Direction;
            Node n = g.AddNode(InitialState);
            n.Attr.FillColor = Color.Red;
            n.LabelText = "1";
            n.UserData = initialStep; //initialLabel;

            Node tempN = g.AddNode(INITIAL_STATE);
            tempN.Attr.LineWidth = 0;
            tempN.Attr.Color = Color.White;
            tempN.LabelText = "";
            tempN.UserData = "";
            g.AddEdge(INITIAL_STATE, InitialState);

            //clear the mapping table
            Mapping.Clear();
            Mapping.Add(GetTraceEvent(this.ListView_Trace.Items.Count) + InitialState,
                        new ProcessData(initialStep, CloneGraph(g), CloneEnabledEvent()));

            SimulatorViewer.Graph = g;
            SimulatorViewer.Validate();

            FillCurrentEnabledList();
            UpdateStore(initialStep);

            WarningFlag = false;


        }


        private void FillCurrentEnabledList()
        {
            this.ListView_EnabledEvents.Items.Clear();
            foreach (EventStepSim step in CurrentEnableEventList)
            {
                //if (DisplayAllEnabledEvents || step.IsCurrentEvent)
                if (displayAllEnabledStatesToolStripMenuItem.Checked)
                {
                    Node s = (g.NodeMap[step.SourceProcess] as Node);
                    ListViewItem item = new ListViewItem(new string[] { s == null ? "" : s.LabelText, step.Event });
                    //s == null ? "" : s.UserData.ToString(),, step.StepToString 
                    ListView_EnabledEvents.Items.Add(item);

                    if (step.IsCurrentEvent && step.IsUnvisitedStep)
                    {
                        item.ForeColor = System.Drawing.Color.Blue;
                        //item.Font = new Font(item.Font.Name, item.Font.Size, FontStyle.Bold);
                    }
                }
                else
                {
                    Node s = (g.NodeMap[step.SourceProcess] as Node);
                    ListViewItem item = new ListViewItem(new string[] {s == null ? "" : s.LabelText, step.Event});
                    //s == null ? "" : s.UserData.ToString(),, step.StepToString 
                    ListView_EnabledEvents.Items.Add(item);

                    if (step.IsUnvisitedStep)
                    {
                        item.ForeColor = System.Drawing.Color.Blue;
                        //item.Font = new Font(item.Font.Name, item.Font.Size, FontStyle.Bold);
                    }
                }
            }
        }



        private void ListView_EnabledEvents_DoubleClick(object sender, EventArgs e)
        {

            if (this.ListView_EnabledEvents.SelectedItems.Count > 0)
            {
                if (CanGrabLock())
                {
                    DisableControls();

                    try
                    {
                        int index = this.ListView_EnabledEvents.SelectedItems[0].Index;
                        EventStepSim step = CurrentEnableEventList[index];
                        MakeOneMove(step, index);

                        EnableControls();
                    }
                    catch (Exception ex)
                    {
                        FillCurrentEnabledList();

                        PrintErrorMsg(ex);
                    }
                }
            }
        }



        /*
         * 
         *       private void Button_ExcuteNext_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.ListView_EnabledEvents.SelectedItems.Count > 0)
                {
                    int index = this.ListView_EnabledEvents.SelectedItems[0].Index;
                    EventStepSim step = CurrentEnableEventList[index];
                    MakeOneMove(step, index);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Exception happened during the simulation: " + ex.Message + "\r\n" + ex.StackTrace,
                                "PAT", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            
        }
        private void Button_Next_Click(object sender, EventArgs e)
        {
            try
            {
                int size = this.ListView_Trace.SelectedIndices.Count;
                if (size > 0)
                {
                    int index = this.ListView_Trace.SelectedIndices[0];
                    if (index + 1 < this.ListView_Trace.Items.Count)
                    {
                        this.ListView_Trace.Items[index + 1].Selected = true;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Exception happened during the simulation: " + ex.Message + "\r\n" + ex.StackTrace,
                                "PAT", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Button_Previous_Click(object sender, EventArgs e)
        {
            try
            {
                int size = this.ListView_Trace.SelectedIndices.Count;
                if (size > 0)
                {
                    int index = this.ListView_Trace.SelectedIndices[0];
                    if (index - 1 >= 0)
                    {
                        this.ListView_Trace.Items[index - 1].Selected = true;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Exception happened during the simulation: " + ex.Message + "\r\n" + ex.StackTrace,
                                "PAT", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }*/

        public string GetTraceEvent(int size)
        {
            string evts = "";
            for (int i = 0; i < size; i++)
            {
                evts += "->" + ListView_Trace.Items[i].SubItems[0].Text;
            }
            return evts;
        }

        private void ListView_Trace_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (this.ListView_Trace.SelectedIndices.Count > 0)
                {
                    int index = this.ListView_Trace.SelectedIndices[0];

                    if (index < this.ListView_Trace.Items.Count - 1)
                    {
                        WarningFlag = true;
                    }
                    else
                    {
                        WarningFlag = false;
                    }

                    string trace = GetTraceEvent(index + 1) + Traces[index];
                    ProcessData processData = Mapping[trace];

                    //redraw the graphics
                    this.SimulatorViewer.Graph = processData.Graph;
                    this.SimulatorViewer.Invalidate(false);


                    g = CloneGraph(processData.Graph); //this.SimulatorViewer.Graph; 

                    //refresh the current Enabled list
                    this.CurrentEnableEventList = processData.CurrentEnabledSteps;
                    this.CurrentEnableEventList = this.CloneEnabledEvent();
                    FillCurrentEnabledList();


                    UpdateStore(processData.State);


                    //refill the visited list           
                    visited.Clear();
                    for (int i = 0; i < index + 1; i++)
                    {
                        if (!visited.ContainsKey(Traces[i]))
                        {
                            visited.Add(Traces[i], null);
                        }
                    }
                }
                else
                {
                    this.ListView_EnabledEvents.Items.Clear();

                }

            }
            catch (Exception ex)
            {
                PrintErrorMsg(ex);
            }
        }

        private void Button_Replay_Click(object sender, EventArgs e)
        {
            try
            {
                if (Button_PlayTrace.Text == PLAY_TRACE)
                {
                    if (this.ListView_Trace.SelectedIndices.Count > 0)
                    {
                        Button_PlayTrace.Text = STOP;
                        Button_PlayTrace.Image = ImageList.Images["Stop"];

                        this.Timer_Replay.Interval = (int) (AnimationSpeed*1000);
                        DisableControls();

                        this.ListView_Trace.SelectedIndexChanged +=
                            new System.EventHandler(this.ListView_Trace_SelectedIndexChanged);
                        Button_PlayTrace.Enabled = true;

                        StatusLabel.Text = Resources.Trace_Play_Starts___;
                        this.Timer_Replay.Start();
                    }
                    else
                    {
                        MessageBox.Show(Resources.Please_select_a_row_in_trace_list_as_the_starting_point_,
                                        Common.Ultility.Ultility.APPLICATION_NAME, MessageBoxButtons.OK,
                                        MessageBoxIcon.Asterisk);

                    }
                }
                else
                {
                    Button_PlayTrace.Text = PLAY_TRACE;
                    Button_PlayTrace.Image = ImageList.Images["Play"];
                    Timer_Replay.Stop();
                    StatusLabel.Text = Resources.Trace_Play_Stopped;
                    EnableControls();
                    this.ListView_Trace.SelectedIndexChanged -=
                        new System.EventHandler(this.ListView_Trace_SelectedIndexChanged);

                }
            }
            catch (Exception ex)
            {
                PrintErrorMsg(ex);
            }
        }

        private void Timer_Replay_Tick(object sender, EventArgs e)
        {
            try
            {
                int size = this.ListView_Trace.SelectedIndices.Count;
                if (size > 0)
                {
                    int index = this.ListView_Trace.SelectedIndices[0];
                    if (index + 1 < this.ListView_Trace.Items.Count)
                    {
                        this.ListView_Trace.Items[index + 1].Selected = true;
                    }
                    else
                    {
                        Button_PlayTrace.Text = PLAY_TRACE;
                        Button_PlayTrace.Image = ImageList.Images["Play"];

                        Timer_Replay.Stop();
                        StatusLabel.Text = Resources.Trace_Play_Finished;
                        EnableControls();
                        this.ListView_Trace.SelectedIndexChanged -=
                            new System.EventHandler(this.ListView_Trace_SelectedIndexChanged);
                    }
                }
            }
            catch (Exception ex)
            {
                Button_PlayTrace.Text = PLAY_TRACE;
                Button_PlayTrace.Image = ImageList.Images["Play"];

                Timer_Replay.Stop();
                EnableControls();
                this.ListView_Trace.SelectedIndexChanged -=
                    new System.EventHandler(this.ListView_Trace_SelectedIndexChanged);
                PrintErrorMsg(ex);


                //Common.Ultility.Ultility.LogException(ex, Spec);
                //MessageBox.Show("Exception happened during the simulation: " + ex.Message + "\r\n" + ex.StackTrace,"PAT", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }





        private void Button_EventPane_CheckStateChanged(object sender, EventArgs e)
        {
            try
            {
                if (Common.Ultility.Ultility.IsUnixOS)
                {
                    if (Button_InteractionPane.Checked)
                    {
                        this.SplitContainer.Visible = true;
                        //this.TableLayoutPanel.ColumnStyles[2].Width = 240;
                    }
                    else
                    {
                        this.SplitContainer.Visible = false;
                        //this.TableLayoutPanel.ColumnStyles[2].Width = 0;
                    }
                }
                else
                {
                    if (Button_InteractionPane.Checked)
                    {
                        if (EventWindow.Tag == null)
                        {
                            EventWindow.Tag = true;
                            EventWindow.Show(DockContainer, DockState.DockRight);
                            EventWindow.DockPanel.DockRightPortion = (240/(double) this.Width);
                        }
                        else
                        {
                            EventWindow.Show(DockContainer);
                        }
                    }
                    else
                    {
                        EventWindow.Hide();
                    }
                }
            }
            catch (Exception ex)
            {
                PrintErrorMsg(ex);

                //Common.Ultility.Ultility.LogException(ex, Spec);
                //MessageBox.Show("Exception happened: " + ex.Message + "\r\n" + ex.StackTrace,"PAT", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Button_State_CheckStateChanged(object sender, EventArgs e)
        {
            try
            {
                if (Common.Ultility.Ultility.IsUnixOS)
                {
                    if (Button_StatePane.Checked)
                    {
                        StateInfoControl.Visible = true;
                        //this.TableLayoutPanel.ColumnStyles[0].Width = 200;
                    }
                    else
                    {
                        StateInfoControl.Visible = false;
                        //this.TableLayoutPanel.ColumnStyles[0].Width = 0;
                    }
                }
                else
                {
                    if (Button_StatePane.Checked)
                    {
                        if (StatePane.Tag == null)
                        {
                            StatePane.Tag = true;
                            //StatePane.DockPanel.DockRightPortion = (180 / (double)this.Width);
                            StatePane.Show(DockContainer, DockState.DockLeft);
                            DockContainer.DockLeftPortion = (180/(double) this.Width);
                        }
                        else
                        {
                            StatePane.Show(DockContainer);
                        }
                    }
                    else
                    {
                        StatePane.Hide();
                    }
                }
            }
            catch (Exception ex)
            {
                //Common.Ultility.Ultility.LogException(ex, Spec);
                PrintErrorMsg(ex);

            }
        }

        private void toolStripMenuItem_SpeedVerySlow_Click_1(object sender, EventArgs e)
        {
            this.Button_SpeedFast.Checked = false;
            this.Button_SpeedNormal.Checked = false;
            this.Button_SpeedSlow.Checked = false;
            this.Button_SpeedVeryFast.Checked = false;
            this.Button_SpeedVerySlow.Checked = false;

            ToolStripMenuItem button = (sender as ToolStripMenuItem);
            button.Checked = true;
            AnimationSpeed = double.Parse(button.Tag.ToString());
        }

        private void secondsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Button_5seconds.Checked = false;
            this.Button_10seconds.Checked = false;
            this.Button_20seconds.Checked = false;
            this.Button_40seconds.Checked = false;
            this.Button_60seconds.Checked = false;

            ToolStripMenuItem button = (sender as ToolStripMenuItem);
            button.Checked = true;
            this.ToolTipDisplayTime = int.Parse(button.Tag.ToString())*1000;
        }

        private void hignlightCounterexampleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Button_DisplayCounterexample.Checked = false;
            this.Button_DisplaySCC.Checked = false;
            ToolStripMenuItem button = (sender as ToolStripMenuItem);
            button.Checked = true;

            DisplayCounterExample();
        }

        private void Button_Direction_Click(object sender, EventArgs e)
        {
            if (Direction == LayerDirection.TB)
            {
                Direction = LayerDirection.LR;
                Button_Direction.Text = "L";
            }
            else
            {
                Direction = LayerDirection.TB;
                Button_Direction.Text = "T";
            }
        }


        private void ListView_Trace_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                if (this.ListView_Trace.SelectedIndices.Count > 0)
                {
                    toolStripMenuItem1.Enabled = true;
                    cpToolStripMenuItem.Enabled = true;
                    copyEventToolStripMenuItem.Enabled = true;
                }
                else
                {
                    toolStripMenuItem1.Enabled = false;
                    cpToolStripMenuItem.Enabled = false;
                    copyEventToolStripMenuItem.Enabled = false;
                }

                MenuStrip_Trace.Show(sender as Control, e.Location);
            }
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Button_PlayTrace.PerformClick();
        }

        private void cpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.ListView_Trace.SelectedIndices.Count > 0)
            {
                try
                {
                    Clipboard.SetDataObject(this.ListView_Trace.SelectedItems[0].Tag, true);
                }
                catch (Exception)
                {

                }

            }
        }

        private void copyEventToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.ListView_Trace.SelectedIndices.Count > 0)
            {
                try
                {
                    Clipboard.SetDataObject(this.ListView_Trace.SelectedItems[0].SubItems[1].Text, true);
                }
                catch (Exception)
                {

                }

            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog svd = new SaveFileDialog();
            svd.Title = Resources.Save_File;
            svd.Filter = "Text Files|*.txt|All files|*.*";

            if (svd.ShowDialog() == DialogResult.OK)
            {
                TextWriter tr = new StreamWriter(svd.FileName);

                StringBuilder sb = new StringBuilder();
                foreach (ListViewItem item in this.ListView_Trace.Items)
                {
                    sb.AppendLine(Resources.Source_State + ": [" + item.SubItems[0].Text + "] -----" +
                                  item.SubItems[1].Text + "-----> " + Resources.Target_State + ": [" +
                                  item.SubItems[2].Text + "] ");

                    int index = item.Index;
                    string trace = GetTraceEvent(index + 1) + Traces[index];
                    ProcessData processData = Mapping[trace];

                    sb.AppendLine(Resources.Target_State + ": " + processData.State);
                    sb.AppendLine();
                }

                tr.WriteLine(sb.ToString());
                tr.Flush();
                tr.Close();
            }
        }

        private void SimulationForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                EnableControls();
                if (GraphBuilder != null)
                {
                    GraphBuilder.Cancel();
                }
            }
            catch (Exception)
            {

            }

            //Common.Ultility.Ultility.SimulationForms.Remove(this);
        }

        private void hideTauTransitionToolStripMenuItem_CheckStateChanged(object sender, EventArgs e)
        {
            HideTauTransition = hideTauTransitionToolStripMenuItem.Checked;
        }

        /// <summary>
        /// Methods for Zhang Xian
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Export_Click(object sender, EventArgs e)
        {
            SaveFileDialog svd = new SaveFileDialog();
            svd.Title = Resources.Save_File;
            svd.Filter = "Text Files|*.txt|All files|*.*";

            if (svd.ShowDialog() == DialogResult.OK)
            {
                TextWriter tr = new StreamWriter(svd.FileName);

                StringBuilder sb = new StringBuilder();
                foreach (ListViewItem item in this.ListView_Trace.Items)
                {
                    sb.AppendLine(Resources.Source_State + ": [" + item.SubItems[0].Text + "] -----" +
                                  item.SubItems[1].Text + "-----> " + Resources.Target_State + ": [" +
                                  item.SubItems[2].Text + "] ");

                    int index = item.Index;
                    string trace = GetTraceEvent(index + 1) + Traces[index];
                    ProcessData processData = Mapping[trace];

                    //sb.AppendLine("Target State: " + processData.State);
                    sb.AppendLine(Resources.Target_State + ": " + processData.State);

                    sb.AppendLine();
                }

                //SimulatorViewer.Graph is the displayed graph
                //you can get all edges and nodes from it.
                //SimulatorViewer.Graph.NodeMap
                //SimulatorViewer.Graph.Edges

                foreach (Edge edge in SimulatorViewer.Graph.Edges)
                {
                    //Edge newEdge = Graph.AddEdge(edge.Source, edge.LabelText, edge.Target);
                    if (edge.Source == INITIAL_STATE)
                    {
                        //this is intial state
                    }
                    //newEdge.SourceNode.LabelText = edge.SourceNode.LabelText;
                    //newEdge.SourceNode.UserData = edge.SourceNode.UserData;
                    //newEdge.TargetNode.LabelText = edge.TargetNode.LabelText;
                    //newEdge.TargetNode.Attr.FillColor = edge.TargetNode.Attr.FillColor;
                    //newEdge.TargetNode.UserData = edge.TargetNode.UserData;
                    //newEdge.Attr.Color = edge.Attr.Color;

                    //edge.SourceNode.UserData;
                }

                tr.WriteLine(sb.ToString());
                tr.Flush();
                tr.Close();
            }
        }

        FormTraceInput FormTraceInput = new FormTraceInput();
        private void Button_SimulateTrace_Click(object sender, EventArgs e)
        {

            try
            {
                if (Button_SimulateTrace.Text == SIMULATE_TRACE)
                {
                    if (FormTraceInput.ShowDialog(this) == DialogResult.OK)
                    {
                        Button_SimulateTrace.Text = STOP;
                        Button_SimulateTrace.Image = ImageList.Images["Stop"];

                        this.Timer_SimulateTrace.Interval = (int)(AnimationSpeed * 1000);
                        DisableControls();

                        //this.ListView_Trace.SelectedIndexChanged +=
                        //    new System.EventHandler(this.ListView_Trace_SelectedIndexChanged);
                        Button_SimulateTrace.Enabled = true;

                        if (this.ListView_Trace.SelectedIndices.Count > 0)
                        {
                            int index = this.ListView_Trace.SelectedIndices[0];

                            while (this.ListView_Trace.Items.Count - 1 > index)
                            {
                                Traces.RemoveAt(this.ListView_Trace.Items.Count - 1);
                                ListView_Trace.Items.RemoveAt(this.ListView_Trace.Items.Count - 1);
                            }
                        }

                        StatusLabel.Text = Resources.Trace_Play_Starts___;
                        this.Timer_SimulateTrace.Start();
                    }
                    //else
                    //{
                    //    MessageBox.Show(Resources.Please_select_a_row_in_trace_list_as_the_starting_point_,
                    //                    Common.Ultility.Ultility.APPLICATION_NAME, MessageBoxButtons.OK,
                    //                    MessageBoxIcon.Asterisk);

                    //}
                }
                else
                {
                    Button_SimulateTrace.Text = SIMULATE_TRACE;
                    Button_SimulateTrace.Image = ImageList.Images["Play"];
                    Timer_SimulateTrace.Stop();
                    StatusLabel.Text = Resources.Trace_Play_Stopped;
                    EnableControls();
                    //this.ListView_Trace.SelectedIndexChanged -=
                    //    new System.EventHandler(this.ListView_Trace_SelectedIndexChanged);

                }
            }
            catch (Exception ex)
            {
                PrintErrorMsg(ex);
            }

        //if (FormTraceInput.ShowDialog(this) == DialogResult.OK)
        //{
        //    if (CanGrabLock())
        //    {
        //        DisableControls(false);

        //        try
        //        {
        //            foreach (string name in FormTraceInput.EventList)
        //            {
        //                foreach (ListViewItem item in this.ListView_EnabledEvents.Items)
        //                {
        //                    if (item.SubItems[1].Text == name)
        //                    {
        //                        int index = item.Index;
        //                        EventStepSim step = CurrentEnableEventList[index];
        //                        MakeOneMove(step);
        //                    }
        //                }
        //            }
        //            EnableControls();
        //        }
        //        catch (Exception ex)
        //        {
        //            FillCurrentEnabledList();

        //            PrintErrorMsg(ex);
        //        }
        //    }
        //}
        }

        private void Timer_SimulateTrace_Tick(object sender, EventArgs e)
        {
            try
            {
                int size = FormTraceInput.EventList.Count;
                string name = "";
                if (size > 0)
                {
                    name = FormTraceInput.EventList[0];
                    FormTraceInput.EventList.RemoveAt(0);
                    int i = 0;
                    foreach (EventStepSim step in CurrentEnableEventList)
                    {
                        if (step.Event.Trim() == name.Trim())
                        {
                            WarningFlag = false;
                            MakeOneMove(step, i);
                            return;
                        }
                        i++;
                    }
                }

                Button_SimulateTrace.Text = SIMULATE_TRACE;
                Button_SimulateTrace.Image = ImageList.Images["Play"];

                Timer_SimulateTrace.Stop();
                StatusLabel.Text = Resources.Trace_Play_Finished;
                EnableControls();
                //this.ListView_Trace.SelectedIndexChanged -=
                //    new System.EventHandler(this.ListView_Trace_SelectedIndexChanged);

                if(size  > 1)
                {
                    MessageBox.Show("There is no matching of event " + name);
                }

            }
            catch (Exception ex)
            {
                Button_SimulateTrace.Text = SIMULATE_TRACE;
                Button_SimulateTrace.Image = ImageList.Images["Play"];

                Timer_SimulateTrace.Stop();
                EnableControls();
                //this.ListView_Trace.SelectedIndexChanged -=
                //    new System.EventHandler(this.ListView_Trace_SelectedIndexChanged);
                PrintErrorMsg(ex);


                //Common.Ultility.Ultility.LogException(ex, Spec);
                //MessageBox.Show("Exception happened during the simulation: " + ex.Message + "\r\n" + ex.StackTrace,"PAT", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }


    public sealed class ProcessData
    {
        public EventStepSim State;
        public Graph Graph;
        public List<EventStepSim> CurrentEnabledSteps;

        public ProcessData(EventStepSim state, Graph g, List<EventStepSim> enabled)
        {
            this.State = state;
            this.Graph = g;
            CurrentEnabledSteps = enabled;
        }
    }
}