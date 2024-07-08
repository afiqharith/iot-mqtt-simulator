using HardwareSimMqtt.Interface;
using System;
using System.Collections.Generic;
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

        public SetHardwareStateJob(HardwareBase hardware, uint newBitState = 0, double newAnalogData = -1)
        {
            this.Hardware = hardware;
            this.NewBitState = newBitState;
            this.NewAnalogData = newAnalogData;
        }

        public virtual void Run()
        {
            bool bRet = this.Hardware.Connect();

            if (bRet)
            {
                this.Hardware.BitState = this.Hardware.GetNewBitStateValue(this.NewBitState);
            }
        }
    }
}
