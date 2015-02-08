using System;
using System.Drawing;

namespace PAT.Common.GUI.Drawing
{
    public class GraphUltility
    {
        public static float Distance(PointF p1, PointF p2)
        {
            float xDist = p1.X - p2.X;
            float yDist = p1.Y - p2.Y;
            return (float)Math.Sqrt(xDist * xDist + yDist * yDist);
        }

        /// <summary>
        /// Return point in line of start and end which is R far from start
        /// </summary>
        public static PointF FindPointByDistance(PointF start, PointF end, float R)
        {
            float distance = Distance(start, end);
            float x = 0;
            float y = 0;

            if (start == end)
            {
                x = start.X + R;
                y = start.Y;
            }
            else if (start.X == end.X)
            {
                x = start.X;
                y = start.Y + R / distance * (end.Y - start.Y);
            }
            else if (start.Y == end.Y)
            {
                x = start.X + R / distance * (end.X - start.X);
                y = start.Y;
            }
            else
            {
                x = start.X + R / distance * (end.X - start.X);
                y = start.Y + R / distance * (end.Y - start.Y);
            }
            return new PointF(x, y);
        }

        public static bool IsBelongRectangle(Rectangle rect, PointF point)
        {
            return (point.X >= rect.Left && point.X <= rect.Right && point.Y >= rect.Top && point.Y <= rect.Bottom);
        }
    }
}