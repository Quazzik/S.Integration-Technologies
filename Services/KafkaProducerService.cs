using System.Text.Json;
using Confluent.Kafka;
using S.Integration_Technologies.Models;

namespace S.Integration_Technologies.Services;

public class KafkaProducerService
{
    private readonly IProducer<string, string> _producer;

    public KafkaProducerService(ProducerConfig config)
    {
        _producer = new ProducerBuilder<string, string>(config).Build();
    }

    public async Task PublishAsync(ArticleDocument article)
    {
        var json = JsonSerializer.Serialize(article);

        await _producer.ProduceAsync("articles", new Message<string, string>
        {
            Key = article.Id.ToString(),
            Value = json
        });
    }
}