using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Xml;
using Tools.Diagrams;
using System.Collections.Generic;
namespace PAT.Common.GUI.Drawing
{
    public class LabelItem : CanvasItem
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")]
        public readonly Font TitleFont = new Font(FontFamily.GenericSansSerif, 14, FontStyle.Regular, GraphicsUnit.Pixel);

        public List<string> labels = new List<string>();
        public List<Color> colors = new List<Color>();
        protected override bool AllowHeightModifications()
        {
            return false;
        }

        public const string STATE_LABEL = "Label";
        public LabelItem()
        {
            this.Height = this.TitleFont.Size + 5;
            this.Width = 50;
        }

        public override bool IsVResizable
        {
            get { return false; }
        }

        public override void DrawToGraphics(Graphics graphics)
        {
            RectangleF rect;
            float width = 0;
            float height = this.TitleFont.Size + 5;
            float currentWidth = 0;
            for (int i = 0; i < this.labels.Count; i++)
            {
                if (!string.IsNullOrEmpty(this.labels[i]))
                {
                    currentWidth = graphics.MeasureString(this.labels[i], this.TitleFont).Width;
                    rect = new RectangleF(X + width, Y, currentWidth, height);
                    graphics.DrawString(this.labels[i], this.TitleFont, new SolidBrush(this.colors[i]), rect);
                    width += currentWidth;
                }
            }
            if (Math.Abs(this.Width - width) > 5)
            {
                this.Width = width;
            }
        }

        public override bool HitTest(PointF pos, bool includeDecorators)
        {
            bool ret = (pos.X >= X && pos.Y >= Y && pos.X < X + this.Width && pos.Y < Y + this.Height);
            return ret;
        }

        public override void RemovedFromCanvas(LTSCanvas canvas)
        {
            StateItem state = this.FindStateBasedOnLabelItem(canvas);
            state.RemovedFromCanvas(canvas);
        }

        public override void AddToCanvas(LTSCanvas canvas)
        {
            StateItem state = this.FindStateBasedOnLabelItem(canvas);
            state.AddToCanvas(canvas);
        }

        public override void HandleMouseHoverIn(LTSCanvas canvas)
        {
            StateItem state = this.FindStateBasedOnLabelItem(canvas);
            state.HandleMouseHoverIn(canvas);
        }

        public override void HandleMouseHoverOut(LTSCanvas canvas)
        {
            StateItem state = this.FindStateBasedOnLabelItem(canvas);
            state.HandleMouseHoverOut(canvas);
        }

        public override void HandleSelected(LTSCanvas canvas)
        {
            StateItem state = this.FindStateBasedOnLabelItem(canvas);
            state.CurrentState = ItemState.Selected;
        }

        public StateItem FindStateBasedOnLabelItem(LTSCanvas canvas)
        {
            foreach (PAT.Common.GUI.Drawing.LTSCanvas.CanvasItemData item in canvas.itemsList)
            {
                if (item.Item is StateItem && (item.Item as StateItem).labelItems.Equals(this))
                {
                    return (item.Item as StateItem);
                }
            }
            throw new Exception("State not found");
        }

        public PAT.Common.GUI.Drawing.LTSCanvas.CanvasItemData FindStateDataBasedOnLabelItem(LTSCanvas canvas)
        {
            foreach (PAT.Common.GUI.Drawing.LTSCanvas.CanvasItemData item in canvas.itemsList)
            {
                if (item.Item is StateItem && (item.Item as StateItem).labelItems.Equals(this))
                {
                    return item;
                }
            }
            throw new Exception("State not found");
        }

        protected override XmlElement CreateXmlElement(XmlDocument doc)
        {
            return doc.CreateElement(STATE_LABEL);
        }
    }
}