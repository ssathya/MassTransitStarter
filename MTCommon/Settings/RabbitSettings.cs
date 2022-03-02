namespace MTCommon.Settings
{
    public class RabbitSettings
    {
        public string Host { get; init; }
        public string UserName { get; init; }
        public string Password { get; init; }
        public string GetConnectionString()
        {
            var connectionStr = $"amqps://{UserName}:{Password}@{Host}/{UserName}";
            return connectionStr;
        }
    }
}
