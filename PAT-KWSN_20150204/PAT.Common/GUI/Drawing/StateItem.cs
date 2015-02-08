using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Xml;
using System.Collections.Generic;
using Tools.Diagrams;

namespace PAT.Common.GUI.Drawing
{
    public class StateItem : CanvasItem, IDisposable
    {
        LinearGradientBrush grad;
        GraphicsPath shadowpath;

        protected static Color[,] getFreeColors = new Color[4,4] 
        {
            {Color.Aqua, Color.LightSeaGreen, Color.PaleGoldenrod, Color.DarkGoldenrod}, 
            {Color.Red, Color.Red, Color.Red, Color.Red},
            {Color.Red, Color.Red, Color.Red, Color.Red},
            {Color.Red, Color.Red, Color.Red, Color.Red}
        };

        protected int freeColor;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")]
        public static readonly Font TitleFont = new Font(FontFamily.GenericSansSerif, 11, FontStyle.Bold, GraphicsUnit.Pixel);

        protected override bool AllowHeightModifications()
        {
            return false;
        }

        public const float R = 10;
        public override float Width
        {
            set
            {
                base.Width = R * 2;
                PrepareFrame();
            }
        }

        protected bool initialState;
        public virtual bool IsInitialState
        {
            get { return initialState; }
            set { initialState = value; }
        }

        public LabelItem labelItems = new LabelItem();

        #region Constructors
        public StateItem(bool initial, string name)
        {
            this.initialState = initial;
            Name = name;

            grad = new LinearGradientBrush(
                new PointF(0, 0), new PointF(1, 0),
                TitleBackground, Color.White);

            ActualHeight = R * 2;
            Width = GetAbsoluteContentWidth();
        }
        #endregion

        public override bool IsVResizable
        {
            get { return false; }
        }

        #region Graphics related members

        static Color titlesBG = Color.FromArgb(255, 217, 225, 241);
        protected virtual Color TitleBackground
        {
            get { return titlesBG; }
        }

        protected virtual LinearGradientBrush TitleBG
        {
            get { return grad; }
        }

        static Brush innerTitlesBG = new SolidBrush(Color.FromArgb(255, 240, 242, 249));
        protected virtual Brush InnerTitlesBackground
        {
            get { return innerTitlesBG; }
        }



        protected virtual bool RoundedCorners
        {
            get { return true; }
        }

        protected virtual int CornerRadius
        {
            get { return 15; }
        }
        #endregion

        #region Preparations



        protected virtual void PrepareFrame()
        {
            if (Container != null) return;

            shadowpath = new GraphicsPath();
            if (RoundedCorners)
            {
                int radius = CornerRadius;
                shadowpath.AddArc(ActualWidth - radius + 4, 3, radius, radius, 300, 60);
                shadowpath.AddArc(ActualWidth - radius + 4, ActualHeight - radius + 3, radius, radius, 0, 90);
                shadowpath.AddArc(4, ActualHeight - radius + 3, radius, radius, 90, 45);
                shadowpath.AddArc(ActualWidth - radius, ActualHeight - radius, radius, radius, 90, -90);
            }
            else
            {
                shadowpath.AddPolygon(new PointF[] {
                                                       new PointF(ActualWidth, 3),
                                                       new PointF(ActualWidth + 4, 3),
                                                       new PointF(ActualWidth + 4, ActualHeight + 3),
                                                       new PointF(4, ActualHeight + 3),
                                                       new PointF(4, ActualHeight),
                                                       new PointF(ActualWidth, ActualHeight)
                                                   });
            }
            shadowpath.CloseFigure();
        }

        #endregion

        public override void DrawToGraphics(Graphics graphics)
        {
            Brush pthGrBrush = Brushes.Black;
            Pen pen = Pens.Black;
            RectangleF EllipseFill = new RectangleF(AbsoluteX - 2, AbsoluteY - 2, 24, 24);

            GraphicsPath path = new GraphicsPath();
            path.AddEllipse(EllipseFill);

            Brush TextColor = Brushes.Black;

            switch (this.CurrentState)
            {
                case ItemState.Free:
                    pthGrBrush = new SolidBrush(ColorDefinition.GetColorToFillState());
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

            if (initialState)
            {
                EllipseFill = new RectangleF(AbsoluteX + 1, AbsoluteY + 1, 18, 18);
                graphics.DrawEllipse(Pens.Black, EllipseFill);
            }

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

            base.DrawToGraphics(graphics);
        }

        protected const int margin = 4;

        #region Behaviour

        //public override void HandleMouseClick (PointF pos)
        //{
        //    base.HandleMouseClick(pos);

        //    if (collapseExpandShape.IsInside(pos.X, pos.Y))
        //    {
        //        Collapsed = !Collapsed;
        //    }
        //    else
        //    {
        //        foreach (InteractiveHeaderedItem tg in groups)
        //        {
        //            if (tg.HitTest(pos))
        //            {
        //                tg.HandleMouseClick(pos);
        //            }
        //        }
        //    }
        //}

        #endregion

        #region Storage

        protected override XmlElement CreateXmlElement(XmlDocument doc)
        {
            return doc.CreateElement("State");
        }

        protected override void FillXmlElement(XmlElement element, XmlDocument document)
        {
            base.FillXmlElement(element, document);
            element.SetAttribute("Name", Name);
            element.SetAttribute("Init", IsInitialState.ToString());
        }

        public override XmlElement WriteToXml(XmlDocument document)
        {
            XmlElement elem = CreateXmlElement(document);
            FillXmlElement(elem, document);
            elem.AppendChild(this.labelItems.WriteToXml(document));
            return elem;
        }

        public override void LoadFromXml(XmlElement element)
        {
            base.LoadFromXml(element);
            this.labelItems.LoadFromXml((XmlElement)element.SelectSingleNode("./Label"));

            Name = element.GetAttribute("Name");
            IsInitialState = bool.Parse(element.GetAttribute("Init"));
        }
        #endregion

        public void Dispose()
        {
            grad.Dispose();
            if (shadowpath != null)
                shadowpath.Dispose();
        }

        public override string ToString()
        {
            return Name;
        }

        public PointF Center()
        {
            return new PointF(this.X + this.Width / 2, this.Y + this.Width / 2);
        }

        public static int StateCounterSpec = 0;

        public virtual string ToSpecificationString()
        {
            return "State: \"" + GetName() + "\"";
        }

        public string GetName()
        {
            if (string.IsNullOrEmpty(Name))
            {
                StateCounterSpec++;
                Name = "State" + StateCounterSpec;
            }

            return Name;
        }

        public virtual string ToLabelString()
        {
            return GetNamePart();
        }

        public string GetNamePart()
        {
            if (string.IsNullOrEmpty(this.Name))
                return string.Empty;

            return this.Name;
        }

        public override void RemovedFromCanvas(LTSCanvas canvas)
        {
            //Remove state
            canvas.RemoveSingleCanvasItem(this);

            //Remve transition
            Stack<Route> routesToRemove = new Stack<Route>();
            foreach (Route r in canvas.diagramRouter.Routes)
            {
                if (r.From == this || r.To == this)
                    routesToRemove.Push(r);
            }

            foreach (Route r in routesToRemove)
                r.RemovedFromCanvas(canvas);

            canvas.RemoveSingleCanvasItem(this.labelItems);
        }

        public override void AddToCanvas(LTSCanvas canvas)
        {
            canvas.AddSingleCanvasItem(this);
            this.labelItems.X = this.X - 10;
            this.labelItems.Y = this.Y + 30;
            canvas.AddSingleCanvasItem(this.labelItems);
        }
    }
}