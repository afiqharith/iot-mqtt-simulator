using System.Windows.Forms;
using System.Drawing;
using BitMap;

namespace Model
{
    internal class SimLamp : HardwareBase
    {
        private Panel _pPanel = null;
        private Panel pPanel
        {
            get => _pPanel;
            set => SetPanelProperty(ref _pPanel, value);
        }

        public override uint BitState
        {
            set
            {
                base.BitState = value;
                this.pPanel.BackColor = GetUiBackColorIndicator(value);
            }
        }

        public SimLamp(Panel panel, eLOC location, string id, eBitMask mask, int ioPort)
            : base(eTYPE.LAMP, location, id, mask, ioPort)
        {
            this.pPanel = panel;
        }

        private void SetPanelProperty(ref Panel panel, Panel newval)
        {
            panel = newval;
        }

        private Color GetUiBackColorIndicator(uint bit)
        {
            return (bit & this.BitMask) == this.BitMask ? Color.Green : Color.Gray;
        }
    }
}
