using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;

namespace PAT.Common.GUI.Simulator
{
    /// <summary>
    /// Summary description for GotoLine.
    /// </summary>
    public class FormTraceInput : Form
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private Container components = null;

        private Button btnCancel;
        private Button btnOK;
        private TextBox txtRow;
    
        /// <summary>
        /// Default constructor for the GotoLineForm.
        /// </summary>
        public FormTraceInput()
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormTraceInput));
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.txtRow = new System.Windows.Forms.TextBox();
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
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            resources.ApplyResources(this.btnOK, "btnOK");
            this.btnOK.Name = "btnOK";
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // txtRow
            // 
            resources.ApplyResources(this.txtRow, "txtRow");
            this.txtRow.Name = "txtRow";
            // 
            // FormTraceInput
            // 
            this.AcceptButton = this.btnOK;
            this.CancelButton = this.btnCancel;
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.txtRow);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnCancel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "FormTraceInput";
            this.ShowInTaskbar = false;
            this.Activated += new System.EventHandler(this.GotoLine_Activated);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public List<string> EventList;  


        private void btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                string[] tokens = txtRow.Text.Split(',');
                
                EventList = new List<string>();
                foreach (string token in tokens)
                {
                    if(token.Contains("(") && token.Contains(")"))
                    {
                        string[] pair = token.Split(new string[] {"(", ")"}, StringSplitOptions.RemoveEmptyEntries);
                        if(pair.Length == 2)
                        {
                            string evtName = pair[0].Trim();
                            int number = int.Parse(pair[1].Trim());
                            for (int i = 0; i < number; i++)
                            {
                                EventList.Add(evtName);
                            }
                        }
                        else
                        {
                            throw new Exception("Wrong format with token " +  token);
                        }
                    }
                    else
                    {
                        EventList.Add(token);
                    }
                }

                this.Hide();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Hide();
        }


        private void GotoLine_Activated(object sender, EventArgs e)
        {
            this.txtRow.Focus();
        }
    }
}