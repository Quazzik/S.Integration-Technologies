using Microsoft.AspNetCore.Mvc;
using S.Integration_Technologies.Services;

namespace S.Integration_Technologies.Controllers;

[ApiController]
[Route("api/broker")]

public class BrokerController(RabbitMqProducerService rabbitMqProducer) : Controller
{
    [HttpGet("send")]
    public async Task<IActionResult> Send()
    {
        await rabbitMqProducer.SendAsync("Message from producer");

        return Ok("Message successfully sent");
    } 
    
//    public IActionResult Index()
//    {
//        return View();
//    }
}