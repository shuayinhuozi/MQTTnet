﻿using MQTTnet.Core.Adapter;

namespace MQTTnet.Core.Client
{
    public interface IMqttCommunicationAdapterFactory
    {
        IMqttCommunicationAdapter CreateMqttCommunicationAdapter(MqttClientOptions options);
    }
}