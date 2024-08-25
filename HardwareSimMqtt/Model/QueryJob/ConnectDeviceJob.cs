using HardwareSimMqtt.Interface;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HardwareSimMqtt.Model.QueryJob
{
    public class ConnectDeviceJob : IJob
    {
        public DeviceBase DeviceBase
        {
            get;
            private set;
        }

        private ListenerWindow ParentWindow
        {
            get => Program.WndHandle;
        }

        public ConnectDeviceJob(DeviceBase deviceBase)
        {
            DeviceBase = deviceBase;
        }

        private bool _isCompleted = false;
        public bool IsCompleted
        {
            get => _isCompleted;
            private set
            {
                _isCompleted = value;
            }
        }

        public virtual void Run()
        {
            bool bRet = false;
            if (!DeviceBase.IsConnected)
            {
                bRet = DeviceBase.Connect();
            }

            if (!IsCompleted && bRet)
            {
                IsCompleted = true;
            }
        }

        public virtual bool WaitFinish()
        {
            while (!IsCompleted)
            {
                Thread.Sleep(100);
                Debug.WriteLine(String.Format("{0} Waiting to connect...", DeviceBase.Id));
            }

            return IsCompleted;
        }
    }
}
