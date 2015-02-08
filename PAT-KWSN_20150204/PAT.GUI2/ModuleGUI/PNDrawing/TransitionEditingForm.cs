using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using PAT.Common.GUI.PNModule;
using PAT.GUI.Docking;

namespace PAT.GUI.PNDrawing
{
    public partial class TransitionEditingForm : Form
    {
        private PNTransition _transition;
        private const string PNTabItem = "Petri nets";
        EditorTabItem TextBox_ProgramTab = new EditorTabItem(PNTabItem);

        public TransitionEditingForm(PNTransition transition)
        {
            InitializeComponent();
            _transition = transition;
            textBox1.Text = _transition.Name;

            this.TextBox_ProgramTab.CodeEditor.Dock = DockStyle.None;
            this.TextBox_ProgramTab.CodeEditor.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.TextBox_ProgramTab.CodeEditor.Location = new System.Drawing.Point(71, 274);
            this.TextBox_ProgramTab.CodeEditor.Name = "TextBox_Program";
            this.TextBox_ProgramTab.CodeEditor.Size = new System.Drawing.Size(365, 180);
            this.TextBox_ProgramTab.CodeEditor.TabIndex = 11;
            this.Controls.Add(TextBox_ProgramTab.CodeEditor);

            TextBox_ProgramTab.HideGoToDeclarition();

            StringBuilder sb = new StringBuilder();
            foreach (PNPlace place in _transition.InputPlaces)
            {
                sb.AppendLine(place.Name);
            }
            txtInputPlaces.Text = sb.ToString();

            sb = new StringBuilder();
            foreach (PNPlace place in _transition.OutputPlaces)
            {
                sb.AppendLine(place.Name);
            }
            txtOutputPlaces.Text = sb.ToString();
            txtGuard.Text = _transition.Guard;
            TextBox_ProgramTab.Text = _transition.Program;

        }

        private void button1_Click(object sender, EventArgs e)
        {
            _transition.Name = textBox1.Text;
            _transition.Guard = txtGuard.Text;
            _transition.Program = TextBox_ProgramTab.Text;
        }
    }
}
