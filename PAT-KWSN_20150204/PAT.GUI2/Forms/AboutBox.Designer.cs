namespace PAT.GUI.Forms
{
    partial class AboutBox
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
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
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.ColumnHeader columnHeader3;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AboutBox));
            System.Windows.Forms.ColumnHeader columnHeader2;
            System.Windows.Forms.ColumnHeader columnHeader1;
            this.tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.Label_ModuleDescription = new System.Windows.Forms.Label();
            this.ListView_Modules = new System.Windows.Forms.ListView();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.Label_InstalledModules = new System.Windows.Forms.Label();
            this.Label_ProductName = new System.Windows.Forms.Label();
            this.Label_Version = new System.Windows.Forms.Label();
            this.Label_Copyright = new System.Windows.Forms.Label();
            this.Label_CompanyName = new System.Windows.Forms.Label();
            this.textBoxDescription = new System.Windows.Forms.TextBox();
            this.Label_URL = new System.Windows.Forms.LinkLabel();
            this.Button_OK = new System.Windows.Forms.Button();
            columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.tableLayoutPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // columnHeader3
            // 
            resources.ApplyResources(columnHeader3, "columnHeader3");
            // 
            // columnHeader2
            // 
            resources.ApplyResources(columnHeader2, "columnHeader2");
            // 
            // columnHeader1
            // 
            resources.ApplyResources(columnHeader1, "columnHeader1");
            // 
            // tableLayoutPanel
            // 
            resources.ApplyResources(this.tableLayoutPanel, "tableLayoutPanel");
            this.tableLayoutPanel.Controls.Add(this.Label_ModuleDescription, 0, 7);
            this.tableLayoutPanel.Controls.Add(this.ListView_Modules, 0, 6);
            this.tableLayoutPanel.Controls.Add(this.Label_InstalledModules, 0, 5);
            this.tableLayoutPanel.Controls.Add(this.Label_ProductName, 0, 0);
            this.tableLayoutPanel.Controls.Add(this.Label_Version, 0, 1);
            this.tableLayoutPanel.Controls.Add(this.Label_Copyright, 0, 2);
            this.tableLayoutPanel.Controls.Add(this.Label_CompanyName, 0, 3);
            this.tableLayoutPanel.Controls.Add(this.textBoxDescription, 0, 8);
            this.tableLayoutPanel.Controls.Add(this.Label_URL, 0, 4);
            this.tableLayoutPanel.Controls.Add(this.Button_OK, 0, 9);
            this.tableLayoutPanel.Name = "tableLayoutPanel";
            // 
            // Label_ModuleDescription
            // 
            resources.ApplyResources(this.Label_ModuleDescription, "Label_ModuleDescription");
            this.Label_ModuleDescription.Name = "Label_ModuleDescription";
            // 
            // ListView_Modules
            // 
            resources.ApplyResources(this.ListView_Modules, "ListView_Modules");
            this.ListView_Modules.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            columnHeader3,
            columnHeader2,
            columnHeader1});
            this.ListView_Modules.FullRowSelect = true;
            this.ListView_Modules.GridLines = true;
            this.ListView_Modules.HideSelection = false;
            this.ListView_Modules.MultiSelect = false;
            this.ListView_Modules.Name = "ListView_Modules";
            this.ListView_Modules.ShowGroups = false;
            this.ListView_Modules.SmallImageList = this.imageList1;
            this.ListView_Modules.UseCompatibleStateImageBehavior = false;
            this.ListView_Modules.View = System.Windows.Forms.View.Details;
            this.ListView_Modules.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.ListView_Modules_ItemSelectionChanged);
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "CSP");
            this.imageList1.Images.SetKeyName(1, "WS");
            // 
            // Label_InstalledModules
            // 
            resources.ApplyResources(this.Label_InstalledModules, "Label_InstalledModules");
            this.Label_InstalledModules.Name = "Label_InstalledModules";
            // 
            // Label_ProductName
            // 
            resources.ApplyResources(this.Label_ProductName, "Label_ProductName");
            this.Label_ProductName.Name = "Label_ProductName";
            // 
            // Label_Version
            // 
            resources.ApplyResources(this.Label_Version, "Label_Version");
            this.Label_Version.Name = "Label_Version";
            // 
            // Label_Copyright
            // 
            resources.ApplyResources(this.Label_Copyright, "Label_Copyright");
            this.Label_Copyright.Name = "Label_Copyright";
            // 
            // Label_CompanyName
            // 
            resources.ApplyResources(this.Label_CompanyName, "Label_CompanyName");
            this.Label_CompanyName.Name = "Label_CompanyName";
            // 
            // textBoxDescription
            // 
            resources.ApplyResources(this.textBoxDescription, "textBoxDescription");
            this.textBoxDescription.Name = "textBoxDescription";
            this.textBoxDescription.ReadOnly = true;
            this.textBoxDescription.TabStop = false;
            // 
            // Label_URL
            // 
            resources.ApplyResources(this.Label_URL, "Label_URL");
            this.Label_URL.Name = "Label_URL";
            this.Label_URL.TabStop = true;
            this.Label_URL.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.Label_URL_LinkClicked);
            // 
            // Button_OK
            // 
            resources.ApplyResources(this.Button_OK, "Button_OK");
            this.Button_OK.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Button_OK.Name = "Button_OK";
            // 
            // AboutBox
            // 
            this.AcceptButton = this.Button_OK;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AboutBox";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.tableLayoutPanel.ResumeLayout(false);
            this.tableLayoutPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel;
        private System.Windows.Forms.Label Label_ProductName;
        private System.Windows.Forms.Label Label_Version;
        private System.Windows.Forms.Label Label_Copyright;
        private System.Windows.Forms.Label Label_CompanyName;
        private System.Windows.Forms.TextBox textBoxDescription;
        private System.Windows.Forms.Button Button_OK;
        private System.Windows.Forms.LinkLabel Label_URL;
        private System.Windows.Forms.Label Label_InstalledModules;
        private System.Windows.Forms.ListView ListView_Modules;
        private System.Windows.Forms.Label Label_ModuleDescription;
        private System.Windows.Forms.ImageList imageList1;
    }
}