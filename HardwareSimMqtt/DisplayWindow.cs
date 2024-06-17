//#define USETHREAD
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using Timer = System.Windows.Forms.Timer;


namespace HardwareSimMqtt
{
    public partial class DisplayWindow : Form
    {
        //private MqttClient m_MqttClient { get; set; }
        private SetMqttBrokerConnectJob m_BrokerConnectJob { get; set; }
        private Timer systemTimer { get; set; }
        private Queue<PacketInfo> m_QPacketInfoReceived { get; set; }
        private Queue<SetHardwareStateJob> m_QSetHardwareStateJob { get; set; }
        internal Dictionary<uint, HardwareBase> m_SimHardwareMap { get; set; }
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

        //Use when de-packet the byte receive from broker
        private struct PacketInfo
        {
            public string topic;
            //public JsonHardwareInfoList hardwareInfoList;
            public List<HardwareInfo> hardwareInfoList;
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
            bPowerUpFinish = false;
            ControlWindow ctrlWindow = new ControlWindow();
            ctrlWindow.Show();
            InitSystemTimer();
            m_QPacketInfoReceived = new Queue<PacketInfo>();
            m_QSetHardwareStateJob = new Queue<SetHardwareStateJob>();

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
            if (m_BrokerConnectJob.Client.IsConnected)
            {
                m_BrokerConnectJob.Client.Disconnect();
            }
        }

        private void DisplayWindow_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (m_BrokerConnectJob.Client.IsConnected)
            {
                m_BrokerConnectJob.Client.Disconnect();
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
            JsonHardwareInfoList hardwareInfoList = JsonConvert.DeserializeObject<JsonHardwareInfoList>(Encoding.UTF8.GetString(e.Message));
            if(hardwareInfoList.InfoList == null) { return; }
            
            PacketInfo packetInfoReceived;
            packetInfoReceived.topic = e.Topic;
            packetInfoReceived.hardwareInfoList = hardwareInfoList.InfoList;
            m_QPacketInfoReceived.Enqueue(packetInfoReceived);
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
            systemTimer = new Timer();
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
                    m_BrokerConnectJob = new SetMqttBrokerConnectJob("broker.emqx.io");
                    bool bEstablished = m_BrokerConnectJob.Run();
                    iAutoNextStep = !bEstablished ? STATE.PU_ESTABLISH_CONNECTION_WITH_BROKER : STATE.PU_DELEGATE_MESSAGE_BROADCASTED_EVT;
                    break;

                case STATE.PU_DELEGATE_MESSAGE_BROADCASTED_EVT:
                    m_BrokerConnectJob.Client.MqttMsgPublishReceived += OnMqttMessageReceived;
                    iAutoNextStep = STATE.PU_SUBSCRIBE_TOPIC;
                    break;

                case STATE.PU_SUBSCRIBE_TOPIC:
                    m_BrokerConnectJob.Client.Subscribe(new string[] { "IotWinformSim" }, new byte[] { MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE });
                    iAutoNextStep = STATE.PU_INIT_SIM_HARDWARE_INSTANCE;
                    break;

                case STATE.PU_INIT_SIM_HARDWARE_INSTANCE:
                    InitializeHardwareMap();
                    iAutoNextStep = STATE.PU_SET_SIM_HARDWARE_INIT_STATE;
                    break;

                case STATE.PU_SET_SIM_HARDWARE_INIT_STATE:
                    foreach (KeyValuePair<uint, HardwareBase> kvp in m_SimHardwareMap)
                    {
                        //kvp.Value.SetCurrentStateProperty(0x0);
                        kvp.Value.CurrentState = 0x0;
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
                    iAutoNextStep = m_QPacketInfoReceived.Count == 0 ? STATE.AUTO_WAIT_NEW_MESSAGE_BROADCAST : STATE.AUTO_PRE_TRANSLATE_RECEIVED_MESSAGE;
                    break;

                case STATE.AUTO_PRE_TRANSLATE_RECEIVED_MESSAGE:
                    TranslatePacketReceived();
                    iAutoNextStep = m_QSetHardwareStateJob.Count > 0 ? STATE.AUTO_UPDATE_HARDWARE_STATE : STATE.AUTO_WAIT_NEW_MESSAGE_BROADCAST;
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
            while (m_QPacketInfoReceived.Count > 0)
            {
                PacketInfo packetReceived = m_QPacketInfoReceived.Dequeue();
                if (packetReceived.topic == "IotWinformSim" /*&& packetReceived.dataList.ListContent != null*/)
                {
                    for (int i = 0; i < packetReceived.hardwareInfoList.Count; i++)
                    {
                        foreach (KeyValuePair<uint, HardwareBase> kvp in m_SimHardwareMap)
                        {
                            if (kvp.Value.Id == packetReceived.hardwareInfoList[i].Id)
                            {
                                m_QSetHardwareStateJob.Enqueue(
                                    new SetHardwareStateJob(
                                        kvp.Value,
                                        packetReceived.hardwareInfoList[i].CurrentState));

                                LogInfo(String.Format(
                                    "HW new state received. HWID: {0}, current state: 0x{1:D2}",
                                    packetReceived.hardwareInfoList[i].Id,
                                    packetReceived.hardwareInfoList[i].CurrentState), 
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
            while (m_QSetHardwareStateJob.Count > 0)
            {
                SetHardwareStateJob hardwareStateJob = m_QSetHardwareStateJob.Dequeue();

                foreach (KeyValuePair<uint, HardwareBase> kvp in m_SimHardwareMap)
                {
                    if (kvp.Value.Id == hardwareStateJob.Hardware.Id &&
                        /*kvp.Value.GetCurrentState()*/ kvp.Value.CurrentState != hardwareStateJob.NewState &&
                        hardwareStateJob != null)
                    {
                        Color color = hardwareStateJob.NewState == 1 ? Color.Green : Color.OrangeRed;
                        LogInfo(String.Format(
                                "HW state changed. HWID: {0}, current state: 0x{1:D2}",
                                hardwareStateJob.Hardware.Id, 
                                hardwareStateJob.NewState),
                                color);
                        hardwareStateJob.Run();
                    }
                }
            }
#if USETHREAD
            }
#endif
        }

        private void InitializeHardwareMap()
        {
            m_SimHardwareMap = new Dictionary<uint, HardwareBase>();
            Panel[] panel = new Panel[]
            {
                panelLamp1,
                panelLamp2,
                panelLamp3,
                panelLamp4,

                panelFan1,
                panelFan2,
                panelFan3,
                panelFan4,
            };

            for (uint i = 0; i < panel.Length; i++)
            {
                string id = i < 4 ? "L-ID{0}" : "F-ID{0}";
                if (i < 4)
                {
                    m_SimHardwareMap.Add(i, new SimpLamp(panel[i], LOC.Loc1 + (int)i, String.Format(id, i + 1)));
                }
                else
                {
                    m_SimHardwareMap.Add(i, new SimFan(panel[i], LOC.Loc1 + (int)i - 4, String.Format(id, i - 3)));
                }
            }
        }

    }
}
