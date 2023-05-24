﻿using System;
using System.Threading.Tasks;

namespace MQTTnet
{
    public sealed class MqttApplicationMessageReceivedEventArgs : EventArgs
    {
        public MqttApplicationMessageReceivedEventArgs(string clientId, MqttApplicationMessage applicationMessage)
        {
            ClientId = clientId;
            ApplicationMessage = applicationMessage ?? throw new ArgumentNullException(nameof(applicationMessage));
        }

        public string ClientId { get; }

        public MqttApplicationMessage ApplicationMessage { get; }

        public bool ProcessingFailed { get; set; }

        public Task<bool> PendingTask { get; set; }
    }
}
