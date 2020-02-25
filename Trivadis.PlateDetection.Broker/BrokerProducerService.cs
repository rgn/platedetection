using Confluent.Kafka;
using Microsoft.Extensions.Options;
using Serilog;
using System;

namespace Trivadis.PlateDetection.Broker
{
    public class BrokerProducerService<TKey, TValue>
    {
        private readonly string defaultTopic;
        private readonly ILogger logger;
        private readonly ProducerConfig config;
        private readonly IProducer<TKey, TValue> producer;        

        public BrokerProducerService(IOptions<BrokerProducerServiceOptions> options, ILogger logger)
        {
            defaultTopic = options.Value.DefaultTopic;
            this.logger = logger;
            config = new ProducerConfig
            {
                BootstrapServers = options.Value.Host

            };

            // If serializers are not specified, default serializers from
            // `Confluent.Kafka.Serializers` will be automatically used where
            // available. Note: by default strings are encoded as UTF8.
            try
            {
                producer = new ProducerBuilder<TKey, TValue>(config)
                    .SetValueSerializer(new JsonSerializer<TValue>())
                    .Build();
            }
            catch(Exception ex)
            {
                logger.Error(ex, "Failed to create producer.");
            }
        }

        public void Send(TKey key, TValue value, string topic = "")
        {
            if (string.IsNullOrEmpty(topic)) topic = defaultTopic;

            try
            {                
                producer.Produce(topic, new Message<TKey, TValue> { Key = key, Value = value }, (dr) =>
                {                      
                    logger.Debug($"Delivered '{dr.Value}' to '{dr.TopicPartitionOffset}'");                    
                });                
            }
            catch (ProduceException<Null, string> e)
            {
                logger.Error($"Delivery failed: {e.Error.Reason}");
            }
        }

        public async void SendAsync(TKey key, TValue value, string topic = "")
        {
            if (string.IsNullOrEmpty(topic)) topic = defaultTopic;

            try
            {
                var dr = await producer.ProduceAsync(topic, new Message<TKey, TValue> { Key = key, Value = value });
                logger.Debug($"Delivered '{dr.Value}' to '{dr.TopicPartitionOffset}'");
            }
            catch (ProduceException<Null, string> e)
            {
                logger.Error($"Delivery failed: {e.Error.Reason}");
            }
        }
    }
}
