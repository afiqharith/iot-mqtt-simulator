using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Model
{
    using BitMap;
    internal class SimpLamp : HardwareBase
    {
        public SimpLamp(Panel panel, eLOC loc, string id, eBitMask mask) 
            : base(panel, eTYPE.LAMP, loc, id, mask) { }
    }
}
