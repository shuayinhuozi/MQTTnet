// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Connections;

#if NETCOREAPP3_1
using System;
#endif

namespace MQTTnet.AspNetCore
{
    public static class ConnectionRouteBuilderExtensions
    {
#if NETCOREAPP3_1
        [Obsolete("This class is obsolete and will be removed in a future version. The recommended alternative is to use MapMqtt inside Microsoft.AspNetCore.Builder.UseEndpoints(...).")]
#endif
#if NETCOREAPP3_1 || NETCOREAPP2_1 || NETSTANDARD
        public static void MapMqtt(this ConnectionsRouteBuilder connection, PathString path)
        {
            connection.MapConnectionHandler<MqttConnectionHandler>(path, options =>
            {
                options.WebSockets.SubProtocolSelector = MqttSubProtocolSelector.SelectSubProtocol;
            });
        }
#endif
    }
}
