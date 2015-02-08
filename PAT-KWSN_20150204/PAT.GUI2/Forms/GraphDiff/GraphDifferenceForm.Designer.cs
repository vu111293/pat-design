namespace PAT.GUI.Forms.GraphDiff
{
    partial class GraphDifferenceForm
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
            this.Label = new System.Windows.Forms.Label();
            this.Button_Compare = new System.Windows.Forms.Button();
            this.Button_Cancel = new System.Windows.Forms.Button();
            this.CheckedListBox_Forms = new System.Windows.Forms.CheckedListBox();
            this.SuspendLayout();
            // 
            // Label
            // 
            this.Label.AutoSize = true;
            this.Label.Location = new System.Drawing.Point(12, 19);
            this.Label.Name = "Label";
            this.Label.Size = new System.Drawing.Size(193, 13);
            this.Label.TabIndex = 1;
            this.Label.Text = "Select two simulators from the list below";
            // 
            // Button_Compare
            // 
            this.Button_Compare.Enabled = false;
            this.Button_Compare.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Button_Compare.Location = new System.Drawing.Point(166, 325);
            this.Button_Compare.Name = "Button_Compare";
            this.Button_Compare.Size = new System.Drawing.Size(75, 23);
            this.Button_Compare.TabIndex = 2;
            this.Button_Compare.Text = "Compare";
            this.Button_Compare.UseVisualStyleBackColor = true;
            this.Button_Compare.Click += new System.EventHandler(this.Button_Compare_Click);
            // 
            // Button_Cancel
            // 
            this.Button_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Button_Cancel.Location = new System.Drawing.Point(299, 325);
            this.Button_Cancel.Name = "Button_Cancel";
            this.Button_Cancel.Size = new System.Drawing.Size(75, 23);
            this.Button_Cancel.TabIndex = 3;
            this.Button_Cancel.Text = "Cancel";
            this.Button_Cancel.UseVisualStyleBackColor = true;
            // 
            // CheckedListBox_Forms
            // 
            this.CheckedListBox_Forms.CheckOnClick = true;
            this.CheckedListBox_Forms.FormattingEnabled = true;
            this.CheckedListBox_Forms.Location = new System.Drawing.Point(12, 44);
            this.CheckedListBox_Forms.Margin = new System.Windows.Forms.Padding(10);
            this.CheckedListBox_Forms.Name = "CheckedListBox_Forms";
            this.CheckedListBox_Forms.Size = new System.Drawing.Size(509, 244);
            this.CheckedListBox_Forms.TabIndex = 4;
            this.CheckedListBox_Forms.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.CheckedListBox_Forms_ItemCheck);
            // 
            // GraphDifferenceForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(540, 371);
            this.Controls.Add(this.CheckedListBox_Forms);
            this.Controls.Add(this.Button_Cancel);
            this.Controls.Add(this.Button_Compare);
            this.Controls.Add(this.Label);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "GraphDifferenceForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Graph Difference Comparison";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label Label;
        private System.Windows.Forms.Button Button_Compare;
        private System.Windows.Forms.Button Button_Cancel;
        private System.Windows.Forms.CheckedListBox CheckedListBox_Forms;
    }
}