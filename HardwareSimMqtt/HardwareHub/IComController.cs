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
        ControllerType ControllerType 
        { 
            get;
        }

        IoType IoType
        {
            get;
        }

        bool OpenPort();
        void SendDigitalOutputCommand(uint bitState);
        bool GetDigitalInputValue();

        void SendAnalogOutputCommand(int analogData);
        int GetAnalogInputValue();
    }

    [Flags]
    public enum ControllerType
    {
        Invalid = -1,
        GPIO,
        SerialPort,
        EmuGPIO,
        GeneralGPIO = GPIO | EmuGPIO,
    }

    public enum IoType
    {
        Invalid = -1,
        DigitalInput,
        DigitalOutput,
        AnalogInput,
        AnalogOutput,
    }
}
