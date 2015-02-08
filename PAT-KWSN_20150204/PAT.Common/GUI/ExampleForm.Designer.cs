namespace PAT.Common.GUI
{
    partial class ExampleForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ExampleForm));
            this.Label_To = new System.Windows.Forms.Label();
            this.NUP_Number = new System.Windows.Forms.NumericUpDown();
            this.Button_OK = new System.Windows.Forms.Button();
            this.Button_Cancel = new System.Windows.Forms.Button();
            this.CheckBox_HasThink = new System.Windows.Forms.CheckBox();
            this.RadioButton_WL = new System.Windows.Forms.RadioButton();
            this.RadioButton_SL = new System.Windows.Forms.RadioButton();
            this.RadioButton_Partial = new System.Windows.Forms.RadioButton();
            this.RadioButton_AllFair = new System.Windows.Forms.RadioButton();
            this.Panel_FairType = new System.Windows.Forms.Panel();
            this.Panel_EventType = new System.Windows.Forms.Panel();
            this.RadioButton_FairEvent = new System.Windows.Forms.RadioButton();
            this.RadioButton_LiveEvent = new System.Windows.Forms.RadioButton();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.Label_ThirdLabel = new System.Windows.Forms.Label();
            this.CheckBox_BDDSpecific = new System.Windows.Forms.CheckBox();
            this.TextBox_Description = new System.Windows.Forms.RichTextBox();
            this.NUP_Size = new System.Windows.Forms.NumericUpDown();
            this.GroupBox_Description = new System.Windows.Forms.GroupBox();
            this.GroupBox_Options = new System.Windows.Forms.GroupBox();
            this.NUP_Third = new System.Windows.Forms.NumericUpDown();
            this.Label_CreatedBy = new System.Windows.Forms.Label();
            this.Label_CreatedAt = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            ((System.ComponentModel.ISupportInitialize)(this.NUP_Number)).BeginInit();
            this.Panel_FairType.SuspendLayout();
            this.Panel_EventType.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NUP_Size)).BeginInit();
            this.GroupBox_Description.SuspendLayout();
            this.GroupBox_Options.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NUP_Third)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // Label_To
            // 
            resources.ApplyResources(this.Label_To, "Label_To");
            this.Label_To.Name = "Label_To";
            this.toolTip1.SetToolTip(this.Label_To, resources.GetString("Label_To.ToolTip"));
            // 
            // NUP_Number
            // 
            resources.ApplyResources(this.NUP_Number, "NUP_Number");
            this.NUP_Number.Maximum = new decimal(new int[] {
            400,
            0,
            0,
            0});
            this.NUP_Number.Minimum = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.NUP_Number.Name = "NUP_Number";
            this.NUP_Number.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            // 
            // Button_OK
            // 
            this.Button_OK.DialogResult = System.Windows.Forms.DialogResult.OK;
            resources.ApplyResources(this.Button_OK, "Button_OK");
            this.Button_OK.Name = "Button_OK";
            this.Button_OK.UseVisualStyleBackColor = true;
            // 
            // Button_Cancel
            // 
            this.Button_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            resources.ApplyResources(this.Button_Cancel, "Button_Cancel");
            this.Button_Cancel.Name = "Button_Cancel";
            this.Button_Cancel.UseVisualStyleBackColor = true;
            // 
            // CheckBox_HasThink
            // 
            resources.ApplyResources(this.CheckBox_HasThink, "CheckBox_HasThink");
            this.CheckBox_HasThink.Name = "CheckBox_HasThink";
            this.toolTip1.SetToolTip(this.CheckBox_HasThink, resources.GetString("CheckBox_HasThink.ToolTip"));
            this.CheckBox_HasThink.UseVisualStyleBackColor = true;
            // 
            // RadioButton_WL
            // 
            resources.ApplyResources(this.RadioButton_WL, "RadioButton_WL");
            this.RadioButton_WL.Checked = true;
            this.RadioButton_WL.Name = "RadioButton_WL";
            this.RadioButton_WL.TabStop = true;
            this.toolTip1.SetToolTip(this.RadioButton_WL, resources.GetString("RadioButton_WL.ToolTip"));
            this.RadioButton_WL.UseVisualStyleBackColor = true;
            this.RadioButton_WL.CheckedChanged += new System.EventHandler(this.RadioButton_WL_CheckedChanged);
            // 
            // RadioButton_SL
            // 
            resources.ApplyResources(this.RadioButton_SL, "RadioButton_SL");
            this.RadioButton_SL.Name = "RadioButton_SL";
            this.RadioButton_SL.UseVisualStyleBackColor = true;
            // 
            // RadioButton_Partial
            // 
            resources.ApplyResources(this.RadioButton_Partial, "RadioButton_Partial");
            this.RadioButton_Partial.Checked = true;
            this.RadioButton_Partial.Name = "RadioButton_Partial";
            this.RadioButton_Partial.TabStop = true;
            this.toolTip1.SetToolTip(this.RadioButton_Partial, resources.GetString("RadioButton_Partial.ToolTip"));
            this.RadioButton_Partial.UseVisualStyleBackColor = true;
            // 
            // RadioButton_AllFair
            // 
            resources.ApplyResources(this.RadioButton_AllFair, "RadioButton_AllFair");
            this.RadioButton_AllFair.Name = "RadioButton_AllFair";
            this.toolTip1.SetToolTip(this.RadioButton_AllFair, resources.GetString("RadioButton_AllFair.ToolTip"));
            this.RadioButton_AllFair.UseVisualStyleBackColor = true;
            // 
            // Panel_FairType
            // 
            this.Panel_FairType.Controls.Add(this.RadioButton_AllFair);
            this.Panel_FairType.Controls.Add(this.RadioButton_Partial);
            resources.ApplyResources(this.Panel_FairType, "Panel_FairType");
            this.Panel_FairType.Name = "Panel_FairType";
            // 
            // Panel_EventType
            // 
            this.Panel_EventType.Controls.Add(this.RadioButton_FairEvent);
            this.Panel_EventType.Controls.Add(this.RadioButton_LiveEvent);
            resources.ApplyResources(this.Panel_EventType, "Panel_EventType");
            this.Panel_EventType.Name = "Panel_EventType";
            // 
            // RadioButton_FairEvent
            // 
            resources.ApplyResources(this.RadioButton_FairEvent, "RadioButton_FairEvent");
            this.RadioButton_FairEvent.Name = "RadioButton_FairEvent";
            this.toolTip1.SetToolTip(this.RadioButton_FairEvent, resources.GetString("RadioButton_FairEvent.ToolTip"));
            this.RadioButton_FairEvent.UseVisualStyleBackColor = true;
            // 
            // RadioButton_LiveEvent
            // 
            resources.ApplyResources(this.RadioButton_LiveEvent, "RadioButton_LiveEvent");
            this.RadioButton_LiveEvent.Checked = true;
            this.RadioButton_LiveEvent.Name = "RadioButton_LiveEvent";
            this.RadioButton_LiveEvent.TabStop = true;
            this.toolTip1.SetToolTip(this.RadioButton_LiveEvent, resources.GetString("RadioButton_LiveEvent.ToolTip"));
            this.RadioButton_LiveEvent.UseVisualStyleBackColor = true;
            // 
            // Label_ThirdLabel
            // 
            resources.ApplyResources(this.Label_ThirdLabel, "Label_ThirdLabel");
            this.Label_ThirdLabel.Name = "Label_ThirdLabel";
            this.toolTip1.SetToolTip(this.Label_ThirdLabel, resources.GetString("Label_ThirdLabel.ToolTip"));
            // 
            // CheckBox_BDDSpecific
            // 
            resources.ApplyResources(this.CheckBox_BDDSpecific, "CheckBox_BDDSpecific");
            this.CheckBox_BDDSpecific.Name = "CheckBox_BDDSpecific";
            this.toolTip1.SetToolTip(this.CheckBox_BDDSpecific, resources.GetString("CheckBox_BDDSpecific.ToolTip"));
            this.CheckBox_BDDSpecific.UseVisualStyleBackColor = true;
            // 
            // TextBox_Description
            // 
            this.TextBox_Description.BackColor = System.Drawing.SystemColors.Control;
            this.TextBox_Description.BorderStyle = System.Windows.Forms.BorderStyle.None;
            resources.ApplyResources(this.TextBox_Description, "TextBox_Description");
            this.TextBox_Description.Name = "TextBox_Description";
            this.TextBox_Description.ReadOnly = true;
            this.TextBox_Description.TabStop = false;
            // 
            // NUP_Size
            // 
            resources.ApplyResources(this.NUP_Size, "NUP_Size");
            this.NUP_Size.Maximum = new decimal(new int[] {
            400,
            0,
            0,
            0});
            this.NUP_Size.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.NUP_Size.Name = "NUP_Size";
            this.NUP_Size.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            // 
            // GroupBox_Description
            // 
            this.GroupBox_Description.Controls.Add(this.TextBox_Description);
            resources.ApplyResources(this.GroupBox_Description, "GroupBox_Description");
            this.GroupBox_Description.Name = "GroupBox_Description";
            this.GroupBox_Description.TabStop = false;
            // 
            // GroupBox_Options
            // 
            this.GroupBox_Options.Controls.Add(this.CheckBox_BDDSpecific);
            this.GroupBox_Options.Controls.Add(this.NUP_Third);
            this.GroupBox_Options.Controls.Add(this.Label_ThirdLabel);
            this.GroupBox_Options.Controls.Add(this.NUP_Number);
            this.GroupBox_Options.Controls.Add(this.Label_To);
            this.GroupBox_Options.Controls.Add(this.Panel_EventType);
            this.GroupBox_Options.Controls.Add(this.NUP_Size);
            this.GroupBox_Options.Controls.Add(this.RadioButton_SL);
            this.GroupBox_Options.Controls.Add(this.Panel_FairType);
            this.GroupBox_Options.Controls.Add(this.CheckBox_HasThink);
            this.GroupBox_Options.Controls.Add(this.RadioButton_WL);
            resources.ApplyResources(this.GroupBox_Options, "GroupBox_Options");
            this.GroupBox_Options.Name = "GroupBox_Options";
            this.GroupBox_Options.TabStop = false;
            // 
            // NUP_Third
            // 
            resources.ApplyResources(this.NUP_Third, "NUP_Third");
            this.NUP_Third.Maximum = new decimal(new int[] {
            400,
            0,
            0,
            0});
            this.NUP_Third.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.NUP_Third.Name = "NUP_Third";
            this.NUP_Third.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            // 
            // Label_CreatedBy
            // 
            resources.ApplyResources(this.Label_CreatedBy, "Label_CreatedBy");
            this.Label_CreatedBy.Name = "Label_CreatedBy";
            // 
            // Label_CreatedAt
            // 
            resources.ApplyResources(this.Label_CreatedAt, "Label_CreatedAt");
            this.Label_CreatedAt.Name = "Label_CreatedAt";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.Label_CreatedBy);
            this.groupBox1.Controls.Add(this.Label_CreatedAt);
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // ExampleForm
            // 
            this.AcceptButton = this.Button_OK;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.GroupBox_Options);
            this.Controls.Add(this.GroupBox_Description);
            this.Controls.Add(this.Button_Cancel);
            this.Controls.Add(this.Button_OK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ExampleForm";
            this.ShowIcon = false;
            ((System.ComponentModel.ISupportInitialize)(this.NUP_Number)).EndInit();
            this.Panel_FairType.ResumeLayout(false);
            this.Panel_FairType.PerformLayout();
            this.Panel_EventType.ResumeLayout(false);
            this.Panel_EventType.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NUP_Size)).EndInit();
            this.GroupBox_Description.ResumeLayout(false);
            this.GroupBox_Options.ResumeLayout(false);
            this.GroupBox_Options.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NUP_Third)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label Label_To;
        private System.Windows.Forms.NumericUpDown NUP_Number;
        private System.Windows.Forms.Button Button_OK;
        private System.Windows.Forms.Button Button_Cancel;
        private System.Windows.Forms.CheckBox CheckBox_HasThink;
        private System.Windows.Forms.RadioButton RadioButton_WL;
        private System.Windows.Forms.RadioButton RadioButton_SL;
        private System.Windows.Forms.RadioButton RadioButton_Partial;
        private System.Windows.Forms.RadioButton RadioButton_AllFair;
        private System.Windows.Forms.Panel Panel_FairType;
        private System.Windows.Forms.Panel Panel_EventType;
        private System.Windows.Forms.RadioButton RadioButton_FairEvent;
        private System.Windows.Forms.RadioButton RadioButton_LiveEvent;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.RichTextBox TextBox_Description;
        private System.Windows.Forms.NumericUpDown NUP_Size;
        private System.Windows.Forms.GroupBox GroupBox_Description;
        private System.Windows.Forms.GroupBox GroupBox_Options;
        private System.Windows.Forms.NumericUpDown NUP_Third;
        private System.Windows.Forms.Label Label_ThirdLabel;
        private System.Windows.Forms.Label Label_CreatedBy;
        private System.Windows.Forms.Label Label_CreatedAt;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox CheckBox_BDDSpecific;

    }
}