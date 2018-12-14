﻿using System;
using System.Collections.Generic;
using System.Text;
using ZigBeeNet.Transaction;
using ZigBeeNet;
using ZigBeeNet.ZCL;
using ZigBeeNet.ZCL.Protocol;
using ZigBeeNet.ZDO;

namespace ZigBeeNet.ZDO.Command
{
    /**
     * Management Permit Joining Request value object class.
     * 
     * The Mgmt_Permit_Joining_req is generated from a Local Device requesting that
     * a remote device or devices allow or disallow association. The
     * Mgmt_Permit_Joining_req is generated by a management application or
     * commissioning tool which directs the request to a remote device(s) where the
     * NLME-PERMIT-JOINING.request is executed using the PermitDuration
     * parameter supplied by Mgmt_Permit_Joining_req. Additionally, if the remote
     * device is the Trust Center and TC_Significance is set to 1, the Trust Center
     * authentication policy will be affected. The addressing may be unicast or
     * "broadcast to all routers and coordinator".
     */
    public class ManagementPermitJoiningRequest : ZdoRequest, IZigBeeTransactionMatcher
    {
        /**
         * PermitDuration command message field.
         */
        public byte PermitDuration { get; set; }

        /**
         * TC_Significance command message field.
         */
        public bool TcSignificance { get; set; }

        /**
         * Default constructor.
         */
        public ManagementPermitJoiningRequest()
        {
            ClusterId = 0x0036;
        }

        public override void Serialize(ZclFieldSerializer serializer)
        {
            base.Serialize(serializer);

            serializer.Serialize((byte)PermitDuration, ZclDataType.Get(DataType.UNSIGNED_8_BIT_INTEGER));
            serializer.Serialize(TcSignificance, ZclDataType.Get(DataType.BOOLEAN));
        }

        public override void Deserialize(ZclFieldDeserializer deserializer)
        {
            base.Deserialize(deserializer);

            PermitDuration = (byte)deserializer.Deserialize(ZclDataType.Get(DataType.UNSIGNED_8_BIT_INTEGER));
            TcSignificance = (bool)deserializer.Deserialize(ZclDataType.Get(DataType.BOOLEAN));
        }

        public bool IsTransactionMatch(ZigBeeCommand request, ZigBeeCommand response)
        {
            if (!(response is ManagementPermitJoiningResponse)) {
                return false;
            }

            return ((ZdoRequest)request).DestinationAddress.Equals(((ManagementPermitJoiningResponse)response).SourceAddress);
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            builder.Append("ManagementPermitJoiningRequest [")
                   .Append(base.ToString())
                   .Append(", permitDuration=")
                   .Append(PermitDuration)
                   .Append(", tcSignificance=")
                   .Append(TcSignificance)
                   .Append(']');

            return builder.ToString();
        }

    }
}
