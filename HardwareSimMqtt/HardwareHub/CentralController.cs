using HardwareSimMqtt.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HardwareSimMqtt.HardwareHub
{
    public class HardwareComm
    {
        public HardwareBase Hardware
        {
            get;
            set;
        }

        public IComController ComController
        {
            get;
            set;
        }

        public HardwareComm(HardwareBase hardware, IComController comController)
        {
            Hardware = hardware;
            ComController = comController;
        }
    }

    public class CentralController
    {
        public Dictionary<uint, HardwareComm> HardwareComMap
        {
            get;
            private set;
        }

        public CentralController() 
        {
            HardwareComMap = new Dictionary<uint, HardwareComm>();
        }

        public CentralController(Dictionary<uint, HardwareComm> hardwareComMap)
        {
            HardwareComMap = hardwareComMap;
        }

        public IComController GetComController(uint bitmask)
        {
            if (!HardwareComMap.ContainsKey(bitmask) || HardwareComMap[bitmask] == null)
            {
                return null;
            }
            return HardwareComMap[bitmask].ComController;
        }

        public int SetBitState(uint bitmask, uint requestBitState)
        {
            if (!HardwareComMap.ContainsKey(bitmask) || HardwareComMap[bitmask] == null)
            {
                return 0;
            }

            HardwareComMap[bitmask].Hardware.BitState = HardwareComMap[bitmask].Hardware.BitMask & requestBitState;
            return 1;
        }

        public int SetBitState(string id, uint requestBitState)
        {
            bool bFound = false;
            foreach (KeyValuePair<uint, HardwareComm> kvp in HardwareComMap)
            {
                if (kvp.Value.Hardware.Id == id)
                {
                    bFound = true;
                    kvp.Value.Hardware.BitState = kvp.Value.Hardware.BitMask & requestBitState;
                    break;
                }
            }
            return bFound ? 1 : 0;
        }

        public string GetHardwareId(uint bitmask)
        {
            if (!HardwareComMap.ContainsKey(bitmask) || HardwareComMap[bitmask] == null)
            {
                return null;
            }
            return HardwareComMap[bitmask].Hardware.Id;
        }

        public uint GetHardwareBitMask(string id)
        {
            uint mask = 0;
            foreach (KeyValuePair<uint, HardwareComm> kvp in HardwareComMap)
            {
                if (kvp.Value.Hardware.Id == id)
                {
                    mask = kvp.Key;
                    break;
                }
            }
            return mask;
        }

        public uint GetNewBitStateValue(uint bitmask, uint requestBitState)
        {
            if (!HardwareComMap.ContainsKey(bitmask) || HardwareComMap[bitmask] == null)
            {
                return 0;
            }
            uint hwBitmask = HardwareComMap[bitmask].Hardware.BitMask;
            return hwBitmask & requestBitState;
        }
    }
}
