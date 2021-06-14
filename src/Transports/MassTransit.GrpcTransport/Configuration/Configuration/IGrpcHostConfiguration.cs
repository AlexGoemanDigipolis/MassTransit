﻿namespace MassTransit.GrpcTransport.Configuration
{
    using System;
    using System.Collections.Generic;
    using Integration;
    using MassTransit.Configuration;
    using Topology.Topologies;


    public interface IGrpcHostConfiguration :
        IHostConfiguration,
        IReceiveConfigurator<IGrpcReceiveEndpointConfigurator>
    {
        /// <summary>
        /// Set the host's base address
        /// </summary>
        Uri BaseAddress { get; set; }

        IGrpcHostConfigurator Configurator { get; }

        IGrpcTransportProvider TransportProvider { get; }

        new IGrpcHostTopology HostTopology { get; }

        IEnumerable<GrpcServerConfiguration> ServerConfigurations { get; }

        void ApplyEndpointDefinition(IGrpcReceiveEndpointConfigurator configurator, IEndpointDefinition definition);

        IGrpcReceiveEndpointConfiguration CreateReceiveEndpointConfiguration(string queueName,
            Action<IGrpcReceiveEndpointConfigurator> configure = null);

        IGrpcReceiveEndpointConfiguration CreateReceiveEndpointConfiguration(string queueName, IGrpcEndpointConfiguration endpointConfiguration,
            Action<IGrpcReceiveEndpointConfigurator> configure = null);
    }
}
