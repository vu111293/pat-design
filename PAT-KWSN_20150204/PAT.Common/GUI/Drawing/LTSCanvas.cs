using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using PAT.Common.LTSModule.Drawing.Diagram.Decorators;
using Tools.Diagrams;

namespace PAT.Common.GUI.Drawing
{
    public partial class LTSCanvas
    {
        private const int Margine = 20;
        public TreeNode Node;
        public string Parameters = "";

        public int StateCounter = 1;
        public List<XmlElement> undoStack = new List<XmlElement>();
        public int currentStateIndex = -1;

        public class CanvasItemData : IDisposable
        {
            public CanvasItemData(CanvasItem item,
                                   EventHandler<SizeGripEventArgs> SizeGripMouseEntered,
                                   EventHandler<SizeGripEventArgs> SizeGripMouseLeft)
            {
                this.item = item;

                focusDecorator = new FocusDecorator(item);
                sizeGripDecorator = new SizeGripDecorator(item);

                sizeGripDecorator.SizeGripMouseEnter += SizeGripMouseEntered;
                sizeGripDecorator.SizeGripMouseLeave += SizeGripMouseLeft;

                item.AddDecorator(focusDecorator);
                item.AddDecorator(sizeGripDecorator);
            }

            CanvasItem item;

            public CanvasItem Item
            {
                get { return item; }
            }

            public bool Focused
            {
                get { return focusDecorator.Active; }
                set
                {
                    focusDecorator.Active = value;
                    sizeGripDecorator.Active = value;
                }
            }

            bool justGainedFocus;
            public bool JustGainedFocus
            {
                get { return justGainedFocus; }
                set { justGainedFocus = value; }
            }

            FocusDecorator focusDecorator;
            SizeGripDecorator sizeGripDecorator;

            public void Dispose()
            {
                item.RemoveDecorator(focusDecorator);
                item.RemoveDecorator(sizeGripDecorator);
            }
        }

        public CanvasItemData dragItemNode = null;
        public CanvasItemData hoverItemNode = null;
        public Route hoverRoute = null;
        public List<CanvasItemData> itemsList = new List<CanvasItemData>();
        public Dictionary<CanvasItem, CanvasItemData> itemsData = new Dictionary<CanvasItem, CanvasItemData>();
        public DiagramRouter diagramRouter = new DiagramRouter();

        public event EventHandler ZoomChanged = delegate { };
        float zoom = 1.0f;
        bool ctrlDown;
        bool holdRedraw;
        bool redrawNeeded;

        private PointF lastMouseClickPosition;

        private enum MultiObjectSelect
        {
            STAR, AREA_SELECTED, START_MOVING
        }

        private MultiObjectSelect multiSelectPhase = MultiObjectSelect.STAR;
        private PointF lastMouseDownPosition;//seperate, since changed by Mouse Click
        private Rectangle selectedArea = new Rectangle();
        private List<CanvasItemData> selectedItems = new List<CanvasItemData>();

        public List<PointF> temporaryNails = new List<PointF>();
        public PointF lastRightClickPosition = PointF.Empty;

        public const string PROCESS_NODE_NAME = "Process";
        public const string NAME_PROCESS_NODE_NAME = "Name";
        public const string PARAMETER_NODE_NAME = "Parameter";
        public const string ZOOM_PROCESS_NODE_NAME = "Zoom";
        public const string STATES_NODE_NAME = "States";
        public const string LINKS_NODE_NAME = "Links";
        public const string STATE_COUNTER = "StateCounter";

        public LTSCanvas()
        {
            //
            // The InitializeComponent() call is required for Windows Forms designer support.
            //
            InitializeComponent();

            //set the references
            Node = new TreeNode();
            Node.Tag = this;
        }

        public float Zoom
        {
            get { return zoom; }
            set
            {
                zoom = value;
                pictureBox1.Invalidate();
                LayoutChanged(this, EventArgs.Empty);
                ZoomChanged(this, EventArgs.Empty);
            }
        }

        public SizeF GetDiagramLogicalSize()
        {
            float w = 1, h = 1;
            foreach (CanvasItemData item in itemsList)
            {
                w = Math.Max(w, item.Item.X + item.Item.ActualWidth + item.Item.Border);
                h = Math.Max(h, item.Item.Y + item.Item.ActualHeight + item.Item.Border);
            }

            return new SizeF(w + Margine, h + Margine);
        }

        public Size GetDiagramPixelSize()
        {
            float zoom = Math.Max(this.zoom, 0.1f);
            SizeF size = GetDiagramLogicalSize();
            return new Size((int)(size.Width * zoom), (int)(size.Height * zoom));
        }

