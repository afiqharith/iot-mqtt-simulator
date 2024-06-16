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

    public interface IHardware
    {
        string Id { get; set; }
        uint CurrentState { get; set; }

    }

    public class HardwareBase
    {
        public virtual string Id { get; private set; }
        public virtual HW_TYPE Type { get; private set; }
        public virtual LOC Loc { get; private set; }
        private Panel pPanel { get;  set; }

        private uint _currentState = 0;
        public virtual uint CurrentState 
        {
            get { return _currentState; } 
            set
            {
                SetCurrentStateProperty(ref _currentState, value);
            }
        }

        public HardwareBase(Panel panel, HW_TYPE type, LOC loc, string id)
        {
            this.Id = id;
            this.Type = type;
            this.Loc = loc;
            this.pPanel = panel;
        }

        public virtual bool Connect()
        {
            return true;
        }

        private void SetCurrentStateProperty(ref uint state, uint newval)
        {
            switch (newval)
            {
                case 0x1:
                    this.pPanel.BackColor = Color.Green;
                    //this.CurrentState = state;
                    state = newval;
                    break;

                case 0x0:
                default:
                    this.pPanel.BackColor = Color.Red;
                    //this.CurrentState = 0x0;
                    state = 0x0;
                    break;
            }
        }

#warning Deprecated code in this method.
        public virtual uint GetCurrentState()
        {
            return this.CurrentState;
        }
    }
}
