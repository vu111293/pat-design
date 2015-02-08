using System;
using System.Windows.Forms;
using PAT.Common.GUI.TAModule;

namespace PAT.GUI.TADrawing
{
    public partial class TAProcessEditingForm : Form
    {
        private TACanvas Canves;
        public TAProcessEditingForm(TACanvas canves)
        {
            InitializeComponent();
            Canves = canves;
            TextBox_Name.Text = Canves.Node.Text;
            string[] strings = Canves.Parameters.Trim().Split(new char[]{'$'}, StringSplitOptions.RemoveEmptyEntries);
            
            if (strings.Length > 0 && !Canves.Parameters.Trim().StartsWith("$"))
            {
                this.TextBox_Paramater.Text = strings[0];
            }
            else if (strings.Length == 1)
            {
                this.TextBox_Clock.Text = strings[0];
            }
            
            if (strings.Length == 2)
            {
                this.TextBox_Clock.Text = strings[1];
            }
        }

        private void Button_OK_Click(object sender, EventArgs e)
        {
           
        }

        public void UpdateData()
        {
            Canves.Node.Text = TextBox_Name.Text;
            Canves.Parameters = TextBox_Paramater.Text + "$" + TextBox_Clock.Text;
        }
    }
}
