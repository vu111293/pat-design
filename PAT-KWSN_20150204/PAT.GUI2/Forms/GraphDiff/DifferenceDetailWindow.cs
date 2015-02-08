using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;
using Fireball.Docking;
using Microsoft.Msagl.Drawing;
using PAT.GenericDiff.diff;
using PAT.GenericDiff.graph;
using Graph=Microsoft.Msagl.Drawing.Graph;

namespace PAT.GUI.Forms.GraphDiff
{
    public class DifferenceDetailWindow : DockableWindow
    {
        private StatusStrip statusStrip1;
        private ToolStripStatusLabel StatusLabel_Status;
        public Microsoft.Msagl.GraphViewerGdi.GViewer SimulatorViewer;

        private ToolStripContainer ToolStripContainer;


        public DifferenceDetailWindow()
        {
            InitializeComponent();

            this.DockableAreas = DockAreas.DockBottom | DockAreas.Float;
        }

        public void ShowDetailedMatch(object[] data)
        {
            if (data == null)
            {
                StatusLabel_Status.Text = "No Match Data";
                SimulatorViewer.Graph = new Graph();
                SimulatorViewer.Validate();
                return;
            }

            Graph graph = new Graph();

            List<PairUpCandidate> matches = data[1] as List<PairUpCandidate>;

            foreach (PairUpCandidate nodePair in matches)
            {
                if (nodePair.enclosingPairupElement is GraphNode)
                {
                    AbstractGraphElement left = nodePair.left as AbstractGraphElement;
                    AbstractGraphElement right = nodePair.right as AbstractGraphElement;

                    if (!nodePair.isPairUpWithEpsilon()) // matched nodes
                    {
                        graph.AddNode(left.additionalInfo + "/" + right.additionalInfo);
                    }
                    else // unmatched nodes
                    {
                        if (!(nodePair.left is EpsilonGraphElement))
                        {
                            Node node = graph.AddNode("l" + left.additionalInfo.ToString());
                            node.Attr.Color = DifferenceDisplayForm.LeftUnmatchColor;
                        }
                        if (!(nodePair.right is EpsilonGraphElement))
                        {
                            Node node = graph.AddNode("r" + right.additionalInfo.ToString());
                            node.Attr.Color = DifferenceDisplayForm.RightUnmatchColor;
                        }
                    }
                }
            }

            foreach (PairUpCandidate edgePair in matches)
            {
                string srcNodePairLabel = "";
                string trgNodePairLabel = "";

                if (edgePair.enclosingPairupElement is GraphEdge)
                {
                    PairUpGraphNode sourceNodePair = (edgePair.enclosingPairupElement as GraphEdge).Source() as PairUpGraphNode;
                    PairUpGraphNode targetNodePair = (edgePair.enclosingPairupElement as GraphEdge).Target() as PairUpGraphNode;
                    PairUpCandidate srcPairUpCandidate = sourceNodePair.candidate;
                    PairUpCandidate trgPairUpCandidate = targetNodePair.candidate;

                    AbstractGraphElement leftNode = srcPairUpCandidate.left as AbstractGraphElement;
                    AbstractGraphElement rightNode = srcPairUpCandidate.right as AbstractGraphElement;
                    if (!srcPairUpCandidate.isPairUpWithEpsilon()) // matched nodes
                    {
                        Debug.Assert(leftNode != null && rightNode != null);
                        srcNodePairLabel  = leftNode.additionalInfo + "/" + rightNode.additionalInfo;
                    }
                    else // unmatched nodes
                    {
                        if (!(srcPairUpCandidate.left is EpsilonGraphElement))
                        {
                            Debug.Assert(leftNode != null);
                            srcNodePairLabel = "l" + leftNode.additionalInfo.ToString();
                        }
                        if (!(srcPairUpCandidate.right is EpsilonGraphElement))
                        {
                            Debug.Assert(rightNode != null);
                            srcNodePairLabel = "r" + rightNode.additionalInfo.ToString();
                        }
                    }

                    leftNode = trgPairUpCandidate.left as AbstractGraphElement;
                    rightNode = trgPairUpCandidate.right as AbstractGraphElement;
                    if (!trgPairUpCandidate.isPairUpWithEpsilon()) // matched nodes
                    {
                        Debug.Assert(leftNode != null && rightNode != null);
                        trgNodePairLabel  = leftNode.additionalInfo + "/" + rightNode.additionalInfo;
                    }
                    else // unmatched nodes
                    {
                        if (!(trgPairUpCandidate.left is EpsilonGraphElement))
                        {
                            Debug.Assert(leftNode != null);
                            trgNodePairLabel = "l" + leftNode.additionalInfo.ToString();
                        }
                        if (!(trgPairUpCandidate.right is EpsilonGraphElement))
                        {
                            Debug.Assert(rightNode != null);
                            trgNodePairLabel = "r" + rightNode.additionalInfo.ToString();
                        }
                    }

                    /*srcNodePairLabel = ((srcPairUpCandidate.left is AbstractGraphElement) ? (srcPairUpCandidate.left as AbstractGraphElement).additionalInfo : "") + "/" +
              ((srcPairUpCandidate.right is AbstractGraphElement) ? (srcPairUpCandidate.right as AbstractGraphElement).additionalInfo : "");
                    srcNodePairLabel = srcNodePairLabel.Trim('/');

                    trgNodePairLabel = ((trgPairUpCandidate.left is AbstractGraphElement) ? (trgPairUpCandidate.left as AbstractGraphElement).additionalInfo : "") + "/" +
                                  ((trgPairUpCandidate.right is AbstractGraphElement) ? (trgPairUpCandidate.right as AbstractGraphElement).additionalInfo : "");

                    trgNodePairLabel = trgNodePairLabel.Trim('/');*/


                    if (!edgePair.isPairUpWithEpsilon()) // matched edges, src/trg must be matched node pairs
                    {
                        if (edgePair.left.getLabel() == "" && edgePair.right.getLabel() == "")
                        {
                            graph.AddEdge(srcNodePairLabel, trgNodePairLabel);    
                        }
                        else
                        {
                            graph.AddEdge(srcNodePairLabel, edgePair.left.getLabel() + "/" + edgePair.right.getLabel(), trgNodePairLabel);    
                        }
                    }
                    else // unmatched nodes
                    {
                        if (!(edgePair.left is EpsilonGraphElement))
                        {
                            Edge edge = graph.AddEdge(srcNodePairLabel, edgePair.left.getLabel(), trgNodePairLabel);
                            edge.Attr.Color = DifferenceDisplayForm.LeftUnmatchColor;
                        }
                        else
                        {
                            Edge edge = graph.AddEdge(srcNodePairLabel, edgePair.right.getLabel(), trgNodePairLabel);
                            edge.Attr.Color = DifferenceDisplayForm.RightUnmatchColor;
                        }
                    }
                }
            }

            this.SimulatorViewer.Graph = graph;
            this.SimulatorViewer.Invalidate(false);

            StatusLabel_Status.Text = "Graph Generated: " + (SimulatorViewer.Graph.NodeCount - 1) + " Nodes, " +
                                      (SimulatorViewer.Graph.EdgeCount - 1) + " Edges";

        }


        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DifferenceDetailWindow));
            this.ToolStripContainer = new System.Windows.Forms.ToolStripContainer();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.StatusLabel_Status = new System.Windows.Forms.ToolStripStatusLabel();
            this.SimulatorViewer = new Microsoft.Msagl.GraphViewerGdi.GViewer();
            this.ToolStripContainer.BottomToolStripPanel.SuspendLayout();
            this.ToolStripContainer.ContentPanel.SuspendLayout();
            this.ToolStripContainer.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // ToolStripContainer
            // 
            // 
            // ToolStripContainer.BottomToolStripPanel
            // 
            this.ToolStripContainer.BottomToolStripPanel.Controls.Add(this.statusStrip1);
            // 
            // ToolStripContainer.ContentPanel
            // 
            this.ToolStripContainer.ContentPanel.Controls.Add(this.SimulatorViewer);
            resources.ApplyResources(this.ToolStripContainer.ContentPanel, "ToolStripContainer.ContentPanel");
            resources.ApplyResources(this.ToolStripContainer, "ToolStripContainer");
            this.ToolStripContainer.LeftToolStripPanelVisible = false;
            this.ToolStripContainer.Name = "ToolStripContainer";
            this.ToolStripContainer.RightToolStripPanelVisible = false;
            // 
            // statusStrip1
            // 
            resources.ApplyResources(this.statusStrip1, "statusStrip1");
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.StatusLabel_Status});
            this.statusStrip1.Name = "statusStrip1";
            // 
            // StatusLabel_Status
            // 
            this.StatusLabel_Status.Name = "StatusLabel_Status";
            resources.ApplyResources(this.StatusLabel_Status, "StatusLabel_Status");
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
            // 
            // DifferenceDetailWindow
            // 
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.ToolStripContainer);
            this.Name = "DifferenceDetailWindow";
            this.ShowIcon = false;
            this.ToolStripContainer.BottomToolStripPanel.ResumeLayout(false);
            this.ToolStripContainer.BottomToolStripPanel.PerformLayout();
            this.ToolStripContainer.ContentPanel.ResumeLayout(false);
            this.ToolStripContainer.ResumeLayout(false);
            this.ToolStripContainer.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);

        }
    }
}