using Confluent.Kafka;
using eshop_productservice.Enums;
using eshop_productservice.Interfaces;

namespace eshop_productservice.Services;

public class ConsumerService(IConfiguration configuration, IServiceScopeFactory factory) : BackgroundService
{
    private const int MaxAttempts = 12;
    private const int AttemptDelayInSeconds = 5;
    private const KafkaTopic TopicName = KafkaTopic.OrderCreated;

    private readonly string _groupId =
        configuration.GetValue<string>("Kafka:GroupId") ?? throw new InvalidOperationException();

    private readonly string _server =
        configuration.GetValue<string>("Kafka:Server") ?? throw new InvalidOperationException();

    private ConsumerConfig GetConfig()
    {
        return new ConsumerConfig
        {
            BootstrapServers = _server,
            GroupId = _groupId,
            AutoOffsetReset = AutoOffsetReset.Earliest
        };
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await WaitForTopic(stoppingToken);

        using var consumer = new ConsumerBuilder<Ignore, string>(GetConfig()).Build();
        consumer.Subscribe(TopicName.ToString());

        await Task.Factory.StartNew(() => ConsumeLoop(consumer, stoppingToken), TaskCreationOptions.LongRunning);
    }

    private async Task ConsumeLoop(IConsumer<Ignore, string> consumer, CancellationToken stoppingToken)
    {
        var attempts = 0;
        while (!stoppingToken.IsCancellationRequested)
            try
            {
                var result = consumer.Consume(stoppingToken);
                Console.WriteLine($"Consumed: {result.Message.Value}");
                Console.WriteLine($"Consumed from topic: {result.Topic}");
                await ProcessMessage(result.Topic, result.Message.Value);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (ConsumeException ex) when (ex.Error.Code == ErrorCode.UnknownTopicOrPart)
            {
                attempts++;
                if (attempts > MaxAttempts) throw;

                Console.WriteLine("Topic not available, retrying...");
                await Task.Delay(AttemptDelayInSeconds * 1000, stoppingToken);
            }
    }

    private async Task WaitForTopic(CancellationToken cancellationToken)
    {
        using var admin = new AdminClientBuilder(new AdminClientConfig { BootstrapServers = _server }).Build();
        var attempts = 0;

        while (!cancellationToken.IsCancellationRequested && attempts < MaxAttempts)
        {
            try
            {
                var metadata = admin.GetMetadata(TopicName.ToString(), TimeSpan.FromSeconds(AttemptDelayInSeconds));
                if (metadata.Topics[0].Error.Code == ErrorCode.NoError)
                    return;
            }
            catch
            {
                // ignored
            }

            attempts++;
            Console.WriteLine($"Waiting for topic... ({attempts}/{MaxAttempts})");
            await Task.Delay(AttemptDelayInSeconds * 1000, cancellationToken);
        }

        throw new TimeoutException(
            $"Topic '{TopicName}' not available after {MaxAttempts * AttemptDelayInSeconds / 60} minute(s)");
    }

    private async Task ProcessMessage(string topic, string data)
    {
        try
        {
            var kafkaTopic = Enum.Parse<KafkaTopic>(topic);
            switch (kafkaTopic)
            {
                case KafkaTopic.OrderCreated:
                    // orderListener.NewOrderCreated(message);
                    using (var scope = factory.CreateScope())
                    {
                        var orderListener = scope.ServiceProvider.GetRequiredService<IOrderListener>();
                        await orderListener.NewOrderCreated(data);
                    }

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error parsing topic to enum, processing message: {e.Message}");
        }
    }
}