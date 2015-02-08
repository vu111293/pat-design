namespace PAT.GUI.Forms
{
    partial class ModelCheckingBatchForm
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
            System.Windows.Forms.ColumnHeader columnHeader3;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ModelCheckingBatchForm));
            System.Windows.Forms.ColumnHeader columnHeader2;
            System.Windows.Forms.ColumnHeader columnHeader1;
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.StatusLabel_Text = new System.Windows.Forms.ToolStripStatusLabel();
            this.ProgressBar = new System.Windows.Forms.ToolStripProgressBar();
            this.ToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.Button_Verify = new System.Windows.Forms.Button();
            this.CheckBox_Verbose = new System.Windows.Forms.CheckBox();
            this.Label_TimeOutAfter = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.Button_RemoveFiles = new System.Windows.Forms.Button();
            this.Button_AddFolder = new System.Windows.Forms.Button();
            this.Button_AddFiles = new System.Windows.Forms.Button();
            this.Button_BrowseOutput = new System.Windows.Forms.Button();
            this.CheckBox_GenerateCounterexample = new System.Windows.Forms.CheckBox();
            this.Button_Clear = new System.Windows.Forms.Button();
            this.Button_GenerateReport = new System.Windows.Forms.Button();
            this.Label_VerificationEngine = new System.Windows.Forms.Label();
            this.Label_AdmissibleBehavior = new System.Windows.Forms.Label();
            this.ImageList = new System.Windows.Forms.ImageList(this.components);
            this.MCTimer = new System.Windows.Forms.Timer(this.components);
            this.GroupBox_Options = new System.Windows.Forms.GroupBox();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.NUD_VerificationEngine = new System.Windows.Forms.NumericUpDown();
            this.NUD_AdmissibleBehavior = new System.Windows.Forms.NumericUpDown();
            this.ComboBox_Modules = new System.Windows.Forms.ComboBox();
            this.NumericUpDown_TimeOut = new System.Windows.Forms.NumericUpDown();
            this.panel2 = new System.Windows.Forms.Panel();
            this.TextBox_Output = new System.Windows.Forms.RichTextBox();
            this.GroupBox_Assertions = new System.Windows.Forms.GroupBox();
            this.ListView_Assertions = new System.Windows.Forms.ListView();
            this.panel1 = new System.Windows.Forms.Panel();
            this.GroupBox_Output = new System.Windows.Forms.GroupBox();
            this.panel3 = new System.Windows.Forms.Panel();
            this.TextBox_OutputFile = new System.Windows.Forms.TextBox();
            columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.statusStrip1.SuspendLayout();
            this.GroupBox_Options.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NUD_VerificationEngine)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUD_AdmissibleBehavior)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumericUpDown_TimeOut)).BeginInit();
            this.panel2.SuspendLayout();
            this.GroupBox_Assertions.SuspendLayout();
            this.panel1.SuspendLayout();
            this.GroupBox_Output.SuspendLayout();
            this.panel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // columnHeader3
            // 
            resources.ApplyResources(columnHeader3, "columnHeader3");
            // 
            // columnHeader2
            // 
            resources.ApplyResources(columnHeader2, "columnHeader2");
            // 
            // columnHeader1
            // 
            resources.ApplyResources(columnHeader1, "columnHeader1");
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.StatusLabel_Text,
            this.ProgressBar});
            this.statusStrip1.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.HorizontalStackWithOverflow;
            resources.ApplyResources(this.statusStrip1, "statusStrip1");
            this.statusStrip1.Name = "statusStrip1";
            // 
            // StatusLabel_Text
            // 
            this.StatusLabel_Text.Name = "StatusLabel_Text";
            resources.ApplyResources(this.StatusLabel_Text, "StatusLabel_Text");
            // 
            // ProgressBar
            // 
            this.ProgressBar.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.ProgressBar.Name = "ProgressBar";
            resources.ApplyResources(this.ProgressBar, "ProgressBar");
            // 
            // ToolTip
            // 
            this.ToolTip.AutomaticDelay = 1;
            this.ToolTip.AutoPopDelay = 10000;
            this.ToolTip.InitialDelay = 1;
            this.ToolTip.IsBalloon = true;
            this.ToolTip.ReshowDelay = 0;
            this.ToolTip.ToolTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            this.ToolTip.ToolTipTitle = "Information";
            // 
            // Button_Verify
            // 
            resources.ApplyResources(this.Button_Verify, "Button_Verify");
            this.Button_Verify.Name = "Button_Verify";
            this.ToolTip.SetToolTip(this.Button_Verify, resources.GetString("Button_Verify.ToolTip"));
            this.Button_Verify.UseVisualStyleBackColor = true;
            this.Button_Verify.Click += new System.EventHandler(this.Button_Verify_Click);
            // 
            // CheckBox_Verbose
            // 
            resources.ApplyResources(this.CheckBox_Verbose, "CheckBox_Verbose");
            this.CheckBox_Verbose.Name = "CheckBox_Verbose";
            this.ToolTip.SetToolTip(this.CheckBox_Verbose, resources.GetString("CheckBox_Verbose.ToolTip"));
            this.CheckBox_Verbose.UseVisualStyleBackColor = true;
            this.CheckBox_Verbose.CheckedChanged += new System.EventHandler(this.CheckBox_Verbose_CheckedChanged);
            // 
            // Label_TimeOutAfter
            // 
            resources.ApplyResources(this.Label_TimeOutAfter, "Label_TimeOutAfter");
            this.Label_TimeOutAfter.Name = "Label_TimeOutAfter";
            this.ToolTip.SetToolTip(this.Label_TimeOutAfter, resources.GetString("Label_TimeOutAfter.ToolTip"));
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            this.ToolTip.SetToolTip(this.label1, resources.GetString("label1.ToolTip"));
            // 
            // Button_RemoveFiles
            // 
            resources.ApplyResources(this.Button_RemoveFiles, "Button_RemoveFiles");
            this.Button_RemoveFiles.Name = "Button_RemoveFiles";
            this.ToolTip.SetToolTip(this.Button_RemoveFiles, resources.GetString("Button_RemoveFiles.ToolTip"));
            this.Button_RemoveFiles.UseVisualStyleBackColor = true;
            this.Button_RemoveFiles.Click += new System.EventHandler(this.Button_RemoveFiles_Click);
            // 
            // Button_AddFolder
            // 
            resources.ApplyResources(this.Button_AddFolder, "Button_AddFolder");
            this.Button_AddFolder.Name = "Button_AddFolder";
            this.ToolTip.SetToolTip(this.Button_AddFolder, resources.GetString("Button_AddFolder.ToolTip"));
            this.Button_AddFolder.UseVisualStyleBackColor = true;
            this.Button_AddFolder.Click += new System.EventHandler(this.Button_AddFolder_Click);
            // 
            // Button_AddFiles
            // 
            resources.ApplyResources(this.Button_AddFiles, "Button_AddFiles");
            this.Button_AddFiles.Name = "Button_AddFiles";
            this.ToolTip.SetToolTip(this.Button_AddFiles, resources.GetString("Button_AddFiles.ToolTip"));
            this.Button_AddFiles.UseVisualStyleBackColor = true;
            this.Button_AddFiles.Click += new System.EventHandler(this.Button_AddFiles_Click);
            // 
            // Button_BrowseOutput
            // 
            resources.ApplyResources(this.Button_BrowseOutput, "Button_BrowseOutput");
            this.Button_BrowseOutput.Name = "Button_BrowseOutput";
            this.ToolTip.SetToolTip(this.Button_BrowseOutput, resources.GetString("Button_BrowseOutput.ToolTip"));
            this.Button_BrowseOutput.UseVisualStyleBackColor = true;
            this.Button_BrowseOutput.Click += new System.EventHandler(this.Button_BrowseOutput_Click);
            // 
            // CheckBox_GenerateCounterexample
            // 
            resources.ApplyResources(this.CheckBox_GenerateCounterexample, "CheckBox_GenerateCounterexample");
            this.CheckBox_GenerateCounterexample.Checked = true;
            this.CheckBox_GenerateCounterexample.CheckState = System.Windows.Forms.CheckState.Checked;
            this.CheckBox_GenerateCounterexample.Name = "CheckBox_GenerateCounterexample";
            this.ToolTip.SetToolTip(this.CheckBox_GenerateCounterexample, resources.GetString("CheckBox_GenerateCounterexample.ToolTip"));
            this.CheckBox_GenerateCounterexample.UseVisualStyleBackColor = true;
            this.CheckBox_GenerateCounterexample.CheckedChanged += new System.EventHandler(this.CheckBox_GenerateCounterexample_CheckedChanged);
            // 
            // Button_Clear
            // 
            resources.ApplyResources(this.Button_Clear, "Button_Clear");
            this.Button_Clear.Name = "Button_Clear";
            this.ToolTip.SetToolTip(this.Button_Clear, resources.GetString("Button_Clear.ToolTip"));
            this.Button_Clear.UseVisualStyleBackColor = true;
            this.Button_Clear.Click += new System.EventHandler(this.button1_Click);
            // 
            // Button_GenerateReport
            // 
            resources.ApplyResources(this.Button_GenerateReport, "Button_GenerateReport");
            this.Button_GenerateReport.Name = "Button_GenerateReport";
            this.ToolTip.SetToolTip(this.Button_GenerateReport, resources.GetString("Button_GenerateReport.ToolTip"));
            this.Button_GenerateReport.UseVisualStyleBackColor = true;
            this.Button_GenerateReport.Click += new System.EventHandler(this.Button_GenerateReport_Click);
            // 
            // Label_VerificationEngine
            // 
            resources.ApplyResources(this.Label_VerificationEngine, "Label_VerificationEngine");
            this.Label_VerificationEngine.Name = "Label_VerificationEngine";
            this.ToolTip.SetToolTip(this.Label_VerificationEngine, resources.GetString("Label_VerificationEngine.ToolTip"));
            // 
            // Label_AdmissibleBehavior
            // 
            resources.ApplyResources(this.Label_AdmissibleBehavior, "Label_AdmissibleBehavior");
            this.Label_AdmissibleBehavior.Name = "Label_AdmissibleBehavior";
            this.ToolTip.SetToolTip(this.Label_AdmissibleBehavior, resources.GetString("Label_AdmissibleBehavior.ToolTip"));
            // 
            // ImageList
            // 
            this.ImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ImageList.ImageStream")));
            this.ImageList.TransparentColor = System.Drawing.Color.Transparent;
            this.ImageList.Images.SetKeyName(0, "True");
            this.ImageList.Images.SetKeyName(1, "False");
            this.ImageList.Images.SetKeyName(2, "Unknown");
            this.ImageList.Images.SetKeyName(3, "Prob True");
            // 
            // MCTimer
            // 
            this.MCTimer.Interval = 1000;
            this.MCTimer.Tick += new System.EventHandler(this.MCTimer_Tick);
            // 
            // GroupBox_Options
            // 
            this.GroupBox_Options.Controls.Add(this.splitContainer1);
            resources.ApplyResources(this.GroupBox_Options, "GroupBox_Options");
            this.GroupBox_Options.Name = "GroupBox_Options";
            this.GroupBox_Options.TabStop = false;
            // 
            // splitContainer1
            // 
            resources.ApplyResources(this.splitContainer1, "splitContainer1");
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.NUD_VerificationEngine);
            this.splitContainer1.Panel1.Controls.Add(this.NUD_AdmissibleBehavior);
            this.splitContainer1.Panel1.Controls.Add(this.Label_VerificationEngine);
            this.splitContainer1.Panel1.Controls.Add(this.Label_AdmissibleBehavior);
            this.splitContainer1.Panel1.Controls.Add(this.CheckBox_GenerateCounterexample);
            this.splitContainer1.Panel1.Controls.Add(this.ComboBox_Modules);
            this.splitContainer1.Panel1.Controls.Add(this.label1);
            this.splitContainer1.Panel1.Controls.Add(this.Label_TimeOutAfter);
            this.splitContainer1.Panel1.Controls.Add(this.NumericUpDown_TimeOut);
            this.splitContainer1.Panel1.Controls.Add(this.CheckBox_Verbose);
            this.splitContainer1.Panel2Collapsed = true;
            // 
            // NUD_VerificationEngine
            // 
            resources.ApplyResources(this.NUD_VerificationEngine, "NUD_VerificationEngine");
            this.NUD_VerificationEngine.Maximum = new decimal(new int[] {
            10000000,
            0,
            0,
            0});
            this.NUD_VerificationEngine.Name = "NUD_VerificationEngine";
            // 
            // NUD_AdmissibleBehavior
            // 
            resources.ApplyResources(this.NUD_AdmissibleBehavior, "NUD_AdmissibleBehavior");
            this.NUD_AdmissibleBehavior.Maximum = new decimal(new int[] {
            10000000,
            0,
            0,
            0});
            this.NUD_AdmissibleBehavior.Name = "NUD_AdmissibleBehavior";
            // 
            // ComboBox_Modules
            // 
            this.ComboBox_Modules.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            resources.ApplyResources(this.ComboBox_Modules, "ComboBox_Modules");
            this.ComboBox_Modules.FormattingEnabled = true;
            this.ComboBox_Modules.Name = "ComboBox_Modules";
            // 
            // NumericUpDown_TimeOut
            // 
            resources.ApplyResources(this.NumericUpDown_TimeOut, "NumericUpDown_TimeOut");
            this.NumericUpDown_TimeOut.Maximum = new decimal(new int[] {
            10000000,
            0,
            0,
            0});
            this.NumericUpDown_TimeOut.Name = "NumericUpDown_TimeOut";
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.Button_GenerateReport);
            this.panel2.Controls.Add(this.Button_Verify);
            this.panel2.Controls.Add(this.TextBox_Output);
            resources.ApplyResources(this.panel2, "panel2");
            this.panel2.Name = "panel2";
            // 
            // TextBox_Output
            // 
            this.TextBox_Output.BackColor = System.Drawing.Color.White;
            resources.ApplyResources(this.TextBox_Output, "TextBox_Output");
            this.TextBox_Output.Name = "TextBox_Output";
            this.TextBox_Output.ReadOnly = true;
            // 
            // GroupBox_Assertions
            // 
            this.GroupBox_Assertions.Controls.Add(this.ListView_Assertions);
            this.GroupBox_Assertions.Controls.Add(this.panel1);
            resources.ApplyResources(this.GroupBox_Assertions, "GroupBox_Assertions");
            this.GroupBox_Assertions.Name = "GroupBox_Assertions";
            this.GroupBox_Assertions.TabStop = false;
            // 
            // ListView_Assertions
            // 
            resources.ApplyResources(this.ListView_Assertions, "ListView_Assertions");
            this.ListView_Assertions.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            columnHeader3,
            columnHeader2,
            columnHeader1});
            this.ListView_Assertions.FullRowSelect = true;
            this.ListView_Assertions.GridLines = true;
            this.ListView_Assertions.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.ListView_Assertions.HideSelection = false;
            this.ListView_Assertions.LargeImageList = this.ImageList;
            this.ListView_Assertions.MultiSelect = false;
            this.ListView_Assertions.Name = "ListView_Assertions";
            this.ListView_Assertions.ShowGroups = false;
            this.ListView_Assertions.SmallImageList = this.ImageList;
            this.ListView_Assertions.UseCompatibleStateImageBehavior = false;
            this.ListView_Assertions.View = System.Windows.Forms.View.Details;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.Button_Clear);
            this.panel1.Controls.Add(this.Button_RemoveFiles);
            this.panel1.Controls.Add(this.Button_AddFolder);
            this.panel1.Controls.Add(this.Button_AddFiles);
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Name = "panel1";
            // 
            // GroupBox_Output
            // 
            this.GroupBox_Output.Controls.Add(this.panel3);
            resources.ApplyResources(this.GroupBox_Output, "GroupBox_Output");
            this.GroupBox_Output.Name = "GroupBox_Output";
            this.GroupBox_Output.TabStop = false;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.TextBox_OutputFile);
            this.panel3.Controls.Add(this.Button_BrowseOutput);
            resources.ApplyResources(this.panel3, "panel3");
            this.panel3.Name = "panel3";
            // 
            // TextBox_OutputFile
            // 
            resources.ApplyResources(this.TextBox_OutputFile, "TextBox_OutputFile");
            this.TextBox_OutputFile.Name = "TextBox_OutputFile";
            // 
            // ModelCheckingBatchForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.GroupBox_Assertions);
            this.Controls.Add(this.GroupBox_Output);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.GroupBox_Options);
            this.Controls.Add(this.statusStrip1);
            this.Name = "ModelCheckingBatchForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ModelCheckingForm_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.ModelCheckingForm_FormClosed);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.GroupBox_Options.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.NUD_VerificationEngine)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUD_AdmissibleBehavior)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumericUpDown_TimeOut)).EndInit();
            this.panel2.ResumeLayout(false);
            this.GroupBox_Assertions.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.GroupBox_Output.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        protected System.Windows.Forms.StatusStrip statusStrip1;
        protected System.Windows.Forms.ToolStripStatusLabel StatusLabel_Text;
        protected System.Windows.Forms.ToolStripProgressBar ProgressBar;
        protected System.Windows.Forms.ToolTip ToolTip;
        protected System.Windows.Forms.ImageList ImageList;
        protected System.Windows.Forms.Timer MCTimer;
        protected System.Windows.Forms.Button Button_Verify;
        protected System.Windows.Forms.GroupBox GroupBox_Options;
        protected System.Windows.Forms.SplitContainer splitContainer1;
        protected System.Windows.Forms.CheckBox CheckBox_Verbose;
        private System.Windows.Forms.NumericUpDown NumericUpDown_TimeOut;
        protected System.Windows.Forms.Label Label_TimeOutAfter;
        protected System.Windows.Forms.ComboBox ComboBox_Modules;
        protected System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel2;
        protected System.Windows.Forms.GroupBox GroupBox_Assertions;
        private System.Windows.Forms.Panel panel1;
        protected System.Windows.Forms.Button Button_RemoveFiles;
        protected System.Windows.Forms.Button Button_AddFolder;
        protected System.Windows.Forms.Button Button_AddFiles;
        protected System.Windows.Forms.GroupBox GroupBox_Output;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.TextBox TextBox_OutputFile;
        protected System.Windows.Forms.Button Button_BrowseOutput;
        protected System.Windows.Forms.ListView ListView_Assertions;
        protected System.Windows.Forms.RichTextBox TextBox_Output;
        protected System.Windows.Forms.CheckBox CheckBox_GenerateCounterexample;
        protected System.Windows.Forms.Button Button_Clear;
        protected System.Windows.Forms.Button Button_GenerateReport;
        private System.Windows.Forms.NumericUpDown NUD_VerificationEngine;
        private System.Windows.Forms.NumericUpDown NUD_AdmissibleBehavior;
        protected System.Windows.Forms.Label Label_VerificationEngine;
        protected System.Windows.Forms.Label Label_AdmissibleBehavior;

    }
}