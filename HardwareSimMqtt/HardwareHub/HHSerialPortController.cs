using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HardwareSimMqtt.HardwareHub
{
    public class HHSerialPortController : IComController
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

        public HHSerialPortController(eIoType ioType, string portName, int baudRate)
        {
            IoType = ioType;
            ControllerType = eControllerType.SerialPort;
            PortName = portName;
            BaudRate = baudRate;
            serialPort = new SerialPort(PortName, BaudRate);
            serialPort.DataReceived += new SerialDataReceivedEventHandler(OnSerialDataReceived);
        }

        ~HHSerialPortController()
        {
            serialPort.Close();
        }

        public bool OpenPort()
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

        //Send digital output command
        public void SendDigitalOutputCommand(uint bitState)
        {
            if (serialPort.IsOpen)
            {
                uint newBitState = BitMask & bitState;
                serialPort.Write(newBitState.ToString());
            }
        }

        //Get digital input value
        public bool GetDigitalInputValue()
        {
            int ret = 0;
            if (serialPort.IsOpen)
            {
                //ret = serialPort.Read();
            }

            return ret != 0;
        }

        public void SendAnalogOutputCommand(int analogData) { }

        public int GetAnalogInputValue()
        {
            return 0;
        }

        public void OnSerialDataReceived(object sender, SerialDataReceivedEventArgs e)
        {

        }
    }
}
