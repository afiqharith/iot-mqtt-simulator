using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;
using CheckBox = System.Windows.Forms.CheckBox;
using Newtonsoft.Json;
using static MqttSim.DisplayWindow;

namespace MqttSim
{
    public class SetMqttBrokerConnectJob
    {
        public MqttClient Client { get; set; }
        public string BrokerHostName { get; private set; }

        public SetMqttBrokerConnectJob(string brokerHostName)
        {
            BrokerHostName = brokerHostName;
        }

        public virtual bool Run()
        {
            Client = new MqttClient(BrokerHostName);
            string guid = Convert.ToString(Guid.NewGuid());
            bool bSuccess = true;
            try
            {
                Client.Connect(guid, "emqx", "public");
            }
            catch
            {
                bSuccess = false;
            }

            return bSuccess;
        }
    }

    public partial class ControlWindow : Form
    {
        private SetMqttBrokerConnectJob m_BrokerConnectJob { get; set; }

        //private Dictionary<ushort, HardwareInfo>
        private Queue<Dictionary<ushort, HardwareInfo>> m_QMsgContentToDisplayOnUI { get; set; }

        public ControlWindow()
        {
            InitializeComponent();
            m_QMsgContentToDisplayOnUI = new Queue<Dictionary<ushort, HardwareInfo>>();

            CheckBox[] checkbox = new CheckBox[]
            {
                checkBoxFan1,
                checkBoxFan2,
                checkBoxFan3,
                checkBoxFan4,

                checkBoxLamp1,
                checkBoxLamp2,
                checkBoxLamp3,
                checkBoxLamp4
            };

            for (int i = 0; i < checkbox.Length; i++)
            {
                checkbox[i].CheckStateChanged += new EventHandler(PerHWUnitCheckbox_CheckStateChanged);
            }

            m_BrokerConnectJob = new SetMqttBrokerConnectJob("broker.emqx.io");
            bool bEstablished = m_BrokerConnectJob.Run();
            if (bEstablished)
            {
                m_BrokerConnectJob.Client.MqttMsgPublished += OnMqttMessagePublished;
            }

        }

        private void checkBoxLoc_CheckStateChanged(object sender, EventArgs e)
        {
            CheckBox checkbox = (CheckBox)sender;
            List<HardwareInfo> hardwareInfoList = new List<HardwareInfo>();

            switch (checkbox.Name)
            {
                case "checkBoxLoc1":
                    hardwareInfoList.Add(new HardwareInfo("L-ID1", (uint)(checkbox.Checked ? 1 : 0)));
                    hardwareInfoList.Add(new HardwareInfo("F-ID1", (uint)(checkbox.Checked ? 1 : 0)));
                    break;

                case "checkBoxLoc2":
                    hardwareInfoList.Add(new HardwareInfo("L-ID2", (uint)(checkbox.Checked ? 1 : 0)));
                    hardwareInfoList.Add(new HardwareInfo("F-ID2", (uint)(checkbox.Checked ? 1 : 0)));
                    break;

                case "checkBoxLoc3":
                    hardwareInfoList.Add(new HardwareInfo("L-ID3", (uint)(checkbox.Checked ? 1 : 0)));
                    hardwareInfoList.Add(new HardwareInfo("F-ID3", (uint)(checkbox.Checked ? 1 : 0)));
                    break;

                case "checkBoxLoc4":
                    hardwareInfoList.Add(new HardwareInfo("L-ID4", (uint)(checkbox.Checked ? 1 : 0)));
                    hardwareInfoList.Add(new HardwareInfo("F-ID4", (uint)(checkbox.Checked ? 1 : 0)));
                    break;

                case "checkBoxShutdownAll":
                default:
                    hardwareInfoList.Add(new HardwareInfo("L-ID1", (uint)(checkbox.Checked ? 1 : 0)));
                    hardwareInfoList.Add(new HardwareInfo("F-ID1", (uint)(checkbox.Checked ? 1 : 0)));
                    hardwareInfoList.Add(new HardwareInfo("L-ID2", (uint)(checkbox.Checked ? 1 : 0)));
                    hardwareInfoList.Add(new HardwareInfo("F-ID2", (uint)(checkbox.Checked ? 1 : 0)));
                    hardwareInfoList.Add(new HardwareInfo("L-ID3", (uint)(checkbox.Checked ? 1 : 0)));
                    hardwareInfoList.Add(new HardwareInfo("F-ID3", (uint)(checkbox.Checked ? 1 : 0)));
                    hardwareInfoList.Add(new HardwareInfo("L-ID4", (uint)(checkbox.Checked ? 1 : 0)));
                    hardwareInfoList.Add(new HardwareInfo("F-ID4", (uint)(checkbox.Checked ? 1 : 0)));
                    break;
            }

            PublishHardwareInfoToBroker(hardwareInfoList);
        }

