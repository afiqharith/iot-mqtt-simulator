# SmartHouse control using MQTT for IoT implementation

This project is a simulation on IoT remote control for SmartHouse. Implemented MQTT to broadcast the bit state to the hardware. Future planning to include physical hardware.

</br>

### Bit mask of the objects

| Hardware ID | Bit7 | Bit6 | Bit5 | Bit4 | Bit3 | Bit2 | Bit1 | Bit0 |
| ----------- | ---- | ---- | ---- | ---- | ---- | ---- | ---- | ---- |
| Fan ID1     | 0    | 0    | 0    | 0    | 0    | 0    | 0    | 1    |
| Fan ID1     | 0    | 0    | 0    | 0    | 0    | 0    | 1    | 0    |
| Fan ID1     | 0    | 0    | 0    | 0    | 0    | 1    | 0    | 0    |
| Fan ID1     | 0    | 0    | 0    | 0    | 1    | 0    | 0    | 0    |
| Lamp ID1    | 0    | 0    | 0    | 1    | 0    | 0    | 0    | 0    |
| Lamp ID2    | 0    | 0    | 1    | 0    | 0    | 0    | 0    | 0    |
| Lamp ID3    | 0    | 1    | 0    | 0    | 0    | 0    | 0    | 0    |
| Lamp ID4    | 1    | 0    | 0    | 0    | 0    | 0    | 0    | 0    |

</br>

### Application window

| ![outputimage](/images/Controller_Window.png) | ![outputimage](/images/Overview_Window.png) |
| --------------------------------------------- | ------------------------------------------- |

_\*\*Only for proof of concept_
