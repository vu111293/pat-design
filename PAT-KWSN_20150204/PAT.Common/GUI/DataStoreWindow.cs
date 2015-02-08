using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Fireball.Docking;


namespace PAT.Common.GUI
{
    public class DataStoreWindow : DockableWindow
    {
        public RichTextBox TextBox = null;
        
        public DataStoreWindow()
        {            
            this.DockableAreas = DockAreas.DockLeft | DockAreas.DockRight| DockAreas.Float;

            TextBox = new RichTextBox();
            TextBox.ReadOnly = true;
            TextBox.BackColor = Color.White;

            TextBox.Dock = DockStyle.Fill;

            ComponentResourceManager resources = new ComponentResourceManager(typeof(DataStoreWindow));

            this.Controls.Add(TextBox);

            this.Icon = ((Icon)(resources.GetObject("$this.Icon")));

            //this.Text = "Data Store";

            this.CloseButton = false;
        }

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DataStoreWindow));
            this.SuspendLayout();
            // 
            // DataStoreWindow
            // 
            resources.ApplyResources(this, "$this");
            this.Name = "DataStoreWindow";
            this.ResumeLayout(false);

        }
    }
}