        public void SetRecommendedGraphicsAttributes(Graphics graphics)
        {
            if (graphics == null) return;

            graphics.CompositingQuality = CompositingQuality.HighSpeed;
            graphics.SmoothingMode = SmoothingMode.HighQuality;
            graphics.PageUnit = GraphicsUnit.Pixel;
            graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;
        }

        public void DrawToGraphics(Graphics graphics)
        {
            if (this.multiSelectPhase == MultiObjectSelect.AREA_SELECTED && !this.itemSelected)
                graphics.FillRectangle(Brushes.Aqua, this.selectedArea);

            foreach (CanvasItemData item in itemsList)
            {
                if (item.Item is StateItem)
                {
                    item.Item.DrawToGraphics(graphics);
                    continue;
                }

                if (item.Item is NailItem)
                {
                    //Only draw a nail if it is selected or the route it belongs is selected
                    NailItem nailItem = (NailItem)item.Item;
                    if (item.Focused || nailItem.Route.CurrentState != ItemState.Free)
                        item.Item.DrawToGraphics(graphics);

                    continue;
                }

                if (item.Item is Transition)
                {
                    item.Item.DrawToGraphics(graphics);
                    continue;
                }
            }

            DrawRoutes(graphics);

            //Draw temporary nail
            if (this.temporaryNails.Count > 1)
            {
                PointF startPoint = GraphUltility.FindPointByDistance(this.temporaryNails[0], this.temporaryNails[1], StateItem.R + 2);
                graphics.DrawLine(Pens.Black, startPoint, this.temporaryNails[1]);
                for (int i = 1; i < this.temporaryNails.Count - 1; i++)
                    graphics.DrawLine(Pens.Black, this.temporaryNails[i], this.temporaryNails[i + 1]);
            }
        }

        private void PictureBox1Paint(object sender, PaintEventArgs e)
        {
            if (itemsList.Count > 0)
            {
                Size bbox = GetDiagramPixelSize();

                pictureBox1.Width = Math.Max(panel1.Width - 3, bbox.Width + 100);
                pictureBox1.Height = Math.Max(panel1.Height - 3, bbox.Height + 100);
            }
            else
            {
                pictureBox1.Width = panel1.Width - 3;
                pictureBox1.Height = panel1.Height - 3;
            }

            e.Graphics.PageScale = zoom;
            SetRecommendedGraphicsAttributes(e.Graphics);
            DrawToGraphics(e.Graphics);
        }

        public static Color SELECT_COLOR = Color.Red;
        private void DrawRoutes(Graphics g)
        {
            foreach (Route route in diagramRouter.Routes)
                route.DrawToGraphics(g);
        }

        private CanvasItemData FindCanvasItemNode(PointF pos)
        {
            foreach (CanvasItemData item in itemsList)
            {
                if (item.Item.HitTest(pos))
                    return item;
            }

            return null;
        }

        #region Diagram Items Drag and Selection
        private void PictureBox1MouseClick(object sender, MouseEventArgs e)
        {
            PointF pos = new PointF(e.X / zoom, e.Y / zoom);
            lastMouseClickPosition = pos;
            CanvasItemData itemNode = FindCanvasItemNode(pos);
            if (itemNode != null)
            {
                itemNode.Item.HandleMouseClick(pos);
                if (itemNode.Focused)
                {
                    if (itemNode.JustGainedFocus)
                    {
                        itemNode.JustGainedFocus = false;
                    }
                    else if (itemNode.Item.StartEditing())
                    {
                        Control ec = itemNode.Item.GetEditingControl();
                        if (ec != null)
                        {
                            //TODO - refactor this damn thing... why couldn't they make the "Scale" scale the font as well?
                            ec.Scale(new SizeF(zoom, zoom));
                            Font ecf = ec.Font;
                            ec.Font = new Font(ecf.FontFamily,
                                               ecf.Size * zoom,
                                               ecf.Style, ec.Font.Unit,
                                               ecf.GdiCharSet, ec.Font.GdiVerticalFont);
                            ec.Hide();
                            ec.VisibleChanged += delegate { if (!ec.Visible) ec.Font = ecf; };
                            panel1.Controls.Add(ec);
                            ec.Top -= panel1.VerticalScroll.Value;
                            ec.Left -= panel1.HorizontalScroll.Value;
                            ec.Show();
                            panel1.Controls.SetChildIndex(ec, 0);
                            this.ActiveControl = ec;
                            ec.Focus();
                        }
                    }
                }
            }
            this.RefreshPictureBox();
        }

