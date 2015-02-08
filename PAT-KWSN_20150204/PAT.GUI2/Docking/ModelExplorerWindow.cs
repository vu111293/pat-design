using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Fireball.Docking;
using PAT.Common.Classes.LTS;
using PAT.Common.Classes.ModuleInterface;

namespace PAT.GUI.Docking
{
    public class ModelExplorerWindow : DockableWindow
    {
        private ToolStripContainer ToolStripContainer;
        private ToolStrip ToolStrip;
        public ToolStripButton Button_Refresh;
        public ImageList imageList1;
        private System.ComponentModel.IContainer components;
        public TreeView TreeView_Model;

        public SpecificationBase Specification;
        

        public ModelExplorerWindow()
        {
            InitializeComponent();
            this.DockableAreas = DockAreas.DockRight | DockAreas.Float;

            TreeView_Model.StateImageList = this.imageList1;
            foreach (TreeNode node in TreeView_Model.Nodes)
            {
                node.StateImageIndex = node.Index;
                //node.ImageIndex = node.Index;
            }
        }

        public void DisplayTree(SpecificationBase Spec)
        {
            try
            {
                Specification = Spec;

                foreach (TreeNode node in TreeView_Model.Nodes)
                {
                    node.Nodes.Clear();
                }

                if (Spec != null)
                {

                    foreach (KeyValuePair<string, Declaration> pair in Spec.DeclaritionTable)
                    {
                        try
                        {
                            TreeNode node = this.TreeView_Model.Nodes[pair.Value.DeclarationType.ToString()].Nodes.Add(pair.Key);

                            node.Tag = pair.Value.DeclarationToken;
                        }
                        catch (Exception)
                        {
                        }
                    }

                    TreeView_Model.ExpandAll();
                }
            }
            catch (Exception)
            {
                
            }
        }

 
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ModelExplorerWindow));
            this.ToolStripContainer = new System.Windows.Forms.ToolStripContainer();
            this.TreeView_Model = new System.Windows.Forms.TreeView();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.ToolStrip = new System.Windows.Forms.ToolStrip();
            this.Button_Refresh = new System.Windows.Forms.ToolStripButton();
            this.ToolStripContainer.ContentPanel.SuspendLayout();
            this.ToolStripContainer.TopToolStripPanel.SuspendLayout();
            this.ToolStripContainer.SuspendLayout();
            this.ToolStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // ToolStripContainer
            // 
            this.ToolStripContainer.BottomToolStripPanelVisible = false;
            // 
            // ToolStripContainer.ContentPanel
            // 
            this.ToolStripContainer.ContentPanel.Controls.Add(this.TreeView_Model);
            resources.ApplyResources(this.ToolStripContainer.ContentPanel, "ToolStripContainer.ContentPanel");
            resources.ApplyResources(this.ToolStripContainer, "ToolStripContainer");
            this.ToolStripContainer.LeftToolStripPanelVisible = false;
            this.ToolStripContainer.Name = "ToolStripContainer";
            this.ToolStripContainer.RightToolStripPanelVisible = false;
            // 
            // ToolStripContainer.TopToolStripPanel
            // 
            this.ToolStripContainer.TopToolStripPanel.Controls.Add(this.ToolStrip);
            // 
            // TreeView_Model
            // 
            resources.ApplyResources(this.TreeView_Model, "TreeView_Model");
            this.TreeView_Model.HideSelection = false;
            this.TreeView_Model.Name = "TreeView_Model";
            this.TreeView_Model.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            ((System.Windows.Forms.TreeNode)(resources.GetObject("TreeView_Model.Nodes"))),
            ((System.Windows.Forms.TreeNode)(resources.GetObject("TreeView_Model.Nodes1"))),
            ((System.Windows.Forms.TreeNode)(resources.GetObject("TreeView_Model.Nodes2"))),
            ((System.Windows.Forms.TreeNode)(resources.GetObject("TreeView_Model.Nodes3"))),
            ((System.Windows.Forms.TreeNode)(resources.GetObject("TreeView_Model.Nodes4")))});
            this.TreeView_Model.StateImageList = this.imageList1;
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "Constants");
            this.imageList1.Images.SetKeyName(1, "Variable");
            this.imageList1.Images.SetKeyName(2, "Channel");
            this.imageList1.Images.SetKeyName(3, "Process");
            this.imageList1.Images.SetKeyName(4, "Declare");
            this.imageList1.Images.SetKeyName(5, "Enum");
            this.imageList1.Images.SetKeyName(6, "Events");
            this.imageList1.Images.SetKeyName(7, "key1.png");
            this.imageList1.Images.SetKeyName(8, "");
            this.imageList1.Images.SetKeyName(9, "variable");
            this.imageList1.Images.SetKeyName(10, "keyword");
            this.imageList1.Images.SetKeyName(11, "define");
            // 
            // ToolStrip
            // 
            resources.ApplyResources(this.ToolStrip, "ToolStrip");
            this.ToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.Button_Refresh});
            this.ToolStrip.Name = "ToolStrip";
            // 
            // Button_Refresh
            // 
            this.Button_Refresh.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.Button_Refresh.Image = global::PAT.GUI.Properties.Resources.ProjectBrowser_Toolbar_Refresh;
            resources.ApplyResources(this.Button_Refresh, "Button_Refresh");
            this.Button_Refresh.Name = "Button_Refresh";
            // 
            // ModelExplorerWindow
            // 
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.ToolStripContainer);
            this.Name = "ModelExplorerWindow";
            this.ToolStripContainer.ContentPanel.ResumeLayout(false);
            this.ToolStripContainer.TopToolStripPanel.ResumeLayout(false);
            this.ToolStripContainer.TopToolStripPanel.PerformLayout();
            this.ToolStripContainer.ResumeLayout(false);
            this.ToolStripContainer.PerformLayout();
            this.ToolStrip.ResumeLayout(false);
            this.ToolStrip.PerformLayout();
            this.ResumeLayout(false);

        }
    }
}