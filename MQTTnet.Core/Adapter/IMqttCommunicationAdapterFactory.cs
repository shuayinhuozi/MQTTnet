﻿using MQTTnet.Core.Client;
using MQTTnet.Core.Channel;

namespace MQTTnet.Core.Adapter
{
    public interface IMqttCommunicationAdapterFactory
    {
        IMqttCommunicationAdapter CreateClientMqttCommunicationAdapter(IMqttClientOptions options);

        IMqttCommunicationAdapter CreateServerMqttCommunicationAdapter(IMqttCommunicationChannel channel);
    }
}