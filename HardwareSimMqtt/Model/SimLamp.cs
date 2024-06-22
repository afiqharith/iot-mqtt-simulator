using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Model
{
    using BitMap;
    using System.Drawing;

    internal class SimpLamp : HardwareBase
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
        public override uint CurrentState
        {
            set
            {
                base.CurrentState = value;
                switch (value)
                {
                    case 0x0001:
                        this.pPanel.BackColor = Color.Green;
                        break;

                    case 0x0000:
                    default:
                        this.pPanel.BackColor = Color.Gray;
                        break;
                }
            }
        }
        public SimpLamp(Panel panel, eLOC loc, string id, eBitMask mask)
            : base(eTYPE.LAMP, loc, id, mask)
        {
            this.pPanel = panel;
        }

        private void SetPanelProperty(ref Panel panel, Panel newval)
        {
            panel = newval;
        }
    }
}
