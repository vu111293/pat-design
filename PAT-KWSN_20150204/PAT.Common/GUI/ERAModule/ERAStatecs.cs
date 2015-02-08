using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Xml;
using PAT.Common.GUI.Drawing; 

namespace PAT.Common.GUI.ERAModule
{
    public class ERAState : StateItem
    {
        public string Invariant = "";
        public bool IsAcceptance = false;
        public bool IsError = false;

        public ERAState(bool initial, string name) : base(initial, name)
        {

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
                    //pen = new Pen(ColorDefinition.GetColorWhenFree());   
                    break;
                case ItemState.Hover:
                    pthGrBrush = new SolidBrush(ColorDefinition.GetColorWhenHover());
                    //pen = new Pen(ColorDefinition.GetColorWhenFree());
                    TextColor = Brushes.White;
                    break;
                case ItemState.Selected:
                    pthGrBrush = new SolidBrush(ColorDefinition.GetColorWhenSelected());
                    //pen = new Pen(ColorDefinition.GetColorWhenFree());
                    break;
            }

            graphics.FillEllipse(pthGrBrush, EllipseFill);

            //if (IsAcceptance)
            //{
            //    pen.Width = 2;              
            //}

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
            }
            if (this.CurrentState == ItemState.Free)
            {
                color = ColorDefinition.GetColorOfName();
            }
            this.labelItems.labels.Add(Name);
            this.labelItems.colors.Add(color);

            if (!string.IsNullOrEmpty(Name))
            {
                string invariant = GetInvariantPart();
                if (this.CurrentState == ItemState.Free)
                {
                    color = ColorDefinition.GetColorOfInvariant();
                }

                this.labelItems.labels.Add(invariant);
                this.labelItems.colors.Add(color);
            }

            this.labelItems.DrawToGraphics(graphics);


            //if (IsError)
            //{
            //    RectangleF rect = new RectangleF(this.AbsoluteX + margin, this.AbsoluteY + margin, 24, 24);
            //    graphics.DrawString("E", TitleFont, TextColor, rect);
            //}

            if (IsAcceptance)
            {
                RectangleF rect = new RectangleF(this.AbsoluteX + margin, this.AbsoluteY + margin, 24, 24);
                graphics.DrawString("A", TitleFont, TextColor, rect);
            }

            //base.DrawToGraphics(graphics);
            DrawDecorators(graphics);
        }

        protected override void FillXmlElement(XmlElement element, XmlDocument document)
        {
            base.FillXmlElement(element, document);
            element.SetAttribute("Name", Name);
            element.SetAttribute("Init", IsInitialState.ToString());
            element.SetAttribute("Invariant", Invariant);
            element.SetAttribute("IsError", IsError.ToString());
            element.SetAttribute("IsAcceptance", this.IsAcceptance.ToString());

        }

        public override void LoadFromXml(XmlElement element)
        {
            base.LoadFromXml(element);
            this.labelItems.LoadFromXml(element.ChildNodes[1] as XmlElement);

            Name = element.GetAttribute("Name");
            IsInitialState = bool.Parse(element.GetAttribute("Init"));
            try
            {
                Invariant = element.GetAttribute("Invariant");
            }
            catch (Exception)
            {
            }
            

            bool.TryParse(element.GetAttribute("IsError"), out IsError);
            bool.TryParse(element.GetAttribute("IsAcceptance"), out IsAcceptance);

        }

        protected string GetInvariantPart()
        {
            if (string.IsNullOrEmpty(this.Invariant))
            {
                return string.Empty;
            }
            else
            {
                return " \\ " + Invariant;
            }
        }

        public override string ToSpecificationString()
        {
            return "State: \"" + GetName() + "\"" + (string.IsNullOrEmpty(Invariant) ? "" : " clocks : <" + Invariant + ">") + (IsAcceptance ? "[A]" : "") + (IsError ? "[E]" : "");
            //return "State: \"" + GetName() + "\"" + (IsAcceptance ? "[A]" : "") + (IsError? "[E]" : "");
        }

        //public override string ToLabelString()
        //{
        //    return GetNamePart() + GetInvariantPart();
        //}
    }
}
