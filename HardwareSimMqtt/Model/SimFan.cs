using System.Windows.Forms;
using System.Drawing;
using HardwareSimMqtt.Model.BitMap;
using HardwareSimMqtt.UIComponent;
using HardwareSimMqtt.HardwareHub;
using System.Threading;
using System;
using System.Threading.Tasks;
using HardwareSimMqtt.Utils;

namespace HardwareSimMqtt.Model
{
    internal class SimFan : DeviceBase
    {
        private Panel _pPanel = null;
        private Panel pPanel
        {
            get => _pPanel;
            set => _pPanel = value;
        }

        public UiDeviceViewerGroup DeviceViewer
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

                if (DeviceViewer != null)
                {
                    DeviceViewer.UpdateUi(IsOn, DeviceViewer.UpdateUiFan);
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

                                    if (DeviceViewer != null)
                                    {
                                        DeviceViewer.DisplayFanSpeed = String.Format("{0}", tempSpeed);
                                    }
                                    DeviceViewer.UpdateUiFanRampingSpeed();
                                    Console.WriteLine(DateTime.Now + " " + base.Id + " speed:" + tempSpeed + "rpm");
                                    Thread.Sleep(new Random().Next(1, 50));
                                }
                                DeviceViewer.UpdateUi(IsOn, DeviceViewer.UpdateUiFan);

                                while (IsOn && !Program.CancelTokenSource.Token.IsCancellationRequested)
                                {
                                    int guardbandLimit = new Random().Next(-5, 5);
                                    tempSpeed = rpm + guardbandLimit;


                                    _speed = tempSpeed;
                                    base.AnalogData = tempSpeed;

                                    if (DeviceViewer != null)
                                    {
                                        DeviceViewer.DisplayFanSpeed = String.Format("{0}", tempSpeed);
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

                                    if (tempSpeed <= 0)
                                    {
                                        tempSpeed = 0;
                                    }
                                    _speed = tempSpeed;
                                    base.AnalogData = tempSpeed;

                                    if (DeviceViewer != null)
                                    {
                                        DeviceViewer.DisplayFanSpeed = String.Format("{0}", tempSpeed);
                                    }
                                    DeviceViewer.UpdateUiFanRampingSpeed();
                                    Console.WriteLine(DateTime.Now + " " + base.Id + " speed:" + tempSpeed + "rpm");
                                    Thread.Sleep(new Random().Next(50, 100));
                                }
                                DeviceViewer.UpdateUi(IsOn, DeviceViewer.UpdateUiFan);
                            });
                            thread.Start();
                        }
                    }
                }
            }
        }

        public SimFan(string id, DeviceBitMask deviceBitMask, DeviceGroup deviceGroup, IoType ioType, int ioPort)
            : base(id, deviceBitMask, DeviceType.FAN, deviceGroup, ioType, ioPort) { }

        public SimFan(string id, DeviceBitMask deviceBitMask, DeviceGroup deviceGroup, IoType ioType, string portName, int baudRate)
            : base(id, deviceBitMask, DeviceType.FAN, deviceGroup, ioType, portName, baudRate) { }

        public SimFan(DevcieConfig deviceConfig)
            : base(deviceConfig) { }

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
