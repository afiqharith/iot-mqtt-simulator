using System;
using System.Diagnostics;
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

        private int _ioPort;
        private int IoPort
        {
            get => _ioPort;
            set
            {
                if (value != -1)
                {
                    _ioPort = value;
                }
                else
                {
                    throw new Exception(String.Format("Invalid I/O port number: {0}", value));
                }
            }
        }

        public virtual eControllerType ControllerType
        {
            get;
            private set;
        }

        public virtual eIoType IoType
        {
            get;
            private set;
        }

        public HHEmuGPIOController(eIoType ioType, int ioPort)
        {
            this.IoType = ioType;
            this.IoPort = ioPort;
            this.ControllerType = eControllerType.EmuGPIO;
            LibGpio.Gpio.TestMode = true;
        }

        public bool OpenPort()
        {
            bool bRet = false;
            try
            {
                if (!bRet)
                {
                    if (this.IoType == eIoType.DigitalInput)
                    {
                        SetDigitalInput(this.IoPort);
                        bRet = true;
                    }
                    else if (this.IoType == eIoType.DigitalOutput)
                    {
                        SetDigitalOutput(this.IoPort);
                        bRet = true;
                    }
                }
            }
            catch
            {
                String strException = String.Format("Unable to setup GPIO Pin {0}", this.IoPort);
                Debug.WriteLine(strException);
                throw new Exception(strException);
            }
            return bRet;
        }

        //Set digital output pin
        private void SetDigitalOutput(int pin)
        {
            LibGpio.Gpio.SetupChannel((BroadcomPinNumber)pin, Direction.Output);
        }

        //Set digital input pin
        private void SetDigitalInput(int pin)
        {
            LibGpio.Gpio.SetupChannel((BroadcomPinNumber)pin, Direction.Input);
        }

        //Send digital output command
        public void SendDigitalOutputCommand(uint bitState)
        {
            uint newBitState = this.BitMask & bitState;
            bool trigger = newBitState != 0 ? true : false;
            LibGpio.Gpio.OutputValue((BroadcomPinNumber)this.IoPort, trigger);
            Thread.Sleep(10);
        }

        //Get digital input value
        public bool GetDigitalInputValue()
        {
            return LibGpio.Gpio.ReadValue((BroadcomPinNumber)this.IoPort);
        }

        public void SendAnalogOutputCommand(int analogData) { }

        public int GetAnalogInputValue()
        {
            return 0;
        }
    }
}
