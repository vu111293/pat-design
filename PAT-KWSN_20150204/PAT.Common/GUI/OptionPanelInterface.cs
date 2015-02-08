using System.Windows.Forms;

namespace PAT.Common.GUI
{
    public class OptionPanelInterface : UserControl
    {
        public virtual void LoadData()
        {}
        public virtual void WriteData()
        {}
        public virtual void ResetDefaultValue()
        {}
    }
}
