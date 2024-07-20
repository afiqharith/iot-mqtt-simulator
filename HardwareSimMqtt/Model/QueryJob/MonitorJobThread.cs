using HardwareSimMqtt.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HardwareSimMqtt.Model.QueryJob
{
    public class MonitorJobThread
    {
        public Queue<IJob> QueuedJob
        {
            get;
            set;
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

        public MonitorJobThread(Dictionary<uint, HardwareBase> hardwareMap)
        {
            this.QueuedJob = new Queue<IJob>();
            this.HardwareMap = hardwareMap;

            monitorJobQueryThread = new Thread(new ThreadStart(MonitorJobQuery));
            if (!monitorJobQueryThread.IsAlive)
            {
                monitorJobQueryThread.Start();
            }
        }

        public MonitorJobThread()
        {
            this.QueuedJob = new Queue<IJob>();

            monitorJobQueryThread = new Thread(new ThreadStart(MonitorJobQuery));
            if (!monitorJobQueryThread.IsAlive)
            {
                monitorJobQueryThread.Start();
            }
        }

        public void EnqueueJob(IJob job)
        {
            this.QueuedJob.Enqueue(job);
        }

        public IJob DequeueJob()
        {
            IJob job = null;
            if(this.QueuedJob.Count > 0)
            {
                job = this.QueuedJob.Dequeue();
            }
            return job;
        }

        public void MonitorJobQuery()
        {
            while (true)
            {
                while (this.QueuedJob.Count > 0)
                {
                    IJob taskJob = DequeueJob();

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
                                        EnqueueJob(new ReadHardwareStateJob(kvp.Value));
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
