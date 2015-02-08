using System;
using System.Windows.Forms;
using PAT.Common.GUI.UMLModule;

namespace PAT.GUI.UMLDrawing
{
    public partial class StateDiagramEditingForm : Form
    {
        protected TreeNode DiagramNode;
        
        public StateDiagramEditingForm(TreeNode diagramNode)
        {
            InitializeComponent();
            TextBox_Name.Text = diagramNode.Text;
            this.DiagramNode = diagramNode;
        }
        
        private void Button_OK_Click(object sender, EventArgs e)
        {
           
        }

        public void UpdateData()
        {
            var newName = TextBox_Name.Text;
            DiagramNode.Text = newName;
            DiagramNode.Name = newName;
            (DiagramNode.Tag as StateDiagram).Name = newName;
        }
        
        private void NodeEditingForm_Load(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

    }

    
}
