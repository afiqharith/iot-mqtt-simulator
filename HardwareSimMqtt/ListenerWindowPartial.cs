using System;
using System.Text;
using System.Drawing;
using System.Configuration;
using System.Windows.Forms;
using System.Threading.Tasks;
using System.Collections.Generic;
using HardwareSimMqtt.Model.BitMap;
using HardwareSimMqtt.Model.QueryJob;
using HardwareSimMqtt.Model.DataContainer;
using uPLibrary.Networking.M2Mqtt.Messages;
using Newtonsoft.Json;
using HardwareSimMqtt.EventArgsModel;

namespace ModelInterface
{
    //Controller
    public partial class ListenerWindow
    {
        private class UnitCheckBoxList
        {
            public List<CheckBox> Data = new List<CheckBox>();

            public UnitCheckBoxList()
            {
                this.Data = new List<CheckBox>();
            }
        }

        private SetBrokerConnectJob controllerBrokerConnectJob
        {
            get;
            set;
        }

        private Queue<Dictionary<ushort, BitInfo>> qMsgContentToDisplayOnUI
        {
            get;
            set;
        }


        private event EventHandler<PublishBitInfoToBrokerEventArgs> OnPublishingBitInfoToBroker;

        private void OnPublishBitInfoToBroker(object sender, PublishBitInfoToBrokerEventArgs e)
        {
            for (int i = 0; i < e.BitInfoList.Count; i++)
            {
                //Queue HardwareInfoList content to display on UI 
                Dictionary<ushort, BitInfo> messageMap = new Dictionary<ushort, BitInfo>();
                messageMap.Add(e.MessageId, e.BitInfoList[i]);
                qMsgContentToDisplayOnUI.Enqueue(messageMap);

                ContollerLogInfo(String.Format(
                    "ID[{0}] HW state change command sent. HWID: {1}, cmd bit: 0x{2:D4}",
                    e.MessageId,
                    e.BitInfoList[i].Id,
                    e.BitInfoList[i].BitState.ToString("X")),
                    Color.Gray);
            }
        }

        public void OnMessagePublished(object sender, MqttMsgPublishedEventArgs e)
        {
            if (e.IsPublished)
            {
                while (qMsgContentToDisplayOnUI.Count > 0)
                {
                    //De-queue message content to display on UI
                    Dictionary<ushort, BitInfo> messageMap = qMsgContentToDisplayOnUI.Dequeue();

                    foreach (KeyValuePair<ushort, BitInfo> kvp in messageMap)
                    {
                        BitInfo bitInfo = kvp.Value;

                        ContollerLogInfo(String.Format(
                            "ID[{0}] HW state change command published. HWID: {1}, cmd bit: 0x{2:D4}",
                            kvp.Key,
                            bitInfo.Id,
                            bitInfo.BitState.ToString("X")),
                            bitInfo.BitState != 0 ? Color.Blue : Color.OrangeRed);

                    }
                }
            }
        }

        private void ContollerLogInfo(string text, Color color)
        {
            SystemHelper.AppendRichTextBox(richTextBox2, text, color);
        }
    }
}
