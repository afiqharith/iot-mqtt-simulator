using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uPLibrary.Networking.M2Mqtt;

namespace HardwareSimMqtt.Model.QueryJob
{
    public class SetBrokerConnectJob
    {
        public MqttClient Client
        {
            get;
            set;
        }

        public string BrokerHostName
        {
            get;
            private set;
        }

        public SetBrokerConnectJob(string brokerHostName)
        {
            this.BrokerHostName = brokerHostName;

            if (this.Client == null)
            {
                this.Client = new MqttClient(this.BrokerHostName);
            }
        }

        public virtual bool Run()
        {
            if (this.Client == null)
            {
                return false;
            }

            string guid = Convert.ToString(Guid.NewGuid());
            bool bSuccess = true;
            try
            {
                this.Client.Connect(guid, "emqx", "public");
            }
            catch
            {
                bSuccess = false;
            }

            return bSuccess;
        }
    }
}
