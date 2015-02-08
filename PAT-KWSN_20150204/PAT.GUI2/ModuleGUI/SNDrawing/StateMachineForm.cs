using System.Collections.Generic;
using System.Windows.Forms;
using PAT.Common.GUI.Drawing;

namespace PAT.GUI.SNDrawing
{
    public partial class StateMachineForm : Form
    {
        private string SensorName = "Sensor";
        private LTSCanvas Canvas;
        public List<LTSCanvas.CanvasItemData> SelectedItems = new List<LTSCanvas.CanvasItemData>();

        public StateMachineForm(string  sensorName, LTSCanvas canvas)
        {
            SensorName = sensorName;
            Canvas = canvas;
            //Canvas.Visible = true;
            //Canvas.Size = this.Size;
            //Canvas.AutoArrange();
            //this.Visible = true;
            this.Controls.Add(Canvas);
            InitializeComponent();
            this.Text = "State Machine of " + SensorName;
        }
    }

}
