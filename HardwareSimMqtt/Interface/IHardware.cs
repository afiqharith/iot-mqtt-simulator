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
        uint BitMask
        {
            get;
        }
        double AnalogData
        {
            get;
            set;
        }
        bool IsOn
        {
            get;
        }
        bool IsOff
        {
            get;
        }
        bool Connect();
        void On();
        void Off();
    }
}
