namespace PAT.GUI.CLTSDrawing
{
    partial class CLTSStateEditingForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CLTSStateEditingForm));
            this.label1 = new System.Windows.Forms.Label();
            this.TextBox_Name = new System.Windows.Forms.TextBox();
            this.Button_OK = new System.Windows.Forms.Button();
            this.Button_Cancel = new System.Windows.Forms.Button();
            this.CheckBox_IsAcceptance = new System.Windows.Forms.CheckBox();
            this.CheckBox_Error = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // TextBox_Name
            // 
            resources.ApplyResources(this.TextBox_Name, "TextBox_Name");
            this.TextBox_Name.Name = "TextBox_Name";
            // 
            // Button_OK
            // 
            this.Button_OK.DialogResult = System.Windows.Forms.DialogResult.OK;
            resources.ApplyResources(this.Button_OK, "Button_OK");
            this.Button_OK.Name = "Button_OK";
            this.Button_OK.UseVisualStyleBackColor = true;
            this.Button_OK.Click += new System.EventHandler(this.Button_OK_Click);
            // 
            // Button_Cancel
            // 
            this.Button_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            resources.ApplyResources(this.Button_Cancel, "Button_Cancel");
            this.Button_Cancel.Name = "Button_Cancel";
            this.Button_Cancel.UseVisualStyleBackColor = true;
            // 
            // CheckBox_IsAcceptance
            // 
            resources.ApplyResources(this.CheckBox_IsAcceptance, "CheckBox_IsAcceptance");
            this.CheckBox_IsAcceptance.Name = "CheckBox_IsAcceptance";
            this.CheckBox_IsAcceptance.UseVisualStyleBackColor = true;
            // 
            // CheckBox_Error
            // 
            resources.ApplyResources(this.CheckBox_Error, "CheckBox_Error");
            this.CheckBox_Error.Name = "CheckBox_Error";
            this.CheckBox_Error.UseVisualStyleBackColor = true;
            // 
            // CLTSStateEditingForm
            // 
            this.AcceptButton = this.Button_OK;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.Button_Cancel;
            this.Controls.Add(this.CheckBox_Error);
            this.Controls.Add(this.CheckBox_IsAcceptance);
            this.Controls.Add(this.Button_Cancel);
            this.Controls.Add(this.Button_OK);
            this.Controls.Add(this.TextBox_Name);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CLTSStateEditingForm";
            this.ShowIcon = false;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox TextBox_Name;
        private System.Windows.Forms.Button Button_OK;
        private System.Windows.Forms.Button Button_Cancel;
        private System.Windows.Forms.CheckBox CheckBox_IsAcceptance;
        private System.Windows.Forms.CheckBox CheckBox_Error;
    }
}