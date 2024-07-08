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
using HardwareSimMqtt.Model.BitMap;
using HardwareSimMqtt.Model.DataContainer;

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

        private Dictionary<CheckBox, UnitCheckBoxList> locGroupCheckBoxListUnitCBMap
        {
            get;
            set;
        }

        private Dictionary<string, CheckBox> idCheckBoxMap
        {
            get;
            set;
        }

        private Dictionary<CheckBox, eBitMask> checkBoxMaskMap
        {
            get;
            set;
        }

        private void InitializeCheckBoxMapping()
        {
            locGroupCheckBoxListUnitCBMap = new Dictionary<CheckBox, UnitCheckBoxList>();
            idCheckBoxMap = new Dictionary<string, CheckBox>();
            checkBoxMaskMap = new Dictionary<CheckBox, eBitMask>();

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
                checkBoxLocGroupList[i].CheckStateChanged += new EventHandler(CheckboxLoc_OnCheckStateChanged);
                if (checkBoxLocGroupList[i] == checkBoxShutdownAll)
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
                checkBoxUnitList[i].CheckStateChanged += new EventHandler(CheckboxUnit_OnCheckStateChanged);

                //Map bitmask to each unit check box
                int nMask = 1 << (int)i;
                checkBoxMaskMap.Add(checkBoxUnitList[i], (eBitMask)nMask);

                //Need to work on this to make it simpler
                {
                    if (checkBoxUnitList[i].Name.ToLower().Contains("fan".ToLower()))
                    {
                        nFanId++;
                        string strFanId = String.Format("HWFID{0}", nFanId);
                        idCheckBoxMap.Add(strFanId, checkBoxUnitList[i]);
                    }
                    else if (checkBoxUnitList[i].Name.ToLower().Contains("lamp".ToLower()))
                    {
                        nLampId++;
                        string strLampId = String.Format("HWLID{0}", nLampId);
                        idCheckBoxMap.Add(strLampId, checkBoxUnitList[i]);
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
                locGroupCheckBoxListUnitCBMap.Add(checkBoxLocGroupList[i], groupedUnitCheckBoxList[i]);
            }
        }

        private void CheckboxLoc_OnCheckStateChanged(object sender, EventArgs e)
        {
            CheckBox checkbox = (CheckBox)sender;
            if (checkbox.CheckState == CheckState.Indeterminate)
            {
                return;
            }

            List<BitInfo> bitInfoList = new List<BitInfo>();

            if (checkbox.Name != "checkBoxShutdownAll")
            {
                foreach (KeyValuePair<CheckBox, UnitCheckBoxList> kvpLoc in locGroupCheckBoxListUnitCBMap)
                {
                    if (checkbox.Name == kvpLoc.Key.Name)
                    {
                        for (int i = 0; i < kvpLoc.Value.Data.Count; i++)
                        {
                            foreach (KeyValuePair<string, CheckBox> kvpId in idCheckBoxMap)
                            {
                                if (kvpLoc.Value.Data[i] == kvpId.Value)
                                {
                                    kvpId.Value.CheckStateChanged -= new EventHandler(CheckboxUnit_OnCheckStateChanged);
                                    kvpId.Value.CheckState = checkbox.Checked ? CheckState.Indeterminate : CheckState.Unchecked;
                                    kvpId.Value.CheckStateChanged += new EventHandler(CheckboxUnit_OnCheckStateChanged);


                                    uint bitState = checkbox.Checked ? (uint)checkBoxMaskMap[kvpId.Value] : ((uint)checkBoxMaskMap[kvpId.Value] & (uint)~checkBoxMaskMap[kvpId.Value]);
                                    bitInfoList.Add(new BitInfo(kvpId.Key, bitState));
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                foreach (KeyValuePair<string, CheckBox> kvpId in idCheckBoxMap)
                {
                    kvpId.Value.CheckStateChanged -= new EventHandler(CheckboxUnit_OnCheckStateChanged);
                    kvpId.Value.CheckState = checkbox.Checked ? CheckState.Indeterminate : CheckState.Unchecked;
                    kvpId.Value.CheckStateChanged += new EventHandler(CheckboxUnit_OnCheckStateChanged);

                    uint bitState = checkbox.Checked ? (uint)checkBoxMaskMap[kvpId.Value] : ((uint)checkBoxMaskMap[kvpId.Value] & (uint)~checkBoxMaskMap[kvpId.Value]);
                    bitInfoList.Add(new BitInfo(kvpId.Key, bitState));
                }

                foreach (KeyValuePair<CheckBox, UnitCheckBoxList> kvpLoc in locGroupCheckBoxListUnitCBMap)
                {
                    kvpLoc.Key.CheckStateChanged -= new EventHandler(CheckboxLoc_OnCheckStateChanged);
                    kvpLoc.Key.CheckState = checkbox.Checked ? CheckState.Indeterminate : CheckState.Unchecked;
                    kvpLoc.Key.CheckStateChanged += new EventHandler(CheckboxLoc_OnCheckStateChanged);
                }
            }

            PublishBitInfoToBroker(bitInfoList);
        }

        private void CheckboxUnit_OnCheckStateChanged(object sender, EventArgs e)
        {
            CheckBox checkbox = (CheckBox)sender;
            if (checkbox.CheckState == CheckState.Indeterminate)
            {
                return;
            }

            List<BitInfo> bitInfoList = new List<BitInfo>();

            foreach (KeyValuePair<string, CheckBox> kvpId in idCheckBoxMap)
            {
                if (checkbox == kvpId.Value)
                {
                    uint bitState = checkbox.Checked ? (uint)checkBoxMaskMap[kvpId.Value] : ((uint)checkBoxMaskMap[kvpId.Value] & (uint)~checkBoxMaskMap[kvpId.Value]);
                    bitInfoList.Add(new BitInfo(kvpId.Key, bitState));
                }
            }

            PublishBitInfoToBroker(bitInfoList);
        }

        private void PublishBitInfoToBroker(List<BitInfo> bitInfoList)
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

                for (int i = 0; i < bitInfoList.Count; i++)
                {
                    //Queue HardwareInfoList content to display on UI 
                    Dictionary<ushort, BitInfo> messageMap = new Dictionary<ushort, BitInfo>();
                    messageMap.Add(msgID, bitInfoList[i]);
                    qMsgContentToDisplayOnUI.Enqueue(messageMap);

                    ContollerLogInfo(String.Format(
                        "ID[{0}] HW state change command sent. HWID: {1}, cmd bit: 0x{2:D4}",
                        msgID,
                        bitInfoList[i].Id,
                        bitInfoList[i].BitState.ToString("X")),
                        Color.Gray);
                }
            }
        }

        private void OnMessagePublished(object sender, MqttMsgPublishedEventArgs e)
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

    public class SetBrokerConnectJob
    {
        public MqttClient Client
        {
            get;
            set;
        }

        public string BrokerHostName
        {
            get;
            private set;
        }

        public SetBrokerConnectJob(string brokerHostName)
        {
            this.BrokerHostName = brokerHostName;

            if (this.Client == null)
            {
                this.Client = new MqttClient(this.BrokerHostName);
            }
        }

        public virtual bool Run()
        {
            if(this.Client == null)
            { 
                return false; 
            }

            string guid = Convert.ToString(Guid.NewGuid());
            bool bSuccess = true;
            try
            {
                this.Client.Connect(guid, "emqx", "public");
            }
            catch
            {
                bSuccess = false;
            }

            return bSuccess;
        }
    }
}
