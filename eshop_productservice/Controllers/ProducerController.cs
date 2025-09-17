using Confluent.Kafka;
using Microsoft.AspNetCore.Mvc;

namespace eshop_productservice.Controllers;

[ApiController]
[Route("api/productservice/v1/[controller]")]
public class ProducerController(IConfiguration configuration, ILogger<ProducerController> logger) : ControllerBase
{
    private readonly string _server = configuration.GetValue<string>("Kafka:Server") ?? throw new InvalidOperationException();

    private const string Topic = "test-topic";

    private ProducerConfig GetConfig()
    {
        return new ProducerConfig
        {
            BootstrapServers = _server,
        };
    }

    [HttpPost("send")]
    public async Task<IActionResult> SendMessage([FromBody] string message)
    {
        logger.LogInformation($"Kafka server: ${_server}");
        
        using var producer = new ProducerBuilder<Null, string>(GetConfig()).Build();
        var result = await producer.ProduceAsync(Topic, new Message<Null, string> { Value = message });
        return Ok($"Sent: {message} to partition {result.Partition}");
    }
}