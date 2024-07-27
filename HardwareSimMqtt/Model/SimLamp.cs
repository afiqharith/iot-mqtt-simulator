using System.Windows.Forms;
using System.Drawing;
using HardwareSimMqtt.Model.BitMap;
using HardwareSimMqtt.UIComponent;
using HardwareSimMqtt.HardwareHub;

namespace HardwareSimMqtt.Model
{
    internal class SimLamp : HardwareBase
    {
        private Panel _pPanel = null;
        private Panel pPanel
        {
            get => _pPanel;
            set => _pPanel = value;
        }

        public UiHardwareViewerGroup HardwareViewer 
        { 
            get; 
            set; 
        }

        public override uint BitState
        {
            set
            {
                base.BitState = value;
                if (pPanel != null)
                {
                    pPanel.BackColor = GetUiBackColorIndicator(IsOn);
                }

                if (HardwareViewer != null)
                {
                    HardwareViewer.ToggleUiLamp(IsOn);
                }
            }
        }

        public SimLamp(string id, eBitMask mask, eGroup location, eIoType ioType, int ioPort)
            : base(id, mask, eHardwareType.LAMP, location, ioType, ioPort) { }

        public SimLamp(string id, eBitMask mask, eGroup location, eIoType ioType, string portName, int baudRate)
            : base(id, mask, eHardwareType.LAMP, location, ioType, portName, baudRate) { }

        //Deprecated: Currently not in use
        public void BindWithUIComponent(Panel panel)
        {
            pPanel = panel;
        }

        private Color GetUiBackColorIndicator(bool isOn) => isOn ? Color.Green : Color.Gray;
    }
}
