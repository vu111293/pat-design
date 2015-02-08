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
    public partial class ModelEditingForm : Form
    {
        private PNCanvas _canvas;

        public ModelEditingForm()
        {
            InitializeComponent();
        }

        public ModelEditingForm(PNCanvas canvas):this()
        {
            _canvas = canvas;
            textBox1.Text = _canvas.Node.Text;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            
        }

        internal void UpdateData()
        {
            _canvas.Node.Text = textBox1.Text;
        }
    }
}
