using System;
using System.Drawing;
using System.Xml;
using Tools.Diagrams;
namespace PAT.Common.GUI.Drawing
{
    public class Transition : CanvasItem
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")]
        public readonly Font TitleFont = new Font(FontFamily.GenericSansSerif, 14, FontStyle.Regular, GraphicsUnit.Pixel);

        public string Select;
        public string Event;
        public string Guard;
        public string ClockGuard;
        public string Program;
        public string ClockReset;

        protected override bool AllowHeightModifications()
        {
            return false;
        }

        public const string LINK_LABEL = "Label";
        public Transition()
        {
            this.Event = this.Guard = ClockGuard = this.Program = ClockReset = string.Empty;
            this.Height = this.TitleFont.Size + 5;
            this.Width = 25;
        }

        public override bool IsVResizable
        {
            get { return false; }
        }

        #region Draw label multi color
        //public override void DrawToGraphics(Graphics graphics)
        //{
        //    string label = this.ToLabelString();
        //    if (!string.IsNullOrEmpty(label))
        //    {
        //        Color color = Color.Black;
        //        switch (this.CurrentState)
        //        {
        //            case ItemState.Hover:
        //                color = ColorDefinition.GetColorWhenHover();
        //                break;
        //            case ItemState.Selected:
        //                color = ColorDefinition.GetColorWhenSelected();
        //                break;
        //        }
        //        RectangleF rect;
        //        float width = 0;
        //        float height = this.TitleFont.Size + 5;
        //        float currentWidth = 0;

        //        //Select
        //        label = GetSelectPart().Replace("\r\n", string.Empty);
        //        if (!string.IsNullOrEmpty(label))
        //        {
        //            if (CurrentState == ItemState.Free)
        //            {
        //                color = ColorDefinition.GetColorOfSelect();
        //            }

        //            currentWidth = graphics.MeasureString(label, this.TitleFont).Width;
        //            rect = new RectangleF(X + width, Y, currentWidth, height);
        //            graphics.DrawString(label, this.TitleFont, new SolidBrush(color), rect);
        //            width += currentWidth;
        //        }

        //        //Clock guard
        //        label = GetClockGuardPart().Replace("\r\n", string.Empty);
        //        if (!string.IsNullOrEmpty(label))
        //        {
        //            if (CurrentState == ItemState.Free)
        //            {
        //                color = ColorDefinition.GetColorOfClockGuard();
        //            }

        //            currentWidth = graphics.MeasureString(label, this.TitleFont).Width;
        //            rect = new RectangleF(X + width, Y, currentWidth, height);
        //            graphics.DrawString(label, this.TitleFont, new SolidBrush(color), rect);
        //            width += currentWidth;
        //        }

        //        //Guard
        //        label = GetGuardPart().Replace("\r\n", string.Empty);
        //        if (!string.IsNullOrEmpty(label))
        //        {
        //            if (CurrentState == ItemState.Free)
        //            {
        //                color = ColorDefinition.GetColorOfGuard();
        //            }
        //            currentWidth = graphics.MeasureString(label, this.TitleFont).Width;
        //            rect = new RectangleF(X + width, Y, currentWidth, height);
        //            graphics.DrawString(label, this.TitleFont, new SolidBrush(color), rect);
        //            width += currentWidth;
        //        }

        //        //Event
        //        label = GetEventPart().Replace("\r\n", string.Empty);
        //        if (!string.IsNullOrEmpty(label))
        //        {
        //            if (CurrentState == ItemState.Free)
        //            {
        //                color = ColorDefinition.GetColorOfEvent();
        //            }
        //            currentWidth = graphics.MeasureString(label, this.TitleFont).Width;
        //            rect = new RectangleF(X + width, Y, currentWidth, height);
        //            graphics.DrawString(label, this.TitleFont, new SolidBrush(color), rect);
        //            width += currentWidth;
        //        }

        //        //Program
        //        label = GetProgramPart().Replace("\r\n", string.Empty);
        //        if (!string.IsNullOrEmpty(label))
        //        {
        //            if (CurrentState == ItemState.Free)
        //            {
        //                color = ColorDefinition.GetColorOfProgram();
        //            }
        //            currentWidth = graphics.MeasureString(label, this.TitleFont).Width;
        //            rect = new RectangleF(X + width, Y, currentWidth, height);
        //            graphics.DrawString(label, this.TitleFont, new SolidBrush(color), rect);
        //            width += currentWidth;
        //        }