        void pictureBox1_MouseDoubleClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            PointF pos = new PointF(e.X / zoom, e.Y / zoom);
            lastMouseClickPosition = pos;
            CanvasItemData itemNode = FindCanvasItemNode(pos);
            if (itemNode != null)
            {
                if (itemNode.Item is LabelItem)
                {
                    CanvasItemData state = (itemNode.Item as LabelItem).FindStateDataBasedOnLabelItem(this);
                    ItemDoubleClick(this, new CanvasItemEventArgs(state));
                }
                else if (itemNode.Item is Transition)
                {
                    Route route = (itemNode.Item as Transition).FindSelectedRouteBasedOnTransition(this);
                    RouteDoubleClick(this, new CanvasRouteEventArgs(route));
                }
                else
                {
                    ItemDoubleClick(this, new CanvasItemEventArgs(itemNode));
                }
            }
            else
            {
                foreach (Route route in diagramRouter.Routes)
                {
                    if (route.IsBelongRoute(pos))
                    {
                        RouteDoubleClick(this, new CanvasRouteEventArgs(route));
                        break;
                    }
                }
            }
        }

        public bool itemSelected = false;

        private void PictureBox1MouseDown(object sender, MouseEventArgs e)
        {
            itemSelected = false;
            HoldRedraw = true;
            PointF pos = new PointF(e.X / zoom, e.Y / zoom);
            if (this.multiSelectPhase == MultiObjectSelect.AREA_SELECTED)
            {
                if (GraphUltility.IsBelongRectangle(this.selectedArea, pos))
                {
                    this.multiSelectPhase = MultiObjectSelect.START_MOVING;
                    foreach (CanvasItemData selectedItem in this.selectedItems)
                    {
                        selectedItem.JustGainedFocus = true;
                        selectedItem.Focused = true;
                        itemsList.Remove(selectedItem);
                        itemsList.Add(selectedItem);
                        selectedItem.Item.HandleMouseDown(new PointF(selectedItem.Item.X, selectedItem.Item.Y));
                    }
                    this.CanvasItemsSelected(this, new CanvasItemsEventArgs(this.selectedItems));
                }
                else
                {
                    this.multiSelectPhase = MultiObjectSelect.STAR;
                }
            }
            lastMouseDownPosition = pos;
            lastMouseClickPosition = pos;
            if (e.Button == MouseButtons.Right)
            {
                this.lastRightClickPosition = pos;
            }

            CanvasItemData itemNode = FindCanvasItemNode(pos);
            dragItemNode = itemNode;

            if (!ctrlDown)
            {
                foreach (CanvasItemData item in itemsList)
                {
                    item.Item.StopEditing();
                    if (itemNode == null || item != itemNode)
                    {
                        item.Focused = false;
                        if (item.Item is StateItem)
                        {
                            (item.Item as StateItem).CurrentState = ItemState.Free;
                        }
                    }
                }
            }

            foreach (Route route in diagramRouter.Routes)
            {
                route.CurrentState = ItemState.Free;
            }

            if (itemNode != null)
            {
                if (!itemNode.Focused)
                {
                    itemNode.JustGainedFocus = true;
                    itemNode.Focused = true;
                    itemsList.Remove(itemNode);
                    itemsList.Add(itemNode);
                    itemNode.Item.HandleMouseDown(pos);
                    itemSelected = true;
                    CanvasItemSelected(this, new CanvasItemEventArgs(itemNode));
                }
                else
                {
                    itemSelected = true;
                    itemNode.Item.HandleMouseDown(pos);
                    CanvasItemSelected(this, new CanvasItemEventArgs(itemNode));
                }
            }
            else
            {
                foreach (Route route in diagramRouter.Routes)
                {
                    if (route.IsBelongRoute(pos))
                    {
                        route.CurrentState = ItemState.Selected;
                        CanvasRouteSelected(this, new CanvasRouteEventArgs(route));
                        this.itemSelected = true;
                        break;
                    }
                }
                if (!this.itemSelected)
                {
                    CanvasItemSelected(this, new CanvasItemEventArgs(null));
                }
            }

            HoldRedraw = false;
            this.RefreshPictureBox();
        }

