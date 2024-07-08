#define SIMULATE
using System;
using System.Threading;
using PiSharp.LibGpio;
using PiSharp.LibGpio.Entities;

namespace HardwareSimMqtt.HardwareHub
{
    public class HHGeneralPurposeIO : HHInterface
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

#if !SIMULATE
        public virtual GpioController Controller
        {
            get;
            set;
        }
#endif

        public HHGeneralPurposeIO(int ioPort)
        {
            this.IoPort = ioPort;
#if !SIMULATE
            Controller = new GpioController();
#else
            LibGpio.Gpio.TestMode = true;
#endif
        }

        public bool OpenPort()
        {
            bool bRet = false;
            try
            {
#if !SIMULATE
                if (!Controller.IsPinOpen(this.IoPort))
#else
                if (true)
#endif
                {
                    SetPinOutput(this.IoPort);
                }

#if !SIMULATE
                if (Controller.IsPinOpen(this.IoPort))
                {
                    bRet = true;
                }
#endif
            }
            catch
            {
                throw new Exception(String.Format("Unable to setup GPIO Pin {0}", this.IoPort));
            }

#if !SIMULATE
            return bRet;
#else
            return true;
#endif
        }

        private void SetPinOutput(int pin)
        {
#if !SIMULATE
            Controller.OpenPin(pin, PinMode.Output);
#else
            LibGpio.Gpio.SetupChannel((BroadcomPinNumber)pin, Direction.Output);
#endif
        }

        private void SetPinInput(int pin)
        {
#if !SIMULATE
            Controller.OpenPin(pin, PinMode.Input);
#else
            LibGpio.Gpio.SetupChannel((BroadcomPinNumber)pin, Direction.Input);
#endif
        }

        public void SendDigitalCommand(uint bitState)
        {
            uint newBitState = this.BitMask & bitState;
#if !SIMULATE
            PinValue pinValue = newBitState != 0 ? PinValue.High : PinValue.Low;
            Controller.Write(this.IoPort, pinValue);
#else
            bool trigger = newBitState != 0 ? true : false;
            LibGpio.Gpio.OutputValue((BroadcomPinNumber)this.IoPort, trigger);
            Thread.Sleep(10);
#endif
        }
        public void SendAnalogCommand(double analogData)
        { }
    }
}