        //        //Clock Reset
        //        label = GetClockResetPart().Replace("\r\n", string.Empty);
        //        if (!string.IsNullOrEmpty(label))
        //        {
        //            if (CurrentState == ItemState.Free)
        //            {
        //                color = ColorDefinition.GetColorOfClockReset();
        //            }
        //            currentWidth = graphics.MeasureString(label, this.TitleFont).Width;
        //            rect = new RectangleF(X + width, Y, currentWidth, height);
        //            graphics.DrawString(label, this.TitleFont, new SolidBrush(color), rect);
        //            width += currentWidth;
        //        }

        //        if (Math.Abs(this.Width - width) > 15)
        //        {
        //            this.Width = width;
        //        }
        //    }
        //}
        #endregion Draw label multi color

        public override void DrawToGraphics(Graphics graphics)
        {
            string label = this.ToLabelString();
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
            bool ret = (pos.X >= X && pos.Y >= Y && pos.X < X + this.Width && pos.Y < Y + this.Height);
            return ret;
        }

        /// <summary>
        /// Display the lable on the screen
        /// </summary>
        private string ToLabelString()
        {
            string label = GetSelectPart() + GetClockGuardPart() + GetGuardPart() + GetEventPart() + GetProgramPart() + GetClockResetPart();
            label = label.Replace("\r\n", string.Empty);
            return label;
        }

        public string ToSpecificationString()
        {
            return GetSelectPart() + GetClockGuardPart() + GetGuardPart() + " ##@@ " + GetEventPart() + " @@## " + GetProgramPart() + GetClockResetPart();
        }

        private string GetProgramPart()
        {
            if (!string.IsNullOrEmpty(Program))
                return " {" + Program + "}";

            return string.Empty;
        }

        private string GetSelectPart()
        {
            if (!string.IsNullOrEmpty(Select))
                return " select : " + Select + "";

            return string.Empty;
        }

        private string GetClockResetPart()
        {
            if (!string.IsNullOrEmpty(ClockReset))
                return " clockreset : " + ClockReset + "";

            return string.Empty;
        }

        private string GetClockGuardPart()
        {
            if (!string.IsNullOrEmpty(ClockGuard))
                return " clocks : <" + ClockGuard + ">";

            return string.Empty;
        }

        private string GetGuardPart()
        {
            if (!string.IsNullOrEmpty(Guard))
                return "[" + Guard + "]";

            return string.Empty;
        }

        public string GetEventPart()
        {
            if (string.IsNullOrEmpty(Event))
                return "";

            return Event;
        }

        public override void RemovedFromCanvas(LTSCanvas canvas)
        {
            Route route = this.FindSelectedRouteBasedOnTransition(canvas);
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
                Route route = this.FindSelectedRouteBasedOnTransition(canvas);
                route.HandleMouseHoverIn(canvas);
            }
            catch { }
        }

        public override void HandleMouseHoverOut(LTSCanvas canvas)
        {
            try
            {
                Route route = this.FindSelectedRouteBasedOnTransition(canvas);
                route.HandleMouseHoverOut(canvas);
            }
            catch { }
        }

        public override void HandleSelected(LTSCanvas canvas)
        {
            Route route = this.FindSelectedRouteBasedOnTransition(canvas);
            route.CurrentState = ItemState.Selected;
        }

        /// <summary>
        /// Throw exception when route not found
        /// </summary>
        public Route FindSelectedRouteBasedOnTransition(LTSCanvas canvas)
        {
            foreach (Route route in canvas.diagramRouter.routes)
            {
                if (route.Transition.Equals(this))
                    return route;
            }

            throw new Exception("Route not found");
        }

        public float GetWidthOfLabel()
        {
            return this.Graphics.MeasureString(this.ToLabelString(), this.TitleFont).Width;
        }

        protected override XmlElement CreateXmlElement(XmlDocument doc)
        {
            return doc.CreateElement(LINK_LABEL);
        }
    }
}