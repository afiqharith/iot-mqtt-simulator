//#define USETHREAD
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

namespace ModelInterface
{

    //Listener
    public partial class ListenerWindow : Form
    {
        public const String TOPIC = "IotWinformSim";

        private class AutoStepChangeEventArgs : EventArgs
        {
            public STATE OldStep
            {
                get;
                set;
            }

            public STATE NewStep
            {
                get;
                set;
            }

            public AutoStepChangeEventArgs(STATE oldStep, STATE newStep)
            {
                this.OldStep = oldStep;
                this.NewStep = newStep;
            }
        }

        private static event EventHandler<AutoStepChangeEventArgs> autoStepChanged;
        private SetBrokerConnectJob listenerBrokerConnectJob
        {
            get;
            set;
        }

        private WinformTimer systemTimer
        {
            get;
            set;
        }

        private Queue<PacketInfo> qPacketInfoReceived
        {
            get;
            set;
        }

        private Queue<IJob> qSetHardwareStateJob
        {
            get;
            set;
        }

        private Queue<IJob> qReadHardwareStateJob
        {
            get;
            set;
        }

        protected Dictionary<uint, HardwareBase> simHardwareMap
        {
            get;
            set;
        }

        private uint realTimeBitSet
        {
            get;
            set;
        }

        private DataTable bitSetDataTable
        {
            get; set;
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
                if (iLastSwitchStep != value)
                {
                    autoStepChanged.Invoke(this, new AutoStepChangeEventArgs(_iAutoNextStep, value));
                    _iAutoNextStep = value;
                }
            }
        }

        //Use when de-packet the data receive from broker
        private struct PacketInfo
        {
            public string topic;
            public List<BitInfo> bitInfoList;
        }

        private enum STATE
        {
            PU_ESTABLISH_CONNECTION_WITH_BROKER,
            PU_DELEGATE_MESSAGE_BROADCASTED_EVT,
            PU_SUBSCRIBE_TOPIC,
            PU_INIT_SIM_HARDWARE_INSTANCE,
            PU_SET_SIM_HARDWARE_INIT_STATE,
            PU_COMPLETE,

            AUTO_WAIT_NEW_MESSAGE_BROADCAST,
            AUTO_PRE_TRANSLATE_RECEIVED_MESSAGE,
            AUTO_UPDATE_HARDWARE_STATE,

            PE_SYSTEM_SHUTDOWN,
        }

#if USETHREAD
        private Thread SetHardwareStateJobChangeThread 
        { 
            get; 
            set; 
        }

        private Thread ReadHardwareStateJobChangeThread 
        { 
            get; 
            set; 
        }
#endif
        public ListenerWindow()
        {
            InitializeComponent();

            //Listener
            isPowerUpFinish = false;
            qPacketInfoReceived = new Queue<PacketInfo>();
            qSetHardwareStateJob = new Queue<IJob>();
            qReadHardwareStateJob = new Queue<IJob>();
            InitializeBitSetDgv();
            InitializeSystemTimer();
            autoStepChanged += new EventHandler<AutoStepChangeEventArgs>(OnAutoStepChanged);

#if USETHREAD
            SetHardwareStateJobChangeThread = new Thread(new ThreadStart(MonitorSetHardwareStateJobChangeThread));

            if (!SetHardwareStateJobChangeThread.IsAlive)
            {
                SetHardwareStateJobChangeThread.Start();
            }

            ReadHardwareStateJobChangeThread = new Thread(new ThreadStart(MonitorReadHardwareStateJobChangeThread));
            if (!ReadHardwareStateJobChangeThread.IsAlive)
            {
                ReadHardwareStateJobChangeThread.Start();
            }
#endif
            //Controller
            qMsgContentToDisplayOnUI = new Queue<Dictionary<ushort, BitInfo>>();
            InitializeCheckBoxMapping();

            controllerBrokerConnectJob = new SetBrokerConnectJob("broker.emqx.io");
            bool bEstablished = controllerBrokerConnectJob.Run();
            if (bEstablished)
            {
                controllerBrokerConnectJob.Client.MqttMsgPublished += OnMessagePublished;
            }
        }

        private void OnFormClosing(object sender, FormClosingEventArgs e)
        {
            if (listenerBrokerConnectJob.Client.IsConnected)
            {
                listenerBrokerConnectJob.Client.Disconnect();
            }

            if (controllerBrokerConnectJob.Client.IsConnected)
            {
                controllerBrokerConnectJob.Client.Disconnect();
            }
        }

        private void OnFormClosed(object sender, FormClosedEventArgs e)
        {
            if (listenerBrokerConnectJob.Client.IsConnected)
            {
                listenerBrokerConnectJob.Client.Disconnect();
            }

            if (controllerBrokerConnectJob.Client.IsConnected)
            {
                controllerBrokerConnectJob.Client.Disconnect();
            }
        }

