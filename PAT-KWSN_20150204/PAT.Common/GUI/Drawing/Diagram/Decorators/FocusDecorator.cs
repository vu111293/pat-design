using System.Drawing;
using System.Drawing.Drawing2D;
using Tools.Diagrams;

namespace PAT.Common.LTSModule.Drawing.Diagram.Decorators
{
    public class FocusDecorator : RectangleDecorator
    {
        public FocusDecorator (IRectangle rectangle) : base (rectangle) {}
		
        static Pen InitPen ()
        {
            Pen pen = new Pen(Color.Black);
            pen.DashStyle = DashStyle.Dot;
            return pen;
        }
		
        static Pen pen = InitPen();
		
        public override void DrawToGraphics(Graphics graphics)
        {
            if (graphics == null) return;

            graphics.DrawRectangle(pen,
                                   Rectangle.AbsoluteX - 4, Rectangle.AbsoluteY - 4,
                                   Rectangle.ActualWidth + 8, Rectangle.ActualHeight + 8);
        }
		
        public override void HandleMouseClick(PointF pos) { }
        public override void HandleMouseDown(PointF pos) { }
        public override void HandleMouseMove(PointF pos) { }
        public override void HandleMouseUp(PointF pos) { }
        public override void HandleMouseLeave() { }	
    }
}