        private void PictureBox1MouseMove(object sender, MouseEventArgs e)
        {
            HoldRedraw = true;
            PointF pos = new PointF(e.X / zoom, e.Y / zoom);
            if (dragItemNode != null)
            {
                dragItemNode.Item.HandleMouseMove(pos);
            }
            else if (this.multiSelectPhase == MultiObjectSelect.START_MOVING)
            {
                float xOffSet = pos.X - lastMouseDownPosition.X;
                float yOffSet = pos.Y - lastMouseDownPosition.Y;
                foreach (CanvasItemData selectedItem in this.selectedItems)
                {
                    float newPositionX = selectedItem.Item.X + xOffSet;
                    float newPositionY = selectedItem.Item.Y + yOffSet;
                    selectedItem.Item.MoveFast(newPositionX, newPositionY);
                }

                this.lastMouseDownPosition = pos;
            }
            else
            {
                Route hoverRoute = null;
                CanvasItemData itemNode = FindCanvasItemNode(pos);
                if (itemNode == null)
                {
                    foreach (Route route in diagramRouter.Routes)
                    {
                        if (route.IsBelongRoute(pos))
                        {
                            hoverRoute = route;
                            break;
                        }
                    }
                }

                //only free when it is not selected and hover out
                //only hover when not selected and hover in
                if (itemNode == null && hoverRoute == null)
                {
                    if (this.hoverItemNode != null)
                        this.hoverItemNode.Item.HandleMouseHoverOut(this);

                    if (this.hoverRoute != null)
                        this.hoverRoute.HandleMouseHoverOut(this);
                }
                else if (itemNode != null && itemNode != this.hoverItemNode)
                {
                    if (this.hoverItemNode != null)
                        this.hoverItemNode.Item.HandleMouseHoverOut(this);

                    itemNode.Item.HandleMouseHoverIn(this);

                    if (this.hoverRoute != null)
                        this.hoverRoute.HandleMouseHoverOut(this);
                }
                else if (hoverRoute != null && hoverRoute != this.hoverRoute)
                {
                    if (this.hoverItemNode != null)
                        this.hoverItemNode.Item.HandleMouseHoverOut(this);

                    if (this.hoverRoute != null)
                        this.hoverRoute.HandleMouseHoverOut(this);

                    hoverRoute.HandleMouseHoverIn(this);
                }

                this.hoverItemNode = itemNode;
                this.hoverRoute = hoverRoute;
                this.Refresh();
                this.RefreshPictureBox();
            }

            HoldRedraw = false;
        }

        private void PictureBox1MouseUp(object sender, MouseEventArgs e)
        {
            PointF pos = new PointF(e.X / zoom, e.Y / zoom);
            switch (this.multiSelectPhase)
            {
                case MultiObjectSelect.STAR:
                    if (!(pos.X == lastMouseDownPosition.X && pos.Y == lastMouseDownPosition.Y))
                    {
                        this.multiSelectPhase = MultiObjectSelect.AREA_SELECTED;
                        this.selectedArea.X = (int)Math.Min(lastMouseDownPosition.X, pos.X);
                        this.selectedArea.Y = (int)Math.Min(lastMouseDownPosition.Y, pos.Y);
                        this.selectedArea.Width = (int)Math.Abs(lastMouseDownPosition.X - pos.X);
                        this.selectedArea.Height = (int)Math.Abs(lastMouseDownPosition.Y - pos.Y);

                        List<CanvasItemData> selectedItems = new List<CanvasItemData>();
                        foreach (CanvasItemData item in itemsList)
                        {
                            if (GraphUltility.IsBelongRectangle(this.selectedArea, new PointF(item.Item.X, item.Item.Y)))
                                selectedItems.Add(item);
                        }

                        this.selectedItems.Clear();
                        this.selectedItems.AddRange(selectedItems);

                        foreach (CanvasItemData selectedItem in this.selectedItems)
                            selectedItem.Item.HandleMouseDown(new PointF(selectedItem.Item.X, selectedItem.Item.Y));

                        CanvasItemsSelected(this, new CanvasItemsEventArgs(selectedItems));
                        this.Refresh();
                        this.RefreshPictureBox();
                    }
                    break;

                case MultiObjectSelect.START_MOVING:
                    this.SaveCurrentCanvas(this, EventArgs.Empty);
                    FinishMultiObjectAction();
                    break;

                default:
                    break;
            }


            if (dragItemNode != null)
            {
                dragItemNode.Item.HandleMouseUp(pos);
                if (this.lastMouseDownPosition != pos)
                    this.SaveCurrentCanvas(this, EventArgs.Empty);
            }
            dragItemNode = null;

            CanvasItemData itemNode = FindCanvasItemNode(pos);
            if (itemNode != null)
                itemNode.Item.HandleMouseUp(pos);
        }
        #endregion

