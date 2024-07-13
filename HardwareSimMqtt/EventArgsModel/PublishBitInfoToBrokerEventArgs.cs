using HardwareSimMqtt.Model.DataContainer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HardwareSimMqtt.EventArgsModel
{
    public class PublishBitInfoToBrokerEventArgs : EventArgs
    {
        public ushort MessageId 
        { 
            get; 
            set; 
        } 

        public List<BitInfo> BitInfoList
        {
            get;
            set;
        }

        public PublishBitInfoToBrokerEventArgs(ushort messageId, List<BitInfo> bitInfoList)
        {
            this.MessageId = messageId;
            this.BitInfoList = bitInfoList;
        }
    }
}
