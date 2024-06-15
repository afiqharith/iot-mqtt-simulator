using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MqttSim
{
    internal class SimFan: HardwareBase
    {
        public SimFan(Panel panel, LOC loc, string id) 
            : base(panel, HW_TYPE.FAN, loc, id) { }
    }
}
