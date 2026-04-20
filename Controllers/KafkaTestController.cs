
using Microsoft.AspNetCore.Mvc;
using S.Integration_Technologies.Database;
using S.Integration_Technologies.Models;

namespace S.Integration_Technologies.Controllers;

[ApiController]
[Route("api/kafka")]
public class KafkaTestController(ArticleDbContext dbContext) : ControllerBase
{
    [HttpPost("Kafka send message")]
    public async Task<IActionResult> CreateArticle([FromBody] ArticleDocument article)
    {
        await dbContext.AddAsync(new ArticleDocument { Content = article.Content, Header = article.Header });
        await dbContext.SaveChangesAsync();

        return Accepted();
    }

    [HttpPost("FastFill")]
    public async Task<IActionResult> FastFill()
    {
        var documents = new[]
        {
            new ArticleDocument { Content = "Моя первая статья по ASP.NET Core и Elasticsearch", Header = "Заголовок со словом .NET"},
            new ArticleDocument { Content = "Полнотекстовый поиск в .NET", Header = "Заголовок со словом Elasticsearch"},
            new ArticleDocument { Content = "Работа с Elasticsearch 9 - быстрый старт", Header = "Заголовок без ключевых слов"},
            new ArticleDocument { Content = "Очень помог сервис который вы показали на паре, спасибо",
                Header = "История о множестве CTRL+C CTRL+V"},
            new ArticleDocument { Content = "Нужно сделать такую запись, чтобы в ней в обоих случаях встречалось одно слово",
                Header = "Я не смог придумать такую запись"},
            new ArticleDocument { Content = "В данной работе можно даже указывать ID, он отбрасывается при обработке входящего объекта",
                Header ="Лайфхаки для удобства"},
            new ArticleDocument { Content = "Ещё нужно поменять топик в котором мы ищем в ArticleSearchService.cs, но elastic не хочет работать с тем переименованным топиком, я без понятия как это фиксить, уже 3 часа с этим долблюсь",
            Header ="Эластичная подстава века"}
        };

        foreach (var document in documents)
        {
            await dbContext.ArticleDocuments.AddAsync(document);
        }
        await dbContext.SaveChangesAsync();

        return Accepted();
    }
    
}