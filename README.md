# SmartHouse control using MQTT for IoT implementation

This project is a simulation on IoT remote control for SmartHouse. Implemented MQTT to broadcast the bit state to the hardware. Future planning to include physical hardware.

</br>

### Bit mask of the objects

| Hardware ID | Broadcom Pin | Bit7 | Bit6 | Bit5 | Bit4 | Bit3 | Bit2 | Bit1 | Bit0 |
| ----------- | ------------ | ---- | ---- | ---- | ---- | ---- | ---- | ---- | ---- |
| Fan ID1     | 4            | 0    | 0    | 0    | 0    | 0    | 0    | 0    | 1    |
| Fan ID2     | 17           | 0    | 0    | 0    | 0    | 0    | 0    | 1    | 0    |
| Fan ID3     | 18           | 0    | 0    | 0    | 0    | 0    | 1    | 0    | 0    |
| Fan ID4     | 21           | 0    | 0    | 0    | 0    | 1    | 0    | 0    | 0    |
| Lamp ID1    | 22           | 0    | 0    | 0    | 1    | 0    | 0    | 0    | 0    |
| Lamp ID2    | 23           | 0    | 0    | 1    | 0    | 0    | 0    | 0    | 0    |
| Lamp ID3    | 24           | 0    | 1    | 0    | 0    | 0    | 0    | 0    | 0    |
| Lamp ID4    | 25           | 1    | 0    | 0    | 0    | 0    | 0    | 0    | 0    |

</br>

### Application window

| ![outputimage](/images/Controller.png) | ![outputimage](/images/Listener.png) | ![outputimage](/images/GPIO_Simulator.png) |
| -------------------------------------- | ------------------------------------ | ------------------------------------------ |

For physical IO simulation, [PiSharp](https://github.com/andycb/PiSharp) is being used. This is to simulate the output value via GPIO.

_\*\*Only for proof of concept_
