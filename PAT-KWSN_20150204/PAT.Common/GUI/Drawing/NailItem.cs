using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Xml;
using Tools.Diagrams;

namespace PAT.Common.GUI.Drawing
{
    public class NailItem : CanvasItem
    {
        protected override bool AllowHeightModifications()
        {
            return false;
        }

        public const float R = 5;

        //to know whether its route is selected, and remove it from route when it is deleted
        private Route route;

        public Route Route
        {
            get { return route; }
        }

        public const string NAIL_NODE_NAME = "Nail";
        #region Constructors
        public NailItem(Route route)
        {
            this.route = route;
            Height = R * 2;
            Width = R * 2;
        }
        #endregion

        public override bool IsVResizable
        {
            get { return false; }
        }

        #region Graphics related members
        protected virtual bool RoundedCorners
        {
            get { return true; }
        }

        protected virtual int CornerRadius
        {
            get { return 15; }
        }
        #endregion

        public override void DrawToGraphics(Graphics graphics)
        {
            RectangleF EllipseFill = new RectangleF(AbsoluteX, AbsoluteY, Width, Height);
            Brush pthGrBrush = new SolidBrush(Color.Yellow);

            graphics.FillEllipse(pthGrBrush, EllipseFill);
            graphics.DrawEllipse(Pens.Black, EllipseFill);
            base.DrawToGraphics(graphics);
        }

        #region Storage
        protected override XmlElement CreateXmlElement(XmlDocument doc)
        {
            return doc.CreateElement(NAIL_NODE_NAME);
        }
        #endregion

        public override bool HitTest(PointF pos, bool includeDecorators)
        {
            bool ret = (pos.X >= X && pos.Y >= Y && pos.X < X + Width && pos.Y < Y + Height);
            return ret;
        }

        public void RemoveFromCurrentRoute()
        {
            this.route.Nails.Remove(this);
        }

        public PointF Center()
        {
            return new PointF(this.X + R, this.Y + R);
        }

        public override void RemovedFromCanvas(LTSCanvas canvas)
        {
            canvas.RemoveSingleCanvasItem(this);
            this.RemoveFromCurrentRoute();
        }

        public override void AddToCanvas(LTSCanvas canvas)
        {
            canvas.AddSingleCanvasItem(this);
            this.route.AddNail(this, this.route.SegmentSelected);
        }
    }
}