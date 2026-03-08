using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using S.Integration_Technologies.Models;
using S.Integration_Technologies.Services;

namespace S.Integration_Technologies.Controllers;

[ApiController]
[Route("api/broker")]

public class BrokerController(RabbitMqProducerService rabbitMqProducer) : Controller
{
    [HttpPost("Send")]
    public async Task<IActionResult> Send([FromBody] ArticleDocument document)
    {
        try
        {
            await rabbitMqProducer.SendAsync(document);
        }
        catch(Exception ex) {return BadRequest(ex.Message);}

        return Ok("Message successfully sent");
    } 
}