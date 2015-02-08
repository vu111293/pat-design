using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace PAT.GUI.Forms
{
    public partial class LaTexPrintingDialog : Form
    {
        public LaTexPrintingDialog()
        {
            InitializeComponent();
        }

        public bool DirectGeneration
        { 
            get
            {
                return this.RadioButton_DirectGeneration.Checked;
            }
        }

        public bool DisplayLineNumber
        {
            get
            {
                return this.CheckBox_DisplayLineNumber.Checked;
            }
        }

        private void Button_GetPATStyleFile_Click(object sender, System.EventArgs e)
        {
            try
            {
                Process.Start(Path.GetDirectoryName(Path.Combine(Common.Ultility.Ultility.APPLICATION_PATH, "Docs")));
            }
            catch
            {

            }
        }
    }
}
