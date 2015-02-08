using System;
using System.Windows.Forms;
using PAT.Common.GUI.Drawing;

namespace PAT.GUI.LTSDrawing
{
    public partial class ProcessEditingForm : Form
    {
        private LTSCanvas Canves;
        public ProcessEditingForm(LTSCanvas canves)
        {
            InitializeComponent();
            Canves = canves;
            TextBox_Name.Text = Canves.Node.Text;
            this.TextBox_Paramater.Text = Canves.Parameters;
        }

        private void Button_OK_Click(object sender, EventArgs e)
        {
           
        }

        public void UpdateData()
        {
            Canves.Node.Text = TextBox_Name.Text;
            Canves.Parameters = TextBox_Paramater.Text;
        }
    }
}
