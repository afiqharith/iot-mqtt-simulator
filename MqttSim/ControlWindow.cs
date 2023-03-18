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
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;
using CheckBox = System.Windows.Forms.CheckBox;
using Newtonsoft.Json;
using static MqttSim.DisplayWindow;

namespace MqttSim
{
    public partial class ControlWindow : Form
    {
        Queue<string> msgQueue = new Queue<string>();

        private MqttClient _client;

        public ControlWindow()
        {
            InitializeComponent();

            checkBoxFan1.CheckStateChanged += new EventHandler(PerHWUnitCheckbox_CheckStateChanged);
            checkBoxFan2.CheckStateChanged += new EventHandler(PerHWUnitCheckbox_CheckStateChanged);
            checkBoxFan3.CheckStateChanged += new EventHandler(PerHWUnitCheckbox_CheckStateChanged);
            checkBoxFan4.CheckStateChanged += new EventHandler(PerHWUnitCheckbox_CheckStateChanged);

            checkBoxLamp1.CheckStateChanged += new EventHandler(PerHWUnitCheckbox_CheckStateChanged);
            checkBoxLamp2.CheckStateChanged += new EventHandler(PerHWUnitCheckbox_CheckStateChanged);
            checkBoxLamp3.CheckStateChanged += new EventHandler(PerHWUnitCheckbox_CheckStateChanged);
            checkBoxLamp4.CheckStateChanged += new EventHandler(PerHWUnitCheckbox_CheckStateChanged);

            _client = new MqttClient("broker.emqx.io");
            string guid = Convert.ToString(Guid.NewGuid());
            _client.Connect(guid, "emqx", "public");
            _client.MqttMsgPublished += _client_MqttMsgPublished;

        }

        private void checkBoxLoc_CheckStateChanged(object sender, EventArgs e)
        {
            CheckBox checkbox = (CheckBox)sender;
            List<HWPayloadState> payloadList = new List<HWPayloadState>();

            switch (checkbox.Name)
            {
                case "checkBoxLoc1":

                    if (checkbox.Checked)
                    {
                        checkBoxLamp1.CheckState = CheckState.Indeterminate;
                        checkBoxFan1.CheckState = CheckState.Indeterminate;
                    }
                    else
                    {
                        checkBoxLamp1.CheckState = CheckState.Unchecked;
                        checkBoxFan1.CheckState = CheckState.Unchecked;
                    }

                    //if (checkbox.Checked)
                    //{
                    //    checkBoxLamp1.CheckState = CheckState.Indeterminate;
                    //    checkBoxFan1.CheckState = CheckState.Indeterminate;
                    //}
                    //else
                    //{
                    //    checkBoxLamp1.CheckState = CheckState.Unchecked;
                    //    checkBoxFan1.CheckState = CheckState.Unchecked;
                    //}

                    payloadList.Add(new HWPayloadState("L-ID1", (uint)(checkbox.Checked ? 1 : 0)));
                    payloadList.Add(new HWPayloadState("F-ID1", (uint)(checkbox.Checked ? 1 : 0)));
                    break;

                case "checkBoxLoc2":
                    //if (checkbox.Checked)
                    //{
                    //    checkBoxLamp2.CheckState = CheckState.Indeterminate;
                    //    checkBoxFan2.CheckState = CheckState.Indeterminate;
                    //}
                    //else
                    //{
                    //    checkBoxLamp2.CheckState = CheckState.Unchecked;
                    //    checkBoxFan2.CheckState = CheckState.Unchecked;
                    //}

                    payloadList.Add(new HWPayloadState("L-ID2", (uint)(checkbox.Checked ? 1 : 0)));
                    payloadList.Add(new HWPayloadState("F-ID2", (uint)(checkbox.Checked ? 1 : 0)));
                    break;

                case "checkBoxLoc3":
                    //if (checkbox.Checked)
                    //{
                    //    checkBoxLamp3.CheckState = CheckState.Indeterminate;
                    //    checkBoxFan3.CheckState = CheckState.Indeterminate;
                    //}
                    //else
                    //{
                    //    checkBoxLamp3.CheckState = CheckState.Unchecked;
                    //    checkBoxFan3.CheckState = CheckState.Unchecked;
                    //}

                    payloadList.Add(new HWPayloadState("L-ID3", (uint)(checkbox.Checked ? 1 : 0)));
                    payloadList.Add(new HWPayloadState("F-ID3", (uint)(checkbox.Checked ? 1 : 0)));
                    break;

                case "checkBoxLoc4":
                    //if (checkbox.Checked)
                    //{
                    //    checkBoxLamp4.CheckState = CheckState.Indeterminate;
                    //    checkBoxFan4.CheckState = CheckState.Indeterminate;
                    //}
                    //else
                    //{
                    //    checkBoxLamp4.CheckState = CheckState.Unchecked;
                    //    checkBoxFan4.CheckState = CheckState.Unchecked;
                    //}

                    payloadList.Add(new HWPayloadState("L-ID4", (uint)(checkbox.Checked ? 1 : 0)));
                    payloadList.Add(new HWPayloadState("F-ID4", (uint)(checkbox.Checked ? 1 : 0)));
                    break;

                case "checkBoxShutdownAll":
                default:
                    //checkBoxLoc1.Checked = checkbox.Checked;
                    //checkBoxLoc2.Checked = checkbox.Checked;
                    //checkBoxLoc3.Checked = checkbox.Checked;
                    //checkBoxLoc4.Checked = checkbox.Checked;

                    payloadList.Add(new HWPayloadState("L-ID1", (uint)(checkbox.Checked ? 1 : 0)));
                    payloadList.Add(new HWPayloadState("F-ID1", (uint)(checkbox.Checked ? 1 : 0)));
                    payloadList.Add(new HWPayloadState("L-ID2", (uint)(checkbox.Checked ? 1 : 0)));
                    payloadList.Add(new HWPayloadState("F-ID2", (uint)(checkbox.Checked ? 1 : 0)));
                    payloadList.Add(new HWPayloadState("L-ID3", (uint)(checkbox.Checked ? 1 : 0)));
                    payloadList.Add(new HWPayloadState("F-ID3", (uint)(checkbox.Checked ? 1 : 0)));
                    payloadList.Add(new HWPayloadState("L-ID4", (uint)(checkbox.Checked ? 1 : 0)));
                    payloadList.Add(new HWPayloadState("F-ID4", (uint)(checkbox.Checked ? 1 : 0)));
                    break;
            }

            PublishPayloadMetaByBatch(payloadList);
        }

