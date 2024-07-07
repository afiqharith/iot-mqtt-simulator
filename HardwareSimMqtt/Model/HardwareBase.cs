using System;

namespace BitMap
{
    public enum eBitMask
    {
        Bit0 = 1 << 0, //0x0001 //Fan1
        Bit1 = 1 << 1, //0x0002 //Fan2
        Bit2 = 1 << 2, //0x0004 //Fan3
        Bit3 = 1 << 3, //0x0008 //Fan4
        Bit4 = 1 << 4, //0x0016 //Lamp1
        Bit5 = 1 << 5, //0x0032 //Lamp2
        Bit6 = 1 << 6, //0x0064 //Lamp3
        Bit7 = 1 << 7, //0x0128 //Lamp4
    }
}

namespace Model
{
    using BitMap;
    using System.Diagnostics;
    using System.Threading;
    using HardwareMap;

    public enum eTYPE
    {
        LAMP,
        FAN,
        AIR_CONDITIONER,
        GATE,
    }

    public enum eLOC
    {
        Area1,
        Area2,
        Area3,
        Area4,

        Loc1 = Area1,
        Loc2,
        Loc3,
        Loc4
    }

    public interface IHardware
    {
        string Id 
        { 
            get; 
        }

        uint BitState 
        { 
            get; 
            set; 
        }

        double AnalogData
        {
            get;
            set;
        }
    }

    public class HardwareBase : HHGeneralPurposeIO, IHardware
    {
        private string _id;
        public virtual string Id
        {
            get => _id;
            protected set => SetIdProperty(ref _id, value);
        }

        public virtual eTYPE Type
        {
            get;
            protected set;
        }

        public virtual eLOC Location
        {
            get;
            protected set;
        }

        public bool IsConnected
        {
            get;
            protected set;
        }

        // Bit index for the hardware, bit map
        private uint _bitMask = 0;
        public virtual uint BitMask
        {
            get => _bitMask;
            protected set => SetBitMaskProperty(ref _bitMask, value);
        }

        // Bit state of the hardware at the bit index
        private uint _bitState = 0;
        public virtual uint BitState
        {
            get => _bitState;
            set => SetCurrentBitStateProperty(ref _bitState, value);
        }

        private double _analogData = -1;
        public virtual double AnalogData
        {
            get => _analogData;
            set => SetAnalogDataProperty(ref _analogData, value);
        }

        public HardwareBase(eTYPE type, eLOC location, string id, eBitMask mask, int ioPort) 
            : /*base("COM12", 9600)*/base(ioPort)
        {
            this.Type = type;
            this.Location = location;
            this.Id = id;
            this.BitMask = (uint)mask;
        }

        private void SetIdProperty(ref string id, string newval)
        {
            string createdId = String.Empty;
            switch (this.Type)
            {
                case eTYPE.FAN:
                    createdId = String.Format("F_ID{0}", newval);
                    break;

                default:
                case eTYPE.LAMP:
                    createdId = String.Format("L_ID{0}", newval);
                    break;
            }
            id = createdId;
        }

        private void SetBitMaskProperty(ref uint bitMask, uint newval)
        {
            bitMask = newval;
            //Map with hardware bit
            base.BitMask = newval;
        }

        private void SetCurrentBitStateProperty(ref uint bitState, uint newval)
        {
            bitState = newval;
            base.SendPacket(bitState);
        }

        private void SetAnalogDataProperty(ref double data, double newval)
        {
            data = newval;
        }

        public virtual bool Connect()
        {
            int iAttempt = 0;
            int elapsedTime = 0;
            int timeStart = Environment.TickCount;
            while (!IsConnected && iAttempt < 3)
            {
                try
                {
                    //Create HW bit map connection here
                    this.IsConnected = base.OpenPort();
                }
                catch
                {
                    Thread.Sleep(200);
                }
                iAttempt++;
            }

            if (IsConnected && iAttempt == 1) //Only log when there is attempt to connect, otherwise it already connect
            {
                elapsedTime = Environment.TickCount - timeStart;
                Debug.WriteLine(String.Format("{0} connected. Bit: 0x{1:D4}, Elapsed: {2}ms", this.Id, this.BitMask.ToString("X"), elapsedTime));
            }

            if (iAttempt > 2)
            {
                elapsedTime = Environment.TickCount - timeStart;
                string exLog = String.Format("{0} Failed {1} attempt to connect. Bit: 0x{1:D4}, Elapsed: {2}ms", this.Id, iAttempt, this.BitMask.ToString("X"), elapsedTime);
                Debug.WriteLine(exLog);
                throw new Exception(exLog);
            }
            return IsConnected;
        }

    }
}
