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

        public uint NewBitState
        {
            get;
            private set;
        }

        public int NewAnalogData
        {
            get;
            private set;
        }

        private ListenerWindow ParentWindow
        {
            get;
            set;
        }

        public SetHardwareStateJob(HardwareBase hardware, uint newBitState = 0, int newAnalogData = -1)
        {
            Hardware = hardware;
            NewBitState = newBitState;
            NewAnalogData = newAnalogData;
        }

        public SetHardwareStateJob(ListenerWindow parentWindow, HardwareBase hardware, uint newBitState = 0, int newAnalogData = -1)
        {
            ParentWindow = parentWindow;
            Hardware = hardware;
            NewBitState = newBitState;
            NewAnalogData = newAnalogData;
        }

        public virtual void Run()
        {
            bool bRet = this.Hardware.Connect();

            if (Hardware.BitState == NewBitState)
            {
                bRet = false;
            }

            if (bRet)
            {
                String msgLog = String.Format("SetHardwareStateJob. HWID: {0}, mask bit: 0x{1:D4}, state bit change from 0x{2:D4} to 0x{3:D4}",
                    Hardware.Id,
                    Hardware.BitMask.ToString("X"),
                    Hardware.BitState.ToString("X"),
                    NewBitState.ToString("X"));

                if (ParentWindow != null)
                {
                    ParentWindow.ListenerLogInfo(msgLog, Color.Orange);
                }

                if (Hardware.GetNewBitStateValue(NewBitState) == Hardware.BitMask)
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
                        fan.Speed = NewAnalogData;
                    }
                    else
                    {
                        fan.Speed = fan.Speed;
                    }
                }

                //this.Hardware.BitState = this.Hardware.GetNewBitStateValue(this.NewBitState);
                if (ParentWindow != null)
                {
                    ParentWindow.UpdateBitSetDgvData(Hardware.BitMask, Hardware.BitState);
                }
            }
        }
    }
}
