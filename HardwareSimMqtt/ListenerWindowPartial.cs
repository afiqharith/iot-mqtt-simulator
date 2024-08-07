﻿using System;
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
using System.Linq;

namespace HardwareSimMqtt
{
    //Controller
    public partial class ListenerWindow
    {
        private SetBrokerConnectJob controllerBrokerConnectJob
        {
            get;
            set;
        }

        private Queue<Dictionary<ushort, BitInfo>> queueBufferMessageToDisplay
        {
            get;
            set;
        }


        private event EventHandler<PublishBitInfoToBrokerEventArgs> onPublishingBitInfoToBroker;

        private void InitializePartialListenerWindow()
        {
            //Controller
            queueBufferMessageToDisplay = new Queue<Dictionary<ushort, BitInfo>>();

            controllerBrokerConnectJob = new SetBrokerConnectJob("broker.emqx.io");
            bool bEstablished = controllerBrokerConnectJob.Run();
            if (bEstablished)
            {
                controllerBrokerConnectJob.Client.MqttMsgPublished += (sender, e) =>
                {
                    if (e.IsPublished)
                    {
                        while (queueBufferMessageToDisplay.Count > 0)
                        {
                            //De-queue message content to display on UI
                            Dictionary<ushort, BitInfo> messageMap = queueBufferMessageToDisplay.Dequeue();

                            foreach (KeyValuePair<ushort, BitInfo> kvp in messageMap)
                            {
                                BitInfo bitInfo = kvp.Value;
                                string log = String.Format("ID[{0}] HW state change command published. HWID: {1}, cmd bit: 0x{2:D4}", kvp.Key, bitInfo.Id, bitInfo.BitState.ToString("X"));
                                ContollerLogInfo(log, bitInfo.BitState != 0 ? Color.Blue : Color.OrangeRed);
                            }
                        }
                    }
                };
            }

            onPublishingBitInfoToBroker += (sender, e) =>
            {
                for (int i = 0; i < e.BitInfoList.Count; i++)
                {
                    //Queue HardwareInfoList content to display on UI 
                    Dictionary<ushort, BitInfo> messageMap = new Dictionary<ushort, BitInfo>();
                    messageMap.Add(e.MessageId, e.BitInfoList[i]);
                    queueBufferMessageToDisplay.Enqueue(messageMap);

                    string log = String.Format("ID[{0}] HW state change command sent. HWID: {1}, cmd bit: 0x{2:D4}", e.MessageId, e.BitInfoList[i].Id, e.BitInfoList[i].BitState.ToString("X"));
                    ContollerLogInfo(log, Color.Gray);
                }
            };
        }

        private List<BitInfo> bitInfoListTemp
        {
            get; 
            set;
        }
        public void PublishAllBitInfoToBroker(List<BitInfo> bitInfoList)
        {
            if (bitInfoListTemp == null)
            {
                bitInfoListTemp = new List<BitInfo>();
            }

            if (bitInfoListTemp.Count != simHardwareMap.Count || bitInfoListTemp.Count == 0)
            {
                bitInfoListTemp.AddRange(bitInfoList);
            }

            if(bitInfoListTemp.Count == simHardwareMap.Count)
            {
                string jsonifiedAllBitInfoList = JsonConvert.SerializeObject(new JsonBitInfoList(bitInfoListTemp));

                if (controllerBrokerConnectJob.Client.IsConnected)
                {
                    //Publish JSON converted HardwareInfoList to MQTT server
                    ushort msgID = controllerBrokerConnectJob.Client.Publish(
                        TOPIC,
                        Encoding.UTF8.GetBytes(jsonifiedAllBitInfoList),
                        MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE,
                        true);

                    onPublishingBitInfoToBroker.Invoke(null, new PublishBitInfoToBrokerEventArgs(msgID, bitInfoListTemp));
                    bitInfoListTemp.Clear();
                }
            }
        }

        public void PublishPartialBitInfoToBroker(List<BitInfo> bitInfoList)
        {
            string jsonifiedBitInfoList = JsonConvert.SerializeObject(new JsonBitInfoList(bitInfoList));

            if (controllerBrokerConnectJob.Client.IsConnected)
            {
                //Publish JSON converted HardwareInfoList to MQTT server
                ushort msgID = controllerBrokerConnectJob.Client.Publish(
                    TOPIC,
                    Encoding.UTF8.GetBytes(jsonifiedBitInfoList),
                    MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE,
                    true);

                onPublishingBitInfoToBroker.Invoke(null, new PublishBitInfoToBrokerEventArgs(msgID, bitInfoList));
            }
        }

        private void ContollerLogInfo(string text, Color color)
        {
            SystemHelper.AppendRichTextBox(richTextBox2, text, color);
        }
    }
}
