using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Reflection;
using System.Windows.Forms;
using Fireball.Docking;
using Microsoft.Msagl.Drawing;
using PAT.Common.Classes.Assertion;
using PAT.Common.Classes.DataStructure;
using PAT.Common.Classes.Expressions;
using PAT.Common.Classes.Expressions.ExpressionClass;
using PAT.Common.GUI;
using PAT.Common.Ultility;
using PAT.GenericDiff.diff;
using PAT.GenericDiff.graph;
using PAT.GenericDiff.utility;
using Color=Microsoft.Msagl.Drawing.Color;
using Graph=Microsoft.Msagl.Drawing.Graph;


namespace PAT.GUI.Forms.GraphDiff
{
    public partial class DifferenceDisplayForm : Form
    {
        //localized string for the button text display
        private System.ComponentModel.ComponentResourceManager resources;

        private SpecificationBase Spec;
        public Graph g;
        private LayerDirection Direction = LayerDirection.TB;

        private int ToolTipDisplayTime = 20000;


        private Control SimulatorViewerDockWindow;
        private DockableWindow EventWindow;


        private Microsoft.Msagl.Drawing.Graph LeftGraph;
        private Microsoft.Msagl.Drawing.Graph RightGraph;

        private SpecificationBase LeftSpec;
        private SpecificationBase RightSpec;

        private DifferenceDetailWindow DetailsWindow;


        #region "TreeListView"

        private readonly List<DataNode> m_Mapper = new List<DataNode>(64);
        private const TextFormatFlags MTff = TextFormatFlags.VerticalCenter | TextFormatFlags.WordEllipsis | TextFormatFlags.EndEllipsis | TextFormatFlags.SingleLine;
        private Rectangle m_Rect;
        private readonly System.Drawing.Color m_ColorFrom = SystemColors.HotTrack;
        private readonly System.Drawing.Color m_ColorTo = SystemColors.ControlLight;
        private readonly Blend m_Blend = new Blend();

        private DictionaryJ<string, object[]> configGraphMatchResults = new DictionaryJ<string, object[]>(); // [0] pairupgraph; [1] a List<PairUpCandidate> of selectedMatchedPairs

        /// <summary>
        /// Obtains all nodes.
        /// </summary>
        /// <param name="nds">The node list.</param>
        private void ObtainAllNodes(List<DataNode> nds)
        {
            foreach (DataNode dn in nds)
            {
                m_Mapper.Add(dn);
                if (dn.Expanded)
                {
                    ObtainAllNodes(dn.Children);
                }
            }
        }


        /// <summary>
        /// Makes the list view item.
        /// </summary>
        /// <param name="dn">The data node.</param>
        /// <returns></returns>
        private static ListViewItem MakeListViewItem(DataNode dn)
        {
            ListViewItem lvi = new ListViewItem();
            lvi.Text = dn.Name;
            //ListViewItem.ListViewSubItem lvsi1 = new ListViewItem.ListViewSubItem();
            //lvsi1.Text = dn.CountChildren.ToString();
            ListViewItem.ListViewSubItem lvsi2 = new ListViewItem.ListViewSubItem();
            lvsi2.Text = dn.CountChildren.ToString();
            ListViewItem.ListViewSubItem lvsi3 = new ListViewItem.ListViewSubItem();
            lvsi3.Text = dn.Comment;

            lvi.IndentCount = dn.Level;

            //lvi.SubItems.Add(lvsi1);
            lvi.SubItems.Add(lvsi2);
            lvi.SubItems.Add(lvsi3);

            if (dn.Expanded)
                lvi.StateImageIndex = 1;
            else if (dn.CountChildren > 0)
                lvi.StateImageIndex = 0;

            return lvi;
        }

        /// <summary>
        /// Retrieve virtual item.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.RetrieveVirtualItemEventArgs"/> instance containing the event data.</param>
        private void ListView1RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
        {
            e.Item = MakeListViewItem(m_Mapper[e.ItemIndex]);
        }


        /// <summary>
        /// Mouse click event handler.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
        private void ListView1MouseClick(object sender, MouseEventArgs e)
        {
            ExpandCollapseNodes(sender, e);
        }

        /// <summary>
        /// Mouse double click event handler.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
        private void ListView1MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ExpandCollapseNodes(sender, e);
        }

