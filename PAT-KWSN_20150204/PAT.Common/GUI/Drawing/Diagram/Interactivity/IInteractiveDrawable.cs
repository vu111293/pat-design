using Tools.Diagrams.Drawables;

namespace PAT.Common.LTSModule.Drawing.Diagram.Interactivity
{
    public interface IInteractiveDrawable : IDrawable, IHitTestable, IMouseInteractable
    {
    }

    public interface IInteractiveRectangle : IDrawableRectangle, IHitTestable, IMouseInteractable
    {
    }
}