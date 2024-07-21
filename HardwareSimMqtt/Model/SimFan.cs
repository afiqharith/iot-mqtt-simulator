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
            //speed = newval;
            //base.AnalogData = newval;
            int tempSpeed = 0;

            //Simulate speed increase overtime (interval 0.5s)
            if (newval != -1)
            {
                int rpm = newval;
                int rps = rpm / 60; //revolution per second

                Thread thread = new Thread(() =>
                {
                    while (!(base.AnalogData >= newval))
                    {
                        int randRps = new Random().Next(1, rps);
                        tempSpeed += randRps;
                        base.AnalogData += randRps;
                        Thread.Sleep(250);
                        Console.WriteLine(DateTime.Now + " " + base.Id + " speed:" + tempSpeed + "rpm");
                    }
                });
                thread.Start();

                speed = tempSpeed;
                base.AnalogData = tempSpeed;
            }
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
