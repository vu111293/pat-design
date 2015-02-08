using System;
using System.Diagnostics;
using System.Windows.Forms;
using Antlr.Runtime;
using PAT.Common.Classes.BA;
using PAT.Common.Classes.LTL2DRA;

namespace PAT.Common.GUI
{
    public partial class LTL2AutoamataConverter : Form
    {
        private string options = "";
        Stopwatch timer = new Stopwatch();
        public LTL2AutoamataConverter(string ltl, string opts)
        {
            InitializeComponent();
            this.Label_IsSafety.Text = "";
            this.Label_IsDeterministic.Text = "";
            this.StatusLabel_Status.Text = "";
            options = opts;
            if (!string.IsNullOrEmpty(ltl))
            {
                TextBox_Prop.Text = ltl;
                Button_GenerateBuchiAutomata.PerformClick();
            }
        }

        private void EnableDisableUI(bool isEnable)
        {
            if (isEnable)
            {
                Button_GenerateBuchiAutomata.Enabled = isEnable;
                this.Button_GenerateRabinAutomata.Enabled = isEnable;
                this.Button_GenerateStreetAutomata.Enabled = isEnable;
                this.TextBox_Prop.Enabled = isEnable;
            }
            else
            {
                this.Label_IsSafety.Text = "";
                this.Label_IsDeterministic.Text = "";
            }
        }

        private void Button_Generate_Click(object sender, System.EventArgs e)
        {
            try
            {
                EnableDisableUI(false);
                timer.Reset();
                timer.Start();
                BuchiAutomata BA = LTL2BA.FormulaToBA(TextBox_Prop.Text, options, new CommonToken(0, ""));
                timer.Stop();

                gViewer.Graph = LTL2BA.AutomatonToDot(BA);
                gViewer.AutoSize = true;

                //this.StatusLabel_Status.Text = "Büchi Automata Generated with " + (gViewer.Graph.NodeCount - 1) + " Nodes " + (gViewer.Graph.EdgeCount - 1) + " Edges (checked in " + timer.Elapsed.TotalSeconds + "s).";
                this.StatusLabel_Status.Text = string.Format(Resources.Büchi_Automata_Generated_with__0__Nodes__1__Edges__checked_in__2__s__, (gViewer.Graph.NodeCount - 1), (gViewer.Graph.EdgeCount - 1), timer.Elapsed.TotalSeconds);
                this.StatusLabel_Status.Tag = timer.Elapsed.TotalSeconds;
                

                timer.Reset();
                timer.Start();
                bool islive = LivenessChecking.isLiveness(BA);
                timer.Stop();

                this.Label_IsSafety.Tag = timer.Elapsed.TotalSeconds;

                if (!islive)
                {
                    this.Label_IsSafety.Text = string.Format(Resources.The_formula_is_a_safety_property__checked_in__0__s__, timer.Elapsed.TotalSeconds);
                }
                
                if (this.TextBox_Prop.Items.Contains(TextBox_Prop.Text))
                {
                    this.TextBox_Prop.Items.Add(TextBox_Prop.Text);
                }

                StatusLabel_Accept.Visible = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(Resources.The_input_LTL_is_not_correct__ + ex.Message, PAT.Common.Ultility.Ultility.APPLICATION_NAME, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            EnableDisableUI(true);
        }

        private void Button_GenerateRabinAutomata_Click(object sender, EventArgs e)
        {
            GenerateDRAGraph(true);

        }

        private void Button_GenerateStreetAutomata_Click(object sender, EventArgs e)
        {
            GenerateDRAGraph(false);
        }

        private void GenerateDRAGraph(bool isRabin)
        {
            try
            {
                EnableDisableUI(false);

                string ltl = TextBox_Prop.Text;
                if (!isRabin)
                {
                    ltl = "! " + ltl;
                }
                BuchiAutomata BA = LTL2BA.FormulaToBA(ltl, options, new CommonToken(0, ""));
                ltl2ba.Node LTLHeadNode = LTL2BA.ParseLTL(ltl, options, new CommonToken(0, ""));

                timer.Reset();
                timer.Start();
                bool islive = LivenessChecking.isLiveness(BA);
                timer.Stop();

                this.Label_IsSafety.Tag = timer.Elapsed.TotalSeconds;

                if (!islive)
                {
                    //this.Label_IsSafety.Text = "The formula is a safety property (checked in " + timer.Elapsed.TotalSeconds + "s).";
                    this.Label_IsSafety.Text = string.Format(Resources.The_formula_is_a_safety_property__checked_in__0__s__, timer.Elapsed.TotalSeconds);

                }
                timer.Reset();
                timer.Start();
                DRA dra = BA2DRAConverter.ConvertBA2DRA(BA, LTLHeadNode);
                timer.Stop();

                gViewer.Graph = dra.AutomatonToDot();
                gViewer.AutoSize = true;

                this.StatusLabel_Status.Tag = timer.Elapsed.TotalSeconds;

                if (isRabin)
                {
                    this.StatusLabel_Status.Text = string.Format(Resources.Rabin_Automata_Generated_with__0__Nodes__1__Edges__checked_in__2_s__, (gViewer.Graph.NodeCount - 1), (gViewer.Graph.EdgeCount - 1), timer.Elapsed.TotalSeconds);
                }
                else
                {
                    this.StatusLabel_Status.Text = string.Format(Resources.Streett_Automata_Generated_with__0__Nodes__1__Edges__checked_in__2_s__, (gViewer.Graph.NodeCount - 1), (gViewer.Graph.EdgeCount - 1), timer.Elapsed.TotalSeconds);
                }
                StatusLabel_Accept.Visible = false;

                if (this.TextBox_Prop.Items.Contains(TextBox_Prop.Text))
                {
                    this.TextBox_Prop.Items.Add(TextBox_Prop.Text);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(Resources.The_input_LTL_is_not_correct__ + ex.Message, PAT.Common.Ultility.Ultility.APPLICATION_NAME, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            EnableDisableUI(true);
        }


        //private void Label_IsSafety_DoubleClick(object sender, EventArgs e)
        //{
        //    Clipboard.SetDataObject((sender as ToolStripStatusLabel).Tag, true);
        //}



    }
}