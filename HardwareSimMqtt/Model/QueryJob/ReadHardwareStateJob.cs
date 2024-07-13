using HardwareSimMqtt.Interface;
using System;
using System.Collections.Generic;
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

        public ReadHardwareStateJob(HardwareBase hardware)
        {
            this.Hardware = hardware;
        }

        public virtual void Run()
        {
            bool bRet = this.Hardware.Connect();

            if (bRet)
            {
                this.BitState = this.Hardware.BitState;
                this.AnalogData = this.Hardware.AnalogData;
            }
        }
    }
}
