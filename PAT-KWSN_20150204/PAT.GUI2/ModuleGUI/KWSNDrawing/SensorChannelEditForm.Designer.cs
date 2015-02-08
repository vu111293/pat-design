namespace PAT.GUI.ModuleGUI.KWSNDrawing
{
    partial class SensorChannelEditForm
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
            this.label1 = new System.Windows.Forms.Label();
            this.cmbSource = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.cmbDest = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.cmbChannelType = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.cmbChannelMode = new System.Windows.Forms.ComboBox();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(60, 20);
            this.label1.TabIndex = 0;
            this.label1.Text = "Source";
            // 
            // cmbSource
            // 
            this.cmbSource.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSource.Enabled = false;
            this.cmbSource.FormattingEnabled = true;
            this.cmbSource.Location = new System.Drawing.Point(164, 10);
            this.cmbSource.Name = "cmbSource";
            this.cmbSource.Size = new System.Drawing.Size(435, 28);
            this.cmbSource.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 52);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(90, 20);
            this.label2.TabIndex = 2;
            this.label2.Text = "Destination";
            // 
            // cmbDest
            // 
            this.cmbDest.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbDest.Enabled = false;
            this.cmbDest.FormattingEnabled = true;
            this.cmbDest.Location = new System.Drawing.Point(164, 49);
            this.cmbDest.Name = "cmbDest";
            this.cmbDest.Size = new System.Drawing.Size(435, 28);
            this.cmbDest.TabIndex = 3;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(13, 91);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(106, 20);
            this.label3.TabIndex = 4;
            this.label3.Text = "Channel Type";
            // 
            // cmbChannelType
            // 
            this.cmbChannelType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbChannelType.FormattingEnabled = true;
            this.cmbChannelType.Location = new System.Drawing.Point(164, 88);
            this.cmbChannelType.Name = "cmbChannelType";
            this.cmbChannelType.Size = new System.Drawing.Size(435, 28);
            this.cmbChannelType.TabIndex = 5;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(13, 130);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(112, 20);
            this.label4.TabIndex = 6;
            this.label4.Text = "Channel Mode";
            // 
            // cmbChannelMode
            // 
            this.cmbChannelMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbChannelMode.FormattingEnabled = true;
            this.cmbChannelMode.Location = new System.Drawing.Point(164, 127);
            this.cmbChannelMode.Name = "cmbChannelMode";
            this.cmbChannelMode.Size = new System.Drawing.Size(435, 28);
            this.cmbChannelMode.TabIndex = 7;
            // 
            // btnOK
            // 
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(107, 466);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(111, 43);
            this.btnOK.TabIndex = 8;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(365, 466);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(116, 43);
            this.btnCancel.TabIndex = 9;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // SensorChannelEditForm
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(611, 534);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.cmbChannelMode);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.cmbChannelType);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.cmbDest);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.cmbSource);
            this.Controls.Add(this.label1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SensorChannelEditForm";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Channel Editing Form";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cmbSource;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cmbDest;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cmbChannelType;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox cmbChannelMode;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
    }
}