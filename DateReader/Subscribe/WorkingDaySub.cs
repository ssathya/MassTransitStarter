using Adept.Stocks.Contracts.NYSECalendar;
using MassTransit;

namespace DateSubscriber.Subscribe
{
    public class WorkingDaySub : IConsumer<WorkingDay>
    {
        public async Task Consume(ConsumeContext<WorkingDay> context)
        {
            var wd = context.Message;
            if (wd == null)
            {
                return;
            }
            await Console.Out.WriteLineAsync($"{wd.Today:MM/dd/yyyy}");
        }
    }
}
