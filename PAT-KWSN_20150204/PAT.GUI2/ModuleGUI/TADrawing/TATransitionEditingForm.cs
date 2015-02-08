using System;
using System.Collections.Generic;
using System.Windows.Forms;
using PAT.Common.GUI.Drawing;
using PAT.GUI.Docking;
using Tools.Diagrams;

namespace PAT.GUI.TADrawing
{
    public partial class TATransitionEditingForm : Form
    {
        private const string LTSTabItem = "Labeled Transition Systems";
        EditorTabItem TextBox_ProgramTab = new EditorTabItem(LTSTabItem);
        private Route route;

        public TATransitionEditingForm(Route route, List<LTSCanvas.CanvasItemData> canvasItems)
        {
            InitializeComponent();

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


            //
            this.route = route;
            foreach (LTSCanvas.CanvasItemData itemData in canvasItems)
            {
                if (itemData.Item is StateItem)
                {
                    this.comboBoxSource.Items.Add(itemData.Item);
                    this.comboBoxDest.Items.Add(itemData.Item);

                    if (itemData.Item.Equals(route.From))
                    {
                        this.comboBoxSource.SelectedItem = itemData.Item;
                    }

                    if (itemData.Item.Equals(route.To))
                    {
                        this.comboBoxDest.SelectedItem = itemData.Item;
                    }
                }
            }


            //put in event, guard, program to dialogbox 
            this.TextBox_Select.Text = route.Transition.Select;
            this.TextBox_Event.Text = route.Transition.Event;

            this.TextBox_ClockGuard.Text = route.Transition.ClockGuard;
            this.TextBox_Guard.Text = route.Transition.Guard;
            this.TextBox_ProgramTab.Text = route.Transition.Program;
            this.TextBox_ResetClocks.Text = route.Transition.ClockReset;

            TextBox_ProgramTab.HideGoToDeclarition();

        }

        private void Button_OK_Click(object sender, EventArgs e)
        {
            this.route.From = (StateItem)this.comboBoxSource.SelectedItem;
            this.route.To = (StateItem)this.comboBoxDest.SelectedItem;
            this.route.Transition.Select = this.TextBox_Select.Text;
            this.route.Transition.Event = this.TextBox_Event.Text;
            this.route.Transition.Guard = this.TextBox_Guard.Text;
            this.route.Transition.ClockGuard = this.TextBox_ClockGuard.Text;
            this.route.Transition.Program = this.TextBox_ProgramTab.Text;
            this.route.Transition.ClockReset = this.TextBox_ResetClocks.Text;
        }
    }
}
