using DateSubscriber.Subscribe;
using MassTransit;
using MTCommon.Settings;
using Newtonsoft.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var configBuilder = new ConfigurationBuilder();
configBuilder.AddSystemsManager($"/MassTransitShell/Development/", TimeSpan.FromMinutes(10));
var cbCfg = configBuilder.Build();
builder.Services.AddSingleton<IConfiguration>(_ => cbCfg);
//logger
builder.Services.AddLogging(lc =>
{
    lc.AddConsole();
    lc.SetMinimumLevel(LogLevel.Trace);
});


RabbitSettings rabbitSettings = JsonConvert.DeserializeObject<RabbitSettings>(cbCfg["RabbitMqConStr"]);
builder.Services.AddMassTransit(config =>
{
    config.AddConsumer<WorkingDaySub>();
    config.AddConsumer<MonthWorkDaysLst>();

    config.AddConsumersFromNamespaceContaining<WorkingDaySub>();
    config.UsingRabbitMq((ctx, cfg) =>
    {
        var configuration1 = ctx.GetRequiredService<IConfiguration>();
        var serviceName = configuration1["ServiceName"] ?? "Unknown";
        cfg.Host(rabbitSettings.GetConnectionString());

        cfg.ReceiveEndpoint(serviceName, c =>
        {
            c.ConfigureConsumer<WorkingDaySub>(ctx);
            c.ConfigureConsumer<MonthWorkDaysLst>(ctx);
        });

    });
});
builder.Services.AddMassTransitHostedService();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
