using System.IO.Ports;
using System;
using System.Collections.Generic;

namespace HardwareMap
{
    public class HHInterface
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

        public HHInterface(string portName, int baudRate)
        {
            this.PortName = portName;
            this.BaudRate = baudRate;
            serialPort = new SerialPort(this.PortName, this.BaudRate);
            serialPort.DataReceived += new SerialDataReceivedEventHandler(OnSerialDataReceived);
            //OpenSerialPort();
        }

        ~HHInterface()
        {
            serialPort.Close();
        }

        protected bool OpenSerialPort()
        {
            bool bRet = false;
            try
            {
                if (!serialPort.IsOpen)
                {
                    //serialPort.Open();
                }

                if (serialPort.IsOpen)
                {
                    bRet = true;
                }
            }
            catch
            {
                throw new Exception("Unable to open COM port");
            }

            return /*bRet*/true;
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
}