        private bool HoldRedraw
        {
            get { return holdRedraw; }
            set
            {
                holdRedraw = value;
                if (!value && redrawNeeded)
                {
                    redrawNeeded = false;
                    HandleRedraw(this, EventArgs.Empty);
                }
            }
        }

        private void HandleItemLayoutChange(object sender, EventArgs args)
        {
            LayoutChanged(this, args);

            if (HoldRedraw)
                redrawNeeded = true;
            else
                HandleRedraw(sender, args);
        }

        private void HandleRedraw(object sender, EventArgs args)
        {
            if (HoldRedraw)
            {
                redrawNeeded = true;
                return;
            }

            this.Invalidate(true);
        }

        private void HandleItemPositionChange(object sender, ValueChangingEventArgs<PointF> args)
        {
            PointF pos = new PointF(args.Value.X, args.Value.Y);
            pos.X = Math.Max((float)Math.Round(pos.X / 10.0f) * 10.0f, Margine);
            pos.Y = Math.Max((float)Math.Round(pos.Y / 10.0f) * 10.0f, Margine);

            args.Cancel = (pos.X == args.Value.X) && (pos.Y == args.Value.Y);
            args.Value = pos;

            LayoutChanged(this, EventArgs.Empty);
        }

        private void HandleItemSizeChange(object sender, ValueChangingEventArgs<SizeF> args)
        {
            SizeF size = new SizeF(args.Value);
            size.Width = (float)Math.Round(size.Width / 10.0f) * 10.0f;
            size.Height = (float)Math.Round(size.Height / 10.0f) * 10.0f;

            args.Cancel = (size.Width == args.Value.Width) && (size.Height == args.Value.Height);
            args.Value = size;

            LayoutChanged(this, EventArgs.Empty);
        }

        private void SizeGripMouseEntered(object sender, SizeGripEventArgs e)
        {
            //TODO TKN We do not allow to change size
            //if ((e.GripPosition & SizeGripPositions.EastWest) != SizeGripPositions.None)
            //{
            //    pictureBox1.Cursor = Cursors.SizeWE;
            //}
            //else if ((e.GripPosition & SizeGripPositions.NorthSouth) != SizeGripPositions.None)
            //{
            //    pictureBox1.Cursor = Cursors.SizeNS;
            //}
        }

        private void SizeGripMouseLeft(object sender, SizeGripEventArgs e)
        {
            pictureBox1.Cursor = Cursors.Default;
        }

        /// <summary>
        /// Do not call it directly
        /// </summary>
        public void AddSingleCanvasItem(CanvasItem item)
        {
            diagramRouter.AddItem(item);
            CanvasItemData itemData = new CanvasItemData(item, SizeGripMouseEntered, SizeGripMouseLeft);
            itemsData[item] = itemData;

            itemsList.Add(itemData);
            item.RedrawNeeded += HandleRedraw;
            item.LayoutChanged += HandleItemLayoutChange;
            item.PositionChanging += HandleItemPositionChange;
            item.SizeChanging += HandleItemSizeChange;

            LayoutChanged(this, EventArgs.Empty);
        }

        public void RemoveSingleCanvasItem(CanvasItem item)
        {
            if (itemsList.Contains(itemsData[item]))
                itemsList.Remove(itemsData[item]);

            diagramRouter.RemoveItem(item);
        }

        public void AddSingleLink(Route route)
        {
            diagramRouter.AddRoute(route);
        }

        public void RemoveSingleRoute(Route route)
        {
            diagramRouter.RemoveRoute(route);
        }

        public void AddCanvasItem(CanvasItem item)
        {
            item.AddToCanvas(this);
            LayoutChanged(this, EventArgs.Empty);
        }

        public void RemoveCanvasItem(CanvasItem item)
        {
            item.RemovedFromCanvas(this);
            LayoutChanged(this, EventArgs.Empty);
        }

        public void AddLink(Route route)
        {
            route.AddToCanvas(this);
            LayoutChanged(this, EventArgs.Empty);
        }

        public void RemoveCanvasRoute(Route route)
        {
            route.RemovedFromCanvas(this);
            LayoutChanged(this, EventArgs.Empty);
        }

        public void ClearCanvas()
        {
            itemsList.Clear();
            dragItemNode = null;
            hoverItemNode = null;
            hoverRoute = null;
            diagramRouter.Clear();
        }

        public void ResetCanvas()
        {
            itemsList = new List<CanvasItemData>();
            dragItemNode = null;
            hoverItemNode = null;
            hoverRoute = null;
            diagramRouter = new DiagramRouter();
        }

