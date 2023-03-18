using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MqttSim
{
    internal class Lamp : SimulatedObjectBase
    {
        private uint _currentState;
        private Panel _panel;

        public Lamp(Panel panel, LOC loc, string id)
        {
            this.Id = id;
            this.Type = HW_TYPE.LAMP;
            this.Loc = loc;
            this._panel = panel;
        }
    }
}
