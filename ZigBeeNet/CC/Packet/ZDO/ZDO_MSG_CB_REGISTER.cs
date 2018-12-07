﻿using System;
using System.Collections.Generic;
using System.Text;

namespace ZigBeeNet.CC.Packet.ZDO
{
    public class ZDO_MSG_CB_REGISTER : ZToolPacket
    {
        public ZDO_MSG_CB_REGISTER(DoubleByte cluster)
        {
            byte[] framedata = new byte[2];
            framedata[0] = cluster.Lsb;
            framedata[1] = cluster.Msb;

            BuildPacket(new DoubleByte(ZToolCMD.ZDO_MSG_CB_REGISTER), framedata);
        }
}
}
