using System;
using System.Windows.Forms;
using PAT.Common.GUI.ERAModule;

namespace PAT.GUI.ERADrawing
{
    public partial class ERAProcessEditingForm : Form
    {
        private ERACanvas Canves;
        public ERAProcessEditingForm(ERACanvas canves)
        {
            InitializeComponent();
            Canves = canves;
            TextBox_Name.Text = Canves.Node.Text;
            this.TextBox_Paramater.Text = Canves.Parameters;
        }

        private void Button_OK_Click(object sender, EventArgs e)
        {
           
        }

        public void DisableParameter()
        {
            this.TextBox_Paramater.Visible = false;
        }

        public void UpdateData()
        {
            Canves.Node.Text = TextBox_Name.Text;
            Canves.Parameters = TextBox_Paramater.Text;
        }
    }
}
