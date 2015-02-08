using System;
using System.Windows.Forms;
using PAT.Common.GUI.Drawing;

namespace PAT.GUI.ModuleGUI.ILADrawing
{
    public partial class ILAStateEditingForm : Form
    {
        //private const string LTSTabItem = "Timed Automata";

        private StateItem StateItem;
        //EditorTabItem TextBox_InvariantTab = new EditorTabItem(LTSTabItem);


        public ILAStateEditingForm(StateItem stateItem)
        {
            InitializeComponent();
            StateItem = stateItem;
            TextBox_Name.Text = stateItem.Name;

            //// 
            //// TextBox_Guard
            //// 
            //TextBox_InvariantTab.Controls.Clear();
            //this.TextBox_InvariantTab.CodeEditor.Dock = DockStyle.None;
            //this.TextBox_InvariantTab.CodeEditor.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            //            | System.Windows.Forms.AnchorStyles.Right)));
            //this.TextBox_InvariantTab.CodeEditor.Location = new System.Drawing.Point(61, 58);
            //this.TextBox_InvariantTab.CodeEditor.Name = "TextBox_Guard";
            //this.TextBox_InvariantTab.CodeEditor.Size = new System.Drawing.Size(317, 91);
            //this.TextBox_InvariantTab.CodeEditor.TabIndex = 2;
            //this.Controls.Add(TextBox_InvariantTab.CodeEditor);

            TextBox_Invariant.Text = stateItem.Invariant;
            //CheckBox_Commited.Checked = stateItem.IsCommitted;
            CheckBox_IsAcceptance.Checked = stateItem.IsCommitted;

        }

        private void Button_OK_Click(object sender, EventArgs e)
        {
            StateItem.Name = TextBox_Name.Text;
            StateItem.Invariant = TextBox_Invariant.Text;
            StateItem.IsCommitted = CheckBox_IsAcceptance.Checked;
           // StateItem.IsUrgent = CheckBox_IsAcceptance.Checked;
        }

        private void CheckBox_IsUrgent_CheckStateChanged(object sender, EventArgs e)
        {
            if(CheckBox_IsAcceptance.Checked)
            {
                CheckBox_Commited.Checked = false;
            }
        }

        private void CheckBox_Commited_CheckedStateChanged(object sender, EventArgs e)
        {
            if (CheckBox_Commited.Checked)
            {
                CheckBox_IsAcceptance.Checked = false;
            }
        }
    }
}
