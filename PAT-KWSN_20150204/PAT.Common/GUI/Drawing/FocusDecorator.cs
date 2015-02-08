using System.Drawing;
using System.Drawing.Drawing2D;
using PAT.Common.LTSModule.Drawing.Diagram.Decorators;
using Tools.Diagrams;

namespace PAT.Common.GUI.Drawing
{
    public class FocusDecorator : RectangleDecorator
    {
        public FocusDecorator (IRectangle rectangle) : base (rectangle) {}
		
        //static Pen InitPenRed()
        //{
        //    Pen pen = new Pen(Color.Red, 2);
        //    pen.DashStyle = DashStyle.DashDot;
        //    return pen;
        //}

        static Pen InitPenBlack()
        {
            Pen pen = new Pen(Color.Black);
            pen.DashStyle = DashStyle.Dot;
            return pen;
        }

        static Pen penBlack = InitPenBlack();
        // static Pen penRed = InitPenRed();
		
        public override void DrawToGraphics(Graphics graphics)
        {
            if (graphics == null) return;

            //if(LTSTabItem.HighLightControlUsingRed)
            //{
            //    graphics.DrawRectangle(penRed,
            //                       Rectangle.AbsoluteX - 4, Rectangle.AbsoluteY - 4,
            //                       Rectangle.ActualWidth + 8, Rectangle.ActualHeight + 8);
            //}
            //else
            //{
            graphics.DrawRectangle(penBlack,
                                   Rectangle.AbsoluteX - 4, Rectangle.AbsoluteY - 4,
                                   Rectangle.ActualWidth + 8, Rectangle.ActualHeight + 8);
            //}
        }
		
        public override void HandleMouseClick(PointF pos) { }
        public override void HandleMouseDown(PointF pos) { }
        public override void HandleMouseMove(PointF pos) { }
        public override void HandleMouseUp(PointF pos) { }
        public override void HandleMouseLeave() { }	
    }
}