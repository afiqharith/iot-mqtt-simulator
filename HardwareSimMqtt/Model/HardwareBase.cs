﻿#define SIMULATE
using System;
using System.Diagnostics;
using System.Threading;
using HardwareSimMqtt.Model.BitMap;
using HardwareSimMqtt.Interface;
using HardwareSimMqtt.HardwareHub;

namespace HardwareSimMqtt.Model
{
    public enum eHardwareType
    {
        LAMP,
        FAN,
        AIR_CONDITIONER,
        GATE,
    }

    public enum eGroup
    {
        Non = -1,
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

    public class HardwareBase : IHardware
    {
        private string _id;
        public virtual string Id
        {
            get => _id;
            protected set
            {
                string createdId = String.Empty;
                switch (Type)
                {
                    default:
                    case eHardwareType.LAMP:
                        createdId = String.Format("HWLID{0}", value);
                        break;

                    case eHardwareType.FAN:
                        createdId = String.Format("HWFID{0}", value);
                        break;

                    case eHardwareType.AIR_CONDITIONER:
                        createdId = String.Format("HWACID{0}", value);
                        break;

                    case eHardwareType.GATE:
                        createdId = String.Format("HWGID{0}", value);
                        break;
                }
                _id = createdId;
            }
        }

        public virtual eHardwareType Type
        {
            get;
            protected set;
        }

        public virtual eGroup Group
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

        // Bit index for the hardware, bit map
        private uint _bitMask = 0;
        public virtual uint BitMask
        {
            get => _bitMask;
            protected set
            {
                _bitMask = value;
                //Map with hardware bit
                ComController.BitMask = value;
            }
        }

        // Bit state of the hardware at the bit index
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
        public HardwareBase(string id, eBitMask mask, eHardwareType type, eGroup group, eIoType ioType, int ioPort)
        {
#if !SIMULATE
            ComController = new HHGPIOController(ioType, ioPort);
#else
            //ComController = new HHEmuGPIOController(ioType, ioPort);
            Program.CentralController.HardwareComMap.Add((uint)mask, new HardwareComm(this, new HHEmuGPIOController(ioType, ioPort)));
            ComController = Program.CentralController.GetComController((uint)mask);
#endif
            Type = type;
            Group = group;
            Id = id;
            BitMask = (uint)mask;


        }

        //Using SerialPort
        public HardwareBase(string id, eBitMask mask, eHardwareType type, eGroup group, eIoType ioType, string portName, int baudRate = 9600)
        {
            //ComController = new HHSerialPortController(ioType, portName, baudRate);
            Program.CentralController.HardwareComMap.Add((uint)mask, new HardwareComm(this, new HHSerialPortController(ioType, portName, baudRate)));
            ComController = Program.CentralController.GetComController((uint)mask);
            
            Id = id;
            BitMask = (uint)mask;
            Type = type;
            Group = group;
        }

        public virtual uint GetNewBitStateValue(uint newBitState) => BitMask & newBitState;

        public virtual void On() => BitState = GetNewBitStateValue(BitMask);

        public virtual void Off() => BitState = GetNewBitStateValue(~BitMask);

        public virtual bool Connect()
        {
            int iAttempt = 0;
            int elapsedTime = 0;
            int timeStart = Environment.TickCount;
            while (!IsConnected && iAttempt < 3)
            {
                try
                {
                    //Attempt hardware connection here
                    IsConnected = ComController.OpenPort();
                }
                catch
                {
                    Thread.Sleep(200);
                }
                iAttempt++;
            }

            if (IsConnected && iAttempt == 1) //Only log when there is attempt to connect, otherwise it already connect
            {
                elapsedTime = Environment.TickCount - timeStart;
                Debug.WriteLine(String.Format("{0} connected. Bit: 0x{1:D4}, Elapsed: {2}ms", Id, BitMask.ToString("X"), elapsedTime));
            }

            if (iAttempt > 2)
            {
                elapsedTime = Environment.TickCount - timeStart;
                string exLog = String.Format("{0} Failed {1} attempt to connect. Bit: 0x{1:D4}, Elapsed: {2}ms", Id, iAttempt, BitMask.ToString("X"), elapsedTime);
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
