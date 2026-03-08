using Elastic.Clients.Elasticsearch;
using Elastic.Transport;
using RabbitMQ.Client;
using S.Integration_Technologies.Services;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddSingleton(new ConnectionFactory
{
    HostName = "localhost",
    Port = 5672,
    UserName = "rabbituser",
    Password = "rabbitpassword"
});

builder.Services.AddSingleton<RabbitMqProducerService>();
builder.Services.AddHostedService<RabbitMqConsumerService>();

var settings = new ElasticsearchClientSettings(new Uri("http://localhost:9200"))
    .Authentication(new BasicAuthentication("elastic", "elastic_password"))
    .DefaultIndex("articles");

// Клиент регистрируется как Singleton
builder.Services.AddSingleton(new ElasticsearchClient(settings));

builder.Services.AddSingleton<ArticleSearchService>();

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