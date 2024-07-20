using HardwareSimMqtt.Interface;
using HardwareSimMqtt;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HardwareSimMqtt.Model.QueryJob
{
    public class ReadHardwareStateJob : IJob
    {
        public HardwareBase Hardware
        {
            get;
            private set;
        }

        public uint BitState
        {
            get;
            private set;
        }

        public double AnalogData
        {
            get;
            set;
        }

        private ListenerWindow Window
        {
            get;
            set;
        }

        public ReadHardwareStateJob(HardwareBase hardware)
        {
            this.Hardware = hardware;
        }

        public ReadHardwareStateJob(ListenerWindow window, HardwareBase hardware)
        {
            this.Window = window;
            this.Hardware = hardware;
        }

        public virtual void Run()
        {
            bool bRet = this.Hardware.Connect();

            if (bRet)
            {
                this.BitState = this.Hardware.BitState;
                this.AnalogData = this.Hardware.AnalogData;

                Color color = ((this.BitState & this.Hardware.BitState) != 0) ? Color.Green : Color.OrangeRed;

                String msgLog = String.Format("ReadHardwareStateJob. HWID: {0}, mask bit: 0x{1:D4}, current state bit 0x{2:D4}",
                    this.Hardware.Id,
                    this.Hardware.BitMask.ToString("X"),
                    this.BitState.ToString("X"));

                if (this.Window != null)
                {
                    this.Window.ListenerLogInfo(msgLog, color);
                }
            }
        }
    }
}