        private void OnSystemTimerTick(object sender, EventArgs e)
        {
            if (!isPowerUpFinish)
                PowerUpOperation();
            else
                AutoOperation();
        }

        private void OnAutoStepChanged(object sender, AutoStepChangeEventArgs e)
        {
            ListenerLogInfo(String.Format(
                "Auto step change({0}) = ({1}) {2}", (int)e.OldStep, (int)e.NewStep, e.NewStep),
                Color.Gray);
            iLastSwitchStep = e.NewStep;
        }

        private void OnMessageReceived(object sender, MqttMsgPublishEventArgs e)
        {
            JsonBitInfoList bitInfoList = JsonConvert.DeserializeObject<JsonBitInfoList>(Encoding.UTF8.GetString(e.Message));
            if (bitInfoList.InfoList == null) { return; }

            PacketInfo packetInfoReceived;
            packetInfoReceived.topic = e.Topic;
            packetInfoReceived.bitInfoList = bitInfoList.InfoList;
            qPacketInfoReceived.Enqueue(packetInfoReceived);
        }

        private void ListenerLogInfo(string text, Color color)
        {
            SystemHelper.AppendRichTextBox(richTextBox1, text, color);
        }

        private bool InitializeSystemTimer()
        {
            systemTimer = new WinformTimer();
            systemTimer.Enabled = true;
            systemTimer.Interval = 1;
            systemTimer.Tick += new EventHandler(OnSystemTimerTick);
            systemTimer.Start();

            return true;
        }

