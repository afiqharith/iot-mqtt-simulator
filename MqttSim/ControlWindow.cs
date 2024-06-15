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
        private Queue<HardwareInfo> m_QMsgContentToDisplayOnUI { get; set; }

        public ControlWindow()
        {
            InitializeComponent();
            m_QMsgContentToDisplayOnUI = new Queue<HardwareInfo>();

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

            PublishPayloadMetaByBatch(hardwareInfoList);
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

            PublishPayloadMetaByBatch(payloadList);
        }

        private void PublishPayloadMetaByBatch(List<HardwareInfo> hardwareInfoList)
        {
            string jsonifiedHardwareInfoList = JsonConvert.SerializeObject(new HardwareInfoList(hardwareInfoList));

            if (m_BrokerConnectJob.Client.IsConnected)
            {
                //Publish JSON converted HardwareInfoList to MQTT server
                m_BrokerConnectJob.Client.Publish(
                    "IotWinformSim",
                    Encoding.ASCII.GetBytes(jsonifiedHardwareInfoList),
                    MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE,
                    true);

                for (int i = 0; i < hardwareInfoList.Count; i++)
                {
                    //Queue HardwareInfoList content to display on UI 
                    m_QMsgContentToDisplayOnUI.Enqueue(hardwareInfoList[i]);

                    AppendRTBText(String.Format(
                        "Sending state change command for {0}, cmd = 0x{1:D2}", hardwareInfoList[i].Id, hardwareInfoList[i].CurrentState),
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
                    HardwareInfo hardwareInfo = m_QMsgContentToDisplayOnUI.Dequeue();

                    AppendRTBText(String.Format(
                        "Successfully sending changed state cmd for {0}, cmd = 0x{1:D2}", hardwareInfo.Id, hardwareInfo.CurrentState),
                        hardwareInfo.CurrentState == 1 ? Color.Blue : Color.OrangeRed);
                }
            }
        }

        private void AppendRTBText(string text, Color color)
        {
            if (richTextBox1.InvokeRequired)
            {
                Action safeThread = delegate { AppendRTBText(text, color); };
                richTextBox1.Invoke(safeThread);
            }
            else
            {
                richTextBox1.SelectionStart = richTextBox1.TextLength;
                richTextBox1.SelectionLength = 0;

                richTextBox1.SelectionColor = color;
                richTextBox1.AppendText(DateTime.Now.ToString("HH:mm:ss,fff") + String.Format(" {0}", text) + Environment.NewLine);
                richTextBox1.SelectionColor = richTextBox1.ForeColor;
                richTextBox1.ScrollToCaret();
            }
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
