﻿using HardwareSimMqtt.EventArgsModel;
using HardwareSimMqtt.Model;
using HardwareSimMqtt.Model.BitMap;
using HardwareSimMqtt.Model.DataContainer;
using HardwareSimMqtt.Model.QueryJob;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace HardwareSimMqtt.UIComponent
{
    public partial class UiHardwareControllerGroup : UserControl
    {
        private eGroup _egroup;
        public eGroup GroupLocation
        {
            get => _egroup;
            set
            {
                _egroup = value;
                GroupBoxLoc.Text = String.Format("Group Loc{0}", (int)value);
            }
        }

        public string CheckBoxLampId
        {
            get => CheckBoxLamp.Text;
            set
            {
                CheckBoxLamp.Text = String.Format("Lamp ID{0}", (int)GroupLocation);
                CheckBoxLamp.Tag = value;
                CheckBoxLamp.CheckStateChanged += new EventHandler(CheckboxUnit_OnCheckStateChanged);
            }
        }

        private eBitMask _checkBoxLampMask;
        public eBitMask CheckBoxLampMask
        {
            get => _checkBoxLampMask;
            set
            {
                _checkBoxLampMask = value;
                checkBoxMaskMap.Add(CheckBoxLamp, value);
            }
        }

        public string CheckBoxFanId
        {
            get => CheckBoxFan.Text;
            set
            {
                CheckBoxFan.Text = String.Format("Fan ID{0}", (int)GroupLocation);
                CheckBoxFan.Tag = value;
                CheckBoxFan.CheckStateChanged += new EventHandler(CheckboxUnit_OnCheckStateChanged);
            }
        }

        private eBitMask _checkBoxFanMask;
        public eBitMask CheckBoxFanMask
        {
            get => _checkBoxFanMask;
            set
            {
                _checkBoxFanMask = value;
                checkBoxMaskMap.Add(CheckBoxFan, value);
            }
        }

        private Dictionary<CheckBox, eBitMask> checkBoxMaskMap
        {
            get;
            set;
        }

        private ListenerWindow ParentWindow
        {
            get => Program.WndHandle;
        }

        public UiHardwareControllerGroup(eGroup egroup)
        {
            InitializeComponent();
            GroupLocation = egroup;
            CheckBoxBoth.Text = "Both";
            CheckBoxBoth.CheckStateChanged += new EventHandler(CheckboxBoth_OnCheckStateChanged);
            checkBoxMaskMap = new Dictionary<CheckBox, eBitMask>();
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
            ParentWindow.PublishPartialBitInfoToBroker(bitInfoList);
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
            ParentWindow.PublishPartialBitInfoToBroker(bitInfoList);
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
            CheckBoxBoth.CheckStateChanged -= new EventHandler(CheckboxBoth_OnCheckStateChanged);
            CheckBoxBoth.CheckState = checkbox.Checked ? CheckState.Indeterminate : CheckState.Unchecked;
            CheckBoxBoth.CheckStateChanged += new EventHandler(CheckboxBoth_OnCheckStateChanged);

            ParentWindow.PublishAllBitInfoToBroker(bitInfoList);
        }

    }
}
