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
    public class SetHardwareStateJob : IJob
    {
        public HardwareBase Hardware
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

        public SetHardwareStateJob(HardwareBase hardware, uint requestBitState = 0, int requestAnalogData = -1)
        {
            Hardware = hardware;
            RequestBitState = requestBitState;
            RequestAnalogData = requestAnalogData;
        }

        public SetHardwareStateJob(HardwareBase hardware, bool requestBoolState = false, int requestAnalogData = -1)
        {
            Hardware = hardware;
            RequestBitState = requestBoolState ? Hardware.GetNewBitStateValue(Hardware.BitMask) : Hardware.GetNewBitStateValue(~Hardware.BitMask);
            RequestAnalogData = requestAnalogData;
        }

        public virtual void Run()
        {
            bool bRet = this.Hardware.Connect();

            if (Hardware.BitState == RequestBitState)
            {
                //bRet = false;
            }

            if (bRet)
            {
                String msgLog = String.Format("SetHardwareStateJob. HWID: {0}, mask bit: 0x{1:D4}, state bit change from 0x{2:D4} to 0x{3:D4}",
                    Hardware.Id,
                    Hardware.BitMask.ToString("X"),
                    Hardware.BitState.ToString("X"),
                    RequestBitState.ToString("X"));

                if (ParentWindow != null)
                {
                    ParentWindow.ListenerLogInfo(msgLog, Color.Orange);
                }

                if (Hardware.GetNewBitStateValue(RequestBitState) == Hardware.BitMask)
                {
                    Hardware.On();
                }
                else
                {
                    Hardware.Off();
                }

                if (Hardware.GetType() == typeof(SimFan))
                {
                    SimFan fan = (SimFan)Hardware;
                    if (Hardware.IsOn)
                    {
                        fan.Speed = RequestAnalogData;
                    }
                    else
                    {
                        fan.Speed = fan.Speed;
                    }
                }

                //this.Hardware.BitState = this.Hardware.GetNewBitStateValue(this.NewBitState);
            }
        }
    }
}
