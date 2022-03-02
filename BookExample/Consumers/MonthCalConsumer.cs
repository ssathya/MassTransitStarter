using Adept.Stocks.Contracts.NYSECalendar;
using GreenPipes;
using MassTransit;
using MassTransit.ConsumeConfigurators;
using MassTransit.Definition;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace BookExample.Consumers;

public class MonthCalConsumer : IConsumer<MonthlyWorkingDays[]>
{
    #region Private Fields

    private readonly ILogger<MonthCalConsumer> logger;

    #endregion Private Fields

    #region Public Constructors

    public MonthCalConsumer(ILogger<MonthCalConsumer> logger)
    {
        this.logger = logger;
    }

    #endregion Public Constructors

    #region Public Methods

    public async Task Consume(ConsumeContext<MonthlyWorkingDays[]> context)
    {
        logger.LogInformation("Monthly message received");
        MonthlyWorkingDays[]? mwd = context.Message;
        if (mwd == null || !mwd.Any())
        {
            logger.LogError("Empty message!");
            return;
        }
        foreach (var workingDay in mwd)
        {
            await Console.Out.WriteLineAsync($"{workingDay.CalDate:MM/dd/yyyy} => " +
                $"{workingDay.TPlusDays} => {workingDay.TMinusDays}");
        }
        return;
    }

    #endregion Public Methods
}
public class MonthCalConsumerDefinition : ConsumerDefinition<MonthCalConsumer>
{
    #region Public Constructors

    public MonthCalConsumerDefinition(IConfiguration configuration)
    {
        var serviceName = configuration["ServiceName"] ?? "Unknown";

        EndpointName = serviceName;
        ConcurrentMessageLimit = 2;
    }

    #endregion Public Constructors

    #region Protected Methods

    protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<MonthCalConsumer> consumerConfigurator)
    {
        // configure message retry with millisecond intervals
        endpointConfigurator.UseMessageRetry(r => r.Intervals(100, 200, 500, 800, 1000));

        // use the outbox to prevent duplicate events from being published
        endpointConfigurator.UseInMemoryOutbox();
    }

    #endregion Protected Methods
}
