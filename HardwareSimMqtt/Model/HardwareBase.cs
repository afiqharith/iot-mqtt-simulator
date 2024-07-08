﻿using System;
using System.Diagnostics;
using System.Threading;
using HardwareSimMqtt.Model.BitMap;
using HardwareSimMqtt.Interface;
using HardwareSimMqtt.HardwareHub;

namespace HardwareSimMqtt.Model
{
    public enum eType
    {
        LAMP,
        FAN,
        AIR_CONDITIONER,
        GATE,
    }

    public enum eLocation
    {
        Area1,
        Area2,
        Area3,
        Area4,

        Loc1 = Area1,
        Loc2,
        Loc3,
        Loc4
    }

    public class HardwareBase : IHardware
    {
        private string _id;
        public virtual string Id
        {
            get => _id;
            protected set => SetIdProperty(ref _id, value);
        }

        public virtual eType Type
        {
            get;
            protected set;
        }

        public virtual eLocation Location
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

        public virtual bool IsOff
        {
            get;
            private set;
        }

        // Bit index for the hardware, bit map
        private uint _bitMask = 0;
        public virtual uint BitMask
        {
            get => _bitMask;
            protected set => SetBitMaskProperty(ref _bitMask, value);
        }

        // Bit state of the hardware at the bit index
        private uint _bitState = 0;
        public virtual uint BitState
        {
            get => _bitState;
            set => SetCurrentBitStateProperty(ref _bitState, value);
        }

        private double _analogData = -1;
        public virtual double AnalogData
        {
            get => _analogData;
            set => SetAnalogDataProperty(ref _analogData, value);
        }

        private HHInterface HHController 
        { 
            get; 
            set; 
        }

        //Using GPIO
        public HardwareBase(eType type, eLocation location, string id, eBitMask mask, int ioPort)
        {
            this.HHController = new HHGeneralPurposeIO(ioPort);
            this.Type = type;
            this.Location = location;
            this.Id = id;
            this.BitMask = (uint)mask;

        }

        //Using SerialPort
        public HardwareBase(eType type, eLocation location, string id, eBitMask mask, string portName, int baudRate = 9600)
        {
            this.HHController = new HHSerialPort(portName, baudRate);
            this.Type = type;
            this.Location = location;
            this.Id = id;
            this.BitMask = (uint)mask;

        }

        private void SetIdProperty(ref string id, string newval)
        {
            string createdId = String.Empty;
            switch (this.Type)
            {
                default:
                case eType.LAMP:
                    createdId = String.Format("HWLID{0}", newval);
                    break;

                case eType.FAN:
                    createdId = String.Format("HWFID{0}", newval);
                    break;

                case eType.AIR_CONDITIONER:
                    createdId = String.Format("HWACID{0}", newval);
                    break;

                case eType.GATE:
                    createdId = String.Format("HWGID{0}", newval);
                    break;
            }
            id = createdId;
        }

        private void SetBitMaskProperty(ref uint bitMask, uint newval)
        {
            bitMask = newval;
            //Map with hardware bit
            this.HHController.BitMask = newval;
        }

        private void SetCurrentBitStateProperty(ref uint bitState, uint newval)
        {
            bitState = newval;
            this.HHController.SendDigitalCommand(this.BitState);
            this.IsOn = GetNewBitStateValue(this.BitState) == this.BitMask;
            this.IsOff = !this.IsOn;
        }

        private void SetAnalogDataProperty(ref double data, double newval)
        {
            data = newval;
        }

        public virtual uint GetNewBitStateValue(uint newBitState) => this.BitMask & newBitState;

        public virtual void On() => this.BitState = GetNewBitStateValue(this.BitMask);

        public virtual void Off() => this.BitState = GetNewBitStateValue(~this.BitMask);

        public virtual bool Connect()
        {
            int iAttempt = 0;
            int elapsedTime = 0;
            int timeStart = Environment.TickCount;
            while (!this.IsConnected && iAttempt < 3)
            {
                try
                {
                    //Attempt hardware connection here
                    this.IsConnected = this.HHController.OpenPort();
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
                Debug.WriteLine(String.Format("{0} connected. Bit: 0x{1:D4}, Elapsed: {2}ms", this.Id, this.BitMask.ToString("X"), elapsedTime));
            }

            if (iAttempt > 2)
            {
                elapsedTime = Environment.TickCount - timeStart;
                string exLog = String.Format("{0} Failed {1} attempt to connect. Bit: 0x{1:D4}, Elapsed: {2}ms", this.Id, iAttempt, this.BitMask.ToString("X"), elapsedTime);
                Debug.WriteLine(exLog);
                throw new Exception(exLog);
            }
            return IsConnected;
        }

    }
}
