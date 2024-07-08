using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HardwareSimMqtt.Interface;

namespace HardwareSimMqtt.Model.DataContainer
{
    public class BitInfo : IHardwarePartial
    {
        public string Id
        {
            get;
            set;
        }

        public uint BitState
        {
            get;
            set;
        }

        public double AnalogData
        {
            get;
            set;
        }

        public BitInfo(string id, uint currentBitState = 0, double analogData = -1)
        {
            this.Id = id;
            this.BitState = currentBitState;
            this.AnalogData = analogData;
        }
    }
}
