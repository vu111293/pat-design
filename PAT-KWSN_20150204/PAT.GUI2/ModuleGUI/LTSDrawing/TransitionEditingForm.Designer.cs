namespace PAT.GUI.LTSDrawing
{
    partial class TransitionEditingForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TransitionEditingForm));
            this.label1 = new System.Windows.Forms.Label();
            this.Button_OK = new System.Windows.Forms.Button();
            this.Button_Cancel = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.TextBox_Guard = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.TextBox_Event = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.comboBoxSource = new System.Windows.Forms.ComboBox();
            this.comboBoxDest = new System.Windows.Forms.ComboBox();
            this.TextBox_Select = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
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
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // TextBox_Guard
            // 
            resources.ApplyResources(this.TextBox_Guard, "TextBox_Guard");
            this.TextBox_Guard.Name = "TextBox_Guard";
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // TextBox_Event
            // 
            resources.ApplyResources(this.TextBox_Event, "TextBox_Event");
            this.TextBox_Event.Name = "TextBox_Event";
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.Name = "label5";
            // 
            // comboBoxSource
            // 
            resources.ApplyResources(this.comboBoxSource, "comboBoxSource");
            this.comboBoxSource.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxSource.FormattingEnabled = true;
            this.comboBoxSource.Name = "comboBoxSource";
            // 
            // comboBoxDest
            // 
            resources.ApplyResources(this.comboBoxDest, "comboBoxDest");
            this.comboBoxDest.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxDest.FormattingEnabled = true;
            this.comboBoxDest.Name = "comboBoxDest";
            // 
            // TextBox_Select
            // 
            resources.ApplyResources(this.TextBox_Select, "TextBox_Select");
            this.TextBox_Select.Name = "TextBox_Select";
            // 
            // label6
            // 
            resources.ApplyResources(this.label6, "label6");
            this.label6.Name = "label6";
            // 
            // TransitionEditingForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.Button_Cancel;
            this.Controls.Add(this.TextBox_Select);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.comboBoxDest);
            this.Controls.Add(this.comboBoxSource);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.TextBox_Event);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.TextBox_Guard);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.Button_Cancel);
            this.Controls.Add(this.Button_OK);
            this.Controls.Add(this.label1);
            this.MinimizeBox = false;
            this.Name = "TransitionEditingForm";
            this.ShowIcon = false;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button Button_OK;
        private System.Windows.Forms.Button Button_Cancel;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox TextBox_Guard;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox TextBox_Event;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        protected System.Windows.Forms.ComboBox comboBoxSource;
        protected System.Windows.Forms.ComboBox comboBoxDest;
        private System.Windows.Forms.TextBox TextBox_Select;
        private System.Windows.Forms.Label label6;
    }
}