using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PAT.Common.GUI.Drawing;
using System.Drawing;
using System.Xml;
using Tools.Diagrams;

namespace PAT.Common.GUI.PNModule
{
    public class PNLabel : CanvasItem
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")]
        public readonly Font TitleFont = new Font(FontFamily.GenericSansSerif, 14, FontStyle.Regular, GraphicsUnit.Pixel);

        protected override bool AllowHeightModifications()
        {
            return false;
        }

        public int Value { get; set; }

        public Color MyColor { get; set; }

        public const string LINK_LABEL = "Label";
        public PNLabel()
        {
            this.Height = this.TitleFont.Size + 5;
            this.Width = 25;
        }

        public override bool IsVResizable
        {
            get { return false; }
        }

        public override void DrawToGraphics(Graphics graphics)
        {
            string label = Value.ToString();
            if (!string.IsNullOrEmpty(label))
            {
                Color color = Color.Black;
                switch (this.CurrentState)
                {
                    case ItemState.Hover:
                        color = ColorDefinition.GetColorWhenHover();
                        break;

                    case ItemState.Selected:
                        color = ColorDefinition.GetColorWhenSelected();
                        break;

                    default:
                        break;
                } // switch

                if (this.CurrentState == ItemState.Free)
                    color = ColorDefinition.GetColorOfName();

                //Shorten label
                int allowedLength = 50;
                if (label.Length > allowedLength)
                    label = label.Substring(0, allowedLength) + "...";

                float width = graphics.MeasureString(label, this.TitleFont).Width;
                float height = this.TitleFont.Size + 5;

                RectangleF rect = new RectangleF(X, Y, width, height);
                graphics.DrawString(label, this.TitleFont, new SolidBrush(color), rect);
                if (Math.Abs(this.Width - width) > 15)
                    this.Width = width;
            }
        }

        public override bool HitTest(PointF pos, bool includeDecorators)
        {
            return (pos.X >= X && pos.Y >= Y && pos.X < X + this.Width && pos.Y < Y + this.Height);
        }

        public override void RemovedFromCanvas(LTSCanvas canvas)
        {
            PNArc route = this.FindSelectedRouteBasedOnTransition(canvas);
            route.RemovedFromCanvas(canvas);
        }

        public override void AddToCanvas(LTSCanvas canvas)
        {
            canvas.AddSingleCanvasItem(this);
        }

        public override void HandleMouseHoverIn(LTSCanvas canvas)
        {
            try
            {
                PNArc route = this.FindSelectedRouteBasedOnTransition(canvas);
                route.HandleMouseHoverIn(canvas);
            }
            catch { }
        }

        public override void HandleMouseHoverOut(LTSCanvas canvas)
        {
            try
            {
                PNArc route = this.FindSelectedRouteBasedOnTransition(canvas);
                route.HandleMouseHoverOut(canvas);
            }
            catch { }
        }

        public override void HandleSelected(LTSCanvas canvas)
        {
            PNArc route = this.FindSelectedRouteBasedOnTransition(canvas);
            route.CurrentState = ItemState.Selected;
        }

        /// <summary>
        /// Throw exception when route not found
        /// </summary>
        public PNArc FindSelectedRouteBasedOnTransition(LTSCanvas canvas)
        {
            foreach (Route route in canvas.diagramRouter.routes)
            {
                if (route is PNArc && (route as PNArc).Label.Equals(this))
                    return route as PNArc;
            }

            throw new Exception("Route not found");
        }

        protected override XmlElement CreateXmlElement(XmlDocument doc)
        {
            return doc.CreateElement(LINK_LABEL);
        }
    }
}
