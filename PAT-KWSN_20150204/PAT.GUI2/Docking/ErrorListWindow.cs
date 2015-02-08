using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Fireball.Docking;
using PAT.Common;

namespace PAT.GUI.Docking
{
    public class ErrorListWindow : DockableWindow
    {
        
        private ToolStripContainer ToolStripContainer;
        private ToolStrip ToolStrip;
        private ToolStripButton Button_Error;
        public ListView ListView;
        private ColumnHeader columnHeader1;
        private ColumnHeader columnHeader2;
        private ColumnHeader columnHeader3;
        private ImageList imageList1;
        private IContainer components;
        private ToolStripButton Button_Warnings;

        private Dictionary<string, ParsingException> Warnings;
        private ColumnHeader columnHeader4;
        private ColumnHeader columnHeader5;
        private Dictionary<string, ParsingException> Errors;


        public ErrorListWindow()
        {
            InitializeComponent();

            this.DockableAreas = DockAreas.DockBottom | DockAreas.Float;
            Warnings = new Dictionary<string, ParsingException>();
            Errors = new Dictionary<string, ParsingException>();
        }

        public void Clear()
        {
            Warnings = new Dictionary<string, ParsingException>();
            Errors = new Dictionary<string, ParsingException>();

            FilterData();
        }


        public void AddWarnings(Dictionary<string, ParsingException> warnings)
        {
            foreach (KeyValuePair<string, ParsingException> pair in warnings)
            {
                if (!Warnings.ContainsKey(pair.Key))
                {
                    Warnings.Add(pair.Key, pair.Value);
                }    
            }
            

            FilterData();
        }

        public void AddErrors(Dictionary<string, ParsingException> errors)
        {
            foreach (KeyValuePair<string, ParsingException> pair in errors)
            {
                if (!Errors.ContainsKey(pair.Key))
                {
                    Errors.Add(pair.Key, pair.Value);
                }
            }


            FilterData();
        }

        public void InsertError(ParsingException warning)
        {
            string key = warning.Message;
            if (!Errors.ContainsKey(key))
            {
                Errors.Add(key, warning);
                FilterData();
            }            
        }
        
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ErrorListWindow));
            this.ToolStripContainer = new System.Windows.Forms.ToolStripContainer();
            this.ListView = new System.Windows.Forms.ListView();
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.ToolStrip = new System.Windows.Forms.ToolStrip();
            this.Button_Error = new System.Windows.Forms.ToolStripButton();
            this.Button_Warnings = new System.Windows.Forms.ToolStripButton();
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
            this.columnHeader3,
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader4,
            this.columnHeader5});
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
            // columnHeader3
            // 
            resources.ApplyResources(this.columnHeader3, "columnHeader3");
            // 
            // columnHeader1
            // 
            resources.ApplyResources(this.columnHeader1, "columnHeader1");
            // 
            // columnHeader2
            // 
            resources.ApplyResources(this.columnHeader2, "columnHeader2");
            // 
            // columnHeader4
            // 
            resources.ApplyResources(this.columnHeader4, "columnHeader4");
            // 
            // columnHeader5
            // 
            resources.ApplyResources(this.columnHeader5, "columnHeader5");
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
            this.Button_Error,
            this.Button_Warnings});
            this.ToolStrip.Name = "ToolStrip";
            // 
            // Button_Error
            // 
            this.Button_Error.Checked = true;
            this.Button_Error.CheckOnClick = true;
            this.Button_Error.CheckState = System.Windows.Forms.CheckState.Checked;
            resources.ApplyResources(this.Button_Error, "Button_Error");
            this.Button_Error.Name = "Button_Error";
            this.Button_Error.CheckStateChanged += new System.EventHandler(this.Button_Error_CheckStateChanged);
            // 
            // Button_Warnings
            // 
            this.Button_Warnings.Checked = true;
            this.Button_Warnings.CheckOnClick = true;
            this.Button_Warnings.CheckState = System.Windows.Forms.CheckState.Checked;
            resources.ApplyResources(this.Button_Warnings, "Button_Warnings");
            this.Button_Warnings.Name = "Button_Warnings";
            this.Button_Warnings.CheckStateChanged += new System.EventHandler(this.Button_Warnings_CheckStateChanged);
            // 
            // ErrorListWindow
            // 
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.ToolStripContainer);
            this.Name = "ErrorListWindow";
            this.ToolStripContainer.ContentPanel.ResumeLayout(false);
            this.ToolStripContainer.TopToolStripPanel.ResumeLayout(false);
            this.ToolStripContainer.TopToolStripPanel.PerformLayout();
            this.ToolStripContainer.ResumeLayout(false);
            this.ToolStripContainer.PerformLayout();
            this.ToolStrip.ResumeLayout(false);
            this.ToolStrip.PerformLayout();
            this.ResumeLayout(false);

        }

        private void Button_Error_CheckStateChanged(object sender, EventArgs e)
        {
            FilterData();
        }

        private void Button_Warnings_CheckStateChanged(object sender, EventArgs e)
        {
            FilterData();
        }

        private void FilterData()
        {
            ListView.Items.Clear();
            
            if(Button_Error.Checked)
            {
                foreach (ParsingException warning in Errors.Values)
                {
                    string[] data = null;

                    if (warning.Line > 0)
                    {
                        data = new string[] { "", (ListView.Items.Count + 1).ToString(), "Error at line " + warning.Line + " column " + warning.CharPositionInLine + " for '" + warning.Text + "': " + warning.Message, warning.DisplayFileName, warning.NodeName };
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(warning.Text))
                        {
                            data = new string[] { "", (ListView.Items.Count + 1).ToString(), "Error: " + warning.Message, warning.DisplayFileName, warning.NodeName };
                        }
                        else
                        {
                            data = new string[] { "", (ListView.Items.Count + 1).ToString(), "Error for '" + warning.Text + "': " + warning.Message, warning.DisplayFileName, warning.NodeName };
                        }
                    }
                    
                    ListViewItem item = new ListViewItem(data);
                    item.ImageIndex = 1;
                    item.Tag = warning;
                    ListView.Items.Add(item);
                }
            }

            if(Button_Warnings.Checked)
            {
                foreach (ParsingException warning in Warnings.Values)
                {
                    string[] data = null;

                    if (warning.Line > 0)
                    {
                        data = new string[] { "", (ListView.Items.Count + 1).ToString(), "Warning at line " + warning.Line + " column " + warning.CharPositionInLine + " for '" + warning.Text + "': " + warning.Message, warning.DisplayFileName, warning.NodeName };
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(warning.Text))
                        {
                            data = new string[] { "", (ListView.Items.Count + 1).ToString(), "Warning: " + warning.Message, warning.DisplayFileName, warning.NodeName };
                        }
                        else
                        {
                            data = new string[] { "", (ListView.Items.Count + 1).ToString(), "Warning for '" + warning.Text + "': " + warning.Message, warning.DisplayFileName, warning.NodeName };
                        }
                    }

                    ListViewItem item = new ListViewItem(data);
                    item.ImageIndex = 0;
                    item.Tag = warning;
                    ListView.Items.Add(item);
                }
            }
        }
    }
}