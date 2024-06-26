﻿//#define USETHREAD
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using uPLibrary.Networking.M2Mqtt.Messages;
using Timer = System.Windows.Forms.Timer;


namespace HardwareSimMqtt
{
    using Model;
    using QueryJob;
    using DataContainer;
    using BitMap;
    using System.Threading;

    public partial class DisplayWindow : Form
    {
        private SetMqttBrokerConnectJob brokerConnectJob { get; set; }
        private System.Windows.Forms.Timer systemTimer { get; set; }
        private Queue<PacketInfo> QpacketInfoReceived { get; set; }
        private Queue<SetHardwareStateJob> QsetHardwareStateJob { get; set; }
        protected Dictionary<uint, HardwareBase> simHardwareMap { get; set; }
        private uint realTimeBitSet { get; set; }
        private DataTable bitSetDataTable { get; set; }
        private bool bPowerUpFinish { get; set; }
        private STATE iLastSwitchStep { get; set; }

        private STATE _iAutoNextStep = 0;
        private STATE iAutoNextStep
        {
            get { return _iAutoNextStep; }
            set
            {
                SetNextStepProperty(ref _iAutoNextStep, value);
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
        private Thread HardwareMonitorJobThread { get; set; }
#endif
        public DisplayWindow()
        {
            InitializeComponent();
            ControlWindow ctrlWindow = new ControlWindow();
            ctrlWindow.Show();

            bPowerUpFinish = false;
            QpacketInfoReceived = new Queue<PacketInfo>();
            QsetHardwareStateJob = new Queue<SetHardwareStateJob>();
            InitializeBitSetDgv();
            InitSystemTimer();

#if USETHREAD
            HardwareMonitorJobThread = new Thread(new ThreadStart(MonitorStateJobChangeThread));
            if (!HardwareMonitorJobThread.IsAlive)
            {
                HardwareMonitorJobThread.Start();
            }
#endif
        }

        private void DisplayWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (brokerConnectJob.Client.IsConnected)
            {
                brokerConnectJob.Client.Disconnect();
            }
        }

        private void DisplayWindow_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (brokerConnectJob.Client.IsConnected)
            {
                brokerConnectJob.Client.Disconnect();
            }
        }

        private void LogInfo(string text, Color color)
        {
            SystemHelper.AppendRTBText(richTextBox1, text, color);
        }

        private void SystemTimer_Tick(object sender, EventArgs e)
        {
            if (!bPowerUpFinish)
            {
                bPowerUpFinish = PowerUpOperation();

            }
            else
            {
                AutoOperation();
            }
        }

        private void OnMqttMessageReceived(object sender, MqttMsgPublishEventArgs e)
        {
            JsonBitInfoList bitInfoList = JsonConvert.DeserializeObject<JsonBitInfoList>(Encoding.UTF8.GetString(e.Message));
            if (bitInfoList.InfoList == null) { return; }

            PacketInfo packetInfoReceived;
            packetInfoReceived.topic = e.Topic;
            packetInfoReceived.bitInfoList = bitInfoList.InfoList;
            QpacketInfoReceived.Enqueue(packetInfoReceived);
        }

        private void SetNextStepProperty(ref STATE step, STATE newval)
        {
            if (iLastSwitchStep != newval)
            {
                LogInfo(String.Format("Seq state change({0}) = ({1}) {2}", (int)step, (int)newval, newval), Color.Gray);
                step = newval;
                iLastSwitchStep = newval;
            }
        }

        private bool InitSystemTimer()
        {
            systemTimer = new System.Windows.Forms.Timer();
            systemTimer.Enabled = true;
            systemTimer.Interval = 1;
            systemTimer.Tick += new EventHandler(SystemTimer_Tick);
            systemTimer.Start();

            return true;
        }

        private bool PowerUpOperation()
        {
            switch (iAutoNextStep)
            {
                case STATE.PU_ESTABLISH_CONNECTION_WITH_BROKER:
                    brokerConnectJob = new SetMqttBrokerConnectJob("broker.emqx.io");
                    bool bEstablished = brokerConnectJob.Run();
                    iAutoNextStep = !bEstablished ? STATE.PU_ESTABLISH_CONNECTION_WITH_BROKER : STATE.PU_DELEGATE_MESSAGE_BROADCASTED_EVT;
                    break;

                case STATE.PU_DELEGATE_MESSAGE_BROADCASTED_EVT:
                    brokerConnectJob.Client.MqttMsgPublishReceived += OnMqttMessageReceived;
                    iAutoNextStep = STATE.PU_SUBSCRIBE_TOPIC;
                    break;

                case STATE.PU_SUBSCRIBE_TOPIC:
                    brokerConnectJob.Client.Subscribe(new string[] { "IotWinformSim" }, new byte[] { MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE });
                    iAutoNextStep = STATE.PU_INIT_SIM_HARDWARE_INSTANCE;
                    break;

                case STATE.PU_INIT_SIM_HARDWARE_INSTANCE:
                    InitializeHardwareMap();
                    iAutoNextStep = STATE.PU_SET_SIM_HARDWARE_INIT_STATE;
                    break;

                case STATE.PU_SET_SIM_HARDWARE_INIT_STATE:
                    foreach (KeyValuePair<uint, HardwareBase> kvp in simHardwareMap)
                    {
                        kvp.Value.CurrentBitState = kvp.Value.BitMask & ~kvp.Value.BitMask;
                    }
                    iAutoNextStep = STATE.PU_COMPLETE;
                    break;

                case STATE.PU_COMPLETE:
                    bPowerUpFinish = true;
                    iAutoNextStep = STATE.AUTO_WAIT_NEW_MESSAGE_BROADCAST;
                    break;

            }
            return bPowerUpFinish;
        }

