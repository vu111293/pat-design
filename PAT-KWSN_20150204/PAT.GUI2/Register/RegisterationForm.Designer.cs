namespace PAT.GUI.Register
{
    partial class RegisterationForm
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
            this.Label_HardwareCode = new System.Windows.Forms.Label();
            this.Button_Purchase = new System.Windows.Forms.Button();
            this.Label_Register = new System.Windows.Forms.Label();
            this.Label_RegisteredTo = new System.Windows.Forms.Label();
            this.Label_RegisteredUser = new System.Windows.Forms.Label();
            this.TextBox_HardwareCode = new System.Windows.Forms.TextBox();
            this.Label_ValidTime = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // Label_HardwareCode
            // 
            this.Label_HardwareCode.AutoSize = true;
            this.Label_HardwareCode.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label_HardwareCode.Location = new System.Drawing.Point(17, 98);
            this.Label_HardwareCode.Name = "Label_HardwareCode";
            this.Label_HardwareCode.Size = new System.Drawing.Size(106, 16);
            this.Label_HardwareCode.TabIndex = 0;
            this.Label_HardwareCode.Text = "Hardware Code:";
            // 
            // Button_Purchase
            // 
            this.Button_Purchase.Location = new System.Drawing.Point(105, 172);
            this.Button_Purchase.Name = "Button_Purchase";
            this.Button_Purchase.Size = new System.Drawing.Size(159, 23);
            this.Button_Purchase.TabIndex = 2;
            this.Button_Purchase.Text = "Purchase the Full Version";
            this.Button_Purchase.UseVisualStyleBackColor = true;
            this.Button_Purchase.Click += new System.EventHandler(this.Button_Purchase_Click);
            // 
            // Label_Register
            // 
            this.Label_Register.AutoSize = true;
            this.Label_Register.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label_Register.Location = new System.Drawing.Point(9, 20);
            this.Label_Register.Name = "Label_Register";
            this.Label_Register.Size = new System.Drawing.Size(368, 25);
            this.Label_Register.TabIndex = 3;
            this.Label_Register.Text = "Your copy of PATPro is unregistered!";
            // 
            // Label_RegisteredTo
            // 
            this.Label_RegisteredTo.AutoSize = true;
            this.Label_RegisteredTo.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label_RegisteredTo.Location = new System.Drawing.Point(17, 63);
            this.Label_RegisteredTo.Name = "Label_RegisteredTo";
            this.Label_RegisteredTo.Size = new System.Drawing.Size(98, 16);
            this.Label_RegisteredTo.TabIndex = 4;
            this.Label_RegisteredTo.Text = "Registered To:";
            // 
            // Label_RegisteredUser
            // 
            this.Label_RegisteredUser.AutoSize = true;
            this.Label_RegisteredUser.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label_RegisteredUser.Location = new System.Drawing.Point(163, 63);
            this.Label_RegisteredUser.Name = "Label_RegisteredUser";
            this.Label_RegisteredUser.Size = new System.Drawing.Size(0, 16);
            this.Label_RegisteredUser.TabIndex = 5;
            // 
            // TextBox_HardwareCode
            // 
            this.TextBox_HardwareCode.Location = new System.Drawing.Point(163, 96);
            this.TextBox_HardwareCode.Name = "TextBox_HardwareCode";
            this.TextBox_HardwareCode.ReadOnly = true;
            this.TextBox_HardwareCode.Size = new System.Drawing.Size(165, 22);
            this.TextBox_HardwareCode.TabIndex = 6;
            this.TextBox_HardwareCode.Text = "####-####-####-####-####";
            // 
            // Label_ValidTime
            // 
            this.Label_ValidTime.AutoSize = true;
            this.Label_ValidTime.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label_ValidTime.Location = new System.Drawing.Point(163, 133);
            this.Label_ValidTime.Name = "Label_ValidTime";
            this.Label_ValidTime.Size = new System.Drawing.Size(0, 16);
            this.Label_ValidTime.TabIndex = 8;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(17, 133);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(76, 16);
            this.label2.TabIndex = 7;
            this.label2.Text = "Valid Time:";
            // 
            // RegisterationForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(386, 207);
            this.Controls.Add(this.Label_ValidTime);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.TextBox_HardwareCode);
            this.Controls.Add(this.Label_RegisteredUser);
            this.Controls.Add(this.Label_RegisteredTo);
            this.Controls.Add(this.Label_Register);
            this.Controls.Add(this.Button_Purchase);
            this.Controls.Add(this.Label_HardwareCode);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "RegisterationForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "PATPro Registration";
            this.Load += new System.EventHandler(this.RegisterationForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label Label_HardwareCode;
        private System.Windows.Forms.Button Button_Purchase;
        private System.Windows.Forms.Label Label_Register;
        private System.Windows.Forms.Label Label_RegisteredTo;
        private System.Windows.Forms.Label Label_RegisteredUser;
        private System.Windows.Forms.TextBox TextBox_HardwareCode;
        private System.Windows.Forms.Label Label_ValidTime;
        private System.Windows.Forms.Label label2;
    }
}