using Confluent.Kafka;
using Microsoft.Extensions.Options;
using Serilog;

namespace Trivadis.PlateDetection.Broker
{
    public class KeylessBrokerProducerService<T> : BrokerProducerService<Null, T>
    {
        public KeylessBrokerProducerService(IOptions<BrokerProducerServiceOptions> options, ILogger logger) : base(options, logger)
        { }
    }
}
