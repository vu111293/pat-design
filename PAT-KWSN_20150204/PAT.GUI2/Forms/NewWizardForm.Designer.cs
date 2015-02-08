namespace PAT.GUI.Forms
{
    partial class NewWizardForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NewWizardForm));
            this.TreeView_Language = new System.Windows.Forms.TreeView();
            this.Label_ModelingLanguage = new System.Windows.Forms.Label();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.ListView_Templates = new System.Windows.Forms.ListView();
            this.Button_OK = new System.Windows.Forms.Button();
            this.Button_Cancel = new System.Windows.Forms.Button();
            this.Label_Template = new System.Windows.Forms.Label();
            this.TextBox_Explanation = new System.Windows.Forms.TextBox();
            this.CheckBox_DefaultLanguage = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // TreeView_Language
            // 
            resources.ApplyResources(this.TreeView_Language, "TreeView_Language");
            this.TreeView_Language.HideSelection = false;
            this.TreeView_Language.Name = "TreeView_Language";
            this.TreeView_Language.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.TreeView_Language_AfterSelect);
            // 
            // Label_ModelingLanguage
            // 
            resources.ApplyResources(this.Label_ModelingLanguage, "Label_ModelingLanguage");
            this.Label_ModelingLanguage.Name = "Label_ModelingLanguage";
            // 
            // splitContainer1
            // 
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            resources.ApplyResources(this.splitContainer1, "splitContainer1");
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.TreeView_Language);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.ListView_Templates);
            // 
            // ListView_Templates
            // 
            resources.ApplyResources(this.ListView_Templates, "ListView_Templates");
            this.ListView_Templates.HideSelection = false;
            this.ListView_Templates.MultiSelect = false;
            this.ListView_Templates.Name = "ListView_Templates";
            this.ListView_Templates.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.ListView_Templates.UseCompatibleStateImageBehavior = false;
            this.ListView_Templates.View = System.Windows.Forms.View.SmallIcon;
            this.ListView_Templates.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.ListView_Templates_ItemSelectionChanged);
            // 
            // Button_OK
            // 
            this.Button_OK.DialogResult = System.Windows.Forms.DialogResult.OK;
            resources.ApplyResources(this.Button_OK, "Button_OK");
            this.Button_OK.Name = "Button_OK";
            this.Button_OK.UseVisualStyleBackColor = true;
            this.Button_OK.Click += new System.EventHandler(this.Button_OK_Click);
            // 
            // Button_Cancel
            // 
            this.Button_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            resources.ApplyResources(this.Button_Cancel, "Button_Cancel");
            this.Button_Cancel.Name = "Button_Cancel";
            this.Button_Cancel.UseVisualStyleBackColor = true;
            // 
            // Label_Template
            // 
            resources.ApplyResources(this.Label_Template, "Label_Template");
            this.Label_Template.Name = "Label_Template";
            // 
            // TextBox_Explanation
            // 
            resources.ApplyResources(this.TextBox_Explanation, "TextBox_Explanation");
            this.TextBox_Explanation.Name = "TextBox_Explanation";
            this.TextBox_Explanation.ReadOnly = true;
            this.TextBox_Explanation.TabStop = false;
            // 
            // CheckBox_DefaultLanguage
            // 
            resources.ApplyResources(this.CheckBox_DefaultLanguage, "CheckBox_DefaultLanguage");
            this.CheckBox_DefaultLanguage.Checked = true;
            this.CheckBox_DefaultLanguage.CheckState = System.Windows.Forms.CheckState.Checked;
            this.CheckBox_DefaultLanguage.Name = "CheckBox_DefaultLanguage";
            this.CheckBox_DefaultLanguage.UseVisualStyleBackColor = true;
            // 
            // NewWizardForm
            // 
            this.AcceptButton = this.Button_OK;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.CheckBox_DefaultLanguage);
            this.Controls.Add(this.TextBox_Explanation);
            this.Controls.Add(this.Label_Template);
            this.Controls.Add(this.Button_Cancel);
            this.Controls.Add(this.Button_OK);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.Label_ModelingLanguage);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "NewWizardForm";
            this.ShowIcon = false;
            this.TopMost = true;
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TreeView TreeView_Language;
        private System.Windows.Forms.Label Label_ModelingLanguage;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ListView ListView_Templates;
        private System.Windows.Forms.Button Button_OK;
        private System.Windows.Forms.Button Button_Cancel;
        private System.Windows.Forms.Label Label_Template;
        private System.Windows.Forms.TextBox TextBox_Explanation;
        private System.Windows.Forms.CheckBox CheckBox_DefaultLanguage;
    }
}