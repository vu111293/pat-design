using System;
using System.IO;
using System.Windows.Forms;
using ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor;
using ICSharpCode.TextEditor.Document;
using PAT.Common.GUI.Drawing;
using PAT.Common.GUI.SNModule;

namespace PAT.GUI.SNDrawing
{
    public partial class NodeEditingForm : Form
    {
        private TreeNode SensorNode;
        private string NetworkPath;//the path where the sensor network application locates

        public NodeEditingForm(TreeNode sensorNode, string networkPath)
        {
            InitializeComponent();
            SensorNode = sensorNode;
            TextBox_Name.Text = sensorNode.Name;
            NetworkPath = Path.GetDirectoryName(networkPath);

            if(sensorNode.Tag != null)
            {
                var nodeData = sensorNode.Tag as NodeData;
                this.textBox_Application.Text = nodeData.TopConfiguration;
                this.TextBox_Sensors.Text = nodeData.SensorRanges;
                //this.TextBox_BufferSize.Text = nodeData.BufferSize;
                this.TextBox_ID.Text = nodeData.TOS_ID;
                this.TextBox_PredefinedVars.Text = nodeData.PredefinedVars;
            }
            else
            {
                this.textBox_Application.Text = "app";
                this.TextBox_Sensors.Text = "/* enter the sensor settings here... */";
                //this.TextBox_BufferSize.Text = "1";
                this.TextBox_ID.Text = "0";
            }
            
        }

        public void UpdateData()
        {
            SensorNode.Name = TextBox_Name.Text;
            SensorNode.Text = TextBox_Name.Text;

            var SensorNodeData = SensorNode.Tag as NodeData;
            SensorNodeData.Name = TextBox_Name.Text;
            SensorNodeData.TOS_ID = TextBox_ID.Text;
            SensorNodeData.TopConfiguration = textBox_Application.Text;
            SensorNodeData.SensorRanges = TextBox_Sensors.Text;
            //SensorNodeData.BufferSize = TextBox_BufferSize.Text;
            SensorNodeData.PredefinedVars = TextBox_PredefinedVars.Text;
        }


        private void textBox_Application_Click(object sender, EventArgs e)
        {
            try
            {
                TextBox textBox = sender as TextBox;

                string path = textBox.Text;

                OpenFileDialog opf = new OpenFileDialog();
                opf.Multiselect = false;
                opf.Filter = "NesC Model (*.nc)|*.nc|All File (*.*)|*.*";
                opf.Title = "Choose the NesC program for the sensor...";

                if (Path.IsPathRooted(path))
                    opf.InitialDirectory = path;
                else
                {
                    opf.InitialDirectory = NetworkPath;
                }

                if (opf.ShowDialog() == DialogResult.OK)
                {
                    this.textBox_Application.Text = GetRelativePath(opf.FileName);
            }
            }
            catch (Exception ex)
            {
                //Ultility.LogException(ex, null);
            }
        }

        private string GetRelativePath(string filePath)
        {
            if (NetworkPath == null)
                return filePath;

            int plength = NetworkPath.Length;
            int flength = filePath.Length;

            if (flength < plength)
                return filePath;

            if(filePath.Substring(0, plength).Equals(NetworkPath))
            {

                return filePath.Substring(plength+1, flength - plength - 1);
            }
            return filePath;
        }

        //private void Button_ChooseFile_Click(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        OpenFileDialog opf = new OpenFileDialog();
        //        opf.Multiselect = false;
        //        opf.Filter = "NesC Model (*.nc)|*.nc|All File (*.*)|*.*";
        //        if (opf.ShowDialog() == DialogResult.OK)
        //        {
        //            this.textBox_Application.Text = opf.FileName;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        //Ultility.LogException(ex, null);
        //    }
        //}
    }

    
}
