using System.Windows.Forms;

namespace PAT.GUI.SNDrawing
{
    partial class NodeEditingForm
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
            this.TextBox_Name = new System.Windows.Forms.TextBox();
            this.label1id = new System.Windows.Forms.Label();
            this.TextBox_ID = new System.Windows.Forms.TextBox();
            this.Button_OK = new System.Windows.Forms.Button();
            this.Button_Cancel = new System.Windows.Forms.Button();
            this.textBox_Application = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.TextBox_Sensors = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.labelPredefined = new System.Windows.Forms.Label();
            this.TextBox_PredefinedVars = new System.Windows.Forms.TextBox();
            this.label_Note = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 25);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(67, 23);
            this.label1.TabIndex = 0;
            this.label1.Text = "*Name:";
            // 
            // TextBox_Name
            // 
            this.TextBox_Name.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TextBox_Name.Location = new System.Drawing.Point(150, 20);
            this.TextBox_Name.Name = "TextBox_Name";
            this.TextBox_Name.Size = new System.Drawing.Size(365, 29);
            this.TextBox_Name.TabIndex = 1;
            // 
            // label1id
            // 
            this.label1id.AutoSize = true;
            this.label1id.Location = new System.Drawing.Point(12, 65);
            this.label1id.Name = "label1id";
            this.label1id.Size = new System.Drawing.Size(130, 23);
            this.label1id.TabIndex = 10;
            this.label1id.Text = "*TOS_NODE_ID:";
            // 
            // TextBox_ID
            // 
            this.TextBox_ID.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TextBox_ID.Location = new System.Drawing.Point(150, 60);
            this.TextBox_ID.Name = "TextBox_ID";
            this.TextBox_ID.Size = new System.Drawing.Size(365, 29);
            this.TextBox_ID.TabIndex = 11;
            // 
            // Button_OK
            // 
            this.Button_OK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Button_OK.Location = new System.Drawing.Point(134, 249);
            this.Button_OK.Name = "Button_OK";
            this.Button_OK.Size = new System.Drawing.Size(76, 32);
            this.Button_OK.TabIndex = 2;
            this.Button_OK.Text = "Ok";
            this.Button_OK.UseVisualStyleBackColor = true;
            // 
            // Button_Cancel
            // 
            this.Button_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Button_Cancel.Location = new System.Drawing.Point(278, 249);
            this.Button_Cancel.Name = "Button_Cancel";
            this.Button_Cancel.Size = new System.Drawing.Size(76, 32);
            this.Button_Cancel.TabIndex = 3;
            this.Button_Cancel.Text = "Cancel";
            this.Button_Cancel.UseVisualStyleBackColor = true;
            // 
            // textBox_Application
            // 
            this.textBox_Application.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox_Application.Location = new System.Drawing.Point(150, 102);
            this.textBox_Application.Name = "textBox_Application";
            this.textBox_Application.Size = new System.Drawing.Size(365, 29);
            this.textBox_Application.TabIndex = 5;
            this.textBox_Application.Click += new System.EventHandler(this.textBox_Application_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 102);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(107, 23);
            this.label2.TabIndex = 4;
            this.label2.Text = "*Application:";
            // 
            // TextBox_Sensors
            // 
            this.TextBox_Sensors.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TextBox_Sensors.Location = new System.Drawing.Point(150, 140);
            this.TextBox_Sensors.Name = "TextBox_Sensors";
            this.TextBox_Sensors.Size = new System.Drawing.Size(365, 29);
            this.TextBox_Sensors.TabIndex = 7;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 145);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(129, 23);
            this.label3.TabIndex = 6;
            this.label3.Text = "Sensors\' Range:";
            // 
            // labelPredefined
            // 
            this.labelPredefined.AutoSize = true;
            this.labelPredefined.Location = new System.Drawing.Point(12, 183);
            this.labelPredefined.Name = "labelPredefined";
            this.labelPredefined.Size = new System.Drawing.Size(131, 23);
            this.labelPredefined.TabIndex = 8;
            this.labelPredefined.Text = "Predefined vars:";
            // 
            // TextBox_PredefinedVars
            // 
            this.TextBox_PredefinedVars.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TextBox_PredefinedVars.Location = new System.Drawing.Point(149, 178);
            this.TextBox_PredefinedVars.Name = "TextBox_PredefinedVars";
            this.TextBox_PredefinedVars.Size = new System.Drawing.Size(366, 29);
            this.TextBox_PredefinedVars.TabIndex = 9;
            // 
            // label_Note
            // 
            this.label_Note.AutoSize = true;
            this.label_Note.Location = new System.Drawing.Point(12, 211);
            this.label_Note.Name = "label_Note";
            this.label_Note.Size = new System.Drawing.Size(311, 23);
            this.label_Note.TabIndex = 0;
            this.label_Note.Text = "(Note: the items with * can not be null.)";
            // 
            // NodeEditingForm
            // 
            this.AcceptButton = this.Button_OK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 23F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.Button_Cancel;
            this.ClientSize = new System.Drawing.Size(522, 304);
            this.Controls.Add(this.TextBox_Sensors);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.labelPredefined);
            this.Controls.Add(this.TextBox_PredefinedVars);
            this.Controls.Add(this.textBox_Application);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.Button_Cancel);
            this.Controls.Add(this.Button_OK);
            this.Controls.Add(this.TextBox_Name);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.TextBox_ID);
            this.Controls.Add(this.label1id);
            this.Controls.Add(this.label_Note);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "NodeEditingForm";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Sensor Editing Form";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        protected System.Windows.Forms.Label label1;
        protected System.Windows.Forms.TextBox TextBox_Name;
        protected System.Windows.Forms.Label label1id;
        protected System.Windows.Forms.TextBox TextBox_ID;
        protected System.Windows.Forms.Button Button_OK;
        protected System.Windows.Forms.Button Button_Cancel;
        protected System.Windows.Forms.TextBox textBox_Application;
        protected System.Windows.Forms.Label label2;
        protected System.Windows.Forms.Label label3;
        protected System.Windows.Forms.Label labelPredefined;
        protected System.Windows.Forms.TextBox TextBox_PredefinedVars;
        protected System.Windows.Forms.TextBox TextBox_Sensors;
        //protected System.Windows.Forms.Label label4;
        //protected System.Windows.Forms.TextBox TextBox_BufferSize;
        protected System.Windows.Forms.Label label_Note;
        //protected System.Windows.Forms.Button Button_ChooseFile;
    }
}