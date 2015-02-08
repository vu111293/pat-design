namespace PAT.GUI.ModuleGUI.KWSNDrawing
{
    partial class SensorEditForm
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
            this.label2 = new System.Windows.Forms.Label();
            this.txtSensorName = new System.Windows.Forms.TextBox();
            this.cmbSensorType = new System.Windows.Forms.ComboBox();
            this.labelSensorMode = new System.Windows.Forms.Label();
            this.cmbSensorMode = new System.Windows.Forms.ComboBox();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(106, 20);
            this.label1.TabIndex = 0;
            this.label1.Text = "Sensor Name";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 52);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(98, 20);
            this.label2.TabIndex = 1;
            this.label2.Text = "Sensor Type";
            // 
            // txtSensorName
            // 
            this.txtSensorName.Location = new System.Drawing.Point(173, 10);
            this.txtSensorName.Name = "txtSensorName";
            this.txtSensorName.Size = new System.Drawing.Size(447, 26);
            this.txtSensorName.TabIndex = 2;
            // 
            // cmbSensorType
            // 
            this.cmbSensorType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSensorType.FormattingEnabled = true;
            this.cmbSensorType.Location = new System.Drawing.Point(173, 49);
            this.cmbSensorType.Name = "cmbSensorType";
            this.cmbSensorType.Size = new System.Drawing.Size(447, 28);
            this.cmbSensorType.TabIndex = 3;
            this.cmbSensorType.SelectedIndexChanged += new System.EventHandler(this.cmbSensorType_SelectedIndexChanged);
            // 
            // labelSensorMode
            // 
            this.labelSensorMode.AutoSize = true;
            this.labelSensorMode.Location = new System.Drawing.Point(13, 94);
            this.labelSensorMode.Name = "labelSensorMode";
            this.labelSensorMode.Size = new System.Drawing.Size(104, 20);
            this.labelSensorMode.TabIndex = 4;
            this.labelSensorMode.Text = "Sensor Mode";
            this.labelSensorMode.Visible = false;
            // 
            // cmbSensorMode
            // 
            this.cmbSensorMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSensorMode.FormattingEnabled = true;
            this.cmbSensorMode.Location = new System.Drawing.Point(173, 91);
            this.cmbSensorMode.Name = "cmbSensorMode";
            this.cmbSensorMode.Size = new System.Drawing.Size(447, 28);
            this.cmbSensorMode.TabIndex = 5;
            this.cmbSensorMode.Visible = false;
            // 
            // btnOK
            // 
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(69, 372);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(100, 37);
            this.btnOK.TabIndex = 6;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(431, 372);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(100, 37);
            this.btnCancel.TabIndex = 7;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // SensorEditForm
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(632, 441);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.cmbSensorMode);
            this.Controls.Add(this.labelSensorMode);
            this.Controls.Add(this.cmbSensorType);
            this.Controls.Add(this.txtSensorName);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SensorEditForm";
            this.ShowIcon = false;
            this.Text = "Sensor Editting Form";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtSensorName;
        private System.Windows.Forms.ComboBox cmbSensorType;
        private System.Windows.Forms.Label labelSensorMode;
        private System.Windows.Forms.ComboBox cmbSensorMode;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
    }
}