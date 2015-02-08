namespace SearchAndReplace
{

    partial class SearchAndReplaceFrom
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SearchAndReplaceFrom));
            this.ToolStrip = new System.Windows.Forms.ToolStrip();
            this.ToolStripButton_Find = new System.Windows.Forms.ToolStripButton();
            this.ToolStripButton_Replace = new System.Windows.Forms.ToolStripButton();
            this.Label_FindWhat = new System.Windows.Forms.Label();
            this.ComboBox_FindWhat = new System.Windows.Forms.ComboBox();
            this.Label_ReplaceWith = new System.Windows.Forms.Label();
            this.ComboBox_ReplaceWith = new System.Windows.Forms.ComboBox();
            this.ComboBox_LookIn = new System.Windows.Forms.ComboBox();
            this.Label_LookIn = new System.Windows.Forms.Label();
            this.CheckBox_MatchCase = new System.Windows.Forms.CheckBox();
            this.CheckBox_MatchWholeWord = new System.Windows.Forms.CheckBox();
            this.ComboBox_Use = new System.Windows.Forms.ComboBox();
            this.Label_Use = new System.Windows.Forms.Label();
            this.Button_FindNext = new System.Windows.Forms.Button();
            this.Button_Find = new System.Windows.Forms.Button();
            this.Button_BookmarkAll = new System.Windows.Forms.Button();
            this.Button_ReplaceAll = new System.Windows.Forms.Button();
            this.Button_Replace = new System.Windows.Forms.Button();
            this.ToolStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // ToolStrip
            // 
            this.ToolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.ToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ToolStripButton_Find,
            this.ToolStripButton_Replace});
            resources.ApplyResources(this.ToolStrip, "ToolStrip");
            this.ToolStrip.Name = "ToolStrip";
            // 
            // ToolStripButton_Find
            // 
            this.ToolStripButton_Find.Checked = true;
            this.ToolStripButton_Find.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ToolStripButton_Find.Image = global::PAT.GUI.Properties.Resources.find;
            resources.ApplyResources(this.ToolStripButton_Find, "ToolStripButton_Find");
            this.ToolStripButton_Find.Name = "ToolStripButton_Find";
            this.ToolStripButton_Find.Click += new System.EventHandler(this.SearchButtonClick);
            // 
            // ToolStripButton_Replace
            // 
            this.ToolStripButton_Replace.Image = global::PAT.GUI.Properties.Resources.replace;
            resources.ApplyResources(this.ToolStripButton_Replace, "ToolStripButton_Replace");
            this.ToolStripButton_Replace.Name = "ToolStripButton_Replace";
            this.ToolStripButton_Replace.Click += new System.EventHandler(this.ReplaceButtonClick);
            // 
            // Label_FindWhat
            // 
            resources.ApplyResources(this.Label_FindWhat, "Label_FindWhat");
            this.Label_FindWhat.Name = "Label_FindWhat";
            // 
            // ComboBox_FindWhat
            // 
            this.ComboBox_FindWhat.FormattingEnabled = true;
            resources.ApplyResources(this.ComboBox_FindWhat, "ComboBox_FindWhat");
            this.ComboBox_FindWhat.Name = "ComboBox_FindWhat";
            // 
            // Label_ReplaceWith
            // 
            resources.ApplyResources(this.Label_ReplaceWith, "Label_ReplaceWith");
            this.Label_ReplaceWith.Name = "Label_ReplaceWith";
            // 
            // ComboBox_ReplaceWith
            // 
            this.ComboBox_ReplaceWith.FormattingEnabled = true;
            resources.ApplyResources(this.ComboBox_ReplaceWith, "ComboBox_ReplaceWith");
            this.ComboBox_ReplaceWith.Name = "ComboBox_ReplaceWith";
            // 
            // ComboBox_LookIn
            // 
            this.ComboBox_LookIn.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ComboBox_LookIn.FormattingEnabled = true;
            this.ComboBox_LookIn.Items.AddRange(new object[] {
            resources.GetString("ComboBox_LookIn.Items"),
            resources.GetString("ComboBox_LookIn.Items1"),
            resources.GetString("ComboBox_LookIn.Items2")});
            resources.ApplyResources(this.ComboBox_LookIn, "ComboBox_LookIn");
            this.ComboBox_LookIn.Name = "ComboBox_LookIn";
            this.ComboBox_LookIn.SelectedIndexChanged += new System.EventHandler(this.LookInSelectedIndexChanged);
            // 
            // Label_LookIn
            // 
            resources.ApplyResources(this.Label_LookIn, "Label_LookIn");
            this.Label_LookIn.Name = "Label_LookIn";
            // 
            // CheckBox_MatchCase
            // 
            resources.ApplyResources(this.CheckBox_MatchCase, "CheckBox_MatchCase");
            this.CheckBox_MatchCase.Name = "CheckBox_MatchCase";
            this.CheckBox_MatchCase.UseVisualStyleBackColor = true;
            // 
            // CheckBox_MatchWholeWord
            // 
            resources.ApplyResources(this.CheckBox_MatchWholeWord, "CheckBox_MatchWholeWord");
            this.CheckBox_MatchWholeWord.Name = "CheckBox_MatchWholeWord";
            this.CheckBox_MatchWholeWord.UseVisualStyleBackColor = true;
            // 
            // ComboBox_Use
            // 
            this.ComboBox_Use.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ComboBox_Use.FormattingEnabled = true;
            this.ComboBox_Use.Items.AddRange(new object[] {
            resources.GetString("ComboBox_Use.Items"),
            resources.GetString("ComboBox_Use.Items1"),
            resources.GetString("ComboBox_Use.Items2")});
            resources.ApplyResources(this.ComboBox_Use, "ComboBox_Use");
            this.ComboBox_Use.Name = "ComboBox_Use";
            // 
            // Label_Use
            // 
            resources.ApplyResources(this.Label_Use, "Label_Use");
            this.Label_Use.Name = "Label_Use";
            // 
            // Button_FindNext
            // 
            resources.ApplyResources(this.Button_FindNext, "Button_FindNext");
            this.Button_FindNext.Name = "Button_FindNext";
            this.Button_FindNext.UseVisualStyleBackColor = true;
            this.Button_FindNext.Click += new System.EventHandler(this.Button_FindNext_Click);
            // 
            // Button_Find
            // 
            resources.ApplyResources(this.Button_Find, "Button_Find");
            this.Button_Find.Name = "Button_Find";
            this.Button_Find.UseVisualStyleBackColor = true;
            this.Button_Find.Click += new System.EventHandler(this.Button_Find_Click);
            // 
            // Button_BookmarkAll
            // 
            resources.ApplyResources(this.Button_BookmarkAll, "Button_BookmarkAll");
            this.Button_BookmarkAll.Name = "Button_BookmarkAll";
            this.Button_BookmarkAll.UseVisualStyleBackColor = true;
            this.Button_BookmarkAll.Click += new System.EventHandler(this.Button_BookmarkAll_Click);
            // 
            // Button_ReplaceAll
            // 
            resources.ApplyResources(this.Button_ReplaceAll, "Button_ReplaceAll");
            this.Button_ReplaceAll.Name = "Button_ReplaceAll";
            this.Button_ReplaceAll.UseVisualStyleBackColor = true;
            this.Button_ReplaceAll.Click += new System.EventHandler(this.Button_ReplaceAll_Click);
            // 
            // Button_Replace
            // 
            resources.ApplyResources(this.Button_Replace, "Button_Replace");
            this.Button_Replace.Name = "Button_Replace";
            this.Button_Replace.UseVisualStyleBackColor = true;
            this.Button_Replace.Click += new System.EventHandler(this.Button_Replace_Click);
            // 
            // SearchAndReplaceFrom
            // 
            this.AcceptButton = this.Button_Find;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.Button_ReplaceAll);
            this.Controls.Add(this.Button_Replace);
            this.Controls.Add(this.Button_BookmarkAll);
            this.Controls.Add(this.Button_Find);
            this.Controls.Add(this.Button_FindNext);
            this.Controls.Add(this.ComboBox_Use);
            this.Controls.Add(this.Label_Use);
            this.Controls.Add(this.CheckBox_MatchWholeWord);
            this.Controls.Add(this.CheckBox_MatchCase);
            this.Controls.Add(this.ComboBox_LookIn);
            this.Controls.Add(this.Label_LookIn);
            this.Controls.Add(this.ComboBox_ReplaceWith);
            this.Controls.Add(this.Label_ReplaceWith);
            this.Controls.Add(this.ComboBox_FindWhat);
            this.Controls.Add(this.Label_FindWhat);
            this.Controls.Add(this.ToolStrip);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SearchAndReplaceFrom";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.ToolStrip.ResumeLayout(false);
            this.ToolStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip ToolStrip;
        private System.Windows.Forms.ToolStripButton ToolStripButton_Find;
        private System.Windows.Forms.ToolStripButton ToolStripButton_Replace;
        private System.Windows.Forms.Label Label_FindWhat;
        private System.Windows.Forms.ComboBox ComboBox_FindWhat;
        private System.Windows.Forms.Label Label_ReplaceWith;
        private System.Windows.Forms.ComboBox ComboBox_ReplaceWith;
        private System.Windows.Forms.ComboBox ComboBox_LookIn;
        private System.Windows.Forms.Label Label_LookIn;
        private System.Windows.Forms.CheckBox CheckBox_MatchCase;
        private System.Windows.Forms.CheckBox CheckBox_MatchWholeWord;
        private System.Windows.Forms.ComboBox ComboBox_Use;
        private System.Windows.Forms.Label Label_Use;
        private System.Windows.Forms.Button Button_FindNext;
        private System.Windows.Forms.Button Button_Find;
        private System.Windows.Forms.Button Button_BookmarkAll;
        private System.Windows.Forms.Button Button_ReplaceAll;
        private System.Windows.Forms.Button Button_Replace;
    }
}