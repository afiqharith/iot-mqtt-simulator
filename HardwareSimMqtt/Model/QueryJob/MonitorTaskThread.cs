using HardwareSimMqtt.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

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
            this.QueuedJob = new PriorityQueue<IJob>();
            this.HardwareMap = hardwareMap;
            InitializeThread();
        }

        public MonitorTaskThread()
        {
            this.QueuedJob = new PriorityQueue<IJob>();
            InitializeThread();
        }

        private void InitializeThread()
        {
            monitorJobQueryThread = new Thread(new ThreadStart(MonitorJobQuery));
            if (!monitorJobQueryThread.IsAlive)
            {
                monitorJobQueryThread.Start();
            }
        }

        public void MonitorJobQuery()
        {
            while (true)
            {
                while (QueuedJob.Count > 0)
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
