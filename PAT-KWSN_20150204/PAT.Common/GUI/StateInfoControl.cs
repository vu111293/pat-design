using System;
using System.Drawing;
using System.Windows.Forms;

namespace PAT.Common.GUI
{
    public partial class StateInfoControl : UserControl
    {
        public StateInfoControl()
        {
            InitializeComponent();
            TextBox_Info.TextChanged += new EventHandler(TextBox_DataPane_TextChanged);
            this.splitContainer1.Panel2Collapsed = true;
        }

        public void SetText(string text, Bitmap image)
        {
            this.TextBox_Info.Text = text;
            
            if(image != null)
            {
                int distance = this.Height - this.Width - 15;
                if(distance > 0)
                {
                    this.splitContainer1.SplitterDistance = distance;
                }
                this.PictureBox_Image.Image = image;
                this.splitContainer1.Panel2Collapsed = false;
            }
            else
            {
                this.splitContainer1.Panel2Collapsed = true;    
            }
            
        }

        void TextBox_DataPane_TextChanged(object sender, EventArgs e)
        {
            TextBox_Info.Font = new Font(TextBox_Info.Font.FontFamily, 8, FontStyle.Regular);
            TextBox_Info.SelectAll();
            TextBox_Info.SelectionFont = TextBox_Info.Font;
            TextBox_Info.SelectionStart = 0;
            TextBox_Info.SelectionLength = 0;

        }

    }
}
