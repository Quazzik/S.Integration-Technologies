using Confluent.Kafka;
using Elastic.Clients.Elasticsearch;
using Elastic.Transport;
using S.Integration_Technologies.Services;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

#region elasticSearch

var settings = new ElasticsearchClientSettings(new Uri("http://localhost:9200"))
    .Authentication(new BasicAuthentication("elastic", "elastic_password"))
    .DefaultIndex("articles");
builder.Services.AddSingleton(new ElasticsearchClient(settings));

builder.Services.AddScoped<ArticleSearchService>();

#endregion

#region KafkaProducer

var bootstrapServers = builder.Configuration.GetValue<string>("Kafka:BootstrapServers");

builder.Services.AddSingleton(new ProducerConfig
{
    BootstrapServers = bootstrapServers
});
builder.Services.AddSingleton<KafkaProducerService>();

#endregion

#region KafkaConsumer

builder.Services.AddSingleton<IConsumer<string, string>>(_ =>
{
    var config = new ConsumerConfig
    {
        BootstrapServers = "localhost:9094",
        GroupId = "article-indexer",
        AutoOffsetReset = AutoOffsetReset.Earliest,
        EnableAutoCommit = false
    };

    return new ConsumerBuilder<string, string>(config)
        .Build();
});

builder.Services.AddHostedService<KafkaToElasticHostedService>();

#endregion

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    
    app.MapScalarApiReference(opt =>
    {
        opt.Title = "Scalar Example";
        opt.Theme = ScalarTheme.Mars;
        opt.DefaultHttpClient = new(ScalarTarget.Http, ScalarClient.Http11);
    });
}

app.UseAuthorization();
app.MapControllers();
app.Run();