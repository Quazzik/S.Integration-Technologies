using Elastic.Clients.Elasticsearch;
using S.Integration_Technologies.Models;

public class ArticleSearchService
{
    private readonly ElasticsearchClient _client;
    private readonly string _indiceName="articles";
    public ArticleSearchService(ElasticsearchClient client)
    {
        _client = client;
    }

    public async Task IndexAsync(IEnumerable<ArticleDocument> documents)
    {
        // Bulk-индексация - предпочтительный способ записи
        var response = await _client.BulkAsync(b => b
            .Index(_indiceName)
            .IndexMany(documents)
        );

        if (response.Errors)
        {
            throw new InvalidOperationException("Ошибка при индексации документов");
        }
    }

    public async Task<IReadOnlyCollection<ArticleDocument>> ContentSearchAsync(string query)
    {
        var response = await _client.SearchAsync<ArticleDocument>(s => s
            .Indices(_indiceName)
            .Query(q => q
                .Match(m => m
                    .Field(f => f.Content)
                    .Query(query)
                )
            )
        );

        return response.Documents;
    }
    
    public async Task<IReadOnlyCollection<ArticleDocument>> HeaderSearchAsync(string query)
    {
        var response = await _client.SearchAsync<ArticleDocument>(s => s
            .Indices(_indiceName)
            .Query(q => q
                .Match(m => m
                    .Field(f => f.Header)
                    .Query(query)
                )
            )
        );

        return response.Documents;
    }
    
    public async Task<IReadOnlyCollection<ArticleDocument>> AnySearchAsync(string query)
    {
        // Выполняем поиск
        var response = await _client.SearchAsync<ArticleDocument>(s => s
            .Index(_indiceName)
            .Query(q => q
                .MultiMatch(m => m
                        .Query(query)
                        .Fields("content, header") 
                )
            )
        );

        return response.Documents;
    }
}