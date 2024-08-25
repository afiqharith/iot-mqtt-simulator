using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using uPLibrary.Networking.M2Mqtt.Messages;
using Newtonsoft.Json;
using WinformTimer = System.Windows.Forms.Timer;
using System.Threading;
using HardwareSimMqtt.Interface;
using HardwareSimMqtt.Model;
using HardwareSimMqtt.Model.DataContainer;
using HardwareSimMqtt.Model.QueryJob;
using HardwareSimMqtt.Model.BitMap;
using System.Configuration;
using HardwareSimMqtt.HardwareHub;
using HardwareSimMqtt.UIComponent;
using HardwareSimMqtt.EventArgsModel;
using System.Xml;
using System.IO;
using System.Diagnostics;
using HardwareSimMqtt.Utils;

namespace HardwareSimMqtt
{

    //Listener
    public partial class ListenerWindow : Form
    {
        public const String TOPIC = "MqttBitInfoBroker";

        private WinformTimer systemTimer
        {
            get;
            set;
        }

        private event EventHandler<AutoStepChangeEventArgs> autoStepChanged;

        private SetBrokerConnectJob listenerBrokerConnectJob
        {
            get;
            set;
        }

        private Dictionary<uint, DeviceBase> _simulatedDeviceDict = null;
        protected Dictionary<uint, DeviceBase> simulatedDeviceDict
        {
            get
            {
                if(_simulatedDeviceDict == null)
                {
                    _simulatedDeviceDict = new Dictionary<uint, DeviceBase>(); 
                }
                return _simulatedDeviceDict;
            }
            set => _simulatedDeviceDict = value;
        }


        private Queue<PacketInfo> _queuePacketInfoReceived = null;
        private Queue<PacketInfo> queuePacketInfoReceived
        {
            get
            {
                if(_queuePacketInfoReceived == null)
                {
                    _queuePacketInfoReceived = new Queue<PacketInfo>();
                }
                return _queuePacketInfoReceived;
            }
            set => queuePacketInfoReceived = value;
        }

        private DataTable bitSetDataTable
        {
            get;
            set;
        }

        private uint realTimeBitSet
        {
            get;
            set;
        }

        private bool isPowerUpFinish
        {
            get;
            set;
        }

        private AutoState iLastSwitchStep
        {
            get;
            set;
        }

        private AutoState _iAutoNextStep = 0;
        private AutoState iAutoNextStep
        {
            get => _iAutoNextStep;
            set
            {
                if (value != iLastSwitchStep)
                {
                    autoStepChanged.Invoke(this, new AutoStepChangeEventArgs(_iAutoNextStep, value));
                    _iAutoNextStep = value;
                }
            }
        }

        //Use when de-packet the data receive from broker
        private struct PacketInfo
        {
            public string HeaderTopic;
            public List<BitInfo> BitInfoList;

            public PacketInfo(string headerTopic, List<BitInfo> bitInfoList)
            {
                HeaderTopic = headerTopic;
                BitInfoList = bitInfoList;
            }
        }

        private MonitorTaskThread _monitorTaskThread = null;
        private MonitorTaskThread monitorTaskJobThread
        {
            get
            {
                if(_monitorTaskThread == null)
                {
                    _monitorTaskThread = new MonitorTaskThread();
                }
                return _monitorTaskThread;
            }
            set => _monitorTaskThread = value;
        }

        public ListenerWindow()
        {
            InitializeComponent();
            InititalizeListnerWindow();
            InitializePartialListenerWindow();
            FormClosing += (sender, e) => DisconnectBrokerConnection();
            FormClosed += (sender, e) => DisconnectBrokerConnection();
        }

        private void OnMessageReceived(object sender, MqttMsgPublishEventArgs e)
        {
            JsonBitInfoList bitInfoList = JsonConvert.DeserializeObject<JsonBitInfoList>(Encoding.UTF8.GetString(e.Message));
            if (bitInfoList.InfoList == null) { return; }
            queuePacketInfoReceived.Enqueue(new PacketInfo(e.Topic, bitInfoList.InfoList));
        }

        public void LInfo(string text, Color color)
        {
            string textTemp = String.Format("Step({0}) = {1}", (int)iAutoNextStep, text);
            SystemHelper.AppendRichTextBox(richTextBox1, textTemp, color);
        }

        private void CInfo(string text, Color color)
        {
            SystemHelper.AppendRichTextBox(richTextBox2, text, color);
        }

        private void InititalizeListnerWindow()
        {
            //Listener
            isPowerUpFinish = false;
            InitializeBitSetDgv();
            InitializeSystemTimer();

            autoStepChanged += (sender, e) =>
            {
                string textTemp = String.Format("Step({0}) = change ({1}) {2}", (int)e.OldStep, (int)e.NewStep, e.NewStep);
                SystemHelper.AppendRichTextBox(richTextBox1, textTemp, Color.Gray);
                iLastSwitchStep = e.NewStep;
            };
        }

