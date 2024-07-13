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

        eIoType IoType
        {
            get;
        }

        bool OpenPort();
        void SendDigitalOutputCommand(uint bitState);
        bool GetDigitalInputValue();

        void SendAnalogOutputCommand(int analogData);
        int GetAnalogInputValue();
    }

    public enum eControllerType
    {
        GPIO,
        SerialPort,
        EmuGPIO,
    }

    public enum eIoType
    {
        DigitalInput,
        DigitalOutput,
        AnalogInput,
        AnalogOutput,
    }
}
