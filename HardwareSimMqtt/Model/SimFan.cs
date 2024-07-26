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

        private UiHardwareViewerGroup hardwareViewer { get; set; }

        public override uint BitState
        {
            set
            {
                base.BitState = value;

                if (pPanel != null)
                {
                    pPanel.BackColor = GetUiBackColorIndicator(IsOn);
                }

                if (hardwareViewer != null)
                {
                    hardwareViewer.ToggleUiFan(IsOn);
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
            if (ComController.GetType() != typeof(HHEmuGPIOController))
            {
                speed = newval;
                base.AnalogData = newval;
            }
            //Simulate the analog value increment/decrement
            else
            {
                if (base.IsOn)
                {
                    int tempSpeed = /*0*/_speed;
                    //Simulate speed increase overtime (interval 0.5s)
                    if (newval != -1)
                    {
                        int rpm = newval;
                        int rps = rpm / 60; //revolution per second

                        Thread thread = new Thread(() =>
                        {
                            while (!(base.AnalogData >= newval) && IsOn && !Program.CancelTokenSource.Token.IsCancellationRequested)
                            {
                                int randRps = new Random().Next(1, rps);
                                tempSpeed += randRps;
                                base.AnalogData += randRps;
                                Thread.Sleep(new Random().Next(50, 100));
                                Console.WriteLine(DateTime.Now + " " + base.Id + " speed:" + tempSpeed + "rpm");
                                SystemHelper.SafeInvoke(hardwareViewer.LabelFanSpeed, () =>
                                {
                                    hardwareViewer.LabelFanSpeed.Text = String.Format("S:{0}", tempSpeed);
                                });

                                _speed = tempSpeed;
                                base.AnalogData = tempSpeed;
                            }
                        });
                        thread.Start();
                    }
                }
                else
                {
                    int tempSpeed = newval;
                    //Simulate speed increase overtime (interval 0.5s)
                    if (newval != -1)
                    {
                        int rpm = newval;
                        int rps = rpm / 60; //revolution per second

                        Thread thread = new Thread(() =>
                        {
                            while ((base.AnalogData > 0) && IsOff && !Program.CancelTokenSource.Token.IsCancellationRequested)
                            {
                                int randRps = new Random().Next(1, rps);
                                tempSpeed -= randRps;
                                base.AnalogData -= randRps;
                                Thread.Sleep(new Random().Next(50, 100));
                                Console.WriteLine(DateTime.Now + " " + base.Id + " speed:" + tempSpeed + "rpm");
                                _speed = tempSpeed;
                                base.AnalogData = tempSpeed;
                                if (tempSpeed <= 0)
                                {
                                    _speed = 0;
                                    base.AnalogData = 0;
                                }
                                SystemHelper.SafeInvoke(hardwareViewer.LabelFanSpeed, () =>
                                {
                                    hardwareViewer.LabelFanSpeed.Text = String.Format("S:{0}", tempSpeed);
                                });
                            }
                        });
                        thread.Start();
                    }
                }
            }
        }

        public override bool Update()
        {
            return base.Update();
        }

        //Deprecated: Currently not in use
        public void BindWithUIComponent(Panel panel)
        {
            this.pPanel = panel;
        }

        public void BindWithUiComponent(UiHardwareViewerGroup hardwareViewer)
        {
            this.hardwareViewer = hardwareViewer;
        }

        private Color GetUiBackColorIndicator(bool isOn) => isOn ? Color.Green : Color.Gray;
    }
}
