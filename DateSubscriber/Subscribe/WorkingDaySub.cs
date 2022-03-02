using Adept.Stocks.Contracts.NYSECalendar;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace DateSubscriber.Subscribe
{
    public class WorkingDaySub : IConsumer<WorkingDay>
    {
        private readonly ILogger<WorkingDaySub> logger;

        public WorkingDaySub(ILogger<WorkingDaySub> logger)
        {
            this.logger = logger;
        }
        public async Task Consume(ConsumeContext<WorkingDay> context)
        {
            var wd = context.Message;
            if (wd == null)
            {
                logger.LogError("Got an empty message");
                return;
            }
            await Console.Out.WriteLineAsync($"{wd.Today:MM/dd/yyyy}");
            logger.LogTrace("Successfully processed messages");
        }
    }
}
