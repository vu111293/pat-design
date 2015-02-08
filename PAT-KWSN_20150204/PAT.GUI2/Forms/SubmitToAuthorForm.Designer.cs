namespace PAT.GUI.Forms
{
    partial class SubmitToAuthorForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SubmitToAuthorForm));
            this.Label_ModelName = new System.Windows.Forms.Label();
            this.TextBox_ModelName = new System.Windows.Forms.TextBox();
            this.TextBox_AuthorEmail = new System.Windows.Forms.TextBox();
            this.Label_AuthorEmail = new System.Windows.Forms.Label();
            this.TextBox_AuthorName = new System.Windows.Forms.TextBox();
            this.Label_ModelDescription = new System.Windows.Forms.Label();
            this.Button_Submit = new System.Windows.Forms.Button();
            this.Button_Cancel = new System.Windows.Forms.Button();
            this.TextBox_ModelDescription = new System.Windows.Forms.TextBox();
            this.Label_AuthorName = new System.Windows.Forms.Label();
            this.TextBox_To = new System.Windows.Forms.TextBox();
            this.Label_To = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // Label_ModelName
            // 
            resources.ApplyResources(this.Label_ModelName, "Label_ModelName");
            this.Label_ModelName.Name = "Label_ModelName";
            // 
            // TextBox_ModelName
            // 
            resources.ApplyResources(this.TextBox_ModelName, "TextBox_ModelName");
            this.TextBox_ModelName.Name = "TextBox_ModelName";
            // 
            // TextBox_AuthorEmail
            // 
            resources.ApplyResources(this.TextBox_AuthorEmail, "TextBox_AuthorEmail");
            this.TextBox_AuthorEmail.Name = "TextBox_AuthorEmail";
            // 
            // Label_AuthorEmail
            // 
            resources.ApplyResources(this.Label_AuthorEmail, "Label_AuthorEmail");
            this.Label_AuthorEmail.Name = "Label_AuthorEmail";
            // 
            // TextBox_AuthorName
            // 
            resources.ApplyResources(this.TextBox_AuthorName, "TextBox_AuthorName");
            this.TextBox_AuthorName.Name = "TextBox_AuthorName";
            // 
            // Label_ModelDescription
            // 
            resources.ApplyResources(this.Label_ModelDescription, "Label_ModelDescription");
            this.Label_ModelDescription.Name = "Label_ModelDescription";
            // 
            // Button_Submit
            // 
            resources.ApplyResources(this.Button_Submit, "Button_Submit");
            this.Button_Submit.Name = "Button_Submit";
            this.Button_Submit.UseVisualStyleBackColor = true;
            this.Button_Submit.Click += new System.EventHandler(this.Button_Submit_Click);
            // 
            // Button_Cancel
            // 
            resources.ApplyResources(this.Button_Cancel, "Button_Cancel");
            this.Button_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Button_Cancel.Name = "Button_Cancel";
            this.Button_Cancel.UseVisualStyleBackColor = true;
            // 
            // TextBox_ModelDescription
            // 
            resources.ApplyResources(this.TextBox_ModelDescription, "TextBox_ModelDescription");
            this.TextBox_ModelDescription.Name = "TextBox_ModelDescription";
            // 
            // Label_AuthorName
            // 
            resources.ApplyResources(this.Label_AuthorName, "Label_AuthorName");
            this.Label_AuthorName.Name = "Label_AuthorName";
            // 
            // TextBox_To
            // 
            resources.ApplyResources(this.TextBox_To, "TextBox_To");
            this.TextBox_To.Name = "TextBox_To";
            // 
            // Label_To
            // 
            resources.ApplyResources(this.Label_To, "Label_To");
            this.Label_To.Name = "Label_To";
            // 
            // SubmitToAuthorForm
            // 
            this.AcceptButton = this.Button_Submit;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.Button_Cancel;
            this.Controls.Add(this.TextBox_To);
            this.Controls.Add(this.Label_To);
            this.Controls.Add(this.Label_AuthorName);
            this.Controls.Add(this.TextBox_ModelDescription);
            this.Controls.Add(this.Button_Cancel);
            this.Controls.Add(this.Button_Submit);
            this.Controls.Add(this.TextBox_AuthorName);
            this.Controls.Add(this.Label_ModelDescription);
            this.Controls.Add(this.TextBox_AuthorEmail);
            this.Controls.Add(this.Label_AuthorEmail);
            this.Controls.Add(this.TextBox_ModelName);
            this.Controls.Add(this.Label_ModelName);
            this.Cursor = System.Windows.Forms.Cursors.Default;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SubmitToAuthorForm";
            this.ShowIcon = false;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SubmitToAuthorForm_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label Label_ModelName;
        private System.Windows.Forms.TextBox TextBox_ModelName;
        private System.Windows.Forms.TextBox TextBox_AuthorEmail;
        private System.Windows.Forms.Label Label_AuthorEmail;
        private System.Windows.Forms.TextBox TextBox_AuthorName;
        private System.Windows.Forms.Label Label_ModelDescription;
        private System.Windows.Forms.Button Button_Submit;
        private System.Windows.Forms.Button Button_Cancel;
        private System.Windows.Forms.TextBox TextBox_ModelDescription;
        private System.Windows.Forms.Label Label_AuthorName;
        private System.Windows.Forms.TextBox TextBox_To;
        private System.Windows.Forms.Label Label_To;
    }
}