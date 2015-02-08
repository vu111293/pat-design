using System.Drawing;

namespace PAT.Common.LTSModule.Drawing.Diagram.Interactivity
{
    public interface IHitTestable
    {
        bool HitTest (PointF pos);
    }
}