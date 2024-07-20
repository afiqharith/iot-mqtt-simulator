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

        public double NewAnalogData
        {
            get;
            private set;
        }

        private ListenerWindow Window
        {
            get;
            set;
        }

        public SetHardwareStateJob(HardwareBase hardware, uint newBitState = 0, double newAnalogData = -1)
        {
            this.Hardware = hardware;
            this.NewBitState = newBitState;
            this.NewAnalogData = newAnalogData;
        }

        public SetHardwareStateJob(ListenerWindow window, HardwareBase hardware, uint newBitState = 0, double newAnalogData = -1)
        {
            this.Window = window;
            this.Hardware = hardware;
            this.NewBitState = newBitState;
            this.NewAnalogData = newAnalogData;
        }

        public virtual void Run()
        {
            bool bRet = this.Hardware.Connect();

            if (this.Hardware.BitState == this.NewBitState)
            {
                bRet = false;
            }

            if (bRet)
            {
                String msgLog = String.Format("SetHardwareStateJob. HWID: {0}, mask bit: 0x{1:D4}, state bit change from 0x{2:D4} to 0x{3:D4}",
                    this.Hardware.Id,
                    this.Hardware.BitMask.ToString("X"),
                    this.Hardware.BitState.ToString("X"),
                    this.NewBitState.ToString("X"));

                if (this.Window != null)
                {
                    this.Window.ListenerLogInfo(msgLog, Color.Orange);
                }

                if (this.Hardware.GetNewBitStateValue(this.NewBitState) == this.Hardware.BitMask)
                {
                    this.Hardware.On();
                }
                else
                {
                    this.Hardware.Off();
                }

                //this.Hardware.BitState = this.Hardware.GetNewBitStateValue(this.NewBitState);
                if (this.Window != null)
                {
                    this.Window.UpdateBitSetDgvData(this.Hardware.BitMask, this.Hardware.BitState);
                }
            }
        }
    }
}
