using Adept.Stocks.Contracts.NYSECalendar;
using GreenPipes;
using MassTransit;
using MassTransit.ConsumeConfigurators;
using MassTransit.Definition;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace BookExample.Consumers;

public class WorkingDayConsumer : IConsumer<WorkingDay>
{
    #region Private Fields

    private readonly ILogger<WorkingDayConsumer> logger;

    #endregion Private Fields

    #region Public Constructors

    public WorkingDayConsumer(ILogger<WorkingDayConsumer> logger)
    {
        this.logger = logger;
    }

    #endregion Public Constructors

    #region Public Methods

    public async Task Consume(ConsumeContext<WorkingDay> context)
    {
        logger.LogInformation("Message received");
        var wd = context.Message;
        if (wd == null)
        {
            return;
        }
        await Console.Out.WriteLineAsync($"{wd.Today:MM/dd/yyyy}");
    }

    #endregion Public Methods
}

public class WorkingDayConsumerDefinition : ConsumerDefinition<WorkingDayConsumer>
{
    #region Public Constructors

    public WorkingDayConsumerDefinition(IConfiguration configuration)
    {
        var serviceName = configuration["ServiceName"] ?? "Unknown";

        EndpointName = serviceName;
        ConcurrentMessageLimit = 2;
    }

    #endregion Public Constructors

    #region Protected Methods

    protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator,
        IConsumerConfigurator<WorkingDayConsumer> consumerConfigurator)
    {
        // configure message retry with millisecond intervals
        endpointConfigurator.UseMessageRetry(r => r.Intervals(100, 200, 500, 800, 1000));

        // use the outbox to prevent duplicate events from being published
        endpointConfigurator.UseInMemoryOutbox();
    }

    #endregion Protected Methods
}