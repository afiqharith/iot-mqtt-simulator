using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MqttSim
{
    public enum HW_TYPE
    {
        LAMP,
        FAN
    }

    public enum LOC
    {
        Loc1,
        Loc2,
        Loc3,
        Loc4
    }

    public class IHardwareBase
    {
        string Id { get; set; }
        bool CurrentState { get; set; }

    }

    public class HardwareBase
    {
        public string Id { get; private set; }
        public HW_TYPE Type { get; private set; }
        public LOC Loc { get; private set; }
        private uint CurrentState { get; set; }
        private Panel pPanel { get;  set; }

        public HardwareBase(Panel panel, HW_TYPE type, LOC loc, string id)
        {
            this.Id = id;
            this.Type = type;
            this.Loc = loc;
            this.pPanel = panel;
        }
        public HardwareBase() { }

        public virtual bool Connect()
        {
            return true;
        }

        public virtual void SetCurrentState(uint state)
        {
            switch (state)
            {
                case 0x1:
                    this.pPanel.BackColor = Color.Green;
                    this.CurrentState = state;
                    break;

                case 0x0:
                default:
                    this.pPanel.BackColor = Color.Red;
                    this.CurrentState = 0x0;
                    break;
            }
        }

        public virtual uint GetCurrentState()
        {
            return this.CurrentState;
        }
    }
}
