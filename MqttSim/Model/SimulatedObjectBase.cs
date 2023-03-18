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

    internal class SimulatedObjectBase
    {
        public string Id;
        public HW_TYPE Type;
        public LOC Loc;
        private uint _currentState;
        private Panel _panel;

        public SimulatedObjectBase(Panel panel, HW_TYPE type, LOC loc, string id)
        {
            this.Id = id;
            this.Type = type;
            this.Loc = loc;
            this._panel = panel;
        }

        public SimulatedObjectBase()
        {

        }

        public virtual void SetCurrentState(uint state)
        {
            switch (state)
            {
                case 0x1:
                    _panel.BackColor = Color.Green;
                    _currentState = state;
                    break;

                case 0x0:
                default:
                    _panel.BackColor = Color.Red;
                    _currentState = 0x0;
                    break;
            }
        }

        public uint GetCurrentState()
        {
            return _currentState;
        }
    }
}
