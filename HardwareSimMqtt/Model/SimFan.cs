using System.Windows.Forms;
using System.Drawing;
using BitMap;

namespace Model
{
    internal class SimFan : HardwareBase
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

        private double _speed = -1;
        public virtual double Speed
        {
            get => _speed;
            protected set => SetSpeedProperty(ref _speed, value);
        }

        public SimFan(Panel panel, eLOC location, string id, eBitMask mask)
            : base(eTYPE.FAN, location, id, mask)
        {
            this.pPanel = panel;
        }

        private void SetPanelProperty(ref Panel panel, Panel newval)
        {
            panel = newval;
        }

        private void SetSpeedProperty(ref double speed, double newval)
        {
            speed = newval;
            base.AnalogData = newval;
        }

        private Color GetUiBackColorIndicator(uint bit)
        {
            return (bit & this.BitMask) == this.BitMask ? Color.Green : Color.Gray;
        }
    }
}