        private void PerHWUnitCheckbox_CheckStateChanged(object sender, EventArgs e)
        {
            CheckBox checkbox = (CheckBox)sender;

            if (checkbox.CheckState == CheckState.Indeterminate)
            {
                return;
            }

            List<HWPayloadState> payloadList = new List<HWPayloadState>();
            HWPayloadState payload = new HWPayloadState();

            switch (checkbox.Name)
            {
                case "checkBoxFan1":
                    payload.Id = "F-ID1";
                    break;

                case "checkBoxFan2":
                    payload.Id = "F-ID2";
                    break;

                case "checkBoxFan3":
                    payload.Id = "F-ID3";
                    break;

                case "checkBoxFan4":
                    payload.Id = "F-ID4";
                    break;


                case "checkBoxLamp1":
                    payload.Id = "L-ID1";
                    break;

                case "checkBoxLamp2":
                    payload.Id = "L-ID2";
                    break;

                case "checkBoxLamp3":
                    payload.Id = "L-ID3";
                    break;

                case "checkBoxLamp4":
                    payload.Id = "L-ID4";
                    break;

                default:
                    payload.Id = String.Empty;
                    break;
            }
            payload.State = Convert.ToUInt32(checkbox.Checked ? 1 : 0);
            payloadList.Add(payload);

            PublishPayloadMetaByBatch(payloadList);
        }

        private void PublishPayloadMetaByBatch(List<HWPayloadState> payloadList)
        {
            PayloadMeta multiplePayloadMeta = new PayloadMeta(payloadList);
            string multipleJsonPayload = JsonConvert.SerializeObject(multiplePayloadMeta);

            if (_client.IsConnected)
            {
                _client.Publish("IotWinformSim", Encoding.ASCII.GetBytes(multipleJsonPayload), MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE, true);

                for (int i = 0; i < payloadList.Count; i++)
                {
                    string jsonPayload = JsonConvert.SerializeObject(payloadList[i]);
                    AppendRTBText(String.Format("Sending state change command for {0}, cmd = 0x{1:D2}", payloadList[i].Id, payloadList[i].State), Color.Gray);
                    msgQueue.Enqueue(jsonPayload);
                }
            }
        }

        private void _client_MqttMsgPublished(object sender, MqttMsgPublishedEventArgs e)
        {
            if (e.IsPublished == true)
            {
                while (msgQueue.Count > 0)
                {
                    HWPayloadState payload = JsonConvert.DeserializeObject<HWPayloadState>(msgQueue.Dequeue());

                    AppendRTBText(
                        String.Format("Successfully sending changed state cmd for {0}, cmd = 0x{1:D2}", payload.Id, payload.State),
                        payload.State == 1 ? Color.Blue : Color.OrangeRed);
                }
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

        private void ControlWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_client.IsConnected)
            {
                _client.Disconnect();
            }
        }

        private void ControlWindow_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (_client.IsConnected)
            {
                _client.Disconnect();
            }
        }
    }
}
