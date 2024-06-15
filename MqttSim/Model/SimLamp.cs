using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MqttSim
{
    internal class SimpLamp : HardwareBase
    {
        public SimpLamp(Panel panel, LOC loc, string id) 
            : base(panel, HW_TYPE.LAMP, loc, id) { }
    }
}