        /// <summary>
        /// Retruns a copy of the the canvas items list as an array.
        /// </summary>
        public CanvasItem[] GetCanvasItems()
        {
            CanvasItem[] items = new CanvasItem[itemsList.Count];
            int i = 0;
            foreach (CanvasItemData item in itemsList)
                items[i++] = item.Item;

            return items;
        }

        public CanvasItem GetSelectedItem()
        {
            foreach (CanvasItemData item in itemsList)
            {
                if (item.Focused)
                    return item.Item;
            }

            return null;
        }

        public bool ContainsLink(CanvasItem from, CanvasItem to)
        {
            foreach (Route route in diagramRouter.Routes)
            {
                if (route.From == from && route.To == to)
                    return true;
            }

            return false;
        }

        public void AutoArrange()
        {
            diagramRouter.RecalcPositions();
        }

        #region File Save/Load
        #region XML saving
        /// <summary>
        /// Write the based LTS properties to XML
        /// </summary>
        /// <param name="doc">XML document</param>
        /// <param name="canvasElement">The canvas hosting element</param>
        protected virtual void WriteBaseProperties(XmlDocument doc, XmlElement canvasElement)
        {
            XmlAttribute attributeOfProcess = doc.CreateAttribute(NAME_PROCESS_NODE_NAME); // Non-Standard attribute
            attributeOfProcess.Value = Node.Text;
            canvasElement.Attributes.Append(attributeOfProcess);

            attributeOfProcess = doc.CreateAttribute(PARAMETER_NODE_NAME); // Non-Standard attribute
            attributeOfProcess.Value = this.Parameters.ToString(System.Globalization.CultureInfo.InvariantCulture);
            canvasElement.Attributes.Append(attributeOfProcess);

            attributeOfProcess = doc.CreateAttribute(ZOOM_PROCESS_NODE_NAME); // Non-Standard attribute
            attributeOfProcess.Value = Zoom.ToString(System.Globalization.CultureInfo.InvariantCulture);
            canvasElement.Attributes.Append(attributeOfProcess);

            //state counter
            attributeOfProcess = doc.CreateAttribute(STATE_COUNTER);
            attributeOfProcess.Value = this.StateCounter.ToString(System.Globalization.CultureInfo.InvariantCulture);
            canvasElement.Attributes.Append(attributeOfProcess);
        }

        /// <summary>
        /// Write the State nodes to XML
        /// </summary>
        /// <param name="doc">XML document</param>
        /// <param name="canvasElement">The canvas hosting element</param>
        protected virtual void WriteStates(XmlDocument doc, XmlElement canvasElement)
        {
            XmlElement statesElement = doc.CreateElement(STATES_NODE_NAME);
            canvasElement.AppendChild(statesElement);
            foreach (CanvasItemData item in itemsList)
            {
                if (item.Item is StateItem)
                    statesElement.AppendChild(item.Item.WriteToXml(doc));
            }
        }

        /// <summary>
        /// Write the Link nodes to XML
        /// </summary>
        /// <param name="doc">XML document</param>
        /// <param name="canvasElement">The canvas hosting element</param>
        protected virtual void WriteLinks(XmlDocument doc, XmlElement canvasElement)
        {
            XmlElement linksElement = doc.CreateElement(LINKS_NODE_NAME);
            canvasElement.AppendChild(linksElement);
            foreach (Route route in diagramRouter.Routes)
                linksElement.AppendChild(route.WriteToXml(doc));
        }

        public virtual XmlElement WriteToXml(XmlDocument doc)
        {
            XmlElement canvasElement = doc.CreateElement(PROCESS_NODE_NAME);

            // write base properties
            WriteBaseProperties(doc, canvasElement);

            // write State nodes
            WriteStates(doc, canvasElement);

            // write Link nodes
            WriteLinks(doc, canvasElement);

            return canvasElement;
        }
        #endregion

        #region XML loading
        protected virtual void LoadBasedProperty(XmlElement elem)
        {
            Node.Text = elem.GetAttribute(NAME_PROCESS_NODE_NAME, "");
            Parameters = elem.GetAttribute(PARAMETER_NODE_NAME, "");
            Zoom = float.Parse(elem.GetAttribute(ZOOM_PROCESS_NODE_NAME, ""));
            try
            {
                StateCounter = int.Parse(elem.GetAttribute(STATE_COUNTER));
            }
            catch
            {
                StateCounter = elem.ChildNodes[0].ChildNodes.Count + 1;
            }
        }

