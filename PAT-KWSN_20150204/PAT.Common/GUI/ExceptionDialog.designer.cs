namespace PAT.Common.GUI
{
    partial class ExceptionDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ExceptionDialog));
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.Label_WhatHappened = new System.Windows.Forms.Label();
            this.Label_HowThisWillAffectYou = new System.Windows.Forms.Label();
            this.ScopeBox = new System.Windows.Forms.RichTextBox();
            this.ErrorBox = new System.Windows.Forms.RichTextBox();
            this.ActionBox = new System.Windows.Forms.RichTextBox();
            this.Label_WhatYouCanDoAboutIt = new System.Windows.Forms.Label();
            this.UserInfoBox = new System.Windows.Forms.RichTextBox();
            this.Label_HowDoYouGetThisError = new System.Windows.Forms.Label();
            this.Button_Email = new System.Windows.Forms.Button();
            this.Button_Continue = new System.Windows.Forms.Button();
            this.Button_Stop = new System.Windows.Forms.Button();
            this.Label_TechnicalDetails = new System.Windows.Forms.Label();
            this.DetailBox = new System.Windows.Forms.RichTextBox();
            this.TextBox_Email = new System.Windows.Forms.RichTextBox();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            resources.ApplyResources(this.pictureBox1, "pictureBox1");
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.TabStop = false;
            // 
            // Label_WhatHappened
            // 
            resources.ApplyResources(this.Label_WhatHappened, "Label_WhatHappened");
            this.Label_WhatHappened.Name = "Label_WhatHappened";
            // 
            // Label_HowThisWillAffectYou
            // 
            resources.ApplyResources(this.Label_HowThisWillAffectYou, "Label_HowThisWillAffectYou");
            this.Label_HowThisWillAffectYou.Name = "Label_HowThisWillAffectYou";
            // 
            // ScopeBox
            // 
            resources.ApplyResources(this.ScopeBox, "ScopeBox");
            this.ScopeBox.BackColor = System.Drawing.SystemColors.Control;
            this.ScopeBox.Name = "ScopeBox";
            this.ScopeBox.TabStop = false;
            // 
            // ErrorBox
            // 
            resources.ApplyResources(this.ErrorBox, "ErrorBox");
            this.ErrorBox.BackColor = System.Drawing.SystemColors.Control;
            this.ErrorBox.Name = "ErrorBox";
            this.ErrorBox.ReadOnly = true;
            this.ErrorBox.TabStop = false;
            // 
            // ActionBox
            // 
            resources.ApplyResources(this.ActionBox, "ActionBox");
            this.ActionBox.BackColor = System.Drawing.SystemColors.Control;
            this.ActionBox.Name = "ActionBox";
            this.ActionBox.TabStop = false;
            // 
            // Label_WhatYouCanDoAboutIt
            // 
            resources.ApplyResources(this.Label_WhatYouCanDoAboutIt, "Label_WhatYouCanDoAboutIt");
            this.Label_WhatYouCanDoAboutIt.Name = "Label_WhatYouCanDoAboutIt";
            // 
            // UserInfoBox
            // 
            resources.ApplyResources(this.UserInfoBox, "UserInfoBox");
            this.UserInfoBox.Name = "UserInfoBox";
            // 
            // Label_HowDoYouGetThisError
            // 
            resources.ApplyResources(this.Label_HowDoYouGetThisError, "Label_HowDoYouGetThisError");
            this.Label_HowDoYouGetThisError.Name = "Label_HowDoYouGetThisError";
            // 
            // Button_Email
            // 
            resources.ApplyResources(this.Button_Email, "Button_Email");
            this.Button_Email.Name = "Button_Email";
            this.Button_Email.UseVisualStyleBackColor = true;
            this.Button_Email.Click += new System.EventHandler(this.Button_Email_Click);
            // 
            // Button_Continue
            // 
            resources.ApplyResources(this.Button_Continue, "Button_Continue");
            this.Button_Continue.DialogResult = System.Windows.Forms.DialogResult.Ignore;
            this.Button_Continue.Name = "Button_Continue";
            this.Button_Continue.UseVisualStyleBackColor = true;
            this.Button_Continue.Click += new System.EventHandler(this.Button_Continue_Click);
            // 
            // Button_Stop
            // 
            resources.ApplyResources(this.Button_Stop, "Button_Stop");
            this.Button_Stop.DialogResult = System.Windows.Forms.DialogResult.Abort;
            this.Button_Stop.Name = "Button_Stop";
            this.Button_Stop.UseVisualStyleBackColor = true;
            this.Button_Stop.Click += new System.EventHandler(this.Button_Stop_Click);
            // 
            // Label_TechnicalDetails
            // 
            resources.ApplyResources(this.Label_TechnicalDetails, "Label_TechnicalDetails");
            this.Label_TechnicalDetails.Name = "Label_TechnicalDetails";
            // 
            // DetailBox
            // 
            resources.ApplyResources(this.DetailBox, "DetailBox");
            this.DetailBox.BackColor = System.Drawing.SystemColors.Control;
            this.DetailBox.Name = "DetailBox";
            this.DetailBox.TabStop = false;
            // 
            // TextBox_Email
            // 
            resources.ApplyResources(this.TextBox_Email, "TextBox_Email");
            this.TextBox_Email.Name = "TextBox_Email";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // ExceptionDialog
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label1);
            this.Controls.Add(this.TextBox_Email);
            this.Controls.Add(this.DetailBox);
            this.Controls.Add(this.Label_TechnicalDetails);
            this.Controls.Add(this.Button_Stop);
            this.Controls.Add(this.Button_Continue);
            this.Controls.Add(this.Button_Email);
            this.Controls.Add(this.UserInfoBox);
            this.Controls.Add(this.Label_HowDoYouGetThisError);
            this.Controls.Add(this.ActionBox);
            this.Controls.Add(this.Label_WhatYouCanDoAboutIt);
            this.Controls.Add(this.ErrorBox);
            this.Controls.Add(this.ScopeBox);
            this.Controls.Add(this.Label_HowThisWillAffectYou);
            this.Controls.Add(this.Label_WhatHappened);
            this.Controls.Add(this.pictureBox1);
            this.MinimizeBox = false;
            this.Name = "ExceptionDialog";
            this.ShowIcon = false;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ExceptionDialog_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label Label_WhatHappened;
        private System.Windows.Forms.Label Label_HowThisWillAffectYou;
        private System.Windows.Forms.RichTextBox ScopeBox;
        private System.Windows.Forms.RichTextBox ErrorBox;
        private System.Windows.Forms.RichTextBox ActionBox;
        private System.Windows.Forms.Label Label_WhatYouCanDoAboutIt;
        private System.Windows.Forms.RichTextBox UserInfoBox;
        private System.Windows.Forms.Label Label_HowDoYouGetThisError;
        private System.Windows.Forms.Button Button_Email;
        private System.Windows.Forms.Button Button_Continue;
        private System.Windows.Forms.Button Button_Stop;
        private System.Windows.Forms.Label Label_TechnicalDetails;
        private System.Windows.Forms.RichTextBox DetailBox;
        private System.Windows.Forms.RichTextBox TextBox_Email;
        private System.Windows.Forms.Label label1;
    }
}