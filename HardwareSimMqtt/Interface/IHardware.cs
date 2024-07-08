using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HardwareSimMqtt.Interface
{
    internal interface IHardware: IHardwarePartial
    {
        uint BitMask
        {
            get;
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

    internal interface IHardwarePartial
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
