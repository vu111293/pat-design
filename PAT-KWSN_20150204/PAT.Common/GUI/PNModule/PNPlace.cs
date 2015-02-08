using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PAT.Common.GUI.Drawing;
using PAT.Common.GUI.KWSNModule;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Xml;
using System.Globalization;

namespace PAT.Common.GUI.PNModule
{
    public class PNPlace : StateItem
    {
        private string _id;
        private int _numberOfTokens;
        private int _capacity;
        public int Capacity
        {
            get { return _capacity; }
            set
            {
                if (value > 0)
                    _capacity = value;
                else
                    _capacity = 0;
            }
        }

        private string _guard;
        public string Guard
        {
            get { return _guard; }
            set { _guard = value; }
        }

        public int NumberOfTokens
        {
            get { return _numberOfTokens; }
            set
            {
                if (value > 0)
                {
                    if (_capacity > 0)
                    {
                        if (value > _capacity)
                            MessageBox.Show(string.Format("This place has capacity is {0}, cannot add more", _capacity));
                        else
                            _numberOfTokens = value;
                    }
                    else
                    {
                        _numberOfTokens = value;
                    }
                }
                else
                {
                    _numberOfTokens = 0;
                }
            }
        }

        public PNPlace(string name, int numberOfToken, string id, int color)
            : base(false, name)
        {
            // TODO: Complete member initialization
            _numberOfTokens = numberOfToken;
            _guard = string.Empty;
            freeColor = color;
            _id = id;
        }

        public override void DrawToGraphics(Graphics graphics)
        {
            Brush pthGrBrush = Brushes.Black;
            Pen pen = new Pen(Color.Black);
            RectangleF EllipseFill = new RectangleF(AbsoluteX - 2, AbsoluteY - 2, 24, 24);

            GraphicsPath path = new GraphicsPath();
            path.AddEllipse(EllipseFill);

            Brush TextColor = Brushes.Black;
            switch (this.CurrentState)
            {
                case ItemState.Free:
                    pthGrBrush = new SolidBrush(ColorDefinition.GetColorToFillState());
                    if (freeColor != 0) pthGrBrush = new SolidBrush((freeColor > 0) ? getFreeColors[freeColor - 1, 0] : getFreeColors[(-1) * freeColor - 1, 2]);
                    break;

                case ItemState.Hover:
                    pthGrBrush = new SolidBrush(ColorDefinition.GetColorWhenHover());
                    TextColor = Brushes.White;
                    break;

                case ItemState.Selected:
                    pthGrBrush = new SolidBrush(ColorDefinition.GetColorWhenSelected());
                    break;

                default:
                    break;
            } // switch

            graphics.FillEllipse(pthGrBrush, EllipseFill);
            graphics.DrawEllipse(pen, EllipseFill);

            //1
            switch (_numberOfTokens)
            {
                case 0:
                    break;

                case 1:
                    EllipseFill = DrawToken(AbsoluteX + 9, AbsoluteY + 9, 4, 4, graphics);
                    break;

                case 2:
                    EllipseFill = DrawToken(AbsoluteX + 4, AbsoluteY + 9, 4, 4, graphics);
                    EllipseFill = DrawToken(AbsoluteX + 14, AbsoluteY + 9, 4, 4, graphics);
                    break;

                case 3:
                    EllipseFill = DrawToken(AbsoluteX + 9, AbsoluteY + 4, 4, 4, graphics);
                    EllipseFill = DrawToken(AbsoluteX + 4, AbsoluteY + 14, 4, 4, graphics);
                    EllipseFill = DrawToken(AbsoluteX + 14, AbsoluteY + 14, 4, 4, graphics);
                    break;

                case 4:
                    EllipseFill = DrawToken(AbsoluteX + 4, AbsoluteY + 4, 4, 4, graphics);
                    EllipseFill = DrawToken(AbsoluteX + 4, AbsoluteY + 14, 4, 4, graphics);
                    EllipseFill = DrawToken(AbsoluteX + 14, AbsoluteY + 4, 4, 4, graphics);
                    EllipseFill = DrawToken(AbsoluteX + 14, AbsoluteY + 14, 4, 4, graphics);
                    break;

                case 5:
                    EllipseFill = DrawToken(AbsoluteX + 4, AbsoluteY + 4, 4, 4, graphics);
                    EllipseFill = DrawToken(AbsoluteX + 4, AbsoluteY + 14, 4, 4, graphics);
                    EllipseFill = DrawToken(AbsoluteX + 14, AbsoluteY + 4, 4, 4, graphics);
                    EllipseFill = DrawToken(AbsoluteX + 14, AbsoluteY + 14, 4, 4, graphics);
                    EllipseFill = DrawToken(AbsoluteX + 9, AbsoluteY + 9, 4, 4, graphics);
                    break;

                default:
                    RectangleF rect = new RectangleF(AbsoluteX + 5, AbsoluteY + 5, 18, 18);
                    Font titleFont = new Font(FontFamily.GenericSansSerif, 10, FontStyle.Regular, GraphicsUnit.Pixel);
                    graphics.DrawString(_numberOfTokens.ToString(), titleFont, new SolidBrush(Color.Black), rect);
                    break;
            } // switch

            this.labelItems.labels.Clear();
            this.labelItems.colors.Clear();

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

            this.labelItems.labels.Add(Name);
            this.labelItems.colors.Add(color);
            this.labelItems.DrawToGraphics(graphics);

            DrawDecorators(graphics);
        }

