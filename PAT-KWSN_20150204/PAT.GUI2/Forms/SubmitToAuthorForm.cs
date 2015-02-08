using System.Text;
using System.Windows.Forms;
using PAT.Common.GUI;
using PAT.GUI.Properties;

namespace PAT.GUI.Forms
{
    public partial class SubmitToAuthorForm : Form
    {
        public string ModelName
        {
            get
            {
                return this.TextBox_ModelName.Text;
            }
        }

        private string Model;
        public string Message
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("Model Name: " + this.TextBox_ModelName.Text);
                sb.AppendLine("Author Name: " + this.TextBox_AuthorName.Text);
                sb.AppendLine("Author Email: " + this.TextBox_AuthorEmail.Text);
                sb.AppendLine("Model Description: " + this.TextBox_ModelDescription.Text);
                sb.AppendLine();
                sb.AppendLine();
                sb.AppendLine("//=======================User Info===============================");
                sb.AppendLine(ExceptionDialog.SysInfoToString());
                sb.AppendLine();
                sb.AppendLine("//=======================Model Details===========================");
                return sb.ToString();
            }
        }

        public SubmitToAuthorForm(string model)
        {
            InitializeComponent();
            Model = model;
            this.TextBox_To.Text = Common.Ultility.Ultility.PAT_EMAIL;
        }

        private bool canCancel = true;
        private void Button_Submit_Click(object sender, System.EventArgs e)
        {
            if (this.TextBox_To.Text.Trim() == "" || !this.TextBox_To.Text.Contains("@") || !this.TextBox_To.Text.Contains("."))
            {
                MessageBox.Show(Resources.Invalid_email_address_, Common.Ultility.Ultility.APPLICATION_NAME, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Cursor = Cursors.WaitCursor;
            this.Button_Submit.Enabled = false;
            this.Button_Cancel.Enabled = false;
            canCancel = false;

            bool result = Common.Ultility.Ultility.SendEmail("PAT Model Submission: " + ModelName, Message + Model, this.TextBox_To.Text);
            if (result)
            {
                MessageBox.Show(Resources.Email_is_sent_successfully_, Common.Ultility.Ultility.APPLICATION_NAME, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show(Resources.Sending_Email_failed_, Common.Ultility.Ultility.APPLICATION_NAME, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            canCancel = true;
            Cursor = Cursors.Default;
            this.Close();
        }

        private void SubmitToAuthorForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(!canCancel)
            {
                e.Cancel = true;
            }
        }
    }
}
