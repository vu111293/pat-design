namespace PAT.GUI.Forms
{
    partial class LaTexPrintingDialog
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LaTexPrintingDialog));
            this.RadioButton_DirectGeneration = new System.Windows.Forms.RadioButton();
            this.CheckBox_DisplayLineNumber = new System.Windows.Forms.CheckBox();
            this.RadioButton_GenerateAfterParsing = new System.Windows.Forms.RadioButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.Button_Cancel = new System.Windows.Forms.Button();
            this.Button_OK = new System.Windows.Forms.Button();
            this.ToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.Button_GetPATStyleFile = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // RadioButton_DirectGeneration
            // 
            resources.ApplyResources(this.RadioButton_DirectGeneration, "RadioButton_DirectGeneration");
            this.RadioButton_DirectGeneration.Checked = true;
            this.RadioButton_DirectGeneration.Name = "RadioButton_DirectGeneration";
            this.RadioButton_DirectGeneration.TabStop = true;
            this.ToolTip.SetToolTip(this.RadioButton_DirectGeneration, resources.GetString("RadioButton_DirectGeneration.ToolTip"));
            this.RadioButton_DirectGeneration.UseVisualStyleBackColor = true;
            // 
            // CheckBox_DisplayLineNumber
            // 
            resources.ApplyResources(this.CheckBox_DisplayLineNumber, "CheckBox_DisplayLineNumber");
            this.CheckBox_DisplayLineNumber.Checked = true;
            this.CheckBox_DisplayLineNumber.CheckState = System.Windows.Forms.CheckState.Checked;
            this.CheckBox_DisplayLineNumber.Name = "CheckBox_DisplayLineNumber";
            this.CheckBox_DisplayLineNumber.UseVisualStyleBackColor = true;
            // 
            // RadioButton_GenerateAfterParsing
            // 
            resources.ApplyResources(this.RadioButton_GenerateAfterParsing, "RadioButton_GenerateAfterParsing");
            this.RadioButton_GenerateAfterParsing.Name = "RadioButton_GenerateAfterParsing";
            this.ToolTip.SetToolTip(this.RadioButton_GenerateAfterParsing, resources.GetString("RadioButton_GenerateAfterParsing.ToolTip"));
            this.RadioButton_GenerateAfterParsing.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.RadioButton_GenerateAfterParsing);
            this.groupBox1.Controls.Add(this.RadioButton_DirectGeneration);
            this.groupBox1.Controls.Add(this.CheckBox_DisplayLineNumber);
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // Button_Cancel
            // 
            this.Button_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            resources.ApplyResources(this.Button_Cancel, "Button_Cancel");
            this.Button_Cancel.Name = "Button_Cancel";
            this.Button_Cancel.UseVisualStyleBackColor = true;
            // 
            // Button_OK
            // 
            this.Button_OK.DialogResult = System.Windows.Forms.DialogResult.OK;
            resources.ApplyResources(this.Button_OK, "Button_OK");
            this.Button_OK.Name = "Button_OK";
            this.Button_OK.UseVisualStyleBackColor = true;
            // 
            // Button_GetPATStyleFile
            // 
            resources.ApplyResources(this.Button_GetPATStyleFile, "Button_GetPATStyleFile");
            this.Button_GetPATStyleFile.Name = "Button_GetPATStyleFile";
            this.Button_GetPATStyleFile.UseVisualStyleBackColor = true;
            this.Button_GetPATStyleFile.Click += new System.EventHandler(this.Button_GetPATStyleFile_Click);
            // 
            // LaTexPrintingDialog
            // 
            this.AcceptButton = this.Button_OK;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.Button_Cancel;
            this.Controls.Add(this.Button_GetPATStyleFile);
            this.Controls.Add(this.Button_OK);
            this.Controls.Add(this.Button_Cancel);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "LaTexPrintingDialog";
            this.ShowIcon = false;
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RadioButton RadioButton_DirectGeneration;
        private System.Windows.Forms.CheckBox CheckBox_DisplayLineNumber;
        private System.Windows.Forms.RadioButton RadioButton_GenerateAfterParsing;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button Button_Cancel;
        private System.Windows.Forms.Button Button_OK;
        private System.Windows.Forms.ToolTip ToolTip;
        private System.Windows.Forms.Button Button_GetPATStyleFile;
    }
}