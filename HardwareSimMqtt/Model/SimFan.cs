using System.Windows.Forms;
using System.Drawing;
using HardwareSimMqtt.Model.BitMap;
using HardwareSimMqtt.UIComponent;

namespace HardwareSimMqtt.Model
{
    internal class SimFan : HardwareBase
    {
        private Panel _pPanel = null;
        private Panel pPanel
        {
            get => _pPanel;
            set => SetPanelProperty(ref _pPanel, value);
        }

        private HardwareViewer hardwareViewer { get; set; }

        public override uint BitState
        {
            set
            {
                base.BitState = value;
                if (this.pPanel != null)
                {
                    this.pPanel.BackColor = GetUiBackColorIndicator(this.IsOn);
                }

                if(this.hardwareViewer != null)
                {
                    this.hardwareViewer.ToggleUiFan(this.IsOn);
                }
            }
        }

        private double _speed = -1;
        public virtual double Speed
        {
            get => _speed;
            protected set => SetSpeedProperty(ref _speed, value);
        }

        public SimFan(string id, eBitMask mask, eLocation location,  int ioPort)
            : base(id, mask, eHardwareType.FAN, location, ioPort) { }

        public SimFan(string id, eBitMask mask, eLocation location,  string portName, int baudRate)
            : base(id, mask, eHardwareType.FAN, location, portName, baudRate) { }

        private void SetPanelProperty(ref Panel panel, Panel newval) => panel = newval;

        private void SetSpeedProperty(ref double speed, double newval)
        {
            speed = newval;
            base.AnalogData = newval;
        }

        public void BindWithUIComponent(Panel panel)
        {
            this.pPanel = panel;
        }

        public void BindWithUiComponent(HardwareViewer hardwareViewer)
        {
            this.hardwareViewer = hardwareViewer;
        }

        private Color GetUiBackColorIndicator(bool isOn) => isOn ? Color.Green : Color.Gray;
    }
}
