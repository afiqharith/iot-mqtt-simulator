using HardwareSimMqtt.Model;

namespace HardwareSimMqtt.Interface
{
    public interface IJob
    {
        DeviceBase DeviceBase
        {
            get;
        }

        bool IsCompleted 
        { 
            get; 
        }

        void Run();
    }
}
