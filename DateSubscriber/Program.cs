// See https://aka.ms/new-console-template for more information
using DateSubscriber.Subscribe;
using GreenPipes;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MTCommon.Settings;
using Newtonsoft.Json;

Console.WriteLine("Starting application");
Console.WriteLine("Setting up Service collection");
IServiceCollection services = new ServiceCollection();
var builder = new ConfigurationBuilder();
builder.AddSystemsManager($"/MassTransitShell/Development/", TimeSpan.FromMinutes(10));
var configuration = builder.Build();
services.AddSingleton<IConfiguration>(_ => configuration);

//logger
services.AddLogging(lc =>
{
    lc.AddConsole();
    lc.SetMinimumLevel(LogLevel.Trace);
});

//MassTransit-rabbitMQ

Console.Title = "Consumer";

//IBusControl bus = Bus.Factory.CreateUsingRabbitMq(cfg =>
//{

//    RabbitSettings rabbitSettings = JsonConvert.DeserializeObject<RabbitSettings>(configuration["RabbitMqConStr"]);
//    cfg.Host(rabbitSettings.GetConnectionString());
//    cfg.ReceiveEndpoint(configuration["ServiceName"] ?? "Unknown", ep =>
//     {
//         ep.Lazy = true;
//         ep.PrefetchCount = 16;
//         ep.UseMessageRetry(r => r.Interval(2, 100));         
//         ep.Consumer<WorkingDaySub>();
//         ep.Consumer<MonthWorkDaysLst>();
//     });
//});


services.AddMassTransit(configurator =>
{
    configurator.AddConsumers(typeof(WorkingDaySub).Assembly);
    //configurator.AddConsumer<WorkingDaySub>();
    //configurator.AddConsumer<MonthWorkDaysLst>();

    configurator.UsingRabbitMq((ctx, config) =>
    {
        var configuration1 = ctx.GetRequiredService<IConfiguration>();
        var serviceName = configuration["ServiceName"] ?? "Unknown";
        RabbitSettings rabbitSettings = JsonConvert.DeserializeObject<RabbitSettings>(configuration["RabbitMqConStr"]);

        config.Host(rabbitSettings.GetConnectionString());
        config.ConfigureEndpoints(ctx);
        //config.ConfigureEndpoints(ctx, new KebabCaseEndpointNameFormatter(serviceName, false));
        config.UseMessageRetry(retryConfigurator =>
        {
            retryConfigurator.Interval(3, TimeSpan.FromSeconds(5));
        });
        config.ReceiveEndpoint(serviceName, e =>
        {
            e.ConfigureConsumer<WorkingDaySub>(ctx);
            e.ConfigureConsumer<MonthWorkDaysLst>(ctx);
        });
    });
});
services.AddMassTransitHostedService();

ServiceProvider sp = services.BuildServiceProvider();
var bus = sp.GetRequiredService<IBusControl>();
var logger = sp.GetRequiredService<ILogger<Program>>();

using CancellationTokenSource cancellationToken = new(TimeSpan.FromSeconds(30));
var busHandle = await bus.StartAsync(cancellationToken.Token);
try
{
    Console.WriteLine("Press enter to exit");
    await Task.Run(() => Console.ReadLine());
}
catch (Exception)
{

    throw;
}
finally
{
    await bus.StopAsync();
}


















//IBusControl bus = Bus.Factory.CreateUsingRabbitMq(cfg =>
//{
//    cfg.Host(rabbitSettings.GetConnectionString());

//    cfg.ReceiveEndpoint(configuration["ServiceName"] ?? "Unknown", ep =>
//     {
//         ep.Lazy = true;
//         ep.PrefetchCount = 16;
//         ep.UseMessageRetry(r => r.Interval(2, 100));
//         //ep.AddConsumers(typeof(WorkingDaySub).Assembly);
//         //ep.Consumer<WorkingDaySub>();
//         //ep.Consumer<MonthWorkDaysLst>();
//     });
//});














//serviceCollection.AddMassTransit(config =>
//{
//    config.AddConsumer<WorkingDaySub>();
//    config.UsingRabbitMq((ctx, cfg) =>
//    {
//        var configuration1 = ctx.GetRequiredService<IConfiguration>();
//        var serviceName = configuration1["ServiceName"] ?? "Unknown";
//        cfg.Host(rabbitSettings.GetConnectionString());
//        cfg.ReceiveEndpoint(serviceName, c =>
//        {
//            c.ConfigureConsumer<WorkingDaySub>(ctx);
//        });
//    });
//});
//serviceCollection.AddMassTransitHostedService();


//ServiceProvider sp = serviceCollection.BuildServiceProvider();
//var logger = sp.GetRequiredService<ILogger<Program>>();
//logger.LogInformation("Information");
//logger.LogDebug("Debug");
//Console.WriteLine("Press any key to stop the application");
//Console.ReadKey();