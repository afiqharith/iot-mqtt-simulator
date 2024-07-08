using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HardwareSimMqtt.Interface
{
    internal interface IHardware
    {
        string Id
        {
            get;
        }

        uint BitState
        {
            get;
            set;
        }

        double AnalogData
        {
            get;
            set;
        }
    }
}
