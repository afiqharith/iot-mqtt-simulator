using System.Windows.Forms;

namespace Model
{
    using BitMap;
    using System.Drawing;

    internal class SimLamp : HardwareBase
    {
        private Panel _pPanel = null;
        private Panel pPanel
        {
            get { return _pPanel; }
            set
            {
                SetPanelProperty(ref _pPanel, value);
            }
        }

        public override uint CurrentBitState
        {
            set
            {
                base.CurrentBitState = value;
                this.pPanel.BackColor = (value & this.BitMask) == this.BitMask ? Color.Green : Color.Gray;
            }
        }

        public SimLamp(Panel panel, eLOC loc, string nID, eBitMask mask)
            : base(eTYPE.LAMP, loc, nID, mask)
        {
            this.pPanel = panel;
        }

        private void SetPanelProperty(ref Panel panel, Panel newval)
        {
            panel = newval;
        }
    }
}