        private bool InitializeSystemTimer()
        {
            systemTimer = new WinformTimer();
            systemTimer.Enabled = true;
            systemTimer.Interval = 1;
            systemTimer.Tick += MainOperation;
            systemTimer.Start();
            return true;
        }

        private void InitializeDeviceBitMap()
        {
            Dictionary<DeviceGroup, UiDeviceViewerGroup> uiDeviceViewerDict = new Dictionary<DeviceGroup, UiDeviceViewerGroup>();
            Dictionary<DeviceGroup, UiDeviceControllerGroup> uiDeviceControllerDict = new Dictionary<DeviceGroup, UiDeviceControllerGroup>();

            string xmlFilePath = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), ConfigurationManager.AppSettings.Get("HardwareConfigFile"));
            ConfigSetting configSetting = new ConfigSetting(xmlFilePath);
            configSetting.Load();

            for (int i = 0; i < configSetting.ConfigList.Count; i++)
            {
                DevcieConfig deviceConfig = configSetting.ConfigList[i];
                if (!Enum.IsDefined(typeof(DeviceBitMask), deviceConfig.BitMask))
                {
                    continue;
                }
                DeviceBase unitDeviceSim;

                if (deviceConfig.ControllerType == ControllerType.SerialPort)
                {
                    deviceConfig.ComPort = ConfigurationManager.AppSettings.Get("ComPort");
                    deviceConfig.Baudrate = Convert.ToInt32(ConfigurationManager.AppSettings.Get("BaudRate"));
                }
                else if (deviceConfig.ControllerType == ControllerType.Invalid)
                {
                    continue;
                }

                switch (deviceConfig.DeviceType)
                {
                    case DeviceType.LAMP:
                        unitDeviceSim = new SimLamp(deviceConfig);
                        break;

                    case DeviceType.FAN:
                        unitDeviceSim = new SimFan(deviceConfig);
                        break;

                    default:
                        unitDeviceSim = new DeviceBase(deviceConfig);
                        break;
                }

                //Device viewer
                if (deviceConfig.Group != DeviceGroup.Invalid &&
                    (!uiDeviceViewerDict.ContainsKey(deviceConfig.Group) || uiDeviceViewerDict[deviceConfig.Group] == null))
                {
                    uiDeviceViewerDict[deviceConfig.Group] = new UiDeviceViewerGroup(deviceConfig.Group);
                }
                else
                {
                    //unit device viewer
                }

                //Device Controller
                if (deviceConfig.Group != DeviceGroup.Invalid &&
                    (!uiDeviceControllerDict.ContainsKey(deviceConfig.Group) || uiDeviceControllerDict[deviceConfig.Group] == null))
                {
                    uiDeviceControllerDict[deviceConfig.Group] = new UiDeviceControllerGroup(deviceConfig.Group);
                }
                else
                {
                    //unit device controller
                }

                if (unitDeviceSim != null)
                {
                    Type deviceObjectType = unitDeviceSim.GetType();
                    //Bind device details to UI controller and viewer
                    if (deviceConfig.Group != DeviceGroup.Invalid)
                    {
                        if (deviceObjectType == typeof(SimLamp))
                        {
                            uiDeviceViewerDict[deviceConfig.Group].DisplayLampId = unitDeviceSim.Id;
                            ((SimLamp)unitDeviceSim).DeviceViewer = uiDeviceViewerDict[deviceConfig.Group];

                            uiDeviceControllerDict[deviceConfig.Group].CheckBoxLampId = unitDeviceSim.Id;
                            uiDeviceControllerDict[deviceConfig.Group].CheckBoxLampMask = (DeviceBitMask)unitDeviceSim.BitMask;

                        }
                        else if (deviceObjectType == typeof(SimFan))
                        {
                            uiDeviceViewerDict[deviceConfig.Group].DisplayFanId = unitDeviceSim.Id;
                            ((SimFan)unitDeviceSim).DeviceViewer = uiDeviceViewerDict[deviceConfig.Group];

                            uiDeviceControllerDict[deviceConfig.Group].CheckBoxFanId = unitDeviceSim.Id;
                            uiDeviceControllerDict[deviceConfig.Group].CheckBoxFanMask = (DeviceBitMask)unitDeviceSim.BitMask;
                        }
                    }

                    if (!simulatedDeviceDict.ContainsKey(Convert.ToUInt32(deviceConfig.Bit)))
                    {
                        simulatedDeviceDict.Add(Convert.ToUInt32(deviceConfig.Bit), unitDeviceSim);
                    }
                    else
                    {
                        simulatedDeviceDict[Convert.ToUInt32(deviceConfig.Bit)] = unitDeviceSim;
                    }
                }
            }

