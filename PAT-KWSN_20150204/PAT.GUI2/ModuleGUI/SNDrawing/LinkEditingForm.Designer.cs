namespace PAT.GUI.SNDrawing
{
    partial class LinkEditingForm
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
            this.TextBox_Source = new System.Windows.Forms.TextBox();
            this.Button_OK = new System.Windows.Forms.Button();
            this.Button_Cancel = new System.Windows.Forms.Button();
            this.TextBox_Target = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.TextBox_LossRate = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.TextBox_Bandwidth = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.TextBox_Note = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(22, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(62, 23);
            this.label1.TabIndex = 0;
            this.label1.Text = "Source";
            // 
            // TextBox_Source
            // 
            this.TextBox_Source.Location = new System.Drawing.Point(110, 21);
            this.TextBox_Source.Name = "TextBox_Source";
            this.TextBox_Source.ReadOnly = true;
            this.TextBox_Source.Size = new System.Drawing.Size(313, 29);
            this.TextBox_Source.TabIndex = 0;
            this.TextBox_Source.TabStop = false;
            // 
            // Button_OK
            // 
            this.Button_OK.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.Button_OK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Button_OK.Location = new System.Drawing.Point(153, 356);
            this.Button_OK.Name = "Button_OK";
            this.Button_OK.Size = new System.Drawing.Size(75, 31);
            this.Button_OK.TabIndex = 6;
            this.Button_OK.Text = "Ok";
            this.Button_OK.UseVisualStyleBackColor = true;
            this.Button_OK.Click += new System.EventHandler(this.Button_OK_Click);
            // 
            // Button_Cancel
            // 
            this.Button_Cancel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.Button_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Button_Cancel.Location = new System.Drawing.Point(284, 356);
            this.Button_Cancel.Name = "Button_Cancel";
            this.Button_Cancel.Size = new System.Drawing.Size(75, 31);
            this.Button_Cancel.TabIndex = 7;
            this.Button_Cancel.Text = "Cancel";
            this.Button_Cancel.UseVisualStyleBackColor = true;
            // 
            // TextBox_Target
            // 
            this.TextBox_Target.Location = new System.Drawing.Point(110, 68);
            this.TextBox_Target.Name = "TextBox_Target";
            this.TextBox_Target.ReadOnly = true;
            this.TextBox_Target.Size = new System.Drawing.Size(313, 29);
            this.TextBox_Target.TabIndex = 1;
            this.TextBox_Target.TabStop = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(26, 71);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(59, 23);
            this.label2.TabIndex = 4;
            this.label2.Text = "Target";
            // 
            // TextBox_LossRate
            // 
            this.TextBox_LossRate.Location = new System.Drawing.Point(110, 157);
            this.TextBox_LossRate.Multiline = true;
            this.TextBox_LossRate.Name = "TextBox_LossRate";
            this.TextBox_LossRate.Size = new System.Drawing.Size(313, 29);
            this.TextBox_LossRate.TabIndex = 4;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(7, 117);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(91, 23);
            this.label3.TabIndex = 6;
            this.label3.Text = "Bandwidth";
            // 
            // TextBox_Bandwidth
            // 
            this.TextBox_Bandwidth.Location = new System.Drawing.Point(110, 114);
            this.TextBox_Bandwidth.Name = "TextBox_Bandwidth";
            this.TextBox_Bandwidth.Size = new System.Drawing.Size(313, 29);
            this.TextBox_Bandwidth.TabIndex = 3;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(8, 161);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(81, 23);
            this.label4.TabIndex = 8;
            this.label4.Text = "Loss Rate";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(32, 210);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(48, 23);
            this.label5.TabIndex = 11;
            this.label5.Text = "Note";
            // 
            // TextBox_Note
            // 
            this.TextBox_Note.Location = new System.Drawing.Point(112, 210);
            this.TextBox_Note.Multiline = true;
            this.TextBox_Note.Name = "TextBox_Note";
            this.TextBox_Note.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.TextBox_Note.Size = new System.Drawing.Size(311, 123);
            this.TextBox_Note.TabIndex = 5;
            this.TextBox_Note.Visible = false;
            // 
            // LinkEditingForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 23F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.Button_Cancel;
            this.ClientSize = new System.Drawing.Size(477, 419);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.TextBox_Note);
            this.Controls.Add(this.TextBox_Bandwidth);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.TextBox_LossRate);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.TextBox_Target);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.Button_Cancel);
            this.Controls.Add(this.Button_OK);
            this.Controls.Add(this.TextBox_Source);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "LinkEditingForm";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Link Editing Form";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox TextBox_Source;
        private System.Windows.Forms.Button Button_OK;
        private System.Windows.Forms.Button Button_Cancel;
        private System.Windows.Forms.TextBox TextBox_Target;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox TextBox_LossRate;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox TextBox_Bandwidth;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox TextBox_Note;
        //private System.Windows.Forms.TextBox TextBox_Select;
        //private System.Windows.Forms.Label label6;
    }
}