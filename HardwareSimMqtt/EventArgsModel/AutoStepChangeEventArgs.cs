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
        public AutoState OldStep
        {
            get;
            set;
        }

        public AutoState NewStep
        {
            get;
            set;
        }

        public AutoStepChangeEventArgs(AutoState oldStep, AutoState newStep)
        {
            this.OldStep = oldStep;
            this.NewStep = newStep;
        }
    }
}
