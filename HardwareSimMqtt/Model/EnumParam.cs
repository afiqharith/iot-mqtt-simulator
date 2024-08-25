using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HardwareSimMqtt.Model
{
    public enum AutoState
    {
        PU_SETUP_CONNECTION_WITH_BROKER,
        PU_INIT_SIM_DEVICE_INSTANCE,
        PU_SET_SIM_DEVICE_INIT_STATE,
        PU_COMPLETE,

        AUTO_WAIT_NEW_MESSAGE_BROADCAST,
        AUTO_PRE_TRANSLATE_RECEIVED_MESSAGE,
        AUTO_UPDATE_DEVICE_STATE,

        PE_SYSTEM_SHUTDOWN,
    }
}
