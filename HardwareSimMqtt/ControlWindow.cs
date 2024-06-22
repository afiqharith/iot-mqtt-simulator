using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using CheckBox = System.Windows.Forms.CheckBox;

namespace HardwareSimMqtt
{
    using DataContainer;
    public partial class ControlWindow : Form
    {
        private SetMqttBrokerConnectJob m_BrokerConnectJob { get; set; }
        private Queue<Dictionary<ushort, HardwareInfo>> m_QMsgContentToDisplayOnUI { get; set; }
        private Dictionary<CheckBox, List<CheckBox>> m_CheckBoxLocGroupMap { get; set; }
        private Dictionary<string, CheckBox> m_CheckBoxIdMap { get; set; }

        public ControlWindow()
        {
            InitializeComponent();
            m_QMsgContentToDisplayOnUI = new Queue<Dictionary<ushort, HardwareInfo>>();
            m_CheckBoxLocGroupMap = new Dictionary<CheckBox, List<CheckBox>>();
            m_CheckBoxIdMap = new Dictionary<string, CheckBox>();
            InitializeCheckBoxMapping();

            m_BrokerConnectJob = new SetMqttBrokerConnectJob("broker.emqx.io");
            bool bEstablished = m_BrokerConnectJob.Run();
            if (bEstablished)
            {
                m_BrokerConnectJob.Client.MqttMsgPublished += OnMqttMessagePublished;
            }
        }

        private void InitializeCheckBoxMapping()
        {
            List<CheckBox> locGroupCheckboxList = new List<CheckBox>()
            {
                checkBoxLoc1,
                checkBoxLoc2,
                checkBoxLoc3,
                checkBoxLoc4
            };

            List<CheckBox> unitCheckboxList = new List<CheckBox>()
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

            //Need to work on this to make simpler
            List<List<CheckBox>> groupCheckboxList = new List<List<CheckBox>>();
            //Create list based on the number of grouploc
            for (int i = 0; i < locGroupCheckboxList.Count; i++)
            {
                groupCheckboxList.Add(new List<CheckBox>());
            }

            int nFanId = 0;
            int nLampId = 0;
            for (int i = 0; i < unitCheckboxList.Count; i++)
            {
                unitCheckboxList[i].CheckStateChanged += new EventHandler(CheckboxUnit_CheckStateChanged);

                if (unitCheckboxList[i].Name.ToLower().Contains("fan".ToLower()))
                {
                    nFanId++;
                    string strFanId = String.Format("F_ID{0}", nFanId);
                    m_CheckBoxIdMap.Add(strFanId, unitCheckboxList[i]);
                }

                if (unitCheckboxList[i].Name.ToLower().Contains("lamp".ToLower()))
                {
                    nLampId++;
                    string strLampId = String.Format("L_ID{0}", nLampId);
                    m_CheckBoxIdMap.Add(strLampId, unitCheckboxList[i]);
                }

                //Need to work on this to make simpler
                if (unitCheckboxList[i].Name.Contains("1"))
                {
                    groupCheckboxList[0].Add(unitCheckboxList[i]);
                }
                else if (unitCheckboxList[i].Name.Contains("2"))
                {
                    groupCheckboxList[1].Add(unitCheckboxList[i]);
                }
                else if (unitCheckboxList[i].Name.Contains("3"))
                {
                    groupCheckboxList[2].Add(unitCheckboxList[i]);
                }
                else
                {
                    groupCheckboxList[3].Add(unitCheckboxList[i]);
                }
            }

            for(int i = 0; i < locGroupCheckboxList.Count; i++)
            {
                m_CheckBoxLocGroupMap.Add(locGroupCheckboxList[i], groupCheckboxList[i]);
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

            if (checkbox.Name != "checkBoxShutdownAll")
            {
                foreach (KeyValuePair<CheckBox, List<CheckBox>> kvpLoc in m_CheckBoxLocGroupMap)
                {
                    if (checkbox.Name == kvpLoc.Key.Name)
                    {
                        for (int i = 0; i < kvpLoc.Value.Count; i++)
                        {
                            foreach (KeyValuePair<string, CheckBox> kvpId in m_CheckBoxIdMap)
                            {
                                if (kvpLoc.Value[i] == kvpId.Value)
                                {
                                    kvpId.Value.CheckStateChanged -= new EventHandler(CheckboxUnit_CheckStateChanged);
                                    kvpId.Value.CheckState = checkbox.Checked ? CheckState.Indeterminate : CheckState.Unchecked;
                                    kvpId.Value.CheckStateChanged += new EventHandler(CheckboxUnit_CheckStateChanged);
                                    hardwareInfoList.Add(new HardwareInfo(kvpId.Key, Convert.ToUInt32(checkbox.CheckState)));
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
                    kvpId.Value.CheckStateChanged -= new EventHandler(CheckboxUnit_CheckStateChanged);                    
                    kvpId.Value.CheckState = checkbox.Checked ? CheckState.Indeterminate : CheckState.Unchecked;
                    kvpId.Value.CheckStateChanged += new EventHandler(CheckboxUnit_CheckStateChanged);
                    hardwareInfoList.Add(new HardwareInfo(kvpId.Key, Convert.ToUInt32(checkbox.CheckState)));
                }

                foreach (KeyValuePair<CheckBox, List<CheckBox>> kvpLoc in m_CheckBoxLocGroupMap)
                {

                        kvpLoc.Key.CheckStateChanged -= new EventHandler(CheckboxLoc_CheckStateChanged);
                        kvpLoc.Key.CheckState = checkbox.Checked ? CheckState.Indeterminate : CheckState.Unchecked;
                        kvpLoc.Key.CheckStateChanged += new EventHandler(CheckboxLoc_CheckStateChanged);
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
                    hardwareInfoList.Add(new HardwareInfo(kvpId.Key, Convert.ToUInt32(checkbox.CheckState)));
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
}
