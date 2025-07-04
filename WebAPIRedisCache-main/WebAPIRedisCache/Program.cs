using ProjetoCep.Api.Clients;
using ProjetoCep.Api.Repositories;
using ProjetoCep.Api.Services;
using MongoDB.Driver;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, services, configuration) => configuration
    .ReadFrom.Configuration(context.Configuration)
    .ReadFrom.Services(services)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var mongoDbConnectionString = builder.Configuration["MongoDbConnection:ConnectionString"];

if (string.IsNullOrEmpty(mongoDbConnectionString))
{
    throw new InvalidOperationException("MongoDB connection string 'MongoDbConnection:ConnectionString' not found in configuration.");
}

builder.Services.AddSingleton<IMongoClient>(s =>
    new MongoClient(mongoDbConnectionString));

builder.Services.AddStackExchangeRedisCache(options =>
{
    var redisConnectionString = builder.Configuration["Redis:ConnectionString"];

    if (string.IsNullOrEmpty(redisConnectionString))
    {
        throw new InvalidOperationException("Redis connection string 'Redis:ConnectionString' not found in configuration.");
    }

    options.Configuration = redisConnectionString;
    options.InstanceName = "ProjetoCep_";
});


builder.Services.AddHttpClient<IBrasilApiClient, BrasilApiClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration.GetSection("BrasilApi:BaseUrl").Value!);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});

builder.Services.AddScoped<ICepRepository, CepRepository>();
builder.Services.AddScoped<ICepService, CepService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();