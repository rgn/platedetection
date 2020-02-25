using Confluent.Kafka;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Text;

namespace Trivadis.PlateDetection.Broker
{
    public class JsonSerializer<TValue> : IAsyncSerializer<TValue>
    {
        public async Task<byte[]> SerializeAsync(TValue data, SerializationContext context)
        {
            return Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(data));
        }
    }
}
