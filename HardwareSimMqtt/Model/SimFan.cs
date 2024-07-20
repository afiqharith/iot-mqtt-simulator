using System.Windows.Forms;
using System.Drawing;
using HardwareSimMqtt.Model.BitMap;
using HardwareSimMqtt.UIComponent;
using HardwareSimMqtt.HardwareHub;
using System.Threading;
using System;
using System.Threading.Tasks;

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

        private HardwareViewerGroup hardwareViewer { get; set; }

        public override uint BitState
        {
            set
            {
                base.BitState = value;
                if (this.pPanel != null)
                {
                    this.pPanel.BackColor = GetUiBackColorIndicator(this.IsOn);
                }

                if (this.hardwareViewer != null)
                {
                    this.hardwareViewer.ToggleUiFan(this.IsOn);
                }
            }
        }

        private int _speed = -1;
        public virtual int Speed
        {
            get => _speed;
            set => SetSpeedProperty(ref _speed, value);
        }

        public SimFan(string id, eBitMask mask, eGroup group, eIoType ioType, int ioPort)
            : base(id, mask, eHardwareType.FAN, group, ioType, ioPort) { }

        public SimFan(string id, eBitMask mask, eGroup group, eIoType ioType, string portName, int baudRate)
            : base(id, mask, eHardwareType.FAN, group, ioType, portName, baudRate) { }

        private void SetPanelProperty(ref Panel panel, Panel newval) => panel = newval;

        private void SetSpeedProperty(ref int speed, int newval)
        {
            speed = newval;
            base.AnalogData = newval;

            //if (newval != -1)
            //{
            //    int rpm = newval;
            //    int rps = rpm / 60; //revolution per second

            //    while (!(base.AnalogData >= newval))
            //    {
            //        speed += rps;
            //        base.AnalogData += rps;
            //        Thread.Sleep(1000);
            //        //Task.Delay(1000);
            //        Console.WriteLine(DateTime.Now +" "+base.Id + " speed:" + speed + "rpm");
            //    }
            //    speed = -1;
            //    base.AnalogData = -1;

            //}
        }

        public void BindWithUIComponent(Panel panel)
        {
            this.pPanel = panel;
        }

        public void BindWithUiComponent(HardwareViewerGroup hardwareViewer)
        {
            this.hardwareViewer = hardwareViewer;
        }

        private Color GetUiBackColorIndicator(bool isOn) => isOn ? Color.Green : Color.Gray;
    }
}
