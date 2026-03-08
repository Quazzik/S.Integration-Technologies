using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using S.Integration_Technologies.Models;

namespace S.Integration_Technologies.Services;

public class RabbitMqProducerService(ConnectionFactory connectionFactory)
{
    private IConnection? _connection;
    private IChannel? _channel;

    private async Task InitializeAsync()
    {
        _connection ??= await connectionFactory.CreateConnectionAsync();

        // Создаём канал (logical connection) - все операции выполняются через него
        _channel = await _connection.CreateChannelAsync();

        // ⚠️ Объявление очереди
        // В RabbitMQ producer обязан гарантировать, что очередь существует.
        // QueueDeclare - идемпотентная операция:
        // если очередь уже есть с теми же параметрами, ничего не произойдёт
        await _channel.QueueDeclareAsync(
            queue: "demo-queue",

            // durable: true  → очередь переживёт рестарт брокера
            // durable: false → очередь будет удалена при перезапуске RabbitMQ
            durable: false,

            // exclusive: true  → очередь доступна только этому соединению
            // exclusive: false → очередь доступна другим подключениям
            exclusive: false,

            // autoDelete: true  → очередь будет удалена,
            //                    когда отключится последний consumer
            // autoDelete: false → очередь живёт до явного удаления
            autoDelete: false,

            arguments: null
        );
    }

    public async Task SendAsync(ArticleDocument document)
    {
        
        var message = JsonSerializer.Serialize(document);
        if (_channel is null)
        {
            await InitializeAsync();
        }

        var body = Encoding.UTF8.GetBytes(message);

        // Отправка сообщения в default exchange ("")
        await _channel.BasicPublishAsync(
            exchange: "",
            routingKey: "demo-queue",
            body: body
        );
    }
}