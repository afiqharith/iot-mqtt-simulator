using HardwareSimMqtt.Model;

namespace HardwareSimMqtt.Interface
{
    internal interface IJob
    {
        HardwareBase Hardware
        {
            get;
        }

        void Run();
    }
}
