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

        public Dictionary<uint, HardwareBase> HardwareMap
        {
            private get;
            set;
        }

        private Thread monitorJobQueryThread
        {
            get;
            set;
        }

        public MonitorTaskThread(Dictionary<uint, HardwareBase> hardwareMap)
        {
            QueuedJob = new PriorityQueue<IJob>();
            HardwareMap = hardwareMap;
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
                        if (taskJob.GetType() == typeof(SetHardwareStateJob))
                        {
                            SetHardwareStateJob setHardwareStateJob = (SetHardwareStateJob)taskJob;
                            if (HardwareMap != null)
                            {
                                foreach (KeyValuePair<uint, HardwareBase> kvp in HardwareMap)
                                {
                                    if (kvp.Value.Id == setHardwareStateJob.Hardware.Id)
                                    {
                                        setHardwareStateJob.Run();
                                        QueuedJob.Enqueue(new ReadHardwareStateJob(kvp.Value), 3);
                                    }
                                }
                            }
                        }

                        if (taskJob.GetType() == typeof(ReadHardwareStateJob))
                        {
                            ReadHardwareStateJob readHardwareStateJob = (ReadHardwareStateJob)taskJob;
                            if (HardwareMap != null)
                            {
                                foreach (KeyValuePair<uint, HardwareBase> kvp in HardwareMap)
                                {
                                    if (kvp.Value.Id == readHardwareStateJob.Hardware.Id)
                                    {
                                        readHardwareStateJob.Run();
                                    }
                                }
                            }
                        }
                    }

                }
            }
        }
    }
}
