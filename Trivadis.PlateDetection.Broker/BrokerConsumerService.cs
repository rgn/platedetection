using Confluent.Kafka;
using Microsoft.Extensions.Options;
using Serilog;
using System;
using System.Threading;

namespace Trivadis.PlateDetection.Broker
{
    public class BrokerConsumerService<TKey, TValue>
    {
        private readonly string defaultTopic;
        private readonly ILogger logger;
        private readonly ConsumerConfig config;        
        private readonly IConsumer<TKey, TValue> consumer;

        public BrokerConsumerService(IOptions<BrokerConsumerServiceOptions> options, ILogger logger)
        {
            defaultTopic = options.Value.DefaultTopic;
            this.logger = logger;

            config = new ConsumerConfig
            {
                GroupId = options.Value.ConsumerGroup,
                BootstrapServers = options.Value.Host,
                // Note: The AutoOffsetReset property determines the start offset in the event
                // there are not yet any committed offsets for the consumer group for the
                // topic/partitions of interest. By default, offsets are committed
                // automatically, so in this example, consumption will only start from the
                // earliest message in the topic 'my-topic' the first time you run the program.
                AutoOffsetReset = AutoOffsetReset.Earliest
            };

            consumer = new ConsumerBuilder<TKey, TValue>(config).Build();
        }

        public void Start(CancellationToken token, Action<object> consumedCallback, string topic = "")
        {
            if (string.IsNullOrEmpty(topic)) topic = defaultTopic;

            consumer.Subscribe(topic);

            try
            {
                while (true)
                {
                    try
                    {
                        var cr = consumer.Consume(token);

                        logger.Debug($"Consumed message '{cr.Value}' at: '{cr.TopicPartitionOffset}'.");

                        consumedCallback?.Invoke(cr.Value);

                    }
                    catch (ConsumeException e)
                    {
                        Console.WriteLine($"Error occured: {e.Error.Reason}");
                    }
                }
            }
            catch (OperationCanceledException)
            {
                // Ensure the consumer leaves the group cleanly and final offsets are committed.
                consumer.Close();
            }
        }
    }
}
