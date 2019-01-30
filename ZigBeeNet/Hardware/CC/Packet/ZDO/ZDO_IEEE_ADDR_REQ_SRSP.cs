﻿using System;
using System.Collections.Generic;
using System.Text;

namespace ZigBeeNet.Hardware.CC.Packet.ZDO
{
    public class ZDO_IEEE_ADDR_REQ_SRSP : ZToolPacket
    {
        public PacketStatus Status { get; private set; }

        public ZDO_IEEE_ADDR_REQ_SRSP(byte[] data)
        {
            Status = (PacketStatus)data[0];

            BuildPacket(new DoubleByte(ZToolCMD.ZDO_IEEE_ADDR_REQ_SRSP), data);
        }
    }
}
