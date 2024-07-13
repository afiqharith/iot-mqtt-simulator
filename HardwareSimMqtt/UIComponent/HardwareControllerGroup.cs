using HardwareSimMqtt.EventArgsModel;
using HardwareSimMqtt.Model;
using HardwareSimMqtt.Model.BitMap;
using HardwareSimMqtt.Model.DataContainer;
using HardwareSimMqtt.Model.QueryJob;
using ModelInterface;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace HardwareSimMqtt.UIComponent
{
    public partial class HardwareControllerGroup : UserControl
    {
        private eGroup _egroup;
        public eGroup GroupLocation
        {
            get => _egroup;
            set => SetGroupLocationProperty(ref _egroup, value);
        }

        private Dictionary<CheckBox, eBitMask> checkBoxMaskMap
        {
            get;
            set;
        }

        private SetBrokerConnectJob hwControllerBrokerConnectJob
        {
            get;
            set;
        }

        private static EventHandler<PublishBitInfoToBrokerEventArgs> publishingBitInfoToBrokerEventHandler;

        public HardwareControllerGroup(eGroup egroup, SetBrokerConnectJob controllerBrokerConnectJob, EventHandler<PublishBitInfoToBrokerEventArgs> publishBitInfoToBrokerEventHandler)
        {
            InitializeComponent();
            this.GroupLocation = egroup;
            this.CheckBoxBoth.Text = "Both";
            this.CheckBoxBoth.CheckStateChanged += new EventHandler(CheckboxBoth_OnCheckStateChanged);

            this.checkBoxMaskMap = new Dictionary<CheckBox, eBitMask>();
            publishingBitInfoToBrokerEventHandler = publishBitInfoToBrokerEventHandler;
            this.hwControllerBrokerConnectJob = controllerBrokerConnectJob;
        }

        private void SetGroupLocationProperty(ref eGroup _group, eGroup newval)
        {
            _group = newval;
            this.GroupBoxLoc.Text = String.Format("Group Loc{0}", (int)newval);
        }

        public void BindCheckboxLampId(string id, eBitMask mask)
        {
            this.CheckBoxLamp.Text = String.Format("Lamp ID{0}", (int)this.GroupLocation); ;
            this.CheckBoxLamp.Tag = id;
            this.CheckBoxLamp.CheckStateChanged += new EventHandler(CheckboxUnit_OnCheckStateChanged);

            this.checkBoxMaskMap.Add(this.CheckBoxLamp, mask);
        }

        public void BindCheckboxFanId(string id, eBitMask mask)
        {
            this.CheckBoxFan.Text = String.Format("Fan ID{0}", (int)this.GroupLocation);
            this.CheckBoxFan.Tag = id;
            this.CheckBoxFan.CheckStateChanged += new EventHandler(CheckboxUnit_OnCheckStateChanged);

            this.checkBoxMaskMap.Add(this.CheckBoxFan, mask);
        }

        private void CheckboxUnit_OnCheckStateChanged(object sender, EventArgs e)
        {
            CheckBox checkbox = (CheckBox)sender;
            if (checkbox.CheckState == CheckState.Indeterminate)
            {
                return;
            }

            List<BitInfo> bitInfoList = new List<BitInfo>();

            foreach (KeyValuePair<CheckBox, eBitMask> kvp in checkBoxMaskMap)
            {
                if (checkbox.Tag == kvp.Key.Tag)
                {
                    uint bitState = checkbox.Checked ? (uint)checkBoxMaskMap[checkbox] : ((uint)checkBoxMaskMap[checkbox] & (uint)~checkBoxMaskMap[checkbox]);
                    bitInfoList.Add(new BitInfo((string)checkbox.Tag, bitState));
                }
            }

            PublishBitInfoToBroker(bitInfoList);
        }

        private void CheckboxBoth_OnCheckStateChanged(object sender, EventArgs e)
        {
            CheckBox checkbox = (CheckBox)sender;
            if (checkbox.CheckState == CheckState.Indeterminate)
            {
                return;
            }

            List<BitInfo> bitInfoList = new List<BitInfo>();

            foreach (KeyValuePair<CheckBox, eBitMask> kvp in checkBoxMaskMap)
            {
                kvp.Key.CheckStateChanged -= new EventHandler(CheckboxUnit_OnCheckStateChanged);
                kvp.Key.CheckState = checkbox.Checked ? CheckState.Indeterminate : CheckState.Unchecked;
                kvp.Key.CheckStateChanged += new EventHandler(CheckboxUnit_OnCheckStateChanged);

                uint bitState = checkbox.Checked ? (uint)checkBoxMaskMap[kvp.Key] : ((uint)checkBoxMaskMap[kvp.Key] & (uint)~checkBoxMaskMap[kvp.Key]);
                bitInfoList.Add(new BitInfo((string)kvp.Key.Tag, bitState));
            }

            PublishBitInfoToBroker(bitInfoList);
        }

        public void CheckboxAll_OnCheckStateChanged(object sender, EventArgs e)
        {
            CheckBox checkbox = (CheckBox)sender;
            if (checkbox.CheckState == CheckState.Indeterminate)
            {
                return;
            }

            List<BitInfo> bitInfoList = new List<BitInfo>();

            foreach (KeyValuePair<CheckBox, eBitMask> kvp in checkBoxMaskMap)
            {
                kvp.Key.CheckStateChanged -= new EventHandler(CheckboxUnit_OnCheckStateChanged);
                kvp.Key.CheckState = checkbox.Checked ? CheckState.Indeterminate : CheckState.Unchecked;
                kvp.Key.CheckStateChanged += new EventHandler(CheckboxUnit_OnCheckStateChanged);

                uint bitState = checkbox.Checked ? (uint)checkBoxMaskMap[kvp.Key] : ((uint)checkBoxMaskMap[kvp.Key] & (uint)~checkBoxMaskMap[kvp.Key]);
                bitInfoList.Add(new BitInfo((string)kvp.Key.Tag, bitState));
            }
            this.CheckBoxBoth.CheckStateChanged -= new EventHandler(CheckboxBoth_OnCheckStateChanged);
            this.CheckBoxBoth.CheckState = checkbox.Checked ? CheckState.Indeterminate : CheckState.Unchecked;
            this.CheckBoxBoth.CheckStateChanged += new EventHandler(CheckboxBoth_OnCheckStateChanged);

            PublishBitInfoToBroker(bitInfoList);
        }

        private void PublishBitInfoToBroker(List<BitInfo> bitInfoList)
        {
            string jsonifiedBitInfoList = JsonConvert.SerializeObject(new JsonBitInfoList(bitInfoList));

            if (this.hwControllerBrokerConnectJob.Client.IsConnected)
            {
                //Publish JSON converted HardwareInfoList to MQTT server
                ushort msgID = this.hwControllerBrokerConnectJob.Client.Publish(
                    "MqttBitInfoBroker",
                    Encoding.UTF8.GetBytes(jsonifiedBitInfoList),
                    MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE,
                    true);

                publishingBitInfoToBrokerEventHandler.Invoke(null, new PublishBitInfoToBrokerEventArgs(msgID, bitInfoList));
            }
        }
    }
}
