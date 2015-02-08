using System;
using System.ComponentModel;
using System.Windows.Forms;
using ICSharpCode.TextEditor;
using PAT.GUI.Properties;

namespace PAT.GUI.EditingFunction.GoTo
{
    /// <summary>
    /// Summary description for GotoLine.
    /// </summary>
    public class GotoLineForm : Form
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private Container components = null;

        private Button btnCancel;
        private Button btnOK;
        private Label lblLines;
        private TextBox txtRow;
        private TextEditorControl mOwner = null;

        /// <summary>
        /// Default constructor for the GotoLineForm.
        /// </summary>
        public GotoLineForm()
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            //
            // TODO: Add any constructor code after InitializeComponent call
            //
        }

        /// <summary>
        /// Creates a GotoLineForm that will be assigned to a specific Owner control.
        /// </summary>
        /// <param name="Owner">The SyntaxBox that will use the GotoLineForm</param>
        public GotoLineForm(TextEditorControl Owner)
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            //
            // TODO: Add any constructor code after InitializeComponent call
            //
            int RowCount = Owner.Document.TotalNumberOfLines;
            lblLines.Text = Resources.Line_number + " (1-" + (RowCount).ToString() + "):";
            mOwner = Owner;
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GotoLineForm));
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.txtRow = new System.Windows.Forms.TextBox();
            this.lblLines = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            resources.ApplyResources(this.btnCancel, "btnCancel");
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOK
            // 
            resources.ApplyResources(this.btnOK, "btnOK");
            this.btnOK.Name = "btnOK";
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // txtRow
            // 
            resources.ApplyResources(this.txtRow, "txtRow");
            this.txtRow.Name = "txtRow";
            // 
            // lblLines
            // 
            resources.ApplyResources(this.lblLines, "lblLines");
            this.lblLines.Name = "lblLines";
            // 
            // GotoLineForm
            // 
            this.AcceptButton = this.btnOK;
            this.CancelButton = this.btnCancel;
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.lblLines);
            this.Controls.Add(this.txtRow);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnCancel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "GotoLineForm";
            this.ShowInTaskbar = false;
            this.Activated += new System.EventHandler(this.GotoLine_Activated);
            this.Closing += new System.ComponentModel.CancelEventHandler(this.GotoLine_Closing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private void btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                int row = int.Parse(txtRow.Text) - 1;
                this.mOwner.ActiveTextAreaControl.JumpTo(row);
            }
            catch
            {
            }
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void GotoLine_Closing(object sender, CancelEventArgs e)
        {
            //e.Cancel =true;
            //this.Hide ();
        }

        private void GotoLine_Activated(object sender, EventArgs e)
        {
            this.txtRow.Focus();
        }
    }
}