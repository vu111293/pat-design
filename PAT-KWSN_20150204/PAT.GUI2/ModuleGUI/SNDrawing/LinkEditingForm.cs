using System;
using System.Windows.Forms;
using PAT.Common.GUI.Drawing;
using PAT.GUI.Docking;

namespace PAT.GUI.SNDrawing
{
    public partial class LinkEditingForm : Form
    {
        private const string SNTabItem = "Sensor Network Systems";
        private Transition transition;
        //EditorTabItem TextBox_GuardTab = new EditorTabItem(LTSTabItem);
        EditorTabItem TextBox_ProgramTab = new EditorTabItem(SNTabItem);
        public LinkEditingForm(Transition transition, string source, string target)
        {
            InitializeComponent();
            this.transition = transition;
            this.TextBox_Source.Text = source;
            this.TextBox_Target.Text = target;

            //put in event, guard, program to dialogbox 
            //this.TextBox_Select.Text = this.transition.Select;
            this.TextBox_Bandwidth.Text = this.transition.Event;

            //// 
            //// TextBox_LossRate
            //// 
            //TextBox_GuardTab.Controls.Clear();
            //this.TextBox_GuardTab.CodeEditor.Dock = DockStyle.None;
            //this.TextBox_GuardTab.CodeEditor.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            //            | System.Windows.Forms.AnchorStyles.Right)));
            //this.TextBox_GuardTab.CodeEditor.Location = new System.Drawing.Point(61, 129);
            //this.TextBox_GuardTab.CodeEditor.Name = "TextBox_LossRate";
            //this.TextBox_GuardTab.CodeEditor.Size = new System.Drawing.Size(365, 62);
            //this.TextBox_GuardTab.CodeEditor.TabIndex = 2;
            //this.Controls.Add(TextBox_GuardTab.CodeEditor);

            // 
            // TextBox_Note
            // 
            TextBox_ProgramTab.Controls.Clear();
            this.TextBox_ProgramTab.CodeEditor.Dock = DockStyle.None;
            this.TextBox_ProgramTab.CodeEditor.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.TextBox_ProgramTab.CodeEditor.Location = new System.Drawing.Point(61, 208);
            this.TextBox_ProgramTab.CodeEditor.Name = "TextBox_Note";
            this.TextBox_ProgramTab.CodeEditor.Size = new System.Drawing.Size(365, 180);
            this.TextBox_ProgramTab.CodeEditor.TabIndex = 3;
            this.Controls.Add(TextBox_ProgramTab.CodeEditor);

            this.TextBox_LossRate.Text = this.transition.Guard;
            this.TextBox_ProgramTab.Text = this.transition.Program;
            TextBox_ProgramTab.HideGoToDeclarition();

        }

        private void Button_OK_Click(object sender, EventArgs e)
        {
            //transition.Select = this.TextBox_Select.Text;
            transition.Event = this.TextBox_Bandwidth.Text;
            transition.Guard = this.TextBox_LossRate.Text;
            transition.Program = this.TextBox_ProgramTab.Text;
            this.Close();
        }

        private void TextBox_Source_TextChanged(object sender, EventArgs e)
        {

        }

        private void Button_Cancel_Click(object sender, EventArgs e)
        {

        }
    }
}
