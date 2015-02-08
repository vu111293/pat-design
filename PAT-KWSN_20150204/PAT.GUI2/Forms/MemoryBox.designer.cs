namespace PAT.GUI.Forms
{
    partial class MemoryBox
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MemoryBox));
            this.Button_Yes = new System.Windows.Forms.Button();
            this.buttonYestoAll = new System.Windows.Forms.Button();
            this.Button_No = new System.Windows.Forms.Button();
            this.Button_NotoAll = new System.Windows.Forms.Button();
            this.Button_Cancel = new System.Windows.Forms.Button();
            this.Label_Message = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // Button_Yes
            // 
            resources.ApplyResources(this.Button_Yes, "Button_Yes");
            this.Button_Yes.Name = "Button_Yes";
            this.Button_Yes.UseVisualStyleBackColor = true;
            this.Button_Yes.Click += new System.EventHandler(this.buttonYes_Click);
            // 
            // buttonYestoAll
            // 
            resources.ApplyResources(this.buttonYestoAll, "buttonYestoAll");
            this.buttonYestoAll.Name = "buttonYestoAll";
            this.buttonYestoAll.UseVisualStyleBackColor = true;
            this.buttonYestoAll.Click += new System.EventHandler(this.buttonYestoAll_Click);
            // 
            // Button_No
            // 
            resources.ApplyResources(this.Button_No, "Button_No");
            this.Button_No.Name = "Button_No";
            this.Button_No.UseVisualStyleBackColor = true;
            this.Button_No.Click += new System.EventHandler(this.buttonNo_Click);
            // 
            // Button_NotoAll
            // 
            resources.ApplyResources(this.Button_NotoAll, "Button_NotoAll");
            this.Button_NotoAll.Name = "Button_NotoAll";
            this.Button_NotoAll.UseVisualStyleBackColor = true;
            this.Button_NotoAll.Click += new System.EventHandler(this.buttonNotoAll_Click);
            // 
            // Button_Cancel
            // 
            this.Button_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            resources.ApplyResources(this.Button_Cancel, "Button_Cancel");
            this.Button_Cancel.Name = "Button_Cancel";
            this.Button_Cancel.UseVisualStyleBackColor = true;
            this.Button_Cancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // Label_Message
            // 
            resources.ApplyResources(this.Label_Message, "Label_Message");
            this.Label_Message.Name = "Label_Message";
            // 
            // MemoryBox
            // 
            this.AcceptButton = this.Button_Yes;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.Button_Cancel;
            this.Controls.Add(this.Label_Message);
            this.Controls.Add(this.Button_Cancel);
            this.Controls.Add(this.Button_NotoAll);
            this.Controls.Add(this.Button_No);
            this.Controls.Add(this.buttonYestoAll);
            this.Controls.Add(this.Button_Yes);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MemoryBox";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button Button_Yes;
        private System.Windows.Forms.Button buttonYestoAll;
        private System.Windows.Forms.Button Button_No;
        private System.Windows.Forms.Button Button_NotoAll;
        private System.Windows.Forms.Button Button_Cancel;
        private System.Windows.Forms.Label Label_Message;
    }
}