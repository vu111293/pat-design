using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using PAT.Common.GUI.Drawing;
using System.Xml;

namespace Tools.Diagrams
{
    public class Route
    {
        private readonly float MAXIMUM_DISTANCE = 3;
        private Transition transition = new Transition();

        public Transition Transition
        {
            get { return transition; }
            set { transition = value; }
        }

        private IRectangle from;
        private IRectangle to;
        private List<NailItem> nails = new List<NailItem>();

        public List<NailItem> Nails
        {
            get { return nails; }
            set { nails = value; }
        }

        private int nailSelected = -1;

        public int NailSelected
        {
            get { return nailSelected; }
        }
        private int segmentSelected = -1;

        public int SegmentSelected
        {
            get { return segmentSelected; }
        }

        /// <summary>
        /// Use property IsSelected instead
        /// </summary>
        private ItemState currentState;
        public ItemState CurrentState
        {
            get { return currentState; }
            set { currentState = value; this.Transition.CurrentState = value; }
        }

        public Route(IRectangle from, IRectangle to)
        {
            this.from = from;
            this.to = to;
        }

        public IRectangle From
        {
            get { return from; }
            set { from = value; }
        }

        public IRectangle To
        {
            get { return to; }
            set { to = value; }
        }


        public PointF GetStartingPoint()
        {
            StateItem from = (StateItem)this.from;
            if (this.nails.Count == 0)
            {
                StateItem to = (StateItem)this.to;
                return GraphUltility.FindPointByDistance(from.Center(), to.Center(), StateItem.R + 2);
            }

            return GraphUltility.FindPointByDistance(from.Center(), this.nails[0].Center(), StateItem.R + 2);
        }

        public PointF GetEndPoint()
        {
            StateItem to = (StateItem)this.to;
            if (this.nails.Count == 0)
            {
                StateItem from = (StateItem)this.from;
                return GraphUltility.FindPointByDistance(to.Center(), from.Center(), StateItem.R + 2);
            }

            return GraphUltility.FindPointByDistance(to.Center(), this.nails[this.nails.Count - 1].Center(), StateItem.R + 2);
        }

        public bool IsBelongRoute(PointF point)
        {
            this.nailSelected = -1;
            this.segmentSelected = -1;

            PointF current = this.GetStartingPoint();
            for (int i = 0; i < nails.Count; i++)
            {
                PointF nailPosition = nails[i].Center();
                if (GraphUltility.Distance(point, nailPosition) <= MAXIMUM_DISTANCE)
                {
                    this.nailSelected = i;
                    return true;
                }

                if (GraphUltility.Distance(current, point) + GraphUltility.Distance(point, nailPosition) - GraphUltility.Distance(current, nailPosition) <= MAXIMUM_DISTANCE)
                {
                    this.segmentSelected = i;
                    return true;
                }

                current = nailPosition;
            }

            PointF end = this.GetEndPoint();
            if (GraphUltility.Distance(current, point) + GraphUltility.Distance(point, end) - GraphUltility.Distance(current, end) <= MAXIMUM_DISTANCE)
            {
                this.segmentSelected = this.nails.Count;
                return true;
            }

            return false;
        }

        public void AddNail(NailItem nail, int segmentIndex)
        {
            if (segmentIndex < 0)
                this.nails.Insert(0, nail);
            else
                this.nails.Insert(segmentIndex, nail);
        }

        public virtual void DrawToGraphics(Graphics graphics)
        {
            Pen pen = Pens.Black;
            switch (this.CurrentState)
            {
                case ItemState.Free:
                    pen = new Pen(ColorDefinition.GetColorWhenFree());
                    break;

                case ItemState.Hover:
                    pen = new Pen(ColorDefinition.GetColorWhenHover());
                    break;

                case ItemState.Selected:
                    pen = new Pen(ColorDefinition.GetColorWhenSelected());
                    break;

                default:
                    break;
            } // switch

            List<PointF> tempNails = new List<PointF>();
            tempNails.Add(this.GetStartingPoint());
            foreach (NailItem nail in this.nails)
                tempNails.Add(nail.Center());
            tempNails.Add(this.GetEndPoint());

            float R = NailItem.R;
            if (this.CurrentState == ItemState.Free)
                R = NailItem.R + 5;

            PointF[] points = new PointF[3];
            PointF firstNail = PointF.Empty;
            firstNail = tempNails[0];
            for (int i = 0; i < tempNails.Count - 1; i++)
            {
                points[1] = tempNails[i + 1];
                points[0] = GraphUltility.FindPointByDistance(points[1], firstNail, R);
                graphics.DrawLine(pen, firstNail, points[0]);

                if (i + 2 < tempNails.Count)
                {
                    points[2] = GraphUltility.FindPointByDistance(points[1], tempNails[i + 2], R);
                    firstNail = points[2];

                    //Make the intersection curved
                    if (this.CurrentState == ItemState.Free)
                        graphics.DrawBezier(pen, points[0], points[1], points[1], points[2]);
                }
            }

            DrawRouteWithBigArrow(graphics, pen, firstNail, tempNails[tempNails.Count - 1]);
        }

