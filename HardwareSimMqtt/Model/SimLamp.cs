using System.Windows.Forms;
using System.Drawing;
using HardwareSimMqtt.Model.BitMap;
using HardwareSimMqtt.UIComponent;
using HardwareSimMqtt.HardwareHub;
using HardwareSimMqtt.Utils;

namespace HardwareSimMqtt.Model
{
    internal class SimLamp : DeviceBase
    {
        private Panel _pPanel = null;
        private Panel pPanel
        {
            get => _pPanel;
            set => _pPanel = value;
        }

        public UiDeviceViewerGroup DeviceViewer 
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

                if (DeviceViewer != null)
                {
                    DeviceViewer.UpdateUi(IsOn, DeviceViewer.UpdateUiLamp);
                }
            }
        }

        public SimLamp(string id, DeviceBitMask deviceBitMask, DeviceGroup deviceGroup, IoType ioType, int ioPort)
            : base(id, deviceBitMask, DeviceType.LAMP, deviceGroup, ioType, ioPort) { }

        public SimLamp(string id, DeviceBitMask deviceBitMask, DeviceGroup deviceGroup, IoType ioType, string portName, int baudRate)
            : base(id, deviceBitMask, DeviceType.LAMP, deviceGroup, ioType, portName, baudRate) { }

        public SimLamp(DevcieConfig deviceConfig)
            : base(deviceConfig) { }

        //Deprecated: Currently not in use
        public void BindWithUIComponent(Panel panel)
        {
            pPanel = panel;
        }

        private Color GetUiBackColorIndicator(bool isOn) => isOn ? Color.Green : Color.Gray;
    }
}
