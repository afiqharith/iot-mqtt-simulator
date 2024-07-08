using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HardwareSimMqtt.Model.DataContainer
{
    //Use ONLY when to serializing/deserializing to JSON object
    public class JsonBitInfoList : EventArgs
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
