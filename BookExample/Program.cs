// See https://aka.ms/new-console-template for more information
using BookExample.Consumers;
using GreenPipes;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MTCommon.Settings;
using Newtonsoft.Json;

var services = new ServiceCollection();

var builder = new ConfigurationBuilder();
builder.AddSystemsManager($"/MassTransitShell/Development/", TimeSpan.FromMinutes(10));
var configuration = builder.Build();
services.AddSingleton<IConfiguration>(_ => configuration);

services.AddLogging(lc =>
{
    lc.AddConsole();
    lc.SetMinimumLevel(LogLevel.Trace);
});

services.AddMassTransit(x =>
{
    x.AddConsumer<WorkingDayConsumer>(typeof(WorkingDayConsumerDefinition));
    x.AddConsumer<MonthCalConsumer>(typeof(MonthCalConsumerDefinition));    
    x.SetKebabCaseEndpointNameFormatter();

    x.UsingRabbitMq((context, cfg) =>
    {
        var config = context.GetRequiredService<IConfiguration>();
        RabbitSettings rabbit = JsonConvert.DeserializeObject<RabbitSettings>(configuration["RabbitMqConStr"]);        
        cfg.Host(rabbit.GetConnectionString());
        cfg.ConfigureEndpoints(context);
    });
});
ServiceProvider provider = services.BuildServiceProvider();
IBusControl? busControl = provider.GetRequiredService<IBusControl>();
if (busControl == null)
{
    System.Environment.Exit(-1);
}
await busControl.StartAsync(new CancellationTokenSource(TimeSpan.FromSeconds(10)).Token);
try
{
    await Task.Run(() => Console.ReadLine());
}
catch (Exception)
{
    throw;
}
finally
{
    await busControl.StopAsync();
}