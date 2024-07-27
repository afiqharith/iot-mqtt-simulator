using HardwareSimMqtt.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HardwareSimMqtt.EventArgsModel
{
    public class AutoStepChangeEventArgs : EventArgs
    {
        public STATE OldStep
        {
            get;
            set;
        }

        public STATE NewStep
        {
            get;
            set;
        }

        public AutoStepChangeEventArgs(STATE oldStep, STATE newStep)
        {
            this.OldStep = oldStep;
            this.NewStep = newStep;
        }
    }
}
