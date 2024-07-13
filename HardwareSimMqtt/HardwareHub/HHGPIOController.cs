using System;
using System.Threading;
using System.Device.Gpio;

namespace HardwareSimMqtt.HardwareHub
{
    public class HHGPIOController : IComController
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

        public virtual eIoType IoType
        {
            get;
            private set;
        }

        public virtual GpioController Controller
        {
            get;
            set;
        }

        public HHGPIOController(eIoType ioType, int ioPort)
        {
            this.IoType = ioType;
            this.IoPort = ioPort;
            this.ControllerType = eControllerType.GPIO;
            Controller = new GpioController();
        }

        public bool OpenPort()
        {
            bool bRet = false;
            try
            {
                if (!Controller.IsPinOpen(this.IoPort))
                {
                    if (this.IoType == eIoType.DigitalInput)
                    {
                        SetDigitalInput(this.IoPort);
                    }
                    else if (this.IoType == eIoType.DigitalOutput)
                    {
                        SetDigitalOutput(this.IoPort);
                    }
                }

                if (Controller.IsPinOpen(this.IoPort))
                {
                    bRet = true;
                }
            }
            catch
            {
                throw new Exception(String.Format("Unable to setup GPIO Pin {0}", this.IoPort));
            }
            return bRet;
        }

        //Set digital output pin
        private void SetDigitalOutput(int pin)
        {
            Controller.OpenPin(pin, PinMode.Output);
        }

        //Set digital input pin
        private void SetDigitalInput(int pin)
        {
            Controller.OpenPin(pin, PinMode.Input);
        }

        //Send digital output command
        public void SendDigitalOutputCommand(uint bitState)
        {
            uint newBitState = this.BitMask & bitState;
            PinValue pinValue = newBitState != 0 ? PinValue.High : PinValue.Low;
            Controller.Write(this.IoPort, pinValue);
            Thread.Sleep(10);
        }

        //Get digital input value
        public bool GetDigitalInputValue()
        {
            return Controller.Read(this.IoPort) == PinValue.High ? true : false;
        }

        public void SendAnalogOutputCommand(int analogData) { }

        public int GetAnalogInputValue()
        {
            return 0;
        }
    }
}
