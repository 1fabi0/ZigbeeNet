﻿using System;
using System.Collections.Generic;
using System.Text;
using ZigBeeNet.Hardware.CC.Util;

namespace ZigBeeNet.Hardware.CC.Packet.ZDO
{
    public class ZDO_UNBIND_RSP : ZToolPacket
    {
        /// <name>TI.ZPI1.ZDO_UNBIND_RSP.SrcAddress</name>
        /// <summary>the message's source network address</summary>
        public ZToolAddress16 SrcAddress { get; private set; }
        /// <name>TI.ZPI1.ZDO_UNBIND_RSP.Status</name>
        /// <summary>this field indicates status of the bind request</summary>
        public int Status { get; private set; }

        /// <name>TI.ZPI1.ZDO_UNBIND_RSP</name>
        /// <summary>Constructor</summary>
        public ZDO_UNBIND_RSP()
        {
        }

        public ZDO_UNBIND_RSP(byte[] framedata)
        {
            this.SrcAddress = new ZToolAddress16(framedata[1], framedata[0]);
            this.Status = framedata[2];
            BuildPacket(new DoubleByte(ZToolCMD.ZDO_UNBIND_RSP), framedata);
        }

        public override string ToString()
        {
            return "ZDO_UNBIND_RSP{" + "SrcAddress=" + SrcAddress + ", Status=" + Status + '}';
        }
    }
}
