using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using CheckBox = System.Windows.Forms.CheckBox;
using Newtonsoft.Json;

namespace HardwareSimMqtt
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

        private Dictionary<string, List<CheckBox>> m_CheckBoxLocGroupMap { get; set; }
        private Dictionary<string, CheckBox> m_CheckBoxIdMap { get; set; }

        public ControlWindow()
        {
            InitializeComponent();
            m_QMsgContentToDisplayOnUI = new Queue<Dictionary<ushort, HardwareInfo>>();
            m_CheckBoxLocGroupMap = new Dictionary<string, List<CheckBox>>();
            m_CheckBoxIdMap = new Dictionary<string, CheckBox>();

            List<CheckBox> checkboxList = new List<CheckBox>()
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

            List<CheckBox> group1CheckboxList = new List<CheckBox>();
            List<CheckBox> group2CheckboxList = new List<CheckBox>();
            List<CheckBox> group3CheckboxList = new List<CheckBox>();
            List<CheckBox> group4CheckboxList = new List<CheckBox>();

            int nFanId = 0; 
            int nLampId = 0;
            for (int i = 0; i < checkboxList.Count; i++)
            {
                checkboxList[i].CheckStateChanged += new EventHandler(CheckboxUnit_CheckStateChanged);

                if(checkboxList[i].Name.ToLower().Contains("fan".ToLower()))
                {
                    nFanId++;
                    string strFanId = String.Format("F-ID{0}", nFanId);
                    m_CheckBoxIdMap.Add(strFanId, checkboxList[i]);
                }

                if (checkboxList[i].Name.ToLower().Contains("lamp".ToLower()))
                {
                    nLampId++;
                    string strLampId = String.Format("L-ID{0}", nLampId);
                    m_CheckBoxIdMap.Add(strLampId, checkboxList[i]);
                }
                
                if (checkboxList[i].Name.Contains("1")) 
                { 
                    group1CheckboxList.Add(checkboxList[i]);
                }
                else if (checkboxList[i].Name.Contains("2")) 
                { 
                    group2CheckboxList.Add(checkboxList[i]);
                }
                else if (checkboxList[i].Name.Contains("3")) 
                { 
                    group3CheckboxList.Add(checkboxList[i]);
                }
                else 
                { 
                    group4CheckboxList.Add(checkboxList[i]);
                }
            }
            m_CheckBoxLocGroupMap.Add("checkBoxLoc1", group1CheckboxList);
            m_CheckBoxLocGroupMap.Add("checkBoxLoc2", group2CheckboxList);
            m_CheckBoxLocGroupMap.Add("checkBoxLoc3", group3CheckboxList);
            m_CheckBoxLocGroupMap.Add("checkBoxLoc4", group4CheckboxList);

            m_BrokerConnectJob = new SetMqttBrokerConnectJob("broker.emqx.io");
            bool bEstablished = m_BrokerConnectJob.Run();
            if (bEstablished)
            {
                m_BrokerConnectJob.Client.MqttMsgPublished += OnMqttMessagePublished;
            }

        }

        private void CheckboxLoc_CheckStateChanged(object sender, EventArgs e)
        {
            CheckBox checkbox = (CheckBox)sender;
            if (checkbox.CheckState == CheckState.Indeterminate)
            {
                return;
            }

            List<HardwareInfo> hardwareInfoList = new List<HardwareInfo>();

            if(checkbox.Name != "checkBoxShutdownAll")
            {
                foreach (KeyValuePair<string, List<CheckBox>> kvpLoc in m_CheckBoxLocGroupMap)
                {
                    if (checkbox.Name == kvpLoc.Key)
                    {
                        for (int i = 0; i < kvpLoc.Value.Count; i++)
                        {
                            foreach (KeyValuePair<string, CheckBox> kvpId in m_CheckBoxIdMap)
                            {
                                if (kvpLoc.Value[i] == kvpId.Value)
                                {
                                    hardwareInfoList.Add(new HardwareInfo(kvpId.Key, Convert.ToUInt32(checkbox.Checked ? 1 : 0)));
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                foreach (KeyValuePair<string, CheckBox> kvpId in m_CheckBoxIdMap)
                {
                    hardwareInfoList.Add(new HardwareInfo(kvpId.Key, Convert.ToUInt32(checkbox.Checked ? 1 : 0)));
                }
            }

            PublishHardwareInfoToBroker(hardwareInfoList);
        }

        private void CheckboxUnit_CheckStateChanged(object sender, EventArgs e)
        {
            CheckBox checkbox = (CheckBox)sender;
            if (checkbox.CheckState == CheckState.Indeterminate)
            {
                return;
            }

            List<HardwareInfo> hardwareInfoList = new List<HardwareInfo>();

            foreach (KeyValuePair<string, CheckBox> kvpId in m_CheckBoxIdMap)
            {
                if (checkbox == kvpId.Value)
                {
                    hardwareInfoList.Add(new HardwareInfo(kvpId.Key, Convert.ToUInt32(checkbox.Checked ? 1 : 0)));
                }
            }

            PublishHardwareInfoToBroker(hardwareInfoList);
        }

        private void PublishHardwareInfoToBroker(List<HardwareInfo> hardwareInfoList)
        {
            string jsonifiedHardwareInfoList = JsonConvert.SerializeObject(new JsonHardwareInfoList(hardwareInfoList));

            if (m_BrokerConnectJob.Client.IsConnected)
            {
                //Publish JSON converted HardwareInfoList to MQTT server
                ushort msgID = m_BrokerConnectJob.Client.Publish(
                    "IotWinformSim",
                    Encoding.UTF8.GetBytes(jsonifiedHardwareInfoList),
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
