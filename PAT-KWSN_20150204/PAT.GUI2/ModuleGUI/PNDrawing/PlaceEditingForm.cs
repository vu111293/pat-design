using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using PAT.Common.GUI.PNModule;

namespace PAT.GUI.PNDrawing
{
    public partial class PlaceEditingForm : Form
    {
        private PNPlace _place;
        private bool check1 = true;
        private bool check2 = true;

        public PlaceEditingForm(PNPlace place)
        {
            InitializeComponent();
            _place = place;
            textBox1.Text = _place.Name;
            textBox2.Text = _place.Capacity.ToString();
            textBox3.Text = _place.NumberOfTokens.ToString();
            TextBox_Guard.Text = _place.Guard;
            label4.ForeColor = Color.Black;
            label4.Text = "if Capacity is 0, the number of tokens is unlimited";
        }

        public string NewName
        {
            get
            {
                return textBox1.Text;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            _place.Name = textBox1.Text;
            _place.Capacity = Convert.ToInt32(textBox2.Text);
            _place.NumberOfTokens = Convert.ToInt32(textBox3.Text);
            _place.Guard = TextBox_Guard.Text;
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            int num;
            if (!(int.TryParse(textBox2.Text, out num) && num >= 0))
            {
                label4.ForeColor = Color.Red;
                label4.Text = "Invalid capacity";
                check1 = false;
            }
            else
            {
                label4.ForeColor = Color.Black;
                label4.Text = "if Capacity is 0, the number of tokens is unlimited";
                check1 = true;
            }
            button1.Enabled = check1 & check2;
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            int num;
            if (!(int.TryParse(textBox3.Text, out num) && num >= 0))
            {
                label4.ForeColor = Color.Red;
                label4.Text = "Invalid Number of tokens";
                check2 = false;
            }
            else
            {
                if (num > _place.Capacity && _place.Capacity != 0)
                {
                    label4.ForeColor = Color.Red;
                    label4.Text = "number of tokens must less than Capacity";
                    check2 = false;
                }
                else
                {
                    label4.ForeColor = Color.Black;
                    label4.Text = "if Capacity is 0, the number of tokens is unlimited";
                    check2 = true;
                }
            }
            button1.Enabled = check1 & check2;
        }
    }
}
