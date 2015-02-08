using System.Windows.Forms;
using Fireball.Docking;

namespace PAT.GUI.Docking
{
    public class EditDocumentWindow : DockableWindow
    {
        public EditDocumentWindow()
        {
            this.Padding = new Padding(1,2,1,1);
            this.DockableAreas = DockAreas.Document;
        }
    }
}