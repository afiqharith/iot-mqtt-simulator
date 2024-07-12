using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HardwareSimMqtt.HardwareHub
{
    public interface IComController
    {
        uint BitMask
        {
            get;
            set;
        }
        eControllerType ControllerType 
        { 
            get;
        }
        bool OpenPort();
        void SendDigitalCommand(uint bitState);
        void SendAnalogCommand(double analogData);
    }

    public enum eControllerType
    {
        GPIO,
        SerialPort,
        EmuGPIO,
    }
}
