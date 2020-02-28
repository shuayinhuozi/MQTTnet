﻿using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using MQTTnet.Core.Adapter;
using MQTTnet.Core.Exceptions;
using MQTTnet.Core.Packets;
using MQTTnet.Core.Protocol;
using Microsoft.Extensions.Logging;

namespace MQTTnet.Core.Server
{
    public sealed class MqttClientPendingMessagesQueue
    {
        private readonly BlockingCollection<MqttPublishPacket> _pendingPublishPackets = new BlockingCollection<MqttPublishPacket>();
        private readonly MqttClientSession _session;
        private readonly MqttServerOptions _options;
        private readonly ILogger<MqttClientPendingMessagesQueue> _logger;

        public MqttClientPendingMessagesQueue(MqttServerOptions options, MqttClientSession session, ILogger<MqttClientPendingMessagesQueue> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _session = session ?? throw new ArgumentNullException(nameof(session));
            _options = options ?? throw new ArgumentNullException(nameof(options));
        }

        public void Start(IMqttCommunicationAdapter adapter, CancellationToken cancellationToken)
        {
            if (adapter == null) throw new ArgumentNullException(nameof(adapter));

            Task.Run(() => SendPendingPublishPacketsAsync(adapter, cancellationToken), cancellationToken);
        }

        public void Enqueue(MqttPublishPacket publishPacket)
        {
            if (publishPacket == null) throw new ArgumentNullException(nameof(publishPacket));

            _pendingPublishPackets.Add(publishPacket);
        }

        private async Task SendPendingPublishPacketsAsync(IMqttCommunicationAdapter adapter, CancellationToken cancellationToken)
        {
            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    await SendPendingPublishPacketAsync(adapter, cancellationToken);
                }
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception exception)
            {
                _logger.LogError(new EventId(), exception, "Unhandled exception while sending pending publish packets.");
            }
        }

        private async Task SendPendingPublishPacketAsync(IMqttCommunicationAdapter adapter, CancellationToken cancellationToken)
        {
            var packet = _pendingPublishPackets.Take(cancellationToken);

            try
            {
                await adapter.SendPacketsAsync(_options.DefaultCommunicationTimeout, cancellationToken, packet).ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                if (exception is MqttCommunicationTimedOutException)
                {
                    _logger.LogWarning(new EventId(), exception, "Sending publish packet failed due to timeout.");
                }
                else if (exception is MqttCommunicationException)
                {
                    _logger.LogWarning(new EventId(), exception, "Sending publish packet failed due to communication exception.");
                }
                if (exception is OperationCanceledException)
                {
                }
                else
                {
                    _logger.LogError(new EventId(), exception, "Sending publish packet failed.");
                }

                if (packet.QualityOfServiceLevel > MqttQualityOfServiceLevel.AtMostOnce)
                {
                    packet.Dup = true;
                    _pendingPublishPackets.Add(packet, cancellationToken);
                }

                _session.Stop();
            }
        }
    }
}
