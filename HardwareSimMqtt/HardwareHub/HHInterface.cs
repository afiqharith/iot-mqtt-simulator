using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HardwareSimMqtt.HardwareHub
{
    public interface HHInterface
    {
        uint BitMask
        {
            get;
            set;
        }
        bool OpenPort();
        void SendDigitalCommand(uint bitState);
        void SendAnalogCommand(double analogData);
    }
}
