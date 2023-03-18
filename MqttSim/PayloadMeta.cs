using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MqttSim
{

    public class PayloadMeta
    {
        public IList<HWPayloadState> hwStateList
        {
            get;
            set;
        }

        public PayloadMeta(IList<HWPayloadState> hwState)
        {
            this.hwStateList = hwState;
        }

        public PayloadMeta() { }
    }


    public class HWPayloadState
    {
        public string Id { get; set; }
        public uint State { get; set; }

        public HWPayloadState(string id, uint state)
        {
            this.Id = id;
            this.State = state;
        }

        public HWPayloadState() { }
    }
}
