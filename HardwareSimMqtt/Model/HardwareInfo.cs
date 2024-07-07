using System.Collections.Generic;
using Model;

namespace DataContainer
{
    public class BitInfo : IHardware
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

    //Use ONLY when to serializing/deserializing to JSON object
    public class JsonBitInfoList
    {
        public List<BitInfo> InfoList
        {
            get;
            set;
        }

        public JsonBitInfoList(List<BitInfo> bitInfoList)
        {
            this.InfoList = bitInfoList;
        }

        public JsonBitInfoList() { }
    }
}

namespace QueryJob
{
    public interface IJob
    {
        HardwareBase Hardware
        {
            get;
        }

        void Run();
    }

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
                this.Hardware.BitState = this.Hardware.BitMask & this.NewBitState;
            }
        }
    }

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
