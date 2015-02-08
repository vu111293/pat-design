using System;
using System.Windows.Forms;
using PAT.Common.GUI.Drawing;
using PAT.GUI.Docking;
using Tools.Diagrams;
using System.Collections.Generic;

namespace PAT.GUI.LTSDrawing
{
    public partial class TransitionEditingForm : Form
    {
        private const string LTSTabItem = "Labeled Transition Systems";
        EditorTabItem TextBox_ProgramTab = new EditorTabItem(LTSTabItem);
        private Route route;

        public TransitionEditingForm(Route route, List<LTSCanvas.CanvasItemData> canvasItems)
        {
            InitializeComponent();

            // TextBox_Program
            TextBox_ProgramTab.Controls.Clear();
            this.TextBox_ProgramTab.CodeEditor.Dock = DockStyle.None;
            this.TextBox_ProgramTab.CodeEditor.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.TextBox_ProgramTab.CodeEditor.Location = new System.Drawing.Point(61, 206);
            this.TextBox_ProgramTab.CodeEditor.Name = "TextBox_Program";
            this.TextBox_ProgramTab.CodeEditor.Size = new System.Drawing.Size(365, 180);
            this.TextBox_ProgramTab.CodeEditor.TabIndex = 11;
            this.Controls.Add(TextBox_ProgramTab.CodeEditor);

            TextBox_ProgramTab.HideGoToDeclarition();

            this.route = route;
            foreach (LTSCanvas.CanvasItemData itemData in canvasItems)
            {
                if (itemData.Item is StateItem)
                {
                    this.comboBoxSource.Items.Add(itemData.Item);
                    this.comboBoxDest.Items.Add(itemData.Item);

                    if (itemData.Item.Equals(route.From))
                        this.comboBoxSource.SelectedItem = itemData.Item;

                    if (itemData.Item.Equals(route.To))
                        this.comboBoxDest.SelectedItem = itemData.Item;
                }
            }

            this.TextBox_Select.Text = route.Transition.Select;
            this.TextBox_Guard.Text = route.Transition.Guard;
            this.TextBox_Event.Text = route.Transition.Event;
            this.TextBox_ProgramTab.Text = route.Transition.Program;
        }

        private void Button_OK_Click(object sender, EventArgs e)
        {
            this.route.From = (StateItem)this.comboBoxSource.SelectedItem;
            this.route.To = (StateItem)this.comboBoxDest.SelectedItem;
            this.route.Transition.Guard = this.TextBox_Guard.Text;
            this.route.Transition.Event = this.TextBox_Event.Text;
            this.route.Transition.Program = this.TextBox_ProgramTab.Text;
            this.route.Transition.Select = this.TextBox_Select.Text;
        }
    }
}