        private bool AutoOperation()
        {
            switch (iAutoNextStep)
            {
                case STATE.AUTO_WAIT_NEW_MESSAGE_BROADCAST:
                    iAutoNextStep = QpacketInfoReceived.Count == 0 ? STATE.AUTO_WAIT_NEW_MESSAGE_BROADCAST : STATE.AUTO_PRE_TRANSLATE_RECEIVED_MESSAGE;
                    break;

                case STATE.AUTO_PRE_TRANSLATE_RECEIVED_MESSAGE:
                    TranslatePacketReceived();
                    iAutoNextStep = QsetHardwareStateJob.Count > 0 ? STATE.AUTO_UPDATE_HARDWARE_STATE : STATE.AUTO_WAIT_NEW_MESSAGE_BROADCAST;
                    break;

                case STATE.AUTO_UPDATE_HARDWARE_STATE:
#if !USETHREAD
                    MonitorStateJobChangeThread();
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
            while (QpacketInfoReceived.Count > 0)
            {
                PacketInfo packetReceived = QpacketInfoReceived.Dequeue();
                if (packetReceived.topic == "IotWinformSim")
                {
                    for (int i = 0; i < packetReceived.bitInfoList.Count; i++)
                    {
                        foreach (KeyValuePair<uint, HardwareBase> kvp in simHardwareMap)
                        {
                            if (kvp.Value.Id == packetReceived.bitInfoList[i].Id)
                            {
                                QsetHardwareStateJob.Enqueue(
                                    new SetHardwareStateJob(
                                        kvp.Value,
                                        packetReceived.bitInfoList[i].CurrentBitState));

                                LogInfo(String.Format(
                                    "HW new state received. HWID: {0}, mask bit: 0x{1:D4}, received state bit: 0x{2:D4}",
                                    packetReceived.bitInfoList[i].Id,
                                    kvp.Value.BitMask.ToString("X"),
                                     packetReceived.bitInfoList[i].CurrentBitState.ToString("X")),
                                    Color.Blue);
                            }
                        }
                    }
                }
            }

            return true;
        }

        private void MonitorStateJobChangeThread()
        {
#if USETHREAD
            while (true)
            {
#endif
            while (QsetHardwareStateJob.Count > 0)
            {
                SetHardwareStateJob hardwareStateJob = QsetHardwareStateJob.Dequeue();

                foreach (KeyValuePair<uint, HardwareBase> kvp in simHardwareMap)
                {
                    if (kvp.Value.Id == hardwareStateJob.Hardware.Id &&
                        kvp.Value.CurrentBitState != hardwareStateJob.NewBitState &&
                        hardwareStateJob != null)
                    {
                        Color color = ((hardwareStateJob.NewBitState & kvp.Value.CurrentBitState) != 0) ? Color.Green : Color.OrangeRed;

                        LogInfo(String.Format(
                                "HW state changed. HWID: {0}, mask bit: 0x{1:D4}, state bit change from 0x{2:D4} to 0x{3:D4}",
                                hardwareStateJob.Hardware.Id,
                                kvp.Value.BitMask.ToString("X"),
                                kvp.Value.CurrentBitState.ToString("X"),
                                hardwareStateJob.NewBitState.ToString("X")),
                                color);
                        hardwareStateJob.Run();
                        UpdateBitSetDgvData(hardwareStateJob.Hardware.BitMask, hardwareStateJob.Hardware.CurrentBitState);
                    }
                }
            }
#if USETHREAD
            }
#endif
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
                            (eBitMask)bitMask));
                }
                else
                {
                    simHardwareMap.Add((uint)i, 
                        new SimLamp(
                            panelList[i], 
                            eLOC.Loc1 + i - 4, 
                            Convert.ToString(i - 3), 
                            (eBitMask)bitMask));
                }
            }

            //Do connection attempt
            foreach(KeyValuePair<uint, HardwareBase> kvp in simHardwareMap)
            {
                kvp.Value.Connect();
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

        private void UpdateBitSetDgvData(uint bitMask, uint currentBitState)
        {
            for (int nCol = bitSetDataTable.Columns.Count - 1; nCol >= 0; nCol--)
            {
                //Update overall system bitSet realtime
                if ((bitMask & currentBitState) != 0 && ((1 << nCol) & bitMask) != 0)
                {
                    realTimeBitSet |= bitMask;

                }
                else if ((bitMask & currentBitState) == 0 && ((1 << nCol) & bitMask) == 0)
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
