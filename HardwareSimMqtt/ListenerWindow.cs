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

        protected Dictionary<uint, HardwareBase> simHardwareMap
        {
            get;
            set;
        }

        private Queue<PacketInfo> queuePacketInfoReceived
        {
            get;
            set;
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

        private STATE iLastSwitchStep
        {
            get;
            set;
        }

        private STATE _iAutoNextStep = 0;
        private STATE iAutoNextStep
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
            public string headerTopic;
            public List<BitInfo> bitInfoList;

            public PacketInfo(string headerTopic, List<BitInfo> bitInfoList)
            {
                this.headerTopic = headerTopic;
                this.bitInfoList = bitInfoList;
            }
        }
        private MonitorTaskThread monitorJobThread
        {
            get;
            set;
        }

        public ListenerWindow()
        {
            InitializeComponent();
            InititalizeListnerWindow();
            InitializePartialListenerWindow();
            FormClosing += (sender, e) => DisconnectBrokerConnection();
            FormClosed += (sender, e) => DisconnectBrokerConnection();
            monitorJobThread = new MonitorTaskThread();
        }

        private void OnMessageReceived(object sender, MqttMsgPublishEventArgs e)
        {
            JsonBitInfoList bitInfoList = JsonConvert.DeserializeObject<JsonBitInfoList>(Encoding.UTF8.GetString(e.Message));
            if (bitInfoList.InfoList == null) { return; }
            queuePacketInfoReceived.Enqueue(new PacketInfo(e.Topic, bitInfoList.InfoList));
        }

        public void ListenerLogInfo(string text, Color color)
        {
            string textTemp = String.Format("Step({0}) = {1}", (int)this.iAutoNextStep, text);
            SystemHelper.AppendRichTextBox(richTextBox1, textTemp, color);
        }

        private void InititalizeListnerWindow()
        {
            //Listener
            isPowerUpFinish = false;
            queuePacketInfoReceived = new Queue<PacketInfo>();
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

        private void InitializeHardwareBitMap()
        {
            simHardwareMap = new Dictionary<uint, HardwareBase>();

            Dictionary<eGroup, UiHardwareViewerGroup> uiHardwareViewerMap = new Dictionary<eGroup, UiHardwareViewerGroup>();
            Dictionary<eGroup, UiHardwareControllerGroup> uiHardwareControllerMap = new Dictionary<eGroup, UiHardwareControllerGroup>();

            string xmlFilePath = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), ConfigurationManager.AppSettings.Get("HardwareConfigFile"));
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(xmlFilePath);
            XmlNodeList settings = xmlDoc.SelectNodes("/configuration/bitmap/setting");

            for (int i = 0; i < settings.Count; i++)
            {
                int bit = Convert.ToInt32(settings[i].Attributes["bit"].Value);
                eBitMask ebitMask = (eBitMask)(1 << bit);

                XmlNodeList details = settings[i].SelectNodes("detail");

                if (details.Count != 0)
                {
                    int ioPort = new int();
                    eIoType ioType = new eIoType();
                    eControllerType econtrollerType = new eControllerType();
                    eHardwareType ehardwaretype = new eHardwareType();
                    eGroup egroup = new eGroup();
                    string id = String.Empty;
                    for (int j = 0; j < details.Count; j++)
                    {
                        if (details[j].Attributes["name"].Value == "IOPort")
                        {
                            ioPort = Convert.ToInt32(details[j].Attributes["value"].Value);
                        }
                        else if (details[j].Attributes["name"].Value == "IOType")
                        {
                            ioType = (eIoType)Convert.ToInt32(details[j].Attributes["value"].Value);
                        }
                        else if (details[j].Attributes["name"].Value == "HardwareType")
                        {
                            ehardwaretype = (eHardwareType)Convert.ToInt32(details[j].Attributes["value"].Value);
                        }
                        else if (details[j].Attributes["name"].Value == "ControllerType")
                        {
                            econtrollerType = (eControllerType)Convert.ToInt32(details[j].Attributes["value"].Value);
                        }
                        else if (details[j].Attributes["name"].Value == "Group")
                        {
                            egroup = (eGroup)Convert.ToInt32(details[j].Attributes["value"].Value);
                            id = Convert.ToString((int)egroup);
                        }
                        else
                        {
                            continue;
                        }
                    }

                    HardwareBase unitSimHardware;
                    if (econtrollerType == eControllerType.GPIO)
                    {
                        //int ioPort = Convert.ToInt32(ConfigurationManager.AppSettings.Get(String.Format("Bit{0}_IOPort", i)));

                        if (ehardwaretype == eHardwareType.LAMP)
                        {
                            unitSimHardware = new SimLamp(id, ebitMask, egroup, ioType, ioPort);
                        }
                        else if (ehardwaretype == eHardwareType.FAN)
                        {
                            unitSimHardware = new SimFan(id, ebitMask, egroup, ioType, ioPort);
                        }
                        else
                        {
                            unitSimHardware = new HardwareBase(id, ebitMask, ehardwaretype, egroup, ioType, ioPort);
                        }
                    }
                    else if (econtrollerType == eControllerType.SerialPort)
                    {
                        string comPort = ConfigurationManager.AppSettings.Get(String.Format("Bit{0}_COMPort", i));
                        int baudRate = Convert.ToInt32(ConfigurationManager.AppSettings.Get(String.Format("Bit{0}_BaudRate", i)));
                        if (ehardwaretype == eHardwareType.LAMP)
                        {
                            unitSimHardware = new SimLamp(id, ebitMask, egroup, ioType, comPort, baudRate);
                        }
                        else if (ehardwaretype == eHardwareType.FAN)
                        {
                            unitSimHardware = new SimFan(id, ebitMask, egroup, ioType, comPort, baudRate);
                        }
                        else
                        {
                            unitSimHardware = new HardwareBase(id, ebitMask, ehardwaretype, egroup, ioType, comPort, baudRate);
                        }
                    }
                    else
                    {
                        continue;
                    }


                    //Hardware viewer
                    if (egroup != eGroup.Non &&
                        (!uiHardwareViewerMap.ContainsKey(egroup) || uiHardwareViewerMap[egroup] == null))
                    {
                        uiHardwareViewerMap[egroup] = new UiHardwareViewerGroup(egroup);
                    }
                    else
                    {
                        //unit hardware viewer
                    }

                    //Hardware Controller
                    if (egroup != eGroup.Non &&
                        (!uiHardwareControllerMap.ContainsKey(egroup) || uiHardwareControllerMap[egroup] == null))
                    {
                        uiHardwareControllerMap[egroup] = new UiHardwareControllerGroup(egroup);
                    }
                    else
                    {
                        //unit hardware controller
                    }

                    if (unitSimHardware != null)
                    {
                        Type hwType = unitSimHardware.GetType();
                        //Bind hardware details to UI controller and viewer
                        if (egroup != eGroup.Non)
                        {
                            if (hwType == typeof(SimLamp))
                            {
                                uiHardwareViewerMap[egroup].DisplayLampId = unitSimHardware.Id;
                                ((SimLamp)unitSimHardware).HardwareViewer = uiHardwareViewerMap[egroup];

                                uiHardwareControllerMap[egroup].CheckBoxLampId = unitSimHardware.Id;
                                uiHardwareControllerMap[egroup].CheckBoxLampMask = (eBitMask)unitSimHardware.BitMask;

                            }
                            else if (hwType == typeof(SimFan))
                            {
                                uiHardwareViewerMap[egroup].DisplayFanId = unitSimHardware.Id;
                                ((SimFan)unitSimHardware).HardwareViewer = uiHardwareViewerMap[egroup];

                                uiHardwareControllerMap[egroup].CheckBoxFanId = unitSimHardware.Id;
                                uiHardwareControllerMap[egroup].CheckBoxFanMask = (eBitMask)unitSimHardware.BitMask;
                            }
                        }
                        simHardwareMap.Add(Convert.ToUInt32(bit), unitSimHardware);
                    }
                }

            }

            //Hardware Viewer
            foreach (KeyValuePair<eGroup, UiHardwareViewerGroup> kvp in uiHardwareViewerMap)
            {
                hardwareViewerFlowLayoutPanel.Controls.Add(kvp.Value);
            }

            //Hardware Controller
            CheckBox checkboxAll = new CheckBox
            {
                Text = "All",
                AutoSize = true
            };

            foreach (KeyValuePair<eGroup, UiHardwareControllerGroup> kvp in uiHardwareControllerMap)
            {
                checkboxAll.CheckStateChanged += new EventHandler(kvp.Value.CheckboxAll_OnCheckStateChanged);
                hardwareControllerFlowLayoutPanel.Controls.Add(kvp.Value);
            }
            hardwareControllerFlowLayoutPanel.Controls.Add(checkboxAll);
            monitorJobThread.HardwareMap = simHardwareMap;
            
            //Force do connection attempt
            foreach (KeyValuePair<uint, HardwareBase> kvp in simHardwareMap)
            {
                if (!kvp.Value.IsConnected)
                {
                    kvp.Value.Connect();
                }
            }
        }

        private void InitializeBitSetDgv()
        {
            bitSetDataTable = new DataTable();

            int nColCount = Enum.GetNames(typeof(eBitMask)).Length;

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
            if (listenerBrokerConnectJob.Client.IsConnected)
            {
                listenerBrokerConnectJob.Client.Disconnect();
            }

            if (controllerBrokerConnectJob.Client.IsConnected)
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
                case STATE.PU_SETUP_CONNECTION_WITH_BROKER:
                    listenerBrokerConnectJob = new SetBrokerConnectJob("broker.emqx.io");
                    bool bEstablished = listenerBrokerConnectJob.Run();
                    if (!bEstablished)
                    {
                        break;
                    }
                    listenerBrokerConnectJob.Client.MqttMsgPublishReceived += OnMessageReceived;
                    listenerBrokerConnectJob.Client.Subscribe(new string[] { TOPIC }, new byte[] { MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE });
                    iAutoNextStep = STATE.PU_INIT_SIM_HARDWARE_INSTANCE;
                    break;

                case STATE.PU_INIT_SIM_HARDWARE_INSTANCE:
                    InitializeHardwareBitMap();
                    iAutoNextStep = STATE.PU_SET_SIM_HARDWARE_INIT_STATE;
                    break;

                case STATE.PU_SET_SIM_HARDWARE_INIT_STATE:
                    foreach (KeyValuePair<uint, HardwareBase> kvp in simHardwareMap)
                    {
                        //kvp.Value.Off();

                        monitorJobThread.QueuedJob.Enqueue(new SetHardwareStateJob(kvp.Value, false), 1);
                    }
                    iAutoNextStep = STATE.PU_COMPLETE;
                    break;

                case STATE.PU_COMPLETE:

                    isPowerUpFinish = true;
                    if (listenerBrokerConnectJob.Client == null)
                    {
                        isPowerUpFinish = false;
                    }

                    if (simHardwareMap.Count == 0)
                    {
                        isPowerUpFinish = false;
                    }

                    if (isPowerUpFinish)
                    {
                        iAutoNextStep = STATE.AUTO_WAIT_NEW_MESSAGE_BROADCAST;
                    }
                    else
                    {
                        iAutoNextStep = STATE.PU_SETUP_CONNECTION_WITH_BROKER;
                    }
                    break;

            }
            return isPowerUpFinish;
        }

        private bool AutoOperation(object sender, EventArgs e)
        {
            switch (iAutoNextStep)
            {
                case STATE.AUTO_WAIT_NEW_MESSAGE_BROADCAST:
                    if (queuePacketInfoReceived.Count == 0)
                    {
                        break;
                    }
                    string log = String.Format("New packet received count: {0}", queuePacketInfoReceived.Count);
                    ListenerLogInfo(log, Color.Blue);
                    iAutoNextStep = STATE.AUTO_PRE_TRANSLATE_RECEIVED_MESSAGE;
                    break;

                case STATE.AUTO_PRE_TRANSLATE_RECEIVED_MESSAGE:
                    TranslatePacketReceived();
                    iAutoNextStep = monitorJobThread.QueuedJob.Count > 0 ? STATE.AUTO_UPDATE_HARDWARE_STATE : STATE.AUTO_WAIT_NEW_MESSAGE_BROADCAST;
                    //Debug.WriteLine(monitorJobThread.QueuedJob.ToString());
                    break;

                case STATE.AUTO_UPDATE_HARDWARE_STATE:
                    //Task query thread will update the hardware state
                    iAutoNextStep = STATE.AUTO_WAIT_NEW_MESSAGE_BROADCAST;
                    break;

                case STATE.PE_SYSTEM_SHUTDOWN:
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
                if (packetReceived.headerTopic == TOPIC)
                {
                    for (int i = 0; i < packetReceived.bitInfoList.Count; i++)
                    {
                        foreach (KeyValuePair<uint, HardwareBase> kvp in simHardwareMap)
                        {
                            if (kvp.Value.Id == packetReceived.bitInfoList[i].Id)
                            {
                                monitorJobThread.QueuedJob.Enqueue(new SetHardwareStateJob(kvp.Value, packetReceived.bitInfoList[i].BitState, 1000), 1);
                                string log = String.Format("TranslatePacketReceived. HWID: {0}, mask bit: 0x{1:D4}, received state bit: 0x{2:D4}", packetReceived.bitInfoList[i].Id, kvp.Value.BitMask.ToString("X"), packetReceived.bitInfoList[i].BitState.ToString("X"));
                                ListenerLogInfo(log, Color.Blue);
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
