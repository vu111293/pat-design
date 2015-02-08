namespace PAT.Common.GUI
{
    partial class StateInfoControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(StateInfoControl));
            this.PictureBox_Image = new System.Windows.Forms.PictureBox();
            this.GroupBox_State = new System.Windows.Forms.GroupBox();
            this.TextBox_Info = new System.Windows.Forms.RichTextBox();
            this.GroupBox_Image = new System.Windows.Forms.GroupBox();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            ((System.ComponentModel.ISupportInitialize)(this.PictureBox_Image)).BeginInit();
            this.GroupBox_State.SuspendLayout();
            this.GroupBox_Image.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // PictureBox_Image
            // 
            resources.ApplyResources(this.PictureBox_Image, "PictureBox_Image");
            this.PictureBox_Image.Name = "PictureBox_Image";
            this.PictureBox_Image.TabStop = false;
            // 
            // GroupBox_State
            // 
            this.GroupBox_State.Controls.Add(this.TextBox_Info);
            resources.ApplyResources(this.GroupBox_State, "GroupBox_State");
            this.GroupBox_State.Name = "GroupBox_State";
            this.GroupBox_State.TabStop = false;
            // 
            // TextBox_Info
            // 
            this.TextBox_Info.BackColor = System.Drawing.Color.White;
            this.TextBox_Info.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.TextBox_Info, "TextBox_Info");
            this.TextBox_Info.Name = "TextBox_Info";
            this.TextBox_Info.ReadOnly = true;
            // 
            // GroupBox_Image
            // 
            this.GroupBox_Image.Controls.Add(this.PictureBox_Image);
            resources.ApplyResources(this.GroupBox_Image, "GroupBox_Image");
            this.GroupBox_Image.Name = "GroupBox_Image";
            this.GroupBox_Image.TabStop = false;
            // 
            // splitContainer1
            // 
            resources.ApplyResources(this.splitContainer1, "splitContainer1");
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.GroupBox_State);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.GroupBox_Image);
            // 
            // StateInfoControl
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.Name = "StateInfoControl";
            ((System.ComponentModel.ISupportInitialize)(this.PictureBox_Image)).EndInit();
            this.GroupBox_State.ResumeLayout(false);
            this.GroupBox_Image.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox PictureBox_Image;
        private System.Windows.Forms.GroupBox GroupBox_State;
        private System.Windows.Forms.RichTextBox TextBox_Info;
        private System.Windows.Forms.GroupBox GroupBox_Image;
        private System.Windows.Forms.SplitContainer splitContainer1;
    }
}
