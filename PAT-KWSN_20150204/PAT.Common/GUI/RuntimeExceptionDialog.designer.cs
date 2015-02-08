namespace PAT.Common.GUI
{
    partial class RuntimeExceptionDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RuntimeExceptionDialog));
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.Label_WhatHappened = new System.Windows.Forms.Label();
            this.ErrorBox = new System.Windows.Forms.RichTextBox();
            this.ActionBox = new System.Windows.Forms.RichTextBox();
            this.Label_WhatYouCanDoAboutIt = new System.Windows.Forms.Label();
            this.Button_Close = new System.Windows.Forms.Button();
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
            // Button_Close
            // 
            resources.ApplyResources(this.Button_Close, "Button_Close");
            this.Button_Close.Name = "Button_Close";
            this.Button_Close.UseVisualStyleBackColor = true;
            this.Button_Close.Click += new System.EventHandler(this.Button_Stop_Click);
            // 
            // RuntimeExceptionDialog
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.Button_Close);
            this.Controls.Add(this.ActionBox);
            this.Controls.Add(this.Label_WhatYouCanDoAboutIt);
            this.Controls.Add(this.ErrorBox);
            this.Controls.Add(this.Label_WhatHappened);
            this.Controls.Add(this.pictureBox1);
            this.MinimizeBox = false;
            this.Name = "RuntimeExceptionDialog";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label Label_WhatHappened;
        private System.Windows.Forms.RichTextBox ErrorBox;
        private System.Windows.Forms.RichTextBox ActionBox;
        private System.Windows.Forms.Label Label_WhatYouCanDoAboutIt;
        private System.Windows.Forms.Button Button_Close;
    }
}