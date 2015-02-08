namespace PAT.GUI.Forms
{
    partial class OptionForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OptionForm));
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.TabPage_General = new System.Windows.Forms.TabPage();
            this.CheckBox_EmailSSL = new System.Windows.Forms.CheckBox();
            this.ComboBox_DefaultModelingLanguage = new System.Windows.Forms.ComboBox();
            this.Label_DefaultModelingLanguage = new System.Windows.Forms.Label();
            this.GroupBox_Verification = new System.Windows.Forms.GroupBox();
            this.CheckBox_PerformDeterminization = new System.Windows.Forms.CheckBox();
            this.Label_SearchInitialSize = new System.Windows.Forms.Label();
            this.NUD_MC_Initial = new System.Windows.Forms.NumericUpDown();
            this.GroupBox_Simulation = new System.Windows.Forms.GroupBox();
            this.NUD_SimulationBound = new System.Windows.Forms.NumericUpDown();
            this.Label_SearchUpperBound = new System.Windows.Forms.Label();
            this.CheckBox_AutoSave = new System.Windows.Forms.CheckBox();
            this.CheckBox_CheckUpdate = new System.Windows.Forms.CheckBox();
            this.tabBDD = new System.Windows.Forms.TabPage();
            this.nudMessageLength = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.nudMessageUpper = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.nudMessageLower = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.NUD_VarUpperBound = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.NUD_VarLowerBound = new System.Windows.Forms.NumericUpDown();
            this.Label_VarLowerBound = new System.Windows.Forms.Label();
            this.textBoxEventParaMin = new System.Windows.Forms.TextBox();
            this.labelEventParaMin = new System.Windows.Forms.Label();
            this.textBoxEventParaMax = new System.Windows.Forms.TextBox();
            this.labelEventParaMax = new System.Windows.Forms.Label();
            this.TabPage_CSP = new System.Windows.Forms.TabPage();
            this.Label_CutNumberUpperBound = new System.Windows.Forms.Label();
            this.NUD_CutNumberBound = new System.Windows.Forms.NumericUpDown();
            this.Label_InitialCutNumber = new System.Windows.Forms.Label();
            this.NUD_CutNumber = new System.Windows.Forms.NumericUpDown();
            this.CheckBox_LinkCSP = new System.Windows.Forms.CheckBox();
            this.Button_OK = new System.Windows.Forms.Button();
            this.Button_Cancel = new System.Windows.Forms.Button();
            this.Button_DefaultSetting = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.toolTipEventParameterRange = new System.Windows.Forms.ToolTip(this.components);
            this.toolTipVariableRange = new System.Windows.Forms.ToolTip(this.components);
            this.tabControl1.SuspendLayout();
            this.TabPage_General.SuspendLayout();
            this.GroupBox_Verification.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NUD_MC_Initial)).BeginInit();
            this.GroupBox_Simulation.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NUD_SimulationBound)).BeginInit();
            this.tabBDD.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudMessageLength)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudMessageUpper)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudMessageLower)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUD_VarUpperBound)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUD_VarLowerBound)).BeginInit();
            this.TabPage_CSP.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NUD_CutNumberBound)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUD_CutNumber)).BeginInit();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.TabPage_General);
            this.tabControl1.Controls.Add(this.tabBDD);
            this.tabControl1.Controls.Add(this.TabPage_CSP);
            resources.ApplyResources(this.tabControl1, "tabControl1");
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            // 
            // TabPage_General
            // 
            this.TabPage_General.Controls.Add(this.CheckBox_EmailSSL);
            this.TabPage_General.Controls.Add(this.ComboBox_DefaultModelingLanguage);
            this.TabPage_General.Controls.Add(this.Label_DefaultModelingLanguage);
            this.TabPage_General.Controls.Add(this.GroupBox_Verification);
            this.TabPage_General.Controls.Add(this.GroupBox_Simulation);
            this.TabPage_General.Controls.Add(this.CheckBox_AutoSave);
            this.TabPage_General.Controls.Add(this.CheckBox_CheckUpdate);
            resources.ApplyResources(this.TabPage_General, "TabPage_General");
            this.TabPage_General.Name = "TabPage_General";
            this.TabPage_General.UseVisualStyleBackColor = true;
            // 
            // CheckBox_EmailSSL
            // 
            resources.ApplyResources(this.CheckBox_EmailSSL, "CheckBox_EmailSSL");
            this.CheckBox_EmailSSL.Name = "CheckBox_EmailSSL";
            this.toolTip1.SetToolTip(this.CheckBox_EmailSSL, resources.GetString("CheckBox_EmailSSL.ToolTip"));
            this.CheckBox_EmailSSL.UseVisualStyleBackColor = true;
            // 
            // ComboBox_DefaultModelingLanguage
            // 
            this.ComboBox_DefaultModelingLanguage.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ComboBox_DefaultModelingLanguage.FormattingEnabled = true;
            resources.ApplyResources(this.ComboBox_DefaultModelingLanguage, "ComboBox_DefaultModelingLanguage");
            this.ComboBox_DefaultModelingLanguage.Name = "ComboBox_DefaultModelingLanguage";
            // 
            // Label_DefaultModelingLanguage
            // 
            resources.ApplyResources(this.Label_DefaultModelingLanguage, "Label_DefaultModelingLanguage");
            this.Label_DefaultModelingLanguage.Name = "Label_DefaultModelingLanguage";
            // 
            // GroupBox_Verification
            // 
            this.GroupBox_Verification.Controls.Add(this.CheckBox_PerformDeterminization);
            this.GroupBox_Verification.Controls.Add(this.Label_SearchInitialSize);
            this.GroupBox_Verification.Controls.Add(this.NUD_MC_Initial);
            resources.ApplyResources(this.GroupBox_Verification, "GroupBox_Verification");
            this.GroupBox_Verification.Name = "GroupBox_Verification";
            this.GroupBox_Verification.TabStop = false;
            // 
            // CheckBox_PerformDeterminization
            // 
            resources.ApplyResources(this.CheckBox_PerformDeterminization, "CheckBox_PerformDeterminization");
            this.CheckBox_PerformDeterminization.Name = "CheckBox_PerformDeterminization";
            this.CheckBox_PerformDeterminization.UseVisualStyleBackColor = true;
            // 
            // Label_SearchInitialSize
            // 
            resources.ApplyResources(this.Label_SearchInitialSize, "Label_SearchInitialSize");
            this.Label_SearchInitialSize.Name = "Label_SearchInitialSize";
            // 
            // NUD_MC_Initial
            // 
            this.NUD_MC_Initial.Increment = new decimal(new int[] {
            100,
            0,
            0,
            0});
            resources.ApplyResources(this.NUD_MC_Initial, "NUD_MC_Initial");
            this.NUD_MC_Initial.Maximum = new decimal(new int[] {
            10000000,
            0,
            0,
            0});
            this.NUD_MC_Initial.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.NUD_MC_Initial.Name = "NUD_MC_Initial";
            this.toolTip1.SetToolTip(this.NUD_MC_Initial, resources.GetString("NUD_MC_Initial.ToolTip"));
            this.NUD_MC_Initial.Value = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            // 
            // GroupBox_Simulation
            // 
            this.GroupBox_Simulation.Controls.Add(this.NUD_SimulationBound);
            this.GroupBox_Simulation.Controls.Add(this.Label_SearchUpperBound);
            resources.ApplyResources(this.GroupBox_Simulation, "GroupBox_Simulation");
            this.GroupBox_Simulation.Name = "GroupBox_Simulation";
            this.GroupBox_Simulation.TabStop = false;
            // 
            // NUD_SimulationBound
            // 
            this.NUD_SimulationBound.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            resources.ApplyResources(this.NUD_SimulationBound, "NUD_SimulationBound");
            this.NUD_SimulationBound.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.NUD_SimulationBound.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.NUD_SimulationBound.Name = "NUD_SimulationBound";
            this.toolTip1.SetToolTip(this.NUD_SimulationBound, resources.GetString("NUD_SimulationBound.ToolTip"));
            this.NUD_SimulationBound.Value = new decimal(new int[] {
            300,
            0,
            0,
            0});
            // 
            // Label_SearchUpperBound
            // 
            resources.ApplyResources(this.Label_SearchUpperBound, "Label_SearchUpperBound");
            this.Label_SearchUpperBound.Name = "Label_SearchUpperBound";
            // 
            // CheckBox_AutoSave
            // 
            resources.ApplyResources(this.CheckBox_AutoSave, "CheckBox_AutoSave");
            this.CheckBox_AutoSave.Name = "CheckBox_AutoSave";
            this.CheckBox_AutoSave.UseVisualStyleBackColor = true;
            // 
            // CheckBox_CheckUpdate
            // 
            resources.ApplyResources(this.CheckBox_CheckUpdate, "CheckBox_CheckUpdate");
            this.CheckBox_CheckUpdate.Name = "CheckBox_CheckUpdate";
            this.CheckBox_CheckUpdate.UseVisualStyleBackColor = true;
            // 
            // tabBDD
            // 
            this.tabBDD.Controls.Add(this.nudMessageLength);
            this.tabBDD.Controls.Add(this.label4);
            this.tabBDD.Controls.Add(this.nudMessageUpper);
            this.tabBDD.Controls.Add(this.label2);
            this.tabBDD.Controls.Add(this.nudMessageLower);
            this.tabBDD.Controls.Add(this.label3);
            this.tabBDD.Controls.Add(this.NUD_VarUpperBound);
            this.tabBDD.Controls.Add(this.label1);
            this.tabBDD.Controls.Add(this.NUD_VarLowerBound);
            this.tabBDD.Controls.Add(this.Label_VarLowerBound);
            this.tabBDD.Controls.Add(this.textBoxEventParaMin);
            this.tabBDD.Controls.Add(this.labelEventParaMin);
            this.tabBDD.Controls.Add(this.textBoxEventParaMax);
            this.tabBDD.Controls.Add(this.labelEventParaMax);
            resources.ApplyResources(this.tabBDD, "tabBDD");
            this.tabBDD.Name = "tabBDD";
            this.tabBDD.UseVisualStyleBackColor = true;
            // 
            // nudMessageLength
            // 
            resources.ApplyResources(this.nudMessageLength, "nudMessageLength");
            this.nudMessageLength.Maximum = new decimal(new int[] {
            1000000000,
            0,
            0,
            0});
            this.nudMessageLength.Name = "nudMessageLength";
            this.toolTipVariableRange.SetToolTip(this.nudMessageLength, resources.GetString("nudMessageLength.ToolTip"));
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // nudMessageUpper
            // 
            resources.ApplyResources(this.nudMessageUpper, "nudMessageUpper");
            this.nudMessageUpper.Maximum = new decimal(new int[] {
            32767,
            0,
            0,
            0});
            this.nudMessageUpper.Minimum = new decimal(new int[] {
            32768,
            0,
            0,
            -2147483648});
            this.nudMessageUpper.Name = "nudMessageUpper";
            this.toolTipVariableRange.SetToolTip(this.nudMessageUpper, resources.GetString("nudMessageUpper.ToolTip"));
            this.nudMessageUpper.Value = new decimal(new int[] {
            32,
            0,
            0,
            0});
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // nudMessageLower
            // 
            resources.ApplyResources(this.nudMessageLower, "nudMessageLower");
            this.nudMessageLower.Maximum = new decimal(new int[] {
            32767,
            0,
            0,
            0});
            this.nudMessageLower.Minimum = new decimal(new int[] {
            32768,
            0,
            0,
            -2147483648});
            this.nudMessageLower.Name = "nudMessageLower";
            this.toolTipVariableRange.SetToolTip(this.nudMessageLower, resources.GetString("nudMessageLower.ToolTip"));
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // NUD_VarUpperBound
            // 
            resources.ApplyResources(this.NUD_VarUpperBound, "NUD_VarUpperBound");
            this.NUD_VarUpperBound.Maximum = new decimal(new int[] {
            32767,
            0,
            0,
            0});
            this.NUD_VarUpperBound.Minimum = new decimal(new int[] {
            32768,
            0,
            0,
            -2147483648});
            this.NUD_VarUpperBound.Name = "NUD_VarUpperBound";
            this.toolTipVariableRange.SetToolTip(this.NUD_VarUpperBound, resources.GetString("NUD_VarUpperBound.ToolTip"));
            this.NUD_VarUpperBound.Value = new decimal(new int[] {
            32,
            0,
            0,
            0});
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // NUD_VarLowerBound
            // 
            resources.ApplyResources(this.NUD_VarLowerBound, "NUD_VarLowerBound");
            this.NUD_VarLowerBound.Maximum = new decimal(new int[] {
            32767,
            0,
            0,
            0});
            this.NUD_VarLowerBound.Minimum = new decimal(new int[] {
            32768,
            0,
            0,
            -2147483648});
            this.NUD_VarLowerBound.Name = "NUD_VarLowerBound";
            this.toolTipVariableRange.SetToolTip(this.NUD_VarLowerBound, resources.GetString("NUD_VarLowerBound.ToolTip"));
            // 
            // Label_VarLowerBound
            // 
            resources.ApplyResources(this.Label_VarLowerBound, "Label_VarLowerBound");
            this.Label_VarLowerBound.Name = "Label_VarLowerBound";
            // 
            // textBoxEventParaMin
            // 
            resources.ApplyResources(this.textBoxEventParaMin, "textBoxEventParaMin");
            this.textBoxEventParaMin.Name = "textBoxEventParaMin";
            this.toolTipEventParameterRange.SetToolTip(this.textBoxEventParaMin, resources.GetString("textBoxEventParaMin.ToolTip"));
            // 
            // labelEventParaMin
            // 
            resources.ApplyResources(this.labelEventParaMin, "labelEventParaMin");
            this.labelEventParaMin.Name = "labelEventParaMin";
            // 
            // textBoxEventParaMax
            // 
            resources.ApplyResources(this.textBoxEventParaMax, "textBoxEventParaMax");
            this.textBoxEventParaMax.Name = "textBoxEventParaMax";
            this.toolTipEventParameterRange.SetToolTip(this.textBoxEventParaMax, resources.GetString("textBoxEventParaMax.ToolTip"));
            // 
            // labelEventParaMax
            // 
            resources.ApplyResources(this.labelEventParaMax, "labelEventParaMax");
            this.labelEventParaMax.Name = "labelEventParaMax";
            // 
            // TabPage_CSP
            // 
            this.TabPage_CSP.Controls.Add(this.Label_CutNumberUpperBound);
            this.TabPage_CSP.Controls.Add(this.NUD_CutNumberBound);
            this.TabPage_CSP.Controls.Add(this.Label_InitialCutNumber);
            this.TabPage_CSP.Controls.Add(this.NUD_CutNumber);
            this.TabPage_CSP.Controls.Add(this.CheckBox_LinkCSP);
            resources.ApplyResources(this.TabPage_CSP, "TabPage_CSP");
            this.TabPage_CSP.Name = "TabPage_CSP";
            this.TabPage_CSP.UseVisualStyleBackColor = true;
            // 
            // Label_CutNumberUpperBound
            // 
            resources.ApplyResources(this.Label_CutNumberUpperBound, "Label_CutNumberUpperBound");
            this.Label_CutNumberUpperBound.Name = "Label_CutNumberUpperBound";
            // 
            // NUD_CutNumberBound
            // 
            resources.ApplyResources(this.NUD_CutNumberBound, "NUD_CutNumberBound");
            this.NUD_CutNumberBound.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.NUD_CutNumberBound.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.NUD_CutNumberBound.Name = "NUD_CutNumberBound";
            this.NUD_CutNumberBound.Value = new decimal(new int[] {
            50,
            0,
            0,
            0});
            // 
            // Label_InitialCutNumber
            // 
            resources.ApplyResources(this.Label_InitialCutNumber, "Label_InitialCutNumber");
            this.Label_InitialCutNumber.Name = "Label_InitialCutNumber";
            // 
            // NUD_CutNumber
            // 
            resources.ApplyResources(this.NUD_CutNumber, "NUD_CutNumber");
            this.NUD_CutNumber.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.NUD_CutNumber.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.NUD_CutNumber.Name = "NUD_CutNumber";
            this.NUD_CutNumber.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            // 
            // CheckBox_LinkCSP
            // 
            resources.ApplyResources(this.CheckBox_LinkCSP, "CheckBox_LinkCSP");
            this.CheckBox_LinkCSP.Name = "CheckBox_LinkCSP";
            this.CheckBox_LinkCSP.UseVisualStyleBackColor = true;
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
            // Button_DefaultSetting
            // 
            resources.ApplyResources(this.Button_DefaultSetting, "Button_DefaultSetting");
            this.Button_DefaultSetting.Name = "Button_DefaultSetting";
            this.Button_DefaultSetting.UseVisualStyleBackColor = true;
            this.Button_DefaultSetting.Click += new System.EventHandler(this.Button_Default_Click);
            // 
            // toolTipEventParameterRange
            // 
            this.toolTipEventParameterRange.ShowAlways = true;
            this.toolTipEventParameterRange.ToolTipIcon = System.Windows.Forms.ToolTipIcon.Warning;
            this.toolTipEventParameterRange.ToolTipTitle = "Event Parameter Range";
            // 
            // OptionForm
            // 
            this.AcceptButton = this.Button_OK;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.Button_Cancel;
            this.Controls.Add(this.Button_DefaultSetting);
            this.Controls.Add(this.Button_Cancel);
            this.Controls.Add(this.Button_OK);
            this.Controls.Add(this.tabControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "OptionForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.tabControl1.ResumeLayout(false);
            this.TabPage_General.ResumeLayout(false);
            this.TabPage_General.PerformLayout();
            this.GroupBox_Verification.ResumeLayout(false);
            this.GroupBox_Verification.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NUD_MC_Initial)).EndInit();
            this.GroupBox_Simulation.ResumeLayout(false);
            this.GroupBox_Simulation.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NUD_SimulationBound)).EndInit();
            this.tabBDD.ResumeLayout(false);
            this.tabBDD.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudMessageLength)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudMessageUpper)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudMessageLower)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUD_VarUpperBound)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUD_VarLowerBound)).EndInit();
            this.TabPage_CSP.ResumeLayout(false);
            this.TabPage_CSP.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NUD_CutNumberBound)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUD_CutNumber)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage TabPage_General;
        private System.Windows.Forms.TabPage TabPage_CSP;
        private System.Windows.Forms.Button Button_OK;
        private System.Windows.Forms.Button Button_Cancel;
        private System.Windows.Forms.Button Button_DefaultSetting;
        private System.Windows.Forms.CheckBox CheckBox_CheckUpdate;
        private System.Windows.Forms.CheckBox CheckBox_AutoSave;
        private System.Windows.Forms.GroupBox GroupBox_Simulation;
        private System.Windows.Forms.GroupBox GroupBox_Verification;
        private System.Windows.Forms.Label Label_SearchInitialSize;
        private System.Windows.Forms.NumericUpDown NUD_MC_Initial;
        private System.Windows.Forms.NumericUpDown NUD_SimulationBound;
        private System.Windows.Forms.Label Label_SearchUpperBound;
        private System.Windows.Forms.CheckBox CheckBox_LinkCSP;
        private System.Windows.Forms.Label Label_InitialCutNumber;
        private System.Windows.Forms.NumericUpDown NUD_CutNumber;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Label Label_CutNumberUpperBound;
        private System.Windows.Forms.NumericUpDown NUD_CutNumberBound;
        private System.Windows.Forms.ComboBox ComboBox_DefaultModelingLanguage;
        private System.Windows.Forms.Label Label_DefaultModelingLanguage;
        private System.Windows.Forms.CheckBox CheckBox_PerformDeterminization;
        private System.Windows.Forms.CheckBox CheckBox_EmailSSL;
        private System.Windows.Forms.TabPage tabBDD;
        private System.Windows.Forms.TextBox textBoxEventParaMin;
        private System.Windows.Forms.Label labelEventParaMin;
        private System.Windows.Forms.ToolTip toolTipEventParameterRange;
        private System.Windows.Forms.TextBox textBoxEventParaMax;
        private System.Windows.Forms.Label labelEventParaMax;
        private System.Windows.Forms.NumericUpDown NUD_VarLowerBound;
        private System.Windows.Forms.Label Label_VarLowerBound;
        private System.Windows.Forms.NumericUpDown NUD_VarUpperBound;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown nudMessageUpper;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown nudMessageLower;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ToolTip toolTipVariableRange;
        private System.Windows.Forms.NumericUpDown nudMessageLength;
        private System.Windows.Forms.Label label4;
    }
}