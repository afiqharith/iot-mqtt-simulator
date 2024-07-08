using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HardwareSimMqtt.HardwareHub
{
    public class HHSerialPort : HHInterface
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
        }

        ~HHSerialPort()
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

        public void SendDigitalCommand(uint bitState)
        {
            if (serialPort.IsOpen)
            {
                uint newBitState = this.BitMask & bitState;
                serialPort.Write(newBitState.ToString());
            }
        }

        public void SendAnalogCommand(double analogData)
        { }

        public void OnSerialDataReceived(object sender, SerialDataReceivedEventArgs e)
        {

        }
    }
}
