using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using S.Integration_Technologies.Models;
using S.Integration_Technologies.Services;

namespace S.Integration_Technologies.Controllers;

[ApiController]
[Route("api/broker")]

public class BrokerController(RabbitMqProducerService? rabbitMqProducer, ArticleSearchService searchService) : Controller
{
    [HttpPost("Send")]
    public async Task<IActionResult> Send([FromBody] ArticleDocument document)
    {
        if (!await searchService.IsExistAsync(document.Id)){
            try
            {
                await rabbitMqProducer.SendAsync(document);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok("Message successfully sent");
        }
        return BadRequest("Message was not sent, Id already exists");
    }

    [HttpGet("Fast Filling")]
    public async Task<IActionResult> FastFilling()
    {
        var documents = new[]
        {
            new ArticleDocument
                { Id = 1, Content = "Моя типа статья, отправленная через брокер сообщений", Header = "Приветствие" },
            new ArticleDocument
            {
                Id = 2,
                Content = "Как я узнал, если отправить в ElasticSearch запись с одним ID, то  он  перезапишет старый вариант",
                Header = "Интересный факт"
            }
        };

        foreach (var document in documents)
        {
            await rabbitMqProducer.SendAsync(document);
        }
        
        return Ok("ElasticSearch must be filled");
    }
}