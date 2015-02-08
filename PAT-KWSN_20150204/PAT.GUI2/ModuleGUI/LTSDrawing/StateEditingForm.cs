using System;
using System.Windows.Forms;
using PAT.Common.GUI.Drawing;

namespace PAT.GUI.LTSDrawing
{
    public partial class StateEditingForm : Form
    {
        private StateItem StateItem;

        public StateEditingForm(StateItem stateItem)
        {
            InitializeComponent();
            StateItem = stateItem;
            TextBox_Name.Text = stateItem.Name;
        }

        private void Button_OK_Click(object sender, EventArgs e)
        {
            StateItem.Name = TextBox_Name.Text;
        }
    }
}
