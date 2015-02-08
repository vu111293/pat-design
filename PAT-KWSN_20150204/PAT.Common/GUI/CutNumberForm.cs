using System.Windows.Forms;

namespace PAT.Common.GUI
{
    public partial class CutNumberForm : Form
    {
        public CutNumberForm()
        {
            InitializeComponent();
        }

        private void Button_OK_Click(object sender, System.EventArgs e)
        {
            if(this.NumericUpDown_CutNumber.Value == 0)
            {                
                return;
            }
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }
    }
}