namespace PAT.GUI.Forms
{
    partial class CSharpLibraryForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CSharpLibraryForm));
            this.toolStripContainer1 = new System.Windows.Forms.ToolStripContainer();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.StatusLabel_Status = new System.Windows.Forms.ToolStripStatusLabel();
            this.StatusLabel_FileName = new System.Windows.Forms.ToolStripStatusLabel();
            this.TextBox_Code = new System.Windows.Forms.RichTextBox();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.Button_NewLibrary = new System.Windows.Forms.ToolStripButton();
            this.Button_NewDataType = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.Button_LoadCSharpCode = new System.Windows.Forms.ToolStripButton();
            this.Button_SaveCode = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.Button_BuildMode = new System.Windows.Forms.ToolStripComboBox();
            this.Button_Build = new System.Windows.Forms.ToolStripButton();
            this.toolStripContainer1.BottomToolStripPanel.SuspendLayout();
            this.toolStripContainer1.ContentPanel.SuspendLayout();
            this.toolStripContainer1.TopToolStripPanel.SuspendLayout();
            this.toolStripContainer1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStripContainer1
            // 
            // 
            // toolStripContainer1.BottomToolStripPanel
            // 
            this.toolStripContainer1.BottomToolStripPanel.Controls.Add(this.statusStrip1);
            // 
            // toolStripContainer1.ContentPanel
            // 
            this.toolStripContainer1.ContentPanel.Controls.Add(this.TextBox_Code);
            resources.ApplyResources(this.toolStripContainer1.ContentPanel, "toolStripContainer1.ContentPanel");
            resources.ApplyResources(this.toolStripContainer1, "toolStripContainer1");
            this.toolStripContainer1.Name = "toolStripContainer1";
            // 
            // toolStripContainer1.TopToolStripPanel
            // 
            this.toolStripContainer1.TopToolStripPanel.Controls.Add(this.toolStrip1);
            // 
            // statusStrip1
            // 
            resources.ApplyResources(this.statusStrip1, "statusStrip1");
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.StatusLabel_Status,
            this.StatusLabel_FileName});
            this.statusStrip1.Name = "statusStrip1";
            // 
            // StatusLabel_Status
            // 
            this.StatusLabel_Status.Name = "StatusLabel_Status";
            resources.ApplyResources(this.StatusLabel_Status, "StatusLabel_Status");
            // 
            // StatusLabel_FileName
            // 
            this.StatusLabel_FileName.IsLink = true;
            this.StatusLabel_FileName.Name = "StatusLabel_FileName";
            resources.ApplyResources(this.StatusLabel_FileName, "StatusLabel_FileName");
            this.StatusLabel_FileName.Spring = true;
            this.StatusLabel_FileName.Click += new System.EventHandler(this.StatusLabel_FileName_Click);
            // 
            // TextBox_Code
            // 
            resources.ApplyResources(this.TextBox_Code, "TextBox_Code");
            this.TextBox_Code.Name = "TextBox_Code";
            // 
            // toolStrip1
            // 
            resources.ApplyResources(this.toolStrip1, "toolStrip1");
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.Button_NewLibrary,
            this.Button_NewDataType,
            this.toolStripSeparator2,
            this.Button_LoadCSharpCode,
            this.Button_SaveCode,
            this.toolStripSeparator1,
            this.Button_BuildMode,
            this.Button_Build});
            this.toolStrip1.Name = "toolStrip1";
            // 
            // Button_NewLibrary
            // 
            this.Button_NewLibrary.Image = global::PAT.GUI.Properties.Resources.csharp;
            resources.ApplyResources(this.Button_NewLibrary, "Button_NewLibrary");
            this.Button_NewLibrary.Name = "Button_NewLibrary";
            this.Button_NewLibrary.Click += new System.EventHandler(this.Button_New_Click);
            // 
            // Button_NewDataType
            // 
            this.Button_NewDataType.Image = global::PAT.GUI.Properties.Resources.csharp;
            resources.ApplyResources(this.Button_NewDataType, "Button_NewDataType");
            this.Button_NewDataType.Name = "Button_NewDataType";
            this.Button_NewDataType.Click += new System.EventHandler(this.Button_Datatype_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            resources.ApplyResources(this.toolStripSeparator2, "toolStripSeparator2");
            // 
            // Button_LoadCSharpCode
            // 
            this.Button_LoadCSharpCode.Image = global::PAT.GUI.Properties.Resources.open_spec;
            resources.ApplyResources(this.Button_LoadCSharpCode, "Button_LoadCSharpCode");
            this.Button_LoadCSharpCode.Name = "Button_LoadCSharpCode";
            this.Button_LoadCSharpCode.Click += new System.EventHandler(this.Button_Load_Click);
            // 
            // Button_SaveCode
            // 
            this.Button_SaveCode.Image = global::PAT.GUI.Properties.Resources.save;
            resources.ApplyResources(this.Button_SaveCode, "Button_SaveCode");
            this.Button_SaveCode.Name = "Button_SaveCode";
            this.Button_SaveCode.Click += new System.EventHandler(this.Button_Save_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            resources.ApplyResources(this.toolStripSeparator1, "toolStripSeparator1");
            // 
            // Button_BuildMode
            // 
            this.Button_BuildMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.Button_BuildMode.Items.AddRange(new object[] {
            resources.GetString("Button_BuildMode.Items"),
            resources.GetString("Button_BuildMode.Items1"),
            resources.GetString("Button_BuildMode.Items2")});
            this.Button_BuildMode.Name = "Button_BuildMode";
            resources.ApplyResources(this.Button_BuildMode, "Button_BuildMode");
            // 
            // Button_Build
            // 
            resources.ApplyResources(this.Button_Build, "Button_Build");
            this.Button_Build.Name = "Button_Build";
            this.Button_Build.Click += new System.EventHandler(this.Button_build_Click);
            // 
            // CSharpLibraryForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.toolStripContainer1);
            this.Name = "CSharpLibraryForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.CSharpLibraryForm_FormClosing);
            this.toolStripContainer1.BottomToolStripPanel.ResumeLayout(false);
            this.toolStripContainer1.BottomToolStripPanel.PerformLayout();
            this.toolStripContainer1.ContentPanel.ResumeLayout(false);
            this.toolStripContainer1.TopToolStripPanel.ResumeLayout(false);
            this.toolStripContainer1.TopToolStripPanel.PerformLayout();
            this.toolStripContainer1.ResumeLayout(false);
            this.toolStripContainer1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ToolStripContainer toolStripContainer1;
        private System.Windows.Forms.RichTextBox TextBox_Code;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton Button_NewLibrary;
        private System.Windows.Forms.ToolStripButton Button_NewDataType;
        private System.Windows.Forms.ToolStripButton Button_LoadCSharpCode;
        private System.Windows.Forms.ToolStripButton Button_SaveCode;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton Button_Build;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel StatusLabel_FileName;
        private System.Windows.Forms.ToolStripComboBox Button_BuildMode;
        private System.Windows.Forms.ToolStripStatusLabel StatusLabel_Status;

    }
}