using System.Windows.Forms;
using PAT.Common.GUI.Drawing;
using PAT.Common.GUI.SNModule;

namespace PAT.GUI.SNDrawing
{
    partial class StateMachineForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();

            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            Canvas = new PAT.Common.GUI.Drawing.LTSCanvas();
            this.SuspendLayout();
            // 
            // Canvas
            // 
            this.Canvas.AllowDrop = true;
            this.Canvas.AutoScroll = true;
            this.Canvas.BackColor = System.Drawing.Color.White;
            this.Canvas.Cursor = System.Windows.Forms.Cursors.Default;
            this.Canvas.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Canvas.Location = new System.Drawing.Point(0, 0);
            this.Canvas.Name = "Canvas";
            this.Canvas.Size = new System.Drawing.Size(1024, 728);
            this.Canvas.TabIndex = 0;
            this.Canvas.Zoom = 1F;
            this.Canvas.Visible = true;
            this.Visible = true;
            this.Controls.Add(Canvas);
            this.Canvas.ItemDoubleClick += Canvas_ItemDoubleClick;
            this.Canvas.RouteDoubleClick += Canvas_RouteDoubleClick;

            // 
            // StateMachineForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1024, 728);
            this.Margin = new System.Windows.Forms.Padding(1, 1, 1, 1);
            this.Name = "StateMachineForm";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "State Machine Sensor ";
            this.ResumeLayout(false);
        }

        #endregion

        private void Canvas_ItemDoubleClick(object sender, CanvasItemEventArgs e)
        {
            if (e.CanvasItem.Item is StateItem)
            {//a state node is clicked
                StateItem editedState = e.CanvasItem.Item as StateItem;
                string sName = editedState.Name;//the name of the sensor clicked on the canvas

                MessageBox.Show("The state clicked is " + sName, "State information",
                      MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void Canvas_RouteDoubleClick(object sender, CanvasRouteEventArgs e)
        {
            if (e.CanvasItem.From is StateItem && e.CanvasItem.To is StateItem)
            {
                var from = (StateItem)e.CanvasItem.From;
                var to = (StateItem)e.CanvasItem.To;
                MessageBox.Show("The transition clicked is " + e.CanvasItem.Transition.ToSpecificationString(),
                    "Transition " + from.Name + " -> " + to.Name, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }
    }
}