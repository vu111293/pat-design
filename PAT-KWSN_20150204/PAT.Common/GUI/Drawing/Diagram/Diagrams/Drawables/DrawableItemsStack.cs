using System.Drawing;

namespace Tools.Diagrams.Drawables
{
	public class DrawableItemsStack : DrawableItemsStack<IDrawableRectangle> {}
	
	public class DrawableItemsStack<T>
		: ItemsStack<T>, IDrawableRectangle
		where T : IDrawable, IRectangle
	{
		public void DrawToGraphics(Graphics graphics)
		{
			Recalculate();
			foreach (IDrawable d in this)
				d.DrawToGraphics(graphics);
		}
	}
}
