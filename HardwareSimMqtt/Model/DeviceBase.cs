#define SIMULATE
using System;
using System.Diagnostics;
using System.Threading;
using HardwareSimMqtt.Model.BitMap;
using HardwareSimMqtt.Interface;
using HardwareSimMqtt.HardwareHub;
using HardwareSimMqtt.Utils;

namespace HardwareSimMqtt.Model
{
    public enum DeviceType
    {
        LAMP,
        FAN,
        AIR_CONDITIONER,
        GATE,
    }

    public enum DeviceGroup
    {
        Invalid = -1,
        Group1 = 1,
        Group2,
        Group3,
        Group4,
        Group5,
        Group6,
        Group7,
        Group8,

        Area1 = Group1,
        Area2,
        Area3,
        Area4,

        Location1 = Area1,
        Location2,
        Location3,
        Location4
    }

    public class DeviceBase : IDevice
    {
        private string _id;
        public virtual string Id
        {
            get => _id;
            protected set
            {
                string createdId = String.Empty;
                switch (DeviceType)
                {
                    default:
                    case DeviceType.LAMP:
                        createdId = String.Format("HWLID{0}", value);
                        break;

                    case DeviceType.FAN:
                        createdId = String.Format("HWFID{0}", value);
                        break;

                    case DeviceType.AIR_CONDITIONER:
                        createdId = String.Format("HWACID{0}", value);
                        break;

                    case DeviceType.GATE:
                        createdId = String.Format("HWGID{0}", value);
                        break;
                }
                _id = createdId;
            }
        }

        public virtual DeviceType DeviceType
        {
            get;
            protected set;
        }

        public virtual DeviceGroup DeviceGroup
        {
            get;
            protected set;
        }

        public virtual bool IsConnected
        {
            get;
            protected set;
        }

        public virtual bool IsOn
        {
            get;
            private set;
        }

        public virtual bool IsOff => !IsOn;

        // Bit index for the device, bit map
        private uint _bitMask = 0;
        public virtual uint BitMask
        {
            get => _bitMask;
            protected set
            {
                _bitMask = value;
                //Map with device bit
                ComController.BitMask = value;
            }
        }

        // Bit state of the device at the bit index
        private uint _bitState = 0;
        public virtual uint BitState
        {
            get => _bitState;
            set
            {
                _bitState = value;
                ComController.SendDigitalOutputCommand(value);
                IsOn = GetNewBitStateValue(value) == BitMask;
            }
        }

        private int _analogData = -1;
        public virtual int AnalogData
        {
            get => _analogData;
            set
            {
                _analogData = value;
                ComController.SendAnalogOutputCommand(value);
            }
        }

        protected IComController ComController
        {
            get;
            set;
        }

        //Using GPIO
        public DeviceBase(string id, DeviceBitMask deviceBitMask, DeviceType deviceType, DeviceGroup deviceGroup, IoType ioType, int ioPort)
        {
#if !SIMULATE
            ComController = new HHGPIOController(ioType, ioPort);
#else
            ComController = new HHEmuGPIOController(ioType, ioPort);
#endif
            DeviceType = deviceType;
            DeviceGroup = deviceGroup;
            Id = id;
            BitMask = (uint)deviceBitMask;


        }

        //Using SerialPort
        public DeviceBase(string id, DeviceBitMask deviceBitMask, DeviceType type, DeviceGroup deviceGroup, IoType ioType, string portName, int baudRate = 9600)
        {
            ComController = new HHSerialPortController(ioType, portName, baudRate);
            Id = id;
            BitMask = (uint)deviceBitMask;
            DeviceType = type;
            DeviceGroup = deviceGroup;
        }
        
        //General
        public DeviceBase(DevcieConfig deviceConfig)
        {

            if (deviceConfig.ControllerType == ControllerType.GPIO)
            {
#if !SIMULATE
                ComController = new HHGPIOController(deviceConfig.IoType, deviceConfig.IoPort);
#else
                ComController = new HHEmuGPIOController(deviceConfig.IoType, deviceConfig.IoPort);
#endif
            }
            else if(deviceConfig.ControllerType == ControllerType.SerialPort)
            {
                ComController = new HHSerialPortController(deviceConfig.IoType, deviceConfig.ComPort, deviceConfig.Baudrate);
            }

            DeviceType = deviceConfig.DeviceType;
            DeviceGroup = deviceConfig.Group;
            Id = deviceConfig.Id;
            BitMask = (uint)deviceConfig.BitMask;
        }


        public virtual uint GetNewBitStateValue(uint newBitState) => BitMask & newBitState;

        public virtual void On() => BitState = GetNewBitStateValue(BitMask);

        public virtual void Off() => BitState = GetNewBitStateValue(~BitMask);

        public virtual bool Connect()
        {
            int attempt = 0;
            int elapsedTime = 0;
            int timeStart = Environment.TickCount;
            while (!IsConnected && attempt < 3)
            {
                try
                {
                    //Attempt device connection here
                    IsConnected = ComController.OpenPort();
                }
                catch
                {
                    Thread.Sleep(200);
                }
                attempt++;
            }

            if (IsConnected && attempt == 1) //Only log when there is attempt to connect, otherwise it already connect
            {
                elapsedTime = Environment.TickCount - timeStart;
                Debug.WriteLine(String.Format("{0} connected. Bit: 0x{1:D4}, Elapsed: {2}ms", Id, BitMask.ToString("X"), elapsedTime));
            }

            if (!IsConnected && attempt > 2)
            {
                elapsedTime = Environment.TickCount - timeStart;
                string exLog = String.Format("{0} Failed {1} attempt to connect. Bit: 0x{1:D4}, Elapsed: {2}ms", Id, attempt, BitMask.ToString("X"), elapsedTime);
                Debug.WriteLine(exLog);
                throw new Exception(exLog);
            }
            return IsConnected;
        }

        public virtual bool Update()
        {
            bool bSuccess = false;

            if (IsConnected)
            {
                //Update digital I/O
                {
                    ComController.SendDigitalOutputCommand(BitState);
                    IsOn = GetNewBitStateValue(BitState) == BitMask;
                }

                //Update analog I/O
                {
                    ComController.SendAnalogOutputCommand(AnalogData);
                }
                bSuccess = true;
            }
            Debug.WriteLine(String.Format("Unable to update, {0} is disconnected. Bit: 0x{1:D4}", Id, BitMask.ToString("X")));
            return bSuccess;
        }

    }
}
