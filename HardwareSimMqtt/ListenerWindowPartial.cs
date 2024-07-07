using BitMap;
using DataContainer;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace HardwareSimMqtt
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
        private SetMqttBrokerConnectJob controllerBrokerConnectJob { get; set; }
        private Queue<Dictionary<ushort, BitInfo>> QMsgContentToDisplayOnUI { get; set; }
        private Dictionary<CheckBox, UnitCheckBoxList> LocGroupCheckBoxListUnitCBMap { get; set; }
        private Dictionary<string, CheckBox> IdCheckBoxMap { get; set; }
        private Dictionary<CheckBox, eBitMask> CheckBoxMaskMap { get; set; }

        private void InitializeCheckBoxMapping()
        {
            LocGroupCheckBoxListUnitCBMap = new Dictionary<CheckBox, UnitCheckBoxList>();
            IdCheckBoxMap = new Dictionary<string, CheckBox>();
            CheckBoxMaskMap = new Dictionary<CheckBox, eBitMask>();

            //List containing all unit check box
            List<CheckBox> checkBoxUnitList = new List<CheckBox>()
            {
                checkBoxUnitFan1,
                checkBoxUnitFan2,
                checkBoxUnitFan3,
                checkBoxUnitFan4,

                checkBoxUnitLamp1,
                checkBoxUnitLamp2,
                checkBoxUnitLamp3,
                checkBoxUnitLamp4
            };
            //List containing all group check box
            List<CheckBox> checkBoxLocGroupList = new List<CheckBox>()
            {
                checkBoxLocGroup1,
                checkBoxLocGroup2,
                checkBoxLocGroup3,
                checkBoxLocGroup4,
                checkBoxShutdownAll

            };

            //Need to work on this to make simpler
            List<UnitCheckBoxList> groupedUnitCheckBoxList = new List<UnitCheckBoxList>();

            //Create list based on the number of grouploc
            //So each check box group contain list of unit check box
            for (int i = 0; i < checkBoxLocGroupList.Count; i++)
            {
                checkBoxLocGroupList[i].CheckStateChanged += new EventHandler(CheckboxLoc_CheckStateChanged);
                if(checkBoxLocGroupList[i] == checkBoxShutdownAll)
                {
                    continue;
                }
                groupedUnitCheckBoxList.Add(new UnitCheckBoxList());
            }


            int nFanId = 0;
            int nLampId = 0;

            for (int i = 0; i < checkBoxUnitList.Count; i++)
            {
                //Register unit check box event handler
                checkBoxUnitList[i].CheckStateChanged += new EventHandler(CheckboxUnit_CheckStateChanged);

                //Map bitmask to each unit check box
                int nMask = 1 << (int)i;
                CheckBoxMaskMap.Add(checkBoxUnitList[i], (eBitMask)nMask);

                //Need to work on this to make it simpler
                {
                    if (checkBoxUnitList[i].Name.ToLower().Contains("fan".ToLower()))
                    {
                        nFanId++;
                        string strFanId = String.Format("F_ID{0}", nFanId);
                        IdCheckBoxMap.Add(strFanId, checkBoxUnitList[i]);
                    }
                    else if (checkBoxUnitList[i].Name.ToLower().Contains("lamp".ToLower()))
                    {
                        nLampId++;
                        string strLampId = String.Format("L_ID{0}", nLampId);
                        IdCheckBoxMap.Add(strLampId, checkBoxUnitList[i]);
                    }

                    if (checkBoxUnitList[i].Name.Contains("1"))
                    {
                        groupedUnitCheckBoxList[0].Data.Add(checkBoxUnitList[i]);
                    }
                    else if (checkBoxUnitList[i].Name.Contains("2"))
                    {
                        groupedUnitCheckBoxList[1].Data.Add(checkBoxUnitList[i]);
                    }
                    else if (checkBoxUnitList[i].Name.Contains("3"))
                    {
                        groupedUnitCheckBoxList[2].Data.Add(checkBoxUnitList[i]);
                    }
                    else
                    {
                        groupedUnitCheckBoxList[3].Data.Add(checkBoxUnitList[i]);
                    }
                }
            }

            for (int i = 0; i < checkBoxLocGroupList.Count; i++)
            {
                if (checkBoxLocGroupList[i] == checkBoxShutdownAll)
                {
                    continue;
                }
                LocGroupCheckBoxListUnitCBMap.Add(checkBoxLocGroupList[i], groupedUnitCheckBoxList[i]);
            }
        }

        private void CheckboxLoc_CheckStateChanged(object sender, EventArgs e)
        {
            CheckBox checkbox = (CheckBox)sender;
            if (checkbox.CheckState == CheckState.Indeterminate)
            {
                return;
            }

            List<BitInfo> bitInfoList = new List<BitInfo>();

            if (checkbox.Name != "checkBoxShutdownAll")
            {
                foreach (KeyValuePair<CheckBox, UnitCheckBoxList> kvpLoc in LocGroupCheckBoxListUnitCBMap)
                {
                    if (checkbox.Name == kvpLoc.Key.Name)
                    {
                        for (int i = 0; i < kvpLoc.Value.Data.Count; i++)
                        {
                            foreach (KeyValuePair<string, CheckBox> kvpId in IdCheckBoxMap)
                            {
                                if (kvpLoc.Value.Data[i] == kvpId.Value)
                                {
                                    kvpId.Value.CheckStateChanged -= new EventHandler(CheckboxUnit_CheckStateChanged);
                                    kvpId.Value.CheckState = checkbox.Checked ? CheckState.Indeterminate : CheckState.Unchecked;
                                    kvpId.Value.CheckStateChanged += new EventHandler(CheckboxUnit_CheckStateChanged);


                                    uint bitState = checkbox.Checked ? (uint)CheckBoxMaskMap[kvpId.Value] : ((uint)CheckBoxMaskMap[kvpId.Value] & (uint)~CheckBoxMaskMap[kvpId.Value]);
                                    bitInfoList.Add(new BitInfo(kvpId.Key, bitState));
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                foreach (KeyValuePair<string, CheckBox> kvpId in IdCheckBoxMap)
                {
                    kvpId.Value.CheckStateChanged -= new EventHandler(CheckboxUnit_CheckStateChanged);
                    kvpId.Value.CheckState = checkbox.Checked ? CheckState.Indeterminate : CheckState.Unchecked;
                    kvpId.Value.CheckStateChanged += new EventHandler(CheckboxUnit_CheckStateChanged);

                    uint bitState = checkbox.Checked ? (uint)CheckBoxMaskMap[kvpId.Value] : ((uint)CheckBoxMaskMap[kvpId.Value] & (uint)~CheckBoxMaskMap[kvpId.Value]);
                    bitInfoList.Add(new BitInfo(kvpId.Key, bitState));
                }

                foreach (KeyValuePair<CheckBox, UnitCheckBoxList> kvpLoc in LocGroupCheckBoxListUnitCBMap)
                {
                    kvpLoc.Key.CheckStateChanged -= new EventHandler(CheckboxLoc_CheckStateChanged);
                    kvpLoc.Key.CheckState = checkbox.Checked ? CheckState.Indeterminate : CheckState.Unchecked;
                    kvpLoc.Key.CheckStateChanged += new EventHandler(CheckboxLoc_CheckStateChanged);
                }
            }

            PublishHardwareInfoToBroker(bitInfoList);
        }

        private void CheckboxUnit_CheckStateChanged(object sender, EventArgs e)
        {
            CheckBox checkbox = (CheckBox)sender;
            if (checkbox.CheckState == CheckState.Indeterminate)
            {
                return;
            }

            List<BitInfo> bitInfoList = new List<BitInfo>();

            foreach (KeyValuePair<string, CheckBox> kvpId in IdCheckBoxMap)
            {
                if (checkbox == kvpId.Value)
                {
                    uint bitState = checkbox.Checked ? (uint)CheckBoxMaskMap[kvpId.Value] : ((uint)CheckBoxMaskMap[kvpId.Value] & (uint)~CheckBoxMaskMap[kvpId.Value]);
                    bitInfoList.Add(new BitInfo(kvpId.Key, bitState));
                }
            }

            PublishHardwareInfoToBroker(bitInfoList);
        }

        private void PublishHardwareInfoToBroker(List<BitInfo> bitInfoList)
        {
            string jsonifiedBitInfoList = JsonConvert.SerializeObject(new JsonBitInfoList(bitInfoList));

            if (controllerBrokerConnectJob.Client.IsConnected)
            {
                //Publish JSON converted HardwareInfoList to MQTT server
                ushort msgID = controllerBrokerConnectJob.Client.Publish(
                    "IotWinformSim",
                    Encoding.UTF8.GetBytes(jsonifiedBitInfoList),
                    MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE,
                    true);

                for (int i = 0; i < bitInfoList.Count; i++)
                {
                    //Queue HardwareInfoList content to display on UI 
                    Dictionary<ushort, BitInfo> messageMap = new Dictionary<ushort, BitInfo>();
                    messageMap.Add(msgID, bitInfoList[i]);
                    QMsgContentToDisplayOnUI.Enqueue(messageMap);

                    ContollerLogInfo(String.Format(
                        "ID[{0}] HW state change command sent. HWID: {1}, cmd bit: 0x{2:D4}",
                        msgID,
                        bitInfoList[i].Id,
                        bitInfoList[i].CurrentBitState.ToString("X")),
                        Color.Gray);
                }
            }
        }

        private void OnMqttMessagePublished(object sender, MqttMsgPublishedEventArgs e)
        {
            if (e.IsPublished)
            {
                while (QMsgContentToDisplayOnUI.Count > 0)
                {
                    //De-queue message content to display on UI
                    Dictionary<ushort, BitInfo> messageMap = QMsgContentToDisplayOnUI.Dequeue();

                    foreach (KeyValuePair<ushort, BitInfo> kvp in messageMap)
                    {
                        BitInfo bitInfo = kvp.Value;

                        ContollerLogInfo(String.Format(
                            "ID[{0}] HW state change command published. HWID: {1}, cmd bit: 0x{2:D4}",
                            kvp.Key,
                            bitInfo.Id,
                            bitInfo.CurrentBitState.ToString("X")),
                            bitInfo.CurrentBitState != 0 ? Color.Blue : Color.OrangeRed);

                    }
                }
            }
        }

        private void ContollerLogInfo(string text, Color color)
        {
            SystemHelper.AppendRTBText(richTextBox2, text, color);
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
