﻿using System;
using System.Collections.Generic;
using System.Text;

namespace ZigBeeNet.CC.Util
{
    /**
     * Represents a double byte XBeeApi Address.
     */
    public abstract class ZToolAddress
    {
        public abstract byte[] Address { get; protected set; }

        public override string ToString()
        {
            return ByteUtils.ToBase16(Address);
        }
    }
}