        protected virtual void LoadStates(XmlElement elem)
        {
            XmlElement statesElement = (XmlElement)elem.ChildNodes[0];
            foreach (XmlElement element in statesElement.ChildNodes)
            {
                StateItem canvasitem = new StateItem(false, "");
                canvasitem.LoadFromXml(element);
                this.AddSingleCanvasItem(canvasitem);
                this.AddSingleCanvasItem(canvasitem.labelItems);
            }
        }

        protected virtual Route LoadRoute(XmlElement element)
        {
            Route route = new Route(null, null);
            route.LoadFromXML(element, this);

            return route;
        }

        protected virtual void LoadLinks(XmlElement elem)
        {
            XmlElement linksElement = (XmlElement)elem.ChildNodes[1];
            Route route = null;
            foreach (XmlElement element in linksElement.ChildNodes)
            {
                route = LoadRoute(element);

                this.AddSingleLink(route);
                foreach (NailItem nailItem in route.Nails)
                    this.AddSingleCanvasItem(nailItem);

                this.AddSingleCanvasItem(route.Transition);
            }
        }

        public virtual void LoadFromXml(XmlElement elem)
        {
            LoadBasedProperty(elem);

            LoadStates(elem);

            LoadLinks(elem);
        }
        #endregion

        public virtual string ToSpecificationString()
        {
            StringBuilder sb = new StringBuilder();
            StateItem.StateCounterSpec = 0;
            string InitState = "";

            foreach (CanvasItemData item in itemsList)
            {
                if (item.Item is StateItem && (item.Item as StateItem).IsInitialState)
                    InitState = (item.Item as StateItem).GetName();
            }

            if (string.IsNullOrEmpty(InitState))
                throw new GraphParsingException(Node.Text, "There is no initial state for process " + Node.Text + "!", -1, -1, "");

            sb.AppendLine("Process \"" + Node.Text + "\"(" + Parameters + ")[\"" + InitState + "\"]:");

            foreach (CanvasItemData item in itemsList)
            {
                if (item.Item is StateItem)
                {
                    if ((item.Item as StateItem).IsInitialState)
                        sb.AppendLine((item.Item as StateItem).ToSpecificationString());
                }
            }

            foreach (CanvasItemData item in itemsList)
            {
                if (item.Item is StateItem)
                {
                    if (!(item.Item as StateItem).IsInitialState)
                        sb.AppendLine((item.Item as StateItem).ToSpecificationString());
                }
            }


            foreach (Route route in diagramRouter.Routes)
                sb.AppendLine("\"" + (route.From as StateItem).GetName() + "\"--" + route.Transition.ToSpecificationString() + "-->\"" + (route.To as StateItem).GetName() + "\"");

            sb.AppendLine(";");

            return sb.ToString();
        }
        #endregion

        public event EventHandler LayoutChanged = delegate { };
        public event EventHandler<CanvasItemEventArgs> CanvasItemSelected = delegate { };
        public event EventHandler<CanvasItemsEventArgs> CanvasItemsSelected = delegate { };
        public event EventHandler<CanvasRouteEventArgs> CanvasRouteSelected = delegate { };
        public event EventHandler<CanvasItemEventArgs> ItemDoubleClick = delegate { };
        public event EventHandler<CanvasRouteEventArgs> RouteDoubleClick = delegate { };
        public event EventHandler SaveCurrentCanvas = delegate { };

        public Bitmap GetAsBitmap()
        {
            Size bbox = GetDiagramPixelSize();
            Bitmap bitmap = new Bitmap(bbox.Width + 30, bbox.Height + 30);
            Graphics g = Graphics.FromImage(bitmap);
            g.PageScale = zoom;
            SetRecommendedGraphicsAttributes(g);
            DrawToGraphics(g);

            return bitmap;
        }

        public void SaveToImage(string filename)
        {
            GetAsBitmap().Save(filename);
        }

        public PointF LastMouseClickPosition
        {
            get { return lastMouseClickPosition; }
        }

        #region Drag/Drop from Class Browser Handling

