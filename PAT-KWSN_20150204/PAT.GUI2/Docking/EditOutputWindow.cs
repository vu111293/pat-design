using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Fireball.Docking;
using PAT.GUI.Properties;

namespace PAT.GUI.Docking
{
    public class EditOutputWindow : DockableWindow
    {
        public RichTextBox TextBox = null;
        private ToolStrip toolStrip1;
        private ToolStripButton ClearToolStripButton;
        private ToolStripButton ExportToolStripButton;

        public EditOutputWindow()
        {
            this.DockableAreas = DockAreas.DockBottom | DockAreas.Float;
            
            TextBox = new RichTextBox();
            
            TextBox.Dock = DockStyle.Fill;
            TextBox.Font = new Font("Courier New", 8);

            ComponentResourceManager resources = new ComponentResourceManager(typeof(EditOutputWindow));
            this.toolStrip1 = new ToolStrip();
            this.ClearToolStripButton = new ToolStripButton();
            this.ExportToolStripButton = new ToolStripButton();
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new ToolStripItem[] {
                                                                                        this.ClearToolStripButton,
                                                                                        this.ExportToolStripButton});
            this.toolStrip1.Location = new Point(0, 2);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new Size(255, 25);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // ClearToolStripButton
            // 
            this.ClearToolStripButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            this.ClearToolStripButton.Image = Resources.Clear;
            this.ClearToolStripButton.ImageTransparentColor = Color.Magenta;
            this.ClearToolStripButton.Name = "ClearToolStripButton";
            this.ClearToolStripButton.Size = new Size(23, 22);
            this.ClearToolStripButton.Text = "Clear";
            this.ClearToolStripButton.Click += new EventHandler(this.ClearToolStripButton_Click);
            // 
            // ExportToolStripButton
            // 
            this.ExportToolStripButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            this.ExportToolStripButton.Image = Resources.save_as;
            this.ExportToolStripButton.ImageTransparentColor = Color.Magenta;
            this.ExportToolStripButton.Name = "ExportToolStripButton";
            this.ExportToolStripButton.Size = new Size(23, 22);
            this.ExportToolStripButton.Text = "Export";
            this.ExportToolStripButton.Click += new EventHandler(this.ExportToolStripButton_Click);
            // 
            // OutputWindow
            // 
            this.Controls.Add(TextBox);
            this.Controls.Add(this.toolStrip1);

            this.Icon = ((Icon)(resources.GetObject("$this.Icon")));

            this.Text = "Output";
            //this.CloseButton = false;
            
        }

        public void CommentWindow()
        {
            ComponentResourceManager resources = new ComponentResourceManager(typeof(EditOutputWindow));
            this.Icon = ((Icon)(resources.GetObject("comments")));

            this.Text = "Comment";
            
        }

        public void AppendOutput(string output)
        {
            TextBox.AppendText(output);

            TextBox.SelectionStart = TextBox.TextLength;
        }

        public void Clear()
        {
            TextBox.Text = string.Empty;
        }

        private void ClearToolStripButton_Click(object sender, EventArgs e)
        {
            this.TextBox.Text = "";
        }

        private void ExportToolStripButton_Click(object sender, EventArgs e)
        {
            SaveFileDialog svd = new SaveFileDialog();
            svd.Title = "Save Output File";
            svd.Filter = "Text Files|*.txt|All files|*.*";

            if (svd.ShowDialog() == DialogResult.OK)
            {
                TextWriter tr = new StreamWriter(svd.FileName);
                tr.WriteLine(this.TextBox.Text);
                tr.Flush();
                tr.Close();
            }
        }

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EditOutputWindow));
            this.SuspendLayout();
            // 
            // EditOutputWindow
            // 
            resources.ApplyResources(this, "$this");
            this.Name = "EditOutputWindow";
            this.ResumeLayout(false);

        }
    }
}