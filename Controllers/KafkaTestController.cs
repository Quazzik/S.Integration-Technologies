
using Microsoft.AspNetCore.Mvc;
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

    [HttpGet("Test Send")]
    public async Task<IActionResult> Send()
    {
        await _kafkaProducer.ProduceAsync(
            topic: "demo-topic",
            message: "Hello from ASP.NET Core"
        );

        return Ok("Message sent to Kafka");
    }
}