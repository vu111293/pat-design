using System;
using System.Diagnostics;
using System.Windows.Forms;
using PAT.Common.Ultility;
using PAT.GUI.Properties;


namespace PAT.GUI.UpdateChecking.WindowsForms
{
    public partial class ConfirmDoUpdateForm : Form
    {
        public ConfirmDoUpdateForm()
        {

            InitializeComponent();

            this.Text = Ultility.APPLICATION_NAME;

            if (UpdateManager.LocalVersion != null)
            {
                lblCurrentVersion.Text = UpdateManager.LocalVersion.ToString();
            }
            else lblCurrentVersion.Text = "Unknown";

            if (UpdateManager.DeploymentManifest != null)
            {
                lblMostRecentVersion.Text = UpdateManager.DeploymentManifest.CurrentPublishedVersion.ToString();
            }
            else lblMostRecentVersion.Text = "Unknown";


#if !ENABLE_UPDATE
            Label_NoUpdate.Visible = true;
            Label_Upgrade.Visible = false;
            this.Button_UpgradeNow.Enabled = false;
#endif
        }

        private void btnLater_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void btnNow_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        private void Label_WhatNew_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                Process.Start("http://www.comp.nus.edu.sg/~pat/#[[Version History]]");
            }
            catch (Exception ex)
            {
                MessageBox.Show(Resources.Exception_happened__ + ex.Message + "\r\n" + ex.StackTrace,
                                Ultility.APPLICATION_NAME, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}