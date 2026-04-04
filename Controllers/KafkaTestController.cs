
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
    
}