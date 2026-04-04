
using Microsoft.AspNetCore.Mvc;
using S.Integration_Technologies.Models;
using S.Integration_Technologies.Services;

namespace S.Integration_Technologies.Controllers;

[ApiController]
[Route("api/kafka")]
public class KafkaTestController : ControllerBase
{
    private readonly KafkaProducerService _kafkaProducer;

    public KafkaTestController(KafkaProducerService kafkaProducer)
    {
        _kafkaProducer = kafkaProducer;
    }
    
    [HttpPost("Kafka send message")]
    public async Task<IActionResult> CreateArticle([FromBody] ArticleDocument article)
    {
        await _kafkaProducer.PublishAsync(article);

        return Accepted();
    }

    [HttpPost("FastFill")]
    public async Task<IActionResult> FastFill()
    {
        var documents = new[]
        {
            new ArticleDocument { Id = 1, Content = "Моя первая статья по ASP.NET Core и Elasticsearch", Header = "Заголовок со словом .NET"},
            new ArticleDocument { Id = 2, Content = "Полнотекстовый поиск в .NET", Header = "Заголовок со словом Elasticsearch"},
            new ArticleDocument { Id = 3, Content = "Работа с Elasticsearch 9 - быстрый старт", Header = "Заголовок без ключевых слов"},
            new ArticleDocument { Id = 4,
                Content = "Очень помог сервис который вы показали на паре, спасибо",
                Header = "История о множестве CTRL+C CTRL+V"},
            new ArticleDocument{Id = 5,
                Content = "Нужно сделать такую запись, чтобы в ней в обоих случаях встречалось одно слово",
                Header = "Я не смог придумать такую запись"}
        };

        foreach (var document in documents)
        {
            await _kafkaProducer.PublishAsync(document);
        }

        return Accepted();
    }
    
}