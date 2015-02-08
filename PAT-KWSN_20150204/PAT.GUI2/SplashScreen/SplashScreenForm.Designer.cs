namespace PAT.GUI.SplashScreen
{
    partial class SplashScreenForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SplashScreenForm));
            this.pictureBox = new System.Windows.Forms.PictureBox();
            this.textLabel = new System.Windows.Forms.Label();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.versionLabel = new System.Windows.Forms.Label();
            this.Label_FullName = new System.Windows.Forms.Label();
            this.Label_RegistrationInfo = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox
            // 
            this.pictureBox.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox.Image")));
            this.pictureBox.InitialImage = ((System.Drawing.Image)(resources.GetObject("pictureBox.InitialImage")));
            this.pictureBox.Location = new System.Drawing.Point(0, 0);
            this.pictureBox.Name = "pictureBox";
            this.pictureBox.Size = new System.Drawing.Size(530, 320);
            this.pictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox.TabIndex = 0;
            this.pictureBox.TabStop = false;
            // 
            // textLabel
            // 
            this.textLabel.AutoSize = true;
            this.textLabel.BackColor = System.Drawing.Color.Transparent;
            this.textLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textLabel.ForeColor = System.Drawing.Color.Black;
            this.textLabel.Location = new System.Drawing.Point(58, 276);
            this.textLabel.Name = "textLabel";
            this.textLabel.Size = new System.Drawing.Size(115, 16);
            this.textLabel.TabIndex = 1;
            this.textLabel.Text = "Checking update..";
            // 
            // progressBar
            // 
            this.progressBar.BackColor = System.Drawing.Color.White;
            this.progressBar.ForeColor = System.Drawing.Color.DarkBlue;
            this.progressBar.Location = new System.Drawing.Point(58, 244);
            this.progressBar.MarqueeAnimationSpeed = 50;
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(414, 16);
            this.progressBar.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progressBar.TabIndex = 0;
            // 
            // versionLabel
            // 
            this.versionLabel.AutoSize = true;
            this.versionLabel.BackColor = System.Drawing.Color.Transparent;
            this.versionLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.versionLabel.ForeColor = System.Drawing.Color.White;
            this.versionLabel.Location = new System.Drawing.Point(65, 142);
            this.versionLabel.Name = "versionLabel";
            this.versionLabel.Size = new System.Drawing.Size(65, 16);
            this.versionLabel.TabIndex = 2;
            this.versionLabel.Text = "Version:";
            // 
            // Label_FullName
            // 
            this.Label_FullName.AutoSize = true;
            this.Label_FullName.BackColor = System.Drawing.Color.Transparent;
            this.Label_FullName.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label_FullName.ForeColor = System.Drawing.Color.White;
            this.Label_FullName.Location = new System.Drawing.Point(248, 108);
            this.Label_FullName.Name = "Label_FullName";
            this.Label_FullName.Size = new System.Drawing.Size(195, 18);
            this.Label_FullName.TabIndex = 3;
            this.Label_FullName.Text = "Process Analysis Toolkit";
            // 
            // Label_RegistrationInfo
            // 
            this.Label_RegistrationInfo.AutoSize = true;
            this.Label_RegistrationInfo.BackColor = System.Drawing.Color.Transparent;
            this.Label_RegistrationInfo.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label_RegistrationInfo.ForeColor = System.Drawing.Color.White;
            this.Label_RegistrationInfo.Location = new System.Drawing.Point(65, 171);
            this.Label_RegistrationInfo.Name = "Label_RegistrationInfo";
            this.Label_RegistrationInfo.Size = new System.Drawing.Size(0, 16);
            this.Label_RegistrationInfo.TabIndex = 4;
            // 
            // SplashScreenForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(530, 320);
            this.Controls.Add(this.Label_RegistrationInfo);
            this.Controls.Add(this.Label_FullName);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.versionLabel);
            this.Controls.Add(this.textLabel);
            this.Controls.Add(this.pictureBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "SplashScreenForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "SplashForm";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SplashForm_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox;
        private System.Windows.Forms.Label textLabel;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Label versionLabel;
        private System.Windows.Forms.Label Label_FullName;
        private System.Windows.Forms.Label Label_RegistrationInfo;
    }
}