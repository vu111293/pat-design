using System;
using System.Windows.Forms;
using PAT.Common.GUI.Drawing;
using PAT.GUI.Docking;

namespace PAT.GUI.ModuleGUI.ILADrawing
{
    public partial class ILATransitionEditingForm : Form
    {
        private const string LTSTabItem = "Labeled Transition Systems";
        private Transition transition;

        EditorTabItem TextBox_ProgramTab = new EditorTabItem(LTSTabItem);
        //EditorTabItem TextBox_GuardTab = new EditorTabItem(LTSTabItem);
        //EditorTabItem TextBox_ClockGuardTab = new EditorTabItem(LTSTabItem);

        public ILATransitionEditingForm(Transition transition, string source, string target)
        {
            InitializeComponent();
            this.transition = transition;
            this.TextBox_Source.Text = source;
            this.TextBox_Target.Text = target;

            //put in event, guard, program to dialogbox 
            this.TextBox_Select.Text = this.transition.Select;
            this.TextBox_Event.Text = this.transition.Event;

            //// 
            //// TextBox_Guard
            //// 
            //TextBox_ClockGuardTab.Controls.Clear();
            //this.TextBox_ClockGuardTab.CodeEditor.Dock = DockStyle.None;
            //this.TextBox_ClockGuardTab.CodeEditor.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            //            | System.Windows.Forms.AnchorStyles.Right)));
            //this.TextBox_ClockGuardTab.CodeEditor.Location = new System.Drawing.Point(80, 105);
            //this.TextBox_ClockGuardTab.CodeEditor.Name = "TextBox_Guard";
            //this.TextBox_ClockGuardTab.CodeEditor.Size = new System.Drawing.Size(346, 43);
            //this.TextBox_ClockGuardTab.CodeEditor.TabIndex = 2;
            //this.Controls.Add(TextBox_ClockGuardTab.CodeEditor);

            //// 
            //// TextBox_Guard
            //// 
            //TextBox_GuardTab.Controls.Clear();
            //this.TextBox_GuardTab.CodeEditor.Dock = DockStyle.None;
            //this.TextBox_GuardTab.CodeEditor.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            //            | System.Windows.Forms.AnchorStyles.Right)));
            //this.TextBox_GuardTab.CodeEditor.Location = new System.Drawing.Point(80, 156);
            //this.TextBox_GuardTab.CodeEditor.Name = "TextBox_Guard";
            //this.TextBox_GuardTab.CodeEditor.Size = new System.Drawing.Size(346, 41);
            //this.TextBox_GuardTab.CodeEditor.TabIndex = 2;
            //this.Controls.Add(TextBox_GuardTab.CodeEditor);



            // 
            // TextBox_Program
            // 
            TextBox_ProgramTab.Controls.Clear();
            this.TextBox_ProgramTab.CodeEditor.Dock = DockStyle.None;
            this.TextBox_ProgramTab.CodeEditor.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.TextBox_ProgramTab.CodeEditor.Location = new System.Drawing.Point(80, 227);
            this.TextBox_ProgramTab.CodeEditor.Name = "TextBox_Program";
            this.TextBox_ProgramTab.CodeEditor.Size = new System.Drawing.Size(346, 160);
            this.TextBox_ProgramTab.CodeEditor.TabIndex = 3;
            this.Controls.Add(TextBox_ProgramTab.CodeEditor);

            this.TextBox_ClockGuard.Text = this.transition.ClockGuard;
            this.TextBox_Guard.Text = this.transition.Guard;
            this.TextBox_ProgramTab.Text = this.transition.Program;
            //this.TextBox_ResetClocks.Text = this.transition.ClockReset;

            TextBox_ProgramTab.HideGoToDeclarition();

        }

        private void Button_OK_Click(object sender, EventArgs e)
        {
            transition.Select = this.TextBox_Select.Text;
            transition.Event = this.TextBox_Event.Text;
            transition.Guard = this.TextBox_Guard.Text;
            transition.ClockGuard = this.TextBox_ClockGuard.Text;
            transition.Program = this.TextBox_ProgramTab.Text;
            //transition.ClockReset = this.TextBox_ResetClocks.Text;

            this.Close();
        }
    }
}
