using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HardwareSimMqtt.Model.BitMap
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