        public virtual void FindLabelLocation()
        {
            List<PointF> tempNails = new List<PointF>();
            tempNails.Add(this.GetStartingPoint());
            foreach (NailItem nail in this.nails)
                tempNails.Add(nail.Center());
            tempNails.Add(this.GetEndPoint());
            float height = this.transition.TitleFont.Size + 5;

            int startIndex = (tempNails.Count - 1) / 2;
            PointF start = tempNails[startIndex];
            PointF end = tempNails[startIndex + 1];

            PointF startText = new PointF((end.X + start.X) / 2, (end.Y + start.Y) / 2);
            if ((end.X - start.X) * (end.Y - start.Y) > 0)
                startText.Y -= height;

            this.transition.X = startText.X;
            this.transition.Y = startText.Y;
        }

        private bool IsLabelBelowTransition(List<PointF> tempNails, int startIndex, Graphics graphics)
        {
            int endIndex = startIndex + 1;
            PointF[] points = new PointF[] { tempNails[startIndex], tempNails[startIndex], tempNails[endIndex], tempNails[endIndex] };
            if (startIndex > 0)
                points[0] = tempNails[startIndex - 1];

            if (endIndex + 1 < tempNails.Count)
                points[3] = tempNails[endIndex + 1];

            Matrix matrix = graphics.Transform;
            matrix.Invert();
            matrix.TransformPoints(points);

            //If the left and right nail are both on the same side and above the start nail, end nail
            //then the lable should be displayed below transition
            return (points[0].Y < points[1].Y && points[3].Y < points[2].Y);
        }

        private float AngleWithXAxis(PointF vector)
        {
            float angle = 90;
            if (vector.X != 0)
                angle = (float)(Math.Atan(vector.Y / vector.X) / Math.PI * 180);

            if (angle > 0 && vector.X < 0 && vector.Y < 0)
                angle -= 180;

            if (angle < 0 && vector.X < 0 && vector.Y > 0)
                angle += 180;

            return angle;
        }

        protected void DrawRouteWithBigArrow(Graphics g, Pen pen, PointF from, PointF to)
        {
            float rate = 0.001F;
            float x = to.X + rate * (from.X - to.X);
            float y = to.Y + rate * (from.Y - to.Y);
            PointF tempPoint = new PointF(x, y);
            pen.EndCap = LineCap.ArrowAnchor;
            pen.Width = 5;
            g.DrawLine(pen, tempPoint, to);
        }

        public const string LINK_NODE_NAME = "Link";
        public const string FROM_NODE_NAME = "From";
        public const string TO_NODE_NAME = "To";
        public const string NAIL_NODE_NAME = "Nail";
        public const string SELECT_NODE_NAME = "Select";
        public const string EVENT_NODE_NAME = "Event";
        public const string GUARD_NODE_NAME = "Guard";
        public const string CLOCK_GUARD_NODE_NAME = "ClockGuard";
        public const string PROGRAM_NODE_NAME = "Program";
        public const string CLOCK_RESET_NODE_NAME = "ClockReset";
        public const string LABEL_NODE_NAME = "Label";

        public virtual XmlElement WriteToXml(XmlDocument doc)
        {
            XmlElement linkElement = doc.CreateElement(LINK_NODE_NAME);
            XmlElement fromElement = doc.CreateElement(FROM_NODE_NAME);
            fromElement.InnerText = ((StateItem)this.from).Name;
            linkElement.AppendChild(fromElement);

            XmlElement toElement = doc.CreateElement(TO_NODE_NAME);
            toElement.InnerText = ((StateItem)this.to).Name;
            linkElement.AppendChild(toElement);

            XmlElement selectElement = doc.CreateElement(SELECT_NODE_NAME);
            selectElement.InnerText = this.transition.Select;
            linkElement.AppendChild(selectElement);

            XmlElement eventElement = doc.CreateElement(EVENT_NODE_NAME);
            eventElement.InnerText = this.transition.Event;
            linkElement.AppendChild(eventElement);

            XmlElement clockguardElement = doc.CreateElement(CLOCK_GUARD_NODE_NAME);
            clockguardElement.InnerText = this.transition.ClockGuard;
            linkElement.AppendChild(clockguardElement);

            XmlElement guardElement = doc.CreateElement(GUARD_NODE_NAME);
            guardElement.InnerText = this.transition.Guard;
            linkElement.AppendChild(guardElement);

            XmlElement programElement = doc.CreateElement(PROGRAM_NODE_NAME);
            programElement.InnerText = this.transition.Program;
            linkElement.AppendChild(programElement);

            XmlElement clockResetElement = doc.CreateElement(CLOCK_RESET_NODE_NAME);
            clockResetElement.InnerText = this.transition.ClockReset;
            linkElement.AppendChild(clockResetElement);

            foreach (NailItem nail in this.nails)
                linkElement.AppendChild(nail.WriteToXml(doc));

            linkElement.AppendChild(this.transition.WriteToXml(doc));
            return linkElement;
        }

