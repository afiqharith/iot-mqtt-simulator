using HardwareSimMqtt.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HardwareSimMqtt.UIComponent
{
    public partial class UiHardwareViewerGroup : UserControl
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
        public string DisplayLampId
        {
            get => LabelLampId.Text;
            set
            {
                LabelLampId.Text = value;
                LabelLampId.Tag = value;
            }
        }

        public string DisplayFanId
        {
            get => LabelFanId.Text;
            set
            {
                LabelFanId.Text = value;
                LabelFanId.Tag = value;
            }
        }

        public string DisplayFanSpeed
        {
            get => LabelFanSpeed.Text;
            set
            {
                SystemHelper.SafeInvoke(LabelFanSpeed, () =>
                {
                    LabelFanSpeed.Text = String.Format("{0}rpm", value);
                });
            }
        }

        public UiHardwareViewerGroup(eGroup egroup)
        {
            InitializeComponent();
            GroupLocation = egroup;
        }

        public UiHardwareViewerGroup()
        {
            InitializeComponent();
        }

        public void ToggleUiLamp(bool isOn)
        {
            splitContainerMain.Panel1.BackColor = GetUiBackColorIndicator(isOn);
        }

        public void ToggleUiFan(bool isOn)
        {
            splitContainerMain.Panel2.BackColor = GetUiBackColorIndicator(isOn);
        }

        private Color GetUiBackColorIndicator(bool isOn) => isOn ? Color.Green : Color.Gray;
    }
}
