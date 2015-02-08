namespace PAT.GUI.UpdateChecking.WindowsForms
{
    partial class ConfirmDoUpdateForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConfirmDoUpdateForm));
            this.Label_Notification = new System.Windows.Forms.Label();
            this.Label_MostRecentVersion = new System.Windows.Forms.Label();
            this.lblMostRecentVersion = new System.Windows.Forms.Label();
            this.Label_Upgrade = new System.Windows.Forms.Label();
            this.Button_UpgradeNow = new System.Windows.Forms.Button();
            this.Button_UpgradeLater = new System.Windows.Forms.Button();
            this.lblCurrentVersion = new System.Windows.Forms.Label();
            this.Label_YourCurrentVersion = new System.Windows.Forms.Label();
            this.Label_WhatNew = new System.Windows.Forms.LinkLabel();
            this.Label_NoUpdate = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // Label_Notification
            // 
            resources.ApplyResources(this.Label_Notification, "Label_Notification");
            this.Label_Notification.ForeColor = System.Drawing.Color.Blue;
            this.Label_Notification.Name = "Label_Notification";
            // 
            // Label_MostRecentVersion
            // 
            resources.ApplyResources(this.Label_MostRecentVersion, "Label_MostRecentVersion");
            this.Label_MostRecentVersion.MinimumSize = new System.Drawing.Size(170, 0);
            this.Label_MostRecentVersion.Name = "Label_MostRecentVersion";
            // 
            // lblMostRecentVersion
            // 
            resources.ApplyResources(this.lblMostRecentVersion, "lblMostRecentVersion");
            this.lblMostRecentVersion.Name = "lblMostRecentVersion";
            // 
            // Label_Upgrade
            // 
            resources.ApplyResources(this.Label_Upgrade, "Label_Upgrade");
            this.Label_Upgrade.Name = "Label_Upgrade";
            // 
            // Button_UpgradeNow
            // 
            resources.ApplyResources(this.Button_UpgradeNow, "Button_UpgradeNow");
            this.Button_UpgradeNow.Name = "Button_UpgradeNow";
            this.Button_UpgradeNow.UseVisualStyleBackColor = true;
            this.Button_UpgradeNow.Click += new System.EventHandler(this.btnNow_Click);
            // 
            // Button_UpgradeLater
            // 
            resources.ApplyResources(this.Button_UpgradeLater, "Button_UpgradeLater");
            this.Button_UpgradeLater.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Button_UpgradeLater.Name = "Button_UpgradeLater";
            this.Button_UpgradeLater.UseVisualStyleBackColor = true;
            this.Button_UpgradeLater.Click += new System.EventHandler(this.btnLater_Click);
            // 
            // lblCurrentVersion
            // 
            resources.ApplyResources(this.lblCurrentVersion, "lblCurrentVersion");
            this.lblCurrentVersion.Name = "lblCurrentVersion";
            // 
            // Label_YourCurrentVersion
            // 
            resources.ApplyResources(this.Label_YourCurrentVersion, "Label_YourCurrentVersion");
            this.Label_YourCurrentVersion.MinimumSize = new System.Drawing.Size(170, 0);
            this.Label_YourCurrentVersion.Name = "Label_YourCurrentVersion";
            // 
            // Label_WhatNew
            // 
            resources.ApplyResources(this.Label_WhatNew, "Label_WhatNew");
            this.Label_WhatNew.Name = "Label_WhatNew";
            this.Label_WhatNew.TabStop = true;
            this.Label_WhatNew.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.Label_WhatNew_LinkClicked);
            // 
            // Label_NoUpdate
            // 
            resources.ApplyResources(this.Label_NoUpdate, "Label_NoUpdate");
            this.Label_NoUpdate.ForeColor = System.Drawing.Color.Red;
            this.Label_NoUpdate.Name = "Label_NoUpdate";
            // 
            // ConfirmDoUpdateForm
            // 
            this.AcceptButton = this.Button_UpgradeNow;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.Button_UpgradeLater;
            this.Controls.Add(this.Label_WhatNew);
            this.Controls.Add(this.lblCurrentVersion);
            this.Controls.Add(this.Label_YourCurrentVersion);
            this.Controls.Add(this.Button_UpgradeLater);
            this.Controls.Add(this.Button_UpgradeNow);
            this.Controls.Add(this.Label_Upgrade);
            this.Controls.Add(this.lblMostRecentVersion);
            this.Controls.Add(this.Label_MostRecentVersion);
            this.Controls.Add(this.Label_Notification);
            this.Controls.Add(this.Label_NoUpdate);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ConfirmDoUpdateForm";
            this.ShowIcon = false;
            this.TopMost = true;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label Label_Notification;
        private System.Windows.Forms.Label Label_MostRecentVersion;
        private System.Windows.Forms.Label lblMostRecentVersion;
        private System.Windows.Forms.Label Label_Upgrade;
        private System.Windows.Forms.Button Button_UpgradeNow;
        private System.Windows.Forms.Button Button_UpgradeLater;
        private System.Windows.Forms.Label lblCurrentVersion;
        private System.Windows.Forms.Label Label_YourCurrentVersion;
        private System.Windows.Forms.LinkLabel Label_WhatNew;
        private System.Windows.Forms.Label Label_NoUpdate;
    }
}