        private void InitializeHardwareMap()
        {
            simHardwareMap = new Dictionary<uint, HardwareBase>();
            List<Panel> panelList = new List<Panel>()
            {
                panelFan1,
                panelFan2,
                panelFan3,
                panelFan4,

                panelLamp1,
                panelLamp2,
                panelLamp3,
                panelLamp4,
            };

            List<int> IoPort = new List<int>()
            {
                4,
                17,
                18,
                21,
                22,
                23,
                24,
                25
            };

            for (int i = 0; i < panelList.Count; i++)
            {
                int bitMask = 1 << i;
                if (i < 4)
                {
                    simHardwareMap.Add((uint)i,
                        new SimFan(
                            panelList[i],
                            eLOC.Loc1 + i,
                            Convert.ToString(i + 1),
                            (eBitMask)bitMask, IoPort[i]));
                }
                else
                {
                    simHardwareMap.Add((uint)i,
                        new SimLamp(
                            panelList[i],
                            eLOC.Loc1 + i - 4,
                            Convert.ToString(i - 3),
                            (eBitMask)bitMask, IoPort[i]));
                }
            }

            //Do connection attempt
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

        private bool PowerUpOperation()
        {
            switch (iAutoNextStep)
            {
                case STATE.PU_ESTABLISH_CONNECTION_WITH_BROKER:
                    listenerBrokerConnectJob = new SetBrokerConnectJob("broker.emqx.io");
                    bool bEstablished = listenerBrokerConnectJob.Run();

                    if (!bEstablished)
                    {
                        break;
                    }
                    iAutoNextStep = STATE.PU_DELEGATE_MESSAGE_BROADCASTED_EVT;
                    break;

                case STATE.PU_DELEGATE_MESSAGE_BROADCASTED_EVT:
                    listenerBrokerConnectJob.Client.MqttMsgPublishReceived += OnMessageReceived;
                    iAutoNextStep = STATE.PU_SUBSCRIBE_TOPIC;
                    break;

                case STATE.PU_SUBSCRIBE_TOPIC:
                    listenerBrokerConnectJob.Client.Subscribe(new string[] { TOPIC }, new byte[] { MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE });
                    iAutoNextStep = STATE.PU_INIT_SIM_HARDWARE_INSTANCE;
                    break;

                case STATE.PU_INIT_SIM_HARDWARE_INSTANCE:
                    InitializeHardwareMap();
                    iAutoNextStep = STATE.PU_SET_SIM_HARDWARE_INIT_STATE;
                    break;

                case STATE.PU_SET_SIM_HARDWARE_INIT_STATE:
                    foreach (KeyValuePair<uint, HardwareBase> kvp in simHardwareMap)
                    {
                        kvp.Value.Off();
                    }
                    iAutoNextStep = STATE.PU_COMPLETE;
                    break;

                case STATE.PU_COMPLETE:
                    isPowerUpFinish = true;
                    iAutoNextStep = STATE.AUTO_WAIT_NEW_MESSAGE_BROADCAST;
                    break;

            }
            return isPowerUpFinish;
        }

        private bool AutoOperation()
        {
            switch (iAutoNextStep)
            {
                case STATE.AUTO_WAIT_NEW_MESSAGE_BROADCAST:
                    if (qPacketInfoReceived.Count == 0)
                    {
                        break;
                    }
                    iAutoNextStep = STATE.AUTO_PRE_TRANSLATE_RECEIVED_MESSAGE;
                    break;

                case STATE.AUTO_PRE_TRANSLATE_RECEIVED_MESSAGE:
                    TranslatePacketReceived();
                    iAutoNextStep = qSetHardwareStateJob.Count > 0 ? STATE.AUTO_UPDATE_HARDWARE_STATE : STATE.AUTO_WAIT_NEW_MESSAGE_BROADCAST;
                    break;

                case STATE.AUTO_UPDATE_HARDWARE_STATE:
#if !USETHREAD
                    MonitorSetHardwareStateJobChangeThread();
                    MonitorReadHardwareStateJobChangeThread();
#endif
                    iAutoNextStep = STATE.AUTO_WAIT_NEW_MESSAGE_BROADCAST;
                    break;

                case STATE.PE_SYSTEM_SHUTDOWN:
                default:
                    break;

            }
            return true;
        }

        private bool TranslatePacketReceived()
        {
            while (qPacketInfoReceived.Count > 0)
            {
                PacketInfo packetReceived = qPacketInfoReceived.Dequeue();
                if (packetReceived.topic == TOPIC)
                {
                    for (int i = 0; i < packetReceived.bitInfoList.Count; i++)
                    {
                        foreach (KeyValuePair<uint, HardwareBase> kvp in simHardwareMap)
                        {
                            if (kvp.Value.Id == packetReceived.bitInfoList[i].Id)
                            {
                                qSetHardwareStateJob.Enqueue(
                                    new SetHardwareStateJob(
                                        kvp.Value,
                                        packetReceived.bitInfoList[i].BitState));

                                ListenerLogInfo(String.Format(
                                    "HW new state received. HWID: {0}, mask bit: 0x{1:D4}, received state bit: 0x{2:D4}",
                                    packetReceived.bitInfoList[i].Id,
                                    kvp.Value.BitMask.ToString("X"),
                                     packetReceived.bitInfoList[i].BitState.ToString("X")),
                                    Color.Blue);
                            }
                        }
                    }
                }
            }

            return true;
        }

        private void MonitorSetHardwareStateJobChangeThread()
        {
#if USETHREAD
            while (true)
            {
#endif
            while (qSetHardwareStateJob.Count > 0)
            {
                SetHardwareStateJob hardwareStateJob = (SetHardwareStateJob)qSetHardwareStateJob.Dequeue();

                foreach (KeyValuePair<uint, HardwareBase> kvp in simHardwareMap)
                {
                    if (hardwareStateJob != null &&
                        kvp.Value.Id == hardwareStateJob.Hardware.Id &&
                        kvp.Value.BitState != hardwareStateJob.NewBitState)
                    {
                        Color color = Color.Orange;

                        ListenerLogInfo(String.Format(
                                "Set new HW state changed. HWID: {0}, mask bit: 0x{1:D4}, state bit change from 0x{2:D4} to 0x{3:D4}",
                                hardwareStateJob.Hardware.Id,
                                kvp.Value.BitMask.ToString("X"),
                                kvp.Value.BitState.ToString("X"),
                                hardwareStateJob.NewBitState.ToString("X")),
                                color);
                        hardwareStateJob.Run();

                        qReadHardwareStateJob.Enqueue(new ReadHardwareStateJob(kvp.Value));

                        UpdateBitSetDgvData(hardwareStateJob.Hardware.BitMask, hardwareStateJob.Hardware.BitState);
                    }
                }
            }

#if USETHREAD
            }
#endif
        }

        private void MonitorReadHardwareStateJobChangeThread()
        {
#if USETHREAD
            while (true)
            {
#endif
            while (qReadHardwareStateJob.Count > 0)
            {
                ReadHardwareStateJob hardwareStateJob = (ReadHardwareStateJob)qReadHardwareStateJob.Dequeue();

                foreach (KeyValuePair<uint, HardwareBase> kvp in simHardwareMap)
                {
                    if (hardwareStateJob != null &&
                        kvp.Value.Id == hardwareStateJob.Hardware.Id
                        //&& kvp.Value.CurrentBitState == hardwareStateJob.CurrentBitState
                        )
                    {
                        hardwareStateJob.Run();
                        Color color = ((hardwareStateJob.BitState & kvp.Value.BitState) != 0) ? Color.Green : Color.OrangeRed;

                        ListenerLogInfo(String.Format(
                                "Read HW state changed done. HWID: {0}, mask bit: 0x{1:D4}, current state bit 0x{2:D4}",
                                hardwareStateJob.Hardware.Id,
                                kvp.Value.BitMask.ToString("X"),
                                hardwareStateJob.BitState.ToString("X")),
                                color);
                    }
                }
            }
#if USETHREAD
            }
#endif
        }

        private void UpdateBitSetDgvData(uint bitMask, uint currentBitState)
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
    }


}
