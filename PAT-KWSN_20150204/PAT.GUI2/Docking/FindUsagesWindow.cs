using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using Fireball.Docking;
using PAT.Common;
using PAT.Common.Classes.ModuleInterface;

namespace PAT.GUI.Docking
{
    public class FindUsagesWindow : DockableWindow
    {
        //public EditorTabItem Tab;
        private ToolStripContainer ToolStripContainer;
        private ImageList imageList1;
        private IContainer components;

        public ListView ListView;
        private ColumnHeader columnHeader1;
        private ColumnHeader columnHeader2;
        private ToolStrip ToolStrip;
        public ToolStripButton Button_Refresh;
        private ToolStripLabel ToolStripLabel_Usage;

        public SpecificationBase Specification;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripButton Button_Clear;
        private ColumnHeader columnHeader3;
        private ColumnHeader columnHeader4;
        private ColumnHeader Definition;
        public string Term;

        public FindUsagesWindow()
        {
            InitializeComponent();

            this.DockableAreas = DockAreas.DockBottom | DockAreas.Float;
        }

   
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FindUsagesWindow));
            this.ToolStripContainer = new System.Windows.Forms.ToolStripContainer();
            this.ListView = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Definition = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.ToolStrip = new System.Windows.Forms.ToolStrip();
            this.ToolStripLabel_Usage = new System.Windows.Forms.ToolStripLabel();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.Button_Clear = new System.Windows.Forms.ToolStripButton();
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
            this.ToolStripContainer.ContentPanel.Controls.Add(this.ListView);
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
            // ListView
            // 
            this.ListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3,
            this.Definition,
            this.columnHeader4});
            resources.ApplyResources(this.ListView, "ListView");
            this.ListView.FullRowSelect = true;
            this.ListView.GridLines = true;
            this.ListView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.ListView.HideSelection = false;
            this.ListView.LargeImageList = this.imageList1;
            this.ListView.MultiSelect = false;
            this.ListView.Name = "ListView";
            this.ListView.SmallImageList = this.imageList1;
            this.ListView.UseCompatibleStateImageBehavior = false;
            this.ListView.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            resources.ApplyResources(this.columnHeader1, "columnHeader1");
            // 
            // columnHeader2
            // 
            resources.ApplyResources(this.columnHeader2, "columnHeader2");
            // 
            // columnHeader3
            // 
            resources.ApplyResources(this.columnHeader3, "columnHeader3");
            // 
            // Definition
            // 
            resources.ApplyResources(this.Definition, "Definition");
            // 
            // columnHeader4
            // 
            resources.ApplyResources(this.columnHeader4, "columnHeader4");
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "warning.ico");
            this.imageList1.Images.SetKeyName(1, "redcross.ico");
            // 
            // ToolStrip
            // 
            resources.ApplyResources(this.ToolStrip, "ToolStrip");
            this.ToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ToolStripLabel_Usage,
            this.toolStripSeparator1,
            this.Button_Clear,
            this.Button_Refresh});
            this.ToolStrip.Name = "ToolStrip";
            // 
            // ToolStripLabel_Usage
            // 
            this.ToolStripLabel_Usage.Name = "ToolStripLabel_Usage";
            resources.ApplyResources(this.ToolStripLabel_Usage, "ToolStripLabel_Usage");
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            resources.ApplyResources(this.toolStripSeparator1, "toolStripSeparator1");
            // 
            // Button_Clear
            // 
            this.Button_Clear.Image = global::PAT.GUI.Properties.Resources.Clear;
            resources.ApplyResources(this.Button_Clear, "Button_Clear");
            this.Button_Clear.Name = "Button_Clear";
            this.Button_Clear.Click += new System.EventHandler(this.Button_Clear_Click);
            // 
            // Button_Refresh
            // 
            resources.ApplyResources(this.Button_Refresh, "Button_Refresh");
            this.Button_Refresh.Name = "Button_Refresh";
            // 
            // FindUsagesWindow
            // 
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.ToolStripContainer);
            this.Name = "FindUsagesWindow";
            this.ToolStripContainer.ContentPanel.ResumeLayout(false);
            this.ToolStripContainer.TopToolStripPanel.ResumeLayout(false);
            this.ToolStripContainer.TopToolStripPanel.PerformLayout();
            this.ToolStripContainer.ResumeLayout(false);
            this.ToolStripContainer.PerformLayout();
            this.ToolStrip.ResumeLayout(false);
            this.ToolStrip.PerformLayout();
            this.ResumeLayout(false);

        }


        public void FillData(string name, List<ParsingException> usages, SpecificationBase Spec)
        {
            ListView.Enabled = true;
            Term = name;
            ListView.Items.Clear();
            
            if (Spec.DeclaritionTable.ContainsKey(name))
            {
                ToolStripLabel_Usage.Text = "Usages of " + Spec.DeclaritionTable[name].DeclarationType.ToString() + " \"" + name + "\"";


                ParsingException warning = Spec.DeclaritionTable[name].DeclarationToken;
                string definition = "";
                if(warning.ContainingDefinition != null)
                {
                    definition = warning.ContainingDefinition.Text;
                }

                string[] data = new string[] { (ListView.Items.Count + 1).ToString(), "Declaration at " + warning.Line + " column " + warning.CharPositionInLine + " for '" + warning.Text + "'", warning.Source, definition, warning.DisplayFileName };

                ListViewItem item = new ListViewItem(data);
                item.Tag = warning;
                ListView.Items.Add(item);
            }
            else
            {
                ToolStripLabel_Usage.Text = "Usages of \"" + name + "\"";
                return;
            }

           

            foreach (ParsingException usage in usages)
            {
                if (usage.Line > 0)
                {
                    string definition = "";
                    if (usage.ContainingDefinition != null)
                    {
                        definition = usage.ContainingDefinition.Text;
                    }

                    string[] data = new string[] { (ListView.Items.Count + 1).ToString(), "Usage found at " + usage.Line + " column " + usage.CharPositionInLine + " for '" + usage.Text + "'", usage.Source, definition, usage.DisplayFileName };
                    ListViewItem item = new ListViewItem(data);
                    item.Tag = usage;
                    ListView.Items.Add(item);
                }
            }
        }

        private void Button_Clear_Click(object sender, EventArgs e)
        {
            Term = "";
            ToolStripLabel_Usage.Text = "";
            this.ListView.Enabled = true;
            this.ListView.Items.Clear();
        }
    }
}