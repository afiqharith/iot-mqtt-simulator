using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MqttSim
{

    public class HardwareInfoList
    {
        public List<HardwareInfo> ListContent { get; set; }

        public HardwareInfoList(List<HardwareInfo> hardwareInfoList)
        {
            this.ListContent = hardwareInfoList;
        }

        public HardwareInfoList() { }
    }


    public class HardwareInfo
    {
        public string Id { get; set; }
        public uint CurrentState { get; set; }

        public HardwareInfo(string id, uint state)
        {
            this.Id = id;
            this.CurrentState = state;
        }

        public HardwareInfo() { }
    }

    public class SetHardwareStateJob: IJob
    {
        public uint NewState { get; private set; }
        public HardwareBase Hardware { get; private set; }

        public SetHardwareStateJob(HardwareBase hardware, uint newState)
        {
            this.Hardware = hardware;
            this.NewState = newState;
        }

        public virtual void Run()
        {
            bool iRet = false;
            iRet = this.Hardware.Connect();

            if(iRet)
            {
                this.Hardware.SetCurrentState(this.NewState);
            }
        }
    }

    public interface IJob
    {
        void Run();
    }
}
