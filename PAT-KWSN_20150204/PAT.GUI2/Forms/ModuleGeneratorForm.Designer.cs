namespace PAT.GUI.Forms
{
    partial class ModuleGeneratorForm
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
            this.Button_Generate = new System.Windows.Forms.Button();
            this.Button_Cancel = new System.Windows.Forms.Button();
            this.Label_ModuleName = new System.Windows.Forms.Label();
            this.Label_ModuleCode = new System.Windows.Forms.Label();
            this.TextBox_ModuleName = new System.Windows.Forms.TextBox();
            this.TextBox_ModuleCode = new System.Windows.Forms.TextBox();
            this.GroupBox_Assertions = new System.Windows.Forms.GroupBox();
            this.CheckBox_Refinement = new System.Windows.Forms.CheckBox();
            this.CheckBox_Deterministic = new System.Windows.Forms.CheckBox();
            this.CheckBox_Divergence = new System.Windows.Forms.CheckBox();
            this.CheckBox_Reachabiliity = new System.Windows.Forms.CheckBox();
            this.CheckBox_LTL = new System.Windows.Forms.CheckBox();
            this.CheckBox_Deadlock = new System.Windows.Forms.CheckBox();
            this.TextBox_OutputFile = new System.Windows.Forms.TextBox();
            this.Button_BrowseOutput = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.ComboBox_SemanticModel = new System.Windows.Forms.ComboBox();
            this.Label_SemanticModel = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.TextBox_CustomSyntax = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.CheckBox_BDD = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.PictureBox_ModuleIcon = new System.Windows.Forms.PictureBox();
            this.LinkLabel_ModuleIcon = new System.Windows.Forms.LinkLabel();
            this.GroupBox_Assertions.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.PictureBox_ModuleIcon)).BeginInit();
            this.SuspendLayout();
            // 
            // Button_Generate
            // 
            this.Button_Generate.Location = new System.Drawing.Point(117, 482);
            this.Button_Generate.Name = "Button_Generate";
            this.Button_Generate.Size = new System.Drawing.Size(75, 23);
            this.Button_Generate.TabIndex = 8;
            this.Button_Generate.Text = "Generate";
            this.Button_Generate.UseVisualStyleBackColor = true;
            this.Button_Generate.Click += new System.EventHandler(this.Button_Generate_Click);
            // 
            // Button_Cancel
            // 
            this.Button_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Button_Cancel.Location = new System.Drawing.Point(242, 481);
            this.Button_Cancel.Name = "Button_Cancel";
            this.Button_Cancel.Size = new System.Drawing.Size(75, 23);
            this.Button_Cancel.TabIndex = 9;
            this.Button_Cancel.Text = "Cancel";
            this.Button_Cancel.UseVisualStyleBackColor = true;
            // 
            // Label_ModuleName
            // 
            this.Label_ModuleName.AutoSize = true;
            this.Label_ModuleName.Location = new System.Drawing.Point(27, 18);
            this.Label_ModuleName.Name = "Label_ModuleName";
            this.Label_ModuleName.Size = new System.Drawing.Size(73, 13);
            this.Label_ModuleName.TabIndex = 0;
            this.Label_ModuleName.Text = "Module Name";
            // 
            // Label_ModuleCode
            // 
            this.Label_ModuleCode.AutoSize = true;
            this.Label_ModuleCode.Location = new System.Drawing.Point(27, 48);
            this.Label_ModuleCode.Name = "Label_ModuleCode";
            this.Label_ModuleCode.Size = new System.Drawing.Size(70, 13);
            this.Label_ModuleCode.TabIndex = 2;
            this.Label_ModuleCode.Text = "Module Code";
            // 
            // TextBox_ModuleName
            // 
            this.TextBox_ModuleName.Location = new System.Drawing.Point(117, 15);
            this.TextBox_ModuleName.Name = "TextBox_ModuleName";
            this.TextBox_ModuleName.Size = new System.Drawing.Size(286, 20);
            this.TextBox_ModuleName.TabIndex = 0;
            this.TextBox_ModuleName.Text = "Event Grammar";
            // 
            // TextBox_ModuleCode
            // 
            this.TextBox_ModuleCode.Location = new System.Drawing.Point(117, 44);
            this.TextBox_ModuleCode.MaxLength = 15;
            this.TextBox_ModuleCode.Name = "TextBox_ModuleCode";
            this.TextBox_ModuleCode.Size = new System.Drawing.Size(286, 20);
            this.TextBox_ModuleCode.TabIndex = 1;
            this.TextBox_ModuleCode.Text = "EG";
            // 
            // GroupBox_Assertions
            // 
            this.GroupBox_Assertions.Controls.Add(this.CheckBox_Refinement);
            this.GroupBox_Assertions.Controls.Add(this.CheckBox_Deterministic);
            this.GroupBox_Assertions.Controls.Add(this.CheckBox_Divergence);
            this.GroupBox_Assertions.Controls.Add(this.CheckBox_Reachabiliity);
            this.GroupBox_Assertions.Controls.Add(this.CheckBox_LTL);
            this.GroupBox_Assertions.Controls.Add(this.CheckBox_Deadlock);
            this.GroupBox_Assertions.Location = new System.Drawing.Point(30, 257);
            this.GroupBox_Assertions.Name = "GroupBox_Assertions";
            this.GroupBox_Assertions.Size = new System.Drawing.Size(373, 99);
            this.GroupBox_Assertions.TabIndex = 4;
            this.GroupBox_Assertions.TabStop = false;
            this.GroupBox_Assertions.Text = "Supported Assertions";
            // 
            // CheckBox_Refinement
            // 
            this.CheckBox_Refinement.AutoSize = true;
            this.CheckBox_Refinement.Checked = true;
            this.CheckBox_Refinement.CheckState = System.Windows.Forms.CheckState.Checked;
            this.CheckBox_Refinement.Location = new System.Drawing.Point(212, 71);
            this.CheckBox_Refinement.Name = "CheckBox_Refinement";
            this.CheckBox_Refinement.Size = new System.Drawing.Size(128, 17);
            this.CheckBox_Refinement.TabIndex = 5;
            this.CheckBox_Refinement.Text = "Refinement Checking";
            this.CheckBox_Refinement.UseVisualStyleBackColor = true;
            // 
            // CheckBox_Deterministic
            // 
            this.CheckBox_Deterministic.AutoSize = true;
            this.CheckBox_Deterministic.Checked = true;
            this.CheckBox_Deterministic.CheckState = System.Windows.Forms.CheckState.Checked;
            this.CheckBox_Deterministic.Location = new System.Drawing.Point(212, 25);
            this.CheckBox_Deterministic.Name = "CheckBox_Deterministic";
            this.CheckBox_Deterministic.Size = new System.Drawing.Size(134, 17);
            this.CheckBox_Deterministic.TabIndex = 3;
            this.CheckBox_Deterministic.Text = "Deterministic Checking";
            this.CheckBox_Deterministic.UseVisualStyleBackColor = true;
            // 
            // CheckBox_Divergence
            // 
            this.CheckBox_Divergence.AutoSize = true;
            this.CheckBox_Divergence.Checked = true;
            this.CheckBox_Divergence.CheckState = System.Windows.Forms.CheckState.Checked;
            this.CheckBox_Divergence.Location = new System.Drawing.Point(212, 48);
            this.CheckBox_Divergence.Name = "CheckBox_Divergence";
            this.CheckBox_Divergence.Size = new System.Drawing.Size(129, 17);
            this.CheckBox_Divergence.TabIndex = 4;
            this.CheckBox_Divergence.Text = "Divergence Checking";
            this.CheckBox_Divergence.UseVisualStyleBackColor = true;
            // 
            // CheckBox_Reachabiliity
            // 
            this.CheckBox_Reachabiliity.AutoSize = true;
            this.CheckBox_Reachabiliity.Checked = true;
            this.CheckBox_Reachabiliity.CheckState = System.Windows.Forms.CheckState.Checked;
            this.CheckBox_Reachabiliity.Location = new System.Drawing.Point(16, 48);
            this.CheckBox_Reachabiliity.Name = "CheckBox_Reachabiliity";
            this.CheckBox_Reachabiliity.Size = new System.Drawing.Size(132, 17);
            this.CheckBox_Reachabiliity.TabIndex = 1;
            this.CheckBox_Reachabiliity.Text = "Reachability Checking";
            this.CheckBox_Reachabiliity.UseVisualStyleBackColor = true;
            // 
            // CheckBox_LTL
            // 
            this.CheckBox_LTL.AutoSize = true;
            this.CheckBox_LTL.Checked = true;
            this.CheckBox_LTL.CheckState = System.Windows.Forms.CheckState.Checked;
            this.CheckBox_LTL.Location = new System.Drawing.Point(16, 71);
            this.CheckBox_LTL.Name = "CheckBox_LTL";
            this.CheckBox_LTL.Size = new System.Drawing.Size(131, 17);
            this.CheckBox_LTL.TabIndex = 2;
            this.CheckBox_LTL.Text = "Linear Temporal Logic";
            this.CheckBox_LTL.UseVisualStyleBackColor = true;
            // 
            // CheckBox_Deadlock
            // 
            this.CheckBox_Deadlock.AutoSize = true;
            this.CheckBox_Deadlock.Checked = true;
            this.CheckBox_Deadlock.CheckState = System.Windows.Forms.CheckState.Checked;
            this.CheckBox_Deadlock.Location = new System.Drawing.Point(16, 25);
            this.CheckBox_Deadlock.Name = "CheckBox_Deadlock";
            this.CheckBox_Deadlock.Size = new System.Drawing.Size(90, 17);
            this.CheckBox_Deadlock.TabIndex = 0;
            this.CheckBox_Deadlock.Text = "Deadlockfree";
            this.CheckBox_Deadlock.UseVisualStyleBackColor = true;
            // 
            // TextBox_OutputFile
            // 
            this.TextBox_OutputFile.Location = new System.Drawing.Point(30, 438);
            this.TextBox_OutputFile.Name = "TextBox_OutputFile";
            this.TextBox_OutputFile.Size = new System.Drawing.Size(286, 20);
            this.TextBox_OutputFile.TabIndex = 6;
            // 
            // Button_BrowseOutput
            // 
            this.Button_BrowseOutput.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.Button_BrowseOutput.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.Button_BrowseOutput.Location = new System.Drawing.Point(322, 436);
            this.Button_BrowseOutput.Name = "Button_BrowseOutput";
            this.Button_BrowseOutput.Size = new System.Drawing.Size(81, 23);
            this.Button_BrowseOutput.TabIndex = 7;
            this.Button_BrowseOutput.Text = "Browse...";
            this.Button_BrowseOutput.UseVisualStyleBackColor = true;
            this.Button_BrowseOutput.Click += new System.EventHandler(this.Button_BrowseOutput_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(27, 422);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(71, 13);
            this.label2.TabIndex = 14;
            this.label2.Text = "Output Folder";
            // 
            // ComboBox_SemanticModel
            // 
            this.ComboBox_SemanticModel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ComboBox_SemanticModel.FormattingEnabled = true;
            this.ComboBox_SemanticModel.Items.AddRange(new object[] {
            "Labeled Transition System (LTS)",
            "Timed Transition System (TTS)",
            "Markov Decision Processes (MDP)"});
            this.ComboBox_SemanticModel.Location = new System.Drawing.Point(105, 14);
            this.ComboBox_SemanticModel.Name = "ComboBox_SemanticModel";
            this.ComboBox_SemanticModel.Size = new System.Drawing.Size(262, 21);
            this.ComboBox_SemanticModel.TabIndex = 0;
            // 
            // Label_SemanticModel
            // 
            this.Label_SemanticModel.AutoSize = true;
            this.Label_SemanticModel.Location = new System.Drawing.Point(8, 18);
            this.Label_SemanticModel.Name = "Label_SemanticModel";
            this.Label_SemanticModel.Size = new System.Drawing.Size(83, 13);
            this.Label_SemanticModel.TabIndex = 4;
            this.Label_SemanticModel.Text = "Semantic Model";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(230, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "Language Syntax Classes (separate by comma)";
            // 
            // TextBox_CustomSyntax
            // 
            this.TextBox_CustomSyntax.Location = new System.Drawing.Point(6, 34);
            this.TextBox_CustomSyntax.MaxLength = 0;
            this.TextBox_CustomSyntax.Multiline = true;
            this.TextBox_CustomSyntax.Name = "TextBox_CustomSyntax";
            this.TextBox_CustomSyntax.Size = new System.Drawing.Size(361, 60);
            this.TextBox_CustomSyntax.TabIndex = 0;
            this.TextBox_CustomSyntax.Text = "Event1,Event2";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.TextBox_CustomSyntax);
            this.groupBox1.Location = new System.Drawing.Point(30, 72);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(373, 100);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Syntax";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.CheckBox_BDD);
            this.groupBox2.Controls.Add(this.Label_SemanticModel);
            this.groupBox2.Controls.Add(this.ComboBox_SemanticModel);
            this.groupBox2.Location = new System.Drawing.Point(30, 178);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(373, 73);
            this.groupBox2.TabIndex = 3;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Semantics";
            // 
            // CheckBox_BDD
            // 
            this.CheckBox_BDD.AutoSize = true;
            this.CheckBox_BDD.Checked = true;
            this.CheckBox_BDD.CheckState = System.Windows.Forms.CheckState.Checked;
            this.CheckBox_BDD.Location = new System.Drawing.Point(16, 45);
            this.CheckBox_BDD.Name = "CheckBox_BDD";
            this.CheckBox_BDD.Size = new System.Drawing.Size(188, 17);
            this.CheckBox_BDD.TabIndex = 1;
            this.CheckBox_BDD.Text = "Generate BDD Encoding Methods";
            this.CheckBox_BDD.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(28, 384);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(66, 13);
            this.label3.TabIndex = 15;
            this.label3.Text = "Module Icon";
            // 
            // PictureBox_ModuleIcon
            // 
            this.PictureBox_ModuleIcon.Location = new System.Drawing.Point(354, 384);
            this.PictureBox_ModuleIcon.Name = "PictureBox_ModuleIcon";
            this.PictureBox_ModuleIcon.Size = new System.Drawing.Size(16, 16);
            this.PictureBox_ModuleIcon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.PictureBox_ModuleIcon.TabIndex = 16;
            this.PictureBox_ModuleIcon.TabStop = false;
            // 
            // LinkLabel_ModuleIcon
            // 
            this.LinkLabel_ModuleIcon.AutoSize = true;
            this.LinkLabel_ModuleIcon.Location = new System.Drawing.Point(100, 385);
            this.LinkLabel_ModuleIcon.Name = "LinkLabel_ModuleIcon";
            this.LinkLabel_ModuleIcon.Size = new System.Drawing.Size(234, 13);
            this.LinkLabel_ModuleIcon.TabIndex = 5;
            this.LinkLabel_ModuleIcon.TabStop = true;
            this.LinkLabel_ModuleIcon.Text = "Click to upload a module icon (16 pixel *16 pixel)";
            this.LinkLabel_ModuleIcon.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
            // 
            // ModuleGeneratorForm
            // 
            this.AcceptButton = this.Button_Generate;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.Button_Cancel;
            this.ClientSize = new System.Drawing.Size(438, 530);
            this.Controls.Add(this.LinkLabel_ModuleIcon);
            this.Controls.Add(this.PictureBox_ModuleIcon);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.TextBox_OutputFile);
            this.Controls.Add(this.Button_BrowseOutput);
            this.Controls.Add(this.GroupBox_Assertions);
            this.Controls.Add(this.TextBox_ModuleCode);
            this.Controls.Add(this.TextBox_ModuleName);
            this.Controls.Add(this.Label_ModuleCode);
            this.Controls.Add(this.Label_ModuleName);
            this.Controls.Add(this.Button_Cancel);
            this.Controls.Add(this.Button_Generate);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ModuleGeneratorForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Module Generator";
            this.GroupBox_Assertions.ResumeLayout(false);
            this.GroupBox_Assertions.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.PictureBox_ModuleIcon)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button Button_Generate;
        private System.Windows.Forms.Button Button_Cancel;
        private System.Windows.Forms.Label Label_ModuleName;
        private System.Windows.Forms.Label Label_ModuleCode;
        private System.Windows.Forms.TextBox TextBox_ModuleName;
        private System.Windows.Forms.TextBox TextBox_ModuleCode;
        private System.Windows.Forms.GroupBox GroupBox_Assertions;
        private System.Windows.Forms.CheckBox CheckBox_Deadlock;
        private System.Windows.Forms.CheckBox CheckBox_Reachabiliity;
        private System.Windows.Forms.CheckBox CheckBox_LTL;
        private System.Windows.Forms.CheckBox CheckBox_Deterministic;
        private System.Windows.Forms.CheckBox CheckBox_Divergence;
        private System.Windows.Forms.TextBox TextBox_OutputFile;
        protected System.Windows.Forms.Button Button_BrowseOutput;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox ComboBox_SemanticModel;
        private System.Windows.Forms.Label Label_SemanticModel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox TextBox_CustomSyntax;
        private System.Windows.Forms.CheckBox CheckBox_Refinement;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.CheckBox CheckBox_BDD;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.PictureBox PictureBox_ModuleIcon;
        private System.Windows.Forms.LinkLabel LinkLabel_ModuleIcon;
    }
}