using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Fireball.Docking;
using PAT.GUI.Properties;

namespace PAT.GUI.Docking
{
    public class DescriptionWindow : EditDockableWindow
    {
        public RichTextBox TextBox = null;
        private ToolStrip toolStrip1;
        private ToolStripButton ExportToolStripButton;

        public DescriptionWindow()
        {
            this.DockableAreas = DockAreas.DockBottom | DockAreas.Float;
            
            TextBox = new RichTextBox();
            TextBox.ReadOnly = true;
            
            TextBox.Dock = DockStyle.Fill;

            ComponentResourceManager resources = new ComponentResourceManager(typeof(EditOutputWindow));
            this.toolStrip1 = new ToolStrip();
            this.ExportToolStripButton = new ToolStripButton();
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new ToolStripItem[] {this.ExportToolStripButton});
            this.toolStrip1.Location = new Point(0, 2);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new Size(255, 25);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";            
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

            this.Text = "Specification Description";           
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
    }
}