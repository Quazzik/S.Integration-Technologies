using Microsoft.AspNetCore.Mvc;
using S.Integration_Technologies.Models;

[ApiController]
[Route("api/search")]
public class SearchController : ControllerBase
{
    private readonly ArticleSearchService _service;

    public SearchController(ArticleSearchService service)
    {
        _service = service;
    }

    [HttpGet("index")]
    public async Task<IActionResult> Index()
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

        await _service.IndexAsync(documents);
        return Ok();
    }

    [HttpGet("Content Search")]
    public async Task<IActionResult> ContentSearch([FromQuery] string q)
    {
        var result = await _service.ContentSearchAsync(q);
        return Ok(result);
    }

    [HttpGet("Header Search")]
    public async Task<IActionResult> HeaderSearch([FromQuery] string q)
    {
        var result = await _service.HeaderSearchAsync(q);
        return Ok(result);
    }
    
    [HttpGet("Any Search")]
    public async Task<IActionResult> AnySearch([FromQuery] string q)
    {
        var result = await _service.AnySearchAsync(q);
        return Ok(result);
    }
}