        private void ClassCanvasDragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Link;
        }

        private void ClassCanvasDragDrop(object sender, DragEventArgs e)
        {

        }

        #endregion

        void ClassCanvasKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Control)
                ctrlDown = true;
        }

        void ClassCanvasKeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Control)
                ctrlDown = false;
        }

        public StateItem FindState(string name)
        {
            foreach (CanvasItemData item in itemsList)
            {
                if (item.Item is StateItem)
                {
                    StateItem state = (StateItem)item.Item;
                    if (state.Name == name)
                        return state;
                }
            }

            throw new Exception("Incorrect state name");
        }

        public XmlElement Clone()
        {
            XmlDocument document = new XmlDocument();
            return this.WriteToXml(document);
        }

        public void Restore(XmlElement canvas)
        {
            this.diagramRouter.Clear();
            this.itemsData.Clear();
            this.itemsList.Clear();
            this.temporaryNails.Clear();
            this.LoadFromXml(canvas);
            this.Refresh();
        }

        public LTSCanvas Duplicate()
        {
            LTSCanvas duplicate = new LTSCanvas();
            duplicate.LoadFromXml(this.Clone());
            duplicate.Node.Text = this.Node.Text + "-Copy";

            bool nameExist = true;
            while (nameExist)
            {
                nameExist = false;
                foreach (TreeNode node in this.Node.Parent.Nodes)
                {
                    if (node.Text.Equals(duplicate.Node.Text, StringComparison.CurrentCultureIgnoreCase))
                    {
                        duplicate.Node.Text = duplicate.Node.Text + "-Copy";
                        nameExist = true;
                        break;
                    }
                }
            }

            return duplicate;
        }

        public void FinishMultiObjectAction()
        {
            this.multiSelectPhase = MultiObjectSelect.STAR;
            this.selectedItems.Clear();
            this.Refresh();
        }

        /// <summary>
        /// For Linux, Max running with Mono
        /// </summary>
        public void RefreshPictureBox()
        {
            this.pictureBox1.Refresh();
        }

        public bool CheckStateNameDuplicate()
        {
            List<string> names = new List<string>();
            foreach (CanvasItemData item in itemsList)
            {
                if (item.Item is StateItem)
                {
                    if (names.Contains((item.Item as StateItem).Name))
                    {
                        MessageBox.Show(Resources.No_duplicated_state_names_are_allowed_, Resources.Error,
                            MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                        return false;
                    }

                    names.Add((item.Item as StateItem).Name);
                }
            }

            return true;
        }
    }

    public class CanvasRouteEventArgs : EventArgs
    {
        public CanvasRouteEventArgs(Route canvasItem)
        {
            this.canvasRoute = canvasItem;
        }

        private Route canvasRoute;

        public Route CanvasItem
        {
            get { return canvasRoute; }
        }
    }

    public class CanvasItemEventArgs : EventArgs
    {
        public CanvasItemEventArgs(LTSCanvas.CanvasItemData canvasItem)
        {
            this.canvasItem = canvasItem;
        }

        private LTSCanvas.CanvasItemData canvasItem;

        public LTSCanvas.CanvasItemData CanvasItem
        {
            get { return canvasItem; }
        }
    }

    public class CanvasItemsEventArgs : EventArgs
    {
        public CanvasItemsEventArgs(List<LTSCanvas.CanvasItemData> canvasItems)
        {
            this.canvasItems = canvasItems;
        }

        private List<LTSCanvas.CanvasItemData> canvasItems;

        public List<LTSCanvas.CanvasItemData> CanvasItem
        {
            get { return canvasItems; }
        }
    }

    public class ColorDefinition
    {
        public static Color GetColorWhenSelected()
        {
            return Color.Red;
        }

        public static Color GetColorWhenHover()
        {
            return Color.Blue;
        }

        public static Color GetColorWhenFree()
        {
            return Color.Black;
        }

        //Transition
        public static Color GetColorOfSelect()
        {
            return Color.FromArgb(255, 192, 0);
        }

        public static Color GetColorOfClockGuard()
        {
            return Color.FromArgb(0, 153, 0);
        }

        public static Color GetColorOfGuard()
        {
            return Color.FromArgb(0, 0, 153);
        }

        public static Color GetColorOfEvent()
        {
            return Color.FromArgb(0, 238, 238);
        }

        public static Color GetColorOfProgram()
        {
            return Color.FromArgb(51, 0, 204);
        }

        public static Color GetColorOfClockReset()
        {
            return Color.FromArgb(51, 255, 0);
        }

        //State
        public static Color GetColorOfName()
        {
            return Color.FromArgb(153, 0, 153);
        }

        public static Color GetColorOfInvariant()
        {
            return Color.FromArgb(204, 0, 153);
        }
        public static Color GetColorToFillState()
        {
            return Color.Silver;
        }
    }

    public enum ItemState
    {
        Free,
        Selected,
        Hover
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix")]
    public enum TestEnum { Dog, Cat, Fish };
}