        public virtual void LoadFromXML(XmlElement xmlElement, LTSCanvas canvas)
        {
            //string startingState = xmlElement.ChildNodes[0].InnerText;
            //this.from = canvas.FindState(startingState);
            //string endState = xmlElement.ChildNodes[1].InnerText;
            //this.to = canvas.FindState(endState);

            //this.Transition.Select = xmlElement.ChildNodes[2].InnerText;
            //this.Transition.Event = xmlElement.ChildNodes[3].InnerText;
            //this.Transition.ClockGuard = xmlElement.ChildNodes[4].InnerText;
            //this.Transition.Guard = xmlElement.ChildNodes[5].InnerText;
            //this.Transition.Program = xmlElement.ChildNodes[6].InnerText;
            //this.Transition.ClockReset = xmlElement.ChildNodes[7].InnerText;
            //this.Transition.Width = this.Transition.GetWidthOfLabel();

            //for (int i = 8; i < xmlElement.ChildNodes.Count - 1; i++)
            //{
            //    NailItem nail = new NailItem(this);
            //    nail.LoadFromXml(xmlElement.ChildNodes[i] as XmlElement);
            //    this.nails.Add(nail);
            //}

            //this.transition.LoadFromXml(xmlElement.ChildNodes[xmlElement.ChildNodes.Count - 1] as XmlElement);

            string startingState = xmlElement.SelectSingleNode("./" + FROM_NODE_NAME).InnerText;
            this.from = canvas.FindState(startingState);
            string endState = xmlElement.SelectSingleNode("./" + TO_NODE_NAME).InnerText;
            this.to = canvas.FindState(endState);

            this.Transition.Select = xmlElement.SelectSingleNode("./" + SELECT_NODE_NAME).InnerText;
            this.Transition.Event = xmlElement.SelectSingleNode("./" + EVENT_NODE_NAME).InnerText;
            this.Transition.ClockGuard = xmlElement.SelectSingleNode("./" + CLOCK_GUARD_NODE_NAME).InnerText;
            this.Transition.Guard = xmlElement.SelectSingleNode("./" + GUARD_NODE_NAME).InnerText;
            this.Transition.Program = xmlElement.SelectSingleNode("./" + PROGRAM_NODE_NAME).InnerText;
            this.Transition.ClockReset = xmlElement.SelectSingleNode("./" + CLOCK_RESET_NODE_NAME).InnerText;
            this.Transition.Width = this.Transition.GetWidthOfLabel();

            XmlNodeList nails = xmlElement.SelectNodes("./" + NAIL_NODE_NAME);
            if (nails != null)
            {
                foreach (XmlElement xmlNail in nails)
                {
                    NailItem nail = new NailItem(this);
                    nail.LoadFromXml(xmlNail);
                    this.nails.Add(nail);
                }
            }

            this.transition.LoadFromXml((XmlElement)xmlElement.SelectSingleNode("./" + LABEL_NODE_NAME));
        }

        public bool IsBelongRectange(Rectangle rect)
        {
            foreach (NailItem nail in this.nails)
            {
                if (!GraphUltility.IsBelongRectangle(rect, new PointF(nail.X, nail.Y)))
                    return false;
            }

            return GraphUltility.IsBelongRectangle(rect, new PointF(this.from.X, this.from.Y)) &&
                            GraphUltility.IsBelongRectangle(rect, new PointF(this.to.X, this.to.Y));
        }

        public virtual void RemovedFromCanvas(LTSCanvas canvas)
        {
            //Remove nail
            foreach (NailItem nailItem in this.Nails)
                canvas.RemoveSingleCanvasItem(nailItem);

            this.Nails.Clear();

            //Remove transition
            canvas.RemoveSingleCanvasItem(this.transition);
            canvas.RemoveSingleRoute(this);
        }

        public virtual void AddToCanvas(LTSCanvas canvas)
        {
            canvas.AddSingleLink(this);
            for (int i = 1; i < canvas.temporaryNails.Count; i++)
            {
                PointF p = canvas.temporaryNails[i];
                NailItem nailItem = new NailItem(this);
                nailItem.X = p.X;
                nailItem.Y = p.Y;
                canvas.AddSingleCanvasItem(nailItem);
                this.AddNail(nailItem, this.Nails.Count);
            }
            canvas.temporaryNails.Clear();

            this.FindLabelLocation();
            canvas.AddSingleCanvasItem(this.transition);
        }

        public void HandleMouseHoverIn(LTSCanvas canvas)
        {
            if (this.CurrentState != ItemState.Selected)
                this.CurrentState = ItemState.Hover;
        }

        public virtual void HandleMouseHoverOut(LTSCanvas canvas)
        {
            if (this.CurrentState == ItemState.Hover)
                this.CurrentState = ItemState.Free;
        }
    }
}
