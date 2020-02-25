namespace Trivadis.PlateDetection.Broker
{
    public class BrokerConsumerServiceOptions
    {
        public string Host { get; set; }        
        public string ConsumerGroup { get; set; }
        public string DefaultTopic { get; set; }
    }
}
