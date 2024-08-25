using HardwareSimMqtt.HardwareHub;
using HardwareSimMqtt.Model;
using HardwareSimMqtt.Model.BitMap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace HardwareSimMqtt.Utils
{
    public class DevcieConfig
    {
        public int Node
        {
            get;
            set;
        }

        public int Bit
        {
            get;
            set;
        }

        public DeviceBitMask BitMask
        {
            get
            {
                if(Bit < 0)
                {
                    return DeviceBitMask.Invalid;
                }
                return (DeviceBitMask)(1 << Bit);
            }                
        }

        public int IoPort
        {
            get;
            set;
        }

        public IoType IoType
        {
            get;
            set;
        }

        public ControllerType ControllerType
        {
            get;
            set;
        }

        public DeviceType DeviceType
        {
            get;
            set;
        }

        public DeviceGroup Group
        {
            get;
            set;
        }

        public string Id
        {
            get
            {
                if (Group != DeviceGroup.Invalid)
                {
                    return Convert.ToString((int)Group);
                }
                return null;
            }            
        }

        public string ComPort
        {
            get;
            set;
        }

        public int Baudrate
        {
            get;
            set;
        }

        public DevcieConfig() { }
    }

    public class ConfigSetting
    {
        private List<DevcieConfig> _configList;
        public List<DevcieConfig> ConfigList
        {
            get
            {
                if (_configList == null)
                {
                    _configList = new List<DevcieConfig>();
                }
                return _configList;
            }
        }

        public string FilePath
        {
            get;
            private set;
        }

        public ConfigSetting(string xmlConfigPath)
        {
            FilePath = xmlConfigPath;
        }

        public void Load()
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(FilePath);
            XmlNodeList settings = xmlDoc.SelectNodes("/configuration/bitmap/setting");

            for (int i = 0; i < settings.Count; i++)
            {
                DevcieConfig config = new DevcieConfig();
                config.Bit = Convert.ToInt32(settings[i].Attributes["bit"].Value);                 

                XmlNodeList details = settings[i].SelectNodes("detail");

                if (details.Count != 0)
                {
                    for (int j = 0; j < details.Count; j++)
                    {
                        if (details[j].Attributes["name"].Value == "IOPort")
                        {
                            config.IoPort = Convert.ToInt32(details[j].Attributes["value"].Value);
                        }
                        else if (details[j].Attributes["name"].Value == "IOType")
                        {
                            config.IoType = (IoType)Convert.ToInt32(details[j].Attributes["value"].Value);
                        }
                        else if (details[j].Attributes["name"].Value == "HardwareType")
                        {
                            config.DeviceType = (DeviceType)Convert.ToInt32(details[j].Attributes["value"].Value);
                        }
                        else if (details[j].Attributes["name"].Value == "ControllerType")
                        {
                            config.ControllerType = (ControllerType)Convert.ToInt32(details[j].Attributes["value"].Value);
                        }
                        else if (details[j].Attributes["name"].Value == "Group")
                        {
                            config.Group = (DeviceGroup)Convert.ToInt32(details[j].Attributes["value"].Value);
                        }
                        else
                        {
                            continue;
                        }
                    }
                }

                ConfigList.Add(config);
            }
        }

        public void Refreash()
        {
            ConfigList.Clear();
            Load();
        }


    }
}
