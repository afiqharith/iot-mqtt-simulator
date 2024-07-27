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

        private ListenerWindow ParentWindow
        {
            get => Program.WndHandle;
        }

        public ReadHardwareStateJob(HardwareBase hardware)
        {
            Hardware = hardware;
        }

        public virtual void Run()
        {
            bool bRet = Hardware.Connect();

            if (bRet)
            {
                BitState = Hardware.BitState;
                AnalogData = Hardware.AnalogData;

                Color color = ((BitState & Hardware.BitState) != 0) ? Color.Green : Color.OrangeRed;

                String msgLog = String.Format("ReadHardwareStateJob. HWID: {0}, mask bit: 0x{1:D4}, current state bit 0x{2:D4}",
                    Hardware.Id,
                    Hardware.BitMask.ToString("X"),
                    BitState.ToString("X"));

                if (ParentWindow != null)
                {
                    ParentWindow.UpdateBitSetDgvData(Hardware.BitMask, BitState);
                    ParentWindow.ListenerLogInfo(msgLog, color);
                }
            }
        }
    }
}
