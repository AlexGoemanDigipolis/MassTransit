namespace MassTransit
{
    using System;
    using Configuration;
    using GreenPipes;


    public interface IBusFactory :
        ISpecification
    {
        /// <summary>
        /// Create the bus endpoint configuration, which is used to create the bus
        /// </summary>
        /// <returns></returns>
        IReceiveEndpointConfiguration CreateBusEndpointConfiguration(Action<IReceiveEndpointConfigurator> configure);
    }
}
