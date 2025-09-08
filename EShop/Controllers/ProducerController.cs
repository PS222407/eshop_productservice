using Confluent.Kafka;
using Microsoft.AspNetCore.Mvc;

namespace EShop.Controllers;

public class ProducerController : Controller
{
    private readonly ProducerConfig _config = new()
    {
        BootstrapServers = "kafka:9092"
    };
    
    [HttpPost("send")]
    public async Task<IActionResult> SendMessage([FromBody] string message)
    {
        using IProducer<Null, string>? producer = new ProducerBuilder<Null, string>(_config).Build();
        DeliveryResult<Null, string>? result = await producer.ProduceAsync("test-topic", new Message<Null, string> { Value = message });
        return Ok($"Sent: {message} to partition {result.Partition}");
    }
}