using PAT.GUI.ERADrawing;

namespace PAT.GUI.ModuleGUI.ILADrawing
{
    partial class ILAStateEditingForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ERAStateEditingForm));
            this.label1 = new System.Windows.Forms.Label();
            this.TextBox_Name = new System.Windows.Forms.TextBox();
            this.Button_OK = new System.Windows.Forms.Button();
            this.Button_Cancel = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.TextBox_Invariant = new System.Windows.Forms.TextBox();
            this.CheckBox_IsAcceptance = new System.Windows.Forms.CheckBox();
            this.CheckBox_Commited = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // TextBox_Name
            // 
            resources.ApplyResources(this.TextBox_Name, "TextBox_Name");
            this.TextBox_Name.Name = "TextBox_Name";
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
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // TextBox_Invariant
            // 
            resources.ApplyResources(this.TextBox_Invariant, "TextBox_Invariant");
            this.TextBox_Invariant.Name = "TextBox_Invariant";
            // 
            // CheckBox_IsAcceptance
            // 
            resources.ApplyResources(this.CheckBox_IsAcceptance, "CheckBox_IsAcceptance");
            this.CheckBox_IsAcceptance.Name = "CheckBox_IsAcceptance";
            this.CheckBox_IsAcceptance.UseVisualStyleBackColor = true;
            this.CheckBox_IsAcceptance.CheckStateChanged += new System.EventHandler(this.CheckBox_IsUrgent_CheckStateChanged);
            // 
            // CheckBox_Commited
            // 
            resources.ApplyResources(this.CheckBox_Commited, "CheckBox_Commited");
            this.CheckBox_Commited.Name = "CheckBox_Commited";
            this.CheckBox_Commited.UseVisualStyleBackColor = true;
            this.CheckBox_Commited.CheckStateChanged += new System.EventHandler(this.CheckBox_Commited_CheckedStateChanged);
            // 
            // ERAStateEditingForm
            // 
            this.AcceptButton = this.Button_OK;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.Button_Cancel;
            this.Controls.Add(this.CheckBox_Commited);
            this.Controls.Add(this.CheckBox_IsAcceptance);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.TextBox_Invariant);
            this.Controls.Add(this.Button_Cancel);
            this.Controls.Add(this.Button_OK);
            this.Controls.Add(this.TextBox_Name);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ERAStateEditingForm";
            this.ShowIcon = false;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox TextBox_Name;
        private System.Windows.Forms.Button Button_OK;
        private System.Windows.Forms.Button Button_Cancel;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox TextBox_Invariant;
        private System.Windows.Forms.CheckBox CheckBox_IsAcceptance;
        private System.Windows.Forms.CheckBox CheckBox_Commited;
    }
}