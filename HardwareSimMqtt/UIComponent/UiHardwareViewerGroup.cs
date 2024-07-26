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
            set => SetGroupLocationProperty(ref _egroup, value);
        }
        public UiHardwareViewerGroup(eGroup egroup)
        {
            InitializeComponent();
            this.GroupLocation = egroup;
        }

        public UiHardwareViewerGroup()
        {
            InitializeComponent();
        }

        private void SetGroupLocationProperty(ref eGroup _group, eGroup newval)
        {
            _group = newval;
            GroupBoxLoc.Text = String.Format("Group Loc{0}", (int)newval);
        }

        public void BindLampId(string id)
        {
            LabelLampId.Text = String.Format("Lamp ID{0}", (int)this.GroupLocation);
            LabelLampId.Tag = id;
        }

        public void BindFanId(string id)
        {
            LabelFanId.Text = String.Format("Fan ID{0}", (int)this.GroupLocation);
            LabelFanId.Tag = id;
        }

        public void BindACId(string id)
        {

        }

        public void BindGateId(string id)
        {

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
