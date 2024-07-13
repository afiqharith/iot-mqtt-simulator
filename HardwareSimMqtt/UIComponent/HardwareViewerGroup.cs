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
    public partial class HardwareViewerGroup : UserControl
    {
        private eGroup _egroup;
        public eGroup GroupLocation
        {
            get => _egroup;
            set => SetGroupLocationProperty(ref _egroup, value);
        }
        public HardwareViewerGroup(eGroup egroup)
        {
            InitializeComponent();
            this.GroupLocation = egroup;
        }

        public HardwareViewerGroup()
        {
            InitializeComponent();
        }

        private void SetGroupLocationProperty(ref eGroup _group, eGroup newval)
        {
            _group = newval;
            this.GroupBoxLoc.Text = String.Format("Group Loc{0}", (int)newval);
        }

        public void BindLampId(string id)
        {
            this.LabelLampId.Text = String.Format("Lamp ID{0}", (int)this.GroupLocation); ;
            this.LabelLampId.Tag = id;
        }

        public void BindFanId(string id)
        {
            this.LabelFanId.Text = String.Format("Fan ID{0}", (int)this.GroupLocation); ;
            this.LabelFanId.Tag = id;
        }

        public void BindACId(string id)
        {

        }

        public void BindGateId(string id)
        {

        }

        public void ToggleUiLamp(bool isOn)
        {
            this.splitContainerMain.Panel1.BackColor = GetUiBackColorIndicator(isOn);
        }

        public void ToggleUiFan(bool isOn)
        {
            this.splitContainerMain.Panel2.BackColor = GetUiBackColorIndicator(isOn);
        }

        private Color GetUiBackColorIndicator(bool isOn) => isOn ? Color.Green : Color.Gray;
    }
}
