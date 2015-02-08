using System;
using PAT.GUI.Docking;

namespace PAT.GUI
{
    public class NewCodeEditorEventArgs:EventArgs
    {
        private EditorTabItem _EditorTabItem = null;

        public EditorTabItem EditorTabItem
        {
            get { 
                return _EditorTabItem;
            }
        }

        internal NewCodeEditorEventArgs(ref EditorTabItem tabItem)
        {
            _EditorTabItem = tabItem;
        }

    }
}