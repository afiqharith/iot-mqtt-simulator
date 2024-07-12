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

        public virtual GpioController Controller
        {
            get;
            set;
        }

        public HHGPIOController(int ioPort)
        {
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
                    SetPinOutput(this.IoPort);
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

        private void SetPinOutput(int pin)
        {
            Controller.OpenPin(pin, PinMode.Output);
        }

        private void SetPinInput(int pin)
        {
            Controller.OpenPin(pin, PinMode.Input);
        }

        public void SendDigitalCommand(uint bitState)
        {
            uint newBitState = this.BitMask & bitState;
            PinValue pinValue = newBitState != 0 ? PinValue.High : PinValue.Low;
            Controller.Write(this.IoPort, pinValue);
            Thread.Sleep(10);
        }

        public void SendAnalogCommand(double analogData)
        { }
    }
}
