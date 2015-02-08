using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;
using PAT.Common.GUI;

namespace PAT.GUI.Forms.GraphDiff
{
    public partial class GraphDifferenceForm : Form
    {
        public GraphDifferenceForm()
        {
            InitializeComponent();

            List<SimulationForm> forms = Common.Ultility.Ultility.SimulationForms;

            this.CheckedListBox_Forms.Items.Clear();
            int counter = 0;
            foreach (SimulationForm form in forms)
            {
                this.CheckedListBox_Forms.Items.Add("Graph " + counter + ": " + form.Text, true);
                counter++;               
            }
        }

        private void CheckedListBox_Forms_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (e.NewValue == CheckState.Checked)
            {
                if (this.CheckedListBox_Forms.CheckedItems.Count >= 2)
                {
                    e.NewValue = CheckState.Unchecked;
                }


                if (this.CheckedListBox_Forms.CheckedItems.Count == 1 && e.NewValue == CheckState.Checked || this.CheckedListBox_Forms.CheckedItems.Count == 2 && e.NewValue == CheckState.Unchecked)
                {
                    Button_Compare.Enabled = true;
                }
                else
                {
                    Button_Compare.Enabled = false;
                }
            }
            else
            {
                Button_Compare.Enabled = false;
            }
        }

        private void Button_Compare_Click(object sender, System.EventArgs e)
        {
            Debug.Assert(this.CheckedListBox_Forms.CheckedItems.Count == 2);

            SimulationForm FirstGraph = Common.Ultility.Ultility.SimulationForms[this.CheckedListBox_Forms.CheckedIndices[0]];
            SimulationForm SecondGraph = Common.Ultility.Ultility.SimulationForms[this.CheckedListBox_Forms.CheckedIndices[1]];

            if (FirstGraph.SimulatorViewer.Graph != null && SecondGraph.SimulatorViewer.Graph != null)
            {
                DifferenceDisplayForm form = new DifferenceDisplayForm(FirstGraph.SimulatorViewer.Graph, SecondGraph.SimulatorViewer.Graph, FirstGraph.Spec, SecondGraph.Spec);
                form.Text = form.Text + " (" + this.CheckedListBox_Forms.CheckedItems[0].ToString() + " vs " + this.CheckedListBox_Forms.CheckedItems[1].ToString() + ")";
                form.Show(this.Parent);

                this.Close();
            }
        }
    }
}
