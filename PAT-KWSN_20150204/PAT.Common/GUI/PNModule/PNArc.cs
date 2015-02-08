using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tools.Diagrams;
using System.Drawing;
using PAT.Common.GUI.Drawing;
using System.Drawing.Drawing2D;
using System.Xml;

namespace PAT.Common.GUI.PNModule
{
    public class PNArc : Route
    {
        private int _weight = 1;
        public int Weight
        {
            get { return _weight; }
            set
            {
                if (value < 1)
                    _weight = 1;
                else
                    _weight = value;
            }
        }

        public PNLabel Label = new PNLabel();

        public PNArc(IRectangle from, IRectangle to) : base(from, to) { }

        public override void DrawToGraphics(Graphics graphics)
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
            foreach (NailItem nail in this.Nails)
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

            Label.Value = _weight;
            Label.CurrentState = this.CurrentState;
            Label.DrawToGraphics(graphics);
        }

        public override void AddToCanvas(LTSCanvas canvas)
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

            FindLabelLocation();
            canvas.AddSingleCanvasItem(Label);
        }

        public override void FindLabelLocation()
        {
            List<PointF> tempNails = new List<PointF>();
            tempNails.Add(this.GetStartingPoint());
            foreach (NailItem nail in this.Nails)
                tempNails.Add(nail.Center());

            tempNails.Add(this.GetEndPoint());
            float height = this.Label.TitleFont.Size + 5;

            int startIndex = (tempNails.Count - 1) / 2;
            PointF start = tempNails[startIndex];
            PointF end = tempNails[startIndex + 1];

            PointF startText = new PointF((end.X + start.X) / 2, (end.Y + start.Y) / 2);
            if ((end.X - start.X) * (end.Y - start.Y) > 0)
                startText.Y -= height;

            this.Label.X = startText.X;
            this.Label.Y = startText.Y;
        }

        public override void RemovedFromCanvas(LTSCanvas canvas)
        {
            //Remove nail
            foreach (NailItem nailItem in this.Nails)
                canvas.RemoveSingleCanvasItem(nailItem);
            this.Nails.Clear();

            //Remove transition
            canvas.RemoveSingleCanvasItem(Label);
            canvas.RemoveSingleRoute(this);
        }

        public override XmlElement WriteToXml(XmlDocument doc)
        {
            XmlElement linkElement = doc.CreateElement("Arc");
            linkElement.SetAttribute("From", (this.From as StateItem).Name);
            linkElement.SetAttribute("To", (this.To as StateItem).Name);
            linkElement.SetAttribute("Weight", Weight.ToString());

            foreach (NailItem nail in this.Nails)
                linkElement.AppendChild(nail.WriteToXml(doc));

            linkElement.AppendChild(Label.WriteToXml(doc));
            return linkElement;
        }

        public override void LoadFromXML(XmlElement xmlElement, LTSCanvas canvas)
        {
            if (canvas is PNCanvas)
            {
                PNCanvas myCanvas = canvas as PNCanvas;
                string startingState = xmlElement.GetAttribute("From");
                this.From = myCanvas.FindState(startingState);
                string endState = xmlElement.GetAttribute("To");
                this.To = myCanvas.FindState(endState);

                if (From is PNTransition)
                    (From as PNTransition).OutputPlaces.Add(To as PNPlace);
                else
                    (To as PNTransition).InputPlaces.Add(From as PNPlace);

                this.Weight = int.Parse(xmlElement.GetAttribute("Weight"));

                for (int i = 0; i < xmlElement.ChildNodes.Count - 1; i++)
                {
                    NailItem nail = new NailItem(this);
                    nail.LoadFromXml(xmlElement.ChildNodes[i] as XmlElement);
                    this.Nails.Add(nail);
                }

                this.Label.LoadFromXml(xmlElement.ChildNodes[xmlElement.ChildNodes.Count - 1] as XmlElement);
            }
        }
    }
}
