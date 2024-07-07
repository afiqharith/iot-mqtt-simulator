#define SIMULATE
using System.IO.Ports;
using System;
using System.Threading;
using PiSharp.LibGpio;
using PiSharp.LibGpio.Entities;
using System.Device.Gpio;

namespace HardwareMap
{
    public interface HHInterface
    {
        bool OpenPort();
        void SendPacket(uint bitState);
        uint BitMask
        {
            get;
            set;
        }
    }

    public class HHSerialPort /*: HHInterface*/
    {
        protected string PortName
        {
            get;
            set;
        }

        protected int BaudRate
        {
            get;
            set;
        }

        private SerialPort serialPort
        {
            get;
            set;
        }

        public virtual uint BitMask
        {
            get;
            set;
        }

        public HHSerialPort(string portName, int baudRate)
        {
            this.PortName = portName;
            this.BaudRate = baudRate;
            serialPort = new SerialPort(this.PortName, this.BaudRate);
            serialPort.DataReceived += new SerialDataReceivedEventHandler(OnSerialDataReceived);
            //OpenSerialPort();
        }

        ~HHSerialPort()
        {
            serialPort.Close();
        }

        protected bool OpenPort()
        {
            bool bRet = false;
            try
            {
                if (!serialPort.IsOpen)
                {
                    serialPort.Open();
                }

                if (serialPort.IsOpen)
                {
                    bRet = true;
                }
            }
            catch
            {
                throw new Exception(String.Format("Unable to open COM{0} port", this.serialPort));
            }

            return bRet;
        }

        protected void SendPacket(uint bitState)
        {
            if (serialPort.IsOpen)
            {
                uint newBitState = this.BitMask & bitState;
                serialPort.Write(newBitState.ToString());
            }
        }

        public void OnSerialDataReceived(object sender, SerialDataReceivedEventArgs e)
        {

        }
    }

    public class HHGeneralPurposeIO /*: HHInterface*/
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

        protected bool OpenPort()
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

        protected void SendPacket(uint bitState)
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
    }
}
