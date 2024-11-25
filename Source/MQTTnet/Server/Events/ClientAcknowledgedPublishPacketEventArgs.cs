// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using MQTTnet.Packets;

namespace MQTTnet.Server
{
    public sealed class ClientAcknowledgedPublishPacketEventArgs : EventArgs
    {
        /// <summary>
        ///     Gets the packet which was used for acknowledge. This can be a PubAck or PubComp packet.
        /// </summary>
        public MqttPacketWithIdentifier AcknowledgePacket { get; internal set; }

        /// <summary>
        ///     Gets the ID of the client which acknowledged a PUBLISH packet.
        /// </summary>
        public string ClientId { get; internal set; }

        /// <summary>
        ///     Gets whether the PUBLISH packet is fully acknowledged. This is the case for PUBACK (QoS 1) and PUBCOMP (QoS 2.
        /// </summary>
        public bool IsCompleted => AcknowledgePacket is MqttPubAckPacket || AcknowledgePacket is MqttPubCompPacket;

        /// <summary>
        ///     Gets the PUBLISH packet which was acknowledged.
        /// </summary>
        public MqttPublishPacket PublishPacket { get; internal set; }

        /// <summary>
        ///     Gets the session items which contain custom user data per session.
        /// </summary>
        public IDictionary SessionItems { get; internal set; }
    }
}