using System;
using System.Windows.Forms;
using PAT.Common.Classes.Expressions.ExpressionClass;
using OutOfMemoryException=PAT.Common.Classes.Expressions.ExpressionClass.OutOfMemoryException;


namespace PAT.Common.GUI
{
    public partial class RuntimeExceptionDialog : Form
    {
        public RuntimeExceptionDialog(RuntimeException ex, string APPLICATION_NAME)
        {
            InitializeComponent();

            this.Text = APPLICATION_NAME + " " + this.Text;

            ActionBox.Text = Resources.You_have_following_options_ + Environment.NewLine;

            if(string.IsNullOrEmpty(ex.Action))
            {
                if (ex is OutOfMemoryException)
                {
                    ActionBox.Text += Resources.This_error_suggests_your_model_is__too_big_to_be_verified_ + Environment.NewLine;
                }
                else
                {
                    ActionBox.Text += Resources._1__Check_your_input_model_for_the_possiblity_of_errors_ + Environment.NewLine;
                }    
            }
            else
            {
                ActionBox.Text += "1." + ex.Action + Environment.NewLine;
            }
            
            
            ActionBox.Text += Resources._2__Email_us_your_model_and_seek_for_help_ + Environment.NewLine;

            string trace = "";
            if (ex.Data.Contains("trace"))
            {
                trace = Environment.NewLine + "Trace leads to exception:" + Environment.NewLine + ex.Data["trace"].ToString();
            }
            ErrorBox.Text = ErrorBox.Text + " " + APPLICATION_NAME + ". " + Environment.NewLine + ex.Message + trace + (ex.InnerStackTrace != null? Environment.NewLine + "Exception stack trace:" + Environment.NewLine + ex.InnerStackTrace: "") ;
        }

        private void Button_Stop_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}