using System;
using System.Threading;
using PiSharp.LibGpio;
using PiSharp.LibGpio.Entities;

namespace HardwareSimMqtt.HardwareHub
{
    public class HHEmuGPIOController : IComController
    {
        public virtual uint BitMask
        {
            get;
            set;
        }

        private int IoPort
        {
            get;
            set;
        }

        public virtual eControllerType ControllerType
        {
            get;
            private set;
        }

        public HHEmuGPIOController(int ioPort)
        {
            this.IoPort = ioPort;
            this.ControllerType = eControllerType.EmuGPIO;
            LibGpio.Gpio.TestMode = true;
        }

        public bool OpenPort()
        {
            bool bRet = false;
            try
            {
                if (true)
                {
                    SetPinOutput(this.IoPort);
                }
            }
            catch
            {
                throw new Exception(String.Format("Unable to setup GPIO Pin {0}", this.IoPort));
            }
            return true;
        }

        private void SetPinOutput(int pin)
        {
            LibGpio.Gpio.SetupChannel((BroadcomPinNumber)pin, Direction.Output);
        }

        private void SetPinInput(int pin)
        {
            LibGpio.Gpio.SetupChannel((BroadcomPinNumber)pin, Direction.Input);
        }

        public void SendDigitalCommand(uint bitState)
        {
            uint newBitState = this.BitMask & bitState;
            bool trigger = newBitState != 0 ? true : false;
            LibGpio.Gpio.OutputValue((BroadcomPinNumber)this.IoPort, trigger);
            Thread.Sleep(10);
        }

        public void SendAnalogCommand(double analogData)
        { }
    }
}
