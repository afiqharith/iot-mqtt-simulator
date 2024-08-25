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
    public class ReadDeviceStateJob : IJob
    {
        public DeviceBase DeviceBase
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

        public ReadDeviceStateJob(DeviceBase deviceBase)
        {
            DeviceBase = deviceBase;
        }

        private bool _isCompleted = false;
        public bool IsCompleted
        {
            get => _isCompleted;
            private set
            {
                _isCompleted = value;
            }
        }

        public virtual void Run()
        {
            bool bRet = DeviceBase.Connect();

            if (bRet)
            {
                BitState = DeviceBase.BitState;
                AnalogData = DeviceBase.AnalogData;

                Color color = ((BitState & DeviceBase.BitState) != 0) ? Color.Green : Color.OrangeRed;

                String msgLog = String.Format("ReadDeviceStateJob. HWID: {0}, mask bit: 0x{1:D4}, current state bit 0x{2:D4}",
                    DeviceBase.Id,
                    DeviceBase.BitMask.ToString("X"),
                    BitState.ToString("X"));

                if (ParentWindow != null)
                {
                    ParentWindow.UpdateBitSetDgvData(DeviceBase.BitMask, BitState);
                    SystemHelper.PrintMessage(ParentWindow.LInfo, msgLog, color);
                }
            }

            if (!IsCompleted && bRet) 
            { 
                IsCompleted = true; 
            }
        }
    }
}
