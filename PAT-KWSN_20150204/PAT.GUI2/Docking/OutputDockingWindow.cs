using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Fireball.Docking;
using PAT.GUI.Properties;

namespace PAT.GUI.Docking
{
    public class OutputDockingWindow : DockableWindow
    {
        
        private ToolStripContainer ToolStripContainer;
        private ToolStrip ToolStrip;
        private ToolStripButton Button_Clear;
        private ToolStripButton Button_SaveAs;
        private RichTextBox TextBox_Content;

        public RichTextBox TextBox
        {
            get
            {
               return this.TextBox_Content;
            }

        }
        
        public OutputDockingWindow()
        {
            InitializeComponent();

            this.DockableAreas = DockAreas.DockBottom | DockAreas.Float;
            this.TextBox_Content.TextChanged += new EventHandler(TextBox_TextChanged);

            //TextBox = new RichTextBox();
            
            //TextBox.Dock = DockStyle.Fill;
            //TextBox.Font = new Font("Courier New", 8);

            //ComponentResourceManager resources = new ComponentResourceManager(typeof(OutputDockingWindow));
            //this.toolStrip1 = new ToolStrip();
            //this.ClearToolStripButton = new ToolStripButton();
            //this.ExportToolStripButton = new ToolStripButton();
            //// 
            //// toolStrip1
            //// 
            //this.toolStrip1.Items.AddRange(new ToolStripItem[] {
            //                                                                            this.ClearToolStripButton,
            //                                                                            this.ExportToolStripButton});
            //this.toolStrip1.Location = new Point(0, 2);
            //this.toolStrip1.Name = "toolStrip1";
            //this.toolStrip1.Size = new Size(255, 25);
            //this.toolStrip1.TabIndex = 0;
            //this.toolStrip1.Text = "toolStrip1";
            //// 
            //// ClearToolStripButton
            //// 
            //this.ClearToolStripButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            //this.ClearToolStripButton.Image = Resources.Clear;
            //this.ClearToolStripButton.ImageTransparentColor = Color.Magenta;
            //this.ClearToolStripButton.Name = "ClearToolStripButton";
            //this.ClearToolStripButton.Size = new Size(23, 22);
            //this.ClearToolStripButton.Text = "Clear";
            //this.ClearToolStripButton.Click += new EventHandler(this.ClearToolStripButton_Click);
            //// 
            //// ExportToolStripButton
            //// 
            //this.ExportToolStripButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            //this.ExportToolStripButton.Image = Resources.save_as;
            //this.ExportToolStripButton.ImageTransparentColor = Color.Magenta;
            //this.ExportToolStripButton.Name = "ExportToolStripButton";
            //this.ExportToolStripButton.Size = new Size(23, 22);
            //this.ExportToolStripButton.Text = "Export";
            //this.ExportToolStripButton.Click += new EventHandler(this.ExportToolStripButton_Click);
            //// 
            //// OutputWindow
            //// 
            //this.Controls.Add(TextBox);
            //this.Controls.Add(this.toolStrip1);

            //this.Icon = ((Icon)(resources.GetObject("$this.Icon")));

            //this.Text = "Output";
            ////this.CloseButton = false;
            
        }

        void TextBox_TextChanged(object sender, EventArgs e)
        {
            TextBox_Content.Font = new Font(TextBox_Content.Font.FontFamily, 8, FontStyle.Regular);
            TextBox_Content.SelectAll();
            TextBox_Content.SelectionFont = TextBox_Content.Font;
            TextBox_Content.SelectionStart = 0;
            TextBox_Content.SelectionLength = 0;
            //TextBox_Content.ScrollToCaret();
        }

        public void AppendOutput(string output)
        {
            TextBox_Content.AppendText(output);

            TextBox_Content.SelectionStart = TextBox_Content.TextLength;
        }

        public void Clear()
        {
            TextBox_Content.Text = string.Empty;
        }

        private void ClearToolStripButton_Click(object sender, EventArgs e)
        {
            TextBox_Content.Text = string.Empty;
        }

        private void ExportToolStripButton_Click(object sender, EventArgs e)
        {
            SaveFileDialog svd = new SaveFileDialog();
            svd.Title = Resources.Save_Output_File;
            svd.AddExtension = true;
            svd.Filter = "Text Files|*.txt|All files|*.*";

            if (svd.ShowDialog() == DialogResult.OK)
            {
                TextWriter tr = new StreamWriter(svd.FileName);
                tr.WriteLine(this.TextBox_Content.Text);
                tr.Flush();
                tr.Close();
            }
        }

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OutputDockingWindow));
            this.ToolStripContainer = new System.Windows.Forms.ToolStripContainer();
            this.TextBox_Content = new System.Windows.Forms.RichTextBox();
            this.ToolStrip = new System.Windows.Forms.ToolStrip();
            this.Button_Clear = new System.Windows.Forms.ToolStripButton();
            this.Button_SaveAs = new System.Windows.Forms.ToolStripButton();
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
            this.ToolStripContainer.ContentPanel.Controls.Add(this.TextBox_Content);
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
            // TextBox_Content
            // 
            resources.ApplyResources(this.TextBox_Content, "TextBox_Content");
            this.TextBox_Content.Name = "TextBox_Content";
            // 
            // ToolStrip
            // 
            resources.ApplyResources(this.ToolStrip, "ToolStrip");
            this.ToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.Button_Clear,
            this.Button_SaveAs});
            this.ToolStrip.Name = "ToolStrip";
            // 
            // Button_Clear
            // 
            this.Button_Clear.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.Button_Clear.Image = global::PAT.GUI.Properties.Resources.Clear;
            resources.ApplyResources(this.Button_Clear, "Button_Clear");
            this.Button_Clear.Name = "Button_Clear";
            this.Button_Clear.Click += new System.EventHandler(this.ClearToolStripButton_Click);
            // 
            // Button_SaveAs
            // 
            this.Button_SaveAs.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.Button_SaveAs.Image = global::PAT.GUI.Properties.Resources.save;
            resources.ApplyResources(this.Button_SaveAs, "Button_SaveAs");
            this.Button_SaveAs.Name = "Button_SaveAs";
            this.Button_SaveAs.Click += new System.EventHandler(this.ExportToolStripButton_Click);
            // 
            // OutputDockingWindow
            // 
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.ToolStripContainer);
            this.Name = "OutputDockingWindow";
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