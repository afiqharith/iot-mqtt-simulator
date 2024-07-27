using HardwareSimMqtt.Model;

namespace HardwareSimMqtt.Interface
{
    public interface IJob
    {
        HardwareBase Hardware
        {
            get;
        }

        void Run();
    }
}