        /// <summary>
        /// Expands or collapse a node.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
        private void ExpandCollapseNodes(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                ListView lv = (ListView)sender;
                ListViewItem lvi = lv.GetItemAt(e.X, e.Y);
                if (lvi != null)
                {
                    // hack to draw first column correctly
                    lv.RedrawItems(lvi.Index, lvi.Index, true);

                    DataNode mbr = m_Mapper[lvi.Index];

                    int xfrom = lvi.IndentCount * 16;
                    int xto = xfrom + 16;

                    if ((e.X >= xfrom && e.X <= xto) || e.Clicks > 1)
                    {
                        if (mbr.CountChildren > 0)
                        {
                            mbr.Expanded = !mbr.Expanded;
                            lvi.Checked = !lvi.Checked;

                            PrepareNodes(lvi.Index, mbr.Expanded);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Prepares the nodes.
        /// </summary>
        /// <param name="pos">The pos.</param>
        /// <param name="add">if set to <c>true</c> [add].</param>
        private void PrepareNodes(int pos, bool add)
        {
            DataNode mbr = m_Mapper[pos];
            pos++;

            if (add)
            {
                PopulateDescendantMembers(ref pos, mbr);
            }
            else
            {
                int kids = ObtainExpandedChildrenCount(pos - 1);
                m_Mapper.RemoveRange(pos, kids);
            }

            this.ListView_Trace.VirtualListSize = m_Mapper.Count;
        }

        /// <summary>
        /// Populates the descendant members.
        /// </summary>
        /// <param name="pos">The pos.</param>
        /// <param name="mbr">The data node.</param>
        private void PopulateDescendantMembers(ref int pos, DataNode mbr)
        {
            foreach (DataNode m in mbr.Children)
            {
                m_Mapper.Insert(pos++, m);
                if (m.Expanded)
                {
                    PopulateDescendantMembers(ref pos, m);
                }
            }
        }

        /// <summary>
        /// Obtains the expanded children count.
        /// </summary>
        /// <param name="pos">The pos.</param>
        /// <returns></returns>
        private int ObtainExpandedChildrenCount(int pos)
        {
            int kids = 0;
            DataNode mi = m_Mapper[pos];
            int level = mi.Level;

            for (int i = pos + 1; i < m_Mapper.Count; i++, kids++)
            {
                DataNode mix = m_Mapper[i];
                int lvlx = mix.Level;
                if (lvlx <= level) break;
            }

            return kids;
        }


        private void DifferenceDisplayForm_Load(object sender, EventArgs e)
        {
            PropertyInfo aProp = typeof(ListView).GetProperty("DoubleBuffered", BindingFlags.NonPublic | BindingFlags.Instance);
            aProp.SetValue(this.ListView_Trace, true, null);
        }

        /// <summary>
        /// Creates the brush.
        /// </summary>
        /// <returns></returns>
        private LinearGradientBrush CreateBrush()
        {
            LinearGradientBrush br = new LinearGradientBrush(m_Rect, m_ColorFrom, m_ColorTo, 90f);
            br.Blend = m_Blend;
            return br;
        }



        /// <summary>
        /// Handles the DrawSubItem event of the listView1 control.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.DrawListViewSubItemEventArgs"/> instance containing the event data.</param>
        private void ListView1DrawSubItem(object sender, DrawListViewSubItemEventArgs e)
        {
            // calculate x offset from ident-level
            int xOffset = e.Item.IndentCount * 16;

            // calculate x position
            int xPos = e.Bounds.X + xOffset + 16;

            Rectangle r = e.Bounds;

            if (e.ColumnIndex == 0)
            {
                // drawing of first column, icon as well as text

                r = e.Bounds;
                r.Y += 1; r.Height -= 1; r.Width -= 1;
                //e.Graphics.FillRectangle(SystemBrushes.ControlLight, r);

                // set rectangle bounds for drawing of state-icon
                m_Rect.Height = 16;
                m_Rect.Width = 16;
                m_Rect.X = e.Bounds.X + xOffset;
                m_Rect.Y = e.Bounds.Y;

                if (e.Item.Checked)
                {
                    // draw expanded icon
                    e.Graphics.DrawImage(Common.Ultility.Ultility.GetImage("Open"), m_Rect);
                }
                else if (0 == e.Item.StateImageIndex)
                {
                    // draw collapsed icon
                    e.Graphics.DrawImage(Common.Ultility.Ultility.GetImage("Collaps"), m_Rect);
                }
                else
                {
                    // draw normal icon (for unexpandable items)
                    //e.Graphics.DrawImage(Common.Ultility.Ultility.GetImage("Collaps"), m_Rect);
                }

                // set rectangle bounds for drawing of item/subitem text
                m_Rect.Height = e.Bounds.Height;
                m_Rect.Width = e.Bounds.Width - xPos;
                m_Rect.X = xPos;
                m_Rect.Y = e.Bounds.Y;

                // draw item/subitem text
                if ((e.ItemState & ListViewItemStates.Selected) != 0)
                {
                    //LinearGradientBrush br = CreateBrush();
                    //e.Graphics.FillRectangle(br, m_Rect);

                    // draw selected item's text
                    TextRenderer.DrawText(e.Graphics, e.Item.Text, e.Item.Font, m_Rect, SystemColors.HotTrack, MTff);
                }
                else
                {
                    // draw unselected item's text
                    TextRenderer.DrawText(e.Graphics, e.Item.Text, e.Item.Font, m_Rect, e.Item.ForeColor, MTff);
                }
            }
            else
            {
                r.Y += 1; r.Height -= 1; r.Width -= 1;
                //e.Graphics.FillRectangle(SystemBrushes.ControlLight, r);

                // drawing of all other columns, e. g. drawing of subitems
                if ((e.ItemState & ListViewItemStates.Selected) != 0)
                {
                    //LinearGradientBrush br = CreateBrush();
                    //e.Graphics.FillRectangle(br, e.Bounds);

                    TextRenderer.DrawText(e.Graphics, e.SubItem.Text, e.Item.Font, e.Bounds, e.Item.ForeColor, MTff);
                }
                else
                {
                    TextRenderer.DrawText(e.Graphics, e.SubItem.Text, e.Item.Font, e.Bounds, e.Item.ForeColor, MTff);
                }
            }
        }

        /// <summary>
        /// Handles the DrawColumnHeader event of the listView1 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.DrawListViewColumnHeaderEventArgs"/> instance containing the event data.</param>
        private void listView1_DrawColumnHeader(object sender, DrawListViewColumnHeaderEventArgs e)
        {
            e.DrawDefault = true;
        }

        #endregion

        public DifferenceDisplayForm(Graph leftGraph, Graph rightGraph, SpecificationBase leftSpec, SpecificationBase rightSpec)
        {
            InitializeComponent();

            //initialize the data store window.
            //initialize the event window
            this.toolStripContainer1.ContentPanel.Controls.Remove(Panel_MatchResult);
            this.Panel_MatchResult.Dock = DockStyle.Fill;
            EventWindow = new DockableWindow();
            EventWindow.DockableAreas = DockAreas.DockBottom | DockAreas.Float;
            EventWindow.CloseButton = false;
            EventWindow.Controls.Add(Panel_MatchResult);

            InitializeResourceText();


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


            StatusLabel.Text = "Ready";

            LeftGraph = leftGraph;
            RightGraph = rightGraph;

            LeftSpec = leftSpec;
            RightSpec = rightSpec;

            ComboBox_MatchType.SelectedIndex = 0;
            ComboBox_MatchResult.SelectedIndex = 0;

            StatusLabel_LeftUnmatched.ForeColor = System.Drawing.Color.Purple;
            StatusLabel_RightUnmatched.ForeColor = System.Drawing.Color.Red;

            float[] myFactors = { .2f, .4f, .6f, .6f, .4f, .2f };
            float[] myPositions = { 0.0f, .2f, .4f, .6f, .8f, 1.0f };
            m_Blend.Factors = myFactors;
            m_Blend.Positions = myPositions;



            List<string> vars = new List<string>();
            Valuation val = leftSpec.GetEnvironment();
            if (val.Variables != null)
            {
                for (int i = 0; i < val.Variables._entries.Length; i++)
                {
                    StringDictionaryEntryWithKey<ExpressionValue> pair = val.Variables._entries[i];
                    if (pair != null)
                    {
                        vars.Add(pair.Key);
                    }
                }
            }

            val = rightSpec.GetEnvironment();
            if (val.Variables != null)
            {
                for (int i = 0; i < val.Variables._entries.Length; i++)
                {
                    StringDictionaryEntryWithKey<ExpressionValue> pair = val.Variables._entries[i];
                    if (pair != null)
                    {
                        if(vars.Contains(pair.Key))
                        {
                            ToolStripMenuItem menuaItem = new System.Windows.Forms.ToolStripMenuItem(pair.Key);
                            menuaItem.Checked = true;
                            menuaItem.CheckOnClick = true;
                            menuaItem.CheckState = System.Windows.Forms.CheckState.Checked;
                            menuaItem.Name = pair.Key;
                            this.Button_Variables.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { menuaItem });

                        }                        
                    }
                }
            }

            if(this.Button_Variables.DropDownItems.Count == 0)
            {
                this.Button_Variables.Visible = false;
            }
        }

 

        public void InitializeResourceText()
        {
            resources = new System.ComponentModel.ComponentResourceManager(typeof(SimulationForm));
            //DataStoreWindow.Icon = Icon.FromHandle(((Bitmap)resources.GetObject("Button_DataPane.Image")).GetHicon());
            EventWindow.Text = "Match Result"; //resources.GetString("InteractionPanel") ?? 
            //EventWindow.Icon = Icon.FromHandle(((Bitmap)resources.GetObject("Button_InteractionPane.Image")).GetHicon());
        }



        private void PrintErrorMsg(Exception ex)
        {
            EnableControls();
            if (ex is RuntimeException)
            {
                Ultility.LogRuntimeException(ex as RuntimeException);
                //MessageBox.Show("Runtime exception!\r\n" + ex.Message,// + "\r\n" + ex.StackTrace, "PAT", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (ex is System.OutOfMemoryException)
            {
                PAT.Common.Classes.Expressions.ExpressionClass.OutOfMemoryException outex = new PAT.Common.Classes.Expressions.ExpressionClass.OutOfMemoryException("");
                Ultility.LogRuntimeException(outex);
                //MessageBox.Show("Runtime exception!\r\n" + ex.Message,// + "\r\n" + ex.StackTrace, "PAT", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                Ultility.LogException(ex, Spec);
                //MessageBox.Show("Exception happened during the simulation: " + ex.Message, //"\r\n" + ex.StackTrace,"PAT", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }



        private void EnableControls()
        {
            this.Cursor = Cursors.Default;
            ComboBox_MatchType.Enabled = true;
            Button_Settings.Enabled = true;
            Button_Reset.Enabled = true;
            Button_MatchResult.Enabled = true;
            Button_GenerateGraph.Enabled = true;


            this.SimulatorViewer.SelectionChanged += new System.EventHandler(this.SimulatorViewer_SelectionChanged);
            this.SimulatorViewer.MouseDoubleClick += new MouseEventHandler(SimulatorViewer_MouseDoubleClick);

            //this.SimulatorViewer.BackwardEnabled = true;
            //this.SimulatorViewer.ForwardEnabled = true;
            //this.SimulatorViewer.NavigationVisible = true;
            //this.SimulatorViewer.PanButtonPressed = true;
            //this.SimulatorViewer.SaveButtonVisible = true;
            //Panel_ToolbarCover.Visible = false;
            this.Button_Direction.Visible = true;

            //this.SimulatorViewer.ToolBarIsVisible = true;
            Panel_Hiding.Visible = false;
            //SpecProcess.UnLockSharedData();
            //PAT.CSP.Ultility.Ultility.UnLockSharedData(SpecProcess);

        }

  

        private void DisableControls()
        {
            this.Cursor = Cursors.WaitCursor;
            ComboBox_MatchType.Enabled = false;
            Button_Settings.Enabled = false;
            Button_Reset.Enabled = false;
            Button_MatchResult.Enabled = false;
            Button_GenerateGraph.Enabled = false;


            this.SimulatorViewer.SelectionChanged -= new System.EventHandler(this.SimulatorViewer_SelectionChanged);
            this.SimulatorViewer.MouseDoubleClick -= new MouseEventHandler(SimulatorViewer_MouseDoubleClick);

            //this.SimulatorViewer.BackwardEnabled = false;
            //this.SimulatorViewer.ForwardEnabled = false;
            //this.SimulatorViewer.NavigationVisible = false;
            //this.SimulatorViewer.PanButtonPressed = false;
            //this.SimulatorViewer.SaveButtonVisible = false;
            //Panel_ToolbarCover.Visible = true;
            this.Button_Direction.Visible = false;
            //this.SimulatorViewer.ToolBarIsVisible = false;

            Panel_Hiding.Visible = true;
            //SpecProcess.LockSharedData();
            //PAT.CSP.Ultility.Ultility.LockSharedData(SpecProcess);
        }




        private void Button_Reset_Click(object sender, EventArgs e)
        {
            try
            {
                this.ListView_Trace.Items.Clear();

                if (this.SimulatorViewer.Graph != null)
                {
                    SimulatorViewer.Graph = new Graph();
                    SimulatorViewer.Validate();
                }

                StatusLabel.Text = "Ready";
            }
            catch (Exception ex)
            {
                PrintErrorMsg(ex);
            }
        }

        public static Graph CloneGraph(Graph graph)
        {
            Graph Graph = new Graph(graph.Label.Text);
            //Graph.GraphAttr.Orientation = graph.GraphAttr.Orientation;
            Graph.Attr.LayerDirection = graph.Attr.LayerDirection;

            Debug.Assert(graph.Edges.Count > 0);

            foreach (Edge edge in graph.Edges)
            {
                Edge newEdge;
                if (edge.Source == SimulationForm.INITIAL_STATE)
                {
                    newEdge = Graph.AddEdge(edge.Source, edge.LabelText, edge.TargetNode.LabelText);
                    newEdge.SourceNode.Attr.LineWidth = 0;
                    newEdge.SourceNode.Attr.Color = Color.White;
                }
                else
                {
                    newEdge = Graph.AddEdge(edge.SourceNode.LabelText, edge.LabelText, edge.TargetNode.LabelText);
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



        #region Event



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

                        ToolTip_Graph.Show(String.Format("edge " + selectedEdge.LabelText + " from {0} to {1}", selectedEdge.SourceNode.LabelText == "" ? "init" : selectedEdge.SourceNode.LabelText, selectedEdge.TargetNode.LabelText), SimulatorViewer, p, ToolTipDisplayTime);
                    }
                    else if (selectedObject is Node)
                    {
                        Node selectedNode = SimulatorViewer.SelectedObject as Node;
                        selectedObjectAttr = selectedNode.Attr.Clone();
                        selectedObjectColor = selectedNode.Label.FontColor;

                        if (selectedNode != null && selectedNode.Id != SimulationForm.INITIAL_STATE)
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
                            ToolTip_Graph.Show(String.Format("node {0}", selectedNode.UserData), SimulatorViewer, p, ToolTipDisplayTime);

                            //ToolTip_Graph.Show(String.Format("node {0}", selectedNode.UserData), SimulatorViewer, System.Windows.Forms.Control.MousePosition, ToolTipDisplayTime);
                        }
                    }
                }
                SimulatorViewer.Invalidate();


            }
        }


        void SimulatorViewer_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left && selectedObject != null)
            {
                //PairUpCandidate match = null;
                string key = "";
                if (selectedObject is Edge)
                {
                    //match = (selectedObject as Edge).UserData as PairUpCandidate;
                }
                else if (selectedObject is Node)
                {
                    //match = (selectedObject as Node).UserData as PairUpCandidate;
                    key = (selectedObject as Node).LabelText;
                }


                if (DetailsWindow == null || DetailsWindow.IsDisposed)
                {
                    DetailsWindow = new DifferenceDetailWindow();
                    DetailsWindow.Show(DockContainer, DockState.Float);
                }

                if(this.configGraphMatchResults.ContainsKey(key))
                {
                    DetailsWindow.ShowDetailedMatch(configGraphMatchResults[key]);
                }
                else
                {
                    DetailsWindow.ShowDetailedMatch(null);
                }
            }
        }

        #endregion



        private void Button_DataStorePane_CheckStateChanged(object sender, EventArgs e)
        {
            try
            {
                if (this.Button_MatchResult.Checked)
                {
                    if (EventWindow.Tag == null)
                    {
                        EventWindow.Tag = true;
                        EventWindow.Show(DockContainer, DockState.DockBottom);
                        EventWindow.DockPanel.DockBottomPortion = (100 / (double)this.Height);
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
            catch (Exception ex)
            {
                Common.Ultility.Ultility.LogException(ex, Spec);
            }

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
            this.ToolTipDisplayTime = int.Parse(button.Tag.ToString()) * 1000;
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


        public static Color LeftUnmatchColor = Color.Purple;
        public static Color RightUnmatchColor = Color.Red;

        private void Button_GenerateGraph_Click(object sender, EventArgs e)
        {
            try
            {
                DisableControls();

                PATModelComparison p = new PATModelComparison();
                List<string> Variables = new List<string>();
                foreach (ToolStripMenuItem item in this.Button_Variables.DropDownItems)
                {
                    if(item.Checked)
                    {
                        Variables.Add(item.Text);
                    }
                }
                
                List<PairUpCandidate> matches = p.execute(LeftGraph, RightGraph, Variables, this.ComboBox_MatchType.SelectedIndex == 1, LeftSpec, RightSpec, this.Button_MatchEventDetails.Checked, this.Button_MatchProcessParameters.Checked, this.Button_MatchStateStructure.Checked, this.Button_MatchIfguardExpression.Checked);

                Graph g = CloneGraph(LeftGraph);

                bool compareParameterizedSystem = (LeftGraph.UserData != null && (int)LeftGraph.UserData != -1) && (RightGraph.UserData != null && (int)RightGraph.UserData != -1);
                bool separateDifference = (this.ComboBox_MatchResult.SelectedIndex == 1);

                DictionaryJ<Node, string> mapLeftMatchedNodeToNewLabelText = new DictionaryJ<Node, string>();
                DictionaryJ<string, Node> mapLeftLabelTextToLeftNode = new DictionaryJ<string, Node>();
                DictionaryJ<string, PairUpCandidate> mapLeftLabelTextToMatchedPair = new DictionaryJ<string, PairUpCandidate>();
                foreach (DictionaryEntry map in g.NodeMap)
                {
                    Node node = map.Value as Node;
                    string nodeLabel = String.Format("[{0}] {1}", node.LabelText, node.UserData.ToString());

                    PairUpCandidate matchedPair = null;

                    foreach (PairUpCandidate match in matches)
                    {
                        string nodeID = match.left.getLabel();
                        if (nodeLabel.Equals(nodeID) && !match.isPairUpWithEpsilon())
                        {
                            mapLeftMatchedNodeToNewLabelText.put(node, node.LabelText + "/" + match.right.getLabel().Split(']')[0].Substring(1));
                            matchedPair = match;
                            break;
                        }
                    }

                    if (matchedPair == null)
                    {
                        if (node.LabelText != "")
                        {
                            if (!separateDifference)
                            {
                                node.Attr.Color = LeftUnmatchColor;
                            }
                        }
                    }
                    else
                    {
                        mapLeftLabelTextToLeftNode.put(node.LabelText, node);
                        mapLeftLabelTextToMatchedPair.put(node.LabelText, matchedPair);
                    }
                }

                DictionaryJ<string, string> mapRightLabelTextToLeftLabelText = new DictionaryJ<string, string>();
                DictionaryJ<Node, Node> mapLeftMatchedNodeToRightMatchedNode = new DictionaryJ<Node, Node>();
                foreach (DictionaryEntry map in RightGraph.NodeMap)
                {
                    Node node = map.Value as Node;
                    bool matched = false;
                    string nodeLabel = String.Format("[{0}] {1}", node.LabelText, node.UserData.ToString());

                    foreach (PairUpCandidate match in matches)
                    {
                        string nodeID = match.right.getLabel();
                        if (nodeLabel.Equals(nodeID) && !match.isPairUpWithEpsilon())
                        {
                            mapRightLabelTextToLeftLabelText.put(node.LabelText, match.left.getLabel().Split(']')[0].Substring(1));
                            matched = true;
                            break;
                        }
                    }

                    if (!matched)
                    {
                        if (node.LabelText != "")
                        {
                            Node d = g.AddNode("r" + node.LabelText);
                            d.Attr.Color = RightUnmatchColor;
                            d.UserData = "\r\n" + node.UserData;
                        }
                    }
                    else
                    {
                        string leftLabelText = mapRightLabelTextToLeftLabelText.get(node.LabelText);
                        Node leftNode = mapLeftLabelTextToLeftNode.get(leftLabelText);
                        mapLeftMatchedNodeToRightMatchedNode.put(leftNode, node);
                    }
                }

                HashSet<Edge> leftEdgesToBeAdded = new HashSet<Edge>();
                HashSet<Node> leftNodesToBeRemoved = new HashSet<Node>();
                DictionaryJ<string, Node> mapLeftLabelTextToDuplicateLeftNode = new DictionaryJ<string, Node>();
                foreach (Edge edge in g.Edges)
                {
                    PairUpCandidate matchedPair = null;
                    foreach (PairUpCandidate match in matches)
                    {
                        if (match.left is GraphEdge && !match.isPairUpWithEpsilon())
                        {
                            Node sourceNode = edge.SourceNode;
                            string srcNodeLabel = String.Format("[{0}] {1}", sourceNode.LabelText, sourceNode.UserData.ToString());

                            Node targetNode = edge.TargetNode;
                            string trgNodeLabel = String.Format("[{0}] {1}", targetNode.LabelText, targetNode.UserData.ToString());

                            GraphNode source = ((GraphEdge) match.left).Source();
                            GraphNode target = ((GraphEdge) match.left).Target();
                            if (srcNodeLabel.Equals(source.getLabel()) && trgNodeLabel.Equals(target.getLabel()))
                            {
                                matchedPair = match;
                                break;
                            }
                        }
                    }
                    if (matchedPair == null)
                    {
                        if (edge.SourceNode.LabelText != "")
                        {
                            if (!separateDifference)
                            {
                                edge.Attr.Color = LeftUnmatchColor;
                            }
                            else
                            {
                                leftEdgesToBeAdded.Add(edge);
                            }
                        }
                    }
                    else
                    {
                        if (!edge.LabelText.Equals(matchedPair.right.getLabel()))
                        {
                            edge.LabelText = edge.LabelText + "/" + matchedPair.right.getLabel();
                        }
                        //edge.UserData = matchCandidate; ??xingzc. what is used for??
                    }
                }

                foreach (Edge leftEdge in leftEdgesToBeAdded)
                {
                    Node duplicateSrcNode = mapLeftLabelTextToDuplicateLeftNode.get(leftEdge.SourceNode.LabelText);
                    if (duplicateSrcNode == null)
                    {
                        string newLabelText = mapLeftMatchedNodeToNewLabelText.get(leftEdge.SourceNode);
                        if (newLabelText != null)
                        {
                            duplicateSrcNode = g.AddNode("d" + leftEdge.SourceNode.LabelText);
                            mapLeftMatchedNodeToNewLabelText.put(duplicateSrcNode, "d" + newLabelText);
                            PairUpCandidate matchedPair = mapLeftLabelTextToMatchedPair.get(leftEdge.SourceNode.LabelText);
                            mapLeftLabelTextToMatchedPair.put(duplicateSrcNode.LabelText, matchedPair);
                        }
                        else
                        {
                            duplicateSrcNode = g.AddNode("l" + leftEdge.SourceNode.LabelText);
                            duplicateSrcNode.Attr.Color = LeftUnmatchColor;
                            leftNodesToBeRemoved.Add(leftEdge.SourceNode);
                        }
                        duplicateSrcNode.UserData = "\r\n" + leftEdge.SourceNode.UserData;
                        mapLeftLabelTextToDuplicateLeftNode.put(leftEdge.SourceNode.LabelText, duplicateSrcNode);
                        Node rightMatchedNode = mapLeftMatchedNodeToRightMatchedNode.get(leftEdge.SourceNode);
                        mapLeftMatchedNodeToRightMatchedNode.put(duplicateSrcNode, rightMatchedNode);
                    }

                    Node duplicateTrgNode = mapLeftLabelTextToDuplicateLeftNode.get(leftEdge.TargetNode.LabelText);
                    if (duplicateTrgNode == null)
                    {
                        string newLabelText = mapLeftMatchedNodeToNewLabelText.get(leftEdge.TargetNode);
                        if (newLabelText != null)
                        {
                            duplicateTrgNode = g.AddNode("d" + leftEdge.TargetNode.LabelText);
                            mapLeftMatchedNodeToNewLabelText.put(duplicateTrgNode, "d" + newLabelText);
                            PairUpCandidate matchedPair = mapLeftLabelTextToMatchedPair.get(leftEdge.TargetNode.LabelText);
                            mapLeftLabelTextToMatchedPair.put(duplicateTrgNode.LabelText, matchedPair);
                        }
                        else
                        {
                            duplicateTrgNode = g.AddNode("l" + leftEdge.TargetNode.LabelText);
                            duplicateTrgNode.Attr.Color = LeftUnmatchColor;
                            leftNodesToBeRemoved.add(leftEdge.TargetNode);
                        }
                        duplicateTrgNode.UserData = "\r\n" + leftEdge.TargetNode.UserData;
                        mapLeftLabelTextToDuplicateLeftNode.put(leftEdge.TargetNode.LabelText, duplicateTrgNode);
                        Node rightMatchedNode = mapLeftMatchedNodeToRightMatchedNode.get(leftEdge.TargetNode);
                        mapLeftMatchedNodeToRightMatchedNode.put(duplicateTrgNode, rightMatchedNode);
                    }

                    Edge newEdge = g.AddEdge(duplicateSrcNode.LabelText, leftEdge.LabelText, duplicateTrgNode.LabelText);
                    newEdge.Attr.Color = LeftUnmatchColor;
                }

                foreach (Edge edge in RightGraph.Edges)
                {
                    bool matched = false;
                    foreach (PairUpCandidate match in matches)
                    {

                        if (match.right is GraphEdge && !match.isPairUpWithEpsilon())
                        {
                            Node sourceNode = edge.SourceNode;
                            string srcNodeLabel = String.Format("[{0}] {1}", sourceNode.LabelText, sourceNode.UserData.ToString());

                            Node targetNode = edge.TargetNode;
                            string trgNodeLabel = String.Format("[{0}] {1}", targetNode.LabelText, targetNode.UserData.ToString());

                            GraphNode source = ((GraphEdge) match.right).Source();
                            GraphNode target = ((GraphEdge) match.right).Target();
                            if (srcNodeLabel.Equals(source.getLabel()) && trgNodeLabel.Equals(target.getLabel()))
                            {
                                matched = true;
                                break;
                            }
                        }
                    }
                    if (!matched)
                    {
                        if (edge.SourceNode.LabelText != "")
                        {
                            string srcLabelText = mapRightLabelTextToLeftLabelText.get(edge.SourceNode.LabelText);
                            if (srcLabelText != null)
                            {
                                if (separateDifference)
                                {
                                    Node duplicateSrcNode = mapLeftLabelTextToDuplicateLeftNode.get(srcLabelText);
                                    if (duplicateSrcNode == null)
                                    {
                                        Node leftNode = mapLeftLabelTextToLeftNode.get(srcLabelText);

                                        duplicateSrcNode = g.AddNode("d" + srcLabelText);
                                        duplicateSrcNode.UserData = leftNode.UserData; // edge.SourceNode.UserData;
                                        mapLeftLabelTextToDuplicateLeftNode.put(srcLabelText, duplicateSrcNode);
                                        mapLeftMatchedNodeToNewLabelText.put(duplicateSrcNode, "d" + srcLabelText + "/" + edge.SourceNode.LabelText);
                                        
                                        PairUpCandidate matchedPair = mapLeftLabelTextToMatchedPair.get(srcLabelText);
                                        mapLeftLabelTextToMatchedPair.put(duplicateSrcNode.LabelText, matchedPair);
                                        
                                        mapLeftMatchedNodeToRightMatchedNode.put(duplicateSrcNode, edge.SourceNode);
                                    }
                                    srcLabelText = duplicateSrcNode.LabelText;
                                }
                            }
                            else
                            {
                                srcLabelText = "r" + edge.SourceNode.LabelText;
                            }

                            string trgLabelText = mapRightLabelTextToLeftLabelText.get(edge.TargetNode.LabelText);
                            if (trgLabelText != null)
                            {
                                if (separateDifference)
                                {
                                    Node duplicateTrgNode = mapLeftLabelTextToDuplicateLeftNode.get(trgLabelText);
                                    if (duplicateTrgNode == null)
                                    {
                                        Node leftNode = mapLeftLabelTextToLeftNode.get(trgLabelText);

                                        duplicateTrgNode = g.AddNode("d" + trgLabelText);
                                        duplicateTrgNode.UserData = leftNode.UserData; // edge.TargetNode.UserData;
                                        mapLeftLabelTextToDuplicateLeftNode.put(trgLabelText, duplicateTrgNode);
                                        mapLeftMatchedNodeToNewLabelText.put(duplicateTrgNode, "d" + trgLabelText + "/" + edge.TargetNode.LabelText);

                                        PairUpCandidate matchedPair = mapLeftLabelTextToMatchedPair.get(trgLabelText);
                                        mapLeftLabelTextToMatchedPair.put(duplicateTrgNode.LabelText, matchedPair);

                                        mapLeftMatchedNodeToRightMatchedNode.put(duplicateTrgNode, edge.TargetNode);
                                    }
                                    trgLabelText = duplicateTrgNode.LabelText;
                                }
                            }
                            else
                            {
                                trgLabelText = "r" + edge.TargetNode.LabelText;
                            }

                            Edge newEdge = g.AddEdge(srcLabelText, edge.LabelText, trgLabelText);
                            newEdge.Attr.Color = RightUnmatchColor;
                        }
                    }
                }

                List<PAT.GenericDiff.graph.Graph> twoGraphs = new MyList<PAT.GenericDiff.graph.Graph>(2);
                foreach (KeyValuePair<Node, string> pair in mapLeftMatchedNodeToNewLabelText)
                {
                    Node leftNode = pair.Key;
                    Node rightNode = mapLeftMatchedNodeToRightMatchedNode.get(leftNode);

                    string oldLabel = leftNode.LabelText;
                    string newLabel = pair.Value;

                    if(!compareParameterizedSystem)
                    {
                        PairUpCandidate matchedPair = mapLeftLabelTextToMatchedPair.get(oldLabel);
                        PAT.GenericDiff.graph.Graph genericDiffLeftGraph = (PAT.GenericDiff.graph.Graph)((AbstractGraphElement)matchedPair.left).additionalInfo;
                        PAT.GenericDiff.graph.Graph genericDiffRightGraph = (PAT.GenericDiff.graph.Graph)((AbstractGraphElement)matchedPair.right).additionalInfo;
                        twoGraphs.Clear();
                        twoGraphs.Add(genericDiffLeftGraph);
                        twoGraphs.Add(genericDiffRightGraph);
                        configGraphMatchResults.put(newLabel, genericDiffLeftGraph.differ.comparisonHistory.get(twoGraphs));
                    }

                    leftNode.LabelText = newLabel;

                    leftNode.UserData = "\r\n" + leftNode.UserData.ToString() + rightNode.UserData.ToString();
                }

                // remove left unmatched nodes, since they are duplicated if separateDifference=true
                HashSet<Edge> leftEdgesToBeRemoved = new HashSet<Edge>();
                foreach (Node lefNode in leftNodesToBeRemoved)
                {
                    if (g.NodeMap.ContainsKey(lefNode.Id))
                    {
                        //remove node
                        g.NodeMap.Remove(lefNode.Id);
                        
                        //remove all edges referring to the deleted node
                        foreach (Edge edge in g.Edges)
                        {
                            if (edge.Source == lefNode.Id || edge.Target == lefNode.Id)
                            {
                                leftEdgesToBeRemoved.Add(edge);
                            }
                        }
                    }
                }
                
                /*
                 * leftEdgeToBeRemoved contains left-unmatched edges from/to left-unmatched nodes. 
                 * leftEdgesToBeAdded also contains such left-unmatched edges. In addition, 
                 * leftEdgesToBeAdded contains left-unmatched edges between two matched node pairs,
                 * which are not collected during the removal of left-unmatched nodes.Thus, need to
                 * add them to leftedgeToBeRemoved.
                 */
                leftEdgesToBeRemoved.addAll(leftEdgesToBeAdded);
                foreach (Edge leftEdgeToBeRemoved in leftEdgesToBeRemoved)
                {
                    g.Edges.Remove(leftEdgeToBeRemoved);
                }

                // after removing edges, there exist isolated nodes in graph. shall we remove them?
                /*if(separateDifference)
                {
                    List<Node> isolatedNodesToBeRemoved = new List<Node>();
                    foreach (DictionaryEntry map in g.NodeMap)
                    {
                        Node node = map.Value as Node;
                        if (node.Edges.isEmpty())
                        {
                            isolatedNodesToBeRemoved.Add(node);
                        }
                    }
                    foreach (Node isolatedNode in isolatedNodesToBeRemoved)
                    {
                        g.NodeMap.Remove(isolatedNode.Id);
                    }
                }*/

                this.SimulatorViewer.Graph = g;
                this.SimulatorViewer.Invalidate(false);

                StatusLabel.Text = "Graph Generated: " + (SimulatorViewer.Graph.NodeCount - 1) + " Nodes, " + (SimulatorViewer.Graph.EdgeCount - 1) + " Edges";

                //update the match list.
                m_Mapper.Clear();
                DataNode allMatch = new DataNode("Matched", "The node matches both of the graphs", 0, false);
                DataNode leftMatchOnly = new DataNode("Left Only", "The node appears only in the left graph", 0, false);
                DataNode rightMatchOnly = new DataNode("Right Only", "The node appears only in the right graph", 0, false);
                
                foreach (PairUpCandidate match in matches)
                {
                    if (match.enclosingPairupElement is GraphNode)
                    {
                        string leftstr = "null";
                        if (match.left != null && !(match.left is EpsilonGraphElement))
                        {
                            leftstr = match.left.getLabel(); //.ToString();
                        }
                        string rightstr = "null";
                        if (match.right != null && !(match.right is EpsilonGraphElement))
                        {
                            rightstr = match.right.getLabel(); //.ToString();
                        }

                        //todo: ly: how to tell whether a match is in matched or left only or right only
                        DataNode level1 = new DataNode(match.uniquelabel, leftstr + "  <-->  " + rightstr, 1, false);
                        
                        if(!match.isPairUpWithEpsilon())
                        {
                            allMatch.AddChild(level1);
                            level1.AddChild(new DataNode("left", leftstr, 2, false));
                            level1.AddChild(new DataNode("right", rightstr, 2, false));
                        }
                        else if (match.right is EpsilonGraphElement)
                        {
                            leftMatchOnly.AddChild(level1);
                            level1.AddChild(new DataNode("left", leftstr, 2, false));                            
                        }
                        else if(match.left is EpsilonGraphElement)
                        {
                            rightMatchOnly.AddChild(level1);
                            level1.AddChild(new DataNode("right", rightstr, 2, false));
                        }                      
                    }
                }

                List<DataNode> DataPool = new List<DataNode>() {allMatch, leftMatchOnly, rightMatchOnly};
                ObtainAllNodes(DataPool);

                ListView_Trace.VirtualListSize = m_Mapper.Count;
                ListView_Trace.VirtualMode = true;
                ListView_Trace.Invalidate();

                EnableControls();
            }
            catch (Exception ex)
            {
                PrintErrorMsg(ex);
            }
        }

        private void DifferenceDisplayForm_Shown(object sender, EventArgs e)
        {
            // Make this form the active form and make it TopMost
            this.TopMost = true;
            this.Focus();
            this.BringToFront();
            this.TopMost = false;
        }


    }
}