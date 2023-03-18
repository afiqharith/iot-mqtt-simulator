using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MqttSim.Model
{
    internal class SimFan: SimulatedObjectBase
    {
        private Panel _panel;

        public SimFan(Panel panel, LOC loc, string id)
        {
            this.Id = id;
            this.Type = HW_TYPE.FAN;
            this.Loc = loc;
            this._panel = panel;
        }
    }
}
