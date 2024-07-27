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
            set => _pPanel = value;
        }

        public UiHardwareViewerGroup HardwareViewer
        {
            private get;
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
                    HardwareViewer.ToggleUiFan(IsOn);
                }
            }
        }

        private int _speed = -1;
        public virtual int Speed
        {
            get => _speed;
            set
            {
                if (ComController.GetType() != typeof(HHEmuGPIOController))
                {
                    _speed = value;
                    base.AnalogData = _speed;
                }
                //Simulate the analog value increment/decrement
                else
                {
                    if (base.IsOn)
                    {
                        int tempSpeed = /*0*/_speed;
                        //Simulate speed increase over time (interval random)
                        if (value != -1)
                        {
                            int rpm = value;
                            int rps = rpm / 60; //revolution per second

                            Thread thread = new Thread(() =>
                            {
                                while (!(base.AnalogData >= value) && IsOn && !Program.CancelTokenSource.Token.IsCancellationRequested)
                                {
                                    int randRps = new Random().Next(1, rps);
                                    tempSpeed += randRps;

                                    _speed = tempSpeed;
                                    base.AnalogData = tempSpeed;

                                    if (HardwareViewer != null)
                                    {
                                        HardwareViewer.DisplayFanSpeed = String.Format("{0}", tempSpeed);
                                    }
                                    Console.WriteLine(DateTime.Now + " " + base.Id + " speed:" + tempSpeed + "rpm");
                                    Thread.Sleep(new Random().Next(1, 50));
                                }

                                while (IsOn && !Program.CancelTokenSource.Token.IsCancellationRequested)
                                {
                                    int guardbandLimit = new Random().Next(-5, 5);
                                    tempSpeed = rpm + guardbandLimit;


                                    _speed = tempSpeed;
                                    base.AnalogData = tempSpeed;

                                    if (HardwareViewer != null)
                                    {
                                        HardwareViewer.DisplayFanSpeed = String.Format("{0}", tempSpeed);
                                    }
                                    Console.WriteLine(DateTime.Now + " " + base.Id + " speed:" + tempSpeed + "rpm");
                                    Thread.Sleep(1000);
                                }
                            });
                            thread.Start();
                        }
                    }
                    else
                    {
                        int tempSpeed = value;
                        //Simulate speed decrease over time (interval random)
                        if (value != -1)
                        {
                            int rpm = value;
                            int rps = rpm / 60; //revolution per second

                            Thread thread = new Thread(() =>
                            {
                                while ((base.AnalogData > 0) && IsOff && !Program.CancelTokenSource.Token.IsCancellationRequested)
                                {
                                    int randRps = new Random().Next(1, rps);
                                    tempSpeed -= randRps;

                                    Thread.Sleep(new Random().Next(50, 100));
                                    Console.WriteLine(DateTime.Now + " " + base.Id + " speed:" + tempSpeed + "rpm");
                                    if (tempSpeed <= 0)
                                    {
                                        tempSpeed = 0;
                                    }
                                    _speed = tempSpeed;
                                    base.AnalogData = tempSpeed;

                                    if (HardwareViewer != null)
                                    {
                                        HardwareViewer.DisplayFanSpeed = String.Format("{0}", tempSpeed);
                                    }
                                }
                            });
                            thread.Start();
                        }
                    }
                }
            }
        }

        public SimFan(string id, eBitMask mask, eGroup group, eIoType ioType, int ioPort)
            : base(id, mask, eHardwareType.FAN, group, ioType, ioPort) { }

        public SimFan(string id, eBitMask mask, eGroup group, eIoType ioType, string portName, int baudRate)
            : base(id, mask, eHardwareType.FAN, group, ioType, portName, baudRate) { }

        public override bool Update()
        {
            return base.Update();
        }

        //Deprecated: Currently not in use
        public void BindWithUIComponent(Panel panel)
        {
            this.pPanel = panel;
        }

        private Color GetUiBackColorIndicator(bool isOn) => isOn ? Color.Green : Color.Gray;
    }
}
