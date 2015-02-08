namespace PAT.Common.GUI
{
    partial class ModelCheckingForm1
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ModelCheckingForm1));
            System.Windows.Forms.ColumnHeader columnHeader2;
            System.Windows.Forms.ColumnHeader columnHeader1;
            this.ContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.MenuItem_Copy = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuItem_Paste = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuItem_Cut = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.MenuItem_SelectAll = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuItem_Clear = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.MenuItem_SaveAs = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.StatusLabel_Text = new System.Windows.Forms.ToolStripStatusLabel();
            this.ProgressBar = new System.Windows.Forms.ToolStripProgressBar();
            this.ToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.Button_Verify = new System.Windows.Forms.Button();
            this.Button_BAGraph = new System.Windows.Forms.Button();
            this.Label_Fairness = new System.Windows.Forms.Label();
            this.CheckBox_ShortestTrace = new System.Windows.Forms.CheckBox();
            this.CheckBox_PartialOrderReduction = new System.Windows.Forms.CheckBox();
            this.CheckBox_BDD = new System.Windows.Forms.CheckBox();
            this.CheckBox_Parallel = new System.Windows.Forms.CheckBox();
            this.CheckBox_Verbose = new System.Windows.Forms.CheckBox();
            this.CheckBox_CheckNonZenoness = new System.Windows.Forms.CheckBox();
            this.Label_TimeOutAfter = new System.Windows.Forms.Label();
            this.ImageList = new System.Windows.Forms.ImageList(this.components);
            this.MCTimer = new System.Windows.Forms.Timer(this.components);
            this.GroupBox_Assertions = new System.Windows.Forms.GroupBox();
            this.ListView_Assertions = new System.Windows.Forms.ListView();
            this.GroupBox_SelectedAssertion = new System.Windows.Forms.GroupBox();
            this.CheckBox_VerifyAll = new System.Windows.Forms.CheckBox();
            this.Label_SelectedAssertion = new System.Windows.Forms.Label();
            this.Button_SimulateWitnessTrace = new System.Windows.Forms.Button();
            this.GroupBox_Options = new System.Windows.Forms.GroupBox();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.ComboBox_Fairness = new System.Windows.Forms.ComboBox();
            this.NumericUpDown_TimeOut = new System.Windows.Forms.NumericUpDown();
            this.GroupBox_Output = new System.Windows.Forms.GroupBox();
            this.TextBox_Output = new System.Windows.Forms.RichTextBox();
            columnHeader3 = new System.Windows.Forms.ColumnHeader();
            columnHeader2 = new System.Windows.Forms.ColumnHeader();
            columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.ContextMenu.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.GroupBox_Assertions.SuspendLayout();
            this.GroupBox_SelectedAssertion.SuspendLayout();
            this.GroupBox_Options.SuspendLayout();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NumericUpDown_TimeOut)).BeginInit();
            this.GroupBox_Output.SuspendLayout();
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
            // ContextMenu
            // 
            this.ContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MenuItem_Copy,
            this.MenuItem_Paste,
            this.MenuItem_Cut,
            this.toolStripSeparator1,
            this.MenuItem_SelectAll,
            this.MenuItem_Clear,
            this.toolStripSeparator2,
            this.MenuItem_SaveAs});
            this.ContextMenu.Name = "ContextMenu";
            resources.ApplyResources(this.ContextMenu, "ContextMenu");
            // 
            // MenuItem_Copy
            // 
            resources.ApplyResources(this.MenuItem_Copy, "MenuItem_Copy");
            this.MenuItem_Copy.Name = "MenuItem_Copy";
            this.MenuItem_Copy.Click += new System.EventHandler(this.MenuItem_Copy_Click);
            // 
            // MenuItem_Paste
            // 
            resources.ApplyResources(this.MenuItem_Paste, "MenuItem_Paste");
            this.MenuItem_Paste.Name = "MenuItem_Paste";
            // 
            // MenuItem_Cut
            // 
            resources.ApplyResources(this.MenuItem_Cut, "MenuItem_Cut");
            this.MenuItem_Cut.Name = "MenuItem_Cut";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            resources.ApplyResources(this.toolStripSeparator1, "toolStripSeparator1");
            // 
            // MenuItem_SelectAll
            // 
            this.MenuItem_SelectAll.Name = "MenuItem_SelectAll";
            resources.ApplyResources(this.MenuItem_SelectAll, "MenuItem_SelectAll");
            this.MenuItem_SelectAll.Click += new System.EventHandler(this.MenuItem_SelectAll_Click);
            // 
            // MenuItem_Clear
            // 
            this.MenuItem_Clear.Name = "MenuItem_Clear";
            resources.ApplyResources(this.MenuItem_Clear, "MenuItem_Clear");
            this.MenuItem_Clear.Click += new System.EventHandler(this.MenuItem_Clear_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            resources.ApplyResources(this.toolStripSeparator2, "toolStripSeparator2");
            // 
            // MenuItem_SaveAs
            // 
            resources.ApplyResources(this.MenuItem_SaveAs, "MenuItem_SaveAs");
            this.MenuItem_SaveAs.Name = "MenuItem_SaveAs";
            this.MenuItem_SaveAs.Click += new System.EventHandler(this.MenuItem_SaveAs_Click);
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
            // Button_BAGraph
            // 
            resources.ApplyResources(this.Button_BAGraph, "Button_BAGraph");
            this.Button_BAGraph.Name = "Button_BAGraph";
            this.ToolTip.SetToolTip(this.Button_BAGraph, resources.GetString("Button_BAGraph.ToolTip"));
            this.Button_BAGraph.UseVisualStyleBackColor = true;
            this.Button_BAGraph.Click += new System.EventHandler(this.Button_BAGraph_Click);
            // 
            // Label_Fairness
            // 
            resources.ApplyResources(this.Label_Fairness, "Label_Fairness");
            this.Label_Fairness.Name = "Label_Fairness";
            this.ToolTip.SetToolTip(this.Label_Fairness, resources.GetString("Label_Fairness.ToolTip"));
            // 
            // CheckBox_ShortestTrace
            // 
            resources.ApplyResources(this.CheckBox_ShortestTrace, "CheckBox_ShortestTrace");
            this.CheckBox_ShortestTrace.Name = "CheckBox_ShortestTrace";
            this.ToolTip.SetToolTip(this.CheckBox_ShortestTrace, resources.GetString("CheckBox_ShortestTrace.ToolTip"));
            this.CheckBox_ShortestTrace.UseVisualStyleBackColor = true;
            this.CheckBox_ShortestTrace.CheckedChanged += new System.EventHandler(this.CheckBox_ShortestTrace_CheckedChanged);
            // 
            // CheckBox_PartialOrderReduction
            // 
            resources.ApplyResources(this.CheckBox_PartialOrderReduction, "CheckBox_PartialOrderReduction");
            this.CheckBox_PartialOrderReduction.Name = "CheckBox_PartialOrderReduction";
            this.ToolTip.SetToolTip(this.CheckBox_PartialOrderReduction, resources.GetString("CheckBox_PartialOrderReduction.ToolTip"));
            this.CheckBox_PartialOrderReduction.UseVisualStyleBackColor = true;
            this.CheckBox_PartialOrderReduction.CheckedChanged += new System.EventHandler(this.CheckBox_POR_CheckedChanged);
            // 
            // CheckBox_BDD
            // 
            resources.ApplyResources(this.CheckBox_BDD, "CheckBox_BDD");
            this.CheckBox_BDD.Name = "CheckBox_BDD";
            this.ToolTip.SetToolTip(this.CheckBox_BDD, resources.GetString("CheckBox_BDD.ToolTip"));
            this.CheckBox_BDD.UseVisualStyleBackColor = true;
            //add the event handler when check box is ticked
            this.CheckBox_BDD.CheckedChanged += new System.EventHandler(this.CheckBox_BDD_CheckedChanged);
            // 
            // CheckBox_Parallel
            // 
            resources.ApplyResources(this.CheckBox_Parallel, "CheckBox_Parallel");
            this.CheckBox_Parallel.Name = "CheckBox_Parallel";
            this.ToolTip.SetToolTip(this.CheckBox_Parallel, resources.GetString("CheckBox_Parallel.ToolTip"));
            this.CheckBox_Parallel.UseVisualStyleBackColor = true;
            this.CheckBox_Parallel.CheckedChanged += new System.EventHandler(this.CheckBox_Parallel_CheckedChanged);
            // 
            // CheckBox_Verbose
            // 
            resources.ApplyResources(this.CheckBox_Verbose, "CheckBox_Verbose");
            this.CheckBox_Verbose.Name = "CheckBox_Verbose";
            this.ToolTip.SetToolTip(this.CheckBox_Verbose, resources.GetString("CheckBox_Verbose.ToolTip"));
            this.CheckBox_Verbose.UseVisualStyleBackColor = true;
            this.CheckBox_Verbose.CheckedChanged += new System.EventHandler(this.CheckBox_Verbose_CheckedChanged);
            // 
            // CheckBox_CheckNonZenoness
            // 
            resources.ApplyResources(this.CheckBox_CheckNonZenoness, "CheckBox_CheckNonZenoness");
            this.CheckBox_CheckNonZenoness.Name = "CheckBox_CheckNonZenoness";
            this.ToolTip.SetToolTip(this.CheckBox_CheckNonZenoness, resources.GetString("CheckBox_CheckNonZenoness.ToolTip"));
            this.CheckBox_CheckNonZenoness.UseVisualStyleBackColor = true;
            this.CheckBox_CheckNonZenoness.CheckedChanged += new System.EventHandler(this.CheckBox_CheckNonZeroness_CheckedChanged);
            // 
            // Label_TimeOutAfter
            // 
            resources.ApplyResources(this.Label_TimeOutAfter, "Label_TimeOutAfter");
            this.Label_TimeOutAfter.Name = "Label_TimeOutAfter";
            this.ToolTip.SetToolTip(this.Label_TimeOutAfter, resources.GetString("Label_TimeOutAfter.ToolTip"));
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
            // GroupBox_Assertions
            // 
            this.GroupBox_Assertions.Controls.Add(this.ListView_Assertions);
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
            this.ListView_Assertions.Name = "ListView_Assertions";
            this.ListView_Assertions.ShowGroups = false;
            this.ListView_Assertions.SmallImageList = this.ImageList;
            this.ListView_Assertions.UseCompatibleStateImageBehavior = false;
            this.ListView_Assertions.View = System.Windows.Forms.View.Details;
            this.ListView_Assertions.DoubleClick += new System.EventHandler(this.ListView_Assertions_DoubleClick);
            this.ListView_Assertions.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.ListView_Assertions_ItemSelectionChanged);
            // 
            // GroupBox_SelectedAssertion
            // 
            this.GroupBox_SelectedAssertion.Controls.Add(this.CheckBox_VerifyAll);
            this.GroupBox_SelectedAssertion.Controls.Add(this.Label_SelectedAssertion);
            this.GroupBox_SelectedAssertion.Controls.Add(this.Button_Verify);
            this.GroupBox_SelectedAssertion.Controls.Add(this.Button_BAGraph);
            this.GroupBox_SelectedAssertion.Controls.Add(this.Button_SimulateWitnessTrace);
            resources.ApplyResources(this.GroupBox_SelectedAssertion, "GroupBox_SelectedAssertion");
            this.GroupBox_SelectedAssertion.Name = "GroupBox_SelectedAssertion";
            this.GroupBox_SelectedAssertion.TabStop = false;
            // 
            // CheckBox_VerifyAll
            // 
            resources.ApplyResources(this.CheckBox_VerifyAll, "CheckBox_VerifyAll");
            this.CheckBox_VerifyAll.Name = "CheckBox_VerifyAll";
            this.CheckBox_VerifyAll.UseVisualStyleBackColor = true;
            // 
            // Label_SelectedAssertion
            // 
            resources.ApplyResources(this.Label_SelectedAssertion, "Label_SelectedAssertion");
            this.Label_SelectedAssertion.Name = "Label_SelectedAssertion";
            // 
            // Button_SimulateWitnessTrace
            // 
            resources.ApplyResources(this.Button_SimulateWitnessTrace, "Button_SimulateWitnessTrace");
            this.Button_SimulateWitnessTrace.Name = "Button_SimulateWitnessTrace";
            this.Button_SimulateWitnessTrace.UseVisualStyleBackColor = true;
            this.Button_SimulateWitnessTrace.Click += new System.EventHandler(this.Button_SimulateCounterExample_Click);
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
            this.splitContainer1.Panel1.Controls.Add(this.Label_TimeOutAfter);
            this.splitContainer1.Panel1.Controls.Add(this.ComboBox_Fairness);
            this.splitContainer1.Panel1.Controls.Add(this.NumericUpDown_TimeOut);
            this.splitContainer1.Panel1.Controls.Add(this.Label_Fairness);
            this.splitContainer1.Panel1.Controls.Add(this.CheckBox_ShortestTrace);
            this.splitContainer1.Panel1.Controls.Add(this.CheckBox_PartialOrderReduction);
            this.splitContainer1.Panel1.Controls.Add(this.CheckBox_BDD);
            this.splitContainer1.Panel1.Controls.Add(this.CheckBox_Parallel);
            this.splitContainer1.Panel1.Controls.Add(this.CheckBox_Verbose);
            this.splitContainer1.Panel1.Controls.Add(this.CheckBox_CheckNonZenoness);
            this.splitContainer1.Panel2Collapsed = true;
            // 
            // ComboBox_Fairness
            // 
            this.ComboBox_Fairness.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            resources.ApplyResources(this.ComboBox_Fairness, "ComboBox_Fairness");
            this.ComboBox_Fairness.FormattingEnabled = true;
            this.ComboBox_Fairness.Items.AddRange(new object[] {
            resources.GetString("ComboBox_Fairness.Items"),
            resources.GetString("ComboBox_Fairness.Items1"),
            resources.GetString("ComboBox_Fairness.Items2"),
            resources.GetString("ComboBox_Fairness.Items3"),
            resources.GetString("ComboBox_Fairness.Items4"),
            resources.GetString("ComboBox_Fairness.Items5")});
            this.ComboBox_Fairness.Name = "ComboBox_Fairness";
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
            // GroupBox_Output
            // 
            this.GroupBox_Output.Controls.Add(this.TextBox_Output);
            resources.ApplyResources(this.GroupBox_Output, "GroupBox_Output");
            this.GroupBox_Output.Name = "GroupBox_Output";
            this.GroupBox_Output.TabStop = false;
            // 
            // TextBox_Output
            // 
            this.TextBox_Output.BackColor = System.Drawing.Color.White;
            resources.ApplyResources(this.TextBox_Output, "TextBox_Output");
            this.TextBox_Output.Name = "TextBox_Output";
            this.TextBox_Output.ReadOnly = true;
            this.TextBox_Output.MouseUp += new System.Windows.Forms.MouseEventHandler(this.TextBox_Output_MouseUp);
            // 
            // ModelCheckingForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.GroupBox_Output);
            this.Controls.Add(this.GroupBox_Options);
            this.Controls.Add(this.GroupBox_SelectedAssertion);
            this.Controls.Add(this.GroupBox_Assertions);
            this.Controls.Add(this.statusStrip1);
            this.Name = "ModelCheckingForm";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.ModelCheckingForm_FormClosed);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ModelCheckingForm_FormClosing);
            this.ContextMenu.ResumeLayout(false);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.GroupBox_Assertions.ResumeLayout(false);
            this.GroupBox_SelectedAssertion.ResumeLayout(false);
            this.GroupBox_SelectedAssertion.PerformLayout();
            this.GroupBox_Options.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.NumericUpDown_TimeOut)).EndInit();
            this.GroupBox_Output.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        protected System.Windows.Forms.ContextMenuStrip ContextMenu;
        protected System.Windows.Forms.ToolStripMenuItem MenuItem_SaveAs;
        protected System.Windows.Forms.ToolStripMenuItem MenuItem_Copy;
        protected System.Windows.Forms.ToolStripMenuItem MenuItem_Paste;
        protected System.Windows.Forms.ToolStripMenuItem MenuItem_Cut;
        protected System.Windows.Forms.ToolStripMenuItem MenuItem_SelectAll;
        protected System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        protected System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        protected System.Windows.Forms.ToolStripMenuItem MenuItem_Clear;
        protected System.Windows.Forms.StatusStrip statusStrip1;
        protected System.Windows.Forms.ToolStripStatusLabel StatusLabel_Text;
        protected System.Windows.Forms.ToolStripProgressBar ProgressBar;
        protected System.Windows.Forms.ToolTip ToolTip;
        protected System.Windows.Forms.ImageList ImageList;
        protected System.Windows.Forms.Timer MCTimer;
        protected System.Windows.Forms.GroupBox GroupBox_Assertions;
        protected System.Windows.Forms.ListView ListView_Assertions;
        protected System.Windows.Forms.GroupBox GroupBox_SelectedAssertion;
        protected System.Windows.Forms.Label Label_SelectedAssertion;
        protected System.Windows.Forms.Button Button_Verify;
        protected System.Windows.Forms.Button Button_BAGraph;
        protected System.Windows.Forms.Button Button_SimulateWitnessTrace;
        protected System.Windows.Forms.GroupBox GroupBox_Options;
        protected System.Windows.Forms.SplitContainer splitContainer1;
        protected System.Windows.Forms.ComboBox ComboBox_Fairness;
        protected System.Windows.Forms.Label Label_Fairness;
        protected System.Windows.Forms.CheckBox CheckBox_ShortestTrace;
        public System.Windows.Forms.CheckBox CheckBox_PartialOrderReduction;
        public System.Windows.Forms.CheckBox CheckBox_BDD;
        protected System.Windows.Forms.CheckBox CheckBox_Parallel;
        protected System.Windows.Forms.CheckBox CheckBox_Verbose;
        protected System.Windows.Forms.CheckBox CheckBox_CheckNonZenoness;
        protected System.Windows.Forms.GroupBox GroupBox_Output;
        protected System.Windows.Forms.RichTextBox TextBox_Output;
        private System.Windows.Forms.NumericUpDown NumericUpDown_TimeOut;
        protected System.Windows.Forms.Label Label_TimeOutAfter;
        protected System.Windows.Forms.CheckBox CheckBox_VerifyAll;

    }
}