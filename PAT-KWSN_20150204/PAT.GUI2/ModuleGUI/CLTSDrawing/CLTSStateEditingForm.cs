using System;
using System.Windows.Forms;
using PAT.Common.GUI.ERAModule;

namespace PAT.GUI.CLTSDrawing
{
    public partial class CLTSStateEditingForm : Form
    {
        //private const string LTSTabItem = "Timed Automata";

        private ERAState StateItem;
        //EditorTabItem TextBox_InvariantTab = new EditorTabItem(LTSTabItem);


        public CLTSStateEditingForm(ERAState stateItem)
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

            //TextBox_Invariant.Text = stateItem.Invariant;
            CheckBox_Error.Checked = stateItem.IsError;
            CheckBox_IsAcceptance.Checked = stateItem.IsAcceptance;

        }

        private void Button_OK_Click(object sender, EventArgs e)
        {
            StateItem.Name = TextBox_Name.Text;
            //StateItem.Invariant = TextBox_Invariant.Text;
            StateItem.IsError = CheckBox_Error.Checked;
            StateItem.IsAcceptance = CheckBox_IsAcceptance.Checked;

        }
    }
}
