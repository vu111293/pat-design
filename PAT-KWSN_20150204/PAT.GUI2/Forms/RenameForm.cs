using System;
using System.Windows.Forms;
using PAT.GUI.Properties;

namespace PAT.GUI.Forms
{
    public partial class RenameForm : Form
    {
        public RenameForm(string name)
        {
            InitializeComponent();
            this.Label_OldName.Text = name;

        }

        private void Button_OK_Click(object sender, EventArgs e)
        {
            string newName = TextBox_NewName.Text;
            if (!Common.Ultility.Ultility.IsAValidName(newName))
            {
                MessageBox.Show(Resources.Invalid_name_, Common.Ultility.Ultility.APPLICATION_NAME, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

     
    }
}
