using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HardwareSimMqtt
{
    public class HardwareInfo: IHardware
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

    //Use ONLY when to serializing/deserializing to JSON object
    public class JsonHardwareInfoList
    {
        public List<HardwareInfo> InfoList { get; set; }

        public JsonHardwareInfoList(List<HardwareInfo> hardwareInfoList)
        {
            this.InfoList = hardwareInfoList;
        }

        public JsonHardwareInfoList() { }
    }

    public interface IJob
    {
        void Run();
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
                this.Hardware.CurrentState = this.NewState;
            }
        }
    }

    
}