            //Device Viewer
            foreach (KeyValuePair<DeviceGroup, UiDeviceViewerGroup> kvp in uiDeviceViewerDict)
            {
                deviceViewerFlowLayoutPanel.Controls.Add(kvp.Value);
            }

            //Device Controller
            CheckBox checkboxAll = new CheckBox
            {
                Text = "All",
                AutoSize = true
            };

            foreach (KeyValuePair<DeviceGroup, UiDeviceControllerGroup> kvp in uiDeviceControllerDict)
            {
                checkboxAll.CheckStateChanged += new EventHandler(kvp.Value.CheckboxAll_OnCheckStateChanged);
                deviceControllerFlowLayoutPanel.Controls.Add(kvp.Value);
            }
            deviceControllerFlowLayoutPanel.Controls.Add(checkboxAll);
            monitorTaskJobThread.DeviceDict = simulatedDeviceDict;

            //Force do connection attempt
            foreach (KeyValuePair<uint, DeviceBase> kvp in simulatedDeviceDict)
            {
                //if (!kvp.Value.IsConnected)
                //{
                //    kvp.Value.Connect();
                //}

                IJob connectTaskJob = new ConnectDeviceJob(kvp.Value);
                monitorTaskJobThread.QueuedJob.Enqueue(connectTaskJob, 1);
                ((ConnectDeviceJob)connectTaskJob).WaitFinish();

            }
        }

        private void InitializeBitSetDgv()
        {
            bitSetDataTable = new DataTable();

            int nColCount = Enum.GetNames(typeof(DeviceBitMask)).Length;

            for (int nCol = nColCount - 1; nCol >= 0; nCol--)
            {
                bitSetDataTable.Columns.Add(new DataColumn(String.Format("Bit{0}", nCol), typeof(uint)));
            }

            DataRow dr = bitSetDataTable.NewRow();
            for (int nCol = nColCount - 1; nCol >= 0; nCol--)
            {
                dr[String.Format("Bit{0}", nCol)] = 0;
            }
            bitSetDataTable.Rows.Add(dr);
            DataGridViewBitSet.DataSource = bitSetDataTable;
        }

        private void DisconnectBrokerConnection()
        {
            if (listenerBrokerConnectJob.Client != null || listenerBrokerConnectJob.Client.IsConnected)
            {
                listenerBrokerConnectJob.Client.Disconnect();
            }

            if (controllerBrokerConnectJob.Client != null || controllerBrokerConnectJob.Client.IsConnected)
            {
                controllerBrokerConnectJob.Client.Disconnect();
            }
            Program.CancelTokenSource.Cancel();
        }

        private void MainOperation(object sender, EventArgs e)
        {
            if (!isPowerUpFinish)
                PowerUpOperation(sender, e);
            else
                AutoOperation(sender, e);
        }

        private bool PowerUpOperation(object sender, EventArgs e)
        {
            switch (iAutoNextStep)
            {
                case AutoState.PU_SETUP_CONNECTION_WITH_BROKER:
                    listenerBrokerConnectJob = new SetBrokerConnectJob("broker.emqx.io");
                    bool bEstablished = listenerBrokerConnectJob.Run();
                    if (!bEstablished)
                    {
                        break;
                    }
                    listenerBrokerConnectJob.Client.MqttMsgPublishReceived += OnMessageReceived;
                    listenerBrokerConnectJob.Client.Subscribe(new string[] { TOPIC }, new byte[] { MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE });
                    iAutoNextStep = AutoState.PU_INIT_SIM_DEVICE_INSTANCE;
                    break;

                case AutoState.PU_INIT_SIM_DEVICE_INSTANCE:
                    InitializeDeviceBitMap();
                    iAutoNextStep = AutoState.PU_SET_SIM_DEVICE_INIT_STATE;
                    break;

                case AutoState.PU_SET_SIM_DEVICE_INIT_STATE:
                    foreach (KeyValuePair<uint, DeviceBase> kvp in simulatedDeviceDict)
                    {
                        //kvp.Value.Off();
                        monitorTaskJobThread.QueuedJob.Enqueue(new SetDeviceStateJob(kvp.Value, false), 1);
                    }
                    iAutoNextStep = AutoState.PU_COMPLETE;
                    break;

                case AutoState.PU_COMPLETE:
                    isPowerUpFinish = true;
                    if (listenerBrokerConnectJob.Client == null)
                    {
                        isPowerUpFinish = false;
                    }

                    if (isPowerUpFinish)
                    {
                        iAutoNextStep = AutoState.AUTO_WAIT_NEW_MESSAGE_BROADCAST;
                    }
                    else
                    {
                        iAutoNextStep = AutoState.PU_SETUP_CONNECTION_WITH_BROKER;
                    }
                    break;

            }
            return isPowerUpFinish;
        }

        private bool AutoOperation(object sender, EventArgs e)
        {
            switch (iAutoNextStep)
            {
                case AutoState.AUTO_WAIT_NEW_MESSAGE_BROADCAST:
                    if (queuePacketInfoReceived.Count == 0)
                    {
                        break;
                    }
                    string log = String.Format("New packet received count: {0}", queuePacketInfoReceived.Count);
                    SystemHelper.PrintMessage(LInfo, log, Color.Blue);
                    iAutoNextStep = AutoState.AUTO_PRE_TRANSLATE_RECEIVED_MESSAGE;
                    break;

                case AutoState.AUTO_PRE_TRANSLATE_RECEIVED_MESSAGE:
                    int translatedPacketCount = 0;
                    if (queuePacketInfoReceived.Count != 0)
                    {
                        translatedPacketCount = TranslatePacketReceived();
                    }
                    iAutoNextStep = translatedPacketCount > 0 ? AutoState.AUTO_UPDATE_DEVICE_STATE : AutoState.AUTO_WAIT_NEW_MESSAGE_BROADCAST;
                    break;

                case AutoState.AUTO_UPDATE_DEVICE_STATE:
                    //Task query thread will update the device state
                    iAutoNextStep = AutoState.AUTO_WAIT_NEW_MESSAGE_BROADCAST;
                    break;

                case AutoState.PE_SYSTEM_SHUTDOWN:
                default:
                    break;

            }
            return true;
        }

        private int TranslatePacketReceived()
        {
            int translatedPacketCount = 0;
            while (queuePacketInfoReceived.Count > 0)
            {
                PacketInfo packetReceived = queuePacketInfoReceived.Dequeue();
                if (packetReceived.HeaderTopic == TOPIC)
                {
                    for (int i = 0; i < packetReceived.BitInfoList.Count; i++)
                    {
                        foreach (KeyValuePair<uint, DeviceBase> kvp in simulatedDeviceDict)
                        {
                            if (kvp.Value.Id == packetReceived.BitInfoList[i].Id)
                            {
                                monitorTaskJobThread.QueuedJob.Enqueue(new SetDeviceStateJob(kvp.Value, packetReceived.BitInfoList[i].BitState, 1000), 1);
                                string log = String.Format("TranslatePacketReceived. HWID: {0}, mask bit: 0x{1:D4}, received state bit: 0x{2:D4}", packetReceived.BitInfoList[i].Id, kvp.Value.BitMask.ToString("X"), packetReceived.BitInfoList[i].BitState.ToString("X"));
                                SystemHelper.PrintMessage(LInfo, log, Color.Blue);
                                translatedPacketCount++;
                            }
                        }
                    }
                }
            }
            return translatedPacketCount;
        }

        public void UpdateBitSetDgvData(uint bitMask, uint currentBitState)
        {
            for (int nCol = bitSetDataTable.Columns.Count - 1; nCol >= 0; nCol--)
            {
                //Update overall system bitSet realtime
                if ((bitMask & currentBitState) != 0 && //Verify if current bit is ON
                    ((1 << nCol) & bitMask) != 0)       //Verify if bit index is correct
                {
                    realTimeBitSet |= bitMask;

                }
                else if ((bitMask & currentBitState) == 0 && //Verify if current bit is OFF
                    ((1 << nCol) & bitMask) != 0)           //Verify if bit index is correct
                {
                    realTimeBitSet &= ~bitMask;
                }

                //Update datatable with realtime bitset value
                int iResult = ((realTimeBitSet & (1 << nCol)) != 0) ? 1 : 0;
                bitSetDataTable.Rows[0][String.Format("Bit{0}", nCol)] = iResult;
            }
            DataGridViewBitSet.DataSource = bitSetDataTable;
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

            // Get the Graphics object
            Graphics g = e.Graphics;

            int x = 0; // X coordinate of the top-left corner
            int y = 0; // Y coordinate of the top-left corner
            int diameter = 15; // Diameter of the circle

            // Create a brush to fill the circle
            using (Brush brush = new SolidBrush(Color.LightBlue))
            {
                // Fill the circle
                g.FillEllipse(brush, x, y, diameter, diameter);
            }

            // Create a pen to draw the circle's outline
            using (Pen pen = new Pen(Color.Blue, 2))
            {
                // Draw the circle's outline
                g.DrawEllipse(pen, x, y, diameter, diameter);
            }
            //g.DrawEllipse(new Pen(Color.Black, 2), 0, 0, 15, 15);
        }
    }
}
