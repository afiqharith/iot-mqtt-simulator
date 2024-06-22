using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BitMap
{
    public enum eBitMask
    {
        Bit1 = 1 << 0, //0x0001 //Fan1
        Bit2 = 1 << 1, //0x0002 //Fan2
        Bit3 = 1 << 2, //0x0004 //Fan3
        Bit4 = 1 << 3, //0x0008 //Fan4
        Bit5 = 1 << 4, //0x0016 //Lamp1
        Bit6 = 1 << 5, //0x0032 //Lamp2
        Bit7 = 1 << 6, //0x0064 //Lamp3
        Bit8 = 1 << 7, //0x0128 //Lamp4
    }
}

namespace Model
{
    using BitMap;
    public enum eTYPE
    {
        LAMP,
        FAN
    }

    public enum eLOC
    {
        Loc1,
        Loc2,
        Loc3,
        Loc4
    }

    public interface IHardware
    {
        string Id { get; }
        uint CurrentState { get; set; }
    }

    public class HardwareBase : IHardware
    {
        public virtual string Id { get; private set; }
        public virtual eTYPE Type { get; private set; }
        public virtual eLOC Loc { get; private set; }
        public bool IsConnected { get; private set; }
        private Panel pPanel { get; set; }

        private uint _currentState = 0;
        public virtual uint CurrentState
        {
            get { return _currentState; }
            set
            {
                SetCurrentStateProperty(ref _currentState, value);
            }
        }

        private uint _bitMask = 0;
        public virtual uint BitMask
        {
            get { return _bitMask; }
            private set
            {
                SetBitMaskProperty(ref _bitMask, value);
            }
        }

        private uint _currentStateBit = 0;
        public virtual uint CurrentStateBit
        {
            get { return _currentStateBit; }
            private set
            {
                SetCurrentStateBitProperty(ref _currentStateBit, value);
            }
        }

        public HardwareBase(Panel panel, eTYPE type, eLOC loc, string id, eBitMask mask)
        {
            this.Id = id;
            this.Type = type;
            this.Loc = loc;
            this.pPanel = panel;
            this.BitMask = (uint)mask;
        }

        public virtual bool Connect()
        {
            int iAttempt = 0;

            while (!IsConnected && iAttempt < 3)
            {
                try
                {
                    //Create HW bit map connection here
                    this.IsConnected = true;
                }
                catch
                {
                    iAttempt++;
                }
            }

            if (iAttempt > 2)
            {
                throw new Exception(String.Format("{0} Failed {1} attempt to connect", this.Id, iAttempt));
            }

            return IsConnected;
        }

        private void SetBitMaskProperty(ref uint bitMask, uint newval)
        {
            bitMask = newval;
            //Map with hardware bit
        }

        private void SetCurrentStateBitProperty(ref uint bitState, uint newval)
        {
            bitState = newval;
        }

        private void SetCurrentStateProperty(ref uint state, uint newval)
        {
            switch (newval)
            {
                case 0x0001:
                    this.pPanel.BackColor = Color.Green;
                    state = newval;
                    //Set the bit location to 1
                    this.CurrentStateBit |= this.BitMask;
                    break;

                case 0x0000:
                default:
                    this.pPanel.BackColor = Color.Gray;
                    state = 0x0;
                    if ((this.CurrentStateBit &= this.BitMask) != 0)
                    {
                        //Set the bit location to 0
                        this.CurrentStateBit &= ~this.BitMask;
                    }
                    break;
            }
        }

#warning Deprecated code in this method.
        public virtual uint GetCurrentState()
        {
            return this.CurrentState;
        }
    }

}
