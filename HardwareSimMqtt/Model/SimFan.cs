using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Model
{
    using BitMap;
    internal class SimFan: HardwareBase
    {
        public SimFan(Panel panel, eLOC loc, string id, eBitMask mask) 
            : base(panel, eTYPE.FAN, loc, id, mask) { }
    }
}
