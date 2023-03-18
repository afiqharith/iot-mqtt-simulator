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

namespace MqttSim
{
    public partial class DisplayWindow : Form
    {
        private MqttClient _client;
        private SimulatedObjectBase[] simHWInstance = new SimulatedObjectBase[8];
        private Timer systemTimer;
        private Queue<s_ReceievedPayload> q_ReceivedPayload = new Queue<s_ReceievedPayload>();
        private Queue<s_HWInstance> q_HWInstanceState = new Queue<s_HWInstance>();
        private bool bPowerUpFinish = false;
        private STATE iLastSwitchStep;

        private STATE _iAutoNextStep;

        private STATE iAutoNextStep
        {
            get
            {
                return _iAutoNextStep;
            }

            set
            {
                if (value != iLastSwitchStep)
                {
                    _iAutoNextStep = value;
                    iLastSwitchStep = value;
                    AppendRTBText(String.Format("State change = ({0}) {1}", (int)value, value), Color.Gray);
                }
            }
        }

        public struct s_ReceievedPayload
        {
            public string topic;
            public PayloadMeta content;
        }

        private struct s_HWInstance
        {
            public string id;
            public uint state;
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

        public DisplayWindow()
        {
            InitializeComponent();

            ControlWindow ctrlWindow = new ControlWindow();
            ctrlWindow.Show();
            InitSystemTimer();
        }

        private void DisplayWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_client.IsConnected)
            {
                _client.Disconnect();
            }
        }

