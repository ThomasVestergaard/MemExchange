namespace MemExchange.Client.UI.Setup
{
    public class Configuration : IConfiguration
    {
        public int ClientId { get; set; }
        public string ServerAddress { get; set; }
        public int ServerCommandPort { get; set; }
        public int ServerPublishPort { get; set; }
    }
}
