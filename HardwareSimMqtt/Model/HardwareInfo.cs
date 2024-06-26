﻿using System.Collections.Generic;

namespace DataContainer
{
    using Model;
    public class BitInfo : IHardware
    {
        public string Id { get; set; }
        public uint CurrentBitState { get; set; }
        public BitInfo(string id, uint currentBitState = 0)
        {
            this.Id = id;
            this.CurrentBitState = currentBitState;
        }
    }

    //Use ONLY when to serializing/deserializing to JSON object
    public class JsonBitInfoList
    {        
        public List<BitInfo> InfoList { get; set; }

        public JsonBitInfoList(List<BitInfo> bitInfoList)
        {
            this.InfoList = bitInfoList;
        }

        public JsonBitInfoList() { }
    }
}

namespace QueryJob
{
    using Model;
    public interface IJob
    {
        void Run();
    }

    public class SetHardwareStateJob : IJob
    {
        public HardwareBase Hardware { get; private set; }

        public uint NewBitState { get; private set; }

        public SetHardwareStateJob(HardwareBase hardware, uint newBitState = 0)
        {
            this.Hardware = hardware;
            this.NewBitState = newBitState;
        }

        public virtual void Run()
        {
            bool bRet = this.Hardware.Connect();

            if (bRet)
            {
                this.Hardware.CurrentBitState = this.Hardware.BitMask & this.NewBitState;
            }
        }
    }
}
