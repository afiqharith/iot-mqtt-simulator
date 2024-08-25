using HardwareSimMqtt.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HardwareSimMqtt;

namespace HardwareSimMqtt.Model.QueryJob
{
    public class MonitorTaskThread
    {
        public PriorityQueue<IJob> QueuedJob
        {
            get;
        }

        public Dictionary<uint, DeviceBase> DeviceDict
        {
            private get;
            set;
        }

        private Thread monitorJobQueryThread
        {
            get;
            set;
        }

        public MonitorTaskThread(Dictionary<uint, DeviceBase> deviceDict)
        {
            QueuedJob = new PriorityQueue<IJob>();
            DeviceDict = deviceDict;
            InitializeThread();
        }

        public MonitorTaskThread()
        {
            QueuedJob = new PriorityQueue<IJob>();
            InitializeThread();
        }

        private void InitializeThread()
        {
            monitorJobQueryThread = new Thread(() => MonitorJobQuery(Program.CancelTokenSource.Token));

            if (!monitorJobQueryThread.IsAlive)
            {
                monitorJobQueryThread.Start();
            }
        }

        public void MonitorJobQuery(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                if (QueuedJob.Count > 0)
                {
                    IJob taskJob = QueuedJob.Dequeue();

                    if (taskJob != null)
                    {
                        if (taskJob.GetType() == typeof(SetDeviceStateJob))
                        {
                            SetDeviceStateJob setDeviceStateJob = (SetDeviceStateJob)taskJob;
                            if (DeviceDict != null)
                            {
                                foreach (KeyValuePair<uint, DeviceBase> kvp in DeviceDict)
                                {
                                    if (kvp.Value.Id == setDeviceStateJob.DeviceBase.Id)
                                    {
                                        setDeviceStateJob.Run();
                                        QueuedJob.Enqueue(new ReadDeviceStateJob(kvp.Value), 3);
                                    }
                                }
                            }
                        }
                        else
                        {
                            taskJob.Run();
                        }
                    }

                }
            }
        }
    }
}
