using System.Drawing;
using System.Drawing.Drawing2D;
using System.Xml;
using PAT.Common.GUI.Drawing;
 
namespace PAT.Common.GUI.TAModule
{
    public class TAState : StateItem
    {
        public string Invariant = "";
        public bool IsUrgent = false;
        public bool IsCommitted = false;
        public bool IsError = false;

        public TAState(bool initial, string name) : base(initial, name)
        {

        }

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


            if (IsUrgent)
            {
                RectangleF rect = new RectangleF(this.AbsoluteX + margin, this.AbsoluteY + margin, 24, 24);
                graphics.DrawString("U", TitleFont, TextColor, rect);
            }
            else if (IsCommitted)
            {
                RectangleF rect = new RectangleF(this.AbsoluteX + margin, this.AbsoluteY + margin, 24, 24);
                graphics.DrawString("C", TitleFont, TextColor, rect);
            }
            else if (IsError)
            {
                RectangleF rect = new RectangleF(this.AbsoluteX + margin, this.AbsoluteY + margin, 24, 24);
                graphics.DrawString("E", TitleFont, TextColor, rect);
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
            element.SetAttribute("IsUrgent", IsUrgent.ToString());
            element.SetAttribute("IsCommitted", this.IsCommitted.ToString());
            element.SetAttribute("IsError", IsError.ToString());


        }

        public override void LoadFromXml(XmlElement element)
        {
            base.LoadFromXml(element);
            labelItems.LoadFromXml(element.ChildNodes[1] as XmlElement);

            Name = element.GetAttribute("Name");
            IsInitialState = bool.Parse(element.GetAttribute("Init"));
            Invariant = element.GetAttribute("Invariant");

            bool.TryParse(element.GetAttribute("IsUrgent"), out IsUrgent);
            bool.TryParse(element.GetAttribute("IsCommitted"), out IsCommitted);
            bool.TryParse(element.GetAttribute("IsError"), out IsError);

        }

        public override string ToSpecificationString()
        {
            return "State: \"" + GetName() + "\"" + (string.IsNullOrEmpty(Invariant) ? "" : " clocks : <" + Invariant + ">") + (IsUrgent ? "[U]" : "") + (IsCommitted ? "[C]" : "") + (IsError ? "[E]" : "");
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


        //public override string ToLabelString()
        //{
        //    return GetNamePart() + GetInvariantPart();
        //}
    }
}