        private void PerHWUnitCheckbox_CheckStateChanged(object sender, EventArgs e)
        {
            CheckBox checkbox = (CheckBox)sender;

            if (checkbox.CheckState == CheckState.Indeterminate)
            {
                return;
            }

            List<HardwareInfo> payloadList = new List<HardwareInfo>();
            HardwareInfo payload = new HardwareInfo();

            switch (checkbox.Name)
            {
                case "checkBoxFan1":
                    payload.Id = "F-ID1";
                    break;

                case "checkBoxFan2":
                    payload.Id = "F-ID2";
                    break;

                case "checkBoxFan3":
                    payload.Id = "F-ID3";
                    break;

                case "checkBoxFan4":
                    payload.Id = "F-ID4";
                    break;


                case "checkBoxLamp1":
                    payload.Id = "L-ID1";
                    break;

                case "checkBoxLamp2":
                    payload.Id = "L-ID2";
                    break;

                case "checkBoxLamp3":
                    payload.Id = "L-ID3";
                    break;

                case "checkBoxLamp4":
                    payload.Id = "L-ID4";
                    break;

                default:
                    payload.Id = String.Empty;
                    break;
            }

            if (!String.IsNullOrWhiteSpace(payload.Id))
            {
                payload.CurrentState = Convert.ToUInt32(checkbox.Checked ? 1 : 0);
                payloadList.Add(payload);
            }

            PublishHardwareInfoToBroker(payloadList);
        }

        private void PublishHardwareInfoToBroker(List<HardwareInfo> hardwareInfoList)
        {
            string jsonifiedHardwareInfoList = JsonConvert.SerializeObject(new HardwareInfoList(hardwareInfoList));

            if (m_BrokerConnectJob.Client.IsConnected)
            {
                //Publish JSON converted HardwareInfoList to MQTT server
                ushort msgID = m_BrokerConnectJob.Client.Publish(
                    "IotWinformSim",
                    Encoding.ASCII.GetBytes(jsonifiedHardwareInfoList),
                    MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE,
                    true);

                for (int i = 0; i < hardwareInfoList.Count; i++)
                {
                    //Queue HardwareInfoList content to display on UI 
                    Dictionary<ushort, HardwareInfo> messageMap = new Dictionary<ushort, HardwareInfo>();
                    messageMap.Add(msgID, hardwareInfoList[i]);
                    m_QMsgContentToDisplayOnUI.Enqueue(messageMap);

                    LogInfo(String.Format(
                        "ID[{0}] HW state change command sent. HWID: {1}, cmd: 0x{2:D2}", 
                        msgID, 
                        hardwareInfoList[i].Id, 
                        hardwareInfoList[i].CurrentState),
                        Color.Gray);
                }
            }
        }

        private void OnMqttMessagePublished(object sender, MqttMsgPublishedEventArgs e)
        {
            if (e.IsPublished)
            {
                while (m_QMsgContentToDisplayOnUI.Count > 0)
                {
                    //De-queue message content to display on UI
                    Dictionary<ushort, HardwareInfo> messageMap = m_QMsgContentToDisplayOnUI.Dequeue();

                    foreach (KeyValuePair<ushort, HardwareInfo> kvp in messageMap)
                    {
                        HardwareInfo hardwareInfo = kvp.Value;

                        LogInfo(String.Format(
                            "ID[{0}] HW state change command published. HWID: {1}, cmd: 0x{2:D2}", 
                            kvp.Key, 
                            hardwareInfo.Id, 
                            hardwareInfo.CurrentState),
                            hardwareInfo.CurrentState == 1 ? Color.Blue : Color.OrangeRed);

                    }
                }
            }
        }

        private void LogInfo(string text, Color color)
        {
            SystemHelper.AppendRTBText(richTextBox1, text, color);
        }

        private void ControlWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (m_BrokerConnectJob.Client.IsConnected)
            {
                m_BrokerConnectJob.Client.Disconnect();
            }
        }

        private void ControlWindow_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (m_BrokerConnectJob.Client.IsConnected)
            {
                m_BrokerConnectJob.Client.Disconnect();
            }
        }
    }
}
