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
    public class SetDeviceStateJob : IJob
    {
        public DeviceBase DeviceBase
        {
            get;
            private set;
        }

        public uint RequestBitState
        {
            get;
            private set;
        }

        public int RequestAnalogData
        {
            get;
            private set;
        }

        private ListenerWindow ParentWindow
        {
            get => Program.WndHandle;
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

        public SetDeviceStateJob(DeviceBase deviceBase, uint requestBitState = 0, int requestAnalogData = -1)
        {
            DeviceBase = deviceBase;
            RequestBitState = requestBitState;
            RequestAnalogData = requestAnalogData;
        }

        public SetDeviceStateJob(DeviceBase deviceBase, bool requestBoolState = false, int requestAnalogData = -1)
        {
            DeviceBase = deviceBase;
            RequestBitState = requestBoolState ? DeviceBase.GetNewBitStateValue(DeviceBase.BitMask) : DeviceBase.GetNewBitStateValue(~DeviceBase.BitMask);
            RequestAnalogData = requestAnalogData;
        }

        public virtual void Run()
        {
            bool bRet = DeviceBase.Connect();

            if (DeviceBase.BitState == RequestBitState)
            {
                //bRet = false;
            }

            if (bRet)
            {
                String msgLog = String.Format("SetDeviceStateJob. HWID: {0}, mask bit: 0x{1:D4}, state bit change from 0x{2:D4} to 0x{3:D4}",
                    DeviceBase.Id,
                    DeviceBase.BitMask.ToString("X"),
                    DeviceBase.BitState.ToString("X"),
                    RequestBitState.ToString("X"));

                if (ParentWindow != null)
                {
                    SystemHelper.PrintMessage(ParentWindow.LInfo, msgLog, Color.Orange);
                }

                if (DeviceBase.GetNewBitStateValue(RequestBitState) == DeviceBase.BitMask)
                {
                    DeviceBase.On();
                }
                else
                {
                    DeviceBase.Off();
                }

                if (DeviceBase.GetType() == typeof(SimFan))
                {
                    SimFan fan = (SimFan)DeviceBase;
                    if (DeviceBase.IsOn)
                    {
                        fan.Speed = RequestAnalogData;
                    }
                    else
                    {
                        fan.Speed = fan.Speed;
                    }
                }

                //this.device.BitState = this.device.GetNewBitStateValue(this.NewBitState);
            }

            if (!IsCompleted && bRet)
            {
                IsCompleted = true;
            }
        }
    }
}