        private void DisplayWindow_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (_client.IsConnected)
            {
                _client.Disconnect();
            }
        }

        private void AppendRTBText(string text, Color color)
        {
            if (richTextBox1.InvokeRequired)
            {
                Action safeThread = delegate { AppendRTBText(text, color); };
                richTextBox1.Invoke(safeThread);
            }
            else
            {
                richTextBox1.SelectionStart = richTextBox1.TextLength;
                richTextBox1.SelectionLength = 0;

                richTextBox1.SelectionColor = color;
                richTextBox1.AppendText(DateTime.Now.ToString("HH:mm:ss,fff") + String.Format(" {0}", text) + Environment.NewLine);
                richTextBox1.SelectionColor = richTextBox1.ForeColor;
                richTextBox1.ScrollToCaret();
            }
        }

        private void SystemTimer_Tick(object sender, EventArgs e)
        {
            if (!bPowerUpFinish)
            {
                bPowerUpFinish = PowerUp();
            }
            else
            {
                Run();
            }
        }

        private void MqttMessageBroadcasted(object sender, MqttMsgPublishEventArgs e)
        {
            s_ReceievedPayload payload;
            payload.topic = e.Topic;
            payload.content = JsonConvert.DeserializeObject<PayloadMeta>(Encoding.UTF8.GetString(e.Message));
            q_ReceivedPayload.Enqueue(payload);
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

        private bool PowerUp()
        {
            switch (iAutoNextStep)
            {
                case STATE.PU_ESTABLISH_CONNECTION_WITH_BROKER:
                    _client = new MqttClient("broker.emqx.io");
                    string guid = Convert.ToString(Guid.NewGuid());

                    bool bCatchedException = false;
                    try
                    {
                        _client.Connect(guid, "emqx", "public");
                    }
                    catch
                    {
                        bCatchedException = true;
                    }
                    iAutoNextStep = bCatchedException ? STATE.PU_ESTABLISH_CONNECTION_WITH_BROKER : STATE.PU_DELEGATE_MESSAGE_BROADCASTED_EVT;
                    break;

                case STATE.PU_DELEGATE_MESSAGE_BROADCASTED_EVT:
                    _client.MqttMsgPublishReceived += MqttMessageBroadcasted;
                    iAutoNextStep = STATE.PU_SUBSCRIBE_TOPIC;
                    break;

                case STATE.PU_SUBSCRIBE_TOPIC:
                    _client.Subscribe(new string[] { "IotWinformSim" }, new byte[] { MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE });
                    iAutoNextStep = STATE.PU_INIT_SIM_HARDWARE_INSTANCE;
                    break;

                case STATE.PU_INIT_SIM_HARDWARE_INSTANCE:

                    simHWInstance[0] = new SimulatedObjectBase(panelLamp1, HW_TYPE.LAMP, LOC.Loc1, "L-ID1");
                    simHWInstance[1] = new SimulatedObjectBase(panelLamp2, HW_TYPE.LAMP, LOC.Loc2, "L-ID2");
                    simHWInstance[2] = new SimulatedObjectBase(panelLamp3, HW_TYPE.LAMP, LOC.Loc3, "L-ID3");
                    simHWInstance[3] = new SimulatedObjectBase(panelLamp4, HW_TYPE.LAMP, LOC.Loc4, "L-ID4");

                    simHWInstance[4] = new SimulatedObjectBase(panelFan1, HW_TYPE.FAN, LOC.Loc1, "F-ID1");
                    simHWInstance[5] = new SimulatedObjectBase(panelFan2, HW_TYPE.FAN, LOC.Loc2, "F-ID2");
                    simHWInstance[6] = new SimulatedObjectBase(panelFan3, HW_TYPE.FAN, LOC.Loc3, "F-ID3");
                    simHWInstance[7] = new SimulatedObjectBase(panelFan4, HW_TYPE.FAN, LOC.Loc4, "F-ID4");

                    iAutoNextStep = STATE.PU_SET_SIM_HARDWARE_INIT_STATE;
                    break;

                case STATE.PU_SET_SIM_HARDWARE_INIT_STATE:
                    for (int i = 0; i < simHWInstance.Length; i++)
                    {
                        simHWInstance[i].SetCurrentState(0x0);
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

        private bool Run()
        {
            switch (iAutoNextStep)
            {
                case STATE.AUTO_WAIT_NEW_MESSAGE_BROADCAST:
                    iAutoNextStep = q_ReceivedPayload.Count == 0 ? STATE.AUTO_WAIT_NEW_MESSAGE_BROADCAST : STATE.AUTO_PRE_TRANSLATE_RECEIVED_MESSAGE;
                    break;

                case STATE.AUTO_PRE_TRANSLATE_RECEIVED_MESSAGE:
                    while (q_ReceivedPayload.Count > 0)
                    {
                        s_ReceievedPayload receivedData = q_ReceivedPayload.Dequeue();
                        if (receivedData.topic == "IotWinformSim")
                        {
                            for (int i = 0; i < receivedData.content.hwStateList.Count; i++)
                            {
                                s_HWInstance hw;
                                hw.id = receivedData.content.hwStateList[i].Id;
                                hw.state = receivedData.content.hwStateList[i].State;
                                q_HWInstanceState.Enqueue(hw);
                            }
                        }
                    }
                    iAutoNextStep = q_HWInstanceState.Count > 0 ? STATE.AUTO_UPDATE_HARDWARE_STATE : STATE.AUTO_WAIT_NEW_MESSAGE_BROADCAST;
                    break;

                case STATE.AUTO_UPDATE_HARDWARE_STATE:
                    while (q_HWInstanceState.Count > 0)
                    {
                        s_HWInstance hwInstance = q_HWInstanceState.Dequeue();

                        for (int i = 0; i < simHWInstance.Length; i++)
                        {
                            if (simHWInstance[i].Id == hwInstance.id &&
                                simHWInstance[i].GetCurrentState() != hwInstance.state)
                            {
                                Color color = hwInstance.state == 1 ? Color.Green : Color.OrangeRed;
                                AppendRTBText(String.Format("{0} current state change = 0x{1:D2}", hwInstance.id, hwInstance.state), color);
                                simHWInstance[i].SetCurrentState(hwInstance.state);
                                //break;
                            }
                        }
                    }
                    iAutoNextStep = STATE.AUTO_WAIT_NEW_MESSAGE_BROADCAST;
                    break;

                case STATE.PE_SYSTEM_SHUTDOWN:
                default:
                    break;

            }
            return true;
        }

    }
}
