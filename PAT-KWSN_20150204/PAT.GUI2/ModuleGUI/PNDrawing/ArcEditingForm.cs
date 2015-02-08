using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using PAT.Common.GUI.PNModule;
using PAT.Common.GUI.Drawing;

namespace PAT.GUI.PNDrawing
{
    public partial class ArcEditingForm : Form
    {
        private PNArc _arc;

        public ArcEditingForm(PNArc arc)
        {
            InitializeComponent();
            _arc = arc;
            textBox3.Text = _arc.Weight.ToString();
            if (arc.From is PNPlace)
            {
                label1.Text = "From:";
            }

            textBox1.Text = (arc.From as StateItem).Name;

            if (arc.To is PNPlace)
            {
                label2.Text = "To:";
            }

            textBox2.Text = (arc.To as StateItem).Name;
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            int num;
            if (!(int.TryParse(textBox3.Text, out num) && num >= 1))
            {
                label4.Text = "Invalid weight";
                button1.Enabled = false;
            }
            else
            {
                label4.Text = string.Empty;
                button1.Enabled = true;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            _arc.Weight = Convert.ToInt32(textBox3.Text);
        }
    }
}