        private RectangleF DrawToken(float x, float y, int w, int h, Graphics graphics)
        {
            RectangleF EllipseFill = new RectangleF(x, y, w, h);
            graphics.FillEllipse(new SolidBrush(Color.Black), EllipseFill);
            graphics.DrawEllipse(Pens.Black, EllipseFill);
            return EllipseFill;
        }

        public override XmlElement WriteToXml(XmlDocument document)
        {
            XmlElement elem = CreateXmlElement(document);
            FillXmlElement(elem, document);
            elem.AppendChild(this.labelItems.WriteToXml(document));

            elem.SetAttribute(PNNode.TAG_REFERENCE_ID, _id);

            XmlElement eGuard = document.CreateElement("Guard");
            eGuard.InnerText = _guard;
            elem.AppendChild(eGuard);

            return elem;
        }

        public override void LoadFromXml(XmlElement XMLelement)
        {
            try
            {
                XmlNodeList position = XMLelement.GetElementsByTagName("Position");
                foreach (XmlElement element in position)
                {
                    X = 100 * float.Parse(element.GetAttribute("X", ""), CultureInfo.InvariantCulture);
                    Y = 100 * float.Parse(element.GetAttribute("Y", ""), CultureInfo.InvariantCulture);
                    Width = 100 * float.Parse(element.GetAttribute("Width", ""), CultureInfo.InvariantCulture);
                    break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

            this.labelItems.LoadFromXml(XMLelement.ChildNodes[1] as XmlElement);
            Guard = XMLelement.GetElementsByTagName("Guard")[0].InnerText;
            Name = XMLelement.GetAttribute("Name");
            NumberOfTokens = int.Parse(XMLelement.GetAttribute("NumOfToken"));
            Capacity = int.Parse(XMLelement.GetAttribute("Capacity"));
        }

        protected override XmlElement CreateXmlElement(XmlDocument doc)
        {
            return doc.CreateElement("Place");
        }

        protected override void FillXmlElement(XmlElement element, XmlDocument document)
        {
            XmlElement position = document.CreateElement("Position");
            FillXmlPositionElement(position, document);
            element.AppendChild(position);
            element.SetAttribute("Name", Name);
            element.SetAttribute("NumOfToken", NumberOfTokens.ToString());
            element.SetAttribute("Capacity", Capacity.ToString());
        }
    }
}
