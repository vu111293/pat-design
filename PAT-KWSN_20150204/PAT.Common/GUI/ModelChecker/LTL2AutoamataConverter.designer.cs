namespace PAT.Common.GUI
{
    partial class LTL2AutoamataConverter
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LTL2AutoamataConverter));
            this.toolStripContainer = new System.Windows.Forms.ToolStripContainer();
            this.statusStrip2 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel2 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel5 = new System.Windows.Forms.ToolStripStatusLabel();
            this.StatusLabel_Accept = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel4 = new System.Windows.Forms.ToolStripStatusLabel();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.StatusLabel_Status = new System.Windows.Forms.ToolStripStatusLabel();
            this.Label_IsSafety = new System.Windows.Forms.ToolStripStatusLabel();
            this.Label_IsDeterministic = new System.Windows.Forms.ToolStripStatusLabel();
            this.gViewer = new Microsoft.Msagl.GraphViewerGdi.GViewer();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.TextBox_Prop = new System.Windows.Forms.ToolStripComboBox();
            this.Button_GenerateStreetAutomata = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.Button_GenerateRabinAutomata = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.Button_GenerateBuchiAutomata = new System.Windows.Forms.ToolStripButton();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.BottomToolStripPanel = new System.Windows.Forms.ToolStripPanel();
            this.TopToolStripPanel = new System.Windows.Forms.ToolStripPanel();
            this.RightToolStripPanel = new System.Windows.Forms.ToolStripPanel();
            this.LeftToolStripPanel = new System.Windows.Forms.ToolStripPanel();
            this.ContentPanel = new System.Windows.Forms.ToolStripContentPanel();
            this.toolStripContainer.BottomToolStripPanel.SuspendLayout();
            this.toolStripContainer.ContentPanel.SuspendLayout();
            this.toolStripContainer.TopToolStripPanel.SuspendLayout();
            this.toolStripContainer.SuspendLayout();
            this.statusStrip2.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStripContainer
            // 
            // 
            // toolStripContainer.BottomToolStripPanel
            // 
            this.toolStripContainer.BottomToolStripPanel.Controls.Add(this.statusStrip2);
            this.toolStripContainer.BottomToolStripPanel.Controls.Add(this.statusStrip1);
            // 
            // toolStripContainer.ContentPanel
            // 
            this.toolStripContainer.ContentPanel.Controls.Add(this.gViewer);
            resources.ApplyResources(this.toolStripContainer.ContentPanel, "toolStripContainer.ContentPanel");
            resources.ApplyResources(this.toolStripContainer, "toolStripContainer");
            this.toolStripContainer.Name = "toolStripContainer";
            // 
            // toolStripContainer.TopToolStripPanel
            // 
            this.toolStripContainer.TopToolStripPanel.Controls.Add(this.toolStrip1);
            // 
            // statusStrip2
            // 
            resources.ApplyResources(this.statusStrip2, "statusStrip2");
            this.statusStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel2,
            this.toolStripStatusLabel1,
            this.toolStripStatusLabel5,
            this.StatusLabel_Accept,
            this.toolStripStatusLabel4});
            this.statusStrip2.Name = "statusStrip2";
            // 
            // toolStripStatusLabel2
            // 
            this.toolStripStatusLabel2.Name = "toolStripStatusLabel2";
            resources.ApplyResources(this.toolStripStatusLabel2, "toolStripStatusLabel2");
            // 
            // toolStripStatusLabel1
            // 
            resources.ApplyResources(this.toolStripStatusLabel1, "toolStripStatusLabel1");
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            // 
            // toolStripStatusLabel5
            // 
            resources.ApplyResources(this.toolStripStatusLabel5, "toolStripStatusLabel5");
            this.toolStripStatusLabel5.Name = "toolStripStatusLabel5";
            // 
            // StatusLabel_Accept
            // 
            resources.ApplyResources(this.StatusLabel_Accept, "StatusLabel_Accept");
            this.StatusLabel_Accept.Name = "StatusLabel_Accept";
            // 
            // toolStripStatusLabel4
            // 
            resources.ApplyResources(this.toolStripStatusLabel4, "toolStripStatusLabel4");
            this.toolStripStatusLabel4.Name = "toolStripStatusLabel4";
            // 
            // statusStrip1
            // 
            resources.ApplyResources(this.statusStrip1, "statusStrip1");
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.StatusLabel_Status,
            this.Label_IsSafety,
            this.Label_IsDeterministic});
            this.statusStrip1.Name = "statusStrip1";
            // 
            // StatusLabel_Status
            // 
            this.StatusLabel_Status.Name = "StatusLabel_Status";
            resources.ApplyResources(this.StatusLabel_Status, "StatusLabel_Status");
            // 
            // Label_IsSafety
            // 
            this.Label_IsSafety.Name = "Label_IsSafety";
            resources.ApplyResources(this.Label_IsSafety, "Label_IsSafety");
            // 
            // Label_IsDeterministic
            // 
            this.Label_IsDeterministic.Name = "Label_IsDeterministic";
            resources.ApplyResources(this.Label_IsDeterministic, "Label_IsDeterministic");
            // 
            // gViewer
            // 
            this.gViewer.AsyncLayout = false;
            resources.ApplyResources(this.gViewer, "gViewer");
            this.gViewer.BackwardEnabled = false;
            this.gViewer.BuildHitTree = true;
            this.gViewer.CurrentLayoutMethod = Microsoft.Msagl.GraphViewerGdi.LayoutMethod.SugiyamaScheme;
            this.gViewer.ForwardEnabled = false;
            this.gViewer.Graph = null;
            this.gViewer.LayoutAlgorithmSettingsButtonVisible = true;
            this.gViewer.LayoutEditingEnabled = true;
            this.gViewer.MouseHitDistance = 0.05D;
            this.gViewer.Name = "gViewer";
            this.gViewer.NavigationVisible = true;
            this.gViewer.NeedToCalculateLayout = true;
            this.gViewer.PanButtonPressed = false;
            this.gViewer.SaveAsImageEnabled = true;
            this.gViewer.SaveAsMsaglEnabled = true;
            this.gViewer.SaveButtonVisible = true;
            this.gViewer.SaveGraphButtonVisible = true;
            this.gViewer.SaveInVectorFormatEnabled = true;
            this.gViewer.ToolBarIsVisible = true;
            this.gViewer.ZoomF = 1D;
            this.gViewer.ZoomFraction = 0.5D;
            this.gViewer.ZoomWindowThreshold = 0.05D;
            // 
            // toolStrip1
            // 
            resources.ApplyResources(this.toolStrip1, "toolStrip1");
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripLabel1,
            this.TextBox_Prop,
            this.Button_GenerateStreetAutomata,
            this.toolStripSeparator1,
            this.Button_GenerateRabinAutomata,
            this.toolStripSeparator2,
            this.Button_GenerateBuchiAutomata});
            this.toolStrip1.Name = "toolStrip1";
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Name = "toolStripLabel1";
            resources.ApplyResources(this.toolStripLabel1, "toolStripLabel1");
            // 
            // TextBox_Prop
            // 
            this.TextBox_Prop.AutoCompleteCustomSource.AddRange(new string[] {
            resources.GetString("TextBox_Prop.AutoCompleteCustomSource"),
            resources.GetString("TextBox_Prop.AutoCompleteCustomSource1")});
            this.TextBox_Prop.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
            this.TextBox_Prop.DropDownWidth = 500;
            this.TextBox_Prop.Name = "TextBox_Prop";
            resources.ApplyResources(this.TextBox_Prop, "TextBox_Prop");
            // 
            // Button_GenerateStreetAutomata
            // 
            this.Button_GenerateStreetAutomata.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.Button_GenerateStreetAutomata.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            resources.ApplyResources(this.Button_GenerateStreetAutomata, "Button_GenerateStreetAutomata");
            this.Button_GenerateStreetAutomata.Name = "Button_GenerateStreetAutomata";
            this.Button_GenerateStreetAutomata.Click += new System.EventHandler(this.Button_GenerateStreetAutomata_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            resources.ApplyResources(this.toolStripSeparator1, "toolStripSeparator1");
            // 
            // Button_GenerateRabinAutomata
            // 
            this.Button_GenerateRabinAutomata.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.Button_GenerateRabinAutomata.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            resources.ApplyResources(this.Button_GenerateRabinAutomata, "Button_GenerateRabinAutomata");
            this.Button_GenerateRabinAutomata.Name = "Button_GenerateRabinAutomata";
            this.Button_GenerateRabinAutomata.Click += new System.EventHandler(this.Button_GenerateRabinAutomata_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            resources.ApplyResources(this.toolStripSeparator2, "toolStripSeparator2");
            // 
            // Button_GenerateBuchiAutomata
            // 
            this.Button_GenerateBuchiAutomata.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.Button_GenerateBuchiAutomata.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            resources.ApplyResources(this.Button_GenerateBuchiAutomata, "Button_GenerateBuchiAutomata");
            this.Button_GenerateBuchiAutomata.Name = "Button_GenerateBuchiAutomata";
            this.Button_GenerateBuchiAutomata.Click += new System.EventHandler(this.Button_Generate_Click);
            // 
            // BottomToolStripPanel
            // 
            resources.ApplyResources(this.BottomToolStripPanel, "BottomToolStripPanel");
            this.BottomToolStripPanel.Name = "BottomToolStripPanel";
            this.BottomToolStripPanel.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.BottomToolStripPanel.RowMargin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            // 
            // TopToolStripPanel
            // 
            resources.ApplyResources(this.TopToolStripPanel, "TopToolStripPanel");
            this.TopToolStripPanel.Name = "TopToolStripPanel";
            this.TopToolStripPanel.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.TopToolStripPanel.RowMargin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            // 
            // RightToolStripPanel
            // 
            resources.ApplyResources(this.RightToolStripPanel, "RightToolStripPanel");
            this.RightToolStripPanel.Name = "RightToolStripPanel";
            this.RightToolStripPanel.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.RightToolStripPanel.RowMargin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            // 
            // LeftToolStripPanel
            // 
            resources.ApplyResources(this.LeftToolStripPanel, "LeftToolStripPanel");
            this.LeftToolStripPanel.Name = "LeftToolStripPanel";
            this.LeftToolStripPanel.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.LeftToolStripPanel.RowMargin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            // 
            // ContentPanel
            // 
            resources.ApplyResources(this.ContentPanel, "ContentPanel");
            // 
            // LTL2AutoamataConverter
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.toolStripContainer);
            this.Name = "LTL2AutoamataConverter";
            this.ShowIcon = false;
            this.toolStripContainer.BottomToolStripPanel.ResumeLayout(false);
            this.toolStripContainer.BottomToolStripPanel.PerformLayout();
            this.toolStripContainer.ContentPanel.ResumeLayout(false);
            this.toolStripContainer.TopToolStripPanel.ResumeLayout(false);
            this.toolStripContainer.TopToolStripPanel.PerformLayout();
            this.toolStripContainer.ResumeLayout(false);
            this.toolStripContainer.PerformLayout();
            this.statusStrip2.ResumeLayout(false);
            this.statusStrip2.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.ToolStripPanel BottomToolStripPanel;
        private System.Windows.Forms.ToolStripPanel TopToolStripPanel;
        private System.Windows.Forms.ToolStripPanel RightToolStripPanel;
        private System.Windows.Forms.ToolStripPanel LeftToolStripPanel;
        private System.Windows.Forms.ToolStripContentPanel ContentPanel;
        private System.Windows.Forms.ToolStripContainer toolStripContainer;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private System.Windows.Forms.ToolStripComboBox TextBox_Prop;
        private System.Windows.Forms.ToolStripButton Button_GenerateBuchiAutomata;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel Label_IsSafety;
        private System.Windows.Forms.ToolStripStatusLabel Label_IsDeterministic;
        private System.Windows.Forms.ToolStripButton Button_GenerateStreetAutomata;
        private System.Windows.Forms.ToolStripButton Button_GenerateRabinAutomata;
        private System.Windows.Forms.StatusStrip statusStrip2;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel2;
        private System.Windows.Forms.ToolStripStatusLabel StatusLabel_Accept;
        public Microsoft.Msagl.GraphViewerGdi.GViewer gViewer;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel4;
        private System.Windows.Forms.ToolStripStatusLabel StatusLabel_Status;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel5;


    }
}