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
    public partial class UiDeviceViewerGroup : UserControl
    {
        private DeviceGroup _egroup;
        public DeviceGroup GroupLocation
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

        public UiDeviceViewerGroup(DeviceGroup egroup)
        {
            InitializeComponent();
            GroupLocation = egroup;
        }

        public UiDeviceViewerGroup()
        {
            InitializeComponent();
        }

        public delegate void UpdateUiColor(Color color);

        public void UpdateUi(bool isOn, UpdateUiColor action)
        {
            if(action != null)
            {
                Color color = GetUiBackColorIndicator(isOn);
                action(color);
            }
        }

        public void UpdateUiLamp(Color color)
        {
            splitContainerMain.Panel1.BackColor = color;
        }

        public void UpdateUiFan(Color color)
        {
            splitContainerMain.Panel2.BackColor = color;
        }

        public void UpdateUiFanRampingSpeed()
        {
            splitContainerMain.Panel2.BackColor = Color.Yellow;
        }

        private Color GetUiBackColorIndicator(bool isOn) => isOn ? Color.Green : Color.Gray;
    }
}
