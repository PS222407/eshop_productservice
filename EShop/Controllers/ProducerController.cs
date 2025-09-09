using Confluent.Kafka;
using Microsoft.AspNetCore.Mvc;

namespace EShop.Controllers;

public class ProducerController : Controller
{
    private readonly ProducerConfig _config = new()
    {
        BootstrapServers = "localhost:9094"
    };

    [HttpPost("send")]
    public async Task<IActionResult> SendMessage([FromBody] string message)
    {
        using var producer = new ProducerBuilder<Null, string>(_config).Build();
        var result = await producer.ProduceAsync("test-topic", new Message<Null, string> { Value = message });
        return Ok($"Sent: {message} to partition {result.Partition}");
    }
}