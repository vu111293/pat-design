namespace PAT.GUI.TADrawing
{
    partial class TATransitionEditingForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TATransitionEditingForm));
            this.Label_Source = new System.Windows.Forms.Label();
            this.Button_OK = new System.Windows.Forms.Button();
            this.Button_Cancel = new System.Windows.Forms.Button();
            this.Label_Target = new System.Windows.Forms.Label();
            this.TextBox_Guard = new System.Windows.Forms.TextBox();
            this.Label_Event = new System.Windows.Forms.Label();
            this.TextBox_Event = new System.Windows.Forms.TextBox();
            this.Label_Guard = new System.Windows.Forms.Label();
            this.Label_Program = new System.Windows.Forms.Label();
            this.TextBox_Program = new System.Windows.Forms.TextBox();
            this.Label_ClockGuard = new System.Windows.Forms.Label();
            this.TextBox_ClockGuard = new System.Windows.Forms.TextBox();
            this.TextBox_ResetClocks = new System.Windows.Forms.TextBox();
            this.Label_ResetClocks = new System.Windows.Forms.Label();
            this.TextBox_Select = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.comboBoxDest = new System.Windows.Forms.ComboBox();
            this.comboBoxSource = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // Label_Source
            // 
            resources.ApplyResources(this.Label_Source, "Label_Source");
            this.Label_Source.Name = "Label_Source";
            // 
            // Button_OK
            // 
            resources.ApplyResources(this.Button_OK, "Button_OK");
            this.Button_OK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Button_OK.Name = "Button_OK";
            this.Button_OK.UseVisualStyleBackColor = true;
            this.Button_OK.Click += new System.EventHandler(this.Button_OK_Click);
            // 
            // Button_Cancel
            // 
            resources.ApplyResources(this.Button_Cancel, "Button_Cancel");
            this.Button_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Button_Cancel.Name = "Button_Cancel";
            this.Button_Cancel.UseVisualStyleBackColor = true;
            // 
            // Label_Target
            // 
            resources.ApplyResources(this.Label_Target, "Label_Target");
            this.Label_Target.Name = "Label_Target";
            // 
            // TextBox_Guard
            // 
            resources.ApplyResources(this.TextBox_Guard, "TextBox_Guard");
            this.TextBox_Guard.Name = "TextBox_Guard";
            // 
            // Label_Event
            // 
            resources.ApplyResources(this.Label_Event, "Label_Event");
            this.Label_Event.Name = "Label_Event";
            // 
            // TextBox_Event
            // 
            resources.ApplyResources(this.TextBox_Event, "TextBox_Event");
            this.TextBox_Event.Name = "TextBox_Event";
            // 
            // Label_Guard
            // 
            resources.ApplyResources(this.Label_Guard, "Label_Guard");
            this.Label_Guard.Name = "Label_Guard";
            // 
            // Label_Program
            // 
            resources.ApplyResources(this.Label_Program, "Label_Program");
            this.Label_Program.Name = "Label_Program";
            // 
            // TextBox_Program
            // 
            resources.ApplyResources(this.TextBox_Program, "TextBox_Program");
            this.TextBox_Program.Name = "TextBox_Program";
            // 
            // Label_ClockGuard
            // 
            resources.ApplyResources(this.Label_ClockGuard, "Label_ClockGuard");
            this.Label_ClockGuard.Name = "Label_ClockGuard";
            // 
            // TextBox_ClockGuard
            // 
            resources.ApplyResources(this.TextBox_ClockGuard, "TextBox_ClockGuard");
            this.TextBox_ClockGuard.Name = "TextBox_ClockGuard";
            // 
            // TextBox_ResetClocks
            // 
            resources.ApplyResources(this.TextBox_ResetClocks, "TextBox_ResetClocks");
            this.TextBox_ResetClocks.Name = "TextBox_ResetClocks";
            // 
            // Label_ResetClocks
            // 
            resources.ApplyResources(this.Label_ResetClocks, "Label_ResetClocks");
            this.Label_ResetClocks.Name = "Label_ResetClocks";
            // 
            // TextBox_Select
            // 
            resources.ApplyResources(this.TextBox_Select, "TextBox_Select");
            this.TextBox_Select.Name = "TextBox_Select";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // comboBoxDest
            // 
            resources.ApplyResources(this.comboBoxDest, "comboBoxDest");
            this.comboBoxDest.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxDest.FormattingEnabled = true;
            this.comboBoxDest.Name = "comboBoxDest";
            // 
            // comboBoxSource
            // 
            resources.ApplyResources(this.comboBoxSource, "comboBoxSource");
            this.comboBoxSource.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxSource.FormattingEnabled = true;
            this.comboBoxSource.Name = "comboBoxSource";
            // 
            // TATransitionEditingForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.Button_Cancel;
            this.Controls.Add(this.comboBoxDest);
            this.Controls.Add(this.comboBoxSource);
            this.Controls.Add(this.TextBox_Select);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.TextBox_ClockGuard);
            this.Controls.Add(this.TextBox_ResetClocks);
            this.Controls.Add(this.Label_ResetClocks);
            this.Controls.Add(this.Label_Program);
            this.Controls.Add(this.TextBox_Program);
            this.Controls.Add(this.TextBox_Event);
            this.Controls.Add(this.Label_Guard);
            this.Controls.Add(this.TextBox_Guard);
            this.Controls.Add(this.Label_Event);
            this.Controls.Add(this.Label_Target);
            this.Controls.Add(this.Button_Cancel);
            this.Controls.Add(this.Button_OK);
            this.Controls.Add(this.Label_Source);
            this.Controls.Add(this.Label_ClockGuard);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "TATransitionEditingForm";
            this.ShowIcon = false;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label Label_Source;
        private System.Windows.Forms.Button Button_OK;
        private System.Windows.Forms.Button Button_Cancel;
        private System.Windows.Forms.Label Label_Target;
        private System.Windows.Forms.TextBox TextBox_Guard;
        private System.Windows.Forms.Label Label_Event;
        private System.Windows.Forms.TextBox TextBox_Event;
        private System.Windows.Forms.Label Label_Guard;
        private System.Windows.Forms.Label Label_Program;
        private System.Windows.Forms.TextBox TextBox_Program;
        private System.Windows.Forms.Label Label_ClockGuard;
        private System.Windows.Forms.TextBox TextBox_ClockGuard;
        private System.Windows.Forms.TextBox TextBox_ResetClocks;
        private System.Windows.Forms.Label Label_ResetClocks;
        private System.Windows.Forms.TextBox TextBox_Select;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox comboBoxDest;
        private System.Windows.Forms.ComboBox comboBoxSource;
    }
}