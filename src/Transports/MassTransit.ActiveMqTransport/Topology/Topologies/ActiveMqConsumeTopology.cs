namespace MassTransit.ActiveMqTransport.Topology.Topologies
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Builders;
    using GreenPipes;
    using MassTransit.Topology;
    using MassTransit.Topology.Topologies;
    using Specifications;


    public class ActiveMqConsumeTopology :
        ConsumeTopology,
        IActiveMqConsumeTopologyConfigurator
    {
        readonly IMessageTopology _messageTopology;
        readonly IActiveMqPublishTopology _publishTopology;
        readonly IList<IActiveMqConsumeTopologySpecification> _specifications;
        IActiveMqConsumerEndpointQueueNameFormatter _consumerEndpointQueueNameFormatter;
        
        public ActiveMqConsumeTopology(IMessageTopology messageTopology, IActiveMqPublishTopology publishTopology,IActiveMqConsumerEndpointQueueNameFormatter consumerEndpointQueueNameFormatter)
        {
            _messageTopology = messageTopology;
            _publishTopology = publishTopology;

            _specifications = new List<IActiveMqConsumeTopologySpecification>();
            _consumerEndpointQueueNameFormatter = consumerEndpointQueueNameFormatter;
        }
        public IActiveMqConsumerEndpointQueueNameFormatter ConsumerEndpointQueueNameFormatter
        {
            get => _consumerEndpointQueueNameFormatter;
            set => _consumerEndpointQueueNameFormatter = value;
        }

        IActiveMqMessageConsumeTopology<T> IActiveMqConsumeTopology.GetMessageTopology<T>()
        {
            return base.GetMessageTopology<T>() as IActiveMqMessageConsumeTopologyConfigurator<T>;
        }

        public void AddSpecification(IActiveMqConsumeTopologySpecification specification)
        {
            if (specification == null)
                throw new ArgumentNullException(nameof(specification));

            _specifications.Add(specification);
        }

        IActiveMqMessageConsumeTopologyConfigurator<T> IActiveMqConsumeTopologyConfigurator.GetMessageTopology<T>()
        {
            return base.GetMessageTopology<T>() as IActiveMqMessageConsumeTopologyConfigurator<T>;
        }

        public void Apply(IReceiveEndpointBrokerTopologyBuilder builder)
        {
            foreach (var specification in _specifications)
                specification.Apply(builder);

            ForEach<IActiveMqMessageConsumeTopologyConfigurator>(x => x.Apply(builder));
        }

        public void Bind(string topicName, Action<ITopicBindingConfigurator> configure = null)
        {
            if (string.IsNullOrEmpty(_publishTopology.VirtualTopicPrefix) || topicName.StartsWith(_publishTopology.VirtualTopicPrefix))
            {
                var specification = new ConsumerConsumeTopologySpecification(topicName, _consumerEndpointQueueNameFormatter);

                configure?.Invoke(specification);

                _specifications.Add(specification);
            }
            else
                _specifications.Add(new InvalidActiveMqConsumeTopologySpecification("Bind", $"Only virtual topics can be bound: {topicName}"));
        }

        public override string CreateTemporaryQueueName(string tag)
        {
            var result = base.CreateTemporaryQueueName(tag);
            return new string(result.Where(c => c != '.').ToArray());
        }

        public override IEnumerable<ValidationResult> Validate()
        {
            return base.Validate().Concat(_specifications.SelectMany(x => x.Validate()));
        }

        protected override IMessageConsumeTopologyConfigurator CreateMessageTopology<T>(Type type)
        {
            var messageTopology = new ActiveMqMessageConsumeTopology<T>(_publishTopology.GetMessageTopology<T>(), _consumerEndpointQueueNameFormatter);

            OnMessageTopologyCreated(messageTopology);

            return messageTopology;
        }
    }
}
