using System;
using System.Windows.Forms;

namespace PAT.GUI.Forms
{
    public enum MemoryBoxResult { Yes, YesToAll, No, NoToAll, Cancel }

    public partial class MemoryBox : Form
    {
        // Internal values
        MemoryBoxResult lastResult = MemoryBoxResult.Cancel;
        MemoryBoxResult result = MemoryBoxResult.Cancel;
        
        // Enums
        // Results

        /// <summary>
        /// The default constructor for MemoryBox.
        /// </summary>
        public MemoryBox()
        {
            InitializeComponent();
        }

        #region Properties
        public String LabelText
        {
            get { return this.Label_Message.Text; }
            set
            {
                this.Label_Message.Text = value;
                UpdateSize();
            }
        }

        public MemoryBoxResult Result
        {
            get { return this.result; }
            set { this.result = value; }
        }
        
        #endregion

        #region Public Methods
        /// <summary>
        /// Call this function instead of ShowDialog, to check for remembered
        /// result.
        /// </summary>
        /// <returns></returns>
        public MemoryBoxResult ShowMemoryDialog()
        {
            result = MemoryBoxResult.Cancel;
            if (lastResult == MemoryBoxResult.NoToAll)
            {
                result = MemoryBoxResult.No;
            }
            else if (lastResult == MemoryBoxResult.YesToAll)
            {
                result = MemoryBoxResult.Yes;
            }
            else
            {
                base.ShowDialog();
            }
            return result;
        }

        public MemoryBoxResult ShowMemoryDialog(String label, string title)
        {
            this.Text = title;
            LabelText = label;
            return ShowMemoryDialog();
        }

        public MemoryBoxResult ShowCloseMemoryDialog(String documentID)
        {
            this.Text = Common.Ultility.Ultility.APPLICATION_NAME;
            this.Label_Message.Text = String.Format(this.Label_Message.Text, documentID);
            return ShowMemoryDialog();
        }

        public MemoryBoxResult ShowCloseMemoryDialog(String documentID, bool hide)
        {
            this.Text = Common.Ultility.Ultility.APPLICATION_NAME;
            this.Label_Message.Text = String.Format(this.Label_Message.Text, documentID);
            this.Button_NotoAll.Visible = false;
            return ShowMemoryDialog();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// This call updates the size of the window based on certain factors,
        /// such as if an icon is present, and the size of label.
        /// </summary>
        private void UpdateSize()
        {
            int newWidth = Label_Message.Size.Width + 40;

            // Add the width of the icon, and some padding.
            //if (pictureBoxIcon.Image != null)
            //{
            //    newWidth += pictureBoxIcon.Width + 20;
            //    labelBody.Location = new Point(118, labelBody.Location.Y);
            //}
            //else
            //{
            //    labelBody.Location = new Point(12, labelBody.Location.Y);
            ////}
            //if (newWidth >= 440)
            //{
            //    this.Width = newWidth;
            //}
            //else
            //{
            //    this.Width = 440;
            //}

            //int newHeight = labelBody.Size.Height + 100;
            //if (newHeight >= 200)
            //{
            //    this.Height = newHeight;
            //}
            //else
            //{
            //    this.Height = 200;
            //}
        }

        #endregion

        private void buttonYes_Click(object sender, EventArgs e)
        {
            result = MemoryBoxResult.Yes;
            lastResult = MemoryBoxResult.Yes;
            DialogResult = DialogResult.Yes;
        }

        private void buttonYestoAll_Click(object sender, EventArgs e)
        {
            result = MemoryBoxResult.Yes;
            lastResult = MemoryBoxResult.YesToAll;
            DialogResult = DialogResult.Yes;
        }

        private void buttonNo_Click(object sender, EventArgs e)
        {
            result = MemoryBoxResult.No;
            lastResult = MemoryBoxResult.No;
            DialogResult = DialogResult.No;
        }

        private void buttonNotoAll_Click(object sender, EventArgs e)
        {
            result = MemoryBoxResult.NoToAll;
            lastResult = MemoryBoxResult.NoToAll;
            DialogResult = DialogResult.No;
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            result = MemoryBoxResult.Cancel;
            lastResult = MemoryBoxResult.Cancel;
            DialogResult = DialogResult.Cancel;
        }
    }
}