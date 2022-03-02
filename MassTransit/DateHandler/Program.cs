// See https://aka.ms/new-console-template for more information
using DateHandler.Publish;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MTCommon.Settings;
using Newtonsoft.Json;

Console.WriteLine("Starting application");
Console.WriteLine("Setting up Service collection");
IServiceCollection serviceCollection = new ServiceCollection();
var builder = new ConfigurationBuilder();
builder.AddSystemsManager($"/MassTransitShell/Development/", TimeSpan.FromMinutes(10));
var configuration = builder.Build();
serviceCollection.AddSingleton<IConfiguration>(_ => configuration);

//logger
serviceCollection.AddLogging(lc =>
{
    lc.AddConsole();
    lc.SetMinimumLevel(LogLevel.Trace);
});

//MassTransit-rabbitMQ
RabbitSettings rabbitSettings = JsonConvert.DeserializeObject<RabbitSettings>(configuration["RabbitMqConStr"]);
serviceCollection.AddMassTransit(config =>
{
    config.UsingRabbitMq((ctx, cfg) =>
    {
        cfg.Host(rabbitSettings.GetConnectionString());
    });
});
serviceCollection.AddMassTransitHostedService();


//App Specific
serviceCollection.AddScoped<PublishDate>();


ServiceProvider sp = serviceCollection.BuildServiceProvider();
var logger = sp.GetRequiredService<ILogger<Program>>();
logger.LogInformation("Information");
logger.LogDebug("Debug");

PublishDate publishDate = sp.GetRequiredService<PublishDate>();
Console.WriteLine("Press any key to publish messages");
var key = Console.ReadKey().KeyChar;
List<char> exitChar = new();

exitChar.Add('X');
exitChar.Add('x');
do
{
    logger.LogInformation("Publishing messages");
    await publishDate.PublishWorkingDayMessage();
    await publishDate.PublishMonthlyWorkingDaysMessage();      
}
while (!exitChar.Contains(Console.ReadKey().KeyChar));
Console.WriteLine("Everything done");




