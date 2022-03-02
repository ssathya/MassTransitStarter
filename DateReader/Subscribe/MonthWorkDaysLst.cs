using Adept.Stocks.Contracts.NYSECalendar;
using MassTransit;

namespace DateSubscriber.Subscribe
{
    public class MonthWorkDaysLst : IConsumer<MonthlyWorkingDays[]>
    {
        public async Task Consume(ConsumeContext<MonthlyWorkingDays[]> context)
        {
            var mwdArray = context.Message;
            foreach (var mwd in mwdArray)
            {
                await Console.Out.WriteLineAsync($"{mwd.CalDate:MM/dd/yyyy} =>" +
                    $"T+ {mwd.TPlusDays} =>T- {mwd.TMinusDays}");
            }
        }
    }
}
