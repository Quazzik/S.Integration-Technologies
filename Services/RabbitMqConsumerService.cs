using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using S.Integration_Technologies.Models;
using S.Integration_Technologies.Services;

namespace S.Integration_Technologies.Services;

public class RabbitMqConsumerService : IHostedService
{
    private readonly ConnectionFactory _connectionFactory;
    private readonly ArticleSearchService _searchService;
    
    private IConnection? _connection;
    private IChannel? _channel;

    public RabbitMqConsumerService(
        ConnectionFactory connectionFactory,
        ArticleSearchService searchService)
    {
        _connectionFactory = connectionFactory;
        _searchService = searchService;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _connection = await _connectionFactory.CreateConnectionAsync();
        _channel = await _connection.CreateChannelAsync();

        await _channel.QueueDeclareAsync(
            queue: "demo-queue",
            durable: false,
            exclusive: false,
            autoDelete: false,
            arguments: null
        );

        var consumer = new AsyncEventingBasicConsumer(_channel);
        
        consumer.ReceivedAsync += async (model, ea) =>
        {
            // 1. Получаем сообщение
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            
            // 2. Десериализуем в ArticleDocument
            var document = JsonSerializer.Deserialize<ArticleDocument>(message);
            
            // 3. Отправляем в ElasticSearch
            if (document != null)
            {
                await _searchService.IndexAsync(new[] { document });
                await _channel.BasicAckAsync(ea.DeliveryTag, false);
            }
        };

        await _channel.BasicConsumeAsync(
            queue: "demo-queue",
            autoAck: false,
            consumer: consumer
        );
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_channel?.IsOpen == true)
            await _channel.CloseAsync();
        if (_connection?.IsOpen == true)
            await _connection.CloseAsync();